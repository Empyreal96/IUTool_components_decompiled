using System;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.Interfaces;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.ImageUpdate.InboxAppLib
{
	// Token: 0x02000031 RID: 49
	public sealed class InboxAppUtils
	{
		// Token: 0x060000B0 RID: 176 RVA: 0x0000476B File Offset: 0x0000296B
		private InboxAppUtils()
		{
			throw new NotSupportedException("The 'InboxAppUtils' class should never be constructed on its own. Please use only the static methods.");
		}

		// Token: 0x060000B1 RID: 177 RVA: 0x0000477D File Offset: 0x0000297D
		public static bool ExtensionMatches(string filename, string extension)
		{
			return string.Compare(Path.GetExtension(filename), extension, StringComparison.OrdinalIgnoreCase) == 0;
		}

		// Token: 0x060000B2 RID: 178 RVA: 0x0000478F File Offset: 0x0000298F
		public static void Unzip(string zipFile, string destinationDirectory)
		{
			ZipFile.ExtractToDirectory(zipFile, destinationDirectory);
		}

		// Token: 0x060000B3 RID: 179 RVA: 0x00004798 File Offset: 0x00002998
		public static string ValidateFileOrDir(string filepathOrDirpath, bool isDir)
		{
			if (string.IsNullOrWhiteSpace(filepathOrDirpath))
			{
				throw new ArgumentNullException("filepathOrDirpath", "The specified path is null or empty.");
			}
			if (isDir)
			{
				if (Directory.Exists(filepathOrDirpath))
				{
					Directory.CreateDirectory(filepathOrDirpath);
					if (!Directory.Exists(filepathOrDirpath))
					{
						throw new DirectoryNotFoundException(string.Format(CultureInfo.InvariantCulture, "The directory \"{0}\" cannot be used as a working directory.", new object[]
						{
							filepathOrDirpath
						}));
					}
				}
			}
			else if (!File.Exists(filepathOrDirpath))
			{
				throw new FileNotFoundException(string.Format(CultureInfo.InvariantCulture, "The file \"{0}\" cannot be used a parameter for InboxApp.", new object[]
				{
					filepathOrDirpath
				}));
			}
			return filepathOrDirpath;
		}

		// Token: 0x060000B4 RID: 180 RVA: 0x00004820 File Offset: 0x00002A20
		public static string ConvertTemporaryAppXProductIDFormatToGuid(string appXAppID)
		{
			string empty = string.Empty;
			string text = appXAppID.Replace("y", "-");
			text = string.Format(CultureInfo.InvariantCulture, "{{{0}}}", new object[]
			{
				text.Trim(new char[]
				{
					'x'
				})
			});
			Guid guid = default(Guid);
			if (!Guid.TryParse(text, out guid))
			{
				return appXAppID;
			}
			return string.Format(CultureInfo.InvariantCulture, "{{{0}}}", new object[]
			{
				guid.ToString()
			});
		}

		// Token: 0x060000B5 RID: 181 RVA: 0x000048A8 File Offset: 0x00002AA8
		public static string MakePackageFullName(string title, string version, string processorArchitecture, string resourceId, string publisher)
		{
			uint capacity = 256U;
			StringBuilder stringBuilder = new StringBuilder((int)capacity);
			string text = string.Empty;
			title.Replace(" ", string.Empty);
			string text2 = resourceId.Replace(" ", string.Empty);
			if (Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version.Major >= 6 && Environment.OSVersion.Version.Minor >= 2)
			{
				LogUtil.Diagnostic("Calling kernel32!PackageFullNameFromId to get PackageFullName");
				NativeMethods.PACKAGE_ID package_ID = default(NativeMethods.PACKAGE_ID);
				package_ID.name = title;
				string a = processorArchitecture.ToLowerInvariant();
				if (!(a == "x86"))
				{
					if (!(a == "arm"))
					{
						if (!(a == "x64"))
						{
							if (!(a == "neutral"))
							{
								LogUtil.Warning("AppxManifest indicates an unknown processorArchitecture=\"{0}\". Defaulting to \"neutral\".", new object[]
								{
									processorArchitecture
								});
								package_ID.processorArchitecture = 11U;
							}
							else
							{
								package_ID.processorArchitecture = 11U;
							}
						}
						else
						{
							package_ID.processorArchitecture = 9U;
							LogUtil.Warning("AppxManifest indicates processorArchitecture=\"x64\", which is not supported on Windows Phone. This application may not function correctly when installed.");
						}
					}
					else
					{
						package_ID.processorArchitecture = 5U;
					}
				}
				else
				{
					package_ID.processorArchitecture = 0U;
				}
				package_ID.resourceId = resourceId;
				package_ID.publisher = publisher;
				short num = 0;
				short num2 = 0;
				short num3 = 0;
				short num4 = 0;
				string[] array = version.Split(new char[]
				{
					'.'
				});
				int num5 = array.Length;
				package_ID.Major = ((num5 > 0 && short.TryParse(array[0], out num)) ? num : 0);
				package_ID.Minor = ((num5 > 1 && short.TryParse(array[1], out num2)) ? num2 : 0);
				package_ID.Build = ((num5 > 2 && short.TryParse(array[2], out num3)) ? num3 : 0);
				package_ID.Revision = ((num5 >= 3 && short.TryParse(array[3], out num4)) ? num4 : 0);
				int num6 = NativeMethods.PackageFullNameFromId(ref package_ID, ref capacity, stringBuilder);
				if (num6 != 0)
				{
					throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "PackageFullNameFromId returned error {5}. One of the following fields may be empty or have an invalid value:\n(title)=\"{0}\" (version)=\"{1}\" (processorArchitecture)=\"{2}\" (resourceId)=\"{3}\" (publisher)=\"{4}\"", new object[]
					{
						title,
						version,
						processorArchitecture,
						resourceId,
						publisher,
						num6
					}), "PackageFullNameFromId(packageID)");
				}
				if (string.IsNullOrWhiteSpace(stringBuilder.ToString()))
				{
					throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "INTERNAL ERROR: PackageFullNameFromId returned a blank PackageFullName!\n(title)=\"{0}\" (version)=\"{1}\" (processorArchitecture)=\"{2}\" (resourceId)=\"{3}\" (publisher)=\"{4}\"", new object[]
					{
						title,
						version,
						processorArchitecture,
						resourceId,
						publisher
					}));
				}
				text = stringBuilder.ToString();
				LogUtil.Diagnostic(string.Format(CultureInfo.InvariantCulture, "Got PackageFullName: \"{0}\"", new object[]
				{
					text
				}));
			}
			else
			{
				string text3 = publisher.GetHashCode().ToString("x");
				text = string.Format(CultureInfo.InvariantCulture, "{0}_{1}_{2}_{3}_{4}", new object[]
				{
					title,
					version,
					processorArchitecture,
					text2,
					text3
				});
				LogUtil.Warning(string.Format(CultureInfo.InvariantCulture, "Since pkggen is not running on Windows 8 or greater, the package full name \"{0}\" is a placeholder and may not be correct. Please run pkggen on Windows 8 or greater.", new object[]
				{
					text
				}));
			}
			return text;
		}

		// Token: 0x060000B6 RID: 182 RVA: 0x00004B94 File Offset: 0x00002D94
		public static string ResolveDestinationPath(string someDestinationPath, bool isOnDataPartition, IPkgProject packageGenerator)
		{
			string text = someDestinationPath;
			if (isOnDataPartition && "$(runtime.data)" != someDestinationPath.Substring(0, "$(runtime.data)".Length))
			{
				text = "$(runtime.data)" + someDestinationPath;
			}
			LogUtil.Diagnostic(string.Format(CultureInfo.InvariantCulture, "DestinationPathToResolve = \"{0}\"", new object[]
			{
				text
			}));
			string text2 = packageGenerator.MacroResolver.Resolve(text, MacroResolveOptions.ErrorOnUnknownMacro);
			LogUtil.Diagnostic(string.Format(CultureInfo.InvariantCulture, "resolvedDestinationPath = \"{0}\"", new object[]
			{
				text2
			}));
			return text2;
		}

		// Token: 0x060000B7 RID: 183 RVA: 0x00004C1C File Offset: 0x00002E1C
		public static string MapNeutralToSpecificCulture(string neutralCulture)
		{
			string empty = string.Empty;
			int num = NativeMethods.Bcp47GetNlsForm(neutralCulture, ref empty);
			if (num != 0)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Bcp47GetNlsForm returned error {2}. One of the following fields may be empty or have an invalid value:\n(languageTag)=\"{0}\" (nlsForm)=\"{1}\"", new object[]
				{
					neutralCulture,
					empty,
					num
				}));
			}
			if (!string.IsNullOrEmpty(empty))
			{
				return empty.ToLowerInvariant();
			}
			return string.Empty;
		}

		// Token: 0x060000B8 RID: 184 RVA: 0x00004C7C File Offset: 0x00002E7C
		public static string CalcHash(string filePath)
		{
			string result = string.Empty;
			if (File.Exists(filePath))
			{
				HashAlgorithm hashAlgorithm = SHA256.Create();
				FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
				byte[] value = hashAlgorithm.ComputeHash(fileStream);
				fileStream.Close();
				result = BitConverter.ToString(value);
			}
			return result;
		}
	}
}
