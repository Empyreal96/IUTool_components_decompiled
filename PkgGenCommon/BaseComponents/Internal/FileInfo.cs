using System;
using System.IO;
using Microsoft.WindowsPhone.ImageUpdate.PkgCommon;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents.Internal
{
	// Token: 0x02000070 RID: 112
	public sealed class FileInfo
	{
		// Token: 0x0400018B RID: 395
		public FileType Type;

		// Token: 0x0400018C RID: 396
		public string SourcePath;

		// Token: 0x0400018D RID: 397
		public string DevicePath;

		// Token: 0x0400018E RID: 398
		public FileAttributes Attributes;

		// Token: 0x0400018F RID: 399
		public string EmbeddedSigningCategory = "None";
	}
}
