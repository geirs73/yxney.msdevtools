using System;
using System.IO;
using System.Diagnostics;
using System.Linq;

namespace DllTreeCmd
{
    internal class Program
    {
        // this program uses DragonFruit magic, because convenience

        public enum OutputFormat
        {
            Csv,
            Txt,
            Json
        }

        /// <param name="path">Path to folder</param>
        /// <param name="pattern">File pattern to match</param>
        /// <param name="exclude">Semi-colon separated list of strings to exclude</param>
        /// <param name="format">Output format, only CSV for now</param>
        public static void Main(string path = ".", string pattern = "*.*", string exclude = "obj", OutputFormat format = OutputFormat.Csv)
        {
            switch (format)
            {
                case OutputFormat.Csv: break;
                default:
                    throw new Exception("Only CSV format is supported at this time");
            }
            var dir = new DirectoryInfo(path);
            var files = dir.GetFiles(pattern, SearchOption.AllDirectories);

            var exclusions = (from d in exclude.Split(';') select d.ToUpperInvariant()).ToList();
            var filtered1 = (from f in files where !exclusions.Any(f.FullName.ToUpperInvariant().Contains) select f).ToList();

            Console.Out.WriteLine("sep=;");
            Console.Out.WriteLine($"Path;Version;LastWrittenTime");
            foreach (FileInfo f in filtered1)
            {
                var version = FileVersionInfo.GetVersionInfo(f.FullName);
                Console.Out.WriteLine($"{f};{version.FileVersion};{f.LastAccessTime:O}");
            }
        }
    }
}
