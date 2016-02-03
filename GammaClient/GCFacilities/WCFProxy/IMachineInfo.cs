using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GammaClient.GCFacilities.WCFProxy
{
    public enum NodeState
    {
        Unknown,
        Online,
        Offline
    }

    public interface IMachineInfo
    {
        IMachineNetworkComponent NetworkCompent { get; set; }
        NodeState Alive { get; set; }
        bool IsSelected { get; set; }
        string MachineName { get; set; }
        string ClusterName { get; set; }
    }
}
