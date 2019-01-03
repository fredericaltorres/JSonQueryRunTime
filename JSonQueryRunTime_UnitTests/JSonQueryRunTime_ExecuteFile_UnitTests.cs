using HiSystems.Interpreter;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Diagnostics;
using JsonQueryRunTimeNS;

namespace JSonQueryRunTime_UnitTests
{
    [TestClass]
    public class JSonQueryRunTime_ExecuteFile_UnitTests
    {
        const string jsonArrayOfObjectFileName = @".\Test Files\jsonArrayOfObject.json";

        [TestMethod]
        [DeploymentItem(jsonArrayOfObjectFileName)]
        public void Execute_Execute_File_WithArrayOfObject()
        {
            var resultLines = new JsonQueryRuntime(@" _id= ""5c2d299add266f6d68570885"" ").ExecuteFile(jsonArrayOfObjectFileName, isJsonLine: false).ToList();
            Assert.AreEqual(1, resultLines.Count);

            resultLines = new JsonQueryRuntime(@"
                eyeColor = 'blue' AND
                age = 37 AND
                name.first = 'Nancy' AND
                Contains(tags, Array('laboris', 'ea')) AND
                EqualArray(range, Array(0,1,2,3,4,5,6,7,8,9))
            ").ExecuteFile(jsonArrayOfObjectFileName, isJsonLine: false).ToList();
            Assert.AreEqual(1, resultLines.Count);

            resultLines = new JsonQueryRuntime(@" eyeColor = 'blue' AND age = 37 AND Path('name.first') = ""Nancy"" AND Contains(tags, Array(""laboris"", ""ea"", ""BAD-VALUE""))").ExecuteFile(jsonArrayOfObjectFileName, isJsonLine: false).ToList();
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

        
    }
}
