// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using FluentEditor.OuterNav;
using FluentEditor.ThemePalette.Model;
using FluentEditorShared;
using FluentEditorShared.ColorPalette;
using FluentEditorShared.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Storage;
using Windows.Storage.Provider;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace FluentEditor.ThemePalette
{
    public class ThemePaletteViewModel : INavItem
    {
        public static ThemePaletteViewModel Parse(IStringProvider stringProvider, JsonObject data, IThemePaletteModel paletteModel, FluentEditor.ControlPalette.Model.IControlPaletteExportProvider exportProvider)
        {
            return new ThemePaletteViewModel(stringProvider, paletteModel, data["Id"].GetOptionalString(), data["Title"].GetOptionalString(), data["Glyph"].GetOptionalString(), exportProvider);
        }

        public ThemePaletteViewModel(IStringProvider stringProvider, IThemePaletteModel paletteModel, string id, string title, string glyph, FluentEditor.ControlPalette.Model.IControlPaletteExportProvider exportProvider)
        {
            _stringProvider = stringProvider;
            _id = id;
            _title = title;
            _glyph = glyph;

            _paletteModel = paletteModel;
            _exportProvider = exportProvider;

            _lightRegionBrush = new SolidColorBrush(_paletteModel.LightRegion.ActiveColor);
            _darkRegionBrush = new SolidColorBrush(_paletteModel.DarkRegion.ActiveColor);

            _paletteModel.LightRegion.ActiveColorChanged += LightRegion_ActiveColorChanged;
            _paletteModel.DarkRegion.ActiveColorChanged += DarkRegion_ActiveColorChanged;

            _paletteModel.ActivePresetChanged += OnActivePresetChanged;
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

        public void OnSaveDataRequested(object sender, RoutedEventArgs e)
        {
            _ = SaveData();
        }

        private async Task SaveData()
        {
            StorageFile file = await FilePickerAdapters.ShowSaveFilePicker("ColorData", ".json", new Tuple<string, IList<string>>[] { new Tuple<string, IList<string>>("JSON", new List<string>() { ".json" }) }, null, Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary, true, true);
            if (file == null)
            {
                return;
            }
            CachedFileManager.DeferUpdates(file);

            ThemePreset savePreset = new ThemePreset(file.Path, file.DisplayName, _paletteModel);
            var saveData = ThemePreset.Serialize(savePreset);
            var saveString = saveData.Stringify();

            await FileIO.WriteTextAsync(file, saveString);
            FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
            if (status == FileUpdateStatus.Complete)
            {
                _paletteModel.AddOrReplacePreset(savePreset);
                _paletteModel.ApplyPreset(savePreset);
            }
            else
            {
                if (file == null || file.Path == null)
                {
                    return;
                }
                var message = string.Format(_stringProvider.GetString("ControlPaletteSaveError"), file.Path);
                ContentDialog saveFailedDialog = new ContentDialog()
                {
                    CloseButtonText = _stringProvider.GetString("ControlPaletteErrorOkButtonCaption"),
                    Title = _stringProvider.GetString("ControlPaletteSaveErrorTitle"),
                    Content = message
                };
                _ = saveFailedDialog.ShowAsync();
                return;
            }
        }

        public void OnLoadDataRequested(object sender, RoutedEventArgs e)
        {
            _ = LoadData();
        }

        private async Task LoadData()
        {
            StorageFile file = await FilePickerAdapters.ShowLoadFilePicker(new string[] { ".json" }, Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary, Windows.Storage.Pickers.PickerViewMode.List, true, true);
            if (file == null)
            {
                return;
            }
            string dataString = await FileIO.ReadTextAsync(file);
            JsonObject rootObject = JsonObject.Parse(dataString);
            ThemePreset loadedPreset = null;
            try
            {
                loadedPreset = ThemePreset.Parse(rootObject, file.Path, file.DisplayName);
            }
            catch
            {
                loadedPreset = null;
            }

            if (loadedPreset == null)
            {
                if (file == null || file.Path == null)
                {
                    return;
                }
                var message = string.Format(_stringProvider.GetString("ControlPaletteLoadError"), file.Path);
                ContentDialog loadFailedDialog = new ContentDialog()
                {
                    CloseButtonText = _stringProvider.GetString("ControlPaletteErrorOkButtonCaption"),
                    Title = _stringProvider.GetString("ControlPaletteLoadErrorTitle"),
                    Content = message
                };
                _ = loadFailedDialog.ShowAsync();
                return;
            }

            _paletteModel.AddOrReplacePreset(loadedPreset);
            _paletteModel.ApplyPreset(loadedPreset);
        }

        private void OnActivePresetChanged(IThemePaletteModel obj)
        {
            RaisePropertyChanged("ActivePreset");
        }

        private readonly IThemePaletteModel _paletteModel;
        private readonly FluentEditor.ControlPalette.Model.IControlPaletteExportProvider _exportProvider;

        public ThemePreset ActivePreset
        {
            get { return _paletteModel.ActivePreset; }
            set
            {
                _paletteModel.ApplyPreset(value);
            }
        }

        public IReadOnlyList<ThemePreset> Presets
        {
            get { return _paletteModel.Presets; }
        }

        public ColorPaletteEntry LightRegion
        {
            get { return _paletteModel.LightRegion; }
        }

        private void LightRegion_ActiveColorChanged(IColorPaletteEntry obj)
        {
            _lightRegionBrush.Color = obj.ActiveColor;
        }

        private SolidColorBrush _lightRegionBrush;
        public SolidColorBrush LightRegionBrush
        {
            get { return _lightRegionBrush; }
        }

        public ColorPaletteEntry DarkRegion
        {
            get { return _paletteModel.DarkRegion; }
        }

        private void DarkRegion_ActiveColorChanged(IColorPaletteEntry obj)
        {
            _darkRegionBrush.Color = obj.ActiveColor;
        }

        private SolidColorBrush _darkRegionBrush;
        public SolidColorBrush DarkRegionBrush
        {
            get { return _darkRegionBrush; }
        }

        public ColorPalette LightBase
        {
            get { return _paletteModel.LightBase; }
        }

        public ColorPalette DarkBase
        {
            get { return _paletteModel.DarkBase; }
        }

        public ColorPalette LightPrimary
        {
            get { return _paletteModel.LightPrimary; }
        }

        public ColorPalette DarkPrimary
        {
            get { return _paletteModel.DarkPrimary; }
        }

        public ColorPalette LightHyperlink
        {
            get { return _paletteModel.LightHyperlink; }
        }

        public ColorPalette DarkHyperlink
        {
            get { return _paletteModel.DarkHyperlink; }
        }

        public IReadOnlyList<ThemeColorMapping> LightColorMapping
        {
            get { return _paletteModel.LightColorMapping; }
        }

        public IReadOnlyList<ThemeColorMapping> DarkColorMapping
        {
            get { return _paletteModel.DarkColorMapping; }
        }

        public void OnExportRequested(object sender, RoutedEventArgs e)
        {
            // Todo Export
            //_exportProvider.ShowExportView(_exportProvider.GenerateExportData(_paletteModel));
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
