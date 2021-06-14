using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Microsoft.WindowsPhone.ImageUpdate.OemCustomizationTool
{
	// Token: 0x0200000A RID: 10
	internal class InputParameters
	{
		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000033 RID: 51 RVA: 0x00003983 File Offset: 0x00001B83
		// (set) Token: 0x06000034 RID: 52 RVA: 0x0000398B File Offset: 0x00001B8B
		public bool IsInputParamValid { get; set; }

		// Token: 0x06000035 RID: 53 RVA: 0x00003994 File Offset: 0x00001B94
		public InputParameters(string[] args)
		{
			if (args.Count<string>() == 0 || (args.Count<string>() == 1 && args[0].StartsWith("/?", StringComparison.OrdinalIgnoreCase)))
			{
				this.Usage();
				return;
			}
			if (args[0] == null || args[1] == null)
			{
				this.Usage();
				return;
			}
			string text = args[0];
			string path = args[1];
			if (!File.Exists(text) || !Directory.Exists(path) || Directory.GetFiles(path).Count<string>() == 0)
			{
				TraceLogger.LogMessage(TraceLevel.Error, "Ensure that the config xml and customization xml directories and files exist.", true);
				this.Usage();
				return;
			}
			Settings.CustomizationFiles = new List<XmlFile>();
			Settings.CustomizationFiles.Add(new XmlFile(text, Settings.CustomizationSchema));
			Settings.CustomizationIncludeDirectory = Path.GetDirectoryName(text);
			Settings.ConfigFiles = new List<XmlFile>();
			foreach (string text2 in Directory.GetFiles(path))
			{
				if (text2.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
				{
					Settings.ConfigFiles.Add(new XmlFile(text2, Settings.ConfigSchema));
				}
			}
			try
			{
				for (int j = 2; j < args.Count<string>(); j++)
				{
					if (args[j] != null && !(args[j] == ""))
					{
						if (args[j].StartsWith("/output=", StringComparison.OrdinalIgnoreCase))
						{
							Settings.OutputDirectoryPath = args[j].Split(new char[]
							{
								'='
							})[1];
						}
						else if (args[j].StartsWith("/version=", StringComparison.OrdinalIgnoreCase))
						{
							Settings.PackageAttributes.VersionString = args[j].Split(new char[]
							{
								'='
							})[1];
						}
						else if (args[j].StartsWith("/cpu=", StringComparison.OrdinalIgnoreCase))
						{
							Settings.PackageAttributes.CpuTypeString = args[j].Split(new char[]
							{
								'='
							})[1];
						}
						else if (args[j].StartsWith("/warnOnMappingNotFound", StringComparison.OrdinalIgnoreCase))
						{
							Settings.WarnOnMappingNotFound = true;
						}
						else
						{
							if (!args[j].StartsWith("/diagnostic", StringComparison.OrdinalIgnoreCase))
							{
								TraceLogger.LogMessage(TraceLevel.Error, "Unexpected parameter: " + args[j], true);
								this.Usage();
								return;
							}
							Settings.Diagnostics = true;
						}
					}
				}
				TraceLogger.LogMessage(TraceLevel.Info, "Validated input parameters.", true);
				this.IsInputParamValid = true;
			}
			catch (Exception ex)
			{
				TraceLogger.LogMessage(TraceLevel.Error, ex.ToString(), true);
				this.Usage();
			}
		}

		// Token: 0x06000036 RID: 54 RVA: 0x00003BE8 File Offset: 0x00001DE8
		private void Usage()
		{
			string arg = Assembly.GetExecutingAssembly().FullName.Split(new char[]
			{
				','
			})[0] + ".exe";
			string str = string.Format("\r\n{0}: Generates customization packages.\r\n\r\nUsage:\r\n------\r\n\r\n{0}     <Customization File> <Config File Directory> [/output=Output Directory] \r\n        [/version=Version String] [/warnOnMappingNotFound] [/diagnostic]\r\n        \r\nParameters:\r\n-----------\r\n\r\n<Customization File>       Required. The path to the Customization XML file. \r\n                           If drawing from multiple sources, this file must \r\n                           include the other Customization XMLs. If a \r\n                           customization is repeated in multiple files in the \r\n                           include hierarchy, the last one in the hierarchy wins.\r\n                           Environment variables should be quoted with the percent \r\n                           sign character, e.g., %CUSTOM_PATH%.\r\n\t\r\n<Config File Directory>    Required. The path to the directory which contains Config \r\n                           XML files. All *.xml files in this directory are processed. \r\n                           Mappings specified in config files must be unique, i.e., if \r\n                           a mapping appears more than once, an exception is thrown.\r\n                           Environment variables should be quoted with the percent \r\n                           sign character, e.g., %CUSTOM_PATH%.\r\n                           \r\n/output=Output Directory   Optional. The path to the directory where the output package(s)\r\n                           should be saved. Default location is present working directory.\r\n                           Environment variables should be quoted with the percent \r\n                           sign character, e.g., %CUSTOM_PATH%.\r\n                           \r\n/version=Version String    Optional. Specifies the version of the package using the format \r\n                           “<major>.<minor>.<hotfix>.<build>”. Default is “0.0.0.0”.\r\n                           \r\n/warnOnMappingNotFound     Optional. If a setting in the CustomizationXML does not have a \r\n                           corresponding mapping in the ConfigXML, the tool will raise an \r\n                           exception and stop processing by default. If this option is \r\n                           specified, the tool will issue a warning and continue processing.\r\n                           \r\n/diagnostic                Optional. Enable verbose debugging messages to console. Default \r\n                           is disabled.", arg);
			TraceLogger.LogMessage(TraceLevel.Error, string.Format(Environment.NewLine + str + Environment.NewLine, new object[0]), true);
		}
	}
}
