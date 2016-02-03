using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GammaClient.GCFacilities.TXManager
{
    class GammaClientTXManagerFactory
    {
        public static GammaClientTXManager GetGammaClientTXManager()
        {
            return GammaClientTXManager.GetInstance();
        }
    }
}
