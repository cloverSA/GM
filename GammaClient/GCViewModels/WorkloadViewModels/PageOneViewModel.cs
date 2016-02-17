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
    class PageOneViewModel : PageViewModel
    {
        #region Command

        public ICommand SetClusterInfoCommand { get { return new RelayCommand<object>(SetClusterInfo); } }
        
        #endregion

        #region Functions

        public void SetClusterInfo(object command_parm)
        {
            CanSwitchPage = false;
            InProgress = true;
            ObservableCollection<ICluster> rs = null;
            var task = GetClusterInfo();
            task.GetAwaiter().OnCompleted(()=> {
                InProgress = false;
                CanSwitchPage = true;
                WorkloadSetupInfo.SetValue(WorkloadSetupKeys.CLUSTERS, rs);
                RaiseNextPageEvent(this, null);
            });
            
        }


        private async Task<ObservableCollection<ICluster>> GetClusterInfo()
        {
            ObservableCollection<ICluster> clusters = new ObservableCollection<ICluster>();
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
            await Task.WhenAll(tasks);

            int counter = 0;

            foreach (var rs in results.Distinct())
            {
                clusters.Add(new Cluster(counter) { ClusterName = rs.Trim() });
                counter += 1;
            }

            return clusters;
        }

        #endregion
    }
}
