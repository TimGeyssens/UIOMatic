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
                SetInstallMarker();
            }
        }
    }
}