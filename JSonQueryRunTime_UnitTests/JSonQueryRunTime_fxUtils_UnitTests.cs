using HiSystems.Interpreter;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Diagnostics;
using JsonQueryRunTimeNS;
using JsonQueryRunTime;
using Newtonsoft.Json.Linq;

namespace JSonQueryRunTime_UnitTests
{
    [TestClass]
    public class JSonQueryRunTime_fxUtils_UnitTests
    {

        [TestMethod]
        public void ConvertInterpreterTypeIntoJTokenType()
        {
            Assert.AreEqual(JTokenType.Float, fxUtils.ConvertInterpreterTypeIntoJTokenType(new Number(1)));
            Assert.AreEqual(JTokenType.String, fxUtils.ConvertInterpreterTypeIntoJTokenType(new Text("")));
            Assert.AreEqual(JTokenType.Boolean, fxUtils.ConvertInterpreterTypeIntoJTokenType(new Boolean(true)));
            Assert.AreEqual(JTokenType.Date, fxUtils.ConvertInterpreterTypeIntoJTokenType(new DateTime(System.DateTime.Now)));
            Assert.AreEqual(JTokenType.Null, fxUtils.ConvertInterpreterTypeIntoJTokenType(new Null(null)));
        }    

        [TestMethod]
        public void ConvertInterpreterArrayTypeIntoJTokenType()
        {
            var stringArray = fxUtils.BuildStringArray(new List<string>() { "a","b","c"});
            Assert.AreEqual(JTokenType.String, fxUtils.ConvertInterpreterArrayTypeIntoJTokenType(stringArray));

            var decimalArray = fxUtils.BuildNumberArray(new List<decimal>() { 1,2,3});
            Assert.AreEqual(JTokenType.Float, fxUtils.ConvertInterpreterArrayTypeIntoJTokenType(decimalArray));

            var boolArray = fxUtils.BuildBooleanArray(new List<bool>() { true, false, true});
            Assert.AreEqual(JTokenType.Boolean, fxUtils.ConvertInterpreterArrayTypeIntoJTokenType(boolArray));

            var dateArray = fxUtils.BuildDateArray(new List<DateTime>() { System.DateTime.Now, System.DateTime.UtcNow});
            Assert.AreEqual(JTokenType.Date, fxUtils.ConvertInterpreterArrayTypeIntoJTokenType(dateArray));
        }    

        [TestMethod]
        public void BuildBooleanArray()
        {
            var boolArray = fxUtils.BuildBooleanArray(new List<bool>() { true, false, true }).ToList();
            Assert.AreEqual(3, boolArray.Count());
            Assert.AreEqual(new HiSystems.Interpreter.Boolean(true), boolArray[0]);
            Assert.AreEqual(new HiSystems.Interpreter.Boolean(false), boolArray[1]);
            Assert.AreEqual(new HiSystems.Interpreter.Boolean(true), boolArray[2]);
        }    

        [TestMethod]
        public void BuildStringArray()
        {
            var stringArray = fxUtils.BuildStringArray(new List<string>() { "a", "b" }).ToList();
            Assert.AreEqual(2, stringArray.Count());
            Assert.AreEqual(new HiSystems.Interpreter.Text("a"), stringArray[0]);
            Assert.AreEqual(new HiSystems.Interpreter.Text("b"), stringArray[1]);
        }    

        [TestMethod]
        public void BuildNumberArray()
        {
            var numberArray = fxUtils.BuildNumberArray(new List<decimal>() { 1,2 }).ToList();
            Assert.AreEqual(2, numberArray.Count());
            Assert.AreEqual(new HiSystems.Interpreter.Number(1), numberArray[0]);
            Assert.AreEqual(new HiSystems.Interpreter.Number(2), numberArray[1]);
        }    

        [TestMethod]
        public void ResolveValueFromJToken()
        {
            JToken jToken = JToken.Parse("{ a:1 }") as JToken;
            var stringObj = @"{
  ""a"": 1
}";
            Assert.AreEqual(new HiSystems.Interpreter.Text(stringObj), fxUtils.ResolveValueFromJToken(jToken));

            jToken = JToken.Parse("1") as JToken;
            Assert.AreEqual(new HiSystems.Interpreter.Number(1), fxUtils.ResolveValueFromJToken(jToken));

            jToken = JToken.Parse("true") as JToken;
            Assert.AreEqual(new HiSystems.Interpreter.Boolean(true), fxUtils.ResolveValueFromJToken(jToken));

            jToken = JToken.Parse(@" ""a"" ") as JToken;
            Assert.AreEqual(new HiSystems.Interpreter.Text("a"), fxUtils.ResolveValueFromJToken(jToken));

            jToken = null;
            Assert.AreEqual(new HiSystems.Interpreter.Null(null), fxUtils.ResolveValueFromJToken(jToken));
        }    

        [TestMethod]
        public void EvalJsonDotNetPath_ParseSimpleObject()
        {
            JToken jo = JToken.Parse("{ a:1 }");
            JToken val = fxUtils.EvalJsonDotNetPath("a", jo as JObject);
            Assert.AreEqual(JTokenType.Integer , val.Type);
            Assert.AreEqual(1, (int)val);
        }

        [TestMethod]
        public void EvalJsonDotNetPath_ParseNestedObject()
        {
            JToken jo = JToken.Parse("{ a:{ b:true} }");
            JToken val = fxUtils.EvalJsonDotNetPath("a.b", jo as JObject);
            Assert.AreEqual(JTokenType.Boolean , val.Type);
            Assert.AreEqual(true, (bool)val);
        }

        [TestMethod]
        public void RemoveFirstToken()
        {
            Assert.AreEqual("b.c", fxUtils.RemoveFirstToken("a.b.c"));
            Assert.AreEqual("abc", fxUtils.RemoveFirstToken("abc"));
            Assert.AreEqual("", fxUtils.RemoveFirstToken(""));
        }

        [TestMethod, ExpectedException(typeof(System.ArgumentException))]
        public void RemoveFirstToken_NullParamater()
        {
            fxUtils.RemoveFirstToken(null);
        }

        [TestMethod]
        public void DerivePathWithUnknown_SimpleObject()
        {
            JObject jo = JToken.Parse("{ a:1, b:true }") as JObject;
            string currentPath = null;
            var result = fxUtils.DerivePathWithUnknown("?", ref currentPath, jo);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(".a", result[0]);
            Assert.AreEqual(".b", result[1]);
        }

        [TestMethod]
        public void DerivePathWithUnknown_NestedObject()
        {
            JObject jo = JToken.Parse("{ a:{ b:1} }") as JObject;
            string currentPath = null;
            var result = fxUtils.DerivePathWithUnknown("a.?", ref currentPath, jo);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(".a.b", result[0]);
        }
        
    }
}


