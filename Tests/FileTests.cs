using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Xml;
using NUnit.Framework;
using RefreshCache.Packager;

namespace RefreshCache.Packager.Tests
{
    [TestFixture]
    class FileTests
    {
        [Test]
        public void BaseConstructorTest()
        {
            File f;


            //
            // No setup needed for test.
            //

            //
            // Run the test.
            //
            f = new File();

            //
            // Verify the test.
            //
            Assert.AreEqual("", f.Path);
            Assert.AreEqual("", f.Source);
        }


        [Test]
        public void InternalConstructorTest()
        {
            Package package;
            String path, source;
            File f;


            //
            // Setup the test.
            //
            path = "UserControls/Custom/Root";
            source = Path.GetTempFileName();
            package = new Package();

            //
            // Run the test.
            //
            f = new File(path, source, package);

            //
            // Verify the test.
            //
            Assert.AreEqual(path, f.Path);
            Assert.AreEqual(source, f.Source);
            Assert.AreSame(package, f.Package);

            new FileInfo(source).Delete();
        }


        [Test]
        public void XmlConstructorLoadTemplateTest()
        {
            XmlDocument xdoc;
            XmlAttribute xattr;
            XmlNode fileNode;
            String path, source;
            File f;


            //
            // Setup the test.
            //
            path = "UserControls/Custom/Root";
            source = Path.GetTempFileName();

            xdoc = new XmlDocument();
            fileNode = xdoc.CreateElement("File");

            xattr = xdoc.CreateAttribute("path");
            xattr.Value = path;
            fileNode.Attributes.Append(xattr);

            xattr = xdoc.CreateAttribute("_source");
            xattr.Value = source;
            fileNode.Attributes.Append(xattr);

            //
            // Run the test.
            //
            f = new File(fileNode);

            //
            // Verify the test.
            //
            Assert.AreEqual(path, f.Path);
            Assert.AreEqual(source, f.Source);
            Assert.AreEqual(null, f.Contents);

            new FileInfo(source).Delete();
        }


        [Test]
        public void XmlConstructorLoadExportTest()
        {
            XmlDocument xdoc;
            XmlAttribute xattr;
            XmlNode fileNode;
            String path, source;
            byte[] content;
            File f;


            //
            // Setup the test.
            //
            path = "UserControls/Custom/Root";
            source = Path.GetTempFileName();
            content = new System.Text.UTF8Encoding().GetBytes("Test Content");

            xdoc = new XmlDocument();
            fileNode = xdoc.CreateElement("File");
            fileNode.InnerText = Convert.ToBase64String(content);

            xattr = xdoc.CreateAttribute("path");
            xattr.Value = path;
            fileNode.Attributes.Append(xattr);

            //
            // Run the test.
            //
            f = new File(fileNode);

            //
            // Verify the test.
            //
            Assert.AreEqual(path, f.Path);
            Assert.AreEqual("", f.Source);
            Assert.AreEqual(content, f.Contents);

            new FileInfo(source).Delete();
        }


        [Test]
        public void SaveTest()
        {
            XmlDocument xdoc;
            XmlNode node;
            Package package;
            String path, source;
            byte[] content;
            File f;


            //
            // Setup the test.
            //
            path = "UserControls/Custom/Test";
            content = new System.Text.UTF8Encoding().GetBytes("Test Content.");
            source = Path.GetTempFileName();
            using (FileStream stream = new FileInfo(source).OpenWrite())
            {
                stream.Write(content, 0, content.Length);
            }
            package = new Package();
            f = new File(path, source, package);
            xdoc = new XmlDocument();

            //
            // Run the test.
            //
            node = f.Save(xdoc, false);

            //
            // Verify the test.
            //
            Assert.AreEqual(0, node.ChildNodes.Count);
            Assert.AreEqual(2, node.Attributes.Count);
            Assert.AreEqual(path, node.Attributes["path"].Value);
            Assert.AreEqual(source, node.Attributes["_source"].Value);
            Assert.AreEqual("", node.InnerText);
        }


        [Test]
        public void ExportTest()
        {
            XmlDocument xdoc;
            XmlNode node;
            Package package;
            String path, source;
            byte[] content;
            File f;


            //
            // Setup the test.
            //
            path = "UserControls/Custom/Test";
            content = new System.Text.UTF8Encoding().GetBytes("Test Content.");
            source = Path.GetTempFileName();
            using (FileStream stream = new FileInfo(source).OpenWrite())
            {
                stream.Write(content, 0, content.Length);
            }
            package = new Package();
            f = new File(path, source, package);
            xdoc = new XmlDocument();

            //
            // Run the test.
            //
            node = f.Save(xdoc, true);

            //
            // Verify the test.
            //
            Assert.AreEqual(1, node.ChildNodes.Count); /* InnerText counts as a child node. */
            Assert.AreEqual(1, node.Attributes.Count);
            Assert.AreEqual(path, node.Attributes["path"].Value);
            Assert.AreEqual(content, Convert.FromBase64String(node.InnerText));
        }


        [Test]
        public void ExportNullSourceTest()
        {
            Package package;
            String path;
            File f;


            //
            // Setup the test.
            //
            path = "UserControls/Custom/Test";
            package = new Package();
            f = new File(path, null, package);
            package.Files.Add(f);

            //
            // Run the test.
            //
            package.Build("C:\\");

            //
            // Verify the test.
            //
            Assert.AreEqual(1, package.BuildMessages.Count);
            Assert.AreEqual(BuildMessageType.Error, package.BuildMessages[0].Type);
            Assert.IsTrue(package.BuildMessages[0].ToString().Contains("no local file"));
        }


        [Test]
        public void ExportEmptySourceTest()
        {
            Package package;
            String path;
            File f;


            //
            // Setup the test.
            //
            path = "UserControls/Custom/Test";
            package = new Package();
            f = new File(path, "", package);
            package.Files.Add(f);

            //
            // Run the test.
            //
            package.Build("C:\\");

            //
            // Verify the test.
            //
            Assert.AreEqual(1, package.BuildMessages.Count);
            Assert.AreEqual(BuildMessageType.Error, package.BuildMessages[0].Type);
            Assert.IsTrue(package.BuildMessages[0].ToString().Contains("no local file"));
        }


        [Test]
        public void ExportNonExistentSourceTest()
        {
            Package package;
            String path;
            File f;


            //
            // Setup the test.
            //
            path = "UserControls/Custom/Test";
            package = new Package();
            f = new File(path, "C:\\doesnotexist.txt", package);
            package.Files.Add(f);

            //
            // Run the test.
            //
            package.Build("C:\\");

            //
            // Verify the test.
            //
            Assert.AreEqual(1, package.BuildMessages.Count);
            Assert.AreEqual(BuildMessageType.Error, package.BuildMessages[0].Type);
            Assert.IsTrue(package.BuildMessages[0].ToString().Contains("does not exist"));
        }


        [Test]
        public void ExistsTest()
        {
            Package package;
            String source;
            File f;


            //
            // Setup the test.
            //
            source = Path.GetTempFileName();
            package = new Package();

            //
            // Run the test.
            //
            f = new File("", source, package);

            //
            // Verify the test.
            //
            Assert.IsTrue(f.Exists);

            new FileInfo(source).Delete();
        }


        [Test]
        public void DoesNotExistTest()
        {
            Package package;
            File f;


            //
            // Setup the test.
            //
            package = new Package();

            //
            // Run the test.
            //
            f = new File("", "C:\\doesnotexist.txt", package);

            //
            // Verify the test.
            //
            Assert.IsTrue(f.Exists == false);
        }

        
        [Test]
        public void InvalidSourceExistTest()
        {
            Package package;
            File f;


            //
            // Setup the test.
            //
            package = new Package();

            //
            // Run the test.
            //
            f = new File("", "", package);

            //
            // Verify the test.
            //
            Assert.IsTrue(f.Exists == false);
        }
    }
}
