using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Linq;
using System.Windows.Input;
using System;
using GammaCrsQAInstaller.NetworkManager;
using GammaCrsQAInstaller.TXManager;

namespace GammaCrsQAInstaller
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private NodeNetManager nodeNetManager;
        private SynchronizationContext ui_sc = null;
        private static object locker = new object();
        private void InitTasks()
        {

            //this.ResultRTB.Document.Blocks.Clear();
            this.ui_sc = SynchronizationContext.Current;
            nodeNetManager = NodeNetManagerFactory.GetSimpleNetworkManager();
            this.MachinesDG.ItemsSource = nodeNetManager.Machines;
            nodeNetManager.StartNodeCheck();
            //Initialize transaction Manager.
            GammaClientTXManagerFactory.GetGammaClientTXManager().OnResultComesBack += GammaCmdExecution_OnResultComesBack;
        }

        public MainWindow()
        {
            InitializeComponent();
            InitTasks();
        }

        private void GoToPageExecuteHandler(object sender, ExecutedRoutedEventArgs e)
        {
            ToolsFrame.NavigationService.Navigate(new Uri((string)e.Parameter, UriKind.Relative));
        }

        private void GoToPageCanExecuteHandler(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CmdExeBtn_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            btn.IsEnabled = false;
            this.CmdResultRTB.Document.Blocks.Clear();
            string input_text = GetTxtFromRichTextBox(this.CmdInputRTB);
            Task client_job;
            if(input_text != null)
            {
                client_job = ExecuteShellOnNodes(input_text);
                client_job.GetAwaiter().OnCompleted(() => {
                    btn.IsEnabled = true;
                });
            }
                    
        }

        private string GetTxtFromRichTextBox(RichTextBox rtb)
        {
            var rs = string.Empty;
            if (rtb != null)
            {
                TextRange textRange = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);
                rs = textRange.Text;
            }
            
            return rs;
        }


        /*
            Excute commands submmited from "CmdInputRTB"
            Note:
            For InSeqRBtn is checked, it will only run the command in sequence on the node, and each node will return *A summary result* when all steps finished.
            For InParallelRBtn is checked, the commands will be sent to the node, and run in parallel, but the result for each command will returned once they are inished.

            Return Task.WhenAll to prevent submitting commands before the previous return.
        */
        private Task ExecuteShellOnNodes(string cmd_to_exe)
        {

            var tasks = new List<Task<string>>();
            var tx_mgr = GammaClientTXManagerFactory.GetGammaClientTXManager();
            string[] cmds = cmd_to_exe.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            if ((bool)InSeqRBtn.IsChecked)
            {
                foreach (var machine in from m in nodeNetManager.Machines where m.IsSelected && m.Alive == NodeState.Online select m)
                {
                    tasks.Add(tx_mgr.StartCmdInSeqTransaction(machine, cmds));
                }
            }
            else
            {
                foreach (var machine in from m in nodeNetManager.Machines where m.IsSelected && m.Alive == NodeState.Online select m)
                {
                    tasks.AddRange(tx_mgr.StartCmdInParallelTransaction(machine, cmds));
                }
            }
            
            return Task.WhenAll(tasks);
        }
        /*
            UI will be updated based on the transaction type, send result to different displayer on the tab control.
            Note:
            All the commands sent out are being run in a separated thread, and the UI is updated when the thread finished its job,
            the sync context is not UI thread when entering this func.
        */
        private void GammaCmdExecution_OnResultComesBack(object sender, GammaUIUpdateArgs e)
        {

            if (e.TRANSACTION.TX_TYPE == GammaTransactionType.COMMAND)
            {
                ui_sc.Post(
                (obj) =>
                {
                    this.CmdResultRTB.AppendText(string.Format("\u2028{0}: {1}\u2028", e.TRANSACTION.Machine, e.TRANSACTION.TX_RESULT));

                }, null);
            }
            else 
            {
                if (e.TRANSACTION.TX_TYPE == GammaTransactionType.CLEARENV)
                {
                    ui_sc.Post(
                    (obj) =>
                    {
                        this.ClearEnvRTB.AppendText(string.Format("\u2028{0}: {1}\u2028", e.TRANSACTION.Machine, e.TRANSACTION.TX_RESULT));
                    }, null);
                }
                else
                {
                    if (e.TRANSACTION.TX_TYPE != GammaTransactionType.QATOOLS)
                    {
                        MessageBox.Show(string.Format("[UNKNOW OR DEBUG INFO]{0},{1}", e.TRANSACTION.Machine, e.TRANSACTION.TX_RESULT));
                    }

                }
            }
            
        }

        private void ClearEnvBtnAble(bool enabled)
        {
            foreach(var child in ClearEnvStack.Children.OfType<Button>())
            {
                child.IsEnabled = enabled;
            }
            //ClearRegBtn.IsEnabled = enabled;
           // ClearGroupBtn.IsEnabled = enabled;
            //RebootBtn.IsEnabled = enabled;
            //ClearDiskBtn.IsEnabled = enabled;
           // RmDrvLtrBtn.IsEnabled = enabled;
            //RmFilesBtn.IsEnabled = enabled;
        }

        private void CrsClearBtn_Click(object sender, RoutedEventArgs e)
        {
            ClearEnv(GammaTXClearEnv.REG);
        }

        private void ClearGroupBtn_Click(object sender, RoutedEventArgs e)
        {
            ClearEnv(GammaTXClearEnv.GROUP);
        }

        private void RebootBtn_Click(object sender, RoutedEventArgs e)
        {
            ClearEnv(GammaTXClearEnv.REBOOT);
        }

        private void ClearDiskBtn_Click(object sender, RoutedEventArgs e)
        {
            ClearEnv(GammaTXClearEnv.DISK);
        }

        private void RmDrvLtrBtn_Click(object sender, RoutedEventArgs e)
        {
            ClearEnv(GammaTXClearEnv.DRIVER);
        }

        private void RmFilesBtn_Click(object sender, RoutedEventArgs e)
        {
            ClearEnv(GammaTXClearEnv.FILE);
        }

        private void ClearEnv(GammaTXClearEnv tx_code)
        {
            ClearEnvBtnAble(false);
            Task client_job = ClearEnvOnNodes(tx_code);
            client_job.GetAwaiter().OnCompleted(() =>
            {
                ClearEnvBtnAble(true);
            });
        }

        private Task ClearEnvOnNodes(GammaTXClearEnv tx_code)
        {
            List<Task<string>> tasks = new List<Task<string>>();
            var tx_mgr = GammaClientTXManagerFactory.GetGammaClientTXManager();
            foreach (var machine in from m in nodeNetManager.Machines where m.IsSelected && m.Alive == NodeState.Online select m)
            {
                tasks.Add(tx_mgr.StartClearEnvTransaction(machine, tx_code));
            }
            return Task.WhenAll(tasks);
        }

    }
}
