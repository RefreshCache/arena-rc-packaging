using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace RefreshCache.Packager.Tests
{
    class TestsHelper
    {
        protected void AddXmlAttribute(XmlNode node, String name, String value)
        {
            XmlAttribute xattr;


            xattr = node.OwnerDocument.CreateAttribute(name);
            xattr.Value = value;
            node.Attributes.Append(xattr);
        }
    }
}
