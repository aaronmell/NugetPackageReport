using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Web.UI;
using System.Linq;

namespace NugetPackageReport
{
    public static class ReportWriter
    {
	    private const string GeneralPageFileName = "Overview.html";
	    private const string ProjectPageFileName = "Projects.html";

        public static void GenerateReport(string inputFilePath, string outputFilePath, Dictionary<PackageConfig, FeedPackage> feedPackages)
        {
	       
	        GenerateProjectPage(inputFilePath, outputFilePath, feedPackages);
			GenerateGeneralPage(inputFilePath, outputFilePath, feedPackages);
        }


	    private static void GenerateProjectPage(string inputFilePath, string outputFilePath,
	                                            Dictionary<PackageConfig, FeedPackage> feedPackages)
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
							writer.RenderBeginTag(HtmlTextWriterTag.B);
							{
								writer.WriteLine("Package Id: {0} Version: {1}", package.Value.CurrentVersion.Id, package.Value.CurrentVersion.Version);
								writer.WriteBreak();
							}
							writer.RenderEndTag();

							foreach (var project in package.Value.ProjectNames)
							{
								writer.WriteLine(project);
								writer.WriteBreak();
							}
							writer.WriteBreak();
						}

					var content = writer.InnerWriter.ToString();

					var path = Path.Combine(outputFilePath, ProjectPageFileName);
					File.WriteAllText(path, content);
			    }
		    }
	    }

	    private static void GenerateGeneralPage(string inputFilePath, string outputFilePath, Dictionary<PackageConfig, FeedPackage> feedPackages)
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


					var projectPath = Path.Combine(outputFilePath, ProjectPageFileName);
					writer.WriteUrlLink(new Uri(projectPath).AbsoluteUri, "Project by Package");
					writer.WriteBreak();

					writer.RenderBeginTag("table");
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

        private static void WriteGeneralPagePackageRows(HtmlTextWriter writer, KeyValuePair<PackageConfig, FeedPackage> package)
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
	
		private static void WriteGeneralInformation(this HtmlTextWriter writer, string inputFilePath,
		                                            Dictionary<PackageConfig, FeedPackage> feedPackages)
		{
			writer.WriteBreak();
					writer.WriteLine("Searched Directory {0}",inputFilePath);
					writer.WriteBreak();
					writer.WriteLine("Found {0} Nuget Packages in {1} projects", feedPackages.Count,
					                 feedPackages.Values.SelectMany(x => x.ProjectNames).Distinct().Count());
					writer.WriteBreak();
		}
	}
}
