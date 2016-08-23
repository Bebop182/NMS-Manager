using System;
using System.IO;

namespace ProfileManager.Model {
    public class PlayerProfile {
        public string Name { get; set; }
        public DateTime CreationDate { get; set; }
        public DirectoryInfo ProfileDirectory { get; }

        public PlayerProfile()
        {
            CreationDate = DateTime.Now;
            ProfileDirectory = null;
        }

        public PlayerProfile(string profilePath) : this()
        {
            ProfileDirectory = Directory.CreateDirectory(profilePath);
        }

        public PlayerProfile(DirectoryInfo profileDirectory)
        {
            Name = profileDirectory.Name;
            ProfileDirectory = profileDirectory;
        }

        public void Delete()
        {
            ProfileDirectory.Delete(true);
        }
    }
}
