using System.Windows.Media;

namespace GuiSupport
{
    public static class Colours
    {
        public static Color BackgroundPink { get; set; } = Colors.LightPink;

        private static Brush fBackground = new SolidColorBrush(Colors.LightPink);
        public static Brush BackgroundBrush 
        { 
            get { return fBackground; }
            set { fBackground = value; }
        }


        private static Brush fBackgroundSecondary = (SolidColorBrush)(new BrushConverter().ConvertFrom("#F98866"));
        public static Brush BackgroundSecondaryBrush
        {
            get { return fBackgroundSecondary; }
            set { fBackgroundSecondary = value; }
        }
    }
}
