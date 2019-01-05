# JsonQueryRuntime

## Overview

JsonQueryRunTime is a .NET library to apply where clause like expression to 
- JSON string
- JSON string list 
- JSON-Lines file.

[JSON-Lines](http://jsonlines.org/) is a text format that contains lines of JSON string, each line is a valid JSON object. 

## JSON Query Syntax

**JSON sample**
```json
{ 
	"n":1, "b":true, "s":"string", "timestamp":"2018-12-20T14:16:32", 
	"o":{ "name":"foo", "b": true, "nil" : null }, "a":[1,2,3] 
}
```
### Query Samples

```js
n = 1 AND s = "string" AND b = true
```
returns true if the JSON properties match each expression 

**Arithmetic operation**

```js
( n * 3 = 3 ) AND ( n / 2 = 0.5 )
```
returns true if the JSON properties match each expression

**Nested object**

```js
o.name = 'foo' AND o.b = true AND o.nil = null
```
returns true if the JSON properties match each expression 

**Advanced query**

```js
eyeColor = 'blue' AND age = 37 AND name.first = "Nancy" AND 
Contains(tags, Array("laboris", "ea")) AND 
EqualArray(range, Array(0,1,2,3,4,5,6,7,8,9))
```

**C# sample:**
```csharp
bool b = new JsonQueryRuntime("n = 1 AND s = 'string' AND b = true").Execute(jsonString));
```

**Boolean operator supported:** 

```js
=, <>, <, <=, >, >=, OR, AND, (, ) 
```
Boolean operators AND and OR are case sensitive.
Function names are not case sensitive.

**Arithmetic operator supported:** 

```js
+, -, *, /
```
- Addition (numbers, date/time + number, string concatenation)
- Subtraction (numbers, date/time - number)

**Date support:**

```
timestamp = #2018-12-20T14:16:00#
```
returns true is property timestamp is equal to the date+time value

```
timestamp >= #2018-12-20T14:16:00# and timestamp <= #2018-12-20T14:16:59#
```
returns true is property timestamp is in the date range (See Range() function aslo).

**Functions:**

Remember that JsonQueryEngine evaluate boolean expression. All function returns true or false (no truthy support).

*string*:

```js
Wildcard(name,"s?r*")
```
returns true if the wildcard match the content of the string property name

```js
Regex(name, "s.r.*")
```
returns true if the regular expression match the content of the string property name

```js
Contains(name, "substring")
```
returns true if the value of the property name contains the sub-string "substring"

*Range*:

```
Range(timestamp, #2018-12-20T14:16:00#, #2018-12-20T14:16:59#)
```
returns true if the date in property timestamp is between the 2 dates

```js
Range(val, 1, 10)
```
returns true if the numeric value in property val is between 1 and 10

```js
Range(s, "a", "d")
```
returns true if the string value in property s is between "a" and "d"

*In*:

```js
In(name, Array("A", "B", "C"))
```
returns true if the value of the property name is included in the array

```js
In(amount, Array(12, 24, 48))
```
returns true if the value of the property amount is included in the array

*Is-xxxxx*:

```js
IsObject(o), IsNumber(n), IsString(s), IsBoolean(b), IsDate(d), IsNull(nil), IsArray(a)
```
returns true is the property value type match the function. 

*Contains*:

```js
Contains(arrOfNumber, Array(12, 24, 48))
```
returns true if property arrOfNumber which is an array of number contains the values 12, 24, 48.

```js
Contains(arrOfString, Array("a", "b", "c"))
```
returns true if property arrOfString which is an array of string contains the values "a", "b", "c".

*ArrayEqual*:

```js
ArrayEqual(arrOfNumber, Array(12, 24, 48))
```
returns true if property arrOfNumber which is an array of number contains and only contains the values 12, 24, 48 in that exact order.

```js
ArrayEqual(arrOfString, Array("a", "b", "c"))
```
returns true if property arrOfString which is an array of string contains and only contains the values "a", "b", "c" in that exact order.

```js
ArrayEqual(arrOfBoolean, Array(true, false, true))
```
returns true if property arrOfBoolean which is an array of boolean contains the following values in the exact order true, false, true in that exact order.

*Miscellaneous*:

```js
Not(expression)
```
returns the inversed boolean value
```js
Len(name)
```
returns the length of the property name which is a string
```js
Sum(number-array)
```
returns the sum of the values in the array
```js
Avg(number-array)
```
returns the average values based on the values in the array

```js
Var(name:string, value:any) : true
```
Declare a variable, that can be used later in the where clause

```js
WriteLine(expression) : true
```
Display in the current output the expression

```js
Format(Number or Date expression, String Format) : string
```
Format a number or date based on .NET ToString() format.

### The Path() function

The `Path(string-path)` function evaluate the string-path according to
[JsonPath](https://goessner.net/articles/JsonPath/) 
and return the value. The symbol "`$.`" is automatically added at the beginning of the `string-path`.

The `Path()` function should be used with query that search for an object into an array.
For example

```js
Path("Manufacturers[?(@.Name == 'Acme Co')].Price") = 99.95
Path(".Products[?(@.Price == 4)].Name") = "Headlight Fluid"
```

### Queries on unknown property or pattern matching on property name

- Syntax: 
```
Path(path-with-unknown, expected-value)
```
- Sample: 
```
Path("main.?.z", 1)
```
- JSON:
```js
	{ 
		"main": { 
			"a":{ "z":1}, "b":{ "z":2}, "c":{ "z":3 } 
		} 
	}
```
The function Path() will run through all possible paths to evaluate `"main.?.z"` and stop 
at the first path that return the value 1 and return true

## .NET Framework

JSonQueryRuntime use dot net core 2.1.

## C# Syntax

```cs
public class JsonQueryRuntime {
	/// <summary>
	/// Apply the where clause to list of JSON object defined in the file
	/// </summary>
	/// <param name="fileName">The name of the JSON file</param>
	/// <param name="isJsonLine">If true the file contains JSON-LINES else the file must contain an array of JSON objects</param>
	/// <returns>The list of JSON string that match the where clause</returns>
	public IEnumerable<string> ExecuteFile(string fileName, bool isJsonLine);

	 /// <summary>
	/// Apply the where clause to list of JSON strings
	/// </summary>
	/// <param name="jsonStrings">A list of JSON string</param>
	/// <returns>The list of JSON string that match the where clause</returns>
	public IEnumerable<string> Execute(IEnumerable<string> jsonStrings);

	/// <summary>
	/// Apply the where clause to list of JSON objects
	/// </summary>
	/// <param name="jObjects"></param>
	/// <returns>The list of JSON string that match the where clause</returns>
	public IEnumerable<string> Execute(List<JObject> jObjects);

   	/// <summary>
	/// Apply the where clause to the JSON string
	/// </summary>
	/// <param name="jsonString"></param>
	/// <returns>true if the where clause apply to the JSON string</returns>
	public bool Execute(string jsonString);

	/// <summary>
	/// Apply the where clause to the JSON object
	/// </summary>
	/// <param name="o"></param>
	/// <returns> true if the where clause apply to the JSON object</returns>
	public bool Execute(JObject o);
}
```
```cs
public const string json0 = @"{ 
	""name"" : ""ok"",""b"":true, ""n"":123, 
	""wildText"" : ""ABCDE"",
	""obj0"": { ""name"" : ""okk"" } 
}";

[TestMethod]
public void Test() {

	Assert.IsTrue(new JsonQueryRuntime(@"name = 'ok' ").Execute(json0));
	Assert.IsTrue(new JsonQueryRuntime(@"name = 'ok' AND b = true ").Execute(json0));
	Assert.IsTrue(new JsonQueryRuntime(@"name = 'ok' AND b = true AND n = 123").Execute(json0));

	Assert.IsTrue(new JsonQueryRuntime(@"name = 'ok' AND Wildcard(wildText, 'ABCDE') ").Execute(json0));

	Assert.IsTrue(new JsonQueryRuntime(@"IsObject(obj0) AND obj0.name = 'okk' ").Execute(json0));
}
```

## Reference

* https://stackoverflow.com/questions/36340266/querying-json-from-c-sharp
* https://jack-vanlightly.com/blog/2016/2/11/implementing-a-dsl-parser


## Attribution

* JSON.net ~ Newtonsoft.json

MIT license and is free for commercial use.

* HiSystems.Interpreter 

```
(c) Hi-Integrity Systems 2012. All rights reserved.
www.hisystems.com.au - Toby Wicks
github.com/hisystems/Interpreter
Licensed under the Apache License, Version 2.0 (the "License");
```

## Tweet

Execute where clause like expression on JSON lines with #dotnet #csharp

age = 37 AND name.first = "Nancy" AND Contains(tags, Array("laboris", "ea"))

- Wildcard, regex
- Boolean and arithmetic expression
- Date support

JsonQueryRunTime github: https://bit.ly/2QsFnL8