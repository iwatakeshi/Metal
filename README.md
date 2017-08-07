# Metal
A fun and simple languange for smarties.

Inspired by and based on the [Crafting Interpreters](http://www.craftinginterpreters.com/) series, Metal is a language that aims to be fun and simple. While there are newer langauges like Swift, TypeScript, etc., they either tend to bloat the language with features that you probably won't use, or not have a feature that you really need. For example, in Swift, you can use the range (`...`) operator and slice an array:

```swift
// declare an array
var a = [0, 1, 2, 3, 4];
// b is now [1, 2, 3], cool!
var b = a[1...3]
```
However, when you try use the range operator in descending order (`3...1`), Swift will throw an error :confused:. 

Obviously, TypeScript doesn't have a range operator :cry: but, Metal does! :smile:. Here's a working/proposed example:

```metalupa

# delcare a variable
var a = [4, 3, 5, 1, 6,]
# 'b' is now '[3, 4, 1]', cool!
var b = a[1..3]

# assign 'b' in descending
b = a[3..1]

# outputs '[1, 5, 3]', yay!
print(b)

```

Cool isn't it? You may be wondering what is the goal for Metal. Well, Metal will ultimately be a typed general-purpose language. However, it won't be super strict unless you specify it. Thus, if you explicitly assign a type to a variable, that variable can only be assigned to the type you specified. It's essentially like TypeScript. However, unlike TypeScript, `undefined` does not exist in Metal. Only the [infamous](https://en.wikipedia.org/wiki/Null_pointer#History) `null`. There are other ideas that I have about Metal. For example, it's obvious that the web should be taken into consideration when designing a language. Maybe adding language-extensions to support front-end and back-end developers could be an idea. Basically, the language's grammar can be extended by "extensions" or "modules" that add, for example, HTML's syntax to the grammar.

## Metal Types

### char
```metalupa
var a = 'x'
```

### string
```metalupa
var a = "x"
```

### number
```metalupa
# integer
var a = 1
# floating point (double)
b = 1.5
```

### bool
```metalupa
var a = true
a = false
```

### array
```metalupa
var a = [1, 3]
```
### range
```metalupa
var a = 1..4
```

## Variables
```metalupa
# Current syntax
var a = "hello"

# Proposed syntax
var a = "hello"
var a: string = "hello"
# and
let a = "hello"
let a: string = "hello"
```

## Functions

### func
```metalupa
# Current syntax
func greet(name) {
  print(name)
}

# Proposed syntax
func greet(name) {
  print(name)
}
# and
func greet(name: string) {
  print(name)
}
# and
function greet(name: string): string {
  return "Hello " + name;
}
```

### lambda
```metalupa
# Current syntax
func () { }

# Proposed syntax
() => { }
```


## Loops

### for-loop
```metalupa
# Current syntax
for i in 0..20 {
  print(i)
}

# Proposed syntax
for i in 0..20 {
 print(i)
}
# and
for (item, index) in 0..20 {
  print("Current item " + item + " at index: " + index)
}
```
### while-loop
```metalupa
var count = 0
while (i < 10) print(i = i + 1)
```
### repeat-while
```metalupa
var i = 0
repeat { 
  print("hello")
} while ((i = i + 1) < 10)
```

## Supported Operators

### Unary operators

* Not (`!a`)
* Negate (`-a`)

### Binary operators

* Add (`a + b`)
* Subract (`a - b`)
* Divide (`a / b`)
* Multiply (`a * b`)

* And (`a and b`)
* Or (`a or b`)

* Range (`a..b`)

* Less-than (`a < b`)
* Less-than or equal-to (`a <= b`)
* Greater-than (`>`)
* Greater-than or equal-to (`a >= b`)
* Equal-to (`a == b`)
* Not Equal-to (`a != b`)

### Ternary operators

* Conditional (`a ? b : c`)

### Other operators

* Assignment (`a = b`)
