using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.IO;

namespace IpTracker
{
    class Program
    {
        static void Main()
        {
            var ipLookupServiceUrl = ConfigurationManager.AppSettings["IpLookupServiceUrl"];
            var currentIpFile = new FileInfo(ConfigurationManager.AppSettings["CurrentIpFilePathAndName"]);
            var ipHistoryFile = new FileInfo(ConfigurationManager.AppSettings["IpHistoryFilePathAndName"]);

            CreateFiles(new List<FileInfo> { currentIpFile, ipHistoryFile });

            var currentIp = GetCurrentIp(ipLookupServiceUrl);
            var lastRecordedIp = File.ReadAllText(currentIpFile.FullName);

            if (currentIp == lastRecordedIp)
            {
                return;
            }

            UpdateIpTextFiles(currentIp, currentIpFile.FullName, ipHistoryFile.FullName);
        }

        private static void CreateFiles(IEnumerable<FileInfo> files)
        {
            foreach (var file in files.Where(file => !File.Exists(file.FullName)))
            {
                if (file.DirectoryName != null)
                {
                    if (!Directory.Exists(file.DirectoryName))
                    {
                        Directory.CreateDirectory(file.DirectoryName);
                    }
                }

                using (File.Create(file.FullName))
                {
                }
            }
        }

        private static string GetCurrentIp(string url)
        {
            string currentIp;

            using (var wb = new WebClient())
            {
                currentIp = wb.DownloadString(url);
            }

            return currentIp;
        }

        private static void UpdateIpTextFiles(string currentIp, string currentIpFile, string ipHistoryFile)
        {
            File.WriteAllText(currentIpFile, currentIp);

            var newIpLine = String.Format("{0}\t{1}\t{2}{3}", 
                DateTime.Now.ToString("yyyy-MM-dd"), 
                DateTime.Now.ToString("HH:mm:ss"), 
                currentIp, 
                Environment.NewLine);

            File.AppendAllText(ipHistoryFile, newIpLine);
        }
    }
}
