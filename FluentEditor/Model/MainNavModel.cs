// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using FluentEditor.ControlPalette.Model;
using FluentEditor.OuterNav;
using FluentEditor.ThemePalette.Model;
using FluentEditorShared;
using FluentEditorShared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Storage;

namespace FluentEditor.Model
{
    public interface IMainNavModel
    {
        Task InitializeData(string dataPath, IControlPaletteModel paletteModel, ControlPaletteExportProvider controlPaletteExportProvider);
        Task HandleAppSuspend();

        IReadOnlyList<INavItem> NavItems { get; }
        INavItem DefaultNavItem { get; }
    }

    public class MainNavModel : IMainNavModel
    {
        public MainNavModel(IStringProvider stringProvider)
        {
            _stringProvider = stringProvider;
        }

        private IStringProvider _stringProvider;

        public async Task InitializeData(string dataPath, IControlPaletteModel paletteModel, ControlPaletteExportProvider controlPaletteExportProvider)
        {
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(dataPath));
            string dataString = await FileIO.ReadTextAsync(file);
            JsonObject rootObject = JsonObject.Parse(dataString);

            List<INavItem> navItems = new List<INavItem>();

            if (rootObject.ContainsKey("Demos"))
            {
                JsonArray demoDataList = rootObject["Demos"].GetArray();
                foreach (var demoData in demoDataList)
                {
                    navItems.Add(ParseNavItem(demoData.GetObject(), paletteModel, controlPaletteExportProvider));
                }
            }

            string defaultDemoId = rootObject.GetOptionalString("DefaultDemoId");
            if (!string.IsNullOrEmpty(defaultDemoId))
            {
                _defaultNavItem = navItems.FirstOrDefault((a) => a.Id == defaultDemoId);
            }

            _navItems = navItems;
        }


        public async Task HandleAppSuspend()
        {
            foreach (var item in _navItems)
            {
                if (item is IControlPaletteModel paletteModel)
                {
                    await paletteModel.HandleAppSuspend();
                }
            }
        }

        private INavItem ParseNavItem(JsonObject data, IControlPaletteModel paletteModel, ControlPaletteExportProvider controlPaletteExportProvider)
        {
            string type = data.GetOptionalString("Type");

            switch (type)
            {
                case "ControlPalette":
                    return ControlPalette.ControlPaletteViewModel.Parse(_stringProvider, data, paletteModel, controlPaletteExportProvider);
                default:
                    throw new Exception(string.Format("Unknown nav item type {0}", type));
            }
        }

        #region 
        public async Task InitializeData(string dataPath, ControlPaletteExportProvider controlPaletteExportProvider)
        {
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(dataPath));
            string dataString = await FileIO.ReadTextAsync(file);
            JsonObject rootObject = JsonObject.Parse(dataString);

            List<INavItem> navItems = new List<INavItem>();

            if (rootObject.ContainsKey("Demos"))
            {
                JsonArray demoDataList = rootObject["Demos"].GetArray();
                foreach (var demoData in demoDataList)
                {
                    navItems.Add(await ParseNavItem(demoData.GetObject(), controlPaletteExportProvider));
                }
            }

            string defaultDemoId = rootObject.GetOptionalString("DefaultDemoId");
            if (!string.IsNullOrEmpty(defaultDemoId))
            {
                _defaultNavItem = navItems.FirstOrDefault((a) => a.Id == defaultDemoId);
            }

            _navItems = navItems;
        }

        private async Task<INavItem> ParseNavItem(JsonObject data, ControlPaletteExportProvider controlPaletteExportProvider)
        {
            string type = data.GetOptionalString("Type");

            switch (type)
            {
                case nameof(ControlPalette):
                    {
                        var paletteModel = new ControlPaletteModel();
                        await paletteModel.InitializeData(_stringProvider, _stringProvider.GetString("ControlPaletteDataPath"));

                        return ControlPalette.ControlPaletteViewModel.Parse(_stringProvider, data, paletteModel, controlPaletteExportProvider);
                    }
                case nameof(ThemePalette):
                    {
                        var paletteModel = new ThemePaletteModel();
                        await paletteModel.InitializeData(_stringProvider, _stringProvider.GetString("ThemePaletteDataPath"));

                        return ThemePalette.ThemePaletteViewModel.Parse(_stringProvider, data, paletteModel, controlPaletteExportProvider);
                    }
                default:
                    throw new Exception(string.Format("Unknown nav item type {0}", type));
            }
        }

        #endregion

        private List<INavItem> _navItems = null;
        public IReadOnlyList<INavItem> NavItems
        {
            get { return _navItems; }
        }

        private INavItem _defaultNavItem = null;
        public INavItem DefaultNavItem
        {
            get { return _defaultNavItem; }
        }
    }
}
