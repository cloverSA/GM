using GammaClient.GCViewModels.WorkloadViewModels.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GammaClient.GCModels;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;

namespace GammaClient.GCViewModels.WorkloadViewModels
{
    class PageThreeViewModel : PageViewModel
    {
        public override void ProcessNavigateArgs(NavigateArgs args)
        {
            DBs = args.Item as ObservableCollection<OraDB>;

            WorkLoads = new ObservableCollection<string>() { "Swingbench", "Aroltp" };
        }

        public ObservableCollection<OraDB> DBs { get; set; }
        public ObservableCollection<string> WorkLoads { get; set; }

        private void GoNext(object command_parm)
        {
            RaisePreviousPageEvent(this, new NavigateArgs(DBs));
        }

        private void GoBack(object command_parm)
        {
            RaisePreviousPageEvent(this, null);
        }

        public ICommand GoNextCommand { get { return new RelayCommand<object>(GoNext); } }
        public ICommand GoBackCommand { get { return new RelayCommand<object>(GoBack); } }
    }
}
