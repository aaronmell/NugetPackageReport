using System;
using System.IO;
using System.Linq;

namespace NugetPackageReport
{
    /// <summary>
    /// Main Program
    /// </summary>
    public class Program
    {
        private static string inputPath;
        private static string outputPath;

        /// <summary>
        /// Entry point for program
        /// </summary>
        /// <param name="args">Arguments for Program, First argument is the input path and second argument is the output path</param>
        private static void Main(string[] args)
        {
            if (!validateArguments(args))
            {
                return;
            }

            var results = Processor.Process(inputPath);

            ReportWriter.GenerateReport(inputPath, outputPath, results);
        }

        private static bool testPath(string path, string outputMessageType)
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
            catch (Exception)
            {
                Console.WriteLine("The {0} directory is not a valid file path", outputMessageType);
            }
            return false;
        }

        private static bool validateArguments(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Invalid Arguments. Two Arguments are required. Input Path and Ouput File Path are required.");
                return false;
            }

            var result = testPath(args[0], "Input");

            if (!result)
                return false;

            inputPath = args[0];

            result = testPath(args[1], "Output");

            if (!result)
                return false;

            outputPath = args[1];

            return true;
        }
    }
}