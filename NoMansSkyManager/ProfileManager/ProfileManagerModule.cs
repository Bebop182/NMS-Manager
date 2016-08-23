using System.Windows;
using Microsoft.Practices.Unity;
using Prism.Regions;
using NoMansSkyManager.Infrastructure;
using Prism.Events;
using Prism.Modularity;
using ProfileManager.Properties;
using ProfileManager.Views;

namespace ProfileManager {
    public class ProfileManagerModule : IModule {
        protected readonly IRegionManager RegionManager;
        protected readonly IEventAggregator EventAggregator;
        protected readonly IUnityContainer UnityContainer;

        public ProfileManagerModule(IUnityContainer unityContainer, IRegionManager regionManager, IEventAggregator eventAggregator) {
            RegionManager = regionManager;
            EventAggregator = eventAggregator;
            UnityContainer = unityContainer;
        }

        public virtual void Initialize() {
            // Notify main application that module is ready to be displayed
            var moduleEvent = new ModuleLoadedEvent() {
                ModuleType = this.GetType(),
                EntryPointView = typeof(ProfileShell)
            };

            EventAggregator.GetEvent<ModuleLoadedEvent>().Publish(moduleEvent);

            Application.Current.Exit += OnExit;
        }

        protected void OnExit(object sender, ExitEventArgs exitEventArgs) {
            // Save settings
            Settings.Default.Save();
        }
    }
}