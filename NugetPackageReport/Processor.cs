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
        private const string packageElementName = "package";

        private const string packagesConfigFileName = "packages.config";

        private const string projectIdentifier = "proj";

        /// <summary>
        /// Processes the path and returns all of the the nuget packages found.
        /// </summary>
        /// <param name="path">The path to process.</param>
        /// <returns>Dictionary containing all the packages found during the search.</returns>
        internal static Dictionary<PackageKey, FeedPackage> Process(string path)
        {
            var v1FeedContext = new V1FeedContext(new Uri("http://packages.nuget.org/v1/FeedService.svc"));
            var feedPackages = new Dictionary<PackageKey, FeedPackage>();

            var pathHasProject = Directory.EnumerateFiles(path).Any(filePath => filePath.EndsWith(projectIdentifier));

            var projectDirectories = Directory.EnumerateDirectories(path, "*", SearchOption.AllDirectories)
                .Where(directoryPath => Directory.EnumerateFiles(directoryPath, "*", SearchOption.TopDirectoryOnly)
                    .Where(filePath => filePath.EndsWith(projectIdentifier)).Any()).ToList();

            if (pathHasProject)
                projectDirectories.Add(path);

            foreach (var projectDirectory in projectDirectories)
            {
                if (!Directory.EnumerateFiles(projectDirectory, packagesConfigFileName, SearchOption.TopDirectoryOnly).Any())
                    continue;

                var packages = getPackages(Path.Combine(projectDirectory, packagesConfigFileName));

                var projectFilePath = Directory.EnumerateFiles(projectDirectory, "*", SearchOption.TopDirectoryOnly).Where(fileName => fileName.EndsWith(projectIdentifier)).First();

                processPackages(feedPackages, v1FeedContext, packages, projectFilePath);
            }

            return feedPackages;
        }

        private static IEnumerable<PackageKey> getPackages(string packageConfigPath)
        {
            foreach (var packageElement in XDocument.Load(packageConfigPath).Descendants(packageElementName))
            {
                yield return new PackageKey
                (
                    id: packageElement.Attribute("id").Value,
                    version: packageElement.Attribute("version").Value
                );
            }
        }

        private static void processPackages(Dictionary<PackageKey, FeedPackage> feedPackageDictionary, V1FeedContext feed,
            IEnumerable<PackageKey> packages, string projectFilePath)
        {
            var projectName = Path.GetFileNameWithoutExtension(projectFilePath);

            foreach (var package in packages)
            {
                if (feedPackageDictionary.ContainsKey(package))
                {
                    feedPackageDictionary[package].ProjectNames.Add(projectName);
                }
                else
                {
                    var v1FeedPackages = feed.Packages.Where(x => x.Id == package.Id).ToList();

                    var currentVersion = v1FeedPackages.FirstOrDefault(x => x.Version == package.Version);

                    var latestVersion = v1FeedPackages.OrderByDescending(x => x, new FeedPackageVersionComparer()).FirstOrDefault();

                    feedPackageDictionary.Add(package, new FeedPackage
                    {
                        CurrentVersion = currentVersion,
                        LatestVersion = latestVersion,
                        ProjectNames = new List<string>
                        {
                            projectName
                        },
                    });
                }
            }
        }
    }
}