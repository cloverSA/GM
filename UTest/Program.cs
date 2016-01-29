using GammaServiceLib.OraCRS;
using GeneralUtility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace UTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //SLibTest.Test();
            SLibTest.Swingbench();
            Console.ReadLine();
        }
    }

    class SLibTest
    {
        public static string GetInventory()
        {
            var t = new CrsEnv();
            return t.GetClusterNames();
        }

        public static string ClearLog()
        {
            var t = new QATools();
            return t.ClearLog();
        }

        public static string GetLog(){
            var t = new QATools();
            return t.GetLog(false);
        }

        public static void Test(string test)
        {
            GammaRSASimplify.GenerateKeyFiles(@"C:\temp\prv", @"C:\temp\pub");
            var enresult = GammaRSASimplify.RSAEncryptString(test, @"C:\temp\pub");
            var deresult = GammaRSASimplify.RSADecryptString(enresult, @"C:\temp\prv");
        }

        public static void Swingbench()
        {
            var sw = new Swingbench(){
                DBDirName ="swingbench",
                DBHost ="rwsam13.us.oracle.com",
                DBPumpUser ="system",
                DBPumpPwd ="oracle",
                SwingbenchDmpDir=@"C:\swingbench\MyDmp\soe_dmp",
                SwingbenchDmpFilename ="SOE.DMP",
                SwingbenchDmpFilePath =Path.Combine(@"C:\swingbench\MyDmp\soe_dmp", "SOE.DMP"),
                TargetDBHome=@"C:\app\cdctest\product\12.2.0\dbhome_1",
                TargetDBName ="raca",
                SysPwd ="oracle",
                SysUsr ="sys"};
            sw.InstallWorkload();
        }
    }
}


namespace RsaCryptoExample
{
  static class Program
  {
    static void Main2()
    {
      //lets take a new CSP with a new 2048 bit rsa key pair
      var csp = new RSACryptoServiceProvider(2048);

      //how to get the private key
      var privKey = csp.ExportParameters(true);

      //and the public key ...
      var pubKey = csp.ExportParameters(false);

      //converting the public key into a string representation
      string pubKeyString;
      {
        //we need some buffer
        var sw = new System.IO.StringWriter();
        //we need a serializer
        var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
        //serialize the key into the stream
        xs.Serialize(sw, pubKey);
        //get the string from the stream
        pubKeyString = sw.ToString();
      }

      //converting it back
      {
        //get a stream from the string
        var sr = new System.IO.StringReader(pubKeyString);
        //we need a deserializer
        var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
        //get the object back from the stream
        pubKey = (RSAParameters)xs.Deserialize(sr);
      }

      //conversion for the private key is no black magic either ... omitted

      //we have a public key ... let's get a new csp and load that key
      csp = new RSACryptoServiceProvider();
      csp.ImportParameters(pubKey);

      //we need some data to encrypt
      var plainTextData = "foobar";

      //for encryption, always handle bytes...
      var bytesPlainTextData = System.Text.Encoding.Unicode.GetBytes(plainTextData);

      //apply pkcs#1.5 padding and encrypt our data 
      var bytesCypherText = csp.Encrypt(bytesPlainTextData, false);

      //we might want a string representation of our cypher text... base64 will do
      var cypherText = Convert.ToBase64String(bytesCypherText);


      /*
       * some transmission / storage / retrieval
       * 
       * and we want to decrypt our cypherText
       */

      //first, get our bytes back from the base64 string ...
      bytesCypherText = Convert.FromBase64String(cypherText);

      //we want to decrypt, therefore we need a csp and load our private key
      csp = new RSACryptoServiceProvider();
      csp.ImportParameters(privKey);

      //decrypt and strip pkcs#1.5 padding
      bytesPlainTextData = csp.Decrypt(bytesCypherText, false);

      //get our original plainText back...
      plainTextData = System.Text.Encoding.Unicode.GetString(bytesPlainTextData);
    }
  }
}