using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using NUnit.Framework;
using RefreshCache.Packager;


namespace RefreshCache.Packager.Tests
{
    [TestFixture]
    class ModuleInstanceSettingTests : TestsHelper
    {
        [Test]
        public void EmptyConstructorTest()
        {
            ModuleInstanceSetting mis;


            //
            // No test preparation needed.
            //

            //
            // Run the test.
            //
            mis = new ModuleInstanceSetting();

            //
            // Verify the test.
            //
            Assert.AreEqual("", mis.Name);
            Assert.AreEqual("", mis.Value);
            Assert.AreEqual("", mis.Guid);
            Assert.AreEqual(ModuleInstanceSettingType.None, mis.Type);
        }


        [Test]
        public void ConstructorTest()
        {
            ModuleInstanceSetting mis;
            String Name;


            //
            // Prepare the test.
            //
            Name = "Setting1";

            //
            // Run the test.
            //
            mis = new ModuleInstanceSetting(Name);

            //
            // Verify the test.
            //
            Assert.AreEqual(Name, mis.Name);
            Assert.AreEqual("", mis.Value);
            Assert.AreEqual("", mis.Guid);
            Assert.AreEqual(ModuleInstanceSettingType.None, mis.Type);
        }


        [Test]
        public void LoadXmlConstructorTest()
        {
            ModuleInstanceSettingType Type;
            ModuleInstanceSetting mis;
            XmlDocument xdoc;
            XmlNode node;
            String Name, Value, Guid;


            //
            // Prepare the test.
            //
            Name = "Setting1";
            Value = "My Value";
            Guid = new Guid().ToString();
            Type = ModuleInstanceSettingType.Text;
            xdoc = new XmlDocument();
            node = xdoc.CreateElement("Setting");
            AddXmlAttribute(node, "name", Name);
            AddXmlAttribute(node, "value", Value);
            AddXmlAttribute(node, "guid", Guid);
            AddXmlAttribute(node, "type_id", ((int)Type).ToString());

            //
            // Run the test.
            //
            mis = new ModuleInstanceSetting(node);

            //
            // Verify the test.
            //
            Assert.AreEqual(Name, mis.Name);
            Assert.AreEqual(Value, mis.Value);
            Assert.AreEqual(Guid, mis.Guid);
            Assert.AreEqual(Type, mis.Type);
        }

        
        [Test]
        public void LoadXmlConstructorEmptyGuidTest()
        {
            ModuleInstanceSetting mis;
            XmlDocument xdoc;
            XmlNode node;


            //
            // Prepare the test.
            //
            xdoc = new XmlDocument();
            node = xdoc.CreateElement("Setting");
            AddXmlAttribute(node, "name", "Setting1");
            AddXmlAttribute(node, "value", "My Value");
            AddXmlAttribute(node, "type_id", ((int)ModuleInstanceSettingType.Text).ToString());

            //
            // Run the test.
            //
            mis = new ModuleInstanceSetting(node);

            //
            // Verify the test.
            //
            Assert.AreEqual("", mis.Guid);
        }


        [Test]
        public void SettingStringTest()
        {
            ModuleInstanceSetting mis;
            String setting;


            //
            // Prepare the test.
            //
            mis = new ModuleInstanceSetting("Setting1");
            mis.Type = ModuleInstanceSettingType.Text;
            mis.Value = "This;Test";

            //
            // Run the test.
            //
            setting = mis.SettingString();

            //
            // Verify the test.
            //
            Assert.AreEqual("Setting1=This^^Test", setting);
        }

        
        [Test]
        public void SettingStringEmptyNameTest()
        {
            ModuleInstanceSetting mis;
            String setting;


            //
            // Prepare the test.
            //
            mis = new ModuleInstanceSetting();
            mis.Type = ModuleInstanceSettingType.Text;
            mis.Value = "This;Test";

            //
            // Run the test.
            //
            setting = mis.SettingString();

            //
            // Verify the test.
            //
            Assert.AreEqual(String.Empty, setting);
        }

        
        [Test]
        public void SettingStringEmptyValueTest()
        {
            ModuleInstanceSetting mis;
            String setting;


            //
            // Prepare the test.
            //
            mis = new ModuleInstanceSetting("Setting1");
            mis.Type = ModuleInstanceSettingType.Text;

            //
            // Run the test.
            //
            setting = mis.SettingString();

            //
            // Verify the test.
            //
            Assert.AreEqual(String.Empty, setting);
        }


        [Test]
        public void SaveTest()
        {
            ModuleInstanceSettingType Type;
            ModuleInstanceSetting mis;
            XmlDocument xdoc;
            XmlNode node;
            String Name, Value, Guid;


            //
            // Prepare the test.
            //
            xdoc = new XmlDocument();
            Name = "Setting1";
            Value = "2843";
            Guid = new Guid().ToString();
            Type = ModuleInstanceSettingType.Page;
            mis = new ModuleInstanceSetting();
            mis.Name = Name;
            mis.Value = Value;
            mis.Guid = Guid;
            mis.Type = Type;

            //
            // Run the test.
            //
            node = mis.Save(xdoc, false);
            
            //
            // Verify the test.
            //
            Assert.AreEqual(0, node.ChildNodes.Count);
            Assert.AreEqual(4, node.Attributes.Count);
            Assert.AreEqual(Name, node.Attributes["name"].Value);
            Assert.AreEqual(Value, node.Attributes["value"].Value);
            Assert.AreEqual(Guid, node.Attributes["guid"].Value);
            Assert.AreEqual((int)Type, Convert.ToInt32(node.Attributes["type_id"].Value));
        }


        [Test]
        public void SaveNoGuidTest()
        {
            ModuleInstanceSetting mis;
            XmlDocument xdoc;
            XmlNode node;


            //
            // Prepare the test.
            //
            xdoc = new XmlDocument();
            mis = new ModuleInstanceSetting();
            mis.Name = "Setting1";
            mis.Value = "2843";
            mis.Type = ModuleInstanceSettingType.Page;

            //
            // Run the test.
            //
            node = mis.Save(xdoc, false);

            //
            // Verify the test.
            //
            Assert.AreEqual(0, node.ChildNodes.Count);
            Assert.AreEqual(3, node.Attributes.Count);
            Assert.AreEqual(null, node.Attributes["guid"]);
        }
    }
}
