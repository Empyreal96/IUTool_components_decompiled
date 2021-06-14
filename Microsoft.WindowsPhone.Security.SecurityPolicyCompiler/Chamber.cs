using System;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace Microsoft.WindowsPhone.Security.SecurityPolicyCompiler
{
	// Token: 0x02000042 RID: 66
	public class Chamber : IPolicyElement
	{
		// Token: 0x17000077 RID: 119
		// (get) Token: 0x06000224 RID: 548 RVA: 0x0000A463 File Offset: 0x00008663
		// (set) Token: 0x06000225 RID: 549 RVA: 0x0000A46B File Offset: 0x0000866B
		[XmlAttribute(AttributeName = "Name")]
		public string Name { get; set; }

		// Token: 0x06000227 RID: 551 RVA: 0x0000A474 File Offset: 0x00008674
		public virtual void Add(IXPathNavigable chamberXmlElement)
		{
			this.AddAttributes((XmlElement)chamberXmlElement);
		}

		// Token: 0x06000228 RID: 552 RVA: 0x0000A484 File Offset: 0x00008684
		protected virtual void AddAttributes(IXPathNavigable chamberXmlElement)
		{
			XmlElement xmlElement = (XmlElement)chamberXmlElement;
			this.Name = xmlElement.GetAttribute("Name");
		}

		// Token: 0x06000229 RID: 553 RVA: 0x0000A4A9 File Offset: 0x000086A9
		public virtual void Print()
		{
			ReportingBase instance = ReportingBase.GetInstance();
			instance.XmlElementLine(ConstantStrings.IndentationLevel3, "Chamber");
			instance.XmlAttributeLine(ConstantStrings.IndentationLevel4, "Name", this.Name);
		}
	}
}
