using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;
using System.IO;
using System.Text;
//GammaLib
using GeneralUtility;


namespace GammaServiceLib.OraCRS
{

    public partial class CrsCleaner : ICrsCleaner
    {
        private const string TX_RESULT_FAIL = "[GAMMA_ERROR]";
        private const string TX_RESULT_SUC = "[GAMMA_SUC]";

        public string RemoveOraKeys()
        {
            StringBuilder sb = new StringBuilder();
            string[] orakeys = new string[]
            {
                @"SOFTWARE\ORACLE",
                @"SOFTWARE\odbc\odbcinst.ini\Ora*",
                @"SYSTEM\CurrentControlSet\Services\Ora*",
                @"SYSTEM\CurrentControlSet\Services\ocfs",
                @"SYSTEM\CurrentControlSet\Services\Eventlog\Application\Ora*",
                @"SYSTEM\CurrentControlSet\Services\Eventlog\System\Ora*",
                @"SYSTEM\ControlSet001\Services\Ora*",
                @"SYSTEM\ControlSet001\Services\ocfs",
                @"SYSTEM\ControlSet001\Services\Eventlog\Application\Ora*",
                @"SYSTEM\ControlSet001\Services\Eventlog\System\Ora*",
                @"SYSTEM\ControlSet002\Services\Ora*",
                @"SYSTEM\ControlSet002\Services\ocfs",
                @"SYSTEM\ControlSet002\Services\Eventlog\Application\Ora*",
                @"SYSTEM\ControlSet002\Services\Eventlog\System\Ora*"

            };
            //this.StatusRTB.Document.Blocks.Clear();

            foreach (string regKey in orakeys)
            {
                if (regKey.Contains('*'))
                {
                    string regLoop = regKey.Replace("Ora*", "").Trim();
                    sb.AppendLine(GammaRegKeyUtility.HKLM_RegKey_RM_By_String(regLoop, "Ora"));
                }
                else
                {
                    sb.AppendLine(GammaRegKeyUtility.HKLM_RegKey_RM_LastElementTree(regKey));
                }
                
            }


            foreach (string subkey in Registry.ClassesRoot.GetSubKeyNames())
            {
                if (subkey.ToLower().StartsWith("ora") || subkey.StartsWith("ORCL"))
                {
                    try
                    {
                        Registry.ClassesRoot.DeleteSubKeyTree(subkey);
                        sb.AppendLine(subkey + " is removed from HK_CROOT");
                    }
                    catch (Exception ex)
                    {
                        sb.AppendLine(subkey + " removed fails from HK_CROOT " + ex.Message);
                    }

                }
            }
            ClearEnvironmentVarPath();
            return sb.ToString();
        }
    }
}
