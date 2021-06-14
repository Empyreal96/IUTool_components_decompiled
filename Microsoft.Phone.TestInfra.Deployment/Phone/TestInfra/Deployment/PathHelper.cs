using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Tools.IO;

namespace Microsoft.Phone.TestInfra.Deployment
{
	// Token: 0x02000011 RID: 17
	public static class PathHelper
	{
		// Token: 0x17000056 RID: 86
		// (get) Token: 0x060000D6 RID: 214 RVA: 0x00005228 File Offset: 0x00003428
		public static string PrebuiltFolderName
		{
			get
			{
				return "Prebuilt";
			}
		}

		// Token: 0x17000057 RID: 87
		// (get) Token: 0x060000D7 RID: 215 RVA: 0x00005240 File Offset: 0x00003440
		public static string NonCritPrebuiltFolderName
		{
			get
			{
				return "Prebuilt_noncrit";
			}
		}

		// Token: 0x060000D8 RID: 216 RVA: 0x00005258 File Offset: 0x00003458
		public static string GetFileNameWithoutExtension(string path, string extension)
		{
			bool flag = string.IsNullOrEmpty(path);
			if (flag)
			{
				throw new ArgumentNullException("path");
			}
			bool flag2 = string.IsNullOrEmpty(extension);
			if (flag2)
			{
				throw new ArgumentNullException("extension");
			}
			bool flag3 = !extension.StartsWith(".", StringComparison.OrdinalIgnoreCase);
			if (flag3)
			{
				extension = "." + extension;
			}
			string fileName = Path.GetFileName(path);
			bool flag4 = fileName.EndsWith(extension, StringComparison.OrdinalIgnoreCase);
			string result;
			if (flag4)
			{
				result = fileName.Substring(0, fileName.Length - extension.Length);
			}
			else
			{
				result = fileName;
			}
			return result;
		}

		// Token: 0x060000D9 RID: 217 RVA: 0x000052EC File Offset: 0x000034EC
		public static string GetPackageNameWithoutExtension(string package)
		{
			string result = LongPathPath.GetFileName(package);
			string[] array = new string[]
			{
				Constants.CabFileExtension,
				Constants.SpkgFileExtension
			};
			foreach (string text in array)
			{
				bool flag = package.EndsWith(text, StringComparison.OrdinalIgnoreCase);
				if (flag)
				{
					result = PathHelper.GetFileNameWithoutExtension(package.Trim().ToLowerInvariant(), text);
					break;
				}
			}
			return result;
		}

		// Token: 0x060000DA RID: 218 RVA: 0x0000535C File Offset: 0x0000355C
		public static string EndWithDirectorySeparator(string path)
		{
			bool flag = string.IsNullOrEmpty(path);
			if (flag)
			{
				throw new ArgumentNullException("path");
			}
			return path.EndsWith(PathHelper.DirectorySeparator, StringComparison.OrdinalIgnoreCase) ? path : (path + PathHelper.DirectorySeparator);
		}

		// Token: 0x060000DB RID: 219 RVA: 0x000053A0 File Offset: 0x000035A0
		public static string ChangeParent(string path, string oldParent, string newParent)
		{
			bool flag = string.IsNullOrEmpty(path);
			if (flag)
			{
				throw new ArgumentNullException("path");
			}
			bool flag2 = string.IsNullOrEmpty(oldParent);
			if (flag2)
			{
				throw new ArgumentNullException("oldParent");
			}
			bool flag3 = newParent == null;
			if (flag3)
			{
				throw new ArgumentNullException("newParent");
			}
			bool flag4 = string.Compare(path, oldParent, StringComparison.OrdinalIgnoreCase) == 0;
			string result;
			if (flag4)
			{
				result = newParent;
			}
			else
			{
				oldParent = PathHelper.EndWithDirectorySeparator(oldParent);
				newParent = (string.IsNullOrEmpty(newParent) ? string.Empty : PathHelper.EndWithDirectorySeparator(newParent));
				bool flag5 = !path.StartsWith(oldParent, StringComparison.OrdinalIgnoreCase);
				if (flag5)
				{
					throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Path '{0}' does not start with '{1}'", new object[]
					{
						path,
						oldParent
					}));
				}
				result = newParent + path.Substring(oldParent.Length);
			}
			return result;
		}

		// Token: 0x060000DC RID: 220 RVA: 0x00005470 File Offset: 0x00003670
		public static string Combine(string path1, string path2)
		{
			bool flag = path1 == null;
			if (flag)
			{
				throw new ArgumentNullException("path1");
			}
			bool flag2 = path2 == null;
			if (flag2)
			{
				throw new ArgumentNullException("path2");
			}
			return Path.Combine(path1, path2.TrimStart(new char[]
			{
				Path.DirectorySeparatorChar
			}));
		}

		// Token: 0x060000DD RID: 221 RVA: 0x000054C4 File Offset: 0x000036C4
		public static string GetWinBuildPath(string path)
		{
			bool flag = path.StartsWith("\\\\");
			string result;
			if (flag)
			{
				result = PathHelper.GetWinBuildPathForPublicBuild(path);
			}
			else
			{
				result = PathHelper.GetWinBuildPathForLocalPath(path);
			}
			return result;
		}

		// Token: 0x060000DE RID: 222 RVA: 0x000054F8 File Offset: 0x000036F8
		public static string GetContainingFolderPath(string path, string containingFolder)
		{
			bool flag = string.IsNullOrEmpty(path);
			if (flag)
			{
				throw new ArgumentNullException("path");
			}
			DirectoryInfo directoryInfo = new DirectoryInfo(path);
			while (directoryInfo != null && string.Compare(directoryInfo.Name, containingFolder, true) != 0)
			{
				directoryInfo = directoryInfo.Parent;
			}
			return (directoryInfo == null) ? string.Empty : directoryInfo.FullName;
		}

		// Token: 0x060000DF RID: 223 RVA: 0x00005560 File Offset: 0x00003760
		public static PathType GetPathType(string path)
		{
			bool flag = string.IsNullOrEmpty(path);
			if (flag)
			{
				throw new ArgumentNullException("path");
			}
			path = path.ToLowerInvariant();
			bool flag2 = path.StartsWith(Settings.Default.WinBuildSharePrefix, StringComparison.OrdinalIgnoreCase);
			PathType result;
			if (flag2)
			{
				result = PathType.WinbBuildPath;
			}
			else
			{
				bool flag3 = path.StartsWith(Settings.Default.PhoneBuildSharePrefix, StringComparison.OrdinalIgnoreCase);
				if (flag3)
				{
					result = PathType.PhoneBuildPath;
				}
				else
				{
					bool flag4 = path.StartsWith("\\\\");
					if (flag4)
					{
						result = PathType.NetworkPath;
					}
					else
					{
						result = PathType.LocalPath;
					}
				}
			}
			return result;
		}

		// Token: 0x060000E0 RID: 224 RVA: 0x000055DC File Offset: 0x000037DC
		public static IOrderedEnumerable<string> SortPathSetOnPathType(HashSet<string> paths)
		{
			return from x in paths
			orderby PathHelper.GetPathType(x)
			select x;
		}

		// Token: 0x060000E1 RID: 225 RVA: 0x00005614 File Offset: 0x00003814
		public static HashSet<string> AddRelatedPathsToSet(HashSet<string> paths)
		{
			HashSet<string> hashSet = new HashSet<string>();
			HashSet<string> hashSet2 = new HashSet<string>();
			string text = string.Empty;
			foreach (string text2 in paths)
			{
				hashSet2 = PathHelper.GetPrebuiltPaths(text2, false);
				bool flag = hashSet2.Count<string>() == 0;
				if (flag)
				{
					hashSet.Add(text2);
				}
				else
				{
					hashSet.UnionWith(hashSet2);
				}
				text = PathHelper.GetWinBuildPath(text2);
				bool flag2 = !string.IsNullOrEmpty(text);
				if (flag2)
				{
					hashSet.UnionWith(PathHelper.GetPrebuiltPaths(text, false));
				}
			}
			return hashSet;
		}

		// Token: 0x060000E2 RID: 226 RVA: 0x000056D0 File Offset: 0x000038D0
		public static HashSet<string> GetPrebuiltPaths(string path, bool throwException = false)
		{
			bool flag = string.IsNullOrEmpty(path);
			if (flag)
			{
				throw new ArgumentNullException("path");
			}
			HashSet<string> hashSet = new HashSet<string>();
			bool flag2 = !Directory.Exists(path);
			HashSet<string> result;
			if (flag2)
			{
				if (throwException)
				{
					throw new InvalidDataException(string.Format("Path {0} does not exist or is not accessible.", path));
				}
				Logger.Info(string.Format("Path {0} does not exist or is not accessible.", path), new object[0]);
				result = hashSet;
			}
			else
			{
				string containingFolderPath = PathHelper.GetContainingFolderPath(path, PathHelper.PrebuiltFolderName);
				string containingFolderPath2 = PathHelper.GetContainingFolderPath(path, PathHelper.NonCritPrebuiltFolderName);
				string text = string.Empty;
				bool flag3 = !string.IsNullOrEmpty(containingFolderPath);
				if (flag3)
				{
					hashSet.Add(containingFolderPath.ToLowerInvariant());
					DirectoryInfo parent = new DirectoryInfo(containingFolderPath).Parent;
					bool flag4 = parent != null;
					if (flag4)
					{
						bool flag5 = string.Compare(parent.Name, "bin", true) == 0;
						if (flag5)
						{
							bool flag6 = parent.Parent != null;
							if (flag6)
							{
								text = parent.Parent.FullName;
							}
						}
						else
						{
							text = parent.FullName;
						}
					}
				}
				else
				{
					bool flag7 = !string.IsNullOrEmpty(containingFolderPath2);
					if (!flag7)
					{
						return hashSet;
					}
					hashSet.Add(containingFolderPath2.ToLowerInvariant());
					DirectoryInfo parent = new DirectoryInfo(containingFolderPath2).Parent;
					bool flag8 = parent != null;
					if (flag8)
					{
						text = parent.FullName;
					}
				}
				bool flag9 = string.IsNullOrEmpty(text);
				if (flag9)
				{
					result = hashSet;
				}
				else
				{
					string[] array = new string[]
					{
						PathHelper.PrebuiltFolderName,
						"bin\\" + PathHelper.PrebuiltFolderName,
						PathHelper.NonCritPrebuiltFolderName,
						"..\\" + PathHelper.NonCritPrebuiltFolderName
					};
					foreach (string path2 in array)
					{
						string fullPath = Path.GetFullPath(Path.Combine(text, path2));
						bool flag10 = ReliableDirectory.Exists(fullPath, Settings.Default.ShareAccessRetryCount, TimeSpan.FromMilliseconds((double)Settings.Default.ShareAccessRetryDelayInMs));
						if (flag10)
						{
							hashSet.Add(fullPath.ToLowerInvariant());
						}
					}
					result = hashSet;
				}
			}
			return result;
		}

		// Token: 0x060000E3 RID: 227 RVA: 0x000058F0 File Offset: 0x00003AF0
		public static string GetPrebuiltPath(string path)
		{
			bool flag = string.IsNullOrWhiteSpace(path);
			if (flag)
			{
				throw new ArgumentException("cannot be null or empty.", "path");
			}
			bool flag2 = !ReliableDirectory.Exists(path, Settings.Default.ShareAccessRetryCount, TimeSpan.FromMilliseconds((double)Settings.Default.ShareAccessRetryDelayInMs));
			if (flag2)
			{
				throw new DirectoryNotFoundException(path);
			}
			string result;
			try
			{
				string text = PathHelper.GetContainingFolderPath(path, Constants.PrebuiltFolderName);
				bool flag3 = PathHelper.IsPrebuiltPath(text);
				if (flag3)
				{
					result = text;
				}
				else
				{
					text = Path.Combine(path, Constants.PrebuiltFolderName);
					bool flag4 = ReliableDirectory.Exists(text, Settings.Default.ShareAccessRetryCount, TimeSpan.FromMilliseconds((double)Settings.Default.ShareAccessRetryDelayInMs)) && PathHelper.IsPrebuiltPath(text);
					if (flag4)
					{
						result = text;
					}
					else
					{
						bool flag5 = PathHelper.IsPrebuiltPath(path);
						if (flag5)
						{
							result = path;
						}
						else
						{
							result = string.Empty;
						}
					}
				}
			}
			catch
			{
				result = string.Empty;
			}
			return result;
		}

		// Token: 0x060000E4 RID: 228 RVA: 0x000059E4 File Offset: 0x00003BE4
		public static IEnumerable<string> GetPackageFilesUnderPath(string path)
		{
			bool flag = PathHelper.IsPrebuiltPath(path);
			IEnumerable<string> enumerable;
			List<string> list;
			if (flag)
			{
				enumerable = from x in PathHelper.GetFilesUnderPrebuiltPath(path, "*" + Constants.SpkgFileExtension)
				select x.ToLowerInvariant();
				list = (from x in PathHelper.GetFilesUnderPrebuiltPath(path, "*" + Constants.CabFileExtension)
				select x.ToLowerInvariant()).ToList<string>();
			}
			else
			{
				enumerable = from x in ReliableDirectory.GetFiles(path, "*" + Constants.SpkgFileExtension, SearchOption.AllDirectories, Settings.Default.ShareAccessRetryCount, TimeSpan.FromMilliseconds((double)Settings.Default.ShareAccessRetryDelayInMs))
				select x.ToLowerInvariant();
				list = (from x in ReliableDirectory.GetFiles(path, "*" + Constants.CabFileExtension, SearchOption.AllDirectories, Settings.Default.ShareAccessRetryCount, TimeSpan.FromMilliseconds((double)Settings.Default.ShareAccessRetryDelayInMs))
				select x.ToLowerInvariant()).ToList<string>();
			}
			foreach (string text in enumerable)
			{
				string item = Path.ChangeExtension(text, Constants.CabFileExtension);
				bool flag2 = list.Contains(item);
				if (flag2)
				{
					list.Remove(item);
				}
				list.Add(text);
			}
			return list;
		}

		// Token: 0x060000E5 RID: 229 RVA: 0x00005BA0 File Offset: 0x00003DA0
		public static bool IsPrebuiltPath(string path)
		{
			bool flag = string.IsNullOrWhiteSpace(path);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = !ReliableDirectory.Exists(path, Settings.Default.ShareAccessRetryCount, TimeSpan.FromMilliseconds((double)Settings.Default.ShareAccessRetryDelayInMs));
				if (flag2)
				{
					throw new DirectoryNotFoundException(path);
				}
				string[] source = new string[]
				{
					"test"
				};
				string[] source2 = new string[]
				{
					"windows"
				};
				bool flag3 = !source.Any((string x) => !ReliableDirectory.Exists(Path.Combine(path, x), Settings.Default.ShareAccessRetryCount, TimeSpan.FromMilliseconds((double)Settings.Default.ShareAccessRetryDelayInMs)));
				bool flag4 = !source2.Any((string x) => !ReliableDirectory.Exists(Path.Combine(path, x), Settings.Default.ShareAccessRetryCount, TimeSpan.FromMilliseconds((double)Settings.Default.ShareAccessRetryDelayInMs)));
				result = (flag3 || flag4);
			}
			return result;
		}

		// Token: 0x060000E6 RID: 230 RVA: 0x00005C68 File Offset: 0x00003E68
		public static IEnumerable<string> GetPrebuiltPathForAllArchitectures(string prebuiltPath)
		{
			bool flag = string.IsNullOrWhiteSpace(prebuiltPath);
			if (flag)
			{
				throw new ArgumentException("cannot be null or empty", "prebuiltPath");
			}
			prebuiltPath = prebuiltPath.ToLowerInvariant();
			string phoneBuildArchitecture = PathHelper.phoneBuildArchitectures.FirstOrDefault((string x) => prebuiltPath.IndexOf(x.ToLowerInvariant()) >= 0);
			bool flag2 = !string.IsNullOrEmpty(phoneBuildArchitecture);
			IEnumerable<string> result;
			if (flag2)
			{
				result = from x in PathHelper.phoneBuildArchitectures
				select prebuiltPath.Replace(phoneBuildArchitecture.ToLowerInvariant(), x.ToLowerInvariant());
			}
			else
			{
				string winBuildArchitecture = PathHelper.winBuildArchitectures.FirstOrDefault((string x) => prebuiltPath.IndexOf(x.ToLowerInvariant()) >= 0);
				bool flag3 = string.IsNullOrEmpty(winBuildArchitecture);
				if (flag3)
				{
					throw new InvalidDataException(string.Format("Cannot decide the build architecture of path {0}", prebuiltPath));
				}
				result = from x in PathHelper.winBuildArchitectures
				select prebuiltPath.Replace(winBuildArchitecture.ToLowerInvariant(), x.ToLowerInvariant().Replace("arm", "woa"));
			}
			return result;
		}

		// Token: 0x060000E7 RID: 231 RVA: 0x00005D5C File Offset: 0x00003F5C
		private static IEnumerable<string> GetFilesUnderPrebuiltPath(string prebuiltPath, string searchPattern)
		{
			bool flag = !ReliableDirectory.Exists(prebuiltPath, Settings.Default.ShareAccessRetryCount, TimeSpan.FromMilliseconds((double)Settings.Default.ShareAccessRetryDelayInMs));
			if (flag)
			{
				throw new DirectoryNotFoundException(prebuiltPath);
			}
			List<string> list = new List<string>();
			string excludedSubDirsUnderPrebuiltPath = Settings.Default.ExcludedSubDirsUnderPrebuiltPath;
			IEnumerable<string> subDirectoriesToExclude = from x in excludedSubDirsUnderPrebuiltPath.Split(new char[]
			{
				';'
			}, StringSplitOptions.RemoveEmptyEntries)
			select x.ToLowerInvariant();
			IEnumerable<string> enumerable = from x in ReliableDirectory.GetDirectories(prebuiltPath, Settings.Default.ShareAccessRetryCount, TimeSpan.FromMilliseconds((double)Settings.Default.ShareAccessRetryDelayInMs))
			where !subDirectoriesToExclude.Contains(Path.GetFileName(x).ToLowerInvariant())
			select x;
			list.AddRange(ReliableDirectory.GetFiles(prebuiltPath, searchPattern, SearchOption.TopDirectoryOnly, Settings.Default.ShareAccessRetryCount, TimeSpan.FromMilliseconds((double)Settings.Default.ShareAccessRetryDelayInMs)).ToList<string>());
			foreach (string path in enumerable)
			{
				list.AddRange(ReliableDirectory.GetFiles(path, searchPattern, SearchOption.AllDirectories, Settings.Default.ShareAccessRetryCount, TimeSpan.FromMilliseconds((double)Settings.Default.ShareAccessRetryDelayInMs)).ToList<string>());
			}
			return list;
		}

		// Token: 0x060000E8 RID: 232 RVA: 0x00005EC4 File Offset: 0x000040C4
		private static string GetWinBuildPathForPublicBuild(string path)
		{
			string text = path;
			string path2 = "windows.ini";
			string winBuildSharePrefix = Settings.Default.WinBuildSharePrefix;
			string text2 = "WINDOWS_BUILD=";
			string path3 = string.Empty;
			string text3 = string.Empty;
			string text4 = string.Empty;
			string text5 = string.Empty;
			string text6 = string.Empty;
			string text7 = string.Empty;
			DirectoryInfo directoryInfo = new DirectoryInfo(text);
			try
			{
				bool flag = false;
				while (!string.IsNullOrEmpty(text) && directoryInfo.Parent != null)
				{
					bool flag2 = File.Exists(Path.Combine(text, path2));
					if (flag2)
					{
						flag = true;
						break;
					}
					directoryInfo = directoryInfo.Parent;
					text = directoryInfo.FullName;
				}
				bool flag3 = !flag;
				if (flag3)
				{
					Logger.Info("Did not find Windows.ini in the Path and its parent directories.", new object[0]);
					return string.Empty;
				}
				path3 = Path.Combine(text, path2);
				DirectoryInfo directoryInfo2 = new DirectoryInfo(text);
				text3 = directoryInfo2.Parent.Name;
				StreamReader streamReader = new StreamReader(path3);
				while ((text5 = streamReader.ReadLine()) != null)
				{
					bool flag4 = text5.StartsWith(text2, StringComparison.OrdinalIgnoreCase);
					if (flag4)
					{
						text4 = Regex.Replace(text5, text2, string.Empty, RegexOptions.IgnoreCase);
						text4 = Regex.Replace(text4, text3 + ".", string.Empty, RegexOptions.IgnoreCase);
						break;
					}
				}
				bool flag5 = string.IsNullOrEmpty(text4);
				if (flag5)
				{
					Logger.Info("Could not get the Windows_build info from the windows.ini.", new object[0]);
					return string.Empty;
				}
				text6 = PathHelper.winBuildArchitectures.First((string x) => path.IndexOf(x, StringComparison.OrdinalIgnoreCase) >= 0);
				bool flag6 = string.IsNullOrEmpty(text6);
				if (flag6)
				{
					Logger.Info("Could not decide the build architecture from the path.", new object[0]);
					return string.Empty;
				}
				bool flag7 = string.Compare(text6, "armfre", true) == 0 || string.Compare(text6, "armchk", true) == 0;
				if (flag7)
				{
					text6 = Regex.Replace(text6, "arm", "woa", RegexOptions.IgnoreCase);
				}
				text7 = Path.Combine(new string[]
				{
					winBuildSharePrefix,
					text3,
					text4,
					text6,
					"Build",
					PathHelper.PrebuiltFolderName
				});
				bool flag8 = !Directory.Exists(text7);
				if (flag8)
				{
					Logger.Info(string.Format("WinBuildPath {0} is not available.", text7), new object[0]);
					return string.Empty;
				}
				Logger.Info(string.Format("Found WinBuildPath {0}.", text7), new object[0]);
				return text7;
			}
			catch (Exception ex)
			{
				Logger.Info(string.Format("Error in GetWinBuildPathForPublicBuild. Error {0}", ex.Message), new object[0]);
			}
			return string.Empty;
		}

		// Token: 0x060000E9 RID: 233 RVA: 0x0000619C File Offset: 0x0000439C
		private static string GetWinBuildPathForLocalPath(string path)
		{
			string fullPath = Path.GetFullPath(path);
			string containingFolderPath = PathHelper.GetContainingFolderPath(fullPath, PathHelper.PrebuiltFolderName);
			string text = string.Empty;
			string text2 = string.Empty;
			try
			{
				bool flag = string.IsNullOrEmpty(containingFolderPath);
				if (flag)
				{
					bool flag2 = Directory.Exists(Path.Combine(fullPath, PathHelper.PrebuiltFolderName));
					if (flag2)
					{
						text = path;
					}
				}
				else
				{
					DirectoryInfo directoryInfo = new DirectoryInfo(containingFolderPath);
					bool flag3 = directoryInfo.Parent != null;
					if (flag3)
					{
						text = directoryInfo.Parent.FullName;
					}
				}
				bool flag4 = string.IsNullOrEmpty(text);
				if (flag4)
				{
					Logger.Info("Path {0} does not appear like a local phone build location. Stop searching for corresponding Windows build path.", new object[]
					{
						text
					});
					return string.Empty;
				}
				string fileName = Path.GetFileName(text);
				string pattern = ".+\\.binaries\\.[arm|amd64|x86|arm64]+[chk|fre]+\\..+\\.[MC|NT]+";
				bool flag5 = !Regex.IsMatch(fileName, pattern, RegexOptions.IgnoreCase);
				if (flag5)
				{
					Logger.Info("Path {0} does not appear like a local phone build location. Stop searching for corresponding Windows build path.", new object[]
					{
						text
					});
					return string.Empty;
				}
				DirectoryInfo directoryInfo2 = new DirectoryInfo(text);
				bool flag6 = directoryInfo2.Parent == null;
				if (flag6)
				{
					Logger.Info("Path {0} does not have a parent directory. Stop searching for corresponding Windows build path.", new object[]
					{
						text
					});
					return string.Empty;
				}
				string[] array = fileName.Split(new char[]
				{
					'.'
				});
				string path2 = string.Concat(new object[]
				{
					array[0],
					'.',
					array[1],
					'.',
					array[2]
				});
				text2 = Path.Combine(directoryInfo2.Parent.FullName, path2);
				text2 = Path.Combine(text2, PathHelper.PrebuiltFolderName);
				bool flag7 = !Directory.Exists(text2);
				if (flag7)
				{
					Logger.Info(string.Format("WinBuildPath {0} is not available.", text2), new object[0]);
					return string.Empty;
				}
				Logger.Info(string.Format("Found WinBuildPath {0}.", text2), new object[0]);
				return text2;
			}
			catch (IOException ex)
			{
				Logger.Info(string.Format("Error in GetWinBuildPathForLocalPath. Error {0}", ex.Message), new object[0]);
			}
			return string.Empty;
		}

		// Token: 0x0400005A RID: 90
		private static readonly string DirectorySeparator = new string(new char[]
		{
			Path.DirectorySeparatorChar
		});

		// Token: 0x0400005B RID: 91
		private static string[] winBuildArchitectures = new string[]
		{
			"armfre",
			"armchk",
			"arm64fre",
			"arm64chk",
			"amd64fre",
			"amd64chk",
			"x86fre",
			"x86chk"
		};

		// Token: 0x0400005C RID: 92
		private static string[] phoneBuildArchitectures = new string[]
		{
			"MC.amd64chk",
			"MC.amd64fre",
			"MC.arm64chk",
			"MC.arm64fre",
			"MC.armchk",
			"MC.armfre",
			"MC.x86chk",
			"MC.x86fre",
			"NT.amd64fre",
			"NT.x86fre"
		};
	}
}
