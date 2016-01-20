using GammaCrsQA.Model;
using GammaCrsQA.NetworkManager;
using GammaCrsQA.WcfProxy;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
namespace GammaCrsQA.ViewModel
{
    class ClusterInfoViewModel : INotifyPropertyChanged
    {
        public ClusterInfoViewModel()
        {
            ClusterItems = WorkloadSetupInfo.GetValue<ObservableCollection<Cluster>>(WorkloadSetupKeys.CLUSTERS);
            WorkLoads = new ObservableCollection<string>() { "Swingbench", "Aroltp" };
        }

        public ObservableCollection<Cluster> ClusterItems { get; set; }
        public ObservableCollection<string> WorkLoads { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<OraDB> GetDbsBySelectedCluster()
        {
            
            var updated_items = WorkloadSetupInfo.GetValue<ObservableCollection<Cluster>>(WorkloadSetupKeys.CLUSTERS);
            var selected = from m in updated_items where m.IsSelected == true select m;
            var rs_list = new ObservableCollection<OraDB>();
            if (selected.Count() == 1)
            {
                var cluster = selected.First<Cluster>();
                var candiate_node = SelectOneNodeFromCluster(cluster.ClusterName);
                var proxy = GammaProxyFactory.GetCrsEnvProxy(candiate_node);
                var rs = proxy.GetDBNames();
                var dbnames = rs.Split(new string[]{Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries).ToList();
                
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

        public void SetDbs(object button)
        {
            var btn = button as Button;
            btn.IsEnabled = false;
            var task = Task.Run(() => {
                WorkloadSetupInfo.SetValue(WorkloadSetupKeys.DBS, GetDbsBySelectedCluster());
            });
            task.GetAwaiter().OnCompleted(() => {
                btn.IsEnabled = true;
            });
            
        }

        private IGammaMachineInfo SelectOneNodeFromCluster(string clustername)
        {
            var net_mgr = NodeNetManagerFactory.GetSimpleNetworkManager();
            var machines = net_mgr.Machines;
            var igroup = from m in machines
                         group m by m.ClusterName into gp
                         select gp;
            var cluster_nodes = from nodes in igroup where nodes.Key == clustername select nodes;
            return cluster_nodes.First().First();

        }

        public ICommand SetDBsCommand { get { return new RelayCommand<object>(SetDbs); } }
    }
}
