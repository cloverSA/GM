using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GammaCrsQA.Model
{
    enum WorkLoad
    {
        Swingbench,
        Aroltp,
    }
    class OraDB : INotifyPropertyChanged
    {
        private string _dbname;
        private string _dbhome;
        private int _dbid;

        public string DBName
        {
            get
            {
                return _dbname;
            }
            set
            {
                if (_dbname == value)
                {
                    return;
                }
                _dbname = value;
                RaisePropertyChangedEvent("DBName");
            }
        }
        public string DBHome
        {
            get
            {
                return _dbhome;
            }
            set
            {
                if (_dbhome == value)
                {
                    return;
                }
                _dbhome = value;
                RaisePropertyChangedEvent("DBHome");
            }
        }
        public int DBID
        {
            get
            {
                return _dbid;
            }
            set
            {
                if (_dbid == value)
                {
                    return;
                }
                _dbid = value;
                RaisePropertyChangedEvent("DBID");
            }
        }
        private bool _isSelected = false;
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                if (_isSelected == value)
                {
                    return;
                }
                _isSelected = value;
                RaisePropertyChangedEvent("IsSelected");
            }
        }
        private WorkLoad _load;
        public WorkLoad WorkLoad
        {
            get
            {
                return _load;
            }
            set
            {
                if (_load == value)
                {
                    return;
                }
                _load = value;
                RaisePropertyChangedEvent("WorkLoad");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public OraDB(int dbid)
        {
            _dbid = dbid;
        }
        private void RaisePropertyChangedEvent(string pname)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(pname));
            }
        }
    }
}
