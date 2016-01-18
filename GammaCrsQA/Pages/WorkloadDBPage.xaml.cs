﻿using System;
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

namespace GammaCrsQA.Pages
{
    /// <summary>
    /// Interaction logic for WorkloadDBPage.xaml
    /// </summary>
    public partial class WorkloadDBPage : Page
    {
        public WorkloadDBPage()
        {
            InitializeComponent();
        }

        private void NextPage_Executed(object sender, ExecutedRoutedEventArgs e)
        {
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
    }
}