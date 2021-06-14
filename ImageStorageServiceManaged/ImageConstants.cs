using System;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x0200004A RID: 74
	[CLSCompliant(false)]
	public sealed class ImageConstants
	{
		// Token: 0x040001C9 RID: 457
		public const uint ONE_MEGABYTE = 1048576U;

		// Token: 0x040001CA RID: 458
		public const uint ONE_GIGABYTE = 1073741824U;

		// Token: 0x040001CB RID: 459
		public static readonly uint PartitionTypeMbr = FullFlashUpdateImage.PartitionTypeMbr;

		// Token: 0x040001CC RID: 460
		public static readonly uint PartitionTypeGpt = FullFlashUpdateImage.PartitionTypeGpt;

		// Token: 0x040001CD RID: 461
		public static readonly uint MaxPartitionNameSizeInWideChars = 36U;

		// Token: 0x040001CE RID: 462
		public static readonly uint DeviceNameSize = 40U;

		// Token: 0x040001CF RID: 463
		public static readonly uint MaxVolumeNameSize = 260U;

		// Token: 0x040001D0 RID: 464
		public static readonly uint DefaultVirtualHardDiskSectorSize = 512U;

		// Token: 0x040001D1 RID: 465
		public static readonly uint RegistryKeyMaxNameSize = 255U;

		// Token: 0x040001D2 RID: 466
		public static readonly uint RegistryValueMaxNameSize = 16383U;

		// Token: 0x040001D3 RID: 467
		public static readonly Guid PARTITION_SYSTEM_GUID = new Guid("{c12a7328-f81f-11d2-ba4b-00a0c93ec93b}");

		// Token: 0x040001D4 RID: 468
		public static readonly Guid PARTITION_BASIC_DATA_GUID = new Guid("{ebd0a0a2-b9e5-4433-87c0-68b6b72699c7}");

		// Token: 0x040001D5 RID: 469
		public static readonly Guid PARTITION_MSFT_RECOVERY_GUID = new Guid("{de94bba4-06d1-4d40-a16a-bfd50179d6ac}");

		// Token: 0x040001D6 RID: 470
		public static readonly Guid SYSTEM_STORE_GUID = new Guid("{AE420040-13DD-41F2-AE7F-0DC35854C8D7}");

		// Token: 0x040001D7 RID: 471
		public static readonly ulong GPT_ATTRIBUTE_NO_DRIVE_LETTER = 9223372036854775808UL;

		// Token: 0x040001D8 RID: 472
		public static readonly uint SYSTEM_STORE_SIGNATURE = 2923561024U;

		// Token: 0x040001D9 RID: 473
		public static readonly uint MINIMUM_NTFS_PARTITION_SIZE = 7340032U;

		// Token: 0x040001DA RID: 474
		public static readonly uint MINIMUM_FAT_PARTITION_SIZE = 7340032U;

		// Token: 0x040001DB RID: 475
		public static readonly uint MINIMUM_FAT32_PARTITION_SIZE = 34603008U;

		// Token: 0x040001DC RID: 476
		public static readonly uint MINIMUM_PARTITION_SIZE = 16384U;

		// Token: 0x040001DD RID: 477
		public static readonly uint PARTITION_TABLE_METADATA_SIZE = 1048576U;

		// Token: 0x040001DE RID: 478
		public static readonly uint MAX_PRIMARY_PARTITIONS = 4U;

		// Token: 0x040001DF RID: 479
		public static readonly uint MBR_METADATA_PARTITION_SIZE = 1048576U;

		// Token: 0x040001E0 RID: 480
		public static readonly byte MBR_METADATA_PARTITION_TYPE = 112;

		// Token: 0x040001E1 RID: 481
		public static readonly string MBR_METADATA_PARTITION_NAME = "MBR_META";

		// Token: 0x040001E2 RID: 482
		public static readonly uint PAYLOAD_BLOCK_SIZE = 131072U;

		// Token: 0x040001E3 RID: 483
		public static readonly string MAINOS_PARTITION_NAME = "MainOS";

		// Token: 0x040001E4 RID: 484
		public static readonly Guid MAINOS_PARTITION_ID = new Guid("{A76B8CE2-0187-4C13-8FCA-8651C9B0620A}");

		// Token: 0x040001E5 RID: 485
		public static readonly string DATA_PARTITION_NAME = "Data";

		// Token: 0x040001E6 RID: 486
		public static readonly string SYSTEM_PARTITION_NAME = "EFIESP";

		// Token: 0x040001E7 RID: 487
		public static readonly string DPP_PARTITION_NAME = "DPP";

		// Token: 0x040001E8 RID: 488
		public static readonly Guid SYSTEM_PARTITION_ID = new Guid("{8183040A-8B44-4592-92F7-C6D9EE0560F7}");

		// Token: 0x040001E9 RID: 489
		public static readonly string BCD_FILE_PATH = "boot\\bcd";

		// Token: 0x040001EA RID: 490
		public static readonly string EFI_BCD_FILE_PATH = "efi\\microsoft\\boot\\bcd";

		// Token: 0x040001EB RID: 491
		public static readonly string MMOS_PARTITION_NAME = "MMOS";

		// Token: 0x040001EC RID: 492
		public static readonly Guid MMOS_PARTITION_ID = new Guid("{27A47557-8243-4C8E-9D30-846844C29C52}");

		// Token: 0x040001ED RID: 493
		public const string VHDMutex = "Global\\VHDMutex_{585b0806-2d3b-4226-b259-9c8d3b237d5c}";
	}
}
