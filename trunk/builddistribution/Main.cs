using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Windows.Forms;

namespace BuildDistribution
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			XmlDocument source;
			StringBuilder sb = new StringBuilder();
			StringWriter writer = new StringWriter(sb);
			XmlNodeList files;
			String sourceFile = null;
			int i;
			

			//sourceFile = "../../source.xml";
			if (sourceFile == null)
			{
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
				Application.Run(new mainForm());
			}
			else
			{
				//
				// Load the source file.
				//
				source = new XmlDocument();
				source.Load(sourceFile);
	
				//
				// Find all File nodes and update.
				//
				files = source.SelectNodes("/descendant::File");
				for (i = 0; i < files.Count; i++)
				{
					byte[] buffer = null;
					FileInfo info;
					
					//
					// Append real file data to all File nodes.
					//
					info = new FileInfo(files[i].Attributes["_source"].InnerText);
					using (FileStream stream = info.OpenRead())
					{
						buffer = new byte[stream.Length];
						stream.Read(buffer, 0, Convert.ToInt32(stream.Length));
						stream.Close();
					}
					
					files[i].InnerText = Convert.ToBase64String(buffer);
					files[i].Attributes.RemoveNamedItem("_source");
				}
	
				//
				// Save the modified XML information.
				//
				source.Save(writer);
	
				Console.WriteLine (sb.ToString());
			}
		}
	}
}

