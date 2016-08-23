using System.Runtime.InteropServices;

namespace NoMansSkyManager.Infrastructure.Helpers {
    public class SymLink {
        [DllImport("kernel32.dll")]
        private static extern bool CreateSymbolicLink(string lpSymlinkFileName, string lpTargetFileName, int dwFlags);

        public static bool CreateSymbolicLink(string sourcePath, string targetPath, bool isDirectory)
        {
            return CreateSymbolicLink(sourcePath, targetPath, isDirectory ? 1 : 0);
        }
    }
}
