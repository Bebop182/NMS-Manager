using System;
using System.Windows;
using Microsoft.Practices.Unity;
using Prism.Events;
using Prism.Modularity;
using Prism.Regions;

namespace NoMansSkyManager.Infrastructure {
    public abstract class ModuleBase : IModule {
        protected readonly IRegionManager RegionManager;
        protected readonly IEventAggregator EventAggregator;
        protected readonly IUnityContainer UnityContainer;

        public virtual Type GetEntryPointType()
        {
            return null;
        }

        public ModuleBase(IUnityContainer unityContainer, IRegionManager regionManager, IEventAggregator eventAggregator) {
            RegionManager = regionManager;
            EventAggregator = eventAggregator;
            UnityContainer = unityContainer;
        }

        public virtual void Initialize() {
            // Notify main application that module is ready to be displayed
            var moduleEvent = new ModuleLoadedEvent() {
                ModuleType = this.GetType(),
                EntryPointView = GetEntryPointType()
            };

            EventAggregator.GetEvent<ModuleLoadedEvent>().Publish(moduleEvent);

            Application.Current.Exit += OnExit;
        }

        protected virtual void OnExit(object sender, ExitEventArgs exitEventArgs) {
        }
    }
}
