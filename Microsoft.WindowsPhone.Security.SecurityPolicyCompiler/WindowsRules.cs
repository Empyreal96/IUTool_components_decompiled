using System;
using System.Text;
using System.Xml;

namespace Microsoft.WindowsPhone.Security.SecurityPolicyCompiler
{
	// Token: 0x02000045 RID: 69
	public class WindowsRules : Capability
	{
		// Token: 0x06000239 RID: 569 RVA: 0x0000A641 File Offset: 0x00008841
		public WindowsRules()
		{
			base.OwnerType = CapabilityOwnerType.WindowsRules;
		}

		// Token: 0x0600023A RID: 570 RVA: 0x0000A650 File Offset: 0x00008850
		protected override void AddAttributes(XmlElement windowsRulesXmlElement)
		{
			base.Id = "ID_CAP_WINRULES_" + windowsRulesXmlElement.GetAttribute("Id");
			base.FriendlyName = windowsRulesXmlElement.GetAttribute("FriendlyName");
			base.SvcCapSID = windowsRulesXmlElement.GetAttribute("SID");
			base.Visibility = "Internal";
		}

		// Token: 0x0600023B RID: 571 RVA: 0x0000A6A8 File Offset: 0x000088A8
		protected override void AddElements(XmlElement windowsRulesXmlElement)
		{
			XmlNodeList xmlNodeList = windowsRulesXmlElement.SelectNodes(".", GlobalVariables.NamespaceManager);
			if (xmlNodeList.Count > 0)
			{
				if (base.CapabilityRules == null)
				{
					base.CapabilityRules = new CapabilityRules(base.OwnerType);
				}
				foreach (object obj in xmlNodeList)
				{
					XmlElement capabilityRulesXmlElement = (XmlElement)obj;
					base.CapabilityRules.Add(capabilityRulesXmlElement, null, base.SvcCapSID);
				}
			}
		}

		// Token: 0x0600023C RID: 572 RVA: 0x00005375 File Offset: 0x00003575
		protected override void CompileAttributes()
		{
			base.ElementId = HashCalculator.CalculateSha256Hash(base.Id, true);
		}

		// Token: 0x0600023D RID: 573 RVA: 0x0000A73C File Offset: 0x0000893C
		protected override void CalculateAttributeHash()
		{
			StringBuilder stringBuilder = new StringBuilder(base.Id);
			stringBuilder.Append(base.Visibility);
			stringBuilder.Append(base.SvcCapSID);
			if (base.CapabilityRules != null)
			{
				stringBuilder.Append(base.CapabilityRules.GetAllAttributesString());
			}
			base.AttributeHash = HashCalculator.CalculateSha256Hash(stringBuilder.ToString(), true);
		}
	}
}
