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

        private Color _accentLight1Color;
        public Color AccentLight1Color
        {
            get { return _accentLight1Color; }
            set
            {
                SetProperty(ref _accentLight1Color, value);
            }
        }

        private Color _accentDark1Color;
        public Color AccentDark1Color
        {
            get { return _accentDark1Color; }
            set
            {
                SetProperty(ref _accentDark1Color, value);
            }
        }

        private Color _accentLight2Color;
        public Color AccentLight2Color
        {
            get { return _accentLight2Color; }
            set
            {
                SetProperty(ref _accentLight2Color, value);
            }
        }

        private Color _accentDark2Color;
        public Color AccentDark2Color
        {
            get { return _accentDark2Color; }
            set
            {
                SetProperty(ref _accentDark2Color, value);
            }
        }

        private Color _accentLight3Color;
        public Color AccentLight3Color
        {
            get { return _accentLight3Color; }
            set
            {
                SetProperty(ref _accentLight3Color, value);
            }
        }

        private Color _accentDark3Color;
        public Color AccentDark3Color
        {
            get { return _accentDark3Color; }
            set
            {
                SetProperty(ref _accentDark3Color, value);
            }
        }

        private Color _hyperlinkButtonForegroundColor;
        public Color HyperlinkButtonForegroundColor
        {
            get { return _hyperlinkButtonForegroundColor; }
            set
            {
                SetProperty(ref _hyperlinkButtonForegroundColor, value);
            }
        }

        private Color _hyperlinkButtonActiveForegroundColor;
        public Color HyperlinkButtonActiveForegroundColor
        {
            get { return _hyperlinkButtonActiveForegroundColor; }
            set
            {
                SetProperty(ref _hyperlinkButtonActiveForegroundColor, value);
            }
        }

        private Color _hyperlinkButtonDisabledForegroundColor;
        public Color HyperlinkButtonDisabledForegroundColor
        {
            get { return _hyperlinkButtonDisabledForegroundColor; }
            set
            {
                SetProperty(ref _hyperlinkButtonDisabledForegroundColor, value);
            }
        }

        private Color _regionBackgroundColor;
        public Color RegionBackgroundColor
        {
            get { return _regionBackgroundColor; }
            set
            {
                SetProperty(ref _regionBackgroundColor, value);
            }
        }

        private Color _mediumBackgroundColor;
        public Color MediumBackgroundColor
        {
            get { return _mediumBackgroundColor; }
            set
            {
                SetProperty(ref _mediumBackgroundColor, value);
            }
        }

        private Color _highBackgroundColor;
        public Color HighBackgroundColor
        {
            get { return _highBackgroundColor; }
            set
            {
                SetProperty(ref _highBackgroundColor, value);
            }
        }

        private AcrylicBrushInfo _acrylicBackground;
        public AcrylicBrushInfo AcrylicBackground
        {
            get { return _acrylicBackground; }
            set
            {
                SetProperty(ref _acrylicBackground, value);
            }
        }

        private AcrylicBrushInfo _acrylicBackground2nd;
        public AcrylicBrushInfo AcrylicBackground2nd
        {
            get { return _acrylicBackground2nd; }
            set
            {
                SetProperty(ref _acrylicBackground2nd, value);
            }
        }

        private AcrylicBrushInfo _acrylicBackground3rd;
        public AcrylicBrushInfo AcrylicBackground3rd
        {
            get { return _acrylicBackground3rd; }
            set
            {
                SetProperty(ref _acrylicBackground3rd, value);
            }
        }

        public static Color GetColor(Color? color)
        {
            return color ?? Colors.Transparent;
        }
    }

    public class AcrylicBrushInfo : BindableBase
    {
        private bool _isHostBackdrop = false;
        public bool IsHostBackdrop
        {
            get { return _isHostBackdrop; }
            set
            {
                SetProperty(ref _isHostBackdrop, value);
            }
        }

        private Color _color;
        public Color Color
        {
            get { return _color; }
            set
            {
                SetProperty(ref _color, value);
            }
        }

        private double _tintOpacity;
        public double TintOpacity
        {
            get { return _tintOpacity; }
            set
            {
                SetProperty(ref _tintOpacity, value);
            }
        }

        private double? _tintLuminosityOpacity;
        public double? TintLuminosityOpacity
        {
            get { return _tintLuminosityOpacity; }
            set
            {
                SetProperty(ref _tintLuminosityOpacity, value);
            }
        }
    }
}
