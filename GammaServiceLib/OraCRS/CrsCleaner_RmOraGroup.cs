using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GammaServiceLib.OraCRS
{
    public partial class CrsCleaner : ICrsCleaner
    {
        public string RmOraGroup()
        {
            System.DirectoryServices.DirectoryEntry entryPC = new System.DirectoryServices.DirectoryEntry();
            entryPC.Path = string.Format("WinNT://{0}", System.Environment.MachineName);

            //int totalCount = entryPC.Children.OfType<System.DirectoryServices.DirectoryEntry>().ToList().Count();
            StringBuilder sb = new StringBuilder();
            foreach (System.DirectoryServices.DirectoryEntry child in entryPC.Children)
            {
                if (child.SchemaClassName == "Group")
                {
                    //sc.Post((obj) => this.StatusRTB.AppendText(string.Format("Checking Group: {0}\n", child.Name)), null);
                    if (child.Name.Trim().StartsWith("ORA_") || child.Name.Trim().StartsWith("ora_"))
                    {
                        entryPC.Children.Remove(child);
                        sb.AppendLine("Group" + child.Name.Trim() + "is removed");
                    }
                }

            }
            return sb.ToString();
        }
    }
}
