using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GammaServiceLib.OraCRS
{
    public partial class CrsEnv : ICrsEnv
    {
        private static string srvctl = "srvctl.bat";
        public static string GetCurrentCrsHome()
        {
            var homes = GetOraHomeInventory((str) =>
            {
                if (str.Contains(@"CRS=""true"""))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            });
            if (homes.Count() != 1)
            {
                throw new Exception("No CRSHome found or Get more than one Active CRS HOME");
            }
            else
            {
                return homes.First();
            }
        }

        public static IEnumerable<string> GetRACHome()
        {
            var homes = GetOraHomeInventory((str) => {
                if (str.Contains(@"CRS=""true"""))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            });
            return homes;
        }

        public string GetDBHOMEByName(string dbname)
        {
            try
            {
                var crs = GetCurrentCrsHome();
                var cmd = Path.Combine(crs, "bin", srvctl);
                var args = string.Format("config database -db {0}", dbname);
                var pattern = @"^Oracle home:(.*)$";
                var cmd_rs = GeneralUtility.PureCmdExec.PureCmdExector(cmd, args);
                Match match = Regex.Match(cmd_rs, pattern, RegexOptions.Multiline);
                if (match.Groups.Count > 1)
                {
                    return match.Groups[1].Value.Trim();
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string GetDBNames()
        {
            try
            {
                var crs = GetCurrentCrsHome();
                var cmd = Path.Combine(crs, "bin", srvctl);
                var args = string.Format("config database");
                return GeneralUtility.PureCmdExec.PureCmdExector(cmd, args);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

        public string GetClusterNames()
        {
            try
            {
                var crs = GetCurrentCrsHome();
                var cmd = Path.Combine(crs, "bin", "cemutlo");
                var args = string.Format("-n");
                return GeneralUtility.PureCmdExec.PureCmdExector(cmd, args);
            }catch(Exception ex)
            {
                return ex.Message;
            }
            
        }

        public string GetScan()
        {
            try
            {
                var crs = GetCurrentCrsHome();
                var cmd = Path.Combine(crs, "bin", "srvctl");
                var args = string.Format("config scan");
                var rs = GeneralUtility.PureCmdExec.PureCmdExector(cmd, args);
                var pattern = @"^SCAN name:(.*?),";
                Match match = Regex.Match(rs, pattern, RegexOptions.Multiline);
                if (match.Groups.Count > 1)
                {
                    return match.Groups[1].Value.Trim();
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

        public string GetScanPort()
        {
            try
            {
                var crs = GetCurrentCrsHome();
                var cmd = Path.Combine(crs, "bin", "srvctl");
                var args = string.Format("config scan_listener");
                var rs = GeneralUtility.PureCmdExec.PureCmdExector(cmd, args);
                var pattern = @".*?Port: TCP:(.*?)$";
                Match match = Regex.Match(rs, pattern, RegexOptions.Multiline);
                if (match.Groups.Count > 1)
                {
                    return match.Groups[1].Value.Trim();
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }
    }
}
