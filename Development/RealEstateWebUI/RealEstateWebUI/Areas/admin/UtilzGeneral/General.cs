using RealEstateWebUI.Areas.admin.Models;
using RealEstateWebUI.Areas.admin.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Reflection;
using System.Web;

namespace RealEstateWebUI.Areas.admin.UtilzGeneral
{
    public class General
    {
        /// <summary>
        /// get description of enum value
        /// </summary>
        /// <param name="enumValue">enum value</param>
        /// <returns>description</returns>
        public static string GetEnumDescription(Enum enumValue, string enumDefault = "")
        {
            try
            {
                string enumValueAsString = enumValue.ToString();

                var type = enumValue.GetType();
                FieldInfo fieldInfo = type.GetField(enumValueAsString);
                object[] attributes = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attributes != null && attributes.Length > 0)
                {
                    var attribute = (DescriptionAttribute)attributes[0];
                    return attribute.Description;
                }

                return enumValueAsString;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return enumDefault;
            }
        }

        /// <summary>
        /// get ip address of client
        /// </summary>
        /// <returns>ip address</returns>
        public static string GetIPAddress()
        {
            try
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return ip.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
            }
            return "";
        }

        /// <summary>
        /// send a email
        /// </summary>
        /// <param name="mailTo">address email will be sent</param>
        /// <param name="subject">subject of email</param>
        /// <param name="body">body of email</param>
        /// <returns>result of sendmail: true if success, else false</returns>
        public static bool SendEmail(string mailTo, string subject, string body)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

                mail.From = new MailAddress(Common.Email);
                mail.To.Add(mailTo);
                mail.Subject = subject;
                mail.Body = body;

                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential(Common.CredentialUserName, Common.CredentialPassword);
                SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);
                return true;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return false;
            }
        }

        
    }
}