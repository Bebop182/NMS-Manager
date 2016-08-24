using System;
using System.IO;

namespace NMSM.Infrastructure {
    public class NMSManagerContext {
        public DirectoryInfo ProfilesDirectory { get; }
        public DirectoryInfo ModsDirectory { get; }
        public DirectoryInfo NMSSavesDirectory { get; set; }
        public DirectoryInfo NMSInstallDirectory { get; set; }
        public DirectoryInfo NMSGameDataDirectory { get; set; }

        public NMSManagerContext() {
            ProfilesDirectory = Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "profiles");
            ModsDirectory = Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "modules");
        }
    }
}
