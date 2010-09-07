using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Xml;
using NUnit.Framework;
using RefreshCache.Packager;
using RefreshCache.Packager.Manager;

namespace Migrator_Tests
{
    [TestFixture]
    class InstallTest
    {
        [Test]
//        [Ignore]
        public void Test()
        {
            SqlConnection con;
            String DataSource = "CONSTANTINE\\HDCArena";
            PackageDatabase pdb;
            XmlDocument xdoc;
            Package package1, package2;


            con = new SqlConnection("Data Source=" + DataSource + ";Initial Catalog=ArenaTestDB;Integrated Security=SSPI");
            con.Open();

            DirectoryInfo di = new DirectoryInfo("C:/Arena/Test");

                if (di.Exists == false)
                    throw new Exception("Arena Test Path does not exist");

            xdoc = new XmlDocument();
            xdoc.Load("../../../../Packager/RefreshCache.Packager.xml");
            package1 = new Package(xdoc);
            xdoc = new XmlDocument();
            xdoc.Load("../../../../Arena.Custom.RC.Utilities/Arena.Custom.RC.Utilities.xml");
            package2 = new Package(xdoc);

            pdb = new PackageDatabase(di.FullName, con);
            pdb.InstallSystem(new Package[] { package1, package2 });
        }
    }
}
