using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GammaCrsQAInstaller.RemoteSetup
{
    static class SetupInfo
    {
        private static Dictionary<string, object> _setupinfo = new Dictionary<string, object>();

        public static void SetValue(string key, object value)
        {
            if (_setupinfo.ContainsKey(key))
            {
                _setupinfo.Remove(key);
            }
            _setupinfo.Add(key, value);
        }

        public static T GetValue<T>(string key)
        {
            if (_setupinfo.ContainsKey(key))
            {
                return (T)_setupinfo[key];
            }
            else
            {
                return default(T);
            }
        }

        public static bool HasKey(string key)
        {
            if (_setupinfo.ContainsKey(key))
            {
                return true;
            }else
            {
                return false;
            }
        }
    }

    static class SetupInfoKeys
    {
        public static string SrvLocBin = "SrvLocBin";
        public static string SrvRemoteBin  = "SrvRemoteBin";
        public static string LogonUser  = "LogonUser";
        public static string LogonPwd = "LogonPwd";
        public static string LogonDomain  = "LogonDomain";
        public static string SrvDisplayName  = "SrvDisplayName";
        public static string NodeList= "NodeList";

    }
}
