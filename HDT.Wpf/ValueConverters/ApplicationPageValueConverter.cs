using System;
using System.Diagnostics;
using System.Globalization;

namespace HDT.Wpf
{
    /// <summary>
    /// Converts the <see cref="ApplicaionPage"/> to an actual view/page
    /// </summary>
    public class ApplicationPageValueConverter : BaseValueConverter<ApplicationPageValueConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Find the appropriate page
            switch ((ApplicaionPage)value)
            {
                case ApplicaionPage.Home:
                    return new HomePage();
                case ApplicaionPage.Settings:
                    return new SettingsPage();
                default:
                    Debugger.Break();
                    return null;
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
