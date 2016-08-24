using NMSM.Infrastructure;
using NMSM.ModManager.Views;
using Prism.Events;
using Prism.Modularity;

namespace NMSM.ModManager {
    public class ModManagerModule : IModule {
        private readonly IEventAggregator _eventAggregator;

        public ModManagerModule(IEventAggregator eventAggregator) {
            _eventAggregator = eventAggregator;
        }

        public void Initialize() {
            // Notify main application that module is ready to be displayed
            var moduleEvent = new ModuleLoadedEvent() {
                ModuleType = this.GetType(),
                EntryPointView = typeof(ModManagerShell)
            };

            _eventAggregator.GetEvent<ModuleLoadedEvent>().Publish(moduleEvent);

            //Application.Current.Exit += OnExit;
        }
    }
}