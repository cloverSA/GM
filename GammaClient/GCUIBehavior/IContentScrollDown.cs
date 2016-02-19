using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GammaClient.GCUIBehavior
{
    public interface IContentScrollDown
    {
        ICommand ScrollDownCommand { get; }
        void ScrollDownResult(object sender);
    }
}
