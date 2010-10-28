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
    class PackageRecommendationTests : TestsHelper
    {
        [Test]
        public void BaseConstructorTest()
        {
            PackageRecommendation rec;


            //
            // No preparation needed.
            //

            //
            // Run the test.
            //
            rec = new PackageRecommendation();

            //
            // Verify the test.
            //
            Assert.AreEqual(null, rec.Description);
            Assert.AreEqual("Recommend", rec.NodeName);
        }


        [Test]
        public void XmlConstructorTest()
        {
            PackageRecommendation rec;
            XmlDocument xdoc;
            XmlNode node;
            String description;


            //
            // Prepare the test.
            //
            xdoc = new XmlDocument();
            node = xdoc.CreateElement("Recommend");
            description = "This is a description of the package.";
            AddXmlAttribute(node, "Description", description);

            //
            // Run the test.
            //
            rec = new PackageRecommendation(node);

            //
            // Verify the test.
            //
            Assert.AreEqual(description, rec.Description);
            Assert.AreEqual("Recommend", rec.NodeName);
        }


        [Test]
        public void XmlConstructorNoDescriptionTest()
        {
            PackageRecommendation rec;
            XmlDocument xdoc;
            XmlNode node;


            //
            // Prepare the test.
            //
            xdoc = new XmlDocument();
            node = xdoc.CreateElement("Recommend");

            //
            // Run the test.
            //
            rec = new PackageRecommendation(node);

            //
            // Verify the test.
            //
            Assert.AreEqual("", rec.Description);
            Assert.AreEqual("Recommend", rec.NodeName);
        }


        [Test]
        public void SaveTest()
        {
            PackageRecommendation rec;
            XmlDocument xdoc;
            XmlNode node;
            String description;


            //
            // Prepare the test.
            //
            xdoc = new XmlDocument();
            rec = new PackageRecommendation();
            rec.Description = description = "A description of the package.";

            //
            // Run the test.
            //
            node = rec.Save(xdoc, false);

            //
            // Verify the test.
            //
            Assert.AreEqual("Recommend", node.Name);
            Assert.AreEqual(0, node.ChildNodes.Count);
            Assert.AreEqual(description, node.Attributes["Description"].Value);
        }
    }
}
