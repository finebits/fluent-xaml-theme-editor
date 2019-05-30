
namespace FluentEditor.ThemePalette.Data
{
    interface IAcrylicBrushEntry
    {
        double Opacity { get; set; }
        bool IsCustomOpacity { get; }

        void ResetOpacity();
    }
}
