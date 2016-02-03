using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GammaCrsQA.ViewModel
{
    /*
        If need navigation inside tabitem page, use this for the view model.
    */
    public abstract class NavigatorInTabPages<T> : TabPagesBaseView
    {
        #region Member

        private T _currentPageViewModel;
        private List<T> _pageViewModels;

        #endregion

        #region Property

        public T CurrentPageViewModel
        {
            get
            {
                return _currentPageViewModel;
            }

            set
            {
                _currentPageViewModel = value;
            }
        }

        public List<T> PageViewModels
        {
            get
            {
                return _pageViewModels;
            }

            set
            {
                _pageViewModels = value;
            }
        }

        public ICommand ChangePageCommand { get { return new RelayCommand<T>(p => ChangeViewModel((T)p)); } }

        public ICommand NextViewModelCommand { get { return new RelayCommand(NextViewModel); } }

        public ICommand PreviousViewModelCommand { get { return new RelayCommand(PreviousViewModel); } }

        #endregion

        #region Function

        private void ChangeViewModel(T viewModel)
        {
            /*
            The button bind the command parameter with the item in PageViewModels, and when click, it is being sent back here to identify the CurrentPage.
            Note CurrentPageViewModel is bind in datatemplate with its usercontrol(ui page).
            */
           CurrentPageViewModel = PageViewModels.FirstOrDefault(vm => vm.Equals(viewModel));
        }

        private void NextViewModel()
        {
            if (CanChangeCurrentPage())
            {
                int current_index = PageViewModels.FindIndex(vm => vm.Equals(CurrentPageViewModel));
                if (current_index + 1 != PageViewModels.Count)
                {
                    CurrentPageViewModel = PageViewModels[current_index + 1];
                }
            }
        }

        private void PreviousViewModel()
        {
            if (CanChangeCurrentPage())
            {
                int current_index = PageViewModels.FindIndex(vm => vm.Equals(CurrentPageViewModel));
                if (current_index != 0)
                {
                    CurrentPageViewModel = PageViewModels[current_index - 1];
                }
            }
        }

        /*hook to allow diy Can Execute, basically do some checking on the current page*/
        public virtual bool CanChangeCurrentPage()
        {
            return true;
        }

        #endregion

    }
}
