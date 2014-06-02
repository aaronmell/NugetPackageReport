namespace NugetPackageReport
{
    /// <summary>
    /// a composite key used to uniquely identify a package
    /// </summary>
    internal class PackageKey
    {
        /// <summary>
        /// The Name of the Package
        /// </summary>
        internal string Id { get; set; }

        /// <summary>
        /// The version of the package
        /// </summary>
        internal string Version { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is PackageKey))
                return false;

            var packageKey = (PackageKey)obj;
            return packageKey.Id == Id && packageKey.Version == Version;
        }

        public override int GetHashCode()
        {
            return Version.GetHashCode() + Id.GetHashCode();
        }
    }
}