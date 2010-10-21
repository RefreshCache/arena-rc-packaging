using System;
using System.Text;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using NUnit.Framework;
using RefreshCache.Packager;
using RefreshCache.Packager.Manager;


namespace RefreshCache.Packager.Tests
{
    [TestFixture]
    public class ManagerTests
    {
        [Test]
        public void FileChangeRestoreOriginal()
        {
            FileChange f;
            String filePath;
            String origMD5;


            //
            // Put some original content in.
            //
            filePath = Path.GetTempFileName();
            using (FileStream writer = new FileInfo(filePath).OpenWrite())
            {
                byte[] data = new System.Text.UTF8Encoding().GetBytes("This is a test message.");

                writer.Write(data, 0, data.Length);
            }
            origMD5 = InstallTest.MD5FromPath(filePath);
            f = new FileChange(new FileInfo(filePath));

            //
            // Update the content.
            //
            using (FileStream writer = new FileInfo(filePath).OpenWrite())
            {
                byte[] data = new System.Text.UTF8Encoding().GetBytes("Some new content to replace the old.");

                writer.Write(data, 0, data.Length);
            }

            //
            // Verify the changed data.
            //
            Assert.IsTrue((origMD5.Equals(InstallTest.MD5FromPath(filePath)) == false), "Failed to update the file contents.");

            //
            // Restore the file and verify original contents.
            //
            f.Restore();
            Assert.IsTrue(origMD5.Equals(InstallTest.MD5FromPath(filePath)), "Restore operation failed.");

            new FileInfo(filePath).Delete();
        }


        [Test]
        public void FileChangeRestoreMissing()
        {
            FileChange f;
            String filePath;


            //
            // Make sure the original file does not exist.
            //
            filePath = Path.GetTempFileName();
            new FileInfo(filePath).Delete();
            f = new FileChange(new FileInfo(filePath));

            //
            // Update the content.
            //
            using (FileStream writer = new FileInfo(filePath).OpenWrite())
            {
                byte[] data = new System.Text.UTF8Encoding().GetBytes("Some new content to replace the old.");

                writer.Write(data, 0, data.Length);
            }

            //
            // Verify the file now exists.
            //
            Assert.IsTrue(new FileInfo(filePath).Exists, "Failed to create a new file.");

            //
            // Restore the file and verify it no longer exists.
            //
            f.Restore();
            Assert.IsTrue((new FileInfo(filePath).Exists == false), "Restore operation failed.");
        }

        
        [Test]
        public void DirectoryChangeRestoreOriginal()
        {
            DirectoryChange d;
            String path;


            //
            // Create an empty directory.
            //
            path = Path.GetTempFileName();
            new FileInfo(path).Delete();
            new DirectoryInfo(path).Create();
            Assert.IsTrue(new DirectoryInfo(path).Exists, "Could not create empty directory.");
            d = new DirectoryChange(new DirectoryInfo(path));

            //
            // Delete the directory and then restore it.
            //
            new DirectoryInfo(path).Delete();
            d.Restore();
            Assert.IsTrue(new DirectoryInfo(path).Exists, "Restore operation failed.");

            new DirectoryInfo(path).Delete();
        }


        [Test]
        public void DirectoryChangeRestoreEmpty()
        {
            DirectoryChange d;
            String path;


            //
            // Determine a directory path.
            //
            path = Path.GetTempFileName();
            new FileInfo(path).Delete();
            d = new DirectoryChange(new DirectoryInfo(path));

            //
            // Create the directory and then restore it.
            //
            new DirectoryInfo(path).Create();
            d.Restore();
            Assert.IsTrue((new DirectoryInfo(path).Exists == false), "Restore operation failed.");
        }


        #region Exception Tests

        [Test]
        public void PackageVersionExceptionTest()
        {
            String msg = "Test Message";
            Exception inner = new Exception("Top Exception");
            PackageVersionException e, e2;


            e = new PackageVersionException();
            e = new PackageVersionException(msg);
            Assert.AreEqual(msg, e.Message);

            e = new PackageVersionException(msg, inner);
            Assert.AreEqual(msg, e.Message);
            Assert.AreEqual(inner.Message, e.InnerException.Message);

            BinaryFormatter formatter = new BinaryFormatter(null, new StreamingContext(StreamingContextStates.File));
            MemoryStream stream = new MemoryStream();
            formatter.Serialize(stream, e);
            stream.Position = 0;
            e2 = (PackageVersionException)formatter.Deserialize(stream);

            Assert.AreEqual(e.Message, e2.Message);
        }


        [Test]
        public void PackageDependencyExceptionTest()
        {
            String msg = "Test Message";
            Exception inner = new Exception("Top Exception");
            PackageDependencyException e, e2;


            e = new PackageDependencyException();
            e = new PackageDependencyException(msg);
            Assert.AreEqual(msg, e.Message);

            e = new PackageDependencyException(msg, inner);
            Assert.AreEqual(msg, e.Message);
            Assert.AreEqual(inner.Message, e.InnerException.Message);

            BinaryFormatter formatter = new BinaryFormatter(null, new StreamingContext(StreamingContextStates.File));
            MemoryStream stream = new MemoryStream();
            formatter.Serialize(stream, e);
            stream.Position = 0;
            e2 = (PackageDependencyException)formatter.Deserialize(stream);

            Assert.AreEqual(e.Message, e2.Message);
        }


        [Test]
        public void PackageNotInstalledExceptionTest()
        {
            String msg = "Test Message";
            Exception inner = new Exception("Top Exception");
            PackageNotInstalledException e, e2;


            e = new PackageNotInstalledException();
            e = new PackageNotInstalledException(msg);
            Assert.AreEqual(msg, e.Message);

            e = new PackageNotInstalledException(msg, inner);
            Assert.AreEqual(msg, e.Message);
            Assert.AreEqual(inner.Message, e.InnerException.Message);

            BinaryFormatter formatter = new BinaryFormatter(null, new StreamingContext(StreamingContextStates.File));
            MemoryStream stream = new MemoryStream();
            formatter.Serialize(stream, e);
            stream.Position = 0;
            e2 = (PackageNotInstalledException)formatter.Deserialize(stream);

            Assert.AreEqual(e.Message, e2.Message);
        }


        [Test]
        public void PackageLocalConflictExceptionTest()
        {
            String msg = "Test Message";
            Exception inner = new Exception("Top Exception");
            PackageLocalConflictException e, e2;


            e = new PackageLocalConflictException();
            e = new PackageLocalConflictException(msg);
            Assert.AreEqual(msg, e.Message);

            e = new PackageLocalConflictException(msg, inner);
            Assert.AreEqual(msg, e.Message);
            Assert.AreEqual(inner.Message, e.InnerException.Message);

            BinaryFormatter formatter = new BinaryFormatter(null, new StreamingContext(StreamingContextStates.File));
            MemoryStream stream = new MemoryStream();
            formatter.Serialize(stream, e);
            stream.Position = 0;
            e2 = (PackageLocalConflictException)formatter.Deserialize(stream);

            Assert.AreEqual(e.Message, e2.Message);
        }

        #endregion
    }
}
