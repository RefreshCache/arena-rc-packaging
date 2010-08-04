﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using NUnit.Framework;
using RefreshCache.Migrator;


namespace RefreshCache.Migrator.Tests
{
    [TestFixture]
    public class CoreTests
    {
        Database db;


        [TestFixtureSetUp]
        public void SetUp()
        {
            SqlConnection con;
            String DataSource = "CONSTANTINE\\HDCArena";


            con = new SqlConnection("Data Source=" + DataSource + ";Initial Catalog=tempdb;Integrated Security=SSPI");
            db = new Database(con);
            db.Verbose = true;
            db.BeginTransaction();
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            db.RollbackTransaction();
            db = null;
        }


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
    }
}
