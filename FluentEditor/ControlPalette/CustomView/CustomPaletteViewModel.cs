// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using FluentEditor.OuterNav;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Data.Json;
using FluentEditorShared.ColorPalette;
using FluentEditorShared.Utils;
using FluentEditor.ControlPalette.Model;
using FluentEditorShared;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI;

namespace FluentEditor.ControlPalette.CustomView
{
    public class CustomPaletteViewModel : INavItem
    {
        public static CustomPaletteViewModel Parse(IStringProvider stringProvider, JsonObject data, IControlPaletteModel paletteModel, IControlPaletteExportProvider exportProvider)
        {
            return new CustomPaletteViewModel(stringProvider, paletteModel, data["Id"].GetOptionalString(), data["Title"].GetOptionalString(), data["Glyph"].GetOptionalString(), exportProvider);
        }

        private IStringProvider _stringProvider;

        private readonly string _id;
        public string Id
        {
            get { return _id; }
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                if (_title != value)
                {
                    _title = value;
                    RaisePropertyChangedFromSource();
                }
            }
        }

        private string _glyph;
        public string Glyph
        {
            get { return _glyph; }
            set
            {
                if (_glyph != value)
                {
                    _glyph = value;
                    RaisePropertyChangedFromSource();
                }
            }
        }

        public CustomPaletteViewModel(IStringProvider stringProvider, IControlPaletteModel paletteModel, string id, string title, string glyph, IControlPaletteExportProvider exportProvider)
        {
            _stringProvider = stringProvider;
            _id = id;
            _title = title;
            _glyph = glyph;
        }

        public void OnSaveDataRequested(object sender, RoutedEventArgs e)
        {
        }

        public void OnLoadDataRequested(object sender, RoutedEventArgs e)
        {
        }

        public void OnExportRequested(object sender, RoutedEventArgs e)
        {
        }


        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void RaisePropertyChangedFromSource([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }
}
