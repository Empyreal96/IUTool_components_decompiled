using System;
using System.IO;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000031 RID: 49
	internal class ValidationEntry
	{
		// Token: 0x170000A0 RID: 160
		// (get) Token: 0x060001EF RID: 495 RVA: 0x0000910C File Offset: 0x0000730C
		// (set) Token: 0x060001F0 RID: 496 RVA: 0x00009114 File Offset: 0x00007314
		public uint SectorIndex { get; set; }

		// Token: 0x170000A1 RID: 161
		// (get) Token: 0x060001F1 RID: 497 RVA: 0x0000911D File Offset: 0x0000731D
		// (set) Token: 0x060001F2 RID: 498 RVA: 0x00009125 File Offset: 0x00007325
		public int SectorOffset { get; set; }

		// Token: 0x170000A2 RID: 162
		// (get) Token: 0x060001F3 RID: 499 RVA: 0x0000912E File Offset: 0x0000732E
		// (set) Token: 0x060001F4 RID: 500 RVA: 0x00009136 File Offset: 0x00007336
		public int ByteCount { get; set; }

		// Token: 0x060001F5 RID: 501 RVA: 0x0000913F File Offset: 0x0000733F
		public void SetCompareData(byte[] data)
		{
			this._compareData = data;
		}

		// Token: 0x060001F6 RID: 502 RVA: 0x00009148 File Offset: 0x00007348
		public byte[] GetCompareData()
		{
			return this._compareData;
		}

		// Token: 0x060001F7 RID: 503 RVA: 0x00009150 File Offset: 0x00007350
		public void Write(BinaryWriter writer)
		{
			writer.Write(this.SectorIndex);
			writer.Write(this.SectorOffset);
			writer.Write(this.ByteCount);
			writer.Write(this.GetCompareData());
		}

		// Token: 0x060001F8 RID: 504 RVA: 0x00009182 File Offset: 0x00007382
		public void Read(BinaryReader reader)
		{
			this.SectorIndex = reader.ReadUInt32();
			this.SectorOffset = reader.ReadInt32();
			this.ByteCount = reader.ReadInt32();
			this.SetCompareData(reader.ReadBytes(this.ByteCount));
		}

		// Token: 0x0400014F RID: 335
		private byte[] _compareData;
	}
}
