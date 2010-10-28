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
    class PackageRequirementTests : TestsHelper
    {
        [Test]
        public void BaseConstructorTest()
        {
            PackageRequirement req;


            //
            // No preparation needed.
            //

            //
            // Run the test.
            //
            req = new PackageRequirement();

            //
            // Verify the test.
            //
            Assert.AreEqual(null, req.Name);
            Assert.AreEqual("Require", req.NodeName);
        }


        [Test]
        public void XmlConstructorTest()
        {
            PackageRequirement req;
            XmlDocument xdoc;
            XmlNode node;
            String name;


            //
            // Prepare the test.
            //
            xdoc = new XmlDocument();
            node = xdoc.CreateElement("Require");
            name = "RC.TestPackage";
            AddXmlAttribute(node, "Name", name);

            //
            // Run the test.
            //
            req = new PackageRequirement(node);

            //
            // Verify the test.
            //
            Assert.AreEqual(name, req.Name);
            Assert.AreEqual("Require", req.NodeName);
        }


        [Test]
        public void XmlConstructorNoNameTest()
        {
            PackageRequirement req;
            XmlDocument xdoc;
            XmlNode node;


            //
            // Prepare the test.
            //
            xdoc = new XmlDocument();
            node = xdoc.CreateElement("Require");

            //
            // Run the test.
            //
            req = new PackageRequirement(node);

            //
            // Verify the test.
            //
            Assert.AreEqual("", req.Name);
            Assert.AreEqual("Require", req.NodeName);
        }


        [Test]
        public void SaveTest()
        {
            PackageRequirement req;
            XmlDocument xdoc;
            XmlNode node;
            String name;


            //
            // Prepare the test.
            //
            xdoc = new XmlDocument();
            req = new PackageRequirement();
            req.Name = name = "RC.Package1";
            
            //
            // Run the test.
            //
            node = req.Save(xdoc, false);

            //
            // Verify the test.
            //
            Assert.AreEqual("Require", node.Name);
            Assert.AreEqual(0, node.ChildNodes.Count);
            Assert.AreEqual(name, node.Attributes["Name"].Value);
        }
    }
}
