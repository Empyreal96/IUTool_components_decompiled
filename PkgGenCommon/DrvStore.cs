using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.WindowsPhone.ImageUpdate.PkgCommon;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon
{
	// Token: 0x0200001F RID: 31
	public class DrvStore : IDisposable
	{
		// Token: 0x06000042 RID: 66 RVA: 0x00002904 File Offset: 0x00000B04
		public DrvStore(string stagingPath, string targetBootDrive)
		{
			this._stagingRootDirectory = Environment.ExpandEnvironmentVariables(stagingPath);
			this._stagingSystemDirectory = Path.Combine(this._stagingRootDirectory, "windows");
			this._targetBootDrive = targetBootDrive;
		}

		// Token: 0x06000043 RID: 67 RVA: 0x00002958 File Offset: 0x00000B58
		~DrvStore()
		{
			this.Dispose(false);
		}

		// Token: 0x06000044 RID: 68 RVA: 0x00002988 File Offset: 0x00000B88
		private void Initialize()
		{
			string directoryName = LongPath.GetDirectoryName(Assembly.GetEntryAssembly().Location);
			string text = Path.Combine(directoryName, "drvstore.dll");
			if (!LongPathFile.Exists(text))
			{
				throw new PkgGenException("Could not find {0} in folder {1}", new object[]
				{
					"drvstore.dll",
					directoryName
				});
			}
			this._hDrvStoreModule = DrvStore.DriverStoreInterop.LoadLibrary(text);
			if (this._hDrvStoreModule == IntPtr.Zero)
			{
				int lastWin32Error = Marshal.GetLastWin32Error();
				LogUtil.Error("Loadlibrary failed on {0}", new object[]
				{
					text
				});
				throw new Win32Exception(lastWin32Error);
			}
			this._isInitialized = true;
		}

		// Token: 0x06000045 RID: 69 RVA: 0x00002A1C File Offset: 0x00000C1C
		public void Create()
		{
			if (!this._isInitialized)
			{
				this.Initialize();
			}
			LogUtil.Diagnostic("Creating driver store at {0}", new object[]
			{
				this._stagingSystemDirectory
			});
			LongPathDirectory.CreateDirectory(this._stagingSystemDirectory);
			if (this._hDrvStore != IntPtr.Zero)
			{
				LogUtil.Diagnostic("Attempting to open a driver store that was not closed {0} ", new object[]
				{
					this._hDrvStore.ToString()
				});
				this.Close();
			}
			this._hDrvStore = DrvStore.DriverStoreInterop.DriverStoreOpen(this._stagingSystemDirectory, this._targetBootDrive, DriverStoreOpenFlag.Create, IntPtr.Zero);
			if (this._hDrvStore == IntPtr.Zero)
			{
				int lastWin32Error = Marshal.GetLastWin32Error();
				LogUtil.Error("DriverStoreOpen failed error 0x{0:X8}", new object[]
				{
					lastWin32Error
				});
				throw new Win32Exception(lastWin32Error);
			}
			LogUtil.Diagnostic("DriverStoreOpen {0} ", new object[]
			{
				this._hDrvStore.ToString()
			});
		}

		// Token: 0x06000046 RID: 70 RVA: 0x00002B14 File Offset: 0x00000D14
		public void SetupConfigOptions(uint configOptions)
		{
			DevPropKey devPropKey = default(DevPropKey);
			devPropKey.fmtid = new Guid("8163eb00-142c-4f7a-94e1-a274cc47dbba");
			devPropKey.pid = 16U;
			LogUtil.Diagnostic("Setting DriverStore ConfigOptions to 0x{0:X}", new object[]
			{
				configOptions
			});
			if (!DrvStore.DriverStoreInterop.DriverStoreSetObjectProperty(this._hDrvStore, DriverStoreObjectType.DriverDatabase, "SYSTEM", ref devPropKey, DevPropType.DevPropTypeUint32, ref configOptions, 4, DriverStoreSetObjectPropertyFlag.None))
			{
				int lastWin32Error = Marshal.GetLastWin32Error();
				LogUtil.Error("DriverStoreSetObjectProperty failed error 0x{0:X8}", new object[]
				{
					lastWin32Error
				});
				throw new Win32Exception(lastWin32Error);
			}
		}

		// Token: 0x06000047 RID: 71 RVA: 0x00002B9C File Offset: 0x00000D9C
		private bool IncludeFileCallback(IntPtr driverPackageHandle, IntPtr pDriverFile, IntPtr lParam)
		{
			Marshal.WriteInt32(lParam, 1);
			return false;
		}

		// Token: 0x06000048 RID: 72 RVA: 0x00002BA8 File Offset: 0x00000DA8
		public bool DriverIncludesInfs(string infFile, CpuId cpuId)
		{
			int num = 0;
			IntPtr intPtr = Marshal.AllocHGlobal(4);
			IntPtr intPtr2 = DrvStore.DriverStoreInterop.DriverPackageOpen(infFile, this.GetProcessArchitectureFromCpuId(cpuId), null, DriverPackageOpenFlag.FilesOnly, IntPtr.Zero);
			if (intPtr2 == IntPtr.Zero)
			{
				LogUtil.Warning("Failed to determine if INF includes other INFs, assuming yes");
				return true;
			}
			Marshal.WriteInt32(intPtr, num);
			DrvStore.DriverStoreInterop.DriverPackageEnumFilesW(intPtr2, IntPtr.Zero, DriverPackageEnumFilesFlag.IncludeInfs, new DrvStore.DriverStoreInterop.EnumFilesDelegate(this.IncludeFileCallback), intPtr);
			num = Marshal.ReadInt32(intPtr);
			DrvStore.DriverStoreInterop.DriverPackageClose(intPtr2);
			Marshal.FreeHGlobal(intPtr);
			LogUtil.Diagnostic("INF includes other INFs: {0}", new object[]
			{
				num
			});
			return num != 0;
		}

		// Token: 0x06000049 RID: 73 RVA: 0x00002C44 File Offset: 0x00000E44
		private ProcessorArchitecture GetProcessArchitectureFromCpuId(CpuId cpuId)
		{
			ProcessorArchitecture result;
			switch (cpuId)
			{
			case CpuId.X86:
				result = ProcessorArchitecture.PROCESSOR_ARCHITECTURE_INTEL;
				break;
			case CpuId.ARM:
				result = ProcessorArchitecture.PROCESSOR_ARCHITECTURE_ARM;
				break;
			case CpuId.ARM64:
				result = ProcessorArchitecture.PROCESSOR_ARCHITECTURE_ARM64;
				break;
			case CpuId.AMD64:
				result = ProcessorArchitecture.PROCESSOR_ARCHITECTURE_AMD64;
				break;
			default:
				throw new PkgGenException("Unexpected CPU type '{0}'", new object[]
				{
					cpuId
				});
			}
			return result;
		}

		// Token: 0x0600004A RID: 74 RVA: 0x00002C98 File Offset: 0x00000E98
		public void ImportDriver(string infPath, string[] referencePaths, string[] stagingSubdirs, CpuId cpuId)
		{
			LogUtil.Diagnostic("Importing driver {0} into store {1}", new object[]
			{
				infPath,
				this._stagingRootDirectory
			});
			if (this._hDrvStore == IntPtr.Zero)
			{
				throw new InvalidOperationException("The driver store has not been created");
			}
			if (string.IsNullOrEmpty(infPath))
			{
				throw new ArgumentNullException("infPath");
			}
			string text = Path.Combine(this._stagingRootDirectory, "import");
			LongPathDirectory.CreateDirectory(text);
			string text2 = DrvStore.CopyToDirectory(infPath, text);
			if (referencePaths != null)
			{
				for (int i = 0; i < referencePaths.Length; i++)
				{
					string text3 = string.IsNullOrEmpty(stagingSubdirs[i]) ? text : Path.Combine(text, stagingSubdirs[i]);
					LongPathDirectory.CreateDirectory(text3);
					DrvStore.CopyToDirectory(referencePaths[i], text3);
				}
			}
			StringBuilder stringBuilder = new StringBuilder(260);
			ProcessorArchitecture processArchitectureFromCpuId = this.GetProcessArchitectureFromCpuId(cpuId);
			DriverStoreImportFlag driverStoreImportFlag = DriverStoreImportFlag.SkipTempCopy | DriverStoreImportFlag.SkipExternalFileCheck | DriverStoreImportFlag.Inbox | DriverStoreImportFlag.SystemDefaultLocale;
			IntPtr hDrvStore = this._hDrvStore;
			string driverPackageFileName = text2;
			ProcessorArchitecture processorArchitecture = processArchitectureFromCpuId;
			string localeName = null;
			DriverStoreImportFlag flags = driverStoreImportFlag;
			StringBuilder stringBuilder2 = stringBuilder;
			uint num = DrvStore.DriverStoreInterop.DriverStoreImport(hDrvStore, driverPackageFileName, processorArchitecture, localeName, flags, stringBuilder2, stringBuilder2.Capacity);
			if (num != 0U)
			{
				LogUtil.Error("DriverStoreImport failed error 0x{0:X8}", new object[]
				{
					num
				});
				throw new Win32Exception((int)num);
			}
			LogUtil.Diagnostic("Driverstore INF path: {0}", new object[]
			{
				stringBuilder
			});
			LogUtil.Diagnostic("Publishing driver");
			StringBuilder stringBuilder3 = new StringBuilder(260);
			bool flag = false;
			IntPtr hDrvStore2 = this._hDrvStore;
			string driverStoreFileName = stringBuilder.ToString();
			DriverStorePublishFlag flag2 = DriverStorePublishFlag.None;
			StringBuilder stringBuilder4 = stringBuilder3;
			num = DrvStore.DriverStoreInterop.DriverStorePublish(hDrvStore2, driverStoreFileName, flag2, stringBuilder4, stringBuilder4.Capacity, ref flag);
			if (num != 0U)
			{
				LogUtil.Error("DriverStorePublish failed error 0x{0:X8}", new object[]
				{
					num
				});
				throw new Win32Exception((int)num);
			}
			LogUtil.Diagnostic("Published INF path: {0}", new object[]
			{
				stringBuilder3
			});
			DriverStoreReflectCriticalFlag flag3 = DriverStoreReflectCriticalFlag.Force | DriverStoreReflectCriticalFlag.Configurations;
			num = DrvStore.DriverStoreInterop.DriverStoreReflectCritical(this._hDrvStore, stringBuilder.ToString(), flag3, null);
			if (num != 0U)
			{
				LogUtil.Error("DriverStoreReflectCritical failed error 0x{0:X8}", new object[]
				{
					num
				});
				throw new Win32Exception((int)num);
			}
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x0600004B RID: 75 RVA: 0x00002E6A File Offset: 0x0000106A
		public string HivePath
		{
			get
			{
				return Path.Combine(this._stagingSystemDirectory, "System32", "config");
			}
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x0600004C RID: 76 RVA: 0x00002E81 File Offset: 0x00001081
		public string ImportLogPath
		{
			get
			{
				return Path.Combine(this._stagingSystemDirectory, "INF", "setupapi.offline.log");
			}
		}

		// Token: 0x0600004D RID: 77 RVA: 0x00002E98 File Offset: 0x00001098
		private static string CopyToDirectory(string filePath, string destinationDirectory)
		{
			string text = Environment.ExpandEnvironmentVariables(filePath);
			LogUtil.Diagnostic("Copying {0} to {1}", new object[]
			{
				text,
				destinationDirectory
			});
			if (!LongPathFile.Exists(text))
			{
				throw new PkgGenException("Can't find required file: {0}", new object[]
				{
					text
				});
			}
			string text2 = Path.Combine(destinationDirectory, Path.GetFileName(text));
			LongPathFile.Copy(text, text2, true);
			LongPathFile.SetAttributes(text2, FileAttributes.Normal);
			return text2;
		}

		// Token: 0x0600004E RID: 78 RVA: 0x00002F02 File Offset: 0x00001102
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x0600004F RID: 79 RVA: 0x00002F14 File Offset: 0x00001114
		private void Dispose(bool disposing)
		{
			object obj = DrvStore.syncRoot;
			lock (obj)
			{
				this.Close();
				if (this._hDrvStoreModule != IntPtr.Zero)
				{
					bool flag2 = DrvStore.DriverStoreInterop.FreeLibrary(this._hDrvStoreModule);
					this._hDrvStoreModule = IntPtr.Zero;
					if (!flag2)
					{
						LogUtil.Warning("Unable to unload drvstore.dll");
					}
				}
			}
		}

		// Token: 0x06000050 RID: 80 RVA: 0x00002F88 File Offset: 0x00001188
		public void Close()
		{
			if (!(this._hDrvStore != IntPtr.Zero))
			{
				return;
			}
			LogUtil.Diagnostic("DriverStoreClose {0} ", new object[]
			{
				this._hDrvStore.ToString()
			});
			if (DrvStore.DriverStoreInterop.DriverStoreClose(this._hDrvStore))
			{
				this._hDrvStore = IntPtr.Zero;
				return;
			}
			throw new PkgGenException(string.Format("Unable to close driver store", new object[0]), new object[0]);
		}

		// Token: 0x040000B4 RID: 180
		private const int MAX_PATH = 260;

		// Token: 0x040000B5 RID: 181
		private static object syncRoot = new object();

		// Token: 0x040000B6 RID: 182
		private IntPtr _hDrvStoreModule = IntPtr.Zero;

		// Token: 0x040000B7 RID: 183
		private IntPtr _hDrvStore = IntPtr.Zero;

		// Token: 0x040000B8 RID: 184
		private bool _isInitialized;

		// Token: 0x040000B9 RID: 185
		private string _stagingRootDirectory;

		// Token: 0x040000BA RID: 186
		private string _stagingSystemDirectory;

		// Token: 0x040000BB RID: 187
		private string _targetBootDrive;

		// Token: 0x040000BC RID: 188
		private const string STR_DRVSTORE_DLL = "drvstore.dll";

		// Token: 0x040000BD RID: 189
		public const uint DriverDatabaseConfigOptionsOneCore = 51U;

		// Token: 0x02000081 RID: 129
		internal static class DriverStoreInterop
		{
			// Token: 0x060002D9 RID: 729
			[DllImport("kernel32", SetLastError = true)]
			internal static extern IntPtr LoadLibrary(string lpFileName);

			// Token: 0x060002DA RID: 730
			[DllImport("kernel32.dll", SetLastError = true)]
			internal static extern bool FreeLibrary(IntPtr hModule);

			// Token: 0x060002DB RID: 731
			[DllImport("drvstore.dll", CharSet = CharSet.Unicode, EntryPoint = "DriverStoreOpenW", SetLastError = true)]
			internal static extern IntPtr DriverStoreOpen(string targetSystemPath, string targetBootDrive, DriverStoreOpenFlag Flags, IntPtr transactionHandle);

			// Token: 0x060002DC RID: 732
			[DllImport("drvstore.dll", SetLastError = true)]
			internal static extern bool DriverStoreClose(IntPtr driverStoreHandle);

			// Token: 0x060002DD RID: 733
			[DllImport("drvstore.dll", CharSet = CharSet.Unicode, EntryPoint = "DriverStoreImportW", SetLastError = true)]
			internal static extern uint DriverStoreImport(IntPtr driverStoreHandle, string driverPackageFileName, ProcessorArchitecture ProcessorArchitecture, string localeName, DriverStoreImportFlag flags, StringBuilder driverStoreFileName, int driverStoreFileNameSize);

			// Token: 0x060002DE RID: 734
			[DllImport("drvstore.dll", CharSet = CharSet.Unicode, EntryPoint = "DriverStoreOfflineAddDriverPackageW", SetLastError = true)]
			internal static extern uint DriverStoreOfflineAddDriverPackage(string DriverPackageInfPath, DriverStoreOfflineAddDriverPackageFlags Flags, IntPtr Reserved, ushort ProcessorArchitecture, string LocaleName, StringBuilder DestInfPath, ref int cchDestInfPath, string TargetSystemRoot, string TargetSystemDrive);

			// Token: 0x060002DF RID: 735
			[DllImport("drvstore.dll", CharSet = CharSet.Unicode, EntryPoint = "DriverStoreConfigureW", SetLastError = true)]
			internal static extern uint DriverStoreConfigure(IntPtr hDriverStore, string DriverStoreFilename, DriverStoreConfigureFlags Flags, string SourceFilter, string TargetFilter);

			// Token: 0x060002E0 RID: 736
			[DllImport("drvstore.dll", CharSet = CharSet.Unicode, EntryPoint = "DriverStoreReflectCriticalW", SetLastError = true)]
			internal static extern uint DriverStoreReflectCritical(IntPtr driverStoreHandle, string driverStoreFileName, DriverStoreReflectCriticalFlag flag, string filterDeviceId);

			// Token: 0x060002E1 RID: 737
			[DllImport("drvstore.dll", CharSet = CharSet.Unicode, EntryPoint = "DriverStoreReflectW", SetLastError = true)]
			internal static extern uint DriverStoreReflect(IntPtr driverStoreHandle, string driverStoreFileName, DriverStoreReflectFlag flag, string filterSectionNames);

			// Token: 0x060002E2 RID: 738
			[DllImport("drvstore.dll", CharSet = CharSet.Unicode, EntryPoint = "DriverStorePublishW", SetLastError = true)]
			internal static extern uint DriverStorePublish(IntPtr driverStoreHandle, string driverStoreFileName, DriverStorePublishFlag flag, StringBuilder publishedFileName, int publishedFileNameSize, ref bool isPublishedFileNameChanged);

			// Token: 0x060002E3 RID: 739
			[DllImport("drvstore.dll", CharSet = CharSet.Unicode, EntryPoint = "DriverStoreSetObjectPropertyW", SetLastError = true)]
			internal static extern bool DriverStoreSetObjectProperty(IntPtr driverStoreHandle, DriverStoreObjectType objectType, string objectName, ref DevPropKey propertyKey, DevPropType propertyType, ref uint propertyBuffer, int propertySize, DriverStoreSetObjectPropertyFlag flag);

			// Token: 0x060002E4 RID: 740
			[DllImport("drvstore.dll", CharSet = CharSet.Unicode, SetLastError = true)]
			internal static extern bool DriverPackageEnumFilesW(IntPtr driverPackageHandle, IntPtr enumContext, DriverPackageEnumFilesFlag flags, DrvStore.DriverStoreInterop.EnumFilesDelegate callbackRoutine, IntPtr lParam);

			// Token: 0x060002E5 RID: 741
			[DllImport("drvstore.dll", CharSet = CharSet.Unicode, EntryPoint = "DriverPackageOpenW", SetLastError = true)]
			internal static extern IntPtr DriverPackageOpen(string driverPackageFilename, ProcessorArchitecture processorArchitecture, string localeName, DriverPackageOpenFlag flags, IntPtr resolveContext);

			// Token: 0x060002E6 RID: 742
			[DllImport("drvstore.dll", CharSet = CharSet.Unicode, SetLastError = true)]
			internal static extern void DriverPackageClose(IntPtr driverPackageHandle);

			// Token: 0x020000B0 RID: 176
			// (Invoke) Token: 0x06000386 RID: 902
			public delegate bool EnumFilesDelegate(IntPtr driverPackageHandle, IntPtr pDriverFile, IntPtr lParam);
		}
	}
}
