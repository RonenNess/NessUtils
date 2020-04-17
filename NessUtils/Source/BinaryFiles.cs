using System.IO;

namespace Ness.Utils
{
    /// <summary>
    /// Utils to handle binary files.
    /// </summary>
    public static class BinaryFiles
    {
        /// <summary>
        /// Open binary file for output.
        /// </summary>
        public static BinaryWriter OpenBinaryFileOutput(string path)
        {
            return new BinaryWriter(File.Open(path, FileMode.Create));
        }

        /// <summary>
        /// Open binary file for input.
        /// </summary>
        public static BinaryReader OpenBinaryFileInput(string path)
        {
            return new BinaryReader(File.Open(path, FileMode.Open));
        }
    }
}
