using System.ServiceModel;
using GeneralUtility;
using GammaCrsQA.NetworkManager;
using GammaCrsQA.Model;

namespace GammaCrsQA.WcfProxy
{

    public abstract class GammaClientBase<T>
    {
        protected string server_host_uri;
        protected ChannelFactory<T> channel_factory;
        protected const string TX_RESULT_FAIL = "[GAMMA_ERROR]";
        protected const string TX_RESULT_SUC = "[GAMMA_SUC]";

        public GammaClientBase(string uri_address)
        {
            server_host_uri = uri_address;
            channel_factory = CreateProxyChannelFactory();
        }

        public abstract ChannelFactory<T> CreateProxyChannelFactory();

    }

    public class GammaProxyFactory
    {
        public static CmdExecutorProxy GetCmdExecutorProxy(IGammaMachineInfo machine)
        {
            string uri = string.Format("http://{0}:{1}/GammaService/CmdExecutor", machine.NetworkCompent.GetWorkingNic(), machine.NetworkCompent.GetWorkingServicePort());
            return new CmdExecutorProxy(uri);
        }

        public static CrsCleanerProxy GetCrsCleanerProxy(IGammaMachineInfo machine)
        {
            string uri = string.Format("http://{0}:{1}/GammaService/CrsCleaner", machine.NetworkCompent.GetWorkingNic(), machine.NetworkCompent.GetWorkingServicePort());
            return new CrsCleanerProxy(uri);
        }

        public static QAToolsProxy GetQAToolsProxy(IGammaMachineInfo machine)
        {
            string uri = string.Format("http://{0}:{1}/GammaService/QATools", machine.NetworkCompent.GetWorkingNic(), machine.NetworkCompent.GetWorkingServicePort());
            return new QAToolsProxy(uri);
        }

        public static CrsEnvProxy GetCrsEnvProxy(IGammaMachineInfo machine)
        {
            string uri = string.Format("http://{0}:{1}/GammaService/CrsEnv", machine.NetworkCompent.GetWorkingNic(), machine.NetworkCompent.GetWorkingServicePort());
            return new CrsEnvProxy(uri);
        }

        public static DBWorkloadProxy GetDBWorkloadProxy(IGammaMachineInfo machine)
        {
            string uri = string.Format("http://{0}:{1}/GammaService/DBWorkload", machine.NetworkCompent.GetWorkingNic(), machine.NetworkCompent.GetWorkingServicePort());
            return new DBWorkloadProxy(uri);
        }
    }

}
