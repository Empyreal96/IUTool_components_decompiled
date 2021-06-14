using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000058 RID: 88
	internal class DynamicHardDisk : IVirtualHardDisk, IDisposable
	{
		// Token: 0x060003DE RID: 990 RVA: 0x00011E48 File Offset: 0x00010048
		public DynamicHardDisk(string fileName, ulong sectorCount)
		{
			ulong num = 0UL;
			this._fileSize = sectorCount * (ulong)this.SectorSize;
			this._fileStream = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite);
			this.SectorCount = sectorCount;
			this._footer = new VhdFooter(this._fileSize, VhdType.Dynamic, (ulong)((long)Marshal.SizeOf(typeof(VhdFooter))));
			this.WriteVHDFooter(num);
			num += (ulong)((long)Marshal.SizeOf(typeof(VhdFooter)));
			this._header = new VhdHeader(this._fileSize);
			this.WriteVHDHeader(num);
			num += (ulong)((long)Marshal.SizeOf(typeof(VhdHeader)));
			this._tableOffset = num;
			this._blockAllocationTable = new BlockAllocationTable(this._header.MaxTableEntries);
			this.WriteBlockAllocationTable(num);
			num += this._blockAllocationTable.SizeInBytes;
			this._footerOffset = num;
			this.WriteVHDFooter(num);
			num += (ulong)((long)Marshal.SizeOf(typeof(VhdFooter)));
		}

		// Token: 0x060003DF RID: 991 RVA: 0x00011F3C File Offset: 0x0001013C
		public DynamicHardDisk(string existingFile, bool addWriteAccess = false)
		{
			FileAccess access = FileAccess.Read;
			if (addWriteAccess)
			{
				access = FileAccess.ReadWrite;
			}
			this._fileStream = new FileStream(existingFile, FileMode.Open, access, FileShare.ReadWrite);
			this._footer = VhdFooter.Read(this._fileStream);
			this._fileSize = this._footer.CurrentSize;
			this.SectorCount = this._fileSize / (ulong)VhdCommon.VHDSectorSize;
			this._fileStream.Position = (long)this._footer.DataOffset;
			this._header = VhdHeader.Read(this._fileStream);
			this._tableOffset = this._header.TableOffset;
			this._fileStream.Position = (long)this._header.TableOffset;
			this._blockAllocationTable = new BlockAllocationTable(this._header.MaxTableEntries);
			this._blockAllocationTable.Read(this._fileStream);
		}

		// Token: 0x060003E0 RID: 992 RVA: 0x0001200F File Offset: 0x0001020F
		public void Close()
		{
			if (this._fileStream != null)
			{
				this._fileStream.Close();
				this._fileStream = null;
			}
		}

		// Token: 0x170000EE RID: 238
		// (get) Token: 0x060003E1 RID: 993 RVA: 0x0001202B File Offset: 0x0001022B
		public uint SectorSize
		{
			get
			{
				return VhdCommon.VHDSectorSize;
			}
		}

		// Token: 0x170000EF RID: 239
		// (get) Token: 0x060003E2 RID: 994 RVA: 0x00012032 File Offset: 0x00010232
		// (set) Token: 0x060003E3 RID: 995 RVA: 0x0001203A File Offset: 0x0001023A
		public ulong SectorCount { get; private set; }

		// Token: 0x060003E4 RID: 996 RVA: 0x00012043 File Offset: 0x00010243
		public void FlushFile()
		{
			this.WriteVHDFooter(this._footerOffset);
			this._fileStream.Flush();
		}

		// Token: 0x170000F0 RID: 240
		// (get) Token: 0x060003E5 RID: 997 RVA: 0x0001205C File Offset: 0x0001025C
		public BlockAllocationTable AllocationTable
		{
			get
			{
				return this._blockAllocationTable;
			}
		}

		// Token: 0x170000F1 RID: 241
		// (get) Token: 0x060003E6 RID: 998 RVA: 0x00012064 File Offset: 0x00010264
		public uint BlockSize
		{
			get
			{
				return this._header.BlockSize;
			}
		}

		// Token: 0x170000F2 RID: 242
		// (get) Token: 0x060003E7 RID: 999 RVA: 0x00012074 File Offset: 0x00010274
		public uint BlockBitmapSectorCount
		{
			get
			{
				uint num = this.BlockSize / this.SectorSize;
				uint num2 = num / 8U;
				if (num % 8U != 0U)
				{
					num2 += 1U;
				}
				uint num3 = num2 / this.SectorSize;
				if (num2 % this.SectorSize != 0U)
				{
					num3 += 1U;
				}
				return num3;
			}
		}

		// Token: 0x060003E8 RID: 1000 RVA: 0x000120B4 File Offset: 0x000102B4
		public bool SectorIsAllocated(ulong sectorIndex)
		{
			uint index = (uint)(sectorIndex / (ulong)this.SectorsPerBlock);
			return uint.MaxValue != this._blockAllocationTable[index];
		}

		// Token: 0x060003E9 RID: 1001 RVA: 0x000120E0 File Offset: 0x000102E0
		public void ReadSector(ulong sector, byte[] buffer, uint offset)
		{
			if (sector >= this.SectorCount)
			{
				throw new ArgumentException("Sector is out of bound", "sector");
			}
			if ((long)buffer.Length - (long)((ulong)offset) < (long)((ulong)this.SectorSize))
			{
				throw new ArgumentException("The buffer, from the given offset, is smaller than the sector size.", "offset");
			}
			uint index = (uint)(sector / (ulong)this.SectorsPerBlock);
			uint num = (uint)(sector % (ulong)this.SectorsPerBlock);
			if (4294967295U == this._blockAllocationTable[index])
			{
				Array.Copy(DynamicHardDisk.emptySectorBuffer, 0L, buffer, (long)((ulong)offset), (long)((ulong)this.SectorSize));
				return;
			}
			byte[] buffer2 = new byte[this.SectorSize];
			this._fileStream.Seek((long)(this._blockAllocationTable[index] * this.SectorSize), SeekOrigin.Begin);
			this._fileStream.Read(buffer2, 0, (int)this.SectorSize);
			uint num2 = num / 8U;
			this._fileStream.Seek((long)((ulong)((this._blockAllocationTable[index] + num + this.BlockBitmapSectorCount) * this.SectorSize)), SeekOrigin.Begin);
			this._fileStream.Read(buffer, (int)offset, (int)this.SectorSize);
		}

		// Token: 0x060003EA RID: 1002 RVA: 0x000121E8 File Offset: 0x000103E8
		public void WriteSector(ulong sector, byte[] buffer, uint offset)
		{
			if (sector >= this.SectorCount)
			{
				throw new ArgumentException("Sector is out of bound", "sector");
			}
			if ((long)buffer.Length - (long)((ulong)offset) < (long)((ulong)this.SectorSize))
			{
				throw new ArgumentException("The buffer, from the given offset, is smaller than the sector size.", "offset");
			}
			uint index = (uint)(sector / (ulong)this.SectorsPerBlock);
			uint num = (uint)(sector % (ulong)this.SectorsPerBlock);
			if (4294967295U == this._blockAllocationTable[index])
			{
				this._blockAllocationTable[index] = (uint)(this._footerOffset / (ulong)this.SectorSize);
				this.WriteBlockAllocationTable(this._tableOffset);
				this._fileStream.Seek((long)this._footerOffset, SeekOrigin.Begin);
				this._fileStream.Write(DynamicHardDisk.emptySectorBuffer, 0, (int)this.SectorSize);
				this._footerOffset += (ulong)(this.SectorSize + VhdCommon.DynamicVHDBlockSize);
			}
			byte[] array = new byte[this.SectorSize];
			this._fileStream.Seek((long)(this._blockAllocationTable[index] * this.SectorSize), SeekOrigin.Begin);
			this._fileStream.Read(array, 0, (int)this.SectorSize);
			uint num2 = num / 8U;
			byte b = (byte)(num % 8U);
			array[(int)num2] = (byte)((int)array[(int)num2] | 1 << (int)b);
			this._fileStream.Seek((long)(this._blockAllocationTable[index] * this.SectorSize), SeekOrigin.Begin);
			this._fileStream.Write(array, 0, (int)this.SectorSize);
			this._fileStream.Seek((long)((ulong)(this._blockAllocationTable[index] + num + 1U) * (ulong)this.SectorSize), SeekOrigin.Begin);
			this._fileStream.Write(buffer, (int)offset, (int)this.SectorSize);
		}

		// Token: 0x060003EB RID: 1003 RVA: 0x00012388 File Offset: 0x00010588
		~DynamicHardDisk()
		{
			this.Dispose(false);
		}

		// Token: 0x060003EC RID: 1004 RVA: 0x000123B8 File Offset: 0x000105B8
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x060003ED RID: 1005 RVA: 0x000123C7 File Offset: 0x000105C7
		protected virtual void Dispose(bool isDisposing)
		{
			if (this._alreadyDisposed)
			{
				return;
			}
			if (this._fileStream != null)
			{
				this._fileStream.Close();
				this._fileStream = null;
			}
			this._alreadyDisposed = true;
		}

		// Token: 0x170000F3 RID: 243
		// (get) Token: 0x060003EE RID: 1006 RVA: 0x000123F5 File Offset: 0x000105F5
		private uint SectorsPerBlock
		{
			get
			{
				return VhdCommon.DynamicVHDBlockSize / this.SectorSize;
			}
		}

		// Token: 0x060003EF RID: 1007 RVA: 0x00012403 File Offset: 0x00010603
		private void WriteVHDFooter(ulong offset)
		{
			this._fileStream.Seek((long)offset, SeekOrigin.Begin);
			this._footer.Write(this._fileStream);
		}

		// Token: 0x060003F0 RID: 1008 RVA: 0x00012424 File Offset: 0x00010624
		private void WriteVHDHeader(ulong offset)
		{
			this._fileStream.Seek((long)offset, SeekOrigin.Begin);
			this._header.Write(this._fileStream);
		}

		// Token: 0x060003F1 RID: 1009 RVA: 0x00012445 File Offset: 0x00010645
		private void WriteBlockAllocationTable(ulong offset)
		{
			this._fileStream.Seek((long)offset, SeekOrigin.Begin);
			this._blockAllocationTable.Write(this._fileStream);
		}

		// Token: 0x04000224 RID: 548
		private FileStream _fileStream;

		// Token: 0x04000225 RID: 549
		private VhdFooter _footer;

		// Token: 0x04000226 RID: 550
		private VhdHeader _header;

		// Token: 0x04000227 RID: 551
		private BlockAllocationTable _blockAllocationTable;

		// Token: 0x04000228 RID: 552
		private ulong _tableOffset;

		// Token: 0x04000229 RID: 553
		private ulong _fileSize;

		// Token: 0x0400022A RID: 554
		private ulong _footerOffset;

		// Token: 0x0400022B RID: 555
		public static byte[] emptySectorBuffer = new byte[VhdCommon.VHDSectorSize];

		// Token: 0x0400022D RID: 557
		private bool _alreadyDisposed;
	}
}
