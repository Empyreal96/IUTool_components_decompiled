using System;
using System.IO;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgCommon
{
	// Token: 0x0200000E RID: 14
	internal class WPExtractedPackage : WPCanonicalPackage
	{
		// Token: 0x0600007B RID: 123 RVA: 0x00004125 File Offset: 0x00002325
		protected WPExtractedPackage(string extractedRootDir, PkgManifest pkgManifest) : base(pkgManifest)
		{
			pkgManifest.BuildSourcePaths(extractedRootDir, BuildPathOption.UseCabPath);
		}

		// Token: 0x0600007C RID: 124 RVA: 0x00004138 File Offset: 0x00002338
		protected override void ExtractFiles(FileEntryBase[] files, string[] targetPaths)
		{
			for (int i = 0; i < files.Length; i++)
			{
				File.Copy(files[i].SourcePath, targetPaths[i], true);
			}
		}

		// Token: 0x0600007D RID: 125 RVA: 0x00004164 File Offset: 0x00002364
		public static WPExtractedPackage Load(string extractedRootDir)
		{
			PkgManifest pkgManifest;
			if (LongPathFile.Exists(Path.Combine(extractedRootDir, PkgConstants.c_strMumFile)))
			{
				pkgManifest = PkgManifest.Load_CBS(Path.Combine(extractedRootDir, PkgConstants.c_strMumFile));
				pkgManifest.PackageStyle = PackageStyle.CBS;
			}
			else
			{
				pkgManifest = PkgManifest.Load(Path.Combine(extractedRootDir, PkgConstants.c_strDsmFile));
				pkgManifest.PackageStyle = PackageStyle.SPKG;
			}
			return new WPExtractedPackage(extractedRootDir, pkgManifest);
		}
	}
}
