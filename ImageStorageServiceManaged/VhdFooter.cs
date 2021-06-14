using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000057 RID: 87
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	internal struct VhdFooter
	{
		// Token: 0x060003D8 RID: 984 RVA: 0x00011B78 File Offset: 0x0000FD78
		public VhdFooter(ulong vhdFileSize, VhdType vhdType, ulong dataOffset)
		{
			this.Cookie = BitConverter.ToUInt64(Encoding.ASCII.GetBytes("conectix"), 0);
			this.Features = 0U;
			this.FileFormatVersion = 65536U;
			this.DataOffset = dataOffset;
			this.TimeStamp = (uint)(DateTime.UtcNow - new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
			this.CreatorApplication = 1987278701U;
			this.CreatorVersion = 65536U;
			this.CreatorHostOs = 1466511979U;
			this.OriginalSize = vhdFileSize;
			this.CurrentSize = vhdFileSize;
			this.UniqueId = default(Guid);
			this.DriveType = (uint)vhdType;
			this.SavedState = 0;
			this.Reserved = new byte[427];
			this.DriveGeometry = VhdFooter.GetDriveGeometry(vhdFileSize);
			this.CheckSum = 0U;
			this.CheckSum = VhdCommon.CalculateChecksum<VhdFooter>(ref this);
		}

		// Token: 0x060003D9 RID: 985 RVA: 0x00011C5A File Offset: 0x0000FE5A
		public VhdFooter(ulong vhdFileSize, VhdType type)
		{
			this = new VhdFooter(vhdFileSize, type, ulong.MaxValue);
		}

		// Token: 0x060003DA RID: 986 RVA: 0x00011C68 File Offset: 0x0000FE68
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

		// Token: 0x060003DB RID: 987 RVA: 0x00011CA4 File Offset: 0x0000FEA4
		public static VhdFooter Read(FileStream reader)
		{
			VhdFooter result = reader.ReadStruct<VhdFooter>();
			result.ChangeByteOrder();
			return result;
		}

		// Token: 0x060003DC RID: 988 RVA: 0x00011CC0 File Offset: 0x0000FEC0
		private void ChangeByteOrder()
		{
			this.Features = VhdCommon.Swap32(this.Features);
			this.FileFormatVersion = VhdCommon.Swap32(this.FileFormatVersion);
			this.DataOffset = VhdCommon.Swap64(this.DataOffset);
			this.TimeStamp = VhdCommon.Swap32(this.TimeStamp);
			this.CreatorApplication = VhdCommon.Swap32(this.CreatorApplication);
			this.CreatorVersion = VhdCommon.Swap32(this.CreatorVersion);
			this.CreatorHostOs = VhdCommon.Swap32(this.CreatorHostOs);
			this.OriginalSize = VhdCommon.Swap64(this.OriginalSize);
			this.CurrentSize = VhdCommon.Swap64(this.CurrentSize);
			this.DriveGeometry = VhdCommon.Swap32(this.DriveGeometry);
			this.DriveType = VhdCommon.Swap32(this.DriveType);
			this.CheckSum = VhdCommon.Swap32(this.CheckSum);
		}

		// Token: 0x060003DD RID: 989 RVA: 0x00011D9C File Offset: 0x0000FF9C
		private static uint GetDriveGeometry(ulong vhdFileSize)
		{
			uint num = (uint)(vhdFileSize / (ulong)VhdCommon.VHDSectorSize);
			uint num2 = 0U;
			if (num > 267382800U)
			{
				num = 267382800U;
			}
			uint num3;
			uint num4;
			uint num6;
			if (num >= 66059280U)
			{
				num3 = 255U;
				num4 = 16U;
				uint num5 = num / num3;
				num6 = num5 / num4;
			}
			else
			{
				num3 = 17U;
				uint num5 = num / num3;
				num4 = (num5 + 1023U) / 1024U;
				if (num4 < 4U)
				{
					num4 = 4U;
				}
				if (num5 >= num4 * 1024U || num4 > 16U)
				{
					num3 = 31U;
					num4 = 16U;
					num5 = num / num3;
				}
				if (num5 >= num4 * 1024U)
				{
					num3 = 63U;
					num4 = 16U;
					num5 = num / num3;
				}
				num6 = num5 / num4;
			}
			return num2 | num6 << 16 | num4 << 8 | num3;
		}

		// Token: 0x0400020F RID: 527
		private const string VHD_FOOTER_COOKIE = "conectix";

		// Token: 0x04000210 RID: 528
		private const uint VHD_FILE_FORMAT_VERSION = 65536U;

		// Token: 0x04000211 RID: 529
		private const uint VHD_FOOTER_CREATOR_APPLICATION = 1987278701U;

		// Token: 0x04000212 RID: 530
		private const uint VHD_FOOTER_CREATOR_VERSION = 65536U;

		// Token: 0x04000213 RID: 531
		private const int VHD_FOOTER_RESERVED_REGION_SIZE = 427;

		// Token: 0x04000214 RID: 532
		public ulong Cookie;

		// Token: 0x04000215 RID: 533
		public uint Features;

		// Token: 0x04000216 RID: 534
		public uint FileFormatVersion;

		// Token: 0x04000217 RID: 535
		public ulong DataOffset;

		// Token: 0x04000218 RID: 536
		public uint TimeStamp;

		// Token: 0x04000219 RID: 537
		public uint CreatorApplication;

		// Token: 0x0400021A RID: 538
		public uint CreatorVersion;

		// Token: 0x0400021B RID: 539
		public uint CreatorHostOs;

		// Token: 0x0400021C RID: 540
		public ulong OriginalSize;

		// Token: 0x0400021D RID: 541
		public ulong CurrentSize;

		// Token: 0x0400021E RID: 542
		private uint DriveGeometry;

		// Token: 0x0400021F RID: 543
		private uint DriveType;

		// Token: 0x04000220 RID: 544
		private uint CheckSum;

		// Token: 0x04000221 RID: 545
		private Guid UniqueId;

		// Token: 0x04000222 RID: 546
		private byte SavedState;

		// Token: 0x04000223 RID: 547
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 427)]
		private byte[] Reserved;

		// Token: 0x0200008A RID: 138
		private enum VhdFooterFeatures : uint
		{
			// Token: 0x040002F1 RID: 753
			VHD_FOOTER_FEATURES_NONE,
			// Token: 0x040002F2 RID: 754
			VHD_FOOTER_FEATURES_TEMPORARY,
			// Token: 0x040002F3 RID: 755
			VHD_FOOTER_FEATURES_RESERVED
		}

		// Token: 0x0200008B RID: 139
		private enum VhdFooterCreaterHostOS : uint
		{
			// Token: 0x040002F5 RID: 757
			VHD_FOOTER_CREATOR_HOST_OS_WINDOWS = 1466511979U,
			// Token: 0x040002F6 RID: 758
			VHD_FOOTER_CREATOR_HOST_OS_MACINTOSH = 1298228000U
		}
	}
}
