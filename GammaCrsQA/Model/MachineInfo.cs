using GammaCrsQA.NetworkManager;
using System;
using System.ComponentModel;
using System.Linq;

namespace GammaCrsQA.Model
{
    public enum NodeState
    {
        Unknown,
        Online,
        Offline
    }

    public interface IGammaMachineInfo
    {
        MachineNetworkComponent NetworkCompent { get; set; }
        NodeState Alive { get; set; }
        bool IsSelected { get; set; }
        string MachineName { get; set; }
        string ClusterName { get; set; }
        void UpdateNotifiableProperties(object property_values);
    }
    public class MachineInfo : INotifyPropertyChanged, IGammaMachineInfo
    {
        private string nodename;

        private string clustername;

        private NodeState alive = NodeState.Offline;

        private bool selected = true;

        private MachineNetworkComponent network_comp;

        public MachineNetworkComponent NetworkCompent
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
            set { alive = value; }
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
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(object sender, string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(sender, new PropertyChangedEventArgs(propertyName));
            }
        }
        //Call the OnPropertyChanged inside this method after modify the property.
        public void UpdateNotifiableProperties(object property_values)
        {
            Alive = (NodeState)property_values;
            OnPropertyChanged(this, "Alive");
        }
    }



    public class MachineInfoFactory
    {
        public static MachineInfo GetSimpleNetworkMachineInfo(string info)
        {
            string[] rs = info.Split(',');
            if (rs.Count() != 3)
            {
                throw new Exception(String.Format("machine config info is incorrect, {0}", info));
            }
            //MachineInfo machine = new MachineInfo() { MachineName = rs[0].Trim() };
            MachineInfo machine = new MachineInfo(rs[0].Trim());
            machine.NetworkCompent = NetworkComponentFactory.GetSimpleNetworkComponent(rs[1].Trim(), rs[2].Trim());
            return machine;
        }
    }

}
