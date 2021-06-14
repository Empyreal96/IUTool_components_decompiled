using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon;

namespace Microsoft.WindowsPhone.Security.SecurityPolicyCompiler
{
	// Token: 0x02000011 RID: 17
	public class AppBinaries : ApplicationFiles
	{
		// Token: 0x1700000C RID: 12
		// (get) Token: 0x0600004D RID: 77 RVA: 0x000034E4 File Offset: 0x000016E4
		// (set) Token: 0x0600004E RID: 78 RVA: 0x000034EC File Offset: 0x000016EC
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

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x0600004F RID: 79 RVA: 0x000034F5 File Offset: 0x000016F5
		// (set) Token: 0x06000050 RID: 80 RVA: 0x000034FD File Offset: 0x000016FD
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

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000051 RID: 81 RVA: 0x00003506 File Offset: 0x00001706
		// (set) Token: 0x06000052 RID: 82 RVA: 0x0000350E File Offset: 0x0000170E
		[XmlAttribute(AttributeName = "AppName")]
		public string AppName
		{
			get
			{
				return this.appName;
			}
			set
			{
				this.appName = value;
			}
		}

		// Token: 0x06000053 RID: 83 RVA: 0x00003517 File Offset: 0x00001717
		public AppBinaries()
		{
			this.checkBinaryFile = false;
		}

		// Token: 0x06000054 RID: 84 RVA: 0x00003548 File Offset: 0x00001748
		public override void Add(IXPathNavigable appBinariesXmlElement)
		{
			GlobalVariables.MacroResolver.BeginLocal();
			try
			{
				XmlElement xmlElement = (XmlElement)appBinariesXmlElement;
				this.AddAttributes(xmlElement);
				this.CompileAttributes();
				base.Add(xmlElement);
				this.CalculateAttributeHash();
			}
			finally
			{
				GlobalVariables.MacroResolver.EndLocal();
			}
		}

		// Token: 0x06000055 RID: 85 RVA: 0x000035A0 File Offset: 0x000017A0
		private void AddAttributes(XmlElement appBinariesXmlElement)
		{
			this.AppName = NormalizedString.Get(appBinariesXmlElement.GetAttribute("Name"));
			KeyValuePair<string, string>[] attributes = new KeyValuePair<string, string>[]
			{
				new KeyValuePair<string, string>("Name", this.AppName)
			};
			MiscUtils.RegisterObjectSpecificMacros(GlobalVariables.MacroResolver, ObjectType.AppResource, attributes);
		}

		// Token: 0x06000056 RID: 86 RVA: 0x000035ED File Offset: 0x000017ED
		private void CompileAttributes()
		{
			this.ElementId = HashCalculator.CalculateSha256Hash(this.AppName, true);
		}

		// Token: 0x06000057 RID: 87 RVA: 0x00003604 File Offset: 0x00001804
		private void CalculateAttributeHash()
		{
			string str = string.Empty;
			str = base.GetAllBinPaths();
			this.AttributeHash = HashCalculator.CalculateSha256Hash(this.AppName + str, true);
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00003638 File Offset: 0x00001838
		public override void Print()
		{
			ReportingBase instance = ReportingBase.GetInstance();
			instance.XmlElementLine(ConstantStrings.IndentationLevel2, "AppBinaries");
			instance.XmlAttributeLine(ConstantStrings.IndentationLevel3, "AppName", this.AppName);
			if (this.applicationFileCollection != null)
			{
				instance.DebugLine(string.Empty);
				instance.XmlElementLine(ConstantStrings.IndentationLevel3, "Binaries");
				foreach (ApplicationFile applicationFile in this.applicationFileCollection)
				{
					instance.DebugLine(string.Empty);
					applicationFile.Print();
				}
			}
		}

		// Token: 0x040000BE RID: 190
		private string elementId = "Not Calculated";

		// Token: 0x040000BF RID: 191
		private string attributeHash = "Not Calculated";

		// Token: 0x040000C0 RID: 192
		private string appName = "Not Calculated";
	}
}
