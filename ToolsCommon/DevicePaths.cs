using System;
using System.IO;

namespace Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
	// Token: 0x02000057 RID: 87
	public class DevicePaths
	{
		// Token: 0x17000047 RID: 71
		// (get) Token: 0x06000278 RID: 632 RVA: 0x0000B9EF File Offset: 0x00009BEF
		public static string ImageUpdatePath
		{
			get
			{
				return DevicePaths._imageUpdatePath;
			}
		}

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x06000279 RID: 633 RVA: 0x0000B9F6 File Offset: 0x00009BF6
		public static string DeviceLayoutFileName
		{
			get
			{
				return DevicePaths._deviceLayoutFileName;
			}
		}

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x0600027A RID: 634 RVA: 0x0000B9FD File Offset: 0x00009BFD
		public static string DeviceLayoutFilePath
		{
			get
			{
				return Path.Combine(DevicePaths.ImageUpdatePath, DevicePaths.DeviceLayoutFileName);
			}
		}

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x0600027B RID: 635 RVA: 0x0000BA0E File Offset: 0x00009C0E
		public static string OemDevicePlatformFileName
		{
			get
			{
				return DevicePaths._oemDevicePlatformFileName;
			}
		}

		// Token: 0x1700004B RID: 75
		// (get) Token: 0x0600027C RID: 636 RVA: 0x0000BA15 File Offset: 0x00009C15
		public static string OemDevicePlatformFilePath
		{
			get
			{
				return Path.Combine(DevicePaths.ImageUpdatePath, DevicePaths.OemDevicePlatformFileName);
			}
		}

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x0600027D RID: 637 RVA: 0x0000BA26 File Offset: 0x00009C26
		public static string UpdateOutputFile
		{
			get
			{
				return DevicePaths._updateOutputFile;
			}
		}

		// Token: 0x1700004D RID: 77
		// (get) Token: 0x0600027E RID: 638 RVA: 0x0000BA2D File Offset: 0x00009C2D
		public static string UpdateOutputFilePath
		{
			get
			{
				return Path.Combine(DevicePaths._updateFilesPath, DevicePaths._updateOutputFile);
			}
		}

		// Token: 0x1700004E RID: 78
		// (get) Token: 0x0600027F RID: 639 RVA: 0x0000BA3E File Offset: 0x00009C3E
		public static string UpdateHistoryFile
		{
			get
			{
				return DevicePaths._updateHistoryFile;
			}
		}

		// Token: 0x1700004F RID: 79
		// (get) Token: 0x06000280 RID: 640 RVA: 0x0000BA45 File Offset: 0x00009C45
		public static string UpdateHistoryFilePath
		{
			get
			{
				return Path.Combine(DevicePaths._imageUpdatePath, DevicePaths._updateHistoryFile);
			}
		}

		// Token: 0x17000050 RID: 80
		// (get) Token: 0x06000281 RID: 641 RVA: 0x0000BA56 File Offset: 0x00009C56
		public static string UpdateOSWIMName
		{
			get
			{
				return DevicePaths._updateOSWIMName;
			}
		}

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x06000282 RID: 642 RVA: 0x0000BA5D File Offset: 0x00009C5D
		public static string UpdateOSWIMFilePath
		{
			get
			{
				return Path.Combine(DevicePaths._UpdateOSPath, DevicePaths.UpdateOSWIMName);
			}
		}

		// Token: 0x17000052 RID: 82
		// (get) Token: 0x06000283 RID: 643 RVA: 0x0000BA6E File Offset: 0x00009C6E
		public static string MMOSWIMName
		{
			get
			{
				return DevicePaths._mmosWIMName;
			}
		}

		// Token: 0x17000053 RID: 83
		// (get) Token: 0x06000284 RID: 644 RVA: 0x0000BA75 File Offset: 0x00009C75
		public static string MMOSWIMFilePath
		{
			get
			{
				return DevicePaths.MMOSWIMName;
			}
		}

		// Token: 0x17000054 RID: 84
		// (get) Token: 0x06000285 RID: 645 RVA: 0x0000BA7C File Offset: 0x00009C7C
		public static string RegistryHivePath
		{
			get
			{
				return DevicePaths._registryHivePath;
			}
		}

		// Token: 0x06000286 RID: 646 RVA: 0x0000BA83 File Offset: 0x00009C83
		public static string GetBCDHivePath(bool isUefiBoot)
		{
			if (!isUefiBoot)
			{
				return DevicePaths._BiosBCDHivePath;
			}
			return DevicePaths._UefiBCDHivePath;
		}

		// Token: 0x06000287 RID: 647 RVA: 0x0000BA93 File Offset: 0x00009C93
		public static string GetRegistryHiveFilePath(SystemRegistryHiveFiles hiveType)
		{
			return DevicePaths.GetRegistryHiveFilePath(hiveType, true);
		}

		// Token: 0x06000288 RID: 648 RVA: 0x0000BA9C File Offset: 0x00009C9C
		public static string GetRegistryHiveFilePath(SystemRegistryHiveFiles hiveType, bool isUefiBoot)
		{
			string result = "";
			switch (hiveType)
			{
			case SystemRegistryHiveFiles.SYSTEM:
				result = Path.Combine(DevicePaths.RegistryHivePath, "SYSTEM");
				break;
			case SystemRegistryHiveFiles.SOFTWARE:
				result = Path.Combine(DevicePaths.RegistryHivePath, "SOFTWARE");
				break;
			case SystemRegistryHiveFiles.DEFAULT:
				result = Path.Combine(DevicePaths.RegistryHivePath, "DEFAULT");
				break;
			case SystemRegistryHiveFiles.DRIVERS:
				result = Path.Combine(DevicePaths.RegistryHivePath, "DRIVERS");
				break;
			case SystemRegistryHiveFiles.SAM:
				result = Path.Combine(DevicePaths.RegistryHivePath, "SAM");
				break;
			case SystemRegistryHiveFiles.SECURITY:
				result = Path.Combine(DevicePaths.RegistryHivePath, "SECURITY");
				break;
			case SystemRegistryHiveFiles.BCD:
				result = Path.Combine(DevicePaths.GetBCDHivePath(isUefiBoot), "BCD");
				break;
			}
			return result;
		}

		// Token: 0x17000055 RID: 85
		// (get) Token: 0x06000289 RID: 649 RVA: 0x0000BB51 File Offset: 0x00009D51
		public static string DeviceLayoutSchema
		{
			get
			{
				return "DeviceLayout.xsd";
			}
		}

		// Token: 0x17000056 RID: 86
		// (get) Token: 0x0600028A RID: 650 RVA: 0x0000BB58 File Offset: 0x00009D58
		public static string DeviceLayoutSchema2
		{
			get
			{
				return "DeviceLayoutv2.xsd";
			}
		}

		// Token: 0x17000057 RID: 87
		// (get) Token: 0x0600028B RID: 651 RVA: 0x0000BB5F File Offset: 0x00009D5F
		public static string UpdateOSInputSchema
		{
			get
			{
				return "UpdateOSInput.xsd";
			}
		}

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x0600028C RID: 652 RVA: 0x0000BB66 File Offset: 0x00009D66
		public static string OEMInputSchema
		{
			get
			{
				return "OEMInput.xsd";
			}
		}

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x0600028D RID: 653 RVA: 0x0000BB6D File Offset: 0x00009D6D
		public static string FeatureManifestSchema
		{
			get
			{
				return "FeatureManifest.xsd";
			}
		}

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x0600028E RID: 654 RVA: 0x0000BB74 File Offset: 0x00009D74
		public static string MicrosoftPhoneSKUSchema
		{
			get
			{
				return "MicrosoftPhoneSKU.xsd";
			}
		}

		// Token: 0x1700005B RID: 91
		// (get) Token: 0x0600028F RID: 655 RVA: 0x0000BB7B File Offset: 0x00009D7B
		public static string UpdateOSOutputSchema
		{
			get
			{
				return "UpdateOSOutput.xsd";
			}
		}

		// Token: 0x1700005C RID: 92
		// (get) Token: 0x06000290 RID: 656 RVA: 0x0000BB82 File Offset: 0x00009D82
		public static string UpdateHistorySchema
		{
			get
			{
				return "UpdateHistory.xsd";
			}
		}

		// Token: 0x1700005D RID: 93
		// (get) Token: 0x06000291 RID: 657 RVA: 0x0000BB89 File Offset: 0x00009D89
		public static string OEMDevicePlatformSchema
		{
			get
			{
				return "OEMDevicePlatform.xsd";
			}
		}

		// Token: 0x1700005E RID: 94
		// (get) Token: 0x06000292 RID: 658 RVA: 0x0000BB90 File Offset: 0x00009D90
		public static string MSFMPath
		{
			get
			{
				return Path.Combine(DevicePaths.ImageUpdatePath, DevicePaths._FMFilesDirectory, "Microsoft");
			}
		}

		// Token: 0x1700005F RID: 95
		// (get) Token: 0x06000293 RID: 659 RVA: 0x0000BBA6 File Offset: 0x00009DA6
		public static string MSFMPathOld
		{
			get
			{
				return DevicePaths.ImageUpdatePath;
			}
		}

		// Token: 0x17000060 RID: 96
		// (get) Token: 0x06000294 RID: 660 RVA: 0x0000BBAD File Offset: 0x00009DAD
		public static string OEMFMPath
		{
			get
			{
				return Path.Combine(DevicePaths.ImageUpdatePath, DevicePaths._FMFilesDirectory, "OEM");
			}
		}

		// Token: 0x17000061 RID: 97
		// (get) Token: 0x06000295 RID: 661 RVA: 0x0000BBC3 File Offset: 0x00009DC3
		public static string OEMInputPath
		{
			get
			{
				return Path.Combine(DevicePaths.ImageUpdatePath, DevicePaths._OEMInputPath);
			}
		}

		// Token: 0x17000062 RID: 98
		// (get) Token: 0x06000296 RID: 662 RVA: 0x0000BBD4 File Offset: 0x00009DD4
		public static string OEMInputFile
		{
			get
			{
				return Path.Combine(DevicePaths.OEMInputPath, DevicePaths._OEMInputFile);
			}
		}

		// Token: 0x0400011E RID: 286
		private static string _imageUpdatePath = "Windows\\ImageUpdate";

		// Token: 0x0400011F RID: 287
		private static string _updateFilesPath = "SharedData\\DuShared";

		// Token: 0x04000120 RID: 288
		private static string _registryHivePath = "Windows\\System32\\Config";

		// Token: 0x04000121 RID: 289
		private static string _BiosBCDHivePath = "boot";

		// Token: 0x04000122 RID: 290
		private static string _UefiBCDHivePath = "efi\\Microsoft\\boot";

		// Token: 0x04000123 RID: 291
		private static string _dsmPath = DevicePaths._imageUpdatePath;

		// Token: 0x04000124 RID: 292
		private static string _UpdateOSPath = "PROGRAMS\\UpdateOS\\";

		// Token: 0x04000125 RID: 293
		private static string _FMFilesDirectory = "FeatureManifest";

		// Token: 0x04000126 RID: 294
		private static string _OEMInputPath = "OEMInput";

		// Token: 0x04000127 RID: 295
		private static string _OEMInputFile = "OEMInput.xml";

		// Token: 0x04000128 RID: 296
		private static string _deviceLayoutFileName = "DeviceLayout.xml";

		// Token: 0x04000129 RID: 297
		private static string _oemDevicePlatformFileName = "OEMDevicePlatform.xml";

		// Token: 0x0400012A RID: 298
		private static string _updateOutputFile = "UpdateOutput.xml";

		// Token: 0x0400012B RID: 299
		private static string _updateHistoryFile = "UpdateHistory.xml";

		// Token: 0x0400012C RID: 300
		private static string _updateOSWIMName = "UpdateOS.wim";

		// Token: 0x0400012D RID: 301
		private static string _mmosWIMName = "MMOS.wim";

		// Token: 0x0400012E RID: 302
		public const string MAINOS_PARTITION_NAME = "MainOS";

		// Token: 0x0400012F RID: 303
		public const string MMOS_PARTITION_NAME = "MMOS";
	}
}
