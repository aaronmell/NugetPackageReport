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
        internal string Id { get; private set; }

        /// <summary>
        /// The version of the package
        /// </summary>
        internal string Version { get; private set; }

        internal PackageKey(string id, string version)
        {
            Id = id;
            Version = version;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is PackageKey))
                return false;

            var packageKey = (PackageKey)obj;
            return packageKey.Id == Id && packageKey.Version == Version;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = (hash * 29) + (Id != null ? Id.GetHashCode() : 0);
                hash = (hash * 29) + (Version != null ? Version.GetHashCode() : 0);
                return hash;
            }
        }
    }
}