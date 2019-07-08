
namespace FluentEditor.ThemePalette.Data
{
    interface IAcrylicBrushEntry
    {
        double TintOpacity { get; set; }
        bool IsCustomTintOpacity { get; }

        double? TintLuminosityOpacity { get; set; }
        bool IsCustomTintLuminosityOpacity { get; }

        void ResetOpacity();
    }
}
