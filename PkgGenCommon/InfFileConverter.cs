using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.WindowsPhone.ImageUpdate.PkgCommon;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents.Internal;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon
{
	// Token: 0x02000020 RID: 32
	public class InfFileConverter
	{
		// Token: 0x06000052 RID: 82 RVA: 0x0000300C File Offset: 0x0000120C
		public static void DoConvert(string infFile, string[] references, string[] stagingSubdirs, string hiveRoot, string wimRoot, string targetPartition, CpuId cpu, string diffedHivesPath, string productName, string toolPaths)
		{
			InfFileConverter.<>c__DisplayClass1_0 CS$<>8__locals1 = new InfFileConverter.<>c__DisplayClass1_0();
			CS$<>8__locals1.diffedHivesPath = diffedHivesPath;
			if (string.IsNullOrEmpty(infFile))
			{
				throw new ArgumentNullException("infFile");
			}
			if (CS$<>8__locals1.diffedHivesPath == null)
			{
				throw new ArgumentNullException("diffedHivesPath");
			}
			if (string.IsNullOrEmpty(hiveRoot))
			{
				throw new ArgumentNullException("hiveRoot");
			}
			if (string.IsNullOrEmpty(targetPartition))
			{
				throw new ArgumentNullException("targetPartition");
			}
			if (string.IsNullOrEmpty(CS$<>8__locals1.diffedHivesPath))
			{
				throw new ArgumentNullException("diffedHivesPath");
			}
			if (references != null)
			{
				if (stagingSubdirs == null)
				{
					throw new ArgumentNullException("stagingSubdirs");
				}
				if (references.Length != stagingSubdirs.Length)
				{
					throw new ArgumentException("Input parameters 'References' and 'StagingSubDirs' should have same size");
				}
			}
			CS$<>8__locals1.baselineHivesPath = FileUtils.GetTempDirectory();
			CS$<>8__locals1.mountPath = FileUtils.GetTempDirectory();
			string text = null;
			string defaultDriveLetter = PackageTools.GetDefaultDriveLetter(targetPartition);
			string path;
			if (string.IsNullOrEmpty(productName) || productName == "$(PRODUCT_NAME)")
			{
				path = "mobilecoreprod.wim";
			}
			else
			{
				path = productName + ".wim";
			}
			string text2 = string.Empty;
			LongPathDirectory.CreateDirectory(CS$<>8__locals1.mountPath);
			if (hiveRoot.Equals("no:hives"))
			{
				InfFileConverter._noHives = true;
			}
			try
			{
				InfFileConverter.<>c__DisplayClass1_1 CS$<>8__locals2 = new InfFileConverter.<>c__DisplayClass1_1();
				CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
				InfFileConverter.<>c__DisplayClass1_1 CS$<>8__locals3 = CS$<>8__locals2;
				CS$<>8__locals3.driverStore = new DrvStore(CS$<>8__locals3.CS$<>8__locals1.mountPath, defaultDriveLetter);
				try
				{
					if (!InfFileConverter._noHives && CS$<>8__locals2.driverStore.DriverIncludesInfs(infFile, cpu))
					{
						text2 = Path.Combine(wimRoot, path);
						if (!LongPathFile.Exists(text2))
						{
							text2 = Path.Combine(hiveRoot, path);
							LogUtil.Warning("WIM_ROOT parameter is needed but no valid path was passed, trying {0}", new object[]
							{
								text2
							});
							if (File.Exists(text2))
							{
								LogUtil.Diagnostic("WIM located");
							}
							else
							{
								LogUtil.Warning("Unable to locate WIM, falling back to hives");
								text2 = string.Empty;
							}
						}
						if (text2 != string.Empty)
						{
							InfFileConverter.ApplyWIM(text2, CS$<>8__locals2.CS$<>8__locals1.mountPath, toolPaths);
						}
					}
					if (InfFileConverter._noHives)
					{
						LogUtil.Diagnostic("Creating driver store with deconstructed state");
						CS$<>8__locals2.driverStore.Create();
						CS$<>8__locals2.driverStore.SetupConfigOptions(51U);
						CS$<>8__locals2.driverStore.Close();
						LogUtil.Diagnostic("Saving hive baselines");
						InfFileConverter.CopyFromMappings(InfFileConverter.CreateHiveCopyMappings(CS$<>8__locals2.driverStore.HivePath, CS$<>8__locals2.CS$<>8__locals1.baselineHivesPath));
						CS$<>8__locals2.driverStore.Create();
						CS$<>8__locals2.driverStore.SetupConfigOptions(51U);
					}
					else
					{
						CS$<>8__locals2.driverStore.Create();
						CS$<>8__locals2.driverStore.Close();
						if (text2 == string.Empty)
						{
							LogUtil.Diagnostic("Building driverstore from hives");
							LogUtil.Diagnostic("Copying hives from HIVE_ROOT into image");
							InfFileConverter.CopyFromMappings(InfFileConverter.CreateHiveCopyMappings(hiveRoot, CS$<>8__locals2.driverStore.HivePath));
						}
						else
						{
							LogUtil.Diagnostic("Using driverstore from WIM");
						}
						LogUtil.Diagnostic("Saving hive baselines");
						InfFileConverter.CopyFromMappings(InfFileConverter.CreateHiveCopyMappings(CS$<>8__locals2.driverStore.HivePath, CS$<>8__locals2.CS$<>8__locals1.baselineHivesPath));
						CS$<>8__locals2.driverStore.Create();
					}
					text = CS$<>8__locals2.driverStore.ImportLogPath;
					LogUtil.Diagnostic("importLogPath = {0}", new object[]
					{
						text
					});
					if (!string.IsNullOrEmpty(text))
					{
						InfFileConverter.RenameLogFile(text);
					}
					CS$<>8__locals2.driverStore.ImportDriver(infFile, references, stagingSubdirs, cpu);
					CS$<>8__locals2.driverStore.Close();
					InfFileConverter.RetryWithWait(delegate
					{
						InfFileConverter.ComputeHiveDiff(CS$<>8__locals2.CS$<>8__locals1.baselineHivesPath, CS$<>8__locals2.driverStore.HivePath, CS$<>8__locals2.CS$<>8__locals1.diffedHivesPath);
					}, 10, InfFileConverter.c_wait);
				}
				finally
				{
					if (CS$<>8__locals2.driverStore != null)
					{
						((IDisposable)CS$<>8__locals2.driverStore).Dispose();
					}
				}
			}
			catch (Win32Exception ex)
			{
				LogUtil.Diagnostic(ex.ToString());
				throw new IUException(string.Format("Error encountered staging {0}", infFile), ex);
			}
			catch (InvalidOperationException ex2)
			{
				LogUtil.Diagnostic(ex2.ToString());
				throw new IUException(string.Format("Error encountered staging {0}", infFile), ex2);
			}
			catch (ArgumentNullException ex3)
			{
				LogUtil.Diagnostic(ex3.ToString());
				throw new IUException(string.Format("Error encountered staging {0}", infFile), ex3);
			}
			finally
			{
				if (!string.IsNullOrEmpty(text))
				{
					InfFileConverter.LogFile(text, "Import Log: ");
				}
				InfFileConverter.RetryWithWait(delegate
				{
					FileUtils.DeleteTree(CS$<>8__locals1.mountPath);
					FileUtils.DeleteTree(CS$<>8__locals1.baselineHivesPath);
				}, 10, InfFileConverter.c_wait);
			}
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000053 RID: 83 RVA: 0x00003484 File Offset: 0x00001684
		public static bool NoHives
		{
			get
			{
				return InfFileConverter._noHives;
			}
		}

		// Token: 0x06000054 RID: 84 RVA: 0x0000348C File Offset: 0x0000168C
		public static Dictionary<string, InfFileConverter.RegKeyData> ExtractKeys(string diffedHivesPath, SystemRegistryHiveFiles hive)
		{
			Dictionary<string, InfFileConverter.RegKeyData> result = new Dictionary<string, InfFileConverter.RegKeyData>(StringComparer.OrdinalIgnoreCase);
			string hivefile = Path.Combine(diffedHivesPath, Enum.GetName(typeof(SystemRegistryHiveFiles), hive));
			string prefix = RegistryUtils.MapHiveToMountPoint(hive);
			using (ORRegistryKey orregistryKey = ORRegistryKey.OpenHive(hivefile, prefix))
			{
				InfFileConverter.PopulateRegKeyTable(orregistryKey, ref result);
			}
			return result;
		}

		// Token: 0x06000055 RID: 85 RVA: 0x000034F4 File Offset: 0x000016F4
		private static void PopulateRegKeyTable(ORRegistryKey key, ref Dictionary<string, InfFileConverter.RegKeyData> regKeyTable)
		{
			if (InfFileConverter.RegKeyExclusionListAll.Contains(key.FullName.ToLowerInvariant()))
			{
				return;
			}
			bool flag = false;
			bool flag2 = false;
			if (!regKeyTable.ContainsKey(key.FullName))
			{
				if (InfFileConverter.NoHives && InfFileConverter.RegKeyExclusionListNoHives.Contains(key.FullName.ToLowerInvariant()))
				{
					flag = true;
					flag2 = true;
				}
				if (!flag)
				{
					regKeyTable.Add(key.FullName, new InfFileConverter.RegKeyData());
				}
			}
			if (!flag2)
			{
				List<InfFileConverter.RegKeyValue> valueList = regKeyTable[key.FullName].ValueList;
				foreach (KeyValuePair<string, RegistryValueType> keyValuePair in key.ValueNameAndTypes)
				{
					InfFileConverter.RegKeyValue regKeyValue = new InfFileConverter.RegKeyValue();
					if (keyValuePair.Value == RegistryValueType.MultiString)
					{
						regKeyValue.IsMultiSz = true;
						regKeyValue.MultiSzName = keyValuePair.Key;
						regKeyValue.MultiSzValue = key.GetMultiStringValue(keyValuePair.Key);
					}
					else
					{
						regKeyValue.IsMultiSz = false;
						regKeyValue.Value = InfFileConverter.ConvertRegValue(key, keyValuePair.Key, keyValuePair.Value);
					}
					valueList.Add(regKeyValue);
				}
			}
			foreach (string subkeyname in key.SubKeys)
			{
				InfFileConverter.PopulateRegKeyTable(key.OpenSubKey(subkeyname), ref regKeyTable);
			}
		}

		// Token: 0x06000056 RID: 86 RVA: 0x00003654 File Offset: 0x00001854
		private static RegValue ConvertRegValue(ORRegistryKey key, string valueName, RegistryValueType type)
		{
			RegValue regValue = new RegValue();
			regValue.Name = valueName;
			switch (type)
			{
			case RegistryValueType.String:
				regValue.RegValType = RegValueType.String;
				regValue.Value = key.GetStringValue(valueName);
				break;
			case RegistryValueType.ExpandString:
				regValue.RegValType = RegValueType.ExpandString;
				regValue.Value = key.GetStringValue(valueName);
				break;
			case RegistryValueType.Binary:
				regValue.RegValType = RegValueType.Binary;
				regValue.Value = BitConverter.ToString(key.GetByteValue(valueName)).Replace('-', ',');
				break;
			case RegistryValueType.DWord:
				regValue.RegValType = RegValueType.DWord;
				regValue.Value = key.GetDwordValue(valueName).ToString("X8");
				break;
			default:
				if (type != RegistryValueType.QWord)
				{
					regValue.RegValType = RegValueType.Hex;
					regValue.Value = string.Format("hex({0:X}):", (int)key.GetValueKind(valueName)) + BitConverter.ToString(key.GetByteValue(valueName)).Replace('-', ',');
				}
				else
				{
					regValue.RegValType = RegValueType.QWord;
					regValue.Value = key.GetQwordValue(valueName).ToString("X16");
				}
				break;
			}
			return regValue;
		}

		// Token: 0x06000057 RID: 87 RVA: 0x00003770 File Offset: 0x00001970
		public static void RetryWithWait(InfFileConverter.Operation operation, int retries, TimeSpan wait)
		{
			while (retries > 0)
			{
				try
				{
					operation();
					break;
				}
				catch (Exception ex)
				{
					LogUtil.Diagnostic(ex.ToString());
					retries--;
					if (retries <= 0)
					{
						throw;
					}
					Thread.Sleep(wait);
				}
			}
		}

		// Token: 0x06000058 RID: 88 RVA: 0x000037BC File Offset: 0x000019BC
		public static void LogFile(string file, string prefix)
		{
			if (LongPathFile.Exists(file))
			{
				using (StreamReader streamReader = new StreamReader(file))
				{
					while (!streamReader.EndOfStream)
					{
						string text = streamReader.ReadLine();
						if (text.StartsWith("!!!", StringComparison.InvariantCulture))
						{
							LogUtil.Error("{0}: {1}", new object[]
							{
								prefix,
								text
							});
						}
						else if (text.StartsWith("!", StringComparison.InvariantCulture))
						{
							LogUtil.Warning("{0}: {1}", new object[]
							{
								prefix,
								text
							});
						}
						else
						{
							LogUtil.Message("{0}: {1}", new object[]
							{
								prefix,
								text
							});
						}
					}
				}
			}
		}

		// Token: 0x06000059 RID: 89 RVA: 0x00003870 File Offset: 0x00001A70
		public static void RenameLogFile(string file)
		{
			if (LongPathFile.Exists(file))
			{
				try
				{
					LongPathFile.Move(file, file + ".old");
				}
				catch
				{
				}
			}
		}

		// Token: 0x0600005A RID: 90 RVA: 0x000038AC File Offset: 0x00001AAC
		public static void ComputeHiveDiff(string baseHivePath, string newHivePath, string diffHivePath)
		{
			LogUtil.Diagnostic("Computing difference hives between {0} and {1} and exporting to {2}", new object[]
			{
				baseHivePath,
				newHivePath,
				diffHivePath
			});
			int num = InfFileConverter.NativeMethods.ExportRegistryHiveDeltas(baseHivePath, newHivePath, diffHivePath);
			if (num != 0)
			{
				throw new Win32Exception(num);
			}
			InfFileConverter.ComputeMultiSZDiff(baseHivePath, diffHivePath);
		}

		// Token: 0x0600005B RID: 91 RVA: 0x000038F0 File Offset: 0x00001AF0
		private static void ComputeMultiSZDiff(string baseHivePath, string diffHivePath)
		{
			foreach (string text in LongPathDirectory.GetFiles(diffHivePath))
			{
				string text2 = Path.Combine(baseHivePath, Path.GetFileName(text));
				if (LongPathFile.Exists(text2))
				{
					using (ORRegistryKey orregistryKey = ORRegistryKey.OpenHive(text))
					{
						using (ORRegistryKey orregistryKey2 = ORRegistryKey.OpenHive(text2))
						{
							string fullPath = LongPath.GetFullPath(text);
							try
							{
								InfFileConverter.RemoveMultiSZDuplicates(orregistryKey, orregistryKey2);
							}
							catch (Exception innerException)
							{
								throw new Exception(string.Format("Error computing Multi_SZ diff in {0}.", fullPath), innerException);
							}
							orregistryKey.SaveHive(fullPath);
						}
					}
				}
			}
		}

		// Token: 0x0600005C RID: 92 RVA: 0x000039B0 File Offset: 0x00001BB0
		private static void RemoveMultiSZDuplicates(ORRegistryKey diffKey, ORRegistryKey baseKey)
		{
			foreach (string subkeyname in diffKey.SubKeys.Intersect(baseKey.SubKeys, StringComparer.OrdinalIgnoreCase))
			{
				using (ORRegistryKey orregistryKey = diffKey.OpenSubKey(subkeyname))
				{
					using (ORRegistryKey orregistryKey2 = baseKey.OpenSubKey(subkeyname))
					{
						InfFileConverter.RemoveMultiSZDuplicates(orregistryKey, orregistryKey2);
					}
				}
			}
			foreach (string text in diffKey.ValueNames.Intersect(baseKey.ValueNames, StringComparer.OrdinalIgnoreCase))
			{
				if (diffKey.GetValueKind(text) == RegistryValueType.MultiString && baseKey.GetValueKind(text) == RegistryValueType.MultiString)
				{
					string[] multiStringValue = diffKey.GetMultiStringValue(text);
					string[] multiStringValue2 = baseKey.GetMultiStringValue(text);
					if (multiStringValue2.Except(multiStringValue, StringComparer.OrdinalIgnoreCase).Any<string>())
					{
						throw new PkgGenException("Multi_SZ elements were removed during driver ingestion in {0}\\{1}", new object[]
						{
							diffKey.FullName,
							text
						});
					}
					string[] values = multiStringValue.Except(multiStringValue2, StringComparer.OrdinalIgnoreCase).ToArray<string>();
					diffKey.SetValue(text, values);
				}
			}
		}

		// Token: 0x0600005D RID: 93 RVA: 0x00003B14 File Offset: 0x00001D14
		public static Dictionary<string, string> CreateHiveCopyMappings(string srcHivePath, string dstHivePath)
		{
			string[] array = new string[]
			{
				"SYSTEM",
				"DRIVERS",
				"SOFTWARE"
			};
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			foreach (string path in array)
			{
				dictionary.Add(Path.Combine(srcHivePath, path), Path.Combine(dstHivePath, path));
			}
			return dictionary;
		}

		// Token: 0x0600005E RID: 94 RVA: 0x00003B70 File Offset: 0x00001D70
		public static void CopyFromMappings(Dictionary<string, string> mappings)
		{
			foreach (string text in mappings.Keys)
			{
				string text2 = mappings[text];
				LogUtil.Diagnostic("Copying {0} to {1}", new object[]
				{
					text,
					text2
				});
				try
				{
					LongPathFile.Copy(text, text2, true);
				}
				catch (FileNotFoundException)
				{
					if (!text2.Contains("SOFTWARE"))
					{
						LogUtil.Error("Failed to copy {0}", new object[]
						{
							text
						});
						throw;
					}
				}
			}
		}

		// Token: 0x0600005F RID: 95 RVA: 0x00003C18 File Offset: 0x00001E18
		public static void ApplyWIM(string inputWIM, string targetDirectory, string toolPaths)
		{
			LogUtil.Diagnostic("Applying WIM {0} to directory {1}", new object[]
			{
				inputWIM,
				targetDirectory
			});
			Process process = new Process();
			process.StartInfo.FileName = InfFileConverter.LocateUtil("dism.exe", toolPaths);
			process.StartInfo.Arguments = string.Format("/apply-image /imagefile=\"{0}\" /Index:1 /applydir=\"{1}\"", inputWIM, targetDirectory);
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardError = true;
			string text = string.Empty;
			short num = 5;
			while (num > 0)
			{
				process.Start();
				text = process.StandardOutput.ReadToEnd();
				process.WaitForExit();
				if (process.ExitCode == 0)
				{
					num = 0;
				}
				else
				{
					num -= 1;
					Thread.Sleep(2000);
				}
			}
			if (process.ExitCode != 0)
			{
				LogUtil.Error(text);
				throw new IUException("Failed to apply {0}, exit code {1}", new object[]
				{
					inputWIM,
					process.ExitCode
				});
			}
			LogUtil.Message(text);
		}

		// Token: 0x06000060 RID: 96 RVA: 0x00003D10 File Offset: 0x00001F10
		private static string LocateUtil(string razzleCmd, string toolPaths)
		{
			if (LongPathFile.Exists(Path.Combine(Directory.GetCurrentDirectory(), razzleCmd)))
			{
				return Path.Combine(Directory.GetCurrentDirectory(), razzleCmd);
			}
			if (!string.IsNullOrWhiteSpace(toolPaths))
			{
				string[] array = toolPaths.Split(new char[]
				{
					';'
				});
				for (int i = 0; i < array.Length; i++)
				{
					string text = Path.Combine(Environment.ExpandEnvironmentVariables(array[i]), razzleCmd);
					if (LongPathFile.Exists(text))
					{
						return text;
					}
				}
			}
			string environmentVariable = Environment.GetEnvironmentVariable("PATH");
			string text2 = string.Empty;
			if (!string.IsNullOrEmpty(environmentVariable))
			{
				text2 = (from p in Environment.GetEnvironmentVariable("PATH").Split(new char[]
				{
					';'
				})
				where LongPathFile.Exists(Path.Combine(p, razzleCmd))
				select p).FirstOrDefault<string>();
			}
			if (string.IsNullOrEmpty(text2))
			{
				throw new FileNotFoundException("Could not find {0} in the environment path", razzleCmd);
			}
			return Path.Combine(text2, razzleCmd);
		}

		// Token: 0x040000BE RID: 190
		public const string NO_HIVES = "no:hives";

		// Token: 0x040000BF RID: 191
		private static bool _noHives = false;

		// Token: 0x040000C0 RID: 192
		private static readonly List<string> RegKeyExclusionListAll = new List<string>
		{
			"hkey_local_machine\\system\\controlset001\\control\\grouporderlist",
			"hkey_local_machine\\system\\controlset001\\services\\wudfrd"
		};

		// Token: 0x040000C1 RID: 193
		private static readonly List<string> RegKeyExclusionListNoHives = new List<string>
		{
			"hkey_local_machine\\software\\microsoft\\windows\\currentversion\\setup",
			"hkey_local_machine\\software\\microsoft\\windows\\currentversion\\setup\\pnplockdownfiles",
			"hkey_local_machine\\software\\microsoft\\windows\\currentversion\\setup\\pnpresources",
			"hkey_local_machine\\system\\controlset001\\control\\class",
			"hkey_local_machine\\system\\driverdatabase",
			"hkey_local_machine\\system\\driverdatabase\\deviceids",
			"hkey_local_machine\\system\\driverdatabase\\driverinffiles",
			"hkey_local_machine\\system\\driverdatabase\\driverpackages"
		};

		// Token: 0x040000C2 RID: 194
		internal const int c_retries = 10;

		// Token: 0x040000C3 RID: 195
		internal static readonly TimeSpan c_wait = TimeSpan.FromSeconds(2.0);

		// Token: 0x040000C4 RID: 196
		private const string STR_HIVEPATH = "Windows\\System32\\Config";

		// Token: 0x040000C5 RID: 197
		private const string c_defaultWimFileName = "mobilecoreprod.wim";

		// Token: 0x040000C6 RID: 198
		private static readonly TimeSpan MutexTimeout = new TimeSpan(0, 6, 0, 0);

		// Token: 0x02000082 RID: 130
		public class RegKeyValue
		{
			// Token: 0x040001E4 RID: 484
			public bool IsMultiSz;

			// Token: 0x040001E5 RID: 485
			public RegValue Value;

			// Token: 0x040001E6 RID: 486
			public string MultiSzName;

			// Token: 0x040001E7 RID: 487
			public string[] MultiSzValue;
		}

		// Token: 0x02000083 RID: 131
		public class RegKeyData
		{
			// Token: 0x040001E8 RID: 488
			public string SDDL;

			// Token: 0x040001E9 RID: 489
			public List<InfFileConverter.RegKeyValue> ValueList = new List<InfFileConverter.RegKeyValue>();
		}

		// Token: 0x02000084 RID: 132
		// (Invoke) Token: 0x060002EA RID: 746
		public delegate void Operation();

		// Token: 0x02000085 RID: 133
		private static class NativeMethods
		{
			// Token: 0x060002ED RID: 749
			[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
			public static extern int ExportRegistryHiveDeltas(string baseHivesPath, string modifiedHivesPath, string outputHivesPath);
		}
	}
}
