using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GammaClient.GCFacilities.WCFProxy
{
    public class GammaProxyFactory
    {
        public static ProxyCmdExecutor GetCmdExecutorProxy(IMachineInfo machine)
        {
            string uri = string.Format("http://{0}:{1}/GammaService/CmdExecutor", machine.NetworkCompent.GetWorkingNic(), machine.NetworkCompent.GetWorkingServicePort());
            return new ProxyCmdExecutor(uri);
        }

        public static ProxyCrsCleaner GetCrsCleanerProxy(IMachineInfo machine)
        {
            string uri = string.Format("http://{0}:{1}/GammaService/CrsCleaner", machine.NetworkCompent.GetWorkingNic(), machine.NetworkCompent.GetWorkingServicePort());
            return new ProxyCrsCleaner(uri);
        }

        public static ProxyQATools GetQAToolsProxy(IMachineInfo machine)
        {
            string uri = string.Format("http://{0}:{1}/GammaService/QATools", machine.NetworkCompent.GetWorkingNic(), machine.NetworkCompent.GetWorkingServicePort());
            return new ProxyQATools(uri);
        }

        public static ProxyCrsEnv GetCrsEnvProxy(IMachineInfo machine)
        {
            string uri = string.Format("http://{0}:{1}/GammaService/CrsEnv", machine.NetworkCompent.GetWorkingNic(), machine.NetworkCompent.GetWorkingServicePort());
            return new ProxyCrsEnv(uri);
        }

        public static ProxyDBWorkload GetDBWorkloadProxy(IMachineInfo machine)
        {
            string uri = string.Format("http://{0}:{1}/GammaService/DBWorkload", machine.NetworkCompent.GetWorkingNic(), machine.NetworkCompent.GetWorkingServicePort());
            return new ProxyDBWorkload(uri);
        }
    }

}
