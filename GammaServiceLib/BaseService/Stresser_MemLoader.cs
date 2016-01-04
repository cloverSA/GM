using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GammaStressAgent.BaseService
{
    class MemLoader : IDisposable, IStresser
    {
        private float GetAvailableRAM()
        {
            PerformanceCounter ramCounter;
            ramCounter = new PerformanceCounter("Memory", "Available MBytes");
            float firstValue = ramCounter.NextValue();
            System.Threading.Thread.Sleep(1000);
            // now matches task manager reading
            float secondValue = ramCounter.NextValue();
            return secondValue;
        }
        private CancellationTokenSource stop_load_source;
        private CancellationToken stop_load;
        private TaskCompletionSource<bool> load_complete;
        private int ram_upper_bond = 30;
        private int ram_lower_bond = 20;
        private string log_source = "GammaMem";
        private int tracking_spans = 15;
        

        public MemLoader(string source)
        {
            this.log_source = source;
            this.MandatoryInit();
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
                        if (tmp[0].ToLower().Equals("mem_lower"))
                        {
                            this.ram_lower_bond = Int32.Parse(tmp[1]);
                            Logger.WriteAppLog(this.log_source, String.Format("[init] workload ram lower: {0}", ram_lower_bond));
                        }
                        else if (tmp[0].ToLower().Equals("mem_upper"))
                        {
                            this.ram_upper_bond = Int32.Parse(tmp[1]);
                            Logger.WriteAppLog(this.log_source, String.Format("[init] workload ram upper: {0}", ram_upper_bond));
                        }
                        else if (tmp[0].ToLower().Equals("track_interval"))
                        {
                            this.tracking_spans = Int32.Parse(tmp[1]);
                            Logger.WriteAppLog(this.log_source, String.Format("[init] workload ram check interval: {0}", tracking_spans));
                        }
                        

                    }
                }
            }
            catch (Exception e)
            {
                Logger.WriteAppError(this.log_source, String.Format("Init file not exist! Exception: {0}", e.Message));
            }
        }
        public MemLoader()
        {
            this.MandatoryInit();
        }
        private void MemoryHog()
        {
            try
            {
                stop_load.ThrowIfCancellationRequested();
            }
            catch
            {
                return;
            }
            
            ComputerInfo cmp = new ComputerInfo();
            float machineMemoryMB = cmp.TotalPhysicalMemory / 1024 / 1024;
            float currentmemoryMB = 0;
            List<MemHold> memList = new List<MemHold>();

            float upper_mb = machineMemoryMB * (ram_upper_bond / 100);
            float lower_mb = machineMemoryMB * (ram_lower_bond / 100);
            
            while (true)
            {

                currentmemoryMB = machineMemoryMB - GetAvailableRAM();
                //Console.WriteLine("Current memory: {0}", currentmemoryMB);
                double rate = currentmemoryMB / machineMemoryMB * 100;
                if (rate < ram_lower_bond)
                //if(memList.Count() < 3)
                {

                    try
                    {
                        MemHold tmp = new MemHold();
                        memList.Add(tmp);
                        GC.Collect();
                        Logger.WriteAppLog(this.log_source, String.Format("An Agent is started! Total Agent: {0}", memList.Count()));
                    }
                    catch (Exception e)
                    {
                        Logger.WriteAppError(this.log_source, String.Format("Start of agent fails! Exception: {0}", e.Message), 101);
                    }

                }
                else if (rate > ram_upper_bond)
                {
                    if (memList.Count > 0)
                    {
                        //Console.WriteLine("Remove some mem");
                        MemHold tmp = memList[memList.Count - 1];
                        try
                        {
                            tmp.Remove();
                            memList.RemoveAt(memList.Count - 1);
                            GC.Collect();
                            Logger.WriteAppLog(this.log_source, String.Format("An Agent is removed! Total Agent: {0}", memList.Count()));
                        }
                        catch (Exception e)
                        {
                            Logger.WriteAppError(this.log_source, String.Format("Termination of agent fails! Exception: {0}", e.Message),  102);
                        }


                    }
                }

                Thread.Sleep(TimeSpan.FromSeconds(tracking_spans));
                
                if (stop_load.IsCancellationRequested)
                {
                    Logger.WriteAppLog(this.log_source, String.Format("Shutting down closing agents ! Total Agent: {0}", memList.Count()));
                    memList = null;
                    System.GC.Collect();
                    load_complete.SetResult(true);
                    break;
                }
            }
        }

        public void Start()
        {
            Task.Run(() => MemoryHog());
        }

        public bool Stop()
        {
            stop_load_source.Cancel();
            //wait for clean shutdown
            Task<bool> load_task = load_complete.Task;
            bool rs = load_task.GetAwaiter().GetResult();
            return rs;
        }

        public void Dispose()
        {
            ((IDisposable)stop_load_source).Dispose();
        }
    }

    class MemHold
    {
        private List<char> rs;
        public MemHold(int au = 1)
        {
            int gb = System.Int32.MaxValue / 4;
            int item_size = gb * au;
            rs = new List<char>();
            for (int i = 0; i < item_size; i++)
            {
                rs.Add('c');
            }
        }

        public void Remove()
        {
            rs.RemoveRange(0, rs.Count);
            rs = null;
            GC.Collect();
        }
    }
}
