using GammaClient.GCFacilities.NetworkManager;
using GammaClient.GCFacilities.WCFProxy;
using GammaClient.GCHelpers;
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
    class PageOneViewModel : IPageViewModel
    {
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
                    machine.ClusterName = t.Result;
                    results.Add(t.Result);
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

        public void SetClusterInfo()
        {
            WorkloadSetupInfo.SetValue(WorkloadSetupKeys.CLUSTERS, GetClusterInfo());
        }

        public ICommand SetClusterInfoCommand { get { return new RelayCommand(SetClusterInfo); } }


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
    }
}
