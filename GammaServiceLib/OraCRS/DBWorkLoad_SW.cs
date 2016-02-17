using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GammaServiceLib.OraCRS
{
    public partial class DBWorkLoad : IDBWorkload
    {
        private const string TX_RESULT_FAIL = "[GAMMA_ERROR]";
        private const string TX_RESULT_SUC = "[GAMMA_SUC]";

        public string InstallSwingBench(string hostname, string dbhome, string dbname, string system_pwd, string sys_pwd, string workloadDmpLoc, string workloadDmpFileName)
        {
            var sw = new Swingbench()
            {
                DBDirName = "swingbench",
                DBHost = hostname,
                DBPumpUser = "system",
                DBPumpPwd = system_pwd,
                SwingbenchDmpDir = workloadDmpLoc,
                SwingbenchDmpFilename = workloadDmpFileName,
                SwingbenchDmpFilePath = Path.Combine(workloadDmpLoc, workloadDmpFileName),
                TargetDBHome = dbhome,
                TargetDBName = dbname,
                SysPwd = sys_pwd,
                SysUsr = "sys"
            };
            try
            {
                sw.InstallWorkload();
            }
            catch (Exception ex)
            {
                return string.Format("{0} hit error when generating the installation script: {1}", TX_RESULT_FAIL, ex.Message);
            }
            return string.Format("{0} please run the script {1} to install workload.", TX_RESULT_SUC, sw.SwingbenchInstallScript);
        }
    }
}
