using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Linq;
using System.Windows.Input;
using System;
using GammaCrsQAInstaller.NetworkManager;
using GammaCrsQAInstaller.TXManager;

namespace GammaCrsQAInstaller
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private NodeNetManager nodeNetManager;
        private SynchronizationContext ui_sc = null;
        private static object locker = new object();
        private void InitTasks()
        {
            nodeNetManager = NodeNetManagerFactory.GetSimpleNetworkManager();
            this.MachinesDG.ItemsSource = nodeNetManager.Machines;
            nodeNetManager.StartNodeCheck();

        }

        public MainWindow()
        {
            InitializeComponent();
            InitTasks();
        }

        private void GoToPageExecuteHandler(object sender, ExecutedRoutedEventArgs e)
        {
            ToolsFrame.NavigationService.Navigate(new Uri((string)e.Parameter, UriKind.Relative));
        }

        private void GoToPageCanExecuteHandler(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

    }
}
