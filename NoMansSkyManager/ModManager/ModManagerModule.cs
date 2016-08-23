using Prism.Modularity;
using Prism.Regions;
using System.Windows;
using Microsoft.Practices.Unity;
using ModManager.Properties;
using ModManager.Views;
using NoMansSkyManager.Infrastructure;
using Prism.Events;

namespace ModManager {
    public class ModManagerModule : IModule {
        protected readonly IRegionManager RegionManager;
        protected readonly IEventAggregator EventAggregator;
        protected readonly IUnityContainer UnityContainer;

        public ModManagerModule(IUnityContainer unityContainer, IRegionManager regionManager, IEventAggregator eventAggregator) {
            RegionManager = regionManager;
            EventAggregator = eventAggregator;
            UnityContainer = unityContainer;
        }

        public virtual void Initialize() {
            // Notify main application that module is ready to be displayed
            var moduleEvent = new ModuleLoadedEvent() {
                ModuleType = this.GetType(),
                EntryPointView = typeof(ModManagerShell)
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