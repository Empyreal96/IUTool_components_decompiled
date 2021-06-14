using System;
using System.Globalization;

namespace Microsoft.CompPlat.PkgBldr.Tools
{
	// Token: 0x0200002D RID: 45
	public static class ProcessPrivilege
	{
		// Token: 0x06000191 RID: 401 RVA: 0x0000763C File Offset: 0x0000583C
		public static void Adjust(ConstValue<string> privilege, bool enablePrivilege)
		{
			int num = NativeSecurityMethods.IU_AdjustProcessPrivilege(privilege.Value, enablePrivilege);
			if (num != 0)
			{
				throw new Exception(string.Format(CultureInfo.InvariantCulture, "Failed to adjust privilege with name {0} and value {1}", new object[]
				{
					privilege.Value,
					enablePrivilege
				}));
			}
		}
	}
}
