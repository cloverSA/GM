using GammaClient.GCFacilities.UIServiceProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GammaClient.GCFacilities.TXManager;
using GalaSoft.MvvmLight;
using GammaClient.GCFacilities.WCFProxy;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;

namespace GammaClient.GCViewModels
{
    class QAToolsViewModel : SPResultInTextBox
    {
        protected override void RaiseResultComeback(object sender, GammaUIUpdateArgs e)
        {
            if (e.TRANSACTION.TX_TYPE == GammaTransactionType.QATOOLS)
            {
                OpResult += string.Format("\r\n{0}: {1}\r\n", e.TRANSACTION.Machine, e.TRANSACTION.TX_RESULT);
            }
        }

        public void CollectLog()
        {
            //bool collect_dump = (MessageBox.Show("Collect system dump?", "Get Log", MessageBoxButton.YesNo) == MessageBoxResult.Yes) ? true : false;
            CanExec = false;
            var task = QAToolsFacade.CollectLog(false);
            task.GetAwaiter().OnCompleted(()=> {
                CanExec = true;
            });
        }

        public void RmLog()
        {
            CanExec = false;
            var task = QAToolsFacade.RmLog();
            task.GetAwaiter().OnCompleted(()=>{
                CanExec = true;
            });
        }

        public ICommand CollectLogCommand { get { return new RelayCommand(CollectLog); } }
        public ICommand RmLogCommand { get { return new RelayCommand(RmLog); } }
    }
}
