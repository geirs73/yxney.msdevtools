using System;
using System.IO;
using System.Diagnostics;
using System.Linq;

namespace DllTreeCmd
{
    class Program
    {
        // this program uses dragonfruit magic, because convenience

        public enum OutputFormat
        {
            csv,
            txt
        }

        // static void Main(string[] args)
        /// <param name="path">Path to folder</param>
        /// <param name="pattern"></param>
        /// <param name="excludeDir">File filter</param>
        /// <param name="format">File filter</param>
        static void Main(string path = ".", string pattern = "*.*", string excludeDir = "", OutputFormat format=OutputFormat.csv)
        {
            var files = Directory.GetFiles(path, pattern, SearchOption.AllDirectories);
            foreach (var f in files)
            {
                var dirs = f.Split(Path.DirectorySeparatorChar).ToList();
                var evilDirs = from d in dirs where d.Equals(excludeDir, StringComparison.OrdinalIgnoreCase) select d;
                if (!string.IsNullOrEmpty(excludeDir) && dirs.Any())
                {
                    var version = FileVersionInfo.GetVersionInfo(f);
                    switch (format)
                    {
                        case OutputFormat.csv:
                        Console.Out.WriteLine($"{f};{version.FileVersion}");
                        break;
                        default:
                        Console.Out.WriteLine($"File={f}\tVersion={version.FileVersion}");
                        break;
                    }

                }
            }
        }
    }
}
