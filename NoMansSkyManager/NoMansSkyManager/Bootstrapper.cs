using System;
using System.IO;
using System.Windows;
using Microsoft.Practices.Unity;
using NMSM.ModManager;
using NMSM.ProfileManager;
using NMSM.Properties;
using NMSM.Views;
using NoMansSkyManager.Infrastructure;
using Prism.Modularity;
using Prism.Unity;

namespace NMSM
{
    class Bootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<ApplicationShell>();
        }

        protected override void InitializeShell()
        {
            Application.Current.MainWindow.Show();
        }

        protected override void ConfigureModuleCatalog()
        {
            var moduleCatalog = (ModuleCatalog)ModuleCatalog;

            moduleCatalog.AddModule(typeof(ModManagerModule));
            moduleCatalog.AddModule(typeof(ProfileManagerModule));
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            var context = new NMSManagerContext()
            {
                NMSInstallDirectory = new DirectoryInfo(Settings.Default.NMSInstallDirectory),
                NMSSavesDirectory = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Settings.Default.NMSSavesDirectory),
                NMSGameDataDirectory = new DirectoryInfo(Settings.Default.NMSInstallDirectory + Settings.Default.NMSGameDataRelativePath),
            };
            Container.RegisterInstance(context, new ContainerControlledLifetimeManager());
        }
    }
}
