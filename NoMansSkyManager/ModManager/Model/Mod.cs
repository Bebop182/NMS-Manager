using System;
using System.IO;
using System.Text.RegularExpressions;

namespace NMSM.ModManager.Model {
    public class Mod {
        public bool Enabled { get; set; }
        public string DisplayName { get; set; }
        public string FileName { get { return ModFile.Name; } }
        public DateTime CreationDate { get; set; }
        public FileInfo ModFile { get; }

        private Mod()
        {
            Enabled = false;
            CreationDate = DateTime.Now;
            ModFile = null;
        }

        public Mod(FileInfo modFile) : this()
        {
            ModFile = modFile;
            var name = Path.GetFileNameWithoutExtension(modFile.Name).TrimStart('_');
            DisplayName = Regex.Replace(name, @"^_?[0-9]_", "");
        }
    }
}
