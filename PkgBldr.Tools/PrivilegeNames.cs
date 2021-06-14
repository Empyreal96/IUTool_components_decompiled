using System;

namespace Microsoft.CompPlat.PkgBldr.Tools
{
	// Token: 0x0200002C RID: 44
	public class PrivilegeNames
	{
		// Token: 0x1700002C RID: 44
		// (get) Token: 0x0600018D RID: 397 RVA: 0x00007617 File Offset: 0x00005817
		public static TokenPrivilege BackupPrivilege
		{
			get
			{
				return new TokenPrivilege("SeBackupPrivilege");
			}
		}

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x0600018E RID: 398 RVA: 0x00007623 File Offset: 0x00005823
		public static TokenPrivilege SecurityPrivilege
		{
			get
			{
				return new TokenPrivilege("SeSecurityPrivilege");
			}
		}

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x0600018F RID: 399 RVA: 0x0000762F File Offset: 0x0000582F
		public static TokenPrivilege RestorePrivilege
		{
			get
			{
				return new TokenPrivilege("SeRestorePrivilege");
			}
		}
	}
}
