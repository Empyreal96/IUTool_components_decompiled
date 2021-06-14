using System;
using System.IO;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgCommon
{
	// Token: 0x02000016 RID: 22
	public class FileEntryBase
	{
		// Token: 0x1700002B RID: 43
		// (get) Token: 0x060000FA RID: 250 RVA: 0x0000678C File Offset: 0x0000498C
		// (set) Token: 0x060000FB RID: 251 RVA: 0x00006794 File Offset: 0x00004994
		public FileType FileType { get; set; }

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x060000FC RID: 252 RVA: 0x0000679D File Offset: 0x0000499D
		// (set) Token: 0x060000FD RID: 253 RVA: 0x000067A5 File Offset: 0x000049A5
		public string SourcePath
		{
			get
			{
				return this._sourcePath;
			}
			set
			{
				this._sourcePath = value;
			}
		}

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x060000FE RID: 254 RVA: 0x000067AE File Offset: 0x000049AE
		// (set) Token: 0x060000FF RID: 255 RVA: 0x000067C9 File Offset: 0x000049C9
		public string FileHash
		{
			get
			{
				if (string.IsNullOrEmpty(this._fileHash))
				{
					return "";
				}
				return this._fileHash;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					this._fileHash = "";
					return;
				}
				this._fileHash = value;
			}
		}

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x06000100 RID: 256 RVA: 0x000067E6 File Offset: 0x000049E6
		// (set) Token: 0x06000101 RID: 257 RVA: 0x000067EE File Offset: 0x000049EE
		public bool SignInfoRequired { get; set; }

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x06000102 RID: 258 RVA: 0x000067F7 File Offset: 0x000049F7
		// (set) Token: 0x06000103 RID: 259 RVA: 0x000067FF File Offset: 0x000049FF
		public string DevicePath { get; set; }

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x06000104 RID: 260 RVA: 0x00006808 File Offset: 0x00004A08
		// (set) Token: 0x06000105 RID: 261 RVA: 0x00006810 File Offset: 0x00004A10
		public string CabPath { get; set; }

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x06000106 RID: 262 RVA: 0x00006819 File Offset: 0x00004A19
		// (set) Token: 0x06000107 RID: 263 RVA: 0x00006821 File Offset: 0x00004A21
		public string FileArch { get; set; }

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x06000108 RID: 264 RVA: 0x0000682A File Offset: 0x00004A2A
		// (set) Token: 0x06000109 RID: 265 RVA: 0x00006838 File Offset: 0x00004A38
		public ulong Size
		{
			get
			{
				this.CalculateFileSizes();
				return this._size;
			}
			set
			{
				this._size = value;
			}
		}

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x0600010A RID: 266 RVA: 0x00006841 File Offset: 0x00004A41
		// (set) Token: 0x0600010B RID: 267 RVA: 0x0000684F File Offset: 0x00004A4F
		public ulong CompressedSize
		{
			get
			{
				this.CalculateFileSizes();
				return this._compressedSize;
			}
			set
			{
				this._compressedSize = value;
			}
		}

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x0600010C RID: 268 RVA: 0x00006858 File Offset: 0x00004A58
		// (set) Token: 0x0600010D RID: 269 RVA: 0x00006866 File Offset: 0x00004A66
		public ulong StagedSize
		{
			get
			{
				this.CalculateFileSizes();
				return this._stagedSize;
			}
			set
			{
				this._stagedSize = value;
			}
		}

		// Token: 0x0600010E RID: 270 RVA: 0x00006870 File Offset: 0x00004A70
		public void Validate()
		{
			if (string.IsNullOrEmpty(this.DevicePath))
			{
				throw new PackageException("DevicePath not specified");
			}
			if (!Path.IsPathRooted(this.DevicePath))
			{
				throw new PackageException("DevicePath must be absolutepath");
			}
			if (string.IsNullOrEmpty(this.CabPath))
			{
				throw new PackageException("CabPath not specified");
			}
		}

		// Token: 0x0600010F RID: 271 RVA: 0x000068C8 File Offset: 0x00004AC8
		public void BuildSourcePath(string rootDir, BuildPathOption option, bool installed)
		{
			if (option == BuildPathOption.UseDevicePath)
			{
				this.SourcePath = Path.Combine(rootDir, this.DevicePath.TrimStart(new char[]
				{
					'\\'
				}));
				return;
			}
			if (option == BuildPathOption.UseCabPath)
			{
				this.SourcePath = Path.Combine(rootDir, this.CabPath.TrimStart(new char[]
				{
					'\\'
				}));
				return;
			}
			throw new PackageException("Unexpected option '{0}' specified in BuigldSourcePaths", new object[]
			{
				option
			});
		}

		// Token: 0x06000110 RID: 272 RVA: 0x0000693C File Offset: 0x00004B3C
		public void CalculateFileSizes()
		{
			if (this._size != 0UL || this._stagedSize != 0UL || this._compressedSize != 0UL)
			{
				return;
			}
			if (!LongPathFile.Exists(this.SourcePath))
			{
				if (string.Equals(Path.GetExtension(this.SourcePath), ".bin", StringComparison.OrdinalIgnoreCase) || this.FileType == FileType.Manifest || this.FileType == FileType.Catalog)
				{
					return;
				}
				throw new PackageException(string.Format("Couldn't get file sizes for file '{0}' since it does not exist", this.SourcePath));
			}
			else
			{
				ulong size = 0UL;
				ulong stagedSize = 0UL;
				ulong compressedSize = 0UL;
				uint num = NativeMethods.IU_GetStagedAndCompressedSize(this.SourcePath, out size, out stagedSize, out compressedSize);
				if (num != 0U)
				{
					throw new PackageException("Failed IU_GetStagedAndCompressedSize with error '{0}'", new object[]
					{
						num
					});
				}
				this._size = size;
				this._stagedSize = stagedSize;
				this._compressedSize = compressedSize;
				return;
			}
		}

		// Token: 0x06000111 RID: 273 RVA: 0x000069FE File Offset: 0x00004BFE
		protected FileEntryBase()
		{
		}

		// Token: 0x06000112 RID: 274 RVA: 0x00006A14 File Offset: 0x00004C14
		protected FileEntryBase(IntPtr filePtr) : this()
		{
			this.FileType = NativeMethods.FileEntryBase_Get_FileType(filePtr);
			this.DevicePath = NativeMethods.FileEntryBase_Get_DevicePath(filePtr);
			this.CabPath = NativeMethods.FileEntryBase_Get_CabPath(filePtr);
			this.FileHash = NativeMethods.FileEntryBase_Get_FileHash(filePtr);
			this.SignInfoRequired = NativeMethods.FileEntryBase_Get_SignInfo(filePtr);
		}

		// Token: 0x04000021 RID: 33
		private string _sourcePath;

		// Token: 0x04000022 RID: 34
		private string _fileHash = string.Empty;

		// Token: 0x04000027 RID: 39
		private ulong _size;

		// Token: 0x04000028 RID: 40
		private ulong _compressedSize;

		// Token: 0x04000029 RID: 41
		private ulong _stagedSize;
	}
}
