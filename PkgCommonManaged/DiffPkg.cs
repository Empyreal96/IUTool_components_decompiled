using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.WindowsPhone.ImageUpdate.Tools;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgCommon
{
	// Token: 0x02000006 RID: 6
	internal class DiffPkg : WPPackageBase, IDiffPkg, IPkgInfo
	{
		// Token: 0x06000031 RID: 49 RVA: 0x000032D8 File Offset: 0x000014D8
		public DiffPkg(DiffPkgManifest diffPkgManifest, PkgManifest pkgManifest, string strCabPath) : base(pkgManifest)
		{
			this.m_strCabPath = strCabPath;
			this.m_diffManifest = diffPkgManifest;
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000032 RID: 50 RVA: 0x000032EF File Offset: 0x000014EF
		public PackageType Type
		{
			get
			{
				return PackageType.Diff;
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000033 RID: 51 RVA: 0x000032F2 File Offset: 0x000014F2
		public PackageStyle Style
		{
			get
			{
				return this.m_pkgManifest.PackageStyle;
			}
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000034 RID: 52 RVA: 0x000032FF File Offset: 0x000014FF
		public VersionInfo PrevVersion
		{
			get
			{
				return this.m_diffManifest.SourceVersion;
			}
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000035 RID: 53 RVA: 0x0000330C File Offset: 0x0000150C
		public byte[] PrevDsmHash
		{
			get
			{
				return this.m_diffManifest.SourceHash;
			}
		}

		// Token: 0x06000036 RID: 54 RVA: 0x0000331C File Offset: 0x0000151C
		public void ExtractFile(string devicePath, string destPath, bool overwriteExistingFiles)
		{
			DiffFileEntry diffFileEntry = null;
			if (!this.m_diffManifest.m_files.TryGetValue(devicePath, out diffFileEntry))
			{
				throw new PackageException("File '{0}' doesn't exist", new object[]
				{
					devicePath
				});
			}
			if (diffFileEntry.DiffType == DiffType.Remove)
			{
				throw new PackageException("File '{0}' is removed, nothing to extract", new object[]
				{
					devicePath
				});
			}
			if (LongPathFile.Exists(destPath) && !overwriteExistingFiles)
			{
				throw new PackageException("Failed to extract file '{0}', file already exists and overwriteExistingFiles not set", new object[]
				{
					destPath
				});
			}
			LongPathFile.Delete(destPath);
			CabApiWrapper.ExtractSelected(this.m_strCabPath, new string[]
			{
				diffFileEntry.CabPath
			}, new string[]
			{
				destPath
			});
		}

		// Token: 0x06000037 RID: 55 RVA: 0x000033C0 File Offset: 0x000015C0
		public void ExtractAll(string outputDir, bool overwriteExistingFiles)
		{
			IEnumerable<DiffFileEntry> source = from x in this.m_diffManifest.m_files.Values
			where x.DiffType != DiffType.Remove
			select x;
			string[] filesToExtract = (from x in source
			select x.CabPath).ToArray<string>();
			string[] array = (from x in source
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
				LongPathFile.Delete(text);
			}
			CabApiWrapper.ExtractSelected(this.m_strCabPath, filesToExtract, array);
		}

		// Token: 0x06000038 RID: 56 RVA: 0x000034A0 File Offset: 0x000016A0
		internal static DiffPkg LoadFromCab(string cabPath)
		{
			if (string.IsNullOrEmpty(cabPath))
			{
				throw new PackageException("Empty CabPath specified for IDiffPkg.LoadFromCab");
			}
			string tempDirectory = FileUtils.GetTempDirectory();
			DiffPkg result;
			try
			{
				DiffPkgManifest diffPkgManifest;
				PkgManifest pkgManifest;
				DiffPkgManifest.Load_Diff_CBS(cabPath, out diffPkgManifest, out pkgManifest);
				DiffPkg diffPkg = new DiffPkg(diffPkgManifest, pkgManifest, cabPath);
				pkgManifest.PackageStyle = PackageStyle.CBS;
				result = diffPkg;
			}
			catch (Exception)
			{
				try
				{
					CabApiWrapper.ExtractSelected(cabPath, tempDirectory, new string[]
					{
						PkgConstants.c_strDsmFile,
						PkgConstants.c_strDiffDsmFile
					});
				}
				catch (Exception innerException)
				{
					throw new PackageException(innerException, "Internal exception when extracting package '{0}'", new object[]
					{
						cabPath
					});
				}
				string text = Path.Combine(tempDirectory, PkgConstants.c_strDiffDsmFile);
				if (!LongPathFile.Exists(text))
				{
					throw new PackageException("No Diff manifest file found when loading package '{0}'", new object[]
					{
						cabPath
					});
				}
				string text2 = Path.Combine(tempDirectory, PkgConstants.c_strDsmFile);
				if (!LongPathFile.Exists(text2))
				{
					throw new PackageException("No package manifest file found when loading package '{0}'", new object[]
					{
						cabPath
					});
				}
				PkgManifest pkgManifest2 = PkgManifest.Load(text2);
				DiffPkg diffPkg2 = new DiffPkg(DiffPkgManifest.Load(text), pkgManifest2, cabPath);
				diffPkg2.Validate();
				pkgManifest2.PackageStyle = PackageStyle.SPKG;
				result = diffPkg2;
			}
			finally
			{
				FileUtils.DeleteTree(tempDirectory);
			}
			return result;
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000039 RID: 57 RVA: 0x000035CC File Offset: 0x000017CC
		public IEnumerable<IDiffEntry> DiffFiles
		{
			get
			{
				return this.m_diffManifest.m_files.Values.Cast<IDiffEntry>();
			}
		}

		// Token: 0x0600003A RID: 58 RVA: 0x000035E4 File Offset: 0x000017E4
		protected void Validate()
		{
			if (!string.Equals(base.Name, this.m_diffManifest.Name, StringComparison.InvariantCultureIgnoreCase))
			{
				throw new PackageException("The package name in DSM ('{0}') doesn't match the name in DDSM ('{1}')", new object[]
				{
					base.Name,
					this.m_diffManifest.Name
				});
			}
			if (base.Version != this.m_diffManifest.TargetVersion)
			{
				throw new PackageException("DSM version ('{0}') and DDSM's Target Version ('{1}') are different", new object[]
				{
					base.Version,
					this.m_diffManifest.TargetVersion
				});
			}
			bool flag = false;
			foreach (IDiffEntry diffEntry in this.DiffFiles)
			{
				DiffFileEntry diffFileEntry = (DiffFileEntry)diffEntry;
				IFileEntry fileEntry = base.FindFile(diffFileEntry.DevicePath);
				if (diffFileEntry.DiffType == DiffType.Remove)
				{
					if (fileEntry != null)
					{
						throw new PackageException("File '{0}' is marked as removed in diff manifest but still shows up in target manifest", new object[]
						{
							diffFileEntry.DevicePath
						});
					}
				}
				else
				{
					if (fileEntry == null)
					{
						throw new PackageException("File '{0}' is in diff manifest but not found in target manifest", new object[]
						{
							diffFileEntry.DevicePath
						});
					}
					if (diffFileEntry.DiffType == DiffType.TargetDSM)
					{
						if (fileEntry.FileType != FileType.Manifest)
						{
							throw new PackageException("Incorrect DSM DevicePath '{0}' in diff manifest", new object[]
							{
								diffFileEntry.DevicePath
							});
						}
						flag = true;
					}
				}
			}
			if (!flag)
			{
				throw new PackageException("No DSM file is found in DiffPackage '{0}'", new object[]
				{
					base.Name
				});
			}
		}

		// Token: 0x04000009 RID: 9
		private string m_strCabPath;

		// Token: 0x0400000A RID: 10
		private DiffPkgManifest m_diffManifest;
	}
}
