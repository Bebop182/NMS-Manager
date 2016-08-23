using Microsoft.Practices.Unity;
using Prism.Unity;
using NoMansSkyManager.Views;
using System.Windows;
using NoMansSkyManager.Infrastructure;
using Prism.Modularity;

namespace NoMansSkyManager {
    class Bootstrapper : UnityBootstrapper {
        protected override DependencyObject CreateShell() {
            return Container.Resolve<ApplicationShell>();
        }

        protected override void InitializeShell() {
            Application.Current.MainWindow.Show();
        }

        protected override void ConfigureModuleCatalog() {
            var moduleCatalog = (ModuleCatalog)ModuleCatalog;

            moduleCatalog.AddModule(typeof(ModManager.ModManagerModule));
            moduleCatalog.AddModule(typeof(ProfileManager.ProfileManagerModule));
        }

        protected override void ConfigureContainer() {
            base.ConfigureContainer();
            Container.RegisterInstance(new NMSManagerContext(), new ContainerControlledLifetimeManager());
        }
    }
}
