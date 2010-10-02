using RefreshCache.Packager;
using RefreshCache.Packager.Manager;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
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

            ExecuteScript("ArenaDB.util_sp_get_databaseVersion.sql");
            ExecuteScript("ArenaDB.port_sp_del_module.sql");
            ExecuteScript("ArenaDB.port_sp_del_module_instance.sql");
            ExecuteScript("ArenaDB.port_sp_del_portal_page.sql");
            ExecuteScript("ArenaDB.port_sp_get_moduleByUrl.sql");
            ExecuteScript("ArenaDB.port_sp_save_module.sql");

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
    }
}
