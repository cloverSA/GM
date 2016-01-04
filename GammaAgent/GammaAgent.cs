using System;
using System.ServiceProcess;
//GammaServiceLib
using GammaStressAgent.BaseService;

namespace GammaAgent
{
    public partial class GammaAgent : ServiceBase
    {
        private StresserCommand cpuLoader = null;
        private StresserCommand memLoader = null;
        private IGammaServerManager server_manager = null;

        public GammaAgent()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            //StresserStart();
            server_manager = GammaServerManagerFactory.GetServerManager();
            server_manager.ServerOpen();
        }

        private void StresserStart()
        {
            StressItemFactory factory = new GammaStressItemFactory();
            cpuLoader = factory.GetCpuStresser(this.ServiceName);
            memLoader = factory.GetMemStresser(this.ServiceName);
            cpuLoader.Execute();
            memLoader.Execute();
        }

        private void StresserStop()
        {
            if (cpuLoader != null)
            {
                bool rs = cpuLoader.Undo();
                Logger.WriteAppLog(this.ServiceName, String.Format("Cpu loader is stopped: {0}", rs));
            }
            if (memLoader != null)
            {
                bool rs = cpuLoader.Undo();
                Logger.WriteAppLog(this.ServiceName, String.Format("Mem loader is stopped: {0}", rs));
            }
        }

        protected override void OnStop()
        {
            this.RequestAdditionalTime(100000);
            //In service stop, it is the rare condition that you may need to force sync wait on async task to shutdown things cleanly.
            //should not use await in here.
            //StresserStop();
            server_manager.ServerClose();
        }

        /*
        private const int test_counter = 222;
        protected override void OnCustomCommand(int command)
        {
            if(command == test_counter)
            {
                //string rs = GammaUtility.ShellExecutor("cmd.exe ", "/c acfsutil info fs");
                //Logger.WriteAppLog(this.ServiceName, rs);
                string rs = GammaUtility.ShellExecutor("acfsutil ", "info fs");
                System.IO.File.WriteAllText(@"C:\temp\a.txt", rs);
            }
        }
        */

    }
}
