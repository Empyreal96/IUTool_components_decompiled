using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.WindowsPhone.ImageUpdate.PkgCommon;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.ImageUpdate.OemCustomizationTool
{
	// Token: 0x0200000F RID: 15
	internal class Settings
	{
		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000045 RID: 69 RVA: 0x000041DC File Offset: 0x000023DC
		public static string ConfigSchema
		{
			get
			{
				if (Settings.configSchema == string.Empty)
				{
					string[] manifestResourceNames = Assembly.GetExecutingAssembly().GetManifestResourceNames();
					TraceLogger.LogMessage(TraceLevel.Info, "Looking for Config schema in resource list:", true);
					foreach (string text in manifestResourceNames)
					{
						TraceLogger.LogMessage(TraceLevel.Info, text, true);
						if (text.EndsWith("Config.xsd", StringComparison.OrdinalIgnoreCase))
						{
							Settings.configSchema = text;
							return Settings.configSchema;
						}
					}
					throw new SystemException("Could not find the Embedded Config schema resource.");
				}
				return Settings.configSchema;
			}
		}

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000046 RID: 70 RVA: 0x00004258 File Offset: 0x00002458
		public static string CustomizationSchema
		{
			get
			{
				if (Settings.customizationSchema == string.Empty)
				{
					string[] manifestResourceNames = Assembly.GetExecutingAssembly().GetManifestResourceNames();
					TraceLogger.LogMessage(TraceLevel.Info, "Looking for Customization schema in resource list:", true);
					foreach (string text in manifestResourceNames)
					{
						TraceLogger.LogMessage(TraceLevel.Info, text, true);
						if (text.EndsWith("Customization.xsd", StringComparison.OrdinalIgnoreCase))
						{
							Settings.customizationSchema = text;
							return Settings.customizationSchema;
						}
					}
					throw new SystemException("Could not find the Embedded Customization schema resource.");
				}
				return Settings.customizationSchema;
			}
		}

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x06000047 RID: 71 RVA: 0x000042D4 File Offset: 0x000024D4
		public static string RegistrySchema
		{
			get
			{
				if (Settings.registrySchema == string.Empty)
				{
					string[] manifestResourceNames = Assembly.GetExecutingAssembly().GetManifestResourceNames();
					TraceLogger.LogMessage(TraceLevel.Info, "Looking for Registry schema in resource list:", true);
					foreach (string text in manifestResourceNames)
					{
						TraceLogger.LogMessage(TraceLevel.Info, text, true);
						if (text.EndsWith("Registry.xsd", StringComparison.OrdinalIgnoreCase))
						{
							Settings.registrySchema = text;
							return Settings.registrySchema;
						}
					}
					throw new SystemException("Could not find the Embedded Registry schema resource.");
				}
				return Settings.registrySchema;
			}
		}

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x06000048 RID: 72 RVA: 0x0000434D File Offset: 0x0000254D
		public static Stream PkgGenCfgXml
		{
			get
			{
				if (Settings.pkgGenCfgXml == null)
				{
					Settings.pkgGenCfgXml = Assembly.GetExecutingAssembly().GetManifestResourceStream(typeof(Program).Namespace + ".pkggen.cfg.xml");
				}
				return Settings.pkgGenCfgXml;
			}
		}

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x06000049 RID: 73 RVA: 0x00004383 File Offset: 0x00002583
		// (set) Token: 0x0600004A RID: 74 RVA: 0x0000438A File Offset: 0x0000258A
		public static string CustomizationIncludeDirectory
		{
			get
			{
				return Settings.customizationIncludeDirectory;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					return;
				}
				if (value.EndsWith("\\", StringComparison.OrdinalIgnoreCase))
				{
					Settings.customizationIncludeDirectory = value;
					return;
				}
				Settings.customizationIncludeDirectory = value + "\\";
			}
		}

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x0600004B RID: 75 RVA: 0x000043BA File Offset: 0x000025BA
		// (set) Token: 0x0600004C RID: 76 RVA: 0x000043C1 File Offset: 0x000025C1
		public static List<XmlFile> CustomizationFiles
		{
			get
			{
				return Settings.customizationFiles;
			}
			set
			{
				Settings.customizationFiles = value;
			}
		}

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x0600004D RID: 77 RVA: 0x000043C9 File Offset: 0x000025C9
		// (set) Token: 0x0600004E RID: 78 RVA: 0x000043D0 File Offset: 0x000025D0
		public static List<XmlFile> ConfigFiles
		{
			get
			{
				return Settings.configFiles;
			}
			set
			{
				Settings.configFiles = value;
			}
		}

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x0600004F RID: 79 RVA: 0x000043D8 File Offset: 0x000025D8
		// (set) Token: 0x06000050 RID: 80 RVA: 0x000043DF File Offset: 0x000025DF
		public static bool WarnOnMappingNotFound
		{
			get
			{
				return Settings.warnOnMappingNotFound;
			}
			set
			{
				Settings.warnOnMappingNotFound = value;
			}
		}

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000051 RID: 81 RVA: 0x000043E7 File Offset: 0x000025E7
		// (set) Token: 0x06000052 RID: 82 RVA: 0x000043EE File Offset: 0x000025EE
		public static bool Diagnostics
		{
			get
			{
				return Settings.diagnostics;
			}
			set
			{
				Settings.diagnostics = value;
				if (Settings.diagnostics)
				{
					TraceLogger.TraceLevel = TraceLevel.Info;
				}
			}
		}

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x06000053 RID: 83 RVA: 0x00004403 File Offset: 0x00002603
		// (set) Token: 0x06000054 RID: 84 RVA: 0x0000440A File Offset: 0x0000260A
		public static string OutputDirectoryPath
		{
			get
			{
				return Settings.outputDirectoryPath;
			}
			set
			{
				Settings.outputDirectoryPath = Environment.ExpandEnvironmentVariables(value);
			}
		}

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x06000055 RID: 85 RVA: 0x00004417 File Offset: 0x00002617
		// (set) Token: 0x06000056 RID: 86 RVA: 0x0000444A File Offset: 0x0000264A
		public static string OutputPkgFilePath
		{
			get
			{
				if (Settings.OutputDirectoryPath.EndsWith("\\", StringComparison.OrdinalIgnoreCase))
				{
					return Path.GetFullPath(Settings.OutputDirectoryPath);
				}
				return Path.GetFullPath(Settings.OutputDirectoryPath) + "\\";
			}
			set
			{
				TraceLogger.LogMessage(TraceLevel.Warn, "Trying to set output package filename to '" + value + "'. Ignoring because pkg filenames are fixed.", true);
			}
		}

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x06000057 RID: 87 RVA: 0x00004463 File Offset: 0x00002663
		public static string TempDirectoryPath
		{
			get
			{
				if (string.IsNullOrEmpty(Settings.tempDirectoryPath))
				{
					Settings.tempDirectoryPath = FileUtils.GetTempDirectory();
					TraceLogger.LogMessage(TraceLevel.Info, "Temp Directory: " + Settings.tempDirectoryPath, true);
				}
				return Settings.tempDirectoryPath;
			}
		}

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x06000058 RID: 88 RVA: 0x00004496 File Offset: 0x00002696
		public static string MergeFilePath
		{
			get
			{
				if (string.IsNullOrEmpty(Settings.mergeFilePath))
				{
					Settings.mergeFilePath = FileUtils.GetTempFile(Settings.TempDirectoryPath);
					TraceLogger.LogMessage(TraceLevel.Info, "Merge file: " + Settings.mergeFilePath, true);
				}
				return Settings.mergeFilePath;
			}
		}

		// Token: 0x0400003C RID: 60
		private static string configSchema = string.Empty;

		// Token: 0x0400003D RID: 61
		private static string customizationSchema = string.Empty;

		// Token: 0x0400003E RID: 62
		private static string registrySchema = string.Empty;

		// Token: 0x0400003F RID: 63
		private const string c_strPkgGenCfgXmlName = "pkggen.cfg.xml";

		// Token: 0x04000040 RID: 64
		private static Stream pkgGenCfgXml = null;

		// Token: 0x04000041 RID: 65
		public const string MagicRegFilename = "OEMSettings.reg";

		// Token: 0x04000042 RID: 66
		private static string customizationIncludeDirectory = string.Empty;

		// Token: 0x04000043 RID: 67
		private static List<XmlFile> customizationFiles = null;

		// Token: 0x04000044 RID: 68
		private static List<XmlFile> configFiles = null;

		// Token: 0x04000045 RID: 69
		private static bool warnOnMappingNotFound = false;

		// Token: 0x04000046 RID: 70
		private static bool diagnostics = false;

		// Token: 0x04000047 RID: 71
		private static string outputDirectoryPath = ".\\";

		// Token: 0x04000048 RID: 72
		private static string tempDirectoryPath = string.Empty;

		// Token: 0x04000049 RID: 73
		private static string mergeFilePath = string.Empty;

		// Token: 0x0200001D RID: 29
		public class PackageAttributes
		{
			// Token: 0x17000027 RID: 39
			// (get) Token: 0x06000092 RID: 146 RVA: 0x00005745 File Offset: 0x00003945
			// (set) Token: 0x06000093 RID: 147 RVA: 0x0000574C File Offset: 0x0000394C
			public static string Owner
			{
				get
				{
					return Settings.PackageAttributes.owner;
				}
				set
				{
					Settings.PackageAttributes.owner = value;
				}
			}

			// Token: 0x17000028 RID: 40
			// (get) Token: 0x06000094 RID: 148 RVA: 0x00005754 File Offset: 0x00003954
			// (set) Token: 0x06000095 RID: 149 RVA: 0x00005768 File Offset: 0x00003968
			public static string OwnerTypeString
			{
				get
				{
					return Settings.PackageAttributes.OwnerType.ToString();
				}
				set
				{
					if (value.Equals(OwnerType.OEM.ToString(), StringComparison.OrdinalIgnoreCase))
					{
						Settings.PackageAttributes.OwnerType = OwnerType.OEM;
						return;
					}
					if (value.Equals(OwnerType.MobileOperator.ToString(), StringComparison.OrdinalIgnoreCase))
					{
						Settings.PackageAttributes.OwnerType = OwnerType.MobileOperator;
						return;
					}
					if (value.Equals(OwnerType.Microsoft.ToString(), StringComparison.OrdinalIgnoreCase))
					{
						Settings.PackageAttributes.OwnerType = OwnerType.Microsoft;
						return;
					}
					if (value.Equals(OwnerType.SiliconVendor.ToString(), StringComparison.OrdinalIgnoreCase))
					{
						Settings.PackageAttributes.OwnerType = OwnerType.SiliconVendor;
						return;
					}
					throw new CustomizationXmlException("OwnerType attribute is invalid. Expecting 'Microsoft', 'OEM', 'SiliconVendor' or 'MobileOperator'. Received " + value);
				}
			}

			// Token: 0x17000029 RID: 41
			// (get) Token: 0x06000096 RID: 150 RVA: 0x00005801 File Offset: 0x00003A01
			// (set) Token: 0x06000097 RID: 151 RVA: 0x00005814 File Offset: 0x00003A14
			public static string ReleaseTypeString
			{
				get
				{
					return Settings.PackageAttributes.ReleaseType.ToString();
				}
				set
				{
					if (value.Equals(ReleaseType.Production.ToString(), StringComparison.OrdinalIgnoreCase))
					{
						Settings.PackageAttributes.ReleaseType = ReleaseType.Production;
						return;
					}
					if (value.Equals(ReleaseType.Test.ToString(), StringComparison.OrdinalIgnoreCase))
					{
						Settings.PackageAttributes.ReleaseType = ReleaseType.Test;
						return;
					}
					throw new CustomizationXmlException("ReleaseType attribute is invalid. Expecting 'Production' or 'Test'. Received " + value);
				}
			}

			// Token: 0x1700002A RID: 42
			// (get) Token: 0x06000098 RID: 152 RVA: 0x0000586F File Offset: 0x00003A6F
			// (set) Token: 0x06000099 RID: 153 RVA: 0x00005884 File Offset: 0x00003A84
			public static string CpuTypeString
			{
				get
				{
					return Settings.PackageAttributes.CpuType.ToString();
				}
				set
				{
					if (value.Equals(CpuId.X86.ToString(), StringComparison.OrdinalIgnoreCase))
					{
						Settings.PackageAttributes.CpuType = CpuId.X86;
						return;
					}
					if (value.Equals(CpuId.ARM.ToString(), StringComparison.OrdinalIgnoreCase))
					{
						Settings.PackageAttributes.CpuType = CpuId.ARM;
						return;
					}
					throw new ArgumentException("CpuType attribute is invalid. Expecting 'X86' or 'ARM'. Received " + value);
				}
			}

			// Token: 0x1700002B RID: 43
			// (get) Token: 0x0600009A RID: 154 RVA: 0x000058DF File Offset: 0x00003ADF
			// (set) Token: 0x0600009B RID: 155 RVA: 0x000058F4 File Offset: 0x00003AF4
			public static string VersionString
			{
				get
				{
					return Settings.PackageAttributes.Version.ToString();
				}
				set
				{
					if (!new Regex("^[0-9]*[.][0-9]*[.][0-9]*[.][0-9]*$").IsMatch(value))
					{
						throw new ArgumentException("Unexpected version string: '" + value + "'");
					}
					string[] array = value.Split(new char[]
					{
						'.'
					});
					Settings.PackageAttributes.Version.Major = ushort.Parse(array[0]);
					Settings.PackageAttributes.Version.Minor = ushort.Parse(array[1]);
					Settings.PackageAttributes.Version.QFE = ushort.Parse(array[2]);
					Settings.PackageAttributes.Version.Build = ushort.Parse(array[3]);
				}
			}

			// Token: 0x1700002C RID: 44
			// (get) Token: 0x0600009C RID: 156 RVA: 0x00005983 File Offset: 0x00003B83
			public static string MainOSPkgFilename
			{
				get
				{
					return string.Concat(new string[]
					{
						Settings.PackageAttributes.Owner,
						".",
						Settings.PackageAttributes.mainOSPartitionString,
						".ImageCustomization.RegistryCustomization",
						PkgConstants.c_strPackageExtension
					});
				}
			}

			// Token: 0x1700002D RID: 45
			// (get) Token: 0x0600009D RID: 157 RVA: 0x000059B8 File Offset: 0x00003BB8
			public static string UpdateOSPkgFilename
			{
				get
				{
					return string.Concat(new string[]
					{
						Settings.PackageAttributes.Owner,
						".",
						Settings.PackageAttributes.updateOSPartitionString,
						".ImageCustomization.RegistryCustomization",
						PkgConstants.c_strPackageExtension
					});
				}
			}

			// Token: 0x1700002E RID: 46
			// (get) Token: 0x0600009E RID: 158 RVA: 0x000059ED File Offset: 0x00003BED
			public static string EfiPkgFilename
			{
				get
				{
					return string.Concat(new string[]
					{
						Settings.PackageAttributes.Owner,
						".",
						Settings.PackageAttributes.efiPartitionString,
						".ImageCustomization.RegistryCustomization",
						PkgConstants.c_strPackageExtension
					});
				}
			}

			// Token: 0x1700002F RID: 47
			// (get) Token: 0x0600009F RID: 159 RVA: 0x00005A24 File Offset: 0x00003C24
			public static BuildType BuildType
			{
				get
				{
					string environmentVariable = Environment.GetEnvironmentVariable("_BuildType");
					if (environmentVariable != null && environmentVariable.Equals("chk", StringComparison.OrdinalIgnoreCase))
					{
						return BuildType.Checked;
					}
					if (environmentVariable != null && environmentVariable.Equals("fre", StringComparison.OrdinalIgnoreCase))
					{
						return BuildType.Retail;
					}
					TraceLogger.LogMessage(TraceLevel.Warn, "Environment variable %_BuildType% not set. Using 'fre'.", true);
					return BuildType.Retail;
				}
			}

			// Token: 0x0400006B RID: 107
			public static readonly string DeviceRegFilePath = PkgConstants.c_strRguDeviceFolder + "\\";

			// Token: 0x0400006C RID: 108
			public static readonly string DeviceLogFilePath = "\\Windows\\Logs\\OemCustomizationTool\\";

			// Token: 0x0400006D RID: 109
			private static string owner = "UnknownOwner";

			// Token: 0x0400006E RID: 110
			public static OwnerType OwnerType = OwnerType.Invalid;

			// Token: 0x0400006F RID: 111
			public static ReleaseType ReleaseType = ReleaseType.Invalid;

			// Token: 0x04000070 RID: 112
			public const string Component = "ImageCustomization";

			// Token: 0x04000071 RID: 113
			public const string SubComponent = "RegistryCustomization";

			// Token: 0x04000072 RID: 114
			public static readonly string mainOSPartitionString = PkgConstants.c_strMainOsPartition;

			// Token: 0x04000073 RID: 115
			public static readonly string updateOSPartitionString = PkgConstants.c_strUpdateOsPartition;

			// Token: 0x04000074 RID: 116
			public static readonly string efiPartitionString = PkgConstants.c_strEfiPartition;

			// Token: 0x04000075 RID: 117
			public static CpuId CpuType = CpuId.ARM;

			// Token: 0x04000076 RID: 118
			public static VersionInfo Version = new VersionInfo(0, 0, 0, 0);
		}
	}
}
