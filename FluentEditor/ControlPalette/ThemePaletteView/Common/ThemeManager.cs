
namespace FluentEditor.ControlPalette.ThemePaletteView.Common
{
    public class ThemeManager : BindableBase
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
