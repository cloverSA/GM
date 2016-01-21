﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Linq;
using System.Windows.Input;
using System;
using GammaCrsQA.NetworkManager;
using GammaCrsQA.TXManager;

namespace GammaCrsQA
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {

        private NodeNetManager nodeNetManager;
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
