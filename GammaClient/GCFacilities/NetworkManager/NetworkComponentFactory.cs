using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GammaClient.GCFacilities.NetworkManager
{
    public class NetworkComponentFactory
    {
        public static WCFProxy.IMachineNetworkComponent GetSimpleNetworkComponent(string ip, string port)
        {
            return new SimpleNetworkComponent(ip, port);
        }
    }
}
