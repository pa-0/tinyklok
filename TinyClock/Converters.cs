/*
  TinyKlok 
  Copyright (C) 2013 Marko Devcic
   
  This program is free software: you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation <http://www.gnu.org/licenses/>.
  
  This application comes without WARRANTY WHATSOEVER!
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace TinyKlok
{
    public class Converters : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool dark = (bool)value;
            if (dark) return "Dark";
            else return "Light";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((value as string) == "Dark") return true;
            else return false;
        }
    }
}
