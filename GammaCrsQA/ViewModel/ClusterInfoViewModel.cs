using GammaCrsQA.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;

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

    }
}
