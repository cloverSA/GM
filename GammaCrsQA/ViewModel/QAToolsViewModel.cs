using GammaCrsQA.TXManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GammaCrsQA.ViewModel
{
    class QAToolsViewModel : NavigatorInTabPages<IPageViewModel>
    {
        public QAToolsViewModel()
        {
            // Add Page ViewModel 
            //PageViewModels.Add();
            GammaClientTXManagerFactory.GetGammaClientTXManager().OnResultComesBack += RaiseResultComeback;
        }

        private void RaiseResultComeback(object sender, GammaUIUpdateArgs e)
        {
            if (e.TRANSACTION.TX_TYPE == GammaTransactionType.QATOOLS)
            {
                OpResult += string.Format("\r\n{0}: {1}\r\n", e.TRANSACTION.Machine, e.TRANSACTION.TX_RESULT);
            }
        }
    }
}
