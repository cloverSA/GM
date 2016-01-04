using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;
using System.IO;
using System.Text;
//GammaLib
using GeneralUtility;


namespace GammaServiceLib.OraCRS
{

    public class CrsCleaner : ICrsCleaner
    {
        private const string TX_RESULT_FAIL = "[GAMMA_ERROR]";
        private const string TX_RESULT_SUC = "[GAMMA_SUC]";
        public string RemoveOraKeys()
        {
            StringBuilder sb = new StringBuilder();
            string[] orakeys = new string[]
            {
                @"SOFTWARE\ORACLE",
                @"SOFTWARE\odbc\odbcinst.ini\Ora*",
                @"SYSTEM\CurrentControlSet\Services\Ora*",
                @"SYSTEM\CurrentControlSet\Services\ocfs",
                @"SYSTEM\CurrentControlSet\Services\Eventlog\Application\Ora*",
                @"SYSTEM\CurrentControlSet\Services\Eventlog\System\Ora*",
                @"SYSTEM\ControlSet001\Services\Ora*",
                @"SYSTEM\ControlSet001\Services\ocfs",
                @"SYSTEM\ControlSet001\Services\Eventlog\Application\Ora*",
                @"SYSTEM\ControlSet001\Services\Eventlog\System\Ora*",
                @"SYSTEM\ControlSet002\Services\Ora*",
                @"SYSTEM\ControlSet002\Services\ocfs",
                @"SYSTEM\ControlSet002\Services\Eventlog\Application\Ora*",
                @"SYSTEM\ControlSet002\Services\Eventlog\System\Ora*"

            };
            //this.StatusRTB.Document.Blocks.Clear();

            foreach (string regKey in orakeys)
            {
                if (regKey.Contains('*'))
                {
                    string regLoop = regKey.Replace("Ora*", "").Trim();
                    sb.AppendLine(GammaRegKeyUtility.HKLM_RegKey_RM_By_String(regLoop, "Ora"));
                }
                else
                {
                    sb.AppendLine(GammaRegKeyUtility.HKLM_RegKey_RM_LastElementTree(regKey));
                }
                
            }


            foreach (string subkey in Registry.ClassesRoot.GetSubKeyNames())
            {
                if (subkey.ToLower().StartsWith("ora") || subkey.StartsWith("ORCL"))
                {
                    try
                    {
                        Registry.ClassesRoot.DeleteSubKeyTree(subkey);
                        sb.AppendLine(subkey + " is removed from HK_CROOT");
                    }
                    catch (Exception ex)
                    {
                        sb.AppendLine(subkey + " removed fails from HK_CROOT " + ex.Message);
                    }

                }
            }
            ClearEnvironmentVarPath();
            return sb.ToString();
        }

        public string RmOraGroup()
        {
            System.DirectoryServices.DirectoryEntry entryPC = new System.DirectoryServices.DirectoryEntry();
            entryPC.Path = string.Format("WinNT://{0}", System.Environment.MachineName);

            //int totalCount = entryPC.Children.OfType<System.DirectoryServices.DirectoryEntry>().ToList().Count();
            StringBuilder sb = new StringBuilder();
            foreach (System.DirectoryServices.DirectoryEntry child in entryPC.Children)
            {
                if (child.SchemaClassName == "Group")
                {
                    //sc.Post((obj) => this.StatusRTB.AppendText(string.Format("Checking Group: {0}\n", child.Name)), null);
                    if (child.Name.Trim().StartsWith("ORA_") || child.Name.Trim().StartsWith("ora_"))
                    {
                        entryPC.Children.Remove(child);
                        sb.AppendLine("Group" + child.Name.Trim() + "is removed");
                    }
                }

            }
            return sb.ToString();
        }

        public string RemoveOraFiles()
        {
            string temp_loc = null;
            temp_loc = Environment.GetEnvironmentVariable("temp");

            List<string> orafiles = new List<string>();
            orafiles.Add(@"C:\app");
            orafiles.Add(@"C:\oracle");
            orafiles.Add(string.Format(@"{0}\oracle", Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)));
            orafiles.Add(string.Format(@"{0}\Temp", Environment.GetFolderPath(Environment.SpecialFolder.Windows)));
            orafiles.Add(string.Format(@"{0}\system32\drivers\ocfs.sys", Environment.GetFolderPath(Environment.SpecialFolder.Windows)));
            orafiles.Add(string.Format(@"{0}\system32\drivers\oracleacfs.sys", Environment.GetFolderPath(Environment.SpecialFolder.Windows)));
            orafiles.Add(string.Format(@"{0}\system32\drivers\oracleadvm.sys", Environment.GetFolderPath(Environment.SpecialFolder.Windows)));
            orafiles.Add(string.Format(@"{0}\system32\drivers\oracleoks.sys", Environment.GetFolderPath(Environment.SpecialFolder.Windows)));
            orafiles.Add(string.Format(@"{0}\system32\drivers\orafenceservice.sys", Environment.GetFolderPath(Environment.SpecialFolder.Windows)));
            orafiles.Add(string.Format(@"{0}\system32\drivers\orafencedrv.sys", Environment.GetFolderPath(Environment.SpecialFolder.Windows)));

            if (temp_loc != null)
            {
                orafiles.Add(temp_loc);
            }
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(GammaUtility.StopService("MSDTC", 1000));
            sb.AppendLine(GammaUtility.StopService("OracleRemExecServiceV2", 1000));
            
            foreach (string f in orafiles)
            {
                try
                {
                    if (File.Exists(f))
                    {
                        File.Delete(f);
                        sb.AppendLine(f + " is removed.");
                    }

                    if (Directory.Exists(f))
                    {
                        sb.AppendLine(GammaFSUtility.DeleteDirectory(f));
                    }
                }
                catch (Exception ex)
                {
                    sb.AppendLine(f + " removed fails, " + ex.Message);
                }

            }

            sb.AppendLine(GammaUtility.StartService("MSDTC", 1000));
            return sb.ToString();
        }

        public string CleanDisk()
        {
            DiskPartUtility dpu = new DiskPartUtility();
            return dpu.DiskPart_ReCreate();
        }

        public string RemoveDrvLtr()
        {
            DiskPartUtility dpu = new DiskPartUtility();
            return dpu.DiskPart_RmDrvLtr();

        }

        public void RestartComputer()
        {
            GammaUtility.ShellExecutor("shutdown.exe", "-r -t 0");
        }

        private void ClearEnvironmentVarPath()
        {
            string clean_token = @"c:\app";
            string pathVar = null;
            string[] pathValues = null;
            string pathFinal = "";
            bool changed = false;
            pathVar = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Machine);
            if (pathVar != null)
            {
                pathValues = pathVar.Split(';');
            }
            foreach (string tmp in pathValues)
            {
                if (!tmp.Trim().ToLower().Contains(clean_token))
                {
                    pathFinal += tmp.Trim();
                    pathFinal += ";";
                }
                else
                {
                    changed = true;
                }
            }

            if (changed)
            {
                Environment.SetEnvironmentVariable("PATH", pathFinal, EnvironmentVariableTarget.Machine);
                
            }


            pathVar = Environment.GetEnvironmentVariable("PERL5LIB", EnvironmentVariableTarget.Machine);
            if (pathVar != null)
            {
                Environment.SetEnvironmentVariable("PERL5LIB", "", EnvironmentVariableTarget.Machine);
            }
        }

    }
}
