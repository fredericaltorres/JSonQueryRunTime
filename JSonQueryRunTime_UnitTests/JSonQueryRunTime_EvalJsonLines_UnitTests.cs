using HiSystems.Interpreter;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Diagnostics;
using JSonQueryRunTimeNS;

namespace JSonQueryRunTime_UnitTests
{
    [TestClass]
    public class JSonQueryRunTime_EvalJsonLines_UnitTests
    {
        public IEnumerable<string> GetJsonLines0()
        {
            var l = new List<string>();
            l.Add(JSonQueryRunTime_EvalOneJsonString_UnitTests.json0);
            l.Add(JSonQueryRunTime_EvalOneJsonString_UnitTests.json0);
            l.Add(JSonQueryRunTime_EvalOneJsonString_UnitTests.json0);
            l.Add(JSonQueryRunTime_EvalOneJsonString_UnitTests.json1);
            return l;
        }

        [TestMethod]
        public void Execute_String_Equal()
        {
            var resultLines = new JsonQueryRuntime(@"name = ""ok"" ").Eval(GetJsonLines0()).ToList();
            Assert.AreEqual(3, resultLines.Count);
            resultLines = new JsonQueryRuntime(@"name = ""foo"" ").Eval(GetJsonLines0()).ToList();
            Assert.AreEqual(0, resultLines.Count);
        }

        [TestMethod]
        public void Execute_String_Equal_Or()
        {
            var resultLines = new JsonQueryRuntime(@"name = ""ok"" OR name = ""ko"" ").Eval(GetJsonLines0()).ToList();
            Assert.AreEqual(4, resultLines.Count);
            resultLines = new JsonQueryRuntime(@"name = ""foo"" OR name = ""bar"" ").Eval(GetJsonLines0()).ToList();
            Assert.AreEqual(0, resultLines.Count);
            resultLines = new JsonQueryRuntime(@"name = ""ok"" AND name = ""ko"" ").Eval(GetJsonLines0()).ToList();
            Assert.AreEqual(0, resultLines.Count);
        }

        [TestMethod]
        public void Execute_String_Equal_And()
        {
            var resultLines = new JsonQueryRuntime(@"name = ""ok"" AND b = true ").Eval(GetJsonLines0()).ToList();
            Assert.AreEqual(3, resultLines.Count);

            resultLines = new JsonQueryRuntime(@"name = ""ok"" AND name = ""ko"" ").Eval(GetJsonLines0()).ToList();
            Assert.AreEqual(0, resultLines.Count);
        }

        [TestMethod]
        public void Execute_String_WildCard()
        {
            var resultLines = new JsonQueryRuntime(@" Wildcard(wildText, ""?BCD?"") ").Eval(GetJsonLines0()).ToList();
            Assert.AreEqual(3, resultLines.Count);
            resultLines = new JsonQueryRuntime(@" Wildcard(wildText, ""?BCD?"") OR Wildcard(wildText, ""XYZ"") ").Eval(GetJsonLines0()).ToList();
            Assert.AreEqual(4, resultLines.Count);
            resultLines = new JsonQueryRuntime(@" Wildcard(wildText, ""?BCD?"") AND Wildcard(wildText, ""XYZ"") ").Eval(GetJsonLines0()).ToList();
            Assert.AreEqual(0, resultLines.Count);
        }
    }
}
