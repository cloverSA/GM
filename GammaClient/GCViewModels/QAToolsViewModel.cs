using GammaClient.GCFacilities.UIServiceProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GammaClient.GCFacilities.TXManager;

namespace GammaClient.GCViewModels
{
    class QAToolsViewModel : SPResultInTextBox
    {
        protected override void RaiseResultComeback(object sender, GammaUIUpdateArgs e)
        {
            if (e.TRANSACTION.TX_TYPE == GammaTransactionType.QATOOLS)
            {
                OpResult += string.Format("\r\n{0}: {1}\r\n", e.TRANSACTION.Machine, e.TRANSACTION.TX_RESULT);
            }
        }
    }
}
