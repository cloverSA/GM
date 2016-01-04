using System;
using System.Text;

namespace GammaServiceLib
{
    public class CmdExecutor : ICmdExecutor
    {
        private const string TX_RESULT_FAIL = "[GAMMA_ERROR]";
        private const string TX_RESULT_SUC = "[GAMMA_SUC]";
        public string ShellExecutor(string filename, string arguments)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();

            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
    
            startInfo.FileName = filename;
            startInfo.Arguments = arguments;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;

            var stdOutput = new StringBuilder();
            var stdErrorOutput = new StringBuilder();
            process.StartInfo = startInfo;
            process.OutputDataReceived += (sender, args) => stdOutput.Append(args.Data + Environment.NewLine);
            process.ErrorDataReceived += (sender, args) => stdErrorOutput.Append(args.Data + Environment.NewLine);

            try
            {
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("{0} OS error when executing: {1} : {2} {3}", TX_RESULT_FAIL, Format(filename, arguments), e.Message, e));
            }

            if (process.ExitCode == 0)
            {
                if (string.IsNullOrEmpty(stdOutput.ToString().Trim()))
                {
                    return string.Format("{0} Execution succeed, no output.", TX_RESULT_SUC);
                }
                else
                {
                    return string.Format("{0} Execution succeed, {1} {2}", TX_RESULT_SUC, Environment.NewLine, stdOutput.ToString());
                }
                
            }
            else
            {
                var message = new StringBuilder();
                message.AppendFormat("{0} {1}  finished with exit code = {2} ", TX_RESULT_FAIL, Format(filename, arguments), process.ExitCode);
                message.AppendLine();
                if (!string.IsNullOrEmpty(stdErrorOutput.ToString().Trim()))
                {
                    message.AppendLine("Std error output:");
                    message.AppendLine(stdErrorOutput.ToString());
                }

                if (stdOutput.Length != 0)
                {
                    message.AppendLine("Std output:");
                    message.AppendLine(stdOutput.ToString());
                }

                return message.ToString();
            }

        }

        private System.Security.SecureString ConvertToSecureString(string strPassword)
        {
            var secureStr = new System.Security.SecureString();
            if (strPassword.Length > 0)
            {
                foreach (var c in strPassword.ToCharArray()) secureStr.AppendChar(c);
            }
            return secureStr;
        }

        private string Format(string filename, string arguments)
        {
            return "'" + filename +
                ((string.IsNullOrEmpty(arguments)) ? string.Empty : " " + arguments) +
                "'";
        }
    }

}
