using GammaServiceLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace GammaClient.GCFacilities.WCFProxy
{

    public class ProxyCmdExecutor : GammaClientBase<ICmdExecutor>, ICmdExecutor
    {
        public ProxyCmdExecutor(string uri_address) : base(uri_address)
        {
        }
        public override ChannelFactory<ICmdExecutor> CreateProxyChannelFactory()
        {
            var service_binding = new WSHttpBinding(SecurityMode.None);
            service_binding.SendTimeout = TimeSpan.FromMinutes(25);
            var service_endpoint = new EndpointAddress(server_host_uri);
            var factory = new ChannelFactory<ICmdExecutor>(service_binding, service_endpoint);
            return factory;
        }

        public string ShellExecutor(string filename, string arguments)
        {
            ICmdExecutor client = null;
            string rs = string.Empty;
            try
            {
                client = channel_factory.CreateChannel();
                rs = client.ShellExecutor(filename, arguments);
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

        public Task<string> ShellExecutorAsync(string filename, string arguments)
        {
            return Task.Run(() => {
                return ShellExecutor(filename, arguments);
            });
        }

    }
}
