using System;
using System.ServiceModel;
using System.Threading.Tasks;

using GeneralUtility;
namespace GammaCrsQA.WcfProxy
{
    public class QAToolsProxy : GammaClientBase<IQATools>, IQATools
    {
        public QAToolsProxy(string uri_address) : base(uri_address)
        {
        }
        public override ChannelFactory<IQATools> CreateProxyChannelFactory()
        {
            var service_binding = new WSHttpBinding(SecurityMode.None);
            service_binding.SendTimeout = TimeSpan.FromMinutes(25);
            var service_endpoint = new EndpointAddress(server_host_uri);
            var factory = new ChannelFactory<IQATools>(service_binding, service_endpoint);
            return factory;
        }

        public string ClearLog()
        {
            IQATools client = null;
            string result = string.Empty;
            try
            {
                client = channel_Factory.CreateChannel();
                result = client.ClearLog();
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

        public string GetLog(bool collect_dump)
        {
            IQATools client = null;
            string result = string.Empty;
            try
            {
                client = channel_Factory.CreateChannel();
                result = client.GetLog(collect_dump);
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

        public string UploadLog(UploadRecord record)
        {
            IQATools client = null;
            string result = string.Empty;
            try
            {
                client = channel_Factory.CreateChannel();
                string rs = client.UploadLog(record);
                if (result.ToLower().Contains("exception"))
                {
                    result = string.Format("{0} {1}", TX_RESULT_FAIL, rs);
                }
                else
                {
                    result = string.Format("{0} {1}", TX_RESULT_SUC, rs);
                }
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

        public Task<string> GetLogAsync(bool collect_dump)
        {
            return Task.Run(()=> {
              return  GetLog(collect_dump);
            });
        }
        public Task<string> UploadLogAsync(UploadRecord record)
        {
            return Task.Run(() => {
               return UploadLog(record);
            });
        }
        public Task<string> ClearLogAsync()
        {
            return Task.Run(() => {
               return ClearLog();
            });
        }

    }
}
