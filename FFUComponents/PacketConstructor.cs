using System;
using System.IO;

namespace FFUComponents
{
	// Token: 0x0200003F RID: 63
	internal class PacketConstructor : IDisposable
	{
		// Token: 0x0600012C RID: 300 RVA: 0x00004C71 File Offset: 0x00002E71
		public PacketConstructor()
		{
			this.packetNumber = 0;
			this.PacketDataSize = (long)PacketConstructor.cbDefaultData;
		}

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x0600012D RID: 301 RVA: 0x00004C8C File Offset: 0x00002E8C
		public static long DefaultPacketDataSize
		{
			get
			{
				return (long)PacketConstructor.cbDefaultData;
			}
		}

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x0600012E RID: 302 RVA: 0x00004C94 File Offset: 0x00002E94
		public static long MaxPacketDataSize
		{
			get
			{
				return (long)PacketConstructor.cbMaxData;
			}
		}

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x0600012F RID: 303 RVA: 0x00004C9C File Offset: 0x00002E9C
		// (set) Token: 0x06000130 RID: 304 RVA: 0x00004CA4 File Offset: 0x00002EA4
		public Stream DataStream { internal get; set; }

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x06000131 RID: 305 RVA: 0x00004CAD File Offset: 0x00002EAD
		// (set) Token: 0x06000132 RID: 306 RVA: 0x00004CB5 File Offset: 0x00002EB5
		public long PacketDataSize { internal get; set; }

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x06000133 RID: 307 RVA: 0x00004CBE File Offset: 0x00002EBE
		public long Position
		{
			get
			{
				return this.DataStream.Position;
			}
		}

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x06000134 RID: 308 RVA: 0x00004CCB File Offset: 0x00002ECB
		public long Length
		{
			get
			{
				return this.DataStream.Length;
			}
		}

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x06000135 RID: 309 RVA: 0x00004CD8 File Offset: 0x00002ED8
		public long RemainingData
		{
			get
			{
				return this.DataStream.Length - this.DataStream.Position;
			}
		}

		// Token: 0x06000136 RID: 310 RVA: 0x00004CF1 File Offset: 0x00002EF1
		public void Reset()
		{
			this.DataStream.Seek(0L, SeekOrigin.Begin);
			this.packetNumber = 0;
		}

		// Token: 0x06000137 RID: 311 RVA: 0x00004D0C File Offset: 0x00002F0C
		public unsafe byte[] GetNextPacket(bool optimize)
		{
			byte[] array = new byte[this.PacketDataSize + 12L];
			Array.Clear(array, 0, array.Length);
			int value = this.DataStream.Read(array, 0, (int)this.PacketDataSize);
			int num = (int)this.PacketDataSize;
			byte[] bytes = BitConverter.GetBytes(value);
			bytes.CopyTo(array, num);
			num += bytes.Length;
			int num2 = this.packetNumber;
			this.packetNumber = num2 + 1;
			bytes = BitConverter.GetBytes(num2);
			bytes.CopyTo(array, num);
			num += bytes.Length;
			uint value2 = 0U;
			if (!optimize)
			{
				fixed (byte* ptr = array)
				{
					value2 = Crc32.GetChecksum(0U, ptr, (uint)(array.Length - 4));
				}
			}
			bytes = BitConverter.GetBytes(value2);
			bytes.CopyTo(array, num);
			return array;
		}

		// Token: 0x06000138 RID: 312 RVA: 0x00004DD0 File Offset: 0x00002FD0
		public byte[] GetZeroLengthPacket()
		{
			this.DataStream.Seek(0L, SeekOrigin.End);
			return this.GetNextPacket(false);
		}

		// Token: 0x06000139 RID: 313 RVA: 0x00004DE8 File Offset: 0x00002FE8
		public void Dispose()
		{
			this.Dispose(true);
		}

		// Token: 0x0600013A RID: 314 RVA: 0x00004DF1 File Offset: 0x00002FF1
		private void Dispose(bool fDisposing)
		{
			if (fDisposing && this.DataStream != null)
			{
				this.DataStream.Dispose();
				this.DataStream = null;
			}
		}

		// Token: 0x040000DF RID: 223
		private static readonly int cbDefaultData = 262144;

		// Token: 0x040000E0 RID: 224
		private static readonly int cbMaxData = 8388608;

		// Token: 0x040000E1 RID: 225
		private int packetNumber;
	}
}
