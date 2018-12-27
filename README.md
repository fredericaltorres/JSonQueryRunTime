# JSonQueryRuntime

## Overview

JSonQueryRunTime is a .NET library to apply where clause like expression to 
- JSON string
- JSON string list 
- JSON-Lines file.

[JSON-Lines](http://jsonlines.org/) is a text format that contains line, each line is a valid json string. 

The JSON Query Runtime allows to filter JSON lines based on a where clause like syntax.

## JSON Query Syntax

- A JSON sample
```json
json { n:1, b:true, s:"string", "timestamp":"2018-12-20T14:16:32.4573737-05:00", o:{ name:"foo" }, a:[1,2,3] }
```

* Queries on the first level of property

    - n = 1 AND s = "string" AND b <> true
		returns true if the json properties match each expression 
    - Wildcard(name,"s?r*")
		returns true if the wildcard match the content of the string property name
    - Regex(name, "s.r.*")
		returns true if the regular expression match the content of the string property name
    - timestamp >= #2018-12-20T14:16:00# and timestamp <= #2018-12-20T14:16:59#
	- DateRange(timestamp, #2018-12-20T14:16:00#, #2018-12-20T14:16:59#)
		returns true if the date in property timestamp is between the 2 date
	- NumberRange(val, 1, 10)
		returns true if the numeric value in property val is between 1 and 10
	- InString(name, Array("A", "B", "C"))
		returns true if the value of the property name is included in the array
	- InNumber(amount, Array(12, 24, 48))
		returns true if the value of the property amount is included in the array
	- ContainString(name, "substring") # todo
		returns true if the value of the property name contains the sub-string "substring"
	- IsObject(o), IsNumber(n), IsString(s), IsBoolean(b), IsDate(d), IsNull(nil) # todo
		return true is the property type match the function

	- ContainArrayNumber(arrOfNumber, Array(12, 24, 48))
	- ContainArrayString(arrOfString, Array("a", "b", "c"))
	- ContainArrayBoolean(arrOfString, Array(true, false, true))

	- EqualArrayNumber(arrOfNumber, Array(12, 24, 48)) # todo
	- EqualArrayString(arrOfString, Array("a", "b", "c")) # todo
	- EqualArrayBoolean(arrOfString, Array(true, false, true)) # todo

	"ABCD" contains "*BC*"

* Queries on the second level of properties
    - where o.name == "foo"

* Queries on property
Json { main: { a: { z:1}, b: { z:2}, c: { z:3} } }

    - where main.?.z == 1 will match main.a.z:1


## Reference

* https://stackoverflow.com/questions/36340266/querying-json-from-c-sharp
* https://jack-vanlightly.com/blog/2016/2/11/implementing-a-dsl-parser