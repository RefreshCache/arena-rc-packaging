using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using NUnit.Framework;
using RefreshCache.Packager;
using RefreshCache.Packager.Manager;

namespace Migrator_Tests
{
    [TestFixture]
    class InstallTest
    {
        [Test]
        [Ignore]
        public void Test()
        {
            SqlConnection con;
            String DataSource = "CONSTANTINE\\HDCArena";
            RefreshCache.Packager.File file;
            PackageDatabase pdb;
            Package package;


            con = new SqlConnection("Data Source=" + DataSource + ";Initial Catalog=ArenaTestDB;Integrated Security=SSPI");
            con.Open();

            DirectoryInfo di = new DirectoryInfo("C:/Arena/Test");

            try
            {
                if (di.Exists == false)
                    throw new Exception("Arena Test Path does not exist");

                package = new Package();
                package.Info.PackageName = "RC.PackageManager";
                package.Info.Distributor = "RefreshCache";
                package.Info.Version = new PackageVersion("1.0.0");
                file = new RefreshCache.Packager.File();
                file.Path = "bin/RefreshCache.Packager.dll";
                file.Source = "RefreshCache.Packager.dll";
                package.Files.Add(file);
                package.MigrationSource = "RefreshCache.Packager.Setup.dll";

                BuildMessageCollection msg = package.Build(Environment.CurrentDirectory);
                package = new Package(package.XmlPackage);
                pdb = new PackageDatabase(di.FullName, con);
                pdb.InstallSystem(package);
            }
            catch
            {
                throw;
            }
        }
    }
}
