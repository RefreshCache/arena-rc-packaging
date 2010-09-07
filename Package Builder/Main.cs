using System;
using System.IO;
using System.Text;
using System.Windows.Forms;


namespace RefreshCache.Packager.Builder
{
	class MainClass
	{
        [STAThread()]
		public static void Main (string[] args)
		{
        	Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new mainForm());
		}
	}
}

