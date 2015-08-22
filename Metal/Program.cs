using System;
using System.IO;
using CommandLine;
using CommandLine.Text;
using Metal.FrontEnd.Lex;
namespace Metal
{
	class About {
		public static String Version { get { return "0.0.0"; } }
		public static String Name { get { return "Metal"; } }
		public static String License { get { return string.Format ("MIT ({0})", DateTime.Now.Year.ToString ()); } }
	}

	/// <summary>
	/// <description>Options class sets up the options for Command Line</description>
	/// </summary>
	class Options {
		[Option('r', "read", Required = false,
			HelpText = "Input file to be processed.")]
		public string InputFile { get; set; }

		[Option('v', "verbose", DefaultValue = true,
			HelpText = "Prints all messages to standard output.")]
		public bool Verbose { get; set; }

		[ParserState]
		public IParserState LastParserState { get; set; }

		[HelpOption]
		public string GetUsage() {
			return HelpText.AutoBuild(this,
				(HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
		}
	}

	class MainClass
	{
		public static void Main (string[] args)
		{
			Console.WriteLine ("Project: {0} License: {1} Version: {2}", About.Name, About.License, About.Version);
			var options = new Options();
			if (CommandLine.Parser.Default.ParseArguments(args, options)) {
				// Values are available here
				if (options.Verbose) {
					string path = Directory.GetParent (Directory.GetCurrentDirectory ()).Parent.FullName;
					string seperator = Path.DirectorySeparatorChar.ToString ();
					string testPath = path + seperator + "Test" + seperator;
					Scanner scanner = new Scanner (testPath, options.InputFile);
					while(!scanner.IsEOF){
						var token = scanner.NextToken ();
						if(token != null) Console.WriteLine(token);
					}
				}

			}
		}
	}
}
