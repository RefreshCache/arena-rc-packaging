using RefreshCache.Packager.Migrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RefreshCache.Packager
{
    /// <summary>
    /// Identifies the version number for use with a migration. It is comparable
    /// and can be used when comparing and working with version numbers.
    /// </summary>
    public class PackageVersion : IComparable
    {
        /// <summary>
        /// The minor version number is the X in a version number of "X.Y.Z Step N".
        /// </summary>
        public Int32 Major { get; set; }

        /// <summary>
        /// The minor version number is the Y in a version number of "X.Y.Z Step N".
        /// </summary>
        public Int32 Minor { get; set; }

        /// <summary>
        /// The revision number is the Z in a version number of "X.Y.Z Step N".
        /// </summary>
        public Int32 Revision { get; set; }


        /// <summary>
        /// Create a new MigratorVersion object with the specified version values.
        /// </summary>
        /// <param name="major">
        /// A <see cref="Int32"/> representing the major version.
        /// </param>
        /// <param name="minor">
        /// A <see cref="Int32"/> representing the minor version.
        /// </param>
        /// <param name="revision">
        /// A <see cref="Int32"/> representing the revision number.
        /// </param>
        public PackageVersion(Int32 major, Int32 minor, Int32 revision)
        {
            Major = major;
            Minor = minor;
            Revision = revision;
        }


        /// <summary>
        /// Create a new MigratorVersion object from the given version string.
        /// </summary>
        /// <param name="version">
        /// A <see cref="String"/> in the format of "Major[.Minor[.Revision]]".
        /// </param>
        public PackageVersion(String version)
        {
            String[] pts = version.Split('.');


            if (pts.Length >= 1 && version.Length > 0)
                Major = Convert.ToInt32(pts[0]);

            if (pts.Length >= 2)
                Minor = Convert.ToInt32(pts[1]);

            if (pts.Length >= 3)
                Revision = Convert.ToInt32(pts[2]);

            //
            // We allow 4 segments (build #), but no more. And we
            // ignore the build number.
            //
            if (pts.Length > 4)
                throw new Exception("Incorrect number of periods in version number.");
        }


        /// <summary>
        /// Compare this version number with that of another version number.
        /// </summary>
        /// <param name="obj">
        /// A <see cref="PackageVersion"/> cast as an object to be compared against.
        /// </param>
        /// <returns>
        /// 0 if the two version are the same; -1 if this version is less than the passed
        /// version and 1 if this version is greater than the passed verison.
        /// </returns>
        public int CompareTo(object obj)
        {
            PackageVersion other = (PackageVersion)obj;
            int result;


            result = this.Major.CompareTo(other.Major);
            if (result != 0)
                return result;

            result = this.Minor.CompareTo(other.Minor);
            if (result != 0)
                return result;

            result = this.Revision.CompareTo(other.Revision);
            if (result != 0)
                return result;

            //
            // A basic version number is always less than a stepped version number
            // if the step is greater than or equal to 0.
            //
            if (this.GetType() == typeof(PackageVersion) && typeof(MigratorVersionStep).IsInstanceOfType(obj))
            {
                if (((MigratorVersionStep)obj).Step >= 0)
                {
                    return -1;
                }
            }

            return 0;
        }


        /// <summary>
        /// Returns a textual representation of this version number.
        /// </summary>
        /// <returns>
        /// A <see cref="String"/> in the format of "3.1.4".
        /// </returns>
        public new String ToString()
        {
            return String.Format("{0}.{1}.{2}", Major, Minor, Revision);
        }
    }
}
