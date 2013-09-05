using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Data;
using System.Xml;
using System.Xml.Linq;
using PhotoCtmService.PhotoCtmDownload;
using System.Configuration;

namespace PhotoCtmService
{
    class PhotoCTMDownloader
    {
        string xmlPath = System.Configuration.ConfigurationManager.AppSettings["XMLFolderPath"].ToString() + "\\Images.xml";

        public void DownloadImages()
        {
            Int32 year, date, month, pid, eid;
            month = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["month"].ToString());
            year = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["year"].ToString());
            date = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["date"].ToString());
            eid = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["eid"].ToString());
            pid = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["PID"].ToString());
            
            DataTable dtlServiceResponse = new DataTable();
            int resultCount;
            Dictionary<double, string> imageDictionary;
            utlSoapClient objSoapProxy = new utlSoapClient();

            try
            {
                dtlServiceResponse = objSoapProxy.geturlandphotoidtable(eid, new DateTime(year, month, date), pid);
            }
            catch (Exception)
            {

                throw;
            }


            resultCount = dtlServiceResponse.Rows.Count;


            if (resultCount > 0)
            {
                imageDictionary = new Dictionary<double, string>();
                for (int i = 0; i < resultCount; i++)
                {
                    imageDictionary.Add(Convert.ToInt64(dtlServiceResponse.Rows[i].ItemArray[0]),
                                        dtlServiceResponse.Rows[i].ItemArray[1].ToString());
                }

                var list = imageDictionary.Keys.ToList();
             list.Sort();
                if (GetLastPID() < list[resultCount - 1])
                {
                    DownloadImages(imageDictionary);
                }

                
            }
        }

        private void DownloadImages(Dictionary<double, string> dictImages)
        {

            if (dictImages != null && dictImages.Count > 0)
            {
                foreach (var pair in dictImages.OrderBy(i => i.Key))
                {
                    string url = pair.Value;
                    string filenamepath = string.Concat(ConfigurationManager.AppSettings["DownloadImagesFolderPath"].ToString() + "\\", pair.Key, ".jpg");
                    // Create an instance of WebClient
                    WebClient client = new WebClient();

                    // Start the download and copy the file to filepath
                    //client.DownloadFile(url, filenamepath);
                    byte[] data = client.DownloadData(url);

                    File.WriteAllBytes(filenamepath, data);
                    WriteToXML(pair.Key);
                }
            }
        }
                
        private void WriteToXML(double latestPID)
        {

            if (File.Exists(xmlPath))
            {
                if (GetLastPID() < latestPID)
                {
                    XmlDocument xmlDoc = new XmlDocument();

                    xmlDoc.Load(xmlPath);
                    XmlNode node = xmlDoc.SelectSingleNode("/LastPID/PID");
                    node.InnerText = latestPID.ToString();
                    xmlDoc.Save(xmlPath);
                }


            }
            else
            {
                //First Time Write Operation
                using (XmlWriter writer = XmlWriter.Create(xmlPath))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("LastPID");
                    writer.WriteElementString("PID", latestPID.ToString());
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
            }
        }

        private double GetLastPID()
        {

            if (File.Exists(xmlPath))
            {
                XElement root = XElement.Load(xmlPath);
                IEnumerable<XElement> pidSet = from el in root.Elements("PID")
                                               select el;
                Int64 pid = Convert.ToInt64(pidSet.ElementAt(0).Value);

                return pid;
            }
            else
            {

                return 0;
            }
        }

    }
}

