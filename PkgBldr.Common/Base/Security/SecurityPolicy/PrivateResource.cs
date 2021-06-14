using System;
using System.Globalization;

namespace Microsoft.CompPlat.PkgBldr.Base.Security.SecurityPolicy
{
	// Token: 0x02000051 RID: 81
	public class PrivateResource : AccessControlPolicy
	{
		// Token: 0x06000186 RID: 390 RVA: 0x0000A16D File Offset: 0x0000836D
		public PrivateResource(ResourceType Type, string ResourceClaimer, PrivateResourceClaimerType ResourceClaimerType, bool ReadOnly) : base(Type)
		{
			this.Init(Type, ResourceClaimer, ResourceClaimerType, ReadOnly);
		}

		// Token: 0x06000187 RID: 391 RVA: 0x0000A184 File Offset: 0x00008384
		public PrivateResource(ResourceType Type, string ResourceClaimer, PrivateResourceClaimerType ResourceClaimerType, bool ReadOnly, bool ProtectToUser) : base(Type)
		{
			this.Init(Type, ResourceClaimer, ResourceClaimerType, ReadOnly);
			if (ProtectToUser)
			{
				switch (Type)
				{
				case ResourceType.File:
				case ResourceType.Directory:
				case ResourceType.Registry:
					this.m_ResourceClaimerTrustee = null;
					return;
				case ResourceType.TransientObject:
					this.m_ResourceClaimerTrustee = "%s";
					return;
				case ResourceType.ComLaunch:
				case ResourceType.ComAccess:
				case ResourceType.WinRt:
					this.m_ResourceClaimerTrustee = "PS";
					return;
				}
				throw new PkgGenException("Invalid resource type for user protection");
			}
			if (ResourceClaimerType == PrivateResourceClaimerType.Task)
			{
				this.m_ResourceClaimerTrustee = "IU";
				return;
			}
			this.m_ResourceClaimerTrustee = SidBuilder.BuildServiceSidString(ResourceClaimer);
		}

		// Token: 0x06000188 RID: 392 RVA: 0x0000A218 File Offset: 0x00008418
		private void Init(ResourceType Type, string ResourceClaimer, PrivateResourceClaimerType ResourceClaimerType, bool ReadOnly)
		{
			switch (Type)
			{
			case ResourceType.ServiceAccess:
				this.m_Access = (ReadOnly ? "GR" : "CCLCSWRPLO");
				break;
			case ResourceType.ComLaunch:
				this.m_Access = "CCDCSW";
				break;
			case ResourceType.ComAccess:
				this.m_Access = "CCDC";
				break;
			default:
				this.m_Access = (ReadOnly ? "GR" : "0x111FFFFF");
				break;
			}
			if (ResourceClaimerType == PrivateResourceClaimerType.Task)
			{
				this.m_ResourceClaimerAdditionalTrustee = SidBuilder.BuildTaskSidString(ResourceClaimer);
				return;
			}
			this.m_ResourceClaimerAdditionalTrustee = null;
		}

		// Token: 0x06000189 RID: 393 RVA: 0x0000A29C File Offset: 0x0000849C
		public override string GetUniqueAccessControlEntries()
		{
			string result = null;
			if (this.m_ResourceClaimerTrustee != null)
			{
				if (this.m_ResourceClaimerAdditionalTrustee != null)
				{
					result = string.Format("(A;{0};{1};;;{2})(A;{0};{1};;;{3})", new object[]
					{
						this.m_InheritanceFlags,
						this.m_Access,
						this.m_ResourceClaimerTrustee,
						this.m_ResourceClaimerAdditionalTrustee,
						CultureInfo.InvariantCulture
					});
				}
				else
				{
					result = string.Format("(A;{0};{1};;;{2})", new object[]
					{
						this.m_InheritanceFlags,
						this.m_Access,
						this.m_ResourceClaimerTrustee,
						CultureInfo.InvariantCulture
					});
				}
			}
			else if (this.m_ResourceClaimerAdditionalTrustee != null)
			{
				result = string.Format("(A;{0};{1};;;{2})", new object[]
				{
					this.m_InheritanceFlags,
					this.m_Access,
					this.m_ResourceClaimerAdditionalTrustee,
					CultureInfo.InvariantCulture
				});
			}
			return result;
		}

		// Token: 0x0600018A RID: 394 RVA: 0x0000A36D File Offset: 0x0000856D
		public override string GetDefaultDacl()
		{
			return "D:P" + this.m_DefaultDacl;
		}

		// Token: 0x04000101 RID: 257
		private string m_ResourceClaimerTrustee;

		// Token: 0x04000102 RID: 258
		private string m_ResourceClaimerAdditionalTrustee;
	}
}
