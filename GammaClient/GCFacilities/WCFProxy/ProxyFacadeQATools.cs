using GammaClient.GCFacilities.NetworkManager;
using GammaClient.GCFacilities.TXManager;
using GammaServiceLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GammaClient.GCFacilities.WCFProxy
{
    class ProxyFacadeQATools
    {
    }
    class QAToolsFacade
    {
        public static void UploadLogToSftp(string usr, string pwd, string bugnum, string location)
        {
            GeneralUtility.IUploadRecord upload_rec = new UploadRecord() { Usr = usr, Passwd = pwd, Target = bugnum, Source = location };
            var tx_mgr = GammaClientTXManagerFactory.GetGammaClientTXManager();
            foreach (var m in from machine in NetworkManagerFactory.GetSimpleNetworkManager().Machines where machine.Alive == NodeState.Online && machine.IsSelected select machine)
            {
                tx_mgr.StartQAToolsTransaction(m, GammaTXQATools.UPLOAD, upload_rec);
            }

        }

        public static void CollectLog(bool collect_dmp)
        {

            var tx_mgr = GammaClientTXManagerFactory.GetGammaClientTXManager();
            foreach (var m in from machine in NetworkManagerFactory.GetSimpleNetworkManager().Machines where machine.Alive == NodeState.Online && machine.IsSelected select machine)
            {
                tx_mgr.StartQAToolsTransaction(m, GammaTXQATools.GETLOG, collect_dmp);
            }

        }

        public static void RmLog()
        {

            var tx_mgr = GammaClientTXManagerFactory.GetGammaClientTXManager();
            foreach (var m in from machine in NetworkManagerFactory.GetSimpleNetworkManager().Machines where machine.Alive == NodeState.Online && machine.IsSelected select machine)
            {
                tx_mgr.StartQAToolsTransaction(m, GammaTXQATools.CLEARLOG);
            }

        }
    }
}
