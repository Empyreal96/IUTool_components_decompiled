using System;
using System.Xml;

namespace Microsoft.WindowsPhone.Security.SecurityPolicyCompiler
{
	// Token: 0x0200002B RID: 43
	public class CapabilityRulesTransientObject : RuleWithPathInput
	{
		// Token: 0x0600014F RID: 335 RVA: 0x00007A37 File Offset: 0x00005C37
		public CapabilityRulesTransientObject()
		{
			base.RuleType = "TransientObject";
			base.RuleInheritanceInfo = false;
		}

		// Token: 0x06000150 RID: 336 RVA: 0x00007A51 File Offset: 0x00005C51
		public CapabilityRulesTransientObject(CapabilityOwnerType value) : this()
		{
			base.OwnerType = value;
		}

		// Token: 0x06000151 RID: 337 RVA: 0x00007A60 File Offset: 0x00005C60
		protected sealed override void AddAttributes(XmlElement BasicRuleXmlElement)
		{
			base.AddAttributes(BasicRuleXmlElement);
			this.transientObjectType = BasicRuleXmlElement.GetAttribute("Type");
			base.Flags |= 5U;
		}

		// Token: 0x06000152 RID: 338 RVA: 0x00007A88 File Offset: 0x00005C88
		protected sealed override void CompileAttributes(string appCapSID, string svcCapSID)
		{
			base.CompileAttributes(appCapSID, svcCapSID);
			this.transientObjectType = base.ResolveMacro(this.transientObjectType, "Type");
			this.CalculatePath();
			base.CalculateElementId(string.Format(GlobalVariables.Culture, "{0}{1}", new object[]
			{
				this.transientObjectType,
				base.NormalizedPath
			}));
		}

		// Token: 0x06000153 RID: 339 RVA: 0x00007AE8 File Offset: 0x00005CE8
		private void CalculatePath()
		{
			base.Path = string.Format(GlobalVariables.Culture, "{0}{1}{2}{3}", new object[]
			{
				"%5C%5C.%5C",
				this.transientObjectType,
				"%5C",
				base.Path
			});
			base.Path = base.Path.Replace("\\", "%5C");
		}

		// Token: 0x04000100 RID: 256
		private string transientObjectType;
	}
}
