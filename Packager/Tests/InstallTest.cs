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
        public void Test()
        {
            SqlConnection con;
            String DataSource = "CONSTANTINE\\HDCArena";
            RefreshCache.Packager.File file;
            PackageInstaller installer;
            Package package;


            con = new SqlConnection("Data Source=" + DataSource + ";Initial Catalog=ArenaTestDB;Integrated Security=SSPI");
            con.Open();

            DirectoryInfo di = new DirectoryInfo("C:/Arena/Test");
            di.Create();

            try
            {
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
                installer = new PackageInstaller(di.FullName, con);
                installer.InstallSystemFromPackage(package);
            }
            catch
            {
                di.Delete(true);
                throw;
            }
        }
    }
}
