using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace Microsoft.WindowsPhone.Security.SecurityPolicyCompiler
{
	// Token: 0x02000017 RID: 23
	public class ApplicationFiles : IPolicyElement
	{
		// Token: 0x17000023 RID: 35
		// (get) Token: 0x060000A4 RID: 164 RVA: 0x00004A91 File Offset: 0x00002C91
		[XmlElement(ElementName = "Binary")]
		public List<ApplicationFile> ApplicationFileCollection
		{
			get
			{
				return this.applicationFileCollection;
			}
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x00004AB3 File Offset: 0x00002CB3
		public virtual void Add(IXPathNavigable applicationXmlElement)
		{
			this.AddElements((XmlElement)applicationXmlElement);
		}

		// Token: 0x060000A7 RID: 167 RVA: 0x00004AC4 File Offset: 0x00002CC4
		private void AddElements(XmlElement applicationXmlElement)
		{
			foreach (object obj in applicationXmlElement.SelectNodes("./WP_Policy:Files/WP_Policy:File", GlobalVariables.NamespaceManager))
			{
				XmlElement fileXmlElement = (XmlElement)obj;
				ApplicationFile applicationFile = new ApplicationFile();
				applicationFile.Add(fileXmlElement);
				if (applicationFile.Path != "Not Calculated")
				{
					this.applicationFileCollection.Add(applicationFile);
				}
			}
			foreach (object obj2 in applicationXmlElement.SelectNodes("./WP_Policy:Executable", GlobalVariables.NamespaceManager))
			{
				XmlElement fileXmlElement2 = (XmlElement)obj2;
				ApplicationFile applicationFile2 = new ApplicationFile();
				applicationFile2.Add(fileXmlElement2);
				if (applicationFile2.Path != "Not Calculated")
				{
					this.applicationFileCollection.Add(applicationFile2);
				}
			}
			if (this.checkBinaryFile && this.applicationFileCollection.Count == 0)
			{
				throw new PolicyCompilerInternalException(string.Format(GlobalVariables.Culture, "No binary file has been defined in the '{0}' element.", new object[]
				{
					applicationXmlElement.LocalName
				}));
			}
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x00004C00 File Offset: 0x00002E00
		public string GetAllBinPaths()
		{
			string text = string.Empty;
			if (this.applicationFileCollection != null)
			{
				foreach (ApplicationFile applicationFile in this.applicationFileCollection)
				{
					text += applicationFile.Path;
				}
			}
			return text;
		}

		// Token: 0x060000A9 RID: 169 RVA: 0x00004C68 File Offset: 0x00002E68
		public virtual void Print()
		{
			ReportingBase instance = ReportingBase.GetInstance();
			instance.XmlElementLine(ConstantStrings.IndentationLevel3, "Binaries");
			foreach (ApplicationFile applicationFile in this.applicationFileCollection)
			{
				instance.DebugLine(string.Empty);
				applicationFile.Print();
			}
		}

		// Token: 0x040000D8 RID: 216
		protected bool checkBinaryFile = true;

		// Token: 0x040000D9 RID: 217
		protected List<ApplicationFile> applicationFileCollection = new List<ApplicationFile>();
	}
}
