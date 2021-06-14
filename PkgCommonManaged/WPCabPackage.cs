using System;
using System.Linq;
using Microsoft.WindowsPhone.ImageUpdate.Tools;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgCommon
{
	// Token: 0x0200000F RID: 15
	internal class WPCabPackage : WPCanonicalPackage
	{
		// Token: 0x0600007E RID: 126 RVA: 0x000041BC File Offset: 0x000023BC
		protected WPCabPackage(string cabPath, PkgManifest pkgManifest) : base(pkgManifest)
		{
			this.m_strCabPath = cabPath;
		}

		// Token: 0x0600007F RID: 127 RVA: 0x000041CC File Offset: 0x000023CC
		protected override void ExtractFiles(FileEntryBase[] files, string[] targetPaths)
		{
			CabApiWrapper.ExtractSelected(this.m_strCabPath, (from x in files
			select x.CabPath).ToArray<string>(), targetPaths);
		}

		// Token: 0x06000080 RID: 128 RVA: 0x00004204 File Offset: 0x00002404
		public static WPCabPackage Load(string cabPath)
		{
			return new WPCabPackage(cabPath, PkgManifest.Load(cabPath, PkgConstants.c_strDsmFile));
		}

		// Token: 0x04000014 RID: 20
		private string m_strCabPath;
	}
}
