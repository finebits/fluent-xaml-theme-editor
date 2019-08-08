using FluentEditor.ThemePalette.Data;
using FluentEditorShared.ColorPalette;
using FluentEditorShared.Utils;
using System;
using System.Collections.Generic;
using Windows.Data.Json;
using Windows.UI;

namespace FluentEditor.ThemePalette.Data
{
    public enum GenerationMode { RGB, LAB, XYZ, Mix };

    internal class ThemeColorPalette : ColorPalette
    {
        public class ThemePaletteEntryData : ColorPalette.PaletteEntryData
        {
            protected readonly IReadOnlyList<(string title, string description)> _paletteEntryCaptions;
            protected readonly IReadOnlyList<(int index, bool isHostBackdrop, double tintOpacity, double? tintLuminosityOpacity)> _paletteEntryAcrylics;

            public ThemePaletteEntryData(IColorPaletteEntry baseColor, 
                                        IReadOnlyList<ContrastColorWrapper> contrastColors, 
                                        IReadOnlyList<(string title, string description)> captions,
                                        IReadOnlyList<(int index, bool isHostBackdrop, double tintOpacity, double? tintLuminosityOpacity)> acrylics)
                : base(baseColor, contrastColors)
            {
                _paletteEntryCaptions = captions;
                _paletteEntryAcrylics = acrylics;
            }

            public override IReadOnlyList<ContrastColorWrapper> GetPaletteEntryContrastColors(int index)
            {
                return base.GetPaletteEntryContrastColors(index);
            }

            public override string GetPaletteEntryDescription(int index)
            {
                var result = base.GetPaletteEntryDescription(index);
                if (_paletteEntryCaptions != null && index >= 0 && index < _paletteEntryCaptions.Count)
                {
                    if (!string.IsNullOrEmpty(_paletteEntryCaptions[index].description))
                    {
                        result = _paletteEntryCaptions[index].description;
                    }
                }

                return result;
            }

            public override string GetPaletteEntryTitle(int index)
            {
                var result = base.GetPaletteEntryTitle(index);
                if (_paletteEntryCaptions != null && index >= 0 && index < _paletteEntryCaptions.Count)
                {
                    if (!string.IsNullOrEmpty(_paletteEntryCaptions[index].title))
                    {
                        result = _paletteEntryCaptions[index].title;
                    }
                }

                return result;
            }

            public override EditableColorPaletteEntry GetColorPaletteEntry(IColorPaletteEntry baseColor, int idx)
            {
                (bool isAcrylic, bool isHostBackdrop, double tintOpacity, double? tintLuminosityOpacity) IsAcrylic()
                {
                    if(_paletteEntryAcrylics != null)
                    {
                        foreach (var acrylic in _paletteEntryAcrylics)
                        {
                            if (acrylic.index == idx)
                            {
                                return (true, acrylic.isHostBackdrop, acrylic.tintOpacity, acrylic.tintLuminosityOpacity);
                            }
                        }
                    }

                    return (false, false, 0, 0);
                }

                (bool isAcrylic, bool isHostBackdrop, double tintOpacity, double? tintLuminosityOpacity) = IsAcrylic();

                if (isAcrylic)
                {
                    return new ThemeEditableAcrylicPaletteEntry(isHostBackdrop, tintOpacity, tintLuminosityOpacity, null, default, false, GetPaletteEntryTitle(idx), GetPaletteEntryDescription(idx), baseColor.ActiveColorStringFormat, GetPaletteEntryContrastColors(idx));
                }
                else
                {
                    return base.GetColorPaletteEntry(baseColor, idx);
                }
            }

            public static IReadOnlyList<(string title, string description)> ParseCaptions(JsonArray data)
            {
                var result = new List<(string title, string description)>();

                foreach (var item in data)
                {
                    var caption = item.GetObject();
                    string title = caption.ContainsKey("Title") ? caption["Title"].GetOptionalString() : null;
                    string description = caption.ContainsKey("Description") ? caption["Description"].GetOptionalString() : null;

                    result.Add((title, description));
                }

                return result;
            }

            public static IReadOnlyList<(int index, bool isHostBackdrop, double tintOpacity, double? tintLuminosityOpacity)> ParseAcrylics(JsonArray data)
            {
                var result = new List<(int index, bool isHostBackdrop, double tintOpacity, double? tintLuminosityOpacity)>();

                foreach (var item in data)
                {
                    var acrylic = item.GetObject();
                    int index = acrylic.ContainsKey("SourceIndex") ? acrylic["SourceIndex"].GetInt() : -1;
                    bool isHostBackdrop = acrylic.ContainsKey("IsHostBackdrop") ? acrylic["IsHostBackdrop"].GetBoolean() : false;
                    double tintOpacity = acrylic.ContainsKey("TintOpacity") ? acrylic["TintOpacity"].GetNumber() : 0.0;
                    double? tintLuminosityOpacity = null;

                    if(acrylic.ContainsKey("TintLuminosityOpacity"))
                    {
                        var tintLuminosityOpacityString = acrylic["TintLuminosityOpacity"].GetString();

                        if(double.TryParse(tintLuminosityOpacityString, out var value))
                        {
                            tintLuminosityOpacity = value;
                        }
                    }

                    result.Add((index, isHostBackdrop, tintOpacity, tintLuminosityOpacity));
                }

                return result;
            }
        }

        public new static ThemeColorPalette Parse(JsonObject data, IReadOnlyList<ContrastColorWrapper> contrastColors)
        {
            IColorPaletteEntry baseColor = null;
            if (data.ContainsKey("EditableBaseColor"))
            {
                baseColor = EditableColorPaletteEntry.Parse(data["EditableBaseColor"].GetObject(), null, contrastColors);
            }
            else if (data.ContainsKey("BaseColor"))
            {
                baseColor = ColorPaletteEntry.Parse(data["BaseColor"].GetObject(), contrastColors);
            }

            int steps = data["Steps"].GetInt();

            IPaletteEntryData paletteEntryData = new ColorPalette.PaletteEntryData(baseColor, contrastColors);
            IReadOnlyList<(string title, string description)> captions = null;
            IReadOnlyList<(int index, bool isHostBackdrop, double tintOpacity, double? tintLuminosityOpacity)> opacities = null;

            if (data.ContainsKey("PaletteEntryCaptions"))
            {
                captions = ThemePaletteEntryData.ParseCaptions(data["PaletteEntryCaptions"].GetArray());
            }

            if (data.ContainsKey("AcrylicData"))
            {
                opacities = ThemePaletteEntryData.ParseAcrylics(data["AcrylicData"].GetArray());
            }

            if(captions!= null || opacities != null)
            {
                paletteEntryData = new ThemePaletteEntryData(baseColor, contrastColors, captions, opacities);
            }

            var palette = new ThemeColorPalette(steps, baseColor, contrastColors, paletteEntryData);

            if(data.ContainsKey("PaletteGenerator"))
            {
                palette.UsePaletteGenerator(data["PaletteGenerator"].GetObject());
            }

            return palette;
        }

        public ThemeColorPalette(int steps, IColorPaletteEntry baseColor, IReadOnlyList<ContrastColorWrapper> contrastColors, IPaletteEntryData paletteEntryData)
            : base(steps, baseColor, contrastColors, paletteEntryData)
        {}

        protected override void UpdatePaletteColors()
        {
            if(GenerationMode == GenerationMode.Mix)
            {
                GetMixPaletteColors();
            }
            else
            {
                base.UpdatePaletteColors();
            }
        }

        private void UsePaletteGenerator(JsonObject data)
        {
            if (data.ContainsKey("Mode"))
            {
                var generationMode = data["Mode"].GetEnum<GenerationMode>();

                if (generationMode == GenerationMode.Mix && data.ContainsKey(nameof(MixColors)))
                {
                    var mixColors = data[nameof(MixColors)].GetArray();

                    foreach (var item in mixColors)
                    {
                        var mixData = item.GetObject();
                        if (mixData.ContainsKey("Pattern") && mixData.ContainsKey("Mask"))
                        {
                            var pattern = mixData["Pattern"].GetColor();
                            var mask = mixData["Mask"].GetColor();
                            MixColors.Add((pattern, mask));
                        }
                    }

                    GenerationMode = generationMode;
                }
                else
                {
                    if (data.ContainsKey(nameof(ClipDark)))
                    {
                        ClipDark = data[nameof(ClipDark)].GetNumber();
                    }

                    if (data.ContainsKey(nameof(ClipLight)))
                    {
                        ClipLight = data[nameof(ClipLight)].GetNumber();
                    }

                    switch (generationMode)
                    {
                        case GenerationMode.RGB:
                            InterpolationMode = ColorScaleInterpolationMode.RGB;
                            break;
                        case GenerationMode.XYZ:
                            InterpolationMode = ColorScaleInterpolationMode.XYZ;
                            break;
                        case GenerationMode.LAB:
                            InterpolationMode = ColorScaleInterpolationMode.LAB;
                            break;
                    }

                    GenerationMode = generationMode;
                }
            }
        }

        private void GetMixPaletteColors()
        {
            for (int i = 0; i < _steps; i++)
            {
                 var color = GetMixColor(i, BaseColor.ActiveColor);
                _palette[i].SourceColor = new ColorPaletteEntry(color, null, null, BaseColor.ActiveColorStringFormat, null);
            }
        }

        private Color GetMixColor(int index, Color baseColor)
        {
            if(index >= 0 && index < MixColors.Count)
            {
                var rawPattern = GetRaw(MixColors[index].pattern);
                var rawMask = GetRaw(MixColors[index].mask);
                var rawBaseColor = GetRaw(baseColor);

                var rawColor = (rawPattern & rawMask) + (rawBaseColor & ~rawMask);

                return GetColorFromRaw(rawColor);
            }

            return baseColor;
        }

        private UInt32 GetRaw(Color color)
        {
            return ((UInt32)color.A << 24) + ((UInt32)color.R << 16) + ((UInt32)color.G << 8) + color.B;
        }

        private Color GetColorFromRaw(UInt32 raw)
        {
            return Color.FromArgb(
                    (byte)((raw & 0xFF000000) >> 24),
                    (byte)((raw & 0x00FF0000) >> 16),
                    (byte)((raw & 0x0000FF00) >> 8),
                    (byte)(raw & 0x000000FF));
        }

        protected GenerationMode _generationMode = GenerationMode.RGB;
        public GenerationMode GenerationMode
        {
            get { return _generationMode; }
            set
            {
                if (_generationMode != value)
                {
                    _generationMode = value;
                    UpdatePaletteColors();
                }
            }
        }

        private IList<(Color pattern, Color mask)> MixColors { get; set; } = new List<(Color pattern, Color mask)>();
    }
}
