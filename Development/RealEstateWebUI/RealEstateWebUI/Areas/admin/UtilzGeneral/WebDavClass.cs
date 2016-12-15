using RealEstateWebUI.Areas.admin.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

/// <summary>
/// Summary description for WebDavClass
/// </summary>

namespace RealEstateWebUI.Areas.admin.UtilzGeneral
{
    public static class WebDavClass
    {
        public static bool UploadFile(string localFile, string urlFileWebDav)
        {
            try
            {
                //string urlFile = SystemFrameWork.SystemConfig.GetExServerUrl() + urlFileWebDav;
                string urlFile = urlFileWebDav;
                FileStream rdr = new FileStream(localFile, FileMode.Open);
                byte[] inData = new byte[rdr.Length];
                int bytesRead = rdr.Read(inData, 0, int.Parse(rdr.Length.ToString()));
                rdr.Close();

                //upload to server (write to file in server) 
                using (WebClient client = new WebClient())
                {
                    client.Credentials = CredentialCache.DefaultCredentials;
                    client.Credentials = getWebDavCredential();
                    client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                    client.UploadData(urlFile, "PUT", inData);
                }
                return true;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return false;
            }
        }
        public static bool DownloadFile(string urlFile, string localFile)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(urlFile);
                request.Credentials = CredentialCache.DefaultCredentials;
                request.Method = "HEAD";
                var response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    WebClient client = new WebClient();
                    client.Credentials = CredentialCache.DefaultCredentials;
                    client.Credentials = getWebDavCredential();
                    client.DownloadFile(urlFile, localFile);

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return false;
            }
        }

        public static bool CreateFolder(string folderName)
        {
            bool status = true;
            try
            {
                string url = "";//SystemFrameWork.SystemConfig.GetExServerUrl();
                WebRequest request = WebRequest.Create(url + folderName);
                request.Credentials = CredentialCache.DefaultCredentials;
                request.Credentials = getWebDavCredential();
                request.Method = "MKCOL";
                WebResponse response = request.GetResponse();
                response.Close();
            }
            catch (WebException)
            {
                //LogService.WriteException(ex);
                status = false;
            }
            return status;
        }

        public static bool CreateFolder(string webdavUrl, string folderName)
        {
            bool status = true;
            try
            {
                WebRequest request = WebRequest.Create(webdavUrl + folderName);
                request.Credentials = CredentialCache.DefaultCredentials;
                request.Credentials = getWebDavCredential();
                request.Method = "MKCOL";
                WebResponse response = request.GetResponse();
                response.Close();
            }
            catch (WebException)
            {
                //LogService.WriteException(ex);
                status = false;
            }
            return status;
        }

        private static string m_UserID;
        private static string m_UserPass;
        public static void setWebDavCredential(string userNameWeb, string passwordWeb)
        {
            m_UserID = userNameWeb;
            m_UserPass = passwordWeb;
        }

        public static NetworkCredential getWebDavCredential()
        {
            //return new NetworkCredential(SystemConfig.GetUserNameWeb(),
            //                                                   SystemConfig.GetPasswordWeb());
            return new NetworkCredential(m_UserID, m_UserPass);
        }
        /// <summary>
        /// Create folder with Url
        /// </summary>
        /// <param name="strUrl"></param>
        /// <history>
        /// </history>
        public static void CreateFolderByUrl(string strUrl)
        {
            try
            {
                string[] strArrUrl = strUrl.Split('/');
                string strDataUpload = "";
                if (strUrl.Count() > 0)
                {
                    int intCountArr = strArrUrl.Count();
                    for (int intIndex = 0; intIndex < intCountArr; intIndex++)
                    {
                        if (!string.IsNullOrEmpty(strArrUrl[intIndex]) && strArrUrl[intIndex] != "~")
                        {
                            strDataUpload += "/" + strArrUrl[intIndex] + "/";
                            CreateFolder(strDataUpload);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
            }
        }

        public static bool DeleteFolder(string url, string folderName)
        {
            bool status = true;
            try
            {
                WebRequest request = WebRequest.Create(url + folderName);
                request.Credentials = CredentialCache.DefaultCredentials;
                request.Credentials = getWebDavCredential();
                request.Method = "DELETE";
                WebResponse response = request.GetResponse();
                response.Close();
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                status = false;
            }
            return status;
        }
        public static bool DeleteFiles(string url, string filesname)
        {
            bool status = true;
            try
            {
                WebRequest request = WebRequest.Create(url + filesname);
                request.Credentials = CredentialCache.DefaultCredentials;
                request.Credentials = getWebDavCredential();
                request.Method = "DELETE";
                WebResponse response = request.GetResponse();
                response.Close();
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                status = false;
            }
            return status;
        }
        public static string ReadFiles(string urlwithfiles)
        {
            string returnsvalues = string.Empty;
            try
            {
                WebRequest request = WebRequest.Create(urlwithfiles);

                request.Credentials = CredentialCache.DefaultCredentials;
                request.Credentials = getWebDavCredential();

                // Define the HTTP method.
                request.Method = @"GET";

                // Specify the request for source code.
                request.Headers.Add(@"Translate", "F");

                // Retrieve the response.
                WebResponse httpGetResponse1 = request.GetResponse();

                // Retrieve the response stream.
                Stream getResponseStream1 =
                   httpGetResponse1.GetResponseStream();

                // Create a stream reader for the response.
                StreamReader getStreamReader1 =
                   new StreamReader(getResponseStream1);
                returnsvalues = getStreamReader1.ReadToEnd();

                // Close the response streams.
                getStreamReader1.Close();
                getResponseStream1.Close();
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
            }
            return returnsvalues;
        }
    }
}