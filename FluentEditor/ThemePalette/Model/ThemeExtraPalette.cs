using FluentEditor.Common;
using Windows.UI;

namespace FluentEditor.ThemePalette.Model
{
    public class ThemeExtraPalette : BindableBase
    {
        private Color _activeTextColor;
        public Color ActiveTextColor
        {
            get { return _activeTextColor; }
            set
            {
                SetProperty(ref _activeTextColor, value);
            }
        }

        public static Color GetColor(Color? color)
        {
            return color ?? Colors.Transparent;
        }
    }
}
