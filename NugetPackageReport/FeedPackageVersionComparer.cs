using NugetPackageReport.Nuget;
using System.Collections.Generic;
using System.Linq;

namespace NugetPackageReport
{
	/// <summary>
	/// A Custom Comparer used to sort packages by their verison number
	/// </summary>
    internal class FeedPackageVersionComparer : IComparer<V1FeedPackage>
    {
		/// <summary>
		/// Compares two objects and returns a value indicating whether one is less than,
		///     equal to, or greater than the other.
		/// </summary>
		/// <param name="x">The first package to compare</param>
		/// <param name="y">The second package to compare</param>
		/// <returns>A signed integer that indicates the relative values of x and y. When the result is less that zero X is less than Y. 
		/// When the result is 0 X and Y are equal and when the results are greater than zero X is greater than Y 
		/// </returns>
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
