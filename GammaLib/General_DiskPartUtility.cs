using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace GeneralUtility
{
    public class DiskPartUtility
    {
        Dictionary<string, List<string>> partInfo = null;
        Dictionary<string, string> diskInfo = null;
        Process diskpart_session;

        public DiskPartUtility()
        {
            diskInfo = DiskPart_DiskCount();
            partInfo = DiskPart_ReadPart();
        }

        private string DiskPart_ListPart(string num)
        {
            DiskPart_Init();
            //store the order the foreach below for disks.
            diskpart_session.StandardInput.WriteLine(string.Format("select disk {0}", num));
            diskpart_session.StandardInput.WriteLine("list part");
            diskpart_session.StandardInput.WriteLine("exit");
            string output = diskpart_session.StandardOutput.ReadToEnd().Trim();
            //Console.WriteLine(output);
            DiskPart_Close();
            return output;
        }

        private string DiskPart_ListDisk()
        {
            DiskPart_Init();
            diskpart_session.StandardInput.WriteLine("rescan");
            diskpart_session.StandardInput.WriteLine("list disk");
            diskpart_session.StandardInput.WriteLine("exit");
            string output = diskpart_session.StandardOutput.ReadToEnd().Trim();

            DiskPart_Close();
            return output;
        }

        private Dictionary<string, List<string>> DiskPart_ReadPart()
        {
            var the_part_info = new Dictionary<string, List<string>>();
            foreach (KeyValuePair<string, string> pair in diskInfo)
            {
                string output = DiskPart_ListPart(pair.Key);
                Regex regex_diskinfo = new Regex(@"Partition\s(\d+).*Logical.*?(\d+)\sGB", RegexOptions.Multiline | RegexOptions.IgnoreCase);

                MatchCollection matches = regex_diskinfo.Matches(output);
                Console.WriteLine(output);
                if (matches.Count != 0)
                {
                    the_part_info[pair.Key] = new List<string>();
                    foreach (Match m in matches)
                    {
                        the_part_info[pair.Key].Add(m.Groups[2].Value);
                    }
                }
            }
            return the_part_info;
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

        private Dictionary<string, string> DiskPart_DiskCount()
        {
            string output = DiskPart_ListDisk();
            diskInfo = new Dictionary<string, string>();
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
            return diskInfo;
        }

        public string DiskPart_ReCreate()
        {
            string cmd = "";
            DiskPart_Init();
            foreach (KeyValuePair<string, List<string>> pair in partInfo)
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
                foreach (string logical_size in pair.Value)
                {
                    cmd += String.Format("CREATE PARTITION LOGICAL SIZE {0}\n", int.Parse(logical_size) * 1024);
                    diskpart_session.StandardInput.WriteLine(String.Format("CREATE PARTITION LOGICAL SIZE {0}", int.Parse(logical_size) * 1024));
                }

            }
            diskpart_session.StandardInput.WriteLine("exit");
            DiskPart_Close();
            return cmd;
        }

        public string DiskPart_RmDrvLtr()
        {
            string cmd = "";
            DiskPart_Init();
            diskpart_session.StandardInput.WriteLine("rescan");
            diskpart_session.StandardInput.WriteLine("list vol");
            diskpart_session.StandardInput.WriteLine("exit");
            string output = diskpart_session.StandardOutput.ReadToEnd().Trim();

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
