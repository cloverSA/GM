using GammaClient.GCFacilities.WCFProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GammaClient.GCModels
{
    public class MachineInfo : GCHelpers.ObservableObject, IMachineInfo
    {
        private string nodename;

        private string clustername;

        private NodeState alive = NodeState.Offline;

        private bool selected = true;

        private IMachineNetworkComponent network_comp;

        public IMachineNetworkComponent NetworkCompent
        {
            get { return this.network_comp; }
            set { this.network_comp = value; }
        }

        public bool IsSelected
        {
            get { return this.selected; }
            set { this.selected = value; }
        }

        public NodeState Alive
        {
            get { return alive; }
            set {
                if(alive == value)
                {
                    return;
                }
                alive = value;
                OnPropertyChanged("Alive");
            }
        }

        public string MachineName
        {
            get { return nodename; }
            set { nodename = value; }
        }

        public string ClusterName
        {
            get { return clustername; }
            set
            {
                if (clustername == value.Trim())
                {
                    return;
                }
                clustername = value.Trim();
            }
        }

        //if we do not provide constructor, the system will use the default constractor which is: 
        //public MachineInfo(){} , when binding the observalecollection<MachineInfo> to datagrid, 
        //it would create a blank row to allow user to add element to the collection.
        //if we provide constructor here, then no allow user to add element from data grid.
        public MachineInfo(string hostname)
        {
            this.MachineName = hostname;
        }
    }


}
