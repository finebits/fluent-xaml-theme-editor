// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using Windows.Data.Json;
using Windows.UI;
using FluentEditorShared.ColorPalette;
using FluentEditor.ControlPalette.ThemePaletteView.Common;

namespace FluentEditor.ControlPalette.Model.ThemePalette
{
    public class ThemePreset
    {
        private readonly string _id;
        public string Id { get { return _id; } }

        private readonly string _name;
        public string Name { get { return _name; } }

        public bool IsSystemTheme { get; private set; }

        private Theme DarkTheme { get; set; }
        private Theme LightTheme { get; set; }

        public static ThemePreset Parse(JsonObject data, string id = null, string name = null)
        {
            if (id == null)
            {
                id = data[nameof(Id)].GetString();
            }
            if (name == null)
            {
                name = data[nameof(Name)].GetString();
            }

            bool isSystemTheme = false;
            if (data.ContainsKey(nameof(IsSystemTheme)))
            {
                isSystemTheme = data[nameof(IsSystemTheme)].GetBoolean();
            }

            Theme darkTheme = new Theme();
            Theme lightTheme = new Theme();

            if(!isSystemTheme)
            {
                darkTheme.Load(data[nameof(DarkTheme)].GetObject());
                lightTheme.Load(data[nameof(LightTheme)].GetObject());
            }

            return new ThemePreset(id, name, darkTheme, lightTheme, isSystemTheme);
        }

        public static JsonObject Serialize(ThemePreset preset)
        {
            JsonObject data = new JsonObject();
            if (!string.IsNullOrEmpty(preset.Id))
            {
                data[nameof(Id)] = JsonValue.CreateStringValue(preset.Id);
            }
            if (!string.IsNullOrEmpty(preset.Name))
            {
                data[nameof(Name)] = JsonValue.CreateStringValue(preset.Name);
            }

            JsonObject darkTheme = preset.DarkTheme.Serialize();
            JsonObject lightTheme = preset.LightTheme.Serialize();

            data[nameof(DarkTheme)] = darkTheme;
            data[nameof(LightTheme)] = lightTheme;
            data[nameof(IsSystemTheme)] = JsonValue.CreateBooleanValue(false);

            return data;
        }

        private ThemePreset(string id, string name, Theme darkTheme, Theme lightTheme, bool isSystemTheme)
        {
            _id = id;
            _name = name;
            IsSystemTheme = isSystemTheme;

            DarkTheme = new Theme(darkTheme);
            LightTheme = new Theme(lightTheme);
        }

        public ThemePreset(string id, string name, IThemePaletteModel model)
        {
            _id = id;
            _name = name;

            DarkTheme = new Theme(model.DarkTheme);
            LightTheme = new Theme(model.LightTheme);
        }

        public ThemePreset(ThemePreset preset)
        {
            _id = preset._id;
            _name = preset._name;
            IsSystemTheme = preset.IsSystemTheme;

            DarkTheme = new Theme(preset.DarkTheme);
            LightTheme = new Theme(preset.LightTheme);
        }

        public bool IsPresetActive(IThemePaletteModel model)
        {
            if (model == null || DarkTheme == null || LightTheme == null)
            {
                return false;
            }

            if(IsSystemTheme && model.IsSystemTheme)
            {
                return true;
            }

            return DarkTheme.IsEquals(model.DarkTheme) && LightTheme.IsEquals(model.LightTheme);
        }

        public Theme CloneDarkTheme()
        {
            return new Theme(DarkTheme);
        }

        public Theme CloneLightTheme()
        {
            return new Theme(LightTheme);
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
