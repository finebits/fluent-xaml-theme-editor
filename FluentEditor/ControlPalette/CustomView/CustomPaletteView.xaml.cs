using FluentEditor.ControlPalette.CustomView.Common;
using FluentEditor.ControlPalette.Model.Custom;
using System;
using System.ComponentModel;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;


namespace FluentEditor.ControlPalette.CustomView
{
    public sealed partial class CustomPaletteView : Page
    {
        public CustomPaletteView()
        {
            this.InitializeComponent();
        }

        #region ViewModelProperty

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel), typeof(CustomPaletteViewModel), typeof(ControlPaletteView), new PropertyMetadata(null));

        public CustomPaletteViewModel ViewModel
        {
            get { return GetValue(ViewModelProperty) as CustomPaletteViewModel; }
            set { SetValue(ViewModelProperty, value); }
        }

        #endregion

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            ViewModel = e.Parameter as CustomPaletteViewModel;

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
            if(e.PropertyName == nameof(ViewModel.ActivePreset))
            {

            }
        }

        private void OnThemeChanged(ICustomPaletteModel obj)
        {
            App app = Application.Current as App;
            app.ThemeManager.DarkTheme = obj.DarkTheme;
            app.ThemeManager.LightTheme = obj.LightTheme;
        }

        #region ColorChanged

        private void OnLightApplicationBackgroundColorChanged(FluentEditorShared.ColorPalette.IColorPaletteEntry obj)
        {
            if(obj is ThemePaletteEntry palette)
            {
                OnColorChanged(palette, (color) => { ViewModel.CurrentModel.LightTheme.AppBackground = color; });
            }
        }

        private void OnDarkApplicationBackgroundColorChanged(FluentEditorShared.ColorPalette.IColorPaletteEntry obj)
        {
            if (obj is ThemePaletteEntry palette)
            {
                OnColorChanged(palette, (color) => { ViewModel.CurrentModel.DarkTheme.AppBackground = color; });
            }
        }

        private void OnColorChanged(ThemePaletteEntry palette, Action<Color> action)
        {
            action(palette.ActiveColor);
        }

        #endregion
    }
}
