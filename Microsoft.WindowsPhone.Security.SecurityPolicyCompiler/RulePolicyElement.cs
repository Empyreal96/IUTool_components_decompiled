using System;
using System.Xml.XPath;

namespace Microsoft.WindowsPhone.Security.SecurityPolicyCompiler
{
	// Token: 0x02000031 RID: 49
	public abstract class RulePolicyElement
	{
		// Token: 0x0600019F RID: 415
		public abstract void Add(IXPathNavigable xmlPathNavigator, string appCapSID, string svcCapSID);

		// Token: 0x060001A0 RID: 416
		public abstract string GetAttributesString();

		// Token: 0x060001A1 RID: 417
		public abstract void Print();
	}
}
