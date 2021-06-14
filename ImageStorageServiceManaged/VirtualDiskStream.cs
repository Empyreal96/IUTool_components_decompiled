using System;
using System.IO;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000059 RID: 89
	internal class VirtualDiskStream : Stream
	{
		// Token: 0x170000F4 RID: 244
		// (get) Token: 0x060003F3 RID: 1011 RVA: 0x00012477 File Offset: 0x00010677
		// (set) Token: 0x060003F4 RID: 1012 RVA: 0x0001247F File Offset: 0x0001067F
		private DynamicHardDisk VirtualDisk { get; set; }

		// Token: 0x060003F5 RID: 1013 RVA: 0x00012488 File Offset: 0x00010688
		public VirtualDiskStream(DynamicHardDisk virtualDisk)
		{
			this.VirtualDisk = virtualDisk;
			this._sectorBuffer = new byte[virtualDisk.SectorSize];
		}

		// Token: 0x170000F5 RID: 245
		// (get) Token: 0x060003F6 RID: 1014 RVA: 0x0000AF1A File Offset: 0x0000911A
		public override bool CanRead
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170000F6 RID: 246
		// (get) Token: 0x060003F7 RID: 1015 RVA: 0x0000AF1A File Offset: 0x0000911A
		public override bool CanWrite
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170000F7 RID: 247
		// (get) Token: 0x060003F8 RID: 1016 RVA: 0x0000AF1A File Offset: 0x0000911A
		public override bool CanSeek
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170000F8 RID: 248
		// (get) Token: 0x060003F9 RID: 1017 RVA: 0x00005269 File Offset: 0x00003469
		public override bool CanTimeout
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170000F9 RID: 249
		// (get) Token: 0x060003FA RID: 1018 RVA: 0x000124AF File Offset: 0x000106AF
		public override long Length
		{
			get
			{
				return (long)(this.VirtualDisk.SectorCount * (ulong)this.VirtualDisk.SectorSize);
			}
		}

		// Token: 0x170000FA RID: 250
		// (get) Token: 0x060003FB RID: 1019 RVA: 0x000124C9 File Offset: 0x000106C9
		// (set) Token: 0x060003FC RID: 1020 RVA: 0x000124D1 File Offset: 0x000106D1
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

		// Token: 0x060003FD RID: 1021 RVA: 0x000124EE File Offset: 0x000106EE
		public override void Flush()
		{
			this.VirtualDisk.FlushFile();
		}

		// Token: 0x060003FE RID: 1022 RVA: 0x000124FC File Offset: 0x000106FC
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

		// Token: 0x060003FF RID: 1023 RVA: 0x0000B038 File Offset: 0x00009238
		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		// Token: 0x06000400 RID: 1024 RVA: 0x000125E4 File Offset: 0x000107E4
		public override int Read(byte[] buffer, int offset, int count)
		{
			int num = 0;
			while (count > 0)
			{
				uint num2 = (uint)(this.Position / (long)((ulong)this.VirtualDisk.SectorSize));
				uint num3 = (uint)(this.Position % (long)((ulong)this.VirtualDisk.SectorSize));
				int num4 = Math.Min(count, (int)(this.VirtualDisk.SectorSize - num3));
				if (this._sectorBufferIndex != num2)
				{
					if (this.VirtualDisk.SectorIsAllocated((ulong)num2))
					{
						if ((long)num4 == (long)((ulong)this.VirtualDisk.SectorSize))
						{
							this.VirtualDisk.ReadSector((ulong)num2, buffer, (uint)offset);
						}
						else
						{
							this.VirtualDisk.ReadSector((ulong)num2, this._sectorBuffer, 0U);
							this._sectorBufferIndex = num2;
							for (int i = 0; i < num4; i++)
							{
								buffer[offset + i] = this._sectorBuffer[(int)(checked((IntPtr)(unchecked((ulong)num3 + (ulong)((long)i)))))];
							}
						}
					}
					else
					{
						for (int j = 0; j < num4; j++)
						{
							buffer[offset + j] = 0;
						}
					}
				}
				else
				{
					for (int k = 0; k < num4; k++)
					{
						buffer[offset + k] = this._sectorBuffer[(int)(checked((IntPtr)(unchecked((ulong)num3 + (ulong)((long)k)))))];
					}
				}
				offset += num4;
				count -= num4;
				num += num4;
				this.Position += (long)num4;
			}
			return num;
		}

		// Token: 0x06000401 RID: 1025 RVA: 0x00012718 File Offset: 0x00010918
		public override void Write(byte[] buffer, int offset, int count)
		{
			if ((long)offset + this.Position > this.Length)
			{
				throw new EndOfStreamException("Cannot write past the end of the stream.");
			}
			while (count > 0)
			{
				uint num = (uint)(this.Position / (long)((ulong)this.VirtualDisk.SectorSize));
				uint num2 = (uint)(this.Position % (long)((ulong)this.VirtualDisk.SectorSize));
				int num3 = Math.Min(count, (int)(this.VirtualDisk.SectorSize - num2));
				if (!this.VirtualDisk.SectorIsAllocated((ulong)num))
				{
					throw new ImageStorageException("Writing to an unallocated virtual disk location is not supported.");
				}
				if (num2 == 0U && (long)num3 == (long)((ulong)this.VirtualDisk.SectorSize))
				{
					this.VirtualDisk.WriteSector((ulong)num, buffer, (uint)offset);
				}
				else
				{
					if (this._sectorBufferIndex != num)
					{
						this.VirtualDisk.ReadSector((ulong)num, this._sectorBuffer, 0U);
						this._sectorBufferIndex = num;
					}
					for (int i = 0; i < num3; i++)
					{
						this._sectorBuffer[(int)(checked((IntPtr)(unchecked((ulong)num2 + (ulong)((long)i)))))] = buffer[offset + i];
					}
					this.VirtualDisk.WriteSector((ulong)num, this._sectorBuffer, 0U);
				}
				offset += num3;
				count -= num3;
				this.Position += (long)num3;
			}
		}

		// Token: 0x0400022E RID: 558
		private uint _sectorBufferIndex = uint.MaxValue;

		// Token: 0x0400022F RID: 559
		private byte[] _sectorBuffer;

		// Token: 0x04000230 RID: 560
		private long _position;
	}
}
