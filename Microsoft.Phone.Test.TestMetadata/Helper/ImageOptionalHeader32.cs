using System;
using System.Runtime.InteropServices;

namespace Microsoft.Phone.Test.TestMetadata.Helper
{
	// Token: 0x0200000B RID: 11
	internal struct ImageOptionalHeader32
	{
		// Token: 0x04000031 RID: 49
		public ushort Magic;

		// Token: 0x04000032 RID: 50
		public byte MajorLinkerVersion;

		// Token: 0x04000033 RID: 51
		public byte MinorLinkerVersion;

		// Token: 0x04000034 RID: 52
		public uint SizeOfCode;

		// Token: 0x04000035 RID: 53
		public uint SizeOfInitializedData;

		// Token: 0x04000036 RID: 54
		public uint SizeOfUninitializedData;

		// Token: 0x04000037 RID: 55
		public uint AddressOfEntryPoint;

		// Token: 0x04000038 RID: 56
		public uint BaseOfCode;

		// Token: 0x04000039 RID: 57
		public uint BaseOfData;

		// Token: 0x0400003A RID: 58
		public uint ImageBase;

		// Token: 0x0400003B RID: 59
		public uint SectionAlignment;

		// Token: 0x0400003C RID: 60
		public uint FileAlignment;

		// Token: 0x0400003D RID: 61
		public ushort MajorOperatingSystemVersion;

		// Token: 0x0400003E RID: 62
		public ushort MinorOperatingSystemVersion;

		// Token: 0x0400003F RID: 63
		public ushort MajorImageVersion;

		// Token: 0x04000040 RID: 64
		public ushort MinorImageVersion;

		// Token: 0x04000041 RID: 65
		public ushort MajorSubsystemVersion;

		// Token: 0x04000042 RID: 66
		public ushort MinorSubsystemVersion;

		// Token: 0x04000043 RID: 67
		public uint Win32VersionValue;

		// Token: 0x04000044 RID: 68
		public uint SizeOfImage;

		// Token: 0x04000045 RID: 69
		public uint SizeOfHeaders;

		// Token: 0x04000046 RID: 70
		public uint CheckSum;

		// Token: 0x04000047 RID: 71
		public ushort Subsystem;

		// Token: 0x04000048 RID: 72
		public ushort DllCharacteristics;

		// Token: 0x04000049 RID: 73
		public uint SizeOfStackReserve;

		// Token: 0x0400004A RID: 74
		public uint SizeOfStackCommit;

		// Token: 0x0400004B RID: 75
		public uint SizeOfHeapReserve;

		// Token: 0x0400004C RID: 76
		public uint SizeOfHeapCommit;

		// Token: 0x0400004D RID: 77
		public uint LoaderFlags;

		// Token: 0x0400004E RID: 78
		public uint NumberOfRvaAndSizes;

		// Token: 0x0400004F RID: 79
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
		public ImageDataDirectory[] DataDirectory;
	}
}
