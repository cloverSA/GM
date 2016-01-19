using GammaCrsQA.Model;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace GammaCrsQA.NetworkManager
{
    abstract class NodeNetManager
    {
        private ObservableCollection<IGammaMachineInfo> _machines;
        public ObservableCollection<IGammaMachineInfo> Machines
        {
            get { return this._machines; }
            protected set { this._machines = value; }
        }

        public abstract void StartNodeCheck();
        public abstract void StopNodeCheck();
    }

    //It is simply using ping to test the network. 
    //And the machine network components are simple, using public_nic and service_port, no failover or service discovery.
    class SimpleNetworkManager : NodeNetManager
    {
        private static SimpleNetworkManager manager = new SimpleNetworkManager();

        public static SimpleNetworkManager GetInstance()
        {
            return manager;
        }

        private SimpleNetworkManager()
        {
            InitMaichines();
        }

        private void InitMaichines()
        {
            string conf = string.Empty;
            try
            {
                conf = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "machines.conf");
                string[] lines = File.ReadAllLines(conf);
                Machines = new ObservableCollection<IGammaMachineInfo>();
                foreach (string info in lines)
                {
                    Machines.Add(MachineInfoFactory.GetSimpleNetworkMachineInfo(info));
                }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("config file {0} cannot access: {1}", conf, ex.Message));
            }
        }
        
        private void NodeCheck()
        {
            foreach (IGammaMachineInfo machine in Machines)
            {
                Task.Run(() => {
                    Ping ping = new Ping();
                    ping.PingCompleted += new PingCompletedEventHandler(Ping_Complete);
                    ping.SendAsync(machine.NetworkCompent.GetWorkingNic(), 1000, machine);
                });
            }

        }
        //Notified update of node status
        private void Ping_Complete(object sender, PingCompletedEventArgs k)
        {
            PingReply reply = k.Reply;
            IGammaMachineInfo machine = k.UserState as IGammaMachineInfo;
            try
            {
                if (reply.Status == IPStatus.Success)
                {
                    machine.UpdateNotifiableProperties(NodeState.Online);
                }
                else
                {
                    machine.UpdateNotifiableProperties(NodeState.Offline);
                }
            }
            catch (NullReferenceException e)
            {
                machine.UpdateNotifiableProperties(NodeState.Unknown);
            }

        }

        private CancellationTokenSource _nodeCheckCTS;

        private CancellationToken _nodeCheckToken;
        private void NodeCheckTask()
        {
            if (_nodeCheckToken.IsCancellationRequested)
            {
                _nodeCheckToken.ThrowIfCancellationRequested();
            }
            while (!_nodeCheckToken.IsCancellationRequested)
            {
                NodeCheck();
                Thread.Sleep(TimeSpan.FromSeconds(15));
            }
        }

        public override void StartNodeCheck()
        {
            _nodeCheckCTS = new CancellationTokenSource();
            _nodeCheckToken = _nodeCheckCTS.Token;
            Task.Run(() => NodeCheckTask(), _nodeCheckToken);
        }

        public override void StopNodeCheck()
        {
            _nodeCheckCTS.Cancel();
        }
    }

    class NodeNetManagerFactory
    {

        public static NodeNetManager GetSimpleNetworkManager()
        {
            return SimpleNetworkManager.GetInstance();
        }
    }
}
