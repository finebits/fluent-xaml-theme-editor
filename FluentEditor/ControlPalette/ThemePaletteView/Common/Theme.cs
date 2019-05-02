using FluentEditorShared.Utils;
using Windows.Data.Json;
using Windows.UI;

namespace FluentEditor.ControlPalette.ThemePaletteView.Common
{
    public class Theme : Common.BindableBase
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


        private Color _textColor;
        public Color TextColor
        {
            get { return _textColor; }
            set
            {
                SetProperty(ref _textColor, value);
            }
        }

        private Color _activeTextColor;
        public Color ActiveTextColor
        {
            get { return _activeTextColor; }
            set
            {
                SetProperty(ref _activeTextColor, value);
            }
        }

        private Color _subTextColor;
        public Color SubTextColor
        {
            get { return _subTextColor; }
            set
            {
                SetProperty(ref _subTextColor, value);
            }
        }

        private Color _subSubTextColor;
        public Color SubSubTextColor
        {
            get { return _subSubTextColor; }
            set
            {
                SetProperty(ref _subSubTextColor, value);
            }
        }

        private Color _accentColor;
        public Color AccentColor
        {
            get { return _accentColor; }
            set
            {
                SetProperty(ref _accentColor, value);
            }
        }

        private Color _activeAccentColor;
        public Color ActiveAccentColor
        {
            get { return _activeAccentColor; }
            set
            {
                SetProperty(ref _activeAccentColor, value);
            }
        }


        private Color _hyperlinkButtonForegroundColor;
        public Color HyperlinkButtonForegroundColor
        {
            get { return _hyperlinkButtonForegroundColor; }
            set
            {
                SetProperty(ref _hyperlinkButtonForegroundColor, value);
            }
        }

        private Color _hyperlinkButtonActiveForegroundColor;
        public Color HyperlinkButtonActiveForegroundColor
        {
            get { return _hyperlinkButtonActiveForegroundColor; }
            set
            {
                SetProperty(ref _hyperlinkButtonActiveForegroundColor, value);
            }
        }

        private Color _hyperlinkButtonDisableForegroundColor;
        public Color HyperlinkButtonDisableForegroundColor
        {
            get { return _hyperlinkButtonDisableForegroundColor; }
            set
            {
                SetProperty(ref _hyperlinkButtonDisableForegroundColor, value);
            }
        }


        private Color _textButtonForegroundColor;
        public Color TextButtonForegroundColor
        {
            get { return _textButtonForegroundColor; }
            set
            {
                SetProperty(ref _textButtonForegroundColor, value);
            }
        }

        private Color _textButtonActiveForegroundColor;
        public Color TextButtonActiveForegroundColor
        {
            get { return _textButtonActiveForegroundColor; }
            set
            {
                SetProperty(ref _textButtonActiveForegroundColor, value);
            }
        }

        private Color _textButtonDisableForegroundColor;
        public Color TextButtonDisableForegroundColor
        {
            get { return _textButtonDisableForegroundColor; }
            set
            {
                SetProperty(ref _textButtonDisableForegroundColor, value);
            }
        }

        private Color _controlForegroundColor;
        public Color ControlForegroundColor
        {
            get { return _controlForegroundColor; }
            set
            {
                SetProperty(ref _controlForegroundColor, value);
            }
        }

        private Color _controlActiveForegroundColor;
        public Color ControlActiveForegroundColor
        {
            get { return _controlActiveForegroundColor; }
            set
            {
                SetProperty(ref _controlActiveForegroundColor, value);
            }
        }

        private Color _controlDisableForegroundColor;
        public Color ControlDisableForegroundColor
        {
            get { return _controlDisableForegroundColor; }
            set
            {
                SetProperty(ref _controlDisableForegroundColor, value);
            }
        }

        private Color _controlDisableBackgroundColor;
        public Color ControlDisableBackgroundColor
        {
            get { return _controlDisableBackgroundColor; }
            set
            {
                SetProperty(ref _controlDisableBackgroundColor, value);
            }
        }

        private Color _controlBackgroundColor;
        public Color ControlBackgroundColor
        {
            get { return _controlBackgroundColor; }
            set
            {
                SetProperty(ref _controlBackgroundColor, value);
            }
        }

        private Color _controlActiveBackgroundColor;
        public Color ControlActiveBackgroundColor
        {
            get { return _controlActiveBackgroundColor; }
            set
            {
                SetProperty(ref _controlActiveBackgroundColor, value);
            }
        }

        public Theme() { }

        public Theme(Theme theme)
        {
            AppBackground = theme.AppBackground;
            ItemBackground0 = theme.ItemBackground0;
            ItemBackground1 = theme.ItemBackground1;
            AcrylicBackgroundColor = theme.AcrylicBackgroundColor;
            AcrylicBackgroundOpacity = theme.AcrylicBackgroundOpacity;
            TextColor = theme.TextColor;
            ActiveTextColor = theme.ActiveTextColor;
            SubTextColor = theme.SubTextColor;
            SubSubTextColor = theme.SubSubTextColor;
            AccentColor = theme.AccentColor;
            ActiveAccentColor = theme.ActiveAccentColor;
            HyperlinkButtonForegroundColor = theme.HyperlinkButtonForegroundColor;
            HyperlinkButtonActiveForegroundColor = theme.HyperlinkButtonActiveForegroundColor;
            HyperlinkButtonDisableForegroundColor = theme.HyperlinkButtonDisableForegroundColor;
            TextButtonForegroundColor = theme.TextButtonForegroundColor;
            TextButtonActiveForegroundColor = theme.TextButtonActiveForegroundColor;
            TextButtonDisableForegroundColor = theme.TextButtonDisableForegroundColor;
        }


        public bool IsEquals(Theme theme)
        {
            if(AppBackground != theme.AppBackground)
            {
                return false;
            }

            if (ItemBackground0 != theme.ItemBackground0)
            {
                return false;
            }

            if (ItemBackground1 != theme.ItemBackground1)
            {
                return false;
            }

            if (AcrylicBackgroundColor != theme.AcrylicBackgroundColor)
            {
                return false;
            }

            if (AcrylicBackgroundOpacity != theme.AcrylicBackgroundOpacity)
            {
                return false;
            }

            if (TextColor != theme.TextColor)
            {
                return false;
            }

            if (ActiveTextColor != theme.ActiveTextColor)
            {
                return false;
            }

            if (SubTextColor != theme.SubTextColor)
            {
                return false;
            }

            if (SubSubTextColor != theme.SubSubTextColor)
            {
                return false;
            }

            if (AccentColor != theme.AccentColor)
            {
                return false;
            }

            if (ActiveAccentColor != theme.ActiveAccentColor)
            {
                return false;
            }

            if (HyperlinkButtonForegroundColor != theme.HyperlinkButtonForegroundColor)
            {
                return false;
            }

            if (HyperlinkButtonActiveForegroundColor != theme.HyperlinkButtonActiveForegroundColor)
            {
                return false;
            }

            if (HyperlinkButtonDisableForegroundColor != theme.HyperlinkButtonDisableForegroundColor)
            {
                return false;
            }

            if (TextButtonForegroundColor != theme.TextButtonForegroundColor)
            {
                return false;
            }

            if (TextButtonActiveForegroundColor != theme.TextButtonActiveForegroundColor)
            {
                return false;
            }

            if (TextButtonDisableForegroundColor != theme.TextButtonDisableForegroundColor)
            {
                return false;
            }

            return true;
        }

        public JsonObject Serialize()
        {
            JsonObject data = new JsonObject();

            data[nameof(AppBackground)] = JsonValue.CreateStringValue(AppBackground.ToString());
            data[nameof(ItemBackground0)] = JsonValue.CreateStringValue(ItemBackground0.ToString());
            data[nameof(ItemBackground1)] = JsonValue.CreateStringValue(ItemBackground1.ToString());
            data[nameof(AcrylicBackgroundColor)] = JsonValue.CreateStringValue(AcrylicBackgroundColor.ToString());
            data[nameof(AcrylicBackgroundOpacity)] = JsonValue.CreateNumberValue(AcrylicBackgroundOpacity);


            data[nameof(TextColor)] = JsonValue.CreateStringValue(TextColor.ToString());
            data[nameof(ActiveTextColor)] = JsonValue.CreateStringValue(ActiveTextColor.ToString());
            data[nameof(SubTextColor)] = JsonValue.CreateStringValue(SubTextColor.ToString());
            data[nameof(SubSubTextColor)] = JsonValue.CreateStringValue(SubSubTextColor.ToString());


            data[nameof(AccentColor)] = JsonValue.CreateStringValue(AccentColor.ToString());
            data[nameof(ActiveAccentColor)] = JsonValue.CreateStringValue(ActiveAccentColor.ToString());

            data[nameof(HyperlinkButtonForegroundColor)] = JsonValue.CreateStringValue(HyperlinkButtonForegroundColor.ToString());
            data[nameof(HyperlinkButtonActiveForegroundColor)] = JsonValue.CreateStringValue(HyperlinkButtonActiveForegroundColor.ToString());
            data[nameof(HyperlinkButtonDisableForegroundColor)] = JsonValue.CreateStringValue(HyperlinkButtonDisableForegroundColor.ToString());

            data[nameof(TextButtonForegroundColor)] = JsonValue.CreateStringValue(TextButtonForegroundColor.ToString());
            data[nameof(TextButtonActiveForegroundColor)] = JsonValue.CreateStringValue(TextButtonActiveForegroundColor.ToString());
            data[nameof(TextButtonDisableForegroundColor)] = JsonValue.CreateStringValue(TextButtonDisableForegroundColor.ToString());

            return data;
        }

        public void Load(JsonObject data)
        {
            AppBackground = data[nameof(AppBackground)].GetColor();
            ItemBackground0 = data[nameof(ItemBackground0)].GetColor();
            ItemBackground1 = data[nameof(ItemBackground1)].GetColor();
            AcrylicBackgroundColor = data[nameof(AcrylicBackgroundColor)].GetColor();
            AcrylicBackgroundOpacity = data[nameof(AcrylicBackgroundOpacity)].GetNumber();

            TextColor = data[nameof(TextColor)].GetColor();
            ActiveTextColor = data[nameof(ActiveTextColor)].GetColor();
            SubTextColor = data[nameof(SubTextColor)].GetColor();
            SubSubTextColor = data[nameof(SubSubTextColor)].GetColor();

            AccentColor = data[nameof(AccentColor)].GetColor();
            ActiveAccentColor = data[nameof(ActiveAccentColor)].GetColor();

            HyperlinkButtonForegroundColor = data[nameof(HyperlinkButtonForegroundColor)].GetColor();
            HyperlinkButtonActiveForegroundColor = data[nameof(HyperlinkButtonActiveForegroundColor)].GetColor();
            HyperlinkButtonDisableForegroundColor = data[nameof(HyperlinkButtonDisableForegroundColor)].GetColor();

            TextButtonForegroundColor = data[nameof(TextButtonForegroundColor)].GetColor();
            TextButtonActiveForegroundColor = data[nameof(TextButtonActiveForegroundColor)].GetColor();
            TextButtonDisableForegroundColor = data[nameof(TextButtonDisableForegroundColor)].GetColor();
        }
    }
}
