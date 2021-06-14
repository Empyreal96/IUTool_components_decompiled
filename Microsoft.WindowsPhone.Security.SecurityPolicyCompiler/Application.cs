using System;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace Microsoft.WindowsPhone.Security.SecurityPolicyCompiler
{
	// Token: 0x02000015 RID: 21
	public class Application : Component, IPolicyElement
	{
		// Token: 0x17000019 RID: 25
		// (get) Token: 0x06000080 RID: 128 RVA: 0x00004324 File Offset: 0x00002524
		// (set) Token: 0x06000081 RID: 129 RVA: 0x0000432C File Offset: 0x0000252C
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

		// Token: 0x06000082 RID: 130 RVA: 0x00004335 File Offset: 0x00002535
		public Application()
		{
			base.OwnerType = CapabilityOwnerType.Application;
		}

		// Token: 0x06000083 RID: 131 RVA: 0x00004344 File Offset: 0x00002544
		public void Add(IXPathNavigable applicationXmlElement)
		{
			GlobalVariables.MacroResolver.BeginLocal();
			try
			{
				XmlElement componentXmlElement = (XmlElement)applicationXmlElement;
				this.AddAttributes(componentXmlElement);
				this.CompileAttributes(componentXmlElement);
				this.AddElements(componentXmlElement);
				this.CalculateAttributeHash();
				base.AddPrivateCapSIDIfAny();
			}
			finally
			{
				GlobalVariables.MacroResolver.EndLocal();
			}
		}

		// Token: 0x06000084 RID: 132 RVA: 0x000043A0 File Offset: 0x000025A0
		protected override void AddElements(XmlElement applicationXmlElement)
		{
			base.RequiredCapabilities.OwnerType = CapabilityOwnerType.Application;
			this.AddBinaries(applicationXmlElement);
			base.AddElements(applicationXmlElement);
		}

		// Token: 0x06000085 RID: 133 RVA: 0x000043BC File Offset: 0x000025BC
		private void AddBinaries(XmlElement applicationXmlElement)
		{
			this.ApplicationFiles = new ApplicationFiles();
			this.applicationFiles.Add(applicationXmlElement);
		}

		// Token: 0x06000086 RID: 134 RVA: 0x000043D8 File Offset: 0x000025D8
		private void CalculateAttributeHash()
		{
			StringBuilder stringBuilder = new StringBuilder(base.Name);
			stringBuilder.Append(this.applicationFiles.GetAllBinPaths());
			stringBuilder.Append(base.RequiredCapabilities.GetAllCapIds());
			stringBuilder.Append(base.OEMExtensible);
			if (base.HasPrivateResources())
			{
				stringBuilder.Append(base.PrivateResources.AttributeHash);
			}
			base.AttributeHash = HashCalculator.CalculateSha256Hash(stringBuilder.ToString(), true);
		}

		// Token: 0x06000087 RID: 135 RVA: 0x00004450 File Offset: 0x00002650
		public void Print()
		{
			ReportingBase instance = ReportingBase.GetInstance();
			instance.XmlElementLine(ConstantStrings.IndentationLevel2, "Application");
			instance.XmlAttributeLine(ConstantStrings.IndentationLevel3, "Name", base.Name);
			if (this.ApplicationFiles != null)
			{
				instance.DebugLine(string.Empty);
				this.applicationFiles.Print();
			}
			if (base.RequiredCapabilities != null)
			{
				instance.DebugLine(string.Empty);
				base.RequiredCapabilities.Print();
			}
		}

		// Token: 0x040000CE RID: 206
		private ApplicationFiles applicationFiles;
	}
}
