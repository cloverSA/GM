using System.IO;
using GeneralUtility;
using System.Reflection;
using System;

namespace GammaCrsQAInstaller.RemoteSetup
{
    class GammaServiceSetup : RemoteServiceSetup
    {
        private string agent_profile = @"machine.conf";
        public string ClientProfile { get; private set; }
        public GammaServiceSetup()
        {
            ClientProfile = "machines.conf";
            ServiceBinName = "GammaAgent.exe";
            ServiceDisplayName = "Gamma Agent";
            ServiceName = "GammaAgent";
        }

        protected override bool HasExtraTask()
        {
            return true;
        }

        protected override void ExtraTask(string host)
        {
            var pwd = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var netpath =  GammaFSUtility.ConvertPathToNetPath(host,Path.Combine(this.Target_location, agent_profile));
            var result = GammaUtility.ShellExecutor("xcopy.exe", string.Format(@" ""{0}"" ""{1}"" /Y", System.IO.Path.Combine(pwd, host),netpath));
            OnOpCompleted(new OpResultArgs() { OpResult = result, Hostname = host, OpType = "prepare agent profile" });

        }
    }

}
