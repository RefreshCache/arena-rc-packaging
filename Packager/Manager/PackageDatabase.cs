﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Text;
using System.Xml;
using RefreshCache.Packager.Migrator;


namespace RefreshCache.Packager.Manager
{
    /// <summary>
    /// Primary interface for user interface elements to interact with the Package
    /// Management system.
    /// </summary>
    public class PackageDatabase
    {
        #region Properties

        /// <summary>
        /// The SqlConnection that this instance will use when performing database
        /// operations.
        /// </summary>
        public SqlConnection Connection { get { return _Connection; } }
        private SqlConnection _Connection;

        /// <summary>
        /// The SqlCommand that is being used by this instance to talk to the
        /// SQL database.
        /// </summary>
        internal SqlCommand Command;

        /// <summary>
        /// The path to the Arena installation. This is the folder that contains
        /// the web.config file.
        /// </summary>
        internal String RootPath;

        #endregion


        #region Constructors

        /// <summary>
        /// Create a new instance of this class with the specified database
        /// connection.
        /// </summary>
        /// <param name="path">The root path to the Arena website, e.g. C:\Program Files\Arena ChMS\Arena.</param>
        /// <param name="connection">The <see cref="SqlConnection"/> object to use for database operations.</param>
        public PackageDatabase(String path, SqlConnection connection)
        {
            RootPath = path;
            if (RootPath[RootPath.Length - 1] == '\\')
                RootPath = RootPath.Substring(0, (RootPath.Length - 1));

            _Connection = connection;
            Command = _Connection.CreateCommand();
        }

        #endregion


        /// <summary>
        /// Determines if the Packager system has been installed on this
        /// database yet.
        /// </summary>
        /// <returns>A Boolean value of true or false indicating if the system is installed.</returns>
        public Boolean IsSystemInstalled()
        {
            Command.CommandType = CommandType.Text;
            Command.CommandText = "SELECT name FROM sys.objects WHERE name = N'cust_rc_packager_sp_get_installed_packages'";

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
        /// <returns>The version number of the package or null if the package was not found.</returns>
        public PackageVersion VersionOfPackage(String packageName)
        {
            Object result;


            Command.CommandType = CommandType.StoredProcedure;
            Command.CommandText = "cust_rc_packager_sp_get_package_version";
            Command.Parameters.Clear();
            Command.Parameters.Add(new SqlParameter("@Name", packageName));

            result = Command.ExecuteScalar();
            if (result == null)
                return null;

            return new PackageVersion(result.ToString());
        }


        /// <summary>
        /// Determines the version of Arena that is installed in the database.
        /// </summary>
        /// <returns>A PackageVersion that identifies the Arena version.</returns>
        public PackageVersion VersionOfArena()
        {
            Command.CommandType = CommandType.StoredProcedure;
            Command.CommandText = "util_sp_get_databaseVersion";
            Command.Parameters.Clear();

            return new PackageVersion(Command.ExecuteScalar().ToString());
        }


        /// <summary>
        /// Retrieves a list of package names that are installed in
        /// the Arena system.
        /// </summary>
        /// <returns>The list of package names that are installed.</returns>
        public List<String> InstalledPackages()
        {
            List<String> packages = new List<String>();
            SqlDataReader rdr;


            Command.CommandType = CommandType.StoredProcedure;
            Command.CommandText = "cust_rc_packager_sp_get_installed_packages";
            Command.Parameters.Clear();
            rdr = Command.ExecuteReader();

            while (rdr.Read())
            {
                packages.Add(rdr[0].ToString());
            }
            rdr.Close();

            return packages;
        }


        /// <summary>
        /// Retrieve the stored package information from the database for an
        /// installed package. Each package that is installed is stored in the
        /// database for later use, this retrieves that data.
        /// </summary>
        /// <param name="packageName">The name of the package to retrieve.</param>
        /// <returns>Package object identified by the packageName, or null if the package is not installed.</returns>
        public Package GetInstalledPackage(String packageName)
        {
            XmlDocument xdoc;
            Package pkg = null;


            Command.CommandText = "cust_rc_packager_sp_get_package";
            Command.CommandType = CommandType.StoredProcedure;
            Command.Parameters.Clear();
            Command.Parameters.Add(new SqlParameter("@Name", packageName));

            using (XmlReader rdr = Command.ExecuteXmlReader())
            {
                xdoc = new XmlDocument();
                xdoc.Load(rdr);

                pkg = new Package(xdoc);
            }

            return pkg;
        }


        /// <summary>
        /// Retrieves a list of package names that recommend the indicated
        /// package for use. This does not include packages that require
        /// the indicated package.
        /// </summary>
        /// <param name="packageName">The name of the package to query against.</param>
        /// <returns>The list of package names that recommend the named package.</returns>
        public List<String> PackagesRecommending(String packageName)
        {
            List<String> packages = new List<String>();
            SqlDataReader rdr;


            Command.CommandType = CommandType.StoredProcedure;
            Command.CommandText = "cust_rc_packager_sp_get_packages_recommending";
            Command.Parameters.Clear();
            Command.Parameters.Add(new SqlParameter("@Name", packageName));
            rdr = Command.ExecuteReader();

            while (rdr.Read())
            {
                packages.Add(rdr[0].ToString());
            }
            rdr.Close();

            return packages;
        }


        /// <summary>
        /// Retrieves a list of installed package names that require the
        /// indicated package in order to function. This does not include
        /// packages that recommend the indicated package.
        /// </summary>
        /// <param name="packageName">The name of the package to query against.</param>
        /// <returns>The list of package names that require the named package.</returns>
        public List<String> PackagesRequiring(String packageName)
        {
            List<String> packages = new List<String>();
            SqlDataReader rdr;


            Command.CommandType = CommandType.StoredProcedure;
            Command.CommandText = "cust_rc_packager_sp_get_packages_requiring";
            Command.Parameters.Clear();
            Command.Parameters.Add(new SqlParameter("@Name", packageName));
            rdr = Command.ExecuteReader();

            while (rdr.Read())
            {
                packages.Add(rdr[0].ToString());
            }
            rdr.Close();

            return packages;
        }


        #region Package Installation, Upgrade and Removal


        /// <summary>
        /// Install the Package Management system into the file system and the database.
        /// </summary>
        /// <param name="packages">The package to be used for the install, this must be a system install package.</param>
        public void InstallSystem(Package[] packages)
        {
            int i;


            //
            // Make sure the system is not already installed.
            //
            if (IsSystemInstalled())
                throw new InvalidOperationException("The Package Management system is already installed.");

            //
            // Verify that the first package is a system package.
            //
            if (packages[0].Info.PackageName != "RC.PackageManager")
                throw new InvalidOperationException("System package is not the first package to install.");

            //
            // Install each package in turn.
            //
            for (i = 0; i < packages.Length; i++)
            {
                try
                {
                    //
                    // Try to install the package.
                    //
                    InstallPackage(packages[i], true);
                }
                catch
                {
                    //
                    // If any package install fails, walk backwards through
                    // the list of packages that did install and un-install
                    // them.
                    //
                    for (--i; i >= 0; i--)
                    {
                        try
                        {
                            RemovePackage(packages[i].Info.PackageName, true);
                        }
                        catch { }
                    }

                    throw;
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
            InstallPackage(package, false);
        }


        /// <summary>
        /// Do the actual leg-work of installing a package.
        /// </summary>
        /// <param name="package">The package to be installed or upgraded.</param>
        /// <param name="systemInstall">Specifies if this is a system install. If true certain safety checks are bypassed.</param>
        private void InstallPackage(Package package, Boolean systemInstall)
        {
            PackageInstaller installer = new PackageInstaller(this);
            PackageVersion version = null;
            Package oldPackage = null;
            Database db;
            Migration mig;
            Boolean LocalTransaction = false;


            //
            // Verify package dependencies.
            //
            try
            {
                if (systemInstall == false)
                    installer.VerifyDependenciesForInstall(package);
            }
            catch (PackageVersionException) { throw; }
            catch (PackageDependencyException) { throw; }

            //
            // Begin the SQL Transaction.
            //
            if (Command.Transaction == null)
            {
                Command.Transaction = Connection.BeginTransaction();
                LocalTransaction = true;
            }
            db = new Database(Command);

            //
            // Begin the install process.
            //
            try
            {
                if (systemInstall == false)
                {
                    //
                    // Get the previous package and the previous package version.
                    //
                    oldPackage = GetInstalledPackage(package.Info.PackageName);
                    version = (oldPackage != null ? oldPackage.Info.Version : null);
                }

                //
                // Migrate the database to the new version.
                //
                try
                {
                    mig = installer.MigrationForPackage(package);
                    if (mig != null)
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
                    installer.InstallPackageFiles(package, oldPackage);
                    installer.InstallPackageModules(package, oldPackage);
                    installer.InstallPackagePages(package, oldPackage);
                }
                catch (IOException) { throw; }
                catch (Exception) { throw; }

                //
                // Configure this package first by itself and then for each
                // dependency it has.
                //
                if (mig != null)
                {
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
                }

                //
                // Configure all packages that recommend this package.
                //
                try
                {
                    foreach (String dependantName in PackagesRecommending(package.Info.PackageName))
                    {
                        Migration mi2 = installer.MigrationForPackage(GetInstalledPackage(dependantName));

                        if (mi2 != null)
                        {
                            mi2.Configure(db, null, package.Info.PackageName);
                        }
                    }
                }
                catch (Exception e)
                {
                    throw new DatabaseMigrationException("Unable to re-configure dependent packages.", e);
                }

                //
                // Store the Package information in the database.
                //
                if (oldPackage != null)
                {
                    Command.CommandType = CommandType.Text;
                    Command.CommandText = "DELETE FROM [cust_rc_packager_packages] WHERE [name] = @Name";
                    Command.Parameters.Clear();
                    Command.Parameters.Add(new SqlParameter("@Name", package.Info.PackageName));
                    Command.ExecuteNonQuery();
                }
                Command.CommandType = CommandType.Text;
                Command.CommandText = "INSERT INTO [cust_rc_packager_packages] ([created_by], [modified_by], [name], [package]) VALUES ('Package Manager', 'Package Manager', @Name, @Package)";
                Command.Parameters.Clear();
                Command.Parameters.Add(new SqlParameter("@Name", package.Info.PackageName));
                Command.Parameters.Add(new SqlParameter("@Package", new SqlXml(new XmlNodeReader(package.XmlPackage))));
                Command.ExecuteNonQuery();

                //
                // Commit database changes, we are all done.
                //
                if (LocalTransaction)
                {
                    Command.Transaction.Commit();
                    Command.Transaction = null;
                    db.Command.Transaction = null;
                }
            }
            catch (Exception e)
            {
                //
                // Rollback file system changes.
                //
                try
                {
                    installer.RevertFileChanges();
                }
                catch { }

                //
                // Rollback database changes.
                //
                if (LocalTransaction)
                {
                    Command.Transaction.Rollback();
                    Command.Transaction = null;
                    db.Command.Transaction = null;
                }

                //
                // Throw the exception again.
                //
                throw e;
            }
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
            RemovePackage(packageName, false);
        }


        /// <summary>
        /// Do the actual leg-work of removing a package. Optionally make it
        /// a forceful removal (used during a failed system install, or possibly
        /// a future system un-install). A forceful removal means that dependency
        /// checks are ignored and an error while deleting the database record
        /// is ignored (since the table may no longer exist).
        /// </summary>
        /// <param name="packageName">The name of the package to remove.</param>
        /// <param name="forceRemove">Wether or not to force the removal.</param>
        private void RemovePackage(String packageName, Boolean forceRemove)
        {
            PackageInstaller installer = new PackageInstaller(this);
            Database db;
            Package package;
            Migration mig;
            Boolean LocalTransaction = false;


            //
            // Check if the package is installed.
            //
            package = GetInstalledPackage(packageName);
            if (package == null)
                throw new PackageNotInstalledException(String.Format("The package {0} is not currently installed.", packageName));

            //
            // Check if there are any packages that depend on the package
            // we are about to remove.
            //
            if (forceRemove == false)
            {
                if (PackagesRequiring(packageName).Count > 0)
                    throw new PackageDependencyException(String.Format("The package {0} as dependencies and cannot be removed.", packageName));
            }

            //
            // Begin the SQL Transaction.
            //
            if (Command.Transaction == null)
            {
                Command.Transaction = Connection.BeginTransaction();
                LocalTransaction = true;
            }
            db = new Database(Command);


            //
            // Begin the actual removal process.
            //
            try
            {
                //
                // Unconfigure all packages that recommend this one.
                //
                try
                {
                    foreach (String name in PackagesRecommending(packageName))
                    {
                        mig = installer.MigrationForPackage(GetInstalledPackage(name));
                        if (mig != null)
                            mig.Unconfigure(db, null, packageName);
                    }
                }
                catch (Exception e)
                {
                    throw new DatabaseMigrationException("Unable to re-configure dependent packages.", e);
                }

                //
                // Unconfigure this package.
                //
                try
                {
                    //
                    // Load the migration object from the package.
                    //
                    mig = installer.MigrationForPackage(package);

                    if (mig != null)
                    {
                        //
                        // Unconfigure this package from its recommendations.
                        //
                        foreach (PackageRecommendation pkg in package.Info.Recommends)
                        {
                            mig.Unconfigure(db, null, pkg.Name);
                        }

                        //
                        // Unconfigure this package from its requirements.
                        //
                        foreach (PackageRequirement pkg in package.Info.Requires)
                        {
                            mig.Unconfigure(db, null, pkg.Name);
                        }

                        //
                        // Do a general unconfigure of this package.
                        //
                        mig.Unconfigure(db, null, null);
                    }
                }
                catch (Exception e)
                {
                    throw new DatabaseMigrationException("Unable to unconfigure package.", e);
                }

                //
                // Remove all the new files, pages, modules, etc.
                //
                try
                {
                    installer.RemovePackagePages(package);
                    installer.RemovePackageModules(package);
                    installer.RemovePackageFiles(package);
                }
                catch (IOException) { throw; }
                catch (Exception) { throw; }

                //
                // Migrate the database to a completely non-existant state.
                //
                if (mig != null)
                {
                    try
                    {
                        mig.Downgrade(db, null);
                    }
                    catch (Exception e)
                    {
                        throw new DatabaseMigrationException("Error while trying to migrate database.", e);
                    }
                }

                //
                // Remove the package from the database.
                //
                try
                {
                    Command.CommandType = CommandType.Text;
                    Command.CommandText = "DELETE FROM [cust_rc_packager_packages] WHERE [name] = @Name";
                    Command.Parameters.Clear();
                    Command.Parameters.Add(new SqlParameter("@Name", package.Info.PackageName));
                    Command.ExecuteNonQuery();
                }
                catch
                {
                    if (forceRemove == false)
                        throw;
                }

                //
                // Commit database changes, we are all done.
                //
                if (LocalTransaction)
                {
                    Command.Transaction.Commit();
                    Command.Transaction = null;
                    db.Command.Transaction = null;
                }
            }
            catch (Exception)
            {
                //
                // Rollback file system changes.
                //
                try
                {
                    installer.RevertFileChanges();
                }
                catch { }

                //
                // Rollback database changes.
                //
                if (LocalTransaction)
                {
                    Command.Transaction.Rollback();
                    Command.Transaction = null;
                    db.Command.Transaction = null;
                }

                //
                // Throw the exception again.
                //
                throw;
            }
        }


        #endregion
    }
}
