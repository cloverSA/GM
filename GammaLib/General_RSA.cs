using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GeneralUtility
{
    public class GammaRSASimplify
    {
        public string PublicKeyLoc { get; set; }
        public string PrivateKeyLoc { get; set; }

        public static void GenerateKeyFiles(string privKeyLoc, string pubKeyLoc)
        {
            var csp = new RSACryptoServiceProvider(2048);
            var privKey = csp.ExportParameters(true);
            SaveKeyToFile(privKeyLoc, privKey);
            var pubKey = csp.ExportParameters(false);
            SaveKeyToFile(pubKeyLoc, pubKey);
        }

        private static void SaveKeyToFile(string filepath, RSAParameters key)
        {
            var sw = new System.IO.StringWriter();
            var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
            xs.Serialize(sw, key);
            var keyString = sw.ToString();
            File.WriteAllText(filepath, keyString);
        }

        private static RSAParameters GetKeyFromFile(string filepath)
        {
            try
            {
                var content = File.ReadAllText(filepath);
                var sr = new System.IO.StringReader(content);
                var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
                return(RSAParameters)xs.Deserialize(sr);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static string RSAEncryptString(string inputStr, string publicKeyFile)
        {
            string cypherText = string.Empty;
            try
            {
                var pubKey = GetKeyFromFile(publicKeyFile);
                var csp = new RSACryptoServiceProvider();
                csp.ImportParameters(pubKey);
                var bytesPlainTextData = System.Text.Encoding.Unicode.GetBytes(inputStr);
                var bytesCypherText = csp.Encrypt(bytesPlainTextData, false);
                cypherText = Convert.ToBase64String(bytesCypherText);
            }
            catch (Exception ex)
            {
                throw;
            }
            return cypherText;
        }

        public static string RSADecryptString(string inputStr, string privateKeyFile)
        {
            string plainText = string.Empty;
            try
            {
                var bytesCypherText = Convert.FromBase64String(inputStr);
                var csp = new RSACryptoServiceProvider();
                var privKey = GetKeyFromFile(privateKeyFile);
                csp.ImportParameters(privKey);
                var bytesPlainTextData = csp.Decrypt(bytesCypherText, false);
                plainText = System.Text.Encoding.Unicode.GetString(bytesPlainTextData);
            }
            catch (Exception ex)
            {
                throw;
            }
            return plainText;
        }

    }
}
