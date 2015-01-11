using System;
using System.ComponentModel.DataAnnotations;

namespace Internet_Banking.Models
{
    public class DateAttribute : RangeAttribute
    {
        public DateAttribute()
            : base(typeof(DateTime),
                    "1/1/1900",
                    DateTime.Now.AddYears(-18).ToShortDateString())
        { }
    }
}