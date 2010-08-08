using RefreshCache.Packager;
using RefreshCache.Packager.Migrator;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace RefreshCache.Packager.Installer
{
    class PackageInstaller
    {
        public SqlConnection Connection { get { return _Connection; } }
        private SqlConnection _Connection;


        public PackageInstaller(SqlConnection connection)
        {
            _Connection = connection;
        }
    }
}
