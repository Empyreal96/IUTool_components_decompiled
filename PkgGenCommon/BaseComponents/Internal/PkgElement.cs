using System;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.Interfaces;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents.Internal
{
	// Token: 0x02000067 RID: 103
	public class PkgElement
	{
		// Token: 0x060001DC RID: 476 RVA: 0x00005B18 File Offset: 0x00003D18
		public virtual void Build(IPackageGenerator pkgGen, SatelliteId satelliteId)
		{
			throw new NotImplementedException();
		}

		// Token: 0x060001DD RID: 477 RVA: 0x00007687 File Offset: 0x00005887
		public virtual void Build(IPackageGenerator pkgGen)
		{
			this.Build(pkgGen, SatelliteId.Neutral);
		}
	}
}
