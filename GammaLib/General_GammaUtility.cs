using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.ServiceProcess;
using System.Text;

namespace GeneralUtility
{
    
    public class GammaUtility
    {
        
        public static string StopService(string serviceName, int timeoutMilliseconds)
        {
            ServiceController service = new ServiceController(serviceName);
            try
            {
                TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);
                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
            }
            catch (Exception ex)
            {
                return string.Format("Hit exception when stopping service {0}: {1}", serviceName, ex.Message);
            }
            return string.Format("Serivce {0} stopped", serviceName);
        }

        public static string StartService(string serviceName, int timeoutMilliseconds)
        {
            ServiceController service = new ServiceController(serviceName);
            try
            {
                TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, timeout);
            }
            catch (Exception ex)
            {
                return string.Format("Hit exception when starting service {0}: {1}", serviceName, ex.Message);
            }
            return string.Format("Serivce {0} started", serviceName);
        }

        public static string ShellExecutor(string filename, string arguments, string working_dir="",string usr = "", SecureString pwd = null, string domain = "")
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
                return (string.Format("OS error when executing: {0} : {1} {2}",  Format(filename, arguments), e.Message, e));
            }

            if (process.ExitCode == 0)
            {
                if (string.IsNullOrEmpty(stdOutput.ToString().Trim()))
                {
                    return string.Format("Execution succeed, no output. {0} {1}", filename, arguments);
                }
                else
                {
                    return string.Format("Execution succeed, {0}", stdOutput.ToString());
                }

            }
            else
            {
                var message = new StringBuilder();
                message.AppendFormat(@"{0}  finished with error/exit code = {1} ", Format(filename, arguments), process.ExitCode);
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

        private static string Format(string filename, string arguments)
        {
            return "'" + filename +
                ((string.IsNullOrEmpty(arguments)) ? string.Empty : " " + arguments) +
                "'";
        }

        public static string SecureStringToString(SecureString value)
        {
            IntPtr bstr = Marshal.SecureStringToBSTR(value);

            try
            {
                return Marshal.PtrToStringBSTR(bstr);
            }
            finally
            {
                Marshal.FreeBSTR(bstr);
            }
        }

        public static SecureString StringToSecureString(string value)
        {
            if (value == null || value == string.Empty)
            {
                return null;
            }
            var secure = new SecureString();
            foreach (char c in value.ToCharArray())
            {
                secure.AppendChar(c);
            }
            return secure;
        }
    }

    public class GammaRegKeyUtility
    {
        //Remove the last portion of regKey From HKLM
        public static string HKLM_RegKey_RM_LastElementTree(string regKey)
        {
            StringBuilder sb = new StringBuilder();
            string upperPath = RegKey_Get_UpperPath(regKey);
            string removeItem = regKey.Replace(upperPath, string.Empty);
            using (RegistryKey rsk = Registry.LocalMachine.OpenSubKey(upperPath, true))
            {
                try
                {
                    rsk.DeleteSubKeyTree(removeItem, true);
                    sb.AppendLine(regKey + " is removed from HKLM.");
                }
                catch (Exception ex)
                {
                    sb.AppendLine(regKey + " removed fails from HKLM " + ex.Message);
                }
            }
            return sb.ToString();
        }
        //Remove any registry key contains "searcher" From HKLM
        public static string HKLM_RegKey_RM_By_String(string regBaseKey, string searcher = "")
        {
            StringBuilder sb = new StringBuilder();
            using (RegistryKey rsk = Registry.LocalMachine.OpenSubKey(regBaseKey, true))
            {
                if (rsk != null)
                {
                    //MessageBox.Show("rsk is not null");
                    foreach (string skName in rsk.GetSubKeyNames())
                    {
                        //sc.Post((obj) => this.StatusRTB.AppendText(string.Format("HKLM\\{0}\\{1} is scanned\n", regBaseKey, skName)), null);
                        if (skName.Contains(searcher))
                        {
                            try
                            {
                                //sc.Post((obj) => this.StatusRTB.AppendText(string.Format("HKLM\\{0}\\{1} is found\n", regBaseKey, skName)), null);
                                rsk.DeleteSubKeyTree(skName, true);
                                sb.AppendLine(skName + " is removed from " + regBaseKey);
                            }
                            catch (Exception ex)
                            {
                                sb.AppendLine(skName + " is removed fails " + regBaseKey + "execption: " + ex.Message);
                            }
                        }
                    }
                }
            }
            return sb.ToString();
        }

        public static string RegKey_Get_UpperPath(string regRelatedPath)
        {
            int tmp = regRelatedPath.Count();
            string[] rs = regRelatedPath.Split('\\');
            string upperPath = "";
            for (int i = 0; i < rs.Count() - 1; i++)
            {
                upperPath += rs[i];
                upperPath += "\\";
            }
            return upperPath;
        }
    }

    public class GammaFSUtility
    {

        public static string DeleteDirectory(string target_dir)
        {
            StringBuilder sb = new StringBuilder();
            string[] files = Directory.GetFiles(target_dir);
            string[] dirs = Directory.GetDirectories(target_dir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                try
                { 
                    File.Delete(file);
                }
                catch(Exception ex)
                {
                    sb.AppendLine(file + "removed fails, " + ex.Message);
                }
            }

            foreach (string dir in dirs)
            {
                DirectoryInfo dirInfo = new DirectoryInfo(dir);
                if(dirInfo.Attributes == FileAttributes.ReparsePoint)
                {
                    dirInfo.Attributes = FileAttributes.Normal;
                }
                DeleteDirectory(dir);
            }
            try
            {
                Directory.Delete(target_dir, false);
            }catch(Exception ex)
            {
                sb.AppendLine(target_dir + "removed fails, " + ex.Message);
            }
            return sb.ToString().Equals(string.Empty) ? string.Format("{0} is removed", target_dir) : sb.ToString();
        }

        public static string NetUse(string hostname, string domain, string username, string password, char drive = 'c')
        {
            var net_path = string.Format(@"\\{0}\{1}$", hostname, drive);
            var domain_user = string.Format(@"{0}\{1}", domain, username);
            return GammaUtility.ShellExecutor("net.exe", string.Format(@"use {0} {1} /USER:{2}", net_path, password, domain_user));
        }

        public static string ConvertPathToNetPath(string hostname, string source)
        {
            var network_path = source.Trim().Replace(':', '$');
            return string.Format(@"\\{0}\{1}", hostname, network_path);
        }

        public static string RemoteCopyFiles(string hostname, string source, string target)
        {
            string source_fixed;
            string target_fixed;
            //XCopy directory does not allow the path ends with slash.
            if (source.EndsWith(@"\"))
            {
                source_fixed = source.Substring(0, source.Length - 1);
            } else
            {
                source_fixed = source;
            }
            if (target.EndsWith(@"\"))
            {
                target_fixed = target.Substring(0, target.Length - 1);
            }
            else
            {
                target_fixed = target;
            }
            return GammaUtility.ShellExecutor(@"C:\windows\system32\xcopy.exe", string.Format(@"/E /I /Y /V ""{0}"" ""{1}""", source_fixed, ConvertPathToNetPath(hostname, target_fixed)),@"C:\Windows\System32");
        }
    }
}
