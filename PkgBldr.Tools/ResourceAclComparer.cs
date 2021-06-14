using System;
using System.Collections.Generic;

namespace Microsoft.CompPlat.PkgBldr.Tools
{
	// Token: 0x02000022 RID: 34
	public class ResourceAclComparer : IEqualityComparer<ResourceAcl>
	{
		// Token: 0x06000155 RID: 341 RVA: 0x0000688C File Offset: 0x00004A8C
		public bool Equals(ResourceAcl x, ResourceAcl y)
		{
			bool result = false;
			if (x != null && y != null && !string.IsNullOrEmpty(x.Path) && !string.IsNullOrEmpty(y.Path))
			{
				result = x.Path.Equals(y.Path, StringComparison.OrdinalIgnoreCase);
			}
			return result;
		}

		// Token: 0x06000156 RID: 342 RVA: 0x000068D0 File Offset: 0x00004AD0
		public int GetHashCode(ResourceAcl obj)
		{
			int result = 0;
			if (obj != null && !string.IsNullOrEmpty(obj.Path))
			{
				result = obj.Path.GetHashCode();
			}
			return result;
		}
	}
}
