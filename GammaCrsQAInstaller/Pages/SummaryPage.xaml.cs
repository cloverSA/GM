using GammaCrsQAInstaller.RemoteSetup;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for SummaryPage.xaml
    /// </summary>
    public partial class SummaryPage : Page
    {
        private bool _can_install = false;
        public SummaryPage()
        {
            InitializeComponent();
            SummaryLabel.Content = GenerateSummaryInfo();
        }

        private string GenerateSummaryInfo()
        {
            var sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine("Service Binary Location: " + SetupInfo.GetValue<string>(SetupInfoKeys.SrvLocBin));
            sb.AppendLine("Installation Location: " + SetupInfo.GetValue<string>(SetupInfoKeys.SrvRemoteBin));
            sb.AppendLine("Domain : " + SetupInfo.GetValue<string>(SetupInfoKeys.LogonDomain));
            sb.AppendLine("Username : " + SetupInfo.GetValue<string>(SetupInfoKeys.LogonUser));
            sb.AppendLine();
            sb.AppendLine("Host information: ");
            var nodelist = SetupInfo.GetValue<ObservableCollection<Node>>(SetupInfoKeys.NodeList);
            foreach(var node in nodelist)
            {
                sb.AppendFormat("hostname:  " + node.Hostname);
                sb.AppendFormat("ip address: " + node.HostIP);
                sb.AppendFormat("port: " + node.HostPort);
                sb.AppendLine();
                sb.AppendLine();
            }
            return sb.ToString();
        }
        private void NextPage_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (_can_install)
            {
                this.NavigationService.Navigate(new Uri((string)e.Parameter, UriKind.Relative));
            }
            
        }

        private void NextPage_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }


        private void PreviousPage_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.NavigationService.CanGoBack)
            {
                this.NavigationService.GoBack();
            }
        }

        private void Preivous_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void InstallBtn_Click(object sender, RoutedEventArgs e)
        {
            _can_install = (MessageBox.Show("Start installation ?", "Setup", MessageBoxButton.YesNo) == MessageBoxResult.Yes) ? true : false;
        }
    }
}
