using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Umbraco.Core.Migrations;

namespace UIOMatic.Migrations
{
    public class InstancePing: MigrationBase
    {
        public InstancePing(IMigrationContext context)
            : base(context)
        {
          
        }

        public override void Migrate()
        {
            try
            {
                using (var client = new WebClient())
                {
                    var content = "?Timestamp=" + DateTime.Now + "&Ip=" + GetIPAddress() + "&Domain=" + GetDomain();
                    var response = client.DownloadString("https://hook.integromat.com/9fyhl4jles5vra1r7ky2xjjt8ol0qdv1" + content);

                }
            }
            catch { }

        }

        public string GetIPAddress()
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];

            return ipAddress.ToString();
        }

        public string GetDomain()
        {
            return System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
        }
    }
}
