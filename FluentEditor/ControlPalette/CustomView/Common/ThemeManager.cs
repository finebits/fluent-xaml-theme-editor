using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace FluentEditor.ControlPalette.CustomView.Common
{
    class ThemeManager : BindableBase
    {
        private Common.Theme _darkTheme;

        public Common.Theme DarkTheme
        {
            get { return _darkTheme; }
            set
            {
                SetProperty(ref _darkTheme, value);
            }
        }

        private Common.Theme _lightTheme;

        public Common.Theme LightTheme
        {
            get { return _lightTheme; }
            set
            {
                SetProperty(ref _lightTheme, value);
            }
        }
    }
}
