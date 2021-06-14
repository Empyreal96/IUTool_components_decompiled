using System;

namespace Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
	// Token: 0x02000021 RID: 33
	public static class ProcessPrivilege
	{
		// Token: 0x0600011B RID: 283 RVA: 0x0000741C File Offset: 0x0000561C
		public static void Adjust(TokenPrivilege privilege, bool enablePrivilege)
		{
			int num = NativeSecurityMethods.IU_AdjustProcessPrivilege(privilege.Value, enablePrivilege);
			if (num != 0)
			{
				throw new Exception(string.Format("Failed to adjust privilege with name {0} and value {1}", privilege.Value, enablePrivilege));
			}
		}
	}
}
