using GammaCrsQA.Model;
using GammaCrsQA.NetworkManager;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GammaCrsQA.ViewModel
{
    class MainWindowViewModel
    {
        private NodeNetManager _nodeNetManager;


        public MainWindowViewModel()
        {
            _nodeNetManager = NodeNetManagerFactory.GetSimpleNetworkManager();
            _nodeNetManager.StartNodeCheck();
            
        }

        public NodeNetManager NodeNetManager
        {
            get
            {
                return _nodeNetManager;
            }
        }

        public ObservableCollection<IGammaMachineInfo> Machines
        {
            get
            {
                return _nodeNetManager.Machines;
            }
        }
    }
}
