using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GammaCrsQA.Model
{
    
    class Cluster : INotifyPropertyChanged
    {
        
        
        public string ClusterName { get; set; }
        public int CUID { get; set; }

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
                RaisePropertyChangedEvent("IsSelected");
            }
        }

        public Cluster(int id)
        {
            CUID = id;
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChangedEvent(string pname)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(pname));
            }
        }
    }
}
