# JSonQueryRuntime

## Overview

JSonQueryRunTime is a .NET library to apply where clause like expression to 
- JSON string
- JSON string list 
- JSON-Lines file.

[JSON-Lines](http://jsonlines.org/) is a text format that contains line, each line is a valid json string. 

The JSON Query Runtime allows to filter JSON lines based on a where clause like syntax.

## JSON Query Syntax

- JSON sample
```json
{ 
	"n":1, "b":true, "s":"string", "timestamp":"2018-12-20T14:16:32.4573737-05:00", 
	"o":{ "name":"foo" }, "a":[1,2,3] 
}
```
### Query Samples

`n = 1 AND s = "string" AND b = true` returns true if the json properties match each expression 

**Operator supported:** 

```
=, <>, <, <=, >, >=, OR, AND, (, )
```

**Date support:**

- `timestamp = #2018-12-20T14:16:00#` returns true is property timestamp is equal to the date+time value

- `timestamp >= #2018-12-20T14:16:00# and timestamp <= #2018-12-20T14:16:59#` returns true is property timestamp is in the date range (See Range() function).

**Functions:**

- `Wildcard(name,"s?r*")` returns true if the wildcard match the content of the string property name

- `Regex(name, "s.r.*")` returns true if the regular expression match the content of the string property name

*Range*:

- `Range(timestamp, #2018-12-20T14:16:00#, #2018-12-20T14:16:59#)` returns true if the date in property timestamp is between the 2 dates

- `Range(val, 1, 10)` returns true if the numeric value in property val is between 1 and 10

- `Range(s, "a", "d")` returns true if the string value in property s is between "a" and "d"

*In*:

- `In(name, Array("A", "B", "C"))` returns true if the value of the property name is included in the array

- `In(amount, Array(12, 24, 48))` returns true if the value of the property amount is included in the array

*String*:

- `ContainString(name, "substring")` # todo returns true if the value of the property name contains the sub-string "substring"

*Is-xxxxx*:

- `IsObject(o), IsNumber(n), IsString(s), IsBoolean(b), IsDate(d), IsNull(nil), IsArray(a)` # todo return true is the property value type match the function

*Contain*:

- `ContainArrayNumber(arrOfNumber, Array(12, 24, 48))` returns true if property arrOfNumber which is an array of number contains the values 12, 24, 48

- `ContainArrayString(arrOfString, Array("a", "b", "c"))` returns true if property arrOfString which is an array of string contains the values "a", "b", "c"

- `ContainArrayBoolean(arrOfBoolean, Array(true, false))` returns true if property arrOfBoolean which is an array of boolean contains the values true and false

*EqualArray*:

- `EqualArrayNumber(arrOfNumber, Array(12, 24, 48))` # todo returns true if property arrOfNumber which is an array of number contains and only contains the values 12, 24, 48

- `EqualArrayString(arrOfString, Array("a", "b", "c"))` # todo returns true if property arrOfString which is an array of string contains and only contains the values "a", "b", "c"

- `EqualArrayBoolean(arrOfBoolean, Array(true, false, true))` # todo returns true if property arrOfBoolean which is an array of boolean contains the following values in the exact order true, false, true

### Queries on the second level of properties

The function `Path(string-path)` returns evaluate the string-path according 
[JsonPath](https://goessner.net/articles/JsonPath/) 
and return the value. The symbol "`$.`" is automatically added at the beginning of the `string-path`.


- Sample:
```js
Path("main.a") = 1
Path("Manufacturers[?(@.Name == 'Acme Co')].Price") = 99.95
Path(".Products[?(@.Price == 4)].Name") = "Headlight Fluid"
```
- JSON:
```js
{ 
	"main": { 
		"a":1 
	} 
}
```	

### Queries on unknown property or pattern matching on property name

Proposal:

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
The function Path() will run throuh all possible paths to evaluate `"main.?.z"` and stop 
at the first path that return the value 1

## .NET Framework

JSonQueryRuntime use dot net core 2.1.

## C# Syntax

```cs
public class JsonQueryRuntime {
	public JsonQueryRuntime(string whereClause);
	/// <summary>
	/// Evaluate a list of JSON string
	/// </summary>
	/// <param name="jsonStrings"></param>
	/// <returns></returns>
	public IEnumerable<string> Eval( IEnumerable<string> jsonStrings);
	/// <summary>
	/// Evaluate one JSON string
	/// </summary>
	/// <param name="jsonString"></param>
	/// <returns></returns>	
	public bool Eval(string jsonString);
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

	Assert.IsTrue(new JsonQueryRuntime(@"name = ""ok"" ").Eval(json0));
	Assert.IsTrue(new JsonQueryRuntime(@"name = ""ok"" AND b = true ").Eval(json0));
	Assert.IsTrue(new JsonQueryRuntime(@"name = ""ok"" AND b = true AND n = 123").Eval(json0));

	Assert.IsTrue(new JsonQueryRuntime(@"name = ""ok"" AND Wildcard(wildText, ""ABCDE"") ").Eval(json0));

	Assert.IsTrue(new JsonQueryRuntime(@"IsObject(obj0) AND Path(""obj0.name"")  = ""okk"" ").Eval(json0));
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
