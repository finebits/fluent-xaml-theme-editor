// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FluentEditor.ThemePalette
{
    public sealed partial class ThemePaletteTestContent : UserControl
    {
        public ThemePaletteTestContent()
        {
            this.InitializeComponent();
        }

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string), typeof(ThemePaletteTestContent), new PropertyMetadata(null));

        public string Title
        {
            get { return GetValue(TitleProperty) as string; }
            set { SetValue(TitleProperty, value); }
        }
    }
}
