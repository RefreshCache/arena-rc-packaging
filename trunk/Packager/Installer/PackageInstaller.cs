using RefreshCache.Packager;
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
        private void InstallPackageFiles(Package newPackage, Package oldPackage, ref List<FileChange> changes)
        {
            List<String> files = new List<String>();


            //
            // Install all new or updated files.
            //
            foreach (File f in newPackage.AllFiles())
            {
                FileInfo target = new FileInfo(RootPath + @"\" + f.Path);

                changes.Add(new FileChange(target));
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
                    changes.Add(new FileChange(target));
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
                    Command.CommandText = "SELECT [module_id] FROM [port_module] WHERE [module_url] = @ModuleUrl";
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
            UpdateAllModuleInstanceSettings(moduleInstances);
        }


        /// <summary>
        /// Create a single page (non-recursively) in the database that did not
        /// exist before.
        /// </summary>
        /// <param name="newPage">The new page information to create with.</param>
        /// <param name="moduleInstances">The module instance collection that will be configured later.</param>
        private void CreateSinglePage(PageInstance newPage, ref Dictionary<ModuleInstance, ModuleInstance> moduleInstances)
        {
            Int32 parent_page_id, template_id;


            //
            // Determine the parent page and the template from that
            // parent page.
            //
            if (newPage.ParentPage != null)
            {
                parent_page_id = PageMap[newPage.ParentPage.PageID];

                Command.CommandType = CommandType.Text;
                Command.CommandText = "SELECT [template_id] FROM [port_portal_page] WHERE [page_id] = " + parent_page_id.ToString();
                Command.Parameters.Clear();
                template_id = Convert.ToInt32(Command.ExecuteScalar());
            }
            else
            {
                parent_page_id = -1;
                template_id = 1;
            }

            //
            // Execute the stored procedure to create the portal page.
            //
            Command.CommandType = CommandType.StoredProcedure;
            Command.CommandText = "INSERT INTO [port_portal_page] (" +
                "[created_by], [modified_by], [template_id], [parent_page_id]" +
                ", [page_order], [display_in_nav], [page_name], [page_desc]" +
                ", [page_settings], [require_ssl], [guid], [validate_request])" +
                " VALUES ('PackageInstaller', 'PackageInstaller', @TemplateID, @ParentPageID" +
                ", 2147483647, @DisplayInNav, @PageName, @PageDesc" +
                ", @PageSettings, @RequireSSL, @Guid, @ValidateRequest);" +
                " SELECT CAST(IDENT_CURRENT('port_portal_page') AS int)";
            Command.Parameters.Clear();
            Command.Parameters.Add(new SqlParameter("@TemplateID", template_id));
            if (parent_page_id != -1)
                Command.Parameters.Add(new SqlParameter("@ParentPageID", parent_page_id));
            else
                Command.Parameters.Add(new SqlParameter(@"ParentPageID", null));
            Command.Parameters.Add(new SqlParameter("@DisplayInNav", newPage.DisplayInNav));
            Command.Parameters.Add(new SqlParameter("@RequireSSL", newPage.RequireSSL));
            Command.Parameters.Add(new SqlParameter("@PageName", newPage.PageName));
            Command.Parameters.Add(new SqlParameter("@PageDesc", newPage.PageDescription));
            Command.Parameters.Add(new SqlParameter("@PageSettings", newPage.PageSettings()));
            Command.Parameters.Add(new SqlParameter("@Guid", newPage.Guid));
            Command.Parameters.Add(new SqlParameter("@ValidateRequest", newPage.ValidateRequest));
            Command.Parameters[Command.Parameters.Count - 1].Direction = ParameterDirection.Output;
            PageMap.Add(newPage.PageID, Convert.ToInt32(Command.ExecuteScalar()));

            //
            // Create all the modules for this page.
            //
            foreach (ModuleInstance mi in newPage.Modules)
            {
                CreateSingleModuleInstance(mi);
                moduleInstances[mi] = null;
            }
        }


        /// <summary>
        /// Create a single module instance in the database. It is created
        /// on the proper page as defined in the Package.
        /// </summary>
        /// <param name="mi">The module instance to be created.</param>
        private void CreateSingleModuleInstance(ModuleInstance mi)
        {
            Command.CommandType = CommandType.Text;
            Command.CommandText = "INSERT INTO [port_module_instance] (" +
                "[created_by], [modified_by], [module_id], [module_title]" +
                ", [show_title], [template_frame_name], [template_frame_order]" +
                ", [module_details], [page_id])" +
                " VALUES ('PackageInstaller', 'PackageInstaller', @ModuleID, @ModuleTitle," +
                ", @ShowTitle, @TemplateFrameName, @TemplateFrameOrder" +
                ", @ModuleDetails, @PageID)";
            Command.Parameters.Clear();
            Command.Parameters.Add(new SqlParameter("@ModuleID", ModuleMap[mi.ModuleTypeID]));
            Command.Parameters.Add(new SqlParameter("@ModuleTitle", mi.ModuleTitle));
            Command.Parameters.Add(new SqlParameter("@ShowTitle", mi.ShowTitle));
            Command.Parameters.Add(new SqlParameter("@TemplateFrameName", mi.TemplateFrameName));
            Command.Parameters.Add(new SqlParameter("@TemplateFrameOrder", mi.TemplateFrameOrder));
            Command.Parameters.Add(new SqlParameter("@ModuleDetails", mi.ModuleDetails));
            Command.Parameters.Add(new SqlParameter("@PageID", PageMap[mi.Page.PageID]));
            ModuleInstanceMap.Add(mi.ModuleInstanceID, Convert.ToInt32(Command.ExecuteScalar()));
        }


        /// <summary>
        /// Update a single page in the database to match the new package
        /// information.
        /// </summary>
        /// <remarks>
        /// TODO: To do this properly we need a guid of some kind on each
        /// module instance, we can't assume each page will have only one module
        /// type on each page.
        /// </remarks>
        /// <param name="oldPage">The old page that is being updated, never null.</param>
        /// <param name="newPage">The new page that should be updated to.</param>
        /// <param name="moduleInstances">The list of module instances that will need their settings updated later.</param>
        private void UpdateSinglePage(PageInstance oldPage, PageInstance newPage, ref Dictionary<ModuleInstance, ModuleInstance> moduleInstances)
        {
            //
            // Remove any module instances that exist in the old but
            // do not exist in the new page.
            //
            foreach (ModuleInstance miOld in oldPage.Modules)
            {
                Boolean delete = true;

                foreach (ModuleInstance miNew in newPage.Modules)
                {
                    if (true /* miOld.Guid == miNew.Guid */)
                    {
                        delete = false;
                        break;
                    }
                }

                //
                // Delete the module instance from the page.
                //
                delete = false;
                if (delete)
                {
                    Int32 module_instance_id;

                    //
                    // Get the database ID of the module instance to delete.
                    //
                    Command.CommandType = CommandType.Text;
                    Command.CommandText = "SELECT [module_instance_id] FROM [port_module_instance] WHERE [guid] = @Guid";
                    Command.Parameters.Clear();
//                    Command.Parameters.Add(new SqlParameter("@Guid", miOld.Guid));
                    module_instance_id = Convert.ToInt32(Command.ExecuteScalar());

                    //
                    // Perform the delete operation.
                    //
                    Command.CommandType = CommandType.StoredProcedure;
                    Command.CommandText = "port_sp_del_module_instance";
                    Command.Parameters.Clear();
                    Command.Parameters.Add(new SqlParameter("@ModuleInstanceID", module_instance_id));
                    Command.ExecuteNonQuery();
                }
            }

            //
            // Update any default module instance settings for module
            // instances that exist in both the old package and the
            // new package.
            //
            foreach (ModuleInstance miOld in oldPage.Modules)
            {
                ModuleInstance newInstance = null;

                foreach (ModuleInstance miNew in newPage.Modules)
                {
                    if (false /* miNew.Guid == miOld.Guid */)
                    {
                        newInstance = miNew;
                        break;
                    }
                }

                //
                // Perform an update on the module instance. Currently
                // all we do is update the module instance settings.
                //
                newInstance = null;
                if (newInstance != null)
                {
                    moduleInstances[newInstance] = miOld;

                    //
                    // Retrieve the database ID of the existing module instance.
                    //
                    Command.CommandType = CommandType.Text;
                    Command.CommandText = "SELECT [module_instance_id] FROM [port_module_instance] WHERE [guid] = @Guid";
                    Command.Parameters.Clear();
//                    Command.Parameters.Add(new SqlParameter("@Guid", newInstance.Guid));
//                    ModuleInstanceMap[newInstance.ModuleInstanceID] = Convert.ToInt32(Command.ExecuteScalar());
                }
            }

            //
            // Create any new module instances that do not exist in
            // the old page but exist in the new page.
            //
            foreach (ModuleInstance miNew in newPage.Modules)
            {
                Boolean create = true;

                foreach (ModuleInstance miOld in oldPage.Modules)
                {
                    if (true /* miNew.Guid == miOld.Guid */)
                    {
                        create = false;
                        break;
                    }
                }

                //
                // Create the module instance.
                //
                create = false;
                if (create)
                {
                    CreateSingleModuleInstance(miNew);
                    moduleInstances[miNew] = null;
                }
            }

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
        /// Walk through the list of module instances that have been
        /// configured and delete, update or create all module instance
        /// settings.
        /// </summary>
        /// <param name="moduleInstances">The list of installed module instances, they key is the new module instance and the value is null or the old module instance.</param>
        private void UpdateAllModuleInstanceSettings(Dictionary<ModuleInstance, ModuleInstance> moduleInstances)
        {
            foreach (KeyValuePair<ModuleInstance, ModuleInstance> kvp in moduleInstances)
            {
                //
                // kvp.Key == the new module instance.
                // kvp.Value == null || the old module instance.
                //

                //
                // Delete module settings that exist in the old package
                // but do not exist in the new package.
                //
                if (kvp.Value != null)
                {
                    foreach (ModuleInstanceSetting oldSetting in kvp.Value.Settings)
                    {
                        Boolean delete = true;

                        foreach (ModuleInstanceSetting s in kvp.Key.Settings)
                        {
                            if (s.Name == oldSetting.Name)
                            {
                                delete = false;
                                break;
                            }
                        }

                        //
                        // Perform the delete operation.
                        //
                        if (delete)
                        {
                            Command.CommandType = CommandType.Text;
                            Command.CommandText = "DELETE FROM [port_module_instance_setting] WHERE [module_instance_id] = @ModuleInstanceID AND [name] = @SettingName";
                            Command.Parameters.Clear();
                            Command.Parameters.Add(new SqlParameter("@ModuleInstanceID", ModuleInstanceMap[kvp.Key.ModuleInstanceID]));
                            Command.Parameters.Add(new SqlParameter("@SettingName", oldSetting.Name));
                            Command.ExecuteNonQuery();
                        }
                    }
                }

                //
                // Update module settings that exist in the old package
                // and exist in the new package, and have not been updated
                // by the user or have had their type changed.
                //
                if (kvp.Value != null)
                {
                    foreach (ModuleInstanceSetting oldSetting in kvp.Value.Settings)
                    {
                        ModuleInstanceSetting newSetting = null;

                        foreach (ModuleInstanceSetting s in kvp.Key.Settings)
                        {
                            if (s.Name == oldSetting.Name)
                            {
                                if (oldSetting.Type != s.Type)
                                {
                                    newSetting = s;
                                }
                                else
                                {
                                    //
                                    // Check if the user has customized the setting.
                                    //
                                    Command.CommandType = CommandType.Text;
                                    Command.CommandText = "SELECT [value] FROM [port_module_instance_setting] WHERE [module_instance_id] = @ModuleInstanceID AND [name] = @SettingName";
                                    Command.Parameters.Clear();
                                    Command.Parameters.Add(new SqlParameter("@ModuleInstanceID", ModuleInstanceMap[kvp.Key.ModuleInstanceID]));
                                    Command.Parameters.Add(new SqlParameter("@SettingName", oldSetting.Name));
                                    if (oldSetting.Value == Command.ExecuteScalar().ToString())
                                    {
                                        newSetting = s;
                                    }
                                }

                                break;
                            }
                        }

                        //
                        // Perform the update operation.
                        //
                        if (newSetting != null)
                        {
                            Command.CommandType = CommandType.Text;
                            Command.CommandText = "UPDATE [port_module_instance_setting] SET [value] = @Value, [type_id] = @TypeID WHERE [module_instance_id] = @ModuleInstanceID AND [name] = @SettingName";
                            Command.Parameters.Clear();
                            Command.Parameters.Add(new SqlParameter("@Value", newSetting.Value));
                            Command.Parameters.Add(new SqlParameter("@TypeID", (Int32)newSetting.Type));
                            Command.Parameters.Add(new SqlParameter("@ModuleInstanceID", ModuleInstanceMap[kvp.Key.ModuleInstanceID]));
                            Command.Parameters.Add(new SqlParameter("@SettingName", oldSetting.Name));
                            Command.ExecuteNonQuery();
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Remove all pages from the system. This needs to be a somewhat recursive
        /// removal as we cannot remove a page that has child pages. Start from the
        /// bottom and work our way up to minimize the chance of deleting stuff out
        /// of order. Because the user might have moved pages around, or even deleted
        /// a page, we need to provide for a page not existing while performing the
        /// delete operation.
        /// </summary>
        /// <remarks>
        /// Before this method is called the user should be informed of any user-defined
        /// pages that will be deleted by this operation so they have a chance to move
        /// them if they wish to keep them.
        /// </remarks>
        /// <param name="package">The package whose pages we need to remove from the database.</param>
        private void RemovePackagePages(Package package)
        {
            //
            // foreach Page
            //  find database page_id
            //  call RemoveSinglePage(package, page_id)
            // TODO: do this.
        }


        /// <summary>
        /// Remove a single page from the database. First remove all module instances
        /// from the page and then remove all child pages. Finally delete the page
        /// itself.
        /// </summary>
        /// <param name="pageID">The ID number of the page to remove from the database.</param>
        private void RemoveSinglePage(Int32 pageID)
        {
            //
            // foreach moduleInstance in moduleInstances
            //  delete moduleInstance
            // foreach page_id in childPages
            //  call RemoveSinglePage(package, page_id)
            // delete pageID
            // TODO: do this.
        }


        /// <summary>
        /// Remove all modules that were installed by this package from the database.
        /// This operation must first remove any module instances that still exist
        /// which reference the modules, as the user might have created their own
        /// instances on their own pages. Once that is done we remove the modules
        /// themselves from the database.
        /// </summary>
        /// <remarks>
        /// Before this method is called the user should be informed of any user-defined
        /// pages that will be left empty by this operation. We do not delete any
        /// pages that have been emptied, but the user might want to know about it ahead
        /// of time.
        /// </remarks>
        /// <param name="package">The package whose modules we need to remove from the database.</param>
        private void RemovePackageModules(Package package)
        {
            //
            // foreach module
            //  delete all module instances
            //  delete module
            // TODO: do this.
        }


        /// <summary>
        /// Remove all files from the file system that were installed by this
        /// package. This is done in a safe way by saving all file changes into
        /// the fileChanges parameter so that they can be restored later if some
        /// error occurs later on in the removal process. No exception is raised
        /// if a file is not found, it is simply ignored.
        /// </summary>
        /// <param name="package">The package whose files are to be removed from the file system.</param>
        /// <param name="fileChanges">The list of changes we make to the file system will be stored in this parameter.</param>
        private void RemovePackageFiles(Package package, ref List<FileChange> fileChanges)
        {
            //
            // foreach file
            //  create and store FileChange.
            //  delete file.
            // TODO: do this.
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
            try
            {
                xdoc = new XmlDocument();
                xdoc.Load(rdr);
            }
            catch
            {
                xdoc = null;
            }
            rdr.Close();
            
            return new Package(xdoc);
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
            Command.CommandText = "cust_rc_packager_get_packages_recommending";
            Command.Parameters.Clear();
            Command.Parameters.Add(new SqlParameter("@Package", packageName));
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
            Command.CommandText = "cust_rc_packager_get_packages_requiring";
            Command.Parameters.Clear();
            Command.Parameters.Add(new SqlParameter("@Package", packageName));
            rdr = Command.ExecuteReader();

            while (rdr.Read())
            {
                packages.Add(rdr[0].ToString());
            }
            rdr.Close();

            return packages;
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
            List<FileChange> fileChanges = new List<FileChange>();
            PackageVersion version;
            Package oldPackage;
            Database db;
            Migration mig;

            
            //
            // Setup database maps.
            //
            PageMap = new Dictionary<int, int>();
            ModuleMap = new Dictionary<int, int>();
            ModuleInstanceMap = new Dictionary<int, int>();

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
                        Migration mi2 = MigrationForPackage(GetInstalledPackage(dependantName));

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
                    foreach (FileChange fc in fileChanges)
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
            List<FileChange> fileChanges = new List<FileChange>();
            Package package;
            Migration mig;
            Database db;


            //
            // Setup database maps.
            //
            PageMap = new Dictionary<int, int>();
            ModuleMap = new Dictionary<int, int>();
            ModuleInstanceMap = new Dictionary<int, int>();

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
            if (PackagesRequiring(packageName).Count > 0)
                throw new PackageDependencyException(String.Format("The package {0} as dependencies and cannot be removed.", packageName));

            //
            // Begin the database transaction.
            //
            db = new Database(Connection);
            db.BeginTransaction();
            Command.Transaction = db.dbTransaction;

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
                        mig = MigrationForPackage(GetInstalledPackage(name));
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
                    mig = MigrationForPackage(package);

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
                    RemovePackagePages(package);
                    RemovePackageModules(package);
                    RemovePackageFiles(package, ref fileChanges);
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
                    foreach (FileChange fc in fileChanges)
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
        }
    }


    /// <summary>
    /// Internal class that saves the changes to a single file. This class
    /// also provides a method to restore the file back to it's original
    /// state.
    /// </summary>
    class FileChange
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
        public FileChange(FileInfo original)
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
