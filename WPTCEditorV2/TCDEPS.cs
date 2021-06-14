using System;
using System.Collections;
using System.IO;
using System.Xml;

namespace WPTCEditorTool
{
	// Token: 0x02000004 RID: 4
	internal class TCDEPS
	{
		// Token: 0x06000003 RID: 3 RVA: 0x00002200 File Offset: 0x00000400
		public void AddTags(string sDepXml)
		{
			XmlDocument xmlDocument = new XmlDocument();
			this.jobLog.Add("Loading doc: " + sDepXml);
			xmlDocument.Load(sDepXml);
			XmlNodeList xmlNodeList = xmlDocument.SelectNodes("/Required/RemoteFile");
			if (xmlNodeList.Count > 0)
			{
				int num = 0;
				foreach (object obj in xmlNodeList)
				{
					XmlNode xmlNode = (XmlNode)obj;
					if (xmlNode.Attributes["Tags"] == null)
					{
						XmlAttribute xmlAttribute = xmlDocument.CreateAttribute("Tags");
						xmlAttribute.Value = "FOAllJobs";
						xmlNode.Attributes.Append(xmlAttribute);
						num++;
					}
				}
				this.jobLog.Add(num.ToString() + " tags were added for " + sDepXml);
				xmlDocument.Save(sDepXml);
			}
		}

		// Token: 0x06000004 RID: 4 RVA: 0x000022F8 File Offset: 0x000004F8
		public void AddTagsLocalInstall(string xmlName)
		{
			string filePath = this.GetFilePath(xmlName, this.localPackagesRootPath);
			if (filePath == null)
			{
				Console.WriteLine("ERROR: File not found: " + xmlName);
				return;
			}
			this.AddTags(filePath);
		}

		// Token: 0x06000005 RID: 5 RVA: 0x00002330 File Offset: 0x00000530
		public void AddTagsFullInstall(string packagesRootPath)
		{
			if (string.IsNullOrEmpty(packagesRootPath))
			{
				packagesRootPath = this.localPackagesRootPath;
			}
			if (!Directory.Exists(packagesRootPath))
			{
				throw new Exception("Path does not exist: " + packagesRootPath + ". Please ensure that Test Central is installed.");
			}
			DirectoryInfo directoryInfo = new DirectoryInfo(packagesRootPath);
			FileInfo[] files = directoryInfo.GetFiles(this.depsType, SearchOption.AllDirectories);
			foreach (FileInfo fileInfo in files)
			{
				this.AddTags(fileInfo.FullName);
			}
		}

		// Token: 0x06000006 RID: 6 RVA: 0x000023A5 File Offset: 0x000005A5
		private static string ProgramFilesX86()
		{
			if (Directory.Exists("C:\\Program Files (x86)"))
			{
				return "C:\\Program Files (x86)";
			}
			return "C:\\Program Files";
		}

		// Token: 0x06000007 RID: 7 RVA: 0x000023C0 File Offset: 0x000005C0
		public string GetFilePath(string fileName, string rootPath)
		{
			if (!Directory.Exists(rootPath))
			{
				Console.WriteLine("ERROR: Path does not exist: " + rootPath + ". Please ensure that Test Central is installed.");
				return null;
			}
			foreach (string path in Directory.GetDirectories(rootPath))
			{
				foreach (string text in Directory.GetFiles(path))
				{
					if (text.ToLower().Contains(fileName.ToLower()))
					{
						return text;
					}
				}
			}
			return null;
		}

		// Token: 0x06000008 RID: 8 RVA: 0x00002448 File Offset: 0x00000648
		public void PrintGeneralLog()
		{
			string path = Path.Combine(Directory.GetCurrentDirectory(), "WPTCEditorLog.txt");
			FileStream fileStream = File.Create(path);
			fileStream.Close();
			foreach (object obj in this.jobLog)
			{
				string value = (string)obj;
				using (StreamWriter streamWriter = File.AppendText(path))
				{
					streamWriter.WriteLine(value);
				}
			}
		}

		// Token: 0x04000004 RID: 4
		public readonly string localPackagesRootPath = TCDEPS.ProgramFilesX86() + "\\Windows Phone Blue Test Central Test Content\\Packages\\PreBuilt";

		// Token: 0x04000005 RID: 5
		private readonly string depsType = "*dep.xml";

		// Token: 0x04000006 RID: 6
		private ArrayList jobLog = new ArrayList();
	}
}
