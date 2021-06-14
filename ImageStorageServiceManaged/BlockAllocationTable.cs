using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000056 RID: 86
	internal class BlockAllocationTable
	{
		// Token: 0x170000EB RID: 235
		public uint this[uint index]
		{
			get
			{
				return this._blockAllocationTable[(int)index];
			}
			set
			{
				this._blockAllocationTable[(int)index] = value;
			}
		}

		// Token: 0x170000EC RID: 236
		// (get) Token: 0x060003D3 RID: 979 RVA: 0x00011A81 File Offset: 0x0000FC81
		public ulong SizeInBytes
		{
			get
			{
				return (ulong)((long)this._blockAllocationTable.Length * (long)Marshal.SizeOf(typeof(uint)));
			}
		}

		// Token: 0x170000ED RID: 237
		// (get) Token: 0x060003D4 RID: 980 RVA: 0x00011A9D File Offset: 0x0000FC9D
		public uint EntryCount
		{
			get
			{
				return (uint)this._blockAllocationTable.Length;
			}
		}

		// Token: 0x060003D5 RID: 981 RVA: 0x00011AA8 File Offset: 0x0000FCA8
		public BlockAllocationTable(uint batSize)
		{
			uint num = VhdCommon.Round(batSize * (uint)Marshal.SizeOf(typeof(uint)), VhdCommon.VHDSectorSize) / (uint)Marshal.SizeOf(typeof(uint));
			this._blockAllocationTable = new uint[num];
			int num2 = 0;
			while ((long)num2 < (long)((ulong)batSize))
			{
				this._blockAllocationTable[num2] = uint.MaxValue;
				num2++;
			}
		}

		// Token: 0x060003D6 RID: 982 RVA: 0x00011B0C File Offset: 0x0000FD0C
		public void Write(FileStream writer)
		{
			uint[] blockAllocationTable = this._blockAllocationTable;
			for (int i = 0; i < blockAllocationTable.Length; i++)
			{
				uint num = VhdCommon.Swap32(blockAllocationTable[i]);
				writer.WriteStruct(ref num);
			}
		}

		// Token: 0x060003D7 RID: 983 RVA: 0x00011B40 File Offset: 0x0000FD40
		public void Read(FileStream reader)
		{
			for (int i = 0; i < this._blockAllocationTable.Length; i++)
			{
				uint data = reader.ReadStruct<uint>();
				this._blockAllocationTable[i] = VhdCommon.Swap32(data);
			}
		}

		// Token: 0x0400020E RID: 526
		private uint[] _blockAllocationTable;
	}
}
