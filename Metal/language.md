# Metal-Lang

### 1. Lexical units
The first phase of running an metal program is lexical analysis which brakes the program up into tokens. A token is the smallest unit metal source code.  Metal tokens all follow into the following categories

#### Token classes
- [Identifier](#Identifier)
- [String Literal](#StringLit)
- [Int literal](#Intlit)
- [Float literal](#FloatLit)
- [Keywords](#Keywords)
- [Punctuators](#Punctuators)
- [Operators](#Operators)

#### 1.1 Identifier <a id="Identifier"></a>
In metal an identifier is string of alphanumeric characters used to identify a structure in code
```bnf
<identifier> ::= <character> { <character> | <digit> }
<character> ::= <alphabet> | "_" | "$"
<alphabet>     ::= "a" ... "z" | "A" ... "Z"
<digit>      ::= "0" ... "9"
```
#### 1.2 String Literal <a id="Stringlit"></a>
A string is surrounded with quotation marks 
``` bnf
<string-literal> ::= "\"" {<ascii-char> | <escape-seq>} "\""
<ascii-char>     ::= "\x00" ... "\xFF"
<escape-seq>     ::= "\\" <escape>
<escape>         ::= "n" | "t" | "r" | "b"
```

#### 1.3 Int literal <a id="IntLit"></a>
An int literal consists of a numeric non decimal value
```bnf
<int-literal> ::= <digit> | {<digit>}
<digit>      ::= "0" ... "9"
```
#### 1.4 Float literal <a id="FloatLit"></a>
A float literal consists of a numeric decimal value
```bnf
<float-literal> ::= <int-literal> "." <int-literal>
``` 

#### 1.5 Keywords <a id="Keywords"></a>
Keywords are reserved words that can not be used as identifiers. The following are iodine keywords
```
if else for func class use self foreach in true false null lambda try except break continue params super return from
```

#### 1.6 Punctuators <a id="Punctuators"></a>
Punctuators are special characters that are used for separation, or grouping. The following are valid iodine punctuators

```
{ } [ ] ( ) , . ; :
```
#### 1.7 Operators
Operators are used in expressions and are either unary or binary. The following are operators in iodine
```
== != <= >= => = && || << >> += -= *= /= &= ^= |= <<= >>= + - / * % & ^ |  ?? is isnot as
```
### 2. Semantic Units

#### 2.1 Expressions
An iodine expression is a sequence of operators, operands and constants that return a value.
##### 2.1.1 Operator Precedence 
| Precedence   |      Operator      |  Associativity |
|----------|:-------------|------:|
| 0 |()<br>[]<br>.        | Left to right|
| 1 |!<br>-<br>~          | Right to left|
| 2 |!<br>-<br>~          | Right to left|
| 3 |*<br>/<br>%          | Left to right|
| 5 |+<br>-               | Left to right|
| 6 |<<<br>>>             | Left to right|
| 7 |<<br>><br><=<br>>=<br>is isnot as| Left to right|
| 8 |==<br>!=             | Left to right|
| 9 |&                    | Left to right|
| 10 |^                   | Left to right|
| 11 |&#124;              | Left to right|
| 12 |&&                  | Left to right|
| 13 |&#124;&#124;<br>??        |Left to right|
| 14 |=<br>+=<br>-=<br>*=<br>/=<br>%=<br><<=<br>>>=<br>&#124;=<br>&=<br>^=   |Left to right|

##### 2.1.2 Lambda Expressions
Anonymous functions can be created using the lambda keyword. Anonymous functions have access to all local variables in their parent function. The syntax for the lambda keyword is
```
lambda () => statement
```

##### 2.1.3 Self reference
Inside instance methods, the self reference points to the current object instance. The self reference can be accessed via the ```self``` keyword.


#### 2.2 Statements

##### 2.2.1 Class Declaration
Classes in Iodine are defined using the ```class``` keyword. The syntax for declaring a class is 
```
 class MyClass {
 
 }
```
Or
```
class MyClass : BaseClass {

}
```
Classes may contain function definitions. A class can extend another class, or multiple interfaces seperated by a comma

##### 2.2.2 Interface Declaration
Interfaces in Iodine are defined using the ```interface``` keyword. The syntax for declaring a interface is
```
interface IMyInterface {
    func aFunction ();
}
```
Only function prototypes are allowed inside the interface body. An inteface may not extend another class or interface.

##### 2.2.3 Enum Declaration
Enumerations in Iodine are defined using the ```'enum``` keyword. The syntax for declaring an ```enum``` is
```
enum MyEnum {
    VALUE_1,
    VALUE_2
}
```
Values may also be assigned a constant numeric value. For example
```
enum MyEnum {
    VALUE_1 = 0,
    VALUE_2 = 1
}
```
##### 2.2.4 Function Declarations
Functions can be declared using the ```func``` keyword. A function contain a list of statements. In Iodine, control statements are only valid inside functions. Classes, enums, and interfaces however may also be declared inside a function. The syntax for declaring a function is
```
func myFunction () {

}
```
A function may also have parameters which are deliminated by commas
```
func myFunction (param1, param2) {

}
```
If the first parameter is named ```self```, then the function is an instance method. Instance methods are typically declared inside classes, however they are also valid outside classes. When calling instances methods, the ```self``` parameter can be ignored.
```
func instanceMethod (self) {

}
```
Variadic functions are supported by using the ```params``` keyword.
```
func variadicFunction (params args) {

}
```
The params keyword must be used on the final parameter. The final parameter will be a tuple containing all extra arguments that were passed to the function.

##### 2.2.5 ```if``` statement
The ```if``` statement can be used to test if a condition is true or false. If the condition is true, the body of the if statement wil be executed. If false, the optional else body will be executed. The syntax for the if statement is
```
if (expression) 
    statement;
else
    statement;
```

##### 2.2.6 ```while``` Statement
The ```while``` loop can be used to continuously execute a block of code while a condition remains true.  The syntax for a while loop is 
```
while (expression)
    statement;
```

##### 2.2.7 ```for``` Statement
The ```for``` loop can be used to continuously execute a block of code while a condition remains true while executing a single expression. The syntax for a for loop is
```
for (initializer; condition; afterthought)
    statement;
````

##### 2.2.8 ```foreach``` Statement
The ```foreach``` loop can be used to iterate through an iterable object. The syntax for the foreach loop is
```
foreach (identifier in expression) 
    statement;
```
##### 2.2.9 ```switch``` Statement
The switch/case statement can be used to test a given value against a series of possible values. In the event that a value is not present, the default statement (If it exists) will be executed.
```
switch (expression) {
    case (expression)
        statement;
    case (expression)
        statement;
    default
        statement
}
```
##### 2.2.10 ```try``` Statement
The try/except statement can be used to catch an exception if raised inside the try block. The syntax is
```
try
    statement;
except
    statement;
```
Additionally, if an exception is thrown it can be stored in a local variable. This is declared in the except statement.
```
try
    statement;
except (identifier)
    statement;
```
The except block can also be written to only catch certain exceptions. The syntax of this is
```
try
    statement;
except (identifier as Type)
    statement;
```
Where identifier is the name of the variable the current exception is to be stored in and Type is the type of the exception that is expected. Multiple types can be specified.
```
try
    statement;
except (identifier as Type1, Type2)
    statement;
```
##### 2.2.11 ```raise``` Statement
The raise statement can be used to throw an exception. The syntax for raise is
```
raise expression;
```
With expression being an expression that returns a value deriving Exception.