using System;
using System.Collections.Generic;

namespace Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
	// Token: 0x0200003F RID: 63
	public class AclCollection : HashSet<ResourceAcl>
	{
		// Token: 0x0600018C RID: 396 RVA: 0x00008B25 File Offset: 0x00006D25
		public AclCollection() : base(ResourceAcl.Comparer)
		{
		}
	}
}
