using System;
using System.Collections.Generic;

namespace Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
	// Token: 0x02000041 RID: 65
	public class ResourceAclComparer : IEqualityComparer<ResourceAcl>
	{
		// Token: 0x060001A7 RID: 423 RVA: 0x00008EE0 File Offset: 0x000070E0
		public bool Equals(ResourceAcl x, ResourceAcl y)
		{
			bool result = false;
			if (!string.IsNullOrEmpty(x.Path) && !string.IsNullOrEmpty(y.Path))
			{
				result = x.Path.Equals(y.Path, StringComparison.OrdinalIgnoreCase);
			}
			return result;
		}

		// Token: 0x060001A8 RID: 424 RVA: 0x00008F20 File Offset: 0x00007120
		public int GetHashCode(ResourceAcl obj)
		{
			int result = 0;
			if (!string.IsNullOrEmpty(obj.Path))
			{
				result = obj.Path.GetHashCode();
			}
			return result;
		}
	}
}
