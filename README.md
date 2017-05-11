# Metal
A toy compiler written in C#

This project is a work in progress and is based on the [Crafting Interpreters](http://www.craftinginterpreters.com/) series.
I started this project so I can learn about programming languages and their grammars. I recently completed a course in Language Processing so
I decided to attempt to try to create a language that is similar to JavaScript. The goal, however, is to add simple types to reduce bugs and make
programming a little more enjoyable. At the moment, the language will be using an interpreter but will eventually be compiled later on as the project
progresses. The following is what I would like accomplish:

```js
/*
 Metal type system
 
 Basic types:
  - char
  - string
  - number (int & double)
  - int
  - double
  - bool
*/

# variable 'a' is a string
var a = "a";
# constant 'b' is an integer
let b = 0;
# constant 'ab' is a string
let ab = a + b;

# an error occurs
let c: double = "0.40";

/*
  Declaring functions
*/
fn isTrue(): bool {
  return true;
}

```
