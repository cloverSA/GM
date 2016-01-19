using GammaServiceLib.OraCRS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTest
{
    class Program
    {
        static void Main(string[] args)
        {
            SLibTest.GetInventory();
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
    }
}
