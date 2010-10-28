using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using RefreshCache.Packager;
using RefreshCache.Packager.Migrator;


namespace RefreshCache.Packager.Tests
{
    [TestFixture]
    class PackageVersionTests
    {
        [Test]
        public void StringConstructorEmptyTest()
        {
            PackageVersion v;


            //
            // No preparation needed.
            //

            //
            // Run the test.
            //
            v = new PackageVersion("");

            //
            // Verify the test.
            //
            Assert.AreEqual(0, v.Major);
            Assert.AreEqual(0, v.Minor);
            Assert.AreEqual(0, v.Revision);
        }


        [Test]
        [ExpectedException(typeof(Exception))]
        public void StringConstructorInvalidTest()
        {
            //
            // No preparation needed.
            //

            //
            // Run the test.
            //
            new PackageVersion("1.2.3.4.5");

            //
            // No verification needed.
            //
        }


        [Test]
        public void IndividualConstructorTest()
        {
            PackageVersion v;
            int major, minor, revision;


            //
            // Prepare the test.
            //
            major = 3;
            minor = 7;
            revision = 1;

            //
            // Run the test.
            //
            v = new PackageVersion(major, minor, revision);

            //
            // Verify the test.
            //
            Assert.AreEqual(major, v.Major);
            Assert.AreEqual(minor, v.Minor);
            Assert.AreEqual(revision, v.Revision);
        }


        [Test]
        public void CompareToEqualTest()
        {
            PackageVersion v1, v2;
            int result;


            //
            // Prepare the test.
            //
            v1 = new PackageVersion("1.3.7");
            v2 = new PackageVersion(1, 3, 7);

            //
            // Run the test.
            //
            result = v1.CompareTo(v2);

            //
            // Verify the test.
            //
            Assert.AreEqual(v1.Major, v2.Major);
            Assert.AreEqual(v1.Minor, v2.Minor);
            Assert.AreEqual(v1.Revision, v2.Revision);
            Assert.AreEqual(0, result);
        }


        [Test]
        public void CompareToLessThanMajorTest()
        {
            PackageVersion v1, v2;
            int result;


            //
            // Prepare the test.
            //
            v1 = new PackageVersion("1.3.7");
            v2 = new PackageVersion("2.7.1");

            //
            // Run the test.
            //
            result = v1.CompareTo(v2);

            //
            // Verify the test.
            //
            Assert.AreEqual(-1, result);
        }


        [Test]
        public void CompareToLessThanMinorTest()
        {
            PackageVersion v1, v2;
            int result;


            //
            // Prepare the test.
            //
            v1 = new PackageVersion("1.3.7");
            v2 = new PackageVersion("1.7.1");

            //
            // Run the test.
            //
            result = v1.CompareTo(v2);

            //
            // Verify the test.
            //
            Assert.AreEqual(-1, result);
        }


        [Test]
        public void CompareToLessThanRevisionTest()
        {
            PackageVersion v1, v2;
            int result;


            //
            // Prepare the test.
            //
            v1 = new PackageVersion("1.3.7");
            v2 = new PackageVersion("1.3.9");

            //
            // Run the test.
            //
            result = v1.CompareTo(v2);

            //
            // Verify the test.
            //
            Assert.AreEqual(-1, result);
        }


        [Test]
        public void CompareToLessThanMigratorStepTest()
        {
            MigratorVersionStep v2;
            PackageVersion v1;
            int result;


            //
            // Prepare the test.
            //
            v1 = new PackageVersion("1.3.7");
            v2 = new MigratorVersionStep(1, 3, 7, 6);

            //
            // Run the test.
            //
            result = v1.CompareTo(v2);

            //
            // Verify the test.
            //
            Assert.AreEqual(-1, result);
        }


        [Test]
        public void CompareToGreaterThanMajorTest()
        {
            PackageVersion v1, v2;
            int result;


            //
            // Prepare the test.
            //
            v1 = new PackageVersion("1.3.7");
            v2 = new PackageVersion("0.7.1");

            //
            // Run the test.
            //
            result = v1.CompareTo(v2);

            //
            // Verify the test.
            //
            Assert.AreEqual(1, result);
        }


        [Test]
        public void CompareToGreaterThanMinorTest()
        {
            PackageVersion v1, v2;
            int result;


            //
            // Prepare the test.
            //
            v1 = new PackageVersion("1.3.7");
            v2 = new PackageVersion("1.2.1");

            //
            // Run the test.
            //
            result = v1.CompareTo(v2);

            //
            // Verify the test.
            //
            Assert.AreEqual(1, result);
        }


        [Test]
        public void CompareToGreaterThanRevisionTest()
        {
            PackageVersion v1, v2;
            int result;


            //
            // Prepare the test.
            //
            v1 = new PackageVersion("1.3.7");
            v2 = new PackageVersion("1.3.5");

            //
            // Run the test.
            //
            result = v1.CompareTo(v2);

            //
            // Verify the test.
            //
            Assert.AreEqual(1, result);
        }


        [Test]
        public void ToStringTest()
        {
            PackageVersion v;
            String version;


            //
            // Prepare the test.
            //
            version = "1.3.7";

            //
            // Run the test.
            //
            v = new PackageVersion(version);

            //
            // Verify the test.
            //
            Assert.AreEqual(version, v.ToString());
        }
    }
}
