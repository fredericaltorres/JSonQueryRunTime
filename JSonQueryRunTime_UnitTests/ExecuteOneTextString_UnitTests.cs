using HiSystems.Interpreter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JsonQueryRunTimeNS;

namespace JSonQueryRunTime_UnitTests
{
    [TestClass]
    public class ExecuteoneTextString_UnitTests
    {
        const string sourceTextABCD = "ABCD";

        [TestMethod]
        public void DoubleQuoteInText()
        {
            const string source = @"A""B";
            Assert.IsTrue(new JsonQueryRuntime(@" Contains(text, 'A') ").Execute(source, JsonQueryRuntimeTextType.TEXT));
            Assert.IsTrue(new JsonQueryRuntime(@" Contains(text, 'A""') ").Execute(source, JsonQueryRuntimeTextType.TEXT));
            Assert.IsTrue(new JsonQueryRuntime(@" Contains(text, 'A""B') ").Execute(source, JsonQueryRuntimeTextType.TEXT));
        }

        [TestMethod]
        public void Contains()
        {
            Assert.IsTrue(new JsonQueryRuntime(@" Contains(text, 'A') ").Execute(sourceTextABCD, JsonQueryRuntimeTextType.TEXT ));
        }
        [TestMethod]
        public void Regex()
        {
            Assert.IsTrue(new JsonQueryRuntime(@" Regex(text, 'A..D') ").Execute(sourceTextABCD, JsonQueryRuntimeTextType.TEXT));
            Assert.IsTrue(new JsonQueryRuntime(@" Regex(text, 'A..D') AND Regex(text, 'A..D') AND Regex(text, 'A..D') ").Execute(sourceTextABCD, JsonQueryRuntimeTextType.TEXT));
        }
        [TestMethod]
        public void WildCard()
        {
            Assert.IsTrue(new JsonQueryRuntime(@" Wildcard(text, 'A??D') ").Execute(sourceTextABCD, JsonQueryRuntimeTextType.TEXT));
        }
        [TestMethod]
        public void InFunc()
        {
            Assert.IsTrue(new JsonQueryRuntime(@" In(text, Array('ABCD', 'XYZ')) ").Execute(sourceTextABCD, JsonQueryRuntimeTextType.TEXT));
        }
    }
}

