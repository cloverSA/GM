using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GammaServiceLib.OraCRS
{
    class Swingbench
    {
        private static string tmp_dir = @"c:\temp\";
        private static string sw_install = Path.Combine(@"c:\temp\", "sw_install.bat");
        private static string sw_pre_sql = Path.Combine(@"c:\temp\", "sw_pre.sql");
        private static string sw_post_sql = Path.Combine(@"c:\temp\", "sw_post.sql");

        public string SwingbenchDmpDir { get; set; }
        public string SwingbenchDmpFilename { get; set; }
        public string SwingbenchDmpFilePath { get; set; }
        public string TargetDBHome { get; set; }
        public string TargetDBName { get; set; }
        public string DBPumpUser { get; set; }
        public string DBPumpPwd { get; set; }
        public string DBHost { get; set; }
        public string DBDirName { get; set; }

        private void GenMainBat()
        {
            if (!Directory.Exists(tmp_dir))
            {
                Directory.CreateDirectory(tmp_dir);
            }
            File.AppendAllText(sw_install, string.Format(@"set ORACLE_HOME={0}", TargetDBHome));
            File.AppendAllText(sw_install, string.Format(@"sqlplus {0}/{1}@{2}/{3}@{4}", DBPumpUser, DBPumpPwd, DBHost, TargetDBName, sw_pre_sql));
            File.AppendAllText(sw_install, string.Format(@"cd {0}", tmp_dir));
            File.AppendAllText(sw_install, string.Format(@"Impdp {0}/{1} fromuser=soe touser=soe directory=soe_workload file={2} log=logfile_imp.log",
                                                            DBPumpUser, DBPumpPwd, SwingbenchDmpFilename));
            File.AppendAllText(sw_install, string.Format(@"sqlplus {0}/{1}@{2}/{3}@{4}", DBPumpUser, DBPumpPwd, DBHost, TargetDBName, sw_post_sql));
        }

        private void GenSwingbenchPrereqSql()
        {
            //SwingbenchDmpDir is also the db directory
            string sw_install_sql = string.Format(@"create or replace directory {1} as '{0}';
                                                grant read,write on directory soe_workload to system;", SwingbenchDmpDir, DBDirName);
            File.WriteAllText(@"c:\temp\sw_pre.sql", sw_install_sql);
        }
        private void GenSwingbenchPostFixupSql()
        {

            //sw_post.sql, fix a bug for impdp
            string sw_post_install_sql = @"grant execute on dbms_lock to public; 
                                           grant execute on dbms_lock to soe; 
                                           alter package ""SOE"".""ORDERENTRY"" compile;      
                                           alter package ""SOE"".""ORDERENTRY"" compile body;";

            File.WriteAllText(@"c:\temp\sw_post.sql", sw_post_install_sql);

        }

        private void InstallWorkload()
        {
            GenMainBat();
            GenSwingbenchPrereqSql();
            GenSwingbenchPostFixupSql();
            GeneralUtility.PureCmdExec.PureCmdExector("cmd.exe", string.Format(@"/C {0}", sw_install));
        }


    }
}
