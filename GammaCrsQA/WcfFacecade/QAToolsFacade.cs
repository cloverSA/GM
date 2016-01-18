using GammaCrsQA.NetworkManager;
using GammaCrsQA.TXManager;
using GeneralUtility;
using System.Linq;

namespace GammaCrsQA.WcfFacecade
{
    class QAToolsFacade
    {
        public static void UploadLogToSftp(string usr, string pwd, string bugnum, string location) 
        {
            UploadRecord upload_rec = new UploadRecord() { Usr = usr, Passwd = pwd, Target = bugnum, Source = location };
            var tx_mgr = GammaClientTXManagerFactory.GetGammaClientTXManager();
            foreach (var m in from machine in NodeNetManagerFactory.GetSimpleNetworkManager().Machines where machine.Alive == NodeState.Online && machine.IsSelected select machine)
            {
                tx_mgr.StartQAToolsTransaction(m, GammaTXQATools.UPLOAD, upload_rec);
            }
            
        }

        public static void CollectLog(bool collect_dmp)
        {
            
            var tx_mgr = GammaClientTXManagerFactory.GetGammaClientTXManager();
            foreach (var m in from machine in NodeNetManagerFactory.GetSimpleNetworkManager().Machines where machine.Alive == NodeState.Online && machine.IsSelected select machine)
            {
                tx_mgr.StartQAToolsTransaction(m, GammaTXQATools.GETLOG, collect_dmp);
            }

        }

        public static void RmLog()
        {

            var tx_mgr = GammaClientTXManagerFactory.GetGammaClientTXManager();
            foreach (var m in from machine in NodeNetManagerFactory.GetSimpleNetworkManager().Machines where machine.Alive == NodeState.Online && machine.IsSelected select machine)
            {
                tx_mgr.StartQAToolsTransaction(m, GammaTXQATools.CLEARLOG);
            }

        }
    }
}
