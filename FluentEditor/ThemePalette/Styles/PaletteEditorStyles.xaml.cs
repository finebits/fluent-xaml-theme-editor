using FluentEditor.ThemePalette.Data;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FluentEditor.ThemePalette.Styles
{
    public sealed partial class PaletteEditorStyles : ResourceDictionary
    {
        public PaletteEditorStyles()
        {
            this.InitializeComponent();
        }
    }

    public class AcrylicTemplateSelector : DataTemplateSelector
    {
        public DataTemplate AcrylicDataTemplate { get; set; }
        public DataTemplate BaseDataTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            if(typeof(ThemeEditableAcrylicPaletteEntry) == item.GetType())
            {
                return AcrylicDataTemplate;
            }

            return BaseDataTemplate;
        }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (typeof(ThemeEditableAcrylicPaletteEntry) == item.GetType())
            {
                return AcrylicDataTemplate;
            }

            return BaseDataTemplate;
        }
    }
}
