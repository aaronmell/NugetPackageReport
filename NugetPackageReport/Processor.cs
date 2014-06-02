using NugetPackageReport.Nuget;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace NugetPackageReport
{
    /// <summary>
    /// Handles Processing the input path and finding all of the nuget packages used by the projects
    /// </summary>
    internal static class Processor
    {
        /// <summary>
        /// Proceesses the path and returns all of the the nuget packags found
        /// </summary>
        /// <param name="path">The path to process</param>
        /// <returns>A dictionary that contains the packages found during the search</returns>
        internal static Dictionary<PackageKey, FeedPackage> Process(string path)
        {
            var v1FeedContext = new V1FeedContext(new Uri("http://packages.nuget.org/v1/FeedService.svc"));
            var feedPackages = new Dictionary<PackageKey, FeedPackage>();

            var directories = new Stack<string>(20);
            directories.Push(path);

            while (directories.Count > 0)
            {
                var directory = directories.Pop();

                var files = Directory.GetFiles(directory);

                var packagePaths = files.Where(x => x.ToLower().EndsWith("packages.config")).ToList();
                var projectName = files.FirstOrDefault(x => x.ToLower().Contains("csproj"));

                if (packagePaths.Any())
                {
                    var packageConfigs = getPackages(packagePaths);

                    processPackages(feedPackages, v1FeedContext, packageConfigs, projectName);
                }

                if (projectName != null)
                {
                    continue;
                }

                var subDirectories = Directory.GetDirectories(directory).Where(x => x != "packages" && x != ".nuget");

                foreach (var dir in subDirectories)
                {
                    directories.Push(dir);
                }
            }
            return feedPackages;
        }

        private static IEnumerable<PackageKey> getPackages(IEnumerable<string> packagePaths)
        {
            foreach (var children in packagePaths.Select(XDocument.Load).Select(doc => doc.Elements().Elements()))
            {
                foreach (var package in children.Select(x => new PackageKey
                {
                    Id = x.Attribute("id").Value,
                    Version = x.Attribute("version").Value,
                }))
                    yield return package;
            }
        }

        private static void processPackages(Dictionary<PackageKey, FeedPackage> feedPackageDictionary, V1FeedContext feed, IEnumerable<PackageKey> packages, string projectPath)
        {
            var projectFileName = Path.GetFileNameWithoutExtension(projectPath);

            foreach (var package in packages)
            {
                var key = feedPackageDictionary.Keys.FirstOrDefault(x => x.Id == package.Id && x.Version == package.Version);

                if (key != null)
                {
                    feedPackageDictionary[key].ProjectNames.Add(projectFileName);
                }
                else
                {
                    var package1 = package;
                    var v1FeedPackages = feed.Packages.Where(x => x.Id == package1.Id).ToList();

                    var currentVersion = v1FeedPackages.FirstOrDefault(x => x.Version == package1.Version);

                    var latestVersion =
                        v1FeedPackages.OrderByDescending(x => x, new FeedPackageVersionComparer()).FirstOrDefault();

                    feedPackageDictionary.Add(package, new FeedPackage
                    {
                        CurrentVersion = currentVersion,
                        LatestVersion = latestVersion,
                        ProjectNames = new List<string>
                        {
                            projectFileName
                        },
                    });
                }
            }
        }
    }
}