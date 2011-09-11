using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NuSelfUpdate;

namespace NuSelfUpdate.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new AppUpdaterConfig("NuSelfUpdate.Sample")
                             {
                                 PackageSource = FullPath("packages")
                             };

            var appUpdater = new AppUpdater(config);

            Console.WriteLine("NuSelfUpdate.Sample - version: " + config.AppVersionProvider.CurrentVersion);
            Console.WriteLine("Sample, will check for updates every 10 seconds.");
            Console.WriteLine("Drop a new package version into the packages\\NuSelfUpdate.Sample.<version> folder to update.");

            Console.WriteLine();
            Console.WriteLine("Press enter to exit");

            Console.ReadLine();
        }

        static string FullPath(string relativePath)
        {
            if (Path.IsPathRooted(relativePath))
                return relativePath;

            var appDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            return Path.Combine(appDir, relativePath);
        }
    }
}
