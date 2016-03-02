using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;

namespace ASMSreviceLib.OraASM
{
    public class ASMCMDExecutor
    {
        public static string ASMCMDExector(string filename, string arguments, string crs_home = "", string working_dir = "", string usr = "", SecureString pwd = null, string domain = "")
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();

            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.ErrorDialog = true;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;

            startInfo.FileName = filename;
            startInfo.Arguments = arguments;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            if (working_dir != "")
            {
                startInfo.WorkingDirectory = working_dir;
            }
            if (usr != "")
            {
                startInfo.UserName = usr;
            }
            if (pwd != null)
            {
                startInfo.Password = pwd;
            }
            if (domain != "")
            {
                startInfo.Domain = domain;
            }
            if (crs_home != "")
            {
                startInfo.EnvironmentVariables["ORACLE_HOME"] = crs_home;
            }
            var stdOutput = new StringBuilder();
            var stdErrorOutput = new StringBuilder();
            process.StartInfo = startInfo;
            process.OutputDataReceived += (sender, args) => stdOutput.Append(args.Data + Environment.NewLine);
            process.ErrorDataReceived += (sender, args) => stdErrorOutput.Append(args.Data + Environment.NewLine);

            try
            {
                process.EnableRaisingEvents = true;

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();
            }
            catch (Exception e)
            {
                return (string.Format("OS error when executing: {0} {1} : {2} {3}", filename, arguments, e.Message, e));
            }

            if (process.ExitCode == 0)
            {
                if (string.IsNullOrEmpty(stdOutput.ToString().Trim()))
                {
                    return string.Empty;
                }
                else
                {
                    return stdOutput.ToString().Trim();
                }

            }
            else
            {
                var message = new StringBuilder();
                if (!string.IsNullOrEmpty(stdErrorOutput.ToString().Trim()))
                {
                    message.AppendLine(stdErrorOutput.ToString());
                }

                if (stdOutput.Length != 0)
                {
                    message.AppendLine(stdOutput.ToString());
                }

                return message.ToString();
            }

        }
    }
}
