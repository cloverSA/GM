using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GammaClient.GCFacilities.NetworkManager
{
    //only support communication using public nic, no service discovery.
    public class SimpleNetworkComponent : WCFProxy.IMachineNetworkComponent
    {
        private string public_ip;
        private string service_port;

        public string GetWorkingNic()
        {
            return this.public_ip;
        }

        public string GetWorkingServicePort()
        {
            return this.service_port;
        }

        public void SetWorkingNic(string value)
        {
            this.public_ip = value;
        }

        public void SetWorkingServicePort(string value)
        {
            this.service_port = value;
        }

        public SimpleNetworkComponent(string ip, string port)
        {
            public_ip = ip;
            service_port = port;
        }

    }


}
