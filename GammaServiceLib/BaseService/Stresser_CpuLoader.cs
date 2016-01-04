using System;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using System.Linq;

namespace GammaStressAgent.BaseService
{
    class CpuLoader : IDisposable, IStresser
    {
        //Show how to use CancellationTokenSource , TaskCompletionSource<T>
        private int cpu_lower_bond = 40;
        private int cpu_upper_bond = 20;
        private string log_source = "GammaCpu";
        private int tracking_spans = 15;
        private bool quick_mem;
        private CancellationTokenSource stop_load_source;
        private CancellationToken stop_load;
        private TaskCompletionSource<bool> load_complete;

        private float GetCurrentCpuUsage()
        {
            PerformanceCounter cpuCounter;
            cpuCounter = new PerformanceCounter();

            cpuCounter.CategoryName = "Processor";
            cpuCounter.CounterName = "% Processor Time";
            cpuCounter.InstanceName = "_Total";
            float firstValue = cpuCounter.NextValue();
            System.Threading.Thread.Sleep(1000);
            // now matches task manager reading
            float secondValue = cpuCounter.NextValue();

            return secondValue;

        }

        private Int64 GetPhysicalProcessor()
        {
            String rs = null;
            foreach (var item in new System.Management.ManagementObjectSearcher("Select NumberOfProcessors from Win32_ComputerSystem").Get())
            {
                rs = item["NumberOfProcessors"].ToString();
            }
            return Int64.Parse(rs);

        }

        private void CpuEater(CancellationTokenSource cts, int percentage = 5)
        {
            try
            {
                cts.Token.ThrowIfCancellationRequested();
            }
            catch
            {
                return;
            }

            for (int i = 0; i < Environment.ProcessorCount; i++)
            {
                Task.Run(() =>
                {
                    try
                    {
                        cts.Token.ThrowIfCancellationRequested();
                    }
                    catch
                    {
                        return;
                    }
                    Stopwatch watch = new Stopwatch();
                    watch.Start();
                    while (true)
                    {
                        // Make the loop go on for "percentage" milliseconds then sleep the 
                        // remaining percentage milliseconds. So 40% utilization means work 40ms and sleep 60ms
                        if (watch.ElapsedMilliseconds > percentage)
                        {
                            Thread.Sleep(100 - percentage);
                            watch.Reset();
                            watch.Start();
                        }
                        if (cts.Token.IsCancellationRequested)
                        {
                            break;
                        }
                    }

                }, cts.Token);
            }

        }

        private void CpuHogerLight()
        {
            try
            {
                stop_load.ThrowIfCancellationRequested();
            }
            catch
            {
                return;
            }

            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.Normal;

            bool burn = true;
            float cpu_used;

            CancellationTokenSource control_cts = null;
            Task load_task = null;

            long cpu_mid_bond = (cpu_lower_bond + cpu_upper_bond) / 2;
            long to_be_used = cpu_mid_bond;
            if (Environment.OSVersion.Version.Major == 6)
            {
                if (Environment.OSVersion.Version.Minor >= 2)
                {
                    Logger.WriteAppLog(this.log_source, String.Format("[init] Environment 2k12! using upper bond: {0}", cpu_upper_bond));
                    to_be_used = (int)(cpu_upper_bond * 0.75);
                }
                else
                {
                    Logger.WriteAppLog(this.log_source, String.Format("[init] Environment 2k12! using cpu_mid_bond bond: {0}", cpu_mid_bond));
                    to_be_used = cpu_mid_bond;
                }

            }

            while (burn)
            {
                cpu_used = GetCurrentCpuUsage();
                //EventLog.WriteEntry(this.ServiceName, String.Format("A tracking task is started! Current load: {0}", cpu_used), EventLogEntryType.Information, 10);
                if (cpu_used < cpu_lower_bond)
                {
                    if (load_task != null && control_cts != null)
                    {
                        control_cts.Cancel();
                        load_task.Wait();
                        load_task.Dispose();
                        control_cts.Dispose();
                        load_task = null;
                        control_cts = null;
                    }
                    //Recollect current system load, and determines how much to add.
                    Thread.Sleep(TimeSpan.FromSeconds(2));

                    cpu_used = GetCurrentCpuUsage();
                    int added = (int)(to_be_used - cpu_used);

                    Logger.WriteAppLog(this.log_source, String.Format("A tracking task is started! Add load: {0}", added));
                    control_cts = new CancellationTokenSource();
                    load_task = Task.Run(() => CpuEater(control_cts, added), control_cts.Token);

                    Console.WriteLine(String.Format("Add load: {0}", added));
                }
                else if (Math.Floor(cpu_used) > cpu_upper_bond)
                {
                    if (load_task != null && control_cts != null)
                    {

                        Logger.WriteAppLog(this.log_source, String.Format("A tracking task is removed! Current load: {0}", cpu_used));
                        control_cts.Cancel();
                        load_task.Wait();
                        load_task.Dispose();
                        control_cts.Dispose();
                        load_task = null;
                        control_cts = null;
                    }

                }

                Thread.Sleep(TimeSpan.FromSeconds(tracking_spans));


                if (stop_load.IsCancellationRequested)
                {
                    Logger.WriteAppLog(this.log_source, String.Format("Shutting down tracking tasks! Current load: {0}", cpu_used));
                    if (load_task != null && control_cts != null)
                    {
                        control_cts.Cancel();
                        load_task.Wait();
                        load_task.Dispose();
                        control_cts.Dispose();
                        load_task = null;
                        control_cts = null;

                    }
                    load_complete.SetResult(true);
                    break;

                }
            }
        }

        public void Start()
        {
            Task.Run(() => CpuHogerLight());
        }

        public bool Stop()
        {
            stop_load_source.Cancel();
            //wait for clean shutdown
            Task<bool> load_task = load_complete.Task;
            bool rs = load_task.GetAwaiter().GetResult();
            return rs;
        }

        private void MandatoryInit()
        {
            stop_load_source = new CancellationTokenSource();
            stop_load = stop_load_source.Token;
            load_complete = new TaskCompletionSource<bool>();
            try
            {
                string[] lines = System.IO.File.ReadAllLines(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Gamma3", "agent", "config.ini"));
                foreach (string line in lines)
                {
                    string[] tmp = line.Split('=');
                    if (tmp != null && tmp.Count() == 2)
                    {
                        if (tmp[0].ToLower().Equals("cpu_upper"))
                        {
                            this.cpu_upper_bond = Int32.Parse(tmp[1]);
                            Logger.WriteAppLog(this.log_source, String.Format("[init] workload upper: {0}", cpu_upper_bond));
                        }
                        else if (tmp[0].ToLower().Equals("cpu_lower"))
                        {
                            this.cpu_lower_bond = Int32.Parse(tmp[1]);
                            Logger.WriteAppLog(this.log_source, String.Format("[init] workload lower: {0}", cpu_upper_bond));
                        }
                        else if (tmp[0].ToLower().Equals("track_interval"))
                        {
                            this.tracking_spans = Int32.Parse(tmp[1]);
                            Logger.WriteAppLog(this.log_source, String.Format("[init] workload  check interval: {0}", tracking_spans));
                        }
                        else if (tmp[0].ToLower().Equals("quick_mem"))
                        {
                            Int64 t = Int64.Parse(tmp[1]);
                            if (t == 1)
                            {
                                this.quick_mem = true;
                            }
                            else
                            {
                                this.quick_mem = false;
                            }
                            Logger.WriteAppLog(this.log_source, String.Format("[init] workload  quick memory: {0}", this.quick_mem.ToString()));
                        }

                    }
                }
            }
            catch (Exception e)
            {
                Logger.WriteAppError(this.log_source, String.Format("Init file not exist! Exception: {0}", e.Message));
            }
        }

        public void Dispose()
        {
            ((IDisposable)stop_load_source).Dispose();
        }

        public CpuLoader()
        {
            this.MandatoryInit();
        }

        public CpuLoader(String source)
        {
            this.log_source = source;
            this.MandatoryInit();
        }
    }


}