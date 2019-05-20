using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace FluentEditor.ThemePalette
{
    public sealed partial class ThemePaletteView : Page
    {
        #region ViewModelProperty

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel), typeof(ThemePaletteViewModel), typeof(ThemePaletteView), new PropertyMetadata(null));

        public ThemePaletteViewModel ViewModel
        {
            get { return GetValue(ViewModelProperty) as ThemePaletteViewModel; }
            set { SetValue(ViewModelProperty, value); }
        }

        #endregion

        public ThemePaletteView()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            ViewModel = e.Parameter as ThemePaletteViewModel;
        }
    }
}
