using GammaCrsQAInstaller.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GammaCrsQAInstaller.Helper
{
    public class ApplicationViewModel : ObservableObject
    {
        #region Fields
        private bool _btnAble = true;

        private ICommand _changePageCommand;

        private IPageViewModel _currentPageViewModel;
        private List<IPageViewModel> _pageViewModels;

        #endregion
        private void RaiseFinishInstallEvent(object sender, EventArgs e)
        {
            BtnAble = false;
        }
        public ApplicationViewModel()
        {
            // Add available pages
            PageViewModels.Add(new BinLocPageViewModel());
            PageViewModels.Add(new UserInfoPageViewModel());
            PageViewModels.Add(new NodeListPageViewModel());
            var rsp = new ResultPageViewModel();
            rsp.FinishInstallEventHandler += RaiseFinishInstallEvent;
            PageViewModels.Add(rsp);

            // Set starting page
            CurrentPageViewModel = PageViewModels[0];
        }

        #region Properties / Commands

        public ICommand ChangePageCommand
        {
            get
            {
                if (_changePageCommand == null)
                {
                    _changePageCommand = new RelayCommand<IPageViewModel>(
                        p => ChangeViewModel((IPageViewModel)p));
                }

                return _changePageCommand;
            }
        }

        public ICommand NextViewModelCommand { get { return new RelayCommand(NextViewModel); } }

        public ICommand PreviousViewModelCommand { get { return new RelayCommand(PreviousViewModel); } }

        public List<IPageViewModel> PageViewModels
        {
            get
            {
                if (_pageViewModels == null)
                    _pageViewModels = new List<IPageViewModel>();

                return _pageViewModels;
            }
        }

        public IPageViewModel CurrentPageViewModel
        {
            get
            {
                return _currentPageViewModel;
            }
            set
            {
                if (_currentPageViewModel != value)
                {
                    _currentPageViewModel = value;
                    OnPropertyChanged("CurrentPageViewModel");
                }
            }
        }

        public bool BtnAble
        {
            get
            {
                return _btnAble;
            }

            set
            {
                _btnAble = value;
                OnPropertyChanged("BtnAble");
            }
        }

        #endregion

        #region Methods

        private void ChangeViewModel(IPageViewModel viewModel)
        {
            /*
            if (!PageViewModels.Contains(viewModel))
            {
                PageViewModels.Add(viewModel);
            }
            */
            if (CurrentPageViewModel.SaveContent())
            {
                CurrentPageViewModel = PageViewModels.FirstOrDefault(vm => vm == viewModel);
            }
               
        }

        private void NextViewModel()
        {
            if (CurrentPageViewModel.SaveContent())
            {
                int current_index = PageViewModels.FindIndex(vm => vm == CurrentPageViewModel);
                if (current_index + 1 != PageViewModels.Count)
                {
                    CurrentPageViewModel = PageViewModels[current_index + 1];
                }
            }
        }

        private void PreviousViewModel()
        {
            int current_index = PageViewModels.FindIndex(vm => vm == CurrentPageViewModel);
            if (current_index != 0)
            {
                CurrentPageViewModel = PageViewModels[current_index - 1];
            }
        }

        #endregion


    }
}
