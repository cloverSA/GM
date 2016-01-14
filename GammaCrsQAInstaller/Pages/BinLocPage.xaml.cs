using GammaCrsQAInstaller.RemoteSetup;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace GammaCrsQAInstaller.Pages
{
    /// <summary>
    /// Interaction logic for BinLocPage.xaml
    /// </summary>
    public partial class BinLocPage : Page
    {
        public BinLocPage()
        {
            InitializeComponent();
        }

        private void BinLocNextPageExecuteHandler(object sender, ExecutedRoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri((string)e.Parameter, UriKind.Relative));
        }

        private void BinLocNextPageCanExecuteHandler(object sender, CanExecuteRoutedEventArgs e)
        {
            if(SrvLocBinTB.Text.Trim().Length !=0 && SrvRemoteBinTB.Text.Trim().Length != 0)
            {
                e.CanExecute = true;
            }
        }

        private void NextBtn_Click(object sender, RoutedEventArgs e)
        {
            var xcopy_template_file = System.IO.Path.Combine(SrvLocBinTB.Text.Trim(), "machine.conf");
            if (!System.IO.File.Exists(xcopy_template_file))
            {
                System.IO.File.WriteAllText(xcopy_template_file, "xcopy template");
            }
            SetupInfo.SetValue(SetupInfoKeys.SrvLocBin, SrvLocBinTB.Text.Trim());
            SetupInfo.SetValue(SetupInfoKeys.SrvRemoteBin, SrvRemoteBinTB.Text.Trim());
        }
    }
}
