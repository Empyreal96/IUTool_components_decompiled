using System;
using System.IO;
using System.Text;
using Microsoft.MobileDevices.TestInfrastructure.UtilityLibrary;
using Microsoft.Tools.IO;

namespace Microsoft.Phone.TestInfra.Deployment
{
	// Token: 0x02000017 RID: 23
	public static class ReliableFile
	{
		// Token: 0x06000110 RID: 272 RVA: 0x00006E8C File Offset: 0x0000508C
		public static bool Exists(string path, int retryCount, TimeSpan retryDelay)
		{
			return RetryHelper.Retry<bool>(() => LongPathFile.Exists(path), retryCount, retryDelay, new Type[]
			{
				typeof(IOException)
			}, null);
		}

		// Token: 0x06000111 RID: 273 RVA: 0x00006ED4 File Offset: 0x000050D4
		public static string ReadAllText(string path, int retryCount, TimeSpan retryDelay)
		{
			return RetryHelper.Retry<string>(delegate()
			{
				string result;
				using (FileStream fileStream = LongPathFile.Open(path, FileMode.Open, FileAccess.Read))
				{
					using (StreamReader streamReader = new StreamReader(fileStream, Encoding.UTF8))
					{
						result = streamReader.ReadToEnd();
					}
				}
				return result;
			}, retryCount, retryDelay, new Type[]
			{
				typeof(IOException)
			}, null);
		}

		// Token: 0x06000112 RID: 274 RVA: 0x00006F1C File Offset: 0x0000511C
		public static void WriteAllText(string path, string contents, int retryCount, TimeSpan retryDelay)
		{
			RetryHelper.Retry(delegate()
			{
				using (FileStream fileStream = LongPathFile.Open(path, FileMode.OpenOrCreate, FileAccess.Write))
				{
					using (StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.UTF8))
					{
						streamWriter.Write(contents);
					}
				}
			}, retryCount, retryDelay, new Type[]
			{
				typeof(IOException)
			});
		}
	}
}
