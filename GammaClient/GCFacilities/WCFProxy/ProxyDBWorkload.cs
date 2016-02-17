using GammaServiceLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace GammaClient.GCFacilities.WCFProxy
{
    public class ProxyDBWorkload : GammaClientBase<IDBWorkload>, IDBWorkload
    {
        public ProxyDBWorkload(string uri) : base(uri)
        {

        }

        public override ChannelFactory<IDBWorkload> CreateProxyChannelFactory()
        {
            var service_binding = new WSHttpBinding(SecurityMode.None);
            service_binding.SendTimeout = TimeSpan.FromMinutes(25);
            var service_endpoint = new EndpointAddress(server_host_uri);
            var factory = new ChannelFactory<IDBWorkload>(service_binding, service_endpoint);
            return factory;
        }

        public string InstallSwingBench(string hostname, string dbhome, string dbname, string system_pwd, string sys_pwd, string workloadDmpLoc, string workloadDmpFileName)
        {
            IDBWorkload client = null;
            string rs = string.Empty;
            try
            {
                client = channel_factory.CreateChannel();
                rs = client.InstallSwingBench(hostname, dbhome, dbname, system_pwd, sys_pwd, workloadDmpLoc, workloadDmpFileName);
                ((ICommunicationObject)client).Close();
            }
            catch (Exception ex)
            {
                rs = string.Format("{0} communication error: {1}", TX_RESULT_FAIL, ex.Message);
                if (client != null)
                {
                    ((ICommunicationObject)client).Abort();
                }
            }
            return rs;
        }

        public Task<string> InstallSwingBenchAsync(string hostname, string dbhome, string dbname, string system_pwd, string sys_pwd, string workloadDmpLoc, string workloadDmpFileName)
        {
            return Task.Run(() =>
            {
                return InstallSwingBench(hostname, dbhome, dbname, system_pwd, sys_pwd, workloadDmpLoc, workloadDmpFileName);
            });
        }
    }
}
