//-----------------------------------------------------------------------
// <copyright file="SelectedValueConverter.cs" company="Visemet">
//     Copyright (c) Visemet. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Visemet.Environment
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Markup;

    /// <summary>
    /// A value converter used for the selected value in a data grid.
    /// </summary>
    public class SelectedValueConverter : MarkupExtension, IValueConverter
    {
        /// <summary>
        /// Lazily defines a singleton.
        /// </summary>
        private static readonly Lazy<SelectedValueConverter> Converter =
            new Lazy<SelectedValueConverter>(() => new SelectedValueConverter());

        /// <summary>
        /// Prevents a default instance of the
        /// <see cref="SelectedValueConverter" /> class from being
        /// created.
        /// </summary>
        private SelectedValueConverter()
        {
        }

        /// <summary>
        /// Gets the singleton.
        /// </summary>
        public static SelectedValueConverter Instance
        {
            get
            {
                return Converter.Value;
            }
        }

        /// <summary>
        /// Returns an object that is provided as the value of the target
        /// property for this markup extension.
        /// </summary>
        /// <param name="serviceProvider">
        /// A service provider helper that can provide services for the
        /// markup extension.
        /// </param>
        /// <returns>
        /// The object value to set on the property where the extension
        /// is applied.
        /// </returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Instance;
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">
        /// The value produced by the binding source.
        /// </param>
        /// <param name="targetType">
        /// The type of the binding target property.
        /// </param>
        /// <param name="parameter">
        /// The converter parameter to use.
        /// </param>
        /// <param name="culture">
        /// The culture to use in the converter.
        /// </param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null
        /// value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">
        /// The value that is produced by the binding target.
        /// </param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">
        /// The converter parameter to use.
        /// </param>
        /// <param name="culture">
        /// The culture to use in the converter.
        /// </param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null
        /// value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
