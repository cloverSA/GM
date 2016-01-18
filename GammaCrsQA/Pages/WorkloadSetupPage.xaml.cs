using GammaCrsQA.ViewModel;
using GammaCrsQA.Model;
using GammaCrsQA.NetworkManager;
using GammaCrsQA.WcfProxy;
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

namespace GammaCrsQA.Pages
{
    /// <summary>
    /// Interaction logic for WorkloadSetupPage.xaml
    /// </summary>
    public partial class WorkloadSetupPage : Page
    {
        public WorkloadSetupPage()
        {
            InitializeComponent();
        }

        private void NextPage_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            WorkloadSetupInfo.SetValue(WorkloadSetupKeys.CLUSTERS, GetClusterInfo());
            this.NavigationService.Navigate(new Uri((string)e.Parameter, UriKind.Relative));
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

        private async Task<ObservableCollection<Cluster>> GetClusterInfo()
        {
            ObservableCollection<Cluster> clusters = new ObservableCollection<Cluster>();
            var net_mgr = NodeNetManagerFactory.GetSimpleNetworkManager();
            var tasks = new List<Task>();
            var results = new List<string>();
            foreach (var machine in from m in net_mgr.Machines where m.Alive == NodeState.Online && m.IsSelected select m)
            {
                var proxy = GammaProxyFactory.GetCrsEnvProxy(machine);
                var rs = proxy.GetClusterNamesAsync();
                tasks.Add(rs.ContinueWith((t) => {
                    results.Add(t.Result);
                }));
            }
            await Task.WhenAll(tasks);
            int counter = 0;

            foreach (var rs in results.Distinct())
            {
                clusters.Add(new Cluster(counter) { ClusterName = rs.Trim() });
            }

            return clusters;
        }
    }
}
