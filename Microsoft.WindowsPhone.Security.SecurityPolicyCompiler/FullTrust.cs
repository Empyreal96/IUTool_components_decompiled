using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon;

namespace Microsoft.WindowsPhone.Security.SecurityPolicyCompiler
{
	// Token: 0x0200001D RID: 29
	public class FullTrust : IPolicyElement
	{
		// Token: 0x1700002E RID: 46
		// (get) Token: 0x060000DB RID: 219 RVA: 0x000057E0 File Offset: 0x000039E0
		// (set) Token: 0x060000DC RID: 220 RVA: 0x000057E8 File Offset: 0x000039E8
		[XmlAttribute(AttributeName = "ElementID")]
		public string ElementId
		{
			get
			{
				return this.elementId;
			}
			set
			{
				this.elementId = value;
			}
		}

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x060000DD RID: 221 RVA: 0x000057F1 File Offset: 0x000039F1
		// (set) Token: 0x060000DE RID: 222 RVA: 0x000057F9 File Offset: 0x000039F9
		[XmlAttribute(AttributeName = "AttributeHash")]
		public string AttributeHash
		{
			get
			{
				return this.attributeHash;
			}
			set
			{
				this.attributeHash = value;
			}
		}

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x060000DF RID: 223 RVA: 0x00005802 File Offset: 0x00003A02
		// (set) Token: 0x060000E0 RID: 224 RVA: 0x0000580A File Offset: 0x00003A0A
		[XmlAttribute(AttributeName = "Name")]
		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x060000E1 RID: 225 RVA: 0x00005813 File Offset: 0x00003A13
		// (set) Token: 0x060000E2 RID: 226 RVA: 0x0000581B File Offset: 0x00003A1B
		[XmlAttribute(AttributeName = "Skip")]
		public bool Skip
		{
			get
			{
				return this.skip;
			}
			set
			{
				this.skip = value;
			}
		}

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x060000E3 RID: 227 RVA: 0x00005824 File Offset: 0x00003A24
		// (set) Token: 0x060000E4 RID: 228 RVA: 0x0000582C File Offset: 0x00003A2C
		[XmlElement(ElementName = "Binaries")]
		public ApplicationFiles ApplicationFiles
		{
			get
			{
				return this.applicationFiles;
			}
			set
			{
				this.applicationFiles = value;
			}
		}

		// Token: 0x060000E6 RID: 230 RVA: 0x00005868 File Offset: 0x00003A68
		public void Add(IXPathNavigable fullTrustXmlElement)
		{
			GlobalVariables.MacroResolver.BeginLocal();
			try
			{
				XmlElement xmlElement = (XmlElement)fullTrustXmlElement;
				this.AddAttributes(xmlElement);
				this.CompileAttributes(xmlElement);
				this.AddElements(xmlElement);
				this.CalculateAttributeHash();
			}
			finally
			{
				GlobalVariables.MacroResolver.EndLocal();
			}
		}

		// Token: 0x060000E7 RID: 231 RVA: 0x000058C0 File Offset: 0x00003AC0
		private void AddAttributes(XmlElement fullTrustXmlElement)
		{
			string text = string.Empty;
			this.Name = NormalizedString.Get(fullTrustXmlElement.GetAttribute("Name"));
			text = fullTrustXmlElement.GetAttribute("Skip");
			if (!string.IsNullOrEmpty(text) && text.Equals("No"))
			{
				this.Skip = false;
			}
			KeyValuePair<string, string>[] attributes = new KeyValuePair<string, string>[]
			{
				new KeyValuePair<string, string>("Name", this.Name)
			};
			MiscUtils.RegisterObjectSpecificMacros(GlobalVariables.MacroResolver, ObjectType.FullTrust, attributes);
		}

		// Token: 0x060000E8 RID: 232 RVA: 0x0000593C File Offset: 0x00003B3C
		private void CompileAttributes(XmlElement componentXmlElement)
		{
			this.ElementId = HashCalculator.CalculateSha256Hash(this.Name, true);
		}

		// Token: 0x060000E9 RID: 233 RVA: 0x00005950 File Offset: 0x00003B50
		private void AddElements(XmlElement fullTrustXmlElement)
		{
			this.AddBinaries(fullTrustXmlElement);
		}

		// Token: 0x060000EA RID: 234 RVA: 0x00005959 File Offset: 0x00003B59
		private void AddBinaries(XmlElement fullTrustXmlElement)
		{
			this.ApplicationFiles = new ApplicationFiles();
			this.applicationFiles.Add(fullTrustXmlElement);
		}

		// Token: 0x060000EB RID: 235 RVA: 0x00005974 File Offset: 0x00003B74
		private void CalculateAttributeHash()
		{
			StringBuilder stringBuilder = new StringBuilder(this.Name);
			stringBuilder.Append(this.applicationFiles.GetAllBinPaths());
			stringBuilder.Append(this.Skip);
			this.AttributeHash = HashCalculator.CalculateSha256Hash(stringBuilder.ToString(), true);
		}

		// Token: 0x060000EC RID: 236 RVA: 0x000059C0 File Offset: 0x00003BC0
		public void Print()
		{
			ReportingBase instance = ReportingBase.GetInstance();
			instance.XmlElementLine(ConstantStrings.IndentationLevel2, "Application");
			instance.XmlAttributeLine(ConstantStrings.IndentationLevel3, "Name", this.Name);
			instance.XmlAttributeLine(ConstantStrings.IndentationLevel3, "Skip", this.Skip.ToString());
			if (this.ApplicationFiles != null)
			{
				instance.DebugLine(string.Empty);
				this.applicationFiles.Print();
			}
		}

		// Token: 0x040000E7 RID: 231
		private string elementId = "Not Calculated";

		// Token: 0x040000E8 RID: 232
		private string attributeHash = "Not Calculated";

		// Token: 0x040000E9 RID: 233
		private string name = "Not Calculated";

		// Token: 0x040000EA RID: 234
		private bool skip = true;

		// Token: 0x040000EB RID: 235
		private ApplicationFiles applicationFiles;
	}
}
