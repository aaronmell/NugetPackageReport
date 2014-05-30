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
            Stack<string> directories = new Stack<string>(20);
            directories.Push(path);

            while (directories.Count > 0)
            {
                var directory = directories.Pop();

                var files = Directory.GetFiles(directory);

                var packagePaths = files.Where(x => x.ToLower().Contains("packages.config"));

                if (packagePaths.Any())
                {
                    var packages = GetPackages(packagePaths);

                    ProcessPackages(feed, packages);
                }
                 
                if (!files.Any(x => x.ToLower().Contains("csproj")))
                {
                    var subDirectories = Directory.GetDirectories(directory).Where(x => x != "packages" && x != ".nuget");
                    
                    foreach (var dir in subDirectories)
                    {
                        directories.Push(dir);
                    }
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

                var key = FeedPackages.Keys.FirstOrDefault(x => x.ID == package.ID && x.Version == package.Version);

                if (key != null)
                {
                    FeedPackages[key].Count++;
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
