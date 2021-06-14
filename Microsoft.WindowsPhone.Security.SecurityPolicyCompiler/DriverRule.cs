using System;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.WindowsPhone.Security.SecurityPolicyCompiler
{
	// Token: 0x02000032 RID: 50
	public class DriverRule : BaseRule
	{
		// Token: 0x060001A3 RID: 419 RVA: 0x00008EA4 File Offset: 0x000070A4
		public DriverRule(string ruleType)
		{
			base.RuleType = ruleType;
		}

		// Token: 0x060001A4 RID: 420 RVA: 0x00008EB3 File Offset: 0x000070B3
		public override void Add(IXPathNavigable driverRuleXmlElement, string appCapSID, string svcCapSID)
		{
			base.AddAttributes((XmlElement)driverRuleXmlElement);
			base.CompileAttributes(appCapSID, svcCapSID);
		}

		// Token: 0x060001A5 RID: 421 RVA: 0x00008EC9 File Offset: 0x000070C9
		public void Add(string appCapSID, string svcCapSID, string rights)
		{
			base.CompileResolvedAttributes(appCapSID, svcCapSID, rights);
		}
	}
}
