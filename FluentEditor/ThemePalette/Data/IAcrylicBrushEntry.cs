
namespace FluentEditor.ThemePalette.Data
{
    interface IAcrylicBrushEntry
    {
        bool IsHostBackdrop { get; set; }
        bool IsCustomHostBackdrop { get; }

        double TintOpacity { get; set; }
        bool IsCustomTintOpacity { get; }

        double? TintLuminosityOpacity { get; set; }
        bool IsCustomTintLuminosityOpacity { get; }

        void ResetOpacity();
    }
}
