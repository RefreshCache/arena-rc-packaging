using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using RefreshCache.Packager;
using RefreshCache.Packager.Migrator;

namespace Arena.Custom.$safeprojectname$.Setup
{
    public class Setup : Migration
    {
        [MigratorVersion(1, 0, 0, 1)]
        public class AddTable__cust_table_name : DatabaseMigrator
        {
            public override void Upgrade(Database db)
            {
            }

            public override void Downgrade(Database db)
            {
            }
        }
    }
}