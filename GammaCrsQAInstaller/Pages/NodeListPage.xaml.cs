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
    /// Interaction logic for NodeList.xaml
    /// </summary>
    public partial class NodeListPage : Page
    {
        private ObservableCollection<Node> nodelist { get; set; }
        private bool _can_goto_next = false;

        public NodeListPage()
        {
            InitializeComponent();
            if (SetupInfo.HasKey(SetupInfoKeys.NodeList))
            {
                nodelist = SetupInfo.GetValue<ObservableCollection<Node>>(SetupInfoKeys.NodeList);
            }
            else
            {
                nodelist = new ObservableCollection<Node>();
            }
            NodeListDG.ItemsSource = nodelist;
        }

        private void NextPage_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (_can_goto_next)
            {
                this.NavigationService.Navigate(new Uri((string)e.Parameter, UriKind.Relative));
            }
        }

        private void NextPage_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if(nodelist.Count > 0)
            {
                e.CanExecute = true;
            }
        }

        private bool NodelistValidator()
        {
            var ok = true;
            
            if (nodelist.Count > 1)
            {
                foreach(var node in nodelist)
                {
                    int currounces = 0;
                    foreach(var another in nodelist)
                    {
                        if (node.Equals(another))
                        {
                            currounces += 1;
                            if (currounces > 1)
                            {
                                break;
                            }
                        }
                    }
                    if (currounces > 1)
                    {
                        ok = false;
                        break;
                    }
                }
            }
            else if(nodelist.Count == 0)
            {   
                ok = false;
            }
            return ok;
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

        private void NextBtn_Click(object sender, RoutedEventArgs e)
        {
            if (NodelistValidator())
            {
                SetupInfo.SetValue(SetupInfoKeys.NodeList, this.nodelist);
                _can_goto_next = true;
            }
            else
            {
                MessageBox.Show("Please verify the input of the nodelist.!");
            }
        }
    }
}
