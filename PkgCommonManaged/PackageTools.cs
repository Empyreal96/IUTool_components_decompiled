using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Win32.SafeHandles;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgCommon
{
	// Token: 0x0200002E RID: 46
	public static class PackageTools
	{
		// Token: 0x060001F0 RID: 496 RVA: 0x0000855B File Offset: 0x0000675B
		public static string GetCatalogPackageName(PkgManifest pkgManifest)
		{
			return PackageTools.GetCatalogPackageName(pkgManifest.ReleaseType, pkgManifest.Name);
		}

		// Token: 0x060001F1 RID: 497 RVA: 0x0000856E File Offset: 0x0000676E
		public static string GetCatalogPackageName(IPkgInfo pkgInfo)
		{
			return PackageTools.GetCatalogPackageName(pkgInfo.ReleaseType, pkgInfo.Name);
		}

		// Token: 0x060001F2 RID: 498 RVA: 0x00008581 File Offset: 0x00006781
		public static string GetCatalogPackageName(ReleaseType releaseType, string prodName)
		{
			if (releaseType == ReleaseType.Test)
			{
				return PackageTools.c_strDebugPackageName;
			}
			return prodName;
		}

		// Token: 0x060001F3 RID: 499 RVA: 0x00008590 File Offset: 0x00006790
		public static byte[] CalculateFileHash(string file)
		{
			byte[] result;
			using (FileStream fileStream = LongPathFile.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				using (HashAlgorithm hashAlgorithm = HashAlgorithm.Create(PkgConstants.c_strHashAlgorithm))
				{
					result = hashAlgorithm.ComputeHash(fileStream);
				}
			}
			return result;
		}

		// Token: 0x060001F4 RID: 500 RVA: 0x000085F0 File Offset: 0x000067F0
		public static string MakeShortPath(string path, string prefix)
		{
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);
			string extension = Path.GetExtension(path);
			string text = string.Format("{0}_{1}", prefix, (fileNameWithoutExtension.Length >= 8) ? fileNameWithoutExtension.Substring(0, 7) : fileNameWithoutExtension);
			if (!string.IsNullOrEmpty(extension) && extension.Length > 1)
			{
				text += extension;
			}
			return text.Replace(" ", "_");
		}

		// Token: 0x060001F5 RID: 501
		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool GetFileSizeEx(SafeFileHandle hFile, out long lpFileSize);

		// Token: 0x060001F6 RID: 502
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern SafeFileHandle CreateFile(string fileName, uint desiredAccess, uint shareMode, IntPtr securityAttributes, uint creationDisposition, uint flagsAndAttributes, IntPtr templateFile);

		// Token: 0x060001F7 RID: 503 RVA: 0x00008658 File Offset: 0x00006858
		private static long GetFileSize(string fileName)
		{
			long result;
			using (SafeFileHandle safeFileHandle = PackageTools.CreateFile(fileName, 2147483648U, 1U, IntPtr.Zero, 3U, 128U, IntPtr.Zero))
			{
				if (safeFileHandle.IsInvalid)
				{
					throw new Exception(string.Concat(new string[]
					{
						"CreateFile() failed while calling GetFileSize().  GetLastError() = ",
						Marshal.GetLastWin32Error().ToString(CultureInfo.InvariantCulture),
						"\nMake sure the file '",
						fileName,
						"' exist and has sharing access."
					}));
				}
				if (!PackageTools.GetFileSizeEx(safeFileHandle, out result))
				{
					throw new Exception(string.Concat(new string[]
					{
						"GetFileSizeEx() failed while calling GetFileSize().  GetLastError() = ",
						Marshal.GetLastWin32Error().ToString(CultureInfo.InvariantCulture),
						"\nMake sure the file '",
						fileName,
						"' exist and has sharing access."
					}));
				}
			}
			return result;
		}

		// Token: 0x060001F8 RID: 504 RVA: 0x00008738 File Offset: 0x00006938
		private static bool IsFileEmpty(string fileName)
		{
			long fileSize = PackageTools.GetFileSize(fileName);
			return fileSize == 0L;
		}

		// Token: 0x060001F9 RID: 505 RVA: 0x00008754 File Offset: 0x00006954
		public static void CreateCDF(string[] filePaths, string[] fileTags, string catalogPath, string packageName, string cdfFile)
		{
			using (TextWriter textWriter = new StreamWriter(LongPathFile.OpenWrite(cdfFile)))
			{
				textWriter.WriteLine("[CatalogHeader]");
				textWriter.WriteLine("Name={0}", Path.GetFileName(catalogPath));
				textWriter.WriteLine("ResultDir={0}", Path.GetDirectoryName(catalogPath));
				textWriter.WriteLine("PublicVersion=0x00000001");
				textWriter.WriteLine("EncodingType=0x00010001");
				textWriter.WriteLine("PageHashes=false");
				textWriter.WriteLine("CatalogVersion=2");
				textWriter.WriteLine("HashAlgorithms=SHA256");
				textWriter.WriteLine("CATATTR1=0x00010001:OSAttr:2:6.1,2:6.0,2:5.2,2:5.1");
				textWriter.WriteLine("CATATTR2=0x00010001:PackageName:{0}\r\n", packageName);
				textWriter.WriteLine();
				textWriter.WriteLine("[CatalogFiles]");
				for (int i = 0; i < filePaths.Length; i++)
				{
					string fullPathUNC = LongPath.GetFullPathUNC(filePaths[i]);
					if (!PackageTools.IsFileEmpty(fullPathUNC))
					{
						string arg = string.Format("<HASH>{0}", fileTags[i].Replace('=', '~'));
						textWriter.WriteLine("{0}={1}", arg, fullPathUNC);
					}
				}
			}
		}

		// Token: 0x060001FA RID: 506 RVA: 0x0000885C File Offset: 0x00006A5C
		public static void CreateCatalog(string[] sourcePaths, string[] devicePaths, string packageName, IPkgFileSigner signer, string catalogPath, PackageTools.SIGNING_HINT signingHint)
		{
			string tempFile = FileUtils.GetTempFile();
			try
			{
				if (sourcePaths.Length != devicePaths.Length)
				{
					throw new PackageException("sourcePaths and devicePaths must be of the same length");
				}
				PackageTools.CreateCDF(sourcePaths, devicePaths, catalogPath, packageName, tempFile);
				int num = 0;
				try
				{
					num = CommonUtils.RunProcess("makecat.exe", string.Format("/v \"{0}\"", tempFile));
				}
				catch (Exception inner)
				{
					throw new PackageException(inner, "makecat.exe failed unexpectedly");
				}
				if (num != 0)
				{
					throw new PackageException("makecat.exe finished with non-zero exit code {0}", new object[]
					{
						num
					});
				}
				if (!File.Exists(catalogPath))
				{
					throw new PackageException("Failed to create package integrity catalog file");
				}
				try
				{
					if (signer != null)
					{
						if (signingHint == PackageTools.SIGNING_HINT.SIGNING_HINT_TEST)
						{
							LogUtil.Message("PkgCommonManaged: CreateCatalog: Signing catalog path {0} with -testonly for package '{1}'.", new object[]
							{
								catalogPath,
								packageName
							});
							signer.SignFileWithOptions(catalogPath, "-testonly");
						}
						else if (signingHint == PackageTools.SIGNING_HINT.SIGNING_HINT_BRANCH_POLICY)
						{
							LogUtil.Message("PkgCommonManaged: CreateCatalog: Signing catalog path {0} according to branch policy for package '{1}'.", new object[]
							{
								catalogPath,
								packageName
							});
							signer.SignFile(catalogPath);
						}
					}
				}
				catch (Exception inner2)
				{
					throw new PackageException(inner2, "Failed to sign package integrity catalog file");
				}
			}
			finally
			{
				FileUtils.DeleteFile(tempFile);
			}
		}

		// Token: 0x060001FB RID: 507 RVA: 0x00008978 File Offset: 0x00006B78
		public static void CreateCatalog(string[] sourcePaths, string[] devicePaths, string packageName, IPkgFileSigner signer, string catalogPath)
		{
			PackageTools.CreateCatalog(sourcePaths, devicePaths, packageName, signer, catalogPath, PackageTools.SIGNING_HINT.SIGNING_HINT_BRANCH_POLICY);
		}

		// Token: 0x060001FC RID: 508 RVA: 0x00008988 File Offset: 0x00006B88
		public static void CreateTestSignedCatalog(string[] sourcePaths, string[] devicePaths, string packageName, string catalogPath)
		{
			IPkgFileSigner signer = new PkgFileSignerDefault();
			PackageTools.CreateCatalog(sourcePaths, devicePaths, packageName, signer, catalogPath, PackageTools.SIGNING_HINT.SIGNING_HINT_TEST);
		}

		// Token: 0x060001FD RID: 509 RVA: 0x000089A8 File Offset: 0x00006BA8
		public static void CreateCatalog(string[] sourcePaths, string[] devicePaths, string packageName, string catalogPath)
		{
			IPkgFileSigner signer = new PkgFileSignerDefault();
			PackageTools.CreateCatalog(sourcePaths, devicePaths, packageName, signer, catalogPath);
		}

		// Token: 0x060001FE RID: 510 RVA: 0x000089C5 File Offset: 0x00006BC5
		public static void CheckCrossPartitionFiles(string pkgName, string partition, IEnumerable<string> devicePaths)
		{
			PackageTools.CheckCrossPartitionFiles(pkgName, partition, devicePaths, true);
		}

		// Token: 0x060001FF RID: 511 RVA: 0x000089D0 File Offset: 0x00006BD0
		public static void CheckCrossPartitionFiles(string pkgName, string partition, IEnumerable<string> devicePaths, bool logCrossers)
		{
			if (!partition.Equals(PkgConstants.c_strMainOsPartition, StringComparison.OrdinalIgnoreCase))
			{
				return;
			}
			bool flag = false;
			foreach (string text in devicePaths)
			{
				foreach (string value in PkgConstants.c_strJunctionPaths)
				{
					if (text.StartsWith(value, StringComparison.OrdinalIgnoreCase))
					{
						if (logCrossers)
						{
							LogUtil.Error("Package '{0}', File '{1}': Referencing files from '{2}' partition using junctions ({2}) is not allowed in production package. Changing ReleaseType to Test or set Partition attribute with the right partition, such as, Partition=\"Data\", and using the right path in DestinationDir", new object[]
							{
								pkgName,
								text,
								partition,
								string.Join(",", PkgConstants.c_strJunctionPaths)
							});
						}
						flag = true;
					}
				}
			}
			if (flag)
			{
				throw new PackageException("Cross partition files found in production package '{0}'", new object[]
				{
					pkgName
				});
			}
		}

		// Token: 0x06000200 RID: 512 RVA: 0x00008A98 File Offset: 0x00006C98
		public static void SignFileWithOptions(string file, string options)
		{
			int num = 0;
			if (options == null)
			{
				options = string.Empty;
			}
			try
			{
				CommonUtils.FindInPath("sign.cmd");
				LogUtil.Message("Calling sign.cmd with options '{0}' and file '{1}'.", new object[]
				{
					options,
					file
				});
				num = CommonUtils.RunProcessVerbose("%COMSPEC%", string.Format("/C sign.cmd {0} \"{1}\"", options, file));
			}
			catch (Exception innerException)
			{
				throw new PackageException(innerException, "Failed to sign the file {0} with options {1}", new object[]
				{
					file,
					options
				});
			}
			if (num != 0)
			{
				throw new PackageException("Failed to sign file {0} with options {1}, exit code {2}", new object[]
				{
					file,
					options,
					num
				});
			}
		}

		// Token: 0x06000201 RID: 513 RVA: 0x00008B3C File Offset: 0x00006D3C
		public static bool FileHasPageHashes(string fileName)
		{
			string fileName2 = CommonUtils.FindInPath("signtool.exe");
			Process process = new Process();
			process.StartInfo.FileName = fileName2;
			process.StartInfo.Arguments = string.Format("verify /v /pa \"{0}\"", fileName);
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardError = true;
			process.Start();
			string text = process.StandardOutput.ReadToEnd();
			string text2 = process.StandardError.ReadToEnd();
			process.WaitForExit();
			if (text2.Contains("SignTool Error"))
			{
				throw new PackageException(text2);
			}
			return text.Contains("File has page hashes");
		}

		// Token: 0x06000202 RID: 514 RVA: 0x00008BE2 File Offset: 0x00006DE2
		public static void SignFile(string file)
		{
			PackageTools.SignFileWithOptions(file, null);
		}

		// Token: 0x06000203 RID: 515 RVA: 0x00008BEB File Offset: 0x00006DEB
		public static string BuildPackageName(string owner, string component)
		{
			return PackageTools.BuildPackageName(owner, component, null, null, null);
		}

		// Token: 0x06000204 RID: 516 RVA: 0x00008BF7 File Offset: 0x00006DF7
		public static string BuildPackageName(string owner, string component, string subComponent)
		{
			return PackageTools.BuildPackageName(owner, component, subComponent, null, null);
		}

		// Token: 0x06000205 RID: 517 RVA: 0x00008C03 File Offset: 0x00006E03
		public static string BuildPackageName(string owner, string component, string subComponent, string culture)
		{
			return PackageTools.BuildPackageName(owner, component, subComponent, culture, null);
		}

		// Token: 0x06000206 RID: 518 RVA: 0x00008C10 File Offset: 0x00006E10
		public static string BuildPackageName(string owner, string component, string subComponent, string culture, string resolution)
		{
			if (string.IsNullOrEmpty(owner) || string.IsNullOrEmpty(component))
			{
				throw new PackageException("Owner and Component can't be null");
			}
			if (!string.IsNullOrEmpty(culture) && !string.IsNullOrEmpty(resolution))
			{
				throw new PackageException("Culture and Resolution can not be set in the same time");
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("{0}.{1}", owner, component);
			if (!string.IsNullOrEmpty(subComponent))
			{
				stringBuilder.AppendFormat(".{0}", subComponent);
			}
			if (!string.IsNullOrEmpty(culture))
			{
				stringBuilder.AppendFormat("_Lang_{0}", culture);
			}
			if (!string.IsNullOrEmpty(resolution))
			{
				stringBuilder.AppendFormat("_RES_{0}", resolution);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06000207 RID: 519 RVA: 0x00008CAF File Offset: 0x00006EAF
		public static string GetDefaultDriveLetter(string partitionName)
		{
			if (PkgConstants.c_strUpdateOsPartition.Equals(partitionName, StringComparison.OrdinalIgnoreCase))
			{
				return PkgConstants.c_strUpdateOSDrive;
			}
			return PkgConstants.c_strDefaultDrive;
		}

		// Token: 0x040000D1 RID: 209
		public static readonly string c_strDebugPackageName = "DebugPackage";

		// Token: 0x040000D2 RID: 210
		private const uint FILE_ATTRIBUTE_NORMAL = 128U;

		// Token: 0x040000D3 RID: 211
		private const short INVALID_HANDLE_VALUE = -1;

		// Token: 0x040000D4 RID: 212
		private const uint GENERIC_READ = 2147483648U;

		// Token: 0x040000D5 RID: 213
		private const uint GENERIC_WRITE = 1073741824U;

		// Token: 0x040000D6 RID: 214
		private const uint CREATE_NEW = 1U;

		// Token: 0x040000D7 RID: 215
		private const uint CREATE_ALWAYS = 2U;

		// Token: 0x040000D8 RID: 216
		private const uint OPEN_EXISTING = 3U;

		// Token: 0x040000D9 RID: 217
		private const uint FILE_SHARE_READ = 1U;

		// Token: 0x0200004C RID: 76
		public enum SIGNING_HINT
		{
			// Token: 0x04000142 RID: 322
			SIGNING_HINT_TEST,
			// Token: 0x04000143 RID: 323
			SIGNING_HINT_BRANCH_POLICY
		}
	}
}
