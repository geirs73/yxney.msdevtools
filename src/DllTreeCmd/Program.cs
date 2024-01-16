using System;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

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
            var filtered2 = (from f in filtered1
                             where (f.Extension == ".exe" || f.Extension == ".dll")
                             select f).ToList();
            var filesStringArray = from f in filtered2 select f.FullName;
            var resolver = new PathAssemblyResolver(filesStringArray);

            Console.Out.WriteLine("sep=;");
            Console.Out.WriteLine($"Path;ProductVersion;FileVersion;LastWrittenTime");
            using (var metaDataContext = new MetadataLoadContext(resolver))
            {
                foreach (FileInfo f in filtered2)
                {
                    Assembly assembly = metaDataContext.LoadFromAssemblyPath(f.FullName);
                    Version version = Assembly.GetEntryAssembly().GetName().Version;
                    var versionString = version.ToString(4);
                    var fileVersion = FileVersionInfo.GetVersionInfo(f.FullName);

                    var formattedTime = f.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss");
                    Console.Out.WriteLine($"{f.DirectoryName};{f.Name};{versionString};{fileVersion.ProductVersion};{fileVersion.FileVersion};{formattedTime}");
                }

            }
        }
    }
}
