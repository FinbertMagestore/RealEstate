using System;
using System.Globalization;
using System.Linq;

namespace RealEstateWebUI.Areas.admin.UtilzGeneral
{
    public class SString
    {
        public static string ConverToString(object value, string strDefault = "")
        {
            try
            {
                return value.ToString();
            }
            catch
            {
                return strDefault;
            }
        }

        /// <summary>
        /// remove all character is strRemove at begin and end of string value
        /// </summary>
        /// <param name="value">string will be removed</param>
        /// <param name="strRemove">character will be remove</param>
        /// <returns>string after remove characters at begin and end of string value</returns>
        public static string RemoveElementAtBeginEnd(string value, string strRemove)
        {
            try
            {
                if (!string.IsNullOrEmpty(value))
                {
                    if (value.StartsWith(strRemove))
                    {
                        value = value.Substring(1, value.Length);
                    }
                    if (value.EndsWith(strRemove))
                    {
                        value = value.Substring(0, value.Length - 1);
                    }
                }
                return value;
            }
            catch 
            {
                return "";
            }
        }

        /// <summary>
        /// generate string random with length input
        /// </summary>
        /// <param name="length">length of string output</param>
        /// <returns>random string</returns>
        public static string RandomString(int length)
        {
            Random random = new Random();
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string FormatMoneyVN(object value)
        {
            try
            {
                if (SNumber.ToDouble(value) > 0)
                {
                    CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");   // try with "en-US"
                    string temp = double.Parse(value.ToString()).ToString("#,###", cul.NumberFormat);
                    return temp;
                }
                return "0";
            }
            catch
            {
                return "0";
                throw;
            }
        }
    }
}