using System;
using System.Threading;

namespace Microsoft.TestInfra.UtilityLibrary
{
	// Token: 0x02000019 RID: 25
	public class LazyDisposable<T> : Lazy<T>, IDisposable where T : IDisposable
	{
		// Token: 0x0600007C RID: 124 RVA: 0x000047D7 File Offset: 0x000029D7
		public LazyDisposable()
		{
		}

		// Token: 0x0600007D RID: 125 RVA: 0x000047E2 File Offset: 0x000029E2
		public LazyDisposable(bool isThreadSafe) : base(isThreadSafe)
		{
		}

		// Token: 0x0600007E RID: 126 RVA: 0x000047EE File Offset: 0x000029EE
		public LazyDisposable(LazyThreadSafetyMode mode) : base(mode)
		{
		}

		// Token: 0x0600007F RID: 127 RVA: 0x000047FA File Offset: 0x000029FA
		public LazyDisposable(Func<T> valueFactory) : base(valueFactory)
		{
		}

		// Token: 0x06000080 RID: 128 RVA: 0x00004806 File Offset: 0x00002A06
		public LazyDisposable(Func<T> valueFactory, bool isThreadSafe) : base(valueFactory, isThreadSafe)
		{
		}

		// Token: 0x06000081 RID: 129 RVA: 0x00004813 File Offset: 0x00002A13
		public LazyDisposable(Func<T> valueFactory, LazyThreadSafetyMode mode) : base(valueFactory, mode)
		{
		}

		// Token: 0x06000082 RID: 130 RVA: 0x00004820 File Offset: 0x00002A20
		public void Dispose()
		{
			if (base.IsValueCreated)
			{
				T value = base.Value;
				value.Dispose();
			}
		}
	}
}
