using System.Windows.Media;

namespace GuiSupport
{
    public static class Colours
    {
        public static Color BackgroundPink { get; set; } = Colors.LightPink;

        private static Brush fBackgroundBrush = new SolidColorBrush(Colors.LightPink);
        public static Brush BackgroundBrush 
        { 
            get { return fBackgroundBrush; }
            set { fBackgroundBrush = value; }
        }


        private static Brush fBackgroundSecondaryBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#F98866"));
        public static Brush BackgroundSecondaryBrush
        {
            get { return fBackgroundSecondaryBrush; }
            set { fBackgroundSecondaryBrush = value; }
        }

        private static Brush fDefaultButtonBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#F99966"));
        public static Brush DefaultTextBrush
        {
            get { return fDefaultButtonBrush; }
            set { fDefaultButtonBrush = value; }
        }
    }
}
