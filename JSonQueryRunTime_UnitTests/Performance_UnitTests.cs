using HiSystems.Interpreter;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Diagnostics;
using JsonQueryRunTimeNS;

namespace JSonQueryRunTime_UnitTests
{
    [TestClass]
    public class Performance_UnitTests
    {
        public IEnumerable<string> GetJsonLines1()
        {
            var l = new List<string>();
            for(var i=0; i< 50000; i++) {
                l.Add(ExecuteOneJsonString_UnitTests.json0);
                l.Add(ExecuteOneJsonString_UnitTests.json1);
            }
            return l;
        }

        [TestMethod]
        public void Perf_Execute_String_Equal()
        {
            var lines = GetJsonLines1().ToList();

            var sw = Stopwatch.StartNew();
                var resultLines = new JsonQueryRuntime(@"name = ""ok"" ").Execute(lines).ToList();
            sw.Stop();

            var expectedCount = lines.Count/2;
            Assert.AreEqual(expectedCount, resultLines.Count);
            Assert.IsTrue(sw.Elapsed < new System.TimeSpan(0,0,4, 200));            
        }    
    }
}
