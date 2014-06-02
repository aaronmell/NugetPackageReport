using NugetPackageReport.Nuget;
using System;
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

            for (int i = 0; i < Math.Min(splitStringsX.Length, splitStringsY.Length); i++)
            {
                var result = CheckVersion(splitStringsX[i], splitStringsY[i]);

                if (result != 0)
                    return result;
            }

            if (splitStringsX.Length > splitStringsY.Length)
                return 1;

            if (splitStringsX.Length < splitStringsY.Length)
                return -1;

            return 0;
        }

        private int CheckVersion(string x, string y)
        {
            var intX = int.Parse(x);
            var intY = int.Parse(y);

            if (intX > intY)
                return 1;

            if (intX < intY)
                return -1;

            return 0;
        }
    }
}