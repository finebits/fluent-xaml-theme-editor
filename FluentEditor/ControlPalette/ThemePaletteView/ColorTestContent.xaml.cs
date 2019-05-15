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
    }
}
