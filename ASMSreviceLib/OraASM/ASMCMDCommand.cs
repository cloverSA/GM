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
    public class ASMCMDCommand:IASMCMDCommand
    {
        public string  RunASMCMD(string command)
        {
            string crs_home = CrsEnv.GetCurrentCrsHome();
            string asmcmd = Path.Combine(crs_home, "bin", "asmcmd.bat");
            string output = ASMCMDExecutor.ASMCMDExector(asmcmd, command, crs_home);
            return output;
        }
    }
}
