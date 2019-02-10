using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace HDT.Wpf
{
    /// <summary>
    /// A base value converter that allows direct XAML usage
    /// </summary>
    /// <typeparam name="T">Value converter type</typeparam>
    public abstract class BaseValueConverter<T> : MarkupExtension, IValueConverter
        where T : class, new()
    {
        #region Private Members
        /// <summary>
        /// Single static instance of this value conveter
        /// </summary>
        private static T mConverter = null;

        #endregion
        #region Markup Extension Methods
        /// <summary>
        /// Returns a static instance of the value converter
        /// </summary>
        /// <param name="serviceProvider">The service provider</param>
        /// <returns></returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return mConverter ?? (mConverter = new T());
        } 
        #endregion

        #region Value Converter Methods

        /// <summary>
        /// Converts one type to another
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);

        /// <summary>
        /// Convetrs back to it's original source type
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public abstract object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture);

        #endregion
    }
}
