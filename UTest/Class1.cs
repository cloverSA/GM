using GammaServiceLib;
using GammaServiceLib.OraCRS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace UTest
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
            if (debug == null)
            {
                host.Description.Behaviors.Add(new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });
            }
            else
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
            try
            {
                host.Abort();
                host = null;
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine("GammaAgent", string.Format("Close failed, Current state of host is: {0}; {1}", host.State.ToString(), e.Message));
            }
            catch (CommunicationObjectFaultedException e)
            {
                Console.WriteLine("GammaAgent", string.Format("Close failed, Current state of host is: {0}", host.State.ToString(), e.Message));
            }

        }
    }

    class CrsCleanerServiceHost : GammaServiceHost
    {
        public CrsCleanerServiceHost(string uri) : base(uri) { }

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

    class DBWorkloadServiceHost : GammaServiceHost
    {
        public DBWorkloadServiceHost(string uri) : base(uri) { }
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

    class GammaAgentInfo
    {
        public string HostIP
        {
            get; private set;
        }
        public string HostPort
        {
            get;
            private set;
        }
        static GammaAgentInfo agent_info = new GammaAgentInfo();

        public static GammaAgentInfo GetInstance()
        {
            return agent_info;
        }

        private GammaAgentInfo()
        {
            GetPublicNicFromConfig();
        }

        private void GetPublicNicFromConfig()
        {
            string[] lines;
            string loc = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            try
            {
                lines = System.IO.File.ReadAllLines(System.IO.Path.Combine(loc, "machine.conf"));
                foreach (string line in lines)
                {
                    if (line.ToLower().Contains("public"))
                    {
                        string[] tmp = line.Split('=');
                        if (tmp.Count() == 2)
                        {
                            HostIP = tmp[1].ToString().Trim();
                        }
                    }
                    if (line.ToLower().Contains("port"))
                    {
                        string[] tmp = line.Split('=');
                        if (tmp.Count() == 2)
                        {
                            HostPort = tmp[1].ToString().Trim();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Agent error, no network config file: {0}", ex.Message));
            }

        }
    }
    interface IGammaServerManager
    {
        void ServerOpen();
        void ServerClose();
    }

    class GammaServerManagerFactory
    {
        public static GammaServerManager GetServerManager()
        {
            return new GammaServerManager();
        }
    }

    class GammaServerManager : IGammaServerManager
    {
        IGammaSerivceHost crs_clean_service;
        IGammaSerivceHost qa_tools_service;
        IGammaSerivceHost shell_executor_service;
        IGammaSerivceHost crsenv_service;
        IGammaSerivceHost dbworkload_service;

        public GammaServerManager()
        {
            GammaServiceHostFactory factory = new GammaSimpleNetworkServiceHostFactory();
            crs_clean_service = factory.GetCrsCleanerSerivceHost();
            qa_tools_service = factory.GetQAToolsSerivceHost();
            shell_executor_service = factory.GetCmdExecutorSerivceHost();
            crsenv_service = factory.GetCrsEnvSerivceHost();
            dbworkload_service = factory.GetDBWorkloadSerivceHost();
        }

        public void ServerOpen()
        {
            crs_clean_service.HostOpen();
            qa_tools_service.HostOpen();
            shell_executor_service.HostOpen();
            crsenv_service.HostOpen();
            dbworkload_service.HostOpen();
        }
        public void ServerClose()
        {
            crs_clean_service.HostClose();
            qa_tools_service.HostClose();
            shell_executor_service.HostClose();
            crsenv_service.HostClose();
            dbworkload_service.HostOpen();
        }
    }
}
