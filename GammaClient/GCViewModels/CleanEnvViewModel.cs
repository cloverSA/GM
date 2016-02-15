using GalaSoft.MvvmLight.CommandWpf;
using GammaClient.GCFacilities.TXManager;
using GammaClient.GCFacilities.UIServiceProvider;
using GammaClient.GCFacilities.WCFProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GammaClient.GCViewModels
{
    class CleanEnvViewModel : SPResultInTextBox
    {
        protected override void RaiseResultComeback(object sender, GammaUIUpdateArgs e)
        {
            if (e.TRANSACTION.TX_TYPE == GammaTransactionType.CLEARENV)
            {
                OpResult += string.Format("\r\n{0}: {1}\r\n", e.TRANSACTION.Machine, e.TRANSACTION.TX_RESULT);
            }
        }

        private void ClearEnv(GammaTXClearEnv tx_code)
        {
            OpResult = string.Empty;
            CanExec = false;
            Task client_job = ClearEnvOnNodes(tx_code);

            client_job.GetAwaiter().OnCompleted(() =>
            {

                CanExec = true;
            });


        }

        private Task ClearEnvOnNodes(GammaTXClearEnv tx_code)
        {
            List<Task<string>> tasks = new List<Task<string>>();
            foreach (var machine in from m in NodeMgr.Machines where m.IsSelected && m.Alive == NodeState.Online select m)
            {
                tasks.Add(TxMgr.StartClearEnvTransaction(machine, tx_code));
            }
            return Task.WhenAll(tasks);
        }

        #region Commands

        void ClearEnvReg()
        {
            ClearEnv(GammaTXClearEnv.REG);
        }

        void ClearEnvGroup()
        {

            ClearEnv(GammaTXClearEnv.GROUP);
        }

        void ClearEnvRegReboot()
        {

            ClearEnv(GammaTXClearEnv.REBOOT);
        }

        void ClearEnvDisk()
        {

            ClearEnv(GammaTXClearEnv.DISK);
        }

        void ClearEnvDrive()
        {

            ClearEnv(GammaTXClearEnv.DRIVER);
        }

        void ClearEnvFile()
        {

            ClearEnv(GammaTXClearEnv.FILE);
        }


        public ICommand ClearRegistry { get { return new RelayCommand(ClearEnvReg); } }
        public ICommand ClearGroup { get { return new RelayCommand(ClearEnvGroup); } }
        public ICommand RebootNode { get { return new RelayCommand(ClearEnvRegReboot); } }
        public ICommand ClearDisk { get { return new RelayCommand(ClearEnvDisk); } }
        public ICommand ClearDrive { get { return new RelayCommand(ClearEnvDrive); } }
        public ICommand ClearFile { get { return new RelayCommand(ClearEnvFile); } }

        #endregion
    }
}
