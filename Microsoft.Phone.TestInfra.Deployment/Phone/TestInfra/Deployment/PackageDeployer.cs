using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.Phone.TestInfra.Deployment
{
	// Token: 0x02000023 RID: 35
	public class PackageDeployer : IDisposable
	{
		// Token: 0x06000185 RID: 389 RVA: 0x00009730 File Offset: 0x00007930
		public PackageDeployer(PackageDeployerParameters packageDeployerParameters)
		{
			this.LogFile = (Path.IsPathRooted(packageDeployerParameters.LogFile) ? packageDeployerParameters.LogFile : Path.Combine(packageDeployerParameters.OutputPath, packageDeployerParameters.LogFile));
			Logger.Configure(packageDeployerParameters.ConsoleTraceLevel, packageDeployerParameters.FileTraceLevel, this.LogFile, false);
			char[] separator = new char[]
			{
				';'
			};
			HashSet<string> hashSet;
			if (!string.IsNullOrWhiteSpace(packageDeployerParameters.Packages))
			{
				hashSet = new HashSet<string>(from x in packageDeployerParameters.Packages.Split(separator, StringSplitOptions.RemoveEmptyEntries)
				where !string.IsNullOrWhiteSpace(x)
				select x, StringComparer.InvariantCultureIgnoreCase);
			}
			else
			{
				hashSet = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
			}
			this.packagesToInstall = hashSet;
			bool flag = !string.IsNullOrWhiteSpace(packageDeployerParameters.PackageFile);
			if (flag)
			{
				bool flag2 = ReliableFile.Exists(packageDeployerParameters.PackageFile, Settings.Default.ShareAccessRetryCount, TimeSpan.FromMilliseconds((double)Settings.Default.ShareAccessRetryDelayInMs));
				if (!flag2)
				{
					throw new FileNotFoundException(packageDeployerParameters.PackageFile);
				}
				foreach (string item in from line in File.ReadLines(packageDeployerParameters.PackageFile)
				select line.Trim() into line
				where !string.IsNullOrWhiteSpace(line)
				select line)
				{
					this.packagesToInstall.Add(item);
				}
			}
			string text = string.Empty;
			bool flag3 = string.IsNullOrWhiteSpace(packageDeployerParameters.CacheRoot);
			if (flag3)
			{
				text = packageDeployerParameters.OutputPath;
			}
			else
			{
				text = this.FindBestCacheRoot(packageDeployerParameters.CacheRoot, packageDeployerParameters.OutputPath);
				try
				{
					bool flag4 = !Directory.Exists(text);
					if (flag4)
					{
						Directory.CreateDirectory(text);
					}
				}
				catch (Exception ex)
				{
					Logger.Warning("Exception creating cache directory {0}: {1} ", new object[]
					{
						text,
						ex.Message
					});
				}
			}
			IEnumerable<string> rootPaths;
			IEnumerable<string> alternateRootPaths;
			this.GetAllRootPaths(packageDeployerParameters.RootPaths, packageDeployerParameters.AlternateRoots, out rootPaths, out alternateRootPaths);
			this.packageManagerConfig = new PackageManagerConfiguration
			{
				ExpiresIn = packageDeployerParameters.ExpiresIn,
				RootPaths = rootPaths,
				AlternateRootPaths = alternateRootPaths,
				OutputPath = packageDeployerParameters.OutputPath,
				CachePath = text,
				PackagesExtractionCachePath = Path.Combine(text, "p2"),
				Macros = this.GetMacros(packageDeployerParameters.Macros),
				SourceRootIsVolatile = packageDeployerParameters.SourceRootIsVolatile,
				RecursiveDeployment = packageDeployerParameters.Recurse
			};
		}

		// Token: 0x17000083 RID: 131
		// (get) Token: 0x06000186 RID: 390 RVA: 0x00009A28 File Offset: 0x00007C28
		// (set) Token: 0x06000187 RID: 391 RVA: 0x00009A30 File Offset: 0x00007C30
		public string LogFile { get; private set; }

		// Token: 0x06000188 RID: 392 RVA: 0x00009A3C File Offset: 0x00007C3C
		public PackageDeployerOutput Run()
		{
			Logger.Info("DeployTest Version {0}", new object[]
			{
				Settings.Default.Version
			});
			Logger.Debug("OutputDirectory = {0}", new object[]
			{
				this.packageManagerConfig.OutputPath
			});
			Logger.Debug("CacheRoot = {0}", new object[]
			{
				this.packageManagerConfig.CachePath
			});
			DateTime utcNow = DateTime.UtcNow;
			TelemetryLogging.LogEvent(TelemetryLogging.SpkgDeployTelemetryEventType.SpkgDeployStart, this.packagesToInstall, this.packageManagerConfig.RootPaths, new string[0], string.Empty, utcNow);
			PackageDeployerOutput packageDeployerOutput = new PackageDeployerOutput();
			int num = 0;
			string text = string.Empty;
			try
			{
				PathCleaner.CleanupExpiredDirectories(this.filePurgingTimeout);
				bool flag = this.packagesToInstall.Count != 0;
				if (flag)
				{
					PackageManager packageManager = new PackageManager(this.packageManagerConfig);
					packageManager.DeployPackages(this.packagesToInstall);
					num += packageManager.ErrorCount;
					ConfigCommandAggregator configCommandAggregator = new ConfigCommandAggregator();
					packageDeployerOutput.ConfigurationCommands = configCommandAggregator.GetConfigCommands(packageManager.DeployedPackages, this.packageManagerConfig.OutputPath);
				}
				else
				{
					Logger.Info("No packages specified to deploy", new object[0]);
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Unhandled exception: {0}", new object[]
				{
					ex
				});
				int num2 = num;
				num = num2 + 1;
			}
			DateTime utcNow2 = DateTime.UtcNow;
			bool flag2 = num != 0;
			if (flag2)
			{
				text = string.Format("{0} Errors found, view logfile for more info.", num);
				Logger.Info(text, new object[0]);
				TelemetryLogging.LogEvent(TelemetryLogging.SpkgDeployTelemetryEventType.SpkgErrorOccurred, this.packagesToInstall, this.packageManagerConfig.RootPaths, new string[0], text, utcNow2);
			}
			else
			{
				string message = string.Format("Total time spent: {0}.", utcNow2 - utcNow);
				TelemetryLogging.LogEvent(TelemetryLogging.SpkgDeployTelemetryEventType.SpkgDeployFinished, this.packagesToInstall, this.packageManagerConfig.RootPaths, new string[0], message, utcNow2);
			}
			Logger.Info("Done package deployment.", new object[0]);
			Logger.Close();
			packageDeployerOutput.Success = (num == 0);
			packageDeployerOutput.ErrorMessage = text;
			return packageDeployerOutput;
		}

		// Token: 0x06000189 RID: 393 RVA: 0x00009C5C File Offset: 0x00007E5C
		public void Dispose()
		{
			this.Dispose(true);
		}

		// Token: 0x0600018A RID: 394 RVA: 0x00009C68 File Offset: 0x00007E68
		protected virtual void Dispose(bool disposing)
		{
			bool flag = !this.disposedValue;
			if (flag)
			{
				if (disposing)
				{
					Logger.Close();
				}
				this.disposedValue = true;
			}
		}

		// Token: 0x0600018B RID: 395 RVA: 0x00009C9C File Offset: 0x00007E9C
		private void GetAllRootPaths(string rootPaths, string alternateRoots, out IEnumerable<string> outRootPathSet, out IEnumerable<string> outAlternateRootPathSet)
		{
			char[] separator = new char[]
			{
				';'
			};
			HashSet<string> hashSet = new HashSet<string>(from x in rootPaths.Split(separator, StringSplitOptions.RemoveEmptyEntries)
			where !string.IsNullOrWhiteSpace(x)
			select Path.GetFullPath(x), StringComparer.InvariantCultureIgnoreCase);
			HashSet<string> hashSet2;
			if (!string.IsNullOrWhiteSpace(alternateRoots))
			{
				hashSet2 = new HashSet<string>(from x in alternateRoots.Split(separator, StringSplitOptions.RemoveEmptyEntries)
				where !string.IsNullOrWhiteSpace(x)
				select Path.GetFullPath(x), StringComparer.InvariantCultureIgnoreCase);
			}
			else
			{
				hashSet2 = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
			}
			HashSet<string> hashSet3 = hashSet2;
			string environmentVariable = Environment.GetEnvironmentVariable("_WINPHONEROOT");
			string environmentVariable2 = Environment.GetEnvironmentVariable("RazzleDataPath");
			bool flag = !string.IsNullOrWhiteSpace(environmentVariable2) && string.IsNullOrWhiteSpace(environmentVariable);
			if (flag)
			{
				hashSet.Add(Environment.GetEnvironmentVariable("_NTTREE"));
			}
			else
			{
				bool flag2 = string.IsNullOrWhiteSpace(environmentVariable2) && !string.IsNullOrWhiteSpace(environmentVariable);
				if (flag2)
				{
					hashSet.Add(Environment.GetEnvironmentVariable("BINARY_ROOT"));
				}
			}
			bool flag3 = hashSet.Count == 0;
			if (flag3)
			{
				throw new ArgumentNullException("rootPathSet");
			}
			Action<HashSet<string>> action = delegate(HashSet<string> rooPaths)
			{
				HashSet<string> hashSet4 = new HashSet<string>();
				foreach (string text in rooPaths)
				{
					bool flag6 = !ReliableDirectory.Exists(text, Settings.Default.ShareAccessRetryCount, TimeSpan.FromMilliseconds((double)Settings.Default.ShareAccessRetryDelayInMs));
					if (flag6)
					{
						Logger.Info("Path {0} is not accessible, remove it from the root path set.", new object[]
						{
							text
						});
						hashSet4.Add(text);
					}
				}
				rooPaths.ExceptWith(hashSet4);
			};
			action(hashSet);
			bool flag4 = hashSet.Count == 0;
			if (flag4)
			{
				throw new InvalidDataException("No path specified in the root path set is usable.");
			}
			HashSet<string> paths = PathHelper.AddRelatedPathsToSet(hashSet);
			outRootPathSet = PathHelper.SortPathSetOnPathType(paths);
			HashSet<string> paths2 = new HashSet<string>();
			outAlternateRootPathSet = null;
			bool flag5 = hashSet3 != null && hashSet3.Count<string>() > 0;
			if (flag5)
			{
				action(hashSet3);
				paths2 = PathHelper.AddRelatedPathsToSet(hashSet3);
				IEnumerable<string> outRootPathSetCopy = outRootPathSet;
				outAlternateRootPathSet = from x in PathHelper.SortPathSetOnPathType(paths2)
				where !outRootPathSetCopy.Contains(x)
				select x;
			}
		}

		// Token: 0x0600018C RID: 396 RVA: 0x00009EC4 File Offset: 0x000080C4
		private Dictionary<string, string> GetMacros(string userMacros)
		{
			Dictionary<string, string> macros = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			Action<string> action = delegate(string macrosInString)
			{
				IEnumerable<string[]> enumerable = (from x in macrosInString.Split(new char[]
				{
					';'
				}, StringSplitOptions.RemoveEmptyEntries)
				where x.Split(new char[]
				{
					'='
				}, StringSplitOptions.RemoveEmptyEntries).Length == 2
				select x).Select(delegate(string x)
				{
					int num = x.IndexOf('=');
					return new string[]
					{
						x.Substring(0, num),
						x.Substring(num + 1)
					};
				});
				foreach (string[] array in enumerable)
				{
					bool flag2 = !string.IsNullOrWhiteSpace(array[0]);
					if (flag2)
					{
						string key = array[0];
						string value = array[1];
						bool flag3 = macros.ContainsKey(key);
						if (flag3)
						{
							macros[key] = value;
						}
						else
						{
							macros.Add(key, value);
						}
					}
				}
			};
			action(Settings.Default.DefaultMacros);
			bool flag = !string.IsNullOrWhiteSpace(userMacros);
			if (flag)
			{
				action(userMacros);
			}
			return macros;
		}

		// Token: 0x0600018D RID: 397 RVA: 0x00009F28 File Offset: 0x00008128
		private string FindBestCacheRoot(string currentCacheRoot, string fallbackLocation)
		{
			try
			{
				string pathRoot = Path.GetPathRoot(currentCacheRoot);
				DriveInfo[] drives = DriveInfo.GetDrives();
				DriveInfo driveInfo = null;
				foreach (DriveInfo driveInfo2 in drives)
				{
					bool flag = driveInfo2.DriveType == DriveType.Fixed && driveInfo2.IsReady;
					if (flag)
					{
						bool flag2 = driveInfo2.Name == pathRoot;
						if (flag2)
						{
							driveInfo = driveInfo2;
							break;
						}
						bool flag3 = driveInfo == null || driveInfo2.AvailableFreeSpace > driveInfo.AvailableFreeSpace;
						if (flag3)
						{
							driveInfo = driveInfo2;
						}
					}
				}
				bool flag4 = driveInfo != null;
				if (flag4)
				{
					string text = currentCacheRoot.Replace(pathRoot, driveInfo.Name);
					bool flag5 = text != currentCacheRoot;
					if (flag5)
					{
						Logger.Warning("Relocated cache directory to {0}", new object[]
						{
							text
						});
					}
					return text;
				}
			}
			catch (Exception ex)
			{
				Logger.Warning("Exception relocating the cache directory: ", new object[]
				{
					ex.Message
				});
			}
			Logger.Warning("Unable to relocate the cache directory.", new object[0]);
			return fallbackLocation;
		}

		// Token: 0x0400009D RID: 157
		private readonly TimeSpan filePurgingTimeout = TimeSpan.FromMinutes(10.0);

		// Token: 0x0400009E RID: 158
		private HashSet<string> packagesToInstall;

		// Token: 0x0400009F RID: 159
		private PackageManagerConfiguration packageManagerConfig;

		// Token: 0x040000A0 RID: 160
		private bool disposedValue = false;
	}
}
