using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace Microsoft.WindowsPhone.Security.SecurityPolicyCompiler
{
	// Token: 0x02000035 RID: 53
	public class CapabilityClass : IPolicyElement
	{
		// Token: 0x1700005E RID: 94
		// (get) Token: 0x060001BE RID: 446 RVA: 0x00009469 File Offset: 0x00007669
		// (set) Token: 0x060001BF RID: 447 RVA: 0x00009471 File Offset: 0x00007671
		[XmlAttribute(AttributeName = "ElementID")]
		public string ElementId { get; set; }

		// Token: 0x1700005F RID: 95
		// (get) Token: 0x060001C0 RID: 448 RVA: 0x0000947A File Offset: 0x0000767A
		// (set) Token: 0x060001C1 RID: 449 RVA: 0x00009482 File Offset: 0x00007682
		[XmlAttribute(AttributeName = "AttributeHash")]
		public string AttributeHash { get; set; }

		// Token: 0x17000060 RID: 96
		// (get) Token: 0x060001C2 RID: 450 RVA: 0x0000948B File Offset: 0x0000768B
		// (set) Token: 0x060001C3 RID: 451 RVA: 0x00009493 File Offset: 0x00007693
		[XmlAttribute(AttributeName = "Name")]
		public string Name { get; set; }

		// Token: 0x17000061 RID: 97
		// (get) Token: 0x060001C4 RID: 452 RVA: 0x0000949C File Offset: 0x0000769C
		// (set) Token: 0x060001C5 RID: 453 RVA: 0x000094A4 File Offset: 0x000076A4
		[XmlElement(ElementName = "MemberCapability")]
		public List<MemberCapability> CapabilityCollection { get; set; }

		// Token: 0x17000062 RID: 98
		// (get) Token: 0x060001C6 RID: 454 RVA: 0x000094AD File Offset: 0x000076AD
		// (set) Token: 0x060001C7 RID: 455 RVA: 0x000094B5 File Offset: 0x000076B5
		[XmlElement(ElementName = "MemberCapabilityClass")]
		public List<MemberCapabilityClass> CapabilityClassCollection { get; set; }

		// Token: 0x060001C9 RID: 457 RVA: 0x000094C0 File Offset: 0x000076C0
		public void Add(IXPathNavigable capabilityClassXmlElement)
		{
			XmlElement capabilityClassXmlElement2 = (XmlElement)capabilityClassXmlElement;
			this.AddAttributes(capabilityClassXmlElement2);
			this.CompileAttributes();
			this.AddElements(capabilityClassXmlElement2);
			this.CalculateAttributeHash();
		}

		// Token: 0x060001CA RID: 458 RVA: 0x000094F0 File Offset: 0x000076F0
		private void AddAttributes(IXPathNavigable capabilityClassXmlElement)
		{
			XmlElement xmlElement = (XmlElement)capabilityClassXmlElement;
			this.Name = xmlElement.GetAttribute("Name");
		}

		// Token: 0x060001CB RID: 459 RVA: 0x00009515 File Offset: 0x00007715
		private void CompileAttributes()
		{
			this.ElementId = HashCalculator.CalculateSha256Hash(this.Name, true);
		}

		// Token: 0x060001CC RID: 460 RVA: 0x0000952C File Offset: 0x0000772C
		private void AddElements(IXPathNavigable capabilityClassXmlElement)
		{
			XmlElement xmlElement = (XmlElement)capabilityClassXmlElement;
			XmlNodeList xmlNodeList = xmlElement.SelectNodes("./WP_Policy:MemberCapability", GlobalVariables.NamespaceManager);
			if (xmlNodeList.Count > 0)
			{
				if (this.CapabilityCollection == null)
				{
					this.CapabilityCollection = new List<MemberCapability>();
				}
				foreach (object obj in xmlNodeList)
				{
					XmlElement capabilityXmlElement = (XmlElement)obj;
					MemberCapability memberCapability = new MemberCapability();
					memberCapability.Add(capabilityXmlElement);
					this.CapabilityCollection.Add(memberCapability);
				}
			}
			XmlNodeList xmlNodeList2 = xmlElement.SelectNodes("./WP_Policy:MemberCapabilityClass", GlobalVariables.NamespaceManager);
			if (xmlNodeList2.Count > 0)
			{
				if (this.CapabilityClassCollection == null)
				{
					this.CapabilityClassCollection = new List<MemberCapabilityClass>();
				}
				foreach (object obj2 in xmlNodeList2)
				{
					XmlElement capabilityClassXmlElement2 = (XmlElement)obj2;
					MemberCapabilityClass memberCapabilityClass = new MemberCapabilityClass();
					memberCapabilityClass.Add(capabilityClassXmlElement2);
					this.CapabilityClassCollection.Add(memberCapabilityClass);
				}
			}
		}

		// Token: 0x060001CD RID: 461 RVA: 0x00009658 File Offset: 0x00007858
		private void CalculateAttributeHash()
		{
			StringBuilder stringBuilder = new StringBuilder(this.Name);
			if (this.CapabilityCollection != null)
			{
				foreach (MemberCapability memberCapability in this.CapabilityCollection)
				{
					stringBuilder.Append(memberCapability.Id);
				}
			}
			if (this.CapabilityClassCollection != null)
			{
				foreach (MemberCapabilityClass memberCapabilityClass in this.CapabilityClassCollection)
				{
					stringBuilder.Append(memberCapabilityClass.Name);
				}
			}
			this.AttributeHash = HashCalculator.CalculateSha256Hash(stringBuilder.ToString(), true);
		}

		// Token: 0x060001CE RID: 462 RVA: 0x0000972C File Offset: 0x0000792C
		public virtual void Print()
		{
			ReportingBase instance = ReportingBase.GetInstance();
			instance.XmlElementLine(ConstantStrings.IndentationLevel2, "CapabilityClass");
			instance.XmlAttributeLine(ConstantStrings.IndentationLevel3, "Name", this.Name);
			if (this.CapabilityCollection != null)
			{
				foreach (MemberCapability memberCapability in this.CapabilityCollection)
				{
					instance.DebugLine(string.Empty);
					memberCapability.Print();
				}
			}
			if (this.CapabilityClassCollection != null)
			{
				foreach (MemberCapabilityClass memberCapabilityClass in this.CapabilityClassCollection)
				{
					instance.DebugLine(string.Empty);
					memberCapabilityClass.Print();
				}
			}
		}
	}
}
