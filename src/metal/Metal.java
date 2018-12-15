package metal;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.nio.charset.Charset;
import java.nio.file.Files;
import java.nio.file.Paths;
import java.util.List;

import metal.frontend.scanner.*;
// should these be bin/production/Metal/... since that's where the classes reside?
// added TT
// TI: LGTM. No need to add bin/. The .classpath file let's vscode know where to output the classes
import metal.frontend.parser.*;
import metal.frontend.parser.grammar.*;
import metal.utilities.*;

public class Metal {
  static boolean hadError = false;
  public static void main(String[] args) throws IOException {
    if (args.length > 1) {
      System.out.println("Usage: metal [script]");
      System.exit(64);
    } else if (args.length == 1) {
      runFile(args[0]);
    } else {
      runREPL();
    }
  }

  /**
   * Executes a file
   */
  private static void runFile(String path) throws IOException {
    byte[] bytes = Files.readAllBytes(Paths.get(path));
    run(new String(bytes, Charset.defaultCharset()));
  }

  /**
   * Executes the repl
   * @throws IOException
   */
  private static void runREPL() throws IOException {
    InputStreamReader input = new InputStreamReader(System.in);
    BufferedReader reader = new BufferedReader(input);

    for (;;) {
      System.out.print("> ");
      run(reader.readLine());
    }
  }

  /**
   * Executes Metal
   */
  private static void run(String source) {    
    Scanner scanner = new Scanner(source);    
    List<Token> tokens = scanner.scanTokens();

    // // For now, just print the tokens.        
    // for (Token token : tokens) {              
    //   System.out.println(token);              
    // } 
    
    //added TT
    Parser parser = new Parser(tokens);                    
    Expression expression = parser.parse();

    // Stop if there was a syntax error.                   
    if (hadError) return;                                  

    System.out.println(new ASTPrinter().print(expression));                                        
  }
  
  public static void error(int line, int column, String message) {                       
    report(line,column, "", message);                                        
  }

  private static void report(int line, int column, String where, String message) {
    System.err.println(                                               
        "[Line " + line + " Column " + column + "] Error" + where + ": " + message);        
    hadError = true;                                                  
  } 

  public static void error(Token token, String message) {              
    if (token.type == TokenType.EOF) {                          
      report(token.line, token.column, " at end", message);                   
    } else {                                                    
      report(token.line, token.column, " at '" + token.lexeme + "'", message);
    }                                                           
  }  

}
