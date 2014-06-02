using NugetPackageReport.Nuget;
using System.Collections.Generic;

namespace NugetPackageReport
{
    /// <summary>
    /// A model that contains the package found from the nuget package repository
    /// </summary>
    internal class FeedPackage
    {
        /// <summary>
        /// The current version of the package. This can be null if using a package that is not part of the standard nuget feed.
        /// </summary>
        internal V1FeedPackage CurrentVersion { get; set; }

        /// <summary>
        /// The latest version of the package. This can be null if using a package that is not part of the standard nuget feed.
        /// </summary>
        internal V1FeedPackage LatestVersion { get; set; }

        /// <summary>
        /// A list of project names that contain a package.config file in them with the given package.
        /// </summary>
        internal List<string> ProjectNames { get; set; }
    }
}