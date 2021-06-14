using System;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents
{
	// Token: 0x0200004B RID: 75
	public enum ServiceStartMode
	{
		// Token: 0x040000FF RID: 255
		Boot,
		// Token: 0x04000100 RID: 256
		System,
		// Token: 0x04000101 RID: 257
		Auto,
		// Token: 0x04000102 RID: 258
		Demand,
		// Token: 0x04000103 RID: 259
		Disabled,
		// Token: 0x04000104 RID: 260
		DelayedAuto = -1
	}
}
