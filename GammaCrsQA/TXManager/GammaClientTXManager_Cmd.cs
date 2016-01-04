using GammaCrsQAInstaller.NetworkManager;
using GammaCrsQAInstaller.WcfFacecade;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;


namespace GammaCrsQAInstaller.TXManager
{
    partial interface IGammaClientTXManager
    {
        Task<string> StartCmdInSeqTransaction(IGammaMachineInfo machine, string[] commands);
        List<Task<string>> StartCmdInParallelTransaction(IGammaMachineInfo machine, string[] commands);
    }
    partial class GammaClientTXManager: IGammaClientTXManager
    {
        /*
            Run Commands on machine in sequence, one after another.
            And the result will be returned when all of them are finished.
        */
        public Task<string> StartCmdInSeqTransaction(IGammaMachineInfo machine, string[] commands)
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
        private Task<string> StartTransactions(IGammaClientTransaction trx, IGammaMachineInfo machine, string[] commands)
        {
            trx.TransactionStarted();
            var task = GammaCmdExecutorServiceFacade.ExecuteCmdsSeqByNodeAsync(machine, commands);
            BindCmdTaskWithTranscation(task, trx);
            return task;
        }
        /*
           if the command is being run as parallel, all the nodes' transaction will share the same seq#,
           so that later we can notice that they are submitted as parallel run on all nodes.
       */
        public List<Task<string>> StartCmdInParallelTransaction(IGammaMachineInfo machine, string[] commands)
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
        private Task<string> StartCmdGroupTransaction(IGammaMachineInfo machine, string command)
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
        private Task<string> StartTransaction(IGammaClientTransaction trx, IGammaMachineInfo machine, string command)
        {
            trx.TransactionStarted();
            var task = GammaCmdExecutorServiceFacade.ExecuteCmdAsync(machine, command);
            BindCmdTaskWithTranscation(task, trx);
            return task;
        }

    }
}
