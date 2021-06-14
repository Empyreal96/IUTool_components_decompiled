using System;
using System.Threading;

namespace Microsoft.Phone.TestInfra.Deployment
{
	// Token: 0x0200000E RID: 14
	public static class WaitHandleEx
	{
		// Token: 0x060000CE RID: 206 RVA: 0x00004E64 File Offset: 0x00003064
		public static void Acquire(this WaitHandle handle, TimeoutHelper timeoutHelper)
		{
			WaitHandleHelper.Acquire(handle, timeoutHelper);
		}

		// Token: 0x060000CF RID: 207 RVA: 0x00004E6F File Offset: 0x0000306F
		public static void Acquire(this WaitHandle handle, TimeSpan timeout)
		{
			WaitHandleHelper.Acquire(handle, timeout);
		}

		// Token: 0x060000D0 RID: 208 RVA: 0x00004E7C File Offset: 0x0000307C
		public static bool TryToAcquire(this WaitHandle handle)
		{
			return WaitHandleHelper.TryToAcquire(handle);
		}
	}
}
