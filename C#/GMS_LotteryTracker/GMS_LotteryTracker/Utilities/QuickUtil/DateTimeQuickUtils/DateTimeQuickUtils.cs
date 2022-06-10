using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GMS_LotteryTracker.Utilities.QuickUtil.DateTimeQuickUtils
{

    //wrapper structure for referrence return of am/pm
    public struct AMPMReturn
    {
        public bool isPM = true;
    };


    public class DateTimeQuickUtils
    {

        //function to know if date time is AM/PM
        public static bool isDateTimeAM(DateTime dt)
        {
            if (dt.Hour > 12)
                return false;
            else return true;
        }

        //Function to convert the hr_12(hour in 12 hr format), to 24 hr
        //hr_12 = input of the hour in 12 hour format
        //isPm = input specifying if its PM / AM
        //returns the converted hour as int
        public static int convertHourTo24(int hr_12, bool isPM)
        {
            int hr = hr_12;
            if (isPM == true)
            {
                if (hr < 12)
                {
                    hr = +12;
                }
            }
            else
            {
                if (hr == 12)
                {
                    hr = 00;
                }
            }
            return hr;
        }

        //function to convert a given hour in 24 format to 12 hr format
        //hr_24 = input hour in 24 format
        //isPm = wrapper object containing output, if its PM / AM
        //returns the converted hour as int
        public static int convertHourTo12(int hr_24, AMPMReturn isPM)
        {
            if (hr_24 == 00)
            {
                isPM.isPM = false;
                return 12;
            }
            else if (hr_24 > 12)
            {
                isPM.isPM = true;
                return hr_24 - 12;
            }

            isPM.isPM = false;
            return hr_24;
        }
        
        public static string getDateTimeTo24String(DateTime dt)
        {
            return dt.ToString("dd-MM-yyyy HH:mm:ss");
        }


        public static string getDateTimeTo12String(DateTime dt)
        {
            return dt.ToString("dd-MM-yyyy hh:mm:ss tt");
        }


        //function to make a date time into a convert to datetime statement for sql queries
        public static string makeSqlDateStatement(DateTime dt)
        {
            string dateString = "convert(datetime, '" +
                ((dt.Date.Year.ToString().Length < 2) ? "0" + dt.Date.Year.ToString() : dt.Date.Year.ToString()) + "-" +
                ((dt.Date.Month.ToString().Length < 2) ? "0" + dt.Date.Month.ToString() : dt.Date.Month.ToString()) + "-" +
                ((dt.Date.Day.ToString().Length < 2) ? "0" + dt.Date.Day.ToString() : dt.Date.Day.ToString()) + " " +
                ((dt.TimeOfDay.Hours.ToString().Length < 2) ? "0" + dt.TimeOfDay.Hours.ToString() : dt.TimeOfDay.Hours.ToString()) + ":" +
                ((dt.TimeOfDay.Minutes.ToString().Length < 2) ? "0" + dt.TimeOfDay.Minutes.ToString() : dt.TimeOfDay.Minutes.ToString()) + ":" +
                ((dt.TimeOfDay.Seconds.ToString().Length < 2) ? "0" + dt.TimeOfDay.Seconds.ToString() : dt.TimeOfDay.Seconds.ToString()) + "', 20)";
            return dateString;
        }

    }
}
