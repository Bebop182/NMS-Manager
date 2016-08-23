using System;
using System.IO;

namespace NoMansSkyManager.Infrastructure {
    public class NMSManagerContext {
        public DirectoryInfo ProfilesDirectory { get; }
        public DirectoryInfo ModulesDirectory { get; }
        public DirectoryInfo NMSDataDirectory { get; }

        public NMSManagerContext() {
            ProfilesDirectory = Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "profiles");
            ModulesDirectory = Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "modules");
            NMSDataDirectory = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\HelloGames\NMS");
        }
    }
}
