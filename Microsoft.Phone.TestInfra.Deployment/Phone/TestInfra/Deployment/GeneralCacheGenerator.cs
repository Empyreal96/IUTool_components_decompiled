using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.Phone.TestInfra.Deployment
{
	// Token: 0x02000026 RID: 38
	public class GeneralCacheGenerator
	{
		// Token: 0x060001B0 RID: 432 RVA: 0x0000A220 File Offset: 0x00008420
		public static void DoWork(string outputPath, string rootPath)
		{
			bool flag = PathHelper.GetPathType(rootPath) != PathType.PhoneBuildPath;
			if (flag)
			{
				throw new InvalidDataException(string.Format("{0} is not a phone build path.", rootPath));
			}
			bool flag2 = !PathHelper.IsPrebuiltPath(rootPath);
			if (flag2)
			{
				throw new InvalidDataException(string.Format("{0} is not a prebuilt path.", rootPath));
			}
			string winBuildPath = PathHelper.GetWinBuildPath(rootPath);
			bool flag3 = string.IsNullOrEmpty(winBuildPath);
			if (flag3)
			{
				throw new InvalidDataException(string.Format("{0} does not have a corresponding Windows build path.", rootPath));
			}
			HashSet<string> prebuiltPaths = PathHelper.GetPrebuiltPaths(winBuildPath, false);
			bool flag4 = prebuiltPaths.Count < 2;
			if (flag4)
			{
				throw new InvalidDataException(string.Format("{0} does not have a prebuilt folder under the bin chunk.", winBuildPath));
			}
			GeneralCacheGenerator.GenerateGeneralBinaryLocationCache(outputPath, winBuildPath);
			GeneralCacheGenerator.GenerateGeneralPackageLocationCache(outputPath, rootPath);
		}

		// Token: 0x060001B1 RID: 433 RVA: 0x0000A2D0 File Offset: 0x000084D0
		private static void GenerateGeneralPackageLocationCache(string outputPath, string rootPath)
		{
			bool flag = string.IsNullOrWhiteSpace(rootPath);
			if (flag)
			{
				throw new ArgumentException("cannot be null or empty", "rootPath");
			}
			bool flag2 = !ReliableDirectory.Exists(rootPath, Settings.Default.ShareAccessRetryCount, TimeSpan.FromMilliseconds((double)Settings.Default.ShareAccessRetryDelayInMs));
			if (flag2)
			{
				throw new DirectoryNotFoundException(rootPath);
			}
			SerializableDictionary<string, HashSet<string>> serializableDictionary = new SerializableDictionary<string, HashSet<string>>();
			List<string> list = new List<string>();
			list.AddRange(PathHelper.GetPrebuiltPaths(PathHelper.GetWinBuildPath(rootPath), false).ToArray<string>());
			list.AddRange(PathHelper.GetPrebuiltPaths(rootPath, false).ToArray<string>());
			foreach (string prebuiltPath in list)
			{
				IEnumerable<string> prebuiltPathForAllArchitectures = PathHelper.GetPrebuiltPathForAllArchitectures(prebuiltPath);
				foreach (string text in prebuiltPathForAllArchitectures)
				{
					bool flag3 = !ReliableDirectory.Exists(text, Settings.Default.ShareAccessRetryCount, TimeSpan.FromMilliseconds((double)Settings.Default.ShareAccessRetryDelayInMs));
					if (flag3)
					{
						Logger.Info("Path {0} is inaccessible, ignored.", new object[]
						{
							text
						});
					}
					else
					{
						Logger.Info("Scanning {0}...", new object[]
						{
							text
						});
						IEnumerable<string> packageFilesUnderPath = PathHelper.GetPackageFilesUnderPath(text);
						foreach (string text2 in packageFilesUnderPath)
						{
							int num = text2.LastIndexOf('\\');
							string text3 = PathHelper.GetPackageNameWithoutExtension(text2.Substring(num + 1)).ToLowerInvariant();
							text3 = PathHelper.GetFileNameWithoutExtension(text3, Constants.CabFileExtension);
							string item = PathHelper.ChangeParent(text2.Substring(0, num), text, string.Empty).ToLowerInvariant();
							bool flag4 = serializableDictionary.ContainsKey(text3);
							if (flag4)
							{
								serializableDictionary[text3].Add(item);
							}
							else
							{
								serializableDictionary[text3] = new HashSet<string>
								{
									item
								};
							}
						}
					}
				}
			}
			serializableDictionary.SerializeToFile(Path.Combine(outputPath, Constants.GeneralPackageLocationCacheFileName));
		}

		// Token: 0x060001B2 RID: 434 RVA: 0x0000A558 File Offset: 0x00008758
		private static void GenerateGeneralBinaryLocationCache(string outputPath, string rootPath)
		{
			bool flag = string.IsNullOrWhiteSpace(rootPath);
			if (flag)
			{
				throw new ArgumentException("cannot be null or empty", "rootPath");
			}
			bool flag2 = !ReliableDirectory.Exists(rootPath, Settings.Default.ShareAccessRetryCount, TimeSpan.FromMilliseconds((double)Settings.Default.ShareAccessRetryDelayInMs));
			if (flag2)
			{
				throw new DirectoryNotFoundException(rootPath);
			}
			Dictionary<string, PackageDescription> dictionary = null;
			foreach (string text in PathHelper.GetPrebuiltPaths(rootPath, false))
			{
				Dictionary<string, PackageDescription> dictionary2 = BinaryLocator.ScanBuild(text);
				bool flag3 = dictionary == null || !dictionary.Any<KeyValuePair<string, PackageDescription>>();
				if (flag3)
				{
					dictionary = dictionary2;
				}
				else
				{
					foreach (KeyValuePair<string, PackageDescription> keyValuePair in dictionary2)
					{
						bool flag4 = dictionary.ContainsKey(keyValuePair.Key);
						if (flag4)
						{
							Logger.Error("Found more than one packages named {0}, ignore the one uncer {1}", new object[]
							{
								keyValuePair.Key,
								text
							});
						}
						else
						{
							dictionary.Add(keyValuePair.Key, keyValuePair.Value);
						}
					}
				}
			}
			SerializableDictionary<string, HashSet<string>> serializableDictionary = BinaryLocator.ComputeBinaryPackageMapping(dictionary);
			serializableDictionary.SerializeToFile(Path.Combine(outputPath, Constants.GeneralBinaryPackageMappingCacheFileName));
			HashSet<string> hashSet = new HashSet<string>();
			string text2 = Path.Combine(Constants.AssemblyDirectory, Constants.SupressionFileName);
			bool flag5 = !File.Exists(text2);
			if (flag5)
			{
				throw new FileNotFoundException(text2);
			}
			IEnumerable<string> source = from x in File.ReadAllLines(text2)
			select x.Trim();
			string suppressBinaryPrefix = "BIN,*,*,";
			foreach (KeyValuePair<string, PackageDescription> keyValuePair2 in dictionary)
			{
				IEnumerable<string> enumerable = from x in keyValuePair2.Value.Dependencies
				where x is BinaryDependency
				select (x as BinaryDependency).FileName;
				using (IEnumerator<string> enumerator4 = enumerable.GetEnumerator())
				{
					while (enumerator4.MoveNext())
					{
						string binaryName = enumerator4.Current;
						bool flag6 = !serializableDictionary.ContainsKey(binaryName) && !source.Any((string x) => string.Compare(x, suppressBinaryPrefix + binaryName, true) == 0);
						if (flag6)
						{
							hashSet.Add(binaryName.ToLowerInvariant());
						}
					}
				}
			}
			string path = "binarySuppressToAppend.txt";
			string path2 = Path.Combine(outputPath, path);
			File.WriteAllLines(path2, from x in hashSet
			select suppressBinaryPrefix + x);
		}
	}
}
