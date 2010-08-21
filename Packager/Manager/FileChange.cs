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
        /// A reference to the information about the original file,
        /// including it's full path.
        /// </summary>
        FileInfo Info { get; set; }

        /// <summary>
        /// The original contents of the file. If the file did not exist
        /// before then this parameter is null.
        /// </summary>
        Byte[] Contents { get; set; }


        /// <summary>
        /// Create a new object instance from the FileInfo object. Also
        /// stores the contents of the original file (if it exists) for
        /// possible restoration later.
        /// </summary>
        /// <param name="original">A FileInfo object that identifies the original file.</param>
        public FileChange(FileInfo original)
        {
            Info = new FileInfo(original.FullName);
            Info.Refresh();

            if (Info.Exists)
            {
                using (FileStream rdr = Info.OpenRead())
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
            FileInfo target = new FileInfo(Info.FullName);


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
}
