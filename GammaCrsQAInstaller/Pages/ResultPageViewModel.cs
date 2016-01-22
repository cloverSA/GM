using GammaCrsQAInstaller.Helper;
using GammaCrsQAInstaller.RemoteSetup;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GammaCrsQAInstaller.Pages
{
    class ResultPageViewModel : ObservableObject, IPageViewModel
    {
        public string Name
        {
            get
            {
                return "Result";
            }
        }

        public string ResultText { get; set; }
        private int progress_step = 1;

        public void RunInstall()
        {
            var progressBar =  FindControlByName.FindChild<ProgressBar>(Application.Current.MainWindow, "InstallProgressBar"); 
            var steps = SetupInfo.GetValue<ObservableCollection<Node>>(SetupInfoKeys.NodeList).Count * 5;
            this.progress_step = Convert.ToInt32((progressBar.Maximum / steps));

            var t = Task.Run(() =>
            {
                var installer = InstallerFactory.GetGammaServiceSetup();
                installer.OnOpCompletedEventHandler += (s, e)=> {
                    ResultText += string.Format("--------------------\n{0}\n{1}\n{2}\n", e.Hostname, e.OpType, e.OpResult);
                    progressBar.Value += this.progress_step;
                };
                return installer.AllInOne();
            });
            t.GetAwaiter().OnCompleted(() => {
                progressBar.Value = 100;
                ResultText += "\n===========FINISHED===============\n";
            });

        }

        private void ScrollDownResult(object sender)
        {
            var tb = sender as TextBox;
            tb.ScrollToEnd();
        }

        public bool SaveContent()
        {
            //throw new NotImplementedException();
            RunInstall();
            return true;
        }

        public ICommand ScrollDownCommand { get { return new RelayCommand<object>(ScrollDownResult); } }
        public ICommand SaveContentCommand { get { return new RelayCommand(RunInstall); } }
    }
}
