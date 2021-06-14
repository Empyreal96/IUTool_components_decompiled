using System;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x0200001A RID: 26
	[CLSCompliant(false)]
	public interface IBlockIoIdentifier : IDeviceIdentifier
	{
		// Token: 0x17000022 RID: 34
		// (get) Token: 0x06000089 RID: 137
		[CLSCompliant(false)]
		BlockIoType BlockType { get; }
	}
}
