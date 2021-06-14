using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Composition.ToolBox;

namespace Microsoft.Composition.Packaging.Interfaces
{
	// Token: 0x0200000A RID: 10
	public interface IManifest : IFile
	{
		// Token: 0x17000067 RID: 103
		// (get) Token: 0x06000118 RID: 280
		ManifestType ManifestType { get; }

		// Token: 0x17000068 RID: 104
		// (get) Token: 0x06000119 RID: 281
		// (set) Token: 0x0600011A RID: 282
		Keyform Keyform { get; set; }

		// Token: 0x17000069 RID: 105
		// (get) Token: 0x0600011B RID: 283
		// (set) Token: 0x0600011C RID: 284
		PhoneInformation PhoneInformation { get; set; }

		// Token: 0x1700006A RID: 106
		// (get) Token: 0x0600011D RID: 285
		bool NoMerge { get; }

		// Token: 0x1700006B RID: 107
		// (get) Token: 0x0600011E RID: 286
		string UpdateName { get; }

		// Token: 0x1700006C RID: 108
		// (get) Token: 0x0600011F RID: 287
		bool SelfUpdate { get; }

		// Token: 0x1700006D RID: 109
		// (get) Token: 0x06000120 RID: 288
		// (set) Token: 0x06000121 RID: 289
		bool? BinaryPartition { get; set; }

		// Token: 0x1700006E RID: 110
		// (get) Token: 0x06000122 RID: 290
		// (set) Token: 0x06000123 RID: 291
		string Partition { get; set; }

		// Token: 0x1700006F RID: 111
		// (get) Token: 0x06000124 RID: 292
		string InfName { get; }

		// Token: 0x17000070 RID: 112
		// (get) Token: 0x06000125 RID: 293
		string InfRanking { get; }

		// Token: 0x17000071 RID: 113
		// (get) Token: 0x06000126 RID: 294
		IEnumerable<IFile> AllFiles { get; }

		// Token: 0x17000072 RID: 114
		// (get) Token: 0x06000127 RID: 295
		IEnumerable<IFile> AllPayloadFiles { get; }

		// Token: 0x17000073 RID: 115
		// (get) Token: 0x06000128 RID: 296
		IEnumerable<IManifest> AllManifestFiles { get; }

		// Token: 0x17000074 RID: 116
		// (get) Token: 0x06000129 RID: 297
		IEnumerable<IFile> CurrentPayloadFiles { get; }

		// Token: 0x17000075 RID: 117
		// (get) Token: 0x0600012A RID: 298
		IEnumerable<IManifest> CurrentManifestFiles { get; }

		// Token: 0x0600012B RID: 299
		void Validate();

		// Token: 0x0600012C RID: 300
		void AddParent(XElement assemblyId);

		// Token: 0x0600012D RID: 301
		void AddFile(IFile file);

		// Token: 0x0600012E RID: 302
		void AddFile(IFile file, string embeddedSigningCategory);

		// Token: 0x0600012F RID: 303
		void AddFile(IManifest file);

		// Token: 0x06000130 RID: 304
		void AddFile(IManifest file, string embeddedSigningCategory);

		// Token: 0x06000131 RID: 305
		void AddFile(FileType fileType, string sourcePath, string destinationPath, string sourcePackage);

		// Token: 0x06000132 RID: 306
		void AddFile(FileType fileType, string sourcePath, string destinationPath, string sourcePackage, string embeddedSigningCategory);

		// Token: 0x06000133 RID: 307
		void RemoveFile(IFile file);

		// Token: 0x06000134 RID: 308
		void RemoveFile(IManifest file);

		// Token: 0x06000135 RID: 309
		void RemoveFile(FileType fileType, string destinationPath);

		// Token: 0x06000136 RID: 310
		void RemoveAllFiles();

		// Token: 0x06000137 RID: 311
		IEnumerable<string> GetAllSourcePaths();

		// Token: 0x06000138 RID: 312
		IEnumerable<string> GetAllCabPaths();

		// Token: 0x06000139 RID: 313
		List<Keyform> GetParents();

		// Token: 0x0600013A RID: 314
		void Clean();

		// Token: 0x0600013B RID: 315
		void Touch();

		// Token: 0x0600013C RID: 316
		void SaveManifest(string outputFolder);

		// Token: 0x0600013D RID: 317
		void SetCBSFeatureInfo(string fmId, string featureId, string pkgGroup, List<IPackageInfo> packages);

		// Token: 0x0600013E RID: 318
		XElement GetCBSFeatureInfo();
	}
}
