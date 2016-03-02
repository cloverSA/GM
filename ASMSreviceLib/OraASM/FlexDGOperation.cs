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
    public class FlexDGOperation:IFlexDGOperation 
    {
        private static string asmcmd = "asmcmd.bat";
        private static string crs_home = CrsEnv.GetCurrentCrsHome();
        string cmd = Path.Combine(crs_home, "bin", asmcmd);

        public string ConvertRedundToFlex(string DG_name)
        {
            string args = string.Format("chdg '<chdg name=\"{0}\" redundancy=\"flex\"/>'", DG_name);
            string output = ASMCMDExecutor.ASMCMDExector(cmd, args, crs_home);
            return output;
        }

        public string MakeFilegroup(string DG_name, string filegroup_name, string client_name, string client_type, string property_name = "", string property_value = "", string file_type = "")
        {
            string property_clause = "";
            if (property_name != "")
                if (file_type != "")
                    property_clause = string.Format("<p name=\"{0}\" value=\"{1}\" file_type=\"{2}\"/>'", property_name, property_value, file_type);
                else
                    property_clause = string.Format("<p name=\"{0}\" value=\"{1}\"/>", property_name, property_value);

            string config_xml = string.Format("'<filegroup name=\"{0}\" dg=\"{1}\" client_type=\"{2}\" client_name=\"{3}\">{4}</filegroup>", filegroup_name, DG_name, client_type, client_name, property_clause);
            string args = "mkfg " + config_xml;
            string output = ASMCMDExecutor.ASMCMDExector(cmd, args, crs_home);
            return output;
        }

        public string ModifyFilegroup(string DG_name, string filegroup_name, string property_name, string property_value, string file_type = "")
        {
            string filetype_clause = "";
            if (file_type != "")
                filetype_clause = string.Format("file_type=\"{0}\"", file_type);
            string config_xml = string.Format("'<filegroup name=\"{0}\" dg_name=\"{1}\"><p name=\"{2}\" value=\"{3}\" {4}/>", filegroup_name, DG_name, property_name, property_value, filetype_clause);
            string args = "chfg " + config_xml;
            string output = ASMCMDExecutor.ASMCMDExector(cmd, args, crs_home);
            return output;
        }

        public string ListFilegroup(string DG_name = "", string filegroup_name = "")
        {
            string DGname_clause = "";
            string FGname_clause = "";
            if (DG_name != "")
                DGname_clause = "-G " + DG_name;
            if (filegroup_name != "")
                FGname_clause = "--filegroup " + filegroup_name;

            string args = string.Format("lsfg {0} {1}", DGname_clause, FGname_clause);
            string output = ASMCMDExecutor.ASMCMDExector(cmd, args, crs_home);
            return output;
        }

        public string RemoveFilegroup(string DG_name, string filegroup_name, bool isRecursive = false)
        {
            string args = "";
            if (isRecursive == false)
                args = string.Format("rmfg {0} {1}", DG_name, filegroup_name);
            else
                args = string.Format("rmfg -r {0} {1}", DG_name, filegroup_name);
            string output = ASMCMDExecutor.ASMCMDExector(cmd, args, crs_home);
            return output;
        }

        public string MakeQuotagroup(string DG_name, string quotagroup_name, string property_name, string property_value)
        {
            string args = string.Format("mkqg -G {0} {1} {2} {3}", DG_name, quotagroup_name, property_name, property_value);
            string output = ASMCMDExecutor.ASMCMDExector(cmd, args, crs_home);
            return output;
        }

        public string ModifyQuotagroup(string DG_name, string quotagroup_name, string property_name, string property_value)
        {
            string args = string.Format("chqg -G {0} {1} {2} {3}", DG_name, quotagroup_name, property_name, property_value);
            string output = ASMCMDExecutor.ASMCMDExector(cmd, args, crs_home);
            return output;
        }

        public string ListQuotagroup(string DG_name = "", string quotagroup_name = "")
        {
            string DGname_clause = "";
            string QGname_clasue = "";
            if (DG_name != "")
                DGname_clause = "-G " + DG_name;
            if (quotagroup_name != "")
                QGname_clasue = "--quotagroup " + quotagroup_name;
            string args = string.Format("lsqg {0} {1}", DGname_clause, QGname_clasue);
            string output = ASMCMDExecutor.ASMCMDExector(cmd, args, crs_home);
            return output;
        }

        public string RemoveQuotagroup(string DG_name, string quotagroup_name)
        {
            string args = string.Format("rmqg -G {0} {1}", DG_name, quotagroup_name);
            string output = ASMCMDExecutor.ASMCMDExector(cmd, args, crs_home);
            return output;
        }

        public string MoveFGtoQG(string DG_name, string quotagroup_name, string filegroup_name)
        {
            string args = string.Format("mvfg -G {0} --filegroup {1} {2}", DG_name, filegroup_name, quotagroup_name);
            string output = ASMCMDExecutor.ASMCMDExector(cmd, args, crs_home);
            return output;
        }
    }
}
