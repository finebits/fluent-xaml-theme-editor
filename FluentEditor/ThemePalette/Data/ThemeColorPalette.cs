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

            var palette = new ThemeColorPalette(steps, baseColor, contrastColors);

            if (data.ContainsKey(nameof(GenerationMode)))
            {
                var generationMode = data[nameof(GenerationMode)].GetEnum<GenerationMode>();

                if(generationMode == GenerationMode.Mix && data.ContainsKey(nameof(MixColors)))
                {
                    var mixColors = data[nameof(MixColors)].GetArray();

                    foreach (var item in mixColors)
                    {
                        var mixData = item.GetObject();
                        if (mixData.ContainsKey("Pattern") && mixData.ContainsKey("Mask"))
                        {
                            var pattern = mixData["Pattern"].GetColor();
                            var mask = mixData["Mask"].GetColor();
                            palette.MixColors.Add((pattern, mask));
                        }
                    }
                    palette.GenerationMode = generationMode;
                }
                else
                {
                    if (data.ContainsKey(nameof(ClipDark)))
                    {
                        palette.ClipDark = data[nameof(ClipDark)].GetNumber();
                    }

                    if (data.ContainsKey(nameof(ClipLight)))
                    {
                        palette.ClipLight = data[nameof(ClipLight)].GetNumber();
                    }

                    switch (generationMode)
                    {
                        case GenerationMode.RGB:
                            palette.InterpolationMode = ColorScaleInterpolationMode.RGB;
                            break;
                        case GenerationMode.XYZ:
                            palette.InterpolationMode = ColorScaleInterpolationMode.XYZ;
                            break;
                        case GenerationMode.LAB:
                            palette.InterpolationMode = ColorScaleInterpolationMode.LAB;
                            break;
                    }
                    palette.GenerationMode = generationMode;
                }
            }

            return palette;
        }

        public ThemeColorPalette(int steps, Color baseColor, IReadOnlyList<ContrastColorWrapper> contrastColors)
            : base(steps, new ColorPaletteEntry(baseColor, null, null, ColorStringFormat.PoundRGB, null), contrastColors)
        {}

        public ThemeColorPalette(int steps, IColorPaletteEntry baseColor, IReadOnlyList<ContrastColorWrapper> contrastColors)
            : base(steps, baseColor, contrastColors)
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
