using System;

namespace Microsoft.Composition.ToolBox.Reflection
{
	// Token: 0x02000012 RID: 18
	public class StatefulObject : ReflectiveObject
	{
		// Token: 0x06000053 RID: 83 RVA: 0x00002FA8 File Offset: 0x000011A8
		public StatefulObject()
		{
			this.WashObject();
		}

		// Token: 0x06000054 RID: 84 RVA: 0x00002FB6 File Offset: 0x000011B6
		public void TouchObject()
		{
			this.cleanHash = new int?(this.GetHashCode() + 1);
		}

		// Token: 0x06000055 RID: 85 RVA: 0x00002FCB File Offset: 0x000011CB
		public void WashObject()
		{
			this.cleanHash = new int?(this.GetHashCode());
		}

		// Token: 0x06000056 RID: 86 RVA: 0x00002FE0 File Offset: 0x000011E0
		public bool IsDirty()
		{
			return this.cleanHash != this.GetHashCode();
		}

		// Token: 0x04000054 RID: 84
		private int? cleanHash;
	}
}
