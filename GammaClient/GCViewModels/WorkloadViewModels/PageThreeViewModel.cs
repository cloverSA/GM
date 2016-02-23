using GammaClient.GCViewModels.WorkloadViewModels.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GammaClient.GCModels;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;
using GammaClient.GCViews.WorkloadViews;
using MahApps.Metro.Controls.Dialogs;

namespace GammaClient.GCViewModels.WorkloadViewModels
{
    class PageThreeViewModel : PageViewModel
    {
        private readonly IDialogCoordinator _dialogCoordinator;

        public PageThreeViewModel()
        {
            _dialogCoordinator = DialogCoordinator.Instance;
        }

        public override void ProcessNavigateArgs(NavigateArgs args)
        {
            DBs = WorkloadSetupInfo.GetValue<ObservableCollection<IOraDB>>(WorkloadSetupKeys.DBS);

            WorkLoads = new ObservableCollection<string>() { "Swingbench", "Aroltp" };
        }

        public ObservableCollection<IOraDB> DBs { get; set; }
        public ObservableCollection<string> WorkLoads { get; set; }

        private void GoNext(object command_parm)
        {
            RaisePreviousPageEvent(this, new NavigateArgs(DBs));
        }

        private void GoBack(object command_parm)
        {
            RaisePreviousPageEvent(this, null);
        }

        private async void RunInstallWorkloadDialog()
        {
            var diaglog = new WorkLoadInstallDialog();
            var diaglogContent = new PageFourViewModel((instance) => {
                _dialogCoordinator.HideMetroDialogAsync(this, diaglog);
            });
            diaglog.Content = new PageFour { DataContext = diaglogContent };
            await _dialogCoordinator.ShowMetroDialogAsync(this, diaglog);
        }
        private ICommand _installcmd;
        public ICommand InstallCommand { get { return _installcmd ?? (this._installcmd = new RelayCommand(RunInstallWorkloadDialog)); } }
        public ICommand GoNextCommand { get { return new RelayCommand<object>(GoNext); } }
        public ICommand GoBackCommand { get { return new RelayCommand<object>(GoBack); } }
    }
}
