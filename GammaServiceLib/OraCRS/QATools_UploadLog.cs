using GeneralUtility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GammaServiceLib.OraCRS
{
    public partial class QATools : IQATools
    {
        public string UploadLog(UploadRecord record)
        {
            var pwd = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            IUploader uploadder = new WinSCPUploader(record) { PrvKeyLoc = Path.Combine(pwd, "prvkf.xml") };
            string rs = uploadder.Upload();
            return rs;
        }
    }
}
