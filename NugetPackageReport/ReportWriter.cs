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
	        GenerateGeneralPage(inputFilePath, outputFilePath, feedPackages);
        }

		private static void GenerateGeneralPage(string inputFilePath, string outputFilePath, Dictionary<PackageConfig, FeedPackage> feedPackages)
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
						WritePackageRows(writer, package);
					}

					writer.RenderEndTag();

					var content = writer.InnerWriter.ToString();
					var path = Path.Combine(outputFilePath, "Overview.html");
					File.WriteAllText(path, content);
					System.Diagnostics.Process.Start(path);
				}
			}
		}

        private static void WritePackageRows(HtmlTextWriter writer, KeyValuePair<PackageConfig, FeedPackage> package)
        {
            if (package.Value.CurrentVersion.Version != package.Value.LatestVersion.Version)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Bgcolor, "#CCCC00;");
            }
          
            writer.RenderBeginTag(HtmlTextWriterTag.Tr);
	        {
		        writer.WriteTableColumn(package.Value.CurrentVersion.Id);

		        writer.WriteTableColumn(package.Value.CurrentVersion.Version);

		        writer.WriteTableColumn(package.Value.LatestVersion.Version);

		        writer.RenderBeginTag(HtmlTextWriterTag.Td);
		        {
			        if (!string.IsNullOrEmpty(package.Value.LatestVersion.LicenseUrl))
			        {
				        writer.WriteUrlLink(package.Value.CurrentVersion.LicenseUrl, "License");
			        }
			        else
			        {
				        writer.Write("No License Url");
			        }
		        }
		        writer.RenderEndTag();

		        writer.WriteTableColumn(package.Value.Count.ToString());
	        }
	        writer.RenderEndTag();
        }

        private static void WriteTableHeaders(HtmlTextWriter writer, params string[] tableHeaderNames)
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
