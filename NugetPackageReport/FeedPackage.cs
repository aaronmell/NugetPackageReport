using NugetPackageReport.Nuget;
using System.Collections.Generic;

namespace NugetPackageReport
{
    public class FeedPackage 
    {
        public V1FeedPackage CurrentVersion {get; set;}

        public V1FeedPackage LatestVersion { get; set; }

		public List<string> ProjectNames { get; set; }
        
    }
}
