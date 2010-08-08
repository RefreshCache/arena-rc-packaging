using RefreshCache.Packager;
using RefreshCache.Packager.Migrator;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;

namespace RefreshCache.Packager.Installer
{
    /// <summary>
    /// This class provides all the interface methods for performing installation
    /// related tasks with packages. A package can be queried, installed, upgraded
    /// or removed via this class.
    /// </summary>
    public class PackageInstaller
    {
        #region Properties

        /// <summary>
        /// The SqlConnection that this instance will use when performing database
        /// operations.
        /// </summary>
        public SqlConnection Connection { get { return _Connection; } }
        private SqlConnection _Connection;

        private SqlCommand Command;

        #endregion


        /// <summary>
        /// Create a new instance of this class with the specified database
        /// connection.
        /// </summary>
        /// <param name="connection">The <see cref="SqlConnection"/> object to use for database operations.</param>
        public PackageInstaller(SqlConnection connection)
        {
            _Connection = connection;
            Command = _Connection.CreateCommand();
        }


        /// <summary>
        /// Determines if the Packager system has been installed on this
        /// database yet.
        /// </summary>
        /// <returns>A Boolean value of true or false indicating if the system is installed.</returns>
        private Boolean IsSystemInstalled()
        {
            Command.CommandType = CommandType.Text;
            Command.CommandText = "SELECT name FROM sys.objects WHERE name = N'cust_rc_installed_packages'";

            return (Command.ExecuteScalar() != null);
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
        public PackageVersion VersionOfPackage(String packageName)
        {
            Object result;


            Command.CommandType = CommandType.StoredProcedure;
            Command.CommandText = "cust_rc_packager_get_package_version";
            Command.Parameters.Clear();
            Command.Parameters.Add(new SqlParameter("@Package", packageName));

            result = Command.ExecuteScalar();
            if (result == null)
                return null;

            return new PackageVersion(result.ToString());
        }


        /// <summary>
        /// Determines the version of Arena that is installed in the database.
        /// </summary>
        /// <returns>A PackageVersion that identifies the Arena version.</returns>
        private PackageVersion VersionOfArena()
        {
            Command.CommandType = CommandType.StoredProcedure;
            Command.CommandText = "util_sp_get_databaseVersion";
            Command.Parameters.Clear();

            return new PackageVersion(Command.ExecuteScalar().ToString());
        }


        /// <summary>
        /// Verify all dependencies of the package for an install or upgrade
        /// operation.
        /// </summary>
        /// <param name="package">The package that is about to be installed or upgraded to.</param>
        /// <exception cref="PackageVersionException">Package is already installed with the same version or a later version number.</exception>
        /// <exception cref="PackageDependencyException">Package has dependencies that have not been met.</exception>
        private void VerifyDependenciesForInstall(Package package)
        {
            PackageVersion version;


            //
            // Verify the package version
            //
            try
            {
                version = VersionOfPackage(package.Info.PackageName);
            }
            catch
            {
                version = new PackageVersion("");
            }
            if (version.CompareTo(package.Info.Version) >= 0)
            {
                throw new PackageVersionException(String.Format(
                    "Version {0} of the package {1} is already installed.",
                    version.ToString(), package.Info.PackageName));
            }

            //
            // Verify Arena version.
            //
            try
            {
                version = VersionOfArena();
            }
            catch
            {
                version = new PackageVersion("");
            }
            try
            {
                package.Info.Arena.ValidateVersion(version, "Arena");
            }
            catch (PackageDependencyException) { throw; }

            //
            // Verify each required package meets version requirements.
            //
            foreach (PackageRequirement req in package.Info.Requires)
            {
                version = VersionOfPackage(req.Name);
                if (version == null)
                {
                    throw new PackageDependencyException(String.Format(
                        "Package {0} is required but not installed.", req.Name));
                }
                else
                {
                    try
                    {
                        req.ValidateVersion(version, String.Format("Installed package {0}", req.Name));
                    }
                    catch (PackageDependencyException) { throw; }
                }
            }
        }


        /// <summary>
        /// Install or upgrade a package into the system. The entire process
        /// is done inside of a transaction state so if an error occures the
        /// system should be left in the state it was before the process
        /// began.
        /// </summary>
        /// <param name="package">The <see cref="Package"/> to be installed or upgraded.</param>
        /// <exception cref="PackageVersionException">Package is already installed with the same version or a later version number.</exception>
        /// <exception cref="PackageDependencyException">Package has dependencies that have not been met.</exception>
        /// <exception cref="DatabaseMigrationException">The Database Migration scripts for this package failed to run.</exception>
        /// <exception cref="IOException">An error occurred while installing files onto the file system.</exception>
        /// <exception cref="Exception">An unknown error occurred during installation.</exception>
        public void InstallPackage(Package package)
        {
            PackageVersion version;
            Database db;
            Migration mig;

            
            //
            // Verify package dependencies.
            //
            try
            {
                VerifyDependenciesForInstall(package);
            }
            catch (PackageVersionException) { throw; }
            catch (PackageDependencyException) { throw; }

            //
            // Begin the SQL Transaction.
            //
            db = new Database(Connection);
            db.BeginTransaction();
            Command.Transaction = db.dbTransaction;
            
            //
            // Begin the install process.
            //
            try
            {
                version = VersionOfPackage(package.Info.PackageName);

                //
                // Migrate the database to the new version.
                //
                try
                {
                    //
                    // Load the Migrator.
                    //
                    mig = null;

                    //
                    // Run the upgrade.
                    //
                    mig.Upgrade(db, version);
                }
                catch (Exception e)
                {
                    throw new DatabaseMigrationException("Unable to install the database changes.", e);
                }

                //
                // Install all the new files, pages, modules, etc.
                //
                try
                {
                    //package.InstallInDatabase(db, "C:\\Program Files (x86)\\Arena ChMS\\Arena", out fileChanges);
                }
                catch (IOException) { throw; }
                catch (Exception) { throw; }

                //
                // Configure this package.
                //
                try
                {
                    mig.Configure(db, version, null);

                    foreach (PackageRequirement pkg in package.Info.Requires)
                    {
                        mig.Configure(db, version, pkg.Name);
                    }

                    foreach (PackageRecommendation pkg in package.Info.Recommends)
                    {
                        mig.Configure(db, version, pkg.Name);
                    }
                }
                catch (Exception e)
                {
                    throw new DatabaseMigrationException("Unable to configure the database changes.", e);
                }

                //
                // Configure all packages that depend on this package.
                //

                //
                // Commit database changes, we are all done.
                //
                db.CommitTransaction();
            }
            catch (Exception)
            {
                //
                // Rollback file system changes.
                //

                //
                // Rollback database changes.
                //
                db.RollbackTransaction();
                Command.Transaction = null;
            }
            //
            // Process:
            // -Verify. (check dependencies, etc.)
            // -Begin Transaction.
            // /Migrate Database of this package to new version.
            // /Install new files (while saving contents of replaced files and locations of newly installed files for undo).
            // /Configure this package (generic run).
            // /Configure this package for each recommendation and requirement.
            // /Configure all packages that recommend this one.
            // -Commit Transaction.
        }


        /// <summary>
        /// Remove the specified package from the system. The entire process is
        /// performed inside of a transactional state so if an error occurs the
        /// process is reversed before returning.
        /// </summary>
        /// <param name="packageName">The name of the package to be removed.</param>
        /// <exception cref="PackageNotInstalledException">Specified package is not installed.</exception>
        /// <exception cref="PackageDependencyException">Existing packages are installed that depend on this package.</exception>
        /// <exception cref="DatabaseMigrationException">The Database Migration scripts have failed to run.</exception>
        /// <exception cref="IOException">A file system error occurred preventing files from being removed.</exception>
        /// <exception cref="Exception">An unknown exception occurred during package removal.</exception>
        public void RemovePackage(String packageName)
        {
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
