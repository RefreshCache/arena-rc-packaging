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
    class ModuleInstanceCollectionTests
    {
        [Test]
        public void EmptyConstructorTest()
        {
            ModuleInstanceCollection mic;


            //
            // No setup needed.
            //

            //
            // Run the test.
            //
            mic = new ModuleInstanceCollection();

            //
            // Verify the test.
            //
            Assert.AreSame(null, mic.Owner);
        }


        [Test]
        public void ConstructorTest()
        {
            ModuleInstanceCollection mic;
            Package package;


            //
            // Setup the test.
            //
            package = new Package();

            //
            // Run the test.
            //
            mic = new ModuleInstanceCollection(package);

            //
            // Verify the test.
            //
            Assert.AreSame(package, mic.Owner);
        }


        [Test]
        public void AddItemTest()
        {
            ModuleInstanceCollection mic;
            ModuleInstance mi;
            Package package;


            //
            // Setup the test.
            //
            package = new Package();
            mic = new ModuleInstanceCollection(package);
            mic.Add(new ModuleInstance());
            mi = new ModuleInstance();

            //
            // Run the test.
            //
            mic.Add(mi);

            //
            // Verify the test.
            //
            Assert.AreSame(mi, mic[1]);
        }


        [Test]
        public void InsertItemTest()
        {
            ModuleInstanceCollection mic;
            ModuleInstance mi;
            Package package;


            //
            // Setup the test.
            //
            package = new Package();
            mic = new ModuleInstanceCollection(package);
            mic.Add(new ModuleInstance());
            mi = new ModuleInstance();

            //
            // Run the test.
            //
            mic.Insert(0, mi);

            //
            // Verify the test.
            //
            Assert.AreSame(mi, mic[0]);
        }


        [Test]
        public void SetItemTest()
        {
            ModuleInstanceCollection mic;
            ModuleInstance mi1, mi2;
            Package package, package2;


            //
            // Setup the test.
            //
            package = new Package();
            package2 = new Package();
            mic = new ModuleInstanceCollection(package);
            mi1 = new ModuleInstance();
            mi1.Package = package2;
            mi2 = new ModuleInstance();
            mic.Add(mi1);

            //
            // Run the test.
            //
            mic[0] = mi2;

            //
            // Verify the test.
            //
            Assert.AreSame(mi2, mic[0]);
            Assert.AreEqual(1, mic.Count);
            Assert.AreEqual(null, mi1.Package);
            Assert.AreEqual(package, mi2.Package);
        }


        [Test]
        public void RemoveItemTest()
        {
            ModuleInstanceCollection mic;
            ModuleInstance mi;
            Package package;


            //
            // Setup the test.
            //
            package = new Package();
            mic = new ModuleInstanceCollection(package);
            mi = new ModuleInstance();
            mic.Add(mi);

            //
            // Run the test.
            //
            mic.Remove(mi);

            //
            // Verify the test.
            //
            Assert.AreEqual(null, mi.Package);
            Assert.AreEqual(0, mic.Count);
        }


        [Test]
        public void SetPackageTest()
        {
            ModuleInstanceCollection mic;
            ModuleInstance mi;
            Package package;


            //
            // Setup the test.
            //
            package = new Package();
            mic = new ModuleInstanceCollection(null);
            mi = new ModuleInstance();
            mic.Add(mi);

            //
            // Run the test.
            //
            mic.Owner = package;

            //
            // Verify the test.
            //
            Assert.AreSame(package, mi.Package);
        }
    }
}
