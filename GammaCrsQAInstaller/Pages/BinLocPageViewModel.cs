using GammaCrsQAInstaller.Helper;
using GammaCrsQAInstaller.RemoteSetup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public string AgentBinLocation {get;set;}
        public string InstallationLocation { get; set; }

        public void SaveContent()
        {
            if (AgentBinLocation!=null && InstallationLocation != null)
            {
                var xcopy_template_file = System.IO.Path.Combine(AgentBinLocation, "machine.conf");
                if (!System.IO.File.Exists(xcopy_template_file))
                {
                    System.IO.File.WriteAllText(xcopy_template_file, "xcopy template");
                }
                SetupInfo.SetValue(SetupInfoKeys.SrvLocBin, AgentBinLocation);
                SetupInfo.SetValue(SetupInfoKeys.SrvRemoteBin, InstallationLocation);
            }
            
        }
        public ICommand ContentSaveCommand { get { return new RelayCommand(SaveContent); } }

    }
}
