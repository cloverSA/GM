using GammaClient.GCFacilities.WCFProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GammaClient.GCFacilities.TXManager
{
    interface IGammaClientTXManager
    {
        // register to this event if the ui would need the result
        event EventHandler<GammaUIUpdateArgs> OnResultComesBack;
        Task<string> StartCmdInSeqTransaction(IMachineInfo machine, string[] commands);
        List<Task<string>> StartCmdInParallelTransaction(IMachineInfo machine, string[] commands);
    }
}
