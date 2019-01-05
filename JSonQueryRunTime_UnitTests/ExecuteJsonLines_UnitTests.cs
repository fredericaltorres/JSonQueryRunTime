using HiSystems.Interpreter;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Diagnostics;
using JsonQueryRunTimeNS;

namespace JSonQueryRunTime_UnitTests
{
    [TestClass]
    public class ExecuteJsonLines_UnitTests
    {
        public IEnumerable<string> GetJsonLines0()
        {
            var l = new List<string>();
            l.Add(ExecuteOneJsonString_UnitTests.json0);
            l.Add(ExecuteOneJsonString_UnitTests.json0);
            l.Add(ExecuteOneJsonString_UnitTests.json0);
            l.Add(ExecuteOneJsonString_UnitTests.json1);
            return l;
        }

        [TestMethod]
        public void String_Equal()
        {
            var resultLines = new JsonQueryRuntime(@"name = ""ok"" ").Execute(GetJsonLines0()).ToList();
            Assert.AreEqual(3, resultLines.Count);
            resultLines = new JsonQueryRuntime(@"name = ""foo"" ").Execute(GetJsonLines0()).ToList();
            Assert.AreEqual(0, resultLines.Count);
        }

        [TestMethod]
        public void String_Equal_Or()
        {
            var resultLines = new JsonQueryRuntime(@"name = ""ok"" OR name = ""ko"" ").Execute(GetJsonLines0()).ToList();
            Assert.AreEqual(4, resultLines.Count);
            resultLines = new JsonQueryRuntime(@"name = ""foo"" OR name = ""bar"" ").Execute(GetJsonLines0()).ToList();
            Assert.AreEqual(0, resultLines.Count);
            resultLines = new JsonQueryRuntime(@"name = ""ok"" AND name = ""ko"" ").Execute(GetJsonLines0()).ToList();
            Assert.AreEqual(0, resultLines.Count);
        }

        [TestMethod]
        public void String_Equal_And()
        {
            var resultLines = new JsonQueryRuntime(@"name = ""ok"" AND b = true ").Execute(GetJsonLines0()).ToList();
            Assert.AreEqual(3, resultLines.Count);

            resultLines = new JsonQueryRuntime(@"name = ""ok"" AND name = ""ko"" ").Execute(GetJsonLines0()).ToList();
            Assert.AreEqual(0, resultLines.Count);
        }

        [TestMethod]
        public void String_WildCard()
        {
            var resultLines = new JsonQueryRuntime(@" Wildcard(wildText, ""?BCD?"") ").Execute(GetJsonLines0()).ToList();
            Assert.AreEqual(3, resultLines.Count);
            resultLines = new JsonQueryRuntime(@" Wildcard(wildText, ""?BCD?"") OR Wildcard(wildText, ""XYZ"") ").Execute(GetJsonLines0()).ToList();
            Assert.AreEqual(4, resultLines.Count);
            resultLines = new JsonQueryRuntime(@" Wildcard(wildText, ""?BCD?"") AND Wildcard(wildText, ""XYZ"") ").Execute(GetJsonLines0()).ToList();
            Assert.AreEqual(0, resultLines.Count);
        }
    }
}
