using System;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace Microsoft.WindowsPhone.Security.SecurityPolicyCompiler
{
	// Token: 0x0200002E RID: 46
	public class CapabilityRulesPrivilege : RulePolicyElement
	{
		// Token: 0x1700004B RID: 75
		// (get) Token: 0x06000176 RID: 374 RVA: 0x000088A3 File Offset: 0x00006AA3
		protected CapabilityOwnerType OwnerType
		{
			get
			{
				return this.ownerType;
			}
		}

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x06000177 RID: 375 RVA: 0x000088AB File Offset: 0x00006AAB
		// (set) Token: 0x06000178 RID: 376 RVA: 0x000088B3 File Offset: 0x00006AB3
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

		// Token: 0x1700004D RID: 77
		// (get) Token: 0x06000179 RID: 377 RVA: 0x000088BC File Offset: 0x00006ABC
		// (set) Token: 0x0600017A RID: 378 RVA: 0x000088C4 File Offset: 0x00006AC4
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

		// Token: 0x0600017B RID: 379 RVA: 0x000088CD File Offset: 0x00006ACD
		public CapabilityRulesPrivilege()
		{
		}

		// Token: 0x0600017C RID: 380 RVA: 0x000088F2 File Offset: 0x00006AF2
		public CapabilityRulesPrivilege(CapabilityOwnerType value)
		{
			this.ownerType = value;
		}

		// Token: 0x0600017D RID: 381 RVA: 0x00008920 File Offset: 0x00006B20
		public override void Add(IXPathNavigable privilegeRuleXmlElement, string appCapSID, string svcCapSID)
		{
			if (this.OwnerType == CapabilityOwnerType.Application || this.OwnerType == CapabilityOwnerType.Service)
			{
				throw new PolicyCompilerInternalException(string.Format(GlobalVariables.Culture, "{0} cannot be private resource", new object[]
				{
					"Privilege"
				}));
			}
			this.AddAttributes((XmlElement)privilegeRuleXmlElement);
			this.CompileAttributes();
		}

		// Token: 0x0600017E RID: 382 RVA: 0x00008974 File Offset: 0x00006B74
		protected void AddAttributes(IXPathNavigable privilegeRuleXmlElement)
		{
			this.id = ((XmlElement)privilegeRuleXmlElement).GetAttribute("Id");
		}

		// Token: 0x0600017F RID: 383 RVA: 0x0000898C File Offset: 0x00006B8C
		protected void CompileAttributes()
		{
			this.Id = GlobalVariables.ResolveMacroReference(this.id, string.Format(GlobalVariables.Culture, "Element={0}, Attribute={1}", new object[]
			{
				"Privilege",
				"Id"
			}));
			this.ElementId = HashCalculator.CalculateSha256Hash(this.id, true);
		}

		// Token: 0x06000180 RID: 384 RVA: 0x000088BC File Offset: 0x00006ABC
		public override string GetAttributesString()
		{
			return this.id;
		}

		// Token: 0x06000181 RID: 385 RVA: 0x000089E4 File Offset: 0x00006BE4
		public override void Print()
		{
			ReportingBase instance = ReportingBase.GetInstance();
			instance.XmlElementLine(ConstantStrings.IndentationLevel4, "Privilege");
			instance.XmlAttributeLine(ConstantStrings.IndentationLevel5, "Id", this.Id);
			instance.XmlAttributeLine(ConstantStrings.IndentationLevel5, "Id", this.ElementId);
		}

		// Token: 0x04000109 RID: 265
		private CapabilityOwnerType ownerType = CapabilityOwnerType.Unknown;

		// Token: 0x0400010A RID: 266
		private string elementId = "Not Calculated";

		// Token: 0x0400010B RID: 267
		private string id = "Not Calculated";
	}
}
