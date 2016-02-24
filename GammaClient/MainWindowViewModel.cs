
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GammaClient.GCViews;
using MahApps.Metro;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace GammaClient
{
    class MainWindowViewModel : ObservableObject
    {
        private bool _toggleDark = false;
        private string _themeconfig = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "ui.conf");
        private IDictionary<string,string> _theme = new Dictionary<string, string>();
        private const string _themekey = "theme";
        private const string _colorkey = "color";
        
        private void OnThemeChanged(object s, string val)
        {
            _theme[_themekey] = val;
            StoreTheme();
        }

        private void OnColorChanged(object s, string val)
        {
            _theme[_colorkey] = val;
            StoreTheme();
        }

        private void CheckNodeMgr(object sender)
        {
            var fo = sender as NodeManagerFlyout;
            fo.IsOpen = !fo.IsOpen;
        }

        private ICommand _checkNodeMgrCommand;
        public ICommand CheckNodeMgrCommand {
            get
            {
                return this._checkNodeMgrCommand ?? (this._checkNodeMgrCommand = new RelayCommand<object>(CheckNodeMgr));
            }
        }

        private ICommand _storeThemeCommand;
        
        public List<AccentColorMenuData> AccentColors { get; private set; }

        public bool ToggleDark
        {
            get
            {
                return _toggleDark;
            }

            set
            {
                _toggleDark = value;
                if (_toggleDark)
                {
                    GetTheme("BaseDark").DoChangeTheme(this);
                } else
                {
                    GetTheme("BaseLight").DoChangeTheme(this);
                }
                RaisePropertyChanged("ToggleDark");
            }
        }

        public ICommand StoreThemeCommand
        {
            get
            {
                return this._storeThemeCommand ?? (this._storeThemeCommand = new RelayCommand(StoreTheme));
            }
        }

        private void StoreTheme()
        {
            var content = new List<string>();
            foreach(KeyValuePair<string, string>kvp in this._theme)
            {
                content.Add(string.Format("{0}={1}",kvp.Key, kvp.Value));
            }
            File.WriteAllLines(_themeconfig, content);
        }

        private AppThemeData GetTheme(string themeName, bool attach = true)
        {
            var rs = from theme in ThemeManager.AppThemes
                    where theme.Name.Contains(themeName)
                    select new AppThemeData() { Name = theme.Name, BorderColorBrush = theme.Resources["BlackColorBrush"] as Brush, ColorBrush = theme.Resources["WhiteColorBrush"] as Brush};
            var aTheme = rs.First();
            if (attach)
            {
                aTheme.OnThemeChangedEvent += this.OnThemeChanged;
            }
            
            return aTheme;
        }

        private AccentColorMenuData GetColor(string colorName, bool attach = true)
        {
            var rs = from color in ThemeManager.Accents
                    where color.Name.Contains(colorName)
                    select new AccentColorMenuData() { Name = color.Name, ColorBrush = color.Resources["AccentColorBrush"] as Brush };
            var aColor = rs.First();
            if (attach)
            {
                aColor.OnThemeChangedEvent += this.OnColorChanged;
            }
            
            return aColor;
        }

        private void InitTheme()
        {
            foreach(KeyValuePair<string,string>kvp in this._theme)
            {
                if(kvp.Key == _themekey)
                {
                    GetTheme(kvp.Value, false).DoChangeTheme(null);
                    if(kvp.Value == "BaseDark")
                    {
                        _toggleDark = true;
                    }
                } else if(kvp.Key == _colorkey)
                {
                    GetColor(kvp.Value, false).DoChangeTheme(null);
                }
                
            }
        }

        public MainWindowViewModel()
        {
            this.AccentColors = ThemeManager.Accents
                                            .Select(a => new AccentColorMenuData(OnColorChanged) { Name = a.Name, ColorBrush = a.Resources["AccentColorBrush"] as Brush })
                                            .ToList();
            _theme[_themekey] = "BaseLight";
            _theme[_colorkey] = "Blue";
            if (File.Exists(_themeconfig))
            {
                var lines = File.ReadAllLines(_themeconfig);
                foreach(var line in lines)
                {
                    if (line.Contains("theme"))
                    {
                        var rs = line.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                        if (rs.Count() > 1)
                        {
                            _theme[_themekey] = rs[1];
                        }
                        
                    } else if (line.Contains("color"))
                    {
                        var rs = line.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                        if (rs.Count() > 1)
                        {
                            _theme[_colorkey] = rs[1];
                        }
                    }
                }
            }
            InitTheme();
        }
    }

    public class AccentColorMenuData
    {
        public event EventHandler<string> OnThemeChangedEvent;
        public string Name { get; set; }
        public Brush BorderColorBrush { get; set; }
        public Brush ColorBrush { get; set; }

        private ICommand changeAccentCommand;

        public AccentColorMenuData() { }
        public AccentColorMenuData(EventHandler<string> act)
        {
            OnThemeChangedEvent += act;
        }
        public ICommand ChangeAccentCommand
        {
            get
            {
                return this.changeAccentCommand ?? (changeAccentCommand = new RelayCommand<object>(x => DoChangeTheme(x))) ;
            }
        }

        protected virtual void OnThemeChanged(string val)
        {
            var handler = OnThemeChangedEvent;
            if(handler != null)
            {
                handler(this, val);
            }
        }

        public virtual void DoChangeTheme(object sender)
        {
            var theme = ThemeManager.DetectAppStyle(Application.Current);
            var accent = ThemeManager.GetAccent(this.Name);
            ThemeManager.ChangeAppStyle(Application.Current, accent, theme.Item1);
            OnThemeChanged(this.Name);
        }
    }

    public class AppThemeData : AccentColorMenuData
    {
        public override void DoChangeTheme(object sender)
        {
            var theme = ThemeManager.DetectAppStyle(Application.Current);
            var appTheme = ThemeManager.GetAppTheme(this.Name);
            ThemeManager.ChangeAppStyle(Application.Current, theme.Item2, appTheme);
            OnThemeChanged(this.Name);
        }
    }
}
