using GammaCrsQAInstaller.NetworkManager;
using GammaCrsQAInstaller.TXManager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GammaCrsQA.ViewModel
{
    class CmdExecuteViewModel : QAToolsBaseView
    {
        #region Members
        private string inputcmd;
        private bool isSeqMode = true;
        private bool isParallelMode = false;
        #endregion 

        #region Properties
        public string InputCmd
        {
            get
            {
                return inputcmd;
            }

            set
            {
                inputcmd = value;
            }
        }

        public bool IsSeqMode
        {
            get
            {
                return isSeqMode;
            }

            set
            {
                isSeqMode = value;
            }
        }

        public bool IsParallelMode
        {
            get
            {
                return isParallelMode;
            }

            set
            {
                isParallelMode = value;
            }
        }
        #endregion  

        #region Constructor

        public CmdExecuteViewModel()
        {
            GammaClientTXManagerFactory.GetGammaClientTXManager().OnResultComesBack += RaiseShellExecuteResultComeback;
        }

        #endregion

        #region Function

        void ExecuteShellOnNodes()
        {
            GroupAbled = false;

            var tasks = new List<Task<string>>();
            var tx_mgr = GammaClientTXManagerFactory.GetGammaClientTXManager();
            var node_mgr = NodeNetManagerFactory.GetSimpleNetworkManager();
            string[] cmds = inputcmd.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            if (isSeqMode)
            {
                foreach (var machine in from m in node_mgr.Machines where m.IsSelected && m.Alive == NodeState.Online select m)
                {
                    tasks.Add(tx_mgr.StartCmdInSeqTransaction(machine, cmds));
                }
            }
            else
            {
                foreach (var machine in from m in node_mgr.Machines where m.IsSelected && m.Alive == NodeState.Online select m)
                {
                    tasks.AddRange(tx_mgr.StartCmdInParallelTransaction(machine, cmds));
                }
            }

            var all_job = Task.WhenAll(tasks);

            all_job.GetAwaiter().OnCompleted(() => {
                GroupAbled = true;
            });

        }
        #endregion


        #region TransactionManager Consumer

        private void RaiseShellExecuteResultComeback(object sender, GammaUIUpdateArgs e)
        {
            if (e.TRANSACTION.TX_TYPE == GammaTransactionType.COMMAND)
            {
                QAOpResult += string.Format("\r\n{0}: {1}\r\n", e.TRANSACTION.Machine, e.TRANSACTION.TX_RESULT);
            }
        }

        #endregion

        #region Command

        public ICommand ExecuteCmds { get { return new RelayCommand(ExecuteShellOnNodes); } }

        #endregion
    }
}
