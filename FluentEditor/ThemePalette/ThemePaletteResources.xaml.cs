// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using FluentEditor.ThemePalette.Model;
using System.Collections.Generic;
using Windows.UI;
using Windows.UI.Xaml;

namespace FluentEditor.ThemePalette
{
    public sealed partial class ThemePaletteResources : ResourceDictionary
    {
        public ThemePaletteResources()
        {
            this.InitializeComponent();

            DarkExtraPalette.PropertyChanged += ExtraPalettePropertyChanged;
            LightExtraPalette.PropertyChanged += ExtraPalettePropertyChanged;
        }

        private void ExtraPalettePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(sender is ThemeExtraPalette palette)
            {
                switch(e.PropertyName)
                {
                    case nameof(ThemeExtraPalette.AcrylicBackdropBackgroundTintLuminosityOpacity):
                    case nameof(ThemeExtraPalette.AcrylicBackdropBackgroundTintOpacity):
                        UpdateAcrylicBrush(palette, "AcrylicBackdropBackgroundBrush", palette.AcrylicBackdropBackgroundTintOpacity, palette.AcrylicBackdropBackgroundTintLuminosityOpacity);
                        break;
                    case nameof(ThemeExtraPalette.AcrylicHostBackdropBackgroundTintLuminosityOpacity):
                    case nameof(ThemeExtraPalette.AcrylicHostBackdropBackgroundTintOpacity):
                        UpdateAcrylicBrush(palette, "AcrylicHostBackdropBackgroundBrush", palette.AcrylicHostBackdropBackgroundTintOpacity, palette.AcrylicHostBackdropBackgroundTintLuminosityOpacity);
                        break;
                }
            }
        }

        private void UpdateAcrylicBrush(ThemeExtraPalette palette, string brushName, double tintOpacity, double? tintLuminosityOpacity)
        {
            ResourceDictionary resourceDictionary = LightResourceDictionary;
            if (palette == DarkExtraPalette)
            {
                resourceDictionary = DarkResourceDictionary;
            }

            if (resourceDictionary[brushName] is Microsoft.UI.Xaml.Media.AcrylicBrush acrylicBrush)
            {
                acrylicBrush.TintOpacity = tintOpacity;
                acrylicBrush.TintLuminosityOpacity = tintLuminosityOpacity;
            }
        }

        private List<ThemeColorMappingInstance> _lightColorMappings;
        private List<ThemeColorMappingInstance> _darkColorMappings;

        #region LinkedElementProperty

        public static readonly DependencyProperty LinkedElementProperty = DependencyProperty.Register("LinkedElement", typeof(FrameworkElement), typeof(ThemePaletteResources), new PropertyMetadata(null, new PropertyChangedCallback(OnLinkedElementPropertyChanged)));

        private static void OnLinkedElementPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ThemePaletteResources target)
            {
                target.OnLinkedElementChanged(e.OldValue as FrameworkElement, e.NewValue as FrameworkElement);
            }
        }

        private void OnLinkedElementChanged(FrameworkElement oldValue, FrameworkElement newValue)
        {
            if (_lightColorMappings != null)
            {
                foreach (var mapping in _lightColorMappings)
                {
                    mapping.LinkedElement = newValue;
                }
            }
            if (_darkColorMappings != null)
            {
                foreach (var mapping in _darkColorMappings)
                {
                    mapping.LinkedElement = newValue;
                }
            }
        }

        public FrameworkElement LinkedElement
        {
            get { return GetValue(LinkedElementProperty) as FrameworkElement; }
            set { SetValue(LinkedElementProperty, value); }
        }

        #endregion

        #region LightColorMappingProperty

        public static readonly DependencyProperty LightColorMappingProperty = DependencyProperty.Register("LightColorMapping", typeof(IReadOnlyList<ThemeColorMapping>), typeof(ThemePaletteResources), new PropertyMetadata(null, new PropertyChangedCallback(OnLightColorMappingPropertyChanged)));

        private static void OnLightColorMappingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ThemePaletteResources target)
            {
                target.OnLightColorMappingChanged(e.OldValue as IReadOnlyList<ThemeColorMapping>, e.NewValue as IReadOnlyList<ThemeColorMapping>);
            }
        }

        private void OnLightColorMappingChanged(IReadOnlyList<ThemeColorMapping> oldValue, IReadOnlyList<ThemeColorMapping> newValue)
        {
            if (_lightColorMappings != null && _lightColorMappings.Count > 0)
            {
                for (int i = 0; i < _lightColorMappings.Count; i++)
                {
                    _lightColorMappings[i].Dispose();
                }
                _lightColorMappings.Clear();
                _lightColorMappings = null;
            }

            if (newValue != null && newValue.Count > 0)
            {
                var linkedElement = LinkedElement;
                _lightColorMappings = new List<ThemeColorMappingInstance>(newValue.Count);
                for (int i = 0; i < newValue.Count; i++)
                {
                    var instance = newValue[i].CreateInstance(LightColorPaletteResources, LightExtraPalette);
                    instance.LinkedElement = linkedElement;
                    _lightColorMappings.Add(instance);
                }
            }
        }

        public IReadOnlyList<ThemeColorMapping> LightColorMapping
        {
            get { return GetValue(LightColorMappingProperty) as IReadOnlyList<ThemeColorMapping>; }
            set { SetValue(LightColorMappingProperty, value); }
        }

        #endregion

        #region DarkColorMappingProperty

        public static readonly DependencyProperty DarkColorMappingProperty = DependencyProperty.Register("DarkColorMapping", typeof(IReadOnlyList<ThemeColorMapping>), typeof(ThemePaletteResources), new PropertyMetadata(null, new PropertyChangedCallback(OnDarkColorMappingPropertyChanged)));

        private static void OnDarkColorMappingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ThemePaletteResources target)
            {
                target.OnDarkColorMappingChanged(e.OldValue as IReadOnlyList<ThemeColorMapping>, e.NewValue as IReadOnlyList<ThemeColorMapping>);
            }
        }

        private void OnDarkColorMappingChanged(IReadOnlyList<ThemeColorMapping> oldValue, IReadOnlyList<ThemeColorMapping> newValue)
        {
            if (_darkColorMappings != null && _darkColorMappings.Count > 0)
            {
                for (int i = 0; i < _darkColorMappings.Count; i++)
                {
                    _darkColorMappings[i].Dispose();
                }
                _darkColorMappings.Clear();
                _darkColorMappings = null;
            }

            if (newValue != null && newValue.Count > 0)
            {
                var linkedElement = LinkedElement;
                _darkColorMappings = new List<ThemeColorMappingInstance>(newValue.Count);
                for (int i = 0; i < newValue.Count; i++)
                {
                    var instance = newValue[i].CreateInstance(DarkColorPaletteResources, DarkExtraPalette);
                    instance.LinkedElement = linkedElement;
                    _darkColorMappings.Add(instance);
                }
            }
        }

        public IReadOnlyList<ThemeColorMapping> DarkColorMapping
        {
            get { return GetValue(DarkColorMappingProperty) as IReadOnlyList<ThemeColorMapping>; }
            set { SetValue(DarkColorMappingProperty, value); }
        }

        #endregion
    }
}
