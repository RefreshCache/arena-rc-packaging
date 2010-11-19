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
            mainForm form;


        	Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

            form = new mainForm();
            if (args.Length > 0)
                form.openFromFile(args[0]);
			Application.Run(form);
		}
	}
}

