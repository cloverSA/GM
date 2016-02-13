using GammaClient.GCFacilities.NetworkManager;
using GammaClient.GCFacilities.TXManager;
using GammaClient.GCHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GammaClient.GCFacilities.UIServiceProvider
{
    class SPBase : ObservableObject
    {
        #region Members

        private GammaClientTXManager _txMgr = GammaClientTXManagerFactory.GetGammaClientTXManager();

        private NetworkManagerBase _nodeMgr = NetworkManagerFactory.GetSimpleNetworkManager();

        #endregion

        #region Property

        internal GammaClientTXManager TxMgr
        {
            get
            {
                return _txMgr;
            }

        }

        internal NetworkManagerBase NodeMgr
        {
            get
            {
                return _nodeMgr;
            }

        }

        #endregion
    }

}
