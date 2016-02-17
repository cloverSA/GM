using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GammaClient.GCModels;

namespace GammaClient.GCViewModels.WorkloadViewModels.Navigation
{
    abstract class PageViewModel : ObservableObject, IPageViewModel
    {

        #region Members

        private bool _canSwitchPage = true;

        private bool _inProgress = false;

        #endregion

        #region Properties

        public bool CanSwitchPage
        {
            get
            {
                return _canSwitchPage;
            }

            set
            {
                _canSwitchPage = value;
                RaisePropertyChanged("CanSwitchPage");
            }
        }

        public bool InProgress
        {
            get
            {
                return _inProgress;
            }
            set
            {
                _inProgress = value;
                RaisePropertyChanged("InProgress");
            }
        }

        #endregion

        #region Events

        public event EventHandler<NavigateArgs> NextPageEventHandler;
        public event EventHandler<NavigateArgs> PreviousPageEventHandler;

        #endregion

        #region Function
        

        public virtual void ProcessNavigateArgs(NavigateArgs args)
        {
            
        }

        public virtual void RaiseNextPageEvent(object sender, NavigateArgs e)
        {
            var handler = NextPageEventHandler;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public virtual void RaisePreviousPageEvent(object sender, NavigateArgs e)
        {
            var handler = PreviousPageEventHandler;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion
    }
}
