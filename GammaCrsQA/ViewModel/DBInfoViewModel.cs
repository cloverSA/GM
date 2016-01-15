using GammaCrsQA.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GammaCrsQA.ViewModel
{
    class DBInfoViewModel : INotifyPropertyChanged
    {
        public DBInfoViewModel()
        {
            DBs = new ObservableCollection<OraDB>() {

                        new OraDB(1) {DBName="DB1", DBHome=@"c:\", DBID=1},
                        new OraDB(2) {DBName="DB2", DBHome=@"c:\", DBID=2 },
                        new OraDB(3) {DBName="DB3", DBHome=@"c:\", DBID=3 }

                };
            WorkLoads = new ObservableCollection<string>() { "Swingbench", "Aroltp" };
        }

        public ObservableCollection<OraDB> DBs { get; set; }
        public ObservableCollection<string> WorkLoads { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
