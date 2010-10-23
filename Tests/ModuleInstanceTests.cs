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
    class ModuleInstanceTests : TestsHelper
    {
        [Test]
        public void BaseConstructorTest()
        {
            ModuleInstance m;


            //
            // No setup needed.
            //

            //
            // Run the test.
            //
            m = new ModuleInstance();

            //
            // Verify the test.
            //
            Assert.AreEqual(0, m._ModuleInstanceID);
            Assert.AreEqual(0, m.Settings.Count);
        }


        [Test]
        public void XmlConstructorLoadTemplateTest()
        {
            ModuleInstance mi;
            XmlDocument xdoc;
            XmlNode moduleInstanceNode, settingNode;
            Boolean ShowTitle;
            String ModuleTitle, TemplateFrameName, ModuleDetails;
            Guid ModuleInstanceGuid;
            int TempModuleInstanceID, TempModuleID;


            //
            // Setup the test.
            //
            xdoc = new XmlDocument();
            moduleInstanceNode = xdoc.CreateElement("ModuleInstance");
            TempModuleInstanceID = -3;
            ModuleTitle = "My Module Instance";
            ShowTitle = true;
            TemplateFrameName = "Left";
            ModuleDetails = "This is very detailed.";
            TempModuleID = -5;
            ModuleInstanceGuid = new Guid();
            AddXmlAttribute(moduleInstanceNode, "temp_module_instance_id", TempModuleInstanceID.ToString());
            AddXmlAttribute(moduleInstanceNode, "module_title", ModuleTitle);
            AddXmlAttribute(moduleInstanceNode, "show_title", Convert.ToInt32(ShowTitle).ToString());
            AddXmlAttribute(moduleInstanceNode, "template_frame_name", TemplateFrameName);
            AddXmlAttribute(moduleInstanceNode, "module_details", ModuleDetails);
            AddXmlAttribute(moduleInstanceNode, "temp_module_id", TempModuleID.ToString());
            AddXmlAttribute(moduleInstanceNode, "module_instance_guid", ModuleInstanceGuid.ToString());
            settingNode = xdoc.CreateElement("Setting");
            AddXmlAttribute(settingNode, "name", "Setting1");
            AddXmlAttribute(settingNode, "value", "Value1");
            AddXmlAttribute(settingNode, "type_id", ((int)ModuleInstanceSettingType.Text).ToString());
            moduleInstanceNode.AppendChild(settingNode);

            //
            // Run the test.
            //
            mi = new ModuleInstance(moduleInstanceNode);

            //
            // Verify the test.
            //
            Assert.AreEqual(1, moduleInstanceNode.ChildNodes.Count);
            Assert.AreEqual(TempModuleInstanceID, mi.ModuleInstanceID);
            Assert.AreEqual(ModuleTitle, mi.ModuleTitle);
            Assert.AreEqual(ShowTitle, mi.ShowTitle);
            Assert.AreEqual(TemplateFrameName, mi.TemplateFrameName);
            Assert.AreEqual(ModuleDetails, mi.ModuleDetails);
            Assert.AreEqual(TempModuleID, mi.ModuleTypeID);
            Assert.AreEqual(ModuleInstanceGuid, mi.Guid);
        }


        [Test]
        public void XmlConstructorLoadTemplateDefaultGuidTest()
        {
            ModuleInstance mi;
            XmlDocument xdoc;
            XmlNode moduleInstanceNode;
            Boolean ShowTitle;
            String ModuleTitle, TemplateFrameName, ModuleDetails;
            Guid ModuleInstanceGuid;
            int TempModuleInstanceID, TempModuleID;


            //
            // Setup the test.
            //
            xdoc = new XmlDocument();
            moduleInstanceNode = xdoc.CreateElement("ModuleInstance");
            TempModuleInstanceID = -3;
            ModuleTitle = "My Module Instance";
            ShowTitle = true;
            TemplateFrameName = "Left";
            ModuleDetails = "This is very detailed.";
            TempModuleID = -5;
            AddXmlAttribute(moduleInstanceNode, "temp_module_instance_id", TempModuleInstanceID.ToString());
            AddXmlAttribute(moduleInstanceNode, "module_title", ModuleTitle);
            AddXmlAttribute(moduleInstanceNode, "show_title", Convert.ToInt32(ShowTitle).ToString());
            AddXmlAttribute(moduleInstanceNode, "template_frame_name", TemplateFrameName);
            AddXmlAttribute(moduleInstanceNode, "module_details", ModuleDetails);
            AddXmlAttribute(moduleInstanceNode, "temp_module_id", TempModuleID.ToString());

            //
            // Run the test.
            //
            mi = new ModuleInstance(moduleInstanceNode);

            //
            // Verify the test.
            //
            Assert.AreEqual(0, moduleInstanceNode.ChildNodes.Count);
            Assert.AreEqual(TempModuleInstanceID, mi.ModuleInstanceID);
            Assert.AreEqual(ModuleTitle, mi.ModuleTitle);
            Assert.AreEqual(ShowTitle, mi.ShowTitle);
            Assert.AreEqual(TemplateFrameName, mi.TemplateFrameName);
            Assert.AreEqual(ModuleDetails, mi.ModuleDetails);
            Assert.AreEqual(TempModuleID, mi.ModuleTypeID);
            Assert.AreNotEqual(null, mi.Guid);
        }


        [Test]
        public void GenerateModuleInstanceIDTest()
        {
            ModuleInstance m;
            Package package;


            //
            // Setup the test.
            //
            package = new Package();
            m = new ModuleInstance();
            m.Package = package;

            //
            // Test is run automatically during verification.
            //

            //
            // Verify the test.
            //
            Assert.AreNotEqual(0, m.ModuleInstanceID);
        }


        [Test]
        public void PageTest()
        {
            ModuleInstance m;
            PageInstance pi;
            Package package;


            //
            // Setup the test.
            //
            package = new Package();
            pi = new PageInstance();
            package.Pages.Add(pi);
            m = new ModuleInstance();
            pi.Modules.Add(m);

            //
            // Test automatically runs during verification.
            //

            //
            // Verify the test.
            //
            Assert.AreSame(pi, m.Page);
        }


        [Test]
        public void InvalidPageTest()
        {
            ModuleInstance m;


            //
            // Setup the test.
            //
            m = new ModuleInstance();

            //
            // Test automatically runs during verification.
            //

            //
            // Verify the test.
            //
            Assert.AreEqual(null, m.Page);
        }


        [Test]
        public void TemplateFrameOrderTest()
        {
            ModuleInstance m;
            PageInstance pi;
            Package package;


            //
            // Setup the test.
            //
            package = new Package();
            pi = new PageInstance();
            package.Pages.Add(pi);
            m = new ModuleInstance();
            pi.Modules.Add(new ModuleInstance());
            pi.Modules.Add(m);

            //
            // Test automatically runs during verification.
            //

            //
            // Verify the test.
            //
            Assert.AreEqual(1, m.TemplateFrameOrder);
        }


        [Test]
        public void ModuleSettingsTest()
        {
            ModuleInstanceSetting s;
            ModuleInstance m;
            String settings;


            //
            // Setup the test.
            //
            m = new ModuleInstance();
            s = new ModuleInstanceSetting("Setting1");
            s.Value = "Value 1";
            m.Settings.Add(s);
            m.Settings.Add(new ModuleInstanceSetting("Setting2"));
            s = new ModuleInstanceSetting("Setting3");
            s.Value = "25";
            m.Settings.Add(s);

            //
            // Run the test.
            //
            settings = m.ModuleSettings();

            //
            // Verify the test.
            //
            Assert.AreEqual("Setting1=Value 1;Setting3=25", settings);
        }


        [Test]
        public void SaveTest()
        {
            ModuleInstanceSetting mis;
            ModuleInstance mi;
            PageInstance pi;
            XmlDocument xdoc;
            Package package;
            XmlNode moduleInstanceNode;
            Boolean ShowTitle;
            String ModuleTitle, TemplateFrameName, ModuleDetails;
            Guid ModuleInstanceGuid;
            int TempModuleInstanceID, TempModuleID, TempPageID;


            //
            // Setup the test.
            //
            xdoc = new XmlDocument();
            package = new Package();
            pi = new PageInstance();
            package.Pages.Add(pi);
            mi = new ModuleInstance();
            pi.Modules.Add(mi);
            TempPageID = -3;
            TempModuleInstanceID = -6;
            ModuleTitle = "New Module";
            ShowTitle = true;
            TemplateFrameName = "Main";
            ModuleDetails = "Some details";
            TempModuleID = -9;
            ModuleInstanceGuid = new Guid();
            pi._PageID = TempPageID;
            mi._ModuleInstanceID = TempModuleInstanceID;
            mi.ModuleTitle = ModuleTitle;
            mi.ShowTitle = Convert.ToBoolean(Convert.ToInt32(ShowTitle));
            mi.TemplateFrameName = TemplateFrameName;
            mi.ModuleDetails = ModuleDetails;
            mi.ModuleTypeID = TempModuleID;
            mi.Guid = ModuleInstanceGuid;
            mis = new ModuleInstanceSetting("Setting1");
            mis.Value = "Value1";
            mis.Type = ModuleInstanceSettingType.Text;
            mi.Settings.Add(mis);
            mis = new ModuleInstanceSetting("Setting2");
            mis.Value = "Value2";
            mis.Type = ModuleInstanceSettingType.Text;
            mi.Settings.Add(mis);

            //
            // Run the test.
            //
            moduleInstanceNode = mi.Save(xdoc, false);
            
            //
            // Verify the test.
            //
            Assert.AreEqual(2, moduleInstanceNode.ChildNodes.Count);
            Assert.AreEqual(TempModuleInstanceID.ToString(), moduleInstanceNode.Attributes["temp_module_instance_id"].Value);
            Assert.AreEqual(ModuleTitle, moduleInstanceNode.Attributes["module_title"].Value);
            Assert.AreEqual(Convert.ToInt32(ShowTitle).ToString(), moduleInstanceNode.Attributes["show_title"].Value);
            Assert.AreEqual(TemplateFrameName, moduleInstanceNode.Attributes["template_frame_name"].Value);
            Assert.AreEqual("0", moduleInstanceNode.Attributes["template_frame_order"].Value);
            Assert.AreEqual(ModuleDetails, moduleInstanceNode.Attributes["module_details"].Value);
            Assert.AreEqual("0", moduleInstanceNode.Attributes["system_flag"].Value);
            Assert.AreEqual("0", moduleInstanceNode.Attributes["mandatory"].Value);
            Assert.AreEqual("0", moduleInstanceNode.Attributes["movable"].Value);
            Assert.AreEqual("", moduleInstanceNode.Attributes["description"].Value);
            Assert.AreEqual("", moduleInstanceNode.Attributes["image_path"].Value);
            Assert.AreEqual(TempModuleID.ToString(), moduleInstanceNode.Attributes["temp_module_id"].Value);
            Assert.AreEqual(TempPageID.ToString(), moduleInstanceNode.Attributes["temp_page_id"].Value);
            Assert.AreEqual("Setting1=Value1;Setting2=Value2", moduleInstanceNode.Attributes["module_settings"].Value);
            Assert.AreEqual(ModuleInstanceGuid.ToString(), moduleInstanceNode.Attributes["module_instance_guid"].Value);
            Assert.AreEqual(TempPageID.ToString(), moduleInstanceNode.Attributes["temp_page_or_template_id"].Value);
            Assert.AreEqual("1", moduleInstanceNode.Attributes["page_instance"].Value);
        }
    }
}
