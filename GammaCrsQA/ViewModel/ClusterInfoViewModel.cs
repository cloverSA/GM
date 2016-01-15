using GammaCrsQA.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GammaCrsQA.ViewModel
{
    class ClusterInfoViewModel : INotifyPropertyChanged
    {
        public ClusterInfoViewModel()
        {
            ClusterItems = new ObservableCollection<Cluster>() {
                new Cluster(1) { ClusterName="111", CUID=1
                },
                new Cluster(2) { ClusterName="222", CUID=2
                },
            };
            WorkLoads = new ObservableCollection<string>() { "Swingbench", "Aroltp" };
        }

        public ObservableCollection<Cluster> ClusterItems { get; set; }
        public ObservableCollection<string> WorkLoads { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;


        
    }
}
