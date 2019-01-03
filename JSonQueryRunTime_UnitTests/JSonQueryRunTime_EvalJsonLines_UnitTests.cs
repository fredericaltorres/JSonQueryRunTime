using HiSystems.Interpreter;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Diagnostics;
using JsonQueryRunTimeNS;

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

        string jsonArrayOfObjectFileName = @"C:\DVT\.NET\JSonQueryRunTime\JSonQueryRunTime_UnitTests\Test Files\jsonArrayOfObject.json";

        [TestMethod]
        public void Execute_Execute_File_WithArrayOfObject()
        {
            var resultLines = new JsonQueryRuntime(@" _id= ""5c2d299add266f6d68570885"" ").ExecuteFile(jsonArrayOfObjectFileName, isJsonLine: false).ToList();
            Assert.AreEqual(1, resultLines.Count);

            resultLines = new JsonQueryRuntime(@"
                eyeColor = ""blue"" AND
                age = 37 AND
                name.first = ""Nancy"" AND
                Contains(tags, Array(""laboris"", ""ea"")) AND
                EqualArray(range, Array(0,1,2,3,4,5,6,7,8,9))
            ").ExecuteFile(jsonArrayOfObjectFileName, isJsonLine: false).ToList();
            Assert.AreEqual(1, resultLines.Count);

            resultLines = new JsonQueryRuntime(@" eyeColor = ""blue"" AND age = 37 AND Path(""name.first"") = ""Nancy"" AND Contains(tags, Array(""laboris"", ""ea"", ""BAD-VALUE""))").ExecuteFile(jsonArrayOfObjectFileName, isJsonLine: false).ToList();
            Assert.AreEqual(0, resultLines.Count);
        }

        [TestMethod]
        public void Execute_Execute_File_WithArrayOfObject_QuerySubObject()
        {
            var resultLines = new JsonQueryRuntime(@"
                 IsObject ( Path ( ""friends[?(@.name == 'Harmon Blankenship')]"" ) ) OR
                 IsObject ( Path ( ""friends[?(@.name == 'Juanita Chapman')]"" ) ) 
            ").ExecuteFile(jsonArrayOfObjectFileName, isJsonLine: false).ToList();
            Assert.AreEqual(2, resultLines.Count);
        }

        [TestMethod]
        public void Execute_String_Equal()
        {
            var resultLines = new JsonQueryRuntime(@"name = ""ok"" ").Execute(GetJsonLines0()).ToList();
            Assert.AreEqual(3, resultLines.Count);
            resultLines = new JsonQueryRuntime(@"name = ""foo"" ").Execute(GetJsonLines0()).ToList();
            Assert.AreEqual(0, resultLines.Count);
        }

        [TestMethod]
        public void Execute_String_Equal_Or()
        {
            var resultLines = new JsonQueryRuntime(@"name = ""ok"" OR name = ""ko"" ").Execute(GetJsonLines0()).ToList();
            Assert.AreEqual(4, resultLines.Count);
            resultLines = new JsonQueryRuntime(@"name = ""foo"" OR name = ""bar"" ").Execute(GetJsonLines0()).ToList();
            Assert.AreEqual(0, resultLines.Count);
            resultLines = new JsonQueryRuntime(@"name = ""ok"" AND name = ""ko"" ").Execute(GetJsonLines0()).ToList();
            Assert.AreEqual(0, resultLines.Count);
        }

        [TestMethod]
        public void Execute_String_Equal_And()
        {
            var resultLines = new JsonQueryRuntime(@"name = ""ok"" AND b = true ").Execute(GetJsonLines0()).ToList();
            Assert.AreEqual(3, resultLines.Count);

            resultLines = new JsonQueryRuntime(@"name = ""ok"" AND name = ""ko"" ").Execute(GetJsonLines0()).ToList();
            Assert.AreEqual(0, resultLines.Count);
        }

        [TestMethod]
        public void Execute_String_WildCard()
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
