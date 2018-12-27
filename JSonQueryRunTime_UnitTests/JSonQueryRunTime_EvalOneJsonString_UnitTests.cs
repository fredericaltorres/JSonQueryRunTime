using HiSystems.Interpreter;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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

        [TestMethod]
        public void Execute_Date_Utc()
        {
            Assert.IsTrue(new JsonQueryRuntime(@" utcnow = #2018-12-26 01:24:46# ").Eval(json0));
            Assert.IsFalse(new JsonQueryRuntime(@" utcnow = #2018-12-26 01:24:47# ").Eval(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" utcnow <> #2018-12-26 01:24:47# ").Eval(json0));
        }
        [TestMethod]
        public void Execute_Date_GreaterThan_OrEqual_Utc()
        {
            Assert.IsTrue(new JsonQueryRuntime(@" utcnow >= #2018-12-26 01:24:46# ").Eval(json0));
            Assert.IsFalse(new JsonQueryRuntime(@" utcnow > #2018-12-26 01:24:46# ").Eval(json0));

            Assert.IsTrue(new JsonQueryRuntime(@" utcnow <= #2018-12-26 01:24:46# ").Eval(json0));
            Assert.IsFalse(new JsonQueryRuntime(@" utcnow < #2018-12-26 01:24:46# ").Eval(json0));
        }
        [TestMethod]
        public void Execute_TimeStamp_Range_Utc()
        {
            //#2018-12-26 01:24:46# ").Eval(json0));
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
            //#2018-12-26 01:24:46# ").Eval(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" 
                utcnow >= #2018-12-01# AND
                utcnow <= #2018-12-31# ").Eval(json0));

            Assert.IsFalse(new JsonQueryRuntime(@" 
                utcnow >= #2018-12-27# AND
                utcnow <= #2018-12-31# ").Eval(json0));
        }
        
        [TestMethod]
        public void Execute_DateRangeFunction_Utc()
        {
            //#2018-12-26 01:24:46# ").Eval(json0));
           
            Assert.IsTrue(new JsonQueryRuntime(@"
                DateRange(utcnow, #2018-12-01 01:24:46#, #2018-12-31 01:24:46#)
            ").Eval(json0));
            Assert.IsFalse(new JsonQueryRuntime(@"
                DateRange(utcnow, #2018-12-27 01:24:46#, #2018-12-31 01:24:46#)
            ").Eval(json0));
        }

        [TestMethod]
        public void Execute_Date_Local()
        {
            Assert.IsTrue(new JsonQueryRuntime(@" now = #2018-12-25 20:23:49# ").Eval(json0));
            Assert.IsFalse(new JsonQueryRuntime(@" now = #2018-12-25 20:23:50# ").Eval(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" now <> #2018-12-25 20:23:50# ").Eval(json0));
        }

        [TestMethod]
        public void Execute_WildCardFunction()
        {
            var d = System.DateTime.UtcNow;
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(d, Newtonsoft.Json.Formatting.Indented);

            Assert.IsTrue(new JsonQueryRuntime(@" Wildcard(wildText, ""ABCDE"") ").Eval(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" Wildcard(wildText, ""?BCD?"") ").Eval(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" Wildcard(wildText, ""?B?D?"") ").Eval(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" Wildcard(wildText, ""A*E"") ").Eval(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" Wildcard(wildText, ""A*C*E"") ").Eval(json0));
        }

        [TestMethod]
        public void Execute_StringBooleanNumeric_Equal_AND()
        {
            Assert.IsTrue(new JsonQueryRuntime(@"name = ""ok"" ").Eval(json0));
            Assert.IsTrue(new JsonQueryRuntime(@"name = ""ok"" AND b = true ").Eval(json0));
            Assert.IsTrue(new JsonQueryRuntime(@"name = ""ok"" AND b = true AND n = 123").Eval(json0));
        }

        [TestMethod]
        public void Execute_StringBooleanNumeric_NotEqual_AND()
        {
            Assert.IsTrue(new JsonQueryRuntime(@"name <> ""okk"" ").Eval(json0));
            Assert.IsTrue(new JsonQueryRuntime(@"name <> ""okk"" AND b <> false ").Eval(json0));
            Assert.IsTrue(new JsonQueryRuntime(@"name <> ""okk"" AND b <> false AND n <> 12.3").Eval(json0));
        }
        [TestMethod]
        public void Execute_StringBooleanNumeric_Equal_OR()
        {
            Assert.IsTrue(new JsonQueryRuntime(@"name = ""okkkk"" OR b = true").Eval(json0));
            Assert.IsTrue(new JsonQueryRuntime(@"name = ""ok"" AND (b = true OR b = false)").Eval(json0));
            Assert.IsTrue(new JsonQueryRuntime(@"name = ""ok"" OR b = true OR n = 123").Eval(json0));
        }

        [TestMethod]
        public void Execute_String_LenFunction()
        {
            Assert.IsTrue(new JsonQueryRuntime(@"len(name) = 2").Eval(json0));
        }

        //[TestMethod]
        //public void Execute_String_LessThan_GreaterThan_OrEqual()
        //{
        //    Assert.IsTrue(new JsonQueryRuntime(@"name < 1000 ").Eval(json0));
        //}

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
            Assert.IsTrue(new JsonQueryRuntime(@"InString(name, Array(""ok"", ""ko"")) ").Eval(json0));
            Assert.IsFalse(new JsonQueryRuntime(@"InString(name, Array(""a"", ""b"")) ").Eval(json0));
        }

        [TestMethod]
        public void Execute_InNumber_Function()
        {
            Assert.IsTrue(new JsonQueryRuntime(@"InNumber(n, Array(122, 123, 124)) ").Eval(json0));
            Assert.IsFalse(new JsonQueryRuntime(@"InNumber(n, Array(122, 124)) ").Eval(json0));
        }

        [TestMethod]
        public void Execute_NumberRangeFunction()
        {
            Assert.IsTrue(new JsonQueryRuntime(@"
                NumberRange(n, 122, 124)
            ").Eval(json0));

            Assert.IsFalse(new JsonQueryRuntime(@"
                NumberRange(n, 100, 110)
            ").Eval(json0));
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
        public void Execute_ContainArrayNumber()
        {
            Assert.IsTrue(new JsonQueryRuntime(@"ContainArrayNumber(arrNumber, Array(1, 3))").Eval(json0));
            Assert.IsFalse(new JsonQueryRuntime(@"ContainArrayNumber(arrNumber, Array(1, 3, 4))").Eval(json0));
        }
        [TestMethod]
        public void Execute_ContainArrayString()
        {
            Assert.IsTrue(new JsonQueryRuntime(@"ContainArrayString(arrString, Array(""a"", ""c""))").Eval(json0));
            Assert.IsFalse(new JsonQueryRuntime(@"ContainArrayString(arrString, Array(""a"", ""d""))").Eval(json0));
        }
        [TestMethod]
        public void Execute_ContainArrayBoolean()
        {
            Assert.IsTrue(new JsonQueryRuntime(@"ContainArrayBoolean(arrBoolean, Array(true, false))").Eval(json0));
        }
        [TestMethod]
        public void Execute_IsObject()
        {
            Assert.IsTrue(new JsonQueryRuntime(@"IsObject(obj0)").Eval(json0));
            Assert.IsFalse(new JsonQueryRuntime(@"IsObject(name)").Eval(json0));
            Assert.IsFalse(new JsonQueryRuntime(@"IsObject(n)").Eval(json0));
            Assert.IsFalse(new JsonQueryRuntime(@"IsObject(nil)").Eval(json0));
        }
        [TestMethod]
        public void Execute_Path_String()
        {
            //= ""ok""//
            Assert.IsTrue(new JsonQueryRuntime(@" Path(""obj0.name"")  = ""okk"" ").Eval(json0));
            Assert.IsTrue(new JsonQueryRuntime(@"IsObject(obj0) AND Path(""obj0.name"")  = ""okk"" ").Eval(json0));
        }
        [TestMethod]
        public void Execute_Path_DoubleNested_String()
        {
            Assert.IsTrue(new JsonQueryRuntime(@" Path(""obj0.obj00.name"") = ""okkk"" ").Eval(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" Path(""obj0.obj00.n"") = 1234 ").Eval(json0));

            Assert.IsTrue(new JsonQueryRuntime(@" Path(""obj0.obj00.name"") = ""okkk"" AND Path(""obj0.obj00.n"") = 1234 ").Eval(json0));
            Assert.IsTrue(new JsonQueryRuntime(@" ( Path(""obj0.obj00.name"") = ""okkk"" ) AND ( Path(""obj0.obj00.n"") = 1234 ) ").Eval(json0));
        }
        [TestMethod]
        public void Execute_Path_Number()
        {
            Assert.IsTrue(new JsonQueryRuntime(@" Path(""obj0.n"") = 124 ").Eval(json0));
        }
        [TestMethod]
        public void Execute_Path_Boolean()
        {
            Assert.IsTrue(new JsonQueryRuntime(@" Path(""obj0.b"") = true ").Eval(json0));
        }
    }
}
