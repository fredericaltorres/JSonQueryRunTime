using HiSystems.Interpreter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JsonQueryRunTimeNS;

namespace JSonQueryRunTime_UnitTests
{
    [TestClass]
    public class ExecuteOneJsonString_UnitTests
    {
        public const string json0 = @"{ ""name"" : ""ok"", ""b"":true, ""n"":123, ""wildText"" : ""ABCDE"", ""now"":""2018-12-25T20:23:49.0-05:00"", ""utcnow"" : ""2018-12-26T01:24:46.0"", ""nil"": null, arrNumber:[1,2,3], arrString:[""a"", ""b"", ""c""], arrBoolean:[true, false, true],
                                            ""obj0"": { ""name"" : ""okk"", ""b"":true, ""n"":124, ""wildText"" : ""ABCDE"", ""now"":""2018-12-25T20:23:49.0-05:00"", ""utcnow"" : ""2018-12-26T01:24:46.0"", ""nil"": null ,

                                                ""obj00"": { ""name"" : ""okkk"", ""b"":false, ""n"":1234, ""wildText"" : ""ABCDE"", ""now"":""2018-12-25T20:23:49.0-05:00"", ""utcnow"" : ""2018-12-26T01:24:46.0"", ""nil"": null }

                                            } 
                                      }";
        public const string json1 = @"{ ""name"" : ""ko"", ""b"":false,""n"":124, ""wildText"" : ""XYZ"", ""now"":""2018-12-31T20:23:49.0-05:00"", ""utcnow"" : ""2018-12-31T01:24:46.0"", ""nil"": null, arrNumber:[4,5,6], arrString:[""x"", ""y"", ""z""], arrBoolean:[false, true, false],
                                        ""obj0"": { ""name"" : ""koo"", ""b"":false,""n"":124, ""wildText"" : ""XYZ"", ""now"":""2018-12-31T20:23:49.0-05:00"", ""utcnow"" : ""2018-12-31T01:24:46.0"", ""nil"": null } 
                                    }";

        public const string json_store = @"{
  'Stores': [
    'Lambton Quay', 'Willis Street'
  ],
  'Manufacturers': [
    {
      'Name': 'Acme Co',
      'Products': [
        {
          'Name': 'Anvil', 'Price': 50
        }
      ]
    },
    {
      'Name': 'Contoso',
      'Products': [
        {
          'Name': 'Elbow Grease', 'Price': 99.95
        },
        {
          'Name': 'Headlight Fluid', 'Price': 4
        }
      ]
    }
  ]
}";

        [TestMethod]
        public void Arithmetic_Operation_String()
        {
            Assert.IsTrue(new JsonQueryRuntime(@" name + '--' = 'ok--' ").Execute(json0));
        }

        [TestMethod]
        public void Arithmetic_Operation_Date()
        {
            Assert.IsTrue(new JsonQueryRuntime(@" WriteLine(now - 1) ").Execute(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" WriteLine(now - n) ").Execute(json0));
            
            // JSON date format
            Assert.IsTrue(new JsonQueryRuntime(@" now - 1 = #2018-12-24 20:23:49# ").Execute(json0));
            // .NET date format
            Assert.IsTrue(new JsonQueryRuntime(@" now - 1 = #12/24/2018 8:23:49 PM# ").Execute(json0));
            // Substract
            Assert.IsTrue(new JsonQueryRuntime(@" now - n = #8/24/2018 8:23:49 PM# ").Execute(json0));
        }

        [TestMethod]
        public void Arithmetic_Operation_Number()
        {
            Assert.IsTrue(new JsonQueryRuntime(@" 1 + 1 = 2 ").Execute(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" n + 1 = 124 ").Execute(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" n - 1 = 122 ").Execute(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" n * 2 = (123*2) ").Execute(json0));
            // Display the result
            Assert.IsTrue(new JsonQueryRuntime(@" WriteLine(n/2) AND n/2 = 61.5 ").Execute(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" n + obj0.n = (123+124) ").Execute(json0));
        }

        [TestMethod]
        public void Sum()
        {
            Assert.IsTrue(new JsonQueryRuntime(@" Sum(Array(1, 2)) = 3 ").Execute(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" Sum(arrNumber) = 6 ").Execute(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" Avg(arrNumber) = 2 ").Execute(json0));
        }

        [TestMethod]
        public void Date_Utc()
        {
            Assert.IsTrue(new JsonQueryRuntime(@" utcnow = #2018-12-26 01:24:46# ").Execute(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" utcnow <> #2018-12-26 01:24:47# ").Execute(json0));

            Assert.IsFalse(new JsonQueryRuntime(@" utcnow = #2018-12-26 01:24:47# ").Execute(json0));
        }

        [TestMethod]
        public void Date_GreaterThan_OrEqual_Utc()
        {
            Assert.IsTrue (new JsonQueryRuntime(@" utcnow >= #2018-12-26 01:24:46# ").Execute(json0));
            Assert.IsFalse(new JsonQueryRuntime(@" utcnow > #2018-12-26 01:24:46# ").Execute(json0));

            Assert.IsTrue (new JsonQueryRuntime(@" utcnow <= #2018-12-26 01:24:46# ").Execute(json0));
            Assert.IsFalse(new JsonQueryRuntime(@" utcnow < #2018-12-26 01:24:46# ").Execute(json0));
        }

        [TestMethod]
        public void TimeStamp_Range_Utc()
        {
            
            Assert.IsTrue(new JsonQueryRuntime(@" 
                utcnow >= #2018-12-01 01:24:46# AND
                utcnow <= #2018-12-31 01:24:46# ").Execute(json0));

            Assert.IsTrue(new JsonQueryRuntime(@" 
                utcnow >= #2018-12-01# AND
                utcnow <= #2018-12-31# ").Execute(json0));

            Assert.IsFalse(new JsonQueryRuntime(@" 
                utcnow >= #2018-12-27 01:24:46# AND
                utcnow <= #2018-12-31 01:24:46# ").Execute(json0));
        }

        [TestMethod]
        public void TimeStamp_Range_With_Arithmetic_Operation_Utc()
        {
            // utcnow : 2018-12-26T01:24:46.0
            // utcnow - 20 : 2018-12-06T01:24:46.0
            Assert.IsTrue(new JsonQueryRuntime(@" 
                utcnow - 20 >= #2018-12-05 01:24:46# AND
                utcnow - 20 <= #2018-12-07 06:24:46# ").Execute(json0));

            // Just difference in time/minute
            Assert.IsTrue(new JsonQueryRuntime(@" 
                utcnow - 20 >= #2018-12-06 01:23:46# AND
                utcnow - 20 <= #2018-12-06 06:25:46# ").Execute(json0));
        }

        [TestMethod]
        public void DateNoTime_Range_Utc()
        {
            Assert.IsTrue(new JsonQueryRuntime(@" 
                utcnow >= #2018-12-01# AND
                utcnow <= #2018-12-31# ").Execute(json0));

            Assert.IsFalse(new JsonQueryRuntime(@" 
                utcnow >= #2018-12-27# AND
                utcnow <= #2018-12-31# ").Execute(json0));
        }

        [TestMethod]
        public void DateRange_Utc()
        {
            Assert.IsTrue (new JsonQueryRuntime(@" Range(utcnow, #2018-12-01 01:24:46#, #2018-12-31 01:24:46#) ").Execute(json0));
            Assert.IsFalse(new JsonQueryRuntime(@" Range(utcnow, #2018-12-27 01:24:46#, #2018-12-31 01:24:46#) ").Execute(json0));
        }

        [TestMethod]
        public void DateRange_ArithmeticOperation_Utc()
        {
            Assert.IsTrue (new JsonQueryRuntime(@"Range(utcnow - 20, #2018-12-05 01:24:46#, #2018-12-07 06:24:46# ) ").Execute(json0));
            Assert.IsTrue (new JsonQueryRuntime(@"Range(utcnow - 20, #2018-12-06 01:23:46# , #2018-12-06 06:25:46#  ) ").Execute(json0));
            Assert.IsFalse (new JsonQueryRuntime(@"Range(utcnow - 20, #2018-12-07 01:23:46# , #2018-12-06 06:25:46#  ) ").Execute(json0));
        }

        [TestMethod]
        public void Range_String()
        {
            Assert.IsTrue(new JsonQueryRuntime(@" Range('b', ""a"", ""d"") ").Execute(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" Range(""aa"", ""a"", ""d"") ").Execute(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" Range(name, ""o"", ""p"") ").Execute(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" Range(name, ""oa"", ""p"") ").Execute(json0));

            Assert.IsFalse(new JsonQueryRuntime(@" Range(""daa"", ""a"", ""d"") ").Execute(json0));
            Assert.IsFalse(new JsonQueryRuntime(@" Range(name, ""a"", ""d"") ").Execute(json0));
            Assert.IsFalse(new JsonQueryRuntime(@" Range(name, ""ol"", ""p"") ").Execute(json0));
        }

        [TestMethod]
        public void Date_Local()
        {
            Assert.IsTrue(new JsonQueryRuntime(@" now = #2018-12-25 20:23:49# ").Execute(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" now <> #2018-12-25 20:23:50# ").Execute(json0));

            Assert.IsFalse(new JsonQueryRuntime(@" now = #2018-12-25 20:23:50# ").Execute(json0));
        }

        [TestMethod]
        public void WildCardFunction()
        {
            Assert.IsTrue(new JsonQueryRuntime(@" Wildcard(wildText, 'ABCDE') ").Execute(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" Wildcard(wildText, '?BCD?') ").Execute(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" Wildcard(wildText, '?B?D?') ").Execute(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" Wildcard(wildText, 'A*E') ").Execute(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" Wildcard(wildText, 'A*C*E') ").Execute(json0));

            Assert.IsFalse(new JsonQueryRuntime(@" Wildcard(wildText, ""ABCDZ"") ").Execute(json0));
            Assert.IsFalse(new JsonQueryRuntime(@" Wildcard(wildText, ""?B-D?"") ").Execute(json0));
        }

        [TestMethod]
        public void RegexFunction()
        {
            Assert.IsTrue(new JsonQueryRuntime(@" Regex(wildText, 'ABCDE') ").Execute(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" Regex(wildText, 'ABCDE') ").Execute(json0));

            Assert.IsTrue(new JsonQueryRuntime(@" Regex(wildText, '^ABCDE') ").Execute(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" Regex(wildText, '^ABC') ").Execute(json0));

            Assert.IsTrue(new JsonQueryRuntime(@" Regex(wildText, 'ABCDE$') ").Execute(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" Regex(wildText, 'CDE$') ").Execute(json0));

            Assert.IsTrue(new JsonQueryRuntime(@" Regex(wildText, '^ABCDE$') ").Execute(json0));

            Assert.IsTrue(new JsonQueryRuntime(@" Regex(wildText, '.BCD.') ").Execute(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" Regex(wildText, '.B.D.') ").Execute(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" Regex(wildText, 'A.*E') ").Execute(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" Regex(wildText, 'A.*C.*E') ").Execute(json0));

            Assert.IsFalse(new JsonQueryRuntime(@" Regex(wildText, '.B-D.') ").Execute(json0));
            Assert.IsFalse(new JsonQueryRuntime(@" Regex(wildText, 'A.*-') ").Execute(json0));

            Assert.IsFalse(new JsonQueryRuntime(@" Regex(wildText, 'zBCDE$') ").Execute(json0));
            Assert.IsFalse(new JsonQueryRuntime(@" Regex(wildText, '^zBCDE$') ").Execute(json0));
            Assert.IsFalse(new JsonQueryRuntime(@" Regex(wildText, '.B-D.') ").Execute(json0));

            Assert.IsTrue(JsonQueryRunTime.fxRegex.RegexCache.Count == 14);
            JsonQueryRunTime.fxRegex.RegexCache.Clear();
            Assert.IsTrue(JsonQueryRunTime.fxRegex.RegexCache.Count == 0);
        }

        [TestMethod]
        public void Var_DeclareVariable()
        {
            Assert.IsTrue(new JsonQueryRuntime(@"Var('newName','ok') AND name = newName ").Execute(json0));
            Assert.IsTrue(new JsonQueryRuntime(@"Var('newN',123) AND n = newN ").Execute(json0));
            Assert.IsTrue(new JsonQueryRuntime(@"Var('newNow',#2018-12-25T20:23:49.0-05:00#) AND now = newNow ").Execute(json0));

            Assert.IsTrue(new JsonQueryRuntime(@"Var('newName','ok') AND WriteLine(newName) AND name = newName ").Execute(json0));
        }

        [TestMethod]
        public void StringBooleanNumeric_Equal_AND()
        {
            Assert.IsTrue(new JsonQueryRuntime(@"name = ""ok"" ").Execute(json0));
            Assert.IsTrue(new JsonQueryRuntime(@"name = ""ok"" AND b = true ").Execute(json0));
            Assert.IsTrue(new JsonQueryRuntime(@"name = ""ok"" AND b = true AND n = 123").Execute(json0));

            Assert.IsFalse(new JsonQueryRuntime(@"name = ""ok"" AND b = false AND n = 123").Execute(json0));
            Assert.IsFalse(new JsonQueryRuntime(@"name = ""ok"" AND b = true AND n = 123 AND 1 = 2").Execute(json0));
        }

        [TestMethod]
        public void StringBooleanNumeric_NotEqual_AND()
        {
            Assert.IsTrue(new JsonQueryRuntime(@"name <> ""okk"" ").Execute(json0));
            Assert.IsTrue(new JsonQueryRuntime(@"name <> ""okk"" AND b <> false ").Execute(json0));
            Assert.IsTrue(new JsonQueryRuntime(@"name <> ""okk"" AND b <> false AND n <> 12.3").Execute(json0));

            Assert.IsFalse(new JsonQueryRuntime(@"name <> ""okk"" AND b <> true AND n <> 12.3").Execute(json0));
        }
        [TestMethod]
        public void StringBooleanNumeric_Equal_OR()
        {
            Assert.IsTrue(new JsonQueryRuntime(@"name = ""okkkk"" OR b = true").Execute(json0));
            Assert.IsTrue(new JsonQueryRuntime(@"name = ""ok"" AND (b = true OR b = false)").Execute(json0));
            Assert.IsTrue(new JsonQueryRuntime(@"name = ""ok"" OR b = true OR n = 123").Execute(json0));

            Assert.IsFalse(new JsonQueryRuntime(@"name = ""okkkk"" OR b = false OR n = 553").Execute(json0));
        }

        [TestMethod]
        public void String_LenFunction()
        {
            Assert.IsTrue(new JsonQueryRuntime(@"Len(name) = 2").Execute(json0));
        }

        [TestMethod]
        public void Numeric_LessThan_GreaterThan_OrEqual()
        {
            Assert.IsTrue(new JsonQueryRuntime(@"n < 124").Execute(json0));
            Assert.IsFalse(new JsonQueryRuntime(@"n > 124").Execute(json0));

            Assert.IsTrue(new JsonQueryRuntime(@"n <= 123").Execute(json0));
            Assert.IsTrue(new JsonQueryRuntime(@"n >= 123").Execute(json0));
            Assert.IsFalse(new JsonQueryRuntime(@"n >= 124").Execute(json0));
        }

        [TestMethod]
        public void Test_HiSystems_Interpreter()
        {
            var expression = new Engine().Parse(@"a = 10 AND (b = 1 OR b = 2 OR b = 3) AND s_o = ""ok"" ");
            expression.Variables["a"].Value = new HiSystems.Interpreter.Number(10);
            expression.Variables["b"].Value = new HiSystems.Interpreter.Number(3);
            expression.Variables["s_o"].Value = new HiSystems.Interpreter.Text("ok");
            bool result = expression.Execute<HiSystems.Interpreter.Boolean>();
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void InString_Function()
        {
            Assert.IsTrue(new JsonQueryRuntime(@"In(name, Array(""ok"", ""ko"")) ").Execute(json0));
            Assert.IsFalse(new JsonQueryRuntime(@"In(name, Array(""a"", ""b"")) ").Execute(json0));
        }

        [TestMethod]
        public void InNumber_Function()
        {
            Assert.IsTrue(new JsonQueryRuntime(@"In(n, Array(122, 123, 124)) ").Execute(json0));
            Assert.IsFalse(new JsonQueryRuntime(@"In(n, Array(122, 124)) ").Execute(json0));
        }

        [TestMethod]
        public void NumberRangeFunction()
        {
            Assert.IsTrue(new JsonQueryRuntime(@" Range(n, 122, 124) ").Execute(json0));
            Assert.IsFalse(new JsonQueryRuntime(@" Range(n, 100, 110) ").Execute(json0));
        }

        [TestMethod]
        public void IsNumber()
        {
            Assert.IsTrue(new JsonQueryRuntime(@"IsNumber(n)").Execute(json0));
            Assert.IsTrue(new JsonQueryRuntime(@"IsNumber(1)").Execute(json0));
            Assert.IsTrue(new JsonQueryRuntime(@"Not( IsNumber(nil) )").Execute(json0));

            Assert.IsFalse(new JsonQueryRuntime(@"IsNumber(name)").Execute(json0));
            Assert.IsFalse(new JsonQueryRuntime(@"IsNumber(b)").Execute(json0));
            Assert.IsFalse(new JsonQueryRuntime(@"IsNumber(now)").Execute(json0));
            Assert.IsFalse(new JsonQueryRuntime(@"IsNumber(nil)").Execute(json0));
        }

        [TestMethod]
        public void IsBoolean()
        {
            Assert.IsTrue(new JsonQueryRuntime(@"IsBoolean(b)").Execute(json0));
            Assert.IsTrue(new JsonQueryRuntime(@"IsBoolean(true)").Execute(json0));
            Assert.IsTrue(new JsonQueryRuntime(@"IsBoolean(false)").Execute(json0));
            Assert.IsTrue(new JsonQueryRuntime(@"IsBoolean( Not( b ) )").Execute(json0));
            Assert.IsTrue(new JsonQueryRuntime(@"IsBoolean( Not( false ) )").Execute(json0));
            Assert.IsTrue(new JsonQueryRuntime(@"IsBoolean( Not( true ) )").Execute(json0));

            Assert.IsFalse(new JsonQueryRuntime(@"IsBoolean(name)").Execute(json0));
            Assert.IsFalse(new JsonQueryRuntime(@"IsBoolean(now)").Execute(json0));
            Assert.IsFalse(new JsonQueryRuntime(@"IsBoolean(nil)").Execute(json0));

            Assert.IsTrue(new JsonQueryRuntime(@"Not( IsBoolean(nil) )").Execute(json0));
        }

        [TestMethod]
        public void IsDate()
        {
            Assert.IsTrue(new JsonQueryRuntime(@"IsDate(#2018-12-26T01:24:46.0#)").Execute(json0));
            Assert.IsTrue(new JsonQueryRuntime(@"IsDate(now)").Execute(json0));
            Assert.IsTrue(new JsonQueryRuntime(@"IsDate(utcnow)").Execute(json0));
            Assert.IsTrue(new JsonQueryRuntime(@"IsDate(Today())").Execute(json0));
            Assert.IsTrue(new JsonQueryRuntime(@"IsDate(#2018-12-25T20:23:49.0-05:00#)").Execute(json0));

            Assert.IsFalse(new JsonQueryRuntime(@"IsDate(name)").Execute(json0));
            Assert.IsFalse(new JsonQueryRuntime(@"IsDate(n)").Execute(json0));
            Assert.IsFalse(new JsonQueryRuntime(@"IsDate(b)").Execute(json0));
            Assert.IsFalse(new JsonQueryRuntime(@"IsDate(nil)").Execute(json0));

            Assert.IsTrue(new JsonQueryRuntime(@"Not( IsDate(nil) )").Execute(json0));
        }

        [TestMethod]
        public void IsArray()
        {
            Assert.IsTrue(new JsonQueryRuntime(@"IsArray(Array(1,2))").Execute(json0));

            Assert.IsTrue(new JsonQueryRuntime(@"IsArray(arrString)").Execute(json0));
            Assert.IsTrue(new JsonQueryRuntime(@"IsArray(arrNumber)").Execute(json0));
            Assert.IsTrue(new JsonQueryRuntime(@"IsArray(arrBoolean)").Execute(json0));

            Assert.IsFalse(new JsonQueryRuntime(@"IsArray(now)").Execute(json0));
            Assert.IsFalse(new JsonQueryRuntime(@"IsArray(utcnow)").Execute(json0));
            Assert.IsFalse(new JsonQueryRuntime(@"IsArray(Today())").Execute(json0));
            Assert.IsFalse(new JsonQueryRuntime(@"IsArray(#2018-12-25T20:23:49.0-05:00#)").Execute(json0));
            Assert.IsFalse(new JsonQueryRuntime(@"IsArray(name)").Execute(json0));
            Assert.IsFalse(new JsonQueryRuntime(@"IsArray(n)").Execute(json0));
            Assert.IsFalse(new JsonQueryRuntime(@"IsArray(b)").Execute(json0));
            Assert.IsFalse(new JsonQueryRuntime(@"IsArray(nil)").Execute(json0));

            Assert.IsTrue(new JsonQueryRuntime(@"Not( IsArray(nil) )").Execute(json0));
        }

        [TestMethod]
        public void IsString()
        {
            Assert.IsTrue(new JsonQueryRuntime(@"IsString(name)").Execute(json0));

            Assert.IsFalse(new JsonQueryRuntime(@"IsString(n)").Execute(json0));
            Assert.IsFalse(new JsonQueryRuntime(@"IsString(b)").Execute(json0));
            Assert.IsFalse(new JsonQueryRuntime(@"IsString(now)").Execute(json0));
            Assert.IsFalse(new JsonQueryRuntime(@"IsString(nil)").Execute(json0));
        }

        [TestMethod]
        public void IsString_WithPathFunctionAsParameter()
        {
            Assert.IsTrue(new JsonQueryRuntime(@"IsString( Path( ""name"" ) )").Execute(json0));
            Assert.IsTrue(new JsonQueryRuntime(@"IsString( Path ( ""obj0.name"" ) )").Execute(json0));
        }

        [TestMethod, ExpectedException(typeof(System.InvalidOperationException))]
        public void ContainsWithEmptyArrayAsParameter_ThrowArgumentException()
        {
            Assert.IsTrue(new JsonQueryRuntime(@"Contains(arrNumber, Array())").Execute(json0));
        }

        [TestMethod]
        public void ContainsString()
        {
            Assert.IsTrue(new JsonQueryRuntime(@"Contains(name, ""ok"")").Execute(json0));
            Assert.IsTrue(new JsonQueryRuntime(@"Contains(name, ""ok"")").Execute(json0));

            Assert.IsFalse(new JsonQueryRuntime(@"Contains(wildText, ""ok"")").Execute(json0));
        }

        [TestMethod, ExpectedException(typeof(System.ArgumentException))]
        public void ContainsString_FirstArgumentNotAStringThrowException()
        {
            Assert.IsFalse(new JsonQueryRuntime(@"Contains(n, ""ok"")").Execute(json0));
        }

        [TestMethod]
        public void ContainsArrayNumber()
        {
            Assert.IsTrue(new JsonQueryRuntime(@"Contains(arrNumber, Array(1, 3))").Execute(json0));
            Assert.IsTrue(new JsonQueryRuntime(@"Contains(arrNumber, Array(1, 2, 3))").Execute(json0));

            Assert.IsFalse(new JsonQueryRuntime(@"Contains(arrNumber, Array(1, 3, 4))").Execute(json0));
        }

        [TestMethod]
        public void EqualArrayNumber()
        {
            Assert.IsTrue(new JsonQueryRuntime(@"EqualArray(arrNumber, Array(1, 2, 3))").Execute(json0));

            Assert.IsFalse(new JsonQueryRuntime(@"EqualArray(arrNumber, Array(1, 2, 3, 4))").Execute(json0));
            Assert.IsFalse(new JsonQueryRuntime(@"EqualArray(arrNumber, Array(1, 2, 4))").Execute(json0));
            Assert.IsFalse(new JsonQueryRuntime(@"EqualArray(arrNumber, Array(1))").Execute(json0));
        }

        [TestMethod]
        public void ContainsArrayString()
        {
            Assert.IsTrue(new JsonQueryRuntime(@"Contains(arrString, Array(""a"", ""c""))").Execute(json0));
            Assert.IsTrue(new JsonQueryRuntime(@"Contains(arrString, Array(""a"", ""c"", ""b""))").Execute(json0));
            Assert.IsTrue(new JsonQueryRuntime(@"Contains(arrString, Array(""a""))").Execute(json0));

            Assert.IsFalse(new JsonQueryRuntime(@"Contains(arrString, Array(""a"", ""d""))").Execute(json0));
        }

        [TestMethod]
        public void EqualArrayString()
        {
            Assert.IsTrue(new JsonQueryRuntime(@"EqualArray(arrString, Array(""a"", ""b"", ""c""))").Execute(json0));

            Assert.IsFalse(new JsonQueryRuntime(@"EqualArray(arrString, Array(""a"", ""b""))").Execute(json0));
            Assert.IsFalse(new JsonQueryRuntime(@"EqualArray(arrString, Array(""a"", ""c"", ""b""))").Execute(json0));
        }

        [TestMethod]
        public void EqualArrayBoolean()
        {
            Assert.IsTrue(new JsonQueryRuntime(@"equalArray(arrBoolean, array(true, false, true))").Execute(json0));
            Assert.IsFalse(new JsonQueryRuntime(@"equalArray(arrBoolean, Array(true))").Execute(json0));
            Assert.IsFalse(new JsonQueryRuntime(@"equalArray(arrBoolean, Array(false))").Execute(json0));
        }

        [TestMethod]
        public void ContainsArrayBoolean()
        {
            Assert.IsTrue(new JsonQueryRuntime(@"Contains(arrBoolean, Array(true, false))").Execute(json0));
            Assert.IsTrue(new JsonQueryRuntime(@"contains(arrBoolean, Array(true))").Execute(json0));
            Assert.IsTrue(new JsonQueryRuntime(@"Contains(arrBoolean, Array(false))").Execute(json0));
        }

        [TestMethod]
        public void IsObject()
        {
            Assert.IsTrue(new JsonQueryRuntime(@"IsObject(obj0)").Execute(json0));

            Assert.IsFalse(new JsonQueryRuntime(@"IsObject(name)").Execute(json0));
            Assert.IsFalse(new JsonQueryRuntime(@"IsObject(n)").Execute(json0));
            Assert.IsFalse(new JsonQueryRuntime(@"IsObject(nil)").Execute(json0));
            Assert.IsFalse(new JsonQueryRuntime(@" IsObject(""aa"") ").Execute(json0));
            Assert.IsFalse(new JsonQueryRuntime(@"IsObject(1)").Execute(json0));
            Assert.IsFalse(new JsonQueryRuntime(@"IsObject(null)").Execute(json0));
            Assert.IsFalse(new JsonQueryRuntime(@"IsObject(#1964-12-11#)").Execute(json0));
        }

        
        [TestMethod]
        public void WriteLine_String()
        {
            Assert.IsTrue(new JsonQueryRuntime(@" WriteLine(""Hello World"") ").Execute(json0));
        }

        [TestMethod]
        public void Path_String()
        {
            Assert.IsTrue(new JsonQueryRuntime(@" obj0.name = ""okk"" ").Execute(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" isObject(obj0) AND obj0.name = ""okk"" ").Execute(json0));

            Assert.IsTrue(new JsonQueryRuntime(@" path(""obj0.name"")  = ""okk"" ").Execute(json0));
            Assert.IsTrue(new JsonQueryRuntime(@"isObject(obj0) AND Path(""obj0.name"")  = ""okk"" ").Execute(json0));
        }

        [TestMethod]
        public void Path_DoubleNested_String()
        {
            Assert.IsTrue(new JsonQueryRuntime(@" obj0.obj00.name = ""okkk"" ").Execute(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" obj0.obj00.n = 1234 ").Execute(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" obj0.obj00.name = ""okkk"" AND obj0.obj00.n = 1234 ").Execute(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" ( obj0.obj00.name = ""okkk"" ) AND ( obj0.obj00.n = 1234 ) ").Execute(json0));

            Assert.IsTrue(new JsonQueryRuntime(@" Path(""obj0.obj00.name"") = ""okkk"" ").Execute(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" Path(""obj0.obj00.n"") = 1234 ").Execute(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" Path(""obj0.obj00.name"") = ""okkk"" AND Path(""obj0.obj00.n"") = 1234 ").Execute(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" ( Path(""obj0.obj00.name"" ) = ""okkk"" ) AND ( Path(""obj0.obj00.n"") = 1234 ) ").Execute(json0));
        }

        [TestMethod]
        public void Path_Number()
        {
            Assert.IsTrue(new JsonQueryRuntime(@" obj0.n = 124 ").Execute(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" obj0.obj00.n = 1234 ").Execute(json0));
            Assert.IsFalse(new JsonQueryRuntime(@" obj0.n = 1246666 ").Execute(json0));

            Assert.IsTrue(new JsonQueryRuntime(@" Path(""obj0.n"") = 124 ").Execute(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" Path(""obj0.obj00.n"") = 1234 ").Execute(json0));
            Assert.IsFalse(new JsonQueryRuntime(@" Path(""obj0.n"") = 1246666 ").Execute(json0));
        }

        [TestMethod, ExpectedException(typeof(System.InvalidOperationException))]
        public void Path_ReturnsStringExpectNumber_Throw()
        {
            // Wrong type
            Assert.IsFalse(new JsonQueryRuntime(@" Path(""obj0.name"") = 123 ").Execute(json0));
        }

        
        [TestMethod, ExpectedException(typeof(System.InvalidOperationException))]
        public void Path_ReturnsStringExpectNumber_Throw_2()
        {
            // Wrong type
            Assert.IsFalse(new JsonQueryRuntime(@" obj0.name = 123 ").Execute(json0));
        }

        [TestMethod]
        public void Path_Boolean()
        {
            Assert.IsTrue(new JsonQueryRuntime(@" obj0.b = true ").Execute(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" obj0.b ").Execute(json0));

            Assert.IsTrue(new JsonQueryRuntime(@" Path(""obj0.b"") = true ").Execute(json0));
            // obj0.b return true, so Path() can be evaluate like that, only for boolean
            Assert.IsTrue(new JsonQueryRuntime(@" Path(""obj0.b"")  ").Execute(json0));
        }

        [TestMethod]
        public void Path_Empty()
        {
            Assert.IsTrue(new JsonQueryRuntime(@" Path("""") = null ").Execute(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" IsNull( Path("""") ) ").Execute(json0));
        }

        [TestMethod]
        public void Path_PatternMatching()
        {
            // property n
            Assert.IsTrue(new JsonQueryRuntime(@" Path(""?"", 123)   ").Execute(json0));
            // property name
            Assert.IsTrue(new JsonQueryRuntime(@" Path(""?"", ""ok"")   ").Execute(json0));
            // property b
            Assert.IsTrue(new JsonQueryRuntime(@" Path(""?"", true)   ").Execute(json0));

            Assert.IsTrue(new JsonQueryRuntime(@" Path(""obj0.obj00.?"", 1234)   ").Execute(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" Path(""obj0.?"", 124)  ").Execute(json0));
        }


        [TestMethod]
        public void IsObject_Path()
        {
            var queryManufacturer = "Manufacturers[?(@.Name == 'Acme Co')]";
            Assert.IsTrue(new JsonQueryRuntime($@" IsObject( Path(""{queryManufacturer}"") )  ").Execute(json_store));
        }


        [TestMethod]
        public void Path_QueryObjectInArray()
        {
            var queryManufacturer = "Manufacturers[?(@.Name == 'Acme Co')]";
            Assert.IsTrue(new JsonQueryRuntime($@" IsObject( Path(""{queryManufacturer}"") )  ").Execute(json_store));
            Assert.IsTrue(new JsonQueryRuntime($@" Path(""{queryManufacturer}.Name"") = ""Acme Co"" ").Execute(json_store));
            Assert.IsTrue(new JsonQueryRuntime($@" Path(""{queryManufacturer}.Name"") = ""Acme Co"" ").Execute(json_store));
            Assert.IsTrue(new JsonQueryRuntime($@" Path(""{queryManufacturer}.Products[0].Price"") = 50 ").Execute(json_store));

            var queryProductPerPrice = ".Products[?(@.Price == 4)]";
            Assert.IsTrue(new JsonQueryRuntime($@" Path(""{queryProductPerPrice}.Name"") = ""Headlight Fluid"" ").Execute(json_store));
            Assert.IsTrue(new JsonQueryRuntime($@" IsObject( Path(""{queryProductPerPrice}"") ) ").Execute(json_store));
            Assert.IsTrue(new JsonQueryRuntime($@" Not( Not( IsObject( Path(""{queryProductPerPrice}"") ) ) )").Execute(json_store));
        }

        [TestMethod]
        public void IsNull()
        {
            Assert.IsFalse(new JsonQueryRuntime(@" name = null ").Execute(json0));

            Assert.IsTrue(new JsonQueryRuntime(@" IsNull(nil) ").Execute(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" nil = null ").Execute(json0));
            

            Assert.IsTrue(new JsonQueryRuntime(@" IsNull(obj0.nil) ").Execute(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" obj0.nil = null ").Execute(json0));

            Assert.IsTrue(new JsonQueryRuntime(@" IsNull(null) ").Execute(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" null = null ").Execute(json0));
            

            Assert.IsFalse(new JsonQueryRuntime(@" nil = 1 ").Execute(json0));

            Assert.IsFalse(new JsonQueryRuntime(@" name = null ").Execute(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" name <> null ").Execute(json0));

            Assert.IsFalse(new JsonQueryRuntime(@" IsNull(name) ").Execute(json0));
            Assert.IsFalse(new JsonQueryRuntime(@" IsNull(n) ").Execute(json0));
            Assert.IsFalse(new JsonQueryRuntime(@" IsNull(1) ").Execute(json0));
            Assert.IsFalse(new JsonQueryRuntime(@" IsNull(true) ").Execute(json0));
            Assert.IsFalse(new JsonQueryRuntime(@" IsNull(false) ").Execute(json0));
            Assert.IsFalse(new JsonQueryRuntime(@" IsNull(#2018-12-26 01:24:46#) ").Execute(json0));
            Assert.IsFalse(new JsonQueryRuntime(@" IsNull(""aaa"") ").Execute(json0));
        }


        [TestMethod]
        public void Not()
        {
            Assert.IsTrue(new JsonQueryRuntime(@" Not(true) = false ").Execute(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" Not(false) = true ").Execute(json0));

            Assert.IsTrue(new JsonQueryRuntime(@" Not( 1 = 1 ) = false ").Execute(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" Not(false = true) = true ").Execute(json0));

            Assert.IsTrue(new JsonQueryRuntime(@" Not( IsNull(name) ) ").Execute(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" Not(Not(Not(true))) = false ").Execute(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" Not(Not(Not(false))) = true ").Execute(json0));

            Assert.IsFalse(new JsonQueryRuntime(@" Not(b) ").Execute(json0));
            Assert.IsFalse(new JsonQueryRuntime(@" Not(IsNull(nil)) ").Execute(json0));
        }

        [TestMethod]
        public void Format_Number()
        {
            Assert.IsTrue(new JsonQueryRuntime(@" WriteLine(Format(n, '0000.000')) AND Format(n, '0000.000') = '0123.000' ").Execute(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" Format(123.1, '0000.000') = '0123.100' ").Execute(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" Format(n, '0000.000') = '0123.000' ").Execute(json0));
        }
        [TestMethod]
        public void Format_DateTime()
        {
            // 2018-12-25 08:23:49
            Assert.IsTrue(new JsonQueryRuntime(@" WriteLine ( Format(now, 'yyyy-MM-dd hh:mm:ss') ) ").Execute(json0));
            Assert.IsTrue(new JsonQueryRuntime(@"
                Var('dateFormat', 'yyyy-MM-dd hh:mm:ss') AND
                WriteLine ( Format(now, dateFormat) ) AND
                Format(now, dateFormat) = '2018-12-25 08:23:49'
            ").Execute(json0));

            Assert.IsTrue(new JsonQueryRuntime(@"
                WriteLine ( Format(now, dateFormat) ) AND
                Format(now, dateFormat) = '2018-12-25 08:23:49'
            ",
            @"
                Var('dateFormat', 'yyyy-MM-dd hh:mm:ss')
            "
            ).Execute(json0));
        }
    }
}



/*
var d = System.DateTime.UtcNow;
var s = Newtonsoft.Json.JsonConvert.SerializeObject(d, Newtonsoft.Json.Formatting.Indented);
*/