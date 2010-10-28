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
    class PackageInfoTests : TestsHelper
    {
        [Test]
        public void BaseContructorTest()
        {
            PackageInfo info;


            //
            // No setup needed.
            //
            
            //
            // Run the test.
            //
            info = new PackageInfo();

            //
            // Verify the test.
            //
            Assert.AreEqual(0, info.Recommends.Count);
            Assert.AreEqual(0, info.Requires.Count);
            Assert.AreEqual(0, info.Changelog.Count);
        }


        [Test]
        public void XmlConstructorTest()
        {
            PackageRecommendation rec;
            PackageRequirement req;
            PackageChangelog changelog;
            VersionRequirement arena;
            PackageInfo info;
            XmlDocument xdoc;
            XmlNode infoNode;
            String distributor, packagename, version, synopsis, description;


            //
            // Prepare the test.
            //
            xdoc = new XmlDocument();
            infoNode = xdoc.CreateElement("Info");
            distributor = "RefrshCache";
            AddSimpleXmlNode(infoNode, "Distributor", distributor);
            packagename = "RC.TestPackage";
            AddSimpleXmlNode(infoNode, "PackageName", packagename);
            version = "3.2.8";
            AddSimpleXmlNode(infoNode, "Version", version);
            synopsis = "This is a short synopsis message.";
            AddSimpleXmlNode(infoNode, "Synopsis", synopsis);
            description = "This is a longer description.";
            AddSimpleXmlNode(infoNode, "Description", description);
            arena = new VersionRequirement();
            arena.Version = new PackageVersion("1.2.3");
            arena.MinVersion = new PackageVersion("4.5.6");
            arena.MaxVersion = new PackageVersion("7.8.9");
            infoNode.AppendChild(arena.Save(xdoc, false).CopyAndRename("ArenaVersion"));
            req = new PackageRequirement();
            req.Name = "RC.Package1";
            infoNode.AppendChild(req.Save(xdoc, false));
            rec = new PackageRecommendation();
            rec.Name = "RC.Package2";
            rec.Description = "You should install this.";
            infoNode.AppendChild(rec.Save(xdoc, false));
            changelog = new PackageChangelog();
            changelog.Version = new PackageVersion("8.9.982");
            changelog.Description = "I made some changes.";
            infoNode.AppendChild(changelog.Save(xdoc, false));

            //
            // Run the test.
            //
            info = new PackageInfo(infoNode);

            //
            // Verify the test.
            //
            Assert.AreEqual(distributor, info.Distributor);
            Assert.AreEqual(packagename, info.PackageName);
            Assert.AreEqual(version, info.Version.ToString());
            Assert.AreEqual(synopsis, info.Synopsis);
            Assert.AreEqual(description, info.Description);
            Assert.AreEqual(arena.MinVersion.ToString(), info.Arena.MinVersion.ToString());
            Assert.AreEqual(arena.MaxVersion.ToString(), info.Arena.MaxVersion.ToString());
            Assert.AreEqual(arena.Version.ToString(), info.Arena.Version.ToString());
            Assert.AreEqual(1, info.Requires.Count);
            Assert.AreEqual(1, info.Recommends.Count);
            Assert.AreEqual(1, info.Changelog.Count);
        }


        [Test]
        public void SaveTest()
        {
            PackageInfo info;
            XmlDocument xdoc;
            XmlNode node;
            String distributor, packagename, version, synopsis, description;


            //
            // Prepare the test.
            //
            xdoc = new XmlDocument();
            info = new PackageInfo();
            info.Distributor = distributor = "RefreshCache";
            info.PackageName = packagename = "RC.TestPackage";
            version = "3.48.2";
            info.Version = new PackageVersion(version);
            info.Synopsis = synopsis = "This is a short synopsis.";
            info.Description = description = "A longer description is here.";
            info.Arena = new VersionRequirement();
            info.Arena.Version = new PackageVersion("1.2.3");
            info.Arena.MinVersion = new PackageVersion("4.5.6");
            info.Arena.MaxVersion = new PackageVersion("7.8.9");
            info.Requires.Add(new PackageRequirement());
            info.Requires[0].Name = "RC.Package1";
            info.Recommends.Add(new PackageRecommendation());
            info.Recommends[0].Name = "RC.Package2";
            info.Recommends[0].Description = "You should install this.";
            info.Changelog.Add(new PackageChangelog());
            info.Changelog[0].Version = new PackageVersion("8.9.982");
            info.Changelog[0].Description = "I made some changes.";

            //
            // Run the test.
            //
            node = info.Save(xdoc, false);

            //
            // Verify the test.
            //
            Assert.AreEqual(9, node.ChildNodes.Count);
            Assert.AreEqual(distributor, node.SelectSingleNode("child::Distributor").InnerText);
            Assert.AreEqual(packagename, node.SelectSingleNode("child::PackageName").InnerText);
            Assert.AreEqual(version, node.SelectSingleNode("child::Version").InnerText);
            Assert.AreEqual(synopsis, node.SelectSingleNode("child::Synopsis").InnerText);
            Assert.AreEqual(description, node.SelectSingleNode("child::Description").InnerText);
            Assert.AreEqual(1, node.SelectNodes("child::ArenaVersion").Count);
            Assert.AreEqual(1, node.SelectNodes("child::Require").Count);
            Assert.AreEqual(1, node.SelectNodes("child::Recommend").Count);
            Assert.AreEqual(1, node.SelectNodes("child::Changelog").Count);
        }
    }
}
