using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using Models;

namespace MindstormController.Helpers
{
    public class RedToGreenScaleConverter : IValueConverter
    {

        // Energy Threshold from Resting to Moving;
        private double ET_RestingToStartMoving { get; set; }

        // Energy Threshold from Resting to Moving;
        private double ET_StartMovingToMoving { get; set; }


        #region IValueConverter Members  
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ET_RestingToStartMoving = 0.1;
            ET_StartMovingToMoving = 0.2;

            return new SolidColorBrush(ColorFromEnergy((double)value));
        }
  
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)    {
            return null;
        }

        private Color ColorFromEnergy(double value)
        {
            if (value < ET_RestingToStartMoving)
                return Colors.Red;
            else if (value < ET_StartMovingToMoving)
                return Colors.Orange;
            else
                return Colors.Green;
        }
        #endregion
    }   
}
