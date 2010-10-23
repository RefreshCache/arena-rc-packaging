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
    class PackageChangelogTests : TestsHelper
    {
        [Test]
        public void EmptyConstructorTest()
        {
            PackageChangelog pc;


            //
            // No preparation needed.
            //

            //
            // Run the test.
            //
            pc = new PackageChangelog();

            //
            // Verify the test.
            //
            Assert.AreEqual("1.0.0", pc.Version.ToString());
            Assert.AreEqual(String.Empty, pc.Description);
        }


        [Test]
        public void XmlConstructorTest()
        {
            PackageChangelog pc;
            XmlDocument xdoc;
            XmlNode node;
            String Version, Description;


            //
            // Prepare the test.
            //
            xdoc = new XmlDocument();
            Version = "1.2.3";
            Description = "This is a new version.";
            node = xdoc.CreateElement("Changelog");
            AddXmlAttribute(node, "Version", Version);
            node.InnerText = Description;

            //
            // Run the test.
            //
            pc = new PackageChangelog(node);

            //
            // Verify the test.
            //
            Assert.AreEqual(Version, pc.Version.ToString());
            Assert.AreEqual(Description, pc.Description);
        }


        [Test]
        public void SaveTest()
        {
            PackageChangelog pc;
            XmlDocument xdoc;
            XmlNode node;
            String Version, Description;


            //
            // Prepare the test.
            //
            xdoc = new XmlDocument();
            Version = "3.2.1";
            Description = "This is a description.";
            pc = new PackageChangelog();
            pc.Version = new PackageVersion(Version);
            pc.Description = Description;
            
            //
            // Run the test.
            //
            node = pc.Save(xdoc, false);

            //
            // Verify the test.
            //
            Assert.AreEqual(1, node.Attributes.Count);
            Assert.AreEqual(1, node.ChildNodes.Count);
            Assert.AreEqual(Version, node.Attributes["Version"].Value);
            Assert.AreEqual(Description, node.InnerText);
        }
    }
}
