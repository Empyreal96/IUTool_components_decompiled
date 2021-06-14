using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.Windows.ImageTools
{
	// Token: 0x0200000F RID: 15
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
	[DebuggerNonUserCode]
	[CompilerGenerated]
	internal class Resources
	{
		// Token: 0x0600002A RID: 42 RVA: 0x00002B2B File Offset: 0x00000D2B
		internal Resources()
		{
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600002B RID: 43 RVA: 0x00003E3F File Offset: 0x0000203F
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (Resources.resourceMan == null)
				{
					Resources.resourceMan = new ResourceManager("Microsoft.Windows.ImageTools.Properties.Resources", typeof(Resources).Assembly);
				}
				return Resources.resourceMan;
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x0600002C RID: 44 RVA: 0x00003E6B File Offset: 0x0000206B
		// (set) Token: 0x0600002D RID: 45 RVA: 0x00003E72 File Offset: 0x00002072
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return Resources.resourceCulture;
			}
			set
			{
				Resources.resourceCulture = value;
			}
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600002E RID: 46 RVA: 0x00003E7A File Offset: 0x0000207A
		internal static string BOOTING_WIM
		{
			get
			{
				return Resources.ResourceManager.GetString("BOOTING_WIM", Resources.resourceCulture);
			}
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x0600002F RID: 47 RVA: 0x00003E90 File Offset: 0x00002090
		internal static string DEVICE_ID
		{
			get
			{
				return Resources.ResourceManager.GetString("DEVICE_ID", Resources.resourceCulture);
			}
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000030 RID: 48 RVA: 0x00003EA6 File Offset: 0x000020A6
		internal static string DEVICE_NO
		{
			get
			{
				return Resources.ResourceManager.GetString("DEVICE_NO", Resources.resourceCulture);
			}
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000031 RID: 49 RVA: 0x00003EBC File Offset: 0x000020BC
		internal static string DEVICE_TYPE
		{
			get
			{
				return Resources.ResourceManager.GetString("DEVICE_TYPE", Resources.resourceCulture);
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000032 RID: 50 RVA: 0x00003ED2 File Offset: 0x000020D2
		internal static string DEVICES_FOUND
		{
			get
			{
				return Resources.ResourceManager.GetString("DEVICES_FOUND", Resources.resourceCulture);
			}
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000033 RID: 51 RVA: 0x00003EE8 File Offset: 0x000020E8
		internal static string ERROR_AT_LEAST_ONE_DEVICE_FAILED
		{
			get
			{
				return Resources.ResourceManager.GetString("ERROR_AT_LEAST_ONE_DEVICE_FAILED", Resources.resourceCulture);
			}
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000034 RID: 52 RVA: 0x00003EFE File Offset: 0x000020FE
		internal static string ERROR_BOOT_WIM
		{
			get
			{
				return Resources.ResourceManager.GetString("ERROR_BOOT_WIM", Resources.resourceCulture);
			}
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000035 RID: 53 RVA: 0x00003F14 File Offset: 0x00002114
		internal static string ERROR_FFU
		{
			get
			{
				return Resources.ResourceManager.GetString("ERROR_FFU", Resources.resourceCulture);
			}
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000036 RID: 54 RVA: 0x00003F2A File Offset: 0x0000212A
		internal static string ERROR_FILE_NOT_FOUND
		{
			get
			{
				return Resources.ResourceManager.GetString("ERROR_FILE_NOT_FOUND", Resources.resourceCulture);
			}
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000037 RID: 55 RVA: 0x00003F40 File Offset: 0x00002140
		internal static string ERROR_MORE_THAN_ONE_DEVICE
		{
			get
			{
				return Resources.ResourceManager.GetString("ERROR_MORE_THAN_ONE_DEVICE", Resources.resourceCulture);
			}
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000038 RID: 56 RVA: 0x00003F56 File Offset: 0x00002156
		internal static string ERROR_NO_PLATFORM_ID
		{
			get
			{
				return Resources.ResourceManager.GetString("ERROR_NO_PLATFORM_ID", Resources.resourceCulture);
			}
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000039 RID: 57 RVA: 0x00003F6C File Offset: 0x0000216C
		internal static string ERROR_RESET_BOOT_MODE
		{
			get
			{
				return Resources.ResourceManager.GetString("ERROR_RESET_BOOT_MODE", Resources.resourceCulture);
			}
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x0600003A RID: 58 RVA: 0x00003F82 File Offset: 0x00002182
		internal static string ERROR_RESET_MASS_STORAGE_MODE
		{
			get
			{
				return Resources.ResourceManager.GetString("ERROR_RESET_MASS_STORAGE_MODE", Resources.resourceCulture);
			}
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x0600003B RID: 59 RVA: 0x00003F98 File Offset: 0x00002198
		internal static string ERROR_SKIP_TRANSFER
		{
			get
			{
				return Resources.ResourceManager.GetString("ERROR_SKIP_TRANSFER", Resources.resourceCulture);
			}
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x0600003C RID: 60 RVA: 0x00003FAE File Offset: 0x000021AE
		internal static string ERROR_TIMED_OUT
		{
			get
			{
				return Resources.ResourceManager.GetString("ERROR_TIMED_OUT", Resources.resourceCulture);
			}
		}

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x0600003D RID: 61 RVA: 0x00003FC4 File Offset: 0x000021C4
		internal static string ERROR_UNEXPECTED_DEVICESTATUS
		{
			get
			{
				return Resources.ResourceManager.GetString("ERROR_UNEXPECTED_DEVICESTATUS", Resources.resourceCulture);
			}
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x0600003E RID: 62 RVA: 0x00003FDA File Offset: 0x000021DA
		internal static string ERROR_WIM_BOOT
		{
			get
			{
				return Resources.ResourceManager.GetString("ERROR_WIM_BOOT", Resources.resourceCulture);
			}
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x0600003F RID: 63 RVA: 0x00003FF0 File Offset: 0x000021F0
		internal static string ERRORS
		{
			get
			{
				return Resources.ResourceManager.GetString("ERRORS", Resources.resourceCulture);
			}
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000040 RID: 64 RVA: 0x00004006 File Offset: 0x00002206
		internal static string FORCE_OPTION_DEPRECATED
		{
			get
			{
				return Resources.ResourceManager.GetString("FORCE_OPTION_DEPRECATED", Resources.resourceCulture);
			}
		}

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000041 RID: 65 RVA: 0x0000401C File Offset: 0x0000221C
		internal static string WIM_FLASH_OPTION_DEPRECATED
		{
			get
			{
				return Resources.ResourceManager.GetString("WIM_FLASH_OPTION_DEPRECATED", Resources.resourceCulture);
			}
		}

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x06000042 RID: 66 RVA: 0x00004032 File Offset: 0x00002232
		internal static string FORMAT_SPEED
		{
			get
			{
				return Resources.ResourceManager.GetString("FORMAT_SPEED", Resources.resourceCulture);
			}
		}

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x06000043 RID: 67 RVA: 0x00004048 File Offset: 0x00002248
		internal static string ID
		{
			get
			{
				return Resources.ResourceManager.GetString("ID", Resources.resourceCulture);
			}
		}

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x06000044 RID: 68 RVA: 0x0000405E File Offset: 0x0000225E
		internal static string LOGGING_SIMPLEIO_TO_ETL
		{
			get
			{
				return Resources.ResourceManager.GetString("LOGGING_SIMPLEIO_TO_ETL", Resources.resourceCulture);
			}
		}

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x06000045 RID: 69 RVA: 0x00004074 File Offset: 0x00002274
		internal static string LOGGING_UFP_TO_LOG
		{
			get
			{
				return Resources.ResourceManager.GetString("LOGGING_UFP_TO_LOG", Resources.resourceCulture);
			}
		}

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x06000046 RID: 70 RVA: 0x0000408A File Offset: 0x0000228A
		internal static string LOGS_PATH
		{
			get
			{
				return Resources.ResourceManager.GetString("LOGS_PATH", Resources.resourceCulture);
			}
		}

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x06000047 RID: 71 RVA: 0x000040A0 File Offset: 0x000022A0
		internal static string NAME
		{
			get
			{
				return Resources.ResourceManager.GetString("NAME", Resources.resourceCulture);
			}
		}

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000048 RID: 72 RVA: 0x000040B6 File Offset: 0x000022B6
		internal static string NO_CONNECTED_DEVICES
		{
			get
			{
				return Resources.ResourceManager.GetString("NO_CONNECTED_DEVICES", Resources.resourceCulture);
			}
		}

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x06000049 RID: 73 RVA: 0x000040CC File Offset: 0x000022CC
		internal static string REMOVE_PLATFORM_ID
		{
			get
			{
				return Resources.ResourceManager.GetString("REMOVE_PLATFORM_ID", Resources.resourceCulture);
			}
		}

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x0600004A RID: 74 RVA: 0x000040E2 File Offset: 0x000022E2
		internal static string RESET_BOOT_MODE
		{
			get
			{
				return Resources.ResourceManager.GetString("RESET_BOOT_MODE", Resources.resourceCulture);
			}
		}

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x0600004B RID: 75 RVA: 0x000040F8 File Offset: 0x000022F8
		internal static string RESET_MASS_STORAGE_MODE
		{
			get
			{
				return Resources.ResourceManager.GetString("RESET_MASS_STORAGE_MODE", Resources.resourceCulture);
			}
		}

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x0600004C RID: 76 RVA: 0x0000410E File Offset: 0x0000230E
		internal static string SERIAL_NO
		{
			get
			{
				return Resources.ResourceManager.GetString("SERIAL_NO", Resources.resourceCulture);
			}
		}

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x0600004D RID: 77 RVA: 0x00004124 File Offset: 0x00002324
		internal static string SERIAL_NO_FORMAT
		{
			get
			{
				return Resources.ResourceManager.GetString("SERIAL_NO_FORMAT", Resources.resourceCulture);
			}
		}

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x0600004E RID: 78 RVA: 0x0000413A File Offset: 0x0000233A
		internal static string STATUS_BOOTING_TO_WIM
		{
			get
			{
				return Resources.ResourceManager.GetString("STATUS_BOOTING_TO_WIM", Resources.resourceCulture);
			}
		}

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x0600004F RID: 79 RVA: 0x00004150 File Offset: 0x00002350
		internal static string STATUS_CONNECTED
		{
			get
			{
				return Resources.ResourceManager.GetString("STATUS_CONNECTED", Resources.resourceCulture);
			}
		}

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x06000050 RID: 80 RVA: 0x00004166 File Offset: 0x00002366
		internal static string STATUS_DONE
		{
			get
			{
				return Resources.ResourceManager.GetString("STATUS_DONE", Resources.resourceCulture);
			}
		}

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x06000051 RID: 81 RVA: 0x0000417C File Offset: 0x0000237C
		internal static string STATUS_ERROR
		{
			get
			{
				return Resources.ResourceManager.GetString("STATUS_ERROR", Resources.resourceCulture);
			}
		}

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x06000052 RID: 82 RVA: 0x00004192 File Offset: 0x00002392
		internal static string STATUS_FLASHING
		{
			get
			{
				return Resources.ResourceManager.GetString("STATUS_FLASHING", Resources.resourceCulture);
			}
		}

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x06000053 RID: 83 RVA: 0x000041A8 File Offset: 0x000023A8
		internal static string STATUS_LOGS
		{
			get
			{
				return Resources.ResourceManager.GetString("STATUS_LOGS", Resources.resourceCulture);
			}
		}

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x06000054 RID: 84 RVA: 0x000041BE File Offset: 0x000023BE
		internal static string STATUS_SKIPPED
		{
			get
			{
				return Resources.ResourceManager.GetString("STATUS_SKIPPED", Resources.resourceCulture);
			}
		}

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x06000055 RID: 85 RVA: 0x000041D4 File Offset: 0x000023D4
		internal static string STATUS_SKIPPING
		{
			get
			{
				return Resources.ResourceManager.GetString("STATUS_SKIPPING", Resources.resourceCulture);
			}
		}

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x06000056 RID: 86 RVA: 0x000041EA File Offset: 0x000023EA
		internal static string STATUS_TRANSFER_WIM
		{
			get
			{
				return Resources.ResourceManager.GetString("STATUS_TRANSFER_WIM", Resources.resourceCulture);
			}
		}

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x06000057 RID: 87 RVA: 0x00004200 File Offset: 0x00002400
		internal static string TRANSFER_STATISTICS
		{
			get
			{
				return Resources.ResourceManager.GetString("TRANSFER_STATISTICS", Resources.resourceCulture);
			}
		}

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x06000058 RID: 88 RVA: 0x00004216 File Offset: 0x00002416
		internal static string USAGE
		{
			get
			{
				return Resources.ResourceManager.GetString("USAGE", Resources.resourceCulture);
			}
		}

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x06000059 RID: 89 RVA: 0x0000422C File Offset: 0x0000242C
		internal static string WIM_TRANSFER_RATE
		{
			get
			{
				return Resources.ResourceManager.GetString("WIM_TRANSFER_RATE", Resources.resourceCulture);
			}
		}

		// Token: 0x0400004A RID: 74
		private static ResourceManager resourceMan;

		// Token: 0x0400004B RID: 75
		private static CultureInfo resourceCulture;
	}
}
