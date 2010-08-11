﻿using RefreshCache.Packager;
using RefreshCache.Packager.Migrator;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;

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
        private String RootPath;

        private Dictionary<Int32, Int32> ModuleMap;
        private Dictionary<Int32, Int32> PageMap;
        private Dictionary<Int32, Int32> ModuleInstanceMap;

        #endregion


        /// <summary>
        /// Create a new instance of this class with the specified database
        /// connection.
        /// </summary>
        /// <param name="path">The root path to the Arena website, e.g. C:\Program Files\Arena ChMS\Arena.</param>
        /// <param name="connection">The <see cref="SqlConnection"/> object to use for database operations.</param>
        public PackageInstaller(String path, SqlConnection connection)
        {
            RootPath = path;
            if (RootPath[RootPath.Length - 1] == '\\')
                RootPath = RootPath.Substring(0, (RootPath.Length - 1));

            _Connection = connection;
            Command = _Connection.CreateCommand();

            ModuleMap = new Dictionary<int, int>();
            PageMap = new Dictionary<int, int>();
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
        /// Retrieves the Migration instance for the package. If no Migration
        /// class exists in the package, null is returned.
        /// </summary>
        /// <param name="package">The Package to load a migration class from.</param>
        /// <returns>An instance of the Migration subclass or null if none found.</returns>
        private Migration MigrationForPackage(Package package)
        {
            Assembly asm;


            //
            // Test if this package has a Migration assembly embeded.
            //
            if (package.Migration == null)
                return null;

            //
            // Look for the first class in the assembly information that is a
            // direct subclass of the Migration class.
            //
            asm = Assembly.Load(package.Migration);
            foreach (Type t in asm.GetTypes())
            {
                if (t.BaseType == typeof(Migration))
                    return (Migration)asm.CreateInstance(t.FullName);
            }

            return null;
        }


        /// <summary>
        /// Install all the file system changes for the given Package. The changes will
        /// be stored in the changes parameter to allow for a file system rollback
        /// later if an error occurs during the process.
        /// </summary>
        /// <param name="newPackage">The new package that is being installed or upgraded.</param>
        /// <param name="oldPackage">If this is an upgrade operation this will contain the previously installed package. Otherwise this parameter should be null.</param>
        /// <param name="changes">The list of file changes, by reference, that will be updated with all file system changes performed.</param>
        private void InstallPackageFiles(Package newPackage, Package oldPackage, ref List<FileChanges> changes)
        {
            List<String> files = new List<String>();


            //
            // Install all new or updated files.
            //
            foreach (File f in newPackage.AllFiles())
            {
                FileInfo target = new FileInfo(RootPath + @"\" + f.Path);

                changes.Add(new FileChanges(target));
                using (FileStream writer = target.Create())
                {
                    writer.Write(f.Contents, 0, f.Contents.Length);
                    writer.Flush();
                }

                files.Add(f.Path);
            }

            //
            // Remove all files that existed in the old package but don't
            // exist in the new package.
            //
            if (oldPackage != null)
            {
                foreach (File f in oldPackage.AllFiles())
                {
                    if (files.Contains(f.Path))
                        continue;

                    //
                    // File has been removed.
                    //
                    FileInfo target = new FileInfo(RootPath + @"\" + f.Path);
                    changes.Add(new FileChanges(target));
                    target.Delete();
                }
            }
        }


        /// <summary>
        /// Install or update all modules for this package. If a module does
        /// not exist in the old package but exists in the new package it is
        /// created. If a module exists in the old package but does not exist
        /// in the new package, it is removed (and all associated module
        /// instances as well as any pages that become empty due to the
        /// removal). If a module exists in both the old package and the new
        /// package then no action is taken.
        /// </summary>
        /// <param name="package">The new package that is being installed.</param>
        /// <param name="oldPackage">The previous version of the package, or null if this is a new install.</param>
        /// <exception cref="PackageLocalConflictException">Indicates a local module with the same name or url already exists.</exception>
        private void InstallPackageModules(Package package, Package oldPackage)
        {
            //
            // Check for any modules that need to be removed.
            //
            if (oldPackage != null)
            {
                foreach (Module m in oldPackage.Modules)
                {
                    Boolean remove = true;

                    foreach (Module m2 in package.Modules)
                    {
                        if (m2.URL == m.URL)
                        {
                            remove = false;
                            break;
                        }
                    }

                    //
                    // The module has been deleted from the package, remove it from the
                    // database.
                    //
                    if (remove)
                    {
                        SqlDataReader rdr;
                        Int32 module_id;
                        Dictionary<Int32, Int32> module_instances;

                        //
                        // Find the database ID of this old module.
                        //
                        Command.CommandType = CommandType.Text;
                        Command.CommandText = "SELECT [module_id] FROM [port_module] WHERE [module_url] = @ModuleUrl";
                        Command.Parameters.Clear();
                        Command.Parameters.Add(new SqlParameter("@ModuleUrl", m.URL));
                        module_id = Convert.ToInt32(Command.ExecuteScalar());

                        //
                        // Get a list of all module instances that need to be removed,
                        // as well as the pages that might need to be removed.
                        //
                        Command.CommandType = CommandType.Text;
                        Command.CommandText = "SELECT [module_instance_id],[page_id] FROM [port_module_instance] WHERE [module_id] = @ModuleID";
                        Command.Parameters.Clear();
                        Command.Parameters.Add(new SqlParameter("@ModuleID", module_id));
                        rdr = Command.ExecuteReader();
                        module_instances = new Dictionary<int, int>();
                        while (rdr.Read())
                        {
                            module_instances.Add(Convert.ToInt32(rdr["module_instance_id"]),
                                Convert.ToInt32(rdr["page_id"]));
                        }
                        rdr.Close();

                        //
                        // Walk through each module instance, delete it and then check
                        // if the page should be deleted.
                        //
                        foreach (KeyValuePair<Int32, Int32> kvp in module_instances)
                        {
                            Int32 Count;

                            //
                            // Delete the module instance.
                            //
                            Command.CommandType = CommandType.StoredProcedure;
                            Command.CommandText = "port_sp_del_module_instance";
                            Command.Parameters.Clear();
                            Command.Parameters.Add(new SqlParameter("@ModuleInstanceID", kvp.Key));
                            Command.ExecuteNonQuery();

                            //
                            // If the page still has any module instances on it then
                            // we do not want to delete it.
                            //
                            Command.CommandType = CommandType.Text;
                            Command.CommandText = "SELECT COUNT([module_instance_id]) FROM [port_module_instance] WHERE [page_id] = @PageID";
                            Command.Parameters.Clear();
                            Command.Parameters.Add(new SqlParameter("@PageID", kvp.Value));
                            Count = Convert.ToInt32(Command.ExecuteScalar());
                            if (Count > 0)
                                continue;

                            //
                            // If the page has any child pages then we do not want to
                            // delete it.
                            //
                            Command.CommandType = CommandType.Text;
                            Command.CommandText = "SELECT COUNT([page_id]) FROM [port_portal_page] WHERE [parent_page_id] = @PageID";
                            Command.Parameters.Clear();
                            Command.Parameters.Add(new SqlParameter("@PageID", kvp.Value));
                            Count = Convert.ToInt32(Command.ExecuteScalar());
                            if (Count > 0)
                                continue;

                            //
                            // Okay, nuke it. No existing modules and no child pages.
                            //
                            Command.CommandType = CommandType.StoredProcedure;
                            Command.CommandText = "port_sp_del_portal_page";
                            Command.Parameters.Clear();
                            Command.Parameters.Add(new SqlParameter("@PageID", kvp.Value));
                            Command.ExecuteNonQuery();
                        }

                        //
                        // Now delete the module.
                        //
                        Command.CommandType = CommandType.StoredProcedure;
                        Command.CommandText = "port_sp_del_module";
                        Command.Parameters.Clear();
                        Command.Parameters.Add(new SqlParameter("@ModuleID", module_id));
                        Command.ExecuteNonQuery();
                    }
                }
            }

            //
            // Check for any modules that need to be created.
            //
            foreach (Module m in package.Modules)
            {
                Boolean create = true;

                if (oldPackage != null)
                {
                    foreach (Module m2 in oldPackage.Modules)
                    {
                        if (m2.URL == m.URL)
                        {
                            create = false;
                            break;
                        }
                    }
                }

                //
                // The module is new in this version and must be created.
                //
                if (create)
                {
                    //
                    // Check if there is a module with the same name, this would
                    // mean the user has something that will conflict.
                    //
                    Command.CommandType = CommandType.Text;
                    Command.CommandText = "SELECT [module_id] FROM [port_module] WHERE [module_name] = @ModuleName";
                    Command.Parameters.Clear();
                    Command.Parameters.Add(new SqlParameter("@ModuleName", m.Name));
                    if (Command.ExecuteScalar() != null)
                        throw new PackageLocalConflictException(String.Format("There is an existing module with the name '{0}'.", m.Name));

                    //
                    // Check if there is a module with the same url, this would
                    // mean the user has something that will conflict.
                    //
                    Command.CommandType = CommandType.Text;
                    Command.CommandText = "SELECT [module_id] FROM [port_module] WHERE [module_url] = @ModuleUr";
                    Command.Parameters.Clear();
                    Command.Parameters.Add(new SqlParameter("@ModuleUrl", m.URL));
                    if (Command.ExecuteScalar() != null)
                        throw new PackageLocalConflictException(String.Format("There is an existing module with the url '{0}'.", m.URL));

                    //
                    // Create the new module in the database.
                    //
                    Command.CommandType = CommandType.StoredProcedure;
                    Command.CommandText = "port_sp_save_module";
                    Command.Parameters.Clear();
                    Command.Parameters.Add(new SqlParameter("@ModuleId", -1));
                    Command.Parameters.Add(new SqlParameter("@UserId", "PackageInstaller"));
                    Command.Parameters.Add(new SqlParameter("@ModuleName", m.Name));
                    Command.Parameters.Add(new SqlParameter("@ModuleUrl", m.URL));
                    Command.Parameters.Add(new SqlParameter("@ModuleDesc", m.Description));
                    Command.Parameters.Add(new SqlParameter("@AllowsChildModules", m.AllowsChildModules));
                    Command.Parameters.Add(new SqlParameter("@ImagePath", m.ImagePath));
                    Command.Parameters.Add(new SqlParameter(@"ID", null));
                    Command.Parameters[Command.Parameters.Count - 1].Direction = ParameterDirection.Output;
                    Command.ExecuteNonQuery();

                    //
                    // Add the new module ID to the database map.
                    //
                    ModuleMap.Add(m.ModuleID, Convert.ToInt32(Command.Parameters[Command.Parameters.Count - 1].Value));
                }
                else
                {
                    //
                    // Update the database ID map.
                    //
                    Command.CommandType = CommandType.StoredProcedure;
                    Command.CommandText = "port_sp_get_moduleByUrl";
                    Command.Parameters.Clear();
                    Command.Parameters.Add(new SqlParameter("@ModuleUrl", m.URL));
                    SqlDataReader rdr = Command.ExecuteReader();

                    ModuleMap.Add(m.ModuleID, Convert.ToInt32(rdr["module_id"]));
                    rdr.Close();
                }
            }
        }


        /// <summary>
        /// This is a fun one. Create, delete and merge pages. First, go through
        /// and delete any pages that existed in the old package but do not
        /// exist in the new package being installed.
        /// Next we step through each page that exists in both and delete any
        /// module instances that have been removed, update any module instance
        /// settings and add any new module instances to the page.
        /// Finally we add any new pages that have been added since the previous
        /// version.
        /// </summary>
        /// <remarks>
        /// TODO: To do this properly we need a guid of some kind on each
        /// module instance, we can't assume each page will have only one module
        /// type on each page.
        /// Before this method is called the user should be prompted about what
        /// will be modified that the user might have created (user-created child
        /// pages of any deleted page here will also be deleted automatically).
        /// </remarks>
        /// <param name="package">The new package being installed or upgraded.</param>
        /// <param name="oldPackage">The previous version of the package or null if this is a new install.</param>
        private void InstallPackagePages(Package package, Package oldPackage)
        {
            Dictionary<ModuleInstance, ModuleInstance> moduleInstances = new Dictionary<ModuleInstance, ModuleInstance>();
            List<PageInstance> oldPageList = null;


            //
            // Retrieve the list of old pages and reverse the order.
            //
            oldPageList = oldPackage.OrderedPages();
            oldPageList.Reverse();

            //
            // Delete any pages that have been completely removed since the
            // previous version.
            //
            if (oldPackage != null)
            {
                foreach (PageInstance page in oldPageList)
                {
                    Boolean delete = true;

                    //
                    // Check if this page exists in the new package.
                    //
                    foreach (PageInstance p2 in package.OrderedPages())
                    {
                        if (p2.Guid == page.Guid)
                        {
                            delete = false;
                            break;
                        }
                    }

                    //
                    // Delete the page from the system.
                    //
                    if (delete)
                    {
                        Int32 page_id;

                        //
                        // Find the database ID of this old page.
                        //
                        Command.CommandType = CommandType.Text;
                        Command.CommandText = "SELECT [page_id] FROM [port_portal_page] WHERE [guid] = @Guid";
                        Command.Parameters.Clear();
                        Command.Parameters.Add(new SqlParameter("@Guid", page.Guid));
                        page_id = Convert.ToInt32(Command.ExecuteScalar());

                        //
                        // Delete the page from the database.
                        //
                        Command.CommandType = CommandType.StoredProcedure;
                        Command.CommandText = "port_sp_del_portal_page";
                        Command.Parameters.Clear();
                        Command.Parameters.Add(new SqlParameter("@PageID", page_id));
                        Command.ExecuteNonQuery();
                    }
                }
            }

            //
            // Update any pages that exist in both the old package and the new
            // package.
            //
            if (oldPackage != null)
            {
                foreach (PageInstance page in oldPageList)
                {
                    PageInstance NewPage = null;

                    //
                    // Check if this page exists in the new package.
                    //
                    foreach (PageInstance p2 in package.OrderedPages())
                    {
                        if (p2.Guid == page.Guid)
                        {
                            NewPage = p2;
                            break;
                        }
                    }

                    //
                    // Update the page in the system.
                    //
                    if (NewPage != null)
                    {
                        UpdateSinglePage(page, NewPage, ref moduleInstances);
                    }
                }
            }

            //
            // Create any pages that exist in the new package but do not exist
            // in the old package.
            //
            foreach (PageInstance page in package.OrderedPages())
            {
                Boolean create = true;

                if (oldPackage != null)
                {
                    foreach (PageInstance oldPage in oldPageList)
                    {
                        if (oldPage.Guid == page.Guid)
                        {
                            create = false;
                            break;
                        }
                    }
                }

                //
                // If the page did not exist already, create it.
                //
                if (create)
                {
                    CreateSinglePage(page, ref moduleInstances);
                }
            }

            //
            // Set or update all module instance settings now that the page
            // structure has been created.
            //
            // TODO: do this.
        }


        /// <summary>
        /// Create a single page (non-recursively) in the database that did not
        /// exist before.
        /// </summary>
        /// <param name="newPage">The new page information to create with.</param>
        /// <param name="moduleInstances">The module instance collection that will be configured later.</param>
        private void CreateSinglePage(PageInstance newPage, ref Dictionary<ModuleInstance, ModuleInstance> moduleInstances)
        {
            // TODO: do this.

            //
            // Add the new page ID to the database map.
            //
            ModuleMap.Add(newPage.PageID, Convert.ToInt32(Command.Parameters[Command.Parameters.Count - 1].Value));
        }


        /// <summary>
        /// Update a single page in the database to match the new package
        /// information.
        /// </summary>
        /// <param name="oldPage">The old page that is being updated, never null.</param>
        /// <param name="newPage">The new page that should be updated to.</param>
        /// <param name="moduleInstances">The list of module instances that will need their settings updated later.</param>
        private void UpdateSinglePage(PageInstance oldPage, PageInstance newPage, ref Dictionary<ModuleInstance, ModuleInstance> moduleInstances)
        {
            // TODO: do this.

            //
            // Remove any module instances that exist in the old but
            // do not exist in the new page.
            //

            //
            // Update any default module instance settings for module
            // instances that exist in both the old package and the
            // new package.
            //

            //
            // Create any new module instances that do not exist in
            // the old page but exist in the new page.
            //

            //
            // Store the database page_id in our map.
            //
            Command.CommandType = CommandType.Text;
            Command.CommandText = "SELECT [page_id] FROM [port_portal_page] WHERE [guid] = @Guid";
            Command.Parameters.Clear();
            Command.Parameters.Add(new SqlParameter("@Guid", oldPage.Guid));
            PageMap[oldPage.PageID] = Convert.ToInt32(Command.ExecuteScalar());

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
            XmlReader rdr;


            Command.CommandText = "cust_rc_packager_get_installed_package";
            Command.CommandType = CommandType.StoredProcedure;
            Command.Parameters.Clear();
            Command.Parameters.Add(new SqlParameter("@Package", packageName));

            rdr = Command.ExecuteXmlReader();
            xdoc = new XmlDocument();
            xdoc.Load(rdr);
            
            return new Package(xdoc);
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
            List<FileChanges> fileChanges = new List<FileChanges>();
            PackageVersion version;
            Package oldPackage;
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
                oldPackage = GetInstalledPackage(package.Info.PackageName);
                version = (oldPackage != null ? oldPackage.Info.Version : null);

                //
                // Migrate the database to the new version.
                //
                try
                {
                    mig = MigrationForPackage(package);
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
                    InstallPackageFiles(package, oldPackage, ref fileChanges);
                    InstallPackageModules(package, oldPackage);
                    InstallPackagePages(package, oldPackage);
                }
                catch (IOException) { throw; }
                catch (Exception) { throw; }

                //
                // Configure this package first by itself and then for each
                // dependency it has.
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
                try
                {
                }
                catch (Exception e)
                {
                    throw new DatabaseMigrationException("Unable to re-configure dependent packages.", e);
                }

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
                try
                {
                    foreach (FileChanges fc in fileChanges)
                    {
                        try
                        {
                            fc.Restore();
                        }
                        catch { }
                    }
                }
                catch { }

                //
                // Rollback database changes.
                //
                db.RollbackTransaction();
                Command.Transaction = null;

                //
                // Throw the exception again.
                //
                throw;
            }
            //
            // Process:
            // -Verify. (check dependencies, etc.)
            // -Begin Transaction.
            // -Migrate Database of this package to new version.
            // -Install new files (while saving contents of replaced files and locations of newly installed files for undo).
            // -Configure this package (generic run).
            // -Configure this package for each recommendation and requirement.
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


    /// <summary>
    /// Internal class that saves the changes to a single file. This class
    /// also provides a method to restore the file back to it's original
    /// state.
    /// </summary>
    class FileChanges
    {
        /// <summary>
        /// A reference to the information about the original file,
        /// including it's full path.
        /// </summary>
        FileInfo Info { get; set; }

        /// <summary>
        /// The original contents of the file. If the file did not exist
        /// before then this parameter is null.
        /// </summary>
        Byte[] Contents { get; set; }


        /// <summary>
        /// Create a new object instance from the FileInfo object. Also
        /// stores the contents of the original file (if it exists) for
        /// possible restoration later.
        /// </summary>
        /// <param name="original">A FileInfo object that identifies the original file.</param>
        public FileChanges(FileInfo original)
        {
            Info = new FileInfo(original.FullName);
            Info.Refresh();

            if (Info.Exists)
            {
                using (FileStream rdr = Info.OpenRead())
                {
                    Contents = new Byte[rdr.Length];

                    rdr.Read(Contents, 0, (int)rdr.Length);
                }
            }
        }


        /// <summary>
        /// Restore the file identified by this object back to it's original
        /// state. If the file was originally missing then it will be deleted,
        /// it it's contents have been changed then they will be restored.
        /// </summary>
        public void Restore()
        {
            FileInfo target = new FileInfo(Info.FullName);


            if (Contents != null)
            {
                using (FileStream writer = target.Create())
                {
                    writer.Write(Contents, 0, Contents.Length);
                    writer.Flush();
                }
            }
            else
                target.Delete();
        }
    }
}
