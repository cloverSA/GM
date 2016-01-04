using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GammaCrsQAInstaller.RemoteSetup
{
    class Node: IEquatable<Node>
    {
        public string Hostname { get; set; }
        public string HostIP { get; set; }
        private int port = 8090;
        public int HostPort { get { return port; } set { port = value; } }

        
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
