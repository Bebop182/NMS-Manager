using System.Runtime.InteropServices;

namespace NMSM.Infrastructure.Helpers {
    public class SymLink {
        [DllImport("kernel32.dll")]
        private static extern bool CreateSymbolicLink(string lpSymlinkFileName, string lpTargetFileName, int dwFlags);

        public static bool CreateSymbolicLink(string targetPath, string sourcePath, bool isDirectory)
        {
            return CreateSymbolicLink(targetPath, sourcePath, isDirectory ? 1 : 0);
        }
    }
}
