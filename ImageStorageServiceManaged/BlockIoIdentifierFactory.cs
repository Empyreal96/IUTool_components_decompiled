using System;
using System.IO;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000019 RID: 25
	public static class BlockIoIdentifierFactory
	{
		// Token: 0x06000087 RID: 135 RVA: 0x000050A0 File Offset: 0x000032A0
		[CLSCompliant(false)]
		public static IBlockIoIdentifier CreateFromStream(BinaryReader reader)
		{
			switch (reader.ReadUInt32())
			{
			case 0U:
				return new HardDiskIdentifier();
			case 1U:
				return new RemovableDiskIdentifier();
			case 2U:
				return new CdRomIdentifier();
			case 3U:
				return new RamDiskIdentifier("", BcdElementBootDevice.CreateBaseBootDevice());
			case 5U:
				return new FileIdentifier("", BcdElementBootDevice.CreateBaseBootDevice());
			case 6U:
				return new VirtualDiskIdentifier();
			}
			throw new ImageStorageException("The block IO type is unrecognized.");
		}

		// Token: 0x040000D0 RID: 208
		[CLSCompliant(false)]
		public static readonly uint SizeOnDisk = 40U;
	}
}
