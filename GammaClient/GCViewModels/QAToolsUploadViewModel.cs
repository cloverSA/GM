using GalaSoft.MvvmLight.CommandWpf;
using GammaClient.GCFacilities.WCFProxy;
using GammaClient.GCUIBehavior;
using GammaClient.GCViewModels.WorkloadViewModels.Navigation;
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
    class QAToolsUploadViewModel : PageViewModel
    {
        private readonly Action<QAToolsUploadViewModel> _closehandler;
        private string _username = string.Empty;
        private string _password = string.Empty;
        private string _bugnum = string.Empty;
        private string _uploadPath = string.Empty;
        private string _uploadResult = string.Empty;
        private bool _resultOnly = false;
        private object _locker = new object();
        private IContentScrollDown _scroller = new TextBoxScrollDown();

        public QAToolsUploadViewModel(Action<QAToolsUploadViewModel> closeHandler)
        {
            _closehandler = closeHandler;
        }

        
        public string Username
        {
            get
            {
                return _username.Trim();
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
                _uploadPath = value.Trim();
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
                _bugnum = value.Trim();
                RaisePropertyChanged("Bugnum");
            }
        }

        public string UploadResult
        {
            get
            {
                return _uploadResult;
            }

            set
            {
                _uploadResult = value;
                RaisePropertyChanged("UploadResult");
            }
        }

        public bool ResultOnly
        {
            get
            {
                return _resultOnly;
            }

            set
            {
                _resultOnly = value;
                RaisePropertyChanged("ResultOnly");
            }
        }

        private void RaisePasswordChangedEvent(object sender)
        {
            var pwdbox = sender as PasswordBox;
            Password = pwdbox.Password;
        }

        private void UploadLog()
        {
            InProgressWait(true);
            var pwd = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var pubkf = System.IO.Path.Combine(pwd, "pubkf.xml");

            
            var task = QAToolsFacade.UploadLogToSftp(Username, 
                                          GeneralUtility.GammaRSASimplify.RSAEncryptString(Password, pubkf), 
                                          Bugnum, 
                                          UploadPath,
                                          UpdateResult);

            task.GetAwaiter().OnCompleted(()=> {
                InProgressWait(false);
                UpdateResult("Finished!");
            });

        }

        public override void InProgressWait(bool inProgress)
        {
            if (inProgress)
            {
                ResultOnly = true;
                CanSwitchPage = false;
                InProgress = true;
            }
            else
            {
                //Not display previous page but keep Result Only
                //Close ProgressRing
                InProgress = false;
            }
        }
        private void UpdateResult(string result)
        {
            lock(_locker)
            {
                UploadResult += Environment.NewLine;
                UploadResult += result;
            }
        }

        private bool CanUpload()
        {
            var rs = false;
            if(Username.Length!=0 && Password.Length!=0 && UploadPath.Length!=0 && Bugnum.Length != 0)
            {
                rs = true;
            }
            return rs;
        }

        public ICommand UploadCommand { get { return new RelayCommand(UploadLog); } }
        public ICommand CloseCommand { get { return new RelayCommand<QAToolsUploadViewModel>(_closehandler); } }
        public ICommand PasswordChangeCommand { get { return new RelayCommand<object>(RaisePasswordChangedEvent); } }
        public ICommand ScrollDownCommand { get { return _scroller.ScrollDownCommand; } }


    }
}
