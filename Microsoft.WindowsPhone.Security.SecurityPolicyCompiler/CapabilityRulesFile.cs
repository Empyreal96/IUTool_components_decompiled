using System;
using System.Xml;

namespace Microsoft.WindowsPhone.Security.SecurityPolicyCompiler
{
	// Token: 0x02000022 RID: 34
	public class CapabilityRulesFile : RuleWithPathInput
	{
		// Token: 0x0600012B RID: 299 RVA: 0x00006B7C File Offset: 0x00004D7C
		public CapabilityRulesFile()
		{
			base.RuleType = "File";
			base.RuleInheritanceInfo = false;
		}

		// Token: 0x0600012C RID: 300 RVA: 0x00006B96 File Offset: 0x00004D96
		public CapabilityRulesFile(CapabilityOwnerType value) : this()
		{
			base.OwnerType = value;
		}

		// Token: 0x0600012D RID: 301 RVA: 0x00006BA5 File Offset: 0x00004DA5
		protected sealed override void AddAttributes(XmlElement BasicRuleXmlElement)
		{
			base.AddAttributes(BasicRuleXmlElement);
			base.Flags |= 260U;
		}

		// Token: 0x0600012E RID: 302 RVA: 0x00006BC0 File Offset: 0x00004DC0
		protected sealed override void ValidateOutPath()
		{
			base.ValidateFileOutPath(false);
		}
	}
}
