using System.ServiceModel;
using GeneralUtility;
namespace GammaServiceLib
{
    [ServiceContract()]
    public interface ICmdExecutor
    {
        [OperationContract()]
        string ShellExecutor(string filename, string arguments);
    }

    [ServiceContract()]
    public interface ICrsCleaner
    {
        [OperationContract()]
        string RemoveOraKeys();

        [OperationContract()]
        string RmOraGroup();

        [OperationContract()]
        string RemoveOraFiles();

        [OperationContract()]
        string CleanDisk();

        [OperationContract()]
        string RemoveDrvLtr();

        [OperationContract()]
        void RestartComputer();
    }

    [ServiceContract()]
    public interface IQATools
    {
        [OperationContract()]
        string ClearLog();

        [OperationContract()]
        string GetLog(bool collect_dump);

        [OperationContract()]
        string UploadLog(UploadRecord record);
    }

    [ServiceContract]
    public interface ICrsEnv
    {
        [OperationContract()]
        string GetClusterNames();

        [OperationContract()]
        string GetDBNames();

        [OperationContract()]
        string GetDBHOMEByName(string dbname);

        [OperationContract()]
        string GetScan();

        [OperationContract()]
        string GetScanPort();
    }
}
