using GammaCrsQA.Model;
using GammaCrsQA.NetworkManager;
using GammaCrsQA.WcfProxy;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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

        private IEnumerable<string> GetDbsBySelectedCluster()
        {
            var net_mgr = NodeNetManagerFactory.GetSimpleNetworkManager();
            var machines = net_mgr.Machines;
            var igroup = from m in machines
                         group m by m.ClusterName into gp
                         select gp;
            var updated_items = WorkloadSetupInfo.GetValue<ObservableCollection<Cluster>>(WorkloadSetupKeys.CLUSTERS);
            var selected = from m in updated_items where m.IsSelected == true select m;
            if (selected.Count() == 1)
            {
                var cluster = selected.First<Cluster>();
                var cluster_nodes = from nodes in igroup where nodes.Key == cluster.ClusterName select nodes;
                var candiate_node = cluster_nodes.First().First();
                var proxy = GammaProxyFactory.GetCrsEnvProxy(candiate_node);
                var rs = proxy.GetDBNames();
                return rs.Split(new string[]{Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries).ToList();
            }
            else
            {
                return null;
            }
        }

        private void SetDbs(IEnumerable<string> dbnames)
        {
            var rs_list = new ObservableCollection<OraDB>();
            int count = 0;
            foreach (var dbname in dbnames)
            {
                rs_list.Add(new OraDB(count) { DBName = dbname });
                count += 1;
            }
        }


    }
}
