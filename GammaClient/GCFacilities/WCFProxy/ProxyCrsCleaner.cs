using GammaServiceLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace GammaClient.GCFacilities.WCFProxy
{

    public class ProxyCrsCleaner : GammaClientBase<ICrsCleaner>, ICrsCleaner
    {
        public ProxyCrsCleaner(string uri_address) : base(uri_address)
        {
        }

        public override ChannelFactory<ICrsCleaner> CreateProxyChannelFactory()
        {
            var service_binding = new WSHttpBinding(SecurityMode.None);
            service_binding.SendTimeout = TimeSpan.FromMinutes(25);
            var service_endpoint = new EndpointAddress(server_host_uri);
            var factory = new ChannelFactory<ICrsCleaner>(service_binding, service_endpoint);
            return factory;
        }

        public string CleanDisk()
        {
            ICrsCleaner client = null;
            string result = string.Empty;
            try
            {
                client = channel_factory.CreateChannel();
                result = client.CleanDisk();
                ((ICommunicationObject)client).Close();
            }
            catch (Exception ex)
            {
                result = string.Format("{0} communication failure, {1}", TX_RESULT_FAIL, ex.Message);
                if (client != null)
                {
                    ((ICommunicationObject)client).Abort();
                }
            }
            return result;
        }

        public string RemoveDrvLtr()
        {
            ICrsCleaner client = null;
            string result = string.Empty;
            try
            {
                client = channel_factory.CreateChannel();
                result = client.RemoveDrvLtr();
                ((ICommunicationObject)client).Close();
            }
            catch (Exception ex)
            {
                result = string.Format("{0} communication failure, {1}", TX_RESULT_FAIL, ex.Message);
                if (client != null)
                {
                    ((ICommunicationObject)client).Abort();
                }
            }
            return result;
        }

        public string RemoveOraFiles()
        {
            ICrsCleaner client = null;
            string result = string.Empty;
            try
            {
                client = channel_factory.CreateChannel();
                result = client.RemoveOraFiles();
                ((ICommunicationObject)client).Close();
            }
            catch (Exception ex)
            {
                result = string.Format("{0} communication failure, {1}", TX_RESULT_FAIL, ex.Message);
                if (client != null)
                {
                    ((ICommunicationObject)client).Abort();
                }
            }
            return result;
        }

        public string RemoveOraKeys()
        {
            ICrsCleaner client = null;
            string result = string.Empty;
            try
            {
                client = channel_factory.CreateChannel();
                result = client.RemoveOraKeys();
                ((ICommunicationObject)client).Close();
            }
            catch (Exception ex)
            {
                result = string.Format("{0} communication failure, {1}", TX_RESULT_FAIL, ex.Message);
                if (client != null)
                {
                    ((ICommunicationObject)client).Abort();
                }
            }
            return result;
        }

        public string RmOraGroup()
        {
            ICrsCleaner client = null;
            string result = string.Empty;
            try
            {
                client = channel_factory.CreateChannel();
                result = client.RmOraGroup();
                ((ICommunicationObject)client).Close();
            }
            catch (Exception ex)
            {
                result = string.Format("{0} communication failure, {1}", TX_RESULT_FAIL, ex.Message);
                if (client != null)
                {
                    ((ICommunicationObject)client).Abort();
                }
            }
            return result;
        }

        public Task<string> CleanDiskAsync()
        {
            return Task.Run(() =>
            {
                return CleanDisk();
            });
        }

        public Task<string> RemoveDrvLtrAsync()
        {
            return Task.Run(() =>
            {
                return RemoveDrvLtr();
            });
        }

        public Task<string> RemoveOraFilesAsync()
        {
            return Task.Run(() =>
            {
                return RemoveOraFiles();
            });
        }

        public Task<string> RemoveOraKeysAsync()
        {
            return Task.Run(() =>
            {
                return RemoveOraKeys();
            });
        }

        public Task<string> RmOraGroupAsync()
        {
            return Task.Run(() =>
            {
                return RmOraGroup();
            });
        }

        public void RestartComputer()
        {
            ICrsCleaner client = null;
            string result = string.Empty;
            try
            {
                client = channel_factory.CreateChannel();
                client.RestartComputer();
                ((ICommunicationObject)client).Close();
            }
            catch (Exception ex)
            {
                result = string.Format("{0} communication failure, {1}", TX_RESULT_FAIL, ex.Message);
                if (client != null)
                {
                    ((ICommunicationObject)client).Abort();
                }
            }
        }

        public Task RestartComputerAsync()
        {
            return Task.Run(() =>
            {
                RestartComputer();
            });
        }
    }

}
