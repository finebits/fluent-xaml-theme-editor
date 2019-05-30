using FluentEditorShared.ColorPalette;
using FluentEditorShared.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Data.Json;
using Windows.UI;

namespace FluentEditor.ThemePalette.Data
{
    class ThemeEditableAcrylicPaletteEntry : EditableColorPaletteEntry, IAcrylicBrushEntry, INotifyPropertyChanged
    {
        public new static EditableColorPaletteEntry Parse(JsonObject data, IColorPaletteEntry sourceColor, IReadOnlyList<ContrastColorWrapper> contrastColors)
        {
            Color customColor;
            if (data.ContainsKey("CustomColor"))
            {
                customColor = data["CustomColor"].GetColor();
            }
            else
            {
                customColor = default(Color);
            }
            bool useCustomColor = false;
            if (data.ContainsKey("UseCustomColor"))
            {
                useCustomColor = data["UseCustomColor"].GetBoolean();
            }

            FluentEditorShared.Utils.ColorStringFormat activeColorStringFormat = FluentEditorShared.Utils.ColorStringFormat.PoundRGB;
            if (data.ContainsKey("ActiveColorStringFormat"))
            {
                activeColorStringFormat = data["ActiveColorStringFormat"].GetEnum<FluentEditorShared.Utils.ColorStringFormat>();
            }

            if (data.ContainsKey(nameof(Opacity)))
            {
                var opacity = data[nameof(Opacity)].GetNumber();
                return new ThemeEditableAcrylicPaletteEntry(opacity, sourceColor, customColor, useCustomColor, data.GetOptionalString("Title"), data.GetOptionalString("Description"), activeColorStringFormat, contrastColors);
            }

            return new EditableColorPaletteEntry(sourceColor, customColor, useCustomColor, data.GetOptionalString("Title"), data.GetOptionalString("Description"), activeColorStringFormat, contrastColors);
        }

        public ThemeEditableAcrylicPaletteEntry(double sourceOpacity, IColorPaletteEntry sourceColor, Color customColor, bool useCustomColor, string title, string description, FluentEditorShared.Utils.ColorStringFormat activeColorStringFormat, IReadOnlyList<ContrastColorWrapper> contrastColors)
            : base(sourceColor, customColor, useCustomColor, title, description, activeColorStringFormat, contrastColors)
        {
            SourceOpacity = sourceOpacity;
            Opacity = SourceOpacity;
        }

        private double _sourceOpacity = 0;
        private double SourceOpacity
        {
            get { return _sourceOpacity; }
            set
            {
                SetOpacity(ref _sourceOpacity, value);
            }
        }

        private double _opacity = 0;
        public double Opacity
        {
            get { return _opacity; }
            set
            {
                if (SetOpacity(ref _opacity, value))
                {
                    RaiseActiveColorChanged();
                    RaisePropertyChangedFromSource();
                }
            }
        }

        public bool IsCustomOpacity
        {
            get
            {
                return Math.Abs(_sourceOpacity - _opacity) > double.Epsilon;
            }
        }

        public void ResetOpacity()
        {
            Opacity = SourceOpacity;
        }

        private bool SetOpacity(ref double opacity, double value)
        {
            if (opacity != value)
            {
                var val = Math.Round(Math.Clamp(value, 0, 1), 2);

                if (opacity != val)
                {
                    opacity = val;
                    return true;
                }
            }

            return false;
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void RaisePropertyChangedFromSource([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }
}
