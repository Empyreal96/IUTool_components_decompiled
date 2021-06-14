using System;
using System.IO;
using Microsoft.WindowsPhone.ImageUpdate.Tools;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgCommon
{
	// Token: 0x02000025 RID: 37
	public interface IPkgBuilder : IDisposable
	{
		// Token: 0x17000076 RID: 118
		// (get) Token: 0x06000197 RID: 407
		string Name { get; }

		// Token: 0x17000077 RID: 119
		// (get) Token: 0x06000198 RID: 408
		// (set) Token: 0x06000199 RID: 409
		string PackageName { get; set; }

		// Token: 0x17000078 RID: 120
		// (get) Token: 0x0600019A RID: 410
		// (set) Token: 0x0600019B RID: 411
		string Owner { get; set; }

		// Token: 0x17000079 RID: 121
		// (get) Token: 0x0600019C RID: 412
		// (set) Token: 0x0600019D RID: 413
		string Component { get; set; }

		// Token: 0x1700007A RID: 122
		// (get) Token: 0x0600019E RID: 414
		// (set) Token: 0x0600019F RID: 415
		string SubComponent { get; set; }

		// Token: 0x1700007B RID: 123
		// (get) Token: 0x060001A0 RID: 416
		// (set) Token: 0x060001A1 RID: 417
		string Partition { get; set; }

		// Token: 0x1700007C RID: 124
		// (get) Token: 0x060001A2 RID: 418
		// (set) Token: 0x060001A3 RID: 419
		string Platform { get; set; }

		// Token: 0x1700007D RID: 125
		// (get) Token: 0x060001A4 RID: 420
		// (set) Token: 0x060001A5 RID: 421
		VersionInfo Version { get; set; }

		// Token: 0x1700007E RID: 126
		// (get) Token: 0x060001A6 RID: 422
		// (set) Token: 0x060001A7 RID: 423
		OwnerType OwnerType { get; set; }

		// Token: 0x1700007F RID: 127
		// (get) Token: 0x060001A8 RID: 424
		// (set) Token: 0x060001A9 RID: 425
		ReleaseType ReleaseType { get; set; }

		// Token: 0x17000080 RID: 128
		// (get) Token: 0x060001AA RID: 426
		// (set) Token: 0x060001AB RID: 427
		PackageStyle PackageStyle { get; set; }

		// Token: 0x17000081 RID: 129
		// (get) Token: 0x060001AC RID: 428
		// (set) Token: 0x060001AD RID: 429
		BuildType BuildType { get; set; }

		// Token: 0x17000082 RID: 130
		// (get) Token: 0x060001AE RID: 430
		// (set) Token: 0x060001AF RID: 431
		CpuId CpuType { get; set; }

		// Token: 0x17000083 RID: 131
		// (get) Token: 0x060001B0 RID: 432
		CpuId ComplexCpuType { get; }

		// Token: 0x17000084 RID: 132
		// (get) Token: 0x060001B1 RID: 433
		bool IsWow { get; }

		// Token: 0x17000085 RID: 133
		// (get) Token: 0x060001B2 RID: 434
		// (set) Token: 0x060001B3 RID: 435
		string Culture { get; set; }

		// Token: 0x17000086 RID: 134
		// (get) Token: 0x060001B4 RID: 436
		// (set) Token: 0x060001B5 RID: 437
		string PublicKey { get; set; }

		// Token: 0x17000087 RID: 135
		// (get) Token: 0x060001B6 RID: 438
		// (set) Token: 0x060001B7 RID: 439
		string Resolution { get; set; }

		// Token: 0x17000088 RID: 136
		// (get) Token: 0x060001B8 RID: 440
		// (set) Token: 0x060001B9 RID: 441
		string BuildString { get; set; }

		// Token: 0x17000089 RID: 137
		// (get) Token: 0x060001BA RID: 442
		// (set) Token: 0x060001BB RID: 443
		string GroupingKey { get; set; }

		// Token: 0x1700008A RID: 138
		// (get) Token: 0x060001BC RID: 444
		// (set) Token: 0x060001BD RID: 445
		string[] TargetGroups { get; set; }

		// Token: 0x060001BE RID: 446
		IFileEntry FindFile(string destination);

		// Token: 0x060001BF RID: 447
		void AddFile(IFileEntry file, string source, string embedSignCategory = "None");

		// Token: 0x060001C0 RID: 448
		void AddFile(FileType type, string source, string destination, FileAttributes attributes, string srcPkg, string embedSignCategory = "None");

		// Token: 0x060001C1 RID: 449
		void AddFile(FileType type, string source, string destination, FileAttributes attributes, string srcPkg, string cabPath, string embedSignCategory);

		// Token: 0x060001C2 RID: 450
		void RemoveFile(string destination);

		// Token: 0x060001C3 RID: 451
		void RemoveAllFiles();

		// Token: 0x060001C4 RID: 452
		void SetIsRemoval(bool isRemoval);

		// Token: 0x060001C5 RID: 453
		void SetPkgFileSigner(IPkgFileSigner signer);

		// Token: 0x060001C6 RID: 454
		void SaveCab(string cabPath);

		// Token: 0x060001C7 RID: 455
		void SaveCab(string cabPath, PackageStyle outputStyle);

		// Token: 0x060001C8 RID: 456
		void SaveCab(string cabPath, bool compress);

		// Token: 0x060001C9 RID: 457
		void SaveCab(string cabPath, bool compress, PackageStyle outputStyle);

		// Token: 0x060001CA RID: 458
		void SaveCab(string cabPath, CompressionType compressionType);

		// Token: 0x060001CB RID: 459
		void SaveCab(string cabPath, CompressionType compressionType, PackageStyle outputStyle);

		// Token: 0x060001CC RID: 460
		void SaveCab(string cabPath, CompressionType compressionType, PackageStyle outputStyle, PackageTools.SIGNING_HINT signingHint);

		// Token: 0x060001CD RID: 461
		void SaveCBSR(string cabPath, CompressionType compressionType);
	}
}
