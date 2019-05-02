using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FluentEditor.ControlPalette.ThemePaletteView.Common
{
    public class BindableBase : INotifyPropertyChanged
    {
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

        protected bool SetProperty<T>(ref T property, T value, [CallerMemberName] string name = null)
        {
            if (object.Equals(property, value))
            {
                return false;
            }

            property = value;
            RaisePropertyChanged(name);

            return true;
        }
    }
}
