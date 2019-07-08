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

            if (data.ContainsKey(nameof(Opacity)) && data.ContainsKey(nameof(LuminosityOpacity)))
            {
                var opacity = data[nameof(Opacity)].GetNumber();
                double? luminosityOpacity = null;
                var luminosityOpacityString = data[nameof(LuminosityOpacity)].GetString();

                if (double.TryParse(luminosityOpacityString, out var value))
                {
                    luminosityOpacity = value;
                }

                return new ThemeEditableAcrylicPaletteEntry(opacity, luminosityOpacity, sourceColor, customColor, useCustomColor, data.GetOptionalString("Title"), data.GetOptionalString("Description"), activeColorStringFormat, contrastColors);
            }

            return new EditableColorPaletteEntry(sourceColor, customColor, useCustomColor, data.GetOptionalString("Title"), data.GetOptionalString("Description"), activeColorStringFormat, contrastColors);
        }

        public ThemeEditableAcrylicPaletteEntry(double sourceOpacity, double? sourceLuminosityOpacity, IColorPaletteEntry sourceColor, Color customColor, bool useCustomColor, string title, string description, FluentEditorShared.Utils.ColorStringFormat activeColorStringFormat, IReadOnlyList<ContrastColorWrapper> contrastColors)
            : base(sourceColor, customColor, useCustomColor, title, description, activeColorStringFormat, contrastColors)
        {
            SourceOpacity = sourceOpacity;
            Opacity = SourceOpacity;

            SourceLuminosityOpacity = sourceLuminosityOpacity;
            LuminosityOpacity = SourceLuminosityOpacity;
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

        private double? _sourceLuminosityOpacity = null;
        private double? SourceLuminosityOpacity
        {
            get { return _sourceLuminosityOpacity; }
            set
            {
                SetOpacity(ref _sourceLuminosityOpacity, value);
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

        private double _lastLuminosityOpacityValue = 0;
        public bool IsLuminosityOpacityOn
        {
            get
            {
                return LuminosityOpacity != null;
            }

            set
            {
                if(IsLuminosityOpacityOn == value)
                {
                    return;
                }

                if(value == true)
                {
                    LuminosityOpacity = _lastLuminosityOpacityValue;
                }
                else
                {
                    _lastLuminosityOpacityValue = LuminosityOpacity ?? 0;
                    LuminosityOpacity = null;
                }

                RaisePropertyChangedFromSource();
            }
        }

        private double? _luminosityOpacity = null;
        public double? LuminosityOpacity
        {
            get { return _luminosityOpacity; }
            set
            {
                if (SetOpacity(ref _luminosityOpacity, value))
                {
                    RaiseActiveColorChanged();
                    RaisePropertyChangedFromSource();
                    RaisePropertyChanged(nameof(IsLuminosityOpacityOn));
                }
            }
        }

        public bool IsCustomLuminosityOpacity
        {
            get
            {
                if(_sourceLuminosityOpacity == null || _luminosityOpacity == null)
                {
                     return _sourceLuminosityOpacity != _luminosityOpacity;
                }

                return Math.Abs((_sourceLuminosityOpacity ?? 0) - (_luminosityOpacity ?? 0)) > double.Epsilon;
            }
        }

        public void ResetOpacity()
        {
            Opacity = SourceOpacity;
            LuminosityOpacity = SourceLuminosityOpacity;
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

        private bool SetOpacity(ref double? opacity, double? value)
        {
            if (opacity != value)
            {
                if(value == null)
                {
                    opacity = null;
                    return true;
                }

                var val = Math.Round(Math.Clamp(value ?? 0, 0, 1), 2);

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
            RaisePropertyChanged(name);
        }

        #endregion
    }
}
