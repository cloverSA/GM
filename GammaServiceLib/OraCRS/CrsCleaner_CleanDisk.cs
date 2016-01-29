using GeneralUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GammaServiceLib.OraCRS
{
    public partial class CrsCleaner : ICrsCleaner
    {
        public string CleanDisk()
        {
            DiskPartUtility dpu = new DiskPartUtility();
            return dpu.DiskPart_ReCreate();
        }

        public string RemoveDrvLtr()
        {
            DiskPartUtility dpu = new DiskPartUtility();
            return dpu.DiskPart_RmDrvLtr();

        }

        public void RestartComputer()
        {
            GammaUtility.ShellExecutor("shutdown.exe", "-r -t 0");
        }
    }
}
