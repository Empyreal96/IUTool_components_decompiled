using System;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
	// Token: 0x02000044 RID: 68
	public class RegistryAcl : ResourceAcl
	{
		// Token: 0x060001BC RID: 444 RVA: 0x00008F49 File Offset: 0x00007149
		public RegistryAcl()
		{
		}

		// Token: 0x060001BD RID: 445 RVA: 0x0000942C File Offset: 0x0000762C
		public RegistryAcl(ORRegistryKey key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			this.m_key = key;
			this.m_nos = key.RegistrySecurity;
			this.m_path = key.FullName;
			this.m_fullPath = key.FullName;
		}

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x060001BE RID: 446 RVA: 0x00009478 File Offset: 0x00007678
		// (set) Token: 0x060001BF RID: 447 RVA: 0x00009012 File Offset: 0x00007212
		[XmlAttribute("SACL")]
		public override string MandatoryIntegrityLabel
		{
			get
			{
				if (!this.m_macLablelProcessed)
				{
					this.m_macLablelProcessed = true;
					if (this.m_nos != null)
					{
						this.m_macLabel = null;
						string text = SecurityUtils.ConvertSDToStringSD(this.m_nos.GetSecurityDescriptorBinaryForm(), (SecurityInformationFlags)24U);
						if (!string.IsNullOrEmpty(text))
						{
							Match match = ResourceAcl.regexExtractMIL.Match(text);
							if (match.Success)
							{
								Group group = match.Groups["MIL"];
								if (group != null)
								{
									this.m_macLabel = SddlNormalizer.FixAceSddl(group.Value);
								}
							}
						}
					}
				}
				return this.m_macLabel;
			}
			set
			{
				this.m_macLabel = value;
			}
		}

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x060001C0 RID: 448 RVA: 0x00009500 File Offset: 0x00007700
		public override NativeObjectSecurity ObjectSecurity
		{
			get
			{
				if (this.m_objectSecurity == null && this.m_nos != null)
				{
					RegistrySecurity registrySecurity = new RegistrySecurity();
					registrySecurity.SetSecurityDescriptorBinaryForm(this.m_nos.GetSecurityDescriptorBinaryForm());
					this.m_objectSecurity = registrySecurity;
				}
				return this.m_objectSecurity;
			}
		}

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x060001C1 RID: 449 RVA: 0x00009543 File Offset: 0x00007743
		protected override string TypeString
		{
			get
			{
				return "RegKey";
			}
		}

		// Token: 0x060001C2 RID: 450 RVA: 0x0000954C File Offset: 0x0000774C
		protected override string ComputeExplicitDACL()
		{
			RegistrySecurity registrySecurity = this.m_key.RegistrySecurity;
			AuthorizationRuleCollection accessRules = registrySecurity.GetAccessRules(true, false, typeof(NTAccount));
			int num = accessRules.Count;
			foreach (object obj in accessRules)
			{
				RegistryAccessRule registryAccessRule = (RegistryAccessRule)obj;
				if (registryAccessRule.IsInherited)
				{
					registrySecurity.RemoveAccessRule(registryAccessRule);
					num--;
				}
			}
			if (base.DACLProtected && registrySecurity.AreAccessRulesCanonical)
			{
				registrySecurity.SetAccessRuleProtection(true, base.PreserveInheritance);
			}
			string text = null;
			if (base.DACLProtected || num > 0)
			{
				text = registrySecurity.GetSecurityDescriptorSddlForm(AccessControlSections.Access);
				if (!string.IsNullOrEmpty(text))
				{
					text = ResourceAcl.regexStripDacl.Replace(text, string.Empty);
				}
			}
			return SddlNormalizer.FixAceSddl(text);
		}

		// Token: 0x040000E3 RID: 227
		private ORRegistryKey m_key;

		// Token: 0x040000E4 RID: 228
		private NativeObjectSecurity m_objectSecurity;
	}
}
