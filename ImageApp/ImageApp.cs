using System;
using System.Globalization;
using System.IO;
using Microsoft.WindowsPhone.ImageUpdate.PkgCommon;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;
using Microsoft.WindowsPhone.Imaging;

namespace Microsoft.WindowsPhone.ImageUpdate
{
	// Token: 0x02000002 RID: 2
	internal static class ImageApp
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		private static void Main()
		{
			LogUtil.LogCopyright();
			IULogger iulogger = new IULogger();
			iulogger.ErrorLogger = new LogString(ImageApp.LogErrorToFileAndConsole);
			iulogger.WarningLogger = new LogString(ImageApp.LogWarningToFileAndConsole);
			iulogger.InformationLogger = new LogString(ImageApp.LogInfoToFileAndConsole);
			iulogger.DebugLogger = null;
			try
			{
				ImageApp.SetCmdLineParams();
				try
				{
					if (!ImageApp.ParseCommandlineParams(Environment.CommandLine))
					{
						Environment.ExitCode = 1;
						ImageApp.ShowUsageString();
						return;
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.ToString());
					Environment.ExitCode = 1;
					ImageApp.ShowUsageString();
					return;
				}
				if (ImageApp._showDebugMessages)
				{
					iulogger.DebugLogger = new LogString(ImageApp.LogDebugToFileAndConsole);
				}
				Imaging imaging = new Imaging(iulogger);
				Console.CancelKeyPress += imaging.CleanupHandler;
				if (ImageApp._bDoingUpdate)
				{
					imaging.UpdateExistingImage(ImageApp._outputFile, ImageApp._updateInputFile, ImageApp._randomizeGptIDs);
				}
				else
				{
					imaging.SkipImaging = ImageApp._skipImaging;
					imaging.FormatDPP = ImageApp._formatDPP;
					imaging.StrictSettingPolicies = ImageApp._strictSettingPolicies;
					imaging.SkipUpdateMain = ImageApp._bSkipUpdateMain;
					imaging.CPUId = ImageApp._cpuId;
					imaging.BSPProductName = ImageApp._bspProductName;
					imaging.BuildNewImage(ImageApp._outputFile, ImageApp._oemInputFile, ImageApp._msPackagesRoot, ImageApp._oemCustomizationXML, ImageApp._oemCustomizationPPKG, ImageApp._oemCustomizationVersion, ImageApp._randomizeGptIDs);
				}
			}
			catch (ImageCommonException ex2)
			{
				iulogger.LogError("{0}", new object[]
				{
					ex2.Message
				});
				if (ex2.InnerException != null)
				{
					iulogger.LogError("\t{0}", new object[]
					{
						ex2.InnerException.ToString()
					});
				}
				Environment.ExitCode = 2;
			}
			catch (Exception ex3)
			{
				iulogger.LogError("{0}", new object[]
				{
					ex3.Message
				});
				if (ex3.InnerException != null)
				{
					iulogger.LogError("\t{0}", new object[]
					{
						ex3.InnerException.ToString()
					});
				}
				iulogger.LogError("An unhandled exception was thrown: {0}", new object[]
				{
					ex3.ToString()
				});
				Environment.ExitCode = 3;
			}
			finally
			{
				if (Environment.ExitCode != 0)
				{
					Console.WriteLine("ImageApp: Error log can be found at '{0}'.", ImageApp._LogFile);
				}
			}
		}

		// Token: 0x06000002 RID: 2 RVA: 0x000022CC File Offset: 0x000004CC
		private static void AppendToLog(string prepend, ImageApp.LogFunc LoggingFunc, string format, params object[] list)
		{
			string text = format;
			if (list != null && list.Length != 0)
			{
				text = string.Format(CultureInfo.CurrentCulture, format, list);
			}
			LoggingFunc(text);
			text = string.Format("{{{0}}} {1} {2}{3}", new object[]
			{
				DateTime.FromFileTime(DateTime.Now.ToFileTime()),
				prepend,
				text,
				Environment.NewLine
			});
			object @lock = ImageApp._lock;
			lock (@lock)
			{
				File.AppendAllText(ImageApp._LogFile, text);
			}
		}

		// Token: 0x06000003 RID: 3 RVA: 0x00002368 File Offset: 0x00000568
		private static void LogErrorToFileAndConsole(string format, params object[] list)
		{
			ImageApp.AppendToLog("Error:", new ImageApp.LogFunc(LogUtil.Error), format, list);
		}

		// Token: 0x06000004 RID: 4 RVA: 0x00002382 File Offset: 0x00000582
		private static void LogWarningToFileAndConsole(string format, params object[] list)
		{
			ImageApp.AppendToLog("Warning:", new ImageApp.LogFunc(LogUtil.Warning), format, list);
		}

		// Token: 0x06000005 RID: 5 RVA: 0x0000239C File Offset: 0x0000059C
		private static void LogInfoToFileAndConsole(string format, params object[] list)
		{
			ImageApp.AppendToLog(string.Empty, new ImageApp.LogFunc(LogUtil.Message), format, list);
		}

		// Token: 0x06000006 RID: 6 RVA: 0x000023B6 File Offset: 0x000005B6
		private static void LogDebugToFileAndConsole(string format, params object[] list)
		{
			ImageApp.AppendToLog("Debug:", new ImageApp.LogFunc(LogUtil.Diagnostic), format, list);
		}

		// Token: 0x06000007 RID: 7 RVA: 0x000023D0 File Offset: 0x000005D0
		private static bool SetCmdLineParams()
		{
			try
			{
				ImageApp._commandLineParser = new CommandLineParser();
				ImageApp._commandLineParser.SetRequiredParameterString("OutputFile", "The path to the image to be created\\modified");
				ImageApp._commandLineParser.SetOptionalParameterString("OEMInputXML", "Path to the OEM Input XML file");
				ImageApp._commandLineParser.SetOptionalSwitchString("OEMCustomizationXML", "Path to the OEM Customization XML file");
				ImageApp._commandLineParser.SetOptionalSwitchString("OEMCustomizationPPKG", "Path to the OEM Customization PPKG file");
				ImageApp._commandLineParser.SetOptionalSwitchString("OEMVersion", "Version to use for OEM inputs such as customizations. e.g. <major>.<minor>.<submajor>.<subminor>");
				ImageApp._commandLineParser.SetOptionalParameterString("MSPackageRoot", "Path to the Microsoft Package files root. Only used when OEM Input XML");
				ImageApp._commandLineParser.SetOptionalSwitchString("UpdateInputXML", "Path to update input file file");
				ImageApp._commandLineParser.SetOptionalSwitchBoolean("FormatDPP", "Formats DPP partition", ImageApp._formatDPP);
				ImageApp._commandLineParser.SetOptionalSwitchBoolean("StrictSettingPolicies", "Causes settings without policies to produce errors", ImageApp._strictSettingPolicies);
				ImageApp._commandLineParser.SetOptionalSwitchBoolean("SkipImageCreation", "Generates the OEM Customization packages without generating the full image", ImageApp._skipImaging);
				ImageApp._commandLineParser.SetOptionalSwitchBoolean("RandomizeGptIDs", "Randomizes the GPT Disk and Partiton IDs for imaging.  Needed to run ImageApp in parallel", ImageApp._randomizeGptIDs);
				ImageApp._commandLineParser.SetOptionalSwitchBoolean("ShowDebugMessages", "Show additional debug messages", ImageApp._showDebugMessages);
				ImageApp._commandLineParser.SetOptionalSwitchString("CPUType", "Specify target CPU type of x86, ARM, ARM64 or AMD64", "nothing", false, new string[]
				{
					"ARM",
					"X86",
					"ARM64",
					"AMD64",
					"nothing"
				});
				ImageApp._commandLineParser.SetOptionalSwitchString("BSPProductName", "Product name which overrides BSP targeting");
			}
			catch (Exception except)
			{
				throw new NoSuchArgumentException("ImageApp!SetCmdLineParams: Unable to set an option", except);
			}
			return true;
		}

		// Token: 0x06000008 RID: 8 RVA: 0x00002578 File Offset: 0x00000778
		private static bool ParseCommandlineParams(string Commandline)
		{
			string empty = string.Empty;
			string text = string.Empty;
			string text2 = string.Empty;
			if (!ImageApp._commandLineParser.ParseString(Commandline, true))
			{
				return false;
			}
			ImageApp._showDebugMessages = ImageApp._commandLineParser.GetSwitchAsBoolean("ShowDebugMessages");
			ImageApp._outputFile = ImageApp._commandLineParser.GetParameterAsString("OutputFile");
			text = Path.GetFileNameWithoutExtension(ImageApp._outputFile);
			if (text.Length == 0)
			{
				Console.WriteLine("ImageApp!ParseCommandLineParams: The Output File cannot be empty when extension is removed.");
				return false;
			}
			ImageApp._LogFile = Path.Combine(FileUtils.GetShortPathName(Path.GetDirectoryName(ImageApp._outputFile)), text + "." + ImageApp.c_LogFileBase);
			if (ImageApp._LogFile.Length > 260)
			{
				ImageApp._LogFile = FileUtils.GetShortPathName(ImageApp._LogFile);
			}
			if (File.Exists(ImageApp._LogFile))
			{
				File.WriteAllText(ImageApp._LogFile, string.Empty);
			}
			text2 = Path.ChangeExtension(ImageApp._LogFile, ".cbs.log");
			if (File.Exists(text2))
			{
				File.WriteAllText(text2, string.Empty);
			}
			Environment.SetEnvironmentVariable("COMPONENT_BASED_SERVICING_LOGFILE", text2);
			Environment.SetEnvironmentVariable("WINDOWS_TRACING_FLAGS", "3");
			text2 = Path.ChangeExtension(ImageApp._LogFile, ".csi.log");
			if (File.Exists(text2))
			{
				File.WriteAllText(text2, string.Empty);
			}
			Environment.SetEnvironmentVariable("WINDOWS_TRACING_LOGFILE", text2);
			ImageApp._formatDPP = ImageApp._commandLineParser.GetSwitchAsBoolean("FormatDPP");
			ImageApp._strictSettingPolicies = ImageApp._commandLineParser.GetSwitchAsBoolean("StrictSettingPolicies");
			ImageApp._skipImaging = ImageApp._commandLineParser.GetSwitchAsBoolean("SkipImageCreation");
			ImageApp._randomizeGptIDs = ImageApp._commandLineParser.GetSwitchAsBoolean("RandomizeGptIDs");
			ImageApp._msPackagesRoot = ImageApp._commandLineParser.GetParameterAsString("MSPackageRoot");
			ImageApp._oemInputFile = ImageApp._commandLineParser.GetParameterAsString("OEMInputXML");
			ImageApp._oemCustomizationXML = ImageApp._commandLineParser.GetSwitchAsString("OEMCustomizationXML");
			ImageApp._oemCustomizationPPKG = ImageApp._commandLineParser.GetSwitchAsString("OEMCustomizationPPKG");
			ImageApp._oemCustomizationVersion = ImageApp._commandLineParser.GetSwitchAsString("OEMVersion");
			ImageApp._bspProductName = ImageApp._commandLineParser.GetSwitchAsString("BSPProductName");
			ImageApp._updateInputFile = ImageApp._commandLineParser.GetSwitchAsString("UpdateInputXML");
			if (!string.IsNullOrEmpty(ImageApp._updateInputFile))
			{
				if (!string.IsNullOrEmpty(ImageApp._oemInputFile))
				{
					Console.WriteLine("ImageApp!ParseCommandLineParams: The OEMInputXML and UpdateInputXML mutually exclusive. Use the OEMInputXML file for creating images and the UpdateInputXML file for updating an existing image.");
					return false;
				}
				ImageApp._bDoingUpdate = true;
			}
			ImageApp._bSkipUpdateMain = (string.IsNullOrEmpty(ImageApp._oemInputFile) && string.IsNullOrEmpty(ImageApp._updateInputFile));
			ImageApp._cpuType = ImageApp._commandLineParser.GetSwitchAsString("CPUType");
			if (string.Compare(ImageApp._cpuType, "nothing", StringComparison.OrdinalIgnoreCase) != 0)
			{
				try
				{
					ImageApp._cpuId = CpuIdParser.Parse(ImageApp._cpuType);
				}
				catch
				{
					Console.WriteLine("ImageApp!ParseCommandLineParams: The CPUType was not a recognized type.");
					return false;
				}
				Console.WriteLine("ImageApp: Setting CPU Type to '" + ImageApp._cpuType + "'.");
				return true;
			}
			return true;
		}

		// Token: 0x06000009 RID: 9 RVA: 0x00002854 File Offset: 0x00000A54
		private static void ShowUsageString()
		{
			Console.WriteLine(ImageApp._commandLineParser.UsageString());
		}

		// Token: 0x04000001 RID: 1
		private static string c_LogFileBase = "ImageApp.log";

		// Token: 0x04000002 RID: 2
		private static string _LogFile = string.Empty;

		// Token: 0x04000003 RID: 3
		private static CommandLineParser _commandLineParser = null;

		// Token: 0x04000004 RID: 4
		private static string _oemInputFile = string.Empty;

		// Token: 0x04000005 RID: 5
		private static string _oemCustomizationXML = string.Empty;

		// Token: 0x04000006 RID: 6
		private static string _oemCustomizationPPKG = string.Empty;

		// Token: 0x04000007 RID: 7
		private static string _oemCustomizationVersion = string.Empty;

		// Token: 0x04000008 RID: 8
		private static string _msPackagesRoot = string.Empty;

		// Token: 0x04000009 RID: 9
		private static string _updateInputFile = string.Empty;

		// Token: 0x0400000A RID: 10
		private static string _outputFile = string.Empty;

		// Token: 0x0400000B RID: 11
		private static bool _bDoingUpdate = false;

		// Token: 0x0400000C RID: 12
		private static bool _bSkipUpdateMain = false;

		// Token: 0x0400000D RID: 13
		private static string _cpuType = string.Empty;

		// Token: 0x0400000E RID: 14
		private static CpuId _cpuId = CpuId.Invalid;

		// Token: 0x0400000F RID: 15
		private static string _bspProductName;

		// Token: 0x04000010 RID: 16
		private static bool _formatDPP = false;

		// Token: 0x04000011 RID: 17
		private static bool _strictSettingPolicies = false;

		// Token: 0x04000012 RID: 18
		private static bool _skipImaging = false;

		// Token: 0x04000013 RID: 19
		private static bool _randomizeGptIDs = false;

		// Token: 0x04000014 RID: 20
		private static bool _showDebugMessages = false;

		// Token: 0x04000015 RID: 21
		private static readonly object _lock = new object();

		// Token: 0x02000003 RID: 3
		// (Invoke) Token: 0x0600000C RID: 12
		public delegate void LogFunc(string msg);
	}
}
