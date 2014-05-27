using NugetPackageReport.Nuget;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NugetPackageReport
{
    public static class Processor
    {
        public static Dictionary<PackageConfig, FeedPackage> FeedPackages = new Dictionary<PackageConfig, FeedPackage>();

        public static void RecurseDirectory(V1FeedContext feed, string path)
        {
            foreach (var directory in Directory.GetDirectories(path))
            {
                var files = Directory.GetFiles(directory);

                var packagePaths = files.Where(x => x.Contains("packages.config"));

                if (packagePaths.Any())
                {
                    var packages = GetPackages(packagePaths);

                    ProcessPackages(feed, packages);
                }

                //Using the where clause to cut down on the number of directories that need to be traversed
                foreach (var directoryToSearch in Directory.GetDirectories(directory).Where(x => x != "bin" && x != "obj" && x != "Properties" && x != "packages" && x != ".nuget"))
                {
                    RecurseDirectory(feed, directoryToSearch);
                }
            }
        }

        private static void ProcessPackages(V1FeedContext feed, List<PackageConfig> packages)
        {
            foreach (var package in packages)
            {
                var feedPackages = feed.Packages.Where(x => x.Id == package.ID).ToList();

                var currentVersion = feedPackages.FirstOrDefault(x => x.Version == package.Version);

                var latestVersion = feedPackages.OrderByDescending(x => x, new FeedPackageVersionComparer()).FirstOrDefault();

                if (FeedPackages.ContainsKey(package))
                {
                    FeedPackages[package].Count++;
                }
                else
                {
                    FeedPackages.Add(package, new FeedPackage
                    {
                        CurrentVersion = currentVersion,
                        LatestVersion = latestVersion,
                        Count = 1
                    });
                }
            }

        }

        private static List<PackageConfig> GetPackages(IEnumerable<string> packagePaths)
        {
            var packages = new List<PackageConfig>();

            foreach (var packagePath in packagePaths)
            {
                var doc = XDocument.Load(packagePath);

                var children = doc.Elements().Elements();

                packages = children.Select(x => new PackageConfig
                {
                    ID = x.Attribute("id").Value,
                    Version = x.Attribute("version").Value,
                }).ToList();
            }

            return packages;
        }     
    }
}
