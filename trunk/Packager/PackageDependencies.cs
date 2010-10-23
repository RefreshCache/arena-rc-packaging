using RefreshCache.Packager.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace RefreshCache.Packager
{
    /// <summary>
    /// Generic version requirement definition. Each of the version properties
    /// defines the minimum, maximum or specific version number required for
    /// the validation to succeed.
    /// </summary>
    public class VersionRequirement
    {
        #region Properties

        /// <summary>
        /// The minimum version required for the validation to succeed.
        /// </summary>
        /// <remarks>Tested as x &gt;= MinVersion</remarks>
        public PackageVersion MinVersion { get; set; }

        /// <summary>
        /// The maximum version required for the validation to succeed.
        /// </summary>
        /// <remarks>Tested as x &lt;= MaxVersion</remarks>
        public PackageVersion MaxVersion { get; set; }

        /// <summary>
        /// The exact version required for the validation to succeed.
        /// </summary>
        /// <remarks>Tested as x == Version</remarks>
        public PackageVersion Version { get; set; }

        internal String NodeName;

        #endregion


        #region Constructors

        /// <summary>
        /// Create an empty version requirement that will be populated
        /// by the user.
        /// </summary>
        public VersionRequirement()
        {
            NodeName = "VersionRequirement";
        }


        /// <summary>
        /// Create a version requirement from the previously saved one
        /// in the XmlNode object.
        /// </summary>
        /// <param name="node">XmlNode that contains the information to load.</param>
        public VersionRequirement(XmlNode node)
            : this()
        {
            if (node.Attributes["MinVersion"] != null)
                MinVersion = new PackageVersion(node.Attributes["MinVersion"].Value);
            if (node.Attributes["MaxVersion"] != null)
                MaxVersion = new PackageVersion(node.Attributes["MaxVersion"].Value);
            if (node.Attributes["Version"] != null)
                Version = new PackageVersion(node.Attributes["Version"].Value);
        }

        #endregion


        /// <summary>
        /// Perform a validation against the specified PackageVersion. If
        /// the validation fails an exception is thrown using the prefix
        /// string as the first part of the message (e.g. the package name).
        /// </summary>
        /// <exception cref="PackageDependencyException">
        /// Version does not meet the requirements. Message includes a
        /// user-friendly description of why.
        /// </exception>
        /// <param name="version">The package version number to compare against.</param>
        /// <param name="prefix">The prefix applied to the error message.</param>
        public void ValidateVersion(PackageVersion version, String prefix)
        {
            if (MinVersion != null && MinVersion.CompareTo(version) > 0)
            {
                throw new PackageDependencyException(String.Format(
                    "{0} version {1} does not meet minimum version requirement of {2}",
                    prefix, version.ToString(), MinVersion.ToString()));
            }
            else if (MaxVersion != null && MaxVersion.CompareTo(version) < 0)
            {
                throw new PackageDependencyException(String.Format(
                    "{0} version {1} exceeds maximum version requirement of {2}",
                    prefix, version.ToString(), MaxVersion.ToString()));
            }
            else if (Version != null && Version.CompareTo(version) != 0)
            {
                throw new PackageDependencyException(String.Format(
                    "{0} version {1} does not meet exact version requirement of {2}",
                    prefix, version.ToString(), Version.ToString()));
            }
        }


        /// <summary>
        /// Save this VersionRequirement as an XmlNode that can later be
        /// re-instantiated by it's constructor.
        /// </summary>
        /// <param name="doc">The XML Document that this node will be a part of.</param>
        /// <param name="isExport">Specifies if this Save operation is for export of a final package.</param>
        /// <returns>A new XmlNode instance which defines this object.</returns>
        public XmlNode Save(XmlDocument doc, Boolean isExport)
        {
            XmlNode node = doc.CreateElement(NodeName);
            XmlAttribute attrib;


            if (MinVersion != null)
            {
                attrib = doc.CreateAttribute("MinVersion");
                attrib.Value = MinVersion.ToString();
                node.Attributes.Append(attrib);
            }

            if (MaxVersion != null)
            {
                attrib = doc.CreateAttribute("MaxVersion");
                attrib.Value = MaxVersion.ToString();
                node.Attributes.Append(attrib);
            }

            if (Version != null)
            {
                attrib = doc.CreateAttribute("Version");
                attrib.Value = Version.ToString();
                node.Attributes.Append(attrib);
            }

            return node;
        }
    }


    /// <summary>
    /// A version requirement that will apply to a specific package
    /// name. The named package must be installed and the installed
    /// version number must meet the requirements.
    /// </summary>
    public class PackageRequirement : VersionRequirement
    {
        #region Properties

        /// <summary>
        /// The full name of the package that will be tested against
        /// during install time.
        /// </summary>
        /// <example>"RC.PackageManager"</example>
        public String Name { get; set; }

        #endregion


        #region Constructors

        /// <summary>
        /// Create a new, empty, package requirement that will be
        /// populated later by the user.
        /// </summary>
        public PackageRequirement()
            : base()
        {
            NodeName = "Require";
        }

        /// <summary>
        /// Re-load a previously saved package requirement from the
        /// specified XmlNode instance.
        /// </summary>
        /// <param name="node">The node that was created by a previous call to the Save() method.</param>
        public PackageRequirement(XmlNode node)
            : base(node)
        {
            NodeName = "Require";

            if (node.Attributes["Name"] != null)
                Name = node.Attributes["Name"].Value;
            else
                Name = "";
        }

        #endregion


        /// <summary>
        /// Save this PackageRequirement as an XmlNode that can later be
        /// re-instantiated via its constructor.
        /// </summary>
        /// <param name="doc">The XmlDocument that will own the created node.</param>
        /// <param name="isExport">Specifies if this save operation is for a final export.</param>
        /// <returns>A new XmlNode that identifies all information contained in this object.</returns>
        public new XmlNode Save(XmlDocument doc, Boolean isExport)
        {
            XmlNode node = base.Save(doc, isExport);
            XmlAttribute attrib;


            attrib = doc.CreateAttribute("Name");
            attrib.Value = (Name != null ? Name : "");
            node.Attributes.Append(attrib);

            return node;
        }
    }


    /// <summary>
    /// Defines a package that is recommended to be installed along
    /// with the associated package, along with the version requirement
    /// to meet the recommendation.
    /// </summary>
    public class PackageRecommendation : PackageRequirement
    {
        #region Properties

        /// <summary>
        /// A user-friendly description of why this package is being
        /// recommended for installation. This should fully explain to
        /// the user what having this "add-on" will provide.
        /// </summary>
        public String Description { get; set; }

        #endregion


        #region Constructors

        /// <summary>
        /// Create a new, empty, instance of the package recommendation
        /// class that can be populated by the user.
        /// </summary>
        public PackageRecommendation()
            : base()
        {
            NodeName = "Recommend";
        }


        /// <summary>
        /// Instantiate a new package recommendation class with the data
        /// from the XmlNode.
        /// </summary>
        /// <param name="node">The node that was created by a previous call to the Save() method.</param>
        public PackageRecommendation(XmlNode node)
            : base(node)
        {
            NodeName = "Recommend";

            if (node.Attributes["Description"] != null)
                Description = node.Attributes["Description"].Value;
            else
                Description = "";
        }

        #endregion


        /// <summary>
        /// Saves this object into an XmlNode that can be used later
        /// to re-instantiate this PackageRecommendation.
        /// </summary>
        /// <param name="doc">The XmlDocument that the new node will be owned by.</param>
        /// <param name="isExport">Wether or not this Save operation is for a final export.</param>
        /// <returns>A new XmlNode object that contains all the information for this recommendation.</returns>
        public new XmlNode Save(XmlDocument doc, Boolean isExport)
        {
            XmlNode node = base.Save(doc, isExport);
            XmlAttribute attrib;


            attrib = doc.CreateAttribute("Description");
            attrib.Value = (Description != null ? Description : "");
            node.Attributes.Append(attrib);

            return node;
        }
    }
}
