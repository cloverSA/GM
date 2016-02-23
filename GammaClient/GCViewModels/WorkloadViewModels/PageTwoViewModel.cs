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
using System.Windows;
using System.Windows.Input;

namespace GammaClient.GCViewModels.WorkloadViewModels
{
    class PageTwoViewModel : PageViewModel
    {
        private ObservableCollection<ICluster> _clusterItems;
        
        public ObservableCollection<ICluster> ClusterItems
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
            ClusterItems = WorkloadSetupInfo.GetValue<ObservableCollection<ICluster>>(WorkloadSetupKeys.CLUSTERS);
           
            foreach(var cluster in ClusterItems)
            {
                var cluster_nodes = CollectNodesFromCluster(cluster.ClusterName);
                if (cluster.Machines == null)
                {
                    cluster.Machines = new List<IMachineInfo>();
                }
                foreach (IMachineInfo m in cluster_nodes)
                {
                    cluster.Machines.Add(m);
                }
            }
        }

        private void GoBack(object command_parm)
        {
            RaisePreviousPageEvent(this, null);
        }

        private ObservableCollection<IOraDB> GetDbsBySelectedCluster()
        {
            var selected = from m in ClusterItems where m.IsSelected == true select m;
            
            if (selected.Count() == 1)
            {
                var cluster = ClusterItems.First<ICluster>();
                var candidate = from m in cluster.Machines
                                where m.Alive == NodeState.Online
                                select m;
                return GetDBnames(candidate.First());
            }
            else
            {
                return null;
            }
        }

        private ObservableCollection<IOraDB> GetDBnames(IMachineInfo machine)
        {
            var proxy = GammaProxyFactory.GetCrsEnvProxy(machine);
            var rs = proxy.GetDBNames();
            var dbnames = rs.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
            int count = 0;
            var rs_list = new ObservableCollection<IOraDB>();
            foreach (var db in dbnames)
            {
                var dbhome = proxy.GetDBHOMEByName(db);
                rs_list.Add(OraDBFactory.GetOraDB(count, db, dbhome));
                count++;
            }
            return rs_list;
        }

        private IGrouping<string, IMachineInfo> CollectNodesFromCluster(string clustername)
        {
            var igroup = from m in BaseFacility.NodeMgr.Machines
                         group m by m.ClusterName into gp
                         select gp;
            var cluster_nodes = from nodes in igroup where nodes.Key == clustername select nodes;
            if(cluster_nodes.Count() == 1)
            {
                return cluster_nodes.First();
            } else
            {
                return null;
            }
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
                    MessageBox.Show("No Db found in the cluster");
                }

            });

        }

    }
}
