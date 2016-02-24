namespace GammaAgent
{
    interface IGammaServerManager
    {
        void ServerOpen();
        void ServerClose();
    }

    class GammaServerManagerFactory
    {
        public static GammaServerManager GetServerManager()
        {
            return new GammaServerManager();
        }
    }

    class GammaServerManager : IGammaServerManager
    {
        IGammaSerivceHost crs_clean_service;
        IGammaSerivceHost qa_tools_service;
        IGammaSerivceHost shell_executor_service;
        IGammaSerivceHost crsenv_service;
        IGammaSerivceHost dbworkload_service;

        public GammaServerManager()
        {
            GammaServiceHostFactory factory = new GammaSimpleNetworkServiceHostFactory();
            crs_clean_service = factory.GetCrsCleanerSerivceHost();
            qa_tools_service = factory.GetQAToolsSerivceHost();
            shell_executor_service = factory.GetCmdExecutorSerivceHost();
            crsenv_service = factory.GetCrsEnvSerivceHost();
            dbworkload_service = factory.GetDBWorkloadSerivceHost();
        }

        public void ServerOpen()
        {
            crs_clean_service.HostOpen();
            qa_tools_service.HostOpen();
            shell_executor_service.HostOpen();
            crsenv_service.HostOpen();
            dbworkload_service.HostOpen();
        }
        public void ServerClose()
        {
            crs_clean_service.HostClose();
            qa_tools_service.HostClose();
            shell_executor_service.HostClose();
            crsenv_service.HostClose();
            dbworkload_service.HostClose();
        }
    }
}
