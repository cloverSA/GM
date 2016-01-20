using GammaCrsQA.WcfFacecade;
using GeneralUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace GammaCrsQA.Pages
{
    /// <summary>
    /// Interaction logic for UploadFilePage.xaml
    /// </summary>
    public partial class UploadFilePage : Page
    {
        public UploadFilePage()
        {
            InitializeComponent();
        }

        private void LogInBtn_Click(object sender, RoutedEventArgs e)
        {
            bool do_upload = (MessageBox.Show("Upload Now?", "Upload Log", MessageBoxButton.YesNo) == MessageBoxResult.Yes) ? true : false;
            if (do_upload)
            {
                var pwd = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                QAToolsFacade.UploadLogToSftp(SftpUserTB.Text.Trim(), GammaRSASimplify.RSAEncryptString(SftpPWDTB.Password, System.IO.Path.Combine(pwd, "pubkf.xml")), SftpLocTB.Text.Trim(), UploadPath.Text.Trim());
            }
        }
    }


}
