using System;

namespace Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
	// Token: 0x02000030 RID: 48
	[Flags]
	public enum OPEN_VIRTUAL_DISK_FLAG
	{
		// Token: 0x04000098 RID: 152
		OPEN_VIRTUAL_DISK_FLAG_NONE = 0,
		// Token: 0x04000099 RID: 153
		OPEN_VIRTUAL_DISK_FLAG_NO_PARENTS = 1,
		// Token: 0x0400009A RID: 154
		OPEN_VIRTUAL_DISK_FLAG_BLANK_FILE = 2,
		// Token: 0x0400009B RID: 155
		OPEN_VIRTUAL_DISK_FLAG_BOOT_DRIVE = 4
	}
}
