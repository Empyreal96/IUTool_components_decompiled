using System;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace Microsoft.WindowsPhone.Security.SecurityPolicyCompiler
{
	// Token: 0x02000044 RID: 68
	public class MemberCapabilityClass : IPolicyElement
	{
		// Token: 0x1700007A RID: 122
		// (get) Token: 0x06000233 RID: 563 RVA: 0x0000A5DE File Offset: 0x000087DE
		// (set) Token: 0x06000234 RID: 564 RVA: 0x0000A5E6 File Offset: 0x000087E6
		[XmlAttribute(AttributeName = "Name")]
		public string Name { get; set; }

		// Token: 0x06000236 RID: 566 RVA: 0x0000A5EF File Offset: 0x000087EF
		public virtual void Add(IXPathNavigable capabilityClassXmlElement)
		{
			this.AddAttributes((XmlElement)capabilityClassXmlElement);
		}

		// Token: 0x06000237 RID: 567 RVA: 0x0000A600 File Offset: 0x00008800
		protected virtual void AddAttributes(IXPathNavigable capabilityClassXmlElement)
		{
			XmlElement xmlElement = (XmlElement)capabilityClassXmlElement;
			this.Name = xmlElement.GetAttribute("Name");
		}

		// Token: 0x06000238 RID: 568 RVA: 0x0000A625 File Offset: 0x00008825
		public virtual void Print()
		{
			ReportingBase.GetInstance().XmlAttributeLine(ConstantStrings.IndentationLevel4, "Name", this.Name);
		}
	}
}
