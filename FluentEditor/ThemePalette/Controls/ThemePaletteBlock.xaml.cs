using FluentEditorShared.ColorPalette;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace FluentEditor.ThemePalette.Controls
{
    public sealed partial class ThemePaletteBlock : UserControl
    {
        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(nameof(Header), typeof(string), typeof(ThemePaletteBlock), new PropertyMetadata(null));


        public ColorPalette LightColorPalette
        {
            get { return (ColorPalette)GetValue(LightColorPaletteProperty); }
            set { SetValue(LightColorPaletteProperty, value); }
        }

        public static readonly DependencyProperty LightColorPaletteProperty =
            DependencyProperty.Register(nameof(LightColorPalette), typeof(ColorPalette), typeof(ThemePaletteBlock), new PropertyMetadata(null));


        public ColorPalette DarkColorPalette
        {
            get { return (ColorPalette)GetValue(DarkColorPaletteProperty); }
            set { SetValue(DarkColorPaletteProperty, value); }
        }

        public static readonly DependencyProperty DarkColorPaletteProperty =
            DependencyProperty.Register(nameof(DarkColorPalette), typeof(ColorPalette), typeof(ThemePaletteBlock), new PropertyMetadata(null));


        public Style PaletteEditorStyle
        {
            get { return (Style)GetValue(PaletteEditorStyleProperty); }
            set { SetValue(PaletteEditorStyleProperty, value); }
        }

        public static readonly DependencyProperty PaletteEditorStyleProperty =
            DependencyProperty.Register(nameof(PaletteEditorStyle), typeof(Style), typeof(ThemePaletteBlock), new PropertyMetadata(null));





        public ThemePaletteBlock()
        {
            this.InitializeComponent();
        }
    }
}
