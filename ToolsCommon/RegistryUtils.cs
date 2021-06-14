using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;

namespace Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
	// Token: 0x02000027 RID: 39
	public static class RegistryUtils
	{
		// Token: 0x17000019 RID: 25
		// (get) Token: 0x06000143 RID: 323 RVA: 0x00007C88 File Offset: 0x00005E88
		public static Dictionary<SystemRegistryHiveFiles, string> KnownMountPoints
		{
			get
			{
				return RegistryUtils.MountPoints;
			}
		}

		// Token: 0x06000144 RID: 324 RVA: 0x00007C90 File Offset: 0x00005E90
		public static void ConvertSystemHiveToRegFile(DriveInfo systemDrive, SystemRegistryHiveFiles hive, string outputRegFile)
		{
			LongPathDirectory.CreateDirectory(LongPath.GetDirectoryName(outputRegFile));
			RegistryUtils.ConvertHiveToRegFile(Path.Combine(Path.Combine(systemDrive.RootDirectory.FullName, "windows\\system32\\config"), Enum.GetName(typeof(SystemRegistryHiveFiles), hive)), RegistryUtils.MapHiveToMountPoint(hive), outputRegFile);
		}

		// Token: 0x06000145 RID: 325 RVA: 0x00007CE3 File Offset: 0x00005EE3
		public static void ConvertHiveToRegFile(string inputhive, string targetRootKey, string outputRegFile)
		{
			OfflineRegUtils.ConvertHiveToReg(inputhive, outputRegFile, targetRootKey);
		}

		// Token: 0x06000146 RID: 326 RVA: 0x00007CF0 File Offset: 0x00005EF0
		public static void LoadHive(string inputhive, string mountpoint)
		{
			string args = string.Format("LOAD {0} {1}", mountpoint, inputhive);
			string command = Environment.ExpandEnvironmentVariables("%windir%\\System32\\REG.EXE");
			int num = CommonUtils.RunProcess(Environment.CurrentDirectory, command, args, true);
			if (0 < num)
			{
				throw new Win32Exception(num);
			}
			Thread.Sleep(500);
		}

		// Token: 0x06000147 RID: 327 RVA: 0x00007D38 File Offset: 0x00005F38
		public static void ExportHive(string mountpoint, string outputfile)
		{
			string args = string.Format("EXPORT {0} {1}", mountpoint, outputfile);
			string command = Environment.ExpandEnvironmentVariables("%windir%\\System32\\REG.EXE");
			int num = CommonUtils.RunProcess(Environment.CurrentDirectory, command, args, true);
			if (0 < num)
			{
				throw new Win32Exception(num);
			}
			Thread.Sleep(500);
		}

		// Token: 0x06000148 RID: 328 RVA: 0x00007D80 File Offset: 0x00005F80
		public static void UnloadHive(string mountpoint)
		{
			string args = string.Format("UNLOAD {0}", mountpoint);
			string command = Environment.ExpandEnvironmentVariables("%windir%\\System32\\REG.EXE");
			int num = CommonUtils.RunProcess(Environment.CurrentDirectory, command, args, true);
			if (0 < num)
			{
				throw new Win32Exception(num);
			}
		}

		// Token: 0x06000149 RID: 329 RVA: 0x00007DBD File Offset: 0x00005FBD
		public static string MapHiveToMountPoint(SystemRegistryHiveFiles hive)
		{
			return RegistryUtils.KnownMountPoints[hive];
		}

		// Token: 0x0600014A RID: 330 RVA: 0x00007DCC File Offset: 0x00005FCC
		public static string MapHiveFileToMountPoint(string hiveFile)
		{
			if (string.IsNullOrEmpty(hiveFile))
			{
				throw new InvalidOperationException("hiveFile cannot be empty");
			}
			SystemRegistryHiveFiles hive;
			if (!RegistryUtils.hiveMap.TryGetValue(Path.GetFileName(hiveFile), out hive))
			{
				return "";
			}
			return RegistryUtils.MapHiveToMountPoint(hive);
		}

		// Token: 0x04000073 RID: 115
		private static Dictionary<string, SystemRegistryHiveFiles> hiveMap = new Dictionary<string, SystemRegistryHiveFiles>(StringComparer.InvariantCultureIgnoreCase)
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

		// Token: 0x04000074 RID: 116
		private const string STR_REG_LOAD = "LOAD {0} {1}";

		// Token: 0x04000075 RID: 117
		private const string STR_REG_EXPORT = "EXPORT {0} {1}";

		// Token: 0x04000076 RID: 118
		private const string STR_REG_UNLOAD = "UNLOAD {0}";

		// Token: 0x04000077 RID: 119
		private const string STR_REGEXE = "%windir%\\System32\\REG.EXE";

		// Token: 0x04000078 RID: 120
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
