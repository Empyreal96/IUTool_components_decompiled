using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgCommon
{
	// Token: 0x02000033 RID: 51
	internal static class MergeWorker
	{
		// Token: 0x06000218 RID: 536 RVA: 0x0000903C File Offset: 0x0000723C
		private static bool IsTargetUpToDate(IEnumerable<string> inputFiles, string output)
		{
			if (!LongPathFile.Exists(output))
			{
				return false;
			}
			DateTime lastWriteTimeUtc = new FileInfo(output).LastWriteTimeUtc;
			using (IEnumerator<string> enumerator = inputFiles.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (new FileInfo(enumerator.Current).LastWriteTimeUtc > lastWriteTimeUtc)
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x06000219 RID: 537 RVA: 0x000090AC File Offset: 0x000072AC
		public static void Merge(IPkgBuilder pkgBuilder, List<string> pkgs, string outputPkg, bool compress, bool incremental)
		{
			if (incremental && MergeWorker.IsTargetUpToDate(pkgs, outputPkg))
			{
				LogUtil.Message("Skipping package '{0}' because all of its source packages are not changed", new object[]
				{
					outputPkg
				});
				return;
			}
			string tempDirectory = FileUtils.GetTempDirectory();
			try
			{
				StringBuilder stringBuilder = new StringBuilder();
				StringBuilder stringBuilder2 = new StringBuilder();
				int num = 0;
				foreach (string text in pkgs)
				{
					string outputDir = Path.Combine(tempDirectory, "PkgMerge" + num.ToString());
					WPExtractedPackage wpextractedPackage = WPCanonicalPackage.ExtractAndLoad(text, outputDir);
					stringBuilder.AppendFormat("{0},{1}", text, wpextractedPackage.Version);
					stringBuilder.AppendLine();
					foreach (IFileEntry fileEntry in wpextractedPackage.Files)
					{
						FileEntry fileEntry2 = (FileEntry)fileEntry;
						if (fileEntry2.FileType != FileType.Manifest && fileEntry2.FileType != FileType.Catalog)
						{
							IFileEntry fileEntry3 = pkgBuilder.FindFile(fileEntry2.DevicePath);
							if (fileEntry3 != null)
							{
								MergeErrors.Instance.Add("Package '{0}' and package with name '{1}' both contain file with same device path '{2}'", new object[]
								{
									text,
									fileEntry3.SourcePackage,
									fileEntry2.DevicePath
								});
							}
							else
							{
								FileType fileType = fileEntry2.FileType;
								if (fileType == FileType.Registry)
								{
									string fileName = Path.GetFileName(fileEntry2.DevicePath);
									if (MergeWorker._settingsFiles.Contains(fileName, StringComparer.InvariantCultureIgnoreCase))
									{
										string destination = fileEntry2.DevicePath.Replace(fileName, "MicrosoftSettings.reg");
										pkgBuilder.AddFile(fileEntry2.FileType, fileEntry2.SourcePath, destination, fileEntry2.Attributes, wpextractedPackage.Name, "None");
									}
									else
									{
										stringBuilder2.AppendLine("; RegistrySource=" + Path.GetFileName(fileEntry2.DevicePath));
										stringBuilder2.AppendLine(File.ReadAllText(fileEntry2.SourcePath));
									}
								}
								else
								{
									pkgBuilder.AddFile(fileEntry2.FileType, fileEntry2.SourcePath, fileEntry2.DevicePath, fileEntry2.Attributes, wpextractedPackage.Name, "None");
								}
							}
						}
					}
					num++;
				}
				string text2 = Path.Combine(tempDirectory, "combined.reg");
				if (stringBuilder2.Length != 0)
				{
					string text3 = pkgBuilder.Name.StartsWith("MSAsOEM", StringComparison.CurrentCultureIgnoreCase) ? "Microsoft." : string.Empty;
					string destination2 = string.Concat(new string[]
					{
						PkgConstants.c_strRguDeviceFolder,
						"\\",
						text3,
						pkgBuilder.Name,
						PkgConstants.c_strRguExtension
					});
					stringBuilder2.Replace("Windows Registry Editor Version 5.00", string.Empty);
					stringBuilder2.Insert(0, "Windows Registry Editor Version 5.00" + Environment.NewLine);
					File.WriteAllText(text2, stringBuilder2.ToString(), Encoding.Unicode);
					pkgBuilder.AddFile(FileType.Registry, text2, destination2, PkgConstants.c_defaultAttributes, null, "None");
				}
				File.WriteAllText(Path.ChangeExtension(outputPkg, ".merged.txt"), stringBuilder.ToString());
				if (!MergeErrors.Instance.HasError)
				{
					pkgBuilder.SaveCab(outputPkg, compress);
				}
			}
			finally
			{
				FileUtils.DeleteTree(tempDirectory);
			}
		}

		// Token: 0x040000DF RID: 223
		private static readonly string[] _settingsFiles = new string[]
		{
			"Microsoft.WifiOnlyFeaturePackOverrides.reg"
		};
	}
}
