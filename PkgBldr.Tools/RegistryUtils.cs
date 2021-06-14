using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Threading;

namespace Microsoft.CompPlat.PkgBldr.Tools
{
	// Token: 0x02000035 RID: 53
	public static class RegistryUtils
	{
		// Token: 0x1700002F RID: 47
		// (get) Token: 0x060001B6 RID: 438 RVA: 0x00007EF4 File Offset: 0x000060F4
		public static Dictionary<SystemRegistryHiveFiles, string> KnownMountPoints
		{
			get
			{
				return RegistryUtils.MountPoints;
			}
		}

		// Token: 0x060001B7 RID: 439 RVA: 0x00007EFC File Offset: 0x000060FC
		public static void ConvertSystemHiveToRegFile(DriveInfo systemDrive, SystemRegistryHiveFiles hive, string outputRegFile)
		{
			LongPathDirectory.CreateDirectory(LongPath.GetDirectoryName(outputRegFile));
			RegistryUtils.ConvertHiveToRegFile(LongPath.Combine(LongPath.Combine(systemDrive.RootDirectory.FullName, "windows\\system32\\config"), Enum.GetName(typeof(SystemRegistryHiveFiles), hive)), RegistryUtils.MapHiveToMountPoint(hive), outputRegFile);
		}

		// Token: 0x060001B8 RID: 440 RVA: 0x00007F4F File Offset: 0x0000614F
		public static void ConvertHiveToRegFile(string inputhive, string targetRootKey, string outputRegFile)
		{
			OfflineRegUtils.ConvertHiveToReg(inputhive, outputRegFile, targetRootKey);
		}

		// Token: 0x060001B9 RID: 441 RVA: 0x00007F5C File Offset: 0x0000615C
		public static void LoadHive(string inputhive, string mountpoint)
		{
			string args = string.Format(CultureInfo.InvariantCulture, "LOAD {0} {1}", new object[]
			{
				mountpoint,
				inputhive
			});
			string command = Environment.ExpandEnvironmentVariables("%windir%\\System32\\REG.EXE");
			int num = CommonUtils.RunProcess(Environment.CurrentDirectory, command, args, true);
			if (0 < num)
			{
				throw new Win32Exception(num);
			}
			Thread.Sleep(500);
		}

		// Token: 0x060001BA RID: 442 RVA: 0x00007FB8 File Offset: 0x000061B8
		public static void ExportHive(string mountpoint, string outputfile)
		{
			string args = string.Format(CultureInfo.InvariantCulture, "EXPORT {0} {1}", new object[]
			{
				mountpoint,
				outputfile
			});
			string command = Environment.ExpandEnvironmentVariables("%windir%\\System32\\REG.EXE");
			int num = CommonUtils.RunProcess(Environment.CurrentDirectory, command, args, true);
			if (0 < num)
			{
				throw new Win32Exception(num);
			}
			Thread.Sleep(500);
		}

		// Token: 0x060001BB RID: 443 RVA: 0x00008014 File Offset: 0x00006214
		public static void UnloadHive(string mountpoint)
		{
			string args = string.Format(CultureInfo.InvariantCulture, "UNLOAD {0}", new object[]
			{
				mountpoint
			});
			string command = Environment.ExpandEnvironmentVariables("%windir%\\System32\\REG.EXE");
			int num = CommonUtils.RunProcess(Environment.CurrentDirectory, command, args, true);
			if (0 < num)
			{
				throw new Win32Exception(num);
			}
		}

		// Token: 0x060001BC RID: 444 RVA: 0x0000805F File Offset: 0x0000625F
		public static string MapHiveToMountPoint(SystemRegistryHiveFiles hive)
		{
			return RegistryUtils.KnownMountPoints[hive];
		}

		// Token: 0x060001BD RID: 445 RVA: 0x0000806C File Offset: 0x0000626C
		public static string MapHiveFileToMountPoint(string hiveFile)
		{
			if (string.IsNullOrEmpty(hiveFile))
			{
				throw new InvalidOperationException("hiveFile cannot be empty");
			}
			SystemRegistryHiveFiles hive;
			if (!RegistryUtils.hiveMap.TryGetValue(LongPath.GetFileName(hiveFile), out hive))
			{
				return "";
			}
			return RegistryUtils.MapHiveToMountPoint(hive);
		}

		// Token: 0x0400008A RID: 138
		private static Dictionary<string, SystemRegistryHiveFiles> hiveMap = new Dictionary<string, SystemRegistryHiveFiles>(StringComparer.OrdinalIgnoreCase)
		{
			{
				"SOFTWARE",
				SystemRegistryHiveFiles.SOFTWARE
			},
			{
				"SYSTEM",
				SystemRegistryHiveFiles.SYSTEM
			},
			{
				"DRIVERS",
				SystemRegistryHiveFiles.DRIVERS
			},
			{
				"DEFAULT",
				SystemRegistryHiveFiles.DEFAULT
			},
			{
				"SAM",
				SystemRegistryHiveFiles.SAM
			},
			{
				"SECURITY",
				SystemRegistryHiveFiles.SECURITY
			},
			{
				"BCD",
				SystemRegistryHiveFiles.BCD
			},
			{
				"NTUSER.DAT",
				SystemRegistryHiveFiles.CURRENTUSER
			}
		};

		// Token: 0x0400008B RID: 139
		private const string STR_REG_LOAD = "LOAD {0} {1}";

		// Token: 0x0400008C RID: 140
		private const string STR_REG_EXPORT = "EXPORT {0} {1}";

		// Token: 0x0400008D RID: 141
		private const string STR_REG_UNLOAD = "UNLOAD {0}";

		// Token: 0x0400008E RID: 142
		private const string STR_REGEXE = "%windir%\\System32\\REG.EXE";

		// Token: 0x0400008F RID: 143
		private static readonly Dictionary<SystemRegistryHiveFiles, string> MountPoints = new Dictionary<SystemRegistryHiveFiles, string>
		{
			{
				SystemRegistryHiveFiles.SOFTWARE,
				"HKEY_LOCAL_MACHINE\\SOFTWARE"
			},
			{
				SystemRegistryHiveFiles.SYSTEM,
				"HKEY_LOCAL_MACHINE\\SYSTEM"
			},
			{
				SystemRegistryHiveFiles.DRIVERS,
				"HKEY_LOCAL_MACHINE\\DRIVERS"
			},
			{
				SystemRegistryHiveFiles.DEFAULT,
				"HKEY_USERS\\.DEFAULT"
			},
			{
				SystemRegistryHiveFiles.SAM,
				"HKEY_LOCAL_MACHINE\\SAM"
			},
			{
				SystemRegistryHiveFiles.SECURITY,
				"HKEY_LOCAL_MACHINE\\SECURITY"
			},
			{
				SystemRegistryHiveFiles.BCD,
				"HKEY_LOCAL_MACHINE\\BCD"
			},
			{
				SystemRegistryHiveFiles.CURRENTUSER,
				"HKEY_CURRENT_USER"
			}
		};
	}
}
