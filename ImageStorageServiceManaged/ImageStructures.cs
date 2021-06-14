using System;
using System.Runtime.InteropServices;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x0200004C RID: 76
	public class ImageStructures
	{
		// Token: 0x02000084 RID: 132
		[Flags]
		public enum DiskAttributes : ulong
		{
			// Token: 0x040002D5 RID: 725
			Offline = 1UL,
			// Token: 0x040002D6 RID: 726
			ReadOnly = 2UL
		}

		// Token: 0x02000085 RID: 133
		[StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
		public struct PARTITION_ENTRY
		{
			// Token: 0x17000129 RID: 297
			// (get) Token: 0x060004F0 RID: 1264 RVA: 0x00014EE3 File Offset: 0x000130E3
			// (set) Token: 0x060004F1 RID: 1265 RVA: 0x00014EEB File Offset: 0x000130EB
			public string PartitionName
			{
				get
				{
					return this.name;
				}
				set
				{
					this.name = value;
				}
			}

			// Token: 0x1700012A RID: 298
			// (get) Token: 0x060004F2 RID: 1266 RVA: 0x00014EF4 File Offset: 0x000130F4
			// (set) Token: 0x060004F3 RID: 1267 RVA: 0x00014EFC File Offset: 0x000130FC
			[CLSCompliant(false)]
			public ulong SectorCount
			{
				get
				{
					return this.sectorCount;
				}
				set
				{
					this.sectorCount = value;
				}
			}

			// Token: 0x1700012B RID: 299
			// (get) Token: 0x060004F4 RID: 1268 RVA: 0x00014F05 File Offset: 0x00013105
			// (set) Token: 0x060004F5 RID: 1269 RVA: 0x00014F0D File Offset: 0x0001310D
			[CLSCompliant(false)]
			public uint AlignmentSizeInBytes
			{
				get
				{
					return this.alignmentSizeInBytes;
				}
				set
				{
					this.alignmentSizeInBytes = value;
				}
			}

			// Token: 0x1700012C RID: 300
			// (get) Token: 0x060004F6 RID: 1270 RVA: 0x00014F16 File Offset: 0x00013116
			// (set) Token: 0x060004F7 RID: 1271 RVA: 0x00014F1E File Offset: 0x0001311E
			[CLSCompliant(false)]
			public uint ClusterSize
			{
				get
				{
					return this.clusterSize;
				}
				set
				{
					this.clusterSize = value;
				}
			}

			// Token: 0x1700012D RID: 301
			// (get) Token: 0x060004F8 RID: 1272 RVA: 0x00014F27 File Offset: 0x00013127
			// (set) Token: 0x060004F9 RID: 1273 RVA: 0x00014F2F File Offset: 0x0001312F
			public string FileSystem
			{
				get
				{
					return this.fileSystem;
				}
				set
				{
					this.fileSystem = value;
				}
			}

			// Token: 0x1700012E RID: 302
			// (get) Token: 0x060004FA RID: 1274 RVA: 0x00014F38 File Offset: 0x00013138
			// (set) Token: 0x060004FB RID: 1275 RVA: 0x00014F40 File Offset: 0x00013140
			public Guid PartitionId
			{
				get
				{
					return this.id;
				}
				set
				{
					this.id = value;
				}
			}

			// Token: 0x1700012F RID: 303
			// (get) Token: 0x060004FC RID: 1276 RVA: 0x00014F49 File Offset: 0x00013149
			// (set) Token: 0x060004FD RID: 1277 RVA: 0x00014F51 File Offset: 0x00013151
			public Guid PartitionType
			{
				get
				{
					return this.type;
				}
				set
				{
					this.type = value;
				}
			}

			// Token: 0x17000130 RID: 304
			// (get) Token: 0x060004FE RID: 1278 RVA: 0x00014F5A File Offset: 0x0001315A
			// (set) Token: 0x060004FF RID: 1279 RVA: 0x00014F62 File Offset: 0x00013162
			[CLSCompliant(false)]
			public ulong PartitionFlags
			{
				get
				{
					return this.flags;
				}
				set
				{
					this.flags = value;
				}
			}

			// Token: 0x17000131 RID: 305
			// (get) Token: 0x06000500 RID: 1280 RVA: 0x00014F6B File Offset: 0x0001316B
			// (set) Token: 0x06000501 RID: 1281 RVA: 0x00014F73 File Offset: 0x00013173
			public byte MBRFlags
			{
				get
				{
					return this.mbrFlags;
				}
				set
				{
					this.mbrFlags = value;
				}
			}

			// Token: 0x17000132 RID: 306
			// (get) Token: 0x06000502 RID: 1282 RVA: 0x00014F7C File Offset: 0x0001317C
			// (set) Token: 0x06000503 RID: 1283 RVA: 0x00014F84 File Offset: 0x00013184
			public byte MBRType
			{
				get
				{
					return this.mbrType;
				}
				set
				{
					this.mbrType = value;
				}
			}

			// Token: 0x040002D7 RID: 727
			[FieldOffset(0)]
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 36)]
			private string name;

			// Token: 0x040002D8 RID: 728
			[FieldOffset(72)]
			private ulong sectorCount;

			// Token: 0x040002D9 RID: 729
			[FieldOffset(80)]
			private uint alignmentSizeInBytes;

			// Token: 0x040002DA RID: 730
			[FieldOffset(84)]
			private uint clusterSize;

			// Token: 0x040002DB RID: 731
			[FieldOffset(88)]
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
			private string fileSystem;

			// Token: 0x040002DC RID: 732
			[FieldOffset(152)]
			private Guid id;

			// Token: 0x040002DD RID: 733
			[FieldOffset(168)]
			private Guid type;

			// Token: 0x040002DE RID: 734
			[FieldOffset(184)]
			private ulong flags;

			// Token: 0x040002DF RID: 735
			[FieldOffset(152)]
			private byte mbrFlags;

			// Token: 0x040002E0 RID: 736
			[FieldOffset(153)]
			private byte mbrType;
		}

		// Token: 0x02000086 RID: 134
		[CLSCompliant(false)]
		[StructLayout(LayoutKind.Explicit)]
		public struct STORE_ID
		{
			// Token: 0x17000133 RID: 307
			// (get) Token: 0x06000504 RID: 1284 RVA: 0x00014F8D File Offset: 0x0001318D
			// (set) Token: 0x06000505 RID: 1285 RVA: 0x00014F95 File Offset: 0x00013195
			public uint StoreType
			{
				get
				{
					return this.storeType;
				}
				set
				{
					this.storeType = value;
				}
			}

			// Token: 0x17000134 RID: 308
			// (get) Token: 0x06000506 RID: 1286 RVA: 0x00014F9E File Offset: 0x0001319E
			// (set) Token: 0x06000507 RID: 1287 RVA: 0x00014FA6 File Offset: 0x000131A6
			public Guid StoreId_GPT
			{
				get
				{
					return this.storeId_GPT;
				}
				set
				{
					this.storeId_GPT = value;
				}
			}

			// Token: 0x17000135 RID: 309
			// (get) Token: 0x06000508 RID: 1288 RVA: 0x00014FAF File Offset: 0x000131AF
			// (set) Token: 0x06000509 RID: 1289 RVA: 0x00014FB7 File Offset: 0x000131B7
			public uint StoreId_MBR
			{
				get
				{
					return this.storeId_MBR;
				}
				set
				{
					this.storeId_MBR = value;
				}
			}

			// Token: 0x17000136 RID: 310
			// (get) Token: 0x0600050A RID: 1290 RVA: 0x00014FC0 File Offset: 0x000131C0
			public bool IsEmpty
			{
				get
				{
					return this.storeId_GPT == Guid.Empty && this.storeId_MBR == 0U;
				}
			}

			// Token: 0x040002E1 RID: 737
			[FieldOffset(0)]
			private uint storeType;

			// Token: 0x040002E2 RID: 738
			[FieldOffset(4)]
			private Guid storeId_GPT;

			// Token: 0x040002E3 RID: 739
			[FieldOffset(4)]
			private uint storeId_MBR;
		}

		// Token: 0x02000087 RID: 135
		[CLSCompliant(false)]
		[StructLayout(LayoutKind.Explicit)]
		public struct PartitionAttributes
		{
			// Token: 0x040002E4 RID: 740
			[FieldOffset(0)]
			public ulong gptAttributes;

			// Token: 0x040002E5 RID: 741
			[FieldOffset(0)]
			public byte mbrAttributes;
		}

		// Token: 0x02000088 RID: 136
		[StructLayout(LayoutKind.Explicit)]
		public struct PartitionType
		{
			// Token: 0x040002E6 RID: 742
			[FieldOffset(0)]
			public Guid gptType;

			// Token: 0x040002E7 RID: 743
			[FieldOffset(0)]
			public byte mbrType;
		}

		// Token: 0x02000089 RID: 137
		public struct SetDiskAttributes
		{
			// Token: 0x040002E8 RID: 744
			public uint Version;

			// Token: 0x040002E9 RID: 745
			public byte Persist;

			// Token: 0x040002EA RID: 746
			private byte dummy1;

			// Token: 0x040002EB RID: 747
			private byte dummy2;

			// Token: 0x040002EC RID: 748
			private byte dummy3;

			// Token: 0x040002ED RID: 749
			public ImageStructures.DiskAttributes Attributes;

			// Token: 0x040002EE RID: 750
			public ImageStructures.DiskAttributes AttributesMask;

			// Token: 0x040002EF RID: 751
			private Guid Reserved;
		}
	}
}
