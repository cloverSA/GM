using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GammaClient.GCModels;
using GammaClient.GCFacilities.UIServiceProvider;

namespace GammaClient.GCViewModels.WorkloadViewModels.Navigation
{
    abstract class PageViewModel : ObservableObject, IPageViewModel
    {

        #region Members

        private bool _canSwitchPage = true;

        private bool _inProgress = false;

        private readonly SPBase _baseFacility = new SPBase();

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

        public SPBase BaseFacility
        {
            get
            {
                return _baseFacility;
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

        public virtual void InProgressWait(bool inProgress)
        {
            if (inProgress)
            {
                CanSwitchPage = false;
                InProgress = true;
            } else
            {
                CanSwitchPage = true;
                InProgress = false;
            }
        }
        #endregion
    }
}
