using GammaCrsQAInstaller.Helper;
using GammaCrsQAInstaller.RemoteSetup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
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

        public void SaveContent(object parameter)
        {
            var passwordBox = parameter as PasswordBox;
            Password = passwordBox.Password;
            //Now go ahead and check the user name and password

            SetupInfo.SetValue(SetupInfoKeys.LogonUser, Username);
            SetupInfo.SetValue(SetupInfoKeys.LogonDomain, Domain);
            SetupInfo.SetValue(SetupInfoKeys.LogonPwd, Password);
        }

        public ICommand SaveContentCommand { get { return new RelayCommand<object>(SaveContent); } }
    }
}
