using System;
using System.Collections.Generic;
using System.IO;

//GammaLib
using GeneralUtility;
using System.IO.Compression;
using System.Text;
//
namespace GammaServiceLib
{

    public class QATools : IQATools
    {
        private const string TX_RESULT_FAIL = "[GAMMA_ERROR]";
        private const string TX_RESULT_SUC = "[GAMMA_SUC]";
        public string ClearLog()
        {
            List<string> orabases = CrsEnv.GetOracleRegProperty("ORACLE_BASE", true);
            List<string> orahome = CrsEnv.GetOracleRegProperty("ORACLE_HOME", true);
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

        public string GetLog(bool collect_dump)
        {
            string inventory_log = System.IO.Path.Combine(CrsEnv.GetInventoryLoc(), "logs");
            string machine_time = Environment.MachineName + "_" + (DateTime.Now).ToString("yyyyMMddHHmmssffff");
            string log_base = @"C:\logbk\";
            string log_upload = @"C:\logbk\upload\";
            string log_dir = System.IO.Path.Combine(log_base, machine_time);
            string filename_base = "log_" + machine_time;
            string info_file = System.IO.Path.Combine(log_dir, string.Format("{0}_orahome_log_info.txt", filename_base));
            string stat_file = System.IO.Path.Combine(log_dir, string.Format("{0}_crs_stat.txt", filename_base));

            StringBuilder sb = new StringBuilder();
            if (!Directory.Exists(log_base))
            {
                Directory.CreateDirectory(log_base);
            }
            if (!Directory.Exists(log_dir))
            {
                Directory.CreateDirectory(log_dir);
            }
            if (!Directory.Exists(log_upload))
            {
                Directory.CreateDirectory(log_upload);
            }
            List<string> orabases = CrsEnv.GetOracleRegProperty("ORACLE_BASE", true);
            List<string> orahome = CrsEnv.GetOracleRegProperty("ORACLE_HOME", true);

            int orahome_counter = 1;
            string zip_log = string.Empty;

            using (FileStream fs = new FileStream(info_file, FileMode.Append))
            using (StreamWriter writer = new StreamWriter(fs))
            using (Zipper zipper = new Zipper(".zip", info_file))
            {

                if (Directory.Exists(inventory_log))
                {
                    sb.AppendLine("Collecting log from "+ inventory_log);
                    zipper.RunZip(inventory_log, System.IO.Path.Combine(log_dir, string.Format("{0}_inventory_log", Environment.MachineName)));
                }
                try { 
                    foreach (string orabase in orabases)
                    {

                        if (Directory.Exists(orabase))
                        {
                            sb.AppendLine("Collecting log from " + orabase);
                            DirectoryInfo dirInfo = new DirectoryInfo(orabase);
                            string diag = System.IO.Path.Combine(orabase, "diag");
                            string crsdata = System.IO.Path.Combine(orabase, "crsdata");
                            string cfgtool = System.IO.Path.Combine(orabase, "cfgtoollogs");
                            if (Directory.Exists(diag))
                                zipper.RunZip(diag, System.IO.Path.Combine(log_dir, string.Format("{0}_{1}_diag", filename_base, dirInfo.Name)));
                            if (Directory.Exists(crsdata))
                                zipper.RunZip(crsdata, System.IO.Path.Combine(log_dir, string.Format("{0}_{1}_crsdata", filename_base, dirInfo.Name)));
                            if (Directory.Exists(cfgtool))
                                zipper.RunZip(cfgtool, System.IO.Path.Combine(log_dir, string.Format("{0}_{1}_cfgtoollogs", filename_base, dirInfo.Name)));
                            dirInfo = null;
                        }

                    }
                }catch(Exception ex)
                {
                    sb.AppendFormat("Hit ex when zipping file in {0}, {1}", orabases.ToString(), ex.Message);
                    sb.AppendLine();
                }
                foreach (string home in orahome)
                {
                    if (Directory.Exists(home))
                    {
                        sb.AppendLine("Collecting log from " + home);
                        DirectoryInfo dirInfo = new DirectoryInfo(home);
                        //basename + version + seq#
                        if (dirInfo.Name.Contains("dbhome"))
                        {
                            zip_log = System.IO.Path.Combine(log_dir, string.Format("{0}_{1}_dbhome_seq_{2}_log", filename_base, dirInfo.Parent.Name, orahome_counter));
                        }
                        else if (dirInfo.Name.Contains("grid"))
                        {
                            zip_log = System.IO.Path.Combine(log_dir, string.Format("{0}_{1}_gihome_seq_{2}_log", filename_base, dirInfo.Name, orahome_counter));
                        }
                        else
                        {
                            zip_log = System.IO.Path.Combine(log_dir, string.Format("{0}_{1}_orahome_seq_{2}_log", filename_base, dirInfo.Parent.Name, orahome_counter));
                        }

                        string home_log = System.IO.Path.Combine(home, "log");
                        if (Directory.Exists(home_log))
                        {
                            zipper.RunZip(home_log, zip_log);
                        }
                           
                        dirInfo = null;

                        writer.WriteLine(string.Format("home: {0}, zip_log: {1}", home, zip_log));
                        orahome_counter += 1;
                    }
                }

            }

            try
            {
                if (collect_dump)
                {
                    sb.AppendLine("Collecting Memory Dump !");
                    using (ZipArchive zip = ZipFile.Open(System.IO.Path.Combine(log_dir, string.Format("{0}_MEMORY_DMP.zip", machine_time)), ZipArchiveMode.Create))
                    {
                        zip.CreateEntryFromFile(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "MEMORY.DMP"), "MEMORY.DMP");
                    }

                }
                ZipFile.CreateFromDirectory(log_dir, System.IO.Path.Combine(log_upload, string.Format("{0}_logs.zip", machine_time)));
                sb.AppendLine("logs are collected and zipped in "+ System.IO.Path.Combine(log_upload, string.Format("{0}_logs.zip", machine_time)));
                using (FileStream fs = new FileStream(stat_file, FileMode.Append))
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    string pathVar = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Machine);
                    Environment.SetEnvironmentVariable("PATH", pathVar);
                    writer.WriteLine(CrsEnv.CrsStatInfo());
                    writer.WriteLine((DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss.ffff"));
                }
            }
            catch (Exception ex)
            {
                sb.AppendLine(string.Format("{0} Operation fails when collecting logs, {1}", TX_RESULT_FAIL, ex.Message));
            }
            GC.Collect();
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
            RemoveFiles(Directory.EnumerateFiles(targetDir, "*.tr*", t), sb);
            RemoveFiles(Directory.EnumerateFiles(targetDir, "*.log", t), sb);
            RemoveFiles(Directory.EnumerateDirectories(targetDir, "cdmp_*", t), sb);

            return sb.ToString();

        }

        private void RemoveFiles(IEnumerable<string> files, StringBuilder sb)
        {
            foreach (string file in files)
            {
                try
                {
                    File.Delete(file.Trim());
                }
                catch (Exception ex)
                {
                    sb.AppendLine(file + " remove fails " + ex.Message);
                }
            }
            
        }

        public string UploadLog(UploadRecord record)
        {
            IUploader uploadder = new WinSCPUploader(record);
            string rs = uploadder.Upload();
            return rs;
        }
    }


}
