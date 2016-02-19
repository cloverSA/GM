using GalaSoft.MvvmLight.CommandWpf;
using GammaClient.GCFacilities.TXManager;
using GammaClient.GCUIBehavior;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace GammaClient.GCFacilities.UIServiceProvider
{
    abstract class SPResult : SPBase
    {
        #region Member

        private string _opResult = string.Empty;

        private bool _canExec = true;

        private IContentScrollDown _scroller = new TextBoxScrollDown();

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
                RaisePropertyChanged("OpResult");
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
                RaisePropertyChanged("CanExec");
            }
        }

        #endregion

        public ICommand ScrollDownCommand { get { return _scroller.ScrollDownCommand; } }

        #region tempalate Constructor

        public SPResult()
        {
            TxMgr.OnResultComesBack += RaiseResultComeback;
        }
        protected abstract void RaiseResultComeback(object sender, GammaUIUpdateArgs e);
        
        #endregion

    }
}
