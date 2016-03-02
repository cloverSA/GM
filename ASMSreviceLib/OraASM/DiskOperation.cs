using ASMServiceLib;
using GammaServiceLib;
using GammaServiceLib.OraCRS;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMSreviceLib.OraASM
{
    public class DiskOperation: IDiskOperation
    {
        private static string asmcmd = "asmcmd.bat";
        private static string crs_home = CrsEnv.GetCurrentCrsHome();
        string cmd = Path.Combine(crs_home, "bin", asmcmd);

        public string AddDisk(string DG_name, string disk_name, string failgroup_name = "", bool isForce = false)
        {
            string disk_clause = "";
            if (isForce == false)
                disk_clause = string.Format("<dsk string = \"{0}\"/>", disk_name);
            else
                disk_clause = string.Format("<dsk string = \"{0}\" force=\"true\"/>", disk_name);

            string args = "";
            if (failgroup_name != "")
                args = string.Format("'<chdg name=\"{0}\"><add><fg name=\"{1}\">{2}</fg></add></chdg>'", DG_name, failgroup_name, disk_clause);
            else
                args = string.Format("'<chdg name=\"{0}\"><add>{1}</add></chdg>'", DG_name, disk_clause);

            string output = ASMCMDExecutor.ASMCMDExector(cmd, args, crs_home);
            return output;
        }

        public string DropDisk(string DG_name, string disk_name, string failgroup_name = "", bool isForce = false)
        {
            string disk_clause = "";
            if (isForce == true)
                disk_clause = string.Format("<dsk name=\"{0}\" force=\"true\"/>", disk_name);
            else
                disk_clause = string.Format("<dsk name=\"{0}\"/>", disk_name);

            string args = "";
            if (failgroup_name != "")
                args = string.Format("'<chdg name=\"{0}\"><drop><fg name=\"{1}\">{2}</fg></drop></chdg>'", DG_name, failgroup_name, disk_clause);
            else
                args = string.Format("'<chdg name=\"{0}\"><drop>{1}</drop></chdg>'", DG_name, disk_clause);

            string output = ASMCMDExecutor.ASMCMDExector(cmd, args, crs_home);
            return output;
        }

        public string OfflineDisk(string DG_name, string disk_name = "", string failgroup_name = "", string drop_time = "")
        {
            string disk_clause = "";
            string failgroup_clause = "";
            string droptime_clause = "";
            if (disk_name != "")
                disk_clause = "-D " + disk_name;
            if (failgroup_name != "")
                failgroup_clause = "-F " + failgroup_name;
            if (drop_time != "")
                droptime_clause = "-t " + drop_time;

            string args = string.Format("offline -G {0} {1} {2} {3}", DG_name, disk_clause, failgroup_clause, droptime_clause);
            string output = ASMCMDExecutor.ASMCMDExector(cmd, args, crs_home);
            return output;
        }

        public string OnlineDisk(string DG_name, string disk_name = "", string failgroup_name = "", bool isAll = false, bool isWait = false, int power = 1)
        {
            string args = "";

            string power_clause = "";
            string wait_clause = "";
            if (power != 1)
                power_clause = "--power " + power.ToString();
            if (isWait == true)
                wait_clause = "-w";

            if(isAll == true)
            {
                args = string.Format("online -G {0} -a {1} {2}", DG_name, power_clause, wait_clause);
            }
            else
            {
                string disk_clause = "";
                string failgroup_clause = "";
                if (disk_name != "")
                    disk_clause = "-D " + disk_name;
                if (failgroup_name != "")
                    failgroup_clause = "-F " + failgroup_clause;

                args = string.Format("online -G {0} {1} {2} {3} {4}", DG_name, failgroup_clause, disk_clause, power_clause, wait_clause);
            }
            string output = ASMCMDExecutor.ASMCMDExector(cmd, args, crs_home);
            return output;
        }

    }
}
