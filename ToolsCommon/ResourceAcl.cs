using System;
using System.Globalization;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
	// Token: 0x02000040 RID: 64
	public abstract class ResourceAcl
	{
		// Token: 0x17000021 RID: 33
		// (get) Token: 0x0600018D RID: 397 RVA: 0x00008B32 File Offset: 0x00006D32
		// (set) Token: 0x0600018E RID: 398 RVA: 0x00008B5D File Offset: 0x00006D5D
		[XmlAttribute("DACL")]
		public string ExplicitDACL
		{
			get
			{
				if (!this.m_explicitDaclProcessed)
				{
					this.m_explicitDaclProcessed = true;
					if (this.m_nos != null)
					{
						this.m_explicitDacl = this.ComputeExplicitDACL();
					}
				}
				return this.m_explicitDacl;
			}
			set
			{
				this.m_explicitDacl = value;
			}
		}

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x0600018F RID: 399
		// (set) Token: 0x06000190 RID: 400
		[XmlAttribute("SACL")]
		public abstract string MandatoryIntegrityLabel { get; set; }

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x06000191 RID: 401 RVA: 0x00008B66 File Offset: 0x00006D66
		// (set) Token: 0x06000192 RID: 402 RVA: 0x00008BA2 File Offset: 0x00006DA2
		[XmlAttribute("Owner")]
		public string Owner
		{
			get
			{
				if (this.m_owner == null && this.m_nos != null)
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

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x06000193 RID: 403 RVA: 0x00008BAC File Offset: 0x00006DAC
		// (set) Token: 0x06000194 RID: 404 RVA: 0x00008C1F File Offset: 0x00006E1F
		[XmlAttribute]
		public string ElementID
		{
			get
			{
				if (this.m_elementId == null && !string.IsNullOrEmpty(this.m_path))
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

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x06000195 RID: 405 RVA: 0x00008C28 File Offset: 0x00006E28
		// (set) Token: 0x06000196 RID: 406 RVA: 0x00008CE8 File Offset: 0x00006EE8
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

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x06000197 RID: 407 RVA: 0x00008CF1 File Offset: 0x00006EF1
		// (set) Token: 0x06000198 RID: 408 RVA: 0x00008CF9 File Offset: 0x00006EF9
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

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x06000199 RID: 409 RVA: 0x00008D02 File Offset: 0x00006F02
		// (set) Token: 0x0600019A RID: 410 RVA: 0x00008D30 File Offset: 0x00006F30
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
				this.m_isProtected = value.Equals("Yes", StringComparison.OrdinalIgnoreCase);
			}
		}

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x0600019B RID: 411 RVA: 0x00008D4C File Offset: 0x00006F4C
		public bool IsEmpty
		{
			get
			{
				if (!this.m_isEmptyProcessed)
				{
					this.m_isEmptyProcessed = true;
					this.m_isEmpty = (string.IsNullOrEmpty(this.ExplicitDACL) && string.IsNullOrEmpty(this.MandatoryIntegrityLabel) && !this.DACLProtected);
				}
				return this.m_isEmpty;
			}
		}

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x0600019C RID: 412 RVA: 0x00008D9C File Offset: 0x00006F9C
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

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x0600019D RID: 413 RVA: 0x00008DE4 File Offset: 0x00006FE4
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

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x0600019E RID: 414 RVA: 0x00008E0E File Offset: 0x0000700E
		public static ResourceAclComparer Comparer
		{
			get
			{
				return ResourceAcl.ResourceAclComparer;
			}
		}

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x0600019F RID: 415
		public abstract NativeObjectSecurity ObjectSecurity { get; }

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x060001A0 RID: 416
		protected abstract string TypeString { get; }

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x060001A1 RID: 417 RVA: 0x00008E15 File Offset: 0x00007015
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

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x060001A2 RID: 418 RVA: 0x00008E4A File Offset: 0x0000704A
		public bool PreserveInheritance
		{
			get
			{
				return this.m_nos != null && this.m_nos.GetAccessRules(false, true, typeof(NTAccount)).Count > 0;
			}
		}

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x060001A3 RID: 419 RVA: 0x00008E75 File Offset: 0x00007075
		public bool DACLProtected
		{
			get
			{
				return this.m_nos != null && this.m_nos.AreAccessRulesProtected;
			}
		}

		// Token: 0x060001A4 RID: 420
		protected abstract string ComputeExplicitDACL();

		// Token: 0x040000CC RID: 204
		protected string m_explicitDacl;

		// Token: 0x040000CD RID: 205
		protected bool m_explicitDaclProcessed;

		// Token: 0x040000CE RID: 206
		protected string m_macLabel;

		// Token: 0x040000CF RID: 207
		protected bool m_macLablelProcessed;

		// Token: 0x040000D0 RID: 208
		protected string m_owner;

		// Token: 0x040000D1 RID: 209
		protected string m_elementId;

		// Token: 0x040000D2 RID: 210
		protected string m_attributeHash;

		// Token: 0x040000D3 RID: 211
		protected string m_path;

		// Token: 0x040000D4 RID: 212
		protected bool m_isProtected;

		// Token: 0x040000D5 RID: 213
		protected bool m_isEmptyProcessed;

		// Token: 0x040000D6 RID: 214
		protected bool m_isEmpty;

		// Token: 0x040000D7 RID: 215
		private static readonly ResourceAclComparer ResourceAclComparer = new ResourceAclComparer();

		// Token: 0x040000D8 RID: 216
		[CLSCompliant(false)]
		protected NativeObjectSecurity m_nos;

		// Token: 0x040000D9 RID: 217
		[CLSCompliant(false)]
		protected AuthorizationRuleCollection m_accessRules;

		// Token: 0x040000DA RID: 218
		protected string m_fullPath = string.Empty;

		// Token: 0x040000DB RID: 219
		protected static readonly Regex regexExtractMIL = new Regex("(?<MIL>\\(ML[^\\)]*\\))", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		// Token: 0x040000DC RID: 220
		protected static readonly Regex regexStripDacl = new Regex("^D:", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		// Token: 0x040000DD RID: 221
		protected static readonly Regex regexStripDriveLetter = new Regex("^[A-Z]:", RegexOptions.IgnoreCase | RegexOptions.Compiled);
	}
}
