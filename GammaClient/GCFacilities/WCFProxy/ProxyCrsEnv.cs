using GammaServiceLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace GammaClient.GCFacilities.WCFProxy
{
    public class ProxyCrsEnv : GammaClientBase<ICrsEnv>, ICrsEnv
    {
        public ProxyCrsEnv(string uri_address) : base(uri_address)
        {

        }

        public override ChannelFactory<ICrsEnv> CreateProxyChannelFactory()
        {
            var service_binding = new WSHttpBinding(SecurityMode.None);
            service_binding.SendTimeout = TimeSpan.FromMinutes(25);
            var service_endpoint = new EndpointAddress(server_host_uri);
            var factory = new ChannelFactory<ICrsEnv>(service_binding, service_endpoint);
            return factory;
        }

        public string GetClusterNames()
        {
            ICrsEnv client = null;
            string rs = string.Empty;
            try
            {
                client = channel_factory.CreateChannel();
                rs = client.GetClusterNames();
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

        public Task<string> GetClusterNamesAsync()
        {
            return Task.Run(() => {
                return GetClusterNames();
            });
        }

        public string GetDBHOMEByName(string dbname)
        {
            ICrsEnv client = null;
            string rs = string.Empty;
            try
            {
                client = channel_factory.CreateChannel();
                rs = client.GetDBHOMEByName(dbname);
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

        public Task<string> GetDBHOMEByNameAsync(string dbname)
        {
            return Task.Run(() => {
                return GetDBHOMEByName(dbname);
            });
        }

        public string GetDBNames()
        {
            ICrsEnv client = null;
            string rs = string.Empty;
            try
            {
                client = channel_factory.CreateChannel();
                rs = client.GetDBNames();
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

        public Task<string> GetDBNamesAsync()
        {
            return Task.Run(() => {
                return GetDBNames();
            });
        }

        public string GetScan()
        {
            ICrsEnv client = null;
            string rs = string.Empty;
            try
            {
                client = channel_factory.CreateChannel();
                rs = client.GetScan();
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

        public Task<string> GetScanAsync()
        {
            return Task.Run(() => {
                return GetScan();
            });
        }

        public string GetScanPort()
        {
            ICrsEnv client = null;
            string rs = string.Empty;
            try
            {
                client = channel_factory.CreateChannel();
                rs = client.GetScanPort();
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

        public Task<string> GetScanPortAsync()
        {
            return Task.Run(() => {
                return GetScan();
            });
        }
    }
}
