using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GammaClient.GCFacilities.TXManager
{
    class GammaUIUpdateArgs
    {
        public IGammaClientTransactionBase TRANSACTION
        {
            get; set;
        }
    }
}
