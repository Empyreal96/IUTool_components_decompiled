using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000033 RID: 51
	internal class DataBlockEntry
	{
		// Token: 0x170000A6 RID: 166
		// (get) Token: 0x06000205 RID: 517 RVA: 0x000092D2 File Offset: 0x000074D2
		// (set) Token: 0x06000206 RID: 518 RVA: 0x000092DA File Offset: 0x000074DA
		private uint BytesPerBlock { get; set; }

		// Token: 0x06000207 RID: 519 RVA: 0x000092E3 File Offset: 0x000074E3
		public DataBlockEntry(uint bytesPerBlock)
		{
			this.BytesPerBlock = bytesPerBlock;
			this.DataSource = new DataBlockSource();
		}

		// Token: 0x170000A7 RID: 167
		// (get) Token: 0x06000208 RID: 520 RVA: 0x00009308 File Offset: 0x00007508
		public List<DiskLocation> BlockLocationsOnDisk
		{
			get
			{
				return this._blockLocations;
			}
		}

		// Token: 0x170000A8 RID: 168
		// (get) Token: 0x06000209 RID: 521 RVA: 0x00009310 File Offset: 0x00007510
		// (set) Token: 0x0600020A RID: 522 RVA: 0x00009318 File Offset: 0x00007518
		public DataBlockSource DataSource { get; set; }

		// Token: 0x170000A9 RID: 169
		// (get) Token: 0x0600020B RID: 523 RVA: 0x00009321 File Offset: 0x00007521
		public int SizeInBytes
		{
			get
			{
				return 8 + this._blockLocations.Count * DiskLocation.SizeInBytes;
			}
		}

		// Token: 0x0600020C RID: 524 RVA: 0x00009338 File Offset: 0x00007538
		public void WriteEntryToStream(Stream outputStream)
		{
			BinaryWriter binaryWriter = new BinaryWriter(outputStream);
			binaryWriter.Write((uint)this._blockLocations.Count);
			binaryWriter.Write(1);
			for (int i = 0; i < this._blockLocations.Count; i++)
			{
				this._blockLocations[i].Write(binaryWriter);
			}
		}

		// Token: 0x0600020D RID: 525 RVA: 0x00009390 File Offset: 0x00007590
		public void ReadEntryFromStream(BinaryReader reader, uint index)
		{
			int num = reader.ReadInt32();
			if (reader.ReadUInt32() != 1U)
			{
				throw new ImageStorageException("More than one block per data block entry is not currently supported.");
			}
			for (int i = 0; i < num; i++)
			{
				DiskLocation diskLocation = new DiskLocation();
				diskLocation.Read(reader);
				this._blockLocations.Add(diskLocation);
			}
			this.DataSource = new DataBlockSource
			{
				Source = DataBlockSource.DataSource.Disk,
				StorageOffset = (ulong)(index * this.BytesPerBlock)
			};
		}

		// Token: 0x0600020E RID: 526 RVA: 0x00009400 File Offset: 0x00007600
		public void WriteDataToByteArray(Stream sourceStream, byte[] blockData, int index, int byteCount)
		{
			int num = Math.Min(byteCount, (int)this.BytesPerBlock);
			switch (this.DataSource.Source)
			{
			case DataBlockSource.DataSource.Zero:
				Array.Clear(blockData, index, num);
				return;
			case DataBlockSource.DataSource.Disk:
				sourceStream.Position = (long)this.DataSource.StorageOffset;
				sourceStream.Read(blockData, index, num);
				return;
			case DataBlockSource.DataSource.Memory:
				Array.Copy(this.DataSource.GetMemoryData(), 0, blockData, index, num);
				return;
			default:
				return;
			}
		}

		// Token: 0x0600020F RID: 527 RVA: 0x00009473 File Offset: 0x00007673
		public void WriteDataToStream(Stream outputStream, Stream sourceStream, byte[] blockData)
		{
			this.WriteDataToByteArray(sourceStream, blockData, 0, (int)this.BytesPerBlock);
			outputStream.Write(blockData, 0, blockData.Length);
		}

		// Token: 0x06000210 RID: 528 RVA: 0x00009490 File Offset: 0x00007690
		public void LogInfo(IULogger logger, bool logSources, ushort indentLevel = 0)
		{
			string str = new StringBuilder().Append(' ', (int)indentLevel).ToString();
			logger.LogInfo(str + "Block Location Count: {0}", new object[]
			{
				this._blockLocations.Count
			});
			foreach (DiskLocation diskLocation in this._blockLocations)
			{
				diskLocation.LogInfo(logger, indentLevel + 2);
			}
			if (logSources)
			{
				this.DataSource.LogInfo(logger, indentLevel + 2);
			}
		}

		// Token: 0x06000211 RID: 529 RVA: 0x00009538 File Offset: 0x00007738
		public DataBlockEntry CreateMemoryBlockEntry(Stream sourceStream)
		{
			DataBlockEntry dataBlockEntry = new DataBlockEntry(this.BytesPerBlock);
			dataBlockEntry._blockLocations = new List<DiskLocation>(this._blockLocations);
			byte[] newMemoryData = new DataBlockSource
			{
				Source = DataBlockSource.DataSource.Memory
			}.GetNewMemoryData(this.BytesPerBlock);
			this.WriteDataToByteArray(sourceStream, newMemoryData, 0, (int)this.BytesPerBlock);
			return dataBlockEntry;
		}

		// Token: 0x04000155 RID: 341
		private List<DiskLocation> _blockLocations = new List<DiskLocation>();
	}
}
