using GammaClient.GCFacilities.WCFProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GammaClient.GCFacilities.TXManager
{
    class GammaClientTXFactory
    {
        internal static GammaClientTransaction GetCmdExecTX(IMachineInfo machine, string command)
        {
            var trx = new GammaClientTransaction()
            {
                Machine = machine.MachineName,
                TX_TYPE = GammaTransactionType.COMMAND,
                TX_CONTENT = command,
                TX_RESULT = string.Empty,
                TX_RESULT_CODE = GammaTransactionRC.RUNNING,
            };
            GammaClientTXLogger.GetInstance().RegisterTransaction(trx);
            return trx;
        }

        internal static GammaClientTransaction GetCmdExecTX(IMachineInfo machine, string[] command)
        {
            var sb = new StringBuilder();
            foreach (var cmd in command)
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
            GammaClientTXLogger.GetInstance().RegisterTransaction(trx);
            return trx;
        }

        internal static GammaClientTransaction GetQAToolsTX(IMachineInfo machine, GammaTXQATools op_code)
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
            GammaClientTXLogger.GetInstance().RegisterTransaction(trx);
            return trx;
        }

        internal static GammaClientTransaction GetClearEnvTX(IMachineInfo machine, GammaTXClearEnv op_code)
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
            GammaClientTXLogger.GetInstance().RegisterTransaction(trx);
            return trx;
        }

        internal static List<GammaClientTransaction> GetCmdParallelExecTX(IMachineInfo machine, string[] commands)
        {
            var transactions = new List<GammaClientTransaction>();
            foreach (var cmd in commands)
            {
                transactions.Add(GetCmdExecTX(machine, commands));
            }
            return transactions;
        }
    }

}
