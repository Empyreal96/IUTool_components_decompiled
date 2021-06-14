using System;
using Microsoft.MobileDevices.TestInfrastructure.UtilityLibrary;

namespace Microsoft.Phone.TestInfra.Deployment
{
	// Token: 0x0200000B RID: 11
	public class CachedFileCopier
	{
		// Token: 0x060000A2 RID: 162 RVA: 0x0000417C File Offset: 0x0000237C
		public CachedFileCopier(string cacheRoot)
		{
			bool flag = string.IsNullOrEmpty(cacheRoot);
			if (flag)
			{
				throw new ArgumentNullException("cacheRoot");
			}
			this.cacheManager = new CacheManager(cacheRoot, null);
		}

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x060000A3 RID: 163 RVA: 0x000041BC File Offset: 0x000023BC
		// (set) Token: 0x060000A4 RID: 164 RVA: 0x000041D9 File Offset: 0x000023D9
		public int CopyRetryCount
		{
			get
			{
				return this.cacheManager.CopyRetryCount;
			}
			set
			{
				this.cacheManager.CopyRetryCount = value;
			}
		}

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x060000A5 RID: 165 RVA: 0x000041EC File Offset: 0x000023EC
		// (set) Token: 0x060000A6 RID: 166 RVA: 0x00004209 File Offset: 0x00002409
		public TimeSpan CopyRetryDelay
		{
			get
			{
				return this.cacheManager.CopyRetryDelay;
			}
			set
			{
				this.cacheManager.CopyRetryDelay = value;
			}
		}

		// Token: 0x060000A7 RID: 167 RVA: 0x00004219 File Offset: 0x00002419
		public void CopyFile(string sourceFile, string targetFile)
		{
			this.CopyFile(sourceFile, targetFile, null);
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x00004228 File Offset: 0x00002428
		public void CopyFile(string sourceFile, string targetFile, Action<string, string, string> copyToDestination)
		{
			bool flag = string.IsNullOrEmpty(sourceFile);
			if (flag)
			{
				throw new ArgumentNullException("sourceFile");
			}
			bool flag2 = string.IsNullOrEmpty(targetFile);
			if (flag2)
			{
				throw new ArgumentNullException("targetFile");
			}
			this.cacheManager.AddFileToCache(sourceFile, delegate(string src, string cached)
			{
				this.CopyToDestination(sourceFile, cached, targetFile, copyToDestination);
			});
		}

		// Token: 0x060000A9 RID: 169 RVA: 0x000042AC File Offset: 0x000024AC
		public void CopyFiles(string source, string destination, bool recursive)
		{
			this.CopyFiles(source, destination, "*", recursive, null);
		}

		// Token: 0x060000AA RID: 170 RVA: 0x000042BF File Offset: 0x000024BF
		public void CopyFiles(string source, string destination, string pattern, bool recursive)
		{
			this.CopyFiles(source, destination, pattern, recursive, null);
		}

		// Token: 0x060000AB RID: 171 RVA: 0x000042D0 File Offset: 0x000024D0
		public void CopyFiles(string source, string destination, string pattern, bool recursive, Action<string, string, string> copyToDestination)
		{
			bool flag = string.IsNullOrEmpty(source);
			if (flag)
			{
				throw new ArgumentNullException("source");
			}
			bool flag2 = string.IsNullOrEmpty(destination);
			if (flag2)
			{
				throw new ArgumentNullException("destination");
			}
			source = PathHelper.EndWithDirectorySeparator(source);
			destination = PathHelper.EndWithDirectorySeparator(destination);
			pattern = (string.IsNullOrEmpty(pattern) ? "*" : pattern);
			Action<string, string> callback = delegate(string srcFile, string cachedFile)
			{
				string targetFile = PathHelper.ChangeParent(srcFile, source, destination);
				this.CopyToDestination(srcFile, cachedFile, targetFile, copyToDestination);
			};
			this.cacheManager.AddFilesToCache(source, pattern, recursive, callback);
		}

		// Token: 0x060000AC RID: 172 RVA: 0x00004390 File Offset: 0x00002590
		private void CopyToDestination(string sourceFile, string cachedFile, string targetFile, Action<string, string, string> userAction)
		{
			try
			{
				bool flag = userAction == null;
				if (flag)
				{
					FileCopyHelper.CopyFile(cachedFile, targetFile, this.CopyRetryCount, this.CopyRetryDelay);
				}
				else
				{
					RetryHelper.Retry(delegate()
					{
						userAction(sourceFile, cachedFile, targetFile);
					}, this.CopyRetryCount, this.CopyRetryDelay);
				}
				Logger.Debug("Copied: {0} to {1}", new object[]
				{
					sourceFile,
					targetFile
				});
			}
			catch (Exception ex)
			{
				Logger.Error("Unable to copy file {0}: {1}", new object[]
				{
					sourceFile,
					ex.Message
				});
				throw;
			}
		}

		// Token: 0x04000047 RID: 71
		private readonly CacheManager cacheManager;
	}
}
