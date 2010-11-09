using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using RefreshCache.Packager;


namespace RefreshCache.Packager.Tests
{
    [TestFixture]
    public class BuildMessageTests : TestsHelper
    {
        [Test]
        public void TypeMessageConstructorTest()
        {
            BuildMessageType type;
            BuildMessage msg;
            String message;


            //
            // Prepare the test.
            //
            type = BuildMessageType.Error;
            message = "This is a message";

            //
            // Run the test.
            //
            msg = new BuildMessage(type, message);

            //
            // Verify the test.
            //
            Assert.AreEqual(type, msg.Type);
            Assert.AreEqual(type.ToString() + ": " + message, msg.ToString());
        }
    }
}
