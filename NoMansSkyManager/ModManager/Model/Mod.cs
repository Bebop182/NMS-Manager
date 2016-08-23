using System;
using System.IO;

namespace ModManager.Model {
    public class Mod {
        public string Name { get; set; }
        public DateTime CreationDate { get; set; }
        public FileInfo ModFile { get; }

        public Mod() {
            CreationDate = DateTime.Now;
            ModFile = null;
        }

        public Mod(string profilePath) : this()
        {
            //ModFile = Directory.CreateDirectory(profilePath);
        }

        public Mod(FileInfo modFile) : this()
        {
            ModFile = modFile;
            Name = Path.GetFileNameWithoutExtension(modFile.Name);
        }

        public void Delete() {
            ModFile.Delete();
        }
    }
}
