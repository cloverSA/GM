using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GammaClient.GCModels
{
    public static class OraDBFactory
    {
        public static IOraDB GetOraDB(int id, string dbname, string dbhome)
        {
            var db = new OraDB(id) { DBHome = dbhome, DBName = dbname };
            return db;
        }
    }
}
