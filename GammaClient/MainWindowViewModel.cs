
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GammaClient.GCViews;
using MahApps.Metro;
using System;
using System.Collections.Generic;
using System.Linq;
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
                    GetTheme("basedark").DoChangeTheme(this);
                } else
                {
                    GetTheme("baselight").DoChangeTheme(this);
                }
                RaisePropertyChanged("ToggleDark");
            }
        }

        private AppThemeData GetTheme(string themeName)
        {
            var t = from theme in ThemeManager.AppThemes
                    where theme.Name.ToLower().Contains(themeName)
                    select new AppThemeData() { Name = theme.Name, BorderColorBrush = theme.Resources["BlackColorBrush"] as Brush, ColorBrush = theme.Resources["WhiteColorBrush"] as Brush };
            return t.First();
        }
        public MainWindowViewModel()
        {
            this.AccentColors = ThemeManager.Accents
                                            .Select(a => new AccentColorMenuData() { Name = a.Name, ColorBrush = a.Resources["AccentColorBrush"] as Brush })
                                            .ToList();
        }
    }

    public class AccentColorMenuData
    {
        public string Name { get; set; }
        public Brush BorderColorBrush { get; set; }
        public Brush ColorBrush { get; set; }

        private ICommand changeAccentCommand;

        public ICommand ChangeAccentCommand
        {
            get
            {
                return this.changeAccentCommand ?? (changeAccentCommand = new RelayCommand<object>(x => DoChangeTheme(x))) ;
            }
        }

        public virtual void DoChangeTheme(object sender)
        {
            var theme = ThemeManager.DetectAppStyle(Application.Current);
            var accent = ThemeManager.GetAccent(this.Name);
            ThemeManager.ChangeAppStyle(Application.Current, accent, theme.Item1);
        }
    }

    public class AppThemeData : AccentColorMenuData
    {
        public override void DoChangeTheme(object sender)
        {
            var theme = ThemeManager.DetectAppStyle(Application.Current);
            var appTheme = ThemeManager.GetAppTheme(this.Name);
            ThemeManager.ChangeAppStyle(Application.Current, theme.Item2, appTheme);
        }
    }
}
