using RefreshCache.Packager.Installer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RefreshCache.Packager
{
    public class PackageInfo
    {
        public String Distributor { get; set; }
        public String PackageName { get; set; }
        public PackageVersion Version { get; set; }
        public String Synopsis { get; set; }
        public String Description { get; set; }
        public PackageVersionRequirement Arena { get; set; }

        public List<PackageRequirement> Requires { get { return _Requires; } }
        private List<PackageRequirement> _Requires;

        public List<PackageRecommendation> Recommends { get { return _Recommends; } }
        private List<PackageRecommendation> _Recommends;

        public List<PackageChangelog> Changelog { get { return _Changelog; } }
        private List<PackageChangelog> _Changelog;
    }

    public class PackageVersionRequirement
    {
        public PackageVersion MinVersion { get; set; }
        public PackageVersion MaxVersion { get; set; }
        public PackageVersion Version { get; set; }

        public void ValidateVersion(PackageVersion version, String prefix)
        {
            if (MinVersion != null && MinVersion.CompareTo(version) >= 0)
            {
                throw new PackageDependencyException(String.Format(
                    "{0} version {1} does not meet minimum version requirement of {2}",
                    prefix, version, MinVersion));
            }
            else if (MaxVersion != null && MaxVersion.CompareTo(version) <= 0)
            {
                throw new PackageDependencyException(String.Format(
                    "{0} version {1} exceeds maximum version requirement of {2}",
                    prefix, version, MaxVersion));
            }
            else if (Version != null && Version.CompareTo(version) == 0)
            {
                throw new PackageDependencyException(String.Format(
                    "{0} version {1} does not meet exact version requirement of {2}",
                    prefix, version, Version));
            }
        }
    }

    public class PackageRequirement : PackageVersionRequirement
    {
        public String Name { get; set; }
    }

    public class PackageRecommendation : PackageRequirement
    {
        public String Description { get; set; }
    }

    public class PackageChangelog
    {
        public PackageVersion Version { get; set; }
        public String Description { get; set; }
    }
}
