using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;


namespace FluentEditor.ControlPalette.CustomView
{
    public sealed partial class CustomPaletteView : Page
    {
        public CustomPaletteView()
        {
            this.InitializeComponent();
        }

        #region ViewModelProperty

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel), typeof(CustomPaletteViewModel), typeof(ControlPaletteView), new PropertyMetadata(null));

        public CustomPaletteViewModel ViewModel
        {
            get { return GetValue(ViewModelProperty) as CustomPaletteViewModel; }
            set { SetValue(ViewModelProperty, value); }
        }

        #endregion

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            ViewModel = e.Parameter as CustomPaletteViewModel;
        }
    }
}
