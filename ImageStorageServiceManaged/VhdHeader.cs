using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000055 RID: 85
	internal struct VhdHeader
	{
		// Token: 0x060003CC RID: 972 RVA: 0x000118C8 File Offset: 0x0000FAC8
		public VhdHeader(ulong vhdFileSize)
		{
			this.Cookie = BitConverter.ToUInt64(Encoding.ASCII.GetBytes("cxsparse"), 0);
			this.DataOffset = ulong.MaxValue;
			this.TableOffset = (ulong)((long)(Marshal.SizeOf(typeof(VhdFooter)) + Marshal.SizeOf(typeof(VhdHeader))));
			this.HeaderVersion = 65536U;
			this.BlockSize = VhdCommon.DynamicVHDBlockSize;
			this.ParentUniqueId = Guid.Empty;
			this.ParentTimeStamp = 0U;
			this.Reserved = new byte[964];
			this.CheckSum = 0U;
			this.MaxTableEntries = 0U;
			this.MaxTableEntries = VhdHeader.CalculateNumberOfBlocks(vhdFileSize);
			this.CheckSum = VhdCommon.CalculateChecksum<VhdHeader>(ref this);
		}

		// Token: 0x060003CD RID: 973 RVA: 0x0001197C File Offset: 0x0000FB7C
		private static uint CalculateNumberOfBlocks(ulong vhdFileSize)
		{
			if (vhdFileSize % (ulong)VhdCommon.DynamicVHDBlockSize != 0UL)
			{
				return (uint)(vhdFileSize / (ulong)VhdCommon.DynamicVHDBlockSize + 1UL);
			}
			return (uint)(vhdFileSize / (ulong)VhdCommon.DynamicVHDBlockSize);
		}

		// Token: 0x060003CE RID: 974 RVA: 0x000119A0 File Offset: 0x0000FBA0
		private void ChangeByteOrder()
		{
			this.DataOffset = VhdCommon.Swap64(this.DataOffset);
			this.TableOffset = VhdCommon.Swap64(this.TableOffset);
			this.HeaderVersion = VhdCommon.Swap32(this.HeaderVersion);
			this.MaxTableEntries = VhdCommon.Swap32(this.MaxTableEntries);
			this.BlockSize = VhdCommon.Swap32(this.BlockSize);
			this.CheckSum = VhdCommon.Swap32(this.CheckSum);
		}

		// Token: 0x060003CF RID: 975 RVA: 0x00011A14 File Offset: 0x0000FC14
		public void Write(FileStream writer)
		{
			this.ChangeByteOrder();
			try
			{
				writer.WriteStruct(ref this);
			}
			finally
			{
				this.ChangeByteOrder();
				writer.Flush();
			}
		}

		// Token: 0x060003D0 RID: 976 RVA: 0x00011A50 File Offset: 0x0000FC50
		public static VhdHeader Read(FileStream reader)
		{
			VhdHeader result = reader.ReadStruct<VhdHeader>();
			result.ChangeByteOrder();
			return result;
		}

		// Token: 0x04000201 RID: 513
		private const int VHD_HEADER_RESERVED_REGION_SIZE = 964;

		// Token: 0x04000202 RID: 514
		private const string VHD_HEADER_COOKIE = "cxsparse";

		// Token: 0x04000203 RID: 515
		private const uint VHD_HEADER_VERSION = 65536U;

		// Token: 0x04000204 RID: 516
		public ulong Cookie;

		// Token: 0x04000205 RID: 517
		public ulong DataOffset;

		// Token: 0x04000206 RID: 518
		public ulong TableOffset;

		// Token: 0x04000207 RID: 519
		public uint HeaderVersion;

		// Token: 0x04000208 RID: 520
		public uint MaxTableEntries;

		// Token: 0x04000209 RID: 521
		public uint BlockSize;

		// Token: 0x0400020A RID: 522
		public uint CheckSum;

		// Token: 0x0400020B RID: 523
		public Guid ParentUniqueId;

		// Token: 0x0400020C RID: 524
		public uint ParentTimeStamp;

		// Token: 0x0400020D RID: 525
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 964)]
		private byte[] Reserved;
	}
}
