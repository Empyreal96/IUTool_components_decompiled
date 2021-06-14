using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace Microsoft.Phone.TestInfra.Deployment
{
	// Token: 0x02000020 RID: 32
	public static class WaitHandleHelper
	{
		// Token: 0x0600016E RID: 366 RVA: 0x00009030 File Offset: 0x00007230
		public static void Acquire(WaitHandle handle, TimeoutHelper timeoutHelper)
		{
			bool flag = handle == null;
			if (flag)
			{
				throw new ArgumentNullException("handle");
			}
			bool flag2 = timeoutHelper == null;
			if (flag2)
			{
				throw new ArgumentNullException("timeoutHelper");
			}
			TimeSpan remaining = timeoutHelper.Remaining;
			bool isExpired = timeoutHelper.IsExpired;
			if (isExpired)
			{
				throw new TimeoutException(string.Format(CultureInfo.InvariantCulture, "Timeout expired: {0}", new object[]
				{
					timeoutHelper.Timeout
				}));
			}
			WaitHandleHelper.Acquire(handle, remaining);
		}

		// Token: 0x0600016F RID: 367 RVA: 0x000090AC File Offset: 0x000072AC
		public static void Acquire(WaitHandle handle, TimeSpan timeout)
		{
			bool flag = handle == null;
			if (flag)
			{
				throw new ArgumentNullException("handle");
			}
			try
			{
				bool flag2 = !handle.WaitOne(timeout);
				if (flag2)
				{
					throw new TimeoutException(string.Format(CultureInfo.InvariantCulture, "Unable to acquire mutex within the {0}", new object[]
					{
						timeout
					}));
				}
			}
			catch (AbandonedMutexException)
			{
			}
		}

		// Token: 0x06000170 RID: 368 RVA: 0x0000911C File Offset: 0x0000731C
		public static bool TryToAcquire(WaitHandle handle)
		{
			try
			{
				WaitHandleHelper.Acquire(handle, TimeSpan.Zero);
			}
			catch (TimeoutException)
			{
				return false;
			}
			return true;
		}

		// Token: 0x06000171 RID: 369 RVA: 0x00009158 File Offset: 0x00007358
		public static void AcquireAll(IEnumerable<WaitHandle> handles, TimeSpan timeout)
		{
			WaitHandleHelper.AcquireAll(handles, new TimeoutHelper(timeout));
		}

		// Token: 0x06000172 RID: 370 RVA: 0x00009168 File Offset: 0x00007368
		public static void AcquireAll(IEnumerable<WaitHandle> handles, TimeoutHelper timeoutHelper)
		{
			bool flag = handles == null || !handles.Any<WaitHandle>();
			if (flag)
			{
				throw new ArgumentNullException("handles");
			}
			bool flag2 = timeoutHelper == null;
			if (flag2)
			{
				throw new ArgumentNullException("timeoutHelper");
			}
			List<WaitHandle> list = new List<WaitHandle>();
			List<WaitHandle> list2 = new List<WaitHandle>(handles);
			int num;
			for (int i = 0; i < list2.Count; i = num + 1)
			{
				for (int j = i + 1; j < list2.Count; j = num + 1)
				{
					bool flag3 = list2[i] == list2[j];
					if (flag3)
					{
						throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Handles list contains equal handles", new object[0]));
					}
					num = j;
				}
				num = i;
			}
			try
			{
				while (list2.Any<WaitHandle>())
				{
					for (int k = 0; k < list2.Count; k = num + 1)
					{
						bool flag4 = !WaitHandleHelper.TryToAcquire(list2[k]);
						if (!flag4)
						{
							list.Add(list2[k]);
							list2.RemoveAt(k);
							num = k;
							k = num - 1;
						}
						num = k;
					}
					bool isExpired = timeoutHelper.IsExpired;
					if (isExpired)
					{
						throw new TimeoutException(string.Format(CultureInfo.InvariantCulture, "Unable to acquire all handles within {0}", new object[]
						{
							timeoutHelper.Timeout
						}));
					}
				}
			}
			catch
			{
				foreach (WaitHandle waitHandle in list)
				{
					Mutex mutex = (Mutex)waitHandle;
					try
					{
						mutex.ReleaseMutex();
					}
					catch (ApplicationException)
					{
					}
				}
				throw;
			}
		}
	}
}
