using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

using FPD.Logic.TemplatesAndStyles;

namespace FPD.UI
{
    public sealed class Bridge : FrameworkElement, INotifyPropertyChanged
    {
        /// <summary>
        /// Event handler controlling property changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        public static readonly DependencyProperty StylesProperty = DependencyProperty.Register("Styles", typeof(UiStyles), typeof(Bridge));

        public UiStyles Styles
        {
            get => (UiStyles)GetValue(StylesProperty);
            set
            {
                SetValue(StylesProperty, value);
                OnPropertyChanged(nameof(Styles));
            }
        }

        public Bridge()
        {
        }

        /// <summary>
        /// invokes the event for property changed.
        /// </summary>
        public void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Updates the existing value if different from the new value
        /// and raises OnPropertyChanged if so.
        /// </summary>
        /// <typeparam name="T">The type of the values.</typeparam>
        /// <param name="existingValue">The existing value.</param>
        /// <param name="newValue">The new value to update with.</param>
        /// <param name="propertyName">The name of the property being changed.</param>
        public void SetAndNotify<T>(ref T existingValue, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (!Equals(existingValue, newValue))
            {
                existingValue = newValue;
                OnPropertyChanged(propertyName);
            }
        }
    }
}
