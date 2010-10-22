using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using NUnit.Framework;
using RefreshCache.Packager;


namespace RefreshCache.Packager.Tests
{
    [TestFixture]
    class ModuleTests : TestsHelper
    {
        [Test]
        public void BaseConstructorTest()
        {
            Module m;


            //
            // No setup needed.
            //

            //
            // Run the test.
            //
            m = new Module();

            //
            // Verify the test.
            //
            Assert.AreEqual(0, m._ModuleID);
        }


        [Test]
        public void XmlConstructorLoadTemplateTest()
        {
            XmlDocument xdoc;
            XmlNode moduleNode;
            Boolean moduleIsSystem, moduleAllowsChildModules;
            String moduleName, moduleURL, moduleImagePath, moduleDescription;
            Module m;
            int tempModuleID;


            //
            // No setup needed.
            //
            xdoc = new XmlDocument();
            moduleNode = xdoc.CreateElement("Module");
            tempModuleID = -3;
            AddXmlAttribute(moduleNode, "temp_module_id", tempModuleID.ToString());
            moduleIsSystem = true;
            AddXmlAttribute(moduleNode, "_system", "1");
            moduleAllowsChildModules = false;
            AddXmlAttribute(moduleNode, "allows_child_modules", "0");
            moduleName = "New Module";
            AddXmlAttribute(moduleNode, "module_name", moduleName);
            moduleURL = "UserControls/Custom/Test";
            AddXmlAttribute(moduleNode, "module_url", moduleURL);
            moduleImagePath = "UserControls/Custom/Image.jpg";
            AddXmlAttribute(moduleNode, "image_path", moduleImagePath);
            moduleDescription = "This is a module.";
            AddXmlAttribute(moduleNode, "module_desc", moduleDescription);

            //
            // Run the test.
            //
            m = new Module(moduleNode);

            //
            // Verify the test.
            //
            Assert.AreEqual(tempModuleID, m.ModuleID);
            Assert.AreEqual(moduleIsSystem, m.IsSystem);
            Assert.AreEqual(moduleAllowsChildModules, m.AllowsChildModules);
            Assert.AreEqual(moduleName, m.Name);
            Assert.AreEqual(moduleURL, m.URL);
            Assert.AreEqual(moduleImagePath, m.ImagePath);
            Assert.AreEqual(moduleDescription, m.Description);
            Assert.IsTrue(String.IsNullOrEmpty(m.Source));
            Assert.IsTrue(String.IsNullOrEmpty(m.SourceImage));
        }


        [Test]
        public void XmlConstructorLoadExportTest()
        {
            XmlDocument xdoc;
            XmlNode moduleNode, fileNode;
            Boolean moduleIsSystem, moduleAllowsChildModules;
            String moduleName, moduleURL, moduleImagePath, moduleDescription, moduleSource, moduleSourceImage;
            Module m;
            int tempModuleID;


            //
            // No setup needed.
            //
            xdoc = new XmlDocument();
            moduleNode = xdoc.CreateElement("Module");
            tempModuleID = -3;
            AddXmlAttribute(moduleNode, "temp_module_id", tempModuleID.ToString());
            moduleIsSystem = true;
            AddXmlAttribute(moduleNode, "_system", "1");
            moduleAllowsChildModules = false;
            AddXmlAttribute(moduleNode, "allows_child_modules", "0");
            moduleName = "New Module";
            AddXmlAttribute(moduleNode, "module_name", moduleName);
            moduleURL = "UserControls/Custom/Test";
            AddXmlAttribute(moduleNode, "module_url", moduleURL);
            moduleImagePath = "UserControls/Custom/Image.jpg";
            AddXmlAttribute(moduleNode, "image_path", moduleImagePath);
            moduleDescription = "This is a module.";
            AddXmlAttribute(moduleNode, "module_desc", moduleDescription);
            moduleSource = "C:\\test.cs";
            AddXmlAttribute(moduleNode, "_source", moduleSource);
            moduleSourceImage = "C:\\test.jpg";
            AddXmlAttribute(moduleNode, "_source_image", moduleSourceImage);
            fileNode = xdoc.CreateElement("File");
            AddXmlAttribute(fileNode, "path", "UserControls/Custom/Test");
            moduleNode.AppendChild(fileNode);

            //
            // Run the test.
            //
            m = new Module(moduleNode);

            //
            // Verify the test.
            //
            Assert.AreEqual(tempModuleID, m.ModuleID);
            Assert.AreEqual(moduleIsSystem, m.IsSystem);
            Assert.AreEqual(moduleAllowsChildModules, m.AllowsChildModules);
            Assert.AreEqual(moduleName, m.Name);
            Assert.AreEqual(moduleURL, m.URL);
            Assert.AreEqual(moduleImagePath, m.ImagePath);
            Assert.AreEqual(moduleDescription, m.Description);
            Assert.AreEqual(moduleSource, m.Source);
            Assert.AreEqual(moduleSourceImage, m.SourceImage);
            Assert.AreEqual(1, m.Files.Count);
        }

        
        [Test]
        public void GenerateModuleIDTest()
        {
            Package package;
            Module m;


            //
            // No setup needed.
            //
            package = new Package();
            m = new Module();
            package.Modules.Add(m);

            //
            // Run the test.
            // Test is run by the verify.
            //

            //
            // Verify the test.
            //
            Assert.AreNotEqual(0, m.ModuleID);
        }

        
        [Test]
        public void UpdatePackageTest()
        {
            Package package;
            Module m;
            File f;


            //
            // No setup needed.
            //
            package = new Package();
            m = new Module();
            f = new File();
            m.Files.Add(f);

            //
            // Run the test.
            //
            package.Modules.Add(m);

            //
            // Verify the test.
            //
            Assert.AreSame(package, f.Package);
        }


        [Test]
        public void SaveTest()
        {
            throw new NotImplementedException();
        }
    }
}
