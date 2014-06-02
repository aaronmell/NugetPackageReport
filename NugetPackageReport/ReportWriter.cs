using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.UI;

namespace NugetPackageReport
{
    /// <summary>
    /// The Report Writer class. Handles all functions related to generation of the different html pages
    /// </summary>
    internal static class ReportWriter
    {
        private const string GeneralPageFileName = "Overview.html";
        private const string ProjectPageFileName = "Projects.html";

        /// <summary>
        /// Generates the Html Reports
        /// </summary>
        /// <param name="inputFilePath">The input path that was searched</param>
        /// <param name="outputFilePath">The path to the output directory of the report</param>
        /// <param name="feedPackages">The packages that were found during the search</param>
        internal static void GenerateReport(string inputFilePath, string outputFilePath, Dictionary<PackageKey, FeedPackage> feedPackages)
        {
            GenerateGeneralPage(inputFilePath, outputFilePath, feedPackages);
            GenerateProjectPage(inputFilePath, outputFilePath, feedPackages);
            GeneratePackagePages(outputFilePath, feedPackages);
        }

        private static void GenerateGeneralPage(string inputFilePath, string outputFilePath, Dictionary<PackageKey, FeedPackage> feedPackages)
        {
            using (var stringWriter = new StringWriter())
            {
                using (var writer = new HtmlTextWriter(stringWriter))
                {
                    writer.RenderBeginTag(HtmlTextWriterTag.H1);
                    {
                        writer.WriteLine("Nuget Package Report Overview");
                    }
                    writer.RenderEndTag();

                    writer.WriteGeneralInformation(inputFilePath, feedPackages);
                    writer.WriteBreak();

                    writer.RenderBeginTag(HtmlTextWriterTag.Table);
                    {
                        WriteGeneralPageTableHeaders(writer, "Package Name", "Current Version", "Latest Version", "License",
                                                     "Instances");

                        foreach (var package in feedPackages)
                        {
                            WriteGeneralPagePackageRows(writer, package);
                        }
                    }
                    writer.RenderEndTag();

                    var content = writer.InnerWriter.ToString();

                    var path = Path.Combine(outputFilePath, GeneralPageFileName);
                    File.WriteAllText(path, content);
                    System.Diagnostics.Process.Start(path);
                }
            }
        }

        private static void GeneratePackagePages(string outputFilePath, Dictionary<PackageKey, FeedPackage> feedPackages)
        {
            foreach (var package in feedPackages.Where(pckge => pckge.Value.CurrentVersion != null))
            {
                using (var stringWriter = new StringWriter())
                {
                    using (var writer = new HtmlTextWriter(stringWriter))
                    {
                        writer.RenderBeginTag(HtmlTextWriterTag.H1);
                        {
                            writer.WriteLine("Nuget Package Report for {0} {1}", package.Value.CurrentVersion.Id, package.Value.CurrentVersion.Version);
                        }
                        writer.RenderEndTag();

                        writer.WriteBreak();
                        writer.WriteBreak();

                        writer.WritePackageInfoLine("Description", package.Value.CurrentVersion.Description);
                        writer.WritePackageInfoLine("Created", package.Value.CurrentVersion.Created.ToShortDateString());
                        writer.WritePackageInfoLine("Last Updated", package.Value.CurrentVersion.LastUpdated.ToShortDateString());
                        writer.WritePackageInfoLine("Release Notes", package.Value.CurrentVersion.ReleaseNotes);

                        writer.RenderBeginTag(HtmlTextWriterTag.B);
                        {
                            writer.WriteLine("License Url");
                        }
                        writer.RenderEndTag();
                        writer.WriteBreak();

                        if (!string.IsNullOrEmpty(package.Value.CurrentVersion.LicenseUrl))
                        {
                            writer.WriteUrlLink(package.Value.CurrentVersion.LicenseUrl, package.Value.CurrentVersion.LicenseUrl);
                        }
                        else
                        {
                            writer.WriteLine("No License Url");
                        }
                        writer.WriteBreak();
                        writer.WriteBreak();

                        writer.RenderBeginTag(HtmlTextWriterTag.B);
                        {
                            writer.WriteLine("Project Url");
                        }
                        writer.RenderEndTag();
                        writer.WriteBreak();

                        if (!string.IsNullOrEmpty(package.Value.CurrentVersion.ProjectUrl))
                        {
                            writer.WriteUrlLink(package.Value.CurrentVersion.ProjectUrl, package.Value.CurrentVersion.ProjectUrl);
                        }
                        else
                        {
                            writer.WriteLine("No Project Url");
                        }
                        writer.WriteBreak();
                        writer.WriteBreak();

                        writer.RenderBeginTag(HtmlTextWriterTag.B);
                        {
                            writer.WriteLine("Gallery Details Url");
                        }
                        writer.RenderEndTag();
                        writer.WriteBreak();

                        if (!string.IsNullOrEmpty(package.Value.CurrentVersion.GalleryDetailsUrl))
                        {
                            writer.WriteUrlLink(package.Value.CurrentVersion.GalleryDetailsUrl, package.Value.CurrentVersion.GalleryDetailsUrl);
                        }
                        else
                        {
                            writer.WriteLine("No Gallery Details Url");
                        }
                        writer.WriteBreak();
                        writer.WriteBreak();

                        var content = writer.InnerWriter.ToString();

                        var path = Path.Combine(outputFilePath, string.Format("{0}{1}.html", package.Value.CurrentVersion.Id, package.Value.CurrentVersion.Version));
                        File.WriteAllText(path, content);
                    }
                }
            }
        }

        private static void GenerateProjectPage(string inputFilePath, string outputFilePath,
                                                Dictionary<PackageKey, FeedPackage> feedPackages)
        {
            using (var stringWriter = new StringWriter())
            {
                using (var writer = new HtmlTextWriter(stringWriter))
                {
                    writer.RenderBeginTag(HtmlTextWriterTag.H1);
                    {
                        writer.WriteLine("Nuget Package Report Projects by Package");
                    }
                    writer.RenderEndTag();

                    writer.WriteGeneralInformation(inputFilePath, feedPackages);
                    writer.WriteBreak();

                    foreach (var package in feedPackages)
                    {
                        writer.RenderBeginTag(HtmlTextWriterTag.H3);
                        {
                            writer.WriteUrlLink(string.Format("{0}{1}.html", package.Key.Id, package.Key.Version), string.Format("{0} {1}", package.Key.Id, package.Key.Version));
                            writer.WriteBreak();
                        }
                        writer.RenderEndTag();

                        foreach (var project in package.Value.ProjectNames.OrderBy(x => x))
                        {
                            writer.WriteLine(project);
                            writer.WriteBreak();
                        }
                        writer.WriteBreak();
                    }

                    var content = writer.InnerWriter.ToString();

                    var path = Path.Combine(outputFilePath, ProjectPageFileName);
                    File.WriteAllText(path, content);
                    System.Diagnostics.Process.Start(path);
                }
            }
        }

        private static void WriteGeneralInformation(this HtmlTextWriter writer, string inputFilePath,
                                                    Dictionary<PackageKey, FeedPackage> feedPackages)
        {
            writer.WriteBreak();
            writer.WriteLine("Searched Directory {0}", Path.GetFullPath(inputFilePath));
            writer.WriteBreak();
            writer.WriteBreak();
            var projectCount = feedPackages.Values.SelectMany(x => x.ProjectNames).Distinct().Count();
            writer.WriteLine("Found {0} Nuget Packages in {1} {2}", feedPackages.Count,
                             projectCount, projectCount > 1 ? "projects" : "project");
            writer.WriteBreak();
        }

        private static void WriteGeneralPagePackageRows(HtmlTextWriter writer, KeyValuePair<PackageKey, FeedPackage> package)
        {
            if (package.Value.CurrentVersion != null && package.Value.LatestVersion != null &&
                package.Value.CurrentVersion.Version != package.Value.LatestVersion.Version)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Bgcolor, "#CCCC00;");
            }

            writer.RenderBeginTag(HtmlTextWriterTag.Tr);
            {
                if (package.Value.CurrentVersion != null)
                {
                    writer.RenderBeginTag(HtmlTextWriterTag.Td);
                    {
                        writer.WriteUrlLink(string.Format("{0}{1}.html", package.Value.CurrentVersion.Id, package.Value.CurrentVersion.Version), package.Value.CurrentVersion.Id);
                    }
                    writer.RenderEndTag();
                }
                else
                {
                    writer.WriteTableColumn(package.Key.Id);
                }

                writer.WriteTableColumn(package.Key.Version);

                writer.WriteTableColumn(package.Value.LatestVersion != null
                                            ? package.Value.LatestVersion.Version
                                            : "Unknown");

                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                {
                    if (package.Value.CurrentVersion != null && !string.IsNullOrEmpty(package.Value.CurrentVersion.LicenseUrl))
                    {
                        writer.WriteUrlLink(package.Value.CurrentVersion.LicenseUrl, "License");
                    }
                    else
                    {
                        writer.Write("Not Found");
                    }
                }
                writer.RenderEndTag();

                writer.WriteTableColumn(package.Value.ProjectNames.Count.ToString(CultureInfo.InvariantCulture));
            }
            writer.RenderEndTag();
        }

        private static void WriteGeneralPageTableHeaders(HtmlTextWriter writer, params string[] tableHeaderNames)
        {
            writer.RenderBeginTag(HtmlTextWriterTag.Tr);
            {
                foreach (var name in tableHeaderNames)
                {
                    writer.RenderBeginTag(HtmlTextWriterTag.Th);
                    writer.Write(name);
                    writer.RenderEndTag();
                }
            }
            writer.RenderEndTag();
        }

        private static void WritePackageInfoLine(this HtmlTextWriter writer, string title, string value)
        {
            writer.RenderBeginTag(HtmlTextWriterTag.B);
            {
                writer.WriteLine(title);
                writer.WriteBreak();
            }
            writer.RenderEndTag();

            if (!string.IsNullOrEmpty(value))
            {
                writer.WriteLine(value);
            }
            else
            {
                writer.WriteLine("No {0} Provided", title);
            }
            writer.WriteBreak();
            writer.WriteBreak();
        }

        private static void WriteTableColumn(this HtmlTextWriter writer, string valueToWrite)
        {
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.Write(valueToWrite);
            writer.RenderEndTag();
        }

        private static void WriteUrlLink(this HtmlTextWriter writer, string url, string displayValue)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Href, url);
            writer.AddAttribute(HtmlTextWriterAttribute.Target, "_blank");
            writer.RenderBeginTag(HtmlTextWriterTag.A);
            writer.Write(displayValue);
            writer.RenderEndTag();
        }
    }
}