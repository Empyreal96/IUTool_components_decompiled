using System;
using System.Globalization;
using System.IO;
using System.Threading;
using Microsoft.MobileDevices.TestInfrastructure.UtilityLibrary;
using Microsoft.WindowsPhone.ImageUpdate.PkgCommon;

namespace Microsoft.Phone.TestInfra.Deployment
{
	// Token: 0x02000029 RID: 41
	public class PackageExtractor
	{
		// Token: 0x060001C8 RID: 456 RVA: 0x0000A9B4 File Offset: 0x00008BB4
		public string ExtractPackage(string packagePath, string extractionPath, out bool newPackage)
		{
			bool flag = string.IsNullOrEmpty(packagePath);
			if (flag)
			{
				throw new ArgumentNullException("packagePath");
			}
			bool flag2 = string.IsNullOrEmpty(extractionPath);
			if (flag2)
			{
				throw new ArgumentNullException("extractionPath");
			}
			bool flag3 = !File.Exists(packagePath);
			if (flag3)
			{
				throw new FileNotFoundException(string.Format(CultureInfo.InvariantCulture, "Package does not exist: {0}", new object[]
				{
					packagePath
				}));
			}
			extractionPath = new DirectoryInfo(extractionPath).FullName;
			string fileNameWithoutExtension = PathHelper.GetFileNameWithoutExtension(packagePath, ".spkg");
			string text = this.GenerateShortName(fileNameWithoutExtension);
			string text2 = Path.Combine(extractionPath, text);
			using (Mutex mutex = this.CreatePackageExtractionLock(packagePath))
			{
				Logger.Debug("Locking package for extraction", new object[0]);
				mutex.Acquire(TimeSpan.FromMinutes(15.0));
				try
				{
					long num = new FileInfo(packagePath).LastWriteTimeUtc.ToBinary();
					newPackage = (num != this.GetDeployedPackageTime(extractionPath, text));
					bool flag4 = newPackage;
					if (flag4)
					{
						Directory.CreateDirectory(text2);
						IPkgInfo pkgInfo = RetryHelper.Retry<IPkgInfo>(() => Package.LoadFromCab(packagePath), 3, PackageExtractor.DefaultRetryDelay);
						string text3 = text2;
						bool flag5 = pkgInfo.Partition.Equals("Data", StringComparison.OrdinalIgnoreCase);
						if (flag5)
						{
							text3 = PathHelper.Combine(text3, "data");
						}
						pkgInfo.ExtractAll("\\\\?\\" + text3, true);
						this.SetDeployedPackageTime(extractionPath, text, num);
						Logger.Debug("Extracted {0}, {1} Files", new object[]
						{
							fileNameWithoutExtension,
							pkgInfo.FileCount
						});
					}
					else
					{
						Logger.Debug("Package {0} is already extracted", new object[]
						{
							fileNameWithoutExtension
						});
					}
				}
				finally
				{
					mutex.ReleaseMutex();
				}
			}
			return text2;
		}

		// Token: 0x060001C9 RID: 457 RVA: 0x0000ABDC File Offset: 0x00008DDC
		private long GetDeployedPackageTime(string path, string packageShortName)
		{
			string path2 = Path.Combine(path, packageShortName + ".metadata");
			bool flag = !File.Exists(path2);
			long result;
			if (flag)
			{
				result = 0L;
			}
			else
			{
				string s = ReliableFile.ReadAllText(path2, 3, PackageExtractor.DefaultRetryDelay);
				long num;
				result = (long.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out num) ? num : 0L);
			}
			return result;
		}

		// Token: 0x060001CA RID: 458 RVA: 0x0000AC38 File Offset: 0x00008E38
		private void SetDeployedPackageTime(string path, string packageShortName, long value)
		{
			string path2 = Path.Combine(path, packageShortName + ".metadata");
			ReliableFile.WriteAllText(path2, value.ToString(CultureInfo.InvariantCulture), 3, PackageExtractor.DefaultRetryDelay);
		}

		// Token: 0x060001CB RID: 459 RVA: 0x0000AC74 File Offset: 0x00008E74
		private string GenerateShortName(string package)
		{
			return package.Trim().ToLowerInvariant().GetHashCode().ToString(CultureInfo.InvariantCulture);
		}

		// Token: 0x060001CC RID: 460 RVA: 0x0000ACA4 File Offset: 0x00008EA4
		private Mutex CreatePackageExtractionLock(string packagePath)
		{
			return new Mutex(false, string.Format(CultureInfo.InvariantCulture, "PE_{0}", new object[]
			{
				packagePath.ToLowerInvariant().GetHashCode()
			}));
		}

		// Token: 0x040000C3 RID: 195
		private const string PackgeExtractionStringFormat = "PE_{0}";

		// Token: 0x040000C4 RID: 196
		private const int DefaultRetryCount = 3;

		// Token: 0x040000C5 RID: 197
		private static readonly TimeSpan DefaultRetryDelay = TimeSpan.FromMilliseconds(300.0);
	}
}
