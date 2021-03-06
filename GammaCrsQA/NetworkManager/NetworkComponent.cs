﻿namespace GammaCrsQA.NetworkManager
{
    //In different network infrustructure, we can still get/set the working nic.
    public interface IMachineNetworkComponent
    {
        string GetWorkingNic();
        void SetWorkingNic(string value);
        string GetWorkingServicePort();
        void SetWorkingServicePort(string value);
    }
    //only support communication using public nic, no service discovery.
    public class SimpleNetworkComponent : IMachineNetworkComponent
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

    public class NetworkComponentFactory
    {
        public static SimpleNetworkComponent GetSimpleNetworkComponent(string ip, string port)
        {
            return new SimpleNetworkComponent(ip, port);
        }
    }
}
