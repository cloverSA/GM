using GammaClient.GCFacilities.TXManager;
using GammaClient.GCFacilities.UIServiceProvider;
using GammaClient.GCFacilities.WCFProxy;
using GammaClient.GCHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GammaClient.GCViewModels
{
    class CommandExecutorViewModel : SPResultInTextBox
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

        #region Function

        void ExecuteShellOnNodes()
        {
            OpResult = string.Empty;
            CanExec = false;

            var tasks = new List<Task<string>>();

            string[] cmds = inputcmd.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            if (isSeqMode)
            {
                foreach (var machine in from m in NodeMgr.Machines where m.IsSelected && m.Alive == NodeState.Online select m)
                {
                    tasks.Add(TxMgr.StartCmdInSeqTransaction(machine, cmds));
                }
            }
            else
            {
                foreach (var machine in from m in NodeMgr.Machines where m.IsSelected && m.Alive == NodeState.Online select m)
                {
                    tasks.AddRange(TxMgr.StartCmdInParallelTransaction(machine, cmds));
                }
            }

            var all_job = Task.WhenAll(tasks);

            all_job.GetAwaiter().OnCompleted(() => {
                CanExec = true;
            });

        }
        #endregion


        #region TransactionManager Consumer

        protected override void RaiseResultComeback(object sender, GammaUIUpdateArgs e)
        {
            if (e.TRANSACTION.TX_TYPE == GammaTransactionType.COMMAND)
            {
                OpResult += string.Format("\r\n{0}: {1}\r\n", e.TRANSACTION.Machine, e.TRANSACTION.TX_RESULT);
            }
        }

        #endregion

        #region Command

        public ICommand ExecuteCmds { get { return new RelayCommand(ExecuteShellOnNodes); } }

        #endregion
    }
}
