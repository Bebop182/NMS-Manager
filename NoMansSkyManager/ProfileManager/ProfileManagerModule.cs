using System.Windows;
using NMSM.Infrastructure;
using NMSM.ProfileManager.Properties;
using NMSM.ProfileManager.Views;
using Prism.Events;
using Prism.Modularity;

namespace NMSM.ProfileManager {
    public class ProfileManagerModule : IModule {
        private readonly IEventAggregator _eventAggregator;

        public ProfileManagerModule(IEventAggregator eventAggregator) {
            _eventAggregator = eventAggregator;
        }

        public void Initialize() {
            // Notify main application that module is ready to be displayed
            var moduleEvent = new ModuleLoadedEvent() {
                ModuleType = this.GetType(),
                EntryPointView = typeof(ProfileShell)
            };

            _eventAggregator.GetEvent<ModuleLoadedEvent>().Publish(moduleEvent);

            Application.Current.Exit += OnExit;
        }

        private void OnExit(object sender, ExitEventArgs exitEventArgs) {
            // Save settings
            Settings.Default.Save();
        }
    }
}