using System;
using System.Globalization;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Microsoft.CompPlat.PkgBldr.Tools
{
	// Token: 0x02000021 RID: 33
	public abstract class ResourceAcl
	{
		// Token: 0x1700000C RID: 12
		// (get) Token: 0x0600013B RID: 315 RVA: 0x00006526 File Offset: 0x00004726
		// (set) Token: 0x0600013C RID: 316 RVA: 0x00006542 File Offset: 0x00004742
		[XmlAttribute("DACL")]
		public string ExplicitDACL
		{
			get
			{
				if (this.m_nos != null)
				{
					this.m_explicitDacl = this.ComputeExplicitDACL();
				}
				return this.m_explicitDacl;
			}
			set
			{
				this.m_explicitDacl = value;
			}
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x0600013D RID: 317
		// (set) Token: 0x0600013E RID: 318
		[XmlAttribute("SACL")]
		public abstract string MandatoryIntegrityLabel { get; set; }

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x0600013F RID: 319 RVA: 0x0000654B File Offset: 0x0000474B
		// (set) Token: 0x06000140 RID: 320 RVA: 0x0000657F File Offset: 0x0000477F
		[XmlAttribute("Owner")]
		public string Owner
		{
			get
			{
				if (this.m_nos != null)
				{
					this.m_owner = this.m_nos.GetSecurityDescriptorSddlForm(AccessControlSections.Owner | AccessControlSections.Group);
					this.m_owner = SddlNormalizer.FixOwnerSddl(this.m_owner);
				}
				return this.m_owner;
			}
			set
			{
				this.m_owner = value;
			}
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000141 RID: 321 RVA: 0x00006588 File Offset: 0x00004788
		// (set) Token: 0x06000142 RID: 322 RVA: 0x000065F3 File Offset: 0x000047F3
		[XmlAttribute]
		public string ElementID
		{
			get
			{
				if (!string.IsNullOrEmpty(this.m_path))
				{
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.Append(this.TypeString);
					stringBuilder.Append(this.m_path.ToUpper(new CultureInfo("en-US", false)));
					this.m_elementId = CommonUtils.GetSha256Hash(Encoding.Unicode.GetBytes(stringBuilder.ToString()));
				}
				return this.m_elementId;
			}
			set
			{
				this.m_elementId = value;
			}
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000143 RID: 323 RVA: 0x000065FC File Offset: 0x000047FC
		// (set) Token: 0x06000144 RID: 324 RVA: 0x000066BC File Offset: 0x000048BC
		[XmlAttribute]
		public virtual string AttributeHash
		{
			get
			{
				if (string.IsNullOrEmpty(this.m_attributeHash))
				{
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.Append(this.TypeString);
					stringBuilder.Append(this.m_path.ToUpper(new CultureInfo("en-US", false)));
					stringBuilder.Append(this.Protected);
					string owner = this.Owner;
					if (!string.IsNullOrEmpty(owner))
					{
						stringBuilder.Append(owner);
					}
					string explicitDACL = this.ExplicitDACL;
					if (!string.IsNullOrEmpty(explicitDACL))
					{
						stringBuilder.Append(explicitDACL);
					}
					string mandatoryIntegrityLabel = this.MandatoryIntegrityLabel;
					if (!string.IsNullOrEmpty(mandatoryIntegrityLabel))
					{
						stringBuilder.Append(mandatoryIntegrityLabel);
					}
					this.m_attributeHash = CommonUtils.GetSha256Hash(Encoding.Unicode.GetBytes(stringBuilder.ToString()));
				}
				return this.m_attributeHash;
			}
			set
			{
				this.m_attributeHash = value;
			}
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000145 RID: 325 RVA: 0x000066C5 File Offset: 0x000048C5
		// (set) Token: 0x06000146 RID: 326 RVA: 0x000066CD File Offset: 0x000048CD
		[XmlAttribute]
		public string Path
		{
			get
			{
				return this.m_path;
			}
			set
			{
				this.m_path = value;
			}
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000147 RID: 327 RVA: 0x000066D6 File Offset: 0x000048D6
		// (set) Token: 0x06000148 RID: 328 RVA: 0x00006704 File Offset: 0x00004904
		[XmlIgnore]
		public string Protected
		{
			get
			{
				if (this.m_nos != null)
				{
					this.m_isProtected = this.m_nos.AreAccessRulesProtected;
				}
				if (!this.m_isProtected)
				{
					return "No";
				}
				return "Yes";
			}
			set
			{
				this.m_isProtected = (value != null && value.Equals("Yes", StringComparison.OrdinalIgnoreCase));
			}
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000149 RID: 329 RVA: 0x00006721 File Offset: 0x00004921
		public bool IsEmpty
		{
			get
			{
				return string.IsNullOrEmpty(this.ExplicitDACL) && string.IsNullOrEmpty(this.MandatoryIntegrityLabel) && !this.DACLProtected;
			}
		}

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x0600014A RID: 330 RVA: 0x00006748 File Offset: 0x00004948
		public string DACL
		{
			get
			{
				string text = string.Empty;
				if (this.m_nos != null)
				{
					text = this.m_nos.GetSecurityDescriptorSddlForm(AccessControlSections.Access);
					if (!string.IsNullOrEmpty(text))
					{
						text = ResourceAcl.regexStripDacl.Replace(text, string.Empty);
					}
				}
				return SddlNormalizer.FixAceSddl(text);
			}
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x0600014B RID: 331 RVA: 0x00006790 File Offset: 0x00004990
		public string FullACL
		{
			get
			{
				string result = string.Empty;
				if (this.m_nos != null)
				{
					result = this.m_nos.GetSecurityDescriptorSddlForm(AccessControlSections.All);
				}
				return result;
			}
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x0600014C RID: 332 RVA: 0x000067BA File Offset: 0x000049BA
		public static ResourceAclComparer Comparer
		{
			get
			{
				return ResourceAcl.ResourceAclComparer;
			}
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x0600014D RID: 333
		public abstract NativeObjectSecurity ObjectSecurity { get; }

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x0600014E RID: 334
		protected abstract string TypeString { get; }

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x0600014F RID: 335 RVA: 0x000067C1 File Offset: 0x000049C1
		protected AuthorizationRuleCollection AccessRules
		{
			get
			{
				if (this.m_accessRules == null && this.m_nos != null)
				{
					this.m_accessRules = this.m_nos.GetAccessRules(true, false, typeof(NTAccount));
				}
				return this.m_accessRules;
			}
		}

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x06000150 RID: 336 RVA: 0x000067F6 File Offset: 0x000049F6
		public bool PreserveInheritance
		{
			get
			{
				return this.m_nos != null && this.m_nos.GetAccessRules(false, true, typeof(NTAccount)).Count > 0;
			}
		}

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x06000151 RID: 337 RVA: 0x00006821 File Offset: 0x00004A21
		public bool DACLProtected
		{
			get
			{
				return this.m_nos != null && this.m_nos.AreAccessRulesProtected;
			}
		}

		// Token: 0x06000152 RID: 338
		protected abstract string ComputeExplicitDACL();

		// Token: 0x0400005B RID: 91
		protected string m_explicitDacl;

		// Token: 0x0400005C RID: 92
		protected string m_macLabel;

		// Token: 0x0400005D RID: 93
		protected string m_owner;

		// Token: 0x0400005E RID: 94
		protected string m_elementId;

		// Token: 0x0400005F RID: 95
		protected string m_attributeHash;

		// Token: 0x04000060 RID: 96
		protected string m_path;

		// Token: 0x04000061 RID: 97
		protected bool m_isProtected;

		// Token: 0x04000062 RID: 98
		private static readonly ResourceAclComparer ResourceAclComparer = new ResourceAclComparer();

		// Token: 0x04000063 RID: 99
		protected NativeObjectSecurity m_nos;

		// Token: 0x04000064 RID: 100
		protected AuthorizationRuleCollection m_accessRules;

		// Token: 0x04000065 RID: 101
		protected string m_fullPath = string.Empty;

		// Token: 0x04000066 RID: 102
		protected static readonly Regex regexExtractMIL = new Regex("(?<MIL>\\(ML[^\\)]*\\))", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		// Token: 0x04000067 RID: 103
		protected static readonly Regex regexStripDacl = new Regex("^D:", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		// Token: 0x04000068 RID: 104
		protected static readonly Regex regexStripDriveLetter = new Regex("^[A-Z]:", RegexOptions.IgnoreCase | RegexOptions.Compiled);
	}
}
