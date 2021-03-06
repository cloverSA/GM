﻿using GammaClient.GCFacilities.WCFProxy;
using GammaClient.GCModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GammaClient.GCFacilities.NetworkManager
{
    class SimpleNetworkManager : NetworkManagerBase
    {
        private static SimpleNetworkManager manager = new SimpleNetworkManager();

        public static SimpleNetworkManager GetInstance()
        {
            return manager;
        }

        private SimpleNetworkManager()
        {
            InitMaichines();
            this.StartNodeCheck();
        }

        private void InitMaichines()
        {
            string conf = string.Empty;
            try
            {
                conf = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "machines.conf");
                string[] lines = File.ReadAllLines(conf);

                Machines = new ObservableCollection<IMachineInfo>();
                foreach (string info in lines)
                {
                    if (info.Length != 0)
                    {
                        Machines.Add(MachineInfoFactory.GetSimpleNetworkMachineInfo(info));
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("config file {0} cannot access: {1}", conf, ex.Message));
            }
        }

        private void NodeCheck()
        {
            foreach (IMachineInfo machine in Machines)
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
            IMachineInfo machine = k.UserState as IMachineInfo;
            try
            {
                if (reply.Status == IPStatus.Success)
                {
                    machine.Alive = (NodeState.Online);
                }
                else
                {
                    machine.Alive = (NodeState.Offline);
                }
            }
            catch (NullReferenceException e)
            {
                machine.Alive = (NodeState.Unknown);
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
}
