using System;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000020 RID: 32
	public class InputStore
	{
		// Token: 0x06000145 RID: 325 RVA: 0x00004257 File Offset: 0x00002457
		public InputStore()
		{
		}

		// Token: 0x06000146 RID: 326 RVA: 0x0000D7CA File Offset: 0x0000B9CA
		public InputStore(string storeType)
		{
			this.StoreType = storeType;
		}

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x06000147 RID: 327 RVA: 0x0000D7D9 File Offset: 0x0000B9D9
		// (set) Token: 0x06000148 RID: 328 RVA: 0x0000D7E1 File Offset: 0x0000B9E1
		public string Id { get; set; }

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x06000149 RID: 329 RVA: 0x0000D7EA File Offset: 0x0000B9EA
		// (set) Token: 0x0600014A RID: 330 RVA: 0x0000D7F2 File Offset: 0x0000B9F2
		public string StoreType { get; set; }

		// Token: 0x1700004B RID: 75
		// (get) Token: 0x0600014B RID: 331 RVA: 0x0000D7FB File Offset: 0x0000B9FB
		// (set) Token: 0x0600014C RID: 332 RVA: 0x0000D803 File Offset: 0x0000BA03
		public string DevicePath { get; set; }

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x0600014D RID: 333 RVA: 0x0000D80C File Offset: 0x0000BA0C
		// (set) Token: 0x0600014E RID: 334 RVA: 0x0000D814 File Offset: 0x0000BA14
		[CLSCompliant(false)]
		public uint SizeInSectors { get; set; }

		// Token: 0x1700004D RID: 77
		// (get) Token: 0x0600014F RID: 335 RVA: 0x0000D81D File Offset: 0x0000BA1D
		// (set) Token: 0x06000150 RID: 336 RVA: 0x0000D825 File Offset: 0x0000BA25
		public bool OnlyAllocateDefinedGptEntries { get; set; }

		// Token: 0x06000151 RID: 337 RVA: 0x0000D82E File Offset: 0x0000BA2E
		public bool IsMainOSStore()
		{
			return string.CompareOrdinal(this.StoreType, "MainOSStore") == 0;
		}

		// Token: 0x040000E4 RID: 228
		[XmlArrayItem(ElementName = "Partition", Type = typeof(InputPartition), IsNullable = false)]
		[XmlArray]
		public InputPartition[] Partitions;
	}
}
