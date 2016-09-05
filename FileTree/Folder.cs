using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace FileTree
{
    public class Folder
    {
        public string Path { get; set; }
        public string Name { get { return System.IO.Path.GetFileName(this.Path); } }
        public ICollection<Folder> SubFolders { get; set; }
        public bool HasSubFolders { get { return this.SubFolders.Count > 0; } }
        public IEnumerable<string> Files { get; set; }
        public bool HasFiles { get { return this.Files != null && this.Files.Any(); } }
        public Folder ParentFolder
        {
            get
            {
                DirectoryInfo parentDirectoryInfo = new DirectoryInfo(this.Path).Parent;
                return parentDirectoryInfo != null ? new Folder(parentDirectoryInfo.FullName) : null;
            }
        }

        public Folder(string folderPath)
        {
            if (!Directory.Exists(folderPath))
                throw new DirectoryNotFoundException(string.Format("The directory was not found: {0}", folderPath));
            this.Path = folderPath;

            try
            {
                this.Files = Directory.GetFiles(this.Path);
            }
            catch (UnauthorizedAccessException)
            {
                // Ignore
            }

            IEnumerable<string> subFolders = null;
            try
            {
                subFolders = Directory.GetDirectories(this.Path);
            }
            catch (UnauthorizedAccessException)
            {
                // Ignore
            }
            if (subFolders != null)
            {
                this.SubFolders = new List<Folder>();
                foreach (var subFolder in subFolders)
                    this.SubFolders.Add(new Folder(subFolder));
            }
        }

        public XmlDocument GetFileTreeXmlWithThisFolderAsRoot()
        {
            XmlDocument xmlDocument = new XmlDocument();

            XmlNode root = xmlDocument.CreateElement("FileTree");
            root.AppendChild(this.GetFolderNode(this, xmlDocument));
            xmlDocument.AppendChild(root);

            return xmlDocument;
        }

        private XmlNode GetFolderNode(Folder folder, XmlDocument xmlDocument)
        {
            XmlNode folderNode = xmlDocument.CreateElement("Folder");
            XmlNode folderName = xmlDocument.CreateElement("Name");
            folderName.InnerText = this.Name;
            folderNode.AppendChild(folderName);

            if (this.HasFiles)
            {
                XmlNode files = xmlDocument.CreateElement("Files");
                foreach (string filePath in this.Files)
                {
                    XmlNode fileNode = xmlDocument.CreateElement("File");
                    fileNode.InnerText = System.IO.Path.GetFileName(filePath);
                    files.AppendChild(fileNode);
                }
                folderNode.AppendChild(files);
            }
            if (this.HasSubFolders)
            {
                XmlNode subfolders = xmlDocument.CreateElement("Subfolders");
                foreach (Folder subFolder in this.SubFolders)
                    subfolders.AppendChild(subFolder.GetFolderNode(subFolder, xmlDocument));
                folderNode.AppendChild(subfolders);
            }

            return folderNode;
        }
    }
}