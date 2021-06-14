using System;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000021 RID: 33
	public class InputPartition
	{
		// Token: 0x1700004E RID: 78
		// (get) Token: 0x06000152 RID: 338 RVA: 0x0000D843 File Offset: 0x0000BA43
		// (set) Token: 0x06000153 RID: 339 RVA: 0x0000D84B File Offset: 0x0000BA4B
		public string Name { get; set; }

		// Token: 0x1700004F RID: 79
		// (get) Token: 0x06000154 RID: 340 RVA: 0x0000D854 File Offset: 0x0000BA54
		// (set) Token: 0x06000155 RID: 341 RVA: 0x0000D85C File Offset: 0x0000BA5C
		public string Type { get; set; }

		// Token: 0x17000050 RID: 80
		// (get) Token: 0x06000156 RID: 342 RVA: 0x0000D865 File Offset: 0x0000BA65
		// (set) Token: 0x06000157 RID: 343 RVA: 0x0000D86D File Offset: 0x0000BA6D
		public string Id { get; set; }

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x06000158 RID: 344 RVA: 0x0000D876 File Offset: 0x0000BA76
		// (set) Token: 0x06000159 RID: 345 RVA: 0x0000D87E File Offset: 0x0000BA7E
		public bool ReadOnly { get; set; }

		// Token: 0x17000052 RID: 82
		// (get) Token: 0x0600015A RID: 346 RVA: 0x0000D887 File Offset: 0x0000BA87
		// (set) Token: 0x0600015B RID: 347 RVA: 0x0000D88F File Offset: 0x0000BA8F
		public bool AttachDriveLetter { get; set; }

		// Token: 0x17000053 RID: 83
		// (get) Token: 0x0600015C RID: 348 RVA: 0x0000D898 File Offset: 0x0000BA98
		// (set) Token: 0x0600015D RID: 349 RVA: 0x0000D8A0 File Offset: 0x0000BAA0
		public bool Hidden { get; set; }

		// Token: 0x17000054 RID: 84
		// (get) Token: 0x0600015E RID: 350 RVA: 0x0000D8A9 File Offset: 0x0000BAA9
		// (set) Token: 0x0600015F RID: 351 RVA: 0x0000D8B1 File Offset: 0x0000BAB1
		public bool Bootable { get; set; }

		// Token: 0x17000055 RID: 85
		// (get) Token: 0x06000160 RID: 352 RVA: 0x0000D8BA File Offset: 0x0000BABA
		// (set) Token: 0x06000161 RID: 353 RVA: 0x0000D8C2 File Offset: 0x0000BAC2
		[CLSCompliant(false)]
		public uint TotalSectors { get; set; }

		// Token: 0x17000056 RID: 86
		// (get) Token: 0x06000162 RID: 354 RVA: 0x0000D8CB File Offset: 0x0000BACB
		// (set) Token: 0x06000163 RID: 355 RVA: 0x0000D8D3 File Offset: 0x0000BAD3
		public bool UseAllSpace { get; set; }

		// Token: 0x17000057 RID: 87
		// (get) Token: 0x06000164 RID: 356 RVA: 0x0000D8DC File Offset: 0x0000BADC
		// (set) Token: 0x06000165 RID: 357 RVA: 0x0000D8E4 File Offset: 0x0000BAE4
		public string FileSystem { get; set; }

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x06000166 RID: 358 RVA: 0x0000D8ED File Offset: 0x0000BAED
		// (set) Token: 0x06000167 RID: 359 RVA: 0x0000D8F5 File Offset: 0x0000BAF5
		public string UpdateType { get; set; }

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x06000168 RID: 360 RVA: 0x0000D8FE File Offset: 0x0000BAFE
		// (set) Token: 0x06000169 RID: 361 RVA: 0x0000D906 File Offset: 0x0000BB06
		public bool Compressed { get; set; }

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x0600016A RID: 362 RVA: 0x0000D90F File Offset: 0x0000BB0F
		// (set) Token: 0x0600016B RID: 363 RVA: 0x0000D917 File Offset: 0x0000BB17
		[XmlElement("RequiresCompression")]
		public bool RequiresCompression { get; set; }

		// Token: 0x1700005B RID: 91
		// (get) Token: 0x0600016D RID: 365 RVA: 0x0000D929 File Offset: 0x0000BB29
		// (set) Token: 0x0600016C RID: 364 RVA: 0x0000D920 File Offset: 0x0000BB20
		public string PrimaryPartition
		{
			get
			{
				if (string.IsNullOrEmpty(this._primaryPartition))
				{
					return this.Name;
				}
				return this._primaryPartition;
			}
			set
			{
				this._primaryPartition = value;
			}
		}

		// Token: 0x1700005C RID: 92
		// (get) Token: 0x0600016E RID: 366 RVA: 0x0000D945 File Offset: 0x0000BB45
		// (set) Token: 0x0600016F RID: 367 RVA: 0x0000D94D File Offset: 0x0000BB4D
		public bool RequiredToFlash { get; set; }

		// Token: 0x1700005D RID: 93
		// (get) Token: 0x06000170 RID: 368 RVA: 0x0000D956 File Offset: 0x0000BB56
		// (set) Token: 0x06000171 RID: 369 RVA: 0x0000D95E File Offset: 0x0000BB5E
		public bool SingleSectorAlignment { get; set; }

		// Token: 0x1700005E RID: 94
		// (get) Token: 0x06000172 RID: 370 RVA: 0x0000D967 File Offset: 0x0000BB67
		// (set) Token: 0x06000173 RID: 371 RVA: 0x0000D96F File Offset: 0x0000BB6F
		[CLSCompliant(false)]
		[XmlIgnore]
		public uint ByteAlignment { get; set; }

		// Token: 0x1700005F RID: 95
		// (get) Token: 0x06000174 RID: 372 RVA: 0x0000D978 File Offset: 0x0000BB78
		// (set) Token: 0x06000175 RID: 373 RVA: 0x0000D998 File Offset: 0x0000BB98
		[XmlElement("ByteAlignment")]
		public string ByteAlignmentString
		{
			get
			{
				return this.ByteAlignment.ToString(CultureInfo.InvariantCulture);
			}
			set
			{
				uint byteAlignment = 0U;
				if (!InputHelpers.StringToUint(value, out byteAlignment))
				{
					throw new ImageCommonException(string.Format("Partition {0}'s byte alignment cannot be parsed.", string.IsNullOrEmpty(this.Name) ? "Unknown" : this.Name));
				}
				this.ByteAlignment = byteAlignment;
			}
		}

		// Token: 0x17000060 RID: 96
		// (get) Token: 0x06000176 RID: 374 RVA: 0x0000D9E2 File Offset: 0x0000BBE2
		// (set) Token: 0x06000177 RID: 375 RVA: 0x0000D9EA File Offset: 0x0000BBEA
		[CLSCompliant(false)]
		[XmlIgnore]
		public uint ClusterSize { get; set; }

		// Token: 0x17000061 RID: 97
		// (get) Token: 0x06000178 RID: 376 RVA: 0x0000D9F4 File Offset: 0x0000BBF4
		// (set) Token: 0x06000179 RID: 377 RVA: 0x0000DA14 File Offset: 0x0000BC14
		[XmlElement("ClusterSize")]
		public string ClusterSizeString
		{
			get
			{
				return this.ClusterSize.ToString(CultureInfo.InvariantCulture);
			}
			set
			{
				uint clusterSize = 0U;
				if (!InputHelpers.StringToUint(value, out clusterSize))
				{
					throw new ImageCommonException(string.Format("Partition {0}'s cluster size cannot be parsed.", string.IsNullOrEmpty(this.Name) ? "Unknown" : this.Name));
				}
				this.ClusterSize = clusterSize;
			}
		}

		// Token: 0x040000ED RID: 237
		[CLSCompliant(false)]
		public uint MinFreeSectors;

		// Token: 0x040000EE RID: 238
		[CLSCompliant(false)]
		public uint GeneratedFileOverheadSectors;

		// Token: 0x040000F4 RID: 244
		private string _primaryPartition;
	}
}
