using System;
using System.IO;
using System.Linq;
using System.Reflection;


namespace GammaAgent
{

    class GammaAgentInfo
    {
        public string HostIP
        {
            get; private set;
        }
        public string HostPort
        {
            get;
            private set;
        }
        static GammaAgentInfo agent_info = new GammaAgentInfo();

        public static GammaAgentInfo GetInstance()
        {
            return agent_info;
        }

        private GammaAgentInfo()
        {
            GetPublicNicFromConfig();
        }

        private void GetPublicNicFromConfig()
        {
            string[] lines;
            string loc = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            try
            {
                lines = System.IO.File.ReadAllLines(System.IO.Path.Combine(loc, "machine.conf"));
                foreach (string line in lines)
                {
                    if (line.ToLower().Contains("public"))
                    {
                        string[] tmp = line.Split('=');
                        if (tmp.Count() == 2)
                        {
                            HostIP = tmp[1].ToString().Trim();
                        }
                    }
                    if (line.ToLower().Contains("port"))
                    {
                        string[] tmp = line.Split('=');
                        if (tmp.Count() == 2)
                        {
                            HostPort = tmp[1].ToString().Trim();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Agent error, no network config file: {0}", ex.Message));    
            }

        }
    }

}
