using GammaCrsQAInstaller.RemoteSetup;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GammaCrsQAInstaller.Pages
{
    /// <summary>
    /// Interaction logic for ResultPage.xaml
    /// </summary>
    public partial class ResultPage : Page
    {
        private static object locker = new object();
        private SynchronizationContext ui_sc = null;
        private int progress_step = 1;
        public ResultPage()
        {
            InitializeComponent();
            ui_sc = SynchronizationContext.Current;
            var steps = SetupInfo.GetValue<ObservableCollection<Node>>(SetupInfoKeys.NodeList).Count * 5;
            this.progress_step = Convert.ToInt32((this.InstallProgressBar.Maximum / steps));

            var t = Task.Run(() =>
            {
                var installer = InstallerFactory.GetGammaServiceSetup();
                installer.OnOpCompletedEventHandler += Installer_OnOpCompletedEventHandler;
                return installer.AllInOne();
            });
            t.GetAwaiter().OnCompleted(()=> {
                this.InstallProgressBar.Value = 100;
                ResultRTB.AppendText("===========FINISHED===============");
            });
            
        }

        private void Installer_OnOpCompletedEventHandler(object sender, GeneralUtility.OpResultArgs e)
        {

            lock (locker)
            {
                ui_sc.Post((obj) => {
                    ResultRTB.AppendText(string.Format("--------------------\u2028{0}\u2028{1}\u2028{2}\u2028", e.Hostname, e.OpType, e.OpResult));
                    this.InstallProgressBar.Value += this.progress_step;
                }, null);
                
            }
            
        }

        private void ResultRTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.ResultRTB.ScrollToEnd();
        }

    }
}
