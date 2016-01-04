using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GeneralUtility
{
    public class RemoteServiceSetup
    {
        public string Domain { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Target_location { get; set; }
        public string Source_location { get; set; }
        public string ServiceName { get; set; }
        public string ServiceDisplayName { get; set; }
        public string ServiceBinName { get; set; }
        public List<string> Hosts { get; set; }
        private string localhost = System.Net.Dns.GetHostName();

        private void VerifyNodes()
        {
            var err_list = new List<string>();
            foreach (var host in Hosts)
            {
                if (!host.ToLower().Contains(localhost.ToLower()))
                {
                    var rs = GammaFSUtility.NetUse(host, Domain, Username, Password, Target_location.Trim().ToCharArray()[0]);
                    if (rs.Contains("error"))
                    {
                        err_list.Add(host);
                        OnOpCompleted(new OpResultArgs() { Hostname = host, OpResult = "net use fails, will not be configured", OpType = "net use setup" });
                    }
                }
            }
            Hosts = Hosts.Except(err_list).ToList();
        }
        private Task<string> RemoteCopyBinaries(string host)
        {
            //var source_dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            
            var t = Task.Run(() => {
                return GammaFSUtility.RemoteCopyFiles(host, Source_location, Target_location);
            });
            t.GetAwaiter().OnCompleted(() =>
            {
                OnOpCompleted(new OpResultArgs() { OpResult = t.Result, Hostname = host, OpType = "copy service file" });
            });
            return t;
        }

        //Installation take cares for some other situation.
        private Task<string> RemoteServiceInstall(string host)
        {
            //Be caution with the space in between the = and value, they are mandatory.
            var args = string.Format(@"create {0} binPath= ""{1}"" DisplayName= ""Gamma Agent"" start= auto", ServiceName, Path.Combine(Target_location, ServiceBinName));

            var t = Task.Run(() =>
            {
                var status = GammaUtility.ShellExecutor("sc.exe", string.Format(@"\\{0} query {1}", host, ServiceName));
                if (status.Contains("STATE"))
                {
                    return "Serivce already exist!";
                }
                else
                {
                    return GammaUtility.ShellExecutor("sc.exe", string.Format(@"\\{0} {1}", host, args));
                }

            });
            t.GetAwaiter().OnCompleted(() =>
            {
                OnOpCompleted(new OpResultArgs() { OpResult = t.Result, Hostname = host, OpType = "service setup" });
            });

            return t;
        }
        //Use for start/stop/query
        private Task<string> SvcMgrShellExecuteAsyncPrototype(string host, string args, Action<OpResultArgs> result_callback, string op_type, bool callback = true)
        {

            var t = Task.Run(() =>
            {
                return GammaUtility.ShellExecutor(@"sc.exe", String.Format(@"\\{0} {1}", host, args));
            });
            if (callback)
            {
                t.GetAwaiter().OnCompleted(() =>
                {
                    result_callback(new OpResultArgs() { OpResult = t.Result, Hostname = host, OpType = op_type });
                });
            }
            return t;
        }
        //Register to the handler to receive any result.
        public EventHandler<OpResultArgs> OnOpCompletedEventHandler;

        protected void OnOpCompleted(OpResultArgs args)
        {
            if (OnOpCompletedEventHandler != null)
            {
                OnOpCompletedEventHandler(this, args);
            }
        }

        public Task<string> RemoteServiceStart(string host, bool callback = true)
        {
            return SvcMgrShellExecuteAsyncPrototype(host, "start " + ServiceName, OnOpCompleted, "start service", callback);
        }

        public Task<string> RemoteServiceStop(string host, bool callback = true)
        {
            return SvcMgrShellExecuteAsyncPrototype(host, "stop " + ServiceName, OnOpCompleted, "stop service", callback);
        }
        public Task<string> RemoteServiceQuery(string host, bool callback = true)
        {
            return SvcMgrShellExecuteAsyncPrototype(host, "query " + ServiceName, OnOpCompleted, "query service", callback);
        }

        public Task<string> RemoteServiceDelete(string host, bool callback = true)
        {
            return SvcMgrShellExecuteAsyncPrototype(host, "delete " + ServiceName, OnOpCompleted, "query service", callback);
        }

        private async Task InstallSingleNode(string host, bool callback = true)
        {
            //Pre-req. Setup net use
            VerifyNodes();
            var status = await RemoteServiceQuery(host, false);
            string rs = string.Empty;

            //Pre-req. Clean service
            if (status.Contains("STATE"))
            {
                if (!status.Contains("STOPPED")) 
                {
                    rs = await RemoteServiceStop(host);
                    if (rs.Contains("error"))
                    {
                        return;
                    }
                }
                rs = await RemoteServiceDelete(host);
                if (rs.Contains("error"))
                {
                    return;
                }
            }

            //Step1. Copy file
            if (!host.ToLower().Contains(localhost.Trim().ToLower()))
            {
                rs = await RemoteCopyBinaries(host);
                if (rs.Contains("error"))
                {
                    return;
                }
            }
            else
            {
                //local node copy file
                if (!Source_location.Trim().ToLower().Equals(Target_location.Trim().ToLower()))
                {
                    GammaUtility.ShellExecutor(@"xcopy.exe", "/I /Y /E /V {0} {1}", Source_location, Target_location);
                }
            }
            
            //Step2. Install service
            rs = await RemoteServiceInstall(host);
            if (rs.Contains("error"))
            {
                return;
            }
            //Step extra, hook to do something else  before starting
            if (HasExtraTask())
            {
                ExtraTask(host);
            }
            //Step4. Start service
            rs = await RemoteServiceStart(host);
            if (rs.Contains("error"))
            {
                return;
            }
            await Task.Delay(TimeSpan.FromSeconds(1));
            //Step5. Query service
            await RemoteServiceQuery(host);
        }

        public Task AllInOne()
        {
            var tasks = new List<Task>();
            foreach (var host in Hosts)
            {
                tasks.Add(InstallSingleNode(host));
            }
            return Task.WhenAll(tasks);
        }

        protected virtual bool HasExtraTask()
        {
            return false;
        }

        protected virtual void ExtraTask(string host)
        {

        }
    }

    public class OpResultArgs
    {
        public string OpResult { get; set; }
        public string Hostname { get; set; }
        public string OpType { get; set; }
    }


}
