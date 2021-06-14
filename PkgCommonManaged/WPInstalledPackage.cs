using System;
using System.IO;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgCommon
{
	// Token: 0x0200000D RID: 13
	internal class WPInstalledPackage : WPCanonicalPackage
	{
		// Token: 0x06000078 RID: 120 RVA: 0x000040AD File Offset: 0x000022AD
		protected WPInstalledPackage(string installationRoot, PkgManifest pkgManifest) : base(pkgManifest)
		{
			pkgManifest.BuildSourcePaths(installationRoot, BuildPathOption.UseDevicePath, true);
		}

		// Token: 0x06000079 RID: 121 RVA: 0x000040C0 File Offset: 0x000022C0
		protected override void ExtractFiles(FileEntryBase[] files, string[] targetPaths)
		{
			for (int i = 0; i < files.Length; i++)
			{
				LongPathFile.Copy(files[i].SourcePath, targetPaths[i], true);
			}
		}

		// Token: 0x0600007A RID: 122 RVA: 0x000040EC File Offset: 0x000022EC
		public static WPInstalledPackage Load(string installationDir, string manifestPath)
		{
			PkgManifest pkgManifest;
			if (Path.GetExtension(manifestPath).Equals(PkgConstants.c_strMumExtension, StringComparison.OrdinalIgnoreCase))
			{
				pkgManifest = PkgManifest.Load_CBS(manifestPath);
			}
			else
			{
				pkgManifest = PkgManifest.Load(manifestPath);
			}
			return new WPInstalledPackage(installationDir, pkgManifest);
		}
	}
}
