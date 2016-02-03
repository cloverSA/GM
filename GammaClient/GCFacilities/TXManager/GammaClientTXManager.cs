using GammaClient.GCFacilities.WCFProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GammaClient.GCFacilities.TXManager
{
    class GammaClientTXManager : IGammaClientTXManager
    {
        private const string TX_RESULT_FAIL = "[GAMMA_ERROR]";
        private const string TX_RESULT_SUC = "[GAMMA_SUC]";
        private List<IGammaClientTransaction> gamma_transactions;
        private ulong seq;
        private object locker = new object();

        public List<IGammaClientTransaction> TRANSACTIONS
        {
            get
            {
                return gamma_transactions;
            }

            set
            {
                gamma_transactions = value;
            }
        }

        static GammaClientTXManager manager = new GammaClientTXManager();

        private GammaClientTXManager()
        {
            gamma_transactions = new List<IGammaClientTransaction>();

            seq = 0;
        }

        public static GammaClientTXManager GetInstance()
        {
            return manager;
        }

        private void AddTransaction(IGammaClientTransaction trx)
        {
            lock (locker)
            {
                trx.TX_ID = seq;
                seq += 1;
                gamma_transactions.Add(trx);
            }
        }

        private void AddTransaction(IGammaClientTransaction trx, ulong task_id)
        {
            lock (locker)
            {
                trx.TX_ID = task_id;
                gamma_transactions.Add(trx);
            }
        }

        private void TransactionExpired(IGammaClientTransaction trx)
        {
            lock (locker)
            {
                gamma_transactions.Remove(trx);
            }
        }

        public event EventHandler<GammaUIUpdateArgs> OnResultComesBack;

        /*
           Bind the task with the transaction, so that  when the task is finished, it will set the trasaction's status
           And rasie the event to update the UI.
        */
        private void BindCmdTaskWithTranscation(Task<string> task, IGammaClientTransaction trx)
        {
            task.GetAwaiter().OnCompleted(() => {//code here are still run by the thread who started the task.
                if (task.IsCompleted)
                {
                    trx.TX_RESULT = task.Result;
                    if (task.Result.ToString().ToLower().Contains(TX_RESULT_FAIL))
                    {
                        trx.TX_RESULT_CODE = GammaTransactionRC.FAIL;
                        trx.TransactionFailed();
                    }
                    else
                    {
                        trx.TX_RESULT_CODE = GammaTransactionRC.SUCCEED;
                        trx.TransactionCompleted();
                    }
                }
                else if (task.IsFaulted)
                {
                    trx.TX_RESULT_CODE = GammaTransactionRC.UNKNOWN;
                    StringBuilder sb = new StringBuilder();
                    sb.AppendFormat("[Task Faulted: Task Status is ] {0}", task.Status);
                    sb.AppendLine();
                    if (task.Exception != null)
                    {
                        task.Exception.Handle((x) => {
                            sb.AppendFormat("[Task Faulted Error: Exception ] {0}", x.Message);
                            sb.AppendLine();
                            return true;
                        });
                    }
                    trx.TX_RESULT = sb.ToString();
                    trx.TransactionFailed();
                }
                else if (task.IsCanceled)
                {
                    trx.TX_RESULT_CODE = GammaTransactionRC.UNKNOWN;
                    trx.TX_RESULT = "Task is canceled";
                    trx.TransactionFailed();
                }
                else
                {
                    trx.TX_RESULT_CODE = GammaTransactionRC.UNKNOWN;
                    StringBuilder sb = new StringBuilder();
                    sb.AppendFormat("[Task Faulted: Task Status is ] {0}", task.Status);
                    sb.AppendLine();
                    trx.TX_RESULT = sb.ToString();
                    trx.TransactionFailed();
                }
                //Notify UI update//
                ResultComesBack(trx);
            });
        }

        private void ResultComesBack(IGammaClientTransaction transaction)
        {
            var handler = OnResultComesBack;
            if (handler != null)
            {

                handler(this, new GammaUIUpdateArgs() { TRANSACTION = transaction });

            }
            TransactionExpired(transaction);
        }

        public Task<T> GenerateCompletedTask<T>(T value)
        {
            var tcs = new TaskCompletionSource<T>();
            tcs.SetResult(value);
            return tcs.Task;
        }

        #region ClearEnv_TRX
        /*
            Clear environment transactions
        */
        public Task<string> StartClearEnvTransaction(IMachineInfo machine, GammaTXClearEnv clear_step)
        {
            return Task.Run(() =>
            {
                var trx = GammaClientTXFactory.GetClearEnvTX(machine, clear_step);
                AddTransaction(trx);
                return StartTransactions(trx, machine, clear_step);

            });
        }

        private Task<string> StartTransactions(IGammaClientTransaction trx, IMachineInfo machine, GammaTXClearEnv clear_step)
        {
            trx.TransactionStarted();
            Task<string> task;
            if (clear_step != GammaTXClearEnv.REBOOT)
            {
                switch (clear_step)
                {
                    case GammaTXClearEnv.REG:
                        task = GammaProxyFactory.GetCrsCleanerProxy(machine).RemoveOraKeysAsync();
                        break;
                    case GammaTXClearEnv.GROUP:
                        task = GammaProxyFactory.GetCrsCleanerProxy(machine).RmOraGroupAsync();
                        break;
                    case GammaTXClearEnv.FILE:
                        task = GammaProxyFactory.GetCrsCleanerProxy(machine).RemoveOraFilesAsync();
                        break;
                    case GammaTXClearEnv.DISK:
                        task = GammaProxyFactory.GetCrsCleanerProxy(machine).CleanDiskAsync();
                        break;
                    case GammaTXClearEnv.DRIVER:
                        task = GammaProxyFactory.GetCrsCleanerProxy(machine).RemoveDrvLtrAsync();
                        break;
                    default:
                        task = GenerateCompletedTask(TX_RESULT_SUC);
                        break;
                }
                BindCmdTaskWithTranscation(task, trx);
            }
            else
            {
                /*
                    reboot node transaction no need to Bind, as it will always fails by its mean.
                */
                var tmp = GammaProxyFactory.GetCrsCleanerProxy(machine).RestartComputerAsync();
                task = tmp.ContinueWith<string>((t) => {
                    trx.TransactionCompleted();
                    return TX_RESULT_SUC;
                });
            }

            return task;
        }
        #endregion

        #region Cmd_TRX
        /*
            Run Commands on machine in sequence, one after another.
            And the result will be returned when all of them are finished.
        */
        public Task<string> StartCmdInSeqTransaction(IMachineInfo machine, string[] commands)
        {
            return Task.Run(() => {
                var trx = GammaClientTXFactory.GetCmdExecTX(machine, commands);
                AddTransaction(trx);
                return StartTransactions(trx, machine, commands);
            });
        }
        /*
           return the task that run the command on the node.
           and also bind the task with the transaction here.
       */
        private Task<string> StartTransactions(IGammaClientTransaction trx, IMachineInfo machine, string[] commands)
        {
            trx.TransactionStarted();
            var task = ProxyFacadeCmdExecutor.ExecuteCmdsSeqByNodeAsync(machine, commands);
            BindCmdTaskWithTranscation(task, trx);
            return task;
        }
        /*
           if the command is being run as parallel, all the nodes' transaction will share the same seq#,
           so that later we can notice that they are submitted as parallel run on all nodes.
       */
        public List<Task<string>> StartCmdInParallelTransaction(IMachineInfo machine, string[] commands)
        {
            List<Task<string>> tasks = new List<Task<string>>();
            //all cmds are run together and share the same seq#
            foreach (string cmd in commands)
            {
                tasks.Add(StartCmdGroupTransaction(machine, cmd));
            }
            return tasks;
        }
        /*
           return the task that run the command on the node with seq# not changed.
           if the command are considered as in the same group, they should use this to start the transaction.
       */
        private Task<string> StartCmdGroupTransaction(IMachineInfo machine, string command)
        {
            return Task.Run(() => {
                var trx = GammaClientTXFactory.GetCmdExecTX(machine, command);
                GammaClientTXLogger.GetInstance().RegisterTransaction(trx);
                AddTransaction(trx, seq);
                return StartTransaction(trx, machine, command);
            });
        }
        /*
            Same as StartTransaction*S*, but single command.
        */
        private Task<string> StartTransaction(IGammaClientTransaction trx, IMachineInfo machine, string command)
        {
            trx.TransactionStarted();
            var task = ProxyFacadeCmdExecutor.ExecuteCmdAsync(machine, command);
            BindCmdTaskWithTranscation(task, trx);
            return task;
        }
        #endregion

        #region QATools_TRX
        /*
          QA tools transactions
        */
        public Task<string> StartQAToolsTransaction(IMachineInfo machine, GammaTXQATools tool, object qatoolArgs = null)
        {
            return Task.Run(() =>
            {
                var trx = GammaClientTXFactory.GetQAToolsTX(machine, tool);
                AddTransaction(trx);
                return StartTransactions(trx, machine, tool, qatoolArgs);

            });
        }
        
        private Task<string> StartTransactions(IGammaClientTransaction trx, IMachineInfo machine, GammaTXQATools tool, object qatoolArgs = null)
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
        #endregion
    }

}
