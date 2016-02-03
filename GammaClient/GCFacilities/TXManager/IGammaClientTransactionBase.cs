using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GammaClient.GCFacilities.TXManager
{
    enum GammaTransactionType
    {
        COMMAND,
        TESTCASE,
        QATOOLS,
        CLEARENV,
    }

    enum GammaTransactionRC
    {
        SUCCEED,
        FAIL,
        RUNNING,
        UNKNOWN,
    }

    enum GammaTXQATools
    {
        UPLOAD,
        GETLOG,
        CLEARLOG,
    }

    enum GammaTXClearEnv
    {
        REG,
        FILE,
        GROUP,
        DISK,
        DRIVER,
        REBOOT,
    }

    interface IGammaClientTransactionBase
    {
        string Machine
        {
            get; set;
        }
        GammaTransactionType TX_TYPE
        {
            get; set;
        }
        GammaTransactionRC TX_RESULT_CODE
        {
            get; set;
        }
        string TX_RESULT
        {
            get; set;
        }
        string TX_CONTENT
        {
            get; set;
        }
        ulong TX_ID
        {
            get; set;
        }
        int TX_SUB_TYPE
        {
            get; set;
        }
    }
}
