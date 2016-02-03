using GammaCrsQA.NetworkManager;
using GammaCrsQA.TXManager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GammaCrsQA.ViewModel
{
    /*
    for each new tab created, its viewmodel should inherit this class to access the base facility
    */
    public class TabPagesBaseView : ObservableObject
    {
        #region Members

        private string _opResult = string.Empty;

        private bool _canExec = true;

        private GammaClientTXManager _txMgr = GammaClientTXManagerFactory.GetGammaClientTXManager();

        private NodeNetManager _nodeMgr = NodeNetManagerFactory.GetSimpleNetworkManager();

        #endregion

        #region Property

        public string OpResult
        {
            get
            {
                return _opResult;
            }

            set
            {
                _opResult = value;
                OnPropertyChanged("OpResult");
            }
        }

        public bool CanExec
        {
            get
            {
                return _canExec;
            }

            set
            {
                _canExec = value;
                OnPropertyChanged("CanExec");
            }
        }

        internal GammaClientTXManager TxMgr
        {
            get
            {
                return _txMgr;
            }

        }

        internal NodeNetManager NodeMgr
        {
            get
            {
                return _nodeMgr;
            }

        }

        #endregion

        private void ScrollDownResult(object sender)
        {
            var tb = sender as TextBox;
            if (tb != null)
            {
                tb.Focus();
                tb.CaretIndex = tb.Text.Length;
                tb.ScrollToEnd();
            }
        }

    }
}
