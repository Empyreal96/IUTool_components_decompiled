using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon;

namespace Microsoft.WindowsPhone.Security.SecurityPolicyCompiler
{
	// Token: 0x02000016 RID: 22
	public abstract class Component
	{
		// Token: 0x1700001A RID: 26
		// (get) Token: 0x06000088 RID: 136 RVA: 0x000044C5 File Offset: 0x000026C5
		// (set) Token: 0x06000089 RID: 137 RVA: 0x000044CD File Offset: 0x000026CD
		protected CapabilityOwnerType OwnerType
		{
			get
			{
				return this.ownerType;
			}
			set
			{
				this.ownerType = value;
			}
		}

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x0600008A RID: 138 RVA: 0x000044D6 File Offset: 0x000026D6
		// (set) Token: 0x0600008B RID: 139 RVA: 0x000044DE File Offset: 0x000026DE
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

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x0600008C RID: 140 RVA: 0x000044E7 File Offset: 0x000026E7
		// (set) Token: 0x0600008D RID: 141 RVA: 0x000044EF File Offset: 0x000026EF
		[XmlAttribute(AttributeName = "OEMExtensible")]
		public string OEMExtensible
		{
			get
			{
				return this.OEMExtensibleValue;
			}
			set
			{
				this.OEMExtensibleValue = value;
			}
		}

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x0600008E RID: 142 RVA: 0x000044F8 File Offset: 0x000026F8
		// (set) Token: 0x0600008F RID: 143 RVA: 0x00004500 File Offset: 0x00002700
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

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x06000090 RID: 144 RVA: 0x00004509 File Offset: 0x00002709
		// (set) Token: 0x06000091 RID: 145 RVA: 0x00004511 File Offset: 0x00002711
		[XmlAttribute(AttributeName = "SID")]
		public string ComponentSID
		{
			get
			{
				return this.componentSid;
			}
			set
			{
				this.componentSid = value;
			}
		}

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000092 RID: 146 RVA: 0x0000451A File Offset: 0x0000271A
		// (set) Token: 0x06000093 RID: 147 RVA: 0x00004522 File Offset: 0x00002722
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

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x06000094 RID: 148 RVA: 0x0000452B File Offset: 0x0000272B
		// (set) Token: 0x06000095 RID: 149 RVA: 0x00004533 File Offset: 0x00002733
		[XmlAttribute(AttributeName = "PrivateCapSID")]
		public string PrivateCapSID
		{
			get
			{
				return this.privateCapSID;
			}
			set
			{
				this.privateCapSID = value;
			}
		}

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x06000096 RID: 150 RVA: 0x0000453C File Offset: 0x0000273C
		// (set) Token: 0x06000097 RID: 151 RVA: 0x00004544 File Offset: 0x00002744
		[XmlElement(ElementName = "RequiredCapabilities")]
		public RequiredCapabilities RequiredCapabilities
		{
			get
			{
				return this.requiredCapabilities;
			}
			set
			{
				this.requiredCapabilities = value;
			}
		}

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x06000098 RID: 152 RVA: 0x0000454D File Offset: 0x0000274D
		// (set) Token: 0x06000099 RID: 153 RVA: 0x00004555 File Offset: 0x00002755
		[XmlIgnore]
		public PrivateResources PrivateResources
		{
			get
			{
				return this.privateResources;
			}
			set
			{
				this.privateResources = value;
			}
		}

		// Token: 0x0600009A RID: 154 RVA: 0x00004560 File Offset: 0x00002760
		private XmlElement CreateNewPrivateCapabilityXmlElementForApplicationCodeFolders(XmlElement componentXmlElement, string FolderName, string FolderPath)
		{
			XmlDocument ownerDocument = componentXmlElement.OwnerDocument;
			XmlElement xmlElement = ownerDocument.CreateElement(null, "PrivateResources", "urn:Microsoft.WindowsPhone/PackageSchema.v8.00");
			XmlElement xmlElement2 = ownerDocument.CreateElement(null, FolderName, "urn:Microsoft.WindowsPhone/PackageSchema.v8.00");
			xmlElement2.SetAttribute("Path", string.Format(GlobalVariables.Culture, FolderPath, new object[]
			{
				this.Name
			}));
			xmlElement.AppendChild(xmlElement2);
			return xmlElement;
		}

		// Token: 0x0600009B RID: 155 RVA: 0x000045C0 File Offset: 0x000027C0
		private XmlElement CreateNewPrivateCapabilityXmlElementForApplicationDataFolders(XmlElement componentXmlElement, string FolderName, string FolderPath)
		{
			XmlDocument ownerDocument = componentXmlElement.OwnerDocument;
			XmlElement xmlElement = ownerDocument.CreateElement(null, "PrivateResources", "urn:Microsoft.WindowsPhone/PackageSchema.v8.00");
			XmlElement xmlElement2 = ownerDocument.CreateElement(null, FolderName, "urn:Microsoft.WindowsPhone/PackageSchema.v8.00");
			xmlElement2.SetAttribute("Path", string.Format(GlobalVariables.Culture, FolderPath, new object[]
			{
				"DA0",
				this.Name
			}));
			xmlElement.AppendChild(xmlElement2);
			return xmlElement;
		}

		// Token: 0x0600009C RID: 156 RVA: 0x00004628 File Offset: 0x00002828
		protected virtual void AddAttributes(XmlElement componentXmlElement)
		{
			this.Name = NormalizedString.Get(componentXmlElement.GetAttribute("Name"));
			KeyValuePair<string, string>[] attributes = new KeyValuePair<string, string>[]
			{
				new KeyValuePair<string, string>("Name", this.Name)
			};
			string text = string.Empty;
			text = componentXmlElement.GetAttribute("OEMExtensible");
			if (!string.IsNullOrEmpty(text))
			{
				this.OEMExtensible = text;
			}
			CapabilityOwnerType capabilityOwnerType = this.ownerType;
			if (capabilityOwnerType == CapabilityOwnerType.Application)
			{
				MiscUtils.RegisterObjectSpecificMacros(GlobalVariables.MacroResolver, ObjectType.Application, attributes);
				return;
			}
			if (capabilityOwnerType != CapabilityOwnerType.Service)
			{
				throw new PolicyCompilerInternalException("SecurityPolicyCompiler Internal Error: Component's OwnerType can't be determined.");
			}
			MiscUtils.RegisterObjectSpecificMacros(GlobalVariables.MacroResolver, ObjectType.Service, attributes);
		}

		// Token: 0x0600009D RID: 157 RVA: 0x000046C0 File Offset: 0x000028C0
		protected virtual void CompileAttributes(XmlElement componentXmlElement)
		{
			this.ElementId = HashCalculator.CalculateSha256Hash(this.Name, true);
			CapabilityOwnerType capabilityOwnerType = this.ownerType;
			if (capabilityOwnerType == CapabilityOwnerType.Application)
			{
				this.ComponentSID = SidBuilder.BuildApplicationSidString(this.Name);
				return;
			}
			if (capabilityOwnerType != CapabilityOwnerType.Service)
			{
				throw new PolicyCompilerInternalException("SecurityPolicyCompiler Internal Error: Component's OwnerType can't be determined");
			}
			this.ComponentSID = SidBuilder.BuildServiceSidString(this.Name);
		}

		// Token: 0x0600009E RID: 158 RVA: 0x0000471E File Offset: 0x0000291E
		protected virtual void AddElements(XmlElement componentXmlElement)
		{
			this.AddRequiredCapabilities(componentXmlElement);
			this.AddPrivateResources(componentXmlElement);
		}

		// Token: 0x0600009F RID: 159 RVA: 0x00004730 File Offset: 0x00002930
		protected void AddRequiredCapabilities(XmlElement componentXmlElement)
		{
			foreach (object obj in componentXmlElement.SelectNodes("./WP_Policy:RequiredCapabilities", GlobalVariables.NamespaceManager))
			{
				XmlElement requiredCapabilitiesXmlElement = (XmlElement)obj;
				this.requiredCapabilities.Add(requiredCapabilitiesXmlElement);
			}
			foreach (string inputCapId in ConstantStrings.ComponentCapabilityIdFilter)
			{
				RequiredCapability requiredCapability = new RequiredCapability(this.ownerType);
				requiredCapability.Add(inputCapId, true);
				this.requiredCapabilities.Add(requiredCapability);
			}
			if (this.ownerType == CapabilityOwnerType.Service)
			{
				foreach (string inputCapId2 in ConstantStrings.ServiceCapabilityIdFilter)
				{
					RequiredCapability requiredCapability2 = new RequiredCapability(this.ownerType);
					requiredCapability2.Add(inputCapId2, true);
					this.requiredCapabilities.Add(requiredCapability2);
				}
			}
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x00004828 File Offset: 0x00002A28
		protected void AddPrivateResources(XmlElement componentXmlElement)
		{
			XmlNodeList xmlNodeList = componentXmlElement.SelectNodes("./WP_Policy:PrivateResources", GlobalVariables.NamespaceManager);
			if (this.ownerType == CapabilityOwnerType.Application)
			{
				XmlElement capabilityXmlElement = this.CreateNewPrivateCapabilityXmlElementForApplicationCodeFolders(componentXmlElement, "InstallationFolder", "\\PROGRAMS\\{0}\\(*)");
				XmlElement capabilityXmlElement2 = this.CreateNewPrivateCapabilityXmlElementForApplicationDataFolders(componentXmlElement, "ChamberProfileDataDefaultFolder", "\\DATA\\{0}\\{1}\\(*)");
				XmlElement capabilityXmlElement3 = this.CreateNewPrivateCapabilityXmlElementForApplicationDataFolders(componentXmlElement, "ChamberProfileDataShellContentFolder", "\\DATA\\{0}\\{1}\\Local\\Shared\\ShellContent\\(*)");
				XmlElement capabilityXmlElement4 = this.CreateNewPrivateCapabilityXmlElementForApplicationDataFolders(componentXmlElement, "ChamberProfileDataMediaFolder", "\\DATA\\{0}\\{1}\\Local\\Shared\\Media\\(*)");
				XmlElement capabilityXmlElement5 = this.CreateNewPrivateCapabilityXmlElementForApplicationDataFolders(componentXmlElement, "ChamberProfileDataPlatformDataFolder", "\\DATA\\{0}\\{1}\\PlatformData\\(*)");
				XmlElement capabilityXmlElement6 = this.CreateNewPrivateCapabilityXmlElementForApplicationDataFolders(componentXmlElement, "ChamberProfileDataLiveTilesFolder", "\\DATA\\{0}\\{1}\\PlatformData\\LiveTiles\\(*)");
				if (this.privateResources == null)
				{
					this.privateResources = new PrivateResources();
					this.privateResources.SetPrivateResourcesOwner(this.Name, this.ownerType, this.componentSid);
				}
				this.privateResources.Add(capabilityXmlElement);
				this.privateResources.Add(capabilityXmlElement2);
				this.privateResources.Add(capabilityXmlElement3);
				this.privateResources.Add(capabilityXmlElement4);
				this.privateResources.Add(capabilityXmlElement5);
				this.privateResources.Add(capabilityXmlElement6);
			}
			if (xmlNodeList.Count > 0)
			{
				if (PolicyCompiler.BlockPolicyDefinition)
				{
					throw new PolicyCompilerInternalException("The private resources can't be defined in this package.");
				}
				if (this.privateResources == null)
				{
					this.privateResources = new PrivateResources();
					this.privateResources.SetPrivateResourcesOwner(this.Name, this.ownerType, this.componentSid);
				}
				foreach (object obj in xmlNodeList)
				{
					XmlElement capabilityXmlElement7 = (XmlElement)obj;
					this.privateResources.Add(capabilityXmlElement7);
				}
			}
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x000049E0 File Offset: 0x00002BE0
		protected void AddPrivateCapSIDIfAny()
		{
			if (!this.HasPrivateResources())
			{
				return;
			}
			CapabilityOwnerType capabilityOwnerType = this.ownerType;
			if (capabilityOwnerType == CapabilityOwnerType.Application)
			{
				this.PrivateCapSID = this.privateResources.AppCapSID;
				return;
			}
			if (capabilityOwnerType != CapabilityOwnerType.Service)
			{
				throw new PolicyCompilerInternalException("SecurityPolicyCompiler Internal Error: ComponentCapabilities's OwnerType can't be determined.");
			}
			this.PrivateCapSID = this.privateResources.SvcCapSID;
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x00004A35 File Offset: 0x00002C35
		public bool HasPrivateResources()
		{
			return this.privateResources != null;
		}

		// Token: 0x040000CF RID: 207
		private CapabilityOwnerType ownerType = CapabilityOwnerType.Unknown;

		// Token: 0x040000D0 RID: 208
		private string elementId = "Not Calculated";

		// Token: 0x040000D1 RID: 209
		private string OEMExtensibleValue;

		// Token: 0x040000D2 RID: 210
		private string attributeHash = "Not Calculated";

		// Token: 0x040000D3 RID: 211
		private string componentSid = "Not Calculated";

		// Token: 0x040000D4 RID: 212
		private string name = "Not Calculated";

		// Token: 0x040000D5 RID: 213
		private string privateCapSID;

		// Token: 0x040000D6 RID: 214
		private RequiredCapabilities requiredCapabilities = new RequiredCapabilities();

		// Token: 0x040000D7 RID: 215
		private PrivateResources privateResources;
	}
}
