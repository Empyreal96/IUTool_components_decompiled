using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Microsoft.CompPlat.PkgBldr.Tools
{
	// Token: 0x02000033 RID: 51
	public static class PEFileUtils
	{
		// Token: 0x060001B3 RID: 435 RVA: 0x00007DA4 File Offset: 0x00005FA4
		private static T ReadStruct<T>(BinaryReader br)
		{
			GCHandle gchandle = GCHandle.Alloc(br.ReadBytes(Marshal.SizeOf(typeof(T))), GCHandleType.Pinned);
			T result = (T)((object)Marshal.PtrToStructure(gchandle.AddrOfPinnedObject(), typeof(T)));
			gchandle.Free();
			return result;
		}

		// Token: 0x060001B4 RID: 436 RVA: 0x00007DF0 File Offset: 0x00005FF0
		public static bool IsPE(string path)
		{
			bool result;
			using (BinaryReader binaryReader = new BinaryReader(LongPathFile.OpenRead(path)))
			{
				if (binaryReader.BaseStream.Length < (long)Marshal.SizeOf(typeof(PEFileUtils.IMAGE_DOS_HEADER)))
				{
					result = false;
				}
				else
				{
					PEFileUtils.IMAGE_DOS_HEADER image_DOS_HEADER = PEFileUtils.ReadStruct<PEFileUtils.IMAGE_DOS_HEADER>(binaryReader);
					if (image_DOS_HEADER.e_lfanew < Marshal.SizeOf(typeof(PEFileUtils.IMAGE_DOS_HEADER)))
					{
						result = false;
					}
					else if (image_DOS_HEADER.e_lfanew > 2147483647 - Marshal.SizeOf(typeof(PEFileUtils.IMAGE_NT_HEADERS32)))
					{
						result = false;
					}
					else if (binaryReader.BaseStream.Length < (long)(image_DOS_HEADER.e_lfanew + Marshal.SizeOf(typeof(PEFileUtils.IMAGE_NT_HEADERS32))))
					{
						result = false;
					}
					else
					{
						binaryReader.BaseStream.Seek((long)image_DOS_HEADER.e_lfanew, SeekOrigin.Begin);
						if (binaryReader.ReadUInt32() != PEFileUtils.c_iPESignature)
						{
							result = false;
						}
						else
						{
							PEFileUtils.ReadStruct<PEFileUtils.IMAGE_FILE_HEADER>(binaryReader);
							result = true;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x04000080 RID: 128
		private static uint c_iPESignature = 4660U;

		// Token: 0x02000047 RID: 71
		[StructLayout(LayoutKind.Explicit)]
		public struct IMAGE_DOS_HEADER
		{
			// Token: 0x040000CD RID: 205
			[FieldOffset(60)]
			public int e_lfanew;
		}

		// Token: 0x02000048 RID: 72
		public struct IMAGE_FILE_HEADER
		{
			// Token: 0x040000CE RID: 206
			public ushort Machine;

			// Token: 0x040000CF RID: 207
			public ushort NumberOfSections;

			// Token: 0x040000D0 RID: 208
			public ulong TimeDateStamp;

			// Token: 0x040000D1 RID: 209
			public ulong PointerToSymbolTable;

			// Token: 0x040000D2 RID: 210
			public ulong NumberOfSymbols;

			// Token: 0x040000D3 RID: 211
			public ushort SizeOfOptionalHeader;

			// Token: 0x040000D4 RID: 212
			public ushort Characteristics;
		}

		// Token: 0x02000049 RID: 73
		public struct IMAGE_DATA_DIRECTORY
		{
			// Token: 0x040000D5 RID: 213
			public uint VirtualAddress;

			// Token: 0x040000D6 RID: 214
			public uint Size;
		}

		// Token: 0x0200004A RID: 74
		public struct IMAGE_OPTIONAL_HEADER32
		{
			// Token: 0x040000D7 RID: 215
			public ushort Magic;

			// Token: 0x040000D8 RID: 216
			public byte MajorLinkerVersion;

			// Token: 0x040000D9 RID: 217
			public byte MinorLinkerVersion;

			// Token: 0x040000DA RID: 218
			public uint SizeOfCode;

			// Token: 0x040000DB RID: 219
			public uint SizeOfInitializedData;

			// Token: 0x040000DC RID: 220
			public uint SizeOfUninitializedData;

			// Token: 0x040000DD RID: 221
			public uint AddressOfEntryPoint;

			// Token: 0x040000DE RID: 222
			public uint BaseOfCode;

			// Token: 0x040000DF RID: 223
			public uint BaseOfData;

			// Token: 0x040000E0 RID: 224
			public uint ImageBase;

			// Token: 0x040000E1 RID: 225
			public uint SectionAlignment;

			// Token: 0x040000E2 RID: 226
			public uint FileAlignment;

			// Token: 0x040000E3 RID: 227
			public ushort MajorOperatingSystemVersion;

			// Token: 0x040000E4 RID: 228
			public ushort MinorOperatingSystemVersion;

			// Token: 0x040000E5 RID: 229
			public ushort MajorImageVersion;

			// Token: 0x040000E6 RID: 230
			public ushort MinorImageVersion;

			// Token: 0x040000E7 RID: 231
			public ushort MajorSubsystemVersion;

			// Token: 0x040000E8 RID: 232
			public ushort MinorSubsystemVersion;

			// Token: 0x040000E9 RID: 233
			public uint Win32VersionValue;

			// Token: 0x040000EA RID: 234
			public uint SizeOfImage;

			// Token: 0x040000EB RID: 235
			public uint SizeOfHeaders;

			// Token: 0x040000EC RID: 236
			public uint CheckSum;

			// Token: 0x040000ED RID: 237
			public ushort Subsystem;

			// Token: 0x040000EE RID: 238
			public ushort DllCharacteristics;

			// Token: 0x040000EF RID: 239
			public uint SizeOfStackReserve;

			// Token: 0x040000F0 RID: 240
			public uint SizeOfStackCommit;

			// Token: 0x040000F1 RID: 241
			public uint SizeOfHeapReserve;

			// Token: 0x040000F2 RID: 242
			public uint SizeOfHeapCommit;

			// Token: 0x040000F3 RID: 243
			public uint LoaderFlags;

			// Token: 0x040000F4 RID: 244
			public uint NumberOfRvaAndSizes;
		}

		// Token: 0x0200004B RID: 75
		public struct IMAGE_OPTIONAL_HEADER64
		{
			// Token: 0x040000F5 RID: 245
			public ushort Magic;

			// Token: 0x040000F6 RID: 246
			public byte MajorLinkerVersion;

			// Token: 0x040000F7 RID: 247
			public byte MinorLinkerVersion;

			// Token: 0x040000F8 RID: 248
			public uint SizeOfCode;

			// Token: 0x040000F9 RID: 249
			public uint SizeOfInitializedData;

			// Token: 0x040000FA RID: 250
			public uint SizeOfUninitializedData;

			// Token: 0x040000FB RID: 251
			public uint AddressOfEntryPoint;

			// Token: 0x040000FC RID: 252
			public uint BaseOfCode;

			// Token: 0x040000FD RID: 253
			public ulong ImageBase;

			// Token: 0x040000FE RID: 254
			public uint SectionAlignment;

			// Token: 0x040000FF RID: 255
			public uint FileAlignment;

			// Token: 0x04000100 RID: 256
			public ushort MajorOperatingSystemVersion;

			// Token: 0x04000101 RID: 257
			public ushort MinorOperatingSystemVersion;

			// Token: 0x04000102 RID: 258
			public ushort MajorImageVersion;

			// Token: 0x04000103 RID: 259
			public ushort MinorImageVersion;

			// Token: 0x04000104 RID: 260
			public ushort MajorSubsystemVersion;

			// Token: 0x04000105 RID: 261
			public ushort MinorSubsystemVersion;

			// Token: 0x04000106 RID: 262
			public uint Win32VersionValue;

			// Token: 0x04000107 RID: 263
			public uint SizeOfImage;

			// Token: 0x04000108 RID: 264
			public uint SizeOfHeaders;

			// Token: 0x04000109 RID: 265
			public uint CheckSum;

			// Token: 0x0400010A RID: 266
			public ushort Subsystem;

			// Token: 0x0400010B RID: 267
			public ushort DllCharacteristics;

			// Token: 0x0400010C RID: 268
			public ulong SizeOfStackReserve;

			// Token: 0x0400010D RID: 269
			public ulong SizeOfStackCommit;

			// Token: 0x0400010E RID: 270
			public ulong SizeOfHeapReserve;

			// Token: 0x0400010F RID: 271
			public ulong SizeOfHeapCommit;

			// Token: 0x04000110 RID: 272
			public uint LoaderFlags;

			// Token: 0x04000111 RID: 273
			public uint NumberOfRvaAndSizes;
		}

		// Token: 0x0200004C RID: 76
		public struct IMAGE_NT_HEADERS32
		{
			// Token: 0x04000112 RID: 274
			public uint Signature;

			// Token: 0x04000113 RID: 275
			public PEFileUtils.IMAGE_FILE_HEADER FileHeader;

			// Token: 0x04000114 RID: 276
			public PEFileUtils.IMAGE_OPTIONAL_HEADER32 OptionalHeader;
		}

		// Token: 0x0200004D RID: 77
		public struct IMAGE_NT_HEADERS64
		{
			// Token: 0x04000115 RID: 277
			public uint Signature;

			// Token: 0x04000116 RID: 278
			public PEFileUtils.IMAGE_FILE_HEADER FileHeader;

			// Token: 0x04000117 RID: 279
			public PEFileUtils.IMAGE_OPTIONAL_HEADER64 OptionalHeader;
		}
	}
}
