using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualStudio.TemplateWizard;
using EnvDTE;
using EnvDTE80;

namespace RefreshCache.VisualStudio.Wizards
{
    public class ProjectWizard : IWizard
    {
        public void RunFinished()
        {
        }


        public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
        {
            String DestinationDirectory = replacementsDictionary["$destinationdirectory$"];
            String SafeProjectName = replacementsDictionary["$safeprojectname$"];
            Solution2 soln = (Solution2)((DTE)automationObject).Solution;
            List<String> DeleteOnError = new List<string>();
            String path, dest;
            PackageElementsForm form;
            Project prj = null;


            form = new PackageElementsForm();
            form.ShowDialog();

            try
            {
                //
                // Create the folder for the project.
                //
                prj = soln.AddSolutionFolder(SafeProjectName);
                SolutionFolder sf = (SolutionFolder)prj.Object;

                //
                // Build the user controls project.
                //
                if (form.UserControls)
                {
                    path = soln.GetProjectTemplate("ArenaUserControlsProject.zip", "CSharp");
                    dest = String.Format("{0}\\UserControls", DestinationDirectory);
                    sf.AddFromTemplate(path, dest, SafeProjectName);
                    DeleteOnError.Add(dest);

                    //
                    // Rename the new project.
                    //
                    foreach (ProjectItem pi in prj.ProjectItems)
                    {
                        if (pi.Kind.Equals("{66A26722-8FB5-11D2-AA7E-00C04F688DDE}", StringComparison.InvariantCultureIgnoreCase))
                        {
                            Project p = (Project)pi.Object;

                            if (p.Name == SafeProjectName)
                            {
                                if (form.UseLongNames)
                                    p.Name = "ArenaWeb.UserControls.Custom." + SafeProjectName;
                                else
                                    p.Name = "UserControls";
                            }
                        }
                    }

                }

                //
                // Build the Business Logic project.
                //
                if (form.BusinessLogic)
                {
                    path = soln.GetProjectTemplate("ArenaBLLProject.zip", "CSharp");
                    dest = String.Format("{0}\\Library", DestinationDirectory);
                    sf.AddFromTemplate(path, dest, SafeProjectName);
                    DeleteOnError.Add(dest);

                    //
                    // Rename the new project.
                    //
                    foreach (ProjectItem pi in prj.ProjectItems)
                    {
                        if (pi.Kind.Equals("{66A26722-8FB5-11D2-AA7E-00C04F688DDE}", StringComparison.InvariantCultureIgnoreCase))
                        {
                            Project p = (Project)pi.Object;

                            if (p.Name == SafeProjectName)
                            {
                                if (form.UseLongNames)
                                    p.Name = "Arena.Custom." + SafeProjectName;
                                else
                                    p.Name = "Library";
                            }
                        }
                    }
                }

                //
                // Build the Setup project.
                //
                if (form.Setup)
                {
                    path = soln.GetProjectTemplate("ArenaSetupProject.zip", "CSharp");
                    dest = String.Format("{0}\\Setup", DestinationDirectory);
                    sf.AddFromTemplate(path, dest, SafeProjectName);
                    DeleteOnError.Add(dest);

                    //
                    // Rename the new project.
                    //
                    foreach (ProjectItem pi in prj.ProjectItems)
                    {
                        if (pi.Kind.Equals("{66A26722-8FB5-11D2-AA7E-00C04F688DDE}", StringComparison.InvariantCultureIgnoreCase))
                        {
                            Project p = (Project)pi.Object;

                            if (p.Name == SafeProjectName)
                            {
                                if (form.UseLongNames)
                                    p.Name = "Arena.Custom." + SafeProjectName + ".Setup";
                                else
                                    p.Name = "Setup";
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                foreach (String p in DeleteOnError)
                {
                    Directory.Delete(p, true);
                }
                if (Directory.Exists(DestinationDirectory))
                {
                    try
                    {
                        Directory.Delete(DestinationDirectory, false);
                    }
                    catch { }
                }
                if (prj != null)
                    prj.Delete();

                throw e;
            }
        }

        public void BeforeOpeningFile(ProjectItem projectItem)
        {
        }

        public void ProjectFinishedGenerating(Project project)
        {
        }

        public void ProjectItemFinishedGenerating(ProjectItem projectItem)
        {
        }

        public bool ShouldAddProjectItem(string filePath)
        {
            return true;
        }
    }
}
