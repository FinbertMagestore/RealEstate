using System;
using System.Globalization;

namespace RealEstateWebUI.Areas.admin.UtilzGeneral
{
    public class SDateTime
    {
        #region property
        /// <summary>
        /// Ex:05/03/2016 1:19:14 PM [tt] = PM
        /// </summary>
        public static string[] FormatGetAMorPM = { "tt" };
        /// <summary>
        /// Ex:05/03/2016 1:19:14 PM [s] = 14
        /// </summary>
        public static string[] FormatSecond = { "s" };
        /// <summary>
        /// Ex:05/03/2016 1:19:14 PM [mm] = 19
        /// </summary>
        public static string[] FormatMinute = { "mm" };
        /// <summary>
        /// Ex:05/03/2016 1:19:14 PM [HH] = 13, [hh] = 01
        /// </summary>
        public static string[] FormatHour = { "HH", "hh" };
        /// <summary>
        /// Ex:05/03/2016 Saturday: [d] = 05/03/2016, [dd] = 05, [ddd] = Sat, [dddd] = Saturday
        /// </summary>
        public static string[] FormatDay = { "d", "dd", "ddd", "dddd" };
        /// <summary>
        /// Ex:05/03/2016: [M] = March 05, [MM] = 03; [MMM] = Mar; [MMMM] = March;
        /// </summary>
        public static string[] FormatMonth = { "M", "MM", "MMM", "MMMM" };
        /// <summary>
        /// Ex:05/03/2016: [y] = March 2016, [yy] = 16, [yyy] = [yyyy] = 2016
        /// </summary>
        public static string[] FormatYear = { "y", "yy", "yyy", "yyyy" };

        public String DB_DATE_FORMAT = "YYYYmmDD";
        public String PARSE_DATE_FORMAT = "DD/mm/YYYY";
        public String DB_DATETIME_FORMAT = "yyyy-MM-dd HH:mm";
        #endregion

        #region timestamp
        /// <summary>
        /// get total seconds from 01/01/1970 00:00:00 to now
        /// </summary>
        /// <returns></returns>
        public static long GetTimeStampToNow()
        {
            try
            {
                long result = 0;
                DateTime now = DateTime.Now;
                DateTime past = new DateTime(1970, 1, 1, 0, 0, 0);
                result = (long)(now - past).TotalSeconds;
                return result;
            }
            catch (Exception)
            {
                return 0;
                //throw;
            }
        }
        /// <summary>
        /// get total seconds from 01/01/1970 00:00:00 to a datetime
        /// </summary>
        /// <returns></returns>
        public static long GetTimeStamp(DateTime dateTime)
        {
            try
            {
                long result = 0;
                DateTime past = new DateTime(1970, 1, 1, 0, 0, 0);
                result = (long)(dateTime - past).TotalSeconds;
                return result;
            }
            catch (Exception)
            {
                return 0;
                //throw;
            }
        }
        public static DateTime GetDateTimeFromTimeStamp(long timeStamp)
        {
            try
            {
                DateTime past = new DateTime(1970, 1, 1, 0, 0, 0);
                DateTime result = past.AddSeconds(timeStamp);
                return result;
            }
            catch (Exception)
            {
                return DateTime.Now;
                //throw;
            }
        }

        /// <summary>
        /// get time span from time
        /// </summary>
        /// <param name="time">13:53</param>
        /// <returns></returns>
        public static TimeSpan GetTimeSpanFromTime(string time)
        {
            try
            {
                string[] temp = time.Split(':');
                return new TimeSpan(SNumber.ToNumber(temp[0]), SNumber.ToNumber(temp[1]), 0);
            }
            catch (Exception)
            {
                return new TimeSpan(0, 0, 0);
                //throw;
            }
        }
        /// <summary>
        /// check time validate format: hh:mm. ex: 14:57
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static bool IsTime(string time)
        {
            try
            {
                string[] temp = time.Split(':');
                if (temp.Length == 2)
                {
                    if (SNumber.IsNumber(temp[0]) && SNumber.IsNumber(temp[1]))
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception)
            {
                return false;
                //throw;
            }
        }
        #endregion

        #region Date time
        public static string GetYYYYMMddHmmSSNow()
        {
            try
            {
                DateTime today = DateTime.Today;
                string dateYYYMMDD = ToDateYYYYmmDD(today.ToString("dd/MM/yyyy"));
                string timeHHMM = DateTime.Now.ToString("HHmmss");
                return dateYYYMMDD + timeHHMM;
            }
            catch
            {
                return "";
            }
        }
        /// <summary>
        /// get date time now at format: dd/MM/YYYY hh:mm.ss
        /// </summary>
        /// <returns>date time now</returns>
        public static string GetDateTimeNow()
        {
            try
            {
                DateTime today = DateTime.Today;
                string dateTimeNow = today.ToString("dd/MM/yyyy") + " " + DateTime.Now.ToString("hh:mm.ss");
                return dateTimeNow;
            }
            catch
            {
                return "";
            }
        }
        /// <summary>
        /// check value at format YYYYmmDDhhMMss
        /// </summary>
        /// <param name="value">value at format dd/mm/yyyy hh:mm.ss</param>
        /// <returns></returns>
        public static bool IsDateTime(String value)
        {
            try
            {
                if (!string.IsNullOrEmpty(ToDateTime(value)))
                {
                    return true;
                }
            }
            catch (Exception)
            {
            }
            return false;
        }
        /// <summary>
        /// convert value from yyyyDDmmHHmmSS to mm/DD/yyy hh:mm.ss
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToDateTime(string value)
        {
            try
            {
                string date = value.Substring(0, 8);
                string time = value.Substring(8, 6);
                DateTime dt = DateTime.ParseExact(date, "yyyyMMdd",
                                  CultureInfo.InvariantCulture);
                string t = time.Substring(0, 2) + ":" + time.Substring(2, 2) + "." + time.Substring(4, 2);
                return dt.ToString("dd/MM/yyyy") + " " + t;
            }
            catch (Exception)
            {
                return "";
            }
        }
        #endregion

        #region Date
        public static string GetDateYYYYmmDDNow()
        {
            try
            {
                DateTime today = DateTime.Today;
                string dateYYYMMDD = today.ToString("yyyyMMdd");
                return dateYYYMMDD ;
            }
            catch
            {
                return "";
            }
        }
        public static bool IsDateYYYYmmDD(String value)
        {
            try
            {
                bool result = false;
                if (string.IsNullOrEmpty(value))
                {
                    return false;
                }
                if (value.Length != 8)
                {
                    return false;
                }
                String YYYY = value.Substring(0, 4);
                String mm = value.Substring(4, 2);
                String DD = value.Substring(6, 2);
                if (SNumber.IsNumber(YYYY))
                {
                    if (SNumber.ToNumber(YYYY) > 1000)
                    {
                        if (SNumber.ToNumber(YYYY) < 9999)
                        {
                            if (SNumber.IsNumber(mm))
                            {
                                if (SNumber.ToNumber(mm) > 0)
                                {
                                    if (SNumber.ToNumber(mm) < 13)
                                    {
                                        if (SNumber.IsNumber(DD))
                                        {
                                            if ("01".Equals(mm) || "03".Equals(mm) || "05".Equals(mm)
                                                    || "07".Equals(mm) || "08".Equals(mm) || "10".Equals(mm)
                                                    || "12".Equals(mm))
                                            {
                                                if (SNumber.ToNumber(DD) > 0)
                                                {
                                                    if (SNumber.ToNumber(DD) < 32)
                                                    {
                                                        result = true;
                                                    }
                                                }
                                            }
                                            else if (SNumber.ToNumber(DD) > 0)
                                            {
                                                if (SNumber.ToNumber(DD) < 31)
                                                {
                                                    result = true;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return result;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// check a string has format is 'DD/mm/YYYY' kiểm tra value có ở dạng 'DD/mm/YYYY'
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsDateDDmmYYYY(String value)
        {
            try
            {
                bool result = false;
                if (string.IsNullOrEmpty(value))
                {
                    return false;
                }
                if (value.Length != 10)
                {
                    return false;
                }
                String YYYY = value.Substring(6, 4);
                String mm = value.Substring(3, 2);
                String DD = value.Substring(0, 2);
                if (SNumber.IsNumber(YYYY))
                {
                    if (SNumber.ToNumber(YYYY) > 1000)
                    {
                        if (SNumber.ToNumber(YYYY) < 9999)
                        {
                            if (SNumber.IsNumber(mm))
                            {
                                if (SNumber.ToNumber(mm) > 0)
                                {
                                    if (SNumber.ToNumber(mm) < 13)
                                    {
                                        if (SNumber.IsNumber(DD))
                                        {
                                            if (SNumber.ToNumber(DD) > 0)
                                            {
                                                if ("12".Equals(mm)
                                                        || "01".Equals(mm) || "03".Equals(mm) || "05".Equals(mm)
                                                        || "07".Equals(mm) || "08".Equals(mm) || "10".Equals(mm))
                                                {
                                                    if (SNumber.ToNumber(DD) < 32)
                                                    {
                                                        result = true;
                                                    }

                                                }
                                                else if ("02".Equals(mm))
                                                {
                                                    if (SNumber.ToNumber(YYYY) % 4 == 0)
                                                    {
                                                        if (SNumber.ToNumber(DD) < 30)
                                                        {
                                                            result = true;
                                                        }
                                                    }
                                                    else if (SNumber.ToNumber(DD) < 29)
                                                    {
                                                        result = true;
                                                    }
                                                }
                                                else if (SNumber.ToNumber(DD) < 31)
                                                {
                                                    result = true;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return result;
            }
            catch (Exception)
            {
                return false;
            }
        }
        /// <summary>
        /// change from dd/mm/yyyy to yyyyMMdd
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static String ToDateYYYYmmDD(String value)
        {
            try
            {
                String result = "";
                if (string.IsNullOrEmpty(value))
                {
                    return "";
                }
                if (IsDateDDmmYYYY(value))
                {
                    return value.Substring(6, 4) + value.Substring(3, 2) + value.Substring(0, 2);
                }
                return result;
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// convert a string from 'YYYYmmDD' to format 'DD/mm/YYYY'
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static String ToDateDDmmYYYY(String value)
        {
            try
            {
                String result = "";
                if (string.IsNullOrEmpty(value))
                {
                    return null;
                }
                if (IsDateYYYYmmDD(value))
                {
                    result = value.Substring(6, 2) + "/" + value.Substring(4, 2) + "/" + value.Substring(0, 4);
                }
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        #region Time
        /// <summary>
        /// convert time from format hh:mm.ss to hhmmss
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToTimeHHmmSS(string value)
        {
            try
            {
                return value.Substring(0, 2) + value.Substring(3, 2) + value.Substring(6, 2);
            }
            catch
            {
                return "";
            }
        }
        #endregion
    }
}
