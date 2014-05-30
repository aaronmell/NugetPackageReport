using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace NugetPackageReport
{
    public static class ReportWriter
    {
        public static void GenerateReport(string inputFilePath, string outputFilePath, Dictionary<PackageConfig, FeedPackage> feedPackages)
        {
            using (var stringWriter = new StringWriter())
            {
                using (var writer = new HtmlTextWriter(stringWriter))
                {
                    writer.Write("Nuget Package Report for directory {0}", inputFilePath);
                    writer.WriteBreak();
                    writer.Write("Found {0} Nuget Packages", feedPackages.Count);
                    writer.WriteBreak();
                    writer.WriteBreak();

                    writer.RenderBeginTag("table");

                    WriteTableHeaders(writer, "Package Name", "Current Version", "Latest Version", "License", "Instances");

                    foreach (var package in feedPackages)
                    {
                        WritePackageRow(writer, package);
                    }

                    writer.RenderEndTag();

                    var content = writer.InnerWriter.ToString();
                    var path = Path.Combine(outputFilePath, "Overview.html");
                    File.WriteAllText(path, content);
                    System.Diagnostics.Process.Start(path);
                }
            }
        }

        private static void WritePackageRow(HtmlTextWriter writer, KeyValuePair<PackageConfig, FeedPackage> package)
        {
            if (package.Value.CurrentVersion.Version != package.Value.LatestVersion.Version)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Bgcolor, "#CCCC00;");
            }
          
            writer.RenderBeginTag(HtmlTextWriterTag.Tr);

            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.Write(package.Value.CurrentVersion.Id);
            writer.RenderEndTag();

            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.Write(package.Value.CurrentVersion.Version);
            writer.RenderEndTag();

            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.Write(package.Value.LatestVersion.Version);
            writer.RenderEndTag();

            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            if (!string.IsNullOrEmpty(package.Value.LatestVersion.LicenseUrl))
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Href, package.Value.CurrentVersion.LicenseUrl);
                writer.AddAttribute(HtmlTextWriterAttribute.Target, "_blank");
                writer.RenderBeginTag(HtmlTextWriterTag.A);
                writer.Write("License");
                writer.RenderEndTag();
            }
            else
            {
                writer.Write("No License Url");
            }

            writer.RenderEndTag();

            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.Write(package.Value.Count);
            writer.RenderEndTag();

            writer.RenderEndTag();
        }

        private static void WriteTableHeaders(HtmlTextWriter writer, params string[] tableHeaderNames)
        {
            writer.RenderBeginTag(HtmlTextWriterTag.Tr);

            foreach (var name in tableHeaderNames)
            {
                writer.RenderBeginTag(HtmlTextWriterTag.Th);
                writer.Write(name);
                writer.RenderEndTag();
            }

            writer.RenderEndTag();
        }
    }
}
