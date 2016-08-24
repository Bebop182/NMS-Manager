using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using Newtonsoft.Json;
using NMSM.Infrastructure;
using NMSM.ModManager.Model;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;

namespace NMSM.ModManager.ViewModels
{
    public class ModManagerShellViewModel : BindableBase
    {
        #region Properties
        public Mod SelectedMod
        {
            get { return _selectedMod; }
            set { SetProperty(ref _selectedMod, value);}
        }
        public ObservableDictionary<string, ObservableCollection<Mod>> ModCollections
        {
            get { return _modCollections; }
            set { SetProperty(ref _modCollections, value); }
        }

        public KeyValuePair<string, ObservableCollection<Mod>> SelectedModCollection
        {
            get { return _selectedModCollection; }
            set { SetProperty(ref _selectedModCollection, value); }
        }
        #endregion

        #region Commands
        public ICommand AddModCollectionCommand { get; set; }
        public ICommand RemoveModCollectionCommand { get; set; }

        public ICommand IncreaseModPriorityCommand { get; set; }
        public ICommand DecreaseModPriorityCommand { get; set; }
        #endregion

        #region Private attributes
        private readonly NMSManagerContext _context;
        private readonly IEventAggregator _eventAggregator;

        private int _collectionNextIndex = 0;
        private List<FileInfo> _modFiles;
        private Mod _selectedMod;
        private ObservableDictionary<string, ObservableCollection<Mod>> _modCollections;
        private KeyValuePair<string, ObservableCollection<Mod>> _selectedModCollection;
        #endregion

        #region Constructor
        public ModManagerShellViewModel(IEventAggregator eventAggregator, NMSManagerContext context)
        {
            _context = context;
            _eventAggregator = eventAggregator;

            LoadModFiles();
            LoadModCollections();
            if (ModCollections == null)
            {
                ModCollections = new ObservableDictionary<string, ObservableCollection<Mod>>();
            }
            if (!ModCollections.Any())
            {
                ModCollections.Add("Collection #1", new ObservableCollection<Mod>(_modFiles.ConvertAll(file => new Mod(file))));
            }
            
            SelectedModCollection = ModCollections.First();

            AddModCollectionCommand = new DelegateCommand<string>(ExecuteAddModCollection);
            RemoveModCollectionCommand = new DelegateCommand(ExecuteRemoveModuleCollection);

            IncreaseModPriorityCommand = new DelegateCommand(ExecuteIncreaseModPriority);
            DecreaseModPriorityCommand = new DelegateCommand(ExecuteDecreaseModPriority);

            _eventAggregator.GetEvent<RequestedLaunchEvent>().Subscribe(OnRequestLaunch);

            Application.Current.Exit += (sender, args) => SaveModCollection();
        }
       

        private void ExecuteIncreaseModPriority()
        {
            if(SelectedMod == null) return;
            var modCollection = SelectedModCollection.Value;
            if (modCollection.First() == SelectedMod) return;

            var modIndex = modCollection.IndexOf(SelectedMod);
            modCollection.Move(modIndex, --modIndex);
        }

        private void ExecuteDecreaseModPriority()
        {
            if (SelectedMod == null) return;
            var modCollection = SelectedModCollection.Value;
            if (modCollection.Last() == SelectedMod) return;

            var modIndex = modCollection.IndexOf(SelectedMod);
            modCollection.Move(modIndex, ++modIndex);
        }
        #endregion

        #region Commands Implementation
        private void ExecuteAddModCollection(string name)
        {
            var collectionName = "";
            do
            {
                collectionName = string.IsNullOrEmpty(name) ? "Collection #" + ++_collectionNextIndex : name;
            } while (ModCollections.ContainsKey(collectionName));
            ModCollections.Add(collectionName, new ObservableCollection<Mod>(_modFiles.ConvertAll(file => new Mod(file))));
        }

        private void ExecuteRemoveModuleCollection()
        {
            if (string.IsNullOrEmpty(SelectedModCollection.Key)) return;
            ModCollections.Remove(SelectedModCollection.Key);
            // Todo: select next item
            if (ModCollections.Any())
                SelectedModCollection = ModCollections.First();
        }
        #endregion

        #region Private Methods
        private void OnRequestLaunch()
        {
            //var gameDataDirectory = new DirectoryInfo(_context.NMSGameDataDirectory);
            var pakFiles = _context.NMSGameDataDirectory.GetFiles("*.pak");

            // Get all custom pak files in game's directory
            // match *.pak but not original gamefile like NMSARC.715AAAD6.pak
            var pattern = @"^(?!NMSARC\.[A-Z0-9]{8}.pak$).*\.pak$";
            var disposabbleModFiles = pakFiles.Where(file => Regex.IsMatch(file.Name, pattern)).ToList();

            // Copy Mods To gamefolder if not yet present
            var modEnabled = SelectedModCollection.Value.Where(m => m.Enabled).ToList();
            var priority = modEnabled.Count();
            foreach (var mod in modEnabled)
            {
                var modName = "_" + priority-- + "_" + mod.DisplayName + ".pak";
                var modFile = disposabbleModFiles.FirstOrDefault(mf => mf.Name.Equals(modName));
                if (modFile != null)
                    disposabbleModFiles.Remove(modFile);
                else
                    File.Copy(mod.ModFile.FullName, Path.Combine(_context.NMSGameDataDirectory.FullName, modName));
            }

            // Remove the rest of the custom pak files
            foreach (var file in disposabbleModFiles)
            {
                file.Delete();
            }
        }

        private void LoadModFiles()
        {
            // Look into mod folder for pak
            _modFiles = new List<FileInfo>(_context.ModsDirectory.GetFiles("*.pak"));
        }

        private void SaveModCollection()
        {
            var collections = JsonConvert.SerializeObject(ModCollections, new JsonSerializerSettings() { DateFormatHandling = DateFormatHandling.MicrosoftDateFormat });
            File.WriteAllText(_context.SettingsDirectory.FullName + @"\userCollections.json", collections);
        }

        private void LoadModCollections()
        {
            try
            {
                var collections = File.ReadAllText(_context.SettingsDirectory.FullName + @"\userCollections.json");
                ModCollections = JsonConvert.DeserializeObject<ObservableDictionary<string, ObservableCollection<Mod>>>(collections);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error loading collections\n" + e.Message);
            }
        }
        #endregion
    }
}
