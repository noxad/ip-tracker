using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.IO;

namespace IpTracker
{
    internal class Program
    {
        private static void Main()
        {
            var ipLookupServiceUrl = ConfigurationManager.AppSettings["IpLookupServiceUrl"];
            var currentIpFile = new FileInfo(ConfigurationManager.AppSettings["CurrentIpFilePathAndName"]);
            var ipHistoryFile = new FileInfo(ConfigurationManager.AppSettings["IpHistoryFilePathAndName"]);
            var ignoredIpsStringList = ConfigurationManager.AppSettings["IgnoredIpsCsv"]?.Split(',');

            var ignoredIps = new List<IPAddress>();
            if (ignoredIpsStringList != null)
                ignoredIps.AddRange(ignoredIpsStringList.Select(IPAddress.Parse));

            CreateFiles(new List<FileInfo> { currentIpFile, ipHistoryFile });

            var currentIp = GetCurrentIp(ipLookupServiceUrl);
            var lastIp = GetLastRecordedIp(currentIpFile);

            if (currentIp.Equals(lastIp))
                return;

            if (ignoredIps.Contains(currentIp))
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
                        Directory.CreateDirectory(file.DirectoryName);
                }

                using (File.Create(file.FullName)) { }
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
                    throw new Exception($"Return value was not a valid IP address. Value returned was:{Environment.NewLine}{apiResult}");
            }

            return ip;
        }

        private static IPAddress GetLastRecordedIp(FileSystemInfo filePath)
        {
            return IPAddress.Parse(File.ReadAllText(filePath.FullName));
        }

        private static void UpdateIpTextFiles(IPAddress currentIp, string currentIpFile, string ipHistoryFile)
        {
            File.WriteAllText(currentIpFile, currentIp.ToString());

            var newIpLine = $"{DateTime.Now:yyyy-MM-dd}\t{DateTime.Now:HH:mm:ss}\t{currentIp}{Environment.NewLine}";

            File.AppendAllText(ipHistoryFile, newIpLine);
        }
    }
}
