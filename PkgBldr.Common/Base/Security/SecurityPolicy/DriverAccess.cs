using System;
using System.Globalization;

namespace Microsoft.CompPlat.PkgBldr.Base.Security.SecurityPolicy
{
	// Token: 0x02000052 RID: 82
	public class DriverAccess : AccessControlPolicy
	{
		// Token: 0x0600018B RID: 395 RVA: 0x0000A37F File Offset: 0x0000857F
		public DriverAccess(DriverAccessType AccessType, string Name, string Access) : base(ResourceType.Driver)
		{
			this.m_AccessType = AccessType;
			this.m_Name = Name;
			this.m_Access = Access;
		}

		// Token: 0x0600018C RID: 396 RVA: 0x0000A3A0 File Offset: 0x000085A0
		public override string GetUniqueAccessControlEntries()
		{
			string result;
			switch (this.m_AccessType)
			{
			case DriverAccessType.Capability:
				result = new Capability(this.m_Name, ResourceType.Driver, this.m_Access, false).GetUniqueAccessControlEntries();
				break;
			case DriverAccessType.Application:
			{
				string text = SidBuilder.BuildApplicationSidString(this.m_Name);
				string text2 = "IU";
				result = string.Format("(A;;{0};;;{1})(A;;{0};;;{2})", new object[]
				{
					this.m_Access,
					text,
					text2,
					CultureInfo.InvariantCulture
				});
				break;
			}
			case DriverAccessType.LegacyApplication:
			{
				string text = SidBuilder.BuildLegacyApplicationSidString(this.m_Name);
				string text2 = "IU";
				result = string.Format("(A;;{0};;;{1})(A;;{0};;;{2})", new object[]
				{
					this.m_Access,
					text,
					text2,
					CultureInfo.InvariantCulture
				});
				break;
			}
			case DriverAccessType.Service:
			{
				string text = SidBuilder.BuildServiceSidString(this.m_Name);
				result = string.Format("(A;;{0};;;{1})", this.m_Access, text, CultureInfo.InvariantCulture);
				break;
			}
			default:
				throw new PkgGenException("Invalid driver access type");
			}
			return result;
		}

		// Token: 0x04000103 RID: 259
		private DriverAccessType m_AccessType;

		// Token: 0x04000104 RID: 260
		private string m_Name;
	}
}
