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
    class PageInstanceTests : TestsHelper
    {
        [Test]
        public void BasicConstructorTest()
        {
            PageInstance page;


            //
            // No preparation needed.
            //
            
            //
            // Run the test.
            //
            page = new PageInstance();

            //
            // Verify the test.
            //
            Assert.IsNotNullOrEmpty(page.Guid);
            Assert.AreEqual(null, page.ParentPage);
            Assert.AreEqual(0, page.Files.Count);
            Assert.AreEqual(0, page.Modules.Count);
            Assert.AreEqual(0, page.Pages.Count);
            Assert.AreEqual(13, page.Settings.Count);
        }


        [Test]
        public void XmlConstructorTest()
        {
            ModuleInstance mi;
            PageInstance page, p;
            XmlDocument xdoc;
            XmlNode pageNode;
            Package pkg;
            Boolean DisplayInNav, RequireSSL, ValidateRequest;
            String PageName, PageDescription, EnableViewState, ShowCss;
            File f;
            Guid PageGuid;
            int TempPageID;


            //
            // Prepare the test.
            //
            xdoc = new XmlDocument();
            pageNode = xdoc.CreateElement("Page");
            TempPageID = -2;
            AddXmlAttribute(pageNode, "temp_page_id", TempPageID.ToString());
            PageName = "Test Page";
            AddXmlAttribute(pageNode, "page_name", PageName);
            PageDescription = "Some description";
            AddXmlAttribute(pageNode, "page_desc", PageDescription);
            DisplayInNav = true;
            AddXmlAttribute(pageNode, "display_in_nav", "1");
            RequireSSL = false;
            AddXmlAttribute(pageNode, "require_ssl", "0");
            ValidateRequest = true;
            AddXmlAttribute(pageNode, "validate_request", "1");
            PageGuid = Guid.NewGuid();
            AddXmlAttribute(pageNode, "guid", PageGuid.ToString());
            EnableViewState = "true";
            ShowCss = "false";
            AddXmlAttribute(pageNode, "page_settings", "EnableViewState=" + EnableViewState + ";ShowCss=" + ShowCss);
            pkg = new Package();
            p = new PageInstance();
            pkg.Pages.Add(p);
            pageNode.AppendChild(p.Save(xdoc, false));
            mi = new ModuleInstance();
            p.Modules.Add(mi);
            pageNode.AppendChild(mi.Save(xdoc, false));
            f = new File();
            pageNode.AppendChild(f.Save(xdoc, false));

            //
            // Run the test.
            //
            page = new PageInstance(pageNode);

            //
            // Verify the test.
            //
            Assert.AreEqual(TempPageID, page.PageID);
            Assert.AreEqual(PageName, page.PageName);
            Assert.AreEqual(PageDescription, page.PageDescription);
            Assert.AreEqual(DisplayInNav, page.DisplayInNav);
            Assert.AreEqual(RequireSSL, page.RequireSSL);
            Assert.AreEqual(ValidateRequest, page.ValidateRequest);
            Assert.AreEqual(PageGuid.ToString(), page.Guid);
            Assert.AreEqual(EnableViewState, page.Settings[0].Value);
            Assert.AreEqual(ShowCss, page.Settings[1].Value);
            Assert.AreEqual(1, page.Files.Count);
            Assert.AreEqual(1, page.Modules.Count);
            Assert.AreEqual(1, page.Pages.Count);
        }


        [Test]
        public void SaveTest()
        {
            ModuleInstance mi;
            PageInstance page, p;
            XmlDocument xdoc;
            XmlNode node;
            Package pkg;
            String PageName, PageDescription, DisplayInNav, RequireSSL, ValidateRequest, settings;
            Guid PageGuid;
            int TempPageID;


            //
            // Setup the test.
            //
            xdoc = new XmlDocument();
            pkg = new Package();
            page = new PageInstance();
            pkg.Pages.Add(page);
            page.PageName = PageName = "Test Page";
            page.PageDescription = PageDescription = "Some Description";
            DisplayInNav = "1";
            page.DisplayInNav = true;
            RequireSSL = "0";
            page.RequireSSL = false;
            ValidateRequest = "1";
            page.ValidateRequest = true;
            settings = "EnableViewState=true;ShowCss=false";
            page.Settings[0].Value = "true";
            page.Settings[1].Value = "false";
            PageGuid = Guid.NewGuid();
            page.Guid = PageGuid.ToString();
            p = new PageInstance();
            page.Pages.Add(p);
            mi = new ModuleInstance();
            page.Modules.Add(mi);

            //
            // Run the test.
            //
            node = page.Save(xdoc, false);

            //
            // Verify the test.
            //
            Assert.AreEqual(12, node.Attributes.Count);
            Assert.AreEqual("-1", node.Attributes["temp_page_id"].Value);
            Assert.AreEqual("2147483647", node.Attributes["page_order"].Value);
            Assert.AreEqual(DisplayInNav, node.Attributes["display_in_nav"].Value);
            Assert.AreEqual(PageName, node.Attributes["page_name"].Value);
            Assert.AreEqual(PageDescription, node.Attributes["page_desc"].Value);
            Assert.AreEqual(settings, node.Attributes["page_settings"].Value);
            Assert.AreEqual(RequireSSL, node.Attributes["require_ssl"].Value);
            Assert.AreEqual(PageGuid.ToString(), node.Attributes["guid"].Value);
            Assert.AreEqual("0", node.Attributes["system_flag"].Value);
            Assert.AreEqual("", node.Attributes["friendly_url"].Value);
            Assert.AreEqual(ValidateRequest, node.Attributes["validate_request"].Value);
            Assert.AreEqual("0", node.Attributes["temp_parent_page_id"].Value);
            Assert.AreEqual(2, node.ChildNodes.Count);
            Assert.AreEqual(1, node.SelectNodes("Page").Count);
            Assert.AreEqual(1, node.SelectNodes("ModuleInstance").Count);
        }


        [Test]
        public void AddPageToPackageTest()
        {
            PageInstance page;
            Package pkg;


            //
            // Prepare the test.
            //
            pkg = new Package();
            page = new PageInstance();

            //
            // Run the test.
            //
            pkg.Pages.Add(page);

            //
            // Verify the test.
            //
            Assert.AreSame(pkg, page.Package);
            Assert.AreEqual(-1, page.PageID);
        }


        [Test]
        public void AddPageToPageTest()
        {
            PageInstance page, parent;
            Package pkg;


            //
            // Prepare the test.
            //
            pkg = new Package();
            parent = new PageInstance();
            pkg.Pages.Add(parent);
            page = new PageInstance();

            //
            // Run the test.
            //
            parent.Pages.Add(page);

            //
            // Verify the test.
            //
            Assert.AreSame(page.ParentPage, parent);
        }


        [Test]
        public void PageSettingsTest()
        {
            PageInstance page;
            String settings;


            //
            // Prepare the test.
            //
            page = new PageInstance();
            page.Settings[0].Value = "true";
            page.Settings[1].Value = "false";

            //
            // Run the test.
            //
            settings = page.PageSettings();

            //
            // Verify the test.
            //
            Assert.AreEqual("EnableViewState=true;ShowCss=false", settings);
        }
    }
}
