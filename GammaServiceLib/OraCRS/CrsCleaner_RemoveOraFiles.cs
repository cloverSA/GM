using GeneralUtility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GammaServiceLib.OraCRS
{
    public partial class CrsCleaner : ICrsCleaner
    {
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
    }
}
