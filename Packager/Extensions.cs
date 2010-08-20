using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;


namespace RefreshCache.Packager
{
    /// <summary>
    /// Extension to the XmlNode class that provides helpful methods when
    /// dealing with nodes.
    /// </summary>
    public static class XmlNodeExtensions
    {
        /// <summary>
        /// Create a copy of a node and rename it. Useful since the Name
        /// property is read-only. Grumble Grumble Grumble.
        /// </summary>
        /// <param name="node">The node to be renamed.</param>
        /// <param name="newName">The new name of the node.</param>
        /// <returns>A copy of the node which has the new name.</returns>
        public static XmlNode CopyAndRename(this XmlNode node, String newName)
        {
            XmlDocument doc = node.OwnerDocument;
            XmlNode newNode = doc.CreateNode(node.NodeType, newName, null);

            while (node.HasChildNodes)
            {
                newNode.AppendChild(node.FirstChild);
            }

            XmlAttributeCollection ac = node.Attributes;
            while (ac.Count > 0)
            {
                newNode.Attributes.Append(ac[0]);
            }

            XmlNode parent = node.ParentNode;
            if (parent != null)
            {
                parent.ReplaceChild(newNode, node);
            }

            return newNode;
        }
    }
}
