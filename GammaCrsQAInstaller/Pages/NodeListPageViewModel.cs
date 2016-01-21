using GammaCrsQAInstaller.Helper;
using GammaCrsQAInstaller.RemoteSetup;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GammaCrsQAInstaller.Pages
{
    class NodeListPageViewModel : ObservableObject, IPageViewModel
    {
        public string Name
        {
            get
            {
                return "Node List";
            }
        }

        public ObservableCollection<Node> NodeList { get; set; }

        public NodeListPageViewModel()
        {
            if (SetupInfo.HasKey(SetupInfoKeys.NodeList))
            {
                NodeList = SetupInfo.GetValue<ObservableCollection<Node>>(SetupInfoKeys.NodeList);
            }
            else
            {
                NodeList = new ObservableCollection<Node>();
            }
        }

        private bool NodelistValidator()
        {
            var ok = true;

            if (NodeList.Count > 1)
            {
                foreach (var node in NodeList)
                {
                    int currounces = 0;
                    foreach (var another in NodeList)
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
            else if (NodeList.Count == 0)
            {
                ok = false;
            }
            return ok;
        }

        public void SaveContent()
        {
            if (NodelistValidator())
            {
                SetupInfo.SetValue(SetupInfoKeys.NodeList, this.NodeList);
            } else
            {
                MessageBox.Show("Invalid input of node information.");
            }
        }

        public ICommand SaveContentCommand { get { return new RelayCommand(SaveContent); } }

    }
}
