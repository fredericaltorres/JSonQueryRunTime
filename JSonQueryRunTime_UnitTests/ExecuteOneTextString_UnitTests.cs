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
            Assert.IsTrue(new JsonQueryRuntime(@" Contains(text, 'A') ").Execute(source, TextType.TEXT));
            Assert.IsTrue(new JsonQueryRuntime(@" Contains(text, 'A""') ").Execute(source, TextType.TEXT));
            Assert.IsTrue(new JsonQueryRuntime(@" Contains(text, 'A""B') ").Execute(source, TextType.TEXT));
        }

        [TestMethod]
        public void Contains()
        {
            Assert.IsTrue(new JsonQueryRuntime(@" Contains(text, 'A') ").Execute(sourceTextABCD, TextType.TEXT ));
        }

        [TestMethod]
        public void Regex()
        {
            Assert.IsTrue(new JsonQueryRuntime(@" Regex(text, 'A..D') ").Execute(sourceTextABCD, TextType.TEXT));
            Assert.IsTrue(new JsonQueryRuntime(@" Regex(text, 'A..D') AND Regex(text, 'A..D') AND Regex(text, 'A..D') ").Execute(sourceTextABCD, TextType.TEXT));
            Assert.IsFalse(new JsonQueryRuntime(@" Regex(text, 'A..E') ").Execute(sourceTextABCD, TextType.TEXT));
        }

        [TestMethod]
        public void WildCard()
        {
            Assert.IsTrue(new JsonQueryRuntime(@" Wildcard(text, 'A??D') ").Execute(sourceTextABCD, TextType.TEXT));
            Assert.IsFalse(new JsonQueryRuntime(@" Wildcard(text, 'A??Z') ").Execute(sourceTextABCD, TextType.TEXT));
        }

        [TestMethod]
        public void InFunc()
        {
            Assert.IsTrue(new JsonQueryRuntime(@" In(text, Array('ABCD', 'XYZ')) ").Execute(sourceTextABCD, TextType.TEXT));
            Assert.IsFalse(new JsonQueryRuntime(@" In(text, Array('OOO', 'XYZ')) ").Execute(sourceTextABCD, TextType.TEXT));
        }

        [TestMethod]
        public void Range()
        {
            var source = "B";
            Assert.IsTrue(new JsonQueryRuntime(@" Range(text, 'A', 'Z') ").Execute(source, TextType.TEXT));
            Assert.IsFalse(new JsonQueryRuntime(@" Range(text, 'BB', 'Z') ").Execute(source, TextType.TEXT));
        }
    }
}

