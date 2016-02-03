using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GammaClient.GCFacilities.TXManager
{
    class GammaClientTransaction : IGammaClientTransaction
    {
        #region Member

        ulong transaction_id;
        string machine_name;
        string transaction_result_content;
        string transaction_result;
        GammaTransactionRC transaction_result_code;
        GammaTransactionType transaction_type;
        int transaction_subtype;

        
        public event EventHandler<EventArgs> OnTransactionStarted;
        public event EventHandler<EventArgs> OnTransactionFailed;
        public event EventHandler<EventArgs> OnTransactionCompleted;

        #endregion

        #region Property

        public string Machine
        {
            get
            {
                return machine_name;
            }

            set
            {
                machine_name = value;
            }
        }

        public string TX_CONTENT
        {
            get
            {
                return transaction_result_content;
            }

            set
            {
                transaction_result_content = value;
            }
        }

        public string TX_RESULT
        {
            get { return transaction_result; }
            set { transaction_result = value; }
        }

        public GammaTransactionRC TX_RESULT_CODE
        {
            get
            {
                return transaction_result_code;
            }

            set
            {
                transaction_result_code = value;
            }
        }

        public GammaTransactionType TX_TYPE
        {
            get
            {
                return transaction_type;
            }

            set
            {
                transaction_type = value;
            }
        }

        public ulong TX_ID
        {
            get
            {
                return transaction_id;
            }

            set
            {
                transaction_id = value;
            }
        }

        public int TX_SUB_TYPE
        {
            get
            {
                return transaction_subtype;
            }

            set
            {
                transaction_subtype = value;
            }
        }

        #endregion

        #region Function

        public void TransactionStarted()
        {
            var handler = OnTransactionStarted;
            if (handler != null)
            {
                handler(this, null);
            }
        }

        public void TransactionFailed()
        {
            var handler = OnTransactionFailed;
            if (handler != null)
            {
                handler(this, null);
            }
        }

        public void TransactionCompleted()
        {
            var handler = OnTransactionCompleted;
            if (handler != null)
            {
                handler(this, null);
            }
        }

        #endregion
    }

}
