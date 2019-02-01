using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using JsonQueryRunTimeNS;

namespace JSonQueryRunTime_UnitTests
{
    [TestClass]
    public class ExecuteJsonLogFile_UnitTests
    {
        const string JsonArrayOfObjectFileName = @".\Test Files\jsonArrayOfObject.json";
        const string JsonLinesFileName = @".\Test Files\json-lines.json";

        [TestMethod]
        public void JsonLineFile()
        {
            const int expectedCount = 100;

            var whereClauses = new List<string>()
            {
                @"name = 'ok'",
                @"name = 'ok' AND utcnow >= #2018-12-26 01:24:46#",
                @"name = 'ok' AND utcnow >= #2018-12-01 01:24:46# AND utcnow <= #2018-12-31 01:24:46# ",
                @" Range(utcnow, #2018-12-01 01:24:46#, #2018-12-31 01:24:46#) ",
                @" now = #2018-12-25 20:23:49# AND now <> #2018-12-25 20:23:50#",
                @" Wildcard(wildText, 'ABCDE') AND 
                   Wildcard(wildText, '?BCD?') AND
                   Wildcard(wildText, '?B?D?') AND
                   Wildcard(wildText, 'A*E')   AND
                   Wildcard(wildText, 'A*C*E') AND
                   Not( Wildcard(wildText, ""ABCDZ"") )
                "
            };
            whereClauses.Add(JsonQueryRuntime.CombineWhereClauseExpressions(whereClauses, "AND"));
            whereClauses.Add(JsonQueryRuntime.CombineWhereClauseExpressions(whereClauses, "OR"));

            foreach (var whereClause in whereClauses)
            {
                var resultLines = new JsonQueryRuntime(whereClause)
                        .ExecuteFile(JsonLinesFileName, TextType.JSON_LINES ).ToList();
                Assert.AreEqual(expectedCount, resultLines.Count);
            }
        }

        [TestMethod]
        [DeploymentItem(JsonArrayOfObjectFileName)]
        public void File_WithArrayOfObject()
        {
            var resultLines = new JsonQueryRuntime(@" _id= ""5c2d299add266f6d68570885"" ").ExecuteFile(JsonArrayOfObjectFileName, TextType.JSON).ToList();
            Assert.AreEqual(1, resultLines.Count);

            resultLines = new JsonQueryRuntime(@"
                eyeColor = 'blue' AND
                age = 37 AND
                name.first = 'Nancy' AND
                Contains(tags, Array('laboris', 'ea')) AND
                EqualArray(range, Array(0,1,2,3,4,5,6,7,8,9))
            ").ExecuteFile(JsonArrayOfObjectFileName, TextType.JSON).ToList();
            Assert.AreEqual(1, resultLines.Count);

            resultLines = new JsonQueryRuntime(@" 
                eyeColor = 'blue' AND 
                age = 37 AND 
                Path('name.first') = ""Nancy"" AND 
                Contains(tags, Array(""laboris"", ""ea"", ""BAD-VALUE""))
            ").ExecuteFile(JsonArrayOfObjectFileName, TextType.JSON).ToList();
            Assert.AreEqual(0, resultLines.Count);
        }

        [TestMethod]
        public void File_WithArrayOfObject_QuerySubObject()
        {
            var resultLines = new JsonQueryRuntime(@"
                 IsObject ( Path ( ""friends[?(@.name == 'Harmon Blankenship')]"" ) ) OR
                 IsObject ( Path ( ""friends[?(@.name == 'Juanita Chapman')]"" ) ) 
            ").ExecuteFile(JsonArrayOfObjectFileName, TextType.JSON).ToList();
            Assert.AreEqual(2, resultLines.Count);
        }

        [TestMethod]
        public void File_WithArrayOfObject_QueryInString()
        {
            var resultLines = new JsonQueryRuntime(@"
                In( company, Array('SQUISH', 'ZILLATIDE', 'SKINSERVE') )
            ").ExecuteFile(JsonArrayOfObjectFileName, TextType.JSON).ToList();
            Assert.AreEqual(3, resultLines.Count);
        }
    }
}
