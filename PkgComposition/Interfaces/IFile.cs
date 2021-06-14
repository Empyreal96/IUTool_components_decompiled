using System;
using Microsoft.Composition.ToolBox;

namespace Microsoft.Composition.Packaging.Interfaces
{
	// Token: 0x02000009 RID: 9
	public interface IFile
	{
		// Token: 0x1700005D RID: 93
		// (get) Token: 0x0600010C RID: 268
		FileType FileType { get; }

		// Token: 0x1700005E RID: 94
		// (get) Token: 0x0600010D RID: 269
		string DevicePath { get; }

		// Token: 0x1700005F RID: 95
		// (get) Token: 0x0600010E RID: 270
		// (set) Token: 0x0600010F RID: 271
		string CabPath { get; set; }

		// Token: 0x17000060 RID: 96
		// (get) Token: 0x06000110 RID: 272
		// (set) Token: 0x06000111 RID: 273
		string SourcePath { get; set; }

		// Token: 0x17000061 RID: 97
		// (get) Token: 0x06000112 RID: 274
		long Size { get; }

		// Token: 0x17000062 RID: 98
		// (get) Token: 0x06000113 RID: 275
		long CompressedSize { get; }

		// Token: 0x17000063 RID: 99
		// (get) Token: 0x06000114 RID: 276
		long StagedSize { get; }

		// Token: 0x17000064 RID: 100
		// (get) Token: 0x06000115 RID: 277
		string SourcePackage { get; }

		// Token: 0x17000065 RID: 101
		// (get) Token: 0x06000116 RID: 278
		string FileHash { get; }

		// Token: 0x17000066 RID: 102
		// (get) Token: 0x06000117 RID: 279
		bool SignInfoRequired { get; }
	}
}
