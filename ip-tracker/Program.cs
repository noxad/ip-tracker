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
            var lastIp = GetLastRecordedIp(currentIpFile);

            if (currentIp.Equals(lastIp))
                return;

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

        private static IPAddress GetCurrentIp(string url)
        {
            IPAddress ip;

            using (var client = new WebClient())
            {
                client.Headers.Add("user-agent", "yourUserAgentHereBot");
                var apiResult = client.DownloadString(url);
                
                if (!IPAddress.TryParse(apiResult, out ip))
                {
                    throw new Exception(String.Format("Return value was not a valid IP address. Value returned was:{0}{1}", Environment.NewLine, apiResult));
                }
            }

            return ip;
        }

        private static IPAddress GetLastRecordedIp(FileInfo filePath)
        {
            IPAddress ip;

            var textFileContents = File.ReadAllText(filePath.FullName);

            if (!IPAddress.TryParse(textFileContents, out ip))
                ip = IPAddress.Parse("127.0.0.1");

            return ip;
        }

        private static void UpdateIpTextFiles(IPAddress currentIp, string currentIpFile, string ipHistoryFile)
        {
            File.WriteAllText(currentIpFile, currentIp.ToString());

            var newIpLine = String.Format("{0}\t{1}\t{2}{3}", 
                                                            DateTime.Now.ToString("yyyy-MM-dd"), 
                                                            DateTime.Now.ToString("HH:mm:ss"), 
                                                            currentIp, 
                                                            Environment.NewLine);

            File.AppendAllText(ipHistoryFile, newIpLine);
        }
    }
}
