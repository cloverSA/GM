using GammaCrsQAInstaller.NetworkManager;
using GammaCrsQAInstaller.TXManager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GammaCrsQA.ViewModel
{
    class QAToolsBaseView : INotifyPropertyChanged
    {
        #region Members

        private string opResult = string.Empty;

        private bool canExec = true; 

        protected GammaClientTXManager txMgr = GammaClientTXManagerFactory.GetGammaClientTXManager();

        protected NodeNetManager nodeMgr = NodeNetManagerFactory.GetSimpleNetworkManager();
        #endregion

        #region Properties

        public string QAOpResult
        {
            get
            {
                return opResult;
            }
            set
            {
                opResult = value;
                RaisePropertyChanged("QAOpResult");
            }
        }

        public bool GroupAbled
        {
            get
            {
                return canExec;
            }
            set
            {
                canExec = value;
                RaisePropertyChanged("GroupAbled");
            }
        }

        #endregion


        #region Eventmembers

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
