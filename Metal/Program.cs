﻿using System;
using System.Collections.Generic;
using Metal.FrontEnd.Scan;
using Metal.FrontEnd.Parse;
using Metal.FrontEnd.Interpret;
using Metal.FrontEnd.Grammar;
using Metal.FrontEnd.Exceptions;
using System.Text;
using System.Text.RegularExpressions;

namespace Metal {

  public static class Metal {
    private static bool hadError = false;
    private static bool hadRuntimeError = false;
    private static Interpreter interpreter = new Interpreter();
    private static StringBuilder lines = new StringBuilder();

    static void Main(string[] args) {
      foreach (var arg in args) {
        Console.WriteLine(arg);
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
      if (!hadError) {
        //Console.WriteLine(new ASTPrinter().Print(expression));
        try {
          Scanner scanner = new Scanner(source, isFile);
          List<Token> tokens = scanner.ScanTokens();
          Parser parser = new Parser(tokens);
          object syntax = parser.ParseREPL();
          if (syntax is List<Statement>) {
            interpreter.Interpret((List<Statement>)syntax);
          } else if (syntax is Expression) {
            string result = interpreter.Interpret((Expression)syntax);
            if (!string.IsNullOrEmpty(result)) Console.WriteLine(result);
          }

        } catch (Exception error) {
          Console.WriteLine(error.Message);
          Console.WriteLine(error);
        }
      } else Console.WriteLine("An error occurred.");
    }

    private static void RunFile(string source) {
      Run(source, true);
      if (hadError) Environment.Exit(65);
      if (hadRuntimeError) Environment.Exit(70);
    }

    private static void RunPrompt() {
      About.Print();
      for (;;) {
        Console.Write("> ");
          lines.AppendLine(Console.In.ReadLine());

        if (lines.ToString().Contains("clear")) {
          hadError = false;
          interpreter.ResetEnvironment();
          lines.Clear();
          Console.Clear();
          About.Print();
        } else {
          var braceCount = 0;
          var line = Regex.Replace(lines.ToString(), @"\t|\n|\r", "");
          if (line[line.Length - 1] == '{') {
            // Initially count all '{' and assume that
            // they are correctly closed with '}'
            foreach(var ch in line) {
              if (ch == '{') braceCount++;
            }

            while (braceCount != 0) {
              if (!hadError && !hadRuntimeError) {
                Console.Write("{0} ", new String('.', braceCount * 3));
              }

              line = Console.In.ReadLine();

              if (line.Contains("{") || line.Contains("}")) {
                foreach (var ch in line) {
                  if (ch == '{') braceCount++;
                  else if (ch == '}') braceCount--;
                }
              }
              lines.AppendLine(line);
              //Console.WriteLine(lines.ToString());
            }
          }
          Run(lines.ToString(), false);
        }
        lines.Clear();
      }
    }

    public static void Error(int line, string message) {
      Report(line, "", message);
    }

    public static void Error(Token token, string message) {
      if (token.Type == TokenType.EOF) {
        Report(token.Line, "at end", message);
      } else {
        Report(token.Line, string.Format("at '{0}'", token.Lexeme), message);
      }
    }
    public static void Report(int line, string where, string message) {
      Console.WriteLine(string.Format("[line {0}] Error {1}: {2}", line, where, message));
      hadError = true;
    }

    public static void RuntimeError(MetalException.Runtime error) {
      if (error.Token == null) {
        Console.WriteLine("Error: {0}", error.Message);
      } else Console.WriteLine("[line {0}] Error: {1}", error.Token.Line, error.Message);
      hadRuntimeError = true;
    }

    class About {
      public static String Author { get { return "Takeshi Iwana"; } }

      public static String Version { get { return "0.0.0"; } }

      public static String Name { get { return "Metal"; } }

      public static String License { get { return String.Format("MIT License \n{0}", Copyright); } }

      public static String Copyright { get { return String.Format("Copyright (c) {0} - {1} {2}", 2015, DateTime.Now.Year.ToString(), Author); } }

      public static void Print() {
        Console.WriteLine(About.Name);
        Console.WriteLine(About.Version);
        Console.WriteLine(About.License);
        Console.WriteLine();
      }
    }
  }
}
