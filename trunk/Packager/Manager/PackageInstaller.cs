using RefreshCache.Packager;
using RefreshCache.Packager.Migrator;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Xml;

namespace RefreshCache.Packager.Manager
{
    /// <summary>
    /// This class provides all the interface methods for performing installation
    /// related tasks with packages. A package can be installed, upgraded or
    /// removed via this class. Only a single operation should be performed per
    /// instance of this class.
    /// </summary>
    internal class PackageInstaller
    {
        #region Properties

        private PackageDatabase pdb;
        private List<FileChange> _FileChanges;
        private Dictionary<Int32, Int32> ModuleMap;
        private Dictionary<Int32, Int32> PageMap;
        private Dictionary<Int32, Int32> ModuleInstanceMap;

        #endregion


        #region Constructors


        public PackageInstaller(PackageDatabase db)
        {
            pdb = db;
            _FileChanges = new List<FileChange>();

            //
            // Setup database maps.
            //
            PageMap = new Dictionary<int, int>();
            ModuleMap = new Dictionary<int, int>();
            ModuleInstanceMap = new Dictionary<int, int>();

        }
        #endregion


        /// <summary>
        /// Verify all dependencies of the package for an install or upgrade
        /// operation.
        /// </summary>
        /// <param name="package">The package that is about to be installed or upgraded to.</param>
        /// <exception cref="PackageVersionException">Package is already installed with the same version or a later version number.</exception>
        /// <exception cref="PackageDependencyException">Package has dependencies that have not been met.</exception>
        public void VerifyDependenciesForInstall(Package package)
        {
            PackageVersion version;


            //
            // Verify the package version
            //
            try
            {
                version = pdb.VersionOfPackage(package.Info.PackageName);
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
                version = pdb.VersionOfArena();
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
                version = pdb.VersionOfPackage(req.Name);
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
        public Migration MigrationForPackage(Package package)
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
        public void InstallPackageFiles(Package newPackage, Package oldPackage)
        {
            List<String> files = new List<String>();


            //
            // Install all new or updated files.
            //
            foreach (File f in newPackage.AllFiles())
            {
                FileInfo target = new FileInfo(pdb.RootPath + @"\" + f.Path);

                _FileChanges.Add(new FileChange(target));
                if (!target.Directory.Exists)
                {
                    target.Directory.Create();
                }
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
                    FileInfo target = new FileInfo(pdb.RootPath + @"\" + f.Path);
                    _FileChanges.Add(new FileChange(target));
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
        public void InstallPackageModules(Package package, Package oldPackage)
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
                        pdb.Command.CommandType = CommandType.Text;
                        pdb.Command.CommandText = "SELECT [module_id] FROM [port_module] WHERE [module_url] = @ModuleUrl";
                        pdb.Command.Parameters.Clear();
                        pdb.Command.Parameters.Add(new SqlParameter("@ModuleUrl", m.URL));
                        module_id = Convert.ToInt32(pdb.Command.ExecuteScalar());

                        //
                        // Get a list of all module instances that need to be removed,
                        // as well as the pages that might need to be removed.
                        //
                        pdb.Command.CommandType = CommandType.Text;
                        pdb.Command.CommandText = "SELECT [module_instance_id],[page_id] FROM [port_module_instance] WHERE [module_id] = @ModuleID";
                        pdb.Command.Parameters.Clear();
                        pdb.Command.Parameters.Add(new SqlParameter("@ModuleID", module_id));
                        rdr = pdb.Command.ExecuteReader();
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
                            pdb.Command.CommandType = CommandType.StoredProcedure;
                            pdb.Command.CommandText = "port_sp_del_module_instance";
                            pdb.Command.Parameters.Clear();
                            pdb.Command.Parameters.Add(new SqlParameter("@ModuleInstanceID", kvp.Key));
                            pdb.Command.ExecuteNonQuery();

                            //
                            // If the page still has any module instances on it then
                            // we do not want to delete it.
                            //
                            pdb.Command.CommandType = CommandType.Text;
                            pdb.Command.CommandText = "SELECT COUNT([module_instance_id]) FROM [port_module_instance] WHERE [page_id] = @PageID";
                            pdb.Command.Parameters.Clear();
                            pdb.Command.Parameters.Add(new SqlParameter("@PageID", kvp.Value));
                            Count = Convert.ToInt32(pdb.Command.ExecuteScalar());
                            if (Count > 0)
                                continue;

                            //
                            // If the page has any child pages then we do not want to
                            // delete it.
                            //
                            pdb.Command.CommandType = CommandType.Text;
                            pdb.Command.CommandText = "SELECT COUNT([page_id]) FROM [port_portal_page] WHERE [parent_page_id] = @PageID";
                            pdb.Command.Parameters.Clear();
                            pdb.Command.Parameters.Add(new SqlParameter("@PageID", kvp.Value));
                            Count = Convert.ToInt32(pdb.Command.ExecuteScalar());
                            if (Count > 0)
                                continue;

                            //
                            // If the page is one that is in the list of Pages (i.e. one
                            // that was created by the package) then do not delete. We
                            // will only delete user-created pages that are empty.
                            //
                            pdb.Command.CommandType = CommandType.Text;
                            pdb.Command.CommandText = "SELECT [guid] FROM [port_portal_page] WHERE [page_id] = @PageID";
                            pdb.Command.Parameters.Clear();
                            pdb.Command.Parameters.Add(new SqlParameter("@PageID", kvp.Value));
                            String pageGuid = pdb.Command.ExecuteScalar().ToString();
                            Count = 0;
                            foreach (PageInstance pg in oldPackage.OrderedPages())
                            {
                                if (pg.Guid == pageGuid)
                                {
                                    Count = 1;
                                    break;
                                }
                            }
                            if (Count > 0)
                                continue;

                            //
                            // Okay, nuke it. No existing modules and no child pages.
                            //
                            pdb.Command.CommandType = CommandType.StoredProcedure;
                            pdb.Command.CommandText = "port_sp_del_portal_page";
                            pdb.Command.Parameters.Clear();
                            pdb.Command.Parameters.Add(new SqlParameter("@PageID", kvp.Value));
                            pdb.Command.ExecuteNonQuery();
                        }

                        //
                        // Now delete the module.
                        //
                        pdb.Command.CommandType = CommandType.StoredProcedure;
                        pdb.Command.CommandText = "port_sp_del_module";
                        pdb.Command.Parameters.Clear();
                        pdb.Command.Parameters.Add(new SqlParameter("@ModuleID", module_id));
                        pdb.Command.ExecuteNonQuery();
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
                    pdb.Command.CommandType = CommandType.Text;
                    pdb.Command.CommandText = "SELECT [module_id] FROM [port_module] WHERE [module_name] = @ModuleName";
                    pdb.Command.Parameters.Clear();
                    pdb.Command.Parameters.Add(new SqlParameter("@ModuleName", m.Name));
                    if (pdb.Command.ExecuteScalar() != null)
                        throw new PackageLocalConflictException(String.Format("There is an existing module with the name '{0}'.", m.Name));

                    //
                    // Check if there is a module with the same url, this would
                    // mean the user has something that will conflict.
                    //
                    pdb.Command.CommandType = CommandType.Text;
                    pdb.Command.CommandText = "SELECT [module_id] FROM [port_module] WHERE [module_url] = @ModuleUrl";
                    pdb.Command.Parameters.Clear();
                    pdb.Command.Parameters.Add(new SqlParameter("@ModuleUrl", m.URL));
                    if (pdb.Command.ExecuteScalar() != null)
                        throw new PackageLocalConflictException(String.Format("There is an existing module with the url '{0}'.", m.URL));

                    //
                    // Create the new module in the database.
                    //
                    pdb.Command.CommandType = CommandType.StoredProcedure;
                    pdb.Command.CommandText = "port_sp_save_module";
                    pdb.Command.Parameters.Clear();
                    pdb.Command.Parameters.Add(new SqlParameter("@ModuleId", -1));
                    pdb.Command.Parameters.Add(new SqlParameter("@UserId", "PackageInstaller"));
                    pdb.Command.Parameters.Add(new SqlParameter("@ModuleName", m.Name));
                    pdb.Command.Parameters.Add(new SqlParameter("@ModuleUrl", m.URL));
                    pdb.Command.Parameters.Add(new SqlParameter("@ModuleDesc", m.Description));
                    pdb.Command.Parameters.Add(new SqlParameter("@AllowsChildModules", m.AllowsChildModules));
                    pdb.Command.Parameters.Add(new SqlParameter("@ImagePath", m.ImagePath));
                    pdb.Command.Parameters.Add(new SqlParameter(@"ID", null));
                    pdb.Command.Parameters[pdb.Command.Parameters.Count - 1].Direction = ParameterDirection.Output;
                    pdb.Command.ExecuteNonQuery();

                    //
                    // Add the new module ID to the database map.
                    //
                    ModuleMap.Add(m.ModuleID, Convert.ToInt32(pdb.Command.Parameters[pdb.Command.Parameters.Count - 1].Value));
                }
                else
                {
                    //
                    // Update the database ID map.
                    //
                    pdb.Command.CommandType = CommandType.StoredProcedure;
                    pdb.Command.CommandText = "port_sp_get_moduleByUrl";
                    pdb.Command.Parameters.Clear();
                    pdb.Command.Parameters.Add(new SqlParameter("@ModuleUrl", m.URL));
                    SqlDataReader rdr = pdb.Command.ExecuteReader();

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
        public void InstallPackagePages(Package package, Package oldPackage)
        {
            Dictionary<ModuleInstance, ModuleInstance> moduleInstances = new Dictionary<ModuleInstance, ModuleInstance>();
            List<PageInstance> oldPageList = null;


            //
            // Retrieve the list of old pages and reverse the order.
            //
            if (oldPackage != null)
            {
                oldPageList = oldPackage.OrderedPages();
                oldPageList.Reverse();
            }

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
                        pdb.Command.CommandType = CommandType.Text;
                        pdb.Command.CommandText = "SELECT [page_id] FROM [port_portal_page] WHERE [guid] = @Guid";
                        pdb.Command.Parameters.Clear();
                        pdb.Command.Parameters.Add(new SqlParameter("@Guid", page.Guid));
                        page_id = Convert.ToInt32(pdb.Command.ExecuteScalar());

                        //
                        // Delete the page from the database.
                        //
                        pdb.Command.CommandType = CommandType.StoredProcedure;
                        pdb.Command.CommandText = "port_sp_del_portal_page";
                        pdb.Command.Parameters.Clear();
                        pdb.Command.Parameters.Add(new SqlParameter("@PageID", page_id));
                        pdb.Command.ExecuteNonQuery();
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

                pdb.Command.CommandType = CommandType.Text;
                pdb.Command.CommandText = "SELECT [template_id] FROM [port_portal_page] WHERE [page_id] = " + parent_page_id.ToString();
                pdb.Command.Parameters.Clear();
                template_id = Convert.ToInt32(pdb.Command.ExecuteScalar());
            }
            else
            {
                parent_page_id = -1;
                template_id = 1;
            }

            //
            // Execute the SQL query to create the portal page.
            //
            pdb.Command.CommandType = CommandType.Text;
            pdb.Command.CommandText = "INSERT INTO [port_portal_page] (" +
                "[created_by], [modified_by], [template_id], [parent_page_id]" +
                ", [page_order], [display_in_nav], [page_name], [page_desc]" +
                ", [page_settings], [require_ssl], [guid], [validate_request])" +
                " VALUES ('PackageInstaller', 'PackageInstaller', @TemplateID, @ParentPageID" +
                ", 2147483647, @DisplayInNav, @PageName, @PageDesc" +
                ", @PageSettings, @RequireSSL, @Guid, @ValidateRequest);" +
                " SELECT CAST(IDENT_CURRENT('port_portal_page') AS int)";
            pdb.Command.Parameters.Clear();
            pdb.Command.Parameters.Add(new SqlParameter("@TemplateID", template_id));
            if (parent_page_id != -1)
                pdb.Command.Parameters.Add(new SqlParameter("@ParentPageID", parent_page_id));
            else
                pdb.Command.Parameters.Add(new SqlParameter(@"ParentPageID", null));
            pdb.Command.Parameters.Add(new SqlParameter("@DisplayInNav", newPage.DisplayInNav));
            pdb.Command.Parameters.Add(new SqlParameter("@RequireSSL", newPage.RequireSSL));
            pdb.Command.Parameters.Add(new SqlParameter("@PageName", newPage.PageName));
            pdb.Command.Parameters.Add(new SqlParameter("@PageDesc", newPage.PageDescription));
            pdb.Command.Parameters.Add(new SqlParameter("@PageSettings", newPage.PageSettings()));
            pdb.Command.Parameters.Add(new SqlParameter("@Guid", newPage.Guid));
            pdb.Command.Parameters.Add(new SqlParameter("@ValidateRequest", newPage.ValidateRequest));
            PageMap.Add(newPage.PageID, Convert.ToInt32(pdb.Command.ExecuteScalar()));

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
            pdb.Command.CommandType = CommandType.Text;
            pdb.Command.CommandText = "INSERT INTO [port_module_instance] (" +
                "[created_by], [modified_by], [module_id], [module_title]" +
                ", [show_title], [template_frame_name], [template_frame_order]" +
                ", [module_details], [page_id])" +
                " VALUES ('PackageInstaller', 'PackageInstaller', @ModuleID, @ModuleTitle," +
                ", @ShowTitle, @TemplateFrameName, @TemplateFrameOrder" +
                ", @ModuleDetails, @PageID);" +
                " SELECT CAST(IDENT_CURRENT('port_module_instance') AS int)";
            pdb.Command.Parameters.Clear();
            pdb.Command.Parameters.Add(new SqlParameter("@ModuleID", ModuleMap[mi.ModuleTypeID]));
            pdb.Command.Parameters.Add(new SqlParameter("@ModuleTitle", mi.ModuleTitle));
            pdb.Command.Parameters.Add(new SqlParameter("@ShowTitle", mi.ShowTitle));
            pdb.Command.Parameters.Add(new SqlParameter("@TemplateFrameName", mi.TemplateFrameName));
            pdb.Command.Parameters.Add(new SqlParameter("@TemplateFrameOrder", mi.TemplateFrameOrder));
            pdb.Command.Parameters.Add(new SqlParameter("@ModuleDetails", mi.ModuleDetails));
            pdb.Command.Parameters.Add(new SqlParameter("@PageID", PageMap[mi.Page.PageID]));
            ModuleInstanceMap.Add(mi.ModuleInstanceID, Convert.ToInt32(pdb.Command.ExecuteScalar()));
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
                    pdb.Command.CommandType = CommandType.Text;
                    pdb.Command.CommandText = "SELECT [module_instance_id] FROM [port_module_instance] WHERE [guid] = @Guid";
                    pdb.Command.Parameters.Clear();
//                    pdb.Command.Parameters.Add(new SqlParameter("@Guid", miOld.Guid));
                    module_instance_id = Convert.ToInt32(pdb.Command.ExecuteScalar());

                    //
                    // Perform the delete operation.
                    //
                    pdb.Command.CommandType = CommandType.StoredProcedure;
                    pdb.Command.CommandText = "port_sp_del_module_instance";
                    pdb.Command.Parameters.Clear();
                    pdb.Command.Parameters.Add(new SqlParameter("@ModuleInstanceID", module_instance_id));
                    pdb.Command.ExecuteNonQuery();
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
                    pdb.Command.CommandType = CommandType.Text;
                    pdb.Command.CommandText = "SELECT [module_instance_id] FROM [port_module_instance] WHERE [guid] = @Guid";
                    pdb.Command.Parameters.Clear();
//                    pdb.Command.Parameters.Add(new SqlParameter("@Guid", newInstance.Guid));
//                    ModuleInstanceMap[newInstance.ModuleInstanceID] = Convert.ToInt32(pdb.Command.ExecuteScalar());
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
            pdb.Command.CommandType = CommandType.Text;
            pdb.Command.CommandText = "SELECT [page_id] FROM [port_portal_page] WHERE [guid] = @Guid";
            pdb.Command.Parameters.Clear();
            pdb.Command.Parameters.Add(new SqlParameter("@Guid", oldPage.Guid));
            PageMap[oldPage.PageID] = Convert.ToInt32(pdb.Command.ExecuteScalar());
        }


        /// <summary>
        /// Walk through the list of module instances that have been
        /// configured and delete, update or create all module instance
        /// settings.
        /// </summary>
        /// <param name="moduleInstances">The list of installed module instances, they key is the new module instance and the value is null or the old module instance.</param>
        public void UpdateAllModuleInstanceSettings(Dictionary<ModuleInstance, ModuleInstance> moduleInstances)
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
                        // Determine if the user has customized the old setting value.
                        //
                        if (delete)
                        {
                            pdb.Command.CommandType = CommandType.Text;
                            pdb.Command.CommandText = "SELECT [value] FROM [port_module_instance_setting] WHERE [module_instance_id] = @ModuleInstanceID AND [name] = @SettingName";
                            pdb.Command.Parameters.Clear();
                            pdb.Command.Parameters.Add(new SqlParameter("@ModuleInstanceID", ModuleInstanceMap[kvp.Key.ModuleInstanceID]));
                            pdb.Command.Parameters.Add(new SqlParameter("@SettingName", oldSetting.Name));
                            Object value = pdb.Command.ExecuteScalar();
                            if (value == null && !String.IsNullOrEmpty(oldSetting.Value))
                            {
                                delete = false;
                            }
                            else if (value.ToString() != oldSetting.Value)
                            {
                                delete = false;
                            }
                        }

                        //
                        // Perform the delete operation.
                        //
                        if (delete)
                        {
                            pdb.Command.CommandType = CommandType.Text;
                            pdb.Command.CommandText = "DELETE FROM [port_module_instance_setting] WHERE [module_instance_id] = @ModuleInstanceID AND [name] = @SettingName";
                            pdb.Command.Parameters.Clear();
                            pdb.Command.Parameters.Add(new SqlParameter("@ModuleInstanceID", ModuleInstanceMap[kvp.Key.ModuleInstanceID]));
                            pdb.Command.Parameters.Add(new SqlParameter("@SettingName", oldSetting.Name));
                            pdb.Command.ExecuteNonQuery();
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
                                    pdb.Command.CommandType = CommandType.Text;
                                    pdb.Command.CommandText = "SELECT [value] FROM [port_module_instance_setting] WHERE [module_instance_id] = @ModuleInstanceID AND [name] = @SettingName";
                                    pdb.Command.Parameters.Clear();
                                    pdb.Command.Parameters.Add(new SqlParameter("@ModuleInstanceID", ModuleInstanceMap[kvp.Key.ModuleInstanceID]));
                                    pdb.Command.Parameters.Add(new SqlParameter("@SettingName", oldSetting.Name));
                                    if (oldSetting.Value == pdb.Command.ExecuteScalar().ToString())
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
                            pdb.Command.CommandType = CommandType.Text;
                            pdb.Command.CommandText = "UPDATE [port_module_instance_setting] SET [value] = @Value, [type_id] = @TypeID WHERE [module_instance_id] = @ModuleInstanceID AND [name] = @SettingName";
                            pdb.Command.Parameters.Clear();
                            pdb.Command.Parameters.Add(new SqlParameter("@Value", newSetting.Value));
                            pdb.Command.Parameters.Add(new SqlParameter("@TypeID", (Int32)newSetting.Type));
                            pdb.Command.Parameters.Add(new SqlParameter("@ModuleInstanceID", ModuleInstanceMap[kvp.Key.ModuleInstanceID]));
                            pdb.Command.Parameters.Add(new SqlParameter("@SettingName", oldSetting.Name));
                            pdb.Command.ExecuteNonQuery();
                        }
                    }
                }

                //
                // Create any new module instance settings that don't exist in
                // the old package but do exist in the new package.
                //
                if (kvp.Value == null)
                {
                    foreach (ModuleInstanceSetting newSetting in kvp.Key.Settings)
                    {
                        Boolean create = true;

                        foreach (ModuleInstanceSetting oldSetting in kvp.Value.Settings)
                        {
                            if (oldSetting.Name == newSetting.Name)
                            {
                                create = false;
                                break;
                            }
                        }

                        //
                        // Create the module instance setting.
                        //
                        if (create)
                        {
                            pdb.Command.CommandType = CommandType.Text;
                            pdb.Command.CommandText = "INSERT INTO [port_module_instance_setting] " +
                                "([module_instance_id], [name], [value], [type_id]) " +
                                "VALUES (@ModuleInstanceID, @SettingName, @Value, @TypeID)";
                            pdb.Command.Parameters.Clear();
                            pdb.Command.Parameters.Add(new SqlParameter("@Value", newSetting.Value));
                            pdb.Command.Parameters.Add(new SqlParameter("@TypeID", (Int32)newSetting.Type));
                            pdb.Command.Parameters.Add(new SqlParameter("@ModuleInstanceID", ModuleInstanceMap[kvp.Key.ModuleInstanceID]));
                            pdb.Command.Parameters.Add(new SqlParameter("@SettingName", newSetting.Name));
                            pdb.Command.ExecuteNonQuery();
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
        public void RemovePackagePages(Package package)
        {
            List<PageInstance> pages;
            Object value;


            pages = package.OrderedPages();
            pages.Reverse();
            foreach (PageInstance page in pages)
            {
                //
                // Try to find the database ID of the page, if it is not found then
                // just ignore it as the user might have deleted it or it might
                // have been deleted by a previous operation.
                //
                pdb.Command.CommandType = CommandType.Text;
                pdb.Command.CommandText = "SELECT [page_id] FROM [port_portal_page] WHERE [guid] = @Guid";
                pdb.Command.Parameters.Clear();
                pdb.Command.Parameters.Add(new SqlParameter("@Guid", page.Guid));
                value = pdb.Command.ExecuteScalar();
                if (value == null)
                    continue;

                RemoveSinglePage(Convert.ToInt32(value));
            }
        }


        /// <summary>
        /// Remove a single page from the database. First remove all module instances
        /// from the page and then remove all child pages. Finally delete the page
        /// itself.
        /// </summary>
        /// <param name="pageID">The ID number of the page to remove from the database.</param>
        private void RemoveSinglePage(Int32 pageID)
        {
            SqlDataReader rdr;
            List<Int32> ids;


            //
            // Find all the module instances on this page.
            //
            pdb.Command.CommandType = CommandType.Text;
            pdb.Command.CommandText = "SELECT [module_instance_id] FROM [port_module_instance] WHERE [page_id] = @PageID";
            pdb.Command.Parameters.Clear();
            pdb.Command.Parameters.Add(new SqlParameter("@PageID", pageID));
            rdr = pdb.Command.ExecuteReader();
            ids = new List<int>();
            while (rdr.Read())
            {
                ids.Add(Convert.ToInt32(rdr[0].ToString()));
            }
            rdr.Close();

            //
            // Loop through and delete each module instance.
            //
            foreach (Int32 module_instance_id in ids)
            {
                pdb.Command.CommandType = CommandType.Text;
                pdb.Command.CommandText = "DELETE FROM [port_module_instance_setting] WHERE [module_instance_id] = @ModuleInstanceID";
                pdb.Command.Parameters.Clear();
                pdb.Command.Parameters.Add(new SqlParameter("@ModuleInstanceID", module_instance_id));
                pdb.Command.ExecuteNonQuery();

                pdb.Command.CommandType = CommandType.StoredProcedure;
                pdb.Command.CommandText = "port_sp_del_module_instance";
                pdb.Command.Parameters.Clear();
                pdb.Command.Parameters.Add(new SqlParameter("@ModuleInstanceID", module_instance_id));
                pdb.Command.ExecuteNonQuery();
            }

            //
            // Find all the child pages of this page.
            //
            pdb.Command.CommandType = CommandType.Text;
            pdb.Command.CommandText = "SELECT [page_id] FROM [port_portal_page] WHERE [parent_page_id] = @PageID";
            pdb.Command.Parameters.Clear();
            pdb.Command.Parameters.Add(new SqlParameter("@PageID", pageID));
            rdr = pdb.Command.ExecuteReader();
            ids = new List<int>();
            while (rdr.Read())
            {
                ids.Add(Convert.ToInt32(rdr[0].ToString()));
            }
            rdr.Close();

            //
            // Loop through and delete each child page.
            //
            foreach (Int32 child_page_id in ids)
            {
                RemoveSinglePage(child_page_id);
            }

            //
            // Delete the portal page passed to us by the caller.
            //
            pdb.Command.CommandType = CommandType.StoredProcedure;
            pdb.Command.CommandText = "port_sp_del_portal_page";
            pdb.Command.Parameters.Clear();
            pdb.Command.Parameters.Add(new SqlParameter("@PageID", pageID));
            pdb.Command.ExecuteNonQuery();
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
        public void RemovePackageModules(Package package)
        {
            foreach (Module m in package.Modules)
            {
                SqlDataReader rdr;
                List<Int32> ids;
                Int32 module_id;


                //
                // Find the module_id of this module.
                //
                pdb.Command.CommandType = CommandType.Text;
                pdb.Command.CommandText = "SELECT [module_id] FROM [port_module] WHERE [module_url] = @URL";
                pdb.Command.Parameters.Clear();
                pdb.Command.Parameters.Add(new SqlParameter("@URL", m.URL));
                module_id = Convert.ToInt32(pdb.Command.ExecuteScalar());

                //
                // Find each module instance of this module.
                //
                pdb.Command.CommandType = CommandType.Text;
                pdb.Command.CommandText = "SELECT [module_instance_id] FROM [port_module_instance] WHERE [module_id] = @ModuleID";
                pdb.Command.Parameters.Clear();
                pdb.Command.Parameters.Add(new SqlParameter("@ModuleID", module_id));
                rdr = pdb.Command.ExecuteReader();
                ids = new List<int>();
                while (rdr.Read())
                {
                    ids.Add(Convert.ToInt32(rdr[0]));
                }
                rdr.Close();

                //
                // Delete each module instance we found.
                //
                foreach (Int32 module_instance_id in ids)
                {
                    pdb.Command.CommandType = CommandType.Text;
                    pdb.Command.CommandText = "DELETE FROM [port_module_instance_setting] WHERE [module_instance_id] = @ModuleInstanceID";
                    pdb.Command.Parameters.Clear();
                    pdb.Command.Parameters.Add(new SqlParameter("@ModuleInstanceID", module_instance_id));
                    pdb.Command.ExecuteNonQuery();

                    pdb.Command.CommandType = CommandType.StoredProcedure;
                    pdb.Command.CommandText = "port_sp_del_module_instance";
                    pdb.Command.Parameters.Clear();
                    pdb.Command.Parameters.Add(new SqlParameter("@ModuleInstanceID", module_instance_id));
                    pdb.Command.ExecuteNonQuery();
                }

                //
                // Delete this module from the database.
                //
                pdb.Command.CommandType = CommandType.StoredProcedure;
                pdb.Command.CommandText = "port_sp_del_module";
                pdb.Command.Parameters.Clear();
                pdb.Command.Parameters.Add(new SqlParameter("@ModuleID", module_id));
                pdb.Command.ExecuteNonQuery();
            }
        }


        /// <summary>
        /// Remove all files from the file system that were installed by this
        /// package. This is done in a safe way by saving all file changes into
        /// the fileChanges parameter so that they can be restored later if some
        /// error occurs later on in the removal process. No exception is raised
        /// if a file is not found, it is simply ignored.
        /// </summary>
        /// <param name="package">The package whose files are to be removed from the file system.</param>
        public void RemovePackageFiles(Package package)
        {
            foreach (File f in package.AllFiles())
            {
                FileInfo fi;

                fi = new FileInfo(pdb.RootPath + @"\" + f.Path);
                _FileChanges.Add(new FileChange(fi));
                fi.Delete();
            }
        }


        public void RevertFileChanges()
        {
            foreach (FileChange fc in _FileChanges)
            {
                try
                {
                    fc.Restore();
                }
                catch { }
            }
        }
    }
}
