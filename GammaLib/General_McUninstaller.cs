using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;

namespace GeneralUtility
{
    public class McUninstaller
    {
        Dictionary<string, string> uninstall = null;
        string enterprise = "McAfee VirusScan Enterprise";
        string agent = "McAfee Agent";
        string agent_uninstall_exe = string.Format(@"{0}\McAfee\Common Framework\FrmInst.exe", Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86));
        string agent_uninstall_argument = @"/Forceuninstall";
        public McUninstaller()
        {
            uninstall = new Dictionary<string, string>();
            GetMcInstallConfig();
        }

        private void GetMcInstallConfig()
        {
            string rsk_root_base = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Installer\UserData\S-1-5-18\Products";
            string[] Names = null;
            using (RegistryKey rsk = Registry.LocalMachine.OpenSubKey(rsk_root_base))
            {
                Names = rsk.GetSubKeyNames();
            }

            foreach (string subKeyName in Names)
            {
                try
                {
                    using (RegistryKey product_key = Registry.LocalMachine.OpenSubKey(string.Format(@"{0}\{1}\InstallProperties", rsk_root_base, subKeyName)))
                    {

                        if (product_key != null)
                        {

                            object obj = product_key.GetValue("Uninstallstring");

                            if (obj != null)
                            {

                                if (product_key.GetValue("DisplayName").ToString().Contains(enterprise))
                                {
                                    uninstall[enterprise] = obj.ToString();
                                }
                                else if (product_key.GetValue("DisplayName").ToString().Contains(agent))
                                {
                                    uninstall[agent] = obj.ToString();
                                }
                                if (uninstall.Count == 2)
                                {
                                    break;
                                }
                            }

                        }

                    }
                }
                catch (Exception ex)
                {
                    //todo
                }

            }

        }

        public void UninstallMc()
        {
            try
            {
                if (uninstall[enterprise] != null)
                {
                    string[] s = uninstall[enterprise].Split(' ');
                    GammaUtility.ShellExecutor(s[0], s[1]);
                }

                if (uninstall[agent] != null)
                {
                    GammaUtility.ShellExecutor(agent_uninstall_exe, agent_uninstall_argument);
                }


            }
            catch (Exception ex)
            {
                //todo
            }

        }

        public bool IsInstalled()
        {
            return uninstall.Count() > 0 ? true : false;
        }

    }
}
