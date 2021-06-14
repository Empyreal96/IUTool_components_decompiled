using System;

namespace Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
	// Token: 0x0200001E RID: 30
	public class ConstValue<T>
	{
		// Token: 0x06000113 RID: 275 RVA: 0x000073CE File Offset: 0x000055CE
		public ConstValue(T value)
		{
			this.Value = value;
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000115 RID: 277 RVA: 0x000073E6 File Offset: 0x000055E6
		// (set) Token: 0x06000114 RID: 276 RVA: 0x000073DD File Offset: 0x000055DD
		public T Value { get; private set; }
	}
}
