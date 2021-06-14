using System;

namespace Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
	// Token: 0x02000020 RID: 32
	public class PrivilegeNames
	{
		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000117 RID: 279 RVA: 0x000073F7 File Offset: 0x000055F7
		public static TokenPrivilege BackupPrivilege
		{
			get
			{
				return new TokenPrivilege("SeBackupPrivilege");
			}
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000118 RID: 280 RVA: 0x00007403 File Offset: 0x00005603
		public static TokenPrivilege SecurityPrivilege
		{
			get
			{
				return new TokenPrivilege("SeSecurityPrivilege");
			}
		}

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000119 RID: 281 RVA: 0x0000740F File Offset: 0x0000560F
		public static TokenPrivilege RestorePrivilege
		{
			get
			{
				return new TokenPrivilege("SeRestorePrivilege");
			}
		}
	}
}
