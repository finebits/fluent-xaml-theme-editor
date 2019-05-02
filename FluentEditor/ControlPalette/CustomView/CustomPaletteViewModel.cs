﻿// Copyright (c) Microsoft Corporation. All rights reserved.
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
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.Storage.Provider;
using FluentEditor.ControlPalette.Model.Custom;

namespace FluentEditor.ControlPalette.CustomView
{
    public class CustomPaletteViewModel : INavItem
    {
        public static CustomPaletteViewModel Parse(IStringProvider stringProvider, JsonObject data, ICustomPaletteModel paletteModel)
        {
            return new CustomPaletteViewModel(stringProvider, paletteModel, data["Id"].GetOptionalString(), data["Title"].GetOptionalString(), data["Glyph"].GetOptionalString(), null);
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

        public ICustomPaletteModel CurrentModel
        {
            get;
            private set;
        }

        public IReadOnlyList<CustomPreset> Presets
        {
            get { return CurrentModel.Presets; }
        }

        public CustomPreset ActivePreset
        {
            get { return CurrentModel.ActivePreset; }
            set
            {
                CurrentModel.ApplyPreset(value);
            }
        }

        public List<ContrastColorWrapper> TextLightContrastColors
        {
            get;
            set;
        }

        public List<ContrastColorWrapper> TextDarkContrastColors
        {
            get;
            set;
        }

        public List<ContrastColorWrapper> BackgroundLightContrastColors
        {
            get;
            set;
        }

        public List<ContrastColorWrapper> BackgroundDarkContrastColors
        {
            get;
            set;
        }

        public List<ContrastColorWrapper> AccentLightContrastColors
        {
            get;
            set;
        }

        public List<ContrastColorWrapper> AccentDarkContrastColors
        {
            get;
            set;
        }

        public List<ContrastColorWrapper> LightTextColors
        {
            get;
            set;
        }

        public List<ContrastColorWrapper> DarkTextColors
        {
            get;
            set;
        }

        public List<ContrastColorWrapper> LightBackgroundColors
        {
            get;
            set;
        }

        public List<ContrastColorWrapper> DarkBackgroundColors
        {
            get;
            set;
        }

        public CustomPaletteViewModel(IStringProvider stringProvider, ICustomPaletteModel paletteModel, string id, string title, string glyph, IControlPaletteExportProvider exportProvider)
        {
            _stringProvider = stringProvider;
            _id = id;
            _title = title;
            _glyph = glyph;

            CurrentModel = paletteModel;

            CurrentModel.ActivePresetChanged += OnActivePresetChanged;
            CurrentModel.ThemeChanged += OnThemeChanged;
        }

        private void OnThemeChanged(ICustomPaletteModel obj)
        {
            ThemeChanged?.Invoke(obj);
        }

        private void OnActivePresetChanged(ICustomPaletteModel obj)
        {
            RaisePropertyChanged(nameof(ActivePreset));
        }

        public void OnSaveDataRequested(object sender, RoutedEventArgs e)
        {
            _ = SaveData();
        }

        public void OnLoadDataRequested(object sender, RoutedEventArgs e)
        {
            _ = LoadData();
        }

        public void OnExportRequested(object sender, RoutedEventArgs e)
        {
        }

        private async Task SaveData()
        {
            StorageFile file = await FilePickerAdapters.ShowSaveFilePicker("ColorData", ".json", new Tuple<string, IList<string>>[] { new Tuple<string, IList<string>>("JSON", new List<string>() { ".json" }) }, null, Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary, true, true);
            if (file == null)
            {
                return;
            }
            CachedFileManager.DeferUpdates(file);

            CustomPreset savePreset = new CustomPreset(file.Path, file.DisplayName, CurrentModel);
            var saveData = CustomPreset.Serialize(savePreset);
            var saveString = saveData.Stringify();

            await FileIO.WriteTextAsync(file, saveString);
            FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
            if (status == FileUpdateStatus.Complete)
            {
                CurrentModel.AddOrReplacePreset(savePreset);
                CurrentModel.ApplyPreset(savePreset);
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


        private async Task LoadData()
        {
            StorageFile file = await FilePickerAdapters.ShowLoadFilePicker(new string[] { ".json" }, Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary, Windows.Storage.Pickers.PickerViewMode.List, true, true);
            if (file == null)
            {
                return;
            }
            string dataString = await FileIO.ReadTextAsync(file);
            JsonObject rootObject = JsonObject.Parse(dataString);
            CustomPreset loadedPreset = null;
            try
            {
                loadedPreset = CustomPreset.Parse(rootObject, file.Path, file.DisplayName);
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

            CurrentModel.AddOrReplacePreset(loadedPreset);
            CurrentModel.ApplyPreset(loadedPreset);
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

        public event Action<ICustomPaletteModel> ThemeChanged;
    }
}
