using System;
using System.IO;
using System.IO.Compression;

namespace GeneralUtility
{
    public class Zipper : IDisposable
    {
        string ball_dir = @"C:\archive_info\";
        string ball_file = @"C:\archive_info\tmp.txt";
        string suffix = string.Empty;
        string zip_log = string.Empty;
        string tmp_loc = @"C:\";

        public Zipper(string suffix, string log)
        {
            this.suffix = suffix;
            this.zip_log = log;
        }

        public void RunZip(string source_dir, string target_zipfile)
        {
            CreateBallFile(target_zipfile);
            UpdateBallFile(source_dir, target_zipfile);
            GC.Collect();
        }

        public void UpdateLog(string content)
        {
            FileStream fs = null;
            if (File.Exists(zip_log))
            {
                fs = new FileStream(this.zip_log, FileMode.Append);
            }
            else
            {
                fs = new FileStream(this.zip_log, FileMode.CreateNew);
            }
            using (StreamWriter writer = new StreamWriter(fs))
            {
                writer.WriteLine(content);
            }
            fs.Close();
        }
        private void UpdateBallFile(string source_dir, string target_zipfile)
        {
            using (ZipArchive archive = ZipFile.Open(target_zipfile + suffix, ZipArchiveMode.Update))
            {
                foreach (string subdir in Directory.GetDirectories(source_dir, "*.*", SearchOption.AllDirectories))
                {
                    string relatedPath = subdir.Replace(source_dir, string.Empty);
                    string entry = relatedPath.Replace("\\", "/").Substring(1);
                    foreach (string file in Directory.GetFiles(subdir))
                    {
                        if (File.Exists(file))
                        {
                            FileInfo info = new FileInfo(file);
                            try
                            {
                                archive.CreateEntryFromFile(file, entry + "/" + info.Name);
                            }
                            catch
                            {
                                try
                                {
                                    string copied_item = Path.Combine(tmp_loc, info.Name);
                                    File.Copy(file, copied_item, true);
                                    archive.CreateEntryFromFile(copied_item, entry + "/" + info.Name);
                                    File.Delete(copied_item);
                                }
                                catch (Exception ex)
                                {
                                    UpdateLog(string.Format("Fails to zip: {0}, {1} ", file, ex.Message));
                                }
                            }
                            info = null;

                        }
                    }
                }
            }
        }


        private string GenArchiveEnry(string source_dir, string sub_dir)
        {
            string relatedPath = sub_dir.Replace(source_dir, string.Empty);
            string archiveEntry = string.Empty;
            archiveEntry = relatedPath.Replace("\\", "/");
            return archiveEntry.Substring(1);

        }
        private void CreateBallFile(string target_zipfile)
        {

            if (File.Exists(ball_file))
            {
                File.Delete(ball_file);
            }
            if (Directory.Exists(ball_dir))
            {
                Directory.Delete(ball_dir);
            }
            Directory.CreateDirectory(ball_dir);
            using (FileStream init_fs = File.Create(ball_file))
            {
                StreamWriter writer = new StreamWriter(init_fs);
                writer.WriteLine(Environment.MachineName + "_" + (DateTime.Now).ToString("yyyyMMddHHmmssffff"));
                writer.WriteLine("Target:" + target_zipfile + suffix);
                writer.WriteLine(Environment.MachineName + "_" + (DateTime.Now).ToString("yyyyMMddHHmmssffff"));

            }
            ZipFile.CreateFromDirectory(ball_dir, target_zipfile + suffix);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                //
            }

            disposed = true;
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private bool disposed = false;

    }
}
