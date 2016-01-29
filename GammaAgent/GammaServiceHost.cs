using System;
using System.ServiceModel;
using System.ServiceModel.Description;

using GammaServiceLib;
using GammaServiceLib.OraCRS;

namespace GammaAgent
{
    interface IGammaSerivceHost
    {
        void HostOpen();
        void HostClose();
    }

    abstract class GammaServiceHost : IGammaSerivceHost
    {
        ServiceHost host = null;
        public GammaServiceHost(string uri)
        {
            Type serivce_type = GetServiceType();
            Type interface_type = GetInterfaceType();

            Uri httpUrl = new Uri(uri);
            host = new ServiceHost(serivce_type, httpUrl);
            
            host.AddServiceEndpoint(interface_type, new WSHttpBinding(SecurityMode.None), "");
            
            ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
            
            smb.HttpGetEnabled = true;
            host.Description.Behaviors.Add(smb);
            
        }

        public abstract Type GetServiceType();
        public abstract Type GetInterfaceType();
        public void HostOpen()
        {
            ServiceDebugBehavior debug = host.Description.Behaviors.Find<ServiceDebugBehavior>();
            if(debug == null)
            {
                host.Description.Behaviors.Add(new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });
            } else
            {
                if (!debug.IncludeExceptionDetailInFaults)
                {
                    debug.IncludeExceptionDetailInFaults = true;
                }
            }
            host.Open();
        }

        public void HostClose()
        {
            host.Close();
            host = null;
        }
    }

    class CrsCleanerServiceHost : GammaServiceHost
    {
        public CrsCleanerServiceHost(string uri):base(uri){ }

        public override Type GetInterfaceType()
        {
            return typeof(ICrsCleaner);
        }

        public override Type GetServiceType()
        {
            return typeof(CrsCleaner);
        }
    }

    class QAToolsServiceHost : GammaServiceHost
    {
        public QAToolsServiceHost(string uri) : base(uri) { }

        public override Type GetInterfaceType()
        {
            return typeof(IQATools);
        }

        public override Type GetServiceType()
        {
            return typeof(QATools);
        }
    }

    class CmdExecutorServiceHost : GammaServiceHost
    {
        public CmdExecutorServiceHost(string uri) : base(uri) { }

        public override Type GetInterfaceType()
        {
            return typeof(ICmdExecutor);
        }

        public override Type GetServiceType()
        {
            return typeof(CmdExecutor);
        }
    }

    class CrsEnvServiceHost : GammaServiceHost
    {
        public CrsEnvServiceHost(string uri) : base(uri) { }

        public override Type GetInterfaceType()
        {
            return typeof(ICrsEnv);
        }

        public override Type GetServiceType()
        {
            return typeof(CrsEnv);
        }
    }

    class DBWorkloadServiceHost: GammaServiceHost
    {
        public DBWorkloadServiceHost(string uri): base(uri) { }
        public override Type GetInterfaceType()
        {
            return typeof(IDBWorkload);
        }
        public override Type GetServiceType()
        {
            return typeof(DBWorkLoad);
        }
    }

    abstract class GammaServiceHostFactory
    {
        public abstract GammaServiceHost GetCrsCleanerSerivceHost();
        public abstract GammaServiceHost GetQAToolsSerivceHost();
        public abstract GammaServiceHost GetCmdExecutorSerivceHost();
        public abstract GammaServiceHost GetCrsEnvSerivceHost();
        public abstract GammaServiceHost GetDBWorkloadSerivceHost();
    }

    class GammaSimpleNetworkServiceHostFactory : GammaServiceHostFactory
    {
        GammaAgentInfo agent_info;
        public GammaSimpleNetworkServiceHostFactory()
        {
            agent_info = GammaAgentInfo.GetInstance();        
        }
        public override GammaServiceHost GetCrsCleanerSerivceHost()
        {
            return new CrsCleanerServiceHost(string.Format("http://{0}:{1}/GammaService/CrsCleaner", agent_info.HostIP, agent_info.HostPort));
        }
        public override GammaServiceHost GetQAToolsSerivceHost()
        {
            return new QAToolsServiceHost(string.Format("http://{0}:{1}/GammaService/QATools", agent_info.HostIP, agent_info.HostPort));
        }
        public override GammaServiceHost GetCmdExecutorSerivceHost()
        {
            return new CmdExecutorServiceHost(string.Format("http://{0}:{1}/GammaService/CmdExecutor", agent_info.HostIP, agent_info.HostPort));
        }
        public override GammaServiceHost GetCrsEnvSerivceHost()
        {
            return new CrsEnvServiceHost(string.Format("http://{0}:{1}/GammaService/CrsEnv", agent_info.HostIP, agent_info.HostPort));
        }
        public override GammaServiceHost GetDBWorkloadSerivceHost()
        {
            return new DBWorkloadServiceHost(string.Format("http://{0}:{1}/GammaService/DBWorkload", agent_info.HostIP, agent_info.HostPort));
        }
    }



}
