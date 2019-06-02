﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FluentEditorShared.ColorPalette
{
    public sealed partial class ExpandedColorPaletteEntryEditor : UserControl
    {
        public ExpandedColorPaletteEntryEditor()
        {
            this.InitializeComponent();
        }

        #region ColorPaletteEntryProperty

        public static readonly DependencyProperty ColorPaletteEntryProperty = DependencyProperty.Register("ColorPaletteEntry", typeof(IColorPaletteEntry), typeof(ExpandedColorPaletteEntryEditor), new PropertyMetadata(null, new PropertyChangedCallback(OnColorPaletteEntryPropertyChanged)));

        private static void OnColorPaletteEntryPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ExpandedColorPaletteEntryEditor target)
            {
                target.OnColorPaletteEntryChanged(e.OldValue as IColorPaletteEntry, e.NewValue as IColorPaletteEntry);
            }
        }

        private void OnColorPaletteEntryChanged(IColorPaletteEntry oldValue, IColorPaletteEntry newValue)
        {
            if (oldValue != null)
            {
                oldValue.ActiveColorChanged -= ColorPaletteEntry_ActiveColorChanged;
            }

            if (newValue != null)
            {
                ColorPicker.Color = newValue.ActiveColor;
                newValue.ActiveColorChanged += ColorPaletteEntry_ActiveColorChanged;

                if (string.IsNullOrEmpty(newValue.Title))
                {
                    TitleField.Visibility = Visibility.Collapsed;
                    TitleField.Text = string.Empty;
                }
                else
                {
                    TitleField.Visibility = Visibility.Visible;
                    TitleField.Text = newValue.Title;
                }
                if (string.IsNullOrEmpty(newValue.Description))
                {
                    DescriptionField.Visibility = Visibility.Collapsed;
                    DescriptionField.Text = string.Empty;
                }
                else
                {
                    DescriptionField.Visibility = Visibility.Visible;
                    DescriptionField.Text = newValue.Description;
                }
                if (newValue is EditableColorPaletteEntry editableNewValue)
                {
                    RevertButton.Visibility = Visibility.Visible;
                    RevertButton.IsEnabled = editableNewValue.UseCustomColor;
                }
                else
                {
                    RevertButton.Visibility = Visibility.Collapsed;
                }

                if (newValue.ContrastColors != null)
                {
                    UpdateContrastColors(newValue);
                }
                else
                {
                    SetValue(ContrastListProperty, null);
                }
            }
            else
            {
                ColorPicker.Color = default(Color);
                SetValue(ContrastListProperty, null);
            }
        }

        public IColorPaletteEntry ColorPaletteEntry
        {
            get { return GetValue(ColorPaletteEntryProperty) as IColorPaletteEntry; }
            set { SetValue(ColorPaletteEntryProperty, value); }
        }

        #endregion

        #region ContrastListProperty

        public static readonly DependencyProperty ContrastListProperty = DependencyProperty.Register("ContrastList", typeof(List<ContrastListItem>), typeof(ExpandedColorPaletteEntryEditor), new PropertyMetadata(null));

        public List<ContrastListItem> ContrastList
        {
            get { return GetValue(ContrastListProperty) as List<ContrastListItem>; }
        }

        #endregion

        #region IsAlphaEnabledProperty

        public bool IsAlphaEnabled
        {
            get { return (bool)GetValue(IsAlphaEnabledProperty); }
            set { SetValue(IsAlphaEnabledProperty, value); }
        }

        public static readonly DependencyProperty IsAlphaEnabledProperty =
            DependencyProperty.Register(nameof(IsAlphaEnabled), typeof(bool), typeof(ExpandedColorPaletteEntryEditor), new PropertyMetadata(false));

        #endregion

        #region ThemeBackgroundProperty

        public Windows.UI.Xaml.Media.SolidColorBrush ThemeBackground
        {
            get { return (Windows.UI.Xaml.Media.SolidColorBrush)GetValue(ThemeBackgroundProperty); }
            set { SetValue(ThemeBackgroundProperty, value); }
        }

        public static readonly DependencyProperty ThemeBackgroundProperty =
            DependencyProperty.Register(nameof(ThemeBackground), typeof(Windows.UI.Xaml.Media.SolidColorBrush), typeof(ExpandedColorPaletteEntryEditor), new PropertyMetadata(null, OnThemeBackgroundChanged));

        private static void OnThemeBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ExpandedColorPaletteEntryEditor target)
            {
                target.UpdateContrastColors(target.ColorPaletteEntry);
            }
        }

        #endregion

        private void ColorPaletteEntry_ActiveColorChanged(IColorPaletteEntry obj)
        {
            if (obj is EditableColorPaletteEntry editablePaletteEntry)
            {
                RevertButton.IsEnabled = editablePaletteEntry.UseCustomColor;
            }
            if (obj != null)
            {
                ColorPicker.Color = obj.ActiveColor;
            }
        }

        private void ColorPicker_ColorChanged(Microsoft.UI.Xaml.Controls.ColorPicker sender, Microsoft.UI.Xaml.Controls.ColorChangedEventArgs args)
        {
            // In this case it is easier to deal with an event than the loopbacks that the ColorPicker creates with a two way binding
            var paletteEntry = ColorPaletteEntry;
            if (paletteEntry != null)
            {
                paletteEntry.ActiveColor = args.NewColor;
            }
        }

        private void RevertButton_Click(object sender, RoutedEventArgs e)
        {
            var paletteEntry = ColorPaletteEntry;
            if (paletteEntry is EditableColorPaletteEntry editablePaletteEntry)
            {
                editablePaletteEntry.UseCustomColor = false;
            }
        }

        private void UpdateContrastColors(IColorPaletteEntry value)
        {
            if (value?.ContrastColors != null)
            {
                List<ContrastListItem> contrastList = new List<ContrastListItem>();

                ContrastListItem CreateContrastListItem(IColorPaletteEntry colorValue, IColorPaletteEntry contrastColor, double minContrast = 4.5)
                {
                    if (IsAlphaEnabled && ThemeBackground != null)
                    {
                        return new ContrastListItem(colorValue, contrastColor, ThemeBackground.Color, minContrast);
                    }

                    return new ContrastListItem(colorValue, contrastColor, minContrast);
                }

                foreach (var c in value.ContrastColors)
                {
                    if (c.ShowInContrastList)
                    {
                        if (c.ShowContrastErrors)
                        {
                            contrastList.Add(CreateContrastListItem(value, c.Color));
                        }
                        else
                        {
                            contrastList.Add(CreateContrastListItem(value, c.Color, 0));
                        }

                    }
                }

                SetValue(ContrastListProperty, contrastList);
            }
        }
    }
}
