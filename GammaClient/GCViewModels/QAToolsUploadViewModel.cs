using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GammaClient.GCFacilities.WCFProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace GammaClient.GCViewModels
{
    class QAToolsUploadViewModel : ObservableObject
    {
        private readonly Action<QAToolsUploadViewModel> _closehandler;
        private string _username = string.Empty;
        private string _password = string.Empty;
        private string _bugnum = string.Empty;
        private string _uploadPath = string.Empty;

        public QAToolsUploadViewModel(Action<QAToolsUploadViewModel> closeHandler)
        {
            _closehandler = closeHandler;
        }

        
        public string Username
        {
            get
            {
                return _username;
            }

            set
            {
                _username = value;
                RaisePropertyChanged("Username");
            }
        }

        public string Password
        {
            get
            {
                return _password;
            }

            set
            {
                _password = value;
            }
        }

        public string UploadPath
        {
            get
            {
                return _uploadPath;
            }

            set
            {
                _uploadPath = value;
                RaisePropertyChanged("UploadPath");
            }
        }

        public string Bugnum
        {
            get
            {
                return _bugnum;
            }

            set
            {
                _bugnum = value;
                RaisePropertyChanged("Bugnum");
            }
        }
        private void RaisePasswordChangedEvent(object sender)
        {
            var pwdbox = sender as PasswordBox;
            Password = pwdbox.Password;
        }

        private void UploadLog()
        {
            var pwd = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var task = QAToolsFacade.UploadLogToSftp(Username.Trim(), GeneralUtility.GammaRSASimplify.RSAEncryptString(Password, System.IO.Path.Combine(pwd, "pubkf.xml")), Bugnum.Trim(), UploadPath.Trim());
            task.GetAwaiter().OnCompleted(()=> {

            });
        }

        private bool CanUpload()
        {
            var rs = false;
            if(Username.Length!=0 && Password.Length!=0 && UploadPath.Length!=0&& Bugnum.Length != 0)
            {
                rs = true;
            }
            return rs;
        }
        public ICommand UploadCommand { get { return new RelayCommand(UploadLog, CanUpload); } }
        public ICommand CloseCommand { get { return new RelayCommand<QAToolsUploadViewModel>(_closehandler); } }
        public ICommand PasswordChangeCommand { get { return new RelayCommand<object>(RaisePasswordChangedEvent); } }


    }
}
