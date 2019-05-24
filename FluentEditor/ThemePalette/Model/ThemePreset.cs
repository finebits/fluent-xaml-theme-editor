// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using FluentEditorShared.ColorPalette;
using FluentEditorShared.Utils;
using System.Collections.Generic;
using Windows.Data.Json;
using Windows.UI;

namespace FluentEditor.ThemePalette.Model
{
    public class ThemePreset
    {
        public static ThemePreset Parse(JsonObject data, string id = null, string name = null)
        {
            if (id == null)
            {
                id = data["Id"].GetString();
            }
            if (name == null)
            {
                name = data["Name"].GetString();
            }

            (Color, Dictionary<int, Color>) ParseBlock(string colorNodeName, string overridesNodeName = null)
            {
                Color color = data[colorNodeName].GetColor();

                Dictionary<int, Color> overrides = new Dictionary<int, Color>();
                if (!string.IsNullOrEmpty(overridesNodeName) && data.ContainsKey(overridesNodeName))
                {
                    var overridesNode = data[overridesNodeName].GetArray();
                    foreach (var v in overridesNode)
                    {
                        var node = v.GetObject();

                        int idx = node["Index"].GetInt();
                        Color clr = node["Color"].GetColor();
                        overrides.Add(idx, clr);
                    }
                }

                return (color, overrides);
            }

            var (lightRegionColor, lightRegionOverrides) = ParseBlock(nameof(LightRegionColor), nameof(DarkRegionOverrides));
            var (darkRegionColor, darkRegionOverrides) = ParseBlock(nameof(DarkRegionColor), nameof(DarkRegionOverrides));

            var (lightBaseColor, lightBaseOverrides) = ParseBlock(nameof(LightBaseColor), nameof(LightBaseOverrides));
            var (darkBaseColor, darkBaseOverrides) = ParseBlock(nameof(DarkBaseColor), nameof(DarkBaseOverrides));
            var (lightPrimaryColor, lightPrimaryOverrides) = ParseBlock(nameof(LightPrimaryColor), nameof(LightPrimaryOverrides));
            var (darkPrimaryColor, darkPrimaryOverrides) = ParseBlock(nameof(DarkPrimaryColor), nameof(DarkPrimaryOverrides));
            var (lightHyperlinkColor, lightHyperlinkOverrides) = ParseBlock(nameof(LightHyperlinkColor), nameof(LightHyperlinkOverrides));
            var (darkHyperlinkColor, darkHyperlinkOverrides) = ParseBlock(nameof(DarkHyperlinkColor), nameof(DarkHyperlinkOverrides));

            return new ThemePreset(id,
                                   name,
                                   lightRegionColor,
                                   lightRegionOverrides,
                                   darkRegionColor,
                                   darkRegionOverrides,
                                   lightBaseColor,
                                   lightBaseOverrides,
                                   darkBaseColor,
                                   darkBaseOverrides,
                                   lightPrimaryColor,
                                   lightPrimaryOverrides,
                                   darkPrimaryColor,
                                   darkPrimaryOverrides,
                                   lightHyperlinkColor,
                                   lightHyperlinkOverrides,
                                   darkHyperlinkColor,
                                   darkHyperlinkOverrides);
        }

        public static JsonObject Serialize(ThemePreset preset)
        {
            JsonObject data = new JsonObject();
            if (!string.IsNullOrEmpty(preset.Id))
            {
                data["Id"] = JsonValue.CreateStringValue(preset.Id);
            }
            if (!string.IsNullOrEmpty(preset.Name))
            {
                data["Name"] = JsonValue.CreateStringValue(preset.Name);
            }

            void SerializeBlock(Color color, string colorNodeName, Dictionary<int, Color> overrides = null, string overridesNodeName = null)
            {
                data[colorNodeName] = JsonValue.CreateStringValue(color.ToString());

                if(overrides != null && !string.IsNullOrEmpty(overridesNodeName))
                {
                    var overridesNode = new JsonArray();
                    foreach (int index in overrides.Keys)
                    {
                        JsonObject node = new JsonObject();
                        node["Index"] = JsonValue.CreateNumberValue(index);
                        node["Color"] = JsonValue.CreateStringValue(overrides[index].ToString());
                        overridesNode.Add(node);
                    }
                    data[overridesNodeName] = overridesNode;
                }
            }

            SerializeBlock(preset.LightRegionColor, nameof(LightRegionColor), preset.LightRegionOverrides, nameof(LightBaseOverrides));
            SerializeBlock(preset.DarkRegionColor, nameof(DarkRegionColor), preset.DarkRegionOverrides, nameof(DarkRegionOverrides));
            SerializeBlock(preset.LightBaseColor, nameof(LightBaseColor), preset.LightBaseOverrides, nameof(LightBaseOverrides));
            SerializeBlock(preset.DarkBaseColor, nameof(DarkBaseColor), preset.DarkBaseOverrides, nameof(DarkBaseOverrides));
            SerializeBlock(preset.LightPrimaryColor, nameof(LightPrimaryColor), preset.LightPrimaryOverrides, nameof(LightPrimaryOverrides));
            SerializeBlock(preset.DarkPrimaryColor, nameof(DarkPrimaryColor), preset.DarkPrimaryOverrides, nameof(DarkPrimaryOverrides));
            SerializeBlock(preset.LightHyperlinkColor, nameof(LightHyperlinkColor), preset.LightHyperlinkOverrides, nameof(LightHyperlinkOverrides));
            SerializeBlock(preset.DarkHyperlinkColor, nameof(DarkHyperlinkColor), preset.DarkHyperlinkOverrides, nameof(DarkHyperlinkOverrides));

            return data;
        }

        public ThemePreset(string id,
                           string name,
                           Color lightRegionColor,
                           Dictionary<int, Color> lightRegionOverrides,
                           Color darkRegionColor,
                           Dictionary<int, Color> darkRegionOverrides,
                           Color lightBaseColor,
                           Dictionary<int, Color> lightBaseOverrides,
                           Color darkBaseColor,
                           Dictionary<int, Color> darkBaseOverrides,
                           Color lightPrimaryColor,
                           Dictionary<int, Color> lightPrimaryOverrides,
                           Color darkPrimaryColor,
                           Dictionary<int, Color> darkPrimaryOverrides,
                           Color lightHyperlinkColor,
                           Dictionary<int, Color> lightHyperlinkOverrides,
                           Color darkHyperlinkColor,
                           Dictionary<int, Color> darkHyperlinkOverrides)
        {
            _id = id;
            _name = name;
            _lightRegionColor = lightRegionColor;
            _lightRegionOverrides = lightRegionOverrides;
            _darkRegionColor = darkRegionColor;
            _lightRegionOverrides = darkRegionOverrides;
            _lightBaseColor = lightBaseColor;
            _lightBaseOverrides = lightBaseOverrides;
            _darkBaseColor = darkBaseColor;
            _darkBaseOverrides = darkBaseOverrides;
            _lightPrimaryColor = lightPrimaryColor;
            _lightPrimaryOverrides = lightPrimaryOverrides;
            _darkPrimaryColor = darkPrimaryColor;
            _darkPrimaryOverrides = darkPrimaryOverrides;
            _lightHyperlinkColor = lightHyperlinkColor;
            _lightHyperlinkOverrides = lightHyperlinkOverrides;
            _darkHyperlinkColor = darkHyperlinkColor;
            _darkHyperlinkOverrides = darkHyperlinkOverrides;
        }

        public ThemePreset(string id, string name, IThemePaletteModel model)
        {
            _id = id;
            _name = name;

            Dictionary<int, Color> Clone(ColorPalette source)
            {
                var overrides = new Dictionary<int, Color>();
                for (int i = 0; i < source.Palette.Count; i++)
                {
                    if (source.Palette[i].UseCustomColor)
                    {
                        overrides.Add(i, source.Palette[i].ActiveColor);
                    }
                }

                return overrides;
            }

            _lightRegionColor = model.LightRegion.BaseColor.ActiveColor;
            _lightRegionOverrides = Clone(model.LightBase);

            _darkRegionColor = model.DarkRegion.BaseColor.ActiveColor;
            _darkRegionOverrides = Clone(model.DarkBase);

            _lightBaseColor = model.LightBase.BaseColor.ActiveColor;
            _lightBaseOverrides = Clone(model.LightBase);

            _darkBaseColor = model.DarkBase.BaseColor.ActiveColor;
            _darkBaseOverrides = Clone(model.DarkBase);

            _lightPrimaryColor = model.LightPrimary.BaseColor.ActiveColor;
            _lightPrimaryOverrides = Clone(model.LightPrimary);

            _darkPrimaryColor = model.DarkPrimary.BaseColor.ActiveColor;
            _darkPrimaryOverrides = Clone(model.DarkPrimary);

            _lightHyperlinkColor = model.LightHyperlink.BaseColor.ActiveColor;
            _lightHyperlinkOverrides = Clone(model.LightHyperlink);

            _darkHyperlinkColor = model.DarkHyperlink.BaseColor.ActiveColor;
            _darkHyperlinkOverrides = Clone(model.DarkHyperlink);
        }

        private readonly string _id;
        public string Id { get { return _id; } }

        private readonly string _name;
        public string Name { get { return _name; } }

        private readonly Color _lightRegionColor;
        public Color LightRegionColor { get { return _lightRegionColor; } }

        private readonly Dictionary<int, Color> _lightRegionOverrides;
        public Dictionary<int, Color> LightRegionOverrides { get { return _lightRegionOverrides; } }

        private readonly Color _darkRegionColor;
        public Color DarkRegionColor { get { return _darkRegionColor; } }

        private readonly Dictionary<int, Color> _darkRegionOverrides;
        public Dictionary<int, Color> DarkRegionOverrides { get { return _darkRegionOverrides; } }

        private readonly Color _lightBaseColor;
        public Color LightBaseColor { get { return _lightBaseColor; } }

        private readonly Dictionary<int, Color> _lightBaseOverrides;
        public Dictionary<int, Color> LightBaseOverrides { get { return _lightBaseOverrides; } }

        private readonly Color _darkBaseColor;
        public Color DarkBaseColor { get { return _darkBaseColor; } }

        private readonly Dictionary<int, Color> _darkBaseOverrides;
        public Dictionary<int, Color> DarkBaseOverrides { get { return _darkBaseOverrides; } }

        private readonly Color _lightPrimaryColor;
        public Color LightPrimaryColor { get { return _lightPrimaryColor; } }

        private readonly Dictionary<int, Color> _lightPrimaryOverrides;
        public Dictionary<int, Color> LightPrimaryOverrides { get { return _lightPrimaryOverrides; } }

        private readonly Color _darkPrimaryColor;
        public Color DarkPrimaryColor { get { return _darkPrimaryColor; } }

        private readonly Dictionary<int, Color> _darkPrimaryOverrides;
        public Dictionary<int, Color> DarkPrimaryOverrides { get { return _darkPrimaryOverrides; } }

        private readonly Color _lightHyperlinkColor;
        public Color LightHyperlinkColor { get { return _lightHyperlinkColor; } }

        private readonly Dictionary<int, Color> _lightHyperlinkOverrides;
        public Dictionary<int, Color> LightHyperlinkOverrides { get { return _lightHyperlinkOverrides; } }

        private readonly Color _darkHyperlinkColor;
        public Color DarkHyperlinkColor { get { return _darkHyperlinkColor; } }

        private readonly Dictionary<int, Color> _darkHyperlinkOverrides;
        public Dictionary<int, Color> DarkHyperlinkOverrides { get { return _darkHyperlinkOverrides; } }

        public bool IsPresetActive(IThemePaletteModel model)
        {
            if (model == null)
            {
                return false;
            }
            if (model.LightRegion.BaseColor.ActiveColor != _lightRegionColor)
            {
                return false;
            }
            if (model.DarkRegion.BaseColor.ActiveColor != _darkRegionColor)
            {
                return false;
            }
            if (model.LightBase.BaseColor.ActiveColor != _lightBaseColor)
            {
                return false;
            }
            if (model.DarkBase.BaseColor.ActiveColor != _darkBaseColor)
            {
                return false;
            }
            if (model.LightPrimary.BaseColor.ActiveColor != _lightPrimaryColor)
            {
                return false;
            }
            if (model.DarkPrimary.BaseColor.ActiveColor != _darkPrimaryColor)
            {
                return false;
            }
            if (model.LightHyperlink.BaseColor.ActiveColor != _lightHyperlinkColor)
            {
                return false;
            }
            if (model.DarkHyperlink.BaseColor.ActiveColor != _darkHyperlinkColor)
            {
                return false;
            }
            if (!CompareOverrides(_lightRegionOverrides, model.LightRegion.Palette))
            {
                return false;
            }
            if (!CompareOverrides(_darkRegionOverrides, model.DarkRegion.Palette))
            {
                return false;
            }
            if (!CompareOverrides(_lightBaseOverrides, model.LightBase.Palette))
            {
                return false;
            }
            if (!CompareOverrides(_darkBaseOverrides, model.DarkBase.Palette))
            {
                return false;
            }
            if (!CompareOverrides(_lightPrimaryOverrides, model.LightPrimary.Palette))
            {
                return false;
            }
            if (!CompareOverrides(_darkPrimaryOverrides, model.DarkPrimary.Palette))
            {
                return false;
            }
            if (!CompareOverrides(_lightHyperlinkOverrides, model.LightHyperlink.Palette))
            {
                return false;
            }
            if (!CompareOverrides(_darkHyperlinkOverrides, model.DarkHyperlink.Palette))
            {
                return false;
            }

            return true;
        }

        private bool CompareOverrides(Dictionary<int, Color> presetDictionary, IReadOnlyList<EditableColorPaletteEntry> palette)
        {
            if (presetDictionary == null || palette == null)
            {
                return false;
            }

            for (int i = 0; i < palette.Count; i++)
            {
                if (palette[i].UseCustomColor)
                {
                    if (!presetDictionary.ContainsKey(i) || presetDictionary[i] != palette[i].ActiveColor)
                    {
                        return false;
                    }
                }
            }

            var keys = presetDictionary.Keys;
            foreach (var index in keys)
            {
                if (index >= palette.Count || index < 0)
                {
                    return false;
                }
                if (!palette[index].UseCustomColor || palette[index].ActiveColor != presetDictionary[index])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
