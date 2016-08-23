using System;
using System.Collections.Generic;
using System.IO;

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
            DisplayName = Path.GetFileNameWithoutExtension(modFile.Name).TrimStart('_');
        }
    }
}
