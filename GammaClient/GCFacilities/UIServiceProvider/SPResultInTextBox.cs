using GalaSoft.MvvmLight.CommandWpf;
using GammaClient.GCFacilities.TXManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace GammaClient.GCFacilities.UIServiceProvider
{
    abstract class SPResultInTextBox : SPBase
    {
        #region Member

        private string _opResult = string.Empty;

        private bool _canExec = true;

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

        #region UI Behavior

        void ScrollDownResult(object sender)
        {
            var tb = sender as TextBox;
            if (tb != null)
            {
                tb.Focus();
                tb.CaretIndex = tb.Text.Length;
                tb.ScrollToEnd();
            }
        }

        public ICommand ScrollDownCommand { get { return new RelayCommand<object>(ScrollDownResult); } }
        #endregion

        #region tempalate Constructor

        public SPResultInTextBox()
        {
            TxMgr.OnResultComesBack += RaiseResultComeback;
        }
        protected abstract void RaiseResultComeback(object sender, GammaUIUpdateArgs e);
        
        #endregion

    }
}
