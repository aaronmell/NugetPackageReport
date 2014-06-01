using System;
using System.IO;
using System.Linq;

namespace NugetPackageReport
{
    class Program
    {
        private static string inputPath;
        private static string outputPath;

        static void Main(string[] args)
        {
	       
			if (!ValidateArguments(args))
            {
                return;
            }

            Processor.ProcessDirectory(inputPath);
            
            ReportWriter.GenerateReport(inputPath, outputPath, Processor.FeedPackages);  
        }

        private static bool ValidateArguments(string[] args)
        {
			if (args.Count() != 2)
			{
				Console.WriteLine("Invalid Arguments. Two Arguments are required. Input Path and Ouput File Path are required.");
				return false;
			}

			var result = TestPath(args[0], "Input");

			if (!result)
				return result;

			inputPath = args[0];

			result = TestPath(args[1], "Output");

			if (!result)
				return result;

			outputPath = args[1];

			return true;
        }

        private static bool TestPath(string path, string outputMessageType)
        {
            try
            {
                var inputPath = Path.GetFullPath(path);

                if (!Directory.Exists(inputPath))
                {
                    Console.WriteLine("The {0} directory does not exist!", outputMessageType);
                    return false;
                }
                return true;
           }
           catch(Exception)
           {
               Console.WriteLine("The {0} directory is not a valid file path",outputMessageType);
           }
            return false;
        }
    }
}
