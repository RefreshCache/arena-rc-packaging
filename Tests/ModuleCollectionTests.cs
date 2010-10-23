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
    class ModuleCollectionTests
    {
        [Test]
        public void ConstructorTest()
        {
            ModuleCollection mc;
            Package package;


            //
            // Setup the test.
            //
            package = new Package();

            //
            // Run the test.
            //
            mc = new ModuleCollection(package);

            //
            // Verify the test.
            //
            Assert.AreSame(package, mc.Owner);
        }


        [Test]
        public void AddItemTest()
        {
            ModuleCollection mc;
            Package package;
            Module m;


            //
            // Setup the test.
            //
            package = new Package();
            mc = new ModuleCollection(package);
            mc.Add(new Module());
            m = new Module();

            //
            // Run the test.
            //
            mc.Add(m);

            //
            // Verify the test.
            //
            Assert.AreSame(m, mc[1]);
        }


        [Test]
        public void InsertItemTest()
        {
            ModuleCollection mc;
            Package package;
            Module m;


            //
            // Setup the test.
            //
            package = new Package();
            mc = new ModuleCollection(package);
            mc.Add(new Module());
            m = new Module();

            //
            // Run the test.
            //
            mc.Insert(0, m);

            //
            // Verify the test.
            //
            Assert.AreSame(m, mc[0]);
        }


        [Test]
        public void SetItemTest()
        {
            ModuleCollection mc;
            Package package, package2;
            Module m1, m2;


            //
            // Setup the test.
            //
            package = new Package();
            package2 = new Package();
            mc = new ModuleCollection(package);
            m1 = new Module();
            m1.Package = package2;
            m2 = new Module();
            mc.Add(m1);

            //
            // Run the test.
            //
            mc[0] = m2;

            //
            // Verify the test.
            //
            Assert.AreSame(m2, mc[0]);
            Assert.AreEqual(1, mc.Count);
            Assert.AreEqual(null, m1.Package);
            Assert.AreEqual(package, m2.Package);
        }


        [Test]
        public void RemoveItemTest()
        {
            ModuleCollection mc;
            Package package;
            Module m;


            //
            // Setup the test.
            //
            package = new Package();
            mc = new ModuleCollection(package);
            m = new Module();
            mc.Add(m);

            //
            // Run the test.
            //
            mc.Remove(m);

            //
            // Verify the test.
            //
            Assert.AreEqual(null, m.Package);
            Assert.AreEqual(0, mc.Count);
        }


        [Test]
        public void SetPackageTest()
        {
            ModuleCollection mc;
            Package package;
            Module m;


            //
            // Setup the test.
            //
            package = new Package();
            mc = new ModuleCollection(null);
            m = new Module();
            mc.Add(m);

            //
            // Run the test.
            //
            mc.Owner = package;

            //
            // Verify the test.
            //
            Assert.AreSame(package, m.Package);
        }
    }
}
