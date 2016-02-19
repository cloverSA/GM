
using GalaSoft.MvvmLight.CommandWpf;
using GammaClient.GCViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GammaClient
{
    class MainWindowViewModel
    {
        private void CheckNodeMgr(object sender)
        {
            var fo = sender as NodeManagerFlyout;
            fo.IsOpen = !fo.IsOpen;
        }

        private ICommand _checkNodeMgrCommand;
        public ICommand CheckNodeMgrCommand {
            get
            {
                return this._checkNodeMgrCommand ?? (this._checkNodeMgrCommand = new RelayCommand<object>(CheckNodeMgr));
            }
        }
    }
}
