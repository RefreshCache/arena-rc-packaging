using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using RefreshCache.Packager;


namespace RefreshCache.Packager.Tests
{
    [TestFixture]
    public class PageSettingTests
    {
        [Test]
        public void BaseConstructorTest()
        {
            PageSetting setting;


            //
            // No preparation needed.
            //

            //
            // Run the test.
            //
            setting = new PageSetting();

            //
            // Verify the test.
            //
            Assert.AreEqual("", setting.Name);
            Assert.AreEqual("", setting.DisplayName);
            Assert.AreEqual("", setting.Value);
        }


        [Test]
        public void NameConstructorTest()
        {
            PageSetting setting;
            String name;


            //
            // Prepare the test.
            //
            name = "MySetting";

            //
            // Run the test.
            //
            setting = new PageSetting(name);

            //
            // Verify the test.
            //
            Assert.AreEqual(name, setting.Name);
            Assert.AreEqual(name, setting.DisplayName);
            Assert.AreEqual("", setting.Value);
        }


        [Test]
        public void NameValueConstructorTest()
        {
            PageSetting setting;
            String name, value;


            //
            // Prepare the test.
            //
            name = "MySetting";
            value = "MyValue";

            //
            // Run the test.
            //
            setting = new PageSetting(name, value);

            //
            // Verify the test.
            //
            Assert.AreEqual(name, setting.Name);
            Assert.AreEqual(name, setting.DisplayName);
            Assert.AreEqual(value, setting.Value);
        }


        [Test]
        public void NameDisplayNameValueConstructorTest()
        {
            PageSetting setting;
            String name, displayname, value;


            //
            // Prepare the test.
            //
            name = "MySetting";
            displayname = "My Setting";
            value = "MyValue";

            //
            // Run the test.
            //
            setting = new PageSetting(name, displayname, value);

            //
            // Verify the test.
            //
            Assert.AreEqual(name, setting.Name);
            Assert.AreEqual(displayname, setting.DisplayName);
            Assert.AreEqual(value, setting.Value);
        }


        [Test]
        public void EmptyValueSettingStringTest()
        {
            PageSetting setting;
            String result;


            //
            // Prepare the test.
            //
            setting = new PageSetting("MySetting");

            //
            // Run the test.
            //
            result = setting.SettingString();

            //
            // Verify the test.
            //
            Assert.AreEqual("", result);
        }


        [Test]
        public void EmptyNameSettingStringTest()
        {
            PageSetting setting;
            String result;


            //
            // Prepare the test.
            //
            setting = new PageSetting();

            //
            // Run the test.
            //
            result = setting.SettingString();

            //
            // Verify the test.
            //
            Assert.AreEqual("", result);
        }


        [Test]
        public void FilledSettingStringTest()
        {
            PageSetting setting;
            String result;


            //
            // Prepare the test.
            //
            setting = new PageSetting("MySetting", "My;Value");

            //
            // Run the test.
            //
            result = setting.SettingString();

            //
            // Verify the test.
            //
            Assert.AreEqual("MySetting=My^^Value", result);
        }
    }
}
