using System;
using System.IO;
using System.Text;
using System.Xml;


namespace RefreshCache.Packager.Builder
{
	class MainClass
	{
        [STAThread()]
		public static void Main (string[] args)
		{
			if (args.Length != 3)
			{
                Console.WriteLine("Usage: pbcli.exe <source.xml> <BaseDirectory> <output.xml>");
                Environment.Exit(-1);
			}
			else
			{
                BuildMessageCollection messages;
                XmlWriterSettings settings;
                XmlDocument source = null;
                XmlWriter writer;
                Package package;

				//
				// Load the source file.
				//
                try
                {
                    source = new XmlDocument();
                    source.Load(args[0]);
                }
                catch
                {
                    Console.WriteLine("Error trying to read the source XML file.");
                    Environment.Exit(-1);
                }

                //
                // Load the package.
                //
                package = new Package(source);

                //
                // Save the package to XML.
                //
                messages = package.Build(args[1]);

                //
                // Check if there were any errors during the build.
                //
                if (package.XmlPackage == null)
                {
                    Console.WriteLine("Errors occured during build:");
                    Console.WriteLine(messages.ToString());

                    Environment.Exit(-1);
                }

                //
                // Dump result.
                //
                settings = new XmlWriterSettings();
                settings.Encoding = Encoding.UTF8;
                settings.Indent = true;
                writer = XmlWriter.Create(args[2], settings);
                package.XmlPackage.WriteTo(writer);
                writer.Close();
			}
		}
	}
}

