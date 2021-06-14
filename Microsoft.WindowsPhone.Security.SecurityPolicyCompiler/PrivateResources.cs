using System;
using System.Xml;

namespace Microsoft.WindowsPhone.Security.SecurityPolicyCompiler
{
	// Token: 0x0200001B RID: 27
	public class PrivateResources : Capability
	{
		// Token: 0x060000C5 RID: 197 RVA: 0x000052E0 File Offset: 0x000034E0
		public void SetPrivateResourcesOwner(string name, CapabilityOwnerType ownerTypeValue, string componentSid)
		{
			base.OwnerType = ownerTypeValue;
			base.Id = "ID_CAP_PRIV_" + name;
			base.FriendlyName = name + " private capability";
			base.Visibility = "Private";
			CapabilityOwnerType ownerType = base.OwnerType;
			if (ownerType == CapabilityOwnerType.Application)
			{
				base.AppCapSID = componentSid;
				return;
			}
			if (ownerType != CapabilityOwnerType.Service)
			{
				return;
			}
			base.SvcCapSID = componentSid;
		}

		// Token: 0x060000C6 RID: 198 RVA: 0x00005340 File Offset: 0x00003540
		protected override void AddAttributes(XmlElement privateResourcesXmlElement)
		{
		}

		// Token: 0x060000C7 RID: 199 RVA: 0x00005342 File Offset: 0x00003542
		protected override void AddElements(XmlElement privateResourcesXmlElement)
		{
			if (base.CapabilityRules == null)
			{
				base.CapabilityRules = new CapabilityRules(base.OwnerType);
			}
			base.CapabilityRules.Add(privateResourcesXmlElement, base.AppCapSID, base.SvcCapSID);
		}

		// Token: 0x060000C8 RID: 200 RVA: 0x00005375 File Offset: 0x00003575
		protected override void CompileAttributes()
		{
			base.ElementId = HashCalculator.CalculateSha256Hash(base.Id, true);
		}
	}
}
