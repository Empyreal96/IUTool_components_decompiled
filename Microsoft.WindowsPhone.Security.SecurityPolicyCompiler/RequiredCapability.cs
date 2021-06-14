using System;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace Microsoft.WindowsPhone.Security.SecurityPolicyCompiler
{
	// Token: 0x0200001A RID: 26
	public class RequiredCapability : IPolicyElement
	{
		// Token: 0x17000027 RID: 39
		// (set) Token: 0x060000BA RID: 186 RVA: 0x000051C8 File Offset: 0x000033C8
		private CapabilityOwnerType OwnerType
		{
			set
			{
				this.ownerType = value;
			}
		}

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x060000BB RID: 187 RVA: 0x000051D1 File Offset: 0x000033D1
		// (set) Token: 0x060000BC RID: 188 RVA: 0x000051D9 File Offset: 0x000033D9
		[XmlAttribute(AttributeName = "CapId")]
		public string CapId
		{
			get
			{
				return this.capId;
			}
			set
			{
				this.capId = value;
			}
		}

		// Token: 0x060000BD RID: 189 RVA: 0x000051E2 File Offset: 0x000033E2
		public RequiredCapability()
		{
		}

		// Token: 0x060000BE RID: 190 RVA: 0x000051FC File Offset: 0x000033FC
		public RequiredCapability(CapabilityOwnerType value)
		{
			this.OwnerType = value;
		}

		// Token: 0x060000BF RID: 191 RVA: 0x0000521D File Offset: 0x0000341D
		public void Add(IXPathNavigable requiredCapabilityXmlElement)
		{
			this.AddAttributes((XmlElement)requiredCapabilityXmlElement);
			this.CompileAttributes();
		}

		// Token: 0x060000C0 RID: 192 RVA: 0x00005231 File Offset: 0x00003431
		public void Add(string inputCapId, bool needCompile)
		{
			this.capId = inputCapId;
			if (needCompile)
			{
				this.CompileAttributes();
			}
		}

		// Token: 0x060000C1 RID: 193 RVA: 0x00005243 File Offset: 0x00003443
		private void AddAttributes(XmlElement requiredCapabilityXmlElement)
		{
			this.CapId = requiredCapabilityXmlElement.GetAttribute("CapId");
		}

		// Token: 0x060000C2 RID: 194 RVA: 0x00005258 File Offset: 0x00003458
		private void CompileAttributes()
		{
			CapabilityOwnerType capabilityOwnerType = this.ownerType;
			if (capabilityOwnerType == CapabilityOwnerType.Application)
			{
				this.CapId = SidBuilder.BuildApplicationCapabilitySidString(this.CapId);
				return;
			}
			if (capabilityOwnerType != CapabilityOwnerType.Service)
			{
				throw new PolicyCompilerInternalException("SecurityPolicyCompiler Internal Error: RequiredCapability's OwnerType can't be determined");
			}
			this.CapId = GlobalVariables.SidMapping[this.CapId];
		}

		// Token: 0x060000C3 RID: 195 RVA: 0x000052A9 File Offset: 0x000034A9
		public void Print()
		{
			ReportingBase instance = ReportingBase.GetInstance();
			instance.XmlElementLine(ConstantStrings.IndentationLevel4, "RequiredCapability");
			instance.XmlAttributeLine(ConstantStrings.IndentationLevel5, "CapId", this.CapId);
		}

		// Token: 0x040000E0 RID: 224
		private CapabilityOwnerType ownerType = CapabilityOwnerType.Unknown;

		// Token: 0x040000E1 RID: 225
		private string capId = "Not Calculated";
	}
}
