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
    class ClearEnvViewModel : QAToolsBaseView
    {
        #region constructor

        public ClearEnvViewModel()
        {
            GammaClientTXManagerFactory.GetGammaClientTXManager().OnResultComesBack += RaiseClearEnvResultComeback;
        }

        #endregion

        #region TransactionManager Consumer

        private void RaiseClearEnvResultComeback(object sender, GammaUIUpdateArgs e)
        {
            if (e.TRANSACTION.TX_TYPE == GammaTransactionType.CLEARENV)
            {
                QAOpResult += string.Format("\r\n{0}: {1}\r\n", e.TRANSACTION.Machine, e.TRANSACTION.TX_RESULT);
            }
        }

        #endregion

        #region function

        private void ClearEnv(GammaTXClearEnv tx_code)
        {
            GroupAbled = false;
            Task client_job = ClearEnvOnNodes(tx_code);
            client_job.GetAwaiter().OnCompleted(() =>
            {
                GroupAbled = true;
            });
        }

        private Task ClearEnvOnNodes(GammaTXClearEnv tx_code)
        {
            List<Task<string>> tasks = new List<Task<string>>();
            var tx_mgr = GammaClientTXManagerFactory.GetGammaClientTXManager();
            var node_mgr = NodeNetManagerFactory.GetSimpleNetworkManager();
            foreach (var machine in from m in node_mgr.Machines where m.IsSelected && m.Alive == NodeState.Online select m)
            {
                tasks.Add(tx_mgr.StartClearEnvTransaction(machine, tx_code));
            }
            return Task.WhenAll(tasks);
        }

        #endregion

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

        bool CanClearEnv()
        {
            return GroupAbled;
        }

        public ICommand ClearRegistry { get { return new RelayCommand(ClearEnvReg, CanClearEnv); } }
        public ICommand ClearGroup { get { return new RelayCommand(ClearEnvGroup, CanClearEnv); } }
        public ICommand RebootNode { get { return new RelayCommand(ClearEnvRegReboot, CanClearEnv); } }
        public ICommand ClearDisk { get { return new RelayCommand(ClearEnvDisk, CanClearEnv); } }
        public ICommand ClearDrive { get { return new RelayCommand(ClearEnvDrive, CanClearEnv); } }
        public ICommand ClearFile { get { return new RelayCommand(ClearEnvFile, CanClearEnv); } }

        #endregion
    }
}
