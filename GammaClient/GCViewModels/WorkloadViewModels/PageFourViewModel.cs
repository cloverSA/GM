using GammaClient.GCViewModels.WorkloadViewModels.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GammaClient.GCModels;

namespace GammaClient.GCViewModels.WorkloadViewModels
{
    class PageFourViewModel : IPageViewModel
    {

        private bool _canSwitchPage = true;

        public bool CanSwitchPage
        {
            get
            {
                return _canSwitchPage;
            }

            set
            {
                _canSwitchPage = value;
            }
        }

        public event EventHandler<NavigateArgs> NextPageEventHandler;
        public event EventHandler<NavigateArgs> PreviousPageEventHandler;

        public void ProcessNavigateArgs(NavigateArgs args)
        {
            
        }

        private void RaiseNextPageEvent(object sender, NavigateArgs e)
        {
            var handler = NextPageEventHandler;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void RaisePreviousPageEvent(object sender, NavigateArgs e)
        {
            var handler = PreviousPageEventHandler;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
