using GammaClient.GCViewModels.WorkloadViewModels.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GammaClient.GCModels;
using System.Collections.ObjectModel;
using GammaClient.GCFacilities.WCFProxy;
using System.Windows.Controls;
using GalaSoft.MvvmLight.CommandWpf;
using System.Windows.Input;
using GammaClient.GCUIBehavior;

namespace GammaClient.GCViewModels.WorkloadViewModels
{
    class PageFourViewModel : PageViewModel
    {
        public PageFourViewModel(Action<PageFourViewModel> handler)
        {
            _closehandler = handler;
            DBs = WorkloadSetupInfo.GetValue<ObservableCollection<IOraDB>>(WorkloadSetupKeys.DBS);
        }

        private readonly Action<PageFourViewModel> _closehandler;

        public ObservableCollection<IOraDB> DBs { get; set; }
        public override void ProcessNavigateArgs(NavigateArgs args)
        {
            
        }
        private string _workloadDmpLoc;
        private string _workloadDmpFilename;
        public string SYSPWD { get; set; }
        public string SYSTEMPWD { get; set; }


        public string WorkloadDmpLoc
        {
            get
            {
                return _workloadDmpLoc;
            }

            set
            {
                _workloadDmpLoc = value;
                RaisePropertyChanged("WorkloadDmpLoc");
            }
        }

        public string WorkloadDmpFilename
        {
            get
            {
                return _workloadDmpFilename;
            }

            set
            {
                _workloadDmpFilename = value;
                RaisePropertyChanged("WorkloadDmpFilename");
            }
        }

        private string _result;
        private IContentScrollDown _scroller = new TextBoxScrollDown();

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

        private async void GenerateScript()
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
            await ok;
            Array.ForEach(ok.Result, (rs) => 
            {
                Result += string.Format("Script generated result : {0} \n", rs);
            });
        }

        private Task<string> GenerateSwingbench(IOraDB db)
        {
            var db_cluster = GetSelectedCluster(WorkloadSetupInfo.GetValue<ObservableCollection<ICluster>>(WorkloadSetupKeys.CLUSTERS));
            var machine = CollectOneNodeFromCluster(db_cluster);
            var proxy = GammaProxyFactory.GetDBWorkloadProxy(machine);
            
            var task = proxy.InstallSwingBenchAsync(machine.MachineName, db.DBHome, db.DBName, SYSTEMPWD, SYSPWD, WorkloadDmpLoc, WorkloadDmpFilename);
            return task;
        }

        private IMachineInfo CollectOneNodeFromCluster(ICluster cluster)
        {
            var rs = from m in cluster.Machines
                     where m.Alive == GCFacilities.WCFProxy.NodeState.Online
                     select m;
            return rs.First();
        }

        private ICluster GetSelectedCluster(ObservableCollection<ICluster> clusters)
        {
            var rs = from c in clusters
                     where c.IsSelected == true
                     select c;
            return rs.First();
        }

        private void SystemPasswordChanged(object sender)
        {
            var pwdbox = sender as PasswordBox;
            SYSTEMPWD = pwdbox.Password;
        }

        private void SysPasswordChanged(object sender)
        {
            var pwdbox = sender as PasswordBox;
            SYSPWD = pwdbox.Password;
        }

        public ICommand SysPasswordChangedCommand { get { return new RelayCommand<object>(SysPasswordChanged); } }
        public ICommand SystemPasswordChangedCommand { get { return new RelayCommand<object>(SystemPasswordChanged); } }
        public ICommand CloseCommand { get { return new RelayCommand<PageFourViewModel>(_closehandler); } }
        public ICommand InstallCommand { get { return new RelayCommand(GenerateScript); } }
        public ICommand ScrollDownCommand { get { return _scroller.ScrollDownCommand; } }
    }
}
