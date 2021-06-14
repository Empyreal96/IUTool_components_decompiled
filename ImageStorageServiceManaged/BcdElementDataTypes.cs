using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000003 RID: 3
	public static class BcdElementDataTypes
	{
		// Token: 0x06000004 RID: 4 RVA: 0x000025C0 File Offset: 0x000007C0
		public static BcdElementDataType GetWellKnownDataType(string dataTypeName)
		{
			BcdElementDataType bcdElementDataType = null;
			foreach (BcdElementDataType bcdElementDataType2 in BcdElementDataTypes.LibraryTypes.Keys)
			{
				if (string.Compare(BcdElementDataTypes.LibraryTypes[bcdElementDataType2], dataTypeName, true, CultureInfo.InvariantCulture) == 0)
				{
					bcdElementDataType = bcdElementDataType2;
					break;
				}
			}
			if (bcdElementDataType == null)
			{
				foreach (BcdElementDataType bcdElementDataType3 in BcdElementDataTypes.ApplicationTypes.Keys)
				{
					if (string.Compare(BcdElementDataTypes.ApplicationTypes[bcdElementDataType3], dataTypeName, true, CultureInfo.InvariantCulture) == 0)
					{
						bcdElementDataType = bcdElementDataType3;
						break;
					}
				}
			}
			if (bcdElementDataType == null)
			{
				foreach (BcdElementDataType bcdElementDataType4 in BcdElementDataTypes.DeviceTypes.Keys)
				{
					if (string.Compare(BcdElementDataTypes.DeviceTypes[bcdElementDataType4], dataTypeName, true, CultureInfo.InvariantCulture) == 0)
					{
						bcdElementDataType = bcdElementDataType4;
						break;
					}
				}
			}
			if (bcdElementDataType == null)
			{
				foreach (BcdElementDataType bcdElementDataType5 in BcdElementDataTypes.OEMTypes.Keys)
				{
					if (string.Compare(BcdElementDataTypes.OEMTypes[bcdElementDataType5], dataTypeName, true, CultureInfo.InvariantCulture) == 0)
					{
						bcdElementDataType = bcdElementDataType5;
						break;
					}
				}
			}
			return bcdElementDataType;
		}

		// Token: 0x04000023 RID: 35
		public static readonly BcdElementDataType DefaultObject = new BcdElementDataType(ElementClass.Application, ElementFormat.Object, 3U);

		// Token: 0x04000024 RID: 36
		public static readonly BcdElementDataType ResumeObject = new BcdElementDataType(ElementClass.Application, ElementFormat.Object, 6U);

		// Token: 0x04000025 RID: 37
		public static readonly BcdElementDataType DisplayOrder = new BcdElementDataType(ElementClass.Application, ElementFormat.ObjectList, 1U);

		// Token: 0x04000026 RID: 38
		public static readonly BcdElementDataType BootSequence = new BcdElementDataType(ElementClass.Application, ElementFormat.ObjectList, 2U);

		// Token: 0x04000027 RID: 39
		public static readonly BcdElementDataType ToolsDisplayOrder = new BcdElementDataType(ElementClass.Application, ElementFormat.ObjectList, 16U);

		// Token: 0x04000028 RID: 40
		public static readonly BcdElementDataType TimeoutValue = new BcdElementDataType(ElementClass.Application, ElementFormat.Integer, 4U);

		// Token: 0x04000029 RID: 41
		public static readonly BcdElementDataType NXPolicy = new BcdElementDataType(ElementClass.Application, ElementFormat.Integer, 32U);

		// Token: 0x0400002A RID: 42
		public static readonly BcdElementDataType PAEPolicy = new BcdElementDataType(ElementClass.Application, ElementFormat.Integer, 33U);

		// Token: 0x0400002B RID: 43
		public static readonly BcdElementDataType DebuggingEnabled = new BcdElementDataType(ElementClass.Application, ElementFormat.Boolean, 6U);

		// Token: 0x0400002C RID: 44
		public static readonly BcdElementDataType DisplayBootMenu = new BcdElementDataType(ElementClass.Application, ElementFormat.Boolean, 32U);

		// Token: 0x0400002D RID: 45
		public static readonly BcdElementDataType WinPEImage = new BcdElementDataType(ElementClass.Application, ElementFormat.Boolean, 34U);

		// Token: 0x0400002E RID: 46
		public static readonly BcdElementDataType RemoveMemory = new BcdElementDataType(ElementClass.Application, ElementFormat.Integer, 49U);

		// Token: 0x0400002F RID: 47
		public static readonly BcdElementDataType KernelDebuggerEnabled = new BcdElementDataType(ElementClass.Application, ElementFormat.Boolean, 160U);

		// Token: 0x04000030 RID: 48
		public static readonly BcdElementDataType KernelEmsEnabled = new BcdElementDataType(ElementClass.Application, ElementFormat.Boolean, 176U);

		// Token: 0x04000031 RID: 49
		public static readonly BcdElementDataType SystemRoot = new BcdElementDataType(ElementClass.Application, ElementFormat.String, 2U);

		// Token: 0x04000032 RID: 50
		public static readonly BcdElementDataType FilePath = new BcdElementDataType(ElementClass.Application, ElementFormat.String, 35U);

		// Token: 0x04000033 RID: 51
		public static readonly BcdElementDataType OsLoaderType = new BcdElementDataType(ElementClass.Application, ElementFormat.Device, 1U);

		// Token: 0x04000034 RID: 52
		public static readonly BcdElementDataType BcdDevice = new BcdElementDataType(ElementClass.Application, ElementFormat.Device, 34U);

		// Token: 0x04000035 RID: 53
		public static readonly BcdElementDataType BootMenuPolicy = new BcdElementDataType(ElementClass.Application, ElementFormat.Integer, 194U);

		// Token: 0x04000036 RID: 54
		public static readonly BcdElementDataType DetectKernelAndHal = new BcdElementDataType(ElementClass.Application, ElementFormat.Boolean, 16U);

		// Token: 0x04000037 RID: 55
		public static readonly BcdElementDataType PersistBootSequence = new BcdElementDataType(ElementClass.Application, ElementFormat.Boolean, 49U);

		// Token: 0x04000038 RID: 56
		public static readonly BcdElementDataType FfuUpdateMode = new BcdElementDataType(ElementClass.Application, ElementFormat.Boolean, 513U);

		// Token: 0x04000039 RID: 57
		public static readonly BcdElementDataType ForceFfu = new BcdElementDataType(ElementClass.Application, ElementFormat.Boolean, 515U);

		// Token: 0x0400003A RID: 58
		public static readonly BcdElementDataType CheckPlatformId = new BcdElementDataType(ElementClass.Application, ElementFormat.Boolean, 516U);

		// Token: 0x0400003B RID: 59
		public static readonly BcdElementDataType DisableCheckPlatformId = new BcdElementDataType(ElementClass.Application, ElementFormat.Boolean, 517U);

		// Token: 0x0400003C RID: 60
		public static readonly BcdElementDataType EnableUFPMode = new BcdElementDataType(ElementClass.Application, ElementFormat.Boolean, 519U);

		// Token: 0x0400003D RID: 61
		public static readonly BcdElementDataType UFPMincryplHashingSupport = new BcdElementDataType(ElementClass.Application, ElementFormat.Boolean, 520U);

		// Token: 0x0400003E RID: 62
		public static readonly BcdElementDataType UFPLogLocation = new BcdElementDataType(ElementClass.Application, ElementFormat.Integer, 521U);

		// Token: 0x0400003F RID: 63
		public static readonly BcdElementDataType CustomActionsList = new BcdElementDataType(ElementClass.Application, ElementFormat.IntegerList, 48U);

		// Token: 0x04000040 RID: 64
		public static readonly BcdElementDataType DebugTransportPath = new BcdElementDataType(ElementClass.Application, ElementFormat.String, 19U);

		// Token: 0x04000041 RID: 65
		public static readonly BcdElementDataType HypervisorDebuggerType = new BcdElementDataType(ElementClass.Application, ElementFormat.Integer, 243U);

		// Token: 0x04000042 RID: 66
		public static readonly BcdElementDataType HypervisorDebuggerPortNumber = new BcdElementDataType(ElementClass.Application, ElementFormat.Integer, 244U);

		// Token: 0x04000043 RID: 67
		public static readonly BcdElementDataType HypervisorDebuggerBaudRate = new BcdElementDataType(ElementClass.Application, ElementFormat.Integer, 245U);

		// Token: 0x04000044 RID: 68
		public static readonly BcdElementDataType MemoryCaptureModeAddress = new BcdElementDataType(ElementClass.Application, ElementFormat.Integer, 1280U);

		// Token: 0x04000045 RID: 69
		public static readonly BcdElementDataType WpDmpSettingsFile = new BcdElementDataType(ElementClass.Application, ElementFormat.String, 1281U);

		// Token: 0x04000046 RID: 70
		public static readonly BcdElementDataType WpDmpLogFile = new BcdElementDataType(ElementClass.Application, ElementFormat.String, 1282U);

		// Token: 0x04000047 RID: 71
		public static readonly BcdElementDataType OemChargingBootThreshold = new BcdElementDataType(ElementClass.Application, ElementFormat.Integer, 1296U);

		// Token: 0x04000048 RID: 72
		public static readonly BcdElementDataType OemChargingModeThreshold = new BcdElementDataType(ElementClass.Application, ElementFormat.Integer, 1297U);

		// Token: 0x04000049 RID: 73
		public static readonly BcdElementDataType OemChargingModeEnabled = new BcdElementDataType(ElementClass.Application, ElementFormat.Boolean, 1298U);

		// Token: 0x0400004A RID: 74
		public static readonly BcdElementDataType BootFlowVariableGlobal = new BcdElementDataType(ElementClass.Application, ElementFormat.Integer, 2730U);

		// Token: 0x0400004B RID: 75
		public static readonly BcdElementDataType ProcessCustomActionsFirst = new BcdElementDataType(ElementClass.Application, ElementFormat.Boolean, 40U);

		// Token: 0x0400004C RID: 76
		public static readonly BcdElementDataType BootStatusPolicy = new BcdElementDataType(ElementClass.Application, ElementFormat.Integer, 224U);

		// Token: 0x0400004D RID: 77
		public static readonly BcdElementDataType AllowUserToResetPhone = new BcdElementDataType(ElementClass.Application, ElementFormat.Boolean, 518U);

		// Token: 0x0400004E RID: 78
		public static readonly BcdElementDataType ManufacturingMode = new BcdElementDataType(ElementClass.Application, ElementFormat.String, 320U);

		// Token: 0x0400004F RID: 79
		public static readonly BcdElementDataType MSEBootDebugPolicy = new BcdElementDataType(ElementClass.Application, ElementFormat.Boolean, 325U);

		// Token: 0x04000050 RID: 80
		public static readonly BcdElementDataType MSEBootOrderClean = new BcdElementDataType(ElementClass.Application, ElementFormat.Boolean, 326U);

		// Token: 0x04000051 RID: 81
		public static readonly BcdElementDataType MSEDeviceId = new BcdElementDataType(ElementClass.Application, ElementFormat.Boolean, 327U);

		// Token: 0x04000052 RID: 82
		public static readonly BcdElementDataType MSEFfuLoader = new BcdElementDataType(ElementClass.Application, ElementFormat.Boolean, 328U);

		// Token: 0x04000053 RID: 83
		public static readonly BcdElementDataType MSEIuLoader = new BcdElementDataType(ElementClass.Application, ElementFormat.Boolean, 329U);

		// Token: 0x04000054 RID: 84
		public static readonly BcdElementDataType MSEMassStorage = new BcdElementDataType(ElementClass.Application, ElementFormat.Boolean, 330U);

		// Token: 0x04000055 RID: 85
		public static readonly BcdElementDataType MSERpmbProvisioning = new BcdElementDataType(ElementClass.Application, ElementFormat.Boolean, 331U);

		// Token: 0x04000056 RID: 86
		public static readonly BcdElementDataType MSESecureBootPolicy = new BcdElementDataType(ElementClass.Application, ElementFormat.Boolean, 332U);

		// Token: 0x04000057 RID: 87
		public static readonly BcdElementDataType MSEStartCharge = new BcdElementDataType(ElementClass.Application, ElementFormat.Boolean, 333U);

		// Token: 0x04000058 RID: 88
		public static readonly BcdElementDataType MSEResetTPM = new BcdElementDataType(ElementClass.Application, ElementFormat.Boolean, 334U);

		// Token: 0x04000059 RID: 89
		public static readonly BcdElementDataType QuietBootEnable = new BcdElementDataType(ElementClass.Application, ElementFormat.Boolean, 65U);

		// Token: 0x0400005A RID: 90
		public static readonly Dictionary<BcdElementDataType, string> ApplicationTypes = new Dictionary<BcdElementDataType, string>
		{
			{
				BcdElementDataTypes.DefaultObject,
				"Default Object"
			},
			{
				BcdElementDataTypes.ResumeObject,
				"Resume Object"
			},
			{
				BcdElementDataTypes.DisplayOrder,
				"Display Order"
			},
			{
				BcdElementDataTypes.BootSequence,
				"Boot Sequence"
			},
			{
				BcdElementDataTypes.ToolsDisplayOrder,
				"Tools Display Order"
			},
			{
				BcdElementDataTypes.TimeoutValue,
				"Timeout Value"
			},
			{
				BcdElementDataTypes.NXPolicy,
				"NX Policy"
			},
			{
				BcdElementDataTypes.PAEPolicy,
				"PAE Policy"
			},
			{
				BcdElementDataTypes.DebuggingEnabled,
				"Debugging Enabled"
			},
			{
				BcdElementDataTypes.DisplayBootMenu,
				"Display Boot Menu"
			},
			{
				BcdElementDataTypes.WinPEImage,
				"WinPE Image"
			},
			{
				BcdElementDataTypes.RemoveMemory,
				"Remove Memory"
			},
			{
				BcdElementDataTypes.KernelDebuggerEnabled,
				"Kernel Debugger Enabled"
			},
			{
				BcdElementDataTypes.KernelEmsEnabled,
				"Kernel EMS Enabled"
			},
			{
				BcdElementDataTypes.SystemRoot,
				"System Root"
			},
			{
				BcdElementDataTypes.FilePath,
				"File Path"
			},
			{
				BcdElementDataTypes.OsLoaderType,
				"OS Loader Type"
			},
			{
				BcdElementDataTypes.BcdDevice,
				"BCD Device"
			},
			{
				BcdElementDataTypes.BootMenuPolicy,
				"Boot Menu Policy"
			},
			{
				BcdElementDataTypes.DetectKernelAndHal,
				"Detect Kernel and HAL"
			},
			{
				BcdElementDataTypes.PersistBootSequence,
				"Persist Boot Sequence"
			},
			{
				BcdElementDataTypes.FfuUpdateMode,
				"FFU Update Mode"
			},
			{
				BcdElementDataTypes.ForceFfu,
				"Force FFU"
			},
			{
				BcdElementDataTypes.CheckPlatformId,
				"Enable Platform ID Check"
			},
			{
				BcdElementDataTypes.DisableCheckPlatformId,
				"Disable Platform ID Check"
			},
			{
				BcdElementDataTypes.EnableUFPMode,
				"Enable UFP Mode"
			},
			{
				BcdElementDataTypes.UFPMincryplHashingSupport,
				"Use Mincrypl for Hashing"
			},
			{
				BcdElementDataTypes.UFPLogLocation,
				"UFP Log Location"
			},
			{
				BcdElementDataTypes.CustomActionsList,
				"Custom Action List"
			},
			{
				BcdElementDataTypes.DebugTransportPath,
				"Debug Transport Path"
			},
			{
				BcdElementDataTypes.HypervisorDebuggerType,
				"Hypervisor Debugger Type"
			},
			{
				BcdElementDataTypes.HypervisorDebuggerPortNumber,
				"Hypervisor Debugger Port Number"
			},
			{
				BcdElementDataTypes.HypervisorDebuggerBaudRate,
				"Hypervisor Debugger Baud Rate"
			},
			{
				BcdElementDataTypes.MemoryCaptureModeAddress,
				"Memory Capture Mode Address"
			},
			{
				BcdElementDataTypes.WpDmpSettingsFile,
				"WpDmp Settings File"
			},
			{
				BcdElementDataTypes.WpDmpLogFile,
				"WpDmp Log File"
			},
			{
				BcdElementDataTypes.OemChargingBootThreshold,
				"OEM Charging Boot Threshold"
			},
			{
				BcdElementDataTypes.OemChargingModeThreshold,
				"OEM Charging Mode Threshold"
			},
			{
				BcdElementDataTypes.OemChargingModeEnabled,
				"OEM Charging Mode Enabled"
			},
			{
				BcdElementDataTypes.BootFlowVariableGlobal,
				"BootFlowAPI Global"
			},
			{
				BcdElementDataTypes.ProcessCustomActionsFirst,
				"Process Custom Actions First"
			},
			{
				BcdElementDataTypes.BootStatusPolicy,
				"Boot Status Policy"
			},
			{
				BcdElementDataTypes.AllowUserToResetPhone,
				"Allow User To Reset Phone"
			},
			{
				BcdElementDataTypes.ManufacturingMode,
				"Manufacturing Mode"
			},
			{
				BcdElementDataTypes.MSEBootDebugPolicy,
				"Enable Boot Debug Policy"
			},
			{
				BcdElementDataTypes.MSEBootOrderClean,
				"Enable Boot Order Clean"
			},
			{
				BcdElementDataTypes.MSEDeviceId,
				"Enable Device Id"
			},
			{
				BcdElementDataTypes.MSEFfuLoader,
				"Enable FFU Loader"
			},
			{
				BcdElementDataTypes.MSEIuLoader,
				"Enable IU Loader"
			},
			{
				BcdElementDataTypes.MSEMassStorage,
				"Enable Mass Storage"
			},
			{
				BcdElementDataTypes.MSERpmbProvisioning,
				"Enable RPMB Provisioning"
			},
			{
				BcdElementDataTypes.MSESecureBootPolicy,
				"Enable Secure Boot Policy"
			},
			{
				BcdElementDataTypes.MSEStartCharge,
				"Enable Start Charge"
			},
			{
				BcdElementDataTypes.MSEResetTPM,
				"Enable Reset TPM"
			},
			{
				BcdElementDataTypes.QuietBootEnable,
				"Quiet Boot Enable"
			}
		};

		// Token: 0x0400005B RID: 91
		public static readonly BcdElementDataType OsLoaderDevice = new BcdElementDataType(ElementClass.Library, ElementFormat.Device, 1U);

		// Token: 0x0400005C RID: 92
		public static readonly BcdElementDataType ApplicationPath = new BcdElementDataType(ElementClass.Library, ElementFormat.String, 2U);

		// Token: 0x0400005D RID: 93
		public static readonly BcdElementDataType Description = new BcdElementDataType(ElementClass.Library, ElementFormat.String, 4U);

		// Token: 0x0400005E RID: 94
		public static readonly BcdElementDataType PreferredLocale = new BcdElementDataType(ElementClass.Library, ElementFormat.String, 5U);

		// Token: 0x0400005F RID: 95
		public static readonly BcdElementDataType AutoRecoveryEnabled = new BcdElementDataType(ElementClass.Library, ElementFormat.Boolean, 9U);

		// Token: 0x04000060 RID: 96
		public static readonly BcdElementDataType DebuggerEnabled = new BcdElementDataType(ElementClass.Library, ElementFormat.Boolean, 16U);

		// Token: 0x04000061 RID: 97
		public static readonly BcdElementDataType EmsEnabled = new BcdElementDataType(ElementClass.Library, ElementFormat.Boolean, 32U);

		// Token: 0x04000062 RID: 98
		public static readonly BcdElementDataType DisplayAdvanceOptions = new BcdElementDataType(ElementClass.Library, ElementFormat.Boolean, 64U);

		// Token: 0x04000063 RID: 99
		public static readonly BcdElementDataType DisplayOptionsEdit = new BcdElementDataType(ElementClass.Library, ElementFormat.Boolean, 65U);

		// Token: 0x04000064 RID: 100
		public static readonly BcdElementDataType DisableIntegrityChecks = new BcdElementDataType(ElementClass.Library, ElementFormat.Boolean, 72U);

		// Token: 0x04000065 RID: 101
		public static readonly BcdElementDataType AllowPreReleaseSignatures = new BcdElementDataType(ElementClass.Library, ElementFormat.Boolean, 73U);

		// Token: 0x04000066 RID: 102
		public static readonly BcdElementDataType AllowFlightSignatures = new BcdElementDataType(ElementClass.Library, ElementFormat.Boolean, 126U);

		// Token: 0x04000067 RID: 103
		public static readonly BcdElementDataType ConsoleExtendedInput = new BcdElementDataType(ElementClass.Library, ElementFormat.Boolean, 80U);

		// Token: 0x04000068 RID: 104
		public static readonly BcdElementDataType Inherit = new BcdElementDataType(ElementClass.Library, ElementFormat.ObjectList, 6U);

		// Token: 0x04000069 RID: 105
		public static readonly BcdElementDataType RecoverySequence = new BcdElementDataType(ElementClass.Library, ElementFormat.ObjectList, 8U);

		// Token: 0x0400006A RID: 106
		public static readonly BcdElementDataType DebuggerType = new BcdElementDataType(ElementClass.Library, ElementFormat.Integer, 17U);

		// Token: 0x0400006B RID: 107
		public static readonly BcdElementDataType DebuggerPortAddress = new BcdElementDataType(ElementClass.Library, ElementFormat.Integer, 18U);

		// Token: 0x0400006C RID: 108
		public static readonly BcdElementDataType PortNumber = new BcdElementDataType(ElementClass.Library, ElementFormat.Integer, 19U);

		// Token: 0x0400006D RID: 109
		public static readonly BcdElementDataType DebuggerUsbTargetName = new BcdElementDataType(ElementClass.Library, ElementFormat.String, 22U);

		// Token: 0x0400006E RID: 110
		public static readonly BcdElementDataType DebuggerBusParameters = new BcdElementDataType(ElementClass.Library, ElementFormat.String, 25U);

		// Token: 0x0400006F RID: 111
		public static readonly BcdElementDataType DebuggerIgnoreUsermodeExceptions = new BcdElementDataType(ElementClass.Library, ElementFormat.Boolean, 23U);

		// Token: 0x04000070 RID: 112
		public static readonly BcdElementDataType DebuggerStartPolicy = new BcdElementDataType(ElementClass.Library, ElementFormat.Integer, 24U);

		// Token: 0x04000071 RID: 113
		public static readonly BcdElementDataType DebuggerNetworkKey = new BcdElementDataType(ElementClass.Library, ElementFormat.String, 29U);

		// Token: 0x04000072 RID: 114
		public static readonly BcdElementDataType DebuggerNetworkHostIp = new BcdElementDataType(ElementClass.Library, ElementFormat.Integer, 26U);

		// Token: 0x04000073 RID: 115
		public static readonly BcdElementDataType DebuggerNetworkPort = new BcdElementDataType(ElementClass.Library, ElementFormat.Integer, 27U);

		// Token: 0x04000074 RID: 116
		public static readonly BcdElementDataType DebuggerNetworkDhcp = new BcdElementDataType(ElementClass.Library, ElementFormat.Boolean, 28U);

		// Token: 0x04000075 RID: 117
		public static readonly BcdElementDataType DebuggerBaudRate = new BcdElementDataType(ElementClass.Library, ElementFormat.Integer, 20U);

		// Token: 0x04000076 RID: 118
		public static readonly BcdElementDataType ForceNoKeyboard = new BcdElementDataType(ElementClass.Library, ElementFormat.Boolean, 114U);

		// Token: 0x04000077 RID: 119
		public static readonly BcdElementDataType BootUxFadeDisable = new BcdElementDataType(ElementClass.Library, ElementFormat.Boolean, 106U);

		// Token: 0x04000078 RID: 120
		public static readonly BcdElementDataType BootUxLogoTransitionEnable = new BcdElementDataType(ElementClass.Library, ElementFormat.Boolean, 122U);

		// Token: 0x04000079 RID: 121
		public static readonly BcdElementDataType BootUxLogoTransitionTime = new BcdElementDataType(ElementClass.Library, ElementFormat.Integer, 121U);

		// Token: 0x0400007A RID: 122
		public static readonly BcdElementDataType BootUxProgressAnimationDisable = new BcdElementDataType(ElementClass.Library, ElementFormat.Boolean, 105U);

		// Token: 0x0400007B RID: 123
		public static readonly BcdElementDataType BootUxTextDisable = new BcdElementDataType(ElementClass.Library, ElementFormat.Boolean, 104U);

		// Token: 0x0400007C RID: 124
		public static readonly BcdElementDataType BootUxErrorScreen = new BcdElementDataType(ElementClass.Library, ElementFormat.Integer, 125U);

		// Token: 0x0400007D RID: 125
		public static readonly BcdElementDataType BsdFilepath = new BcdElementDataType(ElementClass.Library, ElementFormat.String, 68U);

		// Token: 0x0400007E RID: 126
		public static readonly BcdElementDataType BsdPreservePreviousEntries = new BcdElementDataType(ElementClass.Library, ElementFormat.Boolean, 69U);

		// Token: 0x0400007F RID: 127
		public static readonly BcdElementDataType LoadOptions = new BcdElementDataType(ElementClass.Library, ElementFormat.String, 48U);

		// Token: 0x04000080 RID: 128
		public static readonly BcdElementDataType GraphicsModeDisable = new BcdElementDataType(ElementClass.Library, ElementFormat.Boolean, 70U);

		// Token: 0x04000081 RID: 129
		public static readonly Dictionary<BcdElementDataType, string> LibraryTypes = new Dictionary<BcdElementDataType, string>
		{
			{
				BcdElementDataTypes.OsLoaderDevice,
				"Boot Device"
			},
			{
				BcdElementDataTypes.ApplicationPath,
				"Application Path"
			},
			{
				BcdElementDataTypes.Description,
				"Description"
			},
			{
				BcdElementDataTypes.PreferredLocale,
				"Preferred Locale"
			},
			{
				BcdElementDataTypes.AutoRecoveryEnabled,
				"Auto-recovery Enabled"
			},
			{
				BcdElementDataTypes.DebuggerEnabled,
				"Debugger Enabled"
			},
			{
				BcdElementDataTypes.EmsEnabled,
				"EMS Enabled"
			},
			{
				BcdElementDataTypes.DisplayAdvanceOptions,
				"Display Advanced Options"
			},
			{
				BcdElementDataTypes.DisplayOptionsEdit,
				"Display Option Edit"
			},
			{
				BcdElementDataTypes.DisableIntegrityChecks,
				"Disable Integrity Checks"
			},
			{
				BcdElementDataTypes.AllowPreReleaseSignatures,
				"Allow Pre-release Signatures"
			},
			{
				BcdElementDataTypes.AllowFlightSignatures,
				"Allow Flight Signatures"
			},
			{
				BcdElementDataTypes.ConsoleExtendedInput,
				"Console Extended Input"
			},
			{
				BcdElementDataTypes.Inherit,
				"Inherit"
			},
			{
				BcdElementDataTypes.RecoverySequence,
				"Recovery Sequence"
			},
			{
				BcdElementDataTypes.DebuggerType,
				"Debugger Type"
			},
			{
				BcdElementDataTypes.DebuggerPortAddress,
				"Debugger Port Address"
			},
			{
				BcdElementDataTypes.PortNumber,
				"Port Number"
			},
			{
				BcdElementDataTypes.DebuggerUsbTargetName,
				"Debugger USB Target Name"
			},
			{
				BcdElementDataTypes.DebuggerBusParameters,
				"Debugger Bus Parameters"
			},
			{
				BcdElementDataTypes.DebuggerIgnoreUsermodeExceptions,
				"Debugger Ignore Usermode Exceptions"
			},
			{
				BcdElementDataTypes.DebuggerStartPolicy,
				"Debugger Start Policy"
			},
			{
				BcdElementDataTypes.DebuggerNetworkKey,
				"Debugger Network Key"
			},
			{
				BcdElementDataTypes.DebuggerNetworkHostIp,
				"Debugger Network Host IP"
			},
			{
				BcdElementDataTypes.DebuggerNetworkPort,
				"Debugger Network Port"
			},
			{
				BcdElementDataTypes.DebuggerNetworkDhcp,
				"Debugger Network DHCP"
			},
			{
				BcdElementDataTypes.DebuggerBaudRate,
				"Debugger Baud Rate"
			},
			{
				BcdElementDataTypes.ForceNoKeyboard,
				"Force No Keyboard"
			},
			{
				BcdElementDataTypes.BootUxFadeDisable,
				"Boot UX Fade Disable"
			},
			{
				BcdElementDataTypes.BootUxLogoTransitionEnable,
				"Boot UX Logo Transition Enable"
			},
			{
				BcdElementDataTypes.BootUxLogoTransitionTime,
				"Boot UX Logo Transition Time"
			},
			{
				BcdElementDataTypes.BootUxProgressAnimationDisable,
				"Boot UX Progress Animation Disable"
			},
			{
				BcdElementDataTypes.BootUxTextDisable,
				"Boot UX Text Disable"
			},
			{
				BcdElementDataTypes.BootUxErrorScreen,
				"Boot UX Error Screen"
			},
			{
				BcdElementDataTypes.BsdFilepath,
				"BSD Filepath"
			},
			{
				BcdElementDataTypes.BsdPreservePreviousEntries,
				"BSD Preserve Previous Entries"
			},
			{
				BcdElementDataTypes.LoadOptions,
				"Load Options"
			},
			{
				BcdElementDataTypes.GraphicsModeDisable,
				"Graphics Mode Disable"
			}
		};

		// Token: 0x04000082 RID: 130
		public static readonly BcdElementDataType RamDiskSdiPath = new BcdElementDataType(ElementClass.Device, ElementFormat.String, 4U);

		// Token: 0x04000083 RID: 131
		public static readonly BcdElementDataType RamDiskSdiDevice = new BcdElementDataType(ElementClass.Device, ElementFormat.Device, 3U);

		// Token: 0x04000084 RID: 132
		public static readonly Dictionary<BcdElementDataType, string> DeviceTypes = new Dictionary<BcdElementDataType, string>
		{
			{
				BcdElementDataTypes.RamDiskSdiPath,
				"Ramdisk SDI Path"
			},
			{
				BcdElementDataTypes.RamDiskSdiDevice,
				"Ramdisk SDI Device"
			}
		};

		// Token: 0x04000085 RID: 133
		public static readonly BcdElementDataType FlashingAction = new BcdElementDataType(ElementClass.Oem, ElementFormat.ObjectList, 1U);

		// Token: 0x04000086 RID: 134
		public static readonly BcdElementDataType ResetMyPhoneAction = new BcdElementDataType(ElementClass.Oem, ElementFormat.ObjectList, 2U);

		// Token: 0x04000087 RID: 135
		public static readonly BcdElementDataType DeveloperMenuAction = new BcdElementDataType(ElementClass.Oem, ElementFormat.ObjectList, 3U);

		// Token: 0x04000088 RID: 136
		public static readonly Dictionary<BcdElementDataType, string> OEMTypes = new Dictionary<BcdElementDataType, string>
		{
			{
				BcdElementDataTypes.FlashingAction,
				"Flashing Action"
			},
			{
				BcdElementDataTypes.ResetMyPhoneAction,
				"Reset My Phone Action"
			},
			{
				BcdElementDataTypes.DeveloperMenuAction,
				"Developer Menu Action"
			}
		};
	}
}
