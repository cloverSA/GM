using GammaCrsQAInstaller.NetworkManager;
using GammaCrsQAInstaller.WcfProxy;
using System.Threading.Tasks;

namespace GammaCrsQAInstaller.TXManager
{
    partial class GammaClientTXManager : IGammaClientTXManager
    {
        /*
           Clear environment transactions
       */
        public Task<string> StartClearEnvTransaction(IGammaMachineInfo machine, GammaTXClearEnv clear_step)
        {
            return Task.Run(() =>
            {
                var trx = GammaClientTXFactory.GetClearEnvTX(machine, clear_step);
                AddTransaction(trx);
                return StartTransactions(trx, machine, clear_step);

            });
        }

        /*
            return the task based on the enum code.
            and also bind the task with the transaction here.
        */
        private Task<string> StartTransactions(IGammaClientTransaction trx, IGammaMachineInfo machine, GammaTXClearEnv clear_step)
        {
            trx.TransactionStarted();
            Task<string> task;
            if(clear_step != GammaTXClearEnv.REBOOT)
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
                task = tmp.ContinueWith<string>((t)=> {
                    trx.TransactionCompleted();
                    return TX_RESULT_SUC;
                });
            }
            
            return task;
        }
    }
}
