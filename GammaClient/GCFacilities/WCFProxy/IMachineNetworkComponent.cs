using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GammaClient.GCFacilities.WCFProxy
{
    public interface IMachineNetworkComponent
    {
        string GetWorkingNic();
        void SetWorkingNic(string value);
        string GetWorkingServicePort();
        void SetWorkingServicePort(string value);
    }
}
