using System;
using System.IO;
using System.Linq;
using Microsoft.WindowsPhone.ImageUpdate.Tools;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgCommon
{
	// Token: 0x0200000C RID: 12
	internal class WPCanonicalPackage : WPPackageBase, IPkgInfo
	{
		// Token: 0x0600006D RID: 109 RVA: 0x00003F04 File Offset: 0x00002104
		protected WPCanonicalPackage(PkgManifest pkgManifest) : base(pkgManifest)
		{
		}

		// Token: 0x0600006E RID: 110 RVA: 0x00003F0D File Offset: 0x0000210D
		protected virtual void ExtractFiles(FileEntryBase[] files, string[] targetPaths)
		{
			throw new NotImplementedException();
		}

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x0600006F RID: 111 RVA: 0x00003F14 File Offset: 0x00002114
		public PackageType Type
		{
			get
			{
				if (!this.m_pkgManifest.IsRemoval)
				{
					return PackageType.Canonical;
				}
				return PackageType.Removal;
			}
		}

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x06000070 RID: 112 RVA: 0x000032F2 File Offset: 0x000014F2
		public PackageStyle Style
		{
			get
			{
				return this.m_pkgManifest.PackageStyle;
			}
		}

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x06000071 RID: 113 RVA: 0x00003F26 File Offset: 0x00002126
		public VersionInfo PrevVersion
		{
			get
			{
				throw new NotImplementedException("PreVersion is not available for canonical package");
			}
		}

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x06000072 RID: 114 RVA: 0x00003F32 File Offset: 0x00002132
		public byte[] PrevDsmHash
		{
			get
			{
				return null;
			}
		}

		// Token: 0x06000073 RID: 115 RVA: 0x00003F38 File Offset: 0x00002138
		public void ExtractFile(string devicePath, string destPath, bool overwriteExistingFiles)
		{
			FileEntryBase fileEntryBase = base.FindFile(devicePath) as FileEntryBase;
			if (fileEntryBase == null)
			{
				throw new PackageException("Failed to extract file '{0}' to '{1}': file specified doesn't exist in package", new object[]
				{
					devicePath,
					destPath
				});
			}
			if (LongPathFile.Exists(destPath) && !overwriteExistingFiles)
			{
				throw new PackageException("Failed to extract file '{0}', file already exists and overwriteExistingFiles not set", new object[]
				{
					destPath
				});
			}
			this.ExtractFiles(new FileEntryBase[]
			{
				fileEntryBase
			}, new string[]
			{
				destPath
			});
		}

		// Token: 0x06000074 RID: 116 RVA: 0x00003FA8 File Offset: 0x000021A8
		public void ExtractAll(string outputDir, bool overwriteExistingFiles)
		{
			string[] array = (from x in base.Files
			select outputDir + x.DevicePath).ToArray<string>();
			foreach (string text in array)
			{
				if (LongPathFile.Exists(text) && !overwriteExistingFiles)
				{
					throw new PackageException("Failed to extract file '{0}', file already exists and overwriteExistingFiles not set", new object[]
					{
						text
					});
				}
			}
			this.ExtractFiles(base.Files.Cast<FileEntryBase>().ToArray<FileEntryBase>(), array);
		}

		// Token: 0x06000075 RID: 117 RVA: 0x00004030 File Offset: 0x00002230
		internal static WPExtractedPackage ExtractAndLoad(string cabPath, string outputDir)
		{
			try
			{
				CabApiWrapper.Extract(cabPath, outputDir);
			}
			catch (CabException innerException)
			{
				throw new PackageException(innerException, "ExtractAndLoad: Failed to extract contents in cab file '{0}' to '{1}'", new object[]
				{
					cabPath,
					outputDir
				});
			}
			catch (IOException innerException2)
			{
				throw new PackageException(innerException2, "ExtractAndLoad: Failed to extract contents in cab file '{0}' to '{1}'", new object[]
				{
					cabPath,
					outputDir
				});
			}
			return WPExtractedPackage.Load(outputDir);
		}

		// Token: 0x06000076 RID: 118 RVA: 0x0000409C File Offset: 0x0000229C
		internal static WPCabPackage LoadFromCab(string cabPath)
		{
			return WPCabPackage.Load(cabPath);
		}

		// Token: 0x06000077 RID: 119 RVA: 0x000040A4 File Offset: 0x000022A4
		internal static WPInstalledPackage LoadFromInstallationDir(string manifestPath, string dirRoot)
		{
			return WPInstalledPackage.Load(dirRoot, manifestPath);
		}
	}
}
