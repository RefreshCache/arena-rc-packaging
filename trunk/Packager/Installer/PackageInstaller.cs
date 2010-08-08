using RefreshCache.Packager;
using RefreshCache.Packager.Migrator;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace RefreshCache.Packager.Installer
{
    public class PackageInstaller
    {
        public SqlConnection Connection { get { return _Connection; } }
        private SqlConnection _Connection;


        public PackageInstaller(SqlConnection connection)
        {
            _Connection = connection;
        }


        /// <summary>
        /// Determines if a package is installed or not.
        /// </summary>
        /// <param name="packageName">The name of the package to check the status of.</param>
        /// <returns>True if the package is installed or False is the package is not installed.</returns>
        public Boolean IsPackageInstalled(String packageName)
        {
            return (VersionOfPackage(packageName) != null);
        }


        /// <summary>
        /// Determine if a specified package is installed and if so return
        /// the version number of it.
        /// </summary>
        /// <param name="packageName">The name of the package to retrieve version number for.</param>
        /// <returns>The version number as a String representation.</returns>
        public String VersionOfPackage(String packageName)
        {
            return null;
        }


        public void InstallPackage(Package package)
        {
            //
            // Exceptions:
            // PackageVersionException - Package is already installed with the same or later version.
            // PackageDependencyException - Package has dependencies that have not been met.
            // DatabaseMigrationException - The Database Migration scripts failed to run.
            // IOException - An error occurred while installing files onto the file system.
            // Exception - An unknown error occurred during installation.
            //

            //
            // Process:
            // Begin Transaction.
            // Verify. (check dependencies, etc.)
            // Migrate Databse of this package to new version.
            // Install new files (while saving contents of replaced files and locations of newly installed files for undo).
            // Configure this package (generic run).
            // Configure this package for each recommendation and requirement.
            // Configure all packages that recommend this one.
            // Commit Transaction.
        }


        public void RemovePackage(String packageName)
        {
            //
            // Exceptions:
            // PackageNotInstalledException - Package is not installed.
            // PackageDependencyException - Package has existing dependency packages installed.
            // DatabaseMigrationException - The Database Migration scripts failed to run.
            // Exception - Unknown error occurred.
            //

            //
            // Process:
            // Begin Transaction.
            // Verify. (check dependencies, check for package, etc.)
            // Unconfigure all package that recommend this one.
            // Unconfigure this package for each recommendation and requirement.
            // Unconfigure this package (generic run).
            // Remove all files (while saving contents for restoration in case of process failure).
            // Migrate Database of this package to nothing.
            // Commit Transaction.
            //
        }
    }
}
