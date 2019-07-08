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

        private Color _acrylicBackdropBackgroundColor;
        public Color AcrylicBackdropBackgroundColor
        {
            get { return _acrylicBackdropBackgroundColor; }
            set
            {
                SetProperty(ref _acrylicBackdropBackgroundColor, value);
            }
        }

        private double _acrylicBackdropBackgroundOpacity;
        public double AcrylicBackdropBackgroundOpacity
        {
            get { return _acrylicBackdropBackgroundOpacity; }
            set
            {
                SetProperty(ref _acrylicBackdropBackgroundOpacity, value);
            }
        }

        private double? _acrylicBackdropBackgroundLuminosityOpacity;
        public double? AcrylicBackdropBackgroundLuminosityOpacity
        {
            get { return _acrylicBackdropBackgroundLuminosityOpacity; }
            set
            {
                SetProperty(ref _acrylicBackdropBackgroundLuminosityOpacity, value);
            }
        }

        private Color _acrylicHostBackdropBackgroundColor;
        public Color AcrylicHostBackdropBackgroundColor
        {
            get { return _acrylicHostBackdropBackgroundColor; }
            set
            {
                SetProperty(ref _acrylicHostBackdropBackgroundColor, value);
            }
        }

        private double _acrylicHostBackdropBackgroundOpacity;
        public double AcrylicHostBackdropBackgroundOpacity
        {
            get { return _acrylicHostBackdropBackgroundOpacity; }
            set
            {
                SetProperty(ref _acrylicHostBackdropBackgroundOpacity, value);
            }
        }

        private double? _acrylicHostBackdropBackgroundLuminosityOpacity;
        public double? AcrylicHostBackdropBackgroundLuminosityOpacity
        {
            get { return _acrylicHostBackdropBackgroundLuminosityOpacity; }
            set
            {
                SetProperty(ref _acrylicHostBackdropBackgroundLuminosityOpacity, value);
            }
        }

        public static Color GetColor(Color? color)
        {
            return color ?? Colors.Transparent;
        }
    }
}
