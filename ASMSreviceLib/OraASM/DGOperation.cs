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
    public class DGOperation : IDGOperation
    {
        private static string asmcmd = "asmcmd.bat";
        private static string crs_home = CrsEnv.GetCurrentCrsHome();
        string cmd = Path.Combine(crs_home, "bin", asmcmd);

        public string DGRebalance(string DG_name,int power = 1, bool isWait = false)
        {
            string args = "";
            if (isWait == false)
                args = "rebal --power " + power.ToString() + " " + DG_name;
            else
                args = "rebal --power " + power.ToString() + " " + DG_name + " -w";

            string output = ASMCMDExecutor.ASMCMDExector(cmd, args, crs_home);
            return output;
        }

        public string ListDGAttributes(string DG_name)
        {
            string args = "lsattr -ml -G " + DG_name;
            string output = ASMCMDExecutor.ASMCMDExector(cmd, args, crs_home);
            return output;
        }

        public string SetDGAttribute(string DG_name, string attr_name, string attr_value)
        {
            string args = "setattr -G " + DG_name + " " + attr_name + " " + attr_value;
            string output = ASMCMDExecutor.ASMCMDExector(cmd, args, crs_home);
            return output;
        }

        public string MountDG(string DG_name = "", bool isAll = false, bool isRestrict = false, bool isForce = false)
        {
            string all = "";
            string restrict = "";
            string force = "";
            if (isAll == true)
                all = "-a";
            if (isRestrict == true)
                restrict = "--restrict";
            if (isForce == true)
                force = "-f";

            string args = string.Format("mount {0} {1} {2} {3}", restrict, all, force, DG_name);
            string output = ASMCMDExecutor.ASMCMDExector(cmd, args, crs_home);
            return output;
        }

        public string DismountDG(string DG_name = "", bool isALL = false, bool isForce = false)
        {
            string all = "";
            string force = "";
            if (isALL == true)
                all = "-a";
            if (isForce == true)
                force = "-f";

            string args = string.Format("umount {0} {1} {2}", all, force, DG_name);
            string output = ASMCMDExecutor.ASMCMDExector(cmd, args, crs_home);
            return output;
        }

        public string DropDG(string DG_name, bool isForce = false, bool isRecursive = false)
        {
            string force = "";
            string recursive = "";
            if (isForce == true)
                force = "-f";
            if (isRecursive == true)
                recursive = "-r";

            string args = string.Format("dropdg {0} {1} {2}", recursive, force, DG_name);
            string output = ASMCMDExecutor.ASMCMDExector(cmd, args, crs_home);
            return output;
        }


    }

}
