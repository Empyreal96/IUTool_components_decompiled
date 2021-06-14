using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Microsoft.MobileDevices.TestInfrastructure.UtilityLibrary
{
	// Token: 0x02000028 RID: 40
	public static class RetryHelper
	{
		// Token: 0x060000B4 RID: 180 RVA: 0x000057D4 File Offset: 0x000039D4
		public static void Retry(Action action, int retryCount, TimeSpan retryDelay)
		{
			RetryHelper.Retry(action, retryCount, retryDelay, new Type[]
			{
				typeof(Exception)
			});
		}

		// Token: 0x060000B5 RID: 181 RVA: 0x00005828 File Offset: 0x00003A28
		public static void Retry(Action action, int retryCount, TimeSpan retryDelay, IEnumerable<Type> retryableExceptions)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			RetryHelper.Retry<bool>(delegate()
			{
				action();
				return true;
			}, retryCount, retryDelay, retryableExceptions, null);
		}

		// Token: 0x060000B6 RID: 182 RVA: 0x00005878 File Offset: 0x00003A78
		public static T Retry<T>(Func<T> func, int retryCount, TimeSpan retryDelay)
		{
			return RetryHelper.Retry<T>(func, retryCount, retryDelay, new Type[]
			{
				typeof(Exception)
			}, null);
		}

		// Token: 0x060000B7 RID: 183 RVA: 0x000058F4 File Offset: 0x00003AF4
		public static T Retry<T>(Func<T> func, int retryCount, TimeSpan retryDelay, IEnumerable<Type> retryableExceptions, RetryHelper.OnFailureCallback onFailure = null)
		{
			if (retryableExceptions == null || !retryableExceptions.Any<Type>())
			{
				throw new ArgumentNullException("retryableExceptions");
			}
			if (retryableExceptions.Any((Type re) => !typeof(Exception).IsAssignableFrom(re)))
			{
				throw new ArgumentException("Retryable exceptions list contains element(s) that are not exception types");
			}
			return RetryHelper.Retry<T>(func, retryCount, retryDelay, (Exception e) => RetryHelper.DefaultIsRetryableException(retryableExceptions, e), onFailure);
		}

		// Token: 0x060000B8 RID: 184 RVA: 0x000059A4 File Offset: 0x00003BA4
		public static void Retry(Action action, int retryCount, TimeSpan retryDelay, IEnumerable<Type> retryableExceptions, RetryHelper.OnFailureCallback onFailure = null)
		{
			RetryHelper.Retry<bool>(delegate()
			{
				action();
				return true;
			}, retryCount, retryDelay, retryableExceptions, onFailure);
		}

		// Token: 0x060000B9 RID: 185 RVA: 0x000059D8 File Offset: 0x00003BD8
		public static T Retry<T>(Func<T> func, int retryCount, TimeSpan retryDelay, RetryHelper.IsRetryableExceptionCallback isRetryableException, RetryHelper.OnFailureCallback onFailure = null)
		{
			if (func == null)
			{
				throw new ArgumentNullException("func");
			}
			if (retryCount < 0)
			{
				throw new ArgumentOutOfRangeException("retryCount", retryCount, "Retry count is negative");
			}
			if (retryDelay < TimeSpan.Zero)
			{
				throw new ArgumentOutOfRangeException("retryDelay", retryDelay, "Retry delay is negative");
			}
			if (isRetryableException == null)
			{
				throw new ArgumentNullException("isRetryableException");
			}
			T result = default(T);
			for (int i = 0; i <= retryCount; i++)
			{
				try
				{
					result = func();
					break;
				}
				catch (Exception ex)
				{
					if (!isRetryableException(ex))
					{
						throw;
					}
					if (onFailure != null)
					{
						onFailure(ex, i + 1, retryCount + 1);
					}
					if (i >= retryCount)
					{
						throw;
					}
					Thread.Sleep(RetryHelper.MakeRandomSleepInterval(retryDelay));
				}
			}
			return result;
		}

		// Token: 0x060000BA RID: 186 RVA: 0x00005B20 File Offset: 0x00003D20
		public static void Retry(Action action, int retryCount, TimeSpan retryDelay, RetryHelper.IsRetryableExceptionCallback isRetryableException, RetryHelper.OnFailureCallback onFailure = null)
		{
			RetryHelper.Retry<bool>(delegate()
			{
				action();
				return true;
			}, retryCount, retryDelay, isRetryableException, onFailure);
		}

		// Token: 0x060000BB RID: 187 RVA: 0x00005B54 File Offset: 0x00003D54
		public static T KeepTrying<T>(Func<T> func, TimeSpan timeout, TimeSpan retryDelay, TimeSpan? initialDelay = null, RetryHelper.IsRetryableExceptionCallback isRetryableException = null, RetryHelper.OnFailureCallback onFailure = null)
		{
			if (func == null)
			{
				throw new ArgumentNullException("func");
			}
			if (isRetryableException == null)
			{
				isRetryableException = new RetryHelper.IsRetryableExceptionCallback(RetryHelper.AllowAnyException);
			}
			if (initialDelay != null)
			{
				Thread.Sleep(initialDelay.Value);
			}
			DateTime now = DateTime.Now;
			int num = 0;
			T result;
			for (;;)
			{
				try
				{
					num++;
					result = func();
					break;
				}
				catch (Exception ex)
				{
					if (!isRetryableException(ex))
					{
						throw;
					}
					if (onFailure != null)
					{
						onFailure(ex, num, -1);
					}
					if (DateTime.Now - now >= timeout)
					{
						throw;
					}
				}
				Thread.Sleep(retryDelay);
			}
			return result;
		}

		// Token: 0x060000BC RID: 188 RVA: 0x00005C64 File Offset: 0x00003E64
		public static void KeepTrying(Action action, TimeSpan timeout, TimeSpan retryDelay, TimeSpan? initialDelay = null, RetryHelper.IsRetryableExceptionCallback isRetryableException = null, RetryHelper.OnFailureCallback onFailure = null)
		{
			RetryHelper.KeepTrying<bool>(delegate()
			{
				action();
				return true;
			}, timeout, retryDelay, initialDelay, isRetryableException, onFailure);
		}

		// Token: 0x060000BD RID: 189 RVA: 0x00005D28 File Offset: 0x00003F28
		private static bool DefaultIsRetryableException(IEnumerable<Type> retryableExceptions, Exception e)
		{
			if (!retryableExceptions.Any((Type re) => re.IsInstanceOfType(e)))
			{
				AggregateException ex = e as AggregateException;
				bool flag;
				if (ex != null)
				{
					flag = ex.InnerExceptions.Any((Exception ie) => retryableExceptions.Any((Type re) => re.IsInstanceOfType(ie)));
				}
				else
				{
					flag = false;
				}
				if (!flag)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x060000BE RID: 190 RVA: 0x00005DAC File Offset: 0x00003FAC
		private static bool AllowAnyException(Exception e)
		{
			return true;
		}

		// Token: 0x060000BF RID: 191 RVA: 0x00005DC0 File Offset: 0x00003FC0
		private static TimeSpan MakeRandomSleepInterval(TimeSpan baseInterval)
		{
			double totalMilliseconds = baseInterval.TotalMilliseconds;
			double num = 1.0 + (RetryHelper.RandomGenerator.NextDouble() - 0.5) * 0.15 * 2.0;
			double value = Math.Max(totalMilliseconds * num, 0.0);
			return TimeSpan.FromMilliseconds(value);
		}

		// Token: 0x040000A7 RID: 167
		private const double MaxSleepOffsetAdjustmentPercent = 0.15;

		// Token: 0x040000A8 RID: 168
		private static readonly Random RandomGenerator = new Random();

		// Token: 0x02000029 RID: 41
		// (Invoke) Token: 0x060000C3 RID: 195
		public delegate void OnFailureCallback(Exception error, int numAttempts, int maxAttempts);

		// Token: 0x0200002A RID: 42
		// (Invoke) Token: 0x060000C7 RID: 199
		public delegate bool IsRetryableExceptionCallback(Exception e);
	}
}
