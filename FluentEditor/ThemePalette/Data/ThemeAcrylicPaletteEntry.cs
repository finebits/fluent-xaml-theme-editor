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

            if (data.ContainsKey(nameof(TintOpacity)) && data.ContainsKey(nameof(TintLuminosityOpacity)))
            {
                var tintOpacity = data[nameof(TintOpacity)].GetNumber();
                double? tintLuminosityOpacity = null;
                var tintLuminosityOpacityString = data[nameof(TintLuminosityOpacity)].GetString();

                if (double.TryParse(tintLuminosityOpacityString, out var value))
                {
                    tintLuminosityOpacity = value;
                }

                return new ThemeEditableAcrylicPaletteEntry(tintOpacity, tintLuminosityOpacity, sourceColor, customColor, useCustomColor, data.GetOptionalString("Title"), data.GetOptionalString("Description"), activeColorStringFormat, contrastColors);
            }

            return new EditableColorPaletteEntry(sourceColor, customColor, useCustomColor, data.GetOptionalString("Title"), data.GetOptionalString("Description"), activeColorStringFormat, contrastColors);
        }

        public ThemeEditableAcrylicPaletteEntry(double sourceTintOpacity, double? sourceTintLuminosityOpacity, IColorPaletteEntry sourceColor, Color customColor, bool useCustomColor, string title, string description, FluentEditorShared.Utils.ColorStringFormat activeColorStringFormat, IReadOnlyList<ContrastColorWrapper> contrastColors)
            : base(sourceColor, customColor, useCustomColor, title, description, activeColorStringFormat, contrastColors)
        {
            SourceTintOpacity = sourceTintOpacity;
            TintOpacity = SourceTintOpacity;

            SourceTintLuminosityOpacity = sourceTintLuminosityOpacity;
            TintLuminosityOpacity = SourceTintLuminosityOpacity;
        }

        private double _sourceTintOpacity = 0;
        private double SourceTintOpacity
        {
            get { return _sourceTintOpacity; }
            set
            {
                SetDoubleValue(ref _sourceTintOpacity, value);
            }
        }

        private double? _sourceTintLuminosityOpacity = null;
        private double? SourceTintLuminosityOpacity
        {
            get { return _sourceTintLuminosityOpacity; }
            set
            {
                SetDoubleValue(ref _sourceTintLuminosityOpacity, value);
            }
        }

        private double _tintOpacity = 0;
        public double TintOpacity
        {
            get { return _tintOpacity; }
            set
            {
                if (SetDoubleValue(ref _tintOpacity, value))
                {
                    RaiseActiveColorChanged();
                    RaisePropertyChangedFromSource();
                }
            }
        }

        public bool IsCustomTintOpacity
        {
            get
            {
                return Math.Abs(_sourceTintOpacity - _tintOpacity) > double.Epsilon;
            }
        }

        private double _lastTintLuminosityOpacityValue = 0;
        public bool IsTintLuminosityOpacityOn
        {
            get
            {
                return TintLuminosityOpacity != null;
            }

            set
            {
                if(IsTintLuminosityOpacityOn == value)
                {
                    return;
                }

                if(value == true)
                {
                    TintLuminosityOpacity = _lastTintLuminosityOpacityValue;
                }
                else
                {
                    _lastTintLuminosityOpacityValue = TintLuminosityOpacity ?? 0;
                    TintLuminosityOpacity = null;
                }

                RaisePropertyChangedFromSource();
            }
        }

        private double? _tintLuminosityOpacity = null;
        public double? TintLuminosityOpacity
        {
            get { return _tintLuminosityOpacity; }
            set
            {
                if (SetDoubleValue(ref _tintLuminosityOpacity, value))
                {
                    RaiseActiveColorChanged();
                    RaisePropertyChangedFromSource();
                    RaisePropertyChanged(nameof(IsTintLuminosityOpacityOn));
                }
            }
        }

        public bool IsCustomTintLuminosityOpacity
        {
            get
            {
                if(_sourceTintLuminosityOpacity == null || _tintLuminosityOpacity == null)
                {
                     return _sourceTintLuminosityOpacity != _tintLuminosityOpacity;
                }

                return Math.Abs((_sourceTintLuminosityOpacity ?? 0) - (_tintLuminosityOpacity ?? 0)) > double.Epsilon;
            }
        }

        public void ResetOpacity()
        {
            TintOpacity = SourceTintOpacity;
            TintLuminosityOpacity = SourceTintLuminosityOpacity;
        }

        private bool SetDoubleValue(ref double property, double value)
        {
            if (property != value)
            {
                var val = Math.Round(Math.Clamp(value, 0, 1), 2);

                if (property != val)
                {
                    property = val;
                    return true;
                }
            }

            return false;
        }

        private bool SetDoubleValue(ref double? property, double? value)
        {
            if (property != value)
            {
                if(value == null)
                {
                    property = null;
                    return true;
                }

                var val = Math.Round(Math.Clamp(value ?? 0, 0, 1), 2);

                if (property != val)
                {
                    property = val;
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
