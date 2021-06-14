using System;
using System.Collections.Generic;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgCommon
{
	// Token: 0x02000024 RID: 36
	public interface IPkgInfo
	{
		// Token: 0x1700005A RID: 90
		// (get) Token: 0x06000176 RID: 374
		PackageType Type { get; }

		// Token: 0x1700005B RID: 91
		// (get) Token: 0x06000177 RID: 375
		PackageStyle Style { get; }

		// Token: 0x1700005C RID: 92
		// (get) Token: 0x06000178 RID: 376
		string Name { get; }

		// Token: 0x1700005D RID: 93
		// (get) Token: 0x06000179 RID: 377
		// (set) Token: 0x0600017A RID: 378
		string PackageName { get; set; }

		// Token: 0x1700005E RID: 94
		// (get) Token: 0x0600017B RID: 379
		string Owner { get; }

		// Token: 0x1700005F RID: 95
		// (get) Token: 0x0600017C RID: 380
		string Component { get; }

		// Token: 0x17000060 RID: 96
		// (get) Token: 0x0600017D RID: 381
		string SubComponent { get; }

		// Token: 0x17000061 RID: 97
		// (get) Token: 0x0600017E RID: 382
		string Partition { get; }

		// Token: 0x17000062 RID: 98
		// (get) Token: 0x0600017F RID: 383
		string Platform { get; }

		// Token: 0x17000063 RID: 99
		// (get) Token: 0x06000180 RID: 384
		string PublicKey { get; }

		// Token: 0x17000064 RID: 100
		// (get) Token: 0x06000181 RID: 385
		VersionInfo Version { get; }

		// Token: 0x17000065 RID: 101
		// (get) Token: 0x06000182 RID: 386
		OwnerType OwnerType { get; }

		// Token: 0x17000066 RID: 102
		// (get) Token: 0x06000183 RID: 387
		ReleaseType ReleaseType { get; }

		// Token: 0x17000067 RID: 103
		// (get) Token: 0x06000184 RID: 388
		BuildType BuildType { get; }

		// Token: 0x17000068 RID: 104
		// (get) Token: 0x06000185 RID: 389
		CpuId CpuType { get; }

		// Token: 0x17000069 RID: 105
		// (get) Token: 0x06000186 RID: 390
		CpuId ComplexCpuType { get; }

		// Token: 0x1700006A RID: 106
		// (get) Token: 0x06000187 RID: 391
		string Culture { get; }

		// Token: 0x1700006B RID: 107
		// (get) Token: 0x06000188 RID: 392
		string Resolution { get; }

		// Token: 0x1700006C RID: 108
		// (get) Token: 0x06000189 RID: 393
		bool IsBinaryPartition { get; }

		// Token: 0x1700006D RID: 109
		// (get) Token: 0x0600018A RID: 394
		bool IsWow { get; }

		// Token: 0x1700006E RID: 110
		// (get) Token: 0x0600018B RID: 395
		VersionInfo PrevVersion { get; }

		// Token: 0x1700006F RID: 111
		// (get) Token: 0x0600018C RID: 396
		byte[] PrevDsmHash { get; }

		// Token: 0x17000070 RID: 112
		// (get) Token: 0x0600018D RID: 397
		string BuildString { get; }

		// Token: 0x17000071 RID: 113
		// (get) Token: 0x0600018E RID: 398
		string GroupingKey { get; }

		// Token: 0x17000072 RID: 114
		// (get) Token: 0x0600018F RID: 399
		string[] TargetGroups { get; }

		// Token: 0x17000073 RID: 115
		// (get) Token: 0x06000190 RID: 400
		int FileCount { get; }

		// Token: 0x17000074 RID: 116
		// (get) Token: 0x06000191 RID: 401
		IEnumerable<IFileEntry> Files { get; }

		// Token: 0x06000192 RID: 402
		IFileEntry FindFile(string devicePath);

		// Token: 0x06000193 RID: 403
		IFileEntry GetDsmFile();

		// Token: 0x06000194 RID: 404
		void ExtractFile(string devicePath, string destPath, bool overwriteExistingFiles);

		// Token: 0x06000195 RID: 405
		void ExtractAll(string rootDir, bool overwriteExistingFiles);

		// Token: 0x17000075 RID: 117
		// (get) Token: 0x06000196 RID: 406
		string Keyform { get; }
	}
}
