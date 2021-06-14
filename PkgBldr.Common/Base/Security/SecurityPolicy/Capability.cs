using System;
using System.Globalization;

namespace Microsoft.CompPlat.PkgBldr.Base.Security.SecurityPolicy
{
	// Token: 0x02000050 RID: 80
	public class Capability : AccessControlPolicy
	{
		// Token: 0x06000182 RID: 386 RVA: 0x00009F76 File Offset: 0x00008176
		public Capability(string CapabilityId, ResourceType Type, string Access, bool AdminOnMultiSession) : base(Type)
		{
			this.Init(CapabilityId, Type, Access, AdminOnMultiSession);
		}

		// Token: 0x06000183 RID: 387 RVA: 0x00009F8C File Offset: 0x0000818C
		public Capability(string CapabilityId, ResourceType Type, string Access, bool AdminOnMultiSession, bool ProtectToUser) : base(Type)
		{
			this.Init(CapabilityId, Type, Access, AdminOnMultiSession);
			if (ProtectToUser)
			{
				switch (Type)
				{
				case ResourceType.File:
				case ResourceType.Directory:
				case ResourceType.Registry:
					this.m_ApplicationCapabilityGroupTrustee = null;
					return;
				case ResourceType.TransientObject:
					this.m_ApplicationCapabilityGroupTrustee = "%s";
					return;
				case ResourceType.ComLaunch:
				case ResourceType.ComAccess:
				case ResourceType.WinRt:
					this.m_ApplicationCapabilityGroupTrustee = "PS";
					return;
				}
				throw new PkgGenException("Invalid resource type for user protection");
			}
			this.m_ApplicationCapabilityGroupTrustee = "IU";
		}

		// Token: 0x06000184 RID: 388 RVA: 0x0000A00D File Offset: 0x0000820D
		private void Init(string CapabilityId, ResourceType Type, string Access, bool AdminOnMultiSession)
		{
			this.m_CapabilityId = CapabilityId;
			this.m_AdminOnMultiSession = AdminOnMultiSession;
			this.m_Access = Access;
		}

		// Token: 0x06000185 RID: 389 RVA: 0x0000A028 File Offset: 0x00008228
		public override string GetUniqueAccessControlEntries()
		{
			string text;
			string text2;
			if (this.m_CapabilityId == "everyone")
			{
				text = "S-1-15-2-1";
				text2 = "AU";
			}
			else
			{
				text = SidBuilder.BuildApplicationCapabilitySidString(this.m_CapabilityId);
				text2 = SidBuilder.BuildServiceCapabilitySidString(this.m_CapabilityId);
			}
			string result;
			if (this.m_AdminOnMultiSession)
			{
				if (this.m_ApplicationCapabilityGroupTrustee != null)
				{
					result = string.Format("(XA;{0};{1};;;{2};(!(WIN://ISMULTISESSIONSKU)))(XA;{0};{1};;;{3};(!(WIN://ISMULTISESSIONSKU)))(XA;{0};{1};;;{4};(!(WIN://ISMULTISESSIONSKU)))", new object[]
					{
						this.m_InheritanceFlags,
						this.m_Access,
						text,
						this.m_ApplicationCapabilityGroupTrustee,
						text2,
						CultureInfo.InvariantCulture
					});
				}
				else
				{
					result = string.Format("(XA;{0};{1};;;{2};(!(WIN://ISMULTISESSIONSKU)))(XA;{0};{1};;;{3};(!(WIN://ISMULTISESSIONSKU)))", new object[]
					{
						this.m_InheritanceFlags,
						this.m_Access,
						text,
						text2,
						CultureInfo.InvariantCulture
					});
				}
			}
			else if (this.m_ApplicationCapabilityGroupTrustee != null)
			{
				result = string.Format("(A;{0};{1};;;{2})(A;{0};{1};;;{3})(A;{0};{1};;;{4})", new object[]
				{
					this.m_InheritanceFlags,
					this.m_Access,
					text,
					this.m_ApplicationCapabilityGroupTrustee,
					text2,
					CultureInfo.InvariantCulture
				});
			}
			else
			{
				result = string.Format("(A;{0};{1};;;{2})(A;{0};{1};;;{3})", new object[]
				{
					this.m_InheritanceFlags,
					this.m_Access,
					text,
					text2,
					CultureInfo.InvariantCulture
				});
			}
			return result;
		}

		// Token: 0x040000FE RID: 254
		private string m_CapabilityId;

		// Token: 0x040000FF RID: 255
		private string m_ApplicationCapabilityGroupTrustee;

		// Token: 0x04000100 RID: 256
		private bool m_AdminOnMultiSession;
	}
}
