using System;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace Microsoft.WindowsPhone.Security.SecurityPolicyCompiler
{
	// Token: 0x0200001C RID: 28
	public class Service : Component, IPolicyElement
	{
		// Token: 0x17000029 RID: 41
		// (get) Token: 0x060000C9 RID: 201 RVA: 0x00005389 File Offset: 0x00003589
		// (set) Token: 0x060000CA RID: 202 RVA: 0x00005391 File Offset: 0x00003591
		[XmlAttribute(AttributeName = "Executable")]
		public string Executable
		{
			get
			{
				return this.executable;
			}
			set
			{
				this.executable = value;
			}
		}

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x060000CB RID: 203 RVA: 0x0000539A File Offset: 0x0000359A
		// (set) Token: 0x060000CC RID: 204 RVA: 0x000053A2 File Offset: 0x000035A2
		[XmlAttribute(AttributeName = "IsTCB")]
		public string IsTCB
		{
			get
			{
				return this.isTCB;
			}
			set
			{
				this.isTCB = value;
			}
		}

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x060000CD RID: 205 RVA: 0x000053AB File Offset: 0x000035AB
		// (set) Token: 0x060000CE RID: 206 RVA: 0x000053B3 File Offset: 0x000035B3
		[XmlAttribute(AttributeName = "LogonAccount")]
		public string LogonAccount
		{
			get
			{
				return this.logonAccount;
			}
			set
			{
				this.logonAccount = value;
			}
		}

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x060000CF RID: 207 RVA: 0x000053BC File Offset: 0x000035BC
		// (set) Token: 0x060000D0 RID: 208 RVA: 0x000053C4 File Offset: 0x000035C4
		[XmlAttribute(AttributeName = "SvcHostGroupName")]
		public string SvcHostGroupName
		{
			get
			{
				return this.svcHostGroupName;
			}
			set
			{
				this.svcHostGroupName = value;
			}
		}

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x060000D1 RID: 209 RVA: 0x000053CD File Offset: 0x000035CD
		// (set) Token: 0x060000D2 RID: 210 RVA: 0x000053D5 File Offset: 0x000035D5
		[XmlAttribute(AttributeName = "OwnedProc")]
		public string SvcProcessOwnership
		{
			get
			{
				return this.svcProcessOwnership;
			}
			set
			{
				this.svcProcessOwnership = value;
			}
		}

		// Token: 0x060000D3 RID: 211 RVA: 0x000053DE File Offset: 0x000035DE
		public Service()
		{
			base.OwnerType = CapabilityOwnerType.Service;
		}

		// Token: 0x060000D4 RID: 212 RVA: 0x000053F8 File Offset: 0x000035F8
		public void Add(IXPathNavigable appServiceXmlElement)
		{
			GlobalVariables.MacroResolver.BeginLocal();
			try
			{
				XmlElement componentXmlElement = (XmlElement)appServiceXmlElement;
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

		// Token: 0x060000D5 RID: 213 RVA: 0x00005454 File Offset: 0x00003654
		protected override void AddAttributes(XmlElement serviceXmlElement)
		{
			string value = string.Empty;
			string value2 = string.Empty;
			string value3 = string.Empty;
			string value4 = string.Empty;
			base.AddAttributes(serviceXmlElement);
			this.SvcProcessOwnership = ((serviceXmlElement.GetAttribute("Type") == "Win32OwnProcess") ? "Y" : null);
			value = serviceXmlElement.GetAttribute("SvcHostGroupName");
			value2 = serviceXmlElement.GetAttribute("IsTCB");
			value3 = serviceXmlElement.GetAttribute("LogonAccount");
			if (!string.IsNullOrEmpty(value))
			{
				this.SvcHostGroupName = NormalizedString.Get(value);
				XmlElement xmlElement = (XmlElement)serviceXmlElement.SelectSingleNode("./WP_Policy:ServiceDll", GlobalVariables.NamespaceManager);
				if (xmlElement != null)
				{
					value4 = xmlElement.GetAttribute("HostExe");
					if (!string.IsNullOrEmpty(value4))
					{
						this.Executable = value4;
					}
					else
					{
						this.Executable = "$(RUNTIME.SYSTEM32)\\SvcHost.exe";
					}
				}
			}
			if (!string.IsNullOrEmpty(value2))
			{
				if (!string.IsNullOrEmpty(value3))
				{
					throw new PolicyCompilerInternalException(string.Format(GlobalVariables.Culture, "'{0}' and '{1}' can't be present in the same time in service '{2}'.", new object[]
					{
						"IsTCB",
						"LogonAccount",
						base.Name
					}));
				}
				this.IsTCB = value2;
			}
			if (!string.IsNullOrEmpty(value3))
			{
				this.LogonAccount = value3;
			}
		}

		// Token: 0x060000D6 RID: 214 RVA: 0x0000557B File Offset: 0x0000377B
		protected override void CompileAttributes(XmlElement serviceXmlElement)
		{
			this.Executable = GlobalVariables.ResolveMacroReference(this.Executable, string.Format(GlobalVariables.Culture, "Element={0}, Attribute={1}", new object[]
			{
				"Service",
				"Executable"
			}));
			base.CompileAttributes(serviceXmlElement);
		}

		// Token: 0x060000D7 RID: 215 RVA: 0x000055BC File Offset: 0x000037BC
		protected override void AddElements(XmlElement serviceXmlElement)
		{
			base.RequiredCapabilities.OwnerType = CapabilityOwnerType.Service;
			XmlElement xmlElement = (XmlElement)serviceXmlElement.SelectSingleNode("./WP_Policy:Executable", GlobalVariables.NamespaceManager);
			if ((xmlElement == null && this.svcHostGroupName == null) || (xmlElement != null && this.svcHostGroupName != null))
			{
				throw new PolicyCompilerInternalException(string.Format(GlobalVariables.Culture, "Element={0} '{1}', Attributes {2} and {3} are mutually exclusive", new object[]
				{
					"Service",
					base.Name,
					"Executable",
					"SvcHostGroupName"
				}));
			}
			if (xmlElement != null)
			{
				ApplicationFile applicationFile = new ApplicationFile();
				applicationFile.Add(xmlElement);
				if (applicationFile.Path != "Not Calculated")
				{
					this.Executable = applicationFile.Path;
				}
			}
			base.AddElements(serviceXmlElement);
		}

		// Token: 0x060000D8 RID: 216 RVA: 0x00005674 File Offset: 0x00003874
		private void CalculateAttributeHash()
		{
			StringBuilder stringBuilder = new StringBuilder(base.Name);
			stringBuilder.Append(this.SvcHostGroupName);
			stringBuilder.Append(this.Executable);
			stringBuilder.Append(this.IsTCB);
			if (this.LogonAccount != null)
			{
				stringBuilder.Append(this.LogonAccount);
			}
			if (base.OEMExtensible != null)
			{
				stringBuilder.Append(base.OEMExtensible);
			}
			stringBuilder.Append(base.RequiredCapabilities.GetAllCapIds());
			if (base.HasPrivateResources())
			{
				stringBuilder.Append(base.PrivateResources.AttributeHash);
			}
			base.AttributeHash = HashCalculator.CalculateSha256Hash(stringBuilder.ToString(), true);
		}

		// Token: 0x060000D9 RID: 217 RVA: 0x0000571C File Offset: 0x0000391C
		public void Print()
		{
			ReportingBase instance = ReportingBase.GetInstance();
			instance.XmlElementLine(ConstantStrings.IndentationLevel2, "Service");
			instance.XmlAttributeLine(ConstantStrings.IndentationLevel3, "Name", base.Name);
			this.PrintIfNotNull("SvcHostGroupName", this.SvcHostGroupName);
			this.PrintIfNotNull("IsTCB", this.IsTCB);
			this.PrintIfNotNull("LogonAccount", this.LogonAccount);
			this.PrintIfNotNull("OEMExtensible", base.OEMExtensible);
			if (base.RequiredCapabilities != null)
			{
				instance.DebugLine(string.Empty);
				base.RequiredCapabilities.Print();
			}
		}

		// Token: 0x060000DA RID: 218 RVA: 0x000057B8 File Offset: 0x000039B8
		private void PrintIfNotNull(string attributeString, string Attribute)
		{
			ReportingBase instance = ReportingBase.GetInstance();
			if (!string.IsNullOrEmpty(Attribute))
			{
				instance.XmlAttributeLine(ConstantStrings.IndentationLevel3, attributeString, Attribute);
			}
		}

		// Token: 0x040000E2 RID: 226
		private string executable;

		// Token: 0x040000E3 RID: 227
		private string isTCB = "No";

		// Token: 0x040000E4 RID: 228
		private string logonAccount;

		// Token: 0x040000E5 RID: 229
		private string svcHostGroupName;

		// Token: 0x040000E6 RID: 230
		private string svcProcessOwnership;
	}
}
