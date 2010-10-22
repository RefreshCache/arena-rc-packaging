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
    class FileCollectionTests
    {
        [Test]
        public void ConstructorTest()
        {
            FileCollection fc;
            Package package;


            //
            // Setup the test.
            //
            package = new Package();

            //
            // Run the test.
            //
            fc = new FileCollection(package);
            
            //
            // Verify the test.
            //
            Assert.AreSame(package, fc.Owner);
        }


        [Test]
        public void AddItemTest()
        {
            FileCollection fc;
            Package package;
            File f;


            //
            // Setup the test.
            //
            package = new Package();
            fc = new FileCollection(package);
            fc.Add(new File());
            f = new File();

            //
            // Run the test.
            //
            fc.Add(f);

            //
            // Verify the test.
            //
            Assert.AreSame(f, fc[1]);
        }


        [Test]
        public void InsertItemTest()
        {
            FileCollection fc;
            Package package;
            File f;


            //
            // Setup the test.
            //
            package = new Package();
            fc = new FileCollection(package);
            fc.Add(new File());
            f = new File();

            //
            // Run the test.
            //
            fc.Insert(0, f);

            //
            // Verify the test.
            //
            Assert.AreSame(f, fc[0]);
        }


        [Test]
        public void SetItemTest()
        {
            FileCollection fc;
            Package package, package2;
            File f1, f2;


            //
            // Setup the test.
            //
            package = new Package();
            package2 = new Package();
            fc = new FileCollection(package);
            f1 = new File("", "", package2);
            f2 = new File();
            fc.Add(f1);

            //
            // Run the test.
            //
            fc[0] = f2;

            //
            // Verify the test.
            //
            Assert.AreSame(f2, fc[0]);
            Assert.AreEqual(1, fc.Count);
            Assert.AreEqual(null, f1.Package);
            Assert.AreEqual(package, f2.Package);
        }


        [Test]
        public void RemoveItemTest()
        {
            FileCollection fc;
            Package package;
            File f;


            //
            // Setup the test.
            //
            package = new Package();
            fc = new FileCollection(package);
            f = new File();
            fc.Add(f);

            //
            // Run the test.
            //
            fc.Remove(f);

            //
            // Verify the test.
            //
            Assert.AreEqual(null, f.Package);
            Assert.AreEqual(0, fc.Count);
        }


        [Test]
        public void SetPackageTest()
        {
            FileCollection fc;
            Package package;
            File f;


            //
            // Setup the test.
            //
            package = new Package();
            fc = new FileCollection(null);
            f = new File();
            fc.Add(f);

            //
            // Run the test.
            //
            fc.Owner = package;

            //
            // Verify the test.
            //
            Assert.AreSame(package, f.Package);
        }
    }
}
