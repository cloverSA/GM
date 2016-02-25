using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GammaServiceLib.OraCRS
{
    public class Swingbench
    {
        private static string tmp_dir = @"c:\temp\";
        private static string sw_install = Path.Combine(@"c:\temp\", "sw_install.bat");
        private static string sw_pre_sql = Path.Combine(@"c:\temp\", "sw_pre.sql");
        private static string sw_post_sql = Path.Combine(@"c:\temp\", "sw_post.sql");

        public string SwingbenchInstallScript
        {
            get
            {
                return sw_install;
            }
        }
        public string SwingbenchDmpDir { get; set; }
        public string SwingbenchDmpFilename { get; set; }
        public string SwingbenchDmpFilePath { get; set; }
        public string TargetDBHome { get; set; }
        public string TargetDBName { get; set; }
        public string DBPumpUser { get; set; }
        public string DBPumpPwd { get; set; }
        public string DBHost { get; set; }
        public string DBDirName { get; set; }
        public string SysUsr { get; set; }
        public string SysPwd { get; set; }

        private void GenMainBat()
        {
            try
            {
                if (!Directory.Exists(tmp_dir))
                {
                    Directory.CreateDirectory(tmp_dir);
                }
                var lines = new List<string>();
                var sqlplus = Path.Combine(TargetDBHome, "bin", "sqlplus.exe");
                var Impdp = Path.Combine(TargetDBHome, "bin", "Impdp.exe");
                lines.Add(string.Format(@"set ORACLE_HOME={0}", TargetDBHome));
                lines.Add(GetSysExecuteSqlStr(sw_pre_sql));
                lines.Add(string.Format(@"cd {0}", SwingbenchDmpDir));
                lines.Add(string.Format(@"{0} {1}/{2}@{3}/{4} fromuser=soe touser=soe directory={5} file={6} log=logfile_imp.log",
                                                               Impdp, DBPumpUser, DBPumpPwd, DBHost, TargetDBName, DBDirName, SwingbenchDmpFilename));
                lines.Add(GetSysExecuteSqlStr(sw_post_sql));
                lines.Add("exit!!");
                File.WriteAllLines(sw_install, lines);
            }
            catch
            {
                throw;
            }
        }

        private string GetSysExecuteSqlStr(string sql)
        {
            var sqlplus = Path.Combine(TargetDBHome, "bin", "sqlplus.exe");
            return string.Format(@"{0} {1}/{2}@{3}/{4} as sysdba @{5}", sqlplus, SysUsr, SysPwd, DBHost, TargetDBName, sql);
        }
        private void GenSwingbenchPrereqSql()
        {
            //SwingbenchDmpDir is also the db directory
            try
            {


                var lines = new List<string>();
                lines.Add("drop user soe cascade;");
                lines.Add("drop tablespace soe including contents and datafiles;");
                lines.Add("create tablespace soe datafile size 1g autoextend on next 64m maxsize unlimited;");
                lines.Add(string.Format(@"create or replace directory {1} as '{0}';", SwingbenchDmpDir, DBDirName));
                lines.Add(string.Format("grant read,write on directory {0} to {1};", DBDirName, DBPumpUser));
                lines.Add("exit");
                File.WriteAllLines(sw_pre_sql, lines);
            }
            catch
            {
                throw;
            }
        }
        private void GenSwingbenchPostFixupSql()
        {

            //sw_post.sql, fix a bug for impdp
            try
            {
                var lines = new List<string>();
                lines.Add("grant execute on dbms_lock to public;");
                lines.Add("grant execute on dbms_lock to soe;");
                lines.Add(@"alter package ""SOE"".""ORDERENTRY"" compile;");
                lines.Add(@"alter package ""SOE"".""ORDERENTRY"" compile body;");
                lines.Add("exit");
                File.WriteAllLines(sw_post_sql, lines);
            }
            catch
            {
                throw;
            }


        }

        public void InstallWorkload()
        {
            try
            {
                GenMainBat();
                GenSwingbenchPrereqSql();
                GenSwingbenchPostFixupSql();
                //GeneralUtility.PureCmdExec.PureCmdExector("cmd.exe", string.Format(@"/C {0}", sw_install));
            }
            catch
            {
                throw;
            }
        }


    }
}
