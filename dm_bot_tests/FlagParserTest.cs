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

        [TestMethod]
        public void FlagParser_FindsMultipleFlags_InString ()
        {
            // arrange
            var testFlags = new List<string> () { "flag", "flag2", "flag3" };
            var flagParser = new FlagParser (testFlags);

            // act
            var results = flagParser.ParseMessage ("-flag is here -flag2 is all the way over here -flag3 is just some content here");

            // assert
            Assert.AreEqual (results.Count, 3);
            Assert.IsTrue (results.ContainsKey ("flag"), "should have flag in dictionary");
            Assert.IsTrue (results.ContainsKey ("flag2"), "should have flag in dictionary");
            Assert.IsTrue (results.ContainsKey ("flag3"), "should have flag in dictionary");
            Assert.IsTrue (results["flag"] == "is here", "has correct data");
            Assert.IsTrue (results["flag2"] == "is all the way over here", "has correct data");
            Assert.IsTrue (results["flag3"] == "is just some content here", "has correct data");
        }
    }
}