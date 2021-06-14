using System;
using System.Collections.Generic;
using Microsoft.Composition.ToolBox;
using Microsoft.Composition.ToolBox.Cab;

namespace Microsoft.Composition.Packaging.Interfaces
{
	// Token: 0x0200000B RID: 11
	public interface IPackage
	{
		// Token: 0x17000076 RID: 118
		// (get) Token: 0x0600013F RID: 319
		// (set) Token: 0x06000140 RID: 320
		string PackageName { get; set; }

		// Token: 0x17000077 RID: 119
		// (get) Token: 0x06000141 RID: 321
		// (set) Token: 0x06000142 RID: 322
		string Owner { get; set; }

		// Token: 0x17000078 RID: 120
		// (get) Token: 0x06000143 RID: 323
		// (set) Token: 0x06000144 RID: 324
		PhoneReleaseType PhoneReleaseType { get; set; }

		// Token: 0x17000079 RID: 121
		// (get) Token: 0x06000145 RID: 325
		// (set) Token: 0x06000146 RID: 326
		string Component { get; set; }

		// Token: 0x1700007A RID: 122
		// (get) Token: 0x06000147 RID: 327
		// (set) Token: 0x06000148 RID: 328
		string SubComponent { get; set; }

		// Token: 0x1700007B RID: 123
		// (get) Token: 0x06000149 RID: 329
		// (set) Token: 0x0600014A RID: 330
		Version Version { get; set; }

		// Token: 0x1700007C RID: 124
		// (get) Token: 0x0600014B RID: 331
		// (set) Token: 0x0600014C RID: 332
		PhoneOwnerType OwnerType { get; set; }

		// Token: 0x1700007D RID: 125
		// (get) Token: 0x0600014D RID: 333
		// (set) Token: 0x0600014E RID: 334
		string ReleaseType { get; set; }

		// Token: 0x1700007E RID: 126
		// (get) Token: 0x0600014F RID: 335
		// (set) Token: 0x06000150 RID: 336
		BuildType BuildType { get; set; }

		// Token: 0x1700007F RID: 127
		// (get) Token: 0x06000151 RID: 337
		// (set) Token: 0x06000152 RID: 338
		CpuArch HostArch { get; set; }

		// Token: 0x17000080 RID: 128
		// (get) Token: 0x06000153 RID: 339
		// (set) Token: 0x06000154 RID: 340
		CpuArch GuestArch { get; set; }

		// Token: 0x17000081 RID: 129
		// (get) Token: 0x06000155 RID: 341
		string CpuString { get; }

		// Token: 0x17000082 RID: 130
		// (get) Token: 0x06000156 RID: 342
		bool IsWow { get; }

		// Token: 0x17000083 RID: 131
		// (get) Token: 0x06000157 RID: 343
		// (set) Token: 0x06000158 RID: 344
		string Culture { get; set; }

		// Token: 0x17000084 RID: 132
		// (get) Token: 0x06000159 RID: 345
		// (set) Token: 0x0600015A RID: 346
		string PublicKey { get; set; }

		// Token: 0x17000085 RID: 133
		// (get) Token: 0x0600015B RID: 347
		// (set) Token: 0x0600015C RID: 348
		string GroupingKey { get; set; }

		// Token: 0x17000086 RID: 134
		// (get) Token: 0x0600015D RID: 349
		// (set) Token: 0x0600015E RID: 350
		string Partition { get; set; }

		// Token: 0x17000087 RID: 135
		// (get) Token: 0x0600015F RID: 351
		bool? BinaryPartition { get; }

		// Token: 0x17000088 RID: 136
		// (get) Token: 0x06000160 RID: 352
		PackageType PackageType { get; }

		// Token: 0x17000089 RID: 137
		// (get) Token: 0x06000161 RID: 353
		PackageMetrics Metrics { get; }

		// Token: 0x1700008A RID: 138
		// (get) Token: 0x06000162 RID: 354
		IEnumerable<IFile> AllFiles { get; }

		// Token: 0x1700008B RID: 139
		// (get) Token: 0x06000163 RID: 355
		IEnumerable<IFile> AllPayloadFiles { get; }

		// Token: 0x1700008C RID: 140
		// (get) Token: 0x06000164 RID: 356
		IEnumerable<IManifest> AllManifestFiles { get; }

		// Token: 0x06000165 RID: 357
		void Validate();

		// Token: 0x06000166 RID: 358
		void SavePackage(string outputFolder);

		// Token: 0x06000167 RID: 359
		void SaveCab(string cabPath);

		// Token: 0x06000168 RID: 360
		void SaveCab(string cabPath, CabToolBox.CompressionType compressionType);

		// Token: 0x06000169 RID: 361
		IManifest GetRoot();

		// Token: 0x0600016A RID: 362
		IFile FindFile(string destinationPath);

		// Token: 0x0600016B RID: 363
		void AddFile(IFile file);

		// Token: 0x0600016C RID: 364
		void AddFile(IFile file, string embeddedSigningCategory);

		// Token: 0x0600016D RID: 365
		void AddFile(IManifest file);

		// Token: 0x0600016E RID: 366
		void AddFile(IManifest file, string embeddedSigningCategory);

		// Token: 0x0600016F RID: 367
		void AddFile(FileType fileType, string sourcePath, string destinationPath, string sourcePackage);

		// Token: 0x06000170 RID: 368
		void AddFile(FileType fileType, string sourcePath, string destinationPath, string sourcePackage, string embeddedSigningCategory);

		// Token: 0x06000171 RID: 369
		void RemoveFile(IFile file);

		// Token: 0x06000172 RID: 370
		void RemoveFile(IManifest file);

		// Token: 0x06000173 RID: 371
		void RemoveFile(FileType fileType, string destinationPath);

		// Token: 0x06000174 RID: 372
		void RemoveAllFiles();

		// Token: 0x06000175 RID: 373
		IEnumerable<string> GetAllSourcePaths();

		// Token: 0x06000176 RID: 374
		IEnumerable<string> GetAllCabPaths();

		// Token: 0x06000177 RID: 375
		void SetCBSFeatureInfo(string fmId, string featureId, string pkgGroup, List<IPackageInfo> packages);

		// Token: 0x06000178 RID: 376
		void SetCBSFeatureInfo(string fmId, string featureId, string pkgGroup, List<string> pkgPaths);
	}
}
