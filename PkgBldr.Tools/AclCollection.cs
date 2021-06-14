using System;
using System.Collections.Generic;

namespace Microsoft.CompPlat.PkgBldr.Tools
{
	// Token: 0x02000020 RID: 32
	public class AclCollection : HashSet<ResourceAcl>
	{
		// Token: 0x0600013A RID: 314 RVA: 0x00006519 File Offset: 0x00004719
		public AclCollection() : base(ResourceAcl.Comparer)
		{
		}
	}
}
