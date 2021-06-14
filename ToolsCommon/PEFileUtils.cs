using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
	// Token: 0x02000025 RID: 37
	public static class PEFileUtils
	{
		// Token: 0x06000140 RID: 320 RVA: 0x00007B38 File Offset: 0x00005D38
		private static T ReadStruct<T>(BinaryReader br)
		{
			GCHandle gchandle = GCHandle.Alloc(br.ReadBytes(Marshal.SizeOf(typeof(T))), GCHandleType.Pinned);
			T result = (T)((object)Marshal.PtrToStructure(gchandle.AddrOfPinnedObject(), typeof(T)));
			gchandle.Free();
			return result;
		}

		// Token: 0x06000141 RID: 321 RVA: 0x00007B84 File Offset: 0x00005D84
		public static bool IsPE(string path)
		{
			bool result;
			using (BinaryReader binaryReader = new BinaryReader(File.OpenRead(path)))
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

		// Token: 0x04000069 RID: 105
		private static uint c_iPESignature = 4660U;

		// Token: 0x02000062 RID: 98
		[StructLayout(LayoutKind.Explicit)]
		public struct IMAGE_DOS_HEADER
		{
			// Token: 0x04000145 RID: 325
			[FieldOffset(60)]
			public int e_lfanew;
		}

		// Token: 0x02000063 RID: 99
		public struct IMAGE_FILE_HEADER
		{
			// Token: 0x04000146 RID: 326
			public ushort Machine;

			// Token: 0x04000147 RID: 327
			public ushort NumberOfSections;

			// Token: 0x04000148 RID: 328
			public ulong TimeDateStamp;

			// Token: 0x04000149 RID: 329
			public ulong PointerToSymbolTable;

			// Token: 0x0400014A RID: 330
			public ulong NumberOfSymbols;

			// Token: 0x0400014B RID: 331
			public ushort SizeOfOptionalHeader;

			// Token: 0x0400014C RID: 332
			public ushort Characteristics;
		}

		// Token: 0x02000064 RID: 100
		public struct IMAGE_DATA_DIRECTORY
		{
			// Token: 0x0400014D RID: 333
			public uint VirtualAddress;

			// Token: 0x0400014E RID: 334
			public uint Size;
		}

		// Token: 0x02000065 RID: 101
		public struct IMAGE_OPTIONAL_HEADER32
		{
			// Token: 0x0400014F RID: 335
			public ushort Magic;

			// Token: 0x04000150 RID: 336
			public byte MajorLinkerVersion;

			// Token: 0x04000151 RID: 337
			public byte MinorLinkerVersion;

			// Token: 0x04000152 RID: 338
			public uint SizeOfCode;

			// Token: 0x04000153 RID: 339
			public uint SizeOfInitializedData;

			// Token: 0x04000154 RID: 340
			public uint SizeOfUninitializedData;

			// Token: 0x04000155 RID: 341
			public uint AddressOfEntryPoint;

			// Token: 0x04000156 RID: 342
			public uint BaseOfCode;

			// Token: 0x04000157 RID: 343
			public uint BaseOfData;

			// Token: 0x04000158 RID: 344
			public uint ImageBase;

			// Token: 0x04000159 RID: 345
			public uint SectionAlignment;

			// Token: 0x0400015A RID: 346
			public uint FileAlignment;

			// Token: 0x0400015B RID: 347
			public ushort MajorOperatingSystemVersion;

			// Token: 0x0400015C RID: 348
			public ushort MinorOperatingSystemVersion;

			// Token: 0x0400015D RID: 349
			public ushort MajorImageVersion;

			// Token: 0x0400015E RID: 350
			public ushort MinorImageVersion;

			// Token: 0x0400015F RID: 351
			public ushort MajorSubsystemVersion;

			// Token: 0x04000160 RID: 352
			public ushort MinorSubsystemVersion;

			// Token: 0x04000161 RID: 353
			public uint Win32VersionValue;

			// Token: 0x04000162 RID: 354
			public uint SizeOfImage;

			// Token: 0x04000163 RID: 355
			public uint SizeOfHeaders;

			// Token: 0x04000164 RID: 356
			public uint CheckSum;

			// Token: 0x04000165 RID: 357
			public ushort Subsystem;

			// Token: 0x04000166 RID: 358
			public ushort DllCharacteristics;

			// Token: 0x04000167 RID: 359
			public uint SizeOfStackReserve;

			// Token: 0x04000168 RID: 360
			public uint SizeOfStackCommit;

			// Token: 0x04000169 RID: 361
			public uint SizeOfHeapReserve;

			// Token: 0x0400016A RID: 362
			public uint SizeOfHeapCommit;

			// Token: 0x0400016B RID: 363
			public uint LoaderFlags;

			// Token: 0x0400016C RID: 364
			public uint NumberOfRvaAndSizes;
		}

		// Token: 0x02000066 RID: 102
		public struct IMAGE_OPTIONAL_HEADER64
		{
			// Token: 0x0400016D RID: 365
			public ushort Magic;

			// Token: 0x0400016E RID: 366
			public byte MajorLinkerVersion;

			// Token: 0x0400016F RID: 367
			public byte MinorLinkerVersion;

			// Token: 0x04000170 RID: 368
			public uint SizeOfCode;

			// Token: 0x04000171 RID: 369
			public uint SizeOfInitializedData;

			// Token: 0x04000172 RID: 370
			public uint SizeOfUninitializedData;

			// Token: 0x04000173 RID: 371
			public uint AddressOfEntryPoint;

			// Token: 0x04000174 RID: 372
			public uint BaseOfCode;

			// Token: 0x04000175 RID: 373
			public ulong ImageBase;

			// Token: 0x04000176 RID: 374
			public uint SectionAlignment;

			// Token: 0x04000177 RID: 375
			public uint FileAlignment;

			// Token: 0x04000178 RID: 376
			public ushort MajorOperatingSystemVersion;

			// Token: 0x04000179 RID: 377
			public ushort MinorOperatingSystemVersion;

			// Token: 0x0400017A RID: 378
			public ushort MajorImageVersion;

			// Token: 0x0400017B RID: 379
			public ushort MinorImageVersion;

			// Token: 0x0400017C RID: 380
			public ushort MajorSubsystemVersion;

			// Token: 0x0400017D RID: 381
			public ushort MinorSubsystemVersion;

			// Token: 0x0400017E RID: 382
			public uint Win32VersionValue;

			// Token: 0x0400017F RID: 383
			public uint SizeOfImage;

			// Token: 0x04000180 RID: 384
			public uint SizeOfHeaders;

			// Token: 0x04000181 RID: 385
			public uint CheckSum;

			// Token: 0x04000182 RID: 386
			public ushort Subsystem;

			// Token: 0x04000183 RID: 387
			public ushort DllCharacteristics;

			// Token: 0x04000184 RID: 388
			public ulong SizeOfStackReserve;

			// Token: 0x04000185 RID: 389
			public ulong SizeOfStackCommit;

			// Token: 0x04000186 RID: 390
			public ulong SizeOfHeapReserve;

			// Token: 0x04000187 RID: 391
			public ulong SizeOfHeapCommit;

			// Token: 0x04000188 RID: 392
			public uint LoaderFlags;

			// Token: 0x04000189 RID: 393
			public uint NumberOfRvaAndSizes;
		}

		// Token: 0x02000067 RID: 103
		public struct IMAGE_NT_HEADERS32
		{
			// Token: 0x0400018A RID: 394
			public uint Signature;

			// Token: 0x0400018B RID: 395
			public PEFileUtils.IMAGE_FILE_HEADER FileHeader;

			// Token: 0x0400018C RID: 396
			public PEFileUtils.IMAGE_OPTIONAL_HEADER32 OptionalHeader;
		}

		// Token: 0x02000068 RID: 104
		public struct IMAGE_NT_HEADERS64
		{
			// Token: 0x0400018D RID: 397
			public uint Signature;

			// Token: 0x0400018E RID: 398
			public PEFileUtils.IMAGE_FILE_HEADER FileHeader;

			// Token: 0x0400018F RID: 399
			public PEFileUtils.IMAGE_OPTIONAL_HEADER64 OptionalHeader;
		}
	}
}
