using System;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Microsoft.CompPlat.PkgBldr.Tools
{
	// Token: 0x02000027 RID: 39
	public class RegistryStoredAcl : ResourceAcl
	{
		// Token: 0x06000175 RID: 373 RVA: 0x00006F95 File Offset: 0x00005195
		public RegistryStoredAcl()
		{
		}

		// Token: 0x06000176 RID: 374 RVA: 0x00006FA8 File Offset: 0x000051A8
		public RegistryStoredAcl(string typeName, string path, byte[] rawSecurityDescriptor)
		{
			if (rawSecurityDescriptor == null || string.IsNullOrEmpty(path) || string.IsNullOrEmpty(typeName))
			{
				throw new ArgumentException("SDRegValue is null");
			}
			RegistrySecurity registrySecurity = new RegistrySecurity();
			registrySecurity.SetSecurityDescriptorBinaryForm(rawSecurityDescriptor);
			this.m_rawsd = rawSecurityDescriptor;
			this.m_nos = registrySecurity;
			this.m_path = path;
			this.m_fullPath = path;
			this.m_typeName = typeName;
		}

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x06000177 RID: 375 RVA: 0x00007014 File Offset: 0x00005214
		// (set) Token: 0x06000178 RID: 376 RVA: 0x0000701C File Offset: 0x0000521C
		[XmlAttribute("Type")]
		public string SDRegValueTypeName
		{
			get
			{
				return this.m_typeName;
			}
			set
			{
				this.m_typeName = value;
			}
		}

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x06000179 RID: 377 RVA: 0x00007028 File Offset: 0x00005228
		// (set) Token: 0x0600017A RID: 378 RVA: 0x000069BF File Offset: 0x00004BBF
		[XmlAttribute("SACL")]
		public override string MandatoryIntegrityLabel
		{
			get
			{
				this.m_macLabel = null;
				string text = SecurityUtils.ConvertSDToStringSD(this.m_rawsd, SecurityInformationFlags.MANDATORY_ACCESS_LABEL);
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
				return this.m_macLabel;
			}
			set
			{
				this.m_macLabel = value;
			}
		}

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x0600017B RID: 379 RVA: 0x00007094 File Offset: 0x00005294
		// (set) Token: 0x0600017C RID: 380 RVA: 0x000066BC File Offset: 0x000048BC
		[XmlAttribute]
		public override string AttributeHash
		{
			get
			{
				if (string.IsNullOrEmpty(this.m_attributeHash))
				{
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.Append(this.TypeString);
					stringBuilder.Append(this.m_path);
					stringBuilder.Append(base.Protected);
					string owner = base.Owner;
					if (!string.IsNullOrEmpty(owner))
					{
						stringBuilder.Append(owner);
					}
					string explicitDACL = base.ExplicitDACL;
					if (!string.IsNullOrEmpty(explicitDACL))
					{
						stringBuilder.Append(explicitDACL);
					}
					string mandatoryIntegrityLabel = this.MandatoryIntegrityLabel;
					if (!string.IsNullOrEmpty(mandatoryIntegrityLabel))
					{
						stringBuilder.Append(mandatoryIntegrityLabel);
					}
					stringBuilder.Append(this.SDRegValueTypeName);
					this.m_attributeHash = CommonUtils.GetSha256Hash(Encoding.Unicode.GetBytes(stringBuilder.ToString()));
				}
				return this.m_attributeHash;
			}
			set
			{
				this.m_attributeHash = value;
			}
		}

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x0600017D RID: 381 RVA: 0x00007151 File Offset: 0x00005351
		public override NativeObjectSecurity ObjectSecurity
		{
			get
			{
				RegistrySecurity registrySecurity = new RegistrySecurity();
				registrySecurity.SetSecurityDescriptorBinaryForm(this.m_rawsd);
				return registrySecurity;
			}
		}

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x0600017E RID: 382 RVA: 0x00007164 File Offset: 0x00005364
		protected override string TypeString
		{
			get
			{
				return "SDRegValue";
			}
		}

		// Token: 0x0600017F RID: 383 RVA: 0x0000716C File Offset: 0x0000536C
		protected override string ComputeExplicitDACL()
		{
			RegistrySecurity registrySecurity = new RegistrySecurity();
			registrySecurity.SetSecurityDescriptorBinaryForm(this.m_rawsd);
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
					text = SddlNormalizer.FixAceSddl(text);
				}
			}
			return text;
		}

		// Token: 0x0400006D RID: 109
		protected string m_typeName = "Unknown";

		// Token: 0x0400006E RID: 110
		protected byte[] m_rawsd;
	}
}
