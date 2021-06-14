using System;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace Microsoft.WindowsPhone.Security.SecurityPolicyCompiler
{
	// Token: 0x02000037 RID: 55
	public class ExecuteRule : AuthorizationRule
	{
		// Token: 0x17000066 RID: 102
		// (get) Token: 0x060001D9 RID: 473 RVA: 0x00009872 File Offset: 0x00007A72
		// (set) Token: 0x060001DA RID: 474 RVA: 0x0000987A File Offset: 0x00007A7A
		[XmlAttribute(AttributeName = "PrincipalClass")]
		public string PrincipalClass { get; set; }

		// Token: 0x17000067 RID: 103
		// (get) Token: 0x060001DB RID: 475 RVA: 0x00009883 File Offset: 0x00007A83
		// (set) Token: 0x060001DC RID: 476 RVA: 0x0000988B File Offset: 0x00007A8B
		[XmlAttribute(AttributeName = "TargetChamber")]
		public string TargetChamber { get; set; }

		// Token: 0x060001DE RID: 478 RVA: 0x0000989C File Offset: 0x00007A9C
		public override void Add(IXPathNavigable executeRuleXmlElement)
		{
			base.Add(executeRuleXmlElement);
			XmlElement executeRuleXmlElement2 = (XmlElement)executeRuleXmlElement;
			this.AddAttributes(executeRuleXmlElement2);
			this.CompileAttributes();
			this.CalculateAttributeHash();
		}

		// Token: 0x060001DF RID: 479 RVA: 0x000098CA File Offset: 0x00007ACA
		private void AddAttributes(XmlElement executeRuleXmlElement)
		{
			this.PrincipalClass = executeRuleXmlElement.GetAttribute("PrincipalClass");
			this.TargetChamber = NormalizedString.Get(executeRuleXmlElement.GetAttribute("TargetChamber"));
		}

		// Token: 0x060001E0 RID: 480 RVA: 0x000098F4 File Offset: 0x00007AF4
		private void CompileAttributes()
		{
			base.ElementId = HashCalculator.CalculateSha256Hash(base.Name, true);
			this.TargetChamber = GlobalVariables.ResolveMacroReference(this.TargetChamber, string.Format(GlobalVariables.Culture, "Element={0}, Attribute={1}", new object[]
			{
				"ExecuteRule",
				"TargetChamber"
			}));
		}

		// Token: 0x060001E1 RID: 481 RVA: 0x00009949 File Offset: 0x00007B49
		private void CalculateAttributeHash()
		{
			base.AttributeHash = HashCalculator.CalculateSha256Hash(base.Name + this.PrincipalClass + this.TargetChamber, true);
		}

		// Token: 0x060001E2 RID: 482 RVA: 0x00009970 File Offset: 0x00007B70
		public override void Print()
		{
			ReportingBase instance = ReportingBase.GetInstance();
			instance.XmlElementLine(ConstantStrings.IndentationLevel2, "ExecuteRule");
			instance.XmlAttributeLine(ConstantStrings.IndentationLevel3, "Name", base.Name);
			instance.XmlAttributeLine(ConstantStrings.IndentationLevel3, "PrincipalClass", this.PrincipalClass);
			instance.XmlAttributeLine(ConstantStrings.IndentationLevel3, "TargetChamber", this.TargetChamber);
		}
	}
}
