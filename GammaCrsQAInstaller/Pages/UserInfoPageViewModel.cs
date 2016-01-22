using GammaCrsQAInstaller.Helper;
using GammaCrsQAInstaller.RemoteSetup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GammaCrsQAInstaller.Pages
{
    class UserInfoPageViewModel : ObservableObject, IPageViewModel
    {
        public string Name
        {
            get
            {
                return "User info";
            }
        }

        public string Username { get; set; }
        public string Password { get; set; }
        public string Domain { get; set; }

        public bool SaveContent()
        {
            bool rs = true;
            var passwordBox = FindControlByName.FindChild<PasswordBox>(Application.Current.MainWindow, "LogonPwdBox");
            if (passwordBox != null && Username != null && Domain != null && Username.Length != 0 && Domain.Length != 0)
            {
                Password = passwordBox.Password;
                if (Password != null)
                {
                    SetupInfo.SetValue(SetupInfoKeys.LogonUser, Username);
                    SetupInfo.SetValue(SetupInfoKeys.LogonDomain, Domain);
                    SetupInfo.SetValue(SetupInfoKeys.LogonPwd, Password);
                }
                else
                {
                    rs = false;
                }
                
            } else
            {
                MessageBox.Show("Pleaes check the input.");
                rs = false;
            }
            return rs;
            
        }

    }
}
