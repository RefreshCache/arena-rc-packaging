using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RefreshCache.Packager.Manager
{
    /// <summary>
    /// Internal class that saves the changes to a single file. This class
    /// also provides a method to restore the file back to it's original
    /// state.
    /// </summary>
    class FileChange
    {
        /// <summary>
        /// The full path to the object we are saving the state of.
        /// </summary>
        protected String FullName { get; set; }

        /// <summary>
        /// Wether or not this object existed before the operation
        /// we performed.
        /// </summary>
        protected Boolean Exists;

        /// <summary>
        /// The original contents of the file. If the file did not exist
        /// before then this parameter is null.
        /// </summary>
        Byte[] Contents { get; set; }


        /// <summary>
        /// Create an empty class. Used by subclasses.
        /// </summary>
        protected FileChange(String name, Boolean exists)
        {
            FullName = name;
            Exists = exists;
        }


        /// <summary>
        /// Create a new object instance from the FileInfo object. Also
        /// stores the contents of the original file (if it exists) for
        /// possible restoration later.
        /// </summary>
        /// <param name="original">A FileInfo object that identifies the original file.</param>
        public FileChange(FileInfo original)
            : this(original.FullName, original.Exists)
        {
            if (Exists)
            {
                using (FileStream rdr = original.OpenRead())
                {
                    Contents = new Byte[rdr.Length];

                    rdr.Read(Contents, 0, (int)rdr.Length);
                }
            }
        }


        /// <summary>
        /// Restore the file identified by this object back to it's original
        /// state. If the file was originally missing then it will be deleted,
        /// it it's contents have been changed then they will be restored.
        /// </summary>
        public void Restore()
        {
            FileInfo target = new FileInfo(FullName);


            if (Contents != null)
            {
                using (FileStream writer = target.Create())
                {
                    writer.Write(Contents, 0, Contents.Length);
                    writer.Flush();
                }
            }
            else
                target.Delete();
        }
    }


    /// <summary>
    /// Internal class that contains the information about a directory
    /// that has been deleted or created.
    /// </summary>
    class DirectoryChange : FileChange
    {
        /// <summary>
        /// Create a new DirectoryChange object that can later be used to restore
        /// the directory to it's original state (i.e. exists or not).
        /// </summary>
        /// <param name="di">The directory whose state needs to be saved.</param>
        public DirectoryChange(DirectoryInfo di)
            : base(di.FullName, di.Exists)
        {
        }


        /// <summary>
        /// Restore a directory to it's original state. Either re-create it or
        /// delete it.
        /// </summary>
        public void Restore()
        {
            DirectoryInfo target = new DirectoryInfo(FullName);


            if (Exists)
                target.Create();
            else
                target.Delete();
        }
    }
}
