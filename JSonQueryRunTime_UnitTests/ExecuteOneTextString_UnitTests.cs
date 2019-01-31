using HiSystems.Interpreter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JsonQueryRunTimeNS;

namespace JSonQueryRunTime_UnitTests
{
    [TestClass]
    public class ExecuteoneTextString_UnitTests
    {
        [TestMethod]
        public void Contains()
        {
            Assert.IsTrue(new JsonQueryRuntime(@" Contains(text, 'A') ").Execute("[A]", JsonQueryRuntimeTextType.TEXT ));
        }
    }
}


/*
var d = System.DateTime.UtcNow;
var s = Newtonsoft.Json.JsonConvert.SerializeObject(d, Newtonsoft.Json.Formatting.Indented);
*/