using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace GeneralUtility
{
    public class DiskPartUtility
    {
        Dictionary<String, List<String>> partInfo = null;
        Dictionary<String, String> diskInfo = null;
        Process diskpart_session;

        public DiskPartUtility()
        {
            partInfo = new Dictionary<String, List<String>>();
            diskInfo = new Dictionary<String, String>();
            DiskPart_DiskCount();
            DiskPart_ReadPart();
        }

        private void DiskPart_ReadPart()
        {
            DiskPart_Init();
            List<String> diskNumber = new List<String>();//store the order the foreach below for disks.
            int currentDisk = -1;
            foreach (KeyValuePair<String, String> pair in diskInfo)
            {
                diskNumber.Add(pair.Key);
                diskpart_session.StandardInput.WriteLine(String.Format("select disk {0}", pair.Key));
                diskpart_session.StandardInput.WriteLine("list part");
            }
            diskpart_session.StandardInput.WriteLine("exit");
            String output = diskpart_session.StandardOutput.ReadToEnd().Trim();
            //Console.WriteLine(output);
            DiskPart_Close();
            //Process the result 
            Regex regex_diskinfo = new Regex(@"Partition\s(\d+).*Logical.*?(\d+)\sGB", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            MatchCollection matches = regex_diskinfo.Matches(output);
            if (matches.Count != 0)
            {
                foreach (Match m in matches)
                {
                    //Console.WriteLine(m.Value);
                    if (m.Groups[1].Value.Trim() == "1")
                    {
                        currentDisk += 1;
                        partInfo[diskNumber[currentDisk]] = new List<String>();
                    }
                    partInfo[diskNumber[currentDisk]].Add(m.Groups[2].Value);
                }
            }
        }
        private void DiskPart_Init()
        {
            diskpart_session = new Process();
            diskpart_session.StartInfo.UseShellExecute = false;
            diskpart_session.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            diskpart_session.StartInfo.RedirectStandardOutput = true;
            diskpart_session.StartInfo.FileName = String.Format(@"{0}\System32\diskpart.exe", Environment.GetFolderPath(Environment.SpecialFolder.Windows));
            diskpart_session.StartInfo.RedirectStandardInput = true;
            diskpart_session.Start();
        }
        private void DiskPart_DiskCount()
        {
            DiskPart_Init();
            diskpart_session.StandardInput.WriteLine("rescan");
            diskpart_session.StandardInput.WriteLine("list disk");
            diskpart_session.StandardInput.WriteLine("exit");
            String output = diskpart_session.StandardOutput.ReadToEnd().Trim();

            DiskPart_Close();

            //Process the result 
            Regex regex_diskinfo = new Regex(@"Disk\s(\d+).*Online.*?(\d+)\sGB", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            MatchCollection matches = regex_diskinfo.Matches(output);
            //set disk info
            if (matches.Count != 0)
            {
                for (int i = 1; i < matches.Count; i++)
                {
                    diskInfo[matches[i].Groups[1].Value] = matches[i].Groups[2].Value;
                }
            }
        }
        public String DiskPart_ReCreate()
        {
            String cmd = "";
            DiskPart_Init();
            foreach (KeyValuePair<String, List<String>> pair in partInfo)
            {
                cmd += String.Format("SELECT DISK {0}\n", pair.Key);
                cmd += "CLEAN\n";
                cmd += "ATTRIBUTES DISK CLEAR READONLY\n";
                cmd += "CONVERT MBR\n";
                cmd += "CREATE PARTITION EXTENDED\n";

                diskpart_session.StandardInput.WriteLine(String.Format("SELECT DISK {0}", pair.Key));
                diskpart_session.StandardInput.WriteLine("CLEAN");
                diskpart_session.StandardInput.WriteLine("ATTRIBUTES DISK CLEAR READONLY");
                diskpart_session.StandardInput.WriteLine("CONVERT MBR");
                diskpart_session.StandardInput.WriteLine("CREATE PARTITION EXTENDED");
                foreach (String logical_size in pair.Value)
                {
                    cmd += String.Format("CREATE PARTITION LOGICAL SIZE {0}\n", int.Parse(logical_size) * 1024);
                    diskpart_session.StandardInput.WriteLine(String.Format("CREATE PARTITION LOGICAL SIZE {0}", int.Parse(logical_size) * 1024));
                }

            }
            diskpart_session.StandardInput.WriteLine("exit");
            DiskPart_Close();
            return cmd;
        }

        public String DiskPart_RmDrvLtr()
        {
            String cmd = "";
            DiskPart_Init();
            diskpart_session.StandardInput.WriteLine("rescan");
            diskpart_session.StandardInput.WriteLine("list vol");
            diskpart_session.StandardInput.WriteLine("exit");
            String output = diskpart_session.StandardOutput.ReadToEnd().Trim();

            DiskPart_Close();
            Regex regex_diskinfo = new Regex(@"Volume\s(\d+).*Raw", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            MatchCollection matches = regex_diskinfo.Matches(output);
            DiskPart_Init();
            foreach (Match m in matches)
            {
                cmd += String.Format("select vol {0}\n", m.Groups[1].Value);
                cmd += "remove\n";
                diskpart_session.StandardInput.WriteLine(String.Format("select vol {0}", m.Groups[1].Value));
                diskpart_session.StandardInput.WriteLine(String.Format("remove"));
            }
            diskpart_session.StandardInput.WriteLine("exit");
            DiskPart_Close();
            return cmd;

        }

        private void DiskPart_Close()
        {
            diskpart_session.WaitForExit();
            diskpart_session.Close();
            diskpart_session.Dispose();
        }

    }
}
