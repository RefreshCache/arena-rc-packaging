using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using NUnit.Framework;
using RefreshCache.Packager;
using RefreshCache.Packager.Manager;


namespace RefreshCache.Packager.Tests
{
    [TestFixture]
    class VersionRequirementTests : TestsHelper
    {
        [Test]
        public void BaseConstructorTest()
        {
            VersionRequirement vr;


            //
            // No preparation needed.
            //

            //
            // Run the test.
            //
            vr = new VersionRequirement();

            //
            // Verify the test.
            //
            Assert.AreEqual("VersionRequirement", vr.NodeName);
        }


        [Test]
        public void XmlConstructorTest()
        {
            VersionRequirement vr;
            XmlDocument xdoc;
            XmlNode node;
            String MinVersion, MaxVersion, Version;


            //
            // Prepare the test.
            //
            xdoc = new XmlDocument();
            node = xdoc.CreateElement("VersionRequirement");
            MinVersion = "1.2.3";
            AddXmlAttribute(node, "MinVersion", MinVersion);
            MaxVersion = "4.5.6";
            AddXmlAttribute(node, "MaxVersion", MaxVersion);
            Version = "7.8.9";
            AddXmlAttribute(node, "Version", Version);

            //
            // Run the test.
            //
            vr = new VersionRequirement(node);

            //
            // Verify the test.
            //
            Assert.AreEqual(MinVersion, vr.MinVersion.ToString());
            Assert.AreEqual(MaxVersion, vr.MaxVersion.ToString());
            Assert.AreEqual(Version, vr.Version.ToString());
        }


        [Test]
        [ExpectedException(typeof(PackageDependencyException))]
        public void ValidateVersion_LessThanMinimumTest()
        {
            VersionRequirement vr;
            PackageVersion pv;


            //
            // Prepare the test.
            //
            vr = new VersionRequirement();
            vr.MinVersion = new PackageVersion("1.0.0");
            pv = new PackageVersion("0.9.5");

            //
            // Run the test.
            //
            vr.ValidateVersion(pv, "x");

            //
            // Validation is via exception or not.
            //
        }


        [Test]
        public void ValidateVersion_EqualToMinimumTest()
        {
            VersionRequirement vr;
            PackageVersion pv;


            //
            // Prepare the test.
            //
            vr = new VersionRequirement();
            vr.MinVersion = new PackageVersion("1.0.0");
            pv = new PackageVersion("1.0.0");

            //
            // Run the test.
            //
            vr.ValidateVersion(pv, "x");

            //
            // Validation is via exception or not.
            //
        }


        [Test]
        public void ValidateVersion_GreaterThanMinimumTest()
        {
            VersionRequirement vr;
            PackageVersion pv;


            //
            // Prepare the test.
            //
            vr = new VersionRequirement();
            vr.MinVersion = new PackageVersion("1.0.0");
            pv = new PackageVersion("1.9.5");

            //
            // Run the test.
            //
            vr.ValidateVersion(pv, "x");

            //
            // Validation is via exception or not.
            //
        }


        [Test]
        public void ValidateVersion_LessThanMaximumTest()
        {
            VersionRequirement vr;
            PackageVersion pv;


            //
            // Prepare the test.
            //
            vr = new VersionRequirement();
            vr.MaxVersion = new PackageVersion("1.0.0");
            pv = new PackageVersion("0.9.5");

            //
            // Run the test.
            //
            vr.ValidateVersion(pv, "x");

            //
            // Validation is via exception or not.
            //
        }


        [Test]
        public void ValidateVersion_EqualToMaximumTest()
        {
            VersionRequirement vr;
            PackageVersion pv;


            //
            // Prepare the test.
            //
            vr = new VersionRequirement();
            vr.MaxVersion = new PackageVersion("1.0.0");
            pv = new PackageVersion("1.0.0");

            //
            // Run the test.
            //
            vr.ValidateVersion(pv, "x");

            //
            // Validation is via exception or not.
            //
        }


        [Test]
        [ExpectedException(typeof(PackageDependencyException))]
        public void ValidateVersion_GreaterThanMaximumTest()
        {
            VersionRequirement vr;
            PackageVersion pv;


            //
            // Prepare the test.
            //
            vr = new VersionRequirement();
            vr.MaxVersion = new PackageVersion("1.0.0");
            pv = new PackageVersion("1.9.5");

            //
            // Run the test.
            //
            vr.ValidateVersion(pv, "x");

            //
            // Validation is via exception or not.
            //
        }


        [Test]
        [ExpectedException(typeof(PackageDependencyException))]
        public void ValidateVersion_LessThanVersionTest()
        {
            VersionRequirement vr;
            PackageVersion pv;


            //
            // Prepare the test.
            //
            vr = new VersionRequirement();
            vr.Version = new PackageVersion("1.0.0");
            pv = new PackageVersion("0.9.5");

            //
            // Run the test.
            //
            vr.ValidateVersion(pv, "x");

            //
            // Validation is via exception or not.
            //
        }


        [Test]
        public void ValidateVersion_EqualToVersionTest()
        {
            VersionRequirement vr;
            PackageVersion pv;


            //
            // Prepare the test.
            //
            vr = new VersionRequirement();
            vr.Version = new PackageVersion("1.0.0");
            pv = new PackageVersion("1.0.0");

            //
            // Run the test.
            //
            vr.ValidateVersion(pv, "x");

            //
            // Validation is via exception or not.
            //
        }


        [Test]
        [ExpectedException(typeof(PackageDependencyException))]
        public void ValidateVersion_GreaterThanVersionTest()
        {
            VersionRequirement vr;
            PackageVersion pv;


            //
            // Prepare the test.
            //
            vr = new VersionRequirement();
            vr.Version = new PackageVersion("1.0.0");
            pv = new PackageVersion("1.9.5");

            //
            // Run the test.
            //
            vr.ValidateVersion(pv, "x");

            //
            // Validation is via exception or not.
            //
        }


        [Test]
        public void SaveTest()
        {
            VersionRequirement vr;
            XmlDocument xdoc;
            XmlNode node;
            String MinVersion, MaxVersion, Version;


            //
            // Prepare the test.
            //
            xdoc = new XmlDocument();
            vr = new VersionRequirement();
            MinVersion = "1.2.3";
            vr.MinVersion = new PackageVersion("1.2.3");
            MaxVersion = "4.5.6";
            vr.MaxVersion = new PackageVersion("4.5.6");
            Version = "7.8.9";
            vr.Version = new PackageVersion("7.8.9");
            
            //
            // Run the test.
            //
            node = vr.Save(xdoc, false);

            //
            // Verify the test.
            //
            Assert.AreEqual(MinVersion, node.Attributes["MinVersion"].Value);
            Assert.AreEqual(MaxVersion, node.Attributes["MaxVersion"].Value);
            Assert.AreEqual(Version, node.Attributes["Version"].Value);
        }
    }
}
