using System;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
	// Token: 0x02000046 RID: 70
	public class RegistryStoredAcl : ResourceAcl
	{
		// Token: 0x060001C7 RID: 455 RVA: 0x0000964D File Offset: 0x0000784D
		public RegistryStoredAcl()
		{
		}

		// Token: 0x060001C8 RID: 456 RVA: 0x00009660 File Offset: 0x00007860
		public RegistryStoredAcl(string typeName, string path, byte[] rawSecurityDescriptor)
		{
			if (rawSecurityDescriptor == null)
			{
				throw new ArgumentNullException("rawSecurityDescriptor", "rawSecurityDescriptor Cannot be null.");
			}
			if (string.IsNullOrEmpty(path))
			{
				throw new ArgumentNullException("path", "path Cannot be null.");
			}
			if (string.IsNullOrEmpty(typeName))
			{
				throw new ArgumentNullException("typeName", "typeName Cannot be null.");
			}
			RegistrySecurity registrySecurity = new RegistrySecurity();
			registrySecurity.SetSecurityDescriptorBinaryForm(rawSecurityDescriptor);
			this.m_rawsd = rawSecurityDescriptor;
			this.m_nos = registrySecurity;
			this.m_path = path;
			this.m_fullPath = path;
			this.m_typeName = typeName;
		}

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x060001C9 RID: 457 RVA: 0x000096F1 File Offset: 0x000078F1
		// (set) Token: 0x060001CA RID: 458 RVA: 0x000096F9 File Offset: 0x000078F9
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

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x060001CB RID: 459 RVA: 0x00009704 File Offset: 0x00007904
		// (set) Token: 0x060001CC RID: 460 RVA: 0x00009012 File Offset: 0x00007212
		[XmlAttribute("SACL")]
		public override string MandatoryIntegrityLabel
		{
			get
			{
				if (!this.m_macLablelProcessed)
				{
					this.m_macLablelProcessed = true;
					if (this.m_macLabel == null)
					{
						string text = SecurityUtils.ConvertSDToStringSD(this.m_rawsd, SecurityInformationFlags.MANDATORY_ACCESS_LABEL);
						if (this.SDRegValueTypeName == "COM" && text == "S:")
						{
							this.m_macLabel = string.Empty;
						}
						else if (!string.IsNullOrEmpty(text))
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

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x060001CD RID: 461 RVA: 0x000097AC File Offset: 0x000079AC
		// (set) Token: 0x060001CE RID: 462 RVA: 0x00008CE8 File Offset: 0x00006EE8
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

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x060001CF RID: 463 RVA: 0x0000986C File Offset: 0x00007A6C
		public override NativeObjectSecurity ObjectSecurity
		{
			get
			{
				if (this.m_objectSecurity == null)
				{
					RegistrySecurity registrySecurity = new RegistrySecurity();
					registrySecurity.SetSecurityDescriptorBinaryForm(this.m_rawsd);
					this.m_objectSecurity = registrySecurity;
				}
				return this.m_objectSecurity;
			}
		}

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x060001D0 RID: 464 RVA: 0x000098A0 File Offset: 0x00007AA0
		protected override string TypeString
		{
			get
			{
				return "SDRegValue";
			}
		}

		// Token: 0x060001D1 RID: 465 RVA: 0x000098A8 File Offset: 0x00007AA8
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

		// Token: 0x040000E5 RID: 229
		protected string m_typeName = "Unknown";

		// Token: 0x040000E6 RID: 230
		protected byte[] m_rawsd;

		// Token: 0x040000E7 RID: 231
		private NativeObjectSecurity m_objectSecurity;
	}
}
