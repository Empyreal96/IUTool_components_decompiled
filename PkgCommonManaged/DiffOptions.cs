using System;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgCommon
{
	// Token: 0x02000026 RID: 38
	[Flags]
	public enum DiffOptions
	{
		// Token: 0x04000077 RID: 119
		None = 0,
		// Token: 0x04000078 RID: 120
		PrsSignCatalog = 1,
		// Token: 0x04000079 RID: 121
		DeltaThresholdMB = 2
	}
}
