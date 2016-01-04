using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace GammaCrsQAInstaller.TXManager
{

    interface IGammaClientTXLogger
    {

        void RegisterTransaction(IGammaClientTransaction tx);

    }

    class GammaClientTXLogger : IGammaClientTXLogger
    {


        private readonly static object locker = new object();
        private static string pwd = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static string trx_log_loc = Path.Combine(pwd, "transaction.trc");
        private readonly static GammaClientTXLogger transaction_logger = new GammaClientTXLogger();

        private GammaClientTXLogger()
        {

            if (File.Exists(trx_log_loc))
            {
                File.Move(trx_log_loc, Path.Combine(pwd, string.Format("transaction_{0}.trc", (DateTime.Now).ToString("yyyyMMddHHmmssffff"))).ToString());
            }


        }

        public static GammaClientTXLogger GetInstance()
        {
            return transaction_logger;
        }

        public void RegisterTransaction(IGammaClientTransaction clientTrx)
        {
            clientTrx.OnTransactionStarted += ClientTrx_OnTransactionStarted;
            clientTrx.OnTransactionCompleted += ClientTrx_OnTransactionCompleted;
            clientTrx.OnTransactionFailed += ClientTrx_OnTransactionFailed;
        }

        private void ClientTrx_OnTransactionFailed(object sender, EventArgs e)
        {
            IGammaClientTransaction trx = sender as IGammaClientTransaction;
            StringBuilder sb = new StringBuilder();
            if (trx != null)
            {
                sb.AppendFormat("{0} : {1} : {2} : {3} : {4} : {5}",
                                            (DateTime.Now).ToString("yyyyMMddHHmmssffff"),
                                            trx.TX_ID,
                                            trx.TX_TYPE,
                                            trx.TX_RESULT_CODE,
                                            trx.Machine,
                                            trx.TX_RESULT.Trim());
                sb.AppendLine();
                LogInformation(sb.ToString());
            }
        }

        private void ClientTrx_OnTransactionCompleted(object sender, EventArgs e)
        {
            IGammaClientTransaction trx = sender as IGammaClientTransaction;
            StringBuilder sb = new StringBuilder();
            if (trx != null)
            {
                sb.AppendFormat("{0} : {1} : {2} : {3} : {4} : {5}",
                                            (DateTime.Now).ToString("yyyyMMddHHmmssffff"),
                                            trx.TX_ID,
                                            trx.TX_TYPE,
                                            trx.TX_RESULT_CODE,
                                            trx.Machine,
                                            trx.TX_RESULT.Trim());
                sb.AppendLine();
                LogInformation(sb.ToString());
            }
        }

        private void ClientTrx_OnTransactionStarted(object sender, EventArgs e)
        {
            IGammaClientTransaction trx = sender as IGammaClientTransaction;
            StringBuilder sb = new StringBuilder();
            if (trx != null)
            {
                sb.AppendFormat("{0} : {1} : {2} : {3} : {4} : {5}",
                                            (DateTime.Now).ToString("yyyyMMddHHmmssffff"),
                                            trx.TX_ID,
                                            trx.TX_TYPE,
                                            trx.TX_RESULT_CODE,
                                            trx.Machine,
                                            trx.TX_CONTENT.Trim());
                sb.AppendLine();
                LogInformation(sb.ToString());
            }

        }


        private void LogInformation(string text)
        {
            lock (locker)
            {
                try
                {
                    File.AppendAllText(trx_log_loc, text);
                }
                catch (Exception e)
                {
                    throw new Exception(string.Format("The logger fails to add text to the file: {0}\n", e.Message));
                }
            }
        }
    }
}
