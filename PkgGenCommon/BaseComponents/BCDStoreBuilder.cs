using System;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents.Internal;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents
{
	// Token: 0x02000037 RID: 55
	public class BCDStoreBuilder : PkgObjectBuilder<BCDStorePkgObject, BCDStoreBuilder>
	{
		// Token: 0x060000D4 RID: 212 RVA: 0x000052AB File Offset: 0x000034AB
		public BCDStoreBuilder(string source)
		{
			if (string.IsNullOrEmpty(source))
			{
				throw new ArgumentException("source must not be null or empty.");
			}
			this.pkgObject.Source = source;
		}
	}
}
