using FluentEditorShared.ColorPalette;
using FluentEditorShared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;

namespace FluentEditor.ControlPalette.CustomView.Common
{
    public class ThemePaletteEntry : DependencyObject, IColorPaletteEntry
    {
        public ThemePaletteEntry()
        {}

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(ThemePaletteEntry), new PropertyMetadata(null));


        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register(nameof(Description), typeof(string), typeof(ThemePaletteEntry), new PropertyMetadata(null));


        public Color ActiveColor
        {
            get { return (Color)GetValue(ActiveColorProperty); }
            set { SetValue(ActiveColorProperty, value); }
        }

        public static readonly DependencyProperty ActiveColorProperty =
            DependencyProperty.Register(nameof(ActiveColor), typeof(Color), typeof(ThemePaletteEntry), new PropertyMetadata(Colors.White, OnActiveColorChanged));

        private static void OnActiveColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ThemePaletteEntry entry)
            {
                entry.ColorPaletteEntry.ActiveColor = entry.ActiveColor;
            }
        }

        public FluentEditorShared.Utils.ColorStringFormat ActiveColorStringFormat
        {
            get { return (FluentEditorShared.Utils.ColorStringFormat)GetValue(ActiveColorStringFormatProperty); }
            set { SetValue(ActiveColorStringFormatProperty, value); }
        }

        public static readonly DependencyProperty ActiveColorStringFormatProperty =
            DependencyProperty.Register(nameof(ActiveColorStringFormat), typeof(FluentEditorShared.Utils.ColorStringFormat), typeof(ThemePaletteEntry), new PropertyMetadata(FluentEditorShared.Utils.ColorStringFormat.PoundRGB, OnActiveColorStringFormatChanged));

        private static void OnActiveColorStringFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ThemePaletteEntry entry)
            {
                entry.UpdateColorPaletteEntry();
            }
        }

        public string ActiveColorString
        {
            get
            {
                return ColorPaletteEntry.ActiveColorString;
            }
        }

        public IReadOnlyList<ContrastColorWrapper> ContrastColors
        {
            get { return (IReadOnlyList<ContrastColorWrapper>)GetValue(ContrastColorsProperty); }
            set { SetValue(ContrastColorsProperty, value); }
        }

        public static readonly DependencyProperty ContrastColorsProperty =
            DependencyProperty.Register(nameof(ContrastColors), typeof(IReadOnlyList<ContrastColorWrapper>), typeof(ThemePaletteEntry), new PropertyMetadata(null, OnContrastColorsChanged));

        private static void OnContrastColorsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ThemePaletteEntry entry)
            {
                entry.ColorPaletteEntry.ContrastColors = entry.ContrastColors;
            }
        }

        public ContrastColorWrapper BestContrastColor
        {
            get
            {
                return ColorPaletteEntry.BestContrastColor;
            }
        }

        public double BestContrastValue
        {
            get
            {
                return ColorPaletteEntry.BestContrastValue;
            }
        }

        public event Action<IColorPaletteEntry> ActiveColorChanged;
        public event Action<IColorPaletteEntry> ContrastColorChanged;


        private ColorPaletteEntry _colorPaletteEntry = null;
        private ColorPaletteEntry ColorPaletteEntry
        {
            get
            {
                return _colorPaletteEntry ?? (_colorPaletteEntry = new ColorPaletteEntry(ActiveColor, Title, Description, ActiveColorStringFormat, ContrastColors));
            }

            set
            {
                if(_colorPaletteEntry != null)
                {
                    _colorPaletteEntry.ActiveColorChanged -= OnActiveColorChanged;
                    _colorPaletteEntry.ContrastColorChanged -= OnContrastColorChanged;
                }

                _colorPaletteEntry = value;

                if (_colorPaletteEntry != null)
                {
                    _colorPaletteEntry.ActiveColorChanged += OnActiveColorChanged;
                    _colorPaletteEntry.ContrastColorChanged += OnContrastColorChanged;
                }
            }
        }

        private void OnActiveColorChanged(IColorPaletteEntry obj)
        {
            RaiseActiveColorChanged(this);
        }

        private void OnContrastColorChanged(IColorPaletteEntry obj)
        {
            RaiseContrastColorChanged(this);
        }

        private void UpdateColorPaletteEntry()
        {
            ColorPaletteEntry = new ColorPaletteEntry(ActiveColor, Title, Description, ActiveColorStringFormat, ContrastColors);
        }


        private void RaiseActiveColorChanged(IColorPaletteEntry obj)
        {
            ActiveColorChanged?.Invoke(this);
        }

        private void RaiseContrastColorChanged(IColorPaletteEntry obj)
        {
            ContrastColorChanged?.Invoke(this);
        }
    }
}
