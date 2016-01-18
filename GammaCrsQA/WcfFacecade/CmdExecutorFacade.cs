using GammaCrsQA.NetworkManager;
using GammaCrsQA.WcfProxy;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GammaCrsQA.WcfFacecade
{
    class GammaCmdExecutorServiceFacade
    {
        private static string GetFileNameFromCmd(string single_command)
        {
            string[] rs = single_command.Trim().Split((char[]) null, StringSplitOptions.RemoveEmptyEntries);
            if (rs.Length > 0)
            {
                return rs[0];
            }
            else
            {
                return string.Empty;
            }
        }

        private static string GetArgsFromCmd(string single_command)
        {
            string[] rs = single_command.Trim().Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
            if (rs.Length > 1)
            {
                return single_command.Replace(rs[0], string.Empty).Trim();
            }
            else
            {
                return string.Empty;
            }
        }

        public static string ExecuteCmdSync(IGammaMachineInfo machine, string single_command)
        {
            string filename = GetFileNameFromCmd(single_command);
            string args = GetArgsFromCmd(single_command);
            CmdExecutorProxy client = GammaProxyFactory.GetCmdExecutorProxy(machine);
            return client.ShellExecutor(filename, args);
        }

        public static Task<string> ExecuteCmdAsync(IGammaMachineInfo machine, string single_command)
        {
            string filename = GetFileNameFromCmd(single_command);
            string args = GetArgsFromCmd(single_command);
            CmdExecutorProxy client = GammaProxyFactory.GetCmdExecutorProxy(machine);
            return client.ShellExecutorAsync(filename, args);
        }

        public async static Task<string> ExecuteCmdsSeqByNodeAsync(IGammaMachineInfo machine, string[] multi_cmd)
        {
            if (multi_cmd.Count() == 1)
            {
                return ExecuteCmdSync(machine, multi_cmd[0]);
            }
            StringBuilder sb = new StringBuilder();
            foreach (string cmd in multi_cmd)
            {
                string tmp = await ExecuteCmdAsync(machine, cmd);
                sb.AppendFormat("cmd: {0}, result: {1}", cmd, tmp);
                sb.AppendLine();
            }
            return sb.ToString();
        }

        public static List<Task<string>> ExecuteCmdParallelOnNodesAsync(IEnumerable<IGammaMachineInfo> machines, string single_command)
        {
            List<Task<string>> tasks = new List<Task<string>>();
            foreach(IGammaMachineInfo machine in from m in machines where m.IsSelected && m.Alive == NodeState.Online select m)
            {
                tasks.Add(ExecuteCmdAsync(machine, single_command));

            }
            return tasks;
        }

    }
}
