using System;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace Microsoft.WindowsPhone.Security.SecurityPolicyCompiler
{
	// Token: 0x02000041 RID: 65
	public class Certificate : IPolicyElement
	{
		// Token: 0x17000073 RID: 115
		// (get) Token: 0x06000218 RID: 536 RVA: 0x0000A22D File Offset: 0x0000842D
		// (set) Token: 0x06000219 RID: 537 RVA: 0x0000A235 File Offset: 0x00008435
		[XmlAttribute(AttributeName = "Type")]
		public string Type { get; set; }

		// Token: 0x17000074 RID: 116
		// (get) Token: 0x0600021A RID: 538 RVA: 0x0000A23E File Offset: 0x0000843E
		// (set) Token: 0x0600021B RID: 539 RVA: 0x0000A246 File Offset: 0x00008446
		[XmlAttribute(AttributeName = "EKU")]
		public string EKU { get; set; }

		// Token: 0x17000075 RID: 117
		// (get) Token: 0x0600021C RID: 540 RVA: 0x0000A24F File Offset: 0x0000844F
		// (set) Token: 0x0600021D RID: 541 RVA: 0x0000A257 File Offset: 0x00008457
		[XmlAttribute(AttributeName = "Alg")]
		public string ThumbprintAlgorithm { get; set; }

		// Token: 0x17000076 RID: 118
		// (get) Token: 0x0600021E RID: 542 RVA: 0x0000A260 File Offset: 0x00008460
		// (set) Token: 0x0600021F RID: 543 RVA: 0x0000A268 File Offset: 0x00008468
		[XmlAttribute(AttributeName = "Thumbprint")]
		public string Thumbprint { get; set; }

		// Token: 0x06000221 RID: 545 RVA: 0x0000A271 File Offset: 0x00008471
		public virtual void Add(IXPathNavigable certificateXmlElement)
		{
			this.AddAttributes((XmlElement)certificateXmlElement);
		}

		// Token: 0x06000222 RID: 546 RVA: 0x0000A280 File Offset: 0x00008480
		protected virtual void AddAttributes(IXPathNavigable certificateXmlElement)
		{
			XmlElement xmlElement = (XmlElement)certificateXmlElement;
			this.Type = xmlElement.GetAttribute("Type");
			if (xmlElement.HasAttribute("EKU"))
			{
				this.EKU = xmlElement.GetAttribute("EKU");
			}
			if (xmlElement.HasAttribute("Thumbprint"))
			{
				this.ThumbprintAlgorithm = "Sha256";
				uint num = 0U;
				if (!string.IsNullOrEmpty(xmlElement.GetAttribute("Alg")))
				{
					this.ThumbprintAlgorithm = xmlElement.GetAttribute("Alg");
				}
				this.Thumbprint = NormalizedString.Get(xmlElement.GetAttribute("Thumbprint"));
				string thumbprintAlgorithm = this.ThumbprintAlgorithm;
				if (!(thumbprintAlgorithm == "Sha256"))
				{
					if (!(thumbprintAlgorithm == "Sha384"))
					{
						if (thumbprintAlgorithm == "Sha512")
						{
							num = 128U;
						}
					}
					else
					{
						num = 96U;
					}
				}
				else
				{
					num = 64U;
				}
				if ((long)this.Thumbprint.Length != (long)((ulong)num))
				{
					throw new PolicyCompilerInternalException(string.Format(GlobalVariables.Culture, "Invalid {0} attribute. Length should be {1} characters", new object[]
					{
						"Thumbprint",
						num
					}));
				}
			}
			if (this.EKU == null && this.Thumbprint == null)
			{
				throw new PolicyCompilerInternalException(string.Format(GlobalVariables.Culture, "{0} element must have an {1} attribute or a {2} attribute", new object[]
				{
					"Certificate",
					"EKU",
					"Thumbprint"
				}));
			}
		}

		// Token: 0x06000223 RID: 547 RVA: 0x0000A3D8 File Offset: 0x000085D8
		public virtual void Print()
		{
			ReportingBase instance = ReportingBase.GetInstance();
			instance.XmlElementLine(ConstantStrings.IndentationLevel3, "Certificate");
			instance.XmlAttributeLine(ConstantStrings.IndentationLevel4, "Type", this.Type);
			if (this.EKU != null)
			{
				instance.XmlAttributeLine(ConstantStrings.IndentationLevel4, "EKU", this.EKU);
			}
			if (this.Thumbprint != null)
			{
				instance.XmlAttributeLine(ConstantStrings.IndentationLevel4, "Alg", this.ThumbprintAlgorithm);
				instance.XmlAttributeLine(ConstantStrings.IndentationLevel4, "Thumbprint", this.Thumbprint);
			}
		}
	}
}
