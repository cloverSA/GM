using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GammaClient.GCModels;
using GammaClient.GCViewModels.WorkloadViewModels.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GammaClient.GCViewModels.WorkloadViewModels
{
    class WorkloadViewModel : ObservableObject
    {

        #region Fields

        private ICommand _changePageCommand;

        private IPageViewModel _currentPageViewModel;
        private List<IPageViewModel> _pageViewModels;

        #endregion

        public WorkloadViewModel()
        {
            // Add available pages
            var p1 = new PageOneViewModel();
            p1.NextPageEventHandler += HanldeNextPageEvent;
            PageViewModels.Add(p1);
            var p2 = new PageTwoViewModel();
            p2.NextPageEventHandler += HanldeNextPageEvent;
            p2.PreviousPageEventHandler += HanldePreviousPageEvent;
            PageViewModels.Add(p2);
            var p3 = new PageThreeViewModel();
            p3.NextPageEventHandler += HanldeNextPageEvent;
            p3.PreviousPageEventHandler += HanldePreviousPageEvent;
            PageViewModels.Add(p3);
            var p4 = new PageFourViewModel();
            p4.NextPageEventHandler += HanldeNextPageEvent;
            p4.PreviousPageEventHandler += HanldePreviousPageEvent;
            PageViewModels.Add(p4);
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
                    RaisePropertyChanged("CurrentPageViewModel");
                }
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
            if (CurrentPageViewModel.CanSwitchPage)
            {
                CurrentPageViewModel = PageViewModels.FirstOrDefault(vm => vm == viewModel);
            }

        }

        private void NextViewModel()
        {
            if (CurrentPageViewModel.CanSwitchPage)
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

        private void NextViewModel(IPageViewModel current, NavigateArgs e)
        {
            if (current.CanSwitchPage)
            {
                int current_index = PageViewModels.FindIndex(vm => vm == current);
                if (current_index + 1 != PageViewModels.Count)
                {
                    PageViewModels[current_index + 1].ProcessNavigateArgs(e);
                    CurrentPageViewModel = PageViewModels[current_index + 1];
                }
            }
        }

        private void PreviousViewModel(IPageViewModel current, NavigateArgs e)
        {
            int current_index = PageViewModels.FindIndex(vm => vm == current);
            if (current_index != 0)
            {
                PageViewModels[current_index - 1].ProcessNavigateArgs(e);
                CurrentPageViewModel = PageViewModels[current_index - 1];
            }
        }

        public void HanldeNextPageEvent(object sender, NavigateArgs e)
        {
            IPageViewModel vm = sender as IPageViewModel;
            NextViewModel(vm, e);
        }

        public void HanldePreviousPageEvent(object sender, NavigateArgs e)
        {
            IPageViewModel vm = sender as IPageViewModel;
            PreviousViewModel(vm, e);
        }
        #endregion
    }
}
