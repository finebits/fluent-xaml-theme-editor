// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using FluentEditor.ThemePalette.Data;
using FluentEditorShared;
using FluentEditorShared.ColorPalette;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Storage;
using Windows.UI;

namespace FluentEditor.ThemePalette.Model
{
    public interface IThemePaletteModel
    {
        Task InitializeData(IStringProvider stringProvider, string dataPath);
        Task HandleAppSuspend();

        void AddOrReplacePreset(ThemePreset preset);
        void ApplyPreset(ThemePreset preset);
        ObservableList<ThemePreset> Presets { get; }
        ThemePreset ActivePreset { get; }
        event Action<IThemePaletteModel> ActivePresetChanged;

        IReadOnlyList<ThemeColorMapping> LightColorMapping { get; }
        IReadOnlyList<ThemeColorMapping> DarkColorMapping { get; }
        ColorPalette LightRegion { get; }
        ColorPalette DarkRegion { get; }
        ColorPalette LightBase { get; }
        ColorPalette DarkBase { get; }
        ColorPalette LightPrimary { get; }
        ColorPalette DarkPrimary { get; }
        ColorPalette LightHyperlink { get; }
        ColorPalette DarkHyperlink { get; }
    }

    public class ThemePaletteModel : IThemePaletteModel
    {
        public async Task InitializeData(IStringProvider stringProvider, string dataPath)
        {
            _stringProvider = stringProvider;

            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(dataPath));
            string dataString = await FileIO.ReadTextAsync(file);
            JsonObject rootObject = JsonObject.Parse(dataString);

            _whiteColor = new ColorPaletteEntry(Colors.White, _stringProvider.GetString("DarkThemeTextContrastTitle"), null, FluentEditorShared.Utils.ColorStringFormat.PoundRGB, null);
            _blackColor = new ColorPaletteEntry(Colors.Black, _stringProvider.GetString("LightThemeTextContrastTitle"), null, FluentEditorShared.Utils.ColorStringFormat.PoundRGB, null);

            var lightRegionNode = rootObject[nameof(LightRegion)].GetObject();
            _lightRegion = ThemeColorPalette.Parse(lightRegionNode, null);

            var darkRegionNode = rootObject[nameof(DarkRegion)].GetObject();
            _darkRegion = ThemeColorPalette.Parse(darkRegionNode, null);

            var lightBaseNode = rootObject[nameof(LightBase)].GetObject();
            _lightBase = ThemeColorPalette.Parse(lightBaseNode, null);

            var darkBaseNode = rootObject[nameof(DarkBase)].GetObject();
            _darkBase = ThemeColorPalette.Parse(darkBaseNode, null);

            var lightPrimaryNode = rootObject[nameof(LightPrimary)].GetObject();
            _lightPrimary = ThemeColorPalette.Parse(lightPrimaryNode, null);

            var darkPrimaryNode = rootObject[nameof(DarkPrimary)].GetObject();
            _darkPrimary = ThemeColorPalette.Parse(darkPrimaryNode, null);

            var lightHyperlinkNode = rootObject[nameof(LightHyperlink)].GetObject();
            _lightHyperlink = ThemeColorPalette.Parse(lightHyperlinkNode, null);

            var darkHyperlinkNode = rootObject[nameof(DarkHyperlink)].GetObject();
            _darkHyperlink = ThemeColorPalette.Parse(darkHyperlinkNode, null);

            _presets = new ObservableList<ThemePreset>();
            if (rootObject.ContainsKey("Presets"))
            {
                var presetsNode = rootObject["Presets"].GetArray();
                foreach (var presetNode in presetsNode)
                {
                    _presets.Add(ThemePreset.Parse(presetNode.GetObject()));
                }
            }
            if (_presets.Count >= 1)
            {
                ApplyPreset(_presets[0]);
            }

            UpdateActivePreset();

            _lightColorMappings = ThemeColorMapping.ParseList(rootObject["LightPaletteMapping"].GetArray(), _lightRegion, _darkRegion, _lightBase, _darkBase, _lightPrimary, _darkPrimary, _lightHyperlink, _darkHyperlink, _whiteColor, _blackColor);
            _lightColorMappings.Sort((a, b) =>
            {
                return a.Target.ToString().CompareTo(b.Target.ToString());
            });

            _darkColorMappings = ThemeColorMapping.ParseList(rootObject["DarkPaletteMapping"].GetArray(), _lightRegion, _darkRegion, _lightBase, _darkBase, _lightPrimary, _darkPrimary, _lightHyperlink, _darkHyperlink, _whiteColor, _blackColor);
            _darkColorMappings.Sort((a, b) =>
            {
                return a.Target.ToString().CompareTo(b.Target.ToString());
            });

            void InitColorPalette(ColorPalette colorPalette,
                                  List<ThemeColorMapping> mappings,
                                  IReadOnlyList<ContrastColorWrapper> contrastColors,
                                  IReadOnlyList<IReadOnlyList<ContrastColorWrapper>> editContrastColors = null
                                  )
            {
                colorPalette.ContrastColors = contrastColors;

                if (editContrastColors != null)
                {
                    var idx = 0;
                    foreach (var entry in colorPalette.Palette)
                    {
                        if (idx < editContrastColors.Count && editContrastColors[idx] != null)
                        {
                            entry.ContrastColors = editContrastColors[idx];
                        }
                        ++idx;
                    }
                }

                colorPalette.BaseColor.ActiveColorChanged += PaletteEntry_ActiveColorChanged;

                foreach (var entry in colorPalette.Palette)
                {
                    entry.ActiveColorChanged += PaletteEntry_ActiveColorChanged;
                }

                foreach (var entry in colorPalette.Palette)
                {
                    if (entry.Description == null)
                    {
                        entry.Description = GenerateMappingDescription(entry, mappings);
                    }
                }
            }

            InitColorPalette(_lightRegion,
                             _lightColorMappings,
                             new List<ContrastColorWrapper>() { new ContrastColorWrapper(_whiteColor, false, false), new ContrastColorWrapper(_blackColor, false, false), new ContrastColorWrapper(_lightBase.BaseColor, true, false), new ContrastColorWrapper(_lightPrimary.BaseColor, true, false), new ContrastColorWrapper(_lightHyperlink.BaseColor, true, false) },
                             new List<IReadOnlyList<ContrastColorWrapper>>()
                             {
                                new List<ContrastColorWrapper>() { new ContrastColorWrapper(_whiteColor, true, false), new ContrastColorWrapper(_blackColor, true, false), },
                                new List<ContrastColorWrapper>() { new ContrastColorWrapper(_whiteColor, false, false), new ContrastColorWrapper(_blackColor, false, false), new ContrastColorWrapper(_lightRegion.BaseColor, true, false, _lightRegion.BaseColor), new ContrastColorWrapper(_lightRegion.Palette[2], true, false, _lightRegion.BaseColor), },
                                new List<ContrastColorWrapper>() { new ContrastColorWrapper(_whiteColor, false, false), new ContrastColorWrapper(_blackColor, false, false), new ContrastColorWrapper(_lightRegion.BaseColor, true, false, _lightRegion.BaseColor), new ContrastColorWrapper(_lightRegion.Palette[1], true, false, _lightRegion.BaseColor), },
                                new List<ContrastColorWrapper>() { new ContrastColorWrapper(_whiteColor, false, false), new ContrastColorWrapper(_blackColor, false, false), new ContrastColorWrapper(_lightRegion.BaseColor, true, false, _lightRegion.BaseColor), new ContrastColorWrapper(_lightRegion.Palette[1], true, false, _lightRegion.BaseColor), new ContrastColorWrapper(_lightRegion.Palette[2], true, false, _lightRegion.BaseColor), },
                                new List<ContrastColorWrapper>() { new ContrastColorWrapper(_whiteColor, false, false), new ContrastColorWrapper(_blackColor, false, false), new ContrastColorWrapper(_lightRegion.BaseColor, true, false, _lightRegion.BaseColor), new ContrastColorWrapper(_lightRegion.Palette[1], true, false, _lightRegion.BaseColor), new ContrastColorWrapper(_lightRegion.Palette[2], true, false, _lightRegion.BaseColor), },
                             });

            InitColorPalette(_darkRegion,
                             _darkColorMappings,
                             new List<ContrastColorWrapper>() { new ContrastColorWrapper(_whiteColor, false, false), new ContrastColorWrapper(_blackColor, false, false), new ContrastColorWrapper(_darkBase.BaseColor, true, false), new ContrastColorWrapper(_darkPrimary.BaseColor, true, false), new ContrastColorWrapper(_darkHyperlink.BaseColor, true, false) },
                             new List<IReadOnlyList<ContrastColorWrapper>>()
                             {
                                new List<ContrastColorWrapper>() { new ContrastColorWrapper(_whiteColor, true, false), new ContrastColorWrapper(_blackColor, true, false), },
                                new List<ContrastColorWrapper>() { new ContrastColorWrapper(_whiteColor, false, false), new ContrastColorWrapper(_blackColor, false, false), new ContrastColorWrapper(_darkRegion.BaseColor, true, false, _darkRegion.BaseColor), new ContrastColorWrapper(_darkRegion.Palette[2], true, false, _darkRegion.BaseColor), },
                                new List<ContrastColorWrapper>() { new ContrastColorWrapper(_whiteColor, false, false), new ContrastColorWrapper(_blackColor, false, false), new ContrastColorWrapper(_darkRegion.BaseColor, true, false, _darkRegion.BaseColor), new ContrastColorWrapper(_darkRegion.Palette[1], true, false, _darkRegion.BaseColor), },
                                new List<ContrastColorWrapper>() { new ContrastColorWrapper(_whiteColor, false, false), new ContrastColorWrapper(_blackColor, false, false), new ContrastColorWrapper(_darkRegion.BaseColor, true, false, _darkRegion.BaseColor), new ContrastColorWrapper(_darkRegion.Palette[1], true, false, _darkRegion.BaseColor), new ContrastColorWrapper(_darkRegion.Palette[2], true, false, _darkRegion.BaseColor), },
                                new List<ContrastColorWrapper>() { new ContrastColorWrapper(_whiteColor, false, false), new ContrastColorWrapper(_blackColor, false, false), new ContrastColorWrapper(_darkRegion.BaseColor, true, false, _darkRegion.BaseColor), new ContrastColorWrapper(_darkRegion.Palette[1], true, false, _darkRegion.BaseColor), new ContrastColorWrapper(_darkRegion.Palette[2], true, false, _darkRegion.BaseColor), },
                             });

            InitColorPalette(_lightBase, 
                             _lightColorMappings, 
                             new List<ContrastColorWrapper>() { new ContrastColorWrapper(_whiteColor, false, false), new ContrastColorWrapper(_blackColor, false, false), new ContrastColorWrapper(_lightRegion.BaseColor, true, false), },
                             new List<IReadOnlyList<ContrastColorWrapper>>()
                             {
                                new List<ContrastColorWrapper>() { new ContrastColorWrapper(_whiteColor, false, false), new ContrastColorWrapper(_blackColor, false, false), new ContrastColorWrapper(_lightPrimary.Palette[0], true, true, _lightRegion.BaseColor), new ContrastColorWrapper(_lightPrimary.Palette[3], true, true, _lightRegion.BaseColor), new ContrastColorWrapper(_lightPrimary.Palette[6], true, true, _lightRegion.BaseColor),},
                                new List<ContrastColorWrapper>() { new ContrastColorWrapper(_whiteColor, false, false), new ContrastColorWrapper(_blackColor, false, false), new ContrastColorWrapper(_lightRegion.Palette[0], true, true, _lightRegion.BaseColor), new ContrastColorWrapper(_lightRegion.Palette[1], true, true, _lightRegion.BaseColor), new ContrastColorWrapper(_lightRegion.Palette[2], true, true, _lightRegion.BaseColor), new ContrastColorWrapper(_lightRegion.Palette[3], true, true, _lightRegion.BaseColor), new ContrastColorWrapper(_lightRegion.Palette[4], true, true, _lightRegion.BaseColor),},
                                new List<ContrastColorWrapper>() { new ContrastColorWrapper(_whiteColor, false, false), new ContrastColorWrapper(_blackColor, false, false), new ContrastColorWrapper(_lightRegion.Palette[0], true, true, _lightRegion.BaseColor), new ContrastColorWrapper(_lightRegion.Palette[1], true, true, _lightRegion.BaseColor), new ContrastColorWrapper(_lightRegion.Palette[2], true, true, _lightRegion.BaseColor), new ContrastColorWrapper(_lightRegion.Palette[3], true, true, _lightRegion.BaseColor), new ContrastColorWrapper(_lightRegion.Palette[4], true, true, _lightRegion.BaseColor), },
                                new List<ContrastColorWrapper>() { new ContrastColorWrapper(_whiteColor, false, false), new ContrastColorWrapper(_blackColor, false, false), new ContrastColorWrapper(_lightRegion.Palette[0], true, true, _lightRegion.BaseColor), new ContrastColorWrapper(_lightRegion.Palette[1], true, true, _lightRegion.BaseColor), new ContrastColorWrapper(_lightRegion.Palette[2], true, true, _lightRegion.BaseColor), new ContrastColorWrapper(_lightRegion.Palette[3], true, true, _lightRegion.BaseColor), new ContrastColorWrapper(_lightRegion.Palette[4], true, true, _lightRegion.BaseColor), },
                                new List<ContrastColorWrapper>() { new ContrastColorWrapper(_whiteColor, false, false), new ContrastColorWrapper(_blackColor, false, false), new ContrastColorWrapper(_lightRegion.Palette[0], true, true, _lightRegion.BaseColor), new ContrastColorWrapper(_lightRegion.Palette[1], true, true, _lightRegion.BaseColor), new ContrastColorWrapper(_lightRegion.Palette[2], true, true, _lightRegion.BaseColor), new ContrastColorWrapper(_lightRegion.Palette[3], true, true, _lightRegion.BaseColor), new ContrastColorWrapper(_lightRegion.Palette[4], true, true, _lightRegion.BaseColor), },
                                new List<ContrastColorWrapper>() { new ContrastColorWrapper(_whiteColor, false, false), new ContrastColorWrapper(_blackColor, false, false), new ContrastColorWrapper(_lightRegion.Palette[0], true, true, _lightRegion.BaseColor), new ContrastColorWrapper(_lightRegion.Palette[1], true, true, _lightRegion.BaseColor), new ContrastColorWrapper(_lightRegion.Palette[2], true, true, _lightRegion.BaseColor), new ContrastColorWrapper(_lightRegion.Palette[3], true, true, _lightRegion.BaseColor), new ContrastColorWrapper(_lightRegion.Palette[4], true, true, _lightRegion.BaseColor), },
                                new List<ContrastColorWrapper>() { new ContrastColorWrapper(_whiteColor, false, false), new ContrastColorWrapper(_blackColor, false, false), new ContrastColorWrapper(_lightRegion.Palette[0], true, false, _lightRegion.BaseColor), new ContrastColorWrapper(_lightRegion.Palette[1], true, false, _lightRegion.BaseColor), new ContrastColorWrapper(_lightRegion.Palette[2], true, false, _lightRegion.BaseColor), new ContrastColorWrapper(_lightRegion.Palette[3], true, false, _lightRegion.BaseColor), new ContrastColorWrapper(_lightRegion.Palette[4], true, false, _lightRegion.BaseColor), },
                             });

            InitColorPalette(_darkBase, 
                             _darkColorMappings, 
                             new List<ContrastColorWrapper>() { new ContrastColorWrapper(_whiteColor, false, false), new ContrastColorWrapper(_blackColor, false, false), new ContrastColorWrapper(_darkRegion.BaseColor, true, false), },
                             new List<IReadOnlyList<ContrastColorWrapper>>()
                             {
                                new List<ContrastColorWrapper>() { new ContrastColorWrapper(_whiteColor, false, false), new ContrastColorWrapper(_blackColor, false, false), new ContrastColorWrapper(_darkPrimary.Palette[0], true, true, _darkRegion.BaseColor), new ContrastColorWrapper(_darkPrimary.Palette[3], true, true, _darkRegion.BaseColor), new ContrastColorWrapper(_darkPrimary.Palette[6], true, true, _darkRegion.BaseColor),},
                                new List<ContrastColorWrapper>() { new ContrastColorWrapper(_whiteColor, false, false), new ContrastColorWrapper(_blackColor, false, false), new ContrastColorWrapper(_darkRegion.Palette[0], true, true, _darkRegion.BaseColor), new ContrastColorWrapper(_darkRegion.Palette[1], true, true, _darkRegion.BaseColor), new ContrastColorWrapper(_darkRegion.Palette[2], true, true, _darkRegion.BaseColor), new ContrastColorWrapper(_darkRegion.Palette[3], true, true, _darkRegion.BaseColor), new ContrastColorWrapper(_darkRegion.Palette[4], true, true, _darkRegion.BaseColor),},
                                new List<ContrastColorWrapper>() { new ContrastColorWrapper(_whiteColor, false, false), new ContrastColorWrapper(_blackColor, false, false), new ContrastColorWrapper(_darkRegion.Palette[0], true, true, _darkRegion.BaseColor), new ContrastColorWrapper(_darkRegion.Palette[1], true, true, _darkRegion.BaseColor), new ContrastColorWrapper(_darkRegion.Palette[2], true, true, _darkRegion.BaseColor), new ContrastColorWrapper(_darkRegion.Palette[3], true, true, _darkRegion.BaseColor), new ContrastColorWrapper(_darkRegion.Palette[4], true, true, _darkRegion.BaseColor), },
                                new List<ContrastColorWrapper>() { new ContrastColorWrapper(_whiteColor, false, false), new ContrastColorWrapper(_blackColor, false, false), new ContrastColorWrapper(_darkRegion.Palette[0], true, true, _darkRegion.BaseColor), new ContrastColorWrapper(_darkRegion.Palette[1], true, true, _darkRegion.BaseColor), new ContrastColorWrapper(_darkRegion.Palette[2], true, true, _darkRegion.BaseColor), new ContrastColorWrapper(_darkRegion.Palette[3], true, true, _darkRegion.BaseColor), new ContrastColorWrapper(_darkRegion.Palette[4], true, true, _darkRegion.BaseColor), },
                                new List<ContrastColorWrapper>() { new ContrastColorWrapper(_whiteColor, false, false), new ContrastColorWrapper(_blackColor, false, false), new ContrastColorWrapper(_darkRegion.Palette[0], true, true, _darkRegion.BaseColor), new ContrastColorWrapper(_darkRegion.Palette[1], true, true, _darkRegion.BaseColor), new ContrastColorWrapper(_darkRegion.Palette[2], true, true, _darkRegion.BaseColor), new ContrastColorWrapper(_darkRegion.Palette[3], true, true, _darkRegion.BaseColor), new ContrastColorWrapper(_darkRegion.Palette[4], true, true, _darkRegion.BaseColor), },
                                new List<ContrastColorWrapper>() { new ContrastColorWrapper(_whiteColor, false, false), new ContrastColorWrapper(_blackColor, false, false), new ContrastColorWrapper(_darkRegion.Palette[0], true, true, _darkRegion.BaseColor), new ContrastColorWrapper(_darkRegion.Palette[1], true, true, _darkRegion.BaseColor), new ContrastColorWrapper(_darkRegion.Palette[2], true, true, _darkRegion.BaseColor), new ContrastColorWrapper(_darkRegion.Palette[3], true, true, _darkRegion.BaseColor), new ContrastColorWrapper(_darkRegion.Palette[4], true, true, _darkRegion.BaseColor), },
                                new List<ContrastColorWrapper>() { new ContrastColorWrapper(_whiteColor, false, false), new ContrastColorWrapper(_blackColor, false, false), new ContrastColorWrapper(_darkRegion.Palette[0], true, false, _darkRegion.BaseColor), new ContrastColorWrapper(_darkRegion.Palette[1], true, false, _darkRegion.BaseColor), new ContrastColorWrapper(_darkRegion.Palette[2], true, false, _darkRegion.BaseColor), new ContrastColorWrapper(_darkRegion.Palette[3], true, false, _darkRegion.BaseColor), new ContrastColorWrapper(_darkRegion.Palette[4], true, false, _darkRegion.BaseColor), },
                             });

            InitColorPalette(_lightPrimary, 
                             _lightColorMappings, 
                             new List<ContrastColorWrapper>() { new ContrastColorWrapper(_whiteColor, false, false), new ContrastColorWrapper(_blackColor, false, false), new ContrastColorWrapper(_lightRegion.BaseColor, true, false), },
                             new List<IReadOnlyList<ContrastColorWrapper>>()
                             {
                                new List<ContrastColorWrapper>() { new ContrastColorWrapper(_whiteColor, false, false), new ContrastColorWrapper(_blackColor, false, false), new ContrastColorWrapper(_lightRegion.Palette[0], true, true, _lightRegion.BaseColor), new ContrastColorWrapper(_lightRegion.Palette[1], true, true, _lightRegion.BaseColor), new ContrastColorWrapper(_lightRegion.Palette[2], true, true, _lightRegion.BaseColor), new ContrastColorWrapper(_lightRegion.Palette[3], true, true, _lightRegion.BaseColor), new ContrastColorWrapper(_lightRegion.Palette[4], true, true, _lightRegion.BaseColor), },
                                new List<ContrastColorWrapper>() { new ContrastColorWrapper(_whiteColor, false, false), new ContrastColorWrapper(_blackColor, false, false), new ContrastColorWrapper(_lightRegion.Palette[0], true, true, _lightRegion.BaseColor), new ContrastColorWrapper(_lightRegion.Palette[1], true, true, _lightRegion.BaseColor), new ContrastColorWrapper(_lightRegion.Palette[2], true, true, _lightRegion.BaseColor), new ContrastColorWrapper(_lightRegion.Palette[3], true, true, _lightRegion.BaseColor), new ContrastColorWrapper(_lightRegion.Palette[4], true, true, _lightRegion.BaseColor), },
                                new List<ContrastColorWrapper>() { new ContrastColorWrapper(_whiteColor, false, false), new ContrastColorWrapper(_blackColor, false, false), new ContrastColorWrapper(_lightRegion.Palette[0], true, true, _lightRegion.BaseColor), new ContrastColorWrapper(_lightRegion.Palette[1], true, true, _lightRegion.BaseColor), new ContrastColorWrapper(_lightRegion.Palette[2], true, true, _lightRegion.BaseColor), new ContrastColorWrapper(_lightRegion.Palette[3], true, true, _lightRegion.BaseColor), new ContrastColorWrapper(_lightRegion.Palette[4], true, true, _lightRegion.BaseColor), },
                                new List<ContrastColorWrapper>() { new ContrastColorWrapper(_whiteColor, false, false), new ContrastColorWrapper(_blackColor, false, false), new ContrastColorWrapper(_lightRegion.Palette[0], true, true, _lightRegion.BaseColor), new ContrastColorWrapper(_lightRegion.Palette[1], true, true, _lightRegion.BaseColor), new ContrastColorWrapper(_lightRegion.Palette[2], true, true, _lightRegion.BaseColor), new ContrastColorWrapper(_lightRegion.Palette[3], true, true, _lightRegion.BaseColor), new ContrastColorWrapper(_lightRegion.Palette[4], true, true, _lightRegion.BaseColor), },
                                new List<ContrastColorWrapper>() { new ContrastColorWrapper(_whiteColor, false, false), new ContrastColorWrapper(_blackColor, false, false), new ContrastColorWrapper(_lightRegion.Palette[0], true, true, _lightRegion.BaseColor), new ContrastColorWrapper(_lightRegion.Palette[1], true, true, _lightRegion.BaseColor), new ContrastColorWrapper(_lightRegion.Palette[2], true, true, _lightRegion.BaseColor), new ContrastColorWrapper(_lightRegion.Palette[3], true, true, _lightRegion.BaseColor), new ContrastColorWrapper(_lightRegion.Palette[4], true, true, _lightRegion.BaseColor), },
                                new List<ContrastColorWrapper>() { new ContrastColorWrapper(_whiteColor, false, false), new ContrastColorWrapper(_blackColor, false, false), new ContrastColorWrapper(_lightRegion.Palette[0], true, true, _lightRegion.BaseColor), new ContrastColorWrapper(_lightRegion.Palette[1], true, true, _lightRegion.BaseColor), new ContrastColorWrapper(_lightRegion.Palette[2], true, true, _lightRegion.BaseColor), new ContrastColorWrapper(_lightRegion.Palette[3], true, true, _lightRegion.BaseColor), new ContrastColorWrapper(_lightRegion.Palette[4], true, true, _lightRegion.BaseColor), },
                                new List<ContrastColorWrapper>() { new ContrastColorWrapper(_whiteColor, false, false), new ContrastColorWrapper(_blackColor, false, false), new ContrastColorWrapper(_lightRegion.Palette[0], true, true, _lightRegion.BaseColor), new ContrastColorWrapper(_lightRegion.Palette[1], true, true, _lightRegion.BaseColor), new ContrastColorWrapper(_lightRegion.Palette[2], true, true, _lightRegion.BaseColor), new ContrastColorWrapper(_lightRegion.Palette[3], true, true, _lightRegion.BaseColor), new ContrastColorWrapper(_lightRegion.Palette[4], true, true, _lightRegion.BaseColor), },
                             });

            InitColorPalette(_darkPrimary, 
                             _darkColorMappings, 
                             new List<ContrastColorWrapper>() { new ContrastColorWrapper(_whiteColor, false, false), new ContrastColorWrapper(_blackColor, false, false), new ContrastColorWrapper(_darkRegion.BaseColor, true, false), },
                             new List<IReadOnlyList<ContrastColorWrapper>>()
                             {
                                new List<ContrastColorWrapper>() { new ContrastColorWrapper(_whiteColor, false, false), new ContrastColorWrapper(_blackColor, false, false), new ContrastColorWrapper(_darkRegion.Palette[0], true, true, _darkRegion.BaseColor), new ContrastColorWrapper(_darkRegion.Palette[1], true, true, _darkRegion.BaseColor), new ContrastColorWrapper(_darkRegion.Palette[2], true, true, _darkRegion.BaseColor), new ContrastColorWrapper(_darkRegion.Palette[3], true, true, _darkRegion.BaseColor), new ContrastColorWrapper(_darkRegion.Palette[4], true, true, _darkRegion.BaseColor), },
                                new List<ContrastColorWrapper>() { new ContrastColorWrapper(_whiteColor, false, false), new ContrastColorWrapper(_blackColor, false, false), new ContrastColorWrapper(_darkRegion.Palette[0], true, true, _darkRegion.BaseColor), new ContrastColorWrapper(_darkRegion.Palette[1], true, true, _darkRegion.BaseColor), new ContrastColorWrapper(_darkRegion.Palette[2], true, true, _darkRegion.BaseColor), new ContrastColorWrapper(_darkRegion.Palette[3], true, true, _darkRegion.BaseColor), new ContrastColorWrapper(_darkRegion.Palette[4], true, true, _darkRegion.BaseColor), },
                                new List<ContrastColorWrapper>() { new ContrastColorWrapper(_whiteColor, false, false), new ContrastColorWrapper(_blackColor, false, false), new ContrastColorWrapper(_darkRegion.Palette[0], true, true, _darkRegion.BaseColor), new ContrastColorWrapper(_darkRegion.Palette[1], true, true, _darkRegion.BaseColor), new ContrastColorWrapper(_darkRegion.Palette[2], true, true, _darkRegion.BaseColor), new ContrastColorWrapper(_darkRegion.Palette[3], true, true, _darkRegion.BaseColor), new ContrastColorWrapper(_darkRegion.Palette[4], true, true, _darkRegion.BaseColor), },
                                new List<ContrastColorWrapper>() { new ContrastColorWrapper(_whiteColor, false, false), new ContrastColorWrapper(_blackColor, false, false), new ContrastColorWrapper(_darkRegion.Palette[0], true, true, _darkRegion.BaseColor), new ContrastColorWrapper(_darkRegion.Palette[1], true, true, _darkRegion.BaseColor), new ContrastColorWrapper(_darkRegion.Palette[2], true, true, _darkRegion.BaseColor), new ContrastColorWrapper(_darkRegion.Palette[3], true, true, _darkRegion.BaseColor), new ContrastColorWrapper(_darkRegion.Palette[4], true, true, _darkRegion.BaseColor), },
                                new List<ContrastColorWrapper>() { new ContrastColorWrapper(_whiteColor, false, false), new ContrastColorWrapper(_blackColor, false, false), new ContrastColorWrapper(_darkRegion.Palette[0], true, true, _darkRegion.BaseColor), new ContrastColorWrapper(_darkRegion.Palette[1], true, true, _darkRegion.BaseColor), new ContrastColorWrapper(_darkRegion.Palette[2], true, true, _darkRegion.BaseColor), new ContrastColorWrapper(_darkRegion.Palette[3], true, true, _darkRegion.BaseColor), new ContrastColorWrapper(_darkRegion.Palette[4], true, true, _darkRegion.BaseColor), },
                                new List<ContrastColorWrapper>() { new ContrastColorWrapper(_whiteColor, false, false), new ContrastColorWrapper(_blackColor, false, false), new ContrastColorWrapper(_darkRegion.Palette[0], true, true, _darkRegion.BaseColor), new ContrastColorWrapper(_darkRegion.Palette[1], true, true, _darkRegion.BaseColor), new ContrastColorWrapper(_darkRegion.Palette[2], true, true, _darkRegion.BaseColor), new ContrastColorWrapper(_darkRegion.Palette[3], true, true, _darkRegion.BaseColor), new ContrastColorWrapper(_darkRegion.Palette[4], true, true, _darkRegion.BaseColor), },
                                new List<ContrastColorWrapper>() { new ContrastColorWrapper(_whiteColor, false, false), new ContrastColorWrapper(_blackColor, false, false), new ContrastColorWrapper(_darkRegion.Palette[0], true, true, _darkRegion.BaseColor), new ContrastColorWrapper(_darkRegion.Palette[1], true, true, _darkRegion.BaseColor), new ContrastColorWrapper(_darkRegion.Palette[2], true, true, _darkRegion.BaseColor), new ContrastColorWrapper(_darkRegion.Palette[3], true, true, _darkRegion.BaseColor), new ContrastColorWrapper(_darkRegion.Palette[4], true, true, _darkRegion.BaseColor), },
                             });

            InitColorPalette(_lightHyperlink, 
                             _lightColorMappings, 
                             new List<ContrastColorWrapper>() { new ContrastColorWrapper(_whiteColor, false, false), new ContrastColorWrapper(_blackColor, false, false), new ContrastColorWrapper(_lightRegion.BaseColor, true, false), },
                             new List<IReadOnlyList<ContrastColorWrapper>>()
                             {
                                new List<ContrastColorWrapper>() { new ContrastColorWrapper(_whiteColor, false, false), new ContrastColorWrapper(_blackColor, false, false), new ContrastColorWrapper(_lightRegion.Palette[0], true, true, _lightRegion.BaseColor), new ContrastColorWrapper(_lightRegion.Palette[1], true, true, _lightRegion.BaseColor), new ContrastColorWrapper(_lightRegion.Palette[2], true, true, _lightRegion.BaseColor), new ContrastColorWrapper(_lightRegion.Palette[3], true, true, _lightRegion.BaseColor), new ContrastColorWrapper(_lightRegion.Palette[4], true, true, _lightRegion.BaseColor), },
                                new List<ContrastColorWrapper>() { new ContrastColorWrapper(_whiteColor, false, false), new ContrastColorWrapper(_blackColor, false, false), new ContrastColorWrapper(_lightRegion.Palette[0], true, true, _lightRegion.BaseColor), new ContrastColorWrapper(_lightRegion.Palette[1], true, true, _lightRegion.BaseColor), new ContrastColorWrapper(_lightRegion.Palette[2], true, true, _lightRegion.BaseColor), new ContrastColorWrapper(_lightRegion.Palette[3], true, true, _lightRegion.BaseColor), new ContrastColorWrapper(_lightRegion.Palette[4], true, true, _lightRegion.BaseColor), },
                                new List<ContrastColorWrapper>() { new ContrastColorWrapper(_whiteColor, false, false), new ContrastColorWrapper(_blackColor, false, false), new ContrastColorWrapper(_lightRegion.Palette[0], true, true, _lightRegion.BaseColor), new ContrastColorWrapper(_lightRegion.Palette[1], true, true, _lightRegion.BaseColor), new ContrastColorWrapper(_lightRegion.Palette[2], true, true, _lightRegion.BaseColor), new ContrastColorWrapper(_lightRegion.Palette[3], true, true, _lightRegion.BaseColor), new ContrastColorWrapper(_lightRegion.Palette[4], true, true, _lightRegion.BaseColor), },
                             });

            InitColorPalette(_darkHyperlink, 
                             _darkColorMappings, 
                             new List<ContrastColorWrapper>() { new ContrastColorWrapper(_whiteColor, false, false), new ContrastColorWrapper(_blackColor, false, false), new ContrastColorWrapper(_darkRegion.BaseColor, true, false), },
                             new List<IReadOnlyList<ContrastColorWrapper>>()
                             {
                                new List<ContrastColorWrapper>() { new ContrastColorWrapper(_whiteColor, false, false), new ContrastColorWrapper(_blackColor, false, false), new ContrastColorWrapper(_darkRegion.Palette[0], true, true, _darkRegion.BaseColor), new ContrastColorWrapper(_darkRegion.Palette[1], true, true, _darkRegion.BaseColor), new ContrastColorWrapper(_darkRegion.Palette[2], true, true, _darkRegion.BaseColor), new ContrastColorWrapper(_darkRegion.Palette[3], true, true, _darkRegion.BaseColor), new ContrastColorWrapper(_darkRegion.Palette[4], true, true, _darkRegion.BaseColor), },
                                new List<ContrastColorWrapper>() { new ContrastColorWrapper(_whiteColor, false, false), new ContrastColorWrapper(_blackColor, false, false), new ContrastColorWrapper(_darkRegion.Palette[0], true, true, _darkRegion.BaseColor), new ContrastColorWrapper(_darkRegion.Palette[1], true, true, _darkRegion.BaseColor), new ContrastColorWrapper(_darkRegion.Palette[2], true, true, _darkRegion.BaseColor), new ContrastColorWrapper(_darkRegion.Palette[3], true, true, _darkRegion.BaseColor), new ContrastColorWrapper(_darkRegion.Palette[4], true, true, _darkRegion.BaseColor), },
                                new List<ContrastColorWrapper>() { new ContrastColorWrapper(_whiteColor, false, false), new ContrastColorWrapper(_blackColor, false, false), new ContrastColorWrapper(_darkRegion.Palette[0], true, true, _darkRegion.BaseColor), new ContrastColorWrapper(_darkRegion.Palette[1], true, true, _darkRegion.BaseColor), new ContrastColorWrapper(_darkRegion.Palette[2], true, true, _darkRegion.BaseColor), new ContrastColorWrapper(_darkRegion.Palette[3], true, true, _darkRegion.BaseColor), new ContrastColorWrapper(_darkRegion.Palette[4], true, true, _darkRegion.BaseColor), },
                             });
        }

        private string GenerateMappingDescription(IColorPaletteEntry paletteEntry, List<ThemeColorMapping> mappings)
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

        private void PaletteEntry_ActiveColorChanged(IColorPaletteEntry obj)
        {
            UpdateActivePreset();
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

        public void AddOrReplacePreset(ThemePreset preset)
        {
            if (!string.IsNullOrEmpty(preset.Name))
            {
                var oldPreset = _presets.FirstOrDefault<ThemePreset>((a) => a.Id == preset.Id);
                if (oldPreset != null)
                {
                    _presets.Remove(oldPreset);
                }
            }

            _presets.Add(preset);

            UpdateActivePreset();
        }

        public void ApplyPreset(ThemePreset preset)
        {
            if (preset == null)
            {
                ActivePreset = null;
                return;
            }

            _lightRegion.BaseColor.ActiveColor = preset.LightRegionColor;
            _darkRegion.BaseColor.ActiveColor = preset.DarkRegionColor;
            _lightBase.BaseColor.ActiveColor = preset.LightBaseColor;
            _darkBase.BaseColor.ActiveColor = preset.DarkBaseColor;
            _lightPrimary.BaseColor.ActiveColor = preset.LightPrimaryColor;
            _darkPrimary.BaseColor.ActiveColor = preset.DarkPrimaryColor;
            _lightHyperlink.BaseColor.ActiveColor = preset.LightHyperlinkColor;
            _darkHyperlink.BaseColor.ActiveColor = preset.DarkHyperlinkColor;

            ApplyPresetOverrides(_lightRegion.Palette, preset.LightRegionOverrides, preset.LightRegionAcrylicOverrides);
            ApplyPresetOverrides(_darkRegion.Palette, preset.DarkRegionOverrides, preset.DarkRegionAcrylicOverrides);
            ApplyPresetOverrides(_lightBase.Palette, preset.LightBaseOverrides);
            ApplyPresetOverrides(_darkBase.Palette, preset.DarkBaseOverrides);
            ApplyPresetOverrides(_lightPrimary.Palette, preset.LightPrimaryOverrides);
            ApplyPresetOverrides(_darkPrimary.Palette, preset.DarkPrimaryOverrides);
            ApplyPresetOverrides(_lightHyperlink.Palette, preset.LightHyperlinkOverrides);
            ApplyPresetOverrides(_darkHyperlink.Palette, preset.DarkHyperlinkOverrides);
        }

        private void ApplyPresetOverrides(IReadOnlyList<EditableColorPaletteEntry> palette, Dictionary<int, Color> overrides, Dictionary<int, double> acrylicOverrides = null)
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

                if (palette[i] is ThemeEditableAcrylicPaletteEntry acrylicPalette)
                {
                    if (acrylicOverrides != null && acrylicOverrides.ContainsKey(i))
                    {
                        acrylicPalette.Opacity = acrylicOverrides[i];
                    }
                    else
                    {
                        acrylicPalette.ResetOpacity();
                    }
                }
            }
        }

        private ObservableList<ThemePreset> _presets;
        public ObservableList<ThemePreset> Presets
        {
            get { return _presets; }
        }

        private ThemePreset _activePreset;
        public ThemePreset ActivePreset
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

        public event Action<IThemePaletteModel> ActivePresetChanged;

        private List<ThemeColorMapping> _lightColorMappings;
        public IReadOnlyList<ThemeColorMapping> LightColorMapping
        {
            get { return _lightColorMappings; }
        }

        private List<ThemeColorMapping> _darkColorMappings;
        public IReadOnlyList<ThemeColorMapping> DarkColorMapping
        {
            get { return _darkColorMappings; }
        }

        private ColorPaletteEntry _whiteColor;
        private ColorPaletteEntry _blackColor;

        private ColorPalette _lightRegion;
        public ColorPalette LightRegion
        {
            get { return _lightRegion; }
        }

        private ColorPalette _darkRegion;
        public ColorPalette DarkRegion
        {
            get { return _darkRegion; }
        }

        private ColorPalette _lightBase;
        public ColorPalette LightBase
        {
            get { return _lightBase; }
        }

        private ColorPalette _darkBase;
        public ColorPalette DarkBase
        {
            get { return _darkBase; }
        }

        private ColorPalette _lightPrimary;
        public ColorPalette LightPrimary
        {
            get { return _lightPrimary; }
        }

        private ColorPalette _darkPrimary;
        public ColorPalette DarkPrimary
        {
            get { return _darkPrimary; }
        }

        private ColorPalette _lightHyperlink;
        public ColorPalette LightHyperlink
        {
            get { return _lightHyperlink; }
        }

        private ColorPalette _darkHyperlink;
        public ColorPalette DarkHyperlink
        {
            get { return _darkHyperlink; }
        }
    }
}
