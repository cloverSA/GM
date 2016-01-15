using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace GammaCrsQAInstaller.RemoteSetup
{
    class Node: IEquatable<Node>, INotifyPropertyChanged
    {
        private string _hostname;
        private string _ip;
        private int _port = 8090;

        public string Hostname {

            get
            {
                return _hostname;
            }
            set {
                _hostname = value;
                try
                {
                    var ipEntry = Dns.GetHostEntry(_hostname.Trim());
                    var addresses = ipEntry.AddressList;
                    if (addresses.Count() > 0)
                    {
                        HostIP = addresses[0].ToString();
                    }
                } catch(Exception ex)
                {
                    HostIP = "Not reachable";
                }
                
                RaisePropertyChangeEvent("Hostname");
            } }

        public string HostIP
        {
            get
            {
                return _ip;
            }
            set
            {
                _ip = value;
                RaisePropertyChangeEvent("HostIP");
            }
        }

        public int HostPort
        {
            get
            {
                return _port;
            }
            set
            {
                _port = value;
                RaisePropertyChangeEvent("HostPort");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChangeEvent(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        
        public bool Equals(Node other)
        {
            if (HostIP == null || Hostname == null || other.Hostname == null || other.HostIP == null)
            {
                return false;
            }
            if(
                Hostname.Trim()==other.Hostname.Trim()||
                HostIP.Trim() == other.Hostname.Trim())
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
