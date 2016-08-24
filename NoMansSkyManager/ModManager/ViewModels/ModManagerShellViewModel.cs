using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;
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
        public ObservableDictionary<string, IEnumerable<Mod>> ModCollections
        {
            get { return _modCollections; }
            set { SetProperty(ref _modCollections, value); }
        }

        public KeyValuePair<string, IEnumerable<Mod>> SelectedModCollection
        {
            get { return _selectedModCollection; }
            set { SetProperty(ref _selectedModCollection, value); }
        }
        #endregion

        #region Commands
        public ICommand AddModCollectionCommand { get; set; }
        public ICommand RemoveModCollectionCommand { get; set; }
        #endregion

        #region Private attributes
        private readonly NMSManagerContext _context;
        private readonly IEventAggregator _eventAggregator;

        private int _collectionNextIndex = 0;
        private List<FileInfo> _modFiles;
        private ObservableDictionary<string, IEnumerable<Mod>> _modCollections;
        private KeyValuePair<string, IEnumerable<Mod>> _selectedModCollection;
        #endregion

        #region Constructor
        public ModManagerShellViewModel(IEventAggregator eventAggregator, NMSManagerContext context)
        {
            _context = context;
            _eventAggregator = eventAggregator;

            LoadModList();
            ModCollections = new ObservableDictionary<string, IEnumerable<Mod>>
            {
                {"Collection #1", _modFiles.ConvertAll(file => new Mod(file))},
            };

            SelectedModCollection = ModCollections.First();

            AddModCollectionCommand = new DelegateCommand<string>(ExecuteAddModCollection);
            RemoveModCollectionCommand = new DelegateCommand(ExecuteRemoveModuleCollection);

            _eventAggregator.GetEvent<RequestedLaunchEvent>().Subscribe(OnRequestLaunch);
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
            ModCollections.Add(collectionName, _modFiles.ConvertAll(file => new Mod(file)));
        }

        private void ExecuteRemoveModuleCollection()
        {
            if (string.IsNullOrEmpty(SelectedModCollection.Key)) return;
            ModCollections.Remove(SelectedModCollection.Key);
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
            foreach (var mod in SelectedModCollection.Value.Where(m => m.Enabled))
            {
                var modName = "_" + mod.DisplayName + ".pak";
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

        private void LoadModList()
        {
            // Look into mod folder for pak
            _modFiles = new List<FileInfo>(_context.ModsDirectory.GetFiles("*.pak"));
        }
        #endregion
    }
}
