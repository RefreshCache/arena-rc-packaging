﻿using System;
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
            XmlDocument xdoc;
            XmlNode moduleNode;
            Package package;
            String TempModuleID, ModuleURL, ModuleName, ModuleDescription;
            String ModuleAllowsChildModules, ModuleImagePath, ModuleSource;
            String ModuleSourceImage, ModuleIsSystem;
            Module m;


            //
            // Setup the test.
            //
            xdoc = new XmlDocument();
            package = new Package();
            m = new Module();
            TempModuleID = "-3";
            ModuleURL = "UserControls/Custom/Test.ascx";
            ModuleName = "New Module";
            ModuleDescription = "This is a description.";
            ModuleAllowsChildModules = "1";
            ModuleImagePath = "UserControls/Custom/Test.jpg";
            ModuleSource = "C:\\Test.ascx";
            ModuleSourceImage = "C:\\Test.jpg";
            ModuleIsSystem = "0";
            m._ModuleID = Convert.ToInt32(TempModuleID);
            m.URL = ModuleURL;
            m.Name = ModuleName;
            m.Description = ModuleDescription;
            m.AllowsChildModules = Convert.ToBoolean(Convert.ToInt32(ModuleAllowsChildModules));
            m.ImagePath = ModuleImagePath;
            m.Source = ModuleSource;
            m.SourceImage = ModuleSourceImage;
            m.IsSystem = Convert.ToBoolean(Convert.ToInt32(ModuleIsSystem));

            //
            // Run the test.
            //
            moduleNode = m.Save(xdoc, false);

            //
            // Verify the test.
            //
            Assert.AreEqual(9, moduleNode.Attributes.Count);
            Assert.AreEqual(0, moduleNode.ChildNodes.Count);
            Assert.AreEqual(TempModuleID, moduleNode.Attributes["temp_module_id"].Value);
            Assert.AreEqual(ModuleURL, moduleNode.Attributes["module_url"].Value);
            Assert.AreEqual(ModuleName, moduleNode.Attributes["module_name"].Value);
            Assert.AreEqual(ModuleDescription, moduleNode.Attributes["module_desc"].Value);
            Assert.AreEqual(ModuleAllowsChildModules, moduleNode.Attributes["allows_child_modules"].Value);
            Assert.AreEqual(ModuleImagePath, moduleNode.Attributes["image_path"].Value);
            Assert.AreEqual(ModuleSource, moduleNode.Attributes["_source"].Value);
            Assert.AreEqual(ModuleSourceImage, moduleNode.Attributes["_source_image"].Value);
            Assert.AreEqual(ModuleIsSystem, moduleNode.Attributes["_system"].Value);
        }


        [Test]
        public void ExportTest()
        {
            XmlDocument xdoc;
            XmlNode moduleNode;
            String TempModuleID, ModuleURL, ModuleName, ModuleDescription;
            String ModuleAllowsChildModules, ModuleImagePath, ModuleSource;
            String ModuleSourceImage, ModuleIsSystem;
            byte[] ModuleSourceContent, ModuleSourceCodeContent, ModuleSourceImageContent;
            Module m;


            //
            // Setup the test.
            //
            xdoc = new XmlDocument();
            m = new Module();
            TempModuleID = "-3";
            ModuleURL = "UserControls/Custom/Test.ascx";
            ModuleName = "New Module";
            ModuleDescription = "This is a description.";
            ModuleAllowsChildModules = "1";
            ModuleImagePath = "UserControls/Custom/Test.jpg";
            ModuleSource = Path.GetTempFileName();
            ModuleSourceImage = Path.GetTempFileName();
            ModuleIsSystem = "0";
            new FileInfo(ModuleSource).Delete();
            new FileInfo(ModuleSourceImage).Delete();
            ModuleSource += ".ascx";
            ModuleSourceImage += ".jpg";
            m._ModuleID = Convert.ToInt32(TempModuleID);
            m.URL = ModuleURL;
            m.Name = ModuleName;
            m.Description = ModuleDescription;
            m.AllowsChildModules = Convert.ToBoolean(Convert.ToInt32(ModuleAllowsChildModules));
            m.ImagePath = ModuleImagePath;
            m.Source = ModuleSource;
            m.SourceImage = ModuleSourceImage;
            m.IsSystem = Convert.ToBoolean(Convert.ToInt32(ModuleIsSystem));

            ModuleSourceContent = new System.Text.UTF8Encoding().GetBytes("This is some content for the source ascx file.");
            using (FileStream stream = new FileInfo(ModuleSource).OpenWrite())
            {
                stream.Write(ModuleSourceContent, 0, ModuleSourceContent.Length);
            }
            ModuleSourceCodeContent = new System.Text.UTF8Encoding().GetBytes("This is some content for the source code file.");
            using (FileStream stream = new FileInfo(ModuleSource + ".cs").OpenWrite())
            {
                stream.Write(ModuleSourceCodeContent, 0, ModuleSourceCodeContent.Length);
            }
            ModuleSourceImageContent = new System.Text.UTF8Encoding().GetBytes("This is some image data.");
            using (FileStream stream = new FileInfo(ModuleSourceImage).OpenWrite())
            {
                stream.Write(ModuleSourceImageContent, 0, ModuleSourceImageContent.Length);
            }

            //
            // Run the test.
            //
            moduleNode = m.Save(xdoc, true);

            //
            // Verify the test.
            //
            Assert.AreEqual(6, moduleNode.Attributes.Count);
            Assert.AreEqual(3, moduleNode.ChildNodes.Count);
            Assert.AreEqual(TempModuleID, moduleNode.Attributes["temp_module_id"].Value);
            Assert.AreEqual(ModuleURL, moduleNode.Attributes["module_url"].Value);
            Assert.AreEqual(ModuleName, moduleNode.Attributes["module_name"].Value);
            Assert.AreEqual(ModuleDescription, moduleNode.Attributes["module_desc"].Value);
            Assert.AreEqual(ModuleAllowsChildModules, moduleNode.Attributes["allows_child_modules"].Value);
            Assert.AreEqual(ModuleImagePath, moduleNode.Attributes["image_path"].Value);

            foreach (XmlNode fileNode in moduleNode.SelectNodes("//File"))
            {
                if (fileNode.Attributes["path"].Value == ModuleURL)
                {
                    Assert.AreEqual(ModuleSourceContent, Convert.FromBase64String(fileNode.InnerText));
                }
                else if (fileNode.Attributes["path"].Value == ModuleURL + ".cs")
                {
                    Assert.AreEqual(ModuleSourceCodeContent, Convert.FromBase64String(fileNode.InnerText));
                }
                else if (fileNode.Attributes["path"].Value == ModuleImagePath)
                {
                    Assert.AreEqual(ModuleSourceImageContent, Convert.FromBase64String(fileNode.InnerText));
                }
                else
                    Assert.Fail("Unknown file in Module export.");
            }

            //
            // Cleanup the test.
            //
            new FileInfo(ModuleSource).Delete();
            new FileInfo(ModuleSource + ".cs").Delete();
            new FileInfo(ModuleSourceImage).Delete();
        }


        [Test]
        public void ExportNoFileTest()
        {
            Package package;
            Module m;


            //
            // Setup the test.
            //
            package = new Package();
            m = new Module();
            package.Modules.Add(m);
            m._ModuleID = -3;
            m.URL = "UserControls/Custom/Test.ascx";
            m.Name = "New Module";
            m.Description = "This is a description.";
            m.AllowsChildModules = false;
            m.IsSystem = false;

            //
            // Run the test.
            //
            package.Build(Path.GetTempPath());

            //
            // Verify the test.
            //
            Assert.AreEqual(1, package.BuildMessages.Count);
            Assert.AreEqual(BuildMessageType.Error, package.BuildMessages[0].Type);
            Assert.IsTrue(package.BuildMessages[0].ToString().Contains("source file path"));
        }
    }
}
