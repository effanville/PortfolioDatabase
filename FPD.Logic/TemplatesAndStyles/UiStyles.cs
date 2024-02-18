using System.Windows.Media;

using Effanville.Common.UI.ViewModelBases;

namespace FPD.Logic.TemplatesAndStyles
{
    /// <summary>
    /// Contains styling info for the UI.
    /// </summary>
    public sealed class UiStyles : PropertyChangedBase
    {
        public bool IsLightTheme { get; private set; }

        private Brush _backgroundBrush;
        /// <summary>
        /// A brush for the primary background colour.
        /// </summary>
        public Brush BackgroundBrush
        {
            get => _backgroundBrush;
            set => SetAndNotify(ref _backgroundBrush, value);
        }

        private Brush _backgroundSecondaryBrush;
        /// <summary>
        /// A brush for secondary colours.
        /// </summary>
        public Brush BackgroundSecondaryBrush
        {
            get => _backgroundSecondaryBrush;
            set => SetAndNotify(ref _backgroundSecondaryBrush, value);
        }

        private Brush _highlightBrush;
        /// <summary>
        /// A brush for any highlights.
        /// </summary>
        public Brush HighlightBrush
        {
            get => _highlightBrush;
            set => SetAndNotify(ref _highlightBrush, value);
        }

        private Brush _defaultButtonBrush;
        /// <summary>
        /// A brush to be used on buttons.
        /// </summary>
        public Brush DefaultButtonBrush
        {
            get => _defaultButtonBrush;
            set => SetAndNotify(ref _defaultButtonBrush, value);
        }

        private Brush _foregroundBrush;
        /// <summary>
        /// The colour for foreground objects, eg text.
        /// </summary>
        public Brush ForegroundBrush
        {
            get => _foregroundBrush;
            set => SetAndNotify(ref _foregroundBrush, value);
        }

        Brush _chartLineColour1;
        public Brush ChartLineColour1
        {
            get => _chartLineColour1;
            set => SetAndNotify(ref _chartLineColour1, value);
        }
        Brush _chartLineColour2;
        public Brush ChartLineColour2
        {
            get => _chartLineColour2;
            set => SetAndNotify(ref _chartLineColour2, value);
        }

        Brush _chartLineColour3;
        public Brush ChartLineColour3
        {
            get => _chartLineColour3;
            set => SetAndNotify(ref _chartLineColour3, value);
        }

        Brush _chartLineColour4;
        public Brush ChartLineColour4
        {
            get => _chartLineColour4;
            set => SetAndNotify(ref _chartLineColour4, value);
        }

        Brush _chartLineColour5;
        public Brush ChartLineColour5
        {
            get => _chartLineColour5;
            set => SetAndNotify(ref _chartLineColour5, value);
        }
        /// <summary>
        /// The font family for all font display.
        /// </summary>
        public FontFamily FontFamily
        {
            get;
            set;
        }

        /// <summary>
        /// A small font size to use.
        /// </summary>
        public int SmallFontSize
        {
            get;
            set;
        } = 11;

        /// <summary>
        /// An intermediary font size.
        /// </summary>
        public int MediumFontSize
        {
            get;
            set;
        } = 12;

        /// <summary>
        /// The size of font to use in main headers.
        /// </summary>
        public int LargeFontSize
        {
            get;
            set;
        } = 14;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public UiStyles(bool isLightTheme)
        {
            IsLightTheme = isLightTheme;
            FontFamily = new FontFamily("Century Gothic");
            SetColours();
        }

        public void UpdateTheme(bool isLightTheme)
        {
            IsLightTheme = isLightTheme;
            SetColours();
        }

        private void SetColours()
        {
            if (IsLightTheme)
            {
                BackgroundBrush = LightThemeColours.BackgroundBrush;
                BackgroundSecondaryBrush = LightThemeColours.BackgroundSecondaryBrush;
                HighlightBrush = LightThemeColours.HighlightBrush;
                DefaultButtonBrush = LightThemeColours.DefaultButtonBrush;
                ForegroundBrush = LightThemeColours.ForegroundBrush;
                ChartLineColour1 = LightThemeColours.ChartLine1Brush;
                ChartLineColour2 = LightThemeColours.ChartLine2Brush;
                ChartLineColour3 = LightThemeColours.ChartLine3Brush;
                ChartLineColour4 = LightThemeColours.ChartLine4Brush;
                ChartLineColour5 = LightThemeColours.ChartLine5Brush;
            }
            else
            {
                BackgroundBrush = DarkThemeColours.BackgroundBrush;
                BackgroundSecondaryBrush = DarkThemeColours.BackgroundSecondaryBrush;
                HighlightBrush = DarkThemeColours.HighlightBrush;
                DefaultButtonBrush = DarkThemeColours.DefaultButtonBrush;
                ForegroundBrush = DarkThemeColours.ForegroundBrush;
                ChartLineColour1 = DarkThemeColours.ChartLine1Brush;
                ChartLineColour2 = DarkThemeColours.ChartLine2Brush;
                ChartLineColour3 = DarkThemeColours.ChartLine3Brush;
                ChartLineColour4 = DarkThemeColours.ChartLine4Brush;
                ChartLineColour5 = DarkThemeColours.ChartLine5Brush;
            }
        }
    }
}
