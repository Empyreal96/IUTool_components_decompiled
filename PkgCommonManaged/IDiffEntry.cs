using System;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgCommon
{
	// Token: 0x02000028 RID: 40
	public interface IDiffEntry
	{
		// Token: 0x1700008B RID: 139
		// (get) Token: 0x060001CE RID: 462
		FileType FileType { get; }

		// Token: 0x1700008C RID: 140
		// (get) Token: 0x060001CF RID: 463
		DiffType DiffType { get; }

		// Token: 0x1700008D RID: 141
		// (get) Token: 0x060001D0 RID: 464
		string DevicePath { get; }
	}
}
