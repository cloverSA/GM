using System.ServiceModel;
//using GeneralUtility;
using System.Runtime.Serialization;

namespace ASMServiceLib
{
    [ServiceContract()]
    public interface IListInfo
    {
        [OperationContract()]
        string ListDGinfo();
        [OperationContract()]
        string ListMemberDiskInfo(string DG_name);
        [OperationContract()]
        string ListCandidateDiskInfo();
        [OperationContract()]
        string ListOperation();
    }

    [ServiceContract()]
    public interface IDGOperation
    {
        [OperationContract()]
        string DGRebalance(string DG_name, int power, bool isWait);
        [OperationContract()]
        string ListDGAttributes(string DG_name);
        [OperationContract()]
        string SetDGAttribute(string DG_name, string attr_name, string attr_value);
        [OperationContract()]
        string MountDG(string DG_name, bool isAll, bool isRestrict, bool isForce);
        [OperationContract()]
        string DismountDG(string DG_name, bool isALL, bool isForce);
        [OperationContract()]
        string DropDG(string DG_name, bool isForce, bool isRecursive);
    }

    [ServiceContract()]
    public interface IDiskOperation
    {
        [OperationContract()]
        string AddDisk(string DG_name, string disk_name, string failgroup_name, bool isForce);
        [OperationContract()]
        string DropDisk(string DG_name, string disk_name, string failgroup_name, bool isForce);
        [OperationContract()]
        string OfflineDisk(string DG_name, string disk_name, string failgroup_name, string drop_time);
        [OperationContract()]
        string OnlineDisk(string DG_name, string disk_name, string failgroup_name, bool isAll, bool isWait, int power);
    }

    [ServiceContract()]
    public interface IFlexDGOperation
    {
        [OperationContract()]
        string ConvertRedundToFlex(string DG_name);
        [OperationContract()]
        string MakeFilegroup(string DG_name, string filegroup_name, string client_name, string client_type, string property_name, string proterty_value, string file_type);
        [OperationContract()]
        string ModifyFilegroup(string DG_name, string filegroup_name, string property_name, string property_value, string file_type);
        [OperationContract()]
        string ListFilegroup(string DG_name, string filegroup_name);
        [OperationContract()]
        string RemoveFilegroup(string DG_name, string filegroup_name, bool isRecursive);
        [OperationContract()]
        string MakeQuotagroup(string DG_name, string quotagroup_name, string property_name, string property_value);
        [OperationContract()]
        string ModifyQuotagroup(string DG_name, string quotagroup_name, string property_name, string property_value);
        [OperationContract()]
        string ListQuotagroup(string DG_name, string quotagroup_name);
        [OperationContract()]
        string RemoveQuotagroup(string DG_name, string quotagroup_name);
        [OperationContract()]
        string MoveFGtoQG(string DG_name, string quotagroup_name, string filegroup_name);
    }

    [ServiceContract()]
    public interface IASMCMDCommand
    {
        [OperationContract()]
        string RunASMCMD(string command);
    }
}
