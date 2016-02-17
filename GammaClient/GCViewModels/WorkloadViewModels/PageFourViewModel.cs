using GammaClient.GCViewModels.WorkloadViewModels.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GammaClient.GCModels;
using System.Collections.ObjectModel;
using GammaClient.GCFacilities.WCFProxy;


namespace GammaClient.GCViewModels.WorkloadViewModels
{
    class PageFourViewModel : PageViewModel
    {
        
        public ObservableCollection<OraDB> DBs { get; set; }
        public override void ProcessNavigateArgs(NavigateArgs args)
        {
            DBs = args.Item as ObservableCollection<OraDB>;
        }

        public string SYSPWD { get; set; }
        public string SYSTEMPWD { get; set; }
        public string WorkloadDmpLoc { get; set; }
        public string WorkloadDmpFilename { get; set; }

        private string _result;
        public string Result
        {
            get
            {
                return _result;
            }

            set
            {
                _result = value;
                RaisePropertyChanged("Result");
            }
        }

        private void GenerateScript()
        {
            var tasks = new List<Task<string>>();
            foreach (var db in DBs)
            {
                if(db.WorkLoad == WorkLoad.Swingbench)
                {
                    tasks.Add(GenerateSwingbench(db));
                }
            }
            var ok = Task.WhenAll(tasks);
            Array.ForEach(ok.Result, (rs) => 
            {
                Result += string.Format("Script generated result : {0} \n", rs);
            });
        }

        private Task<string> GenerateSwingbench(OraDB db)
        {
            var db_cluster = GetSelectedCluster(WorkloadSetupInfo.GetValue<ObservableCollection<Cluster>>(WorkloadSetupKeys.CLUSTERS));
            var machine = CollectOneNodeFromCluster(db_cluster);
            var proxy = GammaProxyFactory.GetDBWorkloadProxy(machine);
            
            var task = proxy.InstallSwingBenchAsync(machine.MachineName, db.DBHome, db.DBName, SYSTEMPWD, SYSPWD, WorkloadDmpLoc, WorkloadDmpFilename);
            return task;
        }

        private IMachineInfo CollectOneNodeFromCluster(Cluster cluster)
        {
            var rs = from m in cluster.Machines
                     where m.Alive == GCFacilities.WCFProxy.NodeState.Online
                     select m;
            return rs.First();
        }

        private Cluster GetSelectedCluster(ObservableCollection<Cluster> clusters)
        {
            var rs = from c in clusters
                     where c.IsSelected == true
                     select c;
            return rs.First();
        }
    }
}
