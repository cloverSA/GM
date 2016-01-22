using GammaCrsQAInstaller.Helper;
using GammaCrsQAInstaller.RemoteSetup;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GammaCrsQAInstaller.Pages
{
    class BinLocPageViewModel : ObservableObject, IPageViewModel
    {
        public string Name
        {
            get
            {
                return "Bin Location";
            }
        }

        public string AgentBinLocation { get; set; }
        public string InstallationLocation { get; set; }

        public bool SaveContent()
        {
            bool rs = true;
            if (AgentBinLocation!=null && InstallationLocation != null && Directory.Exists(AgentBinLocation) && InstallationLocation.Length!=0)
            {
                var xcopy_template_file = System.IO.Path.Combine(AgentBinLocation, "machine.conf");
                if (!System.IO.File.Exists(xcopy_template_file))
                {
                    System.IO.File.WriteAllText(xcopy_template_file, "xcopy template");
                }
                SetupInfo.SetValue(SetupInfoKeys.SrvLocBin, AgentBinLocation);
                SetupInfo.SetValue(SetupInfoKeys.SrvRemoteBin, InstallationLocation);
            } else
            {
                MessageBox.Show("Invalid input for the path");
                rs = false;
            }
            return rs;
            
        }

    }
}
