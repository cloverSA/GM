using System;
using System.IO;
using System.Runtime.Serialization;
using System.Security;
using System.Text;
using WinSCP;

namespace GeneralUtility
{
    public interface IUploader
    {
        string Upload(string source, string target);
        string Upload();
    }

    public class WinSCPUploader : IUploader
    {
        public UploadRecord Record { get; set; }

        string SshHostKeyFingerprint = "ssh-rsa 1024 8f:e7:e1:85:90:c8:33:15:eb:8a:b7:5d:4e:27:3c:3c";
        string BugSftp = "bugsftp.us.oracle.com";

        public WinSCPUploader(UploadRecord rec)
        {
            Record = rec;
            if (!Record.Usr.Contains(@"@oracle.com"))
            {
                Record.Usr +=  @"@oracle.com";
            }
        }

        public string Upload(string source, string target)
        {
            StringBuilder sb = new StringBuilder();

            try
            {
                using (Session session = new Session())
                {
                    SessionOptions sessionOptions = new SessionOptions
                    {
                        Protocol = Protocol.Sftp,
                        HostName = this.BugSftp,
                        UserName = this.Record.Usr,
                        Password = this.Record.Passwd,
                        SshHostKeyFingerprint = this.SshHostKeyFingerprint
                    };
                    TransferOptions transferOptions = new TransferOptions();
                    transferOptions.TransferMode = TransferMode.Binary;
                    transferOptions.ResumeSupport.State = TransferResumeSupportState.Off;

                    // Connect

                    session.Open(sessionOptions);
                    TransferOperationResult transferResult;
                    sb.AppendLine("Connection established");
                    if (Directory.Exists(source))
                    {
                        sb.AppendLine("Uploading the folder...");
                        transferResult = session.PutFiles(string.Format(@"{0}\*", source), string.Format(@"/{0}/", target), false, transferOptions);
                    }
                    else if (File.Exists(source))
                    {
                        sb.AppendLine("Uploading the file...");
                        transferResult = session.PutFiles(source, string.Format(@"/{0}/", target), false, transferOptions);
                    }
                    else
                    {
                        transferResult = null;
                        sb.AppendLine("The source file/dir is not found...");
                        throw new Exception(@"The file/dir is not found, please verify the location");
                    }

                    if (transferResult != null)
                    {
                        transferResult.Check();

                        foreach (TransferEventArgs transfer in transferResult.Transfers)
                        {
                            if (transfer.Error != null)
                            {
                                sb.AppendLine(transfer.FileName + " transfered failed.");
                            }
                            else
                            {
                                sb.AppendLine(transfer.FileName + " transfered success.");
                            }
                        }
                    } 
                    session.Close();
                }
            }
            catch (Exception ex)
            {
                sb.AppendLine(" Exception: "+ ex.Message);
            }
            return sb.ToString();

        }

        public string Upload()
        {
            return Upload(Record.Source, Record.Target);
        }
    }
    [DataContract]
    public class UploadRecord
    {
        [DataMember]
        public string Usr { get; set; }
        [DataMember]
        public string Passwd { get; set; }
        [DataMember]
        public string Target { get; set; }
        [DataMember]
        public string Source { get; set; }
    }
}
