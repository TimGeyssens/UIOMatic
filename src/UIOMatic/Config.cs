using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;
using System.Xml;
using UC = Umbraco.Core;

namespace UIOMatic
{
    public class Config
    {
        public const string PluginFolder = "~/App_plugins/UIOMatic";

        public const string ConfigFileName = "UIOMatic.config";

        public static Type DefaultObjectControllerType
        {
            get { return Type.GetType(ConfigFile.DocumentElement.Attributes["defaultObjectControllerType"].Value); }
        }

        public static XmlDocument ConfigFile
        {
            get
            {
                var us = (XmlDocument)HttpRuntime.Cache["UIOMaticConfigFile"] ?? EnsureConfig();
                return us;
            }
        }

        private static XmlDocument EnsureConfig()
        {
            var settingsFile = HttpRuntime.Cache["UIOMaticConfigFile"];
            var fullPath = HostingEnvironment.MapPath(PluginFolder + "/" + ConfigFileName);

            if (settingsFile == null)
            {
                var temp = new XmlDocument();
                var settingsReader = new XmlTextReader(fullPath);
                try
                {
                    temp.Load(settingsReader);
                    HttpRuntime.Cache.Insert("UIOMaticConfigFile", temp, new CacheDependency(fullPath));
                }
                catch (XmlException e)
                {
                    throw new XmlException("Your UIOMatic.config file fails to pass as valid XML. Refer to the InnerException for more information", e);
                }
                catch (Exception e)
                {

                    UC.Logging.LogHelper.Error(typeof(Config), e.Message, e);

                }
                settingsReader.Close();
                return temp;
            }
            return (XmlDocument)settingsFile;
        }
    }
}