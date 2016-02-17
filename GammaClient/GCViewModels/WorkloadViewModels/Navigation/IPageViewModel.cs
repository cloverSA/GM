using GammaClient.GCModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GammaClient.GCViewModels.WorkloadViewModels.Navigation
{
    public interface IPageViewModel
    {
        bool CanSwitchPage { get; set; }
        event EventHandler<NavigateArgs> NextPageEventHandler;
        event EventHandler<NavigateArgs> PreviousPageEventHandler;
        void RaiseNextPageEvent(object sender, NavigateArgs e);
        void RaisePreviousPageEvent(object sender, NavigateArgs e);
        void ProcessNavigateArgs(NavigateArgs args);

    }
}
