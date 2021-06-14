using System;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace Microsoft.WindowsPhone.Security.SecurityPolicyCompiler
{
	// Token: 0x02000036 RID: 54
	public abstract class AuthorizationRule : IPolicyElement
	{
		// Token: 0x17000063 RID: 99
		// (get) Token: 0x060001CF RID: 463 RVA: 0x00009810 File Offset: 0x00007A10
		// (set) Token: 0x060001D0 RID: 464 RVA: 0x00009818 File Offset: 0x00007A18
		[XmlAttribute(AttributeName = "ElementID")]
		public string ElementId { get; set; }

		// Token: 0x17000064 RID: 100
		// (get) Token: 0x060001D1 RID: 465 RVA: 0x00009821 File Offset: 0x00007A21
		// (set) Token: 0x060001D2 RID: 466 RVA: 0x00009829 File Offset: 0x00007A29
		[XmlAttribute(AttributeName = "AttributeHash")]
		public string AttributeHash { get; set; }

		// Token: 0x17000065 RID: 101
		// (get) Token: 0x060001D3 RID: 467 RVA: 0x00009832 File Offset: 0x00007A32
		// (set) Token: 0x060001D4 RID: 468 RVA: 0x0000983A File Offset: 0x00007A3A
		[XmlAttribute(AttributeName = "Name")]
		public string Name { get; set; }

		// Token: 0x060001D5 RID: 469 RVA: 0x00002A98 File Offset: 0x00000C98
		public AuthorizationRule()
		{
		}

		// Token: 0x060001D6 RID: 470 RVA: 0x00009844 File Offset: 0x00007A44
		public virtual void Add(IXPathNavigable authorizationRuleXmlElement)
		{
			XmlElement authorizationRuleXmlElement2 = (XmlElement)authorizationRuleXmlElement;
			this.AddAttributes(authorizationRuleXmlElement2);
		}

		// Token: 0x060001D7 RID: 471 RVA: 0x0000985F File Offset: 0x00007A5F
		private void AddAttributes(XmlElement authorizationRuleXmlElement)
		{
			this.Name = authorizationRuleXmlElement.GetAttribute("Name");
		}

		// Token: 0x060001D8 RID: 472
		public abstract void Print();
	}
}
