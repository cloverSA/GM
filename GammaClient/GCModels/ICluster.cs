using System.Collections.Generic;
using GammaClient.GCFacilities.WCFProxy;

namespace GammaClient.GCModels
{
    interface ICluster
    {
        string ClusterName { get; set; }
        int CUID { get; set; }
        bool IsSelected { get; set; }
        IEnumerable<IMachineInfo> Machines { get; set; }
    }
}