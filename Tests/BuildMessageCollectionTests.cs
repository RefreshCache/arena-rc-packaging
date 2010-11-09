using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using RefreshCache.Packager;


namespace RefreshCache.Packager.Tests
{
    [TestFixture]
    public class BuildMessageCollectionTests : TestsHelper
    {
        [Test]
        public void BaseConstructorTest()
        {
            BuildMessageCollection bmc;


            //
            // No preparation needed.
            //

            //
            // Run the test.
            //
            bmc = new BuildMessageCollection();

            //
            // Verify the test.
            //
            Assert.AreEqual(0, bmc.Count);
        }


        [Test]
        public void ToStringTest()
        {
            BuildMessageCollection bmc;
            String result;


            //
            // Prepare the test.
            //
            bmc = new BuildMessageCollection();
            bmc.Add(new BuildMessage(BuildMessageType.Error, "Msg1"));
            bmc.Add(new BuildMessage(BuildMessageType.Warning, "Msg2"));

            //
            // Run the test.
            //
            result = bmc.ToString();

            //
            // Verify the test.
            //
            Assert.AreEqual(2, bmc.Count);
            Assert.AreEqual(true, result.Contains("Msg1"));
            Assert.AreEqual(true, result.Contains("Msg2"));
            Assert.AreEqual(true, result.Contains("Warning:"));
            Assert.AreEqual(true, result.Contains("Error:"));
        }
    }
}
