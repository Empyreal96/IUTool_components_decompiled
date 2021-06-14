using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace Microsoft.WindowsPhone.Security.SecurityPolicyCompiler
{
	// Token: 0x0200001F RID: 31
	public class Capabilities : IPolicyElement
	{
		// Token: 0x1700003C RID: 60
		// (get) Token: 0x06000108 RID: 264 RVA: 0x00006052 File Offset: 0x00004252
		[XmlElement(ElementName = "Capability")]
		public List<Capability> CapabilityCollection
		{
			get
			{
				return this.capabilityCollection;
			}
		}

		// Token: 0x0600010A RID: 266 RVA: 0x0000606D File Offset: 0x0000426D
		public void Add(IXPathNavigable capabilitiesXmlElement)
		{
			this.AddElements((XmlElement)capabilitiesXmlElement);
		}

		// Token: 0x0600010B RID: 267 RVA: 0x0000607B File Offset: 0x0000427B
		public void Append(Capability capability)
		{
			this.capabilityCollection.Add(capability);
		}

		// Token: 0x0600010C RID: 268 RVA: 0x0000608C File Offset: 0x0000428C
		private void AddElements(XmlElement capabilitiesXmlElement)
		{
			foreach (object obj in capabilitiesXmlElement.SelectNodes("./WP_Policy:Capability", GlobalVariables.NamespaceManager))
			{
				XmlElement capabilityXmlElement = (XmlElement)obj;
				Capability capability = new Capability(CapabilityOwnerType.StandAlone);
				capability.Add(capabilityXmlElement);
				this.capabilityCollection.Add(capability);
			}
			foreach (object obj2 in capabilitiesXmlElement.SelectNodes("./WP_Policy:WindowsRules", GlobalVariables.NamespaceManager))
			{
				XmlElement capabilityXmlElement2 = (XmlElement)obj2;
				Capability capability2 = new WindowsRules();
				capability2.Add(capabilityXmlElement2);
				this.capabilityCollection.Add(capability2);
			}
		}

		// Token: 0x0600010D RID: 269 RVA: 0x0000616C File Offset: 0x0000436C
		public bool HasChild()
		{
			return this.capabilityCollection != null && this.capabilityCollection.Count > 0;
		}

		// Token: 0x0600010E RID: 270 RVA: 0x00006188 File Offset: 0x00004388
		public void Print()
		{
			ReportingBase instance = ReportingBase.GetInstance();
			instance.XmlElementLine(ConstantStrings.IndentationLevel1, "Capabilities");
			if (this.capabilityCollection != null)
			{
				foreach (Capability capability in this.capabilityCollection)
				{
					instance.DebugLine(string.Empty);
					capability.Print();
				}
			}
		}

		// Token: 0x040000F6 RID: 246
		private List<Capability> capabilityCollection = new List<Capability>();
	}
}
