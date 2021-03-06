using System;
using System.Collections.Generic;


namespace RefreshCache.Packager.Migrator
{
	/// <summary>
	/// The Migration class must be subclassed by any database migration
	/// collection.  It should then include nested subclasses of DatabaseMigrator
	/// to perform the migration steps.
	/// </summary>
	public abstract class Migration
	{
        /// <summary>
        /// If the Verbose property is set to true then all upgrade/downgrade
        /// tasks are written to the console.
        /// </summary>
        public Boolean Verbose { get; set; }

		/// <summary>
		/// Retrieve the list of DatabaseMigrator nested classes inside of this class. The
		/// list is sorted by version/step number, lowest to highest.
		/// </summary>
		private List<DatabaseMigrator> Migrators
		{
			get
			{
				SortedList<MigratorVersionStep, DatabaseMigrator> migs;
				
				if (_Migrators == null)
				{
					migs = new SortedList<MigratorVersionStep, DatabaseMigrator>();
					
					//
					// Step through each nested class.
					//
					foreach (Type t in GetType().GetNestedTypes())
					{
						MigratorVersionAttribute attrib;
						
						//
						// Ignore anything that is not a direct subclass of DatabaseMigrator.
						//
						if (t.BaseType != typeof(DatabaseMigrator))
							continue;

						//
						// Ignore anything that does not have a migrator version attribute.
						//
						if (t.GetCustomAttributes(typeof(MigratorVersionAttribute), false).Length == 0)
							continue;
						attrib = (MigratorVersionAttribute)t.GetCustomAttributes(typeof(MigratorVersionAttribute), false)[0];

						//
						// We can't have duplicates.
						//
						if (migs.Keys.Contains(attrib.Version))
						{
							throw new Exception("Cannot have identical steps for the same version.");
						}

						//
						// Create an instance of the database migrator and store it along with the
						// attribute version so it is sorted properly.
						//
						DatabaseMigrator mig = (DatabaseMigrator)t.Assembly.CreateInstance(t.FullName);
						migs.Add(attrib.Version, mig);
					}
					
					_Migrators = new List<DatabaseMigrator>(migs.Values);
				}
				
				return _Migrators;
			}
		}
		private List<DatabaseMigrator> _Migrators;
		
		
		/// <summary>
		/// Perform an upgrade on the database object. The upgrade runs from the specified
		/// version to the latest version defined.
		/// </summary>
		/// <param name="db">
		/// A <see cref="Database"/> object that identifies which database is to be upgraded.
		/// </param>
		/// <param name="fromVersion">
		/// The current version number or null if there is nothing currently installed.
		/// </param>
		public void Upgrade(Database db, PackageVersion fromVersion)
		{
            if (fromVersion == null)
                fromVersion = new PackageVersion("0.0.0");

			foreach (DatabaseMigrator migrator in Migrators)
			{
				//
				// Skip versions that are less than or equal to the version we are upgrading from.
                // Also check for the same version since 1.0.0 is less than 1.0.0 step 1.
				//
				if (migrator.Version.CompareTo(fromVersion) <= 0 ||
                    (migrator.Version.Major == fromVersion.Major &&
                     migrator.Version.Minor == fromVersion.Minor &&
                     migrator.Version.Revision == fromVersion.Revision))
					continue;
				
				migrator.Upgrade(db);
                if (this.Verbose)
                {
                    Console.WriteLine("Upgrade Version: " + migrator.Version.ToString());
                }
			}
		}


		/// <summary>
		/// Perform a downgrade on the database object. The downgrade runs from the latest
		/// version defined to the specified version.
		/// </summary>
		/// <param name="db">
		/// A <see cref="Database"/> object that identifies which database is to be downgraded.
		/// </param>
		/// <param name="toVersion">
		/// The target version number or null if this is a complete uninstall.
		/// </param>
		public void Downgrade(Database db, PackageVersion toVersion)
		{
			int i;


            if (toVersion == null)
                toVersion = new PackageVersion("0.0.0");

            for (i = Migrators.Count - 1; i >= 0; i--)
			{
				DatabaseMigrator migrator = Migrators[i];

                //
                // Skip versions that are less than or equal to the version we are downgrading to.
                // Special check on version match so that the step is ignored for an exact match.
                //
                if (migrator.Version.CompareTo(toVersion) <= 0 ||
                    (migrator.Version.Major == toVersion.Major &&
                     migrator.Version.Minor == toVersion.Minor &&
                     migrator.Version.Revision == toVersion.Revision))
                    continue;
				
				migrator.Downgrade(db);
                if (this.Verbose)
                {
                    Console.WriteLine("Downgrade Version: " + migrator.Version.ToString());
                }
			}
		}
		
		
		/// <summary>
		/// Perform any configuration on the database object. If The configuration runs from
		/// the specified version to the latest version defined.
		/// </summary>
		/// <param name="db">
		/// A <see cref="Database"/> object that identifies which database is to be configured.
		/// </param>
		/// <param name="fromVersion">
        /// The current version number of the package being configured or null if there is
        /// nothing currently installed or if a new recommended package has been installed.
		/// </param>
		/// <param name="dependency">
		/// A <see cref="String"/> that contains the dependency this configuration operation is
		/// for or null if this is a new install/upgrade.
		/// </param>
		public void Configure(Database db, PackageVersion fromVersion, String dependency)
		{
            if (fromVersion == null)
                fromVersion = new PackageVersion("0.0.0");

            foreach (DatabaseMigrator migrator in Migrators)
			{
                //
                // Skip versions that are less than or equal to the version we are upgrading from.
                // Also check for the same version since 1.0.0 is less than 1.0.0 step 1.
                //
                if (migrator.Version.CompareTo(fromVersion) <= 0 ||
                    (migrator.Version.Major == fromVersion.Major &&
                     migrator.Version.Minor == fromVersion.Minor &&
                     migrator.Version.Revision == fromVersion.Revision))
                    continue;
				
				migrator.Configure(db, dependency);
                if (this.Verbose)
                {
                    Console.WriteLine("Configure Version: " + migrator.Version.ToString());
                }
			}
		}

		
		/// <summary>
		/// Perform an un-configure on the database object. The un-configure runs from the latest
		/// version defined to the specified version.
		/// </summary>
		/// <param name="db">
		/// A <see cref="Database"/> object that identifies which database is to be un-configured.
		/// </param>
		/// <param name="toVersion">
        /// The target version number that we are un-configuring to or null if this is
        /// a complete uninstall (or if a recommended dependency has been removed).
		/// </param>
		/// <param name="dependency">
		/// A <see cref="String"/> that contains the dependency this un-configuration operation is
		/// for or null if this is a full uninstall/downgrade.
		/// </param>
		public void Unconfigure(Database db, PackageVersion toVersion, String dependency)
		{
			int i;


            if (toVersion == null)
                toVersion = new PackageVersion("0.0.0");

            for (i = Migrators.Count - 1; i >= 0; i--)
			{
				DatabaseMigrator migrator = Migrators[i];

				//
				// Skip versions that are less than or equal to the version we are downgrading to.
                // Special check on version match so that the step is ignored for an exact match.
                //
                if (migrator.Version.CompareTo(toVersion) <= 0 ||
                    (migrator.Version.Major == toVersion.Major &&
                     migrator.Version.Minor == toVersion.Minor &&
                     migrator.Version.Revision == toVersion.Revision))
                    continue;
				
				migrator.Unconfigure(db, dependency);
                if (this.Verbose)
                {
                    Console.WriteLine("Unconfigure Version: " + migrator.Version.ToString());
                }
			}
		}
	}
	
	
	/// <summary>
	/// This attribute must be used on a direct subclass of DatabaseMigrator to identify
	/// which version and sequenced step the migration is for.
	/// </summary>
	public class MigratorVersionAttribute : Attribute
	{
		/// <summary>
		/// The version number and step in the sequence.
		/// </summary>
		public MigratorVersionStep Version { get; set; }
		
		
		/// <summary>
		/// Initialize a new MigratorVersionAttribute object with the specified version
		/// numbering.
		/// </summary>
		/// <param name="major">
		/// A <see cref="Int32"/> value that specifies the major version number.
		/// </param>
		/// <param name="minor">
		/// A <see cref="Int32"/> value that specifies the minor version number.
		/// </param>
		/// <param name="revision">
		/// A <see cref="Int32"/> value that specifies the revision number.
		/// </param>
		/// <param name="step">
		/// A <see cref="Int32"/> value that specifies which step in the upgrade/downgrade
		/// sequence.
		/// </param>
		public MigratorVersionAttribute(Int32 major, Int32 minor, Int32 revision, Int32 step)
			: base()
		{
			Version = new MigratorVersionStep(major, minor, revision, step);
		}
	}
	
	
	/// <summary>
	/// This class is used to identify version numbers and the sequence step in
	/// that version number. It is primarily used internally for sorting operations.
	/// </summary>
	public class MigratorVersionStep : PackageVersion, IComparable
	{
		/// <summary>
		/// The sequence step of this version migration during an upgrade or downgrade
		/// operation. It is the N in a version number of "X.Y.Z Step N".
		/// </summary>
		public Int32 Step { get; set; }
		

		/// <summary>
		/// Initialize a new object with the specified values.
		/// </summary>
		/// <param name="major">
		/// A <see cref="Int32"/> value that specifies the major version number.
		/// </param>
		/// <param name="minor">
		/// A <see cref="Int32"/> value that specifies the minor version number.
		/// </param>
		/// <param name="revision">
		/// A <see cref="Int32"/> value that specifies the revision number.
		/// </param>
		/// <param name="step">
		/// A <see cref="Int32"/> value that specifies the sequence step.
		/// </param>
		public MigratorVersionStep(Int32 major, Int32 minor, Int32 revision, Int32 step)
			: base(major, minor, revision)
		{
			Step = step;
		}
		
		
		/// <summary>
		/// Compare this version number with that of another version number.
		/// </summary>
		/// <param name="obj">
		/// A <see cref="MigratorVersionStep"/> or <see cref="PackageVersion"/> cast as an
		/// object to be compared against.
		/// </param>
		/// <returns>
		/// 0 if the two version are the same; -1 if this version is less than the passed
		/// version and 1 if this version is greater than the passed verison.
		/// </returns>
		public new int CompareTo(object obj)
		{
			if (typeof(MigratorVersionStep).IsInstanceOfType(obj))
			{
				MigratorVersionStep other = (MigratorVersionStep)obj;
				int result = base.CompareTo(obj);
				
				
				if (result != 0)
					return result;
				
				return this.Step.CompareTo(other.Step);
			}
			else
			{
				int result = base.CompareTo(obj);
				
				
				if (result != 0)
					return result;
				
				//
				// If the step is greater than 0 and the other object is a base version
				// number (without steps) then this object is greater than the other
				// object.
				//
				if (this.Step > 0)
					return 1;
				
				return 0;
			}
		}
		
		
		/// <summary>
		/// Returns a textual representation of this version number with its step in the process.
		/// </summary>
		/// <returns>
		/// A <see cref="String"/> in the format of "3.1.4 Step 7".
		/// </returns>
		public new String ToString()
		{
			return String.Format("{0}.{1}.{2} Step {3}", Major, Minor, Revision, Step);
		}
	}
	
	
	/// <summary>
	/// This class provides a basis for performing a single step in an upgrade or downgrade
	/// operation. It should always be inherited directly by a class that will perform
	/// some operations.
	/// </summary>
	public abstract class DatabaseMigrator
	{
		/// <summary>
		/// The version and step number of this migration. It is automatically retrieved from
		/// the custom attributes. Null is returned if no version number was associated with
		/// this class.
		/// </summary>
		public MigratorVersionStep Version
		{
			get
			{
				if (_Version == null)
				{
					object[] attribs;
					
					attribs = GetType().GetCustomAttributes(typeof(MigratorVersionAttribute), false);
					if (attribs.Length == 0)
						return null;

					_Version = ((MigratorVersionAttribute)attribs[0]).Version;
				}
				
				return _Version;
			}
		}
		private MigratorVersionStep _Version;
		
		
		/// <summary>
		/// This method is called when during an upgrade to the version that this class
		/// is defined for.
		/// </summary>
		/// <param name="db">
		/// A <see cref="Database"/> object to be used for database commands.
		/// </param>
		public virtual void Upgrade(Database db)
		{
		}
		
		
		/// <summary>
		/// This method is called when during an dwngrade from the version that this class
		/// is defined for. This is primarily for when an upgrade fails or during an
		/// uninstall. This method should undo anything that was done in the Upgrade method.
		/// </summary>
		/// <param name="db">
		/// A <see cref="Database"/> object to be used for database commands.
		/// </param>
		public virtual void Downgrade(Database db)
		{
		}
		
		
		/// <summary>
		/// After the Upgrade sequence is completed and all files, pages and modules have
		/// been installed in the database this method is called. You should not place any
		/// database functions in this method that future version Upgrade methods will depend
		/// on. This method should also use conditionals to ensure that configure commands
		/// are not duplicated as it can be called more than once. For example if a recommended
		/// support package is later installed, the configure commands will be executed again
		/// to allow any configuration that depends on that package to be performed.
		/// </summary>
		/// <param name="db">
		/// A <see cref="Database"/> object to be used for database commands.
		/// </param>
		/// <param name="dependency">
		/// A <see cref="String"/> object that identifies if this method was called because
		/// of a dependency being installed. This will contain an empty string if this is
		/// a new install or the package name if a dependency was installed.
		/// </param>
		public virtual void Configure(Database db, String dependency)
		{
		}
		
		
		/// <summary>
		/// Before a downgrade or uninstall operation begins this method is called to allow
		/// the migrator to perform any "reverse configuration" commands needed. This can
		/// include things like removing lookups from existing lookup types. It should undo
		/// anything that was done in the Configure method. This method should use conditionals
		/// to determine what to unconfigure as it may be called without the package being
		/// downgraded or un-installed.
		/// </summary>
		/// <param name="db">
		/// A <see cref="Database"/> object to be used for database commands.
		/// </param>
		/// <param name="dependency">
		/// A <see cref="String"/> object that identifies if this method was called because
		/// of a dependency being removed. This will contain an empty string if this is
		/// a new install or the package name if a dependency was removed.
		/// </param>
		public virtual void Unconfigure(Database db, String dependency)
		{
		}
	}
}
