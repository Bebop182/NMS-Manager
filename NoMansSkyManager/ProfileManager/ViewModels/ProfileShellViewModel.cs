using System;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Prism.Commands;
using ProfileManager.Model;
using System.IO;
using System.IO.Compression;
using NoMansSkyManager.Infrastructure;
using Prism.Events;

namespace ProfileManager.ViewModels {
    public class ProfileShellViewModel : BindableBase {
        private int _profileNextIndex;

        private PlayerProfile _activePlayerProfile;
        public PlayerProfile ActivePlayerProfile
        {
            get { return _activePlayerProfile; }
            set
            {
                SetProperty(ref _activePlayerProfile, value);
                Properties.Settings.Default.LastActiveProfile = value.Name;
                ((DelegateCommand)MakeProfileActiveCommand).RaiseCanExecuteChanged();
                ((DelegateCommand)RemoveProfileCommand).RaiseCanExecuteChanged();
            }
        }

        private ObservableCollection<PlayerProfile> _playerProfiles;
        public ObservableCollection<PlayerProfile> PlayerProfiles
        {
            get { return _playerProfiles; }
            set
            {
                SetProperty(ref _playerProfiles, value);
            }
        }

        private PlayerProfile _selectedProfile;

        public PlayerProfile SelectedProfile
        {
            get { return _selectedProfile; }
            set
            {
                SetProperty(ref _selectedProfile, value);
                ((DelegateCommand)MakeProfileActiveCommand).RaiseCanExecuteChanged();
                ((DelegateCommand)RemoveProfileCommand).RaiseCanExecuteChanged();
            }
        }

        public ICommand AddProfileCommand { get; set; }
        public ICommand RemoveProfileCommand { get; set; }
        public ICommand MakeProfileActiveCommand { get; set; }

        private readonly NMSManagerContext _context;
        private readonly IEventAggregator _eventAggregator;

        public ProfileShellViewModel(IEventAggregator eventAggregator, NMSManagerContext context) {
            _context = context;
            _eventAggregator = eventAggregator;
            _profileNextIndex = 0;
            PlayerProfiles = new ObservableCollection<PlayerProfile>();

            AddProfileCommand = new DelegateCommand<string>(ExecuteAddProfile);
            RemoveProfileCommand = new DelegateCommand(ExecuteRemoveProfile, CanExecuteRemoveProfile);
            MakeProfileActiveCommand = new DelegateCommand(ExecuteMakeProfileActive, CanExecuteMakeProfileActive);

            _eventAggregator.GetEvent<RequestedLaunchEvent>().Subscribe(OnRequestLaunch);

            // Get all profile directories
            var profiles = _context.ProfilesDirectory.EnumerateDirectories().ToList();
            if (profiles.Count <= 0)
            {
                ExecuteAddProfile("Default");
            } 
            else
            {
                foreach (var directory in profiles) {
                    PlayerProfiles.Add(new PlayerProfile(directory));
                }
            }

            var lastActiveProfile = Properties.Settings.Default.LastActiveProfile;
            var profile = PlayerProfiles.FirstOrDefault(p => !string.IsNullOrEmpty(lastActiveProfile) && p.Name.Equals(lastActiveProfile));

            ActivePlayerProfile = profile ?? PlayerProfiles.Last();
        }

        private void OnRequestLaunch()
        {
            ExecuteMakeProfileActive();
        }

        private void ExecuteAddProfile(string s) {
            // Todo: safety check on profile's name

            var profileName = "";
            do {
                profileName = string.IsNullOrEmpty(s) ? "Profile " + ++_profileNextIndex : s;
            } while (PlayerProfiles.Any(p => p.Name.Equals(profileName)));

            PlayerProfiles.Add(new PlayerProfile(_context.ProfilesDirectory.FullName + @"\" + profileName) {
                Name = profileName
            });
        }

        private void ExecuteRemoveProfile() {
            if (SelectedProfile == null) return;

            var profile = SelectedProfile;
            PlayerProfiles.Remove(SelectedProfile);
            profile.Delete();
        }

        private bool CanExecuteRemoveProfile() {
            return PlayerProfiles.Count > 1
                && SelectedProfile != null
                && SelectedProfile != ActivePlayerProfile;
        }

        private void ExecuteMakeProfileActive()
        {
            if (SelectedProfile == null) return;
            if (ActivePlayerProfile == null) return;
            if (ActivePlayerProfile == SelectedProfile) return;

            const string zipFilepath = @"\profile.zip";

            // Backup any existing zip in current active profile
            try
            {
                File.Move(ActivePlayerProfile.ProfileDirectory.FullName + zipFilepath,
                    ActivePlayerProfile.ProfileDirectory.FullName + @"\profile_backup_" +
                    DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss") + ".zip");
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }

            // Zip NMSDataDirectory into current active profile
            try {
                ZipFile.CreateFromDirectory(_context.NMSDataDirectory.FullName, ActivePlayerProfile.ProfileDirectory.FullName + zipFilepath);
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
                return;
            }

            // Clean NMSDataDirectory
            CleanDirectory(_context.NMSDataDirectory);

            // Change ActivePlayerProfile
            ActivePlayerProfile = SelectedProfile;

            // Unzip selected profile into NMSDataDirectory
            try {
                ZipFile.ExtractToDirectory(ActivePlayerProfile.ProfileDirectory.FullName + zipFilepath, _context.NMSDataDirectory.FullName);
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
                //return;
            }
        }

        private bool CanExecuteMakeProfileActive() {
            return ActivePlayerProfile != null && SelectedProfile != null
                   && ActivePlayerProfile != SelectedProfile;
        }

        private static void CleanDirectory(DirectoryInfo dir)
        {
            foreach (var directory in dir.EnumerateDirectories())
            {
                directory.Delete(true);
            }
        }
    }
}
