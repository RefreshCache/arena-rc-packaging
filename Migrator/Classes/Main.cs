using System;
using System.Collections.Generic;
using System.Data.SqlClient;


namespace RefreshCache.Database
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Table tb;
			ForeignKeyConstraint fk;

			//
			// Setup the database connection in test mode.
			//
			Database db = new Database();
			db.Verbose = true;
			db.Dryrun = true;

			Migration m = new MyMigration();
			m.Upgrade(db, null);
			m.Configure(db, null, null);
			m.Unconfigure(db, null, null);
			m.Downgrade(db, null);
			Console.WriteLine("done");
			Environment.Exit(0);
			

			fk = new ForeignKeyConstraint("test_fk",
			                              new List<String> { "person_id", "family_id" },
										  "core_person",
										  new List<String> { "id", "fam_id" });
			fk.CascadeOnUpdate = true;
			tb = new Table("test",
			               new Column("person_id", ColumnType.Int, ColumnAttribute.PrimaryKeyIdentity) { Default = "'test'" },
			               new Column("family_id", ColumnType.Int));
			tb.Constraints.Add(new PrimaryKeyConstraint("test_pk", "person_id"));
			tb.Constraints.Add(new UniqueConstraint("test_uq_fam", "family_id"));
			tb.Constraints.Add(fk);
			db.CreateTable(tb);
		}
		
		public void TestConnection()
		{
			SqlConnection con;

			
			con = new SqlConnection("Data Source=CONSTANTINE\\HDCArena;Initial Catalog=tempdb;Integrated Security=SSPI");
			con.Open();
			SqlCommand cmd = con.CreateCommand();
			cmd.CommandText = "SELECT * from sysobjects";
			SqlDataReader reader = cmd.ExecuteReader();
			while (reader.Read())
			{
				Console.WriteLine("Result: " + reader[0].ToString());
			}
		}
	}
	
	

	
	class MyMigration : Migration
	{
		//
		// Version 1.0.0 Step 1
		// Create the check code table.
		//
		[MigratorVersionAttribute(1, 0, 0, 1)]
		public class Version1_0_0_step1 : DatabaseMigrator
		{
			public override void Upgrade (Database db)
			{
				db.CreateTable(new Table("cust_hdc_checkin_code",
				                         new Column("person_id", ColumnType.Int),
				                         new Column("code", ColumnType.VarChar, 20)
				                         ));
			}
			
			public override void Downgrade (Database db)
			{
				db.DropTable("cust_hdc_checkin_code");
			}
		}

		//
		// Version 1.0.0 Step 2
		// Add the foreign key constraint.
		//
		[MigratorVersionAttribute(1, 0, 0, 2)]
		public class Version1_0_0_step2 : DatabaseMigrator
		{
			public override void Upgrade (Database db)
			{
				db.AddTableConstraint("cust_hdc_checkin_code",
				                      new ForeignKeyConstraint("fk_cust_hdc_checkin_code_person_id",
				                                               "person_id",
				                                               "core_person",
				                                               "person_id"
				                                               ));
			}
			
			public override void Downgrade (Database db)
			{
				db.DropTableConstraint("cust_hdc_checkin_code", "fk_cust_hdc_checkin_code_person_id");
			}
		}
		
		//
		// Version 1.2.0 Step 1
		// No database changes were made for this version.
		//
		[MigratorVersionAttribute(1, 2, 0, 1)]
		public class Version1_2_0_step1 : DatabaseMigrator
		{
		}

		//
		// Version 1.3.5 Step 1
		// Add column guid to the cust_hdc_checkin_code table.
		// Add a lookup if we have the CCCEV.CheckIn package available.
		//
		[MigratorVersionAttribute(1, 3, 5, 1)]
		public class Version1_3_5_step1 : DatabaseMigrator
		{
			public override void Upgrade (Database db)
			{
				db.AddTableColumn("cust_hdc_checkin_code",
				                  new Column("guid", ColumnType.UniqueIdentifier)
				                  );
			}
			
			public override void Downgrade (Database db)
			{
				db.DropTableColumn("cust_hdc_checkin_code", "guid");
			}
			
			public override void Configure (Database db, string dependency)
			{
				if (dependency != null && dependency == "CCCEV.CheckIn")
				{
					db.ExecuteNonQuery("INSERT INTO core_lookup VALUES (x, y, z, 'C9D83DC8-9E87-11DF-B968-12FCDED72085')");
				}
			}
			
			public override void Unconfigure (Database db, string dependency)
			{
				if (dependency != null && dependency == "CCCEV.CheckIn")
				{
					db.ExecuteNonQuery("DELETE FROM core_lookup WHERE [guid] = 'C9D83DC8-9E87-11DF-B968-12FCDED72085'");
				}
			}
		}
	}
}

