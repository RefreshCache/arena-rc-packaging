using RefreshCache.Packager;
using RefreshCache.Packager.Migrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RefreshCache.Packager.Setup
{
    public class Setup : Migration
    {
        [MigratorVersion(1, 0, 0, 1)]
        public class AddTable__cust_rc_packager_packages : DatabaseMigrator
        {
            public override void Upgrade(Database db)
            {
                Table tb;


                tb = new Table("cust_rc_packager_packages");
                tb.Columns.Add(new Column("package_id", ColumnType.Int, ColumnAttribute.PrimaryKeyIdentity));
                tb.Columns.Add(new Column("date_created", ColumnType.DateTime, ColumnAttribute.NotNull));
                tb.Columns[tb.Columns.Count - 1].Default = "GETDATE()";
                tb.Columns.Add(new Column("date_modified", ColumnType.DateTime, ColumnAttribute.NotNull));
                tb.Columns[tb.Columns.Count - 1].Default = "GETDATE()";
                tb.Columns.Add(new Column("created_by", ColumnType.VarChar, 80));
                tb.Columns.Add(new Column("modified_by", ColumnType.VarChar, 80));
                tb.Columns.Add(new Column("name", ColumnType.VarChar, (ColumnAttribute.Unique | ColumnAttribute.NotNull), 80, -1, -1));
                tb.Columns.Add(new Column("package", ColumnType.XML));

                db.CreateTable(tb);
            }

            public override void Downgrade(Database db)
            {
                db.DropTable("cust_rc_packager_packages");
            }
        }

        [MigratorVersion(1, 0, 0, 2)]
        public class AddSP__cust_rc_packager_sp_get_installed_packages : DatabaseMigrator
        {
            public override void Upgrade(Database db)
            {
                db.ExecuteNonQuery("CREATE PROCEDURE [cust_rc_packager_sp_get_installed_packages]" +
                    "AS" +
                    "    SELECT [name],[package].query('(//ArenaPackage/Info/Version)[1]', 'varchar(20)')" +
                    "    FROM [cust_rc_packager_packages]"
                    );
            }

            public override void Downgrade(Database db)
            {
                db.DropProcedure("cust_rc_packager_sp_get_installed_packages");
            }
        }

        [MigratorVersion(1, 0, 0, 3)]
        public class AddSP__cust_rc_packager_sp_get_package_version : DatabaseMigrator
        {
            public override void Upgrade(Database db)
            {
                db.ExecuteNonQuery("CREATE PROCEDURE [cust_rc_packager_sp_get_package_version]" +
                    "    @Name AS varchar(80)" +
                    "AS" +
                    "    SELECT [package].query('(//ArenaPackage/Info/Version)[1]', 'varchar(20)')" +
                    "    FROM [cust_rc_packager_packages]" +
                    "    WHERE [name] = @Name"
                    );
            }

            public override void Downgrade(Database db)
            {
                db.DropProcedure("cust_rc_packager_sp_get_package_version");
            }
        }

        [MigratorVersion(1, 0, 0, 4)]
        public class AddSP__cust_rc_packager_sp_get_package : DatabaseMigrator
        {
            public override void Upgrade(Database db)
            {
                db.ExecuteNonQuery("CREATE PROCEDURE [cust_rc_packager_sp_get_package]" +
                    "    @Name AS varchar(80)" +
                    "AS" +
                    "    SELECT [package] FROM [cust_rc_packager_packages] WHERE [name] = @Name"
                    );
            }

            public override void Downgrade(Database db)
            {
                db.DropProcedure("cust_rc_packager_sp_get_package");
            }
        }

        [MigratorVersion(1, 0, 0, 5)]
        public class AddSP__cust_rc_packager_sp_get_packages_requiring : DatabaseMigrator
        {
            public override void Upgrade(Database db)
            {
                db.ExecuteNonQuery(
@"CREATE PROCEDURE [cust_rc_packager_sp_get_packages_requiring]
    @Name AS varchar(80)
AS
    SELECT [name]
	    FROM [cust_rc_packager_packages]
    	CROSS APPLY [package].nodes('//ArenaPackage/Info/Require') AS P(Req)
	    WHERE @v IN (SELECT P.Req.value('@Name', 'varchar(80)'))");
            }

            public override void Downgrade(Database db)
            {
                db.DropProcedure("cust_rc_packager_sp_get_packages_requiring");
            }
        }

        [MigratorVersion(1, 0, 0, 6)]
        public class AddSP__cust_rc_packager_sp_get_packages_recommending : DatabaseMigrator
        {
            public override void Upgrade(Database db)
            {
                db.ExecuteNonQuery(
@"CREATE PROCEDURE [cust_rc_packager_sp_get_packages_recommending]
    @Name AS varchar(80)
AS
    SELECT [name]
	    FROM [cust_rc_packager_packages]
    	CROSS APPLY [package].nodes('//ArenaPackage/Info/Recommend') AS P(Req)
	    WHERE @v IN (SELECT P.Req.value('@Name', 'varchar(80)'))");
            }

            public override void Downgrade(Database db)
            {
                db.DropProcedure("cust_rc_packager_sp_get_packages_recommending");
            }
        }
    }
}
