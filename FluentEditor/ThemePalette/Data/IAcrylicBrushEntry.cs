
namespace FluentEditor.ThemePalette.Data
{
    interface IAcrylicBrushEntry
    {
        double Opacity { get; set; }
        bool IsCustomOpacity { get; }

        double? LuminosityOpacity { get; set; }
        bool IsCustomLuminosityOpacity { get; }

        void ResetOpacity();
    }
}
