using System.Windows.Media;

namespace Effanville.FPD.UI.TemplatesAndStyles
{
    /// <summary>
    /// Stores the colours and brushes used in the program.
    /// </summary>
    public static class DarkThemeColours
    {
        /// <summary>
        /// A brush to be used for background.
        /// </summary>
        public static Brush BackgroundBrush
        {
            get;
            set;
        } = new SolidColorBrush(Color.FromRgb(30, 30, 30));

        /// <summary>
        /// A brush for secondary background colours.
        /// </summary>
        public static Brush BackgroundSecondaryBrush
        {
            get;
            set;
        } = new SolidColorBrush(Color.FromRgb(60, 60, 60));

        /// <summary>
        /// A brush with the colour for buttons.
        /// </summary>
        public static Brush DefaultButtonBrush
        {
            get;
            set;
        } = new SolidColorBrush(Color.FromRgb(100, 100, 100));

        /// <summary>
        /// A brush for any highlights.
        /// </summary>
        public static Brush HighlightBrush
        {
            get;
            set;
        } = new SolidColorBrush(Colors.Gray);

        /// <summary>
        /// A brush for any highlights.
        /// </summary>
        public static Brush ForegroundBrush
        {
            get;
            set;
        } = new SolidColorBrush(Colors.White);

        public static Brush ChartLine1Brush
        {
            get;
        } = new SolidColorBrush(Colors.LightBlue);

        public static Brush ChartLine2Brush
        {
            get;
        } = new SolidColorBrush(Colors.LightPink);

        public static Brush ChartLine3Brush
        {
            get;
        } = new SolidColorBrush(Colors.LightGreen);

        public static Brush ChartLine4Brush
        {
            get;
        } = new SolidColorBrush(Colors.Orange);

        public static Brush ChartLine5Brush
        {
            get;
        } = new SolidColorBrush(Colors.Purple);
    }
}
