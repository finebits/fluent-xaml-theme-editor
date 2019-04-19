using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace FluentEditor.ControlPalette.CustomView.Common
{
    class Theme : Common.BindableBase
    {
        private Color _background;
        public Color AppBackground
        {
            get { return _background; }
            set
            {
                SetProperty(ref _background, value);
            }
        }

        private Color _itemBackground0;
        public Color ItemBackground0
        {
            get { return _itemBackground0; }
            set
            {
                SetProperty(ref _itemBackground0, value);
            }
        }

        private Color _itemBackground1;
        public Color ItemBackground1
        {
            get { return _itemBackground1; }
            set
            {
                SetProperty(ref _itemBackground1, value);
            }
        }

        private Color _acrylicBackgroundColor;
        public Color AcrylicBackgroundColor
        {
            get { return _acrylicBackgroundColor; }
            set
            {
                SetProperty(ref _acrylicBackgroundColor, value);
            }
        }

        private double _acrylicBackgroundOpacity;
        public double AcrylicBackgroundOpacity
        {
            get { return _acrylicBackgroundOpacity; }
            set
            {
                SetProperty(ref _acrylicBackgroundOpacity, value);
            }
        }
    }
}
