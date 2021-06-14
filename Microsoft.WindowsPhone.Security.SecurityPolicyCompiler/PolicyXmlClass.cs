using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace Microsoft.WindowsPhone.Security.SecurityPolicyCompiler
{
	// Token: 0x0200001E RID: 30
	[XmlRoot("PhoneSecurityPolicy")]
	public class PolicyXmlClass
	{
		// Token: 0x17000033 RID: 51
		// (get) Token: 0x060000ED RID: 237 RVA: 0x00005A35 File Offset: 0x00003C35
		// (set) Token: 0x060000EE RID: 238 RVA: 0x00005A3D File Offset: 0x00003C3D
		[XmlAttribute(AttributeName = "Description")]
		public string Description
		{
			get
			{
				return this.description;
			}
			set
			{
				this.description = value;
			}
		}

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x060000EF RID: 239 RVA: 0x00005A46 File Offset: 0x00003C46
		// (set) Token: 0x060000F0 RID: 240 RVA: 0x00005A4E File Offset: 0x00003C4E
		[XmlAttribute(AttributeName = "Vendor")]
		public string Vendor
		{
			get
			{
				return this.vendor;
			}
			set
			{
				this.vendor = value;
			}
		}

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x060000F1 RID: 241 RVA: 0x00005A57 File Offset: 0x00003C57
		// (set) Token: 0x060000F2 RID: 242 RVA: 0x00005A5F File Offset: 0x00003C5F
		[XmlAttribute(AttributeName = "RequiredOSVersion")]
		public string RequiredOSVersion
		{
			get
			{
				return this.requiredOSVersion;
			}
			set
			{
				this.requiredOSVersion = value;
			}
		}

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x060000F3 RID: 243 RVA: 0x00005A68 File Offset: 0x00003C68
		// (set) Token: 0x060000F4 RID: 244 RVA: 0x00005A70 File Offset: 0x00003C70
		[XmlAttribute(AttributeName = "FileVersion")]
		public string FileVersion
		{
			get
			{
				return this.fileVersion;
			}
			set
			{
				this.fileVersion = value;
			}
		}

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x060000F5 RID: 245 RVA: 0x00005A79 File Offset: 0x00003C79
		// (set) Token: 0x060000F6 RID: 246 RVA: 0x00005A81 File Offset: 0x00003C81
		[XmlAttribute(AttributeName = "HashType")]
		public string HashType
		{
			get
			{
				return this.hashType;
			}
			set
			{
				this.hashType = value;
			}
		}

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x060000F7 RID: 247 RVA: 0x00005A8A File Offset: 0x00003C8A
		// (set) Token: 0x060000F8 RID: 248 RVA: 0x00005A94 File Offset: 0x00003C94
		[XmlAttribute(AttributeName = "PackageID")]
		public string PackageId
		{
			get
			{
				return this.packageId;
			}
			set
			{
				this.packageId = value;
				GlobalVariables.IsInPackageAllowList = false;
				foreach (string value2 in ConstantStrings.GetPackageAllowList())
				{
					GlobalVariables.IsInPackageAllowList = this.packageId.Equals(value2, StringComparison.OrdinalIgnoreCase);
					if (GlobalVariables.IsInPackageAllowList)
					{
						break;
					}
				}
			}
		}

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x060000F9 RID: 249 RVA: 0x00005ADF File Offset: 0x00003CDF
		// (set) Token: 0x060000FA RID: 250 RVA: 0x00005AE7 File Offset: 0x00003CE7
		[XmlElement(ElementName = "Capabilities")]
		public Capabilities OutputCapabilities
		{
			get
			{
				return this.capabilities;
			}
			set
			{
				this.capabilities = value;
			}
		}

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x060000FB RID: 251 RVA: 0x00005AF0 File Offset: 0x00003CF0
		// (set) Token: 0x060000FC RID: 252 RVA: 0x00005AF8 File Offset: 0x00003CF8
		[XmlElement(ElementName = "Components")]
		public Components OutputComponents
		{
			get
			{
				return this.components;
			}
			set
			{
				this.components = value;
			}
		}

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x060000FD RID: 253 RVA: 0x00005B01 File Offset: 0x00003D01
		// (set) Token: 0x060000FE RID: 254 RVA: 0x00005B09 File Offset: 0x00003D09
		[XmlElement(ElementName = "Authorization")]
		public Authorization OutputAuthorization
		{
			get
			{
				return this.authorization;
			}
			set
			{
				this.authorization = value;
			}
		}

		// Token: 0x06000100 RID: 256 RVA: 0x00005B30 File Offset: 0x00003D30
		public void Add(string policyXmlFileFullPath, IXPathNavigable xmlPathNavigator)
		{
			if (xmlPathNavigator == null)
			{
				throw new PolicyCompilerInternalException(string.Format(GlobalVariables.Culture, "Error: CompileSecurityPolicy {0}", new object[]
				{
					policyXmlFileFullPath
				}));
			}
			XmlDocument policyXmlDocument = (XmlDocument)xmlPathNavigator;
			this.fileFullPath = policyXmlFileFullPath;
			try
			{
				GlobalVariables.CurrentCompilationState = CompilationState.PolicyFileAddHeaderAttributes;
				this.AddAttributes(policyXmlDocument);
				GlobalVariables.CurrentCompilationState = CompilationState.PolicyFileAddElements;
				this.AddElements(policyXmlDocument);
			}
			catch (XPathException originalException)
			{
				this.Print();
				throw new PolicyCompilerInternalException(string.Format(GlobalVariables.Culture, "Error: CompileSecurityPolicy {0}", new object[]
				{
					policyXmlFileFullPath
				}), originalException);
			}
		}

		// Token: 0x06000101 RID: 257 RVA: 0x00005BC4 File Offset: 0x00003DC4
		private void AddAttributes(XmlDocument policyXmlDocument)
		{
			foreach (object obj in policyXmlDocument.SelectNodes("/WP_Policy:PhoneSecurityPolicy", GlobalVariables.NamespaceManager))
			{
				XmlElement xmlElement = (XmlElement)obj;
				this.Description = xmlElement.GetAttribute("Description");
				this.Vendor = xmlElement.GetAttribute("Vendor");
				this.RequiredOSVersion = xmlElement.GetAttribute("RequiredOSVersion");
				this.FileVersion = xmlElement.GetAttribute("FileVersion");
			}
		}

		// Token: 0x06000102 RID: 258 RVA: 0x00005C64 File Offset: 0x00003E64
		private void AddElements(XmlDocument policyXmlDocument)
		{
			this.AddCapabilites(policyXmlDocument);
			this.AddComponents(policyXmlDocument);
			this.AddAuthorization(policyXmlDocument);
		}

		// Token: 0x06000103 RID: 259 RVA: 0x00005C7C File Offset: 0x00003E7C
		private void AddCapabilites(XmlDocument policyXmlDocument)
		{
			XmlNodeList xmlNodeList = policyXmlDocument.SelectNodes("//WP_Policy:Capabilities", GlobalVariables.NamespaceManager);
			if (xmlNodeList.Count > 0)
			{
				if (PolicyCompiler.BlockPolicyDefinition)
				{
					throw new PolicyCompilerInternalException("The package definition file should not have capability definition");
				}
				if (this.capabilities == null)
				{
					this.capabilities = new Capabilities();
				}
				foreach (object obj in xmlNodeList)
				{
					XmlElement capabilitiesXmlElement = (XmlElement)obj;
					this.capabilities.Add(capabilitiesXmlElement);
				}
			}
		}

		// Token: 0x06000104 RID: 260 RVA: 0x00005D14 File Offset: 0x00003F14
		private void AddComponents(XmlDocument policyXmlDocument)
		{
			XmlNodeList xmlNodeList = policyXmlDocument.SelectNodes("//WP_Policy:Components", GlobalVariables.NamespaceManager);
			if (xmlNodeList.Count != 0)
			{
				if (this.components == null)
				{
					this.components = new Components();
				}
				foreach (object obj in xmlNodeList)
				{
					XmlElement componentXmlElement = (XmlElement)obj;
					this.components.Add(componentXmlElement);
					if (this.components.HasPrivateCapabilities())
					{
						if (this.capabilities == null)
						{
							this.capabilities = new Capabilities();
						}
						foreach (Capability capability in this.components.GetPrivateCapabilities())
						{
							this.capabilities.Append(capability);
						}
					}
				}
			}
		}

		// Token: 0x06000105 RID: 261 RVA: 0x00005E10 File Offset: 0x00004010
		private void AddAuthorization(XmlDocument policyXmlDocument)
		{
			XmlNodeList xmlNodeList = policyXmlDocument.SelectNodes("//WP_Policy:Authorization", GlobalVariables.NamespaceManager);
			if (xmlNodeList.Count != 0)
			{
				if (this.authorization == null)
				{
					this.authorization = new Authorization();
				}
				foreach (object obj in xmlNodeList)
				{
					XmlElement authorizationXmlElement = (XmlElement)obj;
					this.authorization.Add(authorizationXmlElement);
				}
			}
		}

		// Token: 0x06000106 RID: 262 RVA: 0x00005E98 File Offset: 0x00004098
		public bool SaveToXml(string policyFile)
		{
			GlobalVariables.CurrentCompilationState = CompilationState.SaveXmlFile;
			bool result = false;
			if ((this.capabilities != null && this.capabilities.HasChild()) || (this.components != null && this.components.HasChild()) || (this.authorization != null && this.authorization.HasChild()))
			{
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(PolicyXmlClass), "urn:Microsoft.WindowsPhone/PhoneSecurityPolicyInternal.v8.00");
				using (TextWriter textWriter = new StreamWriter(policyFile))
				{
					xmlSerializer.Serialize(textWriter, this);
				}
				result = true;
			}
			return result;
		}

		// Token: 0x06000107 RID: 263 RVA: 0x00005F30 File Offset: 0x00004130
		public void Print()
		{
			ReportingBase instance = ReportingBase.GetInstance();
			instance.DebugLine("Policy file: " + this.fileFullPath);
			instance.DebugLine(string.Empty);
			instance.XmlElementLine("", "PhoneSecurityPolicy");
			instance.XmlAttributeLine(ConstantStrings.IndentationLevel1, "Description", this.Description);
			instance.XmlAttributeLine(ConstantStrings.IndentationLevel1, "Vendor", this.Vendor);
			instance.XmlAttributeLine(ConstantStrings.IndentationLevel1, "RequiredOSVersion", this.RequiredOSVersion);
			instance.XmlAttributeLine(ConstantStrings.IndentationLevel1, "FileVersion", this.FileVersion);
			instance.XmlAttributeLine(ConstantStrings.IndentationLevel1, "HashType", this.HashType);
			instance.XmlAttributeLine(ConstantStrings.IndentationLevel1, "PackageID", this.PackageId);
			if (this.capabilities != null)
			{
				instance.DebugLine(string.Empty);
				this.capabilities.Print();
			}
			if (this.components != null)
			{
				instance.DebugLine(string.Empty);
				this.components.Print();
			}
			if (this.authorization != null)
			{
				instance.DebugLine(string.Empty);
				this.authorization.Print();
			}
		}

		// Token: 0x040000EC RID: 236
		private string fileFullPath = string.Empty;

		// Token: 0x040000ED RID: 237
		private string description;

		// Token: 0x040000EE RID: 238
		private string vendor;

		// Token: 0x040000EF RID: 239
		private string requiredOSVersion;

		// Token: 0x040000F0 RID: 240
		private string fileVersion;

		// Token: 0x040000F1 RID: 241
		private string hashType = "Sha256";

		// Token: 0x040000F2 RID: 242
		private string packageId;

		// Token: 0x040000F3 RID: 243
		private Capabilities capabilities;

		// Token: 0x040000F4 RID: 244
		private Components components;

		// Token: 0x040000F5 RID: 245
		private Authorization authorization;
	}
}
