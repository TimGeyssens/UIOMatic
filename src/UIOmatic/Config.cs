using System;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;
using System.Xml;
using UIOmatic.Services;
using UC = Umbraco.Core;

namespace UIOMatic
{
    public class Config
    {
        public const string PluginFolder = "~/App_plugins/UIOMatic";

        public const string ConfigFileName = "UIOMatic.config";

        public static Type DefaultObjectServiceType
        {
            get
            {
                var attr = ConfigFile.DocumentElement.Attributes["defaultObjectServiceType"];
                return attr != null ? Type.GetType(attr.Value) : typeof(PetaPocoObjectService);
            }
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

            if (settingsFile != null) return (XmlDocument) settingsFile;
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
    }
}