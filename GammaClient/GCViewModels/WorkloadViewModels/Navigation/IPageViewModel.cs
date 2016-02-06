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
    }
}
