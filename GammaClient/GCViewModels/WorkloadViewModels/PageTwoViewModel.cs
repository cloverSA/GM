using GalaSoft.MvvmLight;
using GammaClient.GCModels;
using GammaClient.GCViewModels.WorkloadViewModels.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GammaClient.GCViewModels.WorkloadViewModels
{
    class PageTwoViewModel : ObservableObject, IPageViewModel
    {
        private bool _canSwitchPage = true;
        private ObservableCollection<Cluster> _clusterItems;
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

        public ObservableCollection<Cluster> ClusterItems
        {
            get
            {
                return _clusterItems;
            }

            set
            {
                _clusterItems = value;
                RaisePropertyChanged("ClusterItems");
            }
        }

        public event EventHandler<NavigateArgs> NextPageEventHandler;
        public event EventHandler<NavigateArgs> PreviousPageEventHandler;

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

        public void ProcessNavigateArgs(NavigateArgs args)
        {
            ClusterItems = args.Item as ObservableCollection<Cluster>;
        }
    }
}
