using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GammaClient.GCFacilities.NetworkManager
{
    class NetworkManagerFactory
    {

        public static NetworkmanagerBase GetSimpleNetworkManager()
        {
            return SimpleNetworkManager.GetInstance();
        }
    }
}
