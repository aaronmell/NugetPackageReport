using NugetPackageReport.Nuget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetPackageReport
{
    public class FeedPackageVersionComparer : IComparer<V1FeedPackage>
    {
        public int Compare(V1FeedPackage x, V1FeedPackage y)
        {
            var versionX = x.Version;

            var versionY = y.Version;

            if (versionX == versionY)
                return 0;

            var splitStringsX = versionX.Split('.');
            var splitStringsY = versionY.Split('.');

            var result = CheckVersion(splitStringsX[0], splitStringsY[0]);

            if (result != 0)
                return result;

            result = CheckVersion(splitStringsX.Count() >= 2 ? splitStringsX[1] : "0", splitStringsY.Count() >= 2 ? splitStringsY[1] : "0");

            if (result != 0)
                return result;

            result = CheckVersion(splitStringsX.Count() >= 3 ? splitStringsX[2] : "0", splitStringsY.Count() >= 3 ? splitStringsY[2] : "0");

            if (result != 0)
                return result;

            return CheckVersion(splitStringsX.Count() >= 4 ? splitStringsX[3] : "0", splitStringsY.Count() >= 4 ? splitStringsY[3] : "0");
        }

        private int CheckVersion(string x, string y)
        {
            var intX = int.Parse(x);
            var intY = int.Parse(y);

            if (intX > intY)
                return 1;

            if (intY > intX)
                return -1;

           return 0;
        }
    }
}
