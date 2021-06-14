using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace Microsoft.WindowsPhone.Security.SecurityPolicyCompiler
{
	// Token: 0x0200003C RID: 60
	public class Certificates : IPolicyElement
	{
		// Token: 0x1700006E RID: 110
		// (get) Token: 0x060001FC RID: 508 RVA: 0x00009D04 File Offset: 0x00007F04
		// (set) Token: 0x060001FD RID: 509 RVA: 0x00009D0C File Offset: 0x00007F0C
		[XmlElement(ElementName = "Certificate")]
		public List<Certificate> CertificateCollection { get; set; }

		// Token: 0x060001FF RID: 511 RVA: 0x00009D18 File Offset: 0x00007F18
		public void Add(IXPathNavigable certificatesXmlElement)
		{
			XmlElement certificatesXmlElement2 = (XmlElement)certificatesXmlElement;
			this.AddElements(certificatesXmlElement2);
		}

		// Token: 0x06000200 RID: 512 RVA: 0x00009D34 File Offset: 0x00007F34
		public void AddElements(IXPathNavigable certificatesXmlElement)
		{
			XmlNodeList xmlNodeList = ((XmlElement)certificatesXmlElement).SelectNodes("./WP_Policy:Certificate", GlobalVariables.NamespaceManager);
			if (xmlNodeList.Count > 0)
			{
				if (this.CertificateCollection == null)
				{
					this.CertificateCollection = new List<Certificate>();
				}
				foreach (object obj in xmlNodeList)
				{
					XmlElement certificateXmlElement = (XmlElement)obj;
					Certificate certificate = new Certificate();
					certificate.Add(certificateXmlElement);
					this.CertificateCollection.Add(certificate);
				}
			}
		}

		// Token: 0x06000201 RID: 513 RVA: 0x00009DD0 File Offset: 0x00007FD0
		public string GetAllCertificateProperties()
		{
			string text = string.Empty;
			if (this.CertificateCollection != null)
			{
				foreach (Certificate certificate in this.CertificateCollection)
				{
					text += certificate.Type;
					if (certificate.EKU != null)
					{
						text += certificate.EKU;
					}
					if (certificate.Thumbprint != null)
					{
						text += certificate.ThumbprintAlgorithm;
						text += certificate.Thumbprint;
					}
				}
			}
			return text;
		}

		// Token: 0x06000202 RID: 514 RVA: 0x00009E70 File Offset: 0x00008070
		public void Print()
		{
			ReportingBase instance = ReportingBase.GetInstance();
			instance.XmlElementLine(ConstantStrings.IndentationLevel3, "Certificate");
			foreach (Certificate certificate in this.CertificateCollection)
			{
				instance.DebugLine(string.Empty);
				certificate.Print();
			}
		}
	}
}
