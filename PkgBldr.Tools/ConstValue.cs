using System;

namespace Microsoft.CompPlat.PkgBldr.Tools
{
	// Token: 0x0200002A RID: 42
	public class ConstValue<T>
	{
		// Token: 0x06000189 RID: 393 RVA: 0x000075EE File Offset: 0x000057EE
		public ConstValue(T value)
		{
			this.Value = value;
		}

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x0600018B RID: 395 RVA: 0x00007606 File Offset: 0x00005806
		// (set) Token: 0x0600018A RID: 394 RVA: 0x000075FD File Offset: 0x000057FD
		public T Value { get; private set; }
	}
}
