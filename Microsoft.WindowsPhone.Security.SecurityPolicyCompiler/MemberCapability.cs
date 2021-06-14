using System;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace Microsoft.WindowsPhone.Security.SecurityPolicyCompiler
{
	// Token: 0x02000043 RID: 67
	public class MemberCapability : IPolicyElement
	{
		// Token: 0x17000078 RID: 120
		// (get) Token: 0x0600022A RID: 554 RVA: 0x0000A4D5 File Offset: 0x000086D5
		// (set) Token: 0x0600022B RID: 555 RVA: 0x0000A4DD File Offset: 0x000086DD
		[XmlAttribute(AttributeName = "Id")]
		public string Id { get; set; }

		// Token: 0x17000079 RID: 121
		// (get) Token: 0x0600022C RID: 556 RVA: 0x0000A4E6 File Offset: 0x000086E6
		// (set) Token: 0x0600022D RID: 557 RVA: 0x0000A4EE File Offset: 0x000086EE
		[XmlAttribute(AttributeName = "CapId")]
		public string CapId { get; set; }

		// Token: 0x0600022F RID: 559 RVA: 0x0000A4F7 File Offset: 0x000086F7
		public virtual void Add(IXPathNavigable capabilityXmlElement)
		{
			this.AddAttributes((XmlElement)capabilityXmlElement);
			this.CompileAttributes();
		}

		// Token: 0x06000230 RID: 560 RVA: 0x0000A50C File Offset: 0x0000870C
		protected virtual void AddAttributes(IXPathNavigable capabilityXmlElement)
		{
			XmlElement xmlElement = (XmlElement)capabilityXmlElement;
			this.Id = xmlElement.GetAttribute("Id");
		}

		// Token: 0x06000231 RID: 561 RVA: 0x0000A534 File Offset: 0x00008734
		private void CompileAttributes()
		{
			this.Id = GlobalVariables.ResolveMacroReference(this.Id, string.Format(GlobalVariables.Culture, "Element={0}, Attribute={1}", new object[]
			{
				"MemberCapability",
				"Id"
			}));
			if (this.Id.StartsWith("S-1-15-3", StringComparison.InvariantCulture))
			{
				this.CapId = SidBuilder.BuildApplicationCapabilitySidString(ConstantStrings.EveryoneCapability);
				return;
			}
			this.CapId = SidBuilder.BuildApplicationCapabilitySidString(this.Id);
		}

		// Token: 0x06000232 RID: 562 RVA: 0x0000A5AC File Offset: 0x000087AC
		public virtual void Print()
		{
			ReportingBase instance = ReportingBase.GetInstance();
			instance.XmlAttributeLine(ConstantStrings.IndentationLevel4, "Id", this.Id);
			instance.XmlAttributeLine(ConstantStrings.IndentationLevel4, "CapId", this.CapId);
		}
	}
}
