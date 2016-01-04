using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace GammaServiceLib
{
    class CrsEnv
    {
        private const string oraRegBase = @"SOFTWARE\ORACLE";

        private const string inventoryRegistry = "inst_loc";

        public static List<string> GetOracleRegProperty(string property, bool distinc_value = false)
        {
            List<String> oraProperty = new List<String>();

            using (RegistryKey rsk = Registry.LocalMachine.OpenSubKey(oraRegBase))
            {
                foreach (String tmp in rsk.GetSubKeyNames())
                {
                    if (tmp.ToUpper().Contains("ORADB") || tmp.ToUpper().Contains("ORAGI"))
                    {
                        using (RegistryKey homeKey = Registry.LocalMachine.OpenSubKey(oraRegBase + @"\" + tmp))
                        {
                            if (homeKey.GetValue(property) != null)
                            {
                                String result = homeKey.GetValue(property).ToString().ToLower().Trim();
                                if (distinc_value)
                                {
                                    if (!oraProperty.Contains(result))
                                    {
                                        oraProperty.Add(result);
                                    }
                                }
                                else
                                {
                                    oraProperty.Add(result);
                                }
                            }
                        }
                    }
                }
            }
            return oraProperty;
        }

        public static string GetInventoryLoc()
        {
            string loc = string.Empty;
            using (RegistryKey rsk = Registry.LocalMachine.OpenSubKey(oraRegBase))
            {
                if (rsk.GetValue(inventoryRegistry) != null)
                {
                    loc = rsk.GetValue(inventoryRegistry).ToString();
                }
            }
            return loc;
        }

        public static string CrsStatInfo()
        {
            Process crsctl = new Process();
            crsctl.StartInfo.UseShellExecute = false;
            crsctl.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            crsctl.StartInfo.RedirectStandardOutput = true;
            crsctl.StartInfo.RedirectStandardInput = true;
            crsctl.StartInfo.FileName = "cmd ";
            crsctl.StartInfo.Arguments = "";
            crsctl.Start();
            crsctl.StandardInput.WriteLine("crsctl stat server");
            crsctl.StandardInput.WriteLine("echo =================================");
            crsctl.StandardInput.WriteLine("crsctl stat res -t");
            crsctl.StandardInput.WriteLine("echo =================================");
            crsctl.StandardInput.WriteLine("crsctl stat res -t -init");
            crsctl.StandardInput.WriteLine("echo =================================");
            crsctl.StandardInput.WriteLine("crsctl stat res -f");
            crsctl.StandardInput.WriteLine("echo =================================");
            crsctl.StandardInput.WriteLine("crsctl stat res -init -f");
            crsctl.StandardInput.WriteLine("echo =================================");
            crsctl.StandardInput.WriteLine("exit");
            String output = crsctl.StandardOutput.ReadToEnd().Trim();
            crsctl.WaitForExit();
            crsctl.Close();
            crsctl.Dispose();
            return output;
        }

    }
}
