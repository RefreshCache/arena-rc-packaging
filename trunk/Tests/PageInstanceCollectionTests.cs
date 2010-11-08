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
    class PageInstanceCollectionTests
    {
        [Test]
        public void ConstructorTest()
        {
            PageInstanceCollection pic;
            Package package;


            //
            // Setup the test.
            //
            package = new Package();

            //
            // Run the test.
            //
            pic = new PageInstanceCollection(package);

            //
            // Verify the test.
            //
            Assert.AreSame(package, pic.Owner);
        }


        [Test]
        public void AddItemTest()
        {
            PageInstanceCollection pic;
            PageInstance pi;
            Package package;


            //
            // Setup the test.
            //
            package = new Package();
            pic = new PageInstanceCollection(package);
            pic.Add(new PageInstance());
            pi = new PageInstance();

            //
            // Run the test.
            //
            pic.Add(pi);

            //
            // Verify the test.
            //
            Assert.AreSame(pi, pic[1]);
        }


        [Test]
        public void InsertItemTest()
        {
            PageInstanceCollection pic;
            PageInstance pi;
            Package package;


            //
            // Setup the test.
            //
            package = new Package();
            pic = new PageInstanceCollection(package);
            pic.Add(new PageInstance());
            pi = new PageInstance();

            //
            // Run the test.
            //
            pic.Insert(0, pi);

            //
            // Verify the test.
            //
            Assert.AreSame(pi, pic[0]);
        }


        [Test]
        public void SetItemTest()
        {
            PageInstanceCollection pic;
            PageInstance pi1, pi2;
            Package package, package2;


            //
            // Setup the test.
            //
            package = new Package();
            package2 = new Package();
            pic = new PageInstanceCollection(package);
            pi1 = new PageInstance();
            pi1.Package = package2;
            pi2 = new PageInstance();
            pic.Add(pi1);

            //
            // Run the test.
            //
            pic[0] = pi2;

            //
            // Verify the test.
            //
            Assert.AreSame(pi2, pic[0]);
            Assert.AreEqual(1, pic.Count);
            Assert.AreEqual(null, pi1.Package);
            Assert.AreEqual(package, pi2.Package);
        }


        [Test]
        public void RemoveItemTest()
        {
            PageInstanceCollection pic;
            PageInstance pi;
            Package package;


            //
            // Setup the test.
            //
            package = new Package();
            pic = new PageInstanceCollection(package);
            pi = new PageInstance();
            pic.Add(pi);

            //
            // Run the test.
            //
            pic.Remove(pi);

            //
            // Verify the test.
            //
            Assert.AreEqual(null, pi.Package);
            Assert.AreEqual(0, pic.Count);
        }


        [Test]
        public void SetPackageTest()
        {
            PageInstanceCollection pic;
            PageInstance pi;
            Package package;


            //
            // Setup the test.
            //
            package = new Package();
            pic = new PageInstanceCollection(null);
            pi = new PageInstance();
            pic.Add(pi);

            //
            // Run the test.
            //
            pic.Owner = package;

            //
            // Verify the test.
            //
            Assert.AreSame(package, pi.Package);
        }
    }
}
