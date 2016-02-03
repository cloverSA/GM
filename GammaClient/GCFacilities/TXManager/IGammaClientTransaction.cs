using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GammaClient.GCFacilities.TXManager
{
    interface IGammaClientTransaction : IGammaClientTransactionBase
    {
        void TransactionStarted();
        void TransactionFailed();
        void TransactionCompleted();
        event EventHandler<EventArgs> OnTransactionStarted;
        event EventHandler<EventArgs> OnTransactionFailed;
        event EventHandler<EventArgs> OnTransactionCompleted;
    }

}
