using System;
using System.IO;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgCommon
{
	// Token: 0x02000022 RID: 34
	public interface IFileEntry
	{
		// Token: 0x17000050 RID: 80
		// (get) Token: 0x0600016A RID: 362
		FileType FileType { get; }

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x0600016B RID: 363
		string DevicePath { get; }

		// Token: 0x17000052 RID: 82
		// (get) Token: 0x0600016C RID: 364
		string CabPath { get; }

		// Token: 0x17000053 RID: 83
		// (get) Token: 0x0600016D RID: 365
		FileAttributes Attributes { get; }

		// Token: 0x17000054 RID: 84
		// (get) Token: 0x0600016E RID: 366
		ulong Size { get; }

		// Token: 0x17000055 RID: 85
		// (get) Token: 0x0600016F RID: 367
		ulong CompressedSize { get; }

		// Token: 0x17000056 RID: 86
		// (get) Token: 0x06000170 RID: 368
		string SourcePackage { get; }

		// Token: 0x17000057 RID: 87
		// (get) Token: 0x06000171 RID: 369
		string FileHash { get; }

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x06000172 RID: 370
		bool SignInfoRequired { get; }

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x06000173 RID: 371
		string FileArch { get; }
	}
}
