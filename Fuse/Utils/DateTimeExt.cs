using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fuse.Utils
{
    internal static partial class DateTimeExt
    {
        internal static string ToFriendlyFormat(this DateTime dt)
        {
            DateTime now = DateTime.Now;

            if (now.Date == dt.Date) //same day
                return $"Today at {dt.ToString("HH:mm")}";
            else if ((now - dt).TotalDays == 1) //day before
                return $"Yesterday at {dt.ToString("HH:mm")}";
            else if ((now - dt).TotalDays <= 7) //same week
                return $"Last {dt.ToString("dddd")} at {dt.ToString("HH:mm")}";
            else
            {
                if (dt.Date.Year != now.Date.Year) //older year
                    return dt.Date.ToString("dd/MM/yyyy");
                else //same year
                    return dt.Date.ToString("dd/MM");
            }
        }
    }
}
