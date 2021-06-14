using System;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.WindowsPhone.Security.SecurityPolicyCompiler
{
	// Token: 0x0200002F RID: 47
	public class CapabilityRulesWindows : CapabilityRulesPrivilege
	{
		// Token: 0x06000182 RID: 386 RVA: 0x00008A31 File Offset: 0x00006C31
		public CapabilityRulesWindows()
		{
		}

		// Token: 0x06000183 RID: 387 RVA: 0x00008A39 File Offset: 0x00006C39
		public CapabilityRulesWindows(CapabilityOwnerType value) : base(value)
		{
		}

		// Token: 0x06000184 RID: 388 RVA: 0x00008A44 File Offset: 0x00006C44
		public override void Add(IXPathNavigable windowsCapbilityRuleXmlElement, string appCapSID, string svcCapSID)
		{
			if (base.OwnerType == CapabilityOwnerType.Application || base.OwnerType == CapabilityOwnerType.Service)
			{
				throw new PolicyCompilerInternalException(string.Format(GlobalVariables.Culture, "{0} cannot be private resource", new object[]
				{
					"WindowsCapability"
				}));
			}
			base.AddAttributes((XmlElement)windowsCapbilityRuleXmlElement);
			base.CompileAttributes();
		}

		// Token: 0x06000185 RID: 389 RVA: 0x00008A98 File Offset: 0x00006C98
		public override void Print()
		{
			ReportingBase instance = ReportingBase.GetInstance();
			instance.XmlElementLine(ConstantStrings.IndentationLevel4, "WindowsCapability");
			instance.XmlAttributeLine(ConstantStrings.IndentationLevel5, "Id", base.Id);
			instance.XmlAttributeLine(ConstantStrings.IndentationLevel5, "Id", base.ElementId);
		}
	}
}
