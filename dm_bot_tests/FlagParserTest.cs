using System.Collections.Generic;
using dm_bot.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace dm_bot_tests
{
    [TestClass]
    public class FlagParserTests
    {
        [TestMethod]
        public void FlagParser_FindsAllFlags_InString ()
        {
            // arrange
            var testFlags = new List<string> () { "flag" };
            var flagParser = new FlagParser (testFlags);

            // act
            var results = flagParser.ParseMessage ("-flag is here");

            // assert
            Assert.AreEqual (results.Count, 1);
            Assert.IsTrue (results.ContainsKey ("flag"));
        }
    }
}