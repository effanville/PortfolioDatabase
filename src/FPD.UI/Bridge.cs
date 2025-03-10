﻿using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Effanville.FPD.Logic.TemplatesAndStyles;

namespace Effanville.FPD.UI
{
    public sealed class Bridge : FrameworkElement, INotifyPropertyChanged
    {
        /// <summary>
        /// Event handler controlling property changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        public static readonly DependencyProperty StylesProperty 
            = DependencyProperty.Register(nameof(Styles), typeof(IUiStyles), typeof(Bridge));

        public IUiStyles Styles
        {
            get => (IUiStyles)GetValue(StylesProperty);
            set
            {
                SetValue(StylesProperty, value);
                OnPropertyChanged(nameof(Styles));
            }
        }

        /// <summary>
        /// invokes the event for property changed.
        /// </summary>
        private void OnPropertyChanged(string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

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
            if (Equals(existingValue, newValue))
            {
                return;
            }

            existingValue = newValue;
            OnPropertyChanged(propertyName);
        }
    }
}
