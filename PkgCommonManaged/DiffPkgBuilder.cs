using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgCommon
{
	// Token: 0x02000004 RID: 4
	internal static class DiffPkgBuilder
	{
		// Token: 0x0600000C RID: 12 RVA: 0x00002230 File Offset: 0x00000430
		private static DiffFileEntry CreateDiffEntry(FileEntry source, FileEntry target, string outputDir, CpuId cpuType)
		{
			DiffFileEntry result;
			if (target == null)
			{
				result = DiffPkgBuilder.CreateRemoveEntry(source);
			}
			else if (source == null)
			{
				result = DiffPkgBuilder.CreateCanonicalEntry(target, outputDir);
			}
			else if (target.FileType == FileType.Manifest)
			{
				result = DiffPkgBuilder.CreateDsmEntry(target, outputDir);
			}
			else
			{
				result = DiffPkgBuilder.CreateDeltaEntry(source, target, outputDir, cpuType);
			}
			return result;
		}

		// Token: 0x0600000D RID: 13 RVA: 0x00002276 File Offset: 0x00000476
		private static DiffFileEntry CreateDsmEntry(FileEntry target, string outputDir)
		{
			return new DiffFileEntry(FileType.Manifest, DiffType.TargetDSM, target.DevicePath, target.SourcePath);
		}

		// Token: 0x0600000E RID: 14 RVA: 0x0000228B File Offset: 0x0000048B
		private static DiffFileEntry CreateRemoveEntry(IFileEntry source)
		{
			return new DiffFileEntry(source.FileType, DiffType.Remove, source.DevicePath, null);
		}

		// Token: 0x0600000F RID: 15 RVA: 0x000022A0 File Offset: 0x000004A0
		private static DiffFileEntry CreateCanonicalEntry(FileEntry target, string outputDir)
		{
			return new DiffFileEntry(target.FileType, DiffType.Canonical, target.DevicePath, target.SourcePath);
		}

		// Token: 0x06000010 RID: 16 RVA: 0x000022BC File Offset: 0x000004BC
		private static DiffFileEntry CreateDeltaEntry(FileEntry source, FileEntry target, string outputDir, CpuId cpuType)
		{
			if (source == null)
			{
				throw new PackageException("Can not create a Delta type DiffFileEntry with null source file entry");
			}
			if (target == null)
			{
				throw new PackageException("Can not create a Delta type DiffFileEntry with null target file entry");
			}
			if (string.Compare(source.DevicePath, target.DevicePath, StringComparison.InvariantCultureIgnoreCase) != 0)
			{
				throw new PackageException("To create a Delta type DiffFileEntry the source and target file must have same DevicePath");
			}
			DiffPkgBuilder.VerifyFileForDiff(source, target);
			if (string.IsNullOrEmpty(source.SourcePath))
			{
				throw new PackageException("SoucePath of the source file entry is empty");
			}
			if (string.IsNullOrEmpty(target.SourcePath))
			{
				throw new PackageException("SourcePath of the target file entry is empty");
			}
			if (!LongPathFile.Exists(source.SourcePath))
			{
				throw new PackageException("Source file '{0}' doesn't exist", new object[]
				{
					source.SourcePath
				});
			}
			if (!LongPathFile.Exists(target.SourcePath))
			{
				throw new PackageException("Target file '{0}' doesn't exist", new object[]
				{
					target.SourcePath
				});
			}
			if (File.ReadAllBytes(source.SourcePath).SequenceEqual(File.ReadAllBytes(target.SourcePath)))
			{
				if (source.Attributes != target.Attributes)
				{
					return DiffPkgBuilder.CreateCanonicalEntry(target, outputDir);
				}
				return null;
			}
			else
			{
				if (DiffPkgBuilder.IsCanonicalNeeded(target))
				{
					return DiffPkgBuilder.CreateCanonicalEntry(target, outputDir);
				}
				string text = Path.Combine(outputDir, target.CabPath);
				DiffPkgBuilder.CreateDelta(source, target, text, cpuType);
				return new DiffFileEntry(target.FileType, DiffType.Delta, target.DevicePath, text);
			}
		}

		// Token: 0x06000011 RID: 17 RVA: 0x000023F8 File Offset: 0x000005F8
		private static void CreateDelta(FileEntry source, FileEntry target, string deltaPath, CpuId cpuType)
		{
			ulong num = (ulong)File.GetCreationTimeUtc(source.SourcePath).ToFileTimeUtc();
			if (!MSDeltaInterOp.CreateDeltaW(DELTA_FILE_TYPE.DELTA_FILE_TYPE_RAW, DELTA_FLAG_TYPE.DELTA_FLAG_NONE, DELTA_FLAG_TYPE.DELTA_FLAG_NONE, source.SourcePath, target.SourcePath, null, null, DiffPkgBuilder.dummyInput, ref num, 32U, deltaPath))
			{
				throw new PackageException("MSDeltaInterOp.CreateDelta failed with error code {0} when creating delta from '{1}' to '{2}'", new object[]
				{
					Marshal.GetLastWin32Error(),
					source.SourcePath,
					target.SourcePath
				});
			}
		}

		// Token: 0x06000012 RID: 18 RVA: 0x00002470 File Offset: 0x00000670
		private static SortedDictionary<string, DiffPkgBuilder.Pair<FileEntry, FileEntry>> BuildFileDictionary(WPExtractedPackage source, WPExtractedPackage target)
		{
			SortedDictionary<string, DiffPkgBuilder.Pair<FileEntry, FileEntry>> sortedDictionary = new SortedDictionary<string, DiffPkgBuilder.Pair<FileEntry, FileEntry>>(StringComparer.InvariantCultureIgnoreCase);
			foreach (IFileEntry fileEntry in source.Files)
			{
				FileEntry fileEntry2 = (FileEntry)fileEntry;
				sortedDictionary.Add(fileEntry2.DevicePath, new DiffPkgBuilder.Pair<FileEntry, FileEntry>(fileEntry2, null));
			}
			foreach (IFileEntry fileEntry3 in target.Files)
			{
				FileEntry fileEntry4 = (FileEntry)fileEntry3;
				DiffPkgBuilder.Pair<FileEntry, FileEntry> pair = null;
				if (!sortedDictionary.TryGetValue(fileEntry4.DevicePath, out pair))
				{
					sortedDictionary.Add(fileEntry4.DevicePath, new DiffPkgBuilder.Pair<FileEntry, FileEntry>(null, fileEntry4));
				}
				else
				{
					pair.Second = fileEntry4;
				}
			}
			return sortedDictionary;
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00002544 File Offset: 0x00000744
		private static void VerifyPkgForDiff(WPExtractedPackage source, WPExtractedPackage target)
		{
			if (!string.Equals(source.Name, target.Name, StringComparison.InvariantCultureIgnoreCase))
			{
				throw new PackageException("Source and Target have different name. Source:'{0}' Target:'{1}'", new object[]
				{
					source.Name,
					target.Name
				});
			}
			if (!(source.Version < target.Version))
			{
				throw new PackageException("Target version '{1}' must be higher than Source's '{0}'", new object[]
				{
					source.Version,
					target.Version
				});
			}
			if (source.CpuType != target.CpuType)
			{
				throw new PackageException("Source '{0}' and Target '{1}' have different CPU", new object[]
				{
					source.CpuType,
					target.CpuType
				});
			}
			if (!string.Equals(source.Partition, target.Partition, StringComparison.InvariantCultureIgnoreCase))
			{
				throw new PackageException("Source '{0}'and Target '{1}' have different Partition", new object[]
				{
					source.Partition,
					target.Partition
				});
			}
			if (!string.Equals(source.Platform, target.Platform, StringComparison.InvariantCultureIgnoreCase))
			{
				throw new PackageException("Source '{0}' and Target '{1}' have different Platform", new object[]
				{
					source.Platform,
					target.Platform
				});
			}
			if (source.PackageStyle != target.PackageStyle)
			{
				throw new PackageException("Source '{0}' and Target '{1}' have different Package Style", new object[]
				{
					source.PackageStyle,
					target.PackageStyle
				});
			}
		}

		// Token: 0x06000014 RID: 20 RVA: 0x000026AC File Offset: 0x000008AC
		private static void VerifyFileForDiff(IFileEntry source, IFileEntry target)
		{
			if (source.FileType == FileType.BinaryPartition && target.FileType != FileType.BinaryPartition)
			{
				throw new PackageException("File '{0}': changing file type from BinaryParitition to type '{1}' is not allowed", new object[]
				{
					source.DevicePath,
					target.FileType
				});
			}
			if (source.FileType != FileType.BinaryPartition && target.FileType == FileType.BinaryPartition)
			{
				throw new PackageException("File '{0}': changing file type from type '{1}' to BinaryParitition is not allowed", new object[]
				{
					source.DevicePath,
					source.FileType
				});
			}
		}

		// Token: 0x06000015 RID: 21 RVA: 0x00002730 File Offset: 0x00000930
		private static bool IsCanonicalNeeded(IFileEntry target)
		{
			List<string> source = new List<string>
			{
				"\\Windows\\System32\\config\\FP",
				"\\Windows\\System32\\config\\BBI",
				"\\Programs\\MobileUI\\SendToMediaLib.exe",
				"\\Windows\\ImageUpdate\\OEMDevicePlatform.xml",
				"\\Windows\\System32\\Tasks\\Microsoft\\Windows\\Clip\\License Validation"
			};
			return target.FileType == FileType.BinaryPartition || target.Size >= 8388608UL || source.Contains(target.DevicePath, StringComparer.InvariantCultureIgnoreCase);
		}

		// Token: 0x06000016 RID: 22 RVA: 0x000027A4 File Offset: 0x000009A4
		public static DiffPkgManifest CreateDiff(WPExtractedPackage source, WPExtractedPackage target, string outputDir)
		{
			DiffPkgBuilder.VerifyPkgForDiff(source, target);
			SortedDictionary<string, DiffPkgBuilder.Pair<FileEntry, FileEntry>> sortedDictionary = DiffPkgBuilder.BuildFileDictionary(source, target);
			DiffPkgManifest diffPkgManifest = new DiffPkgManifest();
			diffPkgManifest.Name = target.Name;
			diffPkgManifest.SourceVersion = source.Version;
			diffPkgManifest.TargetVersion = target.Version;
			foreach (KeyValuePair<string, DiffPkgBuilder.Pair<FileEntry, FileEntry>> keyValuePair in sortedDictionary)
			{
				DiffFileEntry diffFileEntry = DiffPkgBuilder.CreateDiffEntry(keyValuePair.Value.First, keyValuePair.Value.Second, outputDir, target.CpuType);
				if (diffFileEntry != null)
				{
					diffPkgManifest.AddFileEntry(diffFileEntry);
				}
			}
			FileEntry fileEntry = source.GetDsmFile() as FileEntry;
			diffPkgManifest.SourceHash = PackageTools.CalculateFileHash(fileEntry.SourcePath);
			return diffPkgManifest;
		}

		// Token: 0x06000017 RID: 23 RVA: 0x00002874 File Offset: 0x00000A74
		private static void SaveDiffPkg(DiffPkgManifest diffPkgManifest, string cabPath)
		{
			if (string.IsNullOrEmpty(cabPath))
			{
				throw new ArgumentNullException("cabPath", "The path for cabinet file is null or empty");
			}
			LongPathFile.Delete(cabPath);
			string tempFile = FileUtils.GetTempFile();
			try
			{
				CabArchiver cabArchiver = new CabArchiver();
				diffPkgManifest.Save(tempFile);
				foreach (DiffFileEntry diffFileEntry in diffPkgManifest.Files)
				{
					if (diffFileEntry.DiffType != DiffType.Remove && diffFileEntry.DiffType != DiffType.TargetDSM)
					{
						cabArchiver.AddFile(diffFileEntry.CabPath, diffFileEntry.SourcePath);
					}
				}
				cabArchiver.AddFileToFront(PkgConstants.c_strDiffDsmFile, tempFile);
				cabArchiver.AddFileToFront(PkgConstants.c_strDsmFile, diffPkgManifest.TargetDsmFile.SourcePath);
				cabArchiver.Save(cabPath, Package.DefaultCompressionType);
				try
				{
					PackageTools.SignFile(cabPath);
				}
				catch (Exception innerException)
				{
					throw new PackageException(innerException, "Failed to sign generated package: {0}", new object[]
					{
						cabPath
					});
				}
			}
			finally
			{
				LongPathFile.Delete(tempFile);
			}
		}

		// Token: 0x06000018 RID: 24
		[DllImport("CbsXpdGen.dll")]
		public static extern int GenerateCbs2CbsXpdEx([MarshalAs(UnmanagedType.LPWStr)] string Source, [MarshalAs(UnmanagedType.LPWStr)] string Target, [MarshalAs(UnmanagedType.LPWStr)] string SourcePdbPath, [MarshalAs(UnmanagedType.LPWStr)] string TargetPdbPath, [MarshalAs(UnmanagedType.U4)] uint deltaThresholdMB, [MarshalAs(UnmanagedType.LPWStr)] string PackageName, [MarshalAs(UnmanagedType.LPWStr)] string OutputFolder);

		// Token: 0x06000019 RID: 25 RVA: 0x00002964 File Offset: 0x00000B64
		private static void CallCbsXpdGen(string sourceDir, string targetDir, uint deltaThresholdMB, string outputDir, string outputCab)
		{
			if (string.IsNullOrEmpty(sourceDir))
			{
				throw new ArgumentNullException("sourceDir", "The path for source is null or empty");
			}
			if (string.IsNullOrEmpty(targetDir))
			{
				throw new ArgumentNullException("targetDir", "The path for target is null or empty");
			}
			if (string.IsNullOrEmpty(outputDir))
			{
				throw new ArgumentNullException("outputDir", "The path for output is null or empty");
			}
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(outputCab);
			NativeMethods.CheckHResult(DiffPkgBuilder.GenerateCbs2CbsXpdEx(sourceDir, targetDir, ".", ".", deltaThresholdMB, fileNameWithoutExtension, outputDir), "GenerateCbs2CbsXpdEx");
		}

		// Token: 0x0600001A RID: 26 RVA: 0x000029E0 File Offset: 0x00000BE0
		internal static DiffError CreateDiff(string sourceCab, string targetCab, DiffOptions options, Dictionary<DiffOptions, object> optionValues, string outputCab)
		{
			string tempDirectory = FileUtils.GetTempDirectory();
			DiffError result;
			try
			{
				string text = Path.Combine(tempDirectory, "source");
				LongPathDirectory.CreateDirectory(text);
				WPExtractedPackage wpextractedPackage = WPCanonicalPackage.ExtractAndLoad(sourceCab, text);
				string text2 = Path.Combine(tempDirectory, "target");
				LongPathDirectory.CreateDirectory(text2);
				WPExtractedPackage wpextractedPackage2 = WPCanonicalPackage.ExtractAndLoad(targetCab, text2);
				bool flag = false;
				if (wpextractedPackage.Version == wpextractedPackage2.Version)
				{
					result = DiffError.SameVersion;
				}
				else
				{
					string text3 = Path.Combine(tempDirectory, "output");
					LongPathDirectory.CreateDirectory(text3);
					try
					{
						PackageTools.CheckCrossPartitionFiles(wpextractedPackage2.Name, wpextractedPackage2.Partition, from x in wpextractedPackage2.Files
						select x.DevicePath, false);
					}
					catch (PackageException)
					{
						flag = true;
					}
					if (flag)
					{
						LongPathFile.Copy(targetCab, outputCab);
					}
					else if (PackageStyle.CBS == wpextractedPackage.PackageStyle && PackageStyle.CBS == wpextractedPackage2.PackageStyle)
					{
						string text4 = Path.Combine(text3, "output", Path.GetFileName(outputCab));
						uint deltaThresholdMB = uint.MaxValue;
						if (options.HasFlag(DiffOptions.DeltaThresholdMB))
						{
							deltaThresholdMB = (uint)optionValues[DiffOptions.DeltaThresholdMB];
						}
						DiffPkgBuilder.CallCbsXpdGen(text, text2, deltaThresholdMB, text3, Path.GetFileName(outputCab));
						PackageTools.SignFile(text4);
						LongPathFile.Copy(text4, outputCab);
					}
					else
					{
						if (PackageStyle.SPKG != wpextractedPackage.PackageStyle || PackageStyle.SPKG != wpextractedPackage2.PackageStyle)
						{
							throw new PackageException("Trying to create diff between invalid source and target package styles.  Source: {0}, Target: {1}", new object[]
							{
								wpextractedPackage.PackageStyle,
								wpextractedPackage2.PackageStyle
							});
						}
						DiffPkgBuilder.SaveDiffPkg(DiffPkgBuilder.CreateDiff(wpextractedPackage, wpextractedPackage2, text3), outputCab);
					}
					result = DiffError.OK;
				}
			}
			finally
			{
				FileUtils.DeleteTree(tempDirectory);
			}
			return result;
		}

		// Token: 0x04000003 RID: 3
		private static readonly DELTA_INPUT dummyInput = new DELTA_INPUT(IntPtr.Zero, UIntPtr.Zero, false);

		// Token: 0x02000038 RID: 56
		private class Pair<T1, T2>
		{
			// Token: 0x170000A3 RID: 163
			// (get) Token: 0x06000251 RID: 593 RVA: 0x0000ABBF File Offset: 0x00008DBF
			// (set) Token: 0x06000252 RID: 594 RVA: 0x0000ABC7 File Offset: 0x00008DC7
			public T1 First { get; set; }

			// Token: 0x170000A4 RID: 164
			// (get) Token: 0x06000253 RID: 595 RVA: 0x0000ABD0 File Offset: 0x00008DD0
			// (set) Token: 0x06000254 RID: 596 RVA: 0x0000ABD8 File Offset: 0x00008DD8
			public T2 Second { get; set; }

			// Token: 0x06000255 RID: 597 RVA: 0x0000ABE1 File Offset: 0x00008DE1
			public Pair(T1 first, T2 second)
			{
				this.First = first;
				this.Second = second;
			}
		}
	}
}
