using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace TopLearn.Core.Convertors
{
    public static class DateConvertor
    {
        public static string ToShamsi(this DateTime value)
        {
            PersianCalendar persian = new PersianCalendar();
            return persian.GetYear(value).ToString() + "/" + persian.GetMonth(value).ToString("00") + "/" +
                   persian.GetDayOfMonth(value).ToString("00");
        }
    }
}
