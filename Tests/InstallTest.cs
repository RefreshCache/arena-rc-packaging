using RefreshCache.Packager;
using RefreshCache.Packager.Manager;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Xml;
using NUnit.Framework;


namespace RefreshCache.Packager.Tests
{
    [TestFixture]
    public class InstallTest
    {
        SqlConnection testConnection;
        String TempPath = null;
        static String DatabaseName = "RCUnitTestDatabase";


        #region Helper Methods

        private XmlDocument XmlFromResource(String resourceName)
        {
            using (Stream stream = GetType().Assembly.GetManifestResourceStream(GetType(), resourceName))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    XmlDocument xdoc = new XmlDocument();

                    xdoc.Load(reader);

                    return xdoc;
                }
            }
        }

        private void ExecuteScript(String scriptName)
        {
            using (Stream stream = GetType().Assembly.GetManifestResourceStream(GetType(), scriptName))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    ExecuteQuery(reader.ReadToEnd());
                }
            }
        }

        private void ExecuteQuery(String fullQuery)
        {
            SqlCommand command = testConnection.CreateCommand();
            String[] queries;


            queries = fullQuery.Split(new string[] { Environment.NewLine + "GO" }, StringSplitOptions.RemoveEmptyEntries);
            command.CommandType = CommandType.Text;
            foreach (String query in queries)
            {
                command.CommandText = query;
                command.ExecuteNonQuery();
            }
        }

        private String MD5FromInstalledPath(String path)
        {
            StringBuilder sb = new StringBuilder();
            FileInfo fi = new FileInfo(TempPath + @"\" + path);
            byte[] hash;
            MD5 hasher = MD5.Create();
            int i;


            if (fi.Exists == false)
                return null;

            using (Stream stream = fi.OpenRead())
            {
                hash = hasher.ComputeHash(stream);
            }

            for (i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("x2"));
            }

            return sb.ToString();
        }

        private Boolean VerifyFileMD5(String path, String shouldBe)
        {
            String md5 = MD5FromInstalledPath(path);


            if (shouldBe == null)
                return (md5 == null);

            return shouldBe.Equals(md5, StringComparison.OrdinalIgnoreCase);
        }

        private void DumpTable(String tableName)
        {
            SqlCommand command = testConnection.CreateCommand();


            command.CommandType = CommandType.Text;
            command.CommandText = "SELECT * FROM " + tableName;
            using (SqlDataReader rdr = command.ExecuteReader())
            {
                for (int f = 0; f < rdr.FieldCount; f++)
                {
                    if (f > 0)
                        Console.Write(", ");
                    Console.Write(rdr.GetName(f));
                }
                Console.WriteLine("");

                while (rdr.Read())
                {
                    for (int c = 0; c < rdr.FieldCount; c++)
                    {
                        if (c > 0)
                            Console.Write(", ");
                        Console.Write(rdr.GetSqlValue(c).ToString());
                    }
                    Console.WriteLine("");
                }
            }
        }

        private int GetRowCount(String tableName)
        {
            SqlCommand command = testConnection.CreateCommand();


            command.CommandType = CommandType.Text;
            command.CommandText = "SELECT COUNT(*) FROM " + tableName;

            return Convert.ToInt32(command.ExecuteScalar());
        }

        #endregion

        [SetUp]
        public void Initialize()
        {
            SqlCommand command;


            testConnection = new SqlConnection("Data Source=localhost; Integrated Security=SSPI");
            testConnection.Open();
            command = testConnection.CreateCommand();

            //
            // Check if the database exists, if so drop it.
            //
            command.CommandType = CommandType.Text;
            command.CommandText = "SELECT 1 FROM sys.databases WHERE name = '" + DatabaseName + "'";
            if (command.ExecuteScalar() != null)
            {
                command.CommandText = "DROP DATABASE [" + DatabaseName + "]";
                command.ExecuteNonQuery();
            }

            //
            // Create the database.
            //
            command.CommandType = CommandType.Text;
            command.CommandText = "CREATE DATABASE [" + DatabaseName + "]";
            command.ExecuteNonQuery();
            testConnection.ChangeDatabase(DatabaseName);


            ExecuteScript("ArenaDB.port_template.sql");
            ExecuteScript("ArenaDB.port_portal_page.sql");
            ExecuteScript("ArenaDB.port_module.sql");
            ExecuteScript("ArenaDB.port_module_instance.sql");
            ExecuteScript("ArenaDB.port_module_instance_setting.sql");
            ExecuteScript("ArenaDB.secu_template.sql");
            ExecuteScript("ArenaDB.secu_permission.sql");

            ExecuteScript("ArenaDB.util_sp_get_databaseVersion.sql");
            ExecuteScript("ArenaDB.port_sp_del_module.sql");
            ExecuteScript("ArenaDB.port_sp_del_module_instance.sql");
            ExecuteScript("ArenaDB.port_sp_del_portal_page.sql");
            ExecuteScript("ArenaDB.port_sp_get_moduleByUrl.sql");
            ExecuteScript("ArenaDB.port_sp_save_module.sql");

            ExecuteScript("ArenaDB.secu_sp_del_permissionByKey.sql");

            TempPath = Path.GetTempPath() + @"\" + DatabaseName;
            Directory.CreateDirectory(TempPath);
        }

        [TearDown]
        public void Cleanup()
        {
            SqlCommand command = testConnection.CreateCommand();

            testConnection.ChangeDatabase("master");
            command.CommandText = "DROP DATABASE [" + DatabaseName + "]";
            command.ExecuteNonQuery();

            Directory.Delete(TempPath, true);
            TempPath = null;
        }

        [Test]
        public void TestSystemInstall()
        {
            PackageDatabase pdb;
            XmlDocument xdoc;
            Package package1, package2;


            xdoc = new XmlDocument();
            xdoc.Load("../../../RefreshCache.Packager.xml");
            package1 = new Package(xdoc);
            xdoc = new XmlDocument();
            xdoc.Load("../../../../RC.Utilities/Arena.Custom.RC.Utilities.xml");
            package2 = new Package(xdoc);

            pdb = new PackageDatabase(TempPath, testConnection);
            pdb.InstallSystem(new Package[] { package1, package2 });
        }

        [Test]
        public void TestBasicInstall()
        {
            PackageDatabase pdb;
            XmlDocument xdoc;
            Package pkg;


            //
            // Setup the basic system.
            //
            try
            {
                TestSystemInstall();
            }
            catch
            {
                throw new Exception("TestSystemInstall() failed.");
            }

            //
            // Load the package.
            //
            xdoc = XmlFromResource("Packages.BasicInstall-v1.xml");
            pkg = new Package(xdoc);

            pdb = new PackageDatabase(TempPath, testConnection);
            pdb.InstallPackage(pkg);

            Assert.True(VerifyFileMD5("UserControls/Custom/Module1.ascx", "0b274e8b3c034d2ab7eb788390973371"));
            Assert.True(VerifyFileMD5("UserControls/Custom/Module1.ascx.cs", "e4b1022c1dd0cadbd137835bf5de237a"));
            Assert.True(VerifyFileMD5("UserControls/Custom/Module2.ascx", "3d52593be1a10219ecf2d740036efbb1"));
            Assert.True(VerifyFileMD5("UserControls/Custom/Module2.ascx.cs", "c60299922a0bd14612ca61c85577ab6b"));
            Assert.True(VerifyFileMD5("UserControls/Custom/transparent.gif", "e1d016abaf996df707ad64fc74bc61b9"));
            Assert.True(VerifyFileMD5("UserControls/Custom/HoverPopup.js", "69659372724be53044ce250a7db9f4d3"));
            Assert.True(VerifyFileMD5("UserControls/Custom/menuStyle.css", "5ba8d3764429245ea7538d01d88eff04"));
        }

        /// <summary>
        /// Test upgrading a package.
        /// Includes: Removing a file.
        /// Includes: Changing file contents.
        /// Includes: Removing a module.
        /// </summary>
        [Test]
        public void TestBasicUpgrade()
        {
            PackageDatabase pdb;
            XmlDocument xdoc;
            Package pkg;


            //
            // Setup the basic system.
            //
            try
            {
                TestBasicInstall();
            }
            catch
            {
                throw new Exception("TestBasicInstall() failed.");
            }

            //
            // Load the package.
            //
            xdoc = XmlFromResource("Packages.BasicInstall-v2.xml");
            pkg = new Package(xdoc);

            pdb = new PackageDatabase(TempPath, testConnection);
            pdb.InstallPackage(pkg);

            Assert.True(VerifyFileMD5("UserControls/Custom/Module1.ascx", "ce895f981f67c595f345d8745a3b2ebd"));
            Assert.True(VerifyFileMD5("UserControls/Custom/Module1.ascx.cs", "e4b1022c1dd0cadbd137835bf5de237a"));
            Assert.True(VerifyFileMD5("UserControls/Custom/Module2.ascx", "3d52593be1a10219ecf2d740036efbb1"));
            Assert.True(VerifyFileMD5("UserControls/Custom/Module2.ascx.cs", "c60299922a0bd14612ca61c85577ab6b"));
            Assert.True(VerifyFileMD5("UserControls/Custom/transparent.gif", null));
            Assert.True(VerifyFileMD5("UserControls/Custom/HoverPopup.js", "69659372724be53044ce250a7db9f4d3"));
            Assert.True(VerifyFileMD5("UserControls/Custom/menuStyle.css", "5ba8d3764429245ea7538d01d88eff04"));
        }

        /// <summary>
        /// Test removing a package.
        /// Includes: Installing a package.
        /// Includes: Removing a file.
        /// Includes: Removing a module.
        /// Includes: Removing a page.
        /// </summary>
        [Test]
        public void TestBasicRemove()
        {
            PackageDatabase pdb;
            SqlCommand command;
            int page_count, module_count, module_instance_count, module_instance_setting_count;


            //
            // Setup the basic system.
            //
            try
            {
                TestSystemInstall();
            }
            catch
            {
                throw new Exception("TestSystemInstall() failed.");
            }

            page_count = GetRowCount("port_portal_page");
            module_count = GetRowCount("port_module");
            module_instance_count = GetRowCount("port_module_instance");
            module_instance_setting_count = GetRowCount("port_module_instance_setting");
            pdb = new PackageDatabase(TempPath, testConnection);

            //
            // Install the basic package.
            //
            try
            {
                XmlDocument xdoc;
                Package pkg;

                //
                // Load the package.
                //
                xdoc = XmlFromResource("Packages.BasicInstall-v1.xml");
                pkg = new Package(xdoc);
                pdb.InstallPackage(pkg);
            }
            catch
            {
                throw new Exception("Install basic package failed.");
            }

            pdb.RemovePackage("RC.TestBasicInstall");

            //
            // Verify all the files are gone.
            //
            Assert.True(VerifyFileMD5("UserControls/Custom/Module1.ascx", null));
            Assert.True(VerifyFileMD5("UserControls/Custom/Module1.ascx.cs", null));
            Assert.True(VerifyFileMD5("UserControls/Custom/Module2.ascx", null));
            Assert.True(VerifyFileMD5("UserControls/Custom/Module2.ascx.cs", null));
            Assert.True(VerifyFileMD5("UserControls/Custom/transparent.gif", null));
            Assert.True(VerifyFileMD5("UserControls/Custom/HoverPopup.js", null));
            Assert.True(VerifyFileMD5("UserControls/Custom/menuStyle.css", null));

            //
            // Prepare the SqlCommand.
            //
            command = testConnection.CreateCommand();
            command.CommandType = CommandType.Text;

            //
            // Verify all the pages are gone.
            //
            command.CommandText = "SELECT COUNT(*) FROM port_portal_page";
            Assert.AreEqual(page_count, command.ExecuteScalar(), "port_portal_page rowcount is wrong.");

            //
            // Verify all the modules are gone.
            //
            command.CommandText = "SELECT COUNT(*) FROM port_module";
            Assert.AreEqual(module_count, command.ExecuteScalar(), "port_module rowcount is wrong.");

            //
            // Verify all the module instances are gone.
            //
            command.CommandText = "SELECT COUNT(*) FROM port_module_instance";
            Assert.AreEqual(module_instance_count, command.ExecuteScalar(), "port_module_instance rowcount is wrong.");

            //
            // Verify all the module instance settings are gone.
            //
            command.CommandText = "SELECT COUNT(*) FROM port_module_instance_setting";
            Assert.AreEqual(module_instance_setting_count, command.ExecuteScalar(), "port_module_instance_setting rowcount is wrong.");
        }
    }
}
