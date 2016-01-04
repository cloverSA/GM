using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GammaCrsQAInstaller.RemoteSetup
{
    class ProgressEventArgs
    {
        public int TotalSteps { get; private set; }
        public ProgressEventArgs(int steps)
        {
            TotalSteps = steps;
        }
    }
}
