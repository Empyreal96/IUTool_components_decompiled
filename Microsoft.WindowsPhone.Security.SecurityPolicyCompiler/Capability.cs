using System;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace Microsoft.WindowsPhone.Security.SecurityPolicyCompiler
{
	// Token: 0x02000012 RID: 18
	[XmlInclude(typeof(PrivateResources))]
	[XmlInclude(typeof(WindowsRules))]
	public class Capability : IPolicyElement
	{
		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000059 RID: 89 RVA: 0x000036E4 File Offset: 0x000018E4
		// (set) Token: 0x0600005A RID: 90 RVA: 0x000036EC File Offset: 0x000018EC
		[XmlIgnore]
		public CapabilityOwnerType OwnerType
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

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x0600005B RID: 91 RVA: 0x000036F5 File Offset: 0x000018F5
		// (set) Token: 0x0600005C RID: 92 RVA: 0x000036FD File Offset: 0x000018FD
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

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x0600005D RID: 93 RVA: 0x00003706 File Offset: 0x00001906
		// (set) Token: 0x0600005E RID: 94 RVA: 0x0000370E File Offset: 0x0000190E
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

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x0600005F RID: 95 RVA: 0x00003717 File Offset: 0x00001917
		// (set) Token: 0x06000060 RID: 96 RVA: 0x0000371F File Offset: 0x0000191F
		[XmlAttribute(AttributeName = "Id")]
		public string Id
		{
			get
			{
				return this.id;
			}
			set
			{
				this.id = value;
			}
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000061 RID: 97 RVA: 0x00003728 File Offset: 0x00001928
		// (set) Token: 0x06000062 RID: 98 RVA: 0x00003730 File Offset: 0x00001930
		[XmlAttribute(AttributeName = "AppCapSID")]
		public string AppCapSID
		{
			get
			{
				return this.appCapSID;
			}
			set
			{
				this.appCapSID = value;
			}
		}

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000063 RID: 99 RVA: 0x00003739 File Offset: 0x00001939
		// (set) Token: 0x06000064 RID: 100 RVA: 0x00003741 File Offset: 0x00001941
		[XmlAttribute(AttributeName = "SvcCapSID")]
		public string SvcCapSID
		{
			get
			{
				return this.svcCapSID;
			}
			set
			{
				this.svcCapSID = value;
			}
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000065 RID: 101 RVA: 0x0000374A File Offset: 0x0000194A
		// (set) Token: 0x06000066 RID: 102 RVA: 0x00003752 File Offset: 0x00001952
		[XmlAttribute(AttributeName = "FriendlyName")]
		public string FriendlyName
		{
			get
			{
				return this.friendlyName;
			}
			set
			{
				this.friendlyName = value;
			}
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000067 RID: 103 RVA: 0x0000375B File Offset: 0x0000195B
		// (set) Token: 0x06000068 RID: 104 RVA: 0x00003763 File Offset: 0x00001963
		[XmlAttribute(AttributeName = "Visibility")]
		public string Visibility
		{
			get
			{
				return this.visibility;
			}
			set
			{
				this.visibility = value;
			}
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000069 RID: 105 RVA: 0x0000376C File Offset: 0x0000196C
		// (set) Token: 0x0600006A RID: 106 RVA: 0x00003774 File Offset: 0x00001974
		[XmlElement(ElementName = "CapabilityRules")]
		public CapabilityRules CapabilityRules
		{
			get
			{
				return this.capabilityRules;
			}
			set
			{
				this.capabilityRules = value;
			}
		}

		// Token: 0x0600006B RID: 107 RVA: 0x00003780 File Offset: 0x00001980
		public Capability()
		{
		}

		// Token: 0x0600006C RID: 108 RVA: 0x000037D4 File Offset: 0x000019D4
		public Capability(CapabilityOwnerType value)
		{
			this.OwnerType = value;
		}

		// Token: 0x0600006D RID: 109 RVA: 0x0000382C File Offset: 0x00001A2C
		public void Add(IXPathNavigable capabilityXmlElement)
		{
			XmlElement capabilityXmlElement2 = (XmlElement)capabilityXmlElement;
			this.AddAttributes(capabilityXmlElement2);
			this.CompileAttributes();
			this.AddElements(capabilityXmlElement2);
			this.CalculateAttributeHash();
		}

		// Token: 0x0600006E RID: 110 RVA: 0x0000385A File Offset: 0x00001A5A
		protected virtual void AddAttributes(XmlElement capabilityXmlElement)
		{
			this.Id = capabilityXmlElement.GetAttribute("Id");
			this.FriendlyName = capabilityXmlElement.GetAttribute("FriendlyName");
			this.Visibility = capabilityXmlElement.GetAttribute("Visibility");
		}

		// Token: 0x0600006F RID: 111 RVA: 0x00003890 File Offset: 0x00001A90
		protected virtual void CompileAttributes()
		{
			bool flag = false;
			foreach (string b in ConstantStrings.GetValidCapabilityVisibilityList())
			{
				if (this.Visibility == b)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				this.Print();
				throw new PolicyCompilerInternalException(string.Format(GlobalVariables.Culture, "Capability visibility is invalid, Capability ID ={0}\nNote visibility value is case sensitive", new object[]
				{
					this.Id
				}));
			}
			this.ElementId = HashCalculator.CalculateSha256Hash(this.id, true);
			if (ConstantStrings.PredefinedServiceCapabilities.Contains(this.id))
			{
				this.SvcCapSID = SidBuilder.BuildSidString("S-1-5-21-2702878673-795188819-444038987", HashCalculator.CalculateSha256Hash(this.id, true), 8);
				this.AppCapSID = null;
				return;
			}
			this.AppCapSID = SidBuilder.BuildApplicationCapabilitySidString(this.id);
			this.SvcCapSID = GlobalVariables.SidMapping[this.id];
		}

		// Token: 0x06000070 RID: 112 RVA: 0x00003968 File Offset: 0x00001B68
		protected virtual void AddElements(XmlElement capabilityXmlElement)
		{
			XmlNodeList xmlNodeList = capabilityXmlElement.SelectNodes("./WP_Policy:CapabilityRules", GlobalVariables.NamespaceManager);
			if (xmlNodeList.Count > 0)
			{
				if (this.capabilityRules == null)
				{
					this.capabilityRules = new CapabilityRules(this.OwnerType);
				}
				bool flag = ConstantStrings.PredefinedServiceCapabilities.Contains(this.id);
				foreach (object obj in xmlNodeList)
				{
					XmlElement capabilityRulesXmlElement = (XmlElement)obj;
					if (flag)
					{
						this.capabilityRules.Add(capabilityRulesXmlElement, null, this.id);
					}
					else
					{
						this.capabilityRules.Add(capabilityRulesXmlElement, this.AppCapSID, this.SvcCapSID);
					}
				}
			}
		}

		// Token: 0x06000071 RID: 113 RVA: 0x00003A30 File Offset: 0x00001C30
		protected virtual void CalculateAttributeHash()
		{
			StringBuilder stringBuilder = new StringBuilder(this.Id);
			stringBuilder.Append(this.Visibility);
			if (this.capabilityRules != null)
			{
				stringBuilder.Append(this.capabilityRules.GetAllAttributesString());
			}
			else
			{
				if (this.AppCapSID != null)
				{
					stringBuilder.Append(this.AppCapSID);
				}
				if (this.SvcCapSID != null)
				{
					stringBuilder.Append(this.SvcCapSID);
				}
			}
			this.AttributeHash = HashCalculator.CalculateSha256Hash(stringBuilder.ToString(), true);
		}

		// Token: 0x06000072 RID: 114 RVA: 0x00003AB0 File Offset: 0x00001CB0
		public void Print()
		{
			ReportingBase instance = ReportingBase.GetInstance();
			instance.XmlElementLine(ConstantStrings.IndentationLevel2, "Capability");
			instance.XmlAttributeLine(ConstantStrings.IndentationLevel3, "ElementID", this.ElementId);
			instance.XmlAttributeLine(ConstantStrings.IndentationLevel3, "AttributeHash", this.AttributeHash);
			instance.XmlAttributeLine(ConstantStrings.IndentationLevel3, "Id", this.Id);
			instance.XmlAttributeLine(ConstantStrings.IndentationLevel3, "AppCapSID", this.AppCapSID);
			instance.XmlAttributeLine(ConstantStrings.IndentationLevel3, "SvcCapSID", this.SvcCapSID);
			instance.XmlAttributeLine(ConstantStrings.IndentationLevel3, "FriendlyName", this.FriendlyName);
			instance.XmlAttributeLine(ConstantStrings.IndentationLevel3, "Visibility", this.Visibility);
			if (this.capabilityRules != null)
			{
				instance.DebugLine(string.Empty);
				this.capabilityRules.Print();
			}
		}

		// Token: 0x040000C1 RID: 193
		private CapabilityOwnerType ownerType = CapabilityOwnerType.Unknown;

		// Token: 0x040000C2 RID: 194
		private string elementId = "Not Calculated";

		// Token: 0x040000C3 RID: 195
		private string attributeHash = "Not Calculated";

		// Token: 0x040000C4 RID: 196
		private string id = "Not Calculated";

		// Token: 0x040000C5 RID: 197
		private string appCapSID;

		// Token: 0x040000C6 RID: 198
		private string svcCapSID;

		// Token: 0x040000C7 RID: 199
		private string friendlyName = "Not Calculated";

		// Token: 0x040000C8 RID: 200
		private string visibility = "Not Calculated";

		// Token: 0x040000C9 RID: 201
		private CapabilityRules capabilityRules;
	}
}
