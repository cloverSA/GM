using GammaCrsQA.Model;
using GammaCrsQA.NetworkManager;
using GammaCrsQA.WcfProxy;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GammaCrsQA.ViewModel
{
    static class WorkloadSetupInfo
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
            }
            else
            {
                return false;
            }
        }
    }

    static class WorkloadSetupKeys
    {
        public static string CLUSTERS = "CLUSTER";
        public static string DBS = "DB";
    }
}
