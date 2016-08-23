using System.Collections.ObjectModel;
using System.Windows.Input;
using ModManager.Model;
using ModManager.Properties;
using NoMansSkyManager.Infrastructure;
using NoMansSkyManager.Infrastructure.Helpers;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;

namespace ModManager.ViewModels {
    public class ModManagerShellViewModel : BindableBase
    {
        private ObservableCollection<Mod> _availableMods;
        public ObservableCollection<Mod> AvailableMods
        {
            get { return _availableMods; }
            set { SetProperty(ref _availableMods, value); }
        }

        private ObservableCollection<Mod> _enabledMods;
        public ObservableCollection<Mod> EnabledMods
        {
            get { return _enabledMods; }
            set { SetProperty(ref _enabledMods, value); }
        }

        public Mod SelectedAvailableMod { get; set; } 
        public Mod SelectedEnabledMod { get; set; } 

        public ICommand EnableModCommand { get; set; }
        public ICommand DisableModCommand { get; set; }

        private readonly NMSManagerContext _context;
        private readonly IEventAggregator _eventAggregator;

        public ModManagerShellViewModel(IEventAggregator eventAggregator, NMSManagerContext context)
        {
            _context = context;
            _eventAggregator = eventAggregator;
            AvailableMods = new ObservableCollection<Mod>();
            EnabledMods = new ObservableCollection<Mod>();

            _eventAggregator.GetEvent<RequestedLaunchEvent>().Subscribe(OnRequestLaunch);

            EnableModCommand = new DelegateCommand(ExecuteEnableMod);
            DisableModCommand = new DelegateCommand(ExecuteDisableMod);

            LoadModList();
        }

        private void OnRequestLaunch()
        {
            var installPath = Settings.Default.NMSInstallationPath + @"\GAMEDATA\PCBANKS";
            // Copy Mods To gamefolder
            foreach (var mod in EnabledMods)
            {
                SymLink.CreateSymbolicLink(mod.ModFile.FullName, installPath + @"\_" + mod.ModFile.Name, false);
            }
        }

        private void ExecuteEnableMod()
        {
            if(SelectedAvailableMod == null) return;
            if(EnabledMods.Contains(SelectedAvailableMod)) return;

            EnabledMods.Add(SelectedAvailableMod);
        }

        private void ExecuteDisableMod() {
            if(SelectedEnabledMod == null) return;

            EnabledMods.Remove(SelectedEnabledMod);
        }

        private void LoadModList()
        {
            // Look into mod folder for pak
            var files = _context.ModulesDirectory.GetFiles("*.pak");

            foreach (var file in files)
            {
                AvailableMods.Add(new Mod(file));
            }
        }
    }
}
