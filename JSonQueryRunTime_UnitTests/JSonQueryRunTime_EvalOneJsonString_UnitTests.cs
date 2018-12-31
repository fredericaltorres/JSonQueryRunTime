﻿using HiSystems.Interpreter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JSonQueryRunTimeNS;

namespace JSonQueryRunTime_UnitTests
{
    [TestClass]
    public class JSonQueryRunTime_EvalOneJsonString_UnitTests
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
        public void Execute_Date_Utc()
        {
            Assert.IsTrue(new JsonQueryRuntime(@" utcnow = #2018-12-26 01:24:46# ").Eval(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" utcnow <> #2018-12-26 01:24:47# ").Eval(json0));

            Assert.IsFalse(new JsonQueryRuntime(@" utcnow = #2018-12-26 01:24:47# ").Eval(json0));
        }

        [TestMethod]
        public void Execute_Date_GreaterThan_OrEqual_Utc()
        {
            Assert.IsTrue (new JsonQueryRuntime(@" utcnow >= #2018-12-26 01:24:46# ").Eval(json0));
            Assert.IsFalse(new JsonQueryRuntime(@" utcnow > #2018-12-26 01:24:46# ").Eval(json0));

            Assert.IsTrue (new JsonQueryRuntime(@" utcnow <= #2018-12-26 01:24:46# ").Eval(json0));
            Assert.IsFalse(new JsonQueryRuntime(@" utcnow < #2018-12-26 01:24:46# ").Eval(json0));
        }

        [TestMethod]
        public void Execute_TimeStamp_Range_Utc()
        {
            Assert.IsTrue(new JsonQueryRuntime(@" 
                utcnow >= #2018-12-01 01:24:46# AND
                utcnow <= #2018-12-31 01:24:46# ").Eval(json0));

            Assert.IsTrue(new JsonQueryRuntime(@" 
                utcnow >= #2018-12-01# AND
                utcnow <= #2018-12-31# ").Eval(json0));

            Assert.IsFalse(new JsonQueryRuntime(@" 
                utcnow >= #2018-12-27 01:24:46# AND
                utcnow <= #2018-12-31 01:24:46# ").Eval(json0));
        }

        [TestMethod]
        public void Execute_DateNoTime_Range_Utc()
        {
            Assert.IsTrue(new JsonQueryRuntime(@" 
                utcnow >= #2018-12-01# AND
                utcnow <= #2018-12-31# ").Eval(json0));

            Assert.IsFalse(new JsonQueryRuntime(@" 
                utcnow >= #2018-12-27# AND
                utcnow <= #2018-12-31# ").Eval(json0));
        }

        [TestMethod]
        public void Execute_DateRange_Utc()
        {
            Assert.IsTrue (new JsonQueryRuntime(@" Range(utcnow, #2018-12-01 01:24:46#, #2018-12-31 01:24:46#) ").Eval(json0));
            Assert.IsFalse(new JsonQueryRuntime(@" Range(utcnow, #2018-12-27 01:24:46#, #2018-12-31 01:24:46#) ").Eval(json0));
        }

        [TestMethod]
        public void Execute_Range_String()
        {
            Assert.IsTrue(new JsonQueryRuntime(@" Range(""b"", ""a"", ""d"") ").Eval(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" Range(""aa"", ""a"", ""d"") ").Eval(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" Range(name, ""o"", ""p"") ").Eval(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" Range(name, ""oa"", ""p"") ").Eval(json0));

            Assert.IsFalse(new JsonQueryRuntime(@" Range(""daa"", ""a"", ""d"") ").Eval(json0));
            Assert.IsFalse(new JsonQueryRuntime(@" Range(name, ""a"", ""d"") ").Eval(json0));
            Assert.IsFalse(new JsonQueryRuntime(@" Range(name, ""ol"", ""p"") ").Eval(json0));
        }

        [TestMethod]
        public void Execute_Date_Local()
        {
            Assert.IsTrue(new JsonQueryRuntime(@" now = #2018-12-25 20:23:49# ").Eval(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" now <> #2018-12-25 20:23:50# ").Eval(json0));

            Assert.IsFalse(new JsonQueryRuntime(@" now = #2018-12-25 20:23:50# ").Eval(json0));
        }

        [TestMethod]
        public void Execute_WildCardFunction()
        {
            Assert.IsTrue(new JsonQueryRuntime(@" Wildcard(wildText, ""ABCDE"") ").Eval(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" Wildcard(wildText, ""?BCD?"") ").Eval(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" Wildcard(wildText, ""?B?D?"") ").Eval(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" Wildcard(wildText, ""A*E"") ").Eval(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" Wildcard(wildText, ""A*C*E"") ").Eval(json0));

            Assert.IsFalse(new JsonQueryRuntime(@" Wildcard(wildText, ""ABCDZ"") ").Eval(json0));
            Assert.IsFalse(new JsonQueryRuntime(@" Wildcard(wildText, ""?B-D?"") ").Eval(json0));
        }

        [TestMethod]
        public void Execute_RegexFunction()
        {
            Assert.IsTrue(new JsonQueryRuntime(@" Regex(wildText, ""ABCDE"") ").Eval(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" Regex(wildText, ""ABCDE"") ").Eval(json0));

            Assert.IsTrue(new JsonQueryRuntime(@" Regex(wildText, ""^ABCDE"") ").Eval(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" Regex(wildText, ""^ABC"") ").Eval(json0));

            Assert.IsTrue(new JsonQueryRuntime(@" Regex(wildText, ""ABCDE$"") ").Eval(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" Regex(wildText, ""CDE$"") ").Eval(json0));

            Assert.IsTrue(new JsonQueryRuntime(@" Regex(wildText, ""^ABCDE$"") ").Eval(json0));

            Assert.IsTrue(new JsonQueryRuntime(@" Regex(wildText, "".BCD."") ").Eval(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" Regex(wildText, "".B.D."") ").Eval(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" Regex(wildText, ""A.*E"") ").Eval(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" Regex(wildText, ""A.*C.*E"") ").Eval(json0));

            Assert.IsFalse(new JsonQueryRuntime(@" Regex(wildText, "".B-D."") ").Eval(json0));
            Assert.IsFalse(new JsonQueryRuntime(@" Regex(wildText, ""A.*-"") ").Eval(json0));

            Assert.IsFalse(new JsonQueryRuntime(@" Regex(wildText, ""zBCDE$"") ").Eval(json0));
            Assert.IsFalse(new JsonQueryRuntime(@" Regex(wildText, ""^zBCDE$"") ").Eval(json0));
            Assert.IsFalse(new JsonQueryRuntime(@" Regex(wildText, "".B-D."") ").Eval(json0));

            Assert.IsTrue(JSonQueryRunTime.fxRegex.RegexCache.Count == 14);
            JSonQueryRunTime.fxRegex.RegexCache.Clear();
            Assert.IsTrue(JSonQueryRunTime.fxRegex.RegexCache.Count == 0);
        }

        [TestMethod]
        public void Execute_StringBooleanNumeric_Equal_AND()
        {
            Assert.IsTrue(new JsonQueryRuntime(@"name = ""ok"" ").Eval(json0));
            Assert.IsTrue(new JsonQueryRuntime(@"name = ""ok"" AND b = true ").Eval(json0));
            Assert.IsTrue(new JsonQueryRuntime(@"name = ""ok"" AND b = true AND n = 123").Eval(json0));

            Assert.IsFalse(new JsonQueryRuntime(@"name = ""ok"" AND b = false AND n = 123").Eval(json0));
            Assert.IsFalse(new JsonQueryRuntime(@"name = ""ok"" AND b = true AND n = 123 AND 1 = 2").Eval(json0));
        }

        [TestMethod]
        public void Execute_StringBooleanNumeric_NotEqual_AND()
        {
            Assert.IsTrue(new JsonQueryRuntime(@"name <> ""okk"" ").Eval(json0));
            Assert.IsTrue(new JsonQueryRuntime(@"name <> ""okk"" AND b <> false ").Eval(json0));
            Assert.IsTrue(new JsonQueryRuntime(@"name <> ""okk"" AND b <> false AND n <> 12.3").Eval(json0));

            Assert.IsFalse(new JsonQueryRuntime(@"name <> ""okk"" AND b <> true AND n <> 12.3").Eval(json0));
        }
        [TestMethod]
        public void Execute_StringBooleanNumeric_Equal_OR()
        {
            Assert.IsTrue(new JsonQueryRuntime(@"name = ""okkkk"" OR b = true").Eval(json0));
            Assert.IsTrue(new JsonQueryRuntime(@"name = ""ok"" AND (b = true OR b = false)").Eval(json0));
            Assert.IsTrue(new JsonQueryRuntime(@"name = ""ok"" OR b = true OR n = 123").Eval(json0));

            Assert.IsFalse(new JsonQueryRuntime(@"name = ""okkkk"" OR b = false OR n = 553").Eval(json0));
        }

        [TestMethod]
        public void Execute_String_LenFunction()
        {
            Assert.IsTrue(new JsonQueryRuntime(@"len(name) = 2").Eval(json0));
        }

        [TestMethod]
        public void Execute_Numeric_LessThan_GreaterThan_OrEqual()
        {
            Assert.IsTrue(new JsonQueryRuntime(@"n < 124").Eval(json0));
            Assert.IsFalse(new JsonQueryRuntime(@"n > 124").Eval(json0));

            Assert.IsTrue(new JsonQueryRuntime(@"n <= 123").Eval(json0));
            Assert.IsTrue(new JsonQueryRuntime(@"n >= 123").Eval(json0));
            Assert.IsFalse(new JsonQueryRuntime(@"n >= 124").Eval(json0));
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
        public void Execute_InString_Function()
        {
            Assert.IsTrue(new JsonQueryRuntime(@"In(name, Array(""ok"", ""ko"")) ").Eval(json0));
            Assert.IsFalse(new JsonQueryRuntime(@"In(name, Array(""a"", ""b"")) ").Eval(json0));
        }

        [TestMethod]
        public void Execute_InNumber_Function()
        {
            Assert.IsTrue(new JsonQueryRuntime(@"In(n, Array(122, 123, 124)) ").Eval(json0));
            Assert.IsFalse(new JsonQueryRuntime(@"In(n, Array(122, 124)) ").Eval(json0));
        }

        [TestMethod]
        public void Execute_NumberRangeFunction()
        {
            Assert.IsTrue(new JsonQueryRuntime(@" Range(n, 122, 124) ").Eval(json0));
            Assert.IsFalse(new JsonQueryRuntime(@" Range(n, 100, 110) ").Eval(json0));
        }

        [TestMethod]
        public void Execute_IsNumber()
        {
            Assert.IsTrue(new JsonQueryRuntime(@"IsNumber(n)").Eval(json0));
            Assert.IsTrue(new JsonQueryRuntime(@"IsNumber(1)").Eval(json0));
            Assert.IsTrue(new JsonQueryRuntime(@"Not( IsNumber(nil) )").Eval(json0));

            Assert.IsFalse(new JsonQueryRuntime(@"IsNumber(name)").Eval(json0));
            Assert.IsFalse(new JsonQueryRuntime(@"IsNumber(b)").Eval(json0));
            Assert.IsFalse(new JsonQueryRuntime(@"IsNumber(now)").Eval(json0));
            Assert.IsFalse(new JsonQueryRuntime(@"IsNumber(nil)").Eval(json0));
        }

        [TestMethod]
        public void Execute_IsBoolean()
        {
            Assert.IsTrue(new JsonQueryRuntime(@"IsBoolean(b)").Eval(json0));
            Assert.IsTrue(new JsonQueryRuntime(@"IsBoolean(true)").Eval(json0));
            Assert.IsTrue(new JsonQueryRuntime(@"IsBoolean(false)").Eval(json0));
            Assert.IsTrue(new JsonQueryRuntime(@"IsBoolean( Not( b ) )").Eval(json0));
            Assert.IsTrue(new JsonQueryRuntime(@"IsBoolean( Not( false ) )").Eval(json0));
            Assert.IsTrue(new JsonQueryRuntime(@"IsBoolean( Not( true ) )").Eval(json0));

            Assert.IsFalse(new JsonQueryRuntime(@"IsBoolean(name)").Eval(json0));
            Assert.IsFalse(new JsonQueryRuntime(@"IsBoolean(now)").Eval(json0));
            Assert.IsFalse(new JsonQueryRuntime(@"IsBoolean(nil)").Eval(json0));

            Assert.IsTrue(new JsonQueryRuntime(@"Not( IsBoolean(nil) )").Eval(json0));
        }

        [TestMethod]
        public void Execute_IsDate()
        {
            Assert.IsTrue(new JsonQueryRuntime(@"IsDate(#2018-12-26T01:24:46.0#)").Eval(json0));
            Assert.IsTrue(new JsonQueryRuntime(@"IsDate(now)").Eval(json0));
            Assert.IsTrue(new JsonQueryRuntime(@"IsDate(utcnow)").Eval(json0));
            Assert.IsTrue(new JsonQueryRuntime(@"IsDate(Today())").Eval(json0));
            Assert.IsTrue(new JsonQueryRuntime(@"IsDate(#2018-12-25T20:23:49.0-05:00#)").Eval(json0));

            Assert.IsFalse(new JsonQueryRuntime(@"IsDate(name)").Eval(json0));
            Assert.IsFalse(new JsonQueryRuntime(@"IsDate(n)").Eval(json0));
            Assert.IsFalse(new JsonQueryRuntime(@"IsDate(b)").Eval(json0));
            Assert.IsFalse(new JsonQueryRuntime(@"IsDate(nil)").Eval(json0));

            Assert.IsTrue(new JsonQueryRuntime(@"Not( IsDate(nil) )").Eval(json0));
        }

        [TestMethod]
        public void Execute_IsArray()
        {
            Assert.IsTrue(new JsonQueryRuntime(@"IsArray(Array(1,2))").Eval(json0));

            Assert.IsTrue(new JsonQueryRuntime(@"IsArray(arrString)").Eval(json0));
            Assert.IsTrue(new JsonQueryRuntime(@"IsArray(arrNumber)").Eval(json0));
            Assert.IsTrue(new JsonQueryRuntime(@"IsArray(arrBoolean)").Eval(json0));

            Assert.IsFalse(new JsonQueryRuntime(@"IsArray(now)").Eval(json0));
            Assert.IsFalse(new JsonQueryRuntime(@"IsArray(utcnow)").Eval(json0));
            Assert.IsFalse(new JsonQueryRuntime(@"IsArray(Today())").Eval(json0));
            Assert.IsFalse(new JsonQueryRuntime(@"IsArray(#2018-12-25T20:23:49.0-05:00#)").Eval(json0));
            Assert.IsFalse(new JsonQueryRuntime(@"IsArray(name)").Eval(json0));
            Assert.IsFalse(new JsonQueryRuntime(@"IsArray(n)").Eval(json0));
            Assert.IsFalse(new JsonQueryRuntime(@"IsArray(b)").Eval(json0));
            Assert.IsFalse(new JsonQueryRuntime(@"IsArray(nil)").Eval(json0));

            Assert.IsTrue(new JsonQueryRuntime(@"Not( IsArray(nil) )").Eval(json0));
        }

        [TestMethod]
        public void Execute_IsString()
        {
            Assert.IsTrue(new JsonQueryRuntime(@"IsString(name)").Eval(json0));

            Assert.IsFalse(new JsonQueryRuntime(@"IsString(n)").Eval(json0));
            Assert.IsFalse(new JsonQueryRuntime(@"IsString(b)").Eval(json0));
            Assert.IsFalse(new JsonQueryRuntime(@"IsString(now)").Eval(json0));
            Assert.IsFalse(new JsonQueryRuntime(@"IsString(nil)").Eval(json0));
        }

        [TestMethod]
        public void Execute_IsString_WithPathFunctionAsParameter()
        {
            Assert.IsTrue(new JsonQueryRuntime(@"IsString( Path( ""name"" ) )").Eval(json0));
            Assert.IsTrue(new JsonQueryRuntime(@"IsString( Path ( ""obj0.name"" ) )").Eval(json0));
        }

        [TestMethod, ExpectedException(typeof(System.InvalidOperationException))]
        public void Execute_ContainsWithEmptyArrayAsParameter_ThrowArgumentException()
        {
            Assert.IsTrue(new JsonQueryRuntime(@"Contains(arrNumber, Array())").Eval(json0));
        }

        [TestMethod]
        public void Execute_ContainsString()
        {
            Assert.IsTrue(new JsonQueryRuntime(@"Contains(name, ""ok"")").Eval(json0));
            Assert.IsTrue(new JsonQueryRuntime(@"Contains(name, ""ok"")").Eval(json0));

            Assert.IsFalse(new JsonQueryRuntime(@"Contains(wildText, ""ok"")").Eval(json0));
        }

        [TestMethod, ExpectedException(typeof(System.ArgumentException))]
        public void Execute_ContainsString_FirstArgumentNotAStringThrowException()
        {
            Assert.IsFalse(new JsonQueryRuntime(@"Contains(n, ""ok"")").Eval(json0));
        }

        [TestMethod]
        public void Execute_ContainsArrayNumber()
        {
            Assert.IsTrue(new JsonQueryRuntime(@"Contains(arrNumber, Array(1, 3))").Eval(json0));
            Assert.IsTrue(new JsonQueryRuntime(@"Contains(arrNumber, Array(1, 2, 3))").Eval(json0));

            Assert.IsFalse(new JsonQueryRuntime(@"Contains(arrNumber, Array(1, 3, 4))").Eval(json0));
        }

        [TestMethod]
        public void Execute_EqualArrayNumber()
        {
            Assert.IsTrue(new JsonQueryRuntime(@"EqualArray(arrNumber, Array(1, 2, 3))").Eval(json0));

            Assert.IsFalse(new JsonQueryRuntime(@"EqualArray(arrNumber, Array(1, 2, 3, 4))").Eval(json0));
            Assert.IsFalse(new JsonQueryRuntime(@"EqualArray(arrNumber, Array(1, 2, 4))").Eval(json0));
            Assert.IsFalse(new JsonQueryRuntime(@"EqualArray(arrNumber, Array(1))").Eval(json0));
        }

        [TestMethod]
        public void Execute_ContainsArrayString()
        {
            Assert.IsTrue(new JsonQueryRuntime(@"Contains(arrString, Array(""a"", ""c""))").Eval(json0));
            Assert.IsTrue(new JsonQueryRuntime(@"Contains(arrString, Array(""a"", ""c"", ""b""))").Eval(json0));
            Assert.IsTrue(new JsonQueryRuntime(@"Contains(arrString, Array(""a""))").Eval(json0));

            Assert.IsFalse(new JsonQueryRuntime(@"Contains(arrString, Array(""a"", ""d""))").Eval(json0));
        }

        [TestMethod]
        public void Execute_EqualArrayString()
        {
            Assert.IsTrue(new JsonQueryRuntime(@"EqualArray(arrString, Array(""a"", ""b"", ""c""))").Eval(json0));

            Assert.IsFalse(new JsonQueryRuntime(@"EqualArray(arrString, Array(""a"", ""b""))").Eval(json0));
            Assert.IsFalse(new JsonQueryRuntime(@"EqualArray(arrString, Array(""a"", ""c"", ""b""))").Eval(json0));
        }

        [TestMethod]
        public void Execute_EqualArrayBoolean()
        {
            Assert.IsTrue(new JsonQueryRuntime(@"EqualArray(arrBoolean, Array(true, false, true))").Eval(json0));

            Assert.IsFalse(new JsonQueryRuntime(@"EqualArray(arrBoolean, Array(true))").Eval(json0));
            Assert.IsFalse(new JsonQueryRuntime(@"EqualArray(arrBoolean, Array(false))").Eval(json0));
        }

        [TestMethod]
        public void Execute_ContainsArrayBoolean()
        {
            Assert.IsTrue(new JsonQueryRuntime(@"Contains(arrBoolean, Array(true, false))").Eval(json0));
            Assert.IsTrue(new JsonQueryRuntime(@"Contains(arrBoolean, Array(true))").Eval(json0));
            Assert.IsTrue(new JsonQueryRuntime(@"Contains(arrBoolean, Array(false))").Eval(json0));
        }

        [TestMethod]
        public void Execute_IsObject()
        {
            Assert.IsTrue(new JsonQueryRuntime(@"IsObject(obj0)").Eval(json0));

            Assert.IsFalse(new JsonQueryRuntime(@"IsObject(name)").Eval(json0));
            Assert.IsFalse(new JsonQueryRuntime(@"IsObject(n)").Eval(json0));
            Assert.IsFalse(new JsonQueryRuntime(@"IsObject(nil)").Eval(json0));
            Assert.IsFalse(new JsonQueryRuntime(@" IsObject(""aa"") ").Eval(json0));
            Assert.IsFalse(new JsonQueryRuntime(@"IsObject(1)").Eval(json0));
            Assert.IsFalse(new JsonQueryRuntime(@"IsObject(null)").Eval(json0));
            Assert.IsFalse(new JsonQueryRuntime(@"IsObject(#1964-12-11#)").Eval(json0));
        }

        [TestMethod]
        public void Execute_Path_String()
        {
            Assert.IsTrue(new JsonQueryRuntime(@" Path(""obj0.name"")  = ""okk"" ").Eval(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" Path(""obj0.name"")  = ""okk"" ").Eval(json0));
            Assert.IsTrue(new JsonQueryRuntime(@"IsObject(obj0) AND Path(""obj0.name"")  = ""okk"" ").Eval(json0));
        }

        [TestMethod]
        public void Execute_Path_DoubleNested_String()
        {
            Assert.IsTrue(new JsonQueryRuntime(@" Path(""obj0.obj00.name"") = ""okkk"" ").Eval(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" Path(""obj0.obj00.n"") = 1234 ").Eval(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" Path(""obj0.obj00.name"") = ""okkk"" AND Path(""obj0.obj00.n"") = 1234 ").Eval(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" ( Path(""obj0.obj00.name"" ) = ""okkk"" ) AND ( Path(""obj0.obj00.n"") = 1234 ) ").Eval(json0));
        }

        [TestMethod]
        public void Execute_Path_Number()
        {
            Assert.IsTrue(new JsonQueryRuntime(@" Path(""obj0.n"") = 124 ").Eval(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" Path(""obj0.obj00.n"") = 1234 ").Eval(json0));
            Assert.IsFalse(new JsonQueryRuntime(@" Path(""obj0.n"") = 1246666 ").Eval(json0));
        }

        [TestMethod, ExpectedException(typeof(System.InvalidOperationException))]
        public void Execute_Path_ReturnsStringExpectNumber_Throw()
        {
            // Wrong type
            Assert.IsFalse(new JsonQueryRuntime(@" Path(""obj0.name"") = 123 ").Eval(json0));
        }

        [TestMethod]
        public void Execute_Path_Boolean()
        {
            Assert.IsTrue(new JsonQueryRuntime(@" Path(""obj0.b"") = true ").Eval(json0));
            // obj0.b return true, so Path() can be evaluate like that, only for boolean
            Assert.IsTrue(new JsonQueryRuntime(@" Path(""obj0.b"")  ").Eval(json0));
        }

        [TestMethod]
        public void Execute_Path_Empty()
        {
            Assert.IsTrue(new JsonQueryRuntime(@" Path("""") = null ").Eval(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" IsNull( Path("""") ) ").Eval(json0));
        }

        [TestMethod]
        public void Execute_Path_PatternMatching()
        {
            // property n
            Assert.IsTrue(new JsonQueryRuntime(@" Path(""?"", 123)   ").Eval(json0));
            // property name
            Assert.IsTrue(new JsonQueryRuntime(@" Path(""?"", ""ok"")   ").Eval(json0));
            // property b
            Assert.IsTrue(new JsonQueryRuntime(@" Path(""?"", true)   ").Eval(json0));

            Assert.IsTrue(new JsonQueryRuntime(@" Path(""obj0.obj00.?"", 1234)   ").Eval(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" Path(""obj0.?"", 124)  ").Eval(json0));
        }


        [TestMethod]
        public void Execute_IsObject_Path()
        {
            var queryManufacturer = "Manufacturers[?(@.Name == 'Acme Co')]";
            Assert.IsTrue(new JsonQueryRuntime($@" IsObject( Path(""{queryManufacturer}"") )  ").Eval(json_store));
        }


        [TestMethod]
        public void Execute_Path_QueryObjectInArray()
        {
            var queryManufacturer = "Manufacturers[?(@.Name == 'Acme Co')]";
            Assert.IsTrue(new JsonQueryRuntime($@" IsObject( Path(""{queryManufacturer}"") )  ").Eval(json_store));
            Assert.IsTrue(new JsonQueryRuntime($@" Path(""{queryManufacturer}.Name"") = ""Acme Co"" ").Eval(json_store));
            Assert.IsTrue(new JsonQueryRuntime($@" Path(""{queryManufacturer}.Name"") = ""Acme Co"" ").Eval(json_store));
            Assert.IsTrue(new JsonQueryRuntime($@" Path(""{queryManufacturer}.Products[0].Price"") = 50 ").Eval(json_store));

            var queryProductPerPrice = ".Products[?(@.Price == 4)]";
            Assert.IsTrue(new JsonQueryRuntime($@" Path(""{queryProductPerPrice}.Name"") = ""Headlight Fluid"" ").Eval(json_store));
            Assert.IsTrue(new JsonQueryRuntime($@" IsObject( Path(""{queryProductPerPrice}"") ) ").Eval(json_store));
            Assert.IsTrue(new JsonQueryRuntime($@" Not( Not( IsObject( Path(""{queryProductPerPrice}"") ) ) )").Eval(json_store));
        }

        [TestMethod]
        public void Execute_IsNull()
        {
            Assert.IsTrue(new JsonQueryRuntime(@" IsNull(null) ").Eval(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" IsNull(nil) ").Eval(json0));

            Assert.IsFalse(new JsonQueryRuntime(@" IsNull(name) ").Eval(json0));
            Assert.IsFalse(new JsonQueryRuntime(@" IsNull(n) ").Eval(json0));
            Assert.IsFalse(new JsonQueryRuntime(@" IsNull(1) ").Eval(json0));
            Assert.IsFalse(new JsonQueryRuntime(@" IsNull(true) ").Eval(json0));
            Assert.IsFalse(new JsonQueryRuntime(@" IsNull(false) ").Eval(json0));
            Assert.IsFalse(new JsonQueryRuntime(@" IsNull(#2018-12-26 01:24:46#) ").Eval(json0));
            Assert.IsFalse(new JsonQueryRuntime(@" IsNull(""aaa"") ").Eval(json0));
        }

        [TestMethod]
        public void CompareString()
        {
            Assert.IsTrue(string.Compare("bb", "aa") == 1);
            Assert.IsTrue(string.Compare("bb", "aaa") == 1);
            Assert.IsTrue(string.Compare("a", "b") == -1);
        }

        [TestMethod]
        public void Execute_Not()
        {
            Assert.IsTrue(new JsonQueryRuntime(@" Not(true) = false ").Eval(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" Not(false) = true ").Eval(json0));

            Assert.IsTrue(new JsonQueryRuntime(@" Not( 1 = 1 ) = false ").Eval(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" Not(false = true) = true ").Eval(json0));

            Assert.IsTrue(new JsonQueryRuntime(@" Not( IsNull(name) ) ").Eval(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" Not(Not(Not(true))) = false ").Eval(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" Not(Not(Not(false))) = true ").Eval(json0));

            Assert.IsFalse(new JsonQueryRuntime(@" Not(b) ").Eval(json0));
            Assert.IsFalse(new JsonQueryRuntime(@" Not(IsNull(nil)) ").Eval(json0));
        }
    }
}



/*
var d = System.DateTime.UtcNow;
var s = Newtonsoft.Json.JsonConvert.SerializeObject(d, Newtonsoft.Json.Formatting.Indented);
*/