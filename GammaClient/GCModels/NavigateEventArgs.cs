using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GammaClient.GCModels
{
    public class NavigateArgs : EventArgs
    {
        private object _item;

        public object Item
        {
            get
            {
                return _item;
            }

            set
            {
                _item = value;
            }
        }

        public NavigateArgs(object parms)
        {
            _item = parms;
        }
    }
}
