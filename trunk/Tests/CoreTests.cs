using System;
using System.Text;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using NUnit.Framework;
using RefreshCache.Packager;
using RefreshCache.Packager.Migrator;


namespace RefreshCache.Packager.Tests
{
    [TestFixture]
    public class CoreTests
    {
        Database db;


        [SetUp]
        public void SetUp()
        {
            SqlConnection con;
            String DataSource = "localhost";


            con = new SqlConnection("Data Source=" + DataSource + ";Initial Catalog=tempdb;Integrated Security=SSPI");
            con.Open();
            SqlCommand cmd = con.CreateCommand();
            cmd.Transaction = con.BeginTransaction();
            db = new Database(cmd);
        }

        [TearDown]
        public void TearDown()
        {
            db.Command.Transaction.Rollback();
            db = null;
        }


        #region Version Tests

        [Test]
        public void VersionStepsAreEqual()
        {
            MigratorVersionStep v1, v2;

            v1 = new MigratorVersionStep(1, 3, 7, 6);
            v2 = new MigratorVersionStep(1, 3, 7, 6);

            Assert.AreEqual(v1.Major, 1);
            Assert.AreEqual(v1.Minor, 3);
            Assert.AreEqual(v1.Revision, 7);
            Assert.AreEqual(v1.Step, 6);
            Assert.True((v1.CompareTo(v2) == 0));
        }

        [Test]
        public void VersionStepsAreNotEqual()
        {
            MigratorVersionStep v;

            v = new MigratorVersionStep(1, 3, 7, 6);
            Assert.True((v.CompareTo(new MigratorVersionStep(2, 7, 1, 9)) != 0));
            Assert.True((v.CompareTo(new MigratorVersionStep(1, 7, 1, 9)) != 0));
            Assert.True((v.CompareTo(new MigratorVersionStep(1, 3, 8, 9)) != 0));
            Assert.True((v.CompareTo(new MigratorVersionStep(1, 3, 7, 9)) != 0));
        }

        [Test]
        public void VersionStepToString()
        {
            MigratorVersionStep v;

            v = new MigratorVersionStep(1, 3, 7, 6);
            Assert.True(v.ToString().Equals("1.3.7 Step 6"));
        }

        [Test]
        public void VersionStepGreaterThanVersion()
        {
            MigratorVersionStep v1;
            PackageVersion v2;

            v1 = new MigratorVersionStep(1, 3, 7, 6);
            v2 = new PackageVersion("1.3.7");
            Assert.True((v1.CompareTo(v2) == 1));

            v2 = new PackageVersion("1.3.6");
            Assert.True((v1.CompareTo(v2) == 1));
        }

        [Test]
        public void VersionStepEqualToVersion()
        {
            MigratorVersionStep v1;
            PackageVersion v2;

            v1 = new MigratorVersionStep(1, 3, 6, 0);
            v2 = new PackageVersion("1.3.6");
            Assert.True((v1.CompareTo(v2) == 0));
        }

        [Test]
        public void VersionAttributeConstructor()
        {
            MigratorVersionAttribute v;


            v = new MigratorVersionAttribute(1, 3, 6, 5);
        }

        #endregion


        #region Create Table Tests

        [Test]
        public void CreateTable()
        {
            Table tb = new Table("test_table");


            tb.Columns.Add(new Column("primary_key", ColumnType.Int, ColumnAttribute.PrimaryKeyIdentity));

            db.CreateTable(tb);
            db.DropTable("test_table");
        }

        [Test]
        public void CreateTableWithMultipleColumns()
        {
            Table tb = new Table("test_table");


            tb.Columns.Add(new Column("primary_key", ColumnType.Int, ColumnAttribute.PrimaryKeyIdentity));
            tb.Columns.Add(new Column("column2", ColumnType.VarChar, 20));

            db.CreateTable(tb);
            db.DropTable("test_table");
        }

        [Test]
        public void CreateTableWithForeignKeyConstraint()
        {
            Table tb;


            tb = new Table("test_table1",
                new Column("primary_key", ColumnType.Int, ColumnAttribute.PrimaryKeyIdentity));
            db.CreateTable(tb);

            tb = new Table("test_table2",
                new Column("primary_key", ColumnType.Int, ColumnAttribute.PrimaryKeyIdentity),
                new Column("secondary_key", ColumnType.Int));
            tb.Constraints.Add(new ForeignKeyConstraint("fk_test_table2_secondary_key_test_table1_primary_key", "secondary_key", "test_table1", "primary_key"));
            db.CreateTable(tb);

            db.DropTable("test_table2");
            db.DropTable("test_table1");
        }

        [Test]
        public void CreateTableWithForeignKeyMultipleColumns()
        {
            Table tb;


            tb = new Table("test_table1",
                new Column("primary_key", ColumnType.Int, ColumnAttribute.NotNull),
                new Column("value", ColumnType.Int, ColumnAttribute.NotNull),
                new Column("value_key", ColumnType.VarChar, 20));
            tb.Constraints.Add(new PrimaryKeyConstraint("pk_test_table1", "primary_key", "value"));
            db.CreateTable(tb);

            tb = new Table("test_table2",
                new Column("primary_key", ColumnType.Int, ColumnAttribute.PrimaryKeyIdentity),
                new Column("secondary_key", ColumnType.Int),
                new Column("third_key", ColumnType.Int));
            tb.Constraints.Add(new ForeignKeyConstraint("fk_test_table2_test_table1",
                new List<string> { "secondary_key", "third_key" },
               "test_table1",
               new List<string> { "primary_key", "value" }));
            db.CreateTable(tb);

            db.DropTable("test_table2");
            db.DropTable("test_table1");
        }

        [Test]
        public void CreateTableWithCompoundPrimaryKey()
        {
            Table tb;


            tb = new Table("test_table",
                new Column("first_name", ColumnType.VarChar, 20),
                new Column("last_name", ColumnType.VarChar, 40));
            tb.Constraints.Add(new PrimaryKeyConstraint("pk_test_table", "first_name", "last_name"));
            db.CreateTable(tb);

            db.DropTable("test_table");
        }

        [Test]
        public void CreateTableWithUniqueConstraint()
        {
            Table tb;


            tb = new Table("test_table",
                new Column("first_name", ColumnType.VarChar, 20),
                new Column("last_name", ColumnType.VarChar, 40));
            tb.Constraints.Add(new UniqueConstraint("uq_test_table", "first_name", "last_name"));
            db.CreateTable(tb);

            db.DropTable("test_table");
        }

        [Test]
        public void CreateTableWithUnique()
        {
            Table tb;

            tb = new Table("test_table",
                new Column("column", ColumnType.Int, ColumnAttribute.Unique)
                );
            db.CreateTable(tb);

            db.DropTable("test_table");
        }

        [Test]
        public void CreateTableWithNotNull()
        {
            Table tb;

            tb = new Table("test_table",
                new Column("column", ColumnType.Int, ColumnAttribute.NotNull)
                );
            db.CreateTable(tb);

            db.DropTable("test_table");
        }

        [Test]
        public void CreateTableWithDefault()
        {
            Table tb;

            tb = new Table("test_table",
                new Column("column", ColumnType.VarChar, 20)
                );
            tb.Columns[0].Default = "\'hello\'";
            db.CreateTable(tb);

            db.DropTable("test_table");
        }

        #endregion


        #region Column Tests

        [Test]
        public void CreateTableWithEveryColumnType()
        {
            Table tb;

            tb = new Table("test_table",
                new Column("column1", ColumnType.BigInt),
                new Column("column2", ColumnType.Binary, 100),
                new Column("column3", ColumnType.Bit),
                new Column("column4", ColumnType.Char, 40),
                new Column("column5", ColumnType.Date),
                new Column("column6", ColumnType.DateTime),
                new Column("column7", ColumnType.DateTimeOffset),
                new Column("column8", ColumnType.Decimal, 8, 3),
                new Column("column9", ColumnType.Float),
                new Column("column10", ColumnType.Image),
                new Column("column11", ColumnType.Int),
                new Column("column12", ColumnType.Money),
                new Column("column13", ColumnType.NChar, 40),
                new Column("column14", ColumnType.NText),
                new Column("column15", ColumnType.Numeric, ColumnAttribute.None, -1, 12, -1),
                new Column("column16", ColumnType.NVarChar, 40),
                new Column("column17", ColumnType.Real),
                new Column("column18", ColumnType.SmallDateTime),
                new Column("column19", ColumnType.SmallInt),
                new Column("column20", ColumnType.SmallMoney),
                new Column("column21", ColumnType.Text),
                new Column("column22", ColumnType.Time),
                new Column("column23", ColumnType.TinyInt),
                new Column("column24", ColumnType.UniqueIdentifier),
                new Column("column25", ColumnType.VarBinary, 2000),
                new Column("column26", ColumnType.VarChar, 40),
                new Column("column27", ColumnType.XML)
                );
            db.CreateTable(tb);
            Assert.AreEqual(Enum.GetValues(typeof(ColumnType)).Length, 27);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ColumnWithNullName()
        {
            new Column(null, ColumnType.Int);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ColumnWithEmptyName()
        {
            new Column("", ColumnType.Int);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ColumnWithInvalidType()
        {
            new Column("test", (ColumnType)(-1));
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ColumnWithForcedInvalidType()
        {
            Column c;

            c = new Column("test", ColumnType.Int);
            c.Type = (ColumnType)(-1);
            c.ToString();
        }

        #endregion


        #region Database Tests

        [Test]
        public void DatabaseDryrun()
        {
            Database d;

            d = new Database(null);
            d.ExecuteScalar("SELECT 1");
            Assert.True(d.Dryrun);
        }

        [Test]
        public void DatabaseScalarValue()
        {
            Assert.True(Convert.ToInt32(db.ExecuteScalar("SELECT 1")) == 1);
        }

        [Test]
        public void AddTableColumn()
        {
            Table tb;

            tb = new Table("test_table",
                new Column("column1", ColumnType.Int)
                );
            db.CreateTable(tb);
            db.AddTableColumn("test_table", new Column("column2", ColumnType.Int));

            db.DropTable("test_table");
        }

        [Test]
        public void AddTableConstraint()
        {
            Table tb;

            tb = new Table("test_table",
                new Column("column1", ColumnType.Int, ColumnAttribute.NotNull)
                );
            db.CreateTable(tb);
            db.AddTableConstraint("test_table", new PrimaryKeyConstraint("pk_test_table", "column1"));

            db.DropTable("test_table");
        }

        [Test]
        public void DropTableColumn()
        {
            Table tb;

            tb = new Table("test_table",
                new Column("column1", ColumnType.Int),
                new Column("column2", ColumnType.Int)
                );
            db.CreateTable(tb);
            db.DropTableColumn("test_table", "column1");

            db.DropTable("test_table");
        }

        [Test]
        public void DropTableConstraint()
        {
            Table tb;

            tb = new Table("test_table",
                new Column("column1", ColumnType.Int, ColumnAttribute.NotNull)
                );
            tb.Constraints.Add(new PrimaryKeyConstraint("pk_test_table", "column1"));
            db.CreateTable(tb);
            db.DropTableConstraint("test_table", "pk_test_table");

            db.DropTable("test_table");
        }

        [Test]
        public void DatabaseObjectExists()
        {
            Table tb;

            tb = new Table("test_table",
                new Column("column1", ColumnType.Int)
                );
            db.CreateTable(tb);
            Assert.True(db.ObjectExists("test_table"));
            Assert.True(db.ObjectExists("test_table", "U"));

            db.DropTable("test_table");
        }

        [Test]
        public void DatabaseDropProcedure()
        {
            db.ExecuteNonQuery("CREATE PROCEDURE cust_sp_test_proc AS SELECT 1");
            db.DropProcedure("cust_sp_test_proc");
        }

        [Test]
        public void DatabaseDropFunction()
        {
            db.ExecuteNonQuery("CREATE FUNCTION cust_fn_test_func() RETURNS int AS BEGIN RETURN 1 END");
            db.DropFunction("cust_fn_test_func");
        }

        [Test]
        public void DatabaseVerbose()
        {
            Database d = new Database(null);
            TextWriter stdout;
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);

            d.Verbose = true;
            stdout = Console.Out;
            Console.SetOut(sw);
            try
            {
                d.ExecuteScalar("SELECT 1");
                Assert.AreEqual("SELECT 1" + Environment.NewLine, sb.ToString(), "ExecuteScalar did not output correct value.");
                sb.Length = 0;

                d.ExecuteNonQuery("CREATE");
                Assert.AreEqual("CREATE" + Environment.NewLine, sb.ToString(), "ExecuteNonQuery did not output correct value.");
            }
            finally
            {
                Console.SetOut(stdout);
            }
        }

        #endregion


        #region Migration Tests

        [Test]
        public void RunMigration()
        {
            Migration mig = new TestMigration();

            mig.Upgrade(db, null);
            mig.Configure(db, null, null);
            mig.Configure(db, null, "CCCEV.CheckIn");
            mig.Unconfigure(db, null, "CCCEV.CheckIn");
            mig.Unconfigure(db, null, null);
            mig.Downgrade(db, null);
        }

        [Test]
        public void InvalidMigrator()
        {
            TestMigration.Ignored_Migration im = new TestMigration.Ignored_Migration();

            Assert.IsNull(im.Version);
        }

        [Test]
        [ExpectedException(typeof(Exception))]
        public void DuplicateStepMigrator()
        {
            DuplicateStepMigration dsm = new DuplicateStepMigration();

            dsm.Configure(null, null, null);
        }

        [Test]
        public void EmptyMigration()
        {
            EmptyMigration em = new EmptyMigration();

            em.Configure(null, null, null);
        }

        [Test]
        public void SequencedMigration()
        {
            SequencedMigration sm = new SequencedMigration();
            TextWriter stdout;
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            String expected;

            sm.Verbose = true;
            stdout = Console.Out;
            Console.SetOut(sw);
            try
            {
                expected = "Upgrade Version: 2.0.0 Step 1" + Environment.NewLine +
                    "Upgrade Version: 3.0.0 Step 1" + Environment.NewLine +
                    "Configure Version: 2.0.0 Step 1" + Environment.NewLine +
                    "Configure Version: 3.0.0 Step 1" + Environment.NewLine +
                    "Unconfigure Version: 3.0.0 Step 1" + Environment.NewLine +
                    "Unconfigure Version: 2.0.0 Step 1" + Environment.NewLine +
                    "Downgrade Version: 3.0.0 Step 1" + Environment.NewLine +
                    "Downgrade Version: 2.0.0 Step 1" + Environment.NewLine;
                sm.Upgrade(null, new PackageVersion("1.0.0"));
                sm.Configure(null, new PackageVersion("1.0.0"), null);
                sm.Unconfigure(null, new PackageVersion("1.0.0"), null);
                sm.Downgrade(null, new PackageVersion("1.0.0"));

                Assert.AreEqual(expected, sb.ToString(), "Sequenced migration was not in the proper order.");
            }
            finally
            {
                Console.SetOut(stdout);
            }
        }

        [Test]
        public void MigratorExceptions()
        {
            String msg = "Test Message";
            Exception inner = new Exception("Top Exception");
            DatabaseMigrationException e, e2;


            e = new DatabaseMigrationException();
            e = new DatabaseMigrationException(msg);
            Assert.AreEqual(msg, e.Message);

            e = new DatabaseMigrationException(msg, inner);
            Assert.AreEqual(msg, e.Message);
            Assert.AreEqual(inner.Message, e.InnerException.Message);

            BinaryFormatter formatter = new BinaryFormatter(null, new StreamingContext(StreamingContextStates.File));
            MemoryStream stream = new MemoryStream();
            formatter.Serialize(stream, e);
            stream.Position = 0;
            e2 = (DatabaseMigrationException)formatter.Deserialize(stream);

            Assert.AreEqual(e.Message, e2.Message);
        }

        #endregion


        #region Build Messages

        [Test]
        public void BuildMessageWarningTest()
        {
            BuildMessage bm;


            bm = new BuildMessage(BuildMessageType.Warning, "Test Message");
            Assert.True(bm.ToString().IndexOf("Warning: ") == 0);
        }


        [Test]
        public void BuildMessageErrorTest()
        {
            BuildMessage bm;


            bm = new BuildMessage(BuildMessageType.Error, "Test Message");
            Assert.True(bm.ToString().IndexOf("Error: ") == 0);
        }


        [Test]
        public void BuildMessageCollectionTest()
        {
            BuildMessageCollection bmc = new BuildMessageCollection();
            String msg, wMsg = "This is a warning.", eMsg = "This is an error.";


            bmc.Add(new BuildMessage(BuildMessageType.Warning, wMsg));
            bmc.Add(new BuildMessage(BuildMessageType.Error, eMsg));

            Assert.AreEqual(2, bmc.Count, "Invalid message count.");
            msg = bmc.ToString();

            Assert.IsTrue(msg.IndexOf("Warning: " + wMsg) == 0);
            Assert.IsTrue(msg.IndexOf("Error: " + eMsg) != 0);
        }

        #endregion
    }

    class TestMigration : Migration
    {
        //
        // Version 1.0.0 Step 1
        // Create the check code table.
        //
        [MigratorVersion(1, 0, 0, 1)]
        public class Version1_0_0_step1 : DatabaseMigrator
        {
            public override void Upgrade(Database db)
            {
                db.CreateTable(new Table("cust_hdc_checkin_code",
                                         new Column("person_id", ColumnType.Int, ColumnAttribute.NotNull),
                                         new Column("code", ColumnType.VarChar, 20)
                                         ));
            }

            public override void Downgrade(Database db)
            {
                db.DropTable("cust_hdc_checkin_code");
            }
        }

        //
        // Version 1.0.0 Step 2
        // Add the foreign key constraint.
        //
        [MigratorVersion(1, 0, 0, 2)]
        public class Version1_0_0_step2 : DatabaseMigrator
        {
            public override void Upgrade(Database db)
            {
                db.AddTableConstraint("cust_hdc_checkin_code",
                                      new PrimaryKeyConstraint("pk_cust_hdc_checkin_code_person_id",
                                                               "person_id")
                                      );
            }

            public override void Downgrade(Database db)
            {
                db.DropTableConstraint("cust_hdc_checkin_code", "pk_cust_hdc_checkin_code_person_id");
            }
        }

        //
        // Version 1.2.0 Step 1
        // No database changes were made for this version.
        //
        [MigratorVersion(1, 2, 0, 1)]
        public class Version1_2_0_step1 : DatabaseMigrator
        {
        }

        //
        // Version 1.3.5 Step 1
        // Add column guid to the cust_hdc_checkin_code table.
        // Add a lookup if we have the CCCEV.CheckIn package available.
        //
        [MigratorVersion(1, 3, 5, 1)]
        public class Version1_3_5_step1 : DatabaseMigrator
        {
            public override void Upgrade(Database db)
            {
                db.AddTableColumn("cust_hdc_checkin_code",
                                  new Column("guid", ColumnType.UniqueIdentifier)
                                  );
            }

            public override void Downgrade(Database db)
            {
                db.DropTableColumn("cust_hdc_checkin_code", "guid");
            }

            public override void Configure(Database db, string dependency)
            {
                if (dependency != null && dependency == "CCCEV.CheckIn")
                {
                    db.ExecuteNonQuery("INSERT INTO cust_hdc_checkin_code VALUES (2, 'X913', 'C9D83DC8-9E87-11DF-B968-12FCDED72085')");
                }
            }

            public override void Unconfigure(Database db, string dependency)
            {
                if (dependency != null && dependency == "CCCEV.CheckIn")
                {
                    db.ExecuteNonQuery("DELETE FROM cust_hdc_checkin_code WHERE [guid] = 'C9D83DC8-9E87-11DF-B968-12FCDED72085'");
                }
            }
        }

        public class Ignored_Migration : DatabaseMigrator
        {
        }
    }

    class SequencedMigration : Migration
    {
        [MigratorVersion(1, 0, 0, 1)]
        public class Version1_0_0_step1 : DatabaseMigrator
        {
        }

        [MigratorVersion(2, 0, 0, 1)]
        public class Version2_0_0_step1 : DatabaseMigrator
        {
        }

        [MigratorVersion(3, 0, 0, 1)]
        public class Version3_0_0_step1 : DatabaseMigrator
        {
        }
    }

    class DuplicateStepMigration : Migration
    {
        [MigratorVersion(1, 0, 0, 1)]
        public class Version1_0_0_stepA : DatabaseMigrator
        {
        }

        [MigratorVersion(1, 0, 0, 1)]
        public class Version1_0_0_stepB : DatabaseMigrator
        {
        }
    }

    class EmptyMigration : Migration
    {
        public class SomeOtherClass
        {
        }
    }
}
