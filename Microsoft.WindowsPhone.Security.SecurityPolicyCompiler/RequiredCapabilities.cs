using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace Microsoft.WindowsPhone.Security.SecurityPolicyCompiler
{
	// Token: 0x02000019 RID: 25
	public class RequiredCapabilities : IPolicyElement
	{
		// Token: 0x17000025 RID: 37
		// (set) Token: 0x060000B2 RID: 178 RVA: 0x00004F61 File Offset: 0x00003161
		internal CapabilityOwnerType OwnerType
		{
			set
			{
				this.ownerType = value;
			}
		}

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x060000B3 RID: 179 RVA: 0x00004F6A File Offset: 0x0000316A
		[XmlElement(ElementName = "RequiredCapability")]
		public List<RequiredCapability> RequiredCapabilityCollection
		{
			get
			{
				return this.requiredCapabilityCollection;
			}
		}

		// Token: 0x060000B5 RID: 181 RVA: 0x00004F8C File Offset: 0x0000318C
		public void Add(IXPathNavigable requiredCapabilitiesXmlElement)
		{
			this.AddElements((XmlElement)requiredCapabilitiesXmlElement);
		}

		// Token: 0x060000B6 RID: 182 RVA: 0x00004F9A File Offset: 0x0000319A
		public void Add(RequiredCapability requiredCapability)
		{
			this.requiredCapabilityCollection.Add(requiredCapability);
		}

		// Token: 0x060000B7 RID: 183 RVA: 0x00004FA8 File Offset: 0x000031A8
		private void AddElements(XmlElement requiredCapabilitiesXmlElement)
		{
			foreach (object obj in requiredCapabilitiesXmlElement.SelectNodes("./WP_Policy:RequiredCapability", GlobalVariables.NamespaceManager))
			{
				XmlElement xmlElement = (XmlElement)obj;
				bool flag = false;
				string attribute = xmlElement.GetAttribute("CapId");
				string[] array = ConstantStrings.ComponentCapabilityIdFilter;
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i] == attribute)
					{
						flag = true;
						break;
					}
				}
				if (this.ownerType == CapabilityOwnerType.Service)
				{
					array = ConstantStrings.ServiceCapabilityIdFilter;
					for (int i = 0; i < array.Length; i++)
					{
						if (array[i] == attribute)
						{
							flag = true;
							break;
						}
					}
				}
				if (this.ownerType == CapabilityOwnerType.Application)
				{
					array = ConstantStrings.BlockedCapabilityIdForApplicationFilter;
					for (int i = 0; i < array.Length; i++)
					{
						if (array[i] == attribute)
						{
							throw new PolicyCompilerInternalException(string.Format(GlobalVariables.Culture, "The capability '{0}' can't be used in application.", new object[]
							{
								attribute
							}));
						}
					}
				}
				if (!flag)
				{
					RequiredCapability requiredCapability = new RequiredCapability(this.ownerType);
					requiredCapability.Add(xmlElement);
					this.Add(requiredCapability);
				}
			}
		}

		// Token: 0x060000B8 RID: 184 RVA: 0x000050EC File Offset: 0x000032EC
		public string GetAllCapIds()
		{
			string text = string.Empty;
			if (this.requiredCapabilityCollection != null)
			{
				foreach (RequiredCapability requiredCapability in this.requiredCapabilityCollection)
				{
					text += requiredCapability.CapId;
				}
			}
			return text;
		}

		// Token: 0x060000B9 RID: 185 RVA: 0x00005154 File Offset: 0x00003354
		public void Print()
		{
			ReportingBase instance = ReportingBase.GetInstance();
			instance.XmlElementLine(ConstantStrings.IndentationLevel3, "RequiredCapabilities");
			foreach (RequiredCapability requiredCapability in this.requiredCapabilityCollection)
			{
				instance.DebugLine(string.Empty);
				requiredCapability.Print();
			}
		}

		// Token: 0x040000DE RID: 222
		private CapabilityOwnerType ownerType = CapabilityOwnerType.Unknown;

		// Token: 0x040000DF RID: 223
		private List<RequiredCapability> requiredCapabilityCollection = new List<RequiredCapability>();
	}
}
