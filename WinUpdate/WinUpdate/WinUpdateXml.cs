using System;
using System.Net;
using System.Xml;

namespace WinUpdate
{
    /// <summary>
    /// Contains update meta data
    /// </summary>
    internal class WinUpdateXml
    {
        /// <summary>
        /// Creates a new AppUpdateXml object
        /// </summary>
        internal WinUpdateXml(Version version, Uri uri, string fileName, string md5, string description, string launchArgs)
        {
            Version = version;
            Uri = uri;
            FileName = fileName;
            MD5 = md5;
            Description = description;
            LaunchArgs = launchArgs;

        }
        /// <summary>
        /// Checks if update's version is newer than the old version
        /// </summary>
        /// <param name="version">Application's current version</param>
        /// <returns>If the update's version # is newer</returns>
        internal bool IsNewerThan(Version version) => Version > version;
        /// <summary>
        /// Checks the uri to make sure file exists
        /// </summary>
        /// <param name="location">The uri of the update.xml</param>
        /// <returns></returns>
        internal static bool ExistOnServer(Uri location)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(location.AbsoluteUri);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                response.Close();
                return response.StatusCode == HttpStatusCode.OK;
            }
            catch 
            {

                return false;
            }
        }
        /// <summary>
        /// Parses the update.xml into AppUpdateXml object
        /// </summary>
        /// <param name="location">The uri of update.xml on the server</param>
        /// <param name="appId">The application Id</param>
        /// <returns>The AppUpdateXml with the data or null if any errors s</returns>
        internal static WinUpdateXml Parse(Uri location,string appId)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(location.AbsoluteUri);
                XmlNode node = doc.DocumentElement.SelectSingleNode("//update[@appId='" + appId + "']");
                if (node == null) return null;

                Version version = Version.Parse(node["version"].InnerText);
                string url = node["url"].InnerText;
                string fileName = node["fileName"].InnerText;
                string md5 = node["md5"].InnerText;
                string description = node["description"].InnerText;
                string launchArgs = node["launchArgs"].InnerText;

                return new WinUpdateXml(version, new Uri(url), fileName, md5, description, launchArgs);

            }
            catch
            {

                return null;
            }
        }
        /// <summary>
        /// The update version
        /// </summary>
        internal Version Version { get; }
        /// <summary>
        /// The location of the update binary
        /// </summary>
        internal Uri Uri { get; }
        /// <summary>
        /// The file name of the binary (exe)
        /// for use on local computer
        /// </summary>
        internal string FileName { get; set; }
        /// <summary>
        /// The MD5 of the update's binary
        /// </summary>
        internal string MD5 { get; set; }
        /// <summary>
        /// The update description
        /// </summary>
        internal string Description { get; set; }
        /// <summary>
        /// The arguments to pass to the updated application on Startup
        /// </summary>
        internal string LaunchArgs { get; set; }

    }
}
