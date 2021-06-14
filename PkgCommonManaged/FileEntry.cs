using System;
using System.IO;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgCommon
{
	// Token: 0x02000017 RID: 23
	public sealed class FileEntry : FileEntryBase, IFileEntry
	{
		// Token: 0x17000035 RID: 53
		// (get) Token: 0x06000113 RID: 275 RVA: 0x00006A63 File Offset: 0x00004C63
		// (set) Token: 0x06000114 RID: 276 RVA: 0x00006A6B File Offset: 0x00004C6B
		public FileAttributes Attributes { get; set; }

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x06000115 RID: 277 RVA: 0x00006A74 File Offset: 0x00004C74
		// (set) Token: 0x06000116 RID: 278 RVA: 0x00006A7C File Offset: 0x00004C7C
		public string SourcePackage { get; private set; }

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x06000117 RID: 279 RVA: 0x00006A85 File Offset: 0x00004C85
		// (set) Token: 0x06000118 RID: 280 RVA: 0x00006A8D File Offset: 0x00004C8D
		public string EmbeddedSigningCategory { get; private set; }

		// Token: 0x06000119 RID: 281 RVA: 0x00006A96 File Offset: 0x00004C96
		public FileEntry()
		{
			base.FileType = FileType.Invalid;
			this.Attributes = PkgConstants.c_defaultAttributes;
			this.SourcePackage = string.Empty;
			this.EmbeddedSigningCategory = "None";
		}

		// Token: 0x0600011A RID: 282 RVA: 0x00006AC8 File Offset: 0x00004CC8
		public FileEntry(IntPtr filePtr) : base(filePtr)
		{
			this.Attributes = NativeMethods.DSMFileEntry_Get_Attributes(filePtr);
			this.SourcePackage = NativeMethods.DSMFileEntry_Get_SourcePackage(filePtr);
			this.EmbeddedSigningCategory = NativeMethods.DSMFileEntry_Get_EmbeddedSigningCategory(filePtr);
			base.Size = NativeMethods.DSMFileEntry_Get_FileSize(filePtr);
			base.CompressedSize = NativeMethods.DSMFileEntry_Get_CompressedFileSize(filePtr);
			base.StagedSize = NativeMethods.DSMFileEntry_Get_StagedFileSize(filePtr);
		}

		// Token: 0x0600011B RID: 283 RVA: 0x00006B24 File Offset: 0x00004D24
		public FileEntry(FileType type, string destination, string source) : this()
		{
			base.FileType = type;
			base.SourcePath = source;
			base.DevicePath = destination;
		}

		// Token: 0x0600011C RID: 284 RVA: 0x00006B41 File Offset: 0x00004D41
		public FileEntry(FileType type, string destination, FileAttributes attributes, string source, string sourcePackage, string embedSignCategory) : this(type, destination, source)
		{
			this.Attributes = attributes;
			this.SourcePackage = ((sourcePackage == null) ? string.Empty : sourcePackage);
			this.EmbeddedSigningCategory = embedSignCategory;
		}
	}
}
