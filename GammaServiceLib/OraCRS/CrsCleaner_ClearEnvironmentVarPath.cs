using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GammaServiceLib.OraCRS
{
    public partial class CrsCleaner : ICrsCleaner
    {
        private void ClearEnvironmentVarPath()
        {
            string clean_token = @"c:\app";
            string pathVar = null;
            string[] pathValues = null;
            string pathFinal = "";
            bool changed = false;
            pathVar = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Machine);
            if (pathVar != null)
            {
                pathValues = pathVar.Split(';');
            }
            foreach (string tmp in pathValues)
            {
                if (!tmp.Trim().ToLower().Contains(clean_token))
                {
                    pathFinal += tmp.Trim();
                    pathFinal += ";";
                }
                else
                {
                    changed = true;
                }
            }

            if (changed)
            {
                Environment.SetEnvironmentVariable("PATH", pathFinal, EnvironmentVariableTarget.Machine);

            }


            pathVar = Environment.GetEnvironmentVariable("PERL5LIB", EnvironmentVariableTarget.Machine);
            if (pathVar != null)
            {
                Environment.SetEnvironmentVariable("PERL5LIB", "", EnvironmentVariableTarget.Machine);
            }
        }
    }
}
