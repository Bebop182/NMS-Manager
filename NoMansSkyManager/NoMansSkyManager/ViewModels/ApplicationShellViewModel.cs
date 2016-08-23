using System;
using System.Windows.Input;
using NoMansSkyManager.Infrastructure;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;

namespace NoMansSkyManager.ViewModels {
    public class ApplicationShellViewModel : BindableBase {
        private string _title = "NO MAN'S SKY MANAGER";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public ICommand RequestLaunchCommand { get; set; }

        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;

        public ApplicationShellViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<ModuleLoadedEvent>().Subscribe(OnModuleLoaded);
            _regionManager = regionManager;
            RequestLaunchCommand = new DelegateCommand(ExecuteRequestLaunch);
        }

        private void ExecuteRequestLaunch()
        {
            _eventAggregator.GetEvent<RequestedLaunchEvent>().Publish();
        }

        private void OnModuleLoaded(ModuleLoadedEvent moduleLoadedEvent) {
            if (moduleLoadedEvent.ModuleType == typeof(ProfileManager.ProfileManagerModule))
                _regionManager.RegisterViewWithRegion(RegionNames.Slot1, moduleLoadedEvent.EntryPointView);
            if (moduleLoadedEvent.ModuleType == typeof(ModManager.ModManagerModule))
                _regionManager.RegisterViewWithRegion(RegionNames.Slot2, moduleLoadedEvent.EntryPointView);
            
        }
    }
}
