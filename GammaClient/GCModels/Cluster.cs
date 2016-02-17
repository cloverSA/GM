using GalaSoft.MvvmLight;
using GammaClient.GCFacilities.WCFProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GammaClient.GCModels
{
    class Cluster : ObservableObject, ICluster
    {
        public string ClusterName { get; set; }
        public int CUID { get; set; }

        public IEnumerable<IMachineInfo> Machines { get; set; }
        private bool _isSelected = false;
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                if (_isSelected == value)
                {
                    return;
                }
                _isSelected = value;
                RaisePropertyChanged("IsSelected");
            }
        }

        public Cluster(int id)
        {
            CUID = id;
        }
    }

}
