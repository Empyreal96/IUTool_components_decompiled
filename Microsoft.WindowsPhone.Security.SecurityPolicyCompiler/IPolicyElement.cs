using System;
using System.Xml.XPath;

namespace Microsoft.WindowsPhone.Security.SecurityPolicyCompiler
{
	// Token: 0x02000014 RID: 20
	public interface IPolicyElement
	{
		// Token: 0x0600007E RID: 126
		void Add(IXPathNavigable rulePolicyXmlElement);

		// Token: 0x0600007F RID: 127
		void Print();
	}
}
