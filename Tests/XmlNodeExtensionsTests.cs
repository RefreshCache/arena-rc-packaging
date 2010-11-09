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
    public class XmlNodeExtensionsTests : TestsHelper
    {
        [Test]
        public void CopyAndRenameTest()
        {
            XmlDocument xdoc;
            XmlNode parent, child, newChild;


            //
            // Prepare the test.
            //
            xdoc = new XmlDocument();
            parent = xdoc.CreateElement("ParentNode");
            child = xdoc.CreateElement("ChildNode");
            parent.AppendChild(child);
            AddXmlAttribute(child, "Attribute1", "Value1");
            AddXmlAttribute(child, "Attribute2", "Value2");
            child.AppendChild(xdoc.CreateElement("Node1"));
            child.AppendChild(xdoc.CreateElement("Node2"));

            //
            // Run the test.
            //
            newChild = child.CopyAndRename("NewNode");

            //
            // Verify the test.
            //
            Assert.AreEqual(1, parent.ChildNodes.Count);
            Assert.AreNotSame(newChild, child);
            Assert.AreSame(newChild, parent.FirstChild);
            Assert.AreEqual(2, newChild.Attributes.Count);
            Assert.AreEqual(2, newChild.ChildNodes.Count);
        }
    }
}
