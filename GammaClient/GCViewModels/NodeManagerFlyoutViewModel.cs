using GammaClient.GCFacilities.NetworkManager;
using GammaClient.GCFacilities.UIServiceProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GammaClient.GCFacilities.WCFProxy;
using System.Collections.ObjectModel;

namespace GammaClient.GCViewModels
{
    class NodeManagerFlyoutViewModel
    {
        private ObservableCollection<IMachineInfo> _netmgr;

        public NodeManagerFlyoutViewModel()
        {
            NetMgr = NetworkManagerFactory.GetSimpleNetworkManager().Machines;
        }

        public ObservableCollection<IMachineInfo> NetMgr
        {
            get
            {
                return _netmgr;
            }

            set
            {
                _netmgr = value;
            }
        }
    }
}
