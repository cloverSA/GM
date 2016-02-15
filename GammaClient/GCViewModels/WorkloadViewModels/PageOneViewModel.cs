using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GammaClient.GCFacilities.NetworkManager;
using GammaClient.GCFacilities.WCFProxy;
using GammaClient.GCModels;
using GammaClient.GCViewModels.WorkloadViewModels.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GammaClient.GCViewModels.WorkloadViewModels
{
    class PageOneViewModel : ObservableObject, IPageViewModel
    {
        #region Members

        private bool _inProgress = false;
        private bool _canSwitchPage = true;

        #endregion

        #region Properties

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

        public event EventHandler<NavigateArgs> NextPageEventHandler;
        public event EventHandler<NavigateArgs> PreviousPageEventHandler;


        public ICommand SetClusterInfoCommand { get { return new RelayCommand<object>(SetClusterInfo); } }

        #endregion

        #region Functions

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

        public void SetClusterInfo(object sender)
        {
            var way = sender as string;
            CanSwitchPage = false;
            InProgress = true;
            ObservableCollection<Cluster> rs = null;
            var task = Task.Run(() =>
            {
                rs = GetClusterInfo();
            });
            task.GetAwaiter().OnCompleted(() =>
            {
                InProgress = false;
                CanSwitchPage = true;
                if (way.Contains("NextPage"))
                {
                    RaiseNextPageEvent(this, new NavigateArgs(rs));
                }
            });
        }


        private ObservableCollection<Cluster> GetClusterInfo()
        {
            ObservableCollection<Cluster> clusters = new ObservableCollection<Cluster>();
            var net_mgr = NetworkManagerFactory.GetSimpleNetworkManager();
            var tasks = new List<Task>();
            var results = new List<string>();
            foreach (var machine in from m in net_mgr.Machines where m.Alive == NodeState.Online && m.IsSelected select m)
            {
                var proxy = GammaProxyFactory.GetCrsEnvProxy(machine);
                var rs = proxy.GetClusterNamesAsync();
                tasks.Add(rs.ContinueWith((t) =>
                {
                    if (!t.Result.ToLower().Contains("error"))
                    {
                        machine.ClusterName = t.Result;
                        results.Add(t.Result);
                    }
                }));
            }
            Task.WhenAll(tasks).GetAwaiter().GetResult();

            int counter = 0;

            foreach (var rs in results.Distinct())
            {
                clusters.Add(new Cluster(counter) { ClusterName = rs.Trim() });
                counter += 1;
            }

            return clusters;
        }

        public void ProcessNavigateArgs(NavigateArgs args)
        {
            
        }



        #endregion
    }
}
