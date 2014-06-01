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
        private static string _inputPath;
        private static string _outputPath;

		/// <summary>
		/// Entry point for program
		/// </summary>
		/// <param name="args">Arguments for Program, First argument is the input path and second argument is the output path</param>
        static void Main(string[] args)
        {
	       
			if (!ValidateArguments(args))
            {
                return;
            }

            var results = Processor.Process(_inputPath);
            
            ReportWriter.GenerateReport(_inputPath, _outputPath, results);  
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
				return false;

			_inputPath = args[0];

			result = TestPath(args[1], "Output");

			if (!result)
				return false;

			_outputPath = args[1];

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
               Console.WriteLine("The {0} directory is not a valid file path", outputMessageType);
           }
            return false;
        }
    }
}
