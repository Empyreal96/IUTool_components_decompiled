using System;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000039 RID: 57
	internal interface IBlockStreamSource
	{
		// Token: 0x170000B1 RID: 177
		// (get) Token: 0x06000241 RID: 577
		long Length { get; }

		// Token: 0x06000242 RID: 578
		void ReadBlock(uint blockIndex, byte[] buffer, int bufferIndex);
	}
}
