using System;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace Microsoft.WindowsPhone.Security.SecurityPolicyCompiler
{
	// Token: 0x02000038 RID: 56
	public class CapabilityRule : AuthorizationRule
	{
		// Token: 0x17000068 RID: 104
		// (get) Token: 0x060001E3 RID: 483 RVA: 0x000099D3 File Offset: 0x00007BD3
		// (set) Token: 0x060001E4 RID: 484 RVA: 0x000099DB File Offset: 0x00007BDB
		[XmlAttribute(AttributeName = "CapabilityClass")]
		public string CapabilityClass { get; set; }

		// Token: 0x17000069 RID: 105
		// (get) Token: 0x060001E5 RID: 485 RVA: 0x000099E4 File Offset: 0x00007BE4
		// (set) Token: 0x060001E6 RID: 486 RVA: 0x000099EC File Offset: 0x00007BEC
		[XmlAttribute(AttributeName = "PrincipalClass")]
		public string PrincipalClass { get; set; }

		// Token: 0x060001E8 RID: 488 RVA: 0x000099F8 File Offset: 0x00007BF8
		public override void Add(IXPathNavigable capabilityRuleXmlElement)
		{
			base.Add(capabilityRuleXmlElement);
			XmlElement capabilityRuleXmlElement2 = (XmlElement)capabilityRuleXmlElement;
			this.AddAttributes(capabilityRuleXmlElement2);
			this.CompileAttributes();
			this.CalculateAttributeHash();
		}

		// Token: 0x060001E9 RID: 489 RVA: 0x00009A26 File Offset: 0x00007C26
		private void AddAttributes(XmlElement capabilityRuleXmlElement)
		{
			this.CapabilityClass = capabilityRuleXmlElement.GetAttribute("CapabilityClass");
			this.PrincipalClass = capabilityRuleXmlElement.GetAttribute("PrincipalClass");
		}

		// Token: 0x060001EA RID: 490 RVA: 0x00009A4A File Offset: 0x00007C4A
		private void CompileAttributes()
		{
			base.ElementId = HashCalculator.CalculateSha256Hash(base.Name, true);
		}

		// Token: 0x060001EB RID: 491 RVA: 0x00009A5E File Offset: 0x00007C5E
		private void CalculateAttributeHash()
		{
			base.AttributeHash = HashCalculator.CalculateSha256Hash(base.Name + this.CapabilityClass + this.PrincipalClass, true);
		}

		// Token: 0x060001EC RID: 492 RVA: 0x00009A84 File Offset: 0x00007C84
		public override void Print()
		{
			ReportingBase instance = ReportingBase.GetInstance();
			instance.XmlElementLine(ConstantStrings.IndentationLevel2, "CapabilityRule");
			instance.XmlAttributeLine(ConstantStrings.IndentationLevel3, "Name", base.Name);
			instance.XmlAttributeLine(ConstantStrings.IndentationLevel3, "CapabilityClass", this.CapabilityClass);
			instance.XmlAttributeLine(ConstantStrings.IndentationLevel3, "PrincipalClass", this.PrincipalClass);
		}
	}
}
