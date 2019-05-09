using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FluentEditor.ControlPalette.ThemePaletteView
{
    public sealed partial class ColorTestContent : UserControl
    {
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(ColorTestContent), new PropertyMetadata(null));


        public bool IsSystemTheme
        {
            get { return (bool)GetValue(IsSystemThemeProperty); }
            set { SetValue(IsSystemThemeProperty, value); }
        }

        public static readonly DependencyProperty IsSystemThemeProperty =
            DependencyProperty.Register(nameof(IsSystemTheme), typeof(bool), typeof(ColorTestContent), new PropertyMetadata(true, OnIsSystemThemeChanged));

        private static void OnIsSystemThemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(d is ColorTestContent control)
            {
                control.UpdateTheme();
            }
        }

        public ResourceDictionary CustomThemeResource
        {
            get { return (ResourceDictionary)GetValue(CustomThemeResourceProperty); }
            set { SetValue(CustomThemeResourceProperty, value); }
        }

        public static readonly DependencyProperty CustomThemeResourceProperty =
            DependencyProperty.Register(nameof(CustomThemeResource), typeof(ResourceDictionary), typeof(ColorTestContent), new PropertyMetadata(null));


        public ColorTestContent()
        {
            this.InitializeComponent();
        }

        private void UpdateTheme()
        {
            if (IsSystemTheme)
            {
                Unload();
            }
            else 
            {
                Load();
            }
        }

        private void Load()
        {
            if (Resources != null && !Resources.MergedDictionaries.Contains(CustomThemeResource))
            {
                Resources.MergedDictionaries.Add(CustomThemeResource);
            }
        }

        private void Unload()
        {
            if (Resources != null && Resources.MergedDictionaries.Contains(CustomThemeResource))
            {
                Resources.MergedDictionaries.Remove(CustomThemeResource);
            }
        }
    }
}
