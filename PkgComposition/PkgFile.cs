using System;
using System.IO;
using System.Security.Cryptography;
using Microsoft.Composition.Packaging.Interfaces;
using Microsoft.Composition.ToolBox;
using Microsoft.Composition.ToolBox.IO;
using Microsoft.Composition.ToolBox.Reflection;

namespace Microsoft.Composition.Packaging
{
	// Token: 0x02000007 RID: 7
	public class PkgFile : StatefulObject, IFile
	{
		// Token: 0x0600009F RID: 159 RVA: 0x00004567 File Offset: 0x00002767
		public PkgFile(FileType fileType, string sourcePath, string destinationPath, string cabPath, string sourcePackage)
		{
			this.LoadFromDisk(fileType, sourcePath, destinationPath, cabPath, sourcePackage, PkgConstants.EmbeddedSigningCategory_None);
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x00004581 File Offset: 0x00002781
		public PkgFile(FileType fileType, string sourcePath, string destinationPath, string cabPath, string sourcePackage, string embeddedSigningCategory)
		{
			this.LoadFromDisk(fileType, sourcePath, destinationPath, cabPath, sourcePackage, embeddedSigningCategory);
		}

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x060000A1 RID: 161 RVA: 0x00004598 File Offset: 0x00002798
		// (set) Token: 0x060000A2 RID: 162 RVA: 0x000045A0 File Offset: 0x000027A0
		public FileType FileType { get; private set; }

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x060000A3 RID: 163 RVA: 0x000045A9 File Offset: 0x000027A9
		// (set) Token: 0x060000A4 RID: 164 RVA: 0x000045B1 File Offset: 0x000027B1
		public string DevicePath { get; private set; }

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x060000A5 RID: 165 RVA: 0x000045BA File Offset: 0x000027BA
		// (set) Token: 0x060000A6 RID: 166 RVA: 0x000045C2 File Offset: 0x000027C2
		public string CabPath { get; set; }

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x060000A7 RID: 167 RVA: 0x000045CB File Offset: 0x000027CB
		// (set) Token: 0x060000A8 RID: 168 RVA: 0x000045D3 File Offset: 0x000027D3
		public string SourcePath { get; set; }

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x060000A9 RID: 169 RVA: 0x000045DC File Offset: 0x000027DC
		// (set) Token: 0x060000AA RID: 170 RVA: 0x000045E4 File Offset: 0x000027E4
		public long Size { get; private set; }

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x060000AB RID: 171 RVA: 0x000045ED File Offset: 0x000027ED
		// (set) Token: 0x060000AC RID: 172 RVA: 0x000045F5 File Offset: 0x000027F5
		public long CompressedSize { get; private set; }

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x060000AD RID: 173 RVA: 0x000045FE File Offset: 0x000027FE
		// (set) Token: 0x060000AE RID: 174 RVA: 0x00004606 File Offset: 0x00002806
		public long StagedSize { get; private set; }

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x060000AF RID: 175 RVA: 0x0000460F File Offset: 0x0000280F
		// (set) Token: 0x060000B0 RID: 176 RVA: 0x00004617 File Offset: 0x00002817
		public string SourcePackage { get; private set; }

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x060000B1 RID: 177 RVA: 0x00004620 File Offset: 0x00002820
		public string FileHash
		{
			get
			{
				string result = string.Empty;
				if (this.SourcePath != null)
				{
					using (FileStream fileStream = FileToolBox.Stream(this.SourcePath, FileAccess.Read))
					{
						result = BitConverter.ToString(new SHA256Managed().ComputeHash(fileStream)).Replace("-", string.Empty);
					}
				}
				return result;
			}
		}

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x060000B2 RID: 178 RVA: 0x00004688 File Offset: 0x00002888
		// (set) Token: 0x060000B3 RID: 179 RVA: 0x00004690 File Offset: 0x00002890
		public bool SignInfoRequired { get; set; }

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x060000B4 RID: 180 RVA: 0x00004699 File Offset: 0x00002899
		// (set) Token: 0x060000B5 RID: 181 RVA: 0x000046A1 File Offset: 0x000028A1
		public string EmbeddedSigningCategory { get; private set; }

		// Token: 0x060000B6 RID: 182 RVA: 0x000046AC File Offset: 0x000028AC
		public void LoadFromDisk(FileType fileType, string sourcePath, string devicePath, string cabPath, string sourcePackage, string embeddedSigningCategory)
		{
			if (!FileToolBox.Exists(sourcePath))
			{
				throw new PkgException("PkgFile::LoadfromDisk: File {0} doesn't exist", new object[]
				{
					sourcePath
				});
			}
			if (string.IsNullOrEmpty(devicePath))
			{
				throw new PkgException("PkgFile::LoadfromDisk: DevicePath can't be empty");
			}
			if (devicePath.Contains("\\.\\") || devicePath.Contains("\\..\\") || devicePath.Contains("\\\\"))
			{
				throw new PkgException("PkgFile::LoadfromDisk: DevicePath can't contain  \\.\\ or \\..\\ or \\\\");
			}
			this.FileType = fileType;
			this.SourcePath = sourcePath;
			this.DevicePath = devicePath;
			this.CabPath = cabPath;
			this.SourcePackage = sourcePackage;
			this.SignInfoRequired = false;
			this.EmbeddedSigningCategory = embeddedSigningCategory;
			ulong size;
			ulong stagedSize;
			ulong compressedSize;
			FileToolBox.ImagingSizes(sourcePath, out size, out stagedSize, out compressedSize);
			this.Size = (long)size;
			this.StagedSize = (long)stagedSize;
			this.CompressedSize = (long)compressedSize;
		}
	}
}
