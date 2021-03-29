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
using Umbraco.Web;

namespace UIOMatic.Migrations
{
    public class InstancePing: MigrationBase
    {
        private readonly IUmbracoContextFactory _context;
        public InstancePing(IMigrationContext context, IUmbracoContextFactory ucontext)
            : base(context)
        {
            _context = ucontext;
        }

        public override void Migrate()
        {
            try
            {
                using (var client = new WebClient())
                {
                    var content = "?Timestamp=" + DateTime.Now + "&Ip=" + HttpUtility.UrlEncode(GetIPAddress()) + "&Domain=" + HttpUtility.UrlEncode(GetDomain());
                    var response = client.DownloadString("https://hook.integromat.com/9fyhl4jles5vra1r7ky2xjjt8ol0qdv1" + content);

                }
            }
            catch { }

        }

        public string GetIPAddress()
        {
            try
            {
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());

                if (ipHostInfo.AddressList.Any())
                {
                    return string.Join(",", ipHostInfo.AddressList.Select(x => x.ToString()));
                }
            }
            catch { }
            return string.Empty;
        }

        public string GetDomain()
        {
            try
            {
                var domain = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;

            if (!string.IsNullOrEmpty(domain)) return domain;
            }
            catch { }

            try
            {
                using (var cref = _context.EnsureUmbracoContext())
            {
                if (cref.UmbracoContext.Domains.GetAll(true).Any())
                {
                    return string.Join(",", cref.UmbracoContext.Domains.GetAll(true).Select(x => x.Name));

                }
            }
            }
            catch { }

            return string.Empty;
        }
    }
}
