// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using FluentEditorShared;
using FluentEditorShared.ColorPalette;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Storage;
using Windows.UI;
using System.Linq;
using FluentEditor.ControlPalette.CustomView.Common;
using System.ComponentModel;

namespace FluentEditor.ControlPalette.Model.Custom
{
    public interface ICustomPaletteModel
    {
        Task InitializeData(IStringProvider stringProvider, string dataPath);
        Task HandleAppSuspend();

        void AddOrReplacePreset(CustomPreset preset);
        void ApplyPreset(CustomPreset preset);
        ObservableList<CustomPreset> Presets { get; }
        CustomPreset ActivePreset { get; }
        event Action<ICustomPaletteModel> ActivePresetChanged;
        event Action<ICustomPaletteModel> ThemeChanged;

        Theme DarkTheme { get; }
        Theme LightTheme { get; }
    }

    public class CustomPaletteModel : ICustomPaletteModel
    {
        private Theme _darkTheme = null;
        public Theme DarkTheme
        {
            get
            {
                return _darkTheme;
            }
            private set
            {
                if(_darkTheme != null)
                {
                    _darkTheme.PropertyChanged -= OnThemePropertyChanged;
                }

                _darkTheme = value;

                if (_darkTheme != null)
                {
                    _darkTheme.PropertyChanged += OnThemePropertyChanged;
                }
            }
        }

        private Theme _lightTheme = null;
        public Theme LightTheme
        {
            get
            {
                return _lightTheme;
            }
            private set
            {
                if (_lightTheme != null)
                {
                    _lightTheme.PropertyChanged -= OnThemePropertyChanged;
                }

                _lightTheme = value;

                if (_lightTheme != null)
                {
                    _lightTheme.PropertyChanged += OnThemePropertyChanged;
                }
            }
        }

        public async Task InitializeData(IStringProvider stringProvider, string dataPath)
        {
            _stringProvider = stringProvider;

            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(dataPath));
            string dataString = await FileIO.ReadTextAsync(file);
            JsonObject rootObject = JsonObject.Parse(dataString);

            _presets = new ObservableList<CustomPreset>();
            if (rootObject.ContainsKey("Presets"))
            {
                var presetsNode = rootObject["Presets"].GetArray();
                foreach (var presetNode in presetsNode)
                {
                    _presets.Add(CustomPreset.Parse(presetNode.GetObject()));
                }
            }
            if (_presets.Count >= 1)
            {
                ApplyPreset(_presets[0]);
            }

            UpdateActivePreset();
        }

        private void OnThemePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateActivePreset();
        }

        private string GenerateMappingDescription(IColorPaletteEntry paletteEntry, List<ColorMapping> mappings)
        {
            string retVal = string.Empty;

            foreach (var mapping in mappings)
            {
                if (mapping.Source == paletteEntry)
                {
                    if (retVal != string.Empty)
                    {
                        retVal += ", ";
                    }
                    retVal += mapping.Target.ToString();
                }
            }

            if (retVal != string.Empty)
            {
                return string.Format(_stringProvider.GetString("ColorFlyoutMappingDescription"), retVal);
            }
            else
            {
                return null;
            }
        }

        public Task HandleAppSuspend()
        {
            // Currently nothing to do here
            return Task.CompletedTask;
        }

        private void UpdateActivePreset()
        {
            if (_presets != null)
            {
                for (int i = 0; i < _presets.Count; i++)
                {
                    if (_presets[i].IsPresetActive(this))
                    {
                        ActivePreset = _presets[i];
                        return;
                    }
                }
            }
            ActivePreset = null;
        }

        private IStringProvider _stringProvider;

        public void AddOrReplacePreset(CustomPreset preset)
        {
            if (!string.IsNullOrEmpty(preset.Name))
            {
                var oldPreset = _presets.FirstOrDefault<CustomPreset>((a) => a.Id == preset.Id);
                if (oldPreset != null)
                {
                    _presets.Remove(oldPreset);
                }
            }

            _presets.Add(preset);

            UpdateActivePreset();
        }

        public void ApplyPreset(CustomPreset preset)
        {
            if (preset == null)
            {
                ActivePreset = null;
                return;
            }

            DarkTheme = preset.CloneDarkTheme();
            LightTheme = preset.CloneLightTheme();

            UpdateActivePreset();

            ThemeChanged?.Invoke(this);
        }

        private void ApplyPresetOverrides(IReadOnlyList<EditableColorPaletteEntry> palette, Dictionary<int, Color> overrides)
        {
            for (int i = 0; i < palette.Count; i++)
            {
                if (overrides != null && overrides.ContainsKey(i))
                {
                    palette[i].CustomColor = overrides[i];
                    palette[i].UseCustomColor = true;
                }
                else
                {
                    palette[i].UseCustomColor = false;
                }
            }
        }

        private ObservableList<CustomPreset> _presets;
        public ObservableList<CustomPreset> Presets
        {
            get { return _presets; }
        }

        private CustomPreset _activePreset;
        public CustomPreset ActivePreset
        {
            get { return _activePreset; }
            private set
            {
                if (_activePreset != value)
                {
                    _activePreset = value;
                    ActivePresetChanged?.Invoke(this);
                }
            }
        }

        public event Action<ICustomPaletteModel> ActivePresetChanged;
        public event Action<ICustomPaletteModel> ThemeChanged;
    }
}
