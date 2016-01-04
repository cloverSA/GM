using GeneralUtility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GammaCrsQAInstaller.RemoteSetup
{
    class InstallerFactory
    {
        public static RemoteServiceSetup GetGammaServiceSetup()
        {
            var pwd = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var installer = new GammaServiceSetup()
            {
                Domain = SetupInfo.GetValue<string>(SetupInfoKeys.LogonDomain),
                Username = SetupInfo.GetValue<string>(SetupInfoKeys.LogonUser),
                Password = SetupInfo.GetValue<string>(SetupInfoKeys.LogonPwd),
                Target_location = SetupInfo.GetValue<string>(SetupInfoKeys.SrvRemoteBin),
                Source_location = SetupInfo.GetValue<string>(SetupInfoKeys.SrvLocBin),
            };
            var nodelist = SetupInfo.GetValue<ObservableCollection<Node>>(SetupInfoKeys.NodeList);
            var hosts = new List<string>();

            var client_profile = System.IO.Path.Combine(pwd, installer.ClientProfile);
            var client_profile_content = new List<string>();
            foreach (var host in nodelist)
            {
                //parse the nodeinfo.
                hosts.Add(host.Hostname);
                //create server profile
                File.WriteAllLines(System.IO.Path.Combine(pwd, host.Hostname),
                    new string[] {
                    string.Format(@"public={0}",host.HostIP),
                    string.Format(@"port={0}",host.HostPort),
                });
                client_profile_content.Add(string.Format(@"{0},{1},{2}", host.Hostname, host.HostIP, host.HostPort));
            }
            //create client profile.
            File.WriteAllLines(client_profile, client_profile_content);
            installer.Hosts = hosts;

            return installer;
        }
    }
}
