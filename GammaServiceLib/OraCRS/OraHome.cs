using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GammaServiceLib.OraCRS
{
    partial class CrsEnv
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
                throw new Exception("CrsEnv Get more than one Active CRS HOME");
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

        public static string GetDBHOMEByName(string dbname)
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

        public static string GetDBNames()
        {
            var crs = GetCurrentCrsHome();
            var cmd = Path.Combine(crs, "bin", srvctl);
            var args = string.Format("config database");
            return GeneralUtility.PureCmdExec.PureCmdExector(cmd, args);
        }

        public static string GetClusterNames()
        {
            var crs = GetCurrentCrsHome();
            var cmd = Path.Combine(crs, "bin", "cemutlo");
            var args = string.Format("-n");
            return GeneralUtility.PureCmdExec.PureCmdExector(cmd, args);
        }
    }
}
