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
        private void GenerateInstallScript(string dmp_loc, string dmp_file_name, string dbhome, string dbname,string sys, string pwd, string host)
        {
            if (!Directory.Exists(tmp_dir))
            {
                Directory.CreateDirectory(tmp_dir);
            }
            File.AppendAllText(sw_install, string.Format(@"set ORACLE_HOME={0}", dbhome));
            File.AppendAllText(sw_install, string.Format(@"sqlplus {0}/{1}@{2}/{3}@{4}", sys, pwd, host, dbname, sw_pre_sql));
            File.AppendAllText(sw_install, string.Format(@"cd {0}", tmp_dir));
            File.AppendAllText(sw_install, string.Format(@"Impdp {0}/{1} fromuser=soe touser=soe directory=soe_workload file={2} log=logfile_imp.log", 
                                                            sys, pwd, dmp_file_name));
            File.AppendAllText(sw_install, string.Format(@"sqlplus {0}/{1}@{2}/{3}@{4}", sys, pwd, host, dbname, sw_post_sql));
            //sw_pre.sql
            string sw_install_sql = string.Format(@"create or replace directory soe_workload as '{0}';
                                                grant read,write on directory soe_workload to system;",dmp_loc);
            File.WriteAllText(@"c:\temp\sw_pre.sql", sw_install_sql);
            //sw_post.sql, fix a bug for impdp
            string sw_post_install_sql = @"grant execute on dbms_lock to public; 
                                           grant execute on dbms_lock to soe; 
                                           alter package ""SOE"".""ORDERENTRY"" compile;      
                                           alter package ""SOE"".""ORDERENTRY"" compile body;";

            File.WriteAllText(@"c:\temp\sw_post.sql", sw_post_install_sql);

        }

        private void InstallWorkload()
        {
            GeneralUtility.PureCmdExec.PureCmdExector("cmd.exe", string.Format(@"/C {0}", sw_install));
        }
    }
}
