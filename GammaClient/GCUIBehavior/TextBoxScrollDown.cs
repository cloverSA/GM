using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace GammaClient.GCUIBehavior
{
    class TextBoxScrollDown : IContentScrollDown
    {
        private ICommand _cmd;
        public ICommand ScrollDownCommand
        {
            get
            {
                return this._cmd ?? (this._cmd = new RelayCommand<object>(ScrollDownResult));
            }
        }

        public void ScrollDownResult(object sender)
        {
            var tb = sender as TextBox;
            if (tb != null)
            {
                tb.Focus();
                tb.CaretIndex = tb.Text.Length;
                tb.ScrollToEnd();
            }
        }
    }
}
