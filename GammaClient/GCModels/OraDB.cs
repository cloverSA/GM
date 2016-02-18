using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GammaClient.GCModels
{
    public enum WorkLoad
    {
        Swingbench,
        Aroltp,
    }
    class OraDB : ObservableObject, IOraDB
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
                RaisePropertyChanged("DBName");
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
                RaisePropertyChanged("DBHome");
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
                RaisePropertyChanged("DBID");
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
                RaisePropertyChanged("IsSelected");
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
                RaisePropertyChanged("WorkLoad");
            }
        }


        public OraDB(int dbid)
        {
            _dbid = dbid;
        }

    }
}
