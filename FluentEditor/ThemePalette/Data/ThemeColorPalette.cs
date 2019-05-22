using FluentEditorShared.ColorPalette;
using FluentEditorShared.Utils;
using System.Collections.Generic;
using Windows.Data.Json;
using Windows.UI;

namespace FluentEditor.ThemePalette.Data
{
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

            return new ThemeColorPalette(steps, baseColor, contrastColors);
        }

        public ThemeColorPalette(int steps, Color baseColor, IReadOnlyList<ContrastColorWrapper> contrastColors)
            : base(steps, new ColorPaletteEntry(baseColor, null, null, ColorStringFormat.PoundRGB, null), contrastColors)
        { }

        public ThemeColorPalette(int steps, IColorPaletteEntry baseColor, IReadOnlyList<ContrastColorWrapper> contrastColors)
            : base(steps, baseColor, contrastColors)
        {
        }

        protected override void UpdatePaletteColors()
        {
            if(IsInterpolationEnabled)
            {
                base.UpdatePaletteColors();
            }
            else
            {
                for (int i = 0; i < _steps; i++)
                {
                    var c = BaseColor.ActiveColor;
                    _palette[i].SourceColor = new ColorPaletteEntry(c, null, null, BaseColor.ActiveColorStringFormat, null);
                }
            }
        }

        public bool IsInterpolationEnabled
        {
            get; set;
        } = true;
    }
}
