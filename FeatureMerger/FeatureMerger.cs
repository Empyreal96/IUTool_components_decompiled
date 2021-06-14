using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.WindowsPhone.CompDB;
using Microsoft.WindowsPhone.FeatureAPI;
using Microsoft.WindowsPhone.ImageUpdate.PkgCommon;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;
using Microsoft.WindowsPhone.Imaging;

namespace Microsoft.WindowsPhone.ImageUpdate.FeatureMerger
{
	// Token: 0x02000002 RID: 2
	public class FeatureMerger
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		private static void Main(string[] args)
		{
			FeatureMerger.SetCmdLineParams();
			if (!FeatureMerger._commandLineParser.ParseString(Environment.CommandLine, true))
			{
				FeatureMerger.DisplayUsage();
				Environment.ExitCode = 1;
				return;
			}
			string parameterAsString = FeatureMerger._commandLineParser.GetParameterAsString("InputFile");
			string parameterAsString2 = FeatureMerger._commandLineParser.GetParameterAsString("OutputPackageDir");
			string parameterAsString3 = FeatureMerger._commandLineParser.GetParameterAsString("OutputFMDir");
			string parameterAsString4 = FeatureMerger._commandLineParser.GetParameterAsString("OutputPackageVersion");
			string switchAsString = FeatureMerger._commandLineParser.GetSwitchAsString("InputFMDir");
			string switchAsString2 = FeatureMerger._commandLineParser.GetSwitchAsString("MergePackageRootReplacement");
			string switchAsString3 = FeatureMerger._commandLineParser.GetSwitchAsString("variables");
			string switchAsString4 = FeatureMerger._commandLineParser.GetSwitchAsString("FMID");
			string switchAsString5 = FeatureMerger._commandLineParser.GetSwitchAsString("OwnerType");
			string switchAsString6 = FeatureMerger._commandLineParser.GetSwitchAsString("OwnerName");
			string switchAsString7 = FeatureMerger._commandLineParser.GetSwitchAsString("Languages");
			string switchAsString8 = FeatureMerger._commandLineParser.GetSwitchAsString("Resolutions");
			string text = FeatureMerger._commandLineParser.GetSwitchAsString("Critical");
			if (string.IsNullOrEmpty(text))
			{
				text = FeatureMerger.CriticalFMProcessing.All.ToString();
			}
			bool switchAsBoolean = FeatureMerger._commandLineParser.GetSwitchAsBoolean("Incremental");
			bool switchAsBoolean2 = FeatureMerger._commandLineParser.GetSwitchAsBoolean("Compress");
			bool switchAsBoolean3 = FeatureMerger._commandLineParser.GetSwitchAsBoolean("ConvertToCBS");
			FeatureMerger._supportedLocales.Add("en-us");
			try
			{
				if (!FeatureMerger.MergeFeatures(parameterAsString, parameterAsString2, parameterAsString3, parameterAsString4, switchAsString, text, switchAsString2, switchAsString3, switchAsString4, switchAsString5, switchAsString6, switchAsString7, switchAsString8, switchAsBoolean2, switchAsBoolean3, switchAsBoolean))
				{
					Environment.ExitCode = 1;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("{0}", ex.Message);
				if (ex.InnerException != null)
				{
					Console.WriteLine("\t{0}", ex.InnerException.ToString());
				}
				Console.WriteLine("An unhandled exception was thrown: {0}", ex.ToString());
				Environment.ExitCode = 3;
			}
		}

		// Token: 0x06000002 RID: 2 RVA: 0x00002240 File Offset: 0x00000440
		public static bool MergeFeatures(string inputFile, string outputPackageDir, string outputFMDir, string versionStr, string inputFMDir, string critical, string packagePathReplacement, string variablesStr, string fmID, string ownerTypeStr, string ownerName, string languages, string resolutions, bool compress, bool convertToCBS, bool incremental)
		{
			FeatureMerger._outputPackageDir = outputPackageDir;
			FeatureMerger._outputFMDir = outputFMDir;
			FeatureMerger._inputFMDir = inputFMDir;
			FeatureMerger._packagePathReplacement = packagePathReplacement;
			FeatureMerger._incremental = incremental;
			FeatureMerger._compress = compress;
			FeatureMerger._convertToCBS = convertToCBS;
			if (string.IsNullOrEmpty(inputFile))
			{
				LogUtil.Error("Input file cannot be empty.");
				FeatureMerger.DisplayUsage();
				Environment.ExitCode = 1;
				return false;
			}
			if (!File.Exists(inputFile))
			{
				LogUtil.Error("Input file must be an existing FMFileList or Feature Manifest.");
				FeatureMerger.DisplayUsage();
				Environment.ExitCode = 1;
				return false;
			}
			if (string.IsNullOrEmpty(versionStr))
			{
				LogUtil.Error("Non-empty output version string is required");
				FeatureMerger.DisplayUsage();
				Environment.ExitCode = 1;
				return false;
			}
			if (!VersionInfo.TryParse(versionStr, out FeatureMerger._version))
			{
				LogUtil.Error("Invalid output version string '{0}'", new object[]
				{
					versionStr
				});
				FeatureMerger.DisplayUsage();
				Environment.ExitCode = 1;
				return false;
			}
			if (!FeatureMerger.LoadFMFiles(inputFile))
			{
				LogUtil.Error("Invalid input file '{0}'", new object[]
				{
					inputFile
				});
				Environment.ExitCode = 1;
				return false;
			}
			if (!FeatureMerger.ParseVariables(variablesStr))
			{
				FeatureMerger.DisplayUsage();
				Environment.ExitCode = 1;
				return false;
			}
			if (FeatureMerger._singleFM)
			{
				if (!FeatureMerger.PrepSingleFM(inputFile, fmID, ownerTypeStr, ownerName, languages, resolutions))
				{
					FeatureMerger.DisplayUsage();
					Environment.ExitCode = 1;
					return false;
				}
			}
			else if (!string.IsNullOrEmpty(critical))
			{
				bool ignoreCase = true;
				if (!Enum.TryParse<FeatureMerger.CriticalFMProcessing>(critical, ignoreCase, out FeatureMerger._critical))
				{
					LogUtil.Error("Critical value is not recognized.  Must one of the following:\n\t{0}\n\t{1}\n\t{2}", new object[]
					{
						FeatureMerger.CriticalFMProcessing.Yes.ToString(),
						FeatureMerger.CriticalFMProcessing.No.ToString(),
						FeatureMerger.CriticalFMProcessing.All.ToString()
					});
					return false;
				}
			}
			if (!Directory.Exists(FeatureMerger._outputPackageDir))
			{
				Directory.CreateDirectory(FeatureMerger._outputPackageDir);
			}
			if (!Directory.Exists(FeatureMerger._outputFMDir))
			{
				Directory.CreateDirectory(FeatureMerger._outputFMDir);
			}
			if (FeatureMerger._doPlatformManifest)
			{
				FeatureMerger.CheckForUserInstallableFM();
			}
			List<FMCollectionItem> allRelavantFMs = FeatureMerger.GetAllRelavantFMs(FeatureMerger._fmFileList);
			if (!FeatureMerger.CheckPackages(allRelavantFMs))
			{
				return false;
			}
			if (allRelavantFMs.Any((FMCollectionItem item) => item.ownerType == OwnerType.Microsoft))
			{
				FeatureMerger._buildInfo = Environment.ExpandEnvironmentVariables("%_RELEASELABEL%.%_PARENTBRANCHBUILDNUMBER%.%_QFELEVEL%.%_BUILDTIME%");
			}
			using (List<FMCollectionItem>.Enumerator enumerator = allRelavantFMs.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!FeatureMerger.MergeOneFM(enumerator.Current))
					{
						return false;
					}
				}
			}
			FeatureMerger.ProcessUnmergedPackages(FeatureMerger._fmFileList.FMs.Except(allRelavantFMs).ToList<FMCollectionItem>());
			return true;
		}

		// Token: 0x06000003 RID: 3 RVA: 0x000024B4 File Offset: 0x000006B4
		private static void ProcessUnmergedPackages(List<FMCollectionItem> fmItems)
		{
			if (fmItems.Any((FMCollectionItem item) => item.ownerType == OwnerType.Microsoft) && FeatureMerger._nonMergedPackageList.Any<string>())
			{
				FeatureMerger._nonMergedPackageList = FeatureMerger._nonMergedPackageList.Distinct(StringComparer.OrdinalIgnoreCase).ToList<string>();
				if (FeatureMerger._critical == FeatureMerger.CriticalFMProcessing.No)
				{
					foreach (FMCollectionItem fmcollectionItem in fmItems)
					{
						if ((fmcollectionItem.CPUType == CpuId.Invalid || fmcollectionItem.CPUType == FeatureMerger._cpuId) && fmcollectionItem.Critical)
						{
							string xmlFile = FMCollection.ResolveFMPath(fmcollectionItem.Path, FeatureMerger._inputFMDir);
							FeatureManifest featureManifest = new FeatureManifest();
							FeatureManifest.ValidateAndLoad(ref featureManifest, xmlFile, FeatureMerger._iuLogger);
							List<string> allPackageFilesList = featureManifest.GetAllPackageFilesList(FeatureMerger._fmFileList.SupportedLanguages, FeatureMerger._supportedLocales, FeatureMerger._fmFileList.SupportedResolutions, FeatureMerger._fmFileList.GetWowGuestCpuTypes(FeatureMerger._cpuId), FeatureMerger._buildTypeStr, FeatureMerger._cpuId.ToString(), FeatureMerger._msPackageRoot);
							List<string> source = (from pkg in allPackageFilesList
							where Path.GetExtension(pkg).Equals(PkgConstants.c_strPackageExtension, StringComparison.OrdinalIgnoreCase)
							select pkg).ToList<string>();
							allPackageFilesList.AddRange(from pkg in source
							select Path.ChangeExtension(pkg, PkgConstants.c_strCBSPackageExtension));
							FeatureMerger._nonMergedPackageList = FeatureMerger._nonMergedPackageList.Except(allPackageFilesList, StringComparer.OrdinalIgnoreCase).ToList<string>();
						}
					}
				}
				File.AppendAllLines(FeatureMerger._nonMergedPackageListFile, FeatureMerger._nonMergedPackageList);
			}
		}

		// Token: 0x06000004 RID: 4 RVA: 0x00002680 File Offset: 0x00000880
		private static bool CheckPackages(List<FMCollectionItem> fmItems)
		{
			bool flag = false;
			IULogger iulogger = new IULogger();
			iulogger.DebugLogger = null;
			iulogger.ErrorLogger = null;
			iulogger.InformationLogger = null;
			iulogger.WarningLogger = null;
			foreach (FMCollectionItem fmcollectionItem in fmItems)
			{
				FeatureManifest featureManifest = new FeatureManifest();
				try
				{
					string xmlFile = FMCollection.ResolveFMPath(fmcollectionItem.Path, FeatureMerger._inputFMDir);
					FeatureManifest.ValidateAndLoad(ref featureManifest, xmlFile, iulogger);
				}
				catch (Exception ex)
				{
					LogUtil.Error("Error: {0}", new object[]
					{
						ex
					});
					LogUtil.Error("Error: failed to load Feature Manifest: {0}", new object[]
					{
						fmcollectionItem.Path
					});
					flag = true;
					continue;
				}
				foreach (FeatureManifest.FMPkgInfo fmpkgInfo in featureManifest.GetAllPackagesByGroups(FeatureMerger._fmFileList.SupportedLanguages, FeatureMerger._supportedLocales, FeatureMerger._fmFileList.SupportedResolutions, FeatureMerger._fmFileList.GetWowGuestCpuTypes(FeatureMerger._cpuId), FeatureMerger._buildTypeStr, FeatureMerger._cpuId.ToString(), FeatureMerger._msPackageRoot))
				{
					if (!File.Exists(fmpkgInfo.PackagePath) && !File.Exists(Path.ChangeExtension(fmpkgInfo.PackagePath, PkgConstants.c_strPackageExtension)))
					{
						LogUtil.Error("Error: Missing package: {0}", new object[]
						{
							fmpkgInfo.PackagePath
						});
						flag = true;
					}
				}
			}
			return !flag;
		}

		// Token: 0x06000005 RID: 5 RVA: 0x00002844 File Offset: 0x00000A44
		private static List<FMCollectionItem> GetAllRelavantFMs(FMCollectionManifest manifest)
		{
			List<FMCollectionItem> list = new List<FMCollectionItem>();
			foreach (FMCollectionItem fmcollectionItem in manifest.FMs)
			{
				if (!fmcollectionItem.UserInstallable && (FeatureMerger._singleFM || fmcollectionItem.CPUType == CpuId.Invalid || fmcollectionItem.CPUType == FeatureMerger._cpuId) && (FeatureMerger._critical == FeatureMerger.CriticalFMProcessing.All || ((FeatureMerger._critical != FeatureMerger.CriticalFMProcessing.Yes || fmcollectionItem.Critical) && (FeatureMerger._critical != FeatureMerger.CriticalFMProcessing.No || !fmcollectionItem.Critical))))
				{
					list.Add(fmcollectionItem);
				}
			}
			return list;
		}

		// Token: 0x06000006 RID: 6 RVA: 0x000028EC File Offset: 0x00000AEC
		private static bool PrepSingleFM(string inputFile, string fmID, string ownerTypeStr, string ownerName, string languages, string resolutions)
		{
			FeatureMerger._fmFileList = new FMCollectionManifest();
			FeatureMerger._fmFileList.FMs = new List<FMCollectionItem>();
			if (string.IsNullOrEmpty(languages))
			{
				LogUtil.Error("A list of languages is required when specifying a single FM.");
				return false;
			}
			FeatureMerger._fmFileList.SupportedLanguages = languages.Split(new char[]
			{
				';'
			}, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
			if (FeatureMerger._fmFileList.SupportedLanguages.Count<string>() == 0)
			{
				LogUtil.Error("The list of languages cannot be empty when specifying a single FM.");
				return false;
			}
			if (string.IsNullOrEmpty(resolutions))
			{
				LogUtil.Error("A list of resolutions is required when specifying a single FM.");
			}
			FeatureMerger._fmFileList.SupportedResolutions = resolutions.Split(new char[]
			{
				';'
			}, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
			if (FeatureMerger._fmFileList.SupportedResolutions.Count<string>() == 0)
			{
				LogUtil.Error("The list of resolutions cannot be empty when specifying a single FM.");
				return false;
			}
			FMCollectionItem fmcollectionItem = new FMCollectionItem();
			fmcollectionItem.ID = fmID;
			fmcollectionItem.ownerType = OwnerType.OEM;
			if (!string.IsNullOrEmpty(ownerTypeStr))
			{
				bool ignoreCase = true;
				if (!Enum.TryParse<OwnerType>(ownerTypeStr, ignoreCase, out fmcollectionItem.ownerType))
				{
					LogUtil.Error("OwnerType is not recognized.  Must one of the following:\n\t{0}\n\t{1}\n\t{2}\n\t{3}", new object[]
					{
						OwnerType.OEM.ToString(),
						OwnerType.Microsoft.ToString(),
						OwnerType.MobileOperator.ToString(),
						OwnerType.SiliconVendor.ToString()
					});
					return false;
				}
			}
			if (fmcollectionItem.ownerType != OwnerType.Microsoft)
			{
				if (string.IsNullOrEmpty(ownerName))
				{
					LogUtil.Error("OwnerName is required when specifying a single FM.");
					return false;
				}
				fmcollectionItem.Owner = ownerName;
			}
			else
			{
				fmcollectionItem.Owner = OwnerType.Microsoft.ToString();
			}
			fmcollectionItem.ValidateAsMicrosoftPhoneFM = Path.GetFileName(inputFile).Equals(FeatureMerger.MicrosoftPhoneFMName, StringComparison.OrdinalIgnoreCase);
			if ((fmcollectionItem.ownerType != OwnerType.Microsoft || !fmcollectionItem.ValidateAsMicrosoftPhoneFM) && string.IsNullOrEmpty(fmcollectionItem.ID))
			{
				LogUtil.Error("FMID must be specified for Single FM InputFile.");
				return false;
			}
			fmcollectionItem.Path = inputFile;
			fmcollectionItem.CPUType = FeatureMerger._cpuId;
			fmcollectionItem.releaseType = FeatureMerger._releaseType;
			FeatureMerger._fmFileList.FMs.Add(fmcollectionItem);
			return true;
		}

		// Token: 0x06000007 RID: 7 RVA: 0x00002AEC File Offset: 0x00000CEC
		private static bool MergeOneFM(FMCollectionItem fmItem)
		{
			FeatureManifest featureManifest = new FeatureManifest();
			bool flag = FeatureMerger._doPlatformManifest && (fmItem.ownerType == OwnerType.Microsoft || !string.IsNullOrEmpty(FeatureMerger._binaryRoot));
			bool flag2 = false;
			if (flag)
			{
				FeatureMerger._pmMainOS = null;
				FeatureMerger._pmUpdateOS = null;
				FeatureMerger._pmEFIESP = null;
				flag2 = (FeatureMerger._uiItem != null && FeatureMerger._msPhoneItem != null && FeatureMerger._msPhoneItem == fmItem);
			}
			string fmPath = FMCollection.ResolveFMPath(fmItem.Path, FeatureMerger._inputFMDir);
			string text = Path.Combine(FeatureMerger._outputFMDir, Path.GetFileName(fmItem.Path));
			bool incremental = FeatureMerger._incremental;
			if (FeatureMerger._incremental)
			{
				if (!FileUtils.IsTargetUpToDate(fmPath, text))
				{
					incremental = false;
				}
				if (flag2 && !FileUtils.IsTargetUpToDate(FMCollection.ResolveFMPath(FeatureMerger._uiItem.Path, FeatureMerger._inputFMDir), text))
				{
					incremental = false;
				}
			}
			if (!fmItem.ValidateAsMicrosoftPhoneFM)
			{
				if (!FeatureMerger._singleFM && string.IsNullOrEmpty(fmItem.ID))
				{
					LogUtil.Error("FMID must be specified for entry '{0}' in the FMFileList InputFile.", new object[]
					{
						fmItem.Path
					});
					return false;
				}
				if (fmItem.ID.Contains('.'))
				{
					LogUtil.Error("FMID '{0} contains the invalid character '.'", new object[]
					{
						fmItem.ID
					});
					return false;
				}
			}
			FeatureManifest.ValidateAndLoad(ref featureManifest, fmPath, FeatureMerger._iuLogger);
			FeatureManifest newFM = new FeatureManifest();
			newFM.SourceFile = featureManifest.SourceFile;
			newFM.OwnerType = fmItem.ownerType;
			newFM.Owner = fmItem.Owner;
			newFM.ReleaseType = fmItem.releaseType;
			newFM.ID = fmItem.ID;
			newFM.OSVersion = FeatureMerger._version.ToString();
			if (featureManifest.Features != null)
			{
				newFM.Features = new FMFeatures();
				if (featureManifest.Features.Microsoft != null)
				{
					newFM.Features.Microsoft = new List<MSOptionalPkgFile>();
					newFM.Features.MSFeatureGroups = featureManifest.Features.MSFeatureGroups;
					if (featureManifest.Features.MSConditionalFeatures != null)
					{
						newFM.Features.MSConditionalFeatures = featureManifest.Features.MSConditionalFeatures;
						foreach (FMConditionalFeature fmconditionalFeature in newFM.Features.MSConditionalFeatures)
						{
							fmconditionalFeature.FMID = fmItem.ID;
						}
					}
				}
				if (featureManifest.Features.OEM != null)
				{
					newFM.Features.OEM = new List<OEMOptionalPkgFile>();
					newFM.Features.OEMFeatureGroups = featureManifest.Features.OEMFeatureGroups;
					if (featureManifest.Features.OEMConditionalFeatures != null)
					{
						newFM.Features.OEMConditionalFeatures = featureManifest.Features.OEMConditionalFeatures;
						foreach (FMConditionalFeature fmconditionalFeature2 in newFM.Features.OEMConditionalFeatures)
						{
							fmconditionalFeature2.FMID = fmItem.ID;
						}
					}
				}
			}
			List<FeatureManifest.FMPkgInfo> pkgs = featureManifest.GetAllPackagesByGroups(FeatureMerger._fmFileList.SupportedLanguages, FeatureMerger._supportedLocales, FeatureMerger._fmFileList.SupportedResolutions, FeatureMerger._fmFileList.GetWowGuestCpuTypes(FeatureMerger._cpuId), FeatureMerger._buildTypeStr, FeatureMerger._cpuId.ToString(), FeatureMerger._msPackageRoot);
			List<FeatureManifest.FMPkgInfo> list = (from pkg in pkgs
			where pkg.FMGroup == FeatureManifest.PackageGroups.OEMDEVICEPLATFORM || pkg.FMGroup == FeatureManifest.PackageGroups.DEVICE
			select pkg).ToList<FeatureManifest.FMPkgInfo>();
			pkgs = pkgs.Except(list).ToList<FeatureManifest.FMPkgInfo>();
			using (List<FeatureManifest.FMPkgInfo>.Enumerator enumerator2 = list.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					FeatureManifest.FMPkgInfo fmpkgInfo;
					if ((fmpkgInfo = enumerator2.Current).FMGroup == FeatureManifest.PackageGroups.OEMDEVICEPLATFORM)
					{
						fmpkgInfo.FMGroup = FeatureManifest.PackageGroups.DEVICE;
					}
					pkgs.Add(fmpkgInfo);
				}
			}
			Dictionary<string, FeatureManifest.PackageGroups> fmGroupLookup = new Dictionary<string, FeatureManifest.PackageGroups>(StringComparer.OrdinalIgnoreCase);
			Dictionary<string, string> fmGroupValueLookup = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			int fmGroupLookupAddedCount = 0;
			using (IEnumerator<string> enumerator3 = (from pkg in pkgs
			select pkg.FeatureID).Distinct(FeatureMerger.IgnoreCase).GetEnumerator())
			{
				while (enumerator3.MoveNext())
				{
					string feature = enumerator3.Current;
					FeatureManifest.FMPkgInfo fmpkgInfo2 = pkgs.FirstOrDefault((FeatureManifest.FMPkgInfo pkg) => pkg.FeatureID.Equals(feature, StringComparison.OrdinalIgnoreCase));
					fmGroupLookup.Add(feature, fmpkgInfo2.FMGroup);
					fmGroupValueLookup.Add(feature, fmpkgInfo2.GroupValue);
					int fmGroupLookupAddedCount2 = fmGroupLookupAddedCount;
					fmGroupLookupAddedCount = fmGroupLookupAddedCount2 + 1;
				}
			}
			if (featureManifest.KeyboardPackages.Count<KeyboardPkgFile>() > 0)
			{
				if (newFM.KeyboardPackages == null)
				{
					newFM.KeyboardPackages = featureManifest.KeyboardPackages;
				}
				else
				{
					newFM.KeyboardPackages.AddRange(featureManifest.KeyboardPackages);
				}
			}
			if (featureManifest.SpeechPackages.Count<SpeechPkgFile>() > 0)
			{
				if (newFM.SpeechPackages == null)
				{
					newFM.SpeechPackages = featureManifest.SpeechPackages;
				}
				else
				{
					newFM.SpeechPackages.AddRange(featureManifest.SpeechPackages);
				}
			}
			List<string> source = (from pkg in pkgs
			where pkg.FMGroup != FeatureManifest.PackageGroups.BOOTUI && pkg.FMGroup != FeatureManifest.PackageGroups.BOOTLOCALE && pkg.FMGroup != FeatureManifest.PackageGroups.SPEECH && pkg.FMGroup != FeatureManifest.PackageGroups.KEYBOARD
			select pkg.FeatureID).Distinct(FeatureMerger.IgnoreCase).ToList<string>();
			if (featureManifest.BootLocalePackageFile != null)
			{
				newFM.BootLocalePackageFile = featureManifest.BootLocalePackageFile;
			}
			if (featureManifest.BootUILanguagePackageFile != null)
			{
				newFM.BootUILanguagePackageFile = featureManifest.BootUILanguagePackageFile;
			}
			if (fmItem.ownerType == OwnerType.Microsoft)
			{
				FeatureMerger._nonMergedPackageList.AddRange(from pkg in pkgs
				where pkg.FMGroup == FeatureManifest.PackageGroups.BOOTUI || pkg.FMGroup == FeatureManifest.PackageGroups.BOOTLOCALE || pkg.FMGroup == FeatureManifest.PackageGroups.SPEECH || pkg.FMGroup == FeatureManifest.PackageGroups.KEYBOARD
				select pkg.PackagePath);
			}
			List<string> list2 = (from feature in source
			where feature.Contains('.') || !Regex.Match(feature, PkgConstants.c_strPackageStringPattern).Success
			select feature).ToList<string>();
			if (list2.Count<string>() > 0)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine("FeatureIDs for Merging only support digits/letters and underscore.");
				stringBuilder.AppendLine("Invalid characters found in the following FeatureIDs for FM " + fmItem.Path);
				foreach (string str in list2)
				{
					stringBuilder.AppendLine("\t" + str);
				}
				LogUtil.Error(stringBuilder.ToString());
				return false;
			}
			List<FeatureManifest.FMPkgInfo> source2 = (from pkg in pkgs
			where Path.GetExtension(pkg.PackagePath).Equals(PkgConstants.c_strCBSPackageExtension, StringComparison.OrdinalIgnoreCase)
			select pkg).ToList<FeatureManifest.FMPkgInfo>();
			if (FeatureMerger._convertToCBS)
			{
				List<FeatureManifest.FMPkgInfo> list3 = (from pkg in source2
				where !File.Exists(Path.ChangeExtension(pkg.PackagePath, PkgConstants.c_strPackageExtension))
				select pkg).ToList<FeatureManifest.FMPkgInfo>();
				pkgs = pkgs.Except(list3).ToList<FeatureManifest.FMPkgInfo>();
				FeatureMerger.AddCBSOnlyPackagesToFM(list3, featureManifest, ref newFM);
				if (list3.Any<FeatureManifest.FMPkgInfo>())
				{
					FeatureMerger._nonMergedPackageList.AddRange(from pkg in list3
					select Path.Combine(FeatureMerger._msPackageRoot, pkg.PackagePath));
				}
			}
			else if (source2.Any<FeatureManifest.FMPkgInfo>())
			{
				string format = "The following packages have an unsupported extension of '{0}': {1}";
				object[] array = new object[2];
				array[0] = PkgConstants.c_strCBSPackageExtension;
				array[1] = string.Join(" ", from pkg in source2
				select pkg.PackagePath);
				LogUtil.Error(format, array);
				return false;
			}
			IPkgInfo templatePkg = null;
			Parallel.ForEach<string>(source, delegate(string feature)
			{
				List<FeatureManifest.FMPkgInfo> list4 = (from pkg in pkgs
				where feature.Equals(pkg.FeatureID, StringComparison.OrdinalIgnoreCase)
				select pkg).ToList<FeatureManifest.FMPkgInfo>();
				string[] inputPkgs;
				if (FeatureMerger._convertToCBS)
				{
					inputPkgs = (from pkg in list4
					select FeatureMerger.ProcessVariables(Path.ChangeExtension(pkg.PackagePath, PkgConstants.c_strPackageExtension))).ToArray<string>();
				}
				else
				{
					inputPkgs = (from pkg in list4
					select FeatureMerger.ProcessVariables(pkg.PackagePath)).ToArray<string>();
				}
				string text2 = feature;
				if (!string.IsNullOrEmpty(fmItem.ID))
				{
					text2 = text2 + "." + fmItem.ID;
				}
				MergeResult[] source3 = Package.MergePackage(inputPkgs, FeatureMerger._outputPackageDir, text2, FeatureMerger._version, fmItem.Owner, fmItem.ownerType, fmItem.releaseType, FeatureMerger._cpuId, FeatureMerger._buildType, FeatureMerger._compress, incremental);
				if (source3.Count<MergeResult>() != 0)
				{
					List<MergeResult> list5 = source3.ToList<MergeResult>();
					if (FeatureMerger._convertToCBS)
					{
						for (int i = 0; i < list5.Count<MergeResult>(); i++)
						{
							list5[i].FilePath = Path.ChangeExtension(list5[i].FilePath, PkgConstants.c_strCBSPackageExtension);
						}
					}
					FeatureMerger._nonMergedPackageList.AddRange(from x in list5
					where x.IsNonMergedPackage
					select x.FilePath);
					if (templatePkg == null)
					{
						foreach (MergeResult mergeResult in list5)
						{
							if (mergeResult.PkgInfo != null && !mergeResult.PkgInfo.IsWow && mergeResult.FeatureIdentifierPackage)
							{
								templatePkg = mergeResult.PkgInfo;
								break;
							}
						}
					}
					if (!fmGroupLookup.ContainsKey(feature))
					{
						LogUtil.Error("ERROR : The Feature {0} could not be found in the LookupTable in FM '{1}'. Getting values from the package list", new object[]
						{
							feature,
							fmPath
						});
						foreach (string text3 in fmGroupLookup.Keys)
						{
							LogUtil.Error("\tExisting Table Entry: {0}", new object[]
							{
								text3
							});
						}
						LogUtil.Error("\tERROR : Number of Table Entries: {0}", new object[]
						{
							fmGroupLookup.Keys.Count
						});
						LogUtil.Error("\tERROR : Expected Number of Table Entries: {0}", new object[]
						{
							fmGroupLookupAddedCount
						});
						throw new ImageCommonException(string.Concat(new string[]
						{
							"Error : FeatureMerger!MergeOneFM: Feature ",
							feature,
							" in FM '",
							fmPath,
							"' could not be found in LookupTable"
						}));
					}
					FeatureManifest.PackageGroups fmGroup = fmGroupLookup[feature];
					string fmGroupValue = fmGroupValueLookup[feature];
					FeatureManifest newFM = newFM;
					lock (newFM)
					{
						newFM.AddPackagesFromMergeResult(list4, fmGroup, fmGroupValue, list5, FeatureMerger._fmFileList.SupportedLanguages, FeatureMerger._fmFileList.SupportedResolutions, FeatureMerger._outputPackageDir, FeatureMerger._packagePathReplacement);
					}
				}
			});
			if (templatePkg == null)
			{
				LogUtil.Warning("The FM '{0}' contains no applicable packages.  The FM will not be saved to the output directory.", new object[]
				{
					fmItem.Path
				});
				return true;
			}
			if (FeatureMerger._convertToCBS)
			{
				List<FeatureManifest.FMPkgInfo> allPackagesByGroups = newFM.GetAllPackagesByGroups(FeatureMerger._fmFileList.SupportedLanguages, FeatureMerger._fmFileList.SupportedLocales, FeatureMerger._fmFileList.SupportedResolutions, FeatureMerger._fmFileList.GetWowGuestCpuTypes(FeatureMerger._cpuId), FeatureMerger._buildTypeStr, FeatureMerger._cpuId.ToString(), FeatureMerger._msPackageRoot);
				FeatureMerger.GenerateCBSPackages((from pkg in (from fip in allPackagesByGroups
				where !fip.FeatureIdentifierPackage
				select fip into pkg
				select Path.ChangeExtension(pkg.PackagePath, PkgConstants.c_strPackageExtension)).ToList<string>()
				where File.Exists(pkg) && !FileUtils.IsTargetUpToDate(pkg, Path.ChangeExtension(pkg, PkgConstants.c_strCBSPackageExtension))
				select pkg).ToList<string>());
				FeatureMerger.GenerateCBSPackagesWithTestSigning((from pkg in (from fip in allPackagesByGroups
				where fip.FeatureIdentifierPackage
				select fip into pkg
				select Path.ChangeExtension(pkg.PackagePath, PkgConstants.c_strPackageExtension)).ToList<string>()
				where File.Exists(pkg) && !FileUtils.IsTargetUpToDate(pkg, Path.ChangeExtension(pkg, PkgConstants.c_strCBSPackageExtension))
				select pkg).ToList<string>());
				if (flag && FeatureMerger._uiItem != fmItem)
				{
					foreach (string path in from pkg in allPackagesByGroups
					select Path.ChangeExtension(pkg.PackagePath, PkgConstants.c_strPackageExtension))
					{
						FeatureMerger.AddPackageToPlatformManifest(Package.LoadFromCab(Path.ChangeExtension(path, PkgConstants.c_strCBSPackageExtension)), fmItem);
					}
				}
			}
			if (flag2)
			{
				FeatureMerger.ProcessUserInstallableFM(FeatureMerger._uiItem, newFM, templatePkg);
			}
			FeatureMerger.FinalizeFeatureManifest(newFM, text, fmItem, templatePkg);
			if (FeatureMerger._convertToCBS)
			{
				FeatureMerger.SetFIPInfo(newFM.GetAllPackagesByGroups(FeatureMerger._fmFileList.SupportedLanguages, FeatureMerger._fmFileList.SupportedLocales, FeatureMerger._fmFileList.SupportedResolutions, FeatureMerger._fmFileList.GetWowGuestCpuTypes(FeatureMerger._cpuId), FeatureMerger._buildTypeStr, FeatureMerger._cpuId.ToString(), FeatureMerger._msPackageRoot), fmItem);
			}
			return true;
		}

		// Token: 0x06000008 RID: 8 RVA: 0x000036AC File Offset: 0x000018AC
		private static void SetFIPInfo(List<FeatureManifest.FMPkgInfo> pkgs, FMCollectionItem fmItem)
		{
			List<FeatureManifest.FMPkgInfo> packages = new List<FeatureManifest.FMPkgInfo>(pkgs);
			foreach (FeatureManifest.FMPkgInfo fmpkgInfo in from pkg in packages
			where pkg.FMGroup == FeatureManifest.PackageGroups.OEMDEVICEPLATFORM
			select pkg)
			{
				fmpkgInfo.FMGroup = FeatureManifest.PackageGroups.DEVICE;
			}
			foreach (FeatureManifest.FMPkgInfo fmpkgInfo2 in from pkg in packages
			where pkg.FMGroup == FeatureManifest.PackageGroups.DEVICELAYOUT
			select pkg)
			{
				fmpkgInfo2.FMGroup = FeatureManifest.PackageGroups.SOC;
			}
			Parallel.ForEach<string>((from pkg in packages
			select pkg.FeatureID).Distinct(FeatureMerger.IgnoreCase).ToList<string>(), delegate(string feature)
			{
				List<FeatureManifest.FMPkgInfo> list = (from pkg in packages
				where feature.Equals(pkg.FeatureID, StringComparison.OrdinalIgnoreCase)
				select pkg).ToList<FeatureManifest.FMPkgInfo>();
				FeatureManifest.FMPkgInfo fmpkgInfo3 = list.FirstOrDefault((FeatureManifest.FMPkgInfo pkg) => pkg.FeatureIdentifierPackage);
				if (fmpkgInfo3 == null)
				{
					fmpkgInfo3 = list.FirstOrDefault((FeatureManifest.FMPkgInfo pkg) => !string.IsNullOrEmpty(pkg.Partition) && pkg.Partition.Equals(PkgConstants.c_strMainOsPartition, StringComparison.OrdinalIgnoreCase));
				}
				PrepCBSFeature.Prep(fmpkgInfo3.PackagePath, fmItem.ID, feature, fmItem.ownerType.ToString(), FeatureMerger._cpuId.ToString(), list, true);
			});
		}

		// Token: 0x06000009 RID: 9 RVA: 0x000037E0 File Offset: 0x000019E0
		private static void CheckForUserInstallableFM()
		{
			if (FeatureMerger._fmFileList == null)
			{
				return;
			}
			FeatureMerger._uiItem = FeatureMerger._fmFileList.FMs.FirstOrDefault((FMCollectionItem fmItem) => fmItem.UserInstallable);
			FeatureMerger._msPhoneItem = FeatureMerger._fmFileList.FMs.FirstOrDefault((FMCollectionItem fmItem) => Path.GetFileName(fmItem.Path).Equals("MicrosoftPhoneFM.xml", StringComparison.OrdinalIgnoreCase));
		}

		// Token: 0x0600000A RID: 10 RVA: 0x0000385C File Offset: 0x00001A5C
		private static void ProcessUserInstallableFM(FMCollectionItem userInstallableFMItem, FeatureManifest newMSPhoneFM, IPkgInfo templatePkgInfo)
		{
			PlatformManifestGen platformManifestGen = new PlatformManifestGen(userInstallableFMItem.MicrosoftFMGUID, FeatureMerger._buildBranchInfo, FeatureMerger._signInfoPath, userInstallableFMItem.releaseType, FeatureMerger._iuLogger);
			FeatureManifest featureManifest = new FeatureManifest();
			string xmlFile = FMCollection.ResolveFMPath(userInstallableFMItem.Path, FeatureMerger._inputFMDir);
			FeatureManifest.ValidateAndLoad(ref featureManifest, xmlFile, FeatureMerger._iuLogger);
			List<string> list = featureManifest.GetAllPackageFilesList(FeatureMerger._fmFileList.SupportedLanguages, FeatureMerger._fmFileList.SupportedResolutions, FeatureMerger._fmFileList.SupportedLocales, FeatureMerger._fmFileList.GetWowGuestCpuTypes(FeatureMerger._cpuId), FeatureMerger._buildTypeStr, FeatureMerger._cpuId.ToString(), FeatureMerger._msPackageRoot).ToList<string>();
			if (userInstallableFMItem.ownerType == OwnerType.Microsoft)
			{
				FeatureMerger._nonMergedPackageList.AddRange(list);
			}
			foreach (string text in list)
			{
				if (!File.Exists(text))
				{
					platformManifestGen.ErrorMessages.Append("Error: FeatureMerger!ProcessUserInstallableFM: The package file '" + text + "' does not exist.");
				}
				else
				{
					IPkgInfo package = Package.LoadFromCab(text);
					platformManifestGen.AddPackage(package);
				}
			}
			if (platformManifestGen.ErrorsFound)
			{
				if (platformManifestGen.ErrorsWithSignInfos)
				{
					FeatureMerger._iuLogger.LogError("Error: " + PlatformManifestGen.SignInfoFailureInstructions, new object[0]);
				}
				throw new ImageCommonException("Error: FeatureMerger!ProcessUserInstallableFM: Failed to create Platform Manfiests for Microsoft: " + Environment.NewLine + platformManifestGen.ErrorMessages.ToString());
			}
			FeatureMerger.CreatePlatformManifestPackage(platformManifestGen, PkgConstants.c_strMainOsPartition, newMSPhoneFM, templatePkgInfo, PlatformManifestGen.c_strPlatformManifestMainOSDevicePath, userInstallableFMItem.Path);
			FeatureMerger.SetFIPInfo(featureManifest.GetAllPackagesByGroups(FeatureMerger._fmFileList.SupportedLanguages, FeatureMerger._fmFileList.SupportedResolutions, FeatureMerger._fmFileList.SupportedLocales, FeatureMerger._fmFileList.GetWowGuestCpuTypes(FeatureMerger._cpuId), FeatureMerger._buildTypeStr, FeatureMerger._cpuId.ToString(), FeatureMerger._msPackageRoot), userInstallableFMItem);
		}

		// Token: 0x0600000B RID: 11 RVA: 0x00003A44 File Offset: 0x00001C44
		private static void AddPackageToPlatformManifest(IPkgInfo package, FMCollectionItem fmItem)
		{
			if (package.Partition.Equals(PkgConstants.c_strMainOsPartition, StringComparison.OrdinalIgnoreCase) || package.Partition.Equals(PkgConstants.c_strDataPartition, StringComparison.OrdinalIgnoreCase))
			{
				if (FeatureMerger._pmMainOS == null)
				{
					FeatureMerger._pmMainOS = new PlatformManifestGen(fmItem.MicrosoftFMGUID, FeatureMerger._buildBranchInfo, FeatureMerger._signInfoPath, fmItem.releaseType, FeatureMerger._iuLogger);
				}
				FeatureMerger._pmMainOS.AddPackage(package);
				return;
			}
			if (package.Partition.Equals(PkgConstants.c_strUpdateOsPartition, StringComparison.OrdinalIgnoreCase))
			{
				if (package.OwnerType == OwnerType.Microsoft)
				{
					if (FeatureMerger._pmUpdateOS == null)
					{
						FeatureMerger._pmUpdateOS = new PlatformManifestGen(fmItem.MicrosoftFMGUID, FeatureMerger._buildBranchInfo, FeatureMerger._signInfoPath, fmItem.releaseType, FeatureMerger._iuLogger);
					}
					FeatureMerger._pmUpdateOS.AddPackage(package);
					return;
				}
			}
			else if (package.Partition.Equals(PkgConstants.c_strEfiPartition, StringComparison.OrdinalIgnoreCase))
			{
				if (FeatureMerger._pmEFIESP == null)
				{
					FeatureMerger._pmEFIESP = new PlatformManifestGen(fmItem.MicrosoftFMGUID, FeatureMerger._buildBranchInfo, FeatureMerger._signInfoPath, fmItem.releaseType, FeatureMerger._iuLogger);
				}
				FeatureMerger._pmEFIESP.AddPackage(package);
			}
		}

		// Token: 0x0600000C RID: 12 RVA: 0x00003B50 File Offset: 0x00001D50
		private static void GenerateCBSPackage(string packageFile)
		{
			FeatureMerger.GenerateCBSPackages(new List<string>
			{
				packageFile
			});
		}

		// Token: 0x0600000D RID: 13 RVA: 0x00003B64 File Offset: 0x00001D64
		private static void GenerateCBSPackages(List<string> packageList)
		{
			if (packageList.Count<string>() == 0)
			{
				return;
			}
			FileUtils.GetTempDirectory();
			PkgConvertDSM.CONVERTDSM_PARAMETERS_FLAGS flags = PkgConvertDSM.CONVERTDSM_PARAMETERS_FLAGS.CONVERTDSM_PARAMETERS_FLAGS_MAKE_CAB | PkgConvertDSM.CONVERTDSM_PARAMETERS_FLAGS.CONVERTDSM_PARAMETERS_FLAGS_SIGN_OUTPUT | PkgConvertDSM.CONVERTDSM_PARAMETERS_FLAGS.CONVERTDSM_PARAMETERS_FLAGS_SKIP_POLICY | PkgConvertDSM.CONVERTDSM_PARAMETERS_FLAGS.CONVERTDSM_PARAMETERS_FLAGS_USE_FILENAME_AS_NAME | PkgConvertDSM.CONVERTDSM_PARAMETERS_FLAGS.CONVERTDSM_PARAMETERS_FLAGS_OUTPUT_NEXT_TO_INPUT;
			FeatureMerger.GenerateCBSPackages(packageList, flags);
		}

		// Token: 0x0600000E RID: 14 RVA: 0x00003B90 File Offset: 0x00001D90
		private static void GenerateCBSPackagesWithTestSigning(List<string> packageList)
		{
			if (packageList.Count<string>() == 0)
			{
				return;
			}
			FileUtils.GetTempDirectory();
			PkgConvertDSM.CONVERTDSM_PARAMETERS_FLAGS flags = PkgConvertDSM.CONVERTDSM_PARAMETERS_FLAGS.CONVERTDSM_PARAMETERS_FLAGS_MAKE_CAB | PkgConvertDSM.CONVERTDSM_PARAMETERS_FLAGS.CONVERTDSM_PARAMETERS_FLAGS_SIGN_OUTPUT | PkgConvertDSM.CONVERTDSM_PARAMETERS_FLAGS.CONVERTDSM_PARAMETERS_FLAGS_SKIP_POLICY | PkgConvertDSM.CONVERTDSM_PARAMETERS_FLAGS.CONVERTDSM_PARAMETERS_FLAGS_USE_FILENAME_AS_NAME | PkgConvertDSM.CONVERTDSM_PARAMETERS_FLAGS.CONVERTDSM_PARAMETERS_FLAGS_OUTPUT_NEXT_TO_INPUT | PkgConvertDSM.CONVERTDSM_PARAMETERS_FLAGS.CONVERTDSM_PARAMETERS_FLAGS_SIGN_TESTONLY;
			FeatureMerger.GenerateCBSPackages(packageList, flags);
		}

		// Token: 0x0600000F RID: 15 RVA: 0x00003BBC File Offset: 0x00001DBC
		private static void GenerateCBSPackages(List<string> packageList, PkgConvertDSM.CONVERTDSM_PARAMETERS_FLAGS flags)
		{
			if (packageList.Count<string>() == 0)
			{
				return;
			}
			FileUtils.GetTempDirectory();
			Parallel.ForEach<string>(packageList.Distinct(StringComparer.OrdinalIgnoreCase).ToList<string>(), delegate(string spkgFile)
			{
				List<string> list = new List<string>();
				list.Add(spkgFile);
				PkgConvertDSM.ConvertPackagesToCBS(flags, list, null);
			});
		}

		// Token: 0x06000010 RID: 16 RVA: 0x00003C08 File Offset: 0x00001E08
		private static void AddCBSOnlyPackagesToFM(List<FeatureManifest.FMPkgInfo> list, FeatureManifest orgFM, ref FeatureManifest newFM)
		{
			if (!list.Any<FeatureManifest.FMPkgInfo>())
			{
				return;
			}
			List<PkgFile> list2 = new List<PkgFile>();
			using (List<FeatureManifest.FMPkgInfo>.Enumerator enumerator = (from pkg in list
			where string.IsNullOrWhiteSpace(pkg.Language) && string.IsNullOrWhiteSpace(pkg.Resolution)
			select pkg).ToList<FeatureManifest.FMPkgInfo>().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					FeatureManifest.FMPkgInfo fmPkgInfo = enumerator.Current;
					PkgFile pkgEntry = orgFM.AllPackages.FirstOrDefault((PkgFile pkg) => pkg.ID.Equals(fmPkgInfo.ID) && pkg.FMGroup == fmPkgInfo.FMGroup && (pkg.FMGroup == FeatureManifest.PackageGroups.BASE || pkg.FMGroup == FeatureManifest.PackageGroups.KEYBOARD || pkg.FMGroup == FeatureManifest.PackageGroups.SPEECH || pkg.GroupValues.Contains(fmPkgInfo.GroupValue, StringComparer.OrdinalIgnoreCase)) && (pkg.CPUIds == null || pkg.CPUIds.Contains(FeatureMerger._cpuId)));
					if (!list2.Any((PkgFile pkg) => pkg == pkgEntry))
					{
						if (pkgEntry == null)
						{
							pkgEntry = orgFM.AllPackages.FirstOrDefault((PkgFile pkg) => pkg.ID.Equals(fmPkgInfo.ID) && (pkg.FMGroup == FeatureManifest.PackageGroups.BASE || pkg.FMGroup == FeatureManifest.PackageGroups.KEYBOARD || pkg.FMGroup == FeatureManifest.PackageGroups.SPEECH || pkg.GroupValues.Contains(fmPkgInfo.GroupValue, StringComparer.OrdinalIgnoreCase)) && (pkg.CPUIds == null || pkg.CPUIds.Contains(FeatureMerger._cpuId)));
						}
						newFM.AddPkgFile(pkgEntry);
						list2.Add(pkgEntry);
					}
				}
			}
		}

		// Token: 0x06000011 RID: 17 RVA: 0x00003D10 File Offset: 0x00001F10
		private static void FinalizeFeatureManifest(FeatureManifest fm, string newFMFilePath, FMCollectionItem item, IPkgInfo templatePackage)
		{
			bool flag;
			if (fm.BasePackages != null && fm.BasePackages.Count<PkgFile>() != 0)
			{
				flag = !fm.BasePackages.Any((PkgFile pkg) => pkg.FeatureIdentifierPackage);
			}
			else
			{
				flag = true;
			}
			bool featureIdentifierPackage = flag;
			if (FeatureMerger._doPlatformManifest && (item.ownerType == OwnerType.Microsoft || !string.IsNullOrEmpty(FeatureMerger._binaryRoot)))
			{
				FeatureMerger.CreatePlatformManifestPackages(fm, templatePackage);
			}
			if (fm.OwnerType == OwnerType.Microsoft)
			{
				fm.BuildID = FeatureMerger._buildID.ToString();
				fm.BuildInfo = FeatureMerger._buildInfo;
			}
			using (IPkgBuilder pkgBuilder = Package.Create())
			{
				pkgBuilder.Partition = "MainOS";
				pkgBuilder.Owner = templatePackage.Owner;
				pkgBuilder.Platform = templatePackage.Platform;
				pkgBuilder.ReleaseType = templatePackage.ReleaseType;
				if (item.ValidateAsMicrosoftPhoneFM)
				{
					pkgBuilder.Component = "PhoneFM";
				}
				else
				{
					pkgBuilder.Component = Path.GetFileNameWithoutExtension(item.Path);
				}
				pkgBuilder.SubComponent = null;
				pkgBuilder.BuildString = templatePackage.BuildString;
				pkgBuilder.OwnerType = templatePackage.OwnerType;
				pkgBuilder.GroupingKey = (string.IsNullOrEmpty(item.ID) ? "" : (item.ID + "."));
				pkgBuilder.GroupingKey = FeatureManifest.PackageGroups.BASE.ToString();
				pkgBuilder.Version = templatePackage.Version;
				pkgBuilder.BuildType = templatePackage.BuildType;
				pkgBuilder.CpuType = templatePackage.CpuType;
				PkgFile pkgFile = new PkgFile();
				if (fm.BasePackages == null)
				{
					fm.BasePackages = new List<PkgFile>();
				}
				pkgFile.FeatureIdentifierPackage = featureIdentifierPackage;
				if (string.IsNullOrEmpty(FeatureMerger._packagePathReplacement))
				{
					pkgFile.Directory = FeatureMerger._outputPackageDir;
				}
				else
				{
					pkgFile.Directory = FeatureMerger._packagePathReplacement;
				}
				pkgFile.Name = pkgBuilder.Name + (FeatureMerger._convertToCBS ? PkgConstants.c_strCBSPackageExtension : PkgConstants.c_strPackageExtension);
				pkgFile.ID = pkgBuilder.Name;
				pkgFile.Partition = pkgBuilder.Partition;
				fm.BasePackages.Add(pkgFile);
				string fileName = Path.GetFileName(newFMFilePath);
				fm.WriteToFile(newFMFilePath);
				string text = "\\";
				if (item.ownerType == OwnerType.Microsoft)
				{
					text = text + DevicePaths.MSFMPath + "\\" + fileName;
				}
				else
				{
					text = text + DevicePaths.OEMFMPath + "\\" + fileName;
				}
				pkgBuilder.AddFile(FileType.Regular, newFMFilePath, text, FileAttributes.Normal, null, "None");
				string text2 = Path.Combine(FeatureMerger._outputPackageDir, pkgBuilder.Name + PkgConstants.c_strPackageExtension);
				pkgBuilder.SaveCab(text2);
				if (FeatureMerger._convertToCBS)
				{
					FeatureMerger.GenerateCBSPackage(text2);
				}
			}
		}

		// Token: 0x06000012 RID: 18 RVA: 0x00003FD4 File Offset: 0x000021D4
		private static void CreatePlatformManifestPackages(FeatureManifest fm, IPkgInfo templatePackage)
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = false;
			if (FeatureMerger._pmMainOS != null)
			{
				if (FeatureMerger._pmMainOS.ErrorsFound)
				{
					stringBuilder.AppendLine(FeatureMerger._pmMainOS.ErrorMessages.ToString());
					flag = (FeatureMerger._pmMainOS.ErrorsWithSignInfos || flag);
				}
				FeatureMerger.CreatePlatformManifestPackage(FeatureMerger._pmMainOS, PkgConstants.c_strMainOsPartition, fm, templatePackage, PlatformManifestGen.c_strPlatformManifestMainOSDevicePath);
				FeatureMerger._pmMainOS = null;
			}
			if (FeatureMerger._pmUpdateOS != null)
			{
				if (FeatureMerger._pmUpdateOS.ErrorsFound)
				{
					stringBuilder.AppendLine(FeatureMerger._pmUpdateOS.ErrorMessages.ToString());
					flag = (FeatureMerger._pmUpdateOS.ErrorsWithSignInfos || flag);
				}
				FeatureMerger.CreatePlatformManifestPackage(FeatureMerger._pmUpdateOS, PkgConstants.c_strUpdateOsPartition, fm, templatePackage, PlatformManifestGen.c_strPlatformManifestMainOSDevicePath);
				FeatureMerger._pmUpdateOS = null;
			}
			if (FeatureMerger._pmEFIESP != null)
			{
				if (FeatureMerger._pmEFIESP.ErrorsFound)
				{
					stringBuilder.AppendLine(FeatureMerger._pmEFIESP.ErrorMessages.ToString());
					flag = (FeatureMerger._pmEFIESP.ErrorsWithSignInfos || flag);
				}
				FeatureMerger.CreatePlatformManifestPackage(FeatureMerger._pmEFIESP, PkgConstants.c_strEfiPartition, fm, templatePackage, PlatformManifestGen.c_strPlatformManifestEFIESPDevicePath);
				FeatureMerger._pmEFIESP = null;
			}
			if (stringBuilder.Length > 0)
			{
				if (templatePackage.OwnerType == OwnerType.Microsoft && (FeatureMerger._cpuId == CpuId.X86 || FeatureMerger._cpuId == CpuId.ARM))
				{
					if (flag)
					{
						FeatureMerger._iuLogger.LogError("Error: " + PlatformManifestGen.SignInfoFailureInstructions, new object[0]);
					}
					throw new ImageCommonException("Error: FeatureMerger!CreatePlatformManifestPackages: Failed to create Platform Manfiests for Microsoft: " + Environment.NewLine + stringBuilder.ToString());
				}
				FeatureMerger._iuLogger.LogWarning("Warning: FeatureMerger!CreatePlatformManifestPackages: Failed to create Platform Manfiests: " + Environment.NewLine + stringBuilder.ToString(), new object[0]);
				if (flag)
				{
					FeatureMerger._iuLogger.LogWarning("Warning: " + PlatformManifestGen.SignInfoFailureInstructions, new object[0]);
				}
			}
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00004194 File Offset: 0x00002394
		private static void CreatePlatformManifestPackage(PlatformManifestGen pm, string partition, FeatureManifest fm, IPkgInfo templatePackage, string DeviceBasePath)
		{
			FeatureMerger.CreatePlatformManifestPackage(pm, partition, fm, templatePackage, DeviceBasePath, fm.SourceFile);
		}

		// Token: 0x06000014 RID: 20 RVA: 0x000041A8 File Offset: 0x000023A8
		private static void CreatePlatformManifestPackage(PlatformManifestGen pm, string partition, FeatureManifest fm, IPkgInfo templatePackage, string DeviceBasePath, string fmName)
		{
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fmName);
			string text = Path.Combine(FeatureMerger._pmPath, fileNameWithoutExtension + "." + partition + PlatformManifestGen.c_strPlatformManifestExtension);
			pm.WriteToFile(text);
			if (pm.ErrorsFound)
			{
				File.WriteAllText(text + ".err", pm.ErrorMessages.ToString());
			}
			using (IPkgBuilder pkgBuilder = Package.Create())
			{
				pkgBuilder.Partition = partition;
				pkgBuilder.Owner = templatePackage.Owner;
				pkgBuilder.Platform = templatePackage.Platform;
				pkgBuilder.ReleaseType = templatePackage.ReleaseType;
				pkgBuilder.Component = Path.GetFileNameWithoutExtension(fileNameWithoutExtension);
				pkgBuilder.SubComponent = PlatformManifestGen.c_strPlatformManifestSubcomponent + "." + partition;
				pkgBuilder.BuildString = templatePackage.BuildString;
				pkgBuilder.OwnerType = templatePackage.OwnerType;
				pkgBuilder.Version = templatePackage.Version;
				pkgBuilder.BuildType = templatePackage.BuildType;
				pkgBuilder.CpuType = templatePackage.CpuType;
				PkgFile pkgFile = new PkgFile();
				if (fm.BasePackages == null || fm.BasePackages.Count<PkgFile>() == 0)
				{
					fm.BasePackages = new List<PkgFile>();
				}
				pkgFile.Directory = (string.IsNullOrEmpty(FeatureMerger._packagePathReplacement) ? FeatureMerger._outputPackageDir : FeatureMerger._packagePathReplacement);
				pkgFile.Name = pkgBuilder.Name + (FeatureMerger._convertToCBS ? PkgConstants.c_strCBSPackageExtension : PkgConstants.c_strPackageExtension);
				pkgFile.ID = pkgBuilder.Name;
				pkgFile.Partition = pkgBuilder.Partition;
				fm.BasePackages.Add(pkgFile);
				string destination = DeviceBasePath + fileNameWithoutExtension + PlatformManifestGen.c_strPlatformManifestExtension;
				pkgBuilder.AddFile(FileType.Regular, text, destination, FileAttributes.Normal, null, "None");
				string text2 = Path.Combine(FeatureMerger._outputPackageDir, pkgBuilder.Name + PkgConstants.c_strPackageExtension);
				pkgBuilder.SaveCab(text2);
				FeatureMerger.GenerateCBSPackage(text2);
			}
		}

		// Token: 0x06000015 RID: 21 RVA: 0x00004398 File Offset: 0x00002598
		private static bool ParseVariables(string variablesStr)
		{
			if (variablesStr != null)
			{
				foreach (string text in variablesStr.Split(new char[]
				{
					';'
				}, StringSplitOptions.RemoveEmptyEntries).ToList<string>())
				{
					List<string> list = text.Split(new char[]
					{
						'='
					}, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
					if (list.Count != 2)
					{
						LogUtil.Error("The variable list contains the '{0}' entry which is not in the proper variable=value format", new object[]
						{
							text
						});
						return false;
					}
					FeatureMerger._variableLookup[list[0]] = list[1];
				}
			}
			FeatureMerger._binaryRoot = Environment.GetEnvironmentVariable("BINARY_ROOT");
			if (string.IsNullOrEmpty(FeatureMerger._binaryRoot))
			{
				FeatureMerger._binaryRoot = Environment.GetEnvironmentVariable("_NTTREE");
				if (!string.IsNullOrEmpty(FeatureMerger._binaryRoot))
				{
					FeatureMerger._binaryRoot += ".WM.MC";
				}
			}
			if (!string.IsNullOrEmpty(FeatureMerger._binaryRoot))
			{
				FeatureMerger._doPlatformManifest = true;
				FeatureMerger._signInfoPath = Path.Combine(FeatureMerger._binaryRoot, PlatformManifestGen.c_strSignInfoDir);
				FeatureMerger._pmPath = LongPath.GetFullPath(Path.Combine(FeatureMerger._binaryRoot, FeatureMerger.PlatformManifestRelativeDir));
				if (!LongPathDirectory.Exists(FeatureMerger._pmPath))
				{
					LongPathDirectory.CreateDirectory(FeatureMerger._pmPath);
				}
				FeatureMerger._buildBranchInfo = Environment.ExpandEnvironmentVariables("%_RELEASELABEL%.%_NTRAZZLEBUILDNUMBER%.%_QFELEVEL%.%_BUILDTIME%");
			}
			string text2 = null;
			if (FeatureMerger._variableLookup.ContainsKey("_cpuType"))
			{
				text2 = FeatureMerger._variableLookup["_cpuType"];
			}
			if (string.IsNullOrEmpty(text2))
			{
				text2 = Environment.GetEnvironmentVariable("_BUILDARCH");
			}
			if (!string.IsNullOrEmpty(text2))
			{
				bool ignoreCase = true;
				if (!Enum.TryParse<CpuId>(text2, ignoreCase, out FeatureMerger._cpuId))
				{
					LogUtil.Error("The variable cputype '{0}' is not recognized as a valid value", new object[]
					{
						text2
					});
					return false;
				}
			}
			if (FeatureMerger._variableLookup.ContainsKey("buildtype"))
			{
				FeatureMerger._buildTypeStr = FeatureMerger._variableLookup["buildtype"];
			}
			if (string.IsNullOrEmpty(FeatureMerger._buildTypeStr))
			{
				FeatureMerger._buildTypeStr = Environment.GetEnvironmentVariable("buildtype");
				if (string.IsNullOrEmpty(FeatureMerger._buildTypeStr))
				{
					FeatureMerger._buildTypeStr = Environment.GetEnvironmentVariable("_buildtype");
				}
			}
			if (!string.IsNullOrEmpty(FeatureMerger._buildTypeStr))
			{
				if (FeatureMerger._buildTypeStr.Equals("fre", StringComparison.OrdinalIgnoreCase))
				{
					FeatureMerger._buildType = BuildType.Retail;
				}
				else if (FeatureMerger._buildTypeStr.Equals("chk", StringComparison.OrdinalIgnoreCase))
				{
					FeatureMerger._buildType = BuildType.Checked;
				}
			}
			if (FeatureMerger._variableLookup.ContainsKey("mspackageroot"))
			{
				FeatureMerger._msPackageRoot = FeatureMerger._variableLookup["mspackageroot"];
			}
			else if (!string.IsNullOrEmpty(FeatureMerger._binaryRoot))
			{
				FeatureMerger._msPackageRoot = Path.Combine(FeatureMerger._binaryRoot, "Prebuilt");
			}
			string text3 = string.Empty;
			if (FeatureMerger._variableLookup.ContainsKey("releasetype"))
			{
				text3 = FeatureMerger._variableLookup["releasetype"];
			}
			if (!string.IsNullOrEmpty(text3))
			{
				bool ignoreCase2 = true;
				if (!Enum.TryParse<ReleaseType>(text3, ignoreCase2, out FeatureMerger._releaseType))
				{
					LogUtil.Error("The variable releaseType '{0}' is not recognized as a valid value", new object[]
					{
						text3
					});
					return false;
				}
			}
			FeatureMerger._nonMergedPackageListFile = Path.Combine(FeatureMerger._msPackageRoot, FeatureMerger.NonMergedPacakgeList);
			FeatureMerger._nonMergedPackageListFile = FeatureMerger._nonMergedPackageListFile.Replace("$(CpuType)", text2);
			FeatureMerger._nonMergedPackageListFile = FeatureMerger._nonMergedPackageListFile.Replace("$(BuildType)", FeatureMerger._buildTypeStr);
			return true;
		}

		// Token: 0x06000016 RID: 22 RVA: 0x000046E8 File Offset: 0x000028E8
		private static string ProcessVariables(string input)
		{
			string text = input.ToLower(CultureInfo.InvariantCulture);
			foreach (string text2 in FeatureMerger._variableLookup.Keys)
			{
				text = text.Replace("$(" + text2 + ")", FeatureMerger._variableLookup[text2].ToLower(CultureInfo.InvariantCulture));
				text = text.Replace("%" + text2 + "%", FeatureMerger._variableLookup[text2].ToLower(CultureInfo.InvariantCulture));
			}
			return text;
		}

		// Token: 0x06000017 RID: 23 RVA: 0x000047A0 File Offset: 0x000029A0
		private static bool SetCmdLineParams()
		{
			try
			{
				FeatureMerger._commandLineParser = new CommandLineParser();
				FeatureMerger._commandLineParser.SetRequiredParameterString("InputFile", "Specify the input file. Can be either a FMFileList XML file or a single Feature Manifest XML file.");
				FeatureMerger._commandLineParser.SetRequiredParameterString("OutputPackageDir", "Directory where merged packages will be placed.");
				FeatureMerger._commandLineParser.SetRequiredParameterString("OutputPackageVersion", "Version string in the form of <major>.<minor>.<qfe>.<build>");
				FeatureMerger._commandLineParser.SetRequiredParameterString("OutputFMDir", "Directory where Feature Manifests will be place.  Feature Manifests are generated with the same file name as the original Feature Manifest.");
				FeatureMerger._commandLineParser.SetOptionalSwitchString("InputFMDir", "Directory where source Feature Manifests can be found. Required with FMFileList XML file.");
				FeatureMerger._commandLineParser.SetOptionalSwitchString("Critical", "Reserved");
				FeatureMerger._commandLineParser.SetOptionalSwitchString("FMID", "Required short ID for Feature Manifest used in merged package name to ensure features from different FM files don't collid.");
				FeatureMerger._commandLineParser.SetOptionalSwitchString("Languages", "Supported UI language identifier list, separated by ';'. Required with single FM as InputFile.");
				FeatureMerger._commandLineParser.SetOptionalSwitchString("Resolutions", "Supported resolutions identifier list, separated by ';'. Required with single FM as InputFile.");
				FeatureMerger._commandLineParser.SetOptionalSwitchString("MergePackageRootReplacement", "Specifies a string to be used in the generated FM file for packages.  Replaces the OutputPackageDir in the package paths.");
				FeatureMerger._commandLineParser.SetOptionalSwitchString("OwnerType", "Resulting Package owner type: Microsoft or OEM.  Ignored when specifying FM File List");
				FeatureMerger._commandLineParser.SetOptionalSwitchString("OwnerName", "Name of package owner.  Ignored when specifying FM File List");
				FeatureMerger._commandLineParser.SetOptionalSwitchBoolean("Incremental", "Specifies to only remerge existing merged packages when one of the sources packages has changed. Default is rebuild all.", false);
				FeatureMerger._commandLineParser.SetOptionalSwitchBoolean("Compress", "Specifies to compress merged packages.", false);
				FeatureMerger._commandLineParser.SetOptionalSwitchBoolean("ConvertToCBS", "Convert all output to CBS.  Default is SPKG and no CBS packages allowed in FMs", false);
				FeatureMerger._commandLineParser.SetOptionalSwitchString("variables", "Additional variables used in the project file, syntax:<name>=<value>;<name>=<value>;...");
			}
			catch (Exception ex)
			{
				Console.WriteLine("FeatureMerger!SetCmdLineParams: Unable to set an option: {0}", ex.Message);
				return false;
			}
			return true;
		}

		// Token: 0x06000018 RID: 24 RVA: 0x00004930 File Offset: 0x00002B30
		private static bool LoadFMFiles(string inputFile)
		{
			try
			{
				LogUtil.Message("Trying to load file '{0}' as a FM file list ...", new object[]
				{
					inputFile
				});
				FeatureMerger._fmFileList = FMCollectionManifest.ValidateAndLoad(inputFile, new IULogger
				{
					ErrorLogger = null,
					InformationLogger = null
				});
				FeatureMerger._singleFM = false;
				return true;
			}
			catch (Exception ex)
			{
				LogUtil.Message("Input file '{0}' doesn't look like a valid FM file list, see the following information:", new object[]
				{
					inputFile
				});
				LogUtil.Message(ex.Message);
			}
			try
			{
				LogUtil.Message("Trying to load file '{0}' as a single FM file ...", new object[]
				{
					inputFile
				});
				FeatureMerger._singleFM = true;
				FMCollectionItem fmcollectionItem = new FMCollectionItem();
				fmcollectionItem.Path = inputFile;
				fmcollectionItem.releaseType = ReleaseType.Test;
				FeatureMerger._fmFileList.FMs.Add(fmcollectionItem);
				return true;
			}
			catch (Exception ex2)
			{
				LogUtil.Message("Input file '{0}' is not a valid FM file, see the following information:", new object[]
				{
					inputFile
				});
				LogUtil.Message(ex2.Message);
			}
			return false;
		}

		// Token: 0x06000019 RID: 25 RVA: 0x00004A28 File Offset: 0x00002C28
		private static void DisplayUsage()
		{
			Console.WriteLine("FeatureMerger Usage Description:");
			Console.WriteLine(FeatureMerger._commandLineParser.UsageString());
			Console.WriteLine("\tExamples:");
			Console.WriteLine("\t\tFeatureMerger C:\\PreMergeFMs\\OEMFM.xml C:\\MergedPackages 8.0.0.1 C:\\MergedFMs /Languages:en-us;de-de /Resolutions:480x800;720x1280;768x1280;1080x1920  /variables:_cputype=arm;buildtype=fre;releasetype=production");
			Console.WriteLine("\t\tFeatureMerger C:\\FMFileList.xml C:\\MergedPackages 8.0.0.1 C:\\MergedFMs /InputFMDir:C:\\PreMergeFMs /MergePackageRootReplacement:$(MSPackageRoot)\\Merged /variables:_cputype=arm;buildtype=fre");
		}

		// Token: 0x04000001 RID: 1
		private static CommandLineParser _commandLineParser = null;

		// Token: 0x04000002 RID: 2
		private static bool _singleFM = false;

		// Token: 0x04000003 RID: 3
		private static FMCollectionManifest _fmFileList = null;

		// Token: 0x04000004 RID: 4
		private static Dictionary<string, string> _variableLookup = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

		// Token: 0x04000005 RID: 5
		private static string _outputPackageDir;

		// Token: 0x04000006 RID: 6
		private static string _outputFMDir;

		// Token: 0x04000007 RID: 7
		private static string _inputFMDir;

		// Token: 0x04000008 RID: 8
		private static string _packagePathReplacement = string.Empty;

		// Token: 0x04000009 RID: 9
		private static string _msPackageRoot = string.Empty;

		// Token: 0x0400000A RID: 10
		private static string _binaryRoot = string.Empty;

		// Token: 0x0400000B RID: 11
		private static CpuId _cpuId = CpuId.Invalid;

		// Token: 0x0400000C RID: 12
		private static string _buildTypeStr = null;

		// Token: 0x0400000D RID: 13
		private static BuildType _buildType = BuildType.Invalid;

		// Token: 0x0400000E RID: 14
		private static VersionInfo _version = VersionInfo.Empty;

		// Token: 0x0400000F RID: 15
		private static ReleaseType _releaseType = ReleaseType.Test;

		// Token: 0x04000010 RID: 16
		private static FeatureMerger.CriticalFMProcessing _critical = FeatureMerger.CriticalFMProcessing.All;

		// Token: 0x04000011 RID: 17
		private static bool _incremental = false;

		// Token: 0x04000012 RID: 18
		private static bool _compress = false;

		// Token: 0x04000013 RID: 19
		private static bool _convertToCBS = false;

		// Token: 0x04000014 RID: 20
		private static StringComparer IgnoreCase = StringComparer.OrdinalIgnoreCase;

		// Token: 0x04000015 RID: 21
		private static readonly string MicrosoftPhoneFMName = "MicrosoftPhoneFM";

		// Token: 0x04000016 RID: 22
		private static IULogger _iuLogger = new IULogger();

		// Token: 0x04000017 RID: 23
		private static bool _doPlatformManifest = false;

		// Token: 0x04000018 RID: 24
		private static PlatformManifestGen _pmMainOS = null;

		// Token: 0x04000019 RID: 25
		private static PlatformManifestGen _pmUpdateOS = null;

		// Token: 0x0400001A RID: 26
		private static PlatformManifestGen _pmEFIESP = null;

		// Token: 0x0400001B RID: 27
		private static readonly string PlatformManifestRelativeDir = "DeviceImaging\\PlatformManifest\\";

		// Token: 0x0400001C RID: 28
		private static readonly string NonMergedPacakgeList = "Merged\\$(CpuType)\\$(BuildType)\\NonMergedPkgs.lst";

		// Token: 0x0400001D RID: 29
		private static string _nonMergedPackageListFile;

		// Token: 0x0400001E RID: 30
		private static List<string> _nonMergedPackageList = new List<string>();

		// Token: 0x0400001F RID: 31
		private static string _pmPath;

		// Token: 0x04000020 RID: 32
		private static string _signInfoPath;

		// Token: 0x04000021 RID: 33
		private static string _buildBranchInfo;

		// Token: 0x04000022 RID: 34
		private static FMCollectionItem _uiItem = null;

		// Token: 0x04000023 RID: 35
		private static FMCollectionItem _msPhoneItem = null;

		// Token: 0x04000024 RID: 36
		private static Guid _buildID = Guid.NewGuid();

		// Token: 0x04000025 RID: 37
		private static string _buildInfo;

		// Token: 0x04000026 RID: 38
		private static List<string> _supportedLocales = new List<string>();

		// Token: 0x02000004 RID: 4
		private enum CriticalFMProcessing
		{
			// Token: 0x04000029 RID: 41
			Yes,
			// Token: 0x0400002A RID: 42
			No,
			// Token: 0x0400002B RID: 43
			All
		}
	}
}
