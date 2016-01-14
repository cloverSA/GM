using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;


//Manager transaction submitted from UI.
//1. Create and return the task for the transaction.
//2. Each task being returned should bind to a transaction.
//3. Task will only bind to the transaction but not mamanged here.
//4. Manage the Update of UI in event OnResultComesBack, 
//   the UI who whish to received the result of task generated here should register to it.

namespace GammaCrsQAInstaller.TXManager
{
    partial interface IGammaClientTXManager
    {
        // register to this event if the ui would need the result
        event EventHandler<GammaUIUpdateArgs> OnResultComesBack;
    }

    partial class GammaClientTXManager : IGammaClientTXManager
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
        /*
        private void ResultComesBack(IGammaClientTransaction transaction)
        {
            var handler = OnResultComesBack;
            if (handler != null)
            {
                Application.Current.Dispatcher.BeginInvoke(
                    DispatcherPriority.Normal, new Action(() =>
                    {
                        if (handler != null)
                        {
                            handler(this, new GammaUIUpdateArgs() { TRANSACTION = transaction });
                        }
                    }));
            }
            TransactionExpired(transaction);
        }
        */
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
    }

    class GammaUIUpdateArgs
    {
        public IGammaClientTransactionBase TRANSACTION
        {
            get; set;
        }
    }

    class GammaClientTXManagerFactory
    {
        public static GammaClientTXManager GetGammaClientTXManager()
        {
            return GammaClientTXManager.GetInstance();
        }
    }
}
