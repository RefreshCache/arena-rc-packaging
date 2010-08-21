using RefreshCache.Packager.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace RefreshCache.Packager
{
    /// <summary>
    /// Defines information about the package that can be displayed
    /// to the user. It also defines information about what the package
    /// needs to be installed properly.
    /// </summary>
    public class PackageInfo
    {
        #region Properties

        /// <summary>
        /// The name of the church (or group) distributing this package.
        /// </summary>
        /// <example>"Refresh Cache"</example>
        public String Distributor { get; set; }

        /// <summary>
        /// The full name of the package, including the distributor short
        /// name.
        /// </summary>
        /// <example>"RC.Package Manager"</example>
        public String PackageName { get; set; }

        /// <summary>
        /// The version of this package.
        /// </summary>
        public PackageVersion Version { get; set; }

        /// <summary>
        /// A short one sentence statement of what this package is for
        /// and what it does. This is displayed to the user before they
        /// view the full details of the package. It should give them
        /// a good idea of what they are getting.
        /// </summary>
        public String Synopsis { get; set; }

        /// <summary>
        /// A full paragraph describing the package and what it provides
        /// to Arena. This is currently a plain text only field, though
        /// it may be expanded later to support some simple html tags
        /// for things like bullet points.
        /// </summary>
        public String Description { get; set; }

        /// <summary>
        /// The version requirements of Arena for this package to be
        /// installed. If this property is null then the package does
        /// not have any Arena version requirement whatsoever.
        /// </summary>
        /// <remarks>
        /// Most packages should include at least a minimum version
        /// number identifying the lowest version number the distributor
        /// is willing to support.
        /// </remarks>
        public VersionRequirement Arena { get; set; }

        /// <summary>
        /// The list of package dependencies that must be installed for
        /// this package to be installed and function properly. Each
        /// PackageRequirement must be met for the package to be installed.
        /// </summary>
        public List<PackageRequirement> Requires { get { return _Requires; } }
        private List<PackageRequirement> _Requires;

        /// <summary>
        /// The list of packages that are recommended to be installed along
        /// with this package. A recommendation can be ignored, but the user
        /// will be informed of all recommendations and be given the chance
        /// to install each of them.
        /// </summary>
        public List<PackageRecommendation> Recommends { get { return _Recommends; } }
        private List<PackageRecommendation> _Recommends;

        /// <summary>
        /// The change log information for this package. Each entry specifies
        /// the changes that were made when the specified version was released.
        /// This collection is used to show the user all changes that were made
        /// to the package before the begin an upgrade process.
        /// </summary>
        public List<PackageChangelog> Changelog { get { return _Changelog; } }
        private List<PackageChangelog> _Changelog;

        #endregion


        #region Constructors

        /// <summary>
        /// Create a new, empty, PackageInfo instance. This method is used when
        /// a new package is created.
        /// </summary>
        public PackageInfo()
        {
            _Requires = new List<PackageRequirement>();
            _Recommends = new List<PackageRecommendation>();
            _Changelog = new List<PackageChangelog>();
        }


        /// <summary>
        /// Load a previously saved PackageInfo instance from the XML
        /// node.
        /// </summary>
        /// <param name="node">The node that is the result of a previous call to Save().</param>
        public PackageInfo(XmlNode node)
            : this()
        {
            XmlNode n;


            //
            // Load all the required properties.
            //
            Distributor = node.SelectSingleNode("child::Distributor").InnerText;
            PackageName = node.SelectSingleNode("child::PackageName").InnerText;
            Version = new PackageVersion(node.SelectSingleNode("child::Version").InnerText);
            Synopsis = node.SelectSingleNode("child::Synopsis").InnerText;
            Description = node.SelectSingleNode("child::Description").InnerText;

            //
            // Load all the optional single-value properties.
            //
            if ((n = node.SelectSingleNode("child::ArenaVersion")) != null)
            {
                Arena = new VersionRequirement(n);
            }

            //
            // Load all the optional multi-value properties.
            //
            foreach (XmlNode mv in node.SelectNodes("child::Require"))
            {
                Requires.Add(new PackageRequirement(mv));
            }
            foreach (XmlNode mv in node.SelectNodes("child::Recommend"))
            {
                Recommends.Add(new PackageRecommendation(mv));
            }
            foreach (XmlNode mv in node.SelectNodes("child::Changelog"))
            {
                Changelog.Add(new PackageChangelog(mv));
            }
        }

        #endregion


        /// <summary>
        /// Save the information identified in this PackageInfo instance
        /// into an XmlNode that can be stored on disk and reloaded later.
        /// </summary>
        /// <param name="doc">The document that the new node will be stored in.</param>
        /// <param name="isExport">Specifies wether or not this save operation is a full export.</param>
        /// <returns>A new XmlNode which contains all the supplied information about this package.</returns>
        public XmlNode Save(XmlDocument doc, Boolean isExport)
        {
            XmlNode node, n;


            node = doc.CreateElement("Info");

            //
            // Save all the required properties.
            //
            n = doc.CreateElement("Distributor");
            n.InnerText = (Distributor != null ? Distributor : "");
            node.AppendChild(n);
            n = doc.CreateElement("PackageName");
            n.InnerText = (PackageName != null ? PackageName : "");
            node.AppendChild(n);
            n = doc.CreateElement("Version");
            n.InnerText = (Version != null ? Version.ToString() : "1.0.0");
            node.AppendChild(n);
            n = doc.CreateElement("Synopsis");
            n.InnerText = (Synopsis != null ? Synopsis : "");
            node.AppendChild(n);
            n = doc.CreateElement("Description");
            n.InnerText = (Description != null ? Description : "");
            node.AppendChild(n);

            //
            // Save all the optional single-value properties.
            //
            if (Arena != null)
            {
                n = Arena.Save(doc, isExport);
                node.AppendChild(n.CopyAndRename("ArenaVersion"));
            }

            //
            // Save all the multi-value properties.
            //
            foreach (PackageRequirement req in Requires)
            {
                node.AppendChild(req.Save(doc, isExport));
            }
            foreach (PackageRecommendation rec in Recommends)
            {
                node.AppendChild(rec.Save(doc, isExport));
            }
            foreach (PackageChangelog chg in Changelog)
            {
                node.AppendChild(chg.Save(doc, isExport));
            }

            return node;
        }
    }
}
