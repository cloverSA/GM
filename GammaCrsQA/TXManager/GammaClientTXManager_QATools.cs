using System;
using System.Threading.Tasks;
using GeneralUtility;
using GammaCrsQA.NetworkManager;
using GammaCrsQA.WcfProxy;
using GammaCrsQA.Model;

namespace GammaCrsQA.TXManager
{
    partial class GammaClientTXManager : IGammaClientTXManager
    {
        /*
          QA tools transactions
        */
        public Task<string> StartQAToolsTransaction(IGammaMachineInfo machine, GammaTXQATools tool, object qatoolArgs = null)
        {
            return Task.Run(() =>
            {
                var trx = GammaClientTXFactory.GetQAToolsTX(machine, tool);
                AddTransaction(trx);
                return StartTransactions(trx, machine, tool, qatoolArgs);

            });
        }
        /*
             return the task based on the enum code.
            and also bind the task with the transaction here.
        */
        private Task<string> StartTransactions(IGammaClientTransaction trx, IGammaMachineInfo machine, GammaTXQATools tool, object qatoolArgs = null)
        {
            trx.TransactionStarted();
            Task<string> task;
            switch (tool)
            {
                case GammaTXQATools.GETLOG:
                    if (qatoolArgs != null)
                    {
                        bool tmp = (bool)qatoolArgs;
                        task = GammaProxyFactory.GetQAToolsProxy(machine).GetLogAsync(tmp);
                    }
                    else
                    {
                        throw new Exception("Unexpected error when try to get operation option.");
                    }
                    break;

                case GammaTXQATools.CLEARLOG:
                    task = GammaProxyFactory.GetQAToolsProxy(machine).ClearLogAsync();
                    break;

                case GammaTXQATools.UPLOAD:
                    if (qatoolArgs != null)
                    {
                        GammaServiceLib.UploadRecord tmp = qatoolArgs as GammaServiceLib.UploadRecord;
                        task = GammaProxyFactory.GetQAToolsProxy(machine).UploadLogAsync(tmp);
                    }
                    else
                    {
                        throw new Exception("Unexpected error when try to get upload information.");
                    }
                    break;

                default:
                    task = GenerateCompletedTask(TX_RESULT_SUC);
                    break;
            }
            BindCmdTaskWithTranscation(task, trx);
            return task;
        }
    }
}
