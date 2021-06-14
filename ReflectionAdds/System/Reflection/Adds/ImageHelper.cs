using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace System.Reflection.Adds
{
	// Token: 0x0200000E RID: 14
	internal class ImageHelper
	{
		// Token: 0x06000077 RID: 119 RVA: 0x00003338 File Offset: 0x00001538
		public ImageHelper(IntPtr baseAddress, long lengthBytes)
		{
			this._baseAddress = baseAddress;
			this._lengthBytes = lengthBytes;
			ImageHelper.IMAGE_DOS_HEADER image_DOS_HEADER = this.MarshalAt<ImageHelper.IMAGE_DOS_HEADER>(0U);
			bool flag = !image_DOS_HEADER.IsValid;
			if (flag)
			{
				throw new ArgumentException(Resources.InvalidFileFormat);
			}
			this._idx = image_DOS_HEADER.e_lfanew;
			ImageHelper.IMAGE_NT_HEADERS_HELPER image_NT_HEADERS_HELPER = this.MarshalAt<ImageHelper.IMAGE_NT_HEADERS_HELPER>(this._idx);
			bool flag2 = !image_NT_HEADERS_HELPER.IsValid;
			if (flag2)
			{
				throw new ArgumentException(Resources.InvalidFileFormat);
			}
			bool flag3 = image_NT_HEADERS_HELPER.Magic == 267;
			if (flag3)
			{
				this.ImageType = ImageType.Pe32bit;
				ImageHelper.IMAGE_NT_HEADERS_32 image_NT_HEADERS_ = this.MarshalAt<ImageHelper.IMAGE_NT_HEADERS_32>(this._idx);
				this._idxSectionStart = this._idx + (uint)Marshal.SizeOf(typeof(ImageHelper.IMAGE_NT_HEADERS_32));
				this._numSections = (uint)image_NT_HEADERS_.FileHeader.NumberOfSections;
				this._clrHeaderRva = image_NT_HEADERS_.OptionalHeader.ClrHeaderTable.VirtualAddress;
			}
			else
			{
				bool flag4 = image_NT_HEADERS_HELPER.Magic == 523;
				if (!flag4)
				{
					throw new ArgumentException(Resources.UnsupportedImageType);
				}
				this.ImageType = ImageType.Pe64bit;
				ImageHelper.IMAGE_NT_HEADERS_64 image_NT_HEADERS_2 = this.MarshalAt<ImageHelper.IMAGE_NT_HEADERS_64>(this._idx);
				this._idxSectionStart = this._idx + (uint)Marshal.SizeOf(typeof(ImageHelper.IMAGE_NT_HEADERS_64));
				this._numSections = (uint)image_NT_HEADERS_2.FileHeader.NumberOfSections;
				this._clrHeaderRva = image_NT_HEADERS_2.OptionalHeader.ClrHeaderTable.VirtualAddress;
			}
		}

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x06000078 RID: 120 RVA: 0x000034A9 File Offset: 0x000016A9
		// (set) Token: 0x06000079 RID: 121 RVA: 0x000034B1 File Offset: 0x000016B1
		public ImageType ImageType { get; private set; }

		// Token: 0x0600007A RID: 122 RVA: 0x000034BC File Offset: 0x000016BC
		public IntPtr GetResourcesSectionStart()
		{
			ImageHelper.IMAGE_COR20_HEADER cor20Header = this.GetCor20Header();
			uint virtualAddress = cor20Header.Resources.VirtualAddress;
			return this.ResolveRva((long)((ulong)virtualAddress));
		}

		// Token: 0x0600007B RID: 123 RVA: 0x000034EC File Offset: 0x000016EC
		internal ImageHelper.IMAGE_COR20_HEADER GetCor20Header()
		{
			IntPtr ptr = this.ResolveRva((long)((ulong)this._clrHeaderRva));
			return (ImageHelper.IMAGE_COR20_HEADER)Marshal.PtrToStructure(ptr, typeof(ImageHelper.IMAGE_COR20_HEADER));
		}

		// Token: 0x0600007C RID: 124 RVA: 0x00003524 File Offset: 0x00001724
		public Token GetEntryPointToken()
		{
			ImageHelper.IMAGE_COR20_HEADER cor20Header = this.GetCor20Header();
			bool flag = (cor20Header.Flags & ImageHelper.CorHdrNumericDefines.COMIMAGE_FLAGS_NATIVE_ENTRYPOINT) > (ImageHelper.CorHdrNumericDefines)0U;
			Token result;
			if (flag)
			{
				result = Token.Nil;
			}
			else
			{
				uint entryPoint = cor20Header.EntryPoint;
				result = new Token(entryPoint);
			}
			return result;
		}

		// Token: 0x0600007D RID: 125 RVA: 0x00003564 File Offset: 0x00001764
		public IntPtr ResolveRva(long rva)
		{
			uint num = this._idxSectionStart;
			int num2 = 0;
			while ((long)num2 < (long)((ulong)this._numSections))
			{
				ImageHelper.IMAGE_SECTION_HEADER image_SECTION_HEADER = this.MarshalAt<ImageHelper.IMAGE_SECTION_HEADER>(num);
				bool flag = rva >= (long)((ulong)image_SECTION_HEADER.VirtualAddress) && rva < (long)((ulong)(image_SECTION_HEADER.VirtualAddress + image_SECTION_HEADER.SizeOfRawData));
				if (flag)
				{
					long value = this._baseAddress.ToInt64() + rva - (long)((ulong)image_SECTION_HEADER.VirtualAddress) + (long)((ulong)image_SECTION_HEADER.PointerToRawData);
					return new IntPtr(value);
				}
				num += (uint)Marshal.SizeOf(typeof(ImageHelper.IMAGE_SECTION_HEADER));
				num2++;
			}
			return IntPtr.Zero;
		}

		// Token: 0x0600007E RID: 126 RVA: 0x00003610 File Offset: 0x00001810
		internal T MarshalAt<T>(uint offset)
		{
			long num = this._baseAddress.ToInt64();
			long num2 = num + this._lengthBytes;
			int num3 = Marshal.SizeOf(typeof(T));
			bool flag = (ulong)offset + (ulong)((long)num3) > (ulong)num2;
			if (flag)
			{
				throw new InvalidOperationException(Resources.CorruptImage);
			}
			IntPtr ptr = new IntPtr(num + (long)((ulong)offset));
			return (T)((object)Marshal.PtrToStructure(ptr, typeof(T)));
		}

		// Token: 0x0400003F RID: 63
		private readonly IntPtr _baseAddress;

		// Token: 0x04000040 RID: 64
		private readonly long _lengthBytes;

		// Token: 0x04000041 RID: 65
		private readonly uint _idx;

		// Token: 0x04000042 RID: 66
		private readonly uint _idxSectionStart;

		// Token: 0x04000043 RID: 67
		private readonly uint _numSections;

		// Token: 0x04000044 RID: 68
		private readonly uint _clrHeaderRva;

		// Token: 0x02000027 RID: 39
		[SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
		[StructLayout(LayoutKind.Explicit)]
		public class IMAGE_DOS_HEADER
		{
			// Token: 0x1700006D RID: 109
			// (get) Token: 0x0600011F RID: 287 RVA: 0x0000549C File Offset: 0x0000369C
			public bool IsValid
			{
				get
				{
					return this.e_magic == 23117;
				}
			}

			// Token: 0x04000091 RID: 145
			[FieldOffset(0)]
			public short e_magic;

			// Token: 0x04000092 RID: 146
			[FieldOffset(60)]
			public uint e_lfanew;
		}

		// Token: 0x02000028 RID: 40
		[SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
		[StructLayout(LayoutKind.Sequential)]
		internal class IMAGE_FILE_HEADER
		{
			// Token: 0x04000093 RID: 147
			public short Machine;

			// Token: 0x04000094 RID: 148
			public short NumberOfSections;

			// Token: 0x04000095 RID: 149
			public uint TimeDateStamp;

			// Token: 0x04000096 RID: 150
			public uint PointerToSymbolTable;

			// Token: 0x04000097 RID: 151
			public uint NumberOfSymbols;

			// Token: 0x04000098 RID: 152
			public short SizeOfOptionalHeader;

			// Token: 0x04000099 RID: 153
			public short Characteristics;
		}

		// Token: 0x02000029 RID: 41
		[StructLayout(LayoutKind.Sequential)]
		internal class IMAGE_NT_HEADERS_HELPER
		{
			// Token: 0x1700006E RID: 110
			// (get) Token: 0x06000122 RID: 290 RVA: 0x000054BC File Offset: 0x000036BC
			public bool IsValid
			{
				get
				{
					return this.Signature == 17744U;
				}
			}

			// Token: 0x0400009A RID: 154
			public uint Signature;

			// Token: 0x0400009B RID: 155
			public ImageHelper.IMAGE_FILE_HEADER FileHeader;

			// Token: 0x0400009C RID: 156
			public ushort Magic;
		}

		// Token: 0x0200002A RID: 42
		[StructLayout(LayoutKind.Sequential)]
		internal class IMAGE_NT_HEADERS_32
		{
			// Token: 0x0400009D RID: 157
			public uint Signature;

			// Token: 0x0400009E RID: 158
			public ImageHelper.IMAGE_FILE_HEADER FileHeader;

			// Token: 0x0400009F RID: 159
			public ImageHelper.IMAGE_OPTIONAL_HEADER_32 OptionalHeader;
		}

		// Token: 0x0200002B RID: 43
		[StructLayout(LayoutKind.Sequential)]
		internal class IMAGE_NT_HEADERS_64
		{
			// Token: 0x040000A0 RID: 160
			public uint Signature;

			// Token: 0x040000A1 RID: 161
			public ImageHelper.IMAGE_FILE_HEADER FileHeader;

			// Token: 0x040000A2 RID: 162
			public ImageHelper.IMAGE_OPTIONAL_HEADER_64 OptionalHeader;
		}

		// Token: 0x0200002C RID: 44
		[StructLayout(LayoutKind.Sequential)]
		private class IMAGE_SECTION_HEADER
		{
			// Token: 0x040000A3 RID: 163
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
			public string name;

			// Token: 0x040000A4 RID: 164
			public uint union;

			// Token: 0x040000A5 RID: 165
			public uint VirtualAddress;

			// Token: 0x040000A6 RID: 166
			public uint SizeOfRawData;

			// Token: 0x040000A7 RID: 167
			public uint PointerToRawData;

			// Token: 0x040000A8 RID: 168
			public uint PointerToRelocations;

			// Token: 0x040000A9 RID: 169
			public uint PointerToLinenumbers;

			// Token: 0x040000AA RID: 170
			public ushort NumberOfRelocations;

			// Token: 0x040000AB RID: 171
			public ushort NumberOfLinenumbers;

			// Token: 0x040000AC RID: 172
			public uint Characteristics;
		}

		// Token: 0x0200002D RID: 45
		[StructLayout(LayoutKind.Sequential)]
		internal class IMAGE_OPTIONAL_HEADER_32
		{
			// Token: 0x040000AD RID: 173
			public ushort Magic;

			// Token: 0x040000AE RID: 174
			public byte MajorLinkerVersion;

			// Token: 0x040000AF RID: 175
			public byte MinorLinkerVersion;

			// Token: 0x040000B0 RID: 176
			public uint SizeOfCode;

			// Token: 0x040000B1 RID: 177
			public uint SizeOfInitializedData;

			// Token: 0x040000B2 RID: 178
			public uint SizeOfUninitializedData;

			// Token: 0x040000B3 RID: 179
			public uint AddressOfEntryPoint;

			// Token: 0x040000B4 RID: 180
			public uint BaseOfCode;

			// Token: 0x040000B5 RID: 181
			public uint BaseOfData;

			// Token: 0x040000B6 RID: 182
			public uint ImageBase;

			// Token: 0x040000B7 RID: 183
			public uint SectionAlignment;

			// Token: 0x040000B8 RID: 184
			public uint FileAlignment;

			// Token: 0x040000B9 RID: 185
			public ushort MajorOperatingSystemVersion;

			// Token: 0x040000BA RID: 186
			public ushort MinorOperatingSystemVersion;

			// Token: 0x040000BB RID: 187
			public ushort MajorImageVersion;

			// Token: 0x040000BC RID: 188
			public ushort MinorImageVersion;

			// Token: 0x040000BD RID: 189
			public ushort MajorSubsystemVersion;

			// Token: 0x040000BE RID: 190
			public ushort MinorSubsystemVersion;

			// Token: 0x040000BF RID: 191
			public uint Win32VersionValue;

			// Token: 0x040000C0 RID: 192
			public uint SizeOfImage;

			// Token: 0x040000C1 RID: 193
			public uint SizeOfHeaders;

			// Token: 0x040000C2 RID: 194
			public uint CheckSum;

			// Token: 0x040000C3 RID: 195
			public ushort Subsystem;

			// Token: 0x040000C4 RID: 196
			public ushort DllCharacteristics;

			// Token: 0x040000C5 RID: 197
			public uint SizeOfStackReserve;

			// Token: 0x040000C6 RID: 198
			public uint SizeOfStackCommit;

			// Token: 0x040000C7 RID: 199
			public uint SizeOfHeapReserve;

			// Token: 0x040000C8 RID: 200
			public uint SizeOfHeapCommit;

			// Token: 0x040000C9 RID: 201
			public uint LoaderFlags;

			// Token: 0x040000CA RID: 202
			public uint NumberOfRvaAndSizes;

			// Token: 0x040000CB RID: 203
			public ImageHelper.IMAGE_DATA_DIRECTORY ExportTable;

			// Token: 0x040000CC RID: 204
			public ImageHelper.IMAGE_DATA_DIRECTORY ImportTable;

			// Token: 0x040000CD RID: 205
			public ImageHelper.IMAGE_DATA_DIRECTORY ResourceTable;

			// Token: 0x040000CE RID: 206
			public ImageHelper.IMAGE_DATA_DIRECTORY ExceptionTable;

			// Token: 0x040000CF RID: 207
			public ImageHelper.IMAGE_DATA_DIRECTORY CertificateTable;

			// Token: 0x040000D0 RID: 208
			public ImageHelper.IMAGE_DATA_DIRECTORY BaseRelocationTable;

			// Token: 0x040000D1 RID: 209
			public ImageHelper.IMAGE_DATA_DIRECTORY DebugData;

			// Token: 0x040000D2 RID: 210
			public ImageHelper.IMAGE_DATA_DIRECTORY ArchitectureData;

			// Token: 0x040000D3 RID: 211
			public ImageHelper.IMAGE_DATA_DIRECTORY GlobalPointer;

			// Token: 0x040000D4 RID: 212
			public ImageHelper.IMAGE_DATA_DIRECTORY TlsTable;

			// Token: 0x040000D5 RID: 213
			public ImageHelper.IMAGE_DATA_DIRECTORY LoadConfigTable;

			// Token: 0x040000D6 RID: 214
			public ImageHelper.IMAGE_DATA_DIRECTORY BoundImportTable;

			// Token: 0x040000D7 RID: 215
			public ImageHelper.IMAGE_DATA_DIRECTORY ImportAddressTable;

			// Token: 0x040000D8 RID: 216
			public ImageHelper.IMAGE_DATA_DIRECTORY DelayImportTable;

			// Token: 0x040000D9 RID: 217
			public ImageHelper.IMAGE_DATA_DIRECTORY ClrHeaderTable;

			// Token: 0x040000DA RID: 218
			public ImageHelper.IMAGE_DATA_DIRECTORY Reserved;
		}

		// Token: 0x0200002E RID: 46
		[StructLayout(LayoutKind.Sequential)]
		internal class IMAGE_OPTIONAL_HEADER_64
		{
			// Token: 0x040000DB RID: 219
			public ushort Magic;

			// Token: 0x040000DC RID: 220
			public byte MajorLinkerVersion;

			// Token: 0x040000DD RID: 221
			public byte MinorLinkerVersion;

			// Token: 0x040000DE RID: 222
			public uint SizeOfCode;

			// Token: 0x040000DF RID: 223
			public uint SizeOfInitializedData;

			// Token: 0x040000E0 RID: 224
			public uint SizeOfUninitializedData;

			// Token: 0x040000E1 RID: 225
			public uint AddressOfEntryPoint;

			// Token: 0x040000E2 RID: 226
			public uint BaseOfCode;

			// Token: 0x040000E3 RID: 227
			public ulong ImageBase;

			// Token: 0x040000E4 RID: 228
			public uint SectionAlignment;

			// Token: 0x040000E5 RID: 229
			public uint FileAlignment;

			// Token: 0x040000E6 RID: 230
			public ushort MajorOperatingSystemVersion;

			// Token: 0x040000E7 RID: 231
			public ushort MinorOperatingSystemVersion;

			// Token: 0x040000E8 RID: 232
			public ushort MajorImageVersion;

			// Token: 0x040000E9 RID: 233
			public ushort MinorImageVersion;

			// Token: 0x040000EA RID: 234
			public ushort MajorSubsystemVersion;

			// Token: 0x040000EB RID: 235
			public ushort MinorSubsystemVersion;

			// Token: 0x040000EC RID: 236
			public uint Win32VersionValue;

			// Token: 0x040000ED RID: 237
			public uint SizeOfImage;

			// Token: 0x040000EE RID: 238
			public uint SizeOfHeaders;

			// Token: 0x040000EF RID: 239
			public uint CheckSum;

			// Token: 0x040000F0 RID: 240
			public ushort Subsystem;

			// Token: 0x040000F1 RID: 241
			public ushort DllCharacteristics;

			// Token: 0x040000F2 RID: 242
			public ulong SizeOfStackReserve;

			// Token: 0x040000F3 RID: 243
			public ulong SizeOfStackCommit;

			// Token: 0x040000F4 RID: 244
			public ulong SizeOfHeapReserve;

			// Token: 0x040000F5 RID: 245
			public ulong SizeOfHeapCommit;

			// Token: 0x040000F6 RID: 246
			public uint LoaderFlags;

			// Token: 0x040000F7 RID: 247
			public uint NumberOfRvaAndSizes;

			// Token: 0x040000F8 RID: 248
			public ImageHelper.IMAGE_DATA_DIRECTORY ExportTable;

			// Token: 0x040000F9 RID: 249
			public ImageHelper.IMAGE_DATA_DIRECTORY ImportTable;

			// Token: 0x040000FA RID: 250
			public ImageHelper.IMAGE_DATA_DIRECTORY ResourceTable;

			// Token: 0x040000FB RID: 251
			public ImageHelper.IMAGE_DATA_DIRECTORY ExceptionTable;

			// Token: 0x040000FC RID: 252
			public ImageHelper.IMAGE_DATA_DIRECTORY CertificateTable;

			// Token: 0x040000FD RID: 253
			public ImageHelper.IMAGE_DATA_DIRECTORY BaseRelocationTable;

			// Token: 0x040000FE RID: 254
			public ImageHelper.IMAGE_DATA_DIRECTORY DebugData;

			// Token: 0x040000FF RID: 255
			public ImageHelper.IMAGE_DATA_DIRECTORY ArchitectureData;

			// Token: 0x04000100 RID: 256
			public ImageHelper.IMAGE_DATA_DIRECTORY GlobalPointer;

			// Token: 0x04000101 RID: 257
			public ImageHelper.IMAGE_DATA_DIRECTORY TlsTable;

			// Token: 0x04000102 RID: 258
			public ImageHelper.IMAGE_DATA_DIRECTORY LoadConfigTable;

			// Token: 0x04000103 RID: 259
			public ImageHelper.IMAGE_DATA_DIRECTORY BoundImportTable;

			// Token: 0x04000104 RID: 260
			public ImageHelper.IMAGE_DATA_DIRECTORY ImportAddressTable;

			// Token: 0x04000105 RID: 261
			public ImageHelper.IMAGE_DATA_DIRECTORY DelayImportTable;

			// Token: 0x04000106 RID: 262
			public ImageHelper.IMAGE_DATA_DIRECTORY ClrHeaderTable;

			// Token: 0x04000107 RID: 263
			public ImageHelper.IMAGE_DATA_DIRECTORY Reserved;
		}

		// Token: 0x0200002F RID: 47
		[StructLayout(LayoutKind.Sequential)]
		internal class IMAGE_DATA_DIRECTORY
		{
			// Token: 0x04000108 RID: 264
			public uint VirtualAddress;

			// Token: 0x04000109 RID: 265
			public uint Size;
		}

		// Token: 0x02000030 RID: 48
		internal enum CorHdrNumericDefines : uint
		{
			// Token: 0x0400010B RID: 267
			COMIMAGE_FLAGS_ILONLY = 1U,
			// Token: 0x0400010C RID: 268
			COMIMAGE_FLAGS_32BITREQUIRED,
			// Token: 0x0400010D RID: 269
			COMIMAGE_FLAGS_IL_LIBRARY = 4U,
			// Token: 0x0400010E RID: 270
			COMIMAGE_FLAGS_STRONGNAMESIGNED = 8U,
			// Token: 0x0400010F RID: 271
			COMIMAGE_FLAGS_NATIVE_ENTRYPOINT = 16U,
			// Token: 0x04000110 RID: 272
			COMIMAGE_FLAGS_TRACKDEBUGDATA = 65536U,
			// Token: 0x04000111 RID: 273
			COMIMAGE_FLAGS_ISIBCOPTIMIZED = 131072U
		}

		// Token: 0x02000031 RID: 49
		[StructLayout(LayoutKind.Sequential)]
		internal class IMAGE_COR20_HEADER
		{
			// Token: 0x04000112 RID: 274
			public uint cb;

			// Token: 0x04000113 RID: 275
			public ushort MajorRuntimeVersion;

			// Token: 0x04000114 RID: 276
			public ushort MinorRuntimeVersion;

			// Token: 0x04000115 RID: 277
			public ImageHelper.IMAGE_DATA_DIRECTORY MetaData;

			// Token: 0x04000116 RID: 278
			public ImageHelper.CorHdrNumericDefines Flags;

			// Token: 0x04000117 RID: 279
			public uint EntryPoint;

			// Token: 0x04000118 RID: 280
			public ImageHelper.IMAGE_DATA_DIRECTORY Resources;

			// Token: 0x04000119 RID: 281
			public ImageHelper.IMAGE_DATA_DIRECTORY StrongNameSignature;

			// Token: 0x0400011A RID: 282
			public ImageHelper.IMAGE_DATA_DIRECTORY CodeManagerTable;

			// Token: 0x0400011B RID: 283
			public ImageHelper.IMAGE_DATA_DIRECTORY VTableFixups;

			// Token: 0x0400011C RID: 284
			public ImageHelper.IMAGE_DATA_DIRECTORY ExportAddressTableJumps;

			// Token: 0x0400011D RID: 285
			public ImageHelper.IMAGE_DATA_DIRECTORY ManagedNativeHeader;
		}
	}
}
