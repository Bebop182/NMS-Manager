using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using NMSM.ModManager;
using NMSM.ProfileManager;
using NoMansSkyManager.Infrastructure;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;

namespace NMSM.ViewModels {
    public class ApplicationShellViewModel : BindableBase {
        private string _title = "NO MAN'S SKY MANAGER";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private bool _canLaunch = true;

        public ICommand RequestLaunchCommand { get; set; }

        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly NMSManagerContext _context;

        public ApplicationShellViewModel(IRegionManager regionManager, IEventAggregator eventAggregator, NMSManagerContext context)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<ModuleLoadedEvent>().Subscribe(OnModuleLoaded);
            _context = context;
            RequestLaunchCommand = new DelegateCommand(ExecuteRequestLaunch, CanExecuteRequestLaunch);
        }

        private bool CanExecuteRequestLaunch()
        {
            return _canLaunch;
        }

        private  async void ExecuteRequestLaunch()
        {
            // Prevent gamefile modification by NMSM while the game is running
            _canLaunch = false;
            ((DelegateCommand)RequestLaunchCommand).RaiseCanExecuteChanged();

            _eventAggregator.GetEvent<RequestedLaunchEvent>().Publish();
            await Task.Delay(1000);
            var nms = Process.Start(_context.NMSInstallDirectory + @"\Binaries\NMS.exe");
            nms.Exited += NmsOnExited;
            nms.EnableRaisingEvents = true;
        }

        private void NmsOnExited(object sender, EventArgs eventArgs)
        {
            Console.WriteLine("Game quitted");
            ((DelegateCommand)RequestLaunchCommand).RaiseCanExecuteChanged();
            _canLaunch = true;
        }

        private void OnModuleLoaded(ModuleLoadedEvent moduleLoadedEvent) {
            if (moduleLoadedEvent.ModuleType == typeof(ProfileManagerModule))
                _regionManager.RegisterViewWithRegion(RegionNames.Slot1, moduleLoadedEvent.EntryPointView);
            if (moduleLoadedEvent.ModuleType == typeof(ModManagerModule))
                _regionManager.RegisterViewWithRegion(RegionNames.Slot2, moduleLoadedEvent.EntryPointView);
        }
    }
}
