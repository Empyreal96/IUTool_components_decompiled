using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.MobileDevices.TestInfrastructure.UtilityLibrary;
using Microsoft.Tools.IO;

namespace Microsoft.Phone.TestInfra.Deployment
{
	// Token: 0x02000016 RID: 22
	public static class ReliableDirectory
	{
		// Token: 0x06000109 RID: 265 RVA: 0x00006C5C File Offset: 0x00004E5C
		public static bool Exists(string path, int retryCount, TimeSpan retryDelay)
		{
			return RetryHelper.Retry<bool>(() => LongPathDirectory.Exists(path), retryCount, retryDelay, new Type[]
			{
				typeof(IOException)
			}, null);
		}

		// Token: 0x0600010A RID: 266 RVA: 0x00006CA4 File Offset: 0x00004EA4
		public static string[] GetDirectories(string path, int retryCount, TimeSpan retryDelay)
		{
			return RetryHelper.Retry<string[]>(() => LongPathDirectory.EnumerateDirectories(path).ToArray<string>(), retryCount, retryDelay, new Type[]
			{
				typeof(IOException)
			}, null);
		}

		// Token: 0x0600010B RID: 267 RVA: 0x00006CEC File Offset: 0x00004EEC
		public static string[] GetDirectories(string path, string searchPattern, int retryCount, TimeSpan retryDelay)
		{
			return RetryHelper.Retry<string[]>(() => LongPathDirectory.EnumerateDirectories(path, searchPattern).ToArray<string>(), retryCount, retryDelay, new Type[]
			{
				typeof(IOException)
			}, null);
		}

		// Token: 0x0600010C RID: 268 RVA: 0x00006D3C File Offset: 0x00004F3C
		public static string[] GetFiles(string path, string searchPattern, int retryCount, TimeSpan retryDelay)
		{
			return RetryHelper.Retry<string[]>(() => LongPathDirectory.EnumerateFiles(path, searchPattern).ToArray<string>(), retryCount, retryDelay, new Type[]
			{
				typeof(IOException)
			}, null);
		}

		// Token: 0x0600010D RID: 269 RVA: 0x00006D8C File Offset: 0x00004F8C
		public static string[] GetFiles(string path, string searchPattern, SearchOption searchOption, int retryCount, TimeSpan retryDelay)
		{
			return RetryHelper.Retry<string[]>(() => LongPathDirectory.EnumerateFiles(path, searchPattern, searchOption).ToArray<string>(), retryCount, retryDelay, new Type[]
			{
				typeof(IOException)
			}, null);
		}

		// Token: 0x0600010E RID: 270 RVA: 0x00006DE4 File Offset: 0x00004FE4
		public static IEnumerable<string> EnumerateFiles(string path, string searchPattern, int retryCount, TimeSpan retryDelay)
		{
			return RetryHelper.Retry<IEnumerable<string>>(() => LongPathDirectory.EnumerateFiles(path, searchPattern), retryCount, retryDelay, new Type[]
			{
				typeof(IOException)
			}, null);
		}

		// Token: 0x0600010F RID: 271 RVA: 0x00006E34 File Offset: 0x00005034
		public static IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption, int retryCount, TimeSpan retryDelay)
		{
			return RetryHelper.Retry<IEnumerable<string>>(() => LongPathDirectory.EnumerateFiles(path, searchPattern, searchOption), retryCount, retryDelay, new Type[]
			{
				typeof(IOException)
			}, null);
		}
	}
}
