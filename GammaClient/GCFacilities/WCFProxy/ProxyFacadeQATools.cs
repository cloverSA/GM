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
        public static Task<string[]> UploadLogToSftp(string usr, string pwd, string bugnum, string location, Action<string> callback)
        {
            GeneralUtility.IUploadRecord upload_rec = new UploadRecord() { Usr = usr, Passwd = pwd, Target = bugnum, Source = location };
            var tx_mgr = GammaClientTXManagerFactory.GetGammaClientTXManager();
            var tasks = new List<Task<string>>();
            foreach (var m in from machine in NetworkManagerFactory.GetSimpleNetworkManager().Machines where machine.Alive == NodeState.Online && machine.IsSelected select machine)
            {
                tasks.Add(tx_mgr.StartQAToolsTransaction(m, GammaTXQATools.UPLOAD, upload_rec));
            }
            return Task.WhenAll(tasks);
        }

        public static Task CollectLog(bool collect_dmp)
        {
            var tasks = new List<Task<string>>();
            var tx_mgr = GammaClientTXManagerFactory.GetGammaClientTXManager();
            foreach (var m in from machine in NetworkManagerFactory.GetSimpleNetworkManager().Machines where machine.Alive == NodeState.Online && machine.IsSelected select machine)
            {
                var t = tx_mgr.StartQAToolsTransaction(m, GammaTXQATools.GETLOG, collect_dmp);
                tasks.Add(t);
            }
            return Task.WhenAll(tasks);
        }

        public static Task RmLog()
        {
            var tasks = new List<Task<string>>();
            var tx_mgr = GammaClientTXManagerFactory.GetGammaClientTXManager();
            foreach (var m in from machine in NetworkManagerFactory.GetSimpleNetworkManager().Machines where machine.Alive == NodeState.Online && machine.IsSelected select machine)
            {
                var t = tx_mgr.StartQAToolsTransaction(m, GammaTXQATools.CLEARLOG);
                tasks.Add(t);
            }
            return Task.WhenAll(tasks);
        }
    }
}
