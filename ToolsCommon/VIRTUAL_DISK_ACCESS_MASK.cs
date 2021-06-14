﻿using System;

namespace Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
	// Token: 0x02000032 RID: 50
	public enum VIRTUAL_DISK_ACCESS_MASK
	{
		// Token: 0x040000A0 RID: 160
		VIRTUAL_DISK_ACCESS_ATTACH_RO = 65536,
		// Token: 0x040000A1 RID: 161
		VIRTUAL_DISK_ACCESS_ATTACH_RW = 131072,
		// Token: 0x040000A2 RID: 162
		VIRTUAL_DISK_ACCESS_DETACH = 262144,
		// Token: 0x040000A3 RID: 163
		VIRTUAL_DISK_ACCESS_GET_INFO = 524288,
		// Token: 0x040000A4 RID: 164
		VIRTUAL_DISK_ACCESS_CREATE = 1048576,
		// Token: 0x040000A5 RID: 165
		VIRTUAL_DISK_ACCESS_METAOPS = 2097152,
		// Token: 0x040000A6 RID: 166
		VIRTUAL_DISK_ACCESS_READ = 851968,
		// Token: 0x040000A7 RID: 167
		VIRTUAL_DISK_ACCESS_ALL = 4128768,
		// Token: 0x040000A8 RID: 168
		VIRTUAL_DISK_ACCESS_WRITABLE = 3276800
	}
}
