using GammaCrsQAInstaller.NetworkManager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace GammaCrsQAInstaller.TXManager
{
    enum GammaTransactionType
    {
        COMMAND,
        TESTCASE,
        QATOOLS,
        CLEARENV,
    }

    enum GammaTransactionRC
    {
        SUCCEED,
        FAIL,
        RUNNING,
        UNKNOWN,
    }

    enum GammaTXQATools
    {
        UPLOAD,
        GETLOG,
        CLEARLOG,
    }

    enum GammaTXClearEnv
    {
        REG,
        FILE,
        GROUP,
        DISK,
        DRIVER,
        REBOOT,
    }
    //
    interface IGammaClientTransactionBase
    {
        string Machine
        {
            get; set;
        }
        GammaTransactionType TX_TYPE
        {
            get; set;
        }
        GammaTransactionRC TX_RESULT_CODE
        {
            get; set;
        }
        string TX_RESULT
        {
            get; set;
        }
        string TX_CONTENT
        {
            get; set;
        }
        ulong TX_ID
        {
            get; set;
        }
        int TX_SUB_TYPE
        {
            get; set;
        }
    }

    interface IGammaClientTransaction : IGammaClientTransactionBase
    {
        void TransactionStarted();
        void TransactionFailed();
        void TransactionCompleted();
        event EventHandler<EventArgs> OnTransactionStarted;
        event EventHandler<EventArgs> OnTransactionFailed;
        event EventHandler<EventArgs> OnTransactionCompleted;
    }

    class GammaClientTransaction : IGammaClientTransaction
    {
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

        public void TransactionStarted()
        {
            if (OnTransactionStarted != null)
            {
                OnTransactionStarted(this, null);
            }
        }

        public void TransactionFailed()
        {
            if (OnTransactionFailed != null)
            {
                OnTransactionFailed(this, null);
            }
        }

        public void TransactionCompleted()
        {
            if (OnTransactionCompleted != null)
            {
                OnTransactionCompleted(this, null);
            }
        }

    }

    class GammaClientTXFactory
    {
        internal static GammaClientTransaction GetCmdExecTX(IGammaMachineInfo machine, string command)
        {
            var trx =  new GammaClientTransaction()
            {
                Machine = machine.MachineName,
                TX_TYPE = GammaTransactionType.COMMAND,
                TX_CONTENT = command,
                TX_RESULT = string.Empty,
                TX_RESULT_CODE = GammaTransactionRC.RUNNING,
            };
            //init event handler
            GammaClientTXLogger.GetInstance().RegisterTransaction(trx);
            return trx;
        }

        internal static GammaClientTransaction GetCmdExecTX(IGammaMachineInfo machine, string[] command)
        {
            var sb = new StringBuilder();
            foreach(var cmd in command)
            {
                sb.AppendFormat("{0};", cmd);
            }
            var trx = new GammaClientTransaction()
            {
                Machine = machine.MachineName,
                TX_TYPE = GammaTransactionType.COMMAND,
                TX_CONTENT = sb.ToString(),
                TX_RESULT = string.Empty,
                TX_RESULT_CODE = GammaTransactionRC.RUNNING,
            };
            //init event handler
            GammaClientTXLogger.GetInstance().RegisterTransaction(trx);
            return trx;
        }

        internal static GammaClientTransaction GetQAToolsTX(IGammaMachineInfo machine, GammaTXQATools op_code)
        {
            var trx = new GammaClientTransaction()
            {
                Machine = machine.MachineName,
                TX_TYPE = GammaTransactionType.QATOOLS,
                TX_SUB_TYPE = (int)op_code,
                TX_CONTENT = string.Empty,
                TX_RESULT = string.Empty,
                TX_RESULT_CODE = GammaTransactionRC.RUNNING,
            };
            //init event handler
            GammaClientTXLogger.GetInstance().RegisterTransaction(trx);
            return trx;
        }

        internal static GammaClientTransaction GetClearEnvTX(IGammaMachineInfo machine, GammaTXClearEnv op_code)
        {
            var trx = new GammaClientTransaction()
            {
                Machine = machine.MachineName,
                TX_TYPE = GammaTransactionType.CLEARENV,
                TX_SUB_TYPE = (int)op_code,
                TX_CONTENT = string.Empty,
                TX_RESULT = string.Empty,
                TX_RESULT_CODE = GammaTransactionRC.RUNNING,
            };
            //init event handler
            GammaClientTXLogger.GetInstance().RegisterTransaction(trx);
            return trx;
        }

        internal static List<GammaClientTransaction> GetCmdParallelExecTX(IGammaMachineInfo machine, string[] commands)
        {
            var transactions = new List<GammaClientTransaction>();
            foreach(var cmd in commands)
            {
                transactions.Add(GetCmdExecTX(machine, commands));
            }
            return transactions;
        }
    }

}
