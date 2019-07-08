// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using FluentEditor.ThemePalette.Data;
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

            (Color, Dictionary<int, Color>, Dictionary<int, (double, double?)>) ParseBlock(string colorNodeName, string overridesNodeName = null, string acrylicOverridesNodeName = null)
            {
                Color color = data[colorNodeName].GetColor();

                Dictionary<int, Color> overrides = new Dictionary<int, Color>();
                Dictionary<int, (double, double?)> acrylicOverrides = new Dictionary<int, (double, double?)>();

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

                if (!string.IsNullOrEmpty(acrylicOverridesNodeName) && data.ContainsKey(acrylicOverridesNodeName))
                {
                    var overridesNode = data[acrylicOverridesNodeName].GetArray();
                    foreach (var v in overridesNode)
                    {
                        var node = v.GetObject();

                        int idx = node["Index"].GetInt();
                        double tintOpacity = node["TintOpacity"].GetNumber();

                        string tintLuminosityOpacityString = node["TintLuminosityOpacity"].GetString();
                        double? tintLuminosityOpacity = null;
                        if (double.TryParse(tintLuminosityOpacityString, out double value))
                        {
                            tintLuminosityOpacity = value;
                        }

                        acrylicOverrides.Add(idx, (tintOpacity, tintLuminosityOpacity));
                    }
                }

                return (color, overrides, acrylicOverrides);
            }

            var (lightRegionColor, lightRegionOverrides, lightRegionAcrylicOverrides) = ParseBlock(nameof(LightRegionColor), nameof(LightRegionOverrides), nameof(LightRegionAcrylicOverrides));
            var (darkRegionColor, darkRegionOverrides, darkRegionAcrylicOverrides) = ParseBlock(nameof(DarkRegionColor), nameof(DarkRegionOverrides), nameof(DarkRegionAcrylicOverrides));

            var (lightBaseColor, lightBaseOverrides, _) = ParseBlock(nameof(LightBaseColor), nameof(LightBaseOverrides));
            var (darkBaseColor, darkBaseOverrides, _) = ParseBlock(nameof(DarkBaseColor), nameof(DarkBaseOverrides));
            var (lightPrimaryColor, lightPrimaryOverrides, _) = ParseBlock(nameof(LightPrimaryColor), nameof(LightPrimaryOverrides));
            var (darkPrimaryColor, darkPrimaryOverrides, _) = ParseBlock(nameof(DarkPrimaryColor), nameof(DarkPrimaryOverrides));
            var (lightHyperlinkColor, lightHyperlinkOverrides, _) = ParseBlock(nameof(LightHyperlinkColor), nameof(LightHyperlinkOverrides));
            var (darkHyperlinkColor, darkHyperlinkOverrides, _) = ParseBlock(nameof(DarkHyperlinkColor), nameof(DarkHyperlinkOverrides));

            return new ThemePreset(id,
                                   name,
                                   lightRegionColor,
                                   lightRegionOverrides,
                                   lightRegionAcrylicOverrides,
                                   darkRegionColor,
                                   darkRegionOverrides,
                                   darkRegionAcrylicOverrides,
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

            void SerializeBlock(Color color, string colorNodeName, Dictionary<int, Color> overrides = null, string overridesNodeName = null, Dictionary<int, (double,double?)> acrylicOverrides = null, string acrylicOverridesNodeName = null)
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

                if (acrylicOverrides != null && !string.IsNullOrEmpty(acrylicOverridesNodeName))
                {
                    var acrylicOverridesNode = new JsonArray();
                    foreach ((int index, (double tintOpacity, double? tintLuminosityOpacity)) in acrylicOverrides)
                    {
                        JsonObject node = new JsonObject();
                        node["Index"] = JsonValue.CreateNumberValue(index);
                        node["TintOpacity"] = JsonValue.CreateNumberValue(tintOpacity);

                        var tintLuminosityOpacityString = (tintLuminosityOpacity == null) ? "null": tintLuminosityOpacity.ToString();
                        node["TintLuminosityOpacity"] = JsonValue.CreateStringValue(tintLuminosityOpacityString);

                        acrylicOverridesNode.Add(node);
                    }
                    data[acrylicOverridesNodeName] = acrylicOverridesNode;
                }
            }

            SerializeBlock(preset.LightRegionColor, nameof(LightRegionColor), preset.LightRegionOverrides, nameof(LightRegionOverrides), preset.LightRegionAcrylicOverrides, nameof(LightRegionAcrylicOverrides));
            SerializeBlock(preset.DarkRegionColor, nameof(DarkRegionColor), preset.DarkRegionOverrides, nameof(DarkRegionOverrides), preset.DarkRegionAcrylicOverrides, nameof(DarkRegionAcrylicOverrides));
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
                           Dictionary<int, (double, double?)> lightRegionAcrylicOverrides,
                           Color darkRegionColor,
                           Dictionary<int, Color> darkRegionOverrides,
                           Dictionary<int, (double, double?)> darkRegionAcrylicOverrides,
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
            _lightRegionAcrylicOverrides = lightRegionAcrylicOverrides;
            _darkRegionColor = darkRegionColor;
            _darkRegionOverrides = darkRegionOverrides;
            _darkRegionAcrylicOverrides = darkRegionAcrylicOverrides;
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

            Dictionary<int, (double, double?)> CloneAcrylic(ColorPalette source)
            {
                var overrides = new Dictionary<int, (double, double?)>();
                for (int i = 0; i < source.Palette.Count; i++)
                {
                    if (source.Palette[i] is ThemeEditableAcrylicPaletteEntry acrylicPalette && (acrylicPalette.IsCustomTintOpacity || acrylicPalette.IsCustomTintLuminosityOpacity))
                    {
                        overrides.Add(i, (acrylicPalette.TintOpacity, acrylicPalette.TintLuminosityOpacity));
                    }
                }

                return overrides;
            }

            _lightRegionColor = model.LightRegion.BaseColor.ActiveColor;
            _lightRegionOverrides = Clone(model.LightRegion);
            _lightRegionAcrylicOverrides = CloneAcrylic(model.LightRegion);

            _darkRegionColor = model.DarkRegion.BaseColor.ActiveColor;
            _darkRegionOverrides = Clone(model.DarkRegion);
            _darkRegionAcrylicOverrides = CloneAcrylic(model.DarkRegion);

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

        private readonly Dictionary<int, (double, double?)> _lightRegionAcrylicOverrides;
        public Dictionary<int, (double,double?)> LightRegionAcrylicOverrides { get { return _lightRegionAcrylicOverrides; } }

        private readonly Dictionary<int, Color> _lightRegionOverrides;
        public Dictionary<int, Color> LightRegionOverrides { get { return _lightRegionOverrides; } }

        private readonly Color _darkRegionColor;
        public Color DarkRegionColor { get { return _darkRegionColor; } }

        private readonly Dictionary<int, (double, double?)> _darkRegionAcrylicOverrides;
        public Dictionary<int, (double, double?)> DarkRegionAcrylicOverrides { get { return _darkRegionAcrylicOverrides; } }

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
            if (!CompareAcrylicOverrides(_lightRegionAcrylicOverrides, model.LightRegion.Palette))
            {
                return false;
            }
            if (!CompareOverrides(_darkRegionOverrides, model.DarkRegion.Palette))
            {
                return false;
            }
            if (!CompareAcrylicOverrides(_darkRegionAcrylicOverrides, model.DarkRegion.Palette))
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

        private bool CompareAcrylicOverrides(Dictionary<int, (double,double?)> presetDictionary, IReadOnlyList<EditableColorPaletteEntry> palette)
        {
            if (presetDictionary == null || palette == null)
            {
                return false;
            }

            for (int i = 0; i < palette.Count; i++)
            {
                if (palette[i] is ThemeEditableAcrylicPaletteEntry acrylicPalette && (acrylicPalette.IsCustomTintOpacity || acrylicPalette.IsCustomTintLuminosityOpacity))
                {
                    if (!presetDictionary.ContainsKey(i) || presetDictionary[i].Item1 != acrylicPalette.TintOpacity || presetDictionary[i].Item2 != acrylicPalette.TintLuminosityOpacity)
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
                if (palette[index] is ThemeEditableAcrylicPaletteEntry acrylicPalette)
                {
                    if (!(acrylicPalette.IsCustomTintOpacity || acrylicPalette.IsCustomTintLuminosityOpacity) || acrylicPalette.TintOpacity != presetDictionary[index].Item1 || acrylicPalette.TintLuminosityOpacity != presetDictionary[index].Item2)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
