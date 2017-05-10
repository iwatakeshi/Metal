﻿﻿using System;
//using CommandLine;
//using CommandLine.Text;
using Metal.FrontEnd.Scan;
using System.Collections.Generic;
using Metal.FrontEnd.Parse;
using Metal.FrontEnd.Parse.Grammar;

namespace Metal {

  public static class Metal {
    private static bool hadError = true;
    static void Main(string[] args) {
            foreach(var arg in args) {
                Console.WriteLine (arg);
            }
      if (args.Length > 1) {
        Console.WriteLine("Usage: metal [file]");
      } else if (args.Length == 1) {
        RunFile(args[0]);
      } else {
        RunPrompt();
      }
    }

    private static void Run(string source, bool isFile) {
      Scanner scanner = new Scanner(source, isFile);
      List<Token> tokens = scanner.ScanTokens();
      Parser parser = new Parser(tokens);
      Expression expression = parser.Parse();

      if (!hadError) {
        Console.WriteLine(new ASTPrinter().Print(expression));
      }
      // foreach (var token in tokens) {
      //   Console.WriteLine(token);
      // }
    }

    private static void RunFile(string source) {
      Run(source, true);
      if (hadError) Environment.Exit(65);
    }

    private static void RunPrompt() {
      Console.WriteLine(About.Name);
      Console.WriteLine(About.Version);
      Console.WriteLine(About.License);
      Console.WriteLine();
      for (;;) {
        Console.Write("> ");
        Run(Console.In.ReadLine(), false);
      }
    }


    public static void Error(int line, string message) {
      Report(line, "", message);
    }

    public static void Error(Token token, string message) {
      if (token.Type == TokenType.EOF) {
        Report(token.Line, " at end", message);
      } else {
        Report(token.Line, " at '" + token.Lexeme + "'", message);
      }
    }
    public static void Report(int line, string where, string message) {
      Console.WriteLine(string.Format("[line {0}] Error {1}: {2}", line, where, message));
      hadError = true;
    }

    class About {
      public static String Author { get { return "Takeshi Iwana"; } }

      public static String Version { get { return "0.0.0"; } }

      public static String Name { get { return "Metal"; } }

      public static String License { get { return String.Format("MIT License \n{0}", Copyright); } }

      public static String Copyright { get { return String.Format("Copyright (c) {0} - {1} {2}", 2015, DateTime.Now.Year.ToString(), Author); } }
    }

  }

  //class About {
  //  public static String Author { get { return "Takeshi Iwana"; } }

  //  public static String Version { get { return "0.0.0"; } }

  //  public static String Name { get { return "Metal"; } }

  //  public static String License { get { return String.Format ("The MIT License (MIT)\n{0}", Copyright); } }

  //  public static String Copyright { get { return String.Format ("Copyright (c) {0} {1}", DateTime.Now.Year.ToString (), Author); } }
  //}

  ///// <summary>
  ///// <description>Options class sets up the options for Command Line</description>
  ///// </summary>
  //class Options {
  //  [Option ('r', "read", Required = false,
  //    HelpText = "Input file to be processed.")]
  //  public string InputFile { get; set; }

  //  [Option ('v', "verbose", DefaultValue = true,
  //    HelpText = "Prints all messages to standard output.")]
  //  public bool Verbose { get; set; }

  //  [ParserState]
  //  public IParserState LastParserState { get; set; }

  //  [HelpOption]
  //  public string GetUsage () {
  //    return HelpText.AutoBuild (this,
  //      (HelpText current) => HelpText.DefaultParsingErrorsHandler (this, current));
  //  }
  //}

  //class MainClass {
  //  public static void Main (string[] args) {
  //    Console.WriteLine ("{0}\n{1}\nVersion: {2}", About.Name, About.License, About.Version);
  //    var options = new Options ();
  //    if (CommandLine.Parser.Default.ParseArguments (args, options)) {
  //      // Values are available here
  //      if (options.Verbose) {
  //        string path = Directory.GetParent (Directory.GetCurrentDirectory ()).Parent.FullName;
  //        string seperator = Path.DirectorySeparatorChar.ToString ();
  //        string testPath = path + seperator + "Source Files" + seperator;
  //        Scanner scanner = new Scanner (testPath, options.InputFile);
  //        scanner.Scan();
  //      }
  //    }
  //  }
  //}
}
