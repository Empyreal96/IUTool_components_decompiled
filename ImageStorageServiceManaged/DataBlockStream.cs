using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x0200003C RID: 60
	internal class DataBlockStream : Stream
	{
		// Token: 0x170000B4 RID: 180
		// (get) Token: 0x06000250 RID: 592 RVA: 0x0000AE9F File Offset: 0x0000909F
		// (set) Token: 0x06000251 RID: 593 RVA: 0x0000AEA7 File Offset: 0x000090A7
		public List<DataBlockEntry> BlockEntries { get; private set; }

		// Token: 0x170000B5 RID: 181
		// (get) Token: 0x06000252 RID: 594 RVA: 0x0000AEB0 File Offset: 0x000090B0
		// (set) Token: 0x06000253 RID: 595 RVA: 0x0000AEB8 File Offset: 0x000090B8
		private uint BytesPerBlock { get; set; }

		// Token: 0x170000B6 RID: 182
		// (get) Token: 0x06000254 RID: 596 RVA: 0x0000AEC1 File Offset: 0x000090C1
		// (set) Token: 0x06000255 RID: 597 RVA: 0x0000AEC9 File Offset: 0x000090C9
		internal SortedDictionary<int, int> EntryLookupTable { get; set; }

		// Token: 0x170000B7 RID: 183
		// (get) Token: 0x06000256 RID: 598 RVA: 0x0000AED2 File Offset: 0x000090D2
		// (set) Token: 0x06000257 RID: 599 RVA: 0x0000AEDA File Offset: 0x000090DA
		private IBlockStreamSource Source { get; set; }

		// Token: 0x06000258 RID: 600 RVA: 0x0000AEE3 File Offset: 0x000090E3
		public DataBlockStream(IBlockStreamSource streamSource, uint bytesPerBlock)
		{
			this.BytesPerBlock = bytesPerBlock;
			this.EntryLookupTable = new SortedDictionary<int, int>();
			this.BlockEntries = new List<DataBlockEntry>();
			this.Source = streamSource;
		}

		// Token: 0x170000B8 RID: 184
		// (get) Token: 0x06000259 RID: 601 RVA: 0x0000AF1A File Offset: 0x0000911A
		public override bool CanRead
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170000B9 RID: 185
		// (get) Token: 0x0600025A RID: 602 RVA: 0x0000AF1A File Offset: 0x0000911A
		public override bool CanWrite
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170000BA RID: 186
		// (get) Token: 0x0600025B RID: 603 RVA: 0x0000AF1A File Offset: 0x0000911A
		public override bool CanSeek
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170000BB RID: 187
		// (get) Token: 0x0600025C RID: 604 RVA: 0x00005269 File Offset: 0x00003469
		public override bool CanTimeout
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170000BC RID: 188
		// (get) Token: 0x0600025D RID: 605 RVA: 0x0000AF1D File Offset: 0x0000911D
		public override long Length
		{
			get
			{
				return this.Source.Length;
			}
		}

		// Token: 0x170000BD RID: 189
		// (get) Token: 0x0600025E RID: 606 RVA: 0x0000AF2A File Offset: 0x0000912A
		// (set) Token: 0x0600025F RID: 607 RVA: 0x0000AF32 File Offset: 0x00009132
		public override long Position
		{
			get
			{
				return this._position;
			}
			set
			{
				if (value > this.Length)
				{
					throw new ImageStorageException("The given position is beyond the end of the image payload.");
				}
				this._position = value;
			}
		}

		// Token: 0x06000260 RID: 608 RVA: 0x00006180 File Offset: 0x00004380
		public override void Flush()
		{
		}

		// Token: 0x06000261 RID: 609 RVA: 0x0000AF50 File Offset: 0x00009150
		public override long Seek(long offset, SeekOrigin origin)
		{
			if (offset > this.Length)
			{
				throw new ImageStorageException("The  offset is beyond the end of the image.");
			}
			switch (origin)
			{
			case SeekOrigin.Begin:
				this._position = offset;
				return this._position;
			case SeekOrigin.Current:
				if (offset == 0L)
				{
					return this._position;
				}
				if (offset < 0L)
				{
					throw new ImageStorageException("Negative offsets are not implemented.");
				}
				if (this._position >= this.Length)
				{
					throw new ImageStorageException("The offset is beyond the end of the image.");
				}
				if (this.Length - this._position < offset)
				{
					throw new ImageStorageException("The offset is beyond the end of the image.");
				}
				this._position = offset;
				return this._position;
			case SeekOrigin.End:
				if (offset > 0L)
				{
					throw new ImageStorageException("The offset is beyond the end of the image.");
				}
				if (this.Length + offset < 0L)
				{
					throw new ImageStorageException("The offset is invalid.");
				}
				this._position = this.Length + offset;
				return this._position;
			default:
				throw new ImageStorageException("The origin parameter is invalid.");
			}
		}

		// Token: 0x06000262 RID: 610 RVA: 0x0000B038 File Offset: 0x00009238
		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		// Token: 0x06000263 RID: 611 RVA: 0x0000B040 File Offset: 0x00009240
		public override int Read(byte[] buffer, int offset, int count)
		{
			int num = 0;
			do
			{
				int num2 = (int)Math.Min((long)((ulong)this.BytesPerBlock - (ulong)(this._position % (long)((ulong)this.BytesPerBlock))), (long)(count - num));
				int blockIndexFromStreamPosition = this.BlockIndexFromStreamPosition;
				if (this._blockIndex != blockIndexFromStreamPosition)
				{
					if (!this.EntryLookupTable.ContainsKey(blockIndexFromStreamPosition))
					{
						DataBlockEntry dataBlockEntry = new DataBlockEntry(this.BytesPerBlock);
						dataBlockEntry.DataSource.Source = DataBlockSource.DataSource.Memory;
						byte[] newMemoryData = dataBlockEntry.DataSource.GetNewMemoryData(this.BytesPerBlock);
						this.Source.ReadBlock((uint)blockIndexFromStreamPosition, newMemoryData, 0);
						dataBlockEntry.BlockLocationsOnDisk.Add(new DiskLocation((uint)blockIndexFromStreamPosition, DiskLocation.DiskAccessMethod.DiskBegin));
						this.BlockEntries.Add(dataBlockEntry);
						this.EntryLookupTable.Add(blockIndexFromStreamPosition, this.BlockEntries.Count - 1);
						this._currentEntry = dataBlockEntry;
					}
					else
					{
						this._currentEntry = this.BlockEntries[this.EntryLookupTable[blockIndexFromStreamPosition]];
					}
					this._blockIndex = blockIndexFromStreamPosition;
				}
				int sourceIndex = (int)(this._position % (long)((ulong)this.BytesPerBlock));
				Array.Copy(this._currentEntry.DataSource.GetMemoryData(), sourceIndex, buffer, offset, num2);
				offset += num2;
				num += num2;
				this._position += (long)num2;
			}
			while (num < count);
			return num;
		}

		// Token: 0x06000264 RID: 612 RVA: 0x0000B184 File Offset: 0x00009384
		public override void Write(byte[] buffer, int offset, int count)
		{
			if ((long)offset + this.Position > this.Length)
			{
				throw new EndOfStreamException("Cannot write past the end of the stream.");
			}
			int num = 0;
			for (;;)
			{
				int num2 = (int)Math.Min((long)((ulong)this.BytesPerBlock - (ulong)(this._position % (long)((ulong)this.BytesPerBlock))), (long)(count - num));
				int blockIndexFromStreamPosition = this.BlockIndexFromStreamPosition;
				if (!this.EntryLookupTable.ContainsKey(blockIndexFromStreamPosition))
				{
					break;
				}
				if (blockIndexFromStreamPosition != this._blockIndex)
				{
					this._currentEntry = this.BlockEntries[this.EntryLookupTable[blockIndexFromStreamPosition]];
					this._blockIndex = blockIndexFromStreamPosition;
				}
				int destinationIndex = (int)(this._position % (long)((ulong)this.BytesPerBlock));
				Array.Copy(buffer, offset, this._currentEntry.DataSource.GetMemoryData(), destinationIndex, num2);
				offset += num2;
				num += num2;
				this._position += (long)num2;
				if (num >= count)
				{
					return;
				}
			}
			throw new ImageStorageException("Attempting to write to an unallocated block data stream location.");
		}

		// Token: 0x170000BE RID: 190
		// (get) Token: 0x06000265 RID: 613 RVA: 0x0000B265 File Offset: 0x00009465
		private int BlockIndexFromStreamPosition
		{
			get
			{
				if (this._position / (long)((ulong)this.BytesPerBlock) > 2147483647L)
				{
					throw new ImageStorageException("The stream position is outside the addressable block range.");
				}
				return (int)(this._position / (long)((ulong)this.BytesPerBlock));
			}
		}

		// Token: 0x0400017C RID: 380
		private int _blockIndex = int.MaxValue;

		// Token: 0x0400017D RID: 381
		private DataBlockEntry _currentEntry;

		// Token: 0x0400017E RID: 382
		private long _position;
	}
}
