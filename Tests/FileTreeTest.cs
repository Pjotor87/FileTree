using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FileTree;
using System.IO;
using System.Xml;

namespace Tests
{
    [TestClass]
    public class FileTreeTest
    {
        public string RootfolderPath { get { return @"C:\CODE\TestData\FileTree\RootFolder"; } }

        [TestMethod]
        public void CanGetFileTree()
        {
            Folder rootFolder = new Folder(this.RootfolderPath);
            Assert.AreEqual("RootFolder", rootFolder.Name);
            Assert.IsTrue(rootFolder.HasSubFolders);
            Assert.IsTrue(rootFolder.SubFolders.Count == 2);
            Assert.IsTrue(rootFolder.HasFiles);
            foreach (string file in rootFolder.Files)
                Assert.IsTrue(Path.GetFileNameWithoutExtension(file) == "File1");
            foreach (Folder subFolder in rootFolder.SubFolders)
            {
                if (subFolder.Name == "Subfolder1")
                {
                    Assert.IsFalse(subFolder.HasFiles);
                    Assert.IsFalse(subFolder.HasSubFolders);
                }
                else if (subFolder.Name == "Subfolder2")
                {
                    Assert.IsFalse(subFolder.HasFiles);
                    Assert.IsTrue(subFolder.HasSubFolders);
                    Assert.IsTrue(subFolder.SubFolders.Count == 1);
                    foreach (Folder subSubFolder in subFolder.SubFolders)
                    {
                        if (subSubFolder.Name == "Subfolder2-1")
                        {
                            Assert.IsFalse(subSubFolder.HasSubFolders);
                            Assert.IsTrue(subSubFolder.HasFiles);
                        }
                        else
                        {
                            Assert.Fail("Folder is not: Subfolder2-1");
                        }
                    }
                }
                else
                {
                    Assert.Fail("Folder is not: Subfolder1 or Subfolder2");
                }
            }
        }
    }
}
