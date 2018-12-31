using HiSystems.Interpreter;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Diagnostics;
using JSonQueryRunTimeNS;
using JSonQueryRunTime;
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
    }
}
