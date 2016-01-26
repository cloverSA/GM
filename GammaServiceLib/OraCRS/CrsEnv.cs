using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace GammaServiceLib.OraCRS
{
    public partial class CrsEnv
    {
        private const string oraRegBase = @"SOFTWARE\ORACLE";

        private const string inventoryRegistry = "inst_loc";

        private static string[] ora_regkey_keywords = new string[]{ "ORADB", "ORAGI" };

        public static IEnumerable<string> GetOracleRegProperty(string property, Func<string, bool> subkey_filter, bool distinc_value = false)
        {
            var oraProperty = new List<string>();

            using (RegistryKey rsk = Registry.LocalMachine.OpenSubKey(oraRegBase))
            {
                foreach (string tmp in rsk.GetSubKeyNames().Where(subkey_filter))
                {
                    using (RegistryKey homeKey = Registry.LocalMachine.OpenSubKey(oraRegBase + @"\" + tmp))
                    {
                        if (homeKey.GetValue(property) != null)
                        {
                            string result = homeKey.GetValue(property).ToString().ToLower().Trim();
                            oraProperty.Add(result);
                        }
                    }

                }
            }
            if (distinc_value)
            {
                return oraProperty.Distinct();
            }
            else
            {
                return oraProperty;
            }
            
        }

        public static IEnumerable<string> GetOracleRegProperty(string property, string[] home_keys, bool distinc_value = false)
        {
            //str is the elem in IEnumberable<string> that who will uses these function in its Where statement.
            Func<string, bool> subkey_filter = (str) => {
                var tmp = from key in home_keys where str.ToUpper().Contains(key) select key;
                /*
                var tmp = home_keys.Where(
                    (s) => {
                        return s.Contains(str);
                    });
                */
                if (tmp.Count() > 0)
                {
                    return true;
                } else
                {
                    return false;
                }
                
            };

            return GetOracleRegProperty(property, subkey_filter, distinc_value);

            
        }
        
        public static IEnumerable<string> GetOracleRegProperty(string property, bool distinc_value = false)
        {
            return GetOracleRegProperty(property, ora_regkey_keywords, distinc_value);
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
            string output = crsctl.StandardOutput.ReadToEnd().Trim();
            crsctl.WaitForExit();
            crsctl.Close();
            crsctl.Dispose();
            return output;
        }

        public static IEnumerable<string> GetInActiveOraHome()
        {
            var reg_homes = GetOracleRegProperty("ORACLE_HOME", true);
            var inv_homes = GetOraHomeInventory();
            return reg_homes.Except(inv_homes);

        }

        public static IEnumerable<string> GetOraHomeInventory()
        {
            var result = new List<string>();
            string pattern = @"<HOME.*?LOC=""(.*?)"".*>";
            var inventory = GetInventoryLoc();
            var inf = Path.Combine(inventory, @"ContentsXML\inventory.xml");
            string[] lines = File.ReadAllLines(inf);
            foreach (var line in lines)
            {
                foreach (Match match in Regex.Matches(line, pattern))
                {
                    result.Add(match.Groups[1].Value);
                }
            }

            return result;
        }

        public static IEnumerable<string> GetOraHomeInventory(Func<string, bool> content_filter)
        {
            var result = new List<string>();
            string pattern = @"<HOME.*?LOC=""(.*?)"".*>";
            var inventory = GetInventoryLoc();
            var inf = Path.Combine(inventory, @"ContentsXML\inventory.xml");
            string[] lines = File.ReadAllLines(inf);
            foreach (var line in lines.Where(content_filter))
            {
                foreach (Match match in Regex.Matches(line, pattern))
                {
                    result.Add(match.Groups[1].Value);
                }
            }

            return result;
        }
    }
}
