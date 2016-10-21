using System;
using System.IO;
using System.Linq;
using System.Xml;
using Umbraco.Core;
using Umbraco.Core.IO;
using Umbraco.Core.Logging;

namespace UIOMatic
{
    //TODO: Move all this to migrations?
    public class Installer : ApplicationEventHandler
    {
        private readonly string _marker = IOHelper.MapPath(Path.Combine(Config.PluginFolder, "installed"));

        public bool NeedsInstall()
        {
            if (File.Exists(_marker) == false)
                return true;

            return false;
        }

        public void SetUserAccess()
        {
            int i;
            var users = ApplicationContext.Current.Services.UserService.GetAll(0, 100, out i).Where(x => x.UserType.Alias == "admin");

            foreach (var user in users.Where(user => user.AllowedSections.Contains(Config.ApplicationAlias) == false))
            {
                user.AddAllowedSection(Config.ApplicationAlias);
                ApplicationContext.Current.Services.UserService.Save(user);
            }
        }

        public void InstallLanguageFiles()
        {
            var fileName = IOHelper.MapPath(umbraco.GlobalSettings.Path + "/config/lang/en.xml");
            if (File.Exists(fileName) == false)
                return;

            var languageFile = new XmlDocument { PreserveWhitespace = true };
            languageFile.Load(fileName);

            if (languageFile.DocumentElement == null)
                return;
           
            var sectionsRoot = languageFile.DocumentElement.SelectSingleNode("//area [@alias = 'sections']");
            if (sectionsRoot == null)
                return;

            var languageKey = languageFile.CreateNode(XmlNodeType.Element, "key", "");
            languageKey.InnerText = "UI-O-Matic";

            var attribute = languageFile.CreateAttribute("alias");
            attribute.Value = "uiomatic";

            if (languageKey.Attributes != null)
                languageKey.Attributes.Append(attribute);

            sectionsRoot.AppendChild(languageKey);
            languageFile.Save(fileName);

        }
        public void SetInstallMarker()
        {
            try
            {

                File.WriteAllText(_marker, string.Empty);
                using (var streamWriter = new StreamWriter(_marker, false))
                {
                    streamWriter.Write("Installed");
                }
            }
            catch (Exception exception)
            {
                LogHelper.Error<Installer>(string.Format("Can't create the install marker at {0}", _marker), exception);
            }
        }

        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            if (NeedsInstall())
            {
                SetUserAccess();
                InstallLanguageFiles();
                SetInstallMarker();
            }
        }
    }
}