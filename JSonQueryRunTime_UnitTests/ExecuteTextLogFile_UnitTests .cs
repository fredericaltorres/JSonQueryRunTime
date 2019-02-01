using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using JsonQueryRunTimeNS;

namespace JSonQueryRunTime_UnitTests
{
    [TestClass]
    public class ExecuteTextLogFile_UnitTests
    {
        const string LogTextFileName = @".\Test Files\log.txt";

        [TestMethod]
        [DeploymentItem(LogTextFileName)]
        public void Contains()
        {
            var resultLines = new JsonQueryRuntime(@" Contains(text, 'ABCD') ")
                .ExecuteFile(LogTextFileName, TextType.TEXT).ToList();
            Assert.AreEqual(10, resultLines.Count);

            resultLines = new JsonQueryRuntime(@" Contains(text, 'ZZZZ') ")
                .ExecuteFile(LogTextFileName, TextType.TEXT).ToList();
            Assert.AreEqual(1, resultLines.Count);
        }
    }
}
