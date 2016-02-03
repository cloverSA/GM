using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace GammaClient.GCFacilities.WCFProxy
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
}
