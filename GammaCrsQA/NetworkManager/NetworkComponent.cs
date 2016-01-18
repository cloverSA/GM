namespace GammaCrsQA.NetworkManager
{
    //In different network infrustructure, we can still get/set the working nic.
    public abstract class MachineNetworkComponent
    {
        public abstract string GetWorkingNic();
        public abstract void SetWorkingNic(string value);
        public abstract string GetWorkingServicePort();
        public abstract void SetWorkingServicePort(string value);
    }
    //only support communication using public nic, no service discovery.
    public class SimpleNetworkComponent : MachineNetworkComponent
    {
        private string public_ip;
        private string service_port;

        public override string GetWorkingNic()
        {
            return this.public_ip;
        }

        public override string GetWorkingServicePort()
        {
            return this.service_port;
        }

        public override void SetWorkingNic(string value)
        {
            this.public_ip = value;
        }

        public override void SetWorkingServicePort(string value)
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
