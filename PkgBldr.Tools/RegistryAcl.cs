using System;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Microsoft.CompPlat.PkgBldr.Tools
{
	// Token: 0x02000025 RID: 37
	public class RegistryAcl : ResourceAcl
	{
		// Token: 0x0600016A RID: 362 RVA: 0x000068FC File Offset: 0x00004AFC
		public RegistryAcl()
		{
		}

		// Token: 0x0600016B RID: 363 RVA: 0x00006D98 File Offset: 0x00004F98
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

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x0600016C RID: 364 RVA: 0x00006DE4 File Offset: 0x00004FE4
		// (set) Token: 0x0600016D RID: 365 RVA: 0x000069BF File Offset: 0x00004BBF
		[XmlAttribute("SACL")]
		public override string MandatoryIntegrityLabel
		{
			get
			{
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
				return this.m_macLabel;
			}
			set
			{
				this.m_macLabel = value;
			}
		}

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x0600016E RID: 366 RVA: 0x00006E5C File Offset: 0x0000505C
		public override NativeObjectSecurity ObjectSecurity
		{
			get
			{
				RegistrySecurity registrySecurity = null;
				if (this.m_nos != null)
				{
					registrySecurity = new RegistrySecurity();
					registrySecurity.SetSecurityDescriptorBinaryForm(this.m_nos.GetSecurityDescriptorBinaryForm());
				}
				return registrySecurity;
			}
		}

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x0600016F RID: 367 RVA: 0x00006E8B File Offset: 0x0000508B
		protected override string TypeString
		{
			get
			{
				return "RegKey";
			}
		}

		// Token: 0x06000170 RID: 368 RVA: 0x00006E94 File Offset: 0x00005094
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

		// Token: 0x0400006C RID: 108
		private ORRegistryKey m_key;
	}
}
