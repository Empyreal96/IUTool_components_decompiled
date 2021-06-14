using System;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace Microsoft.WindowsPhone.Security.SecurityPolicyCompiler
{
	// Token: 0x02000018 RID: 24
	public class ApplicationFile : IPolicyElement
	{
		// Token: 0x17000024 RID: 36
		// (get) Token: 0x060000AA RID: 170 RVA: 0x00004CDC File Offset: 0x00002EDC
		// (set) Token: 0x060000AB RID: 171 RVA: 0x00004CE4 File Offset: 0x00002EE4
		[XmlAttribute(AttributeName = "Path")]
		public string Path
		{
			get
			{
				return this.path;
			}
			set
			{
				this.path = value;
			}
		}

		// Token: 0x060000AD RID: 173 RVA: 0x00004D00 File Offset: 0x00002F00
		public void Add(IXPathNavigable fileXmlElement)
		{
			this.AddAttributes((XmlElement)fileXmlElement);
			this.CompileAttributes();
		}

		// Token: 0x060000AE RID: 174 RVA: 0x00004D14 File Offset: 0x00002F14
		private void AddAttributes(XmlElement fileXmlElement)
		{
			if (fileXmlElement.HasAttribute("Path"))
			{
				this.Path = fileXmlElement.GetAttribute("Path");
				return;
			}
			this.sourcePath = fileXmlElement.GetAttribute("Source");
			this.destDir = fileXmlElement.GetAttribute("DestinationDir");
			this.name = fileXmlElement.GetAttribute("Name");
		}

		// Token: 0x060000AF RID: 175 RVA: 0x00004D74 File Offset: 0x00002F74
		private void CompileAttributes()
		{
			if (this.Path == "Not Calculated")
			{
				if (!string.IsNullOrEmpty(this.name))
				{
					this.Path = this.name;
				}
				else
				{
					int num = this.sourcePath.LastIndexOf("\\", GlobalVariables.GlobalStringComparison);
					if (num == -1)
					{
						this.Path = this.sourcePath;
					}
					else if (this.sourcePath.Length > num + 1)
					{
						this.Path = this.sourcePath.Substring(num + 1);
					}
				}
				if (string.IsNullOrEmpty(this.destDir))
				{
					this.Path = "$(runtime.default)\\" + this.Path;
				}
				else if (this.destDir.EndsWith("\\", GlobalVariables.GlobalStringComparison))
				{
					this.Path = this.destDir + this.Path;
				}
				else
				{
					this.Path = this.destDir + "\\" + this.Path;
				}
			}
			try
			{
				this.Path = GlobalVariables.ResolveMacroReference(this.path, string.Format(GlobalVariables.Culture, "Element={0}, Attribute={1}", new object[]
				{
					"Binary",
					"Path"
				}));
			}
			catch (PolicyCompilerInternalException)
			{
				this.Path = "Not Calculated";
				return;
			}
			if (!ApplicationFile.IsBinary(this.path))
			{
				this.Path = "Not Calculated";
				return;
			}
			this.Path = NormalizedString.Get(this.path);
		}

		// Token: 0x060000B0 RID: 176 RVA: 0x00004EF0 File Offset: 0x000030F0
		public void Print()
		{
			ReportingBase instance = ReportingBase.GetInstance();
			instance.XmlElementLine(ConstantStrings.IndentationLevel4, "Binary");
			instance.XmlAttributeLine(ConstantStrings.IndentationLevel5, "Path", this.path);
		}

		// Token: 0x060000B1 RID: 177 RVA: 0x00004F1C File Offset: 0x0000311C
		private static bool IsBinary(string filePath)
		{
			string[] array = new string[]
			{
				".exe"
			};
			if (!string.IsNullOrEmpty(filePath))
			{
				foreach (string value in array)
				{
					if (filePath.EndsWith(value, StringComparison.OrdinalIgnoreCase))
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x040000DA RID: 218
		private string sourcePath;

		// Token: 0x040000DB RID: 219
		private string destDir;

		// Token: 0x040000DC RID: 220
		private string name;

		// Token: 0x040000DD RID: 221
		private string path = "Not Calculated";
	}
}
