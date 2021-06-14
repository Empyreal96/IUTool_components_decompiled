using System;
using System.IO;
using Microsoft.WindowsPhone.ImageUpdate.Tools;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgCommon
{
	// Token: 0x02000012 RID: 18
	internal static class PKRBuilder
	{
		// Token: 0x060000A2 RID: 162 RVA: 0x00006594 File Offset: 0x00004794
		internal static void Create(string inputCab, string outputCab)
		{
			Console.WriteLine(Path.ChangeExtension(outputCab, PkgConstants.c_strRemovalCbsExtension));
			PkgManifest pkgManifest = PkgManifest.Load(inputCab, PkgConstants.c_strDsmFile);
			if (pkgManifest.IsRemoval)
			{
				throw new PackageException("Input package '{0}' can not be a removal package", new object[]
				{
					inputCab
				});
			}
			if (pkgManifest.IsBinaryPartition)
			{
				throw new PackageException("Input package '{0}' can not be a binary partition package", new object[]
				{
					inputCab
				});
			}
			pkgManifest.IsRemoval = true;
			IPkgBuilder pkgBuilder = new PkgBuilder(pkgManifest);
			if (pkgManifest.PackageStyle == PackageStyle.CBS)
			{
				pkgBuilder.SaveCBSR(outputCab, CompressionType.FastLZX);
				return;
			}
			pkgBuilder.SaveCab(outputCab, true);
		}
	}
}
