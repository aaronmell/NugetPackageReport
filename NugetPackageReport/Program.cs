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

        private static bool handleArguments(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Invalid Arguments. Two Arguments are required. Input Path and Ouput File Path are required.");
                return false;
            }

            var validPaths = testPath(args[0], "Input") && testPath(args[1], "Output");

            if (!validPaths)
                return false;

            inputPath = args[0];
            outputPath = args[1];

            return true;
        }

        /// <summary>
        /// Entry point for the program.
        /// </summary>
        /// <param name="args">Arguments for the program. First argument is the input path; second argument is the output path.</param>
        private static void Main(string[] args)
        {
            if (!handleArguments(args))
                return;

            var results = Processor.Process(inputPath);

            ReportWriter.GenerateReport(inputPath, outputPath, results);
        }

        private static bool testPath(string path, string directoryType)
        {
            if (!Directory.Exists(path))
            {
                Console.WriteLine("Can't access the {0} directory!", directoryType);
                return false;
            }

            return true;
        }
    }
}