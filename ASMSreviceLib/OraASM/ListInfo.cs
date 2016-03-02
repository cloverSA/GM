using ASMServiceLib;
using GammaServiceLib;
using GammaServiceLib.OraCRS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMSreviceLib.OraASM
{
    public class ListInfo: IListInfo
    {
        private static string asmcmd = "asmcmd.bat";
        private static string crs_home = CrsEnv.GetCurrentCrsHome();
        string cmd = Path.Combine(crs_home, "bin", asmcmd);

        public string ListDGinfo()
        {
            string args = "lsdg";
            string output = ASMCMDExecutor.ASMCMDExector(cmd, args, crs_home);
            return output;
        }

        public string ListMemberDiskInfo(string DG_name = "")
        {
            string args = "";
            if (DG_name == "")
                args = "lsdsk -kp --member";
            else
                args = "lsdsk -kp -G " + DG_name;

            string output = ASMCMDExecutor.ASMCMDExector(cmd, args, crs_home);
            return output;
        }
        public string ListCandidateDiskInfo()
        {
            string args = "lsdsk -kp --candidate";
            string output = ASMCMDExecutor.ASMCMDExector(cmd, args, crs_home);
            return output;
        }
        public string ListOperation()
        {
            string args = "lsop";
            string output = ASMCMDExecutor.ASMCMDExector(cmd, args, crs_home);
            return output;
        }
    }
}
