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
    class PageTwoViewModel : PageViewModel
    {
        private ObservableCollection<Cluster> _clusterItems;

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

        #region Command

        public ICommand SetDBsCommand { get { return new RelayCommand<object>(SetDbs); } }
        public ICommand GoBackCommand { get { return new RelayCommand<object>(GoBack); } }
        
        #endregion

        public override void ProcessNavigateArgs(NavigateArgs args)
        {
            ClusterItems = WorkloadSetupInfo.GetValue<ObservableCollection<Cluster>>(WorkloadSetupKeys.CLUSTERS);
        }

        private void GoBack(object command_parm)
        {
            RaisePreviousPageEvent(this, null);
        }

        private ObservableCollection<OraDB> GetDbsBySelectedCluster()
        {

            var updated_items = WorkloadSetupInfo.GetValue<ObservableCollection<Cluster>>(WorkloadSetupKeys.CLUSTERS);
            var selected = from m in updated_items where m.IsSelected == true select m;
            var rs_list = new ObservableCollection<OraDB>();
            if (selected.Count() == 1)
            {
                var cluster = selected.First<Cluster>();
                var cluster_node = CollectNodesFromCluster(cluster.ClusterName);
                foreach(IMachineInfo m in cluster_node)
                {
                    cluster.Machines.ToList<IMachineInfo>().Add(m);
                }
                var candiate_node = cluster_node.First();
                var proxy = GammaProxyFactory.GetCrsEnvProxy(candiate_node);
                var rs = proxy.GetDBNames();
                var dbnames = rs.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
                int count = 0;
                foreach (var db in dbnames)
                {
                    var dbhome = proxy.GetDBHOMEByName(db);
                    rs_list.Add(new OraDB(count) { DBHome = dbhome, DBName = db });
                    count++;
                }
                return rs_list;
            }
            else
            {
                return null;
            }
        }

        private IGrouping<string, IMachineInfo> CollectNodesFromCluster(string clustername)
        {
            var net_mgr = NetworkManagerFactory.GetSimpleNetworkManager();
            var machines = net_mgr.Machines;
            var igroup = from m in machines
                         group m by m.ClusterName into gp
                         select gp;
            var cluster_nodes = from nodes in igroup where nodes.Key == clustername select nodes;
            return cluster_nodes.First();
        }

        public void SetDbs(object command_parm)
        {
            CanSwitchPage = false;
            InProgress = true;
            var task = Task.Run(() =>
            {
                return GetDbsBySelectedCluster();
            });
            task.GetAwaiter().OnCompleted(() =>
            {

                CanSwitchPage = true;
                InProgress = false;
                if (task.Result != null)
                {
                    WorkloadSetupInfo.SetValue(WorkloadSetupKeys.DBS, task.Result);
                    RaiseNextPageEvent(this, null);
                }
                else
                {
                    //show dialog of no db found.
                }

            });

        }

    }
}
