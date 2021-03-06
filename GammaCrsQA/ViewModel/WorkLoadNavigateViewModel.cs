﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GammaCrsQA.ViewModel
{
    public class WorkLoadNavigateViewModel : TabPagesBaseView
    {
        #region Fields

        private ICommand _changePageCommand;

        private IPageViewModel _currentPageViewModel;
        private List<IPageViewModel> _pageViewModels;

        #endregion

        public WorkLoadNavigateViewModel()
        {
            // Add available pages
            PageViewModels.Add(new WorkLoadSetupViewModel());
            PageViewModels.Add(new WorkLoadClusterInfoViewModel());
            PageViewModels.Add(new WorkLoadDBInfoViewModel());
            PageViewModels.Add(new WorkLoadInstallViewModel());
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

        #endregion


    }
}
