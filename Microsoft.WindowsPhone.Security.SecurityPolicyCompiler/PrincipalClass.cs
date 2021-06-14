using System;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace Microsoft.WindowsPhone.Security.SecurityPolicyCompiler
{
	// Token: 0x02000034 RID: 52
	public class PrincipalClass : IPolicyElement
	{
		// Token: 0x17000057 RID: 87
		// (get) Token: 0x060001A9 RID: 425 RVA: 0x00009168 File Offset: 0x00007368
		// (set) Token: 0x060001AA RID: 426 RVA: 0x00009170 File Offset: 0x00007370
		[XmlAttribute(AttributeName = "ElementID")]
		public string ElementId { get; set; }

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x060001AB RID: 427 RVA: 0x00009179 File Offset: 0x00007379
		// (set) Token: 0x060001AC RID: 428 RVA: 0x00009181 File Offset: 0x00007381
		[XmlAttribute(AttributeName = "AttributeHash")]
		public string AttributeHash { get; set; }

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x060001AD RID: 429 RVA: 0x0000918A File Offset: 0x0000738A
		// (set) Token: 0x060001AE RID: 430 RVA: 0x00009192 File Offset: 0x00007392
		[XmlAttribute(AttributeName = "Name")]
		public string Name { get; set; }

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x060001AF RID: 431 RVA: 0x0000919B File Offset: 0x0000739B
		// (set) Token: 0x060001B0 RID: 432 RVA: 0x000091A3 File Offset: 0x000073A3
		[XmlElement(ElementName = "Executables")]
		public Executables Executables { get; set; }

		// Token: 0x1700005B RID: 91
		// (get) Token: 0x060001B1 RID: 433 RVA: 0x000091AC File Offset: 0x000073AC
		// (set) Token: 0x060001B2 RID: 434 RVA: 0x000091B4 File Offset: 0x000073B4
		[XmlElement(ElementName = "Directories")]
		public Directories Directories { get; set; }

		// Token: 0x1700005C RID: 92
		// (get) Token: 0x060001B3 RID: 435 RVA: 0x000091BD File Offset: 0x000073BD
		// (set) Token: 0x060001B4 RID: 436 RVA: 0x000091C5 File Offset: 0x000073C5
		[XmlElement(ElementName = "Certificates")]
		public Certificates Certificates { get; set; }

		// Token: 0x1700005D RID: 93
		// (get) Token: 0x060001B5 RID: 437 RVA: 0x000091CE File Offset: 0x000073CE
		// (set) Token: 0x060001B6 RID: 438 RVA: 0x000091D6 File Offset: 0x000073D6
		[XmlElement(ElementName = "Chambers")]
		public Chambers Chambers { get; set; }

		// Token: 0x060001B8 RID: 440 RVA: 0x000091E0 File Offset: 0x000073E0
		public void Add(IXPathNavigable principalClassXmlElement)
		{
			XmlElement principalClassXmlElement2 = (XmlElement)principalClassXmlElement;
			this.AddAttributes(principalClassXmlElement2);
			this.CompileAttributes();
			this.AddElements(principalClassXmlElement2);
			this.CalculateAttributeHash();
		}

		// Token: 0x060001B9 RID: 441 RVA: 0x0000920E File Offset: 0x0000740E
		private void AddAttributes(XmlElement principalClassXmlElement)
		{
			this.Name = principalClassXmlElement.GetAttribute("Name");
		}

		// Token: 0x060001BA RID: 442 RVA: 0x00009221 File Offset: 0x00007421
		private void CompileAttributes()
		{
			this.ElementId = HashCalculator.CalculateSha256Hash(this.Name, true);
		}

		// Token: 0x060001BB RID: 443 RVA: 0x00009238 File Offset: 0x00007438
		private void AddElements(XmlElement principalClassXmlElement)
		{
			XmlNodeList xmlNodeList = principalClassXmlElement.SelectNodes("./WP_Policy:Executables", GlobalVariables.NamespaceManager);
			if (xmlNodeList.Count > 0)
			{
				this.Executables = new Executables();
				this.Executables.Add(xmlNodeList[0]);
			}
			XmlNodeList xmlNodeList2 = principalClassXmlElement.SelectNodes("./WP_Policy:Directories", GlobalVariables.NamespaceManager);
			if (xmlNodeList2.Count > 0)
			{
				this.Directories = new Directories();
				this.Directories.Add(xmlNodeList2[0]);
			}
			XmlNodeList xmlNodeList3 = principalClassXmlElement.SelectNodes("./WP_Policy:Certificates", GlobalVariables.NamespaceManager);
			if (xmlNodeList3.Count > 0)
			{
				this.Certificates = new Certificates();
				this.Certificates.Add(xmlNodeList3[0]);
			}
			XmlNodeList xmlNodeList4 = principalClassXmlElement.SelectNodes("./WP_Policy:Chambers", GlobalVariables.NamespaceManager);
			if (xmlNodeList4.Count > 0)
			{
				this.Chambers = new Chambers();
				this.Chambers.Add(xmlNodeList4[0]);
			}
		}

		// Token: 0x060001BC RID: 444 RVA: 0x00009324 File Offset: 0x00007524
		private void CalculateAttributeHash()
		{
			StringBuilder stringBuilder = new StringBuilder(this.Name);
			if (this.Executables != null)
			{
				stringBuilder.Append(this.Executables.GetAllPaths());
			}
			if (this.Directories != null)
			{
				stringBuilder.Append(this.Directories.GetAllPaths());
			}
			if (this.Certificates != null)
			{
				stringBuilder.Append(this.Certificates.GetAllCertificateProperties());
			}
			if (this.Chambers != null)
			{
				stringBuilder.Append(this.Chambers.GetAllChamberProperties());
			}
			this.AttributeHash = HashCalculator.CalculateSha256Hash(stringBuilder.ToString(), true);
		}

		// Token: 0x060001BD RID: 445 RVA: 0x000093B8 File Offset: 0x000075B8
		public virtual void Print()
		{
			ReportingBase instance = ReportingBase.GetInstance();
			instance.XmlElementLine(ConstantStrings.IndentationLevel2, "PrincipalClass");
			instance.XmlAttributeLine(ConstantStrings.IndentationLevel3, "Name", this.Name);
			if (this.Executables != null)
			{
				instance.DebugLine(string.Empty);
				this.Executables.Print();
			}
			if (this.Directories != null)
			{
				instance.DebugLine(string.Empty);
				this.Directories.Print();
			}
			if (this.Certificates != null)
			{
				instance.DebugLine(string.Empty);
				this.Certificates.Print();
			}
			if (this.Chambers != null)
			{
				instance.DebugLine(string.Empty);
				this.Chambers.Print();
			}
		}
	}
}
