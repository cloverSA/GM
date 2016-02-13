using GammaClient.GCFacilities.WCFProxy;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GammaClient.GCFacilities.NetworkManager
{
    abstract class NetworkManagerBase
    {
        private ObservableCollection<IMachineInfo> _machines;
        public ObservableCollection<IMachineInfo> Machines
        {
            get { return this._machines; }
            protected set { this._machines = value; }
        }

        public abstract void StartNodeCheck();
        public abstract void StopNodeCheck();
    }
}
