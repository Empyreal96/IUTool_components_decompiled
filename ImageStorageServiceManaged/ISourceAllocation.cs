using System;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x0200003F RID: 63
	internal interface ISourceAllocation
	{
		// Token: 0x0600027D RID: 637
		uint GetAllocationSize();

		// Token: 0x0600027E RID: 638
		bool BlockIsAllocated(ulong diskByteOffset);
	}
}
