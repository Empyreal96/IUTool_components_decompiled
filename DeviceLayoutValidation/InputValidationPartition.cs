using System;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000004 RID: 4
	public class InputValidationPartition
	{
		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000007 RID: 7 RVA: 0x000020A6 File Offset: 0x000002A6
		// (set) Token: 0x06000008 RID: 8 RVA: 0x000020AE File Offset: 0x000002AE
		public string Name { get; set; }

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000009 RID: 9 RVA: 0x000020B7 File Offset: 0x000002B7
		// (set) Token: 0x0600000A RID: 10 RVA: 0x000020BF File Offset: 0x000002BF
		public string Position { get; set; }

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x0600000B RID: 11 RVA: 0x000020C8 File Offset: 0x000002C8
		// (set) Token: 0x0600000C RID: 12 RVA: 0x000020D0 File Offset: 0x000002D0
		public string PartitionType { get; set; }

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600000D RID: 13 RVA: 0x000020D9 File Offset: 0x000002D9
		// (set) Token: 0x0600000E RID: 14 RVA: 0x000020E1 File Offset: 0x000002E1
		public string Optional { get; set; }

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x0600000F RID: 15 RVA: 0x000020EA File Offset: 0x000002EA
		// (set) Token: 0x06000010 RID: 16 RVA: 0x000020F2 File Offset: 0x000002F2
		public string ReadOnly { get; set; }

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000011 RID: 17 RVA: 0x000020FB File Offset: 0x000002FB
		// (set) Token: 0x06000012 RID: 18 RVA: 0x00002103 File Offset: 0x00000303
		public string AttachDriveLetter { get; set; }

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000013 RID: 19 RVA: 0x0000210C File Offset: 0x0000030C
		// (set) Token: 0x06000014 RID: 20 RVA: 0x00002114 File Offset: 0x00000314
		public string Hidden { get; set; }

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000015 RID: 21 RVA: 0x0000211D File Offset: 0x0000031D
		// (set) Token: 0x06000016 RID: 22 RVA: 0x00002125 File Offset: 0x00000325
		public string Bootable { get; set; }

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000017 RID: 23 RVA: 0x0000212E File Offset: 0x0000032E
		// (set) Token: 0x06000018 RID: 24 RVA: 0x00002136 File Offset: 0x00000336
		public string TotalSectors { get; set; }

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000019 RID: 25 RVA: 0x0000213F File Offset: 0x0000033F
		// (set) Token: 0x0600001A RID: 26 RVA: 0x00002147 File Offset: 0x00000347
		public string MinFreeSectors { get; set; }

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x0600001B RID: 27 RVA: 0x00002150 File Offset: 0x00000350
		// (set) Token: 0x0600001C RID: 28 RVA: 0x00002158 File Offset: 0x00000358
		public string GeneratedFileOverheadSectors { get; set; }

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x0600001D RID: 29 RVA: 0x00002161 File Offset: 0x00000361
		// (set) Token: 0x0600001E RID: 30 RVA: 0x00002169 File Offset: 0x00000369
		public string UseAllSpace { get; set; }

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x0600001F RID: 31 RVA: 0x00002172 File Offset: 0x00000372
		// (set) Token: 0x06000020 RID: 32 RVA: 0x0000217A File Offset: 0x0000037A
		public string FileSystem { get; set; }

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000021 RID: 33 RVA: 0x00002183 File Offset: 0x00000383
		// (set) Token: 0x06000022 RID: 34 RVA: 0x0000218B File Offset: 0x0000038B
		public string UpdateType { get; set; }

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000023 RID: 35 RVA: 0x00002194 File Offset: 0x00000394
		// (set) Token: 0x06000024 RID: 36 RVA: 0x0000219C File Offset: 0x0000039C
		public string Compressed { get; set; }

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000025 RID: 37 RVA: 0x000021A5 File Offset: 0x000003A5
		// (set) Token: 0x06000026 RID: 38 RVA: 0x000021AD File Offset: 0x000003AD
		public string RequiresCompression { get; set; }

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000028 RID: 40 RVA: 0x000021BF File Offset: 0x000003BF
		// (set) Token: 0x06000027 RID: 39 RVA: 0x000021B6 File Offset: 0x000003B6
		public string PrimaryPartition { get; set; }

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000029 RID: 41 RVA: 0x000021C7 File Offset: 0x000003C7
		// (set) Token: 0x0600002A RID: 42 RVA: 0x000021CF File Offset: 0x000003CF
		public string RequiredToFlash { get; set; }

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x0600002B RID: 43 RVA: 0x000021D8 File Offset: 0x000003D8
		// (set) Token: 0x0600002C RID: 44 RVA: 0x000021E0 File Offset: 0x000003E0
		public string SingleSectorAlignment { get; set; }

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x0600002D RID: 45 RVA: 0x000021E9 File Offset: 0x000003E9
		// (set) Token: 0x0600002E RID: 46 RVA: 0x000021F1 File Offset: 0x000003F1
		public string ByteAlignment { get; set; }

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x0600002F RID: 47 RVA: 0x000021FA File Offset: 0x000003FA
		// (set) Token: 0x06000030 RID: 48 RVA: 0x00002202 File Offset: 0x00000402
		public string ClusterSize { get; set; }
	}
}
