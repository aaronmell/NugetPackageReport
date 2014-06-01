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
		public string Id { get; set; }

		/// <summary>
		/// The version of the package
		/// </summary>
		internal string Version { get; set; }
	}
}
