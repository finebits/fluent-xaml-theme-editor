// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using FluentEditor.ThemePalette.Model;
using FluentEditorShared.ColorPalette;
using FluentEditorShared.Utils;
using System;
using System.Collections.Generic;
using Windows.Data.Json;
using Windows.UI.Xaml;

namespace FluentEditor.ThemePalette
{
    public enum ThemeColorTarget
    {
        Accent,
        ErrorText,
        AltHigh,
        AltLow,
        AltMedium,
        AltMediumHigh,
        AltMediumLow,
        BaseHigh,
        BaseLow,
        BaseMedium,
        BaseMediumHigh,
        BaseMediumLow,
        ChromeAltLow,
        ChromeBlackHigh,
        ChromeBlackLow,
        ChromeBlackMedium,
        ChromeBlackMediumLow,
        ChromeDisabledHigh,
        ChromeDisabledLow,
        ChromeGray,
        ChromeHigh,
        ChromeLow,
        ChromeMedium,
        ChromeMediumLow,
        ChromeWhite,
        ListLow,
        ListMedium,

        // Extra
        TextActive,
        LightAccent1,
        DarkAccent1,
        LightAccent2,
        DarkAccent2,
        LightAccent3,
        DarkAccent3,

        Hyperlink,
        HyperlinkActive,
        HyperlinkDisabled,
    }
    public enum ThemeColorSource
    {
        LightRegion,
        DarkRegion,
        LightBase,
        DarkBase,
        LightPrimary,
        DarkPrimary,
        White,
        Black,

        // Extra
        LightHyperlink,
        DarkHyperlink,
    }

    public class ThemeColorMapping
    {
        public static ThemeColorMapping Parse(JsonObject data, IColorPaletteEntry lightRegion, IColorPaletteEntry darkRegion, ColorPalette lightBase, ColorPalette darkBase, ColorPalette lightPrimary, ColorPalette darkPrimary, ColorPalette lightHyperlink, ColorPalette darkHyperlink, IColorPaletteEntry white, IColorPaletteEntry black)
        {
            var target = data["Target"].GetEnum<ThemeColorTarget>();
            var source = data["Source"].GetEnum<ThemeColorSource>();
            int index = 0;
            if (data.ContainsKey("SourceIndex"))
            {
                index = data["SourceIndex"].GetInt();
            }

            switch (source)
            {
                case ThemeColorSource.LightRegion:
                    return new ThemeColorMapping(lightRegion, target);
                case ThemeColorSource.DarkRegion:
                    return new ThemeColorMapping(darkRegion, target);
                case ThemeColorSource.LightBase:
                    return new ThemeColorMapping(lightBase.Palette[index], target);
                case ThemeColorSource.DarkBase:
                    return new ThemeColorMapping(darkBase.Palette[index], target);
                case ThemeColorSource.LightPrimary:
                    return new ThemeColorMapping(lightPrimary.Palette[index], target);
                case ThemeColorSource.DarkPrimary:
                    return new ThemeColorMapping(darkPrimary.Palette[index], target);
                case ThemeColorSource.LightHyperlink:
                    return new ThemeColorMapping(lightHyperlink.Palette[index], target);
                case ThemeColorSource.DarkHyperlink:
                    return new ThemeColorMapping(darkHyperlink.Palette[index], target);
                case ThemeColorSource.White:
                    return new ThemeColorMapping(white, target);
                case ThemeColorSource.Black:
                    return new ThemeColorMapping(black, target);
            }

            return null;
        }

        public static List<ThemeColorMapping> ParseList(JsonArray data,
                                                        IColorPaletteEntry lightRegion,
                                                        IColorPaletteEntry darkRegion,
                                                        ColorPalette lightBase,
                                                        ColorPalette darkBase,
                                                        ColorPalette lightPrimary,
                                                        ColorPalette darkPrimary,
                                                        ColorPalette lightHyperlink,
                                                        ColorPalette darkHyperlink,
                                                        IColorPaletteEntry white,
                                                        IColorPaletteEntry black)
        {
            List<ThemeColorMapping> retVal = new List<ThemeColorMapping>();
            foreach (var node in data)
            {
                retVal.Add(ThemeColorMapping.Parse(node.GetObject(), lightRegion, darkRegion, lightBase, darkBase, lightPrimary, darkPrimary, lightHyperlink, darkHyperlink, white, black));
            }
            return retVal;
        }

        public ThemeColorMapping(IColorPaletteEntry source, ThemeColorTarget targetColor)
        {
            _source = source;
            _targetColor = targetColor;
        }

        private readonly IColorPaletteEntry _source;
        public IColorPaletteEntry Source
        {
            get { return _source; }
        }

        private readonly ThemeColorTarget _targetColor;
        public ThemeColorTarget Target
        {
            get { return _targetColor; }
        }

        public ThemeColorMappingInstance CreateInstance(ColorPaletteResources targetResources, ThemeExtraPalette extraPalette)
        {
            return new ThemeColorMappingInstance(_source, _targetColor, targetResources, extraPalette);
        }
    }

    public class ThemeColorMappingInstance : IDisposable
    {
        public ThemeColorMappingInstance(IColorPaletteEntry source, ThemeColorTarget targetColor, ColorPaletteResources targetResources, ThemeExtraPalette extraPalette)
        {
            _source = source;
            _targetColor = targetColor;
            _targetResources = targetResources;
            _extraPalette = extraPalette;

            Apply();

            _source.ActiveColorChanged += Source_ActiveColorChanged;
        }

        private readonly IColorPaletteEntry _source;
        private readonly ThemeColorTarget _targetColor;
        private readonly ColorPaletteResources _targetResources;
        private readonly ThemeExtraPalette _extraPalette;

        private static object _linkMapLock = new object();
        private static Dictionary<FrameworkElement, bool> _updateInProgress = new Dictionary<FrameworkElement, bool>();

        private FrameworkElement _linkedElement;
        public FrameworkElement LinkedElement
        {
            set
            {
                lock (_linkMapLock)
                {
                    if (_linkedElement != null)
                    {
                        if (_updateInProgress.ContainsKey(_linkedElement))
                        {
                            _updateInProgress.Remove(_linkedElement);
                        }
                        _linkedElement.Unloaded -= _linkedElement_Unloaded;
                    }

                    if (_linkedElement != value)
                    {
                        _linkedElement = value;
                        _linkedElement.Unloaded += _linkedElement_Unloaded;
                    }
                }
            }
        }

        private void _linkedElement_Unloaded(object sender, RoutedEventArgs e)
        {
            lock (_linkMapLock)
            {
                if (_linkedElement != null)
                {
                    if (_updateInProgress.ContainsKey(_linkedElement))
                    {
                        _updateInProgress.Remove(_linkedElement);
                    }
                    _linkedElement.Unloaded -= _linkedElement_Unloaded;
                    _linkedElement = null;
                }
            }
        }

        private void ForceThemeUpdateInLinkedElement()
        {
            FrameworkElement element = null;
            lock (_linkMapLock)
            {
                if (_linkedElement == null)
                {
                    return;
                }
                element = _linkedElement;
                if (_updateInProgress.ContainsKey(element))
                {
                    if (_updateInProgress[element])
                    {
                        return;
                    }
                    else
                    {
                        _updateInProgress[element] = true;
                    }
                }
                else
                {
                    _updateInProgress.Add(element, true);
                }
            }

            _ = element.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if (element.RequestedTheme == ElementTheme.Light)
                {
                    element.RequestedTheme = ElementTheme.Dark;
                    element.RequestedTheme = ElementTheme.Light;
                }
                else if (element.RequestedTheme == ElementTheme.Dark)
                {
                    element.RequestedTheme = ElementTheme.Light;
                    element.RequestedTheme = ElementTheme.Dark;
                }
                else
                {
                    element.RequestedTheme = ElementTheme.Light;
                    element.RequestedTheme = ElementTheme.Dark;
                    element.RequestedTheme = ElementTheme.Default;
                }

                lock (_linkMapLock)
                {
                    if (_updateInProgress.ContainsKey(element))
                    {
                        _updateInProgress[element] = false;
                    }
                }
            });
        }

        private void Source_ActiveColorChanged(IColorPaletteEntry obj)
        {
            Apply();
            ForceThemeUpdateInLinkedElement();
        }

        public void Dispose()
        {
            _source.ActiveColorChanged -= Source_ActiveColorChanged;
        }

        public void Apply()
        {
            if (_targetResources == null)
            {
                return;
            }
            switch (_targetColor)
            {
                case ThemeColorTarget.Accent:
                    _targetResources.Accent = _source.ActiveColor;
                    break;
                case ThemeColorTarget.ErrorText:
                    _targetResources.ErrorText = _source.ActiveColor;
                    break;
                case ThemeColorTarget.AltHigh:
                    _targetResources.AltHigh = _source.ActiveColor;
                    break;
                case ThemeColorTarget.AltLow:
                    _targetResources.AltLow = _source.ActiveColor;
                    break;
                case ThemeColorTarget.AltMedium:
                    _targetResources.AltMedium = _source.ActiveColor;
                    break;
                case ThemeColorTarget.AltMediumHigh:
                    _targetResources.AltMediumHigh = _source.ActiveColor;
                    break;
                case ThemeColorTarget.AltMediumLow:
                    _targetResources.AltMediumLow = _source.ActiveColor;
                    break;
                case ThemeColorTarget.BaseHigh:
                    _targetResources.BaseHigh = _source.ActiveColor;
                    break;
                case ThemeColorTarget.BaseLow:
                    _targetResources.BaseLow = _source.ActiveColor;
                    break;
                case ThemeColorTarget.BaseMedium:
                    _targetResources.BaseMedium = _source.ActiveColor;
                    break;
                case ThemeColorTarget.BaseMediumHigh:
                    _targetResources.BaseMediumHigh = _source.ActiveColor;
                    break;
                case ThemeColorTarget.BaseMediumLow:
                    _targetResources.BaseMediumLow = _source.ActiveColor;
                    break;
                case ThemeColorTarget.ChromeAltLow:
                    _targetResources.ChromeAltLow = _source.ActiveColor;
                    break;
                case ThemeColorTarget.ChromeBlackHigh:
                    _targetResources.ChromeBlackHigh = _source.ActiveColor;
                    break;
                case ThemeColorTarget.ChromeBlackLow:
                    _targetResources.ChromeBlackLow = _source.ActiveColor;
                    break;
                case ThemeColorTarget.ChromeBlackMedium:
                    _targetResources.ChromeBlackMedium = _source.ActiveColor;
                    break;
                case ThemeColorTarget.ChromeBlackMediumLow:
                    _targetResources.ChromeBlackMediumLow = _source.ActiveColor;
                    break;
                case ThemeColorTarget.ChromeDisabledHigh:
                    _targetResources.ChromeDisabledHigh = _source.ActiveColor;
                    break;
                case ThemeColorTarget.ChromeDisabledLow:
                    _targetResources.ChromeDisabledLow = _source.ActiveColor;
                    break;
                case ThemeColorTarget.ChromeGray:
                    _targetResources.ChromeGray = _source.ActiveColor;
                    break;
                case ThemeColorTarget.ChromeHigh:
                    _targetResources.ChromeHigh = _source.ActiveColor;
                    break;
                case ThemeColorTarget.ChromeLow:
                    _targetResources.ChromeLow = _source.ActiveColor;
                    break;
                case ThemeColorTarget.ChromeMedium:
                    _targetResources.ChromeMedium = _source.ActiveColor;
                    break;
                case ThemeColorTarget.ChromeMediumLow:
                    _targetResources.ChromeMediumLow = _source.ActiveColor;
                    break;
                case ThemeColorTarget.ChromeWhite:
                    _targetResources.ChromeWhite = _source.ActiveColor;
                    break;
                case ThemeColorTarget.ListLow:
                    _targetResources.ListLow = _source.ActiveColor;
                    break;
                case ThemeColorTarget.ListMedium:
                    _targetResources.ListMedium = _source.ActiveColor;
                    break;

                // Extra

                case ThemeColorTarget.TextActive:
                    _extraPalette.ActiveTextColor = _source.ActiveColor;
                    break;
                case ThemeColorTarget.LightAccent1:
                    _extraPalette.AccentLight1Color = _source.ActiveColor;
                    break;
                case ThemeColorTarget.DarkAccent1:
                    _extraPalette.AccentDark1Color = _source.ActiveColor;
                    break;
                case ThemeColorTarget.LightAccent2:
                    _extraPalette.AccentLight2Color = _source.ActiveColor;
                    break;
                case ThemeColorTarget.DarkAccent2:
                    _extraPalette.AccentDark2Color = _source.ActiveColor;
                    break;
                case ThemeColorTarget.LightAccent3:
                    _extraPalette.AccentLight3Color = _source.ActiveColor;
                    break;
                case ThemeColorTarget.DarkAccent3:
                    _extraPalette.AccentDark3Color = _source.ActiveColor;
                    break;

                case ThemeColorTarget.Hyperlink:
                    _extraPalette.HyperlinkButtonForegroundColor = _source.ActiveColor;
                    break;
                case ThemeColorTarget.HyperlinkActive:
                    _extraPalette.HyperlinkButtonActiveForegroundColor = _source.ActiveColor;
                    break;
                case ThemeColorTarget.HyperlinkDisabled:
                    _extraPalette.HyperlinkButtonDisabledForegroundColor = _source.ActiveColor;
                    break;
            }
        }
    }
}
