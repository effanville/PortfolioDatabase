using System.Windows.Media;

namespace Effanville.FPD.Logic.TemplatesAndStyles
{
    /// <summary>
    /// Stores the colours and brushes used in the program.
    /// </summary>
    public static class LightThemeColours
    {
        /// <summary>
        /// A brush to be used for background.
        /// </summary>
        public static Brush BackgroundBrush
        {
            get;
        } = new SolidColorBrush(Colors.AliceBlue);

        /// <summary>
        /// A brush for secondary background colours.
        /// </summary>
        public static Brush BackgroundSecondaryBrush
        {
            get;
        } = new SolidColorBrush(Colors.LightBlue);

        /// <summary>
        /// A brush with the colour for buttons.
        /// </summary>
        public static Brush DefaultButtonBrush
        {
            get;
        } = new SolidColorBrush(Colors.Lavender);

        /// <summary>
        /// A brush for any highlights.
        /// </summary>
        public static Brush HighlightBrush
        {
            get;
        } = new SolidColorBrush(Colors.DarkSlateBlue);

        /// <summary>
        /// A brush for any highlights.
        /// </summary>
        public static Brush ForegroundBrush
        {
            get;
        } = new SolidColorBrush(Colors.Black);

        public static Brush ChartLine1Brush
        {
            get;
        } = new SolidColorBrush(Colors.Blue);

        public static Brush ChartLine2Brush
        {
            get;
        } = new SolidColorBrush(Colors.Red);

        public static Brush ChartLine3Brush
        {
            get;
        } = new SolidColorBrush(Colors.Green);

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
