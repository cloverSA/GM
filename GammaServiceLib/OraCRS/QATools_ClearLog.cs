using System;
using System.Collections.Generic;
using System.IO;

//GammaLib
using GeneralUtility;
using System.IO.Compression;
using System.Text;
using System.Reflection;
//
namespace GammaServiceLib.OraCRS
{

    public partial class QATools : IQATools
    {
        private const string TX_RESULT_FAIL = "[GAMMA_ERROR]";
        private const string TX_RESULT_SUC = "[GAMMA_SUC]";

        public string ClearLog()
        {
            var orabases = CrsEnv.GetOracleRegProperty("ORACLE_BASE", true);
            var orahome = CrsEnv.GetOracleRegProperty("ORACLE_HOME", true);
            StringBuilder sb = new StringBuilder();


            foreach (string orabase in orabases)
            {
                if (Directory.Exists(orabase))
                {
                    string diag = System.IO.Path.Combine(orabase, "diag");
                    string crsdata = System.IO.Path.Combine(orabase, "crsdata");
                    string cfgtool = System.IO.Path.Combine(orabase, "cfgtoollogs");
                    if (Directory.Exists(diag))
                    {
                        sb.AppendLine("cleaning dir " + diag);
                        sb.AppendLine(ClearLogFromDir(diag, true));
                    }

                    if (Directory.Exists(crsdata))
                    {
                        sb.AppendLine("cleaning dir " + crsdata);
                        sb.AppendLine(ClearLogFromDir(crsdata, true));
                    }

                    if (Directory.Exists(cfgtool))
                    {
                        sb.AppendLine("cleaning dir " + cfgtool);
                        sb.AppendLine(ClearLogFromDir(cfgtool, true));
                    }

                }

                foreach (string home in orahome)
                {
                    if (Directory.Exists(home))
                    {
                        string log = System.IO.Path.Combine(home, "log");
                        if (Directory.Exists(log))
                        {
                            sb.AppendLine("cleaning dir " + log);
                            sb.AppendLine(ClearLogFromDir(log, true));
                        }
                            
                    }

                }

            }

            return sb.ToString();
        }

        private string ClearLogFromDir(string targetDir, bool recursive = true)
        {
            StringBuilder sb = new StringBuilder();
            System.IO.SearchOption t;
            if (recursive)
            {
                t = System.IO.SearchOption.AllDirectories;
            }
            else
            {
                t = System.IO.SearchOption.TopDirectoryOnly;
            }
            RemoveFiles(Directory.EnumerateFiles(targetDir, "*.tr*", t));
            RemoveFiles(Directory.EnumerateFiles(targetDir, "*.log", t));
            RemoveFiles(Directory.EnumerateDirectories(targetDir, "cdmp_*", t));
            sb.AppendLine("finished!");
            return sb.ToString();

        }

        private void RemoveFiles(IEnumerable<string> files)
        {
            foreach (string file in files)
            {
                try
                {
                    File.Delete(file.Trim());
                }
                catch (Exception ex)
                {
                    //do nothing.
                }
                
            }
            
        }


    }


}
