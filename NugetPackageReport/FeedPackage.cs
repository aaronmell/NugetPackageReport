using NugetPackageReport.Nuget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetPackageReport
{
    public class FeedPackage 
    {
        public V1FeedPackage CurrentVersion {get; set;}

        public V1FeedPackage LatestVersion { get; set; }

        public int Count { get; set; }
    }
}
