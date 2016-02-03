using GammaCrsQA.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GammaCrsQA.ViewModel
{
    class WorkLoadDBInfoViewModel : IPageViewModel
    {
        private bool _canSwitchPage = false;
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
        public WorkLoadDBInfoViewModel()
        {
            DBs = WorkloadSetupInfo.GetValue <ObservableCollection<OraDB>>(WorkloadSetupKeys.DBS);

            WorkLoads = new ObservableCollection<string>() { "Swingbench", "Aroltp" };
        }

        public ObservableCollection<OraDB> DBs { get; set; }
        public ObservableCollection<string> WorkLoads { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
