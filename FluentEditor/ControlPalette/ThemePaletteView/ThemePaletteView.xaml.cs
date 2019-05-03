using FluentEditor.ControlPalette.ThemePaletteView.Common;
using FluentEditor.ControlPalette.Model.ThemePalette;
using System;
using System.ComponentModel;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using FluentEditorShared.ColorPalette;

namespace FluentEditor.ControlPalette.ThemePaletteView
{
    public sealed partial class ThemePaletteView : Page
    {
        public ThemePaletteView()
        {
            this.InitializeComponent();
        }

        #region ViewModelProperty

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel), typeof(ThemePaletteViewModel), typeof(ControlPaletteView), new PropertyMetadata(null));

        public ThemePaletteViewModel ViewModel
        {
            get { return GetValue(ViewModelProperty) as ThemePaletteViewModel; }
            set { SetValue(ViewModelProperty, value); }
        }

        #endregion

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            ViewModel = e.Parameter as ThemePaletteViewModel;

            ViewModel.PropertyChanged += OnViewModelPropertyChanged;
            ViewModel.ThemeChanged += OnThemeChanged;
            OnThemeChanged(ViewModel.CurrentModel);

            ViewModel.LightBackgroundColors = new System.Collections.Generic.List<FluentEditorShared.ColorPalette.ContrastColorWrapper>()
            {
                new FluentEditorShared.ColorPalette.ContrastColorWrapper(LightApplicationBackgroundPalette, true, true),
                new FluentEditorShared.ColorPalette.ContrastColorWrapper(LightItemBackground0Palette, true, true),
                new FluentEditorShared.ColorPalette.ContrastColorWrapper(LightItemBackground1Palette, true, true),
                new FluentEditorShared.ColorPalette.ContrastColorWrapper(LightAcrylicBackgroundPalette, true, true),
            };

            ViewModel.DarkBackgroundColors = new System.Collections.Generic.List<FluentEditorShared.ColorPalette.ContrastColorWrapper>()
            {
                new FluentEditorShared.ColorPalette.ContrastColorWrapper(DarkApplicationBackgroundPalette, true, true),
                new FluentEditorShared.ColorPalette.ContrastColorWrapper(DarkItemBackground0Palette, true, true),
                new FluentEditorShared.ColorPalette.ContrastColorWrapper(DarkItemBackground1Palette, true, true),
                new FluentEditorShared.ColorPalette.ContrastColorWrapper(DarkAcrylicBackgroundPalette, true, true),
            };

            ViewModel.LightTextColors = new System.Collections.Generic.List<FluentEditorShared.ColorPalette.ContrastColorWrapper>()
            {
                new FluentEditorShared.ColorPalette.ContrastColorWrapper(LightTextPalette, true, true),
                new FluentEditorShared.ColorPalette.ContrastColorWrapper(LightSubTextPalette, true, true),
                new FluentEditorShared.ColorPalette.ContrastColorWrapper(LightSubSubTextPalette, true, true),
                new FluentEditorShared.ColorPalette.ContrastColorWrapper(LightActiveTextPalette, true, true),
            };

            ViewModel.DarkTextColors = new System.Collections.Generic.List<FluentEditorShared.ColorPalette.ContrastColorWrapper>()
            {
                new FluentEditorShared.ColorPalette.ContrastColorWrapper(DarkTextPalette, true, true),
                new FluentEditorShared.ColorPalette.ContrastColorWrapper(DarkSubTextPalette, true, true),
                new FluentEditorShared.ColorPalette.ContrastColorWrapper(DarkSubSubTextPalette, true, true),
                new FluentEditorShared.ColorPalette.ContrastColorWrapper(DarkActiveTextPalette, true, true),
            };

            var LightAccentColors = new System.Collections.Generic.List<FluentEditorShared.ColorPalette.ContrastColorWrapper>()
            {
                new FluentEditorShared.ColorPalette.ContrastColorWrapper(LightAccentPalette, true, true),
                new FluentEditorShared.ColorPalette.ContrastColorWrapper(LightActiveAccentPalette, true, true),
            };

            var DarkAccentColors = new System.Collections.Generic.List<FluentEditorShared.ColorPalette.ContrastColorWrapper>()
            {
                new FluentEditorShared.ColorPalette.ContrastColorWrapper(DarkAccentPalette, true, true),
                new FluentEditorShared.ColorPalette.ContrastColorWrapper(DarkActiveAccentPalette, true, true),
            };

            ViewModel.TextLightContrastColors = new System.Collections.Generic.List<FluentEditorShared.ColorPalette.ContrastColorWrapper>(ViewModel.LightBackgroundColors);
            ViewModel.TextLightContrastColors.AddRange(LightAccentColors);

            ViewModel.TextDarkContrastColors = new System.Collections.Generic.List<FluentEditorShared.ColorPalette.ContrastColorWrapper>(ViewModel.DarkBackgroundColors);
            ViewModel.TextDarkContrastColors.AddRange(DarkAccentColors);

            ViewModel.AccentLightContrastColors = new System.Collections.Generic.List<FluentEditorShared.ColorPalette.ContrastColorWrapper>(ViewModel.LightBackgroundColors);
            ViewModel.AccentLightContrastColors.AddRange(ViewModel.LightTextColors);

            ViewModel.AccentDarkContrastColors = new System.Collections.Generic.List<FluentEditorShared.ColorPalette.ContrastColorWrapper>(ViewModel.DarkBackgroundColors);
            ViewModel.AccentDarkContrastColors.AddRange(ViewModel.DarkTextColors);

            ViewModel.BackgroundLightContrastColors = new System.Collections.Generic.List<FluentEditorShared.ColorPalette.ContrastColorWrapper>(ViewModel.LightTextColors);
            ViewModel.BackgroundLightContrastColors.AddRange(LightAccentColors);

            ViewModel.BackgroundDarkContrastColors = new System.Collections.Generic.List<FluentEditorShared.ColorPalette.ContrastColorWrapper>(ViewModel.DarkTextColors);
            ViewModel.BackgroundDarkContrastColors.AddRange(DarkAccentColors);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            ViewModel.PropertyChanged -= OnViewModelPropertyChanged;
            ViewModel.ThemeChanged -= OnThemeChanged;
        }

        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ViewModel.ActivePreset))
            {

            }
        }

        private void OnThemeChanged(IThemePaletteModel obj)
        {
            App app = Application.Current as App;
            app.ThemeManager.DarkTheme = obj.DarkTheme;
            app.ThemeManager.LightTheme = obj.LightTheme;
        }

        #region Theme parameters changed

        private void UpdateLightThemeColor(IColorPaletteEntry palette, Action<Theme, Color> action)
        {
            action(ViewModel.CurrentModel.LightTheme, palette.ActiveColor);
        }

        private void UpdateDarkThemeColor(IColorPaletteEntry palette, Action<Theme, Color> action)
        {
            action(ViewModel.CurrentModel.DarkTheme, palette.ActiveColor);
        }

        private void OnLightApplicationBackgroundColorChanged(FluentEditorShared.ColorPalette.IColorPaletteEntry palette)
        {
            UpdateLightThemeColor(palette, (theme, color) => { theme.AppBackground = color; });
        }

        private void OnDarkApplicationBackgroundColorChanged(IColorPaletteEntry palette)
        {

            UpdateDarkThemeColor(palette, (theme, color) => { theme.AppBackground = color; });
        }

        private void OnLightItemBackground0ColorChanged(IColorPaletteEntry palette)
        {
            UpdateLightThemeColor(palette, (theme, color) => { theme.ItemBackground0 = color; });
        }

        private void OnDarkItemBackground0ColorChanged(IColorPaletteEntry palette)
        {
            UpdateDarkThemeColor(palette, (theme, color) => { theme.ItemBackground0 = color; });
        }

        private void OnLightItemBackground1ColorChanged(IColorPaletteEntry palette)
        {
            UpdateLightThemeColor(palette, (theme, color) => { theme.ItemBackground1 = color; });
        }

        private void OnDarkItemBackground1ColorChanged(IColorPaletteEntry palette)
        {
            UpdateDarkThemeColor(palette, (theme, color) => { theme.ItemBackground1 = color; });
        }

        private void OnLightAcrylicBackgroundColorChanged(IColorPaletteEntry palette)
        {
            UpdateLightThemeColor(palette, (theme, color) => { theme.AcrylicBackgroundColor = color; });
        }

        private void OnDarkAcrylicBackgroundColorChanged(IColorPaletteEntry palette)
        {
            UpdateDarkThemeColor(palette, (theme, color) => { theme.AcrylicBackgroundColor = color; });
        }

        private void OnLightAcrylicBackgroundOpacityChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            if(ViewModel?.CurrentModel?.LightTheme != null)
            {
                ViewModel.CurrentModel.LightTheme.AcrylicBackgroundOpacity = e.NewValue;
            }
        }

        private void OnDarkAcrylicBackgroundOpacityChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            if (ViewModel?.CurrentModel?.DarkTheme != null)
            {
                ViewModel.CurrentModel.DarkTheme.AcrylicBackgroundOpacity = e.NewValue;
            }
        }

        private void OnLightTextColorChanged(IColorPaletteEntry obj)
        {
            UpdateLightThemeColor(obj, (theme, color) => { theme.TextColor = color; });
        }

        private void OnDarkTextColorChanged(IColorPaletteEntry obj)
        {
            UpdateDarkThemeColor(obj, (theme, color) => { theme.TextColor = color; });
        }

        private void OnLightSubTextColorChanged(IColorPaletteEntry obj)
        {
            UpdateLightThemeColor(obj, (theme, color) => { theme.SubTextColor = color; });
        }

        private void OnDarkSubTextColorChanged(IColorPaletteEntry obj)
        {
            UpdateDarkThemeColor(obj, (theme, color) => { theme.SubTextColor = color; });
        }

        private void OnLightSubSubTextColorChanged(IColorPaletteEntry obj)
        {
            UpdateLightThemeColor(obj, (theme, color) => { theme.SubSubTextColor = color; });
        }

        private void OnDarkSubSubTextColorChanged(IColorPaletteEntry obj)
        {
            UpdateDarkThemeColor(obj, (theme, color) => { theme.SubSubTextColor = color; });
        }

        private void OnLightActiveTextColorChanged(IColorPaletteEntry obj)
        {
            UpdateLightThemeColor(obj, (theme, color) => { theme.ActiveTextColor = color; });
        }

        private void OnDarkActiveTextColorChanged(IColorPaletteEntry obj)
        {
            UpdateDarkThemeColor(obj, (theme, color) => { theme.ActiveTextColor = color; });
        }

        private void OnLightAccentColorChanged(IColorPaletteEntry obj)
        {
            UpdateLightThemeColor(obj, (theme, color) => { theme.AccentColor = color; });
        }

        private void OnDarkAccentColorChanged(IColorPaletteEntry obj)
        {
            UpdateDarkThemeColor(obj, (theme, color) => { theme.AccentColor = color; });
        }

        private void OnLightActiveAccentColorChanged(IColorPaletteEntry obj)
        {
            UpdateLightThemeColor(obj, (theme, color) => { theme.ActiveAccentColor = color; });
        }

        private void OnDarkActiveAccentColorChanged(IColorPaletteEntry obj)
        {
            UpdateDarkThemeColor(obj, (theme, color) => { theme.ActiveAccentColor = color; });
        }

        private void OnLightHyperlinkButtonForegroundColorChanged(IColorPaletteEntry obj)
        {
            UpdateLightThemeColor(obj, (theme, color) => { theme.HyperlinkButtonForegroundColor = color; });
        }

        private void OnDarkHyperlinkButtonForegroundColorChanged(IColorPaletteEntry obj)
        {
            UpdateDarkThemeColor(obj, (theme, color) => { theme.HyperlinkButtonForegroundColor = color; });
        }

        private void OnLightHyperlinkButtonActiveForegroundColorChanged(IColorPaletteEntry obj)
        {
            UpdateLightThemeColor(obj, (theme, color) => { theme.HyperlinkButtonActiveForegroundColor = color; });
        }

        private void OnDarkHyperlinkButtonActiveForegroundColorChanged(IColorPaletteEntry obj)
        {
            UpdateDarkThemeColor(obj, (theme, color) => { theme.HyperlinkButtonActiveForegroundColor = color; });
        }

        private void OnLightHyperlinkButtonDisableForegroundColorChanged(IColorPaletteEntry obj)
        {
            UpdateLightThemeColor(obj, (theme, color) => { theme.HyperlinkButtonDisableForegroundColor = color; });
        }

        private void OnDarkHyperlinkButtonDisableForegroundColorChanged(IColorPaletteEntry obj)
        {
            UpdateDarkThemeColor(obj, (theme, color) => { theme.HyperlinkButtonDisableForegroundColor = color; });
        }

        private void OnLightTextButtonForegroundColorChanged(IColorPaletteEntry obj)
        {
            UpdateLightThemeColor(obj, (theme, color) => { theme.TextButtonForegroundColor = color; });
        }

        private void OnDarkTextButtonForegroundColorChanged(IColorPaletteEntry obj)
        {
            UpdateDarkThemeColor(obj, (theme, color) => { theme.TextButtonForegroundColor = color; });
        }

        private void OnLightTextButtonActiveForegroundColorChanged(IColorPaletteEntry obj)
        {
            UpdateLightThemeColor(obj, (theme, color) => { theme.TextButtonActiveForegroundColor = color; });
        }

        private void OnDarkTextButtonActiveForegroundColorChanged(IColorPaletteEntry obj)
        {
            UpdateDarkThemeColor(obj, (theme, color) => { theme.TextButtonActiveForegroundColor = color; });
        }

        private void OnLightTextButtonDisableForegroundColorChanged(IColorPaletteEntry obj)
        {
            UpdateLightThemeColor(obj, (theme, color) => { theme.TextButtonDisableForegroundColor = color; });
        }

        private void OnDarkTextButtonDisableForegroundColorChanged(IColorPaletteEntry obj)
        {
            UpdateDarkThemeColor(obj, (theme, color) => { theme.TextButtonDisableForegroundColor = color; });
        }

        #endregion
    }
}
