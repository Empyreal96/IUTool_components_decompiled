using System;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents.Internal;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.Interfaces;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents
{
	// Token: 0x02000046 RID: 70
	public abstract class PkgObject
	{
		// Token: 0x17000042 RID: 66
		// (get) Token: 0x06000121 RID: 289 RVA: 0x00005B5B File Offset: 0x00003D5B
		// (set) Token: 0x06000122 RID: 290 RVA: 0x00005B63 File Offset: 0x00003D63
		internal MacroTable LocalMacros { get; set; }

		// Token: 0x06000123 RID: 291 RVA: 0x00003E08 File Offset: 0x00002008
		internal PkgObject()
		{
		}

		// Token: 0x06000124 RID: 292 RVA: 0x00004469 File Offset: 0x00002669
		protected virtual void DoPreprocess(PackageProject proj, IMacroResolver macroResolver)
		{
		}

		// Token: 0x06000125 RID: 293 RVA: 0x00004469 File Offset: 0x00002669
		protected virtual void DoBuild(IPackageGenerator pkgGen)
		{
		}

		// Token: 0x06000126 RID: 294 RVA: 0x00005B6C File Offset: 0x00003D6C
		public void Preprocess(PackageProject proj, IMacroResolver macroResolver)
		{
			this.DoPreprocess(proj, macroResolver);
		}

		// Token: 0x06000127 RID: 295 RVA: 0x00005B76 File Offset: 0x00003D76
		public void Build(IPackageGenerator pkgGen)
		{
			pkgGen.MacroResolver.BeginLocal();
			if (this.LocalMacros != null)
			{
				pkgGen.MacroResolver.Register(this.LocalMacros.Values);
			}
			this.DoBuild(pkgGen);
			pkgGen.MacroResolver.EndLocal();
		}
	}
}
