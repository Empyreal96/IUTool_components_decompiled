using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using Microsoft.WindowsPhone.FeatureAPI;
using Microsoft.WindowsPhone.ImageUpdate.PkgCommon;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;
using Microsoft.WindowsPhone.Imaging;

namespace Microsoft.WindowsPhone.CompDB
{
	// Token: 0x0200000F RID: 15
	public class DesktopCompDBGen
	{
		// Token: 0x06000093 RID: 147 RVA: 0x00005C15 File Offset: 0x00003E15
		public static void Initialize(string pkgRoot, string buildArch, string sdxRoot, IULogger logger)
		{
			DesktopCompDBGen._pkgRoot = pkgRoot;
			DesktopCompDBGen._buildArch = buildArch;
			DesktopCompDBGen._sdxRoot = sdxRoot;
			DesktopCompDBGen._logger = logger;
			DesktopCompDBGen._bInitialized = true;
		}

		// Token: 0x06000094 RID: 148 RVA: 0x00005C38 File Offset: 0x00003E38
		public static BuildCompDB GenerateCompDB(FMCollection fmCollection, string fmDirectory, string msPackageRoot, string language, string buildType, CpuId buildArch, string buildInfo, IULogger logger)
		{
			return DesktopCompDBGen.GenerateCompDB(fmCollection, fmDirectory, msPackageRoot, language, buildType, buildArch, buildInfo, logger, null, null, false);
		}

		// Token: 0x06000095 RID: 149 RVA: 0x00005C5C File Offset: 0x00003E5C
		public static BuildCompDB GenerateCompDB(FMCollection fmCollection, string fmDirectory, string msPackageRoot, string language, string buildType, CpuId buildArch, string buildInfo, IULogger logger, List<DesktopCompDBGen.DesktopFilters> filters, List<string> featureIDs, bool generateHashes)
		{
			DesktopCompDBGen.<>c__DisplayClass57_0 CS$<>8__locals1 = new DesktopCompDBGen.<>c__DisplayClass57_0();
			CS$<>8__locals1.featureIDs = featureIDs;
			DesktopCompDBGen._generateHashes = generateHashes;
			if (!string.IsNullOrEmpty(fmCollection.Manifest.ChunkMappingsFile))
			{
				string chunkMappingFile = fmCollection.Manifest.GetChunkMappingFile(fmDirectory);
				if (LongPathFile.Exists(chunkMappingFile))
				{
					BuildCompDB.InitializeChunkMapping(chunkMappingFile, fmCollection.Manifest.SupportedLanguages, logger);
				}
			}
			CS$<>8__locals1.newCompDB = new BuildCompDB(logger);
			if (fmCollection.Manifest == null)
			{
				throw new ImageCommonException("ImageCommon::DesktopCompDBGen!GenerateCompDB: Unable to generate Build CompDB without a FM Collection.");
			}
			DesktopCompDBGen.Initialize(msPackageRoot, buildArch.ToString(), null, logger);
			CS$<>8__locals1.newCompDB.Product = fmCollection.Manifest.Product;
			CS$<>8__locals1.newCompDB.BuildArch = buildArch.ToString();
			List<string> list = new List<string>();
			List<FMCollectionItem> list2 = new List<FMCollectionItem>();
			if (string.IsNullOrEmpty(language))
			{
				list2.AddRange(from fm in fmCollection.Manifest.FMs
				where fm.ID.Equals(DesktopCompDBGen.c_MicrosoftDesktopEditionsFM_ID, StringComparison.OrdinalIgnoreCase) || fm.ID.Equals(DesktopCompDBGen.c_MicrosoftDesktopToolsFM_ID, StringComparison.OrdinalIgnoreCase)
				select fm);
			}
			else if (language.Equals("Neutral", StringComparison.OrdinalIgnoreCase))
			{
				list2.AddRange(from fm in fmCollection.Manifest.FMs
				where fm.ID.Equals(DesktopCompDBGen.c_MicrosoftDesktopNeutralFM_ID, StringComparison.OrdinalIgnoreCase) || fm.ID.Equals(DesktopCompDBGen.c_MicorosftDesktopConditionalFeaturesFMName, StringComparison.OrdinalIgnoreCase)
				select fm);
			}
			else
			{
				if (string.IsNullOrEmpty(language))
				{
					list = fmCollection.Manifest.SupportedLanguages;
				}
				else
				{
					list.Add(language);
				}
				list2.AddRange(from fm in fmCollection.Manifest.FMs
				where fm.ID.Equals(DesktopCompDBGen.c_MicrosoftDesktopLangsFM_ID, StringComparison.OrdinalIgnoreCase)
				select fm);
			}
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = false;
			foreach (FMCollectionItem fmcollectionItem in list2)
			{
				if (buildArch == fmcollectionItem.CPUType || fmcollectionItem.CPUType == CpuId.Invalid)
				{
					FeatureManifest featureManifest = new FeatureManifest();
					string xmlFile = Environment.ExpandEnvironmentVariables(fmcollectionItem.Path);
					xmlFile = fmcollectionItem.ResolveFMPath(fmDirectory);
					FeatureManifest.ValidateAndLoad(ref featureManifest, xmlFile, DesktopCompDBGen._logger);
					if (featureManifest.OwnerType == OwnerType.Microsoft && CS$<>8__locals1.newCompDB.BuildID == Guid.Empty && !string.IsNullOrEmpty(featureManifest.BuildID))
					{
						CS$<>8__locals1.newCompDB.BuildID = new Guid(featureManifest.BuildID);
						CS$<>8__locals1.newCompDB.BuildInfo = featureManifest.BuildInfo;
						CS$<>8__locals1.newCompDB.OSVersion = featureManifest.OSVersion;
					}
					List<FeatureManifest.FMPkgInfo> allPackageByGroups = featureManifest.GetAllPackageByGroups(list, fmCollection.Manifest.SupportedLocales, fmCollection.Manifest.SupportedResolutions, buildType, DesktopCompDBGen._buildArch, msPackageRoot);
					if (list.Any<string>())
					{
						foreach (FeatureManifest.FMPkgInfo fmpkgInfo in allPackageByGroups)
						{
							if (!string.IsNullOrEmpty(fmpkgInfo.Language))
							{
								fmpkgInfo.PackagePath = fmpkgInfo.PackagePath.Replace(PkgFile.DefaultLanguagePattern + fmpkgInfo.Language, "", StringComparison.OrdinalIgnoreCase);
								fmpkgInfo.ID = fmpkgInfo.ID.Replace(PkgFile.DefaultLanguagePattern + fmpkgInfo.Language, "", StringComparison.OrdinalIgnoreCase);
							}
						}
					}
					List<string> list3 = (from pkg in allPackageByGroups
					select pkg.FeatureID).Distinct(StringComparer.OrdinalIgnoreCase).ToList<string>();
					if (filters != null && filters.Contains(DesktopCompDBGen.DesktopFilters.IncludeByFeatureIDs))
					{
						IEnumerable<string> source = list3;
						Func<string, bool> predicate;
						if ((predicate = CS$<>8__locals1.<>9__4) == null)
						{
							DesktopCompDBGen.<>c__DisplayClass57_0 CS$<>8__locals2 = CS$<>8__locals1;
							predicate = (CS$<>8__locals2.<>9__4 = ((string feat) => CS$<>8__locals2.featureIDs.Contains(FMFeatures.GetFeatureIDWithoutPrefix(feat), StringComparer.OrdinalIgnoreCase)));
						}
						list3 = source.Where(predicate).ToList<string>();
					}
					else if (string.IsNullOrEmpty(language))
					{
						list3.Remove("MS_" + DesktopCompDBGen.c_BaseNeutralFeatureID);
						list3.Remove("MS_" + DesktopCompDBGen.c_BaseLanguageFeatureID);
					}
					using (List<string>.Enumerator enumerator3 = list3.GetEnumerator())
					{
						while (enumerator3.MoveNext())
						{
							string feature = enumerator3.Current;
							List<FeatureManifest.FMPkgInfo> list4 = (from pkg in allPackageByGroups
							where pkg.FeatureID.Equals(feature, StringComparison.OrdinalIgnoreCase)
							select pkg).ToList<FeatureManifest.FMPkgInfo>();
							CompDBFeature compDBFeature = new CompDBFeature(feature, fmcollectionItem.ID, CompDBFeature.CompDBFeatureTypes.MobileFeature, (fmcollectionItem.ownerType == OwnerType.Microsoft) ? fmcollectionItem.ownerType.ToString() : OwnerType.OEM.ToString());
							compDBFeature.FeatureID = FMFeatures.GetFeatureIDWithoutPrefix(compDBFeature.FeatureID);
							using (List<FeatureManifest.FMPkgInfo>.Enumerator enumerator2 = list4.GetEnumerator())
							{
								while (enumerator2.MoveNext())
								{
									FeatureManifest.FMPkgInfo pkg = enumerator2.Current;
									if (!CS$<>8__locals1.newCompDB.Packages.Any((CompDBPackageInfo pkg2) => pkg2.ID.Equals(pkg.ID, StringComparison.OrdinalIgnoreCase)))
									{
										AssemblyIdentity assemblyIdentity = new AssemblyIdentity();
										if (!language.Equals("Neutral", StringComparison.OrdinalIgnoreCase))
										{
											assemblyIdentity.language = language;
										}
										assemblyIdentity.buildType = "fre";
										assemblyIdentity.name = pkg.ID;
										assemblyIdentity.processorArchitecture = buildArch.ToString();
										if (pkg.Version != null)
										{
											assemblyIdentity.version = pkg.Version.ToString();
										}
										CompDBPackageInfo compDBPackageInfo = DesktopCompDBGen.GenerateFeaturePackage(compDBFeature, pkg.ID, assemblyIdentity, pkg.PackagePath, language, msPackageRoot);
										if ((string.IsNullOrEmpty(compDBPackageInfo.VersionStr) || compDBPackageInfo.VersionStr.Equals("0.0.0.0")) && !string.IsNullOrEmpty(assemblyIdentity.version))
										{
											compDBPackageInfo.VersionStr = assemblyIdentity.version;
										}
										CS$<>8__locals1.newCompDB.Packages.Add(compDBPackageInfo);
									}
									else
									{
										CompDBFeaturePackage compDBFeaturePackage = CS$<>8__locals1.newCompDB.Features.SelectMany((CompDBFeature feat) => feat.Packages).FirstOrDefault((CompDBFeaturePackage featPkg) => featPkg.ID.Equals(pkg.ID, StringComparison.OrdinalIgnoreCase));
										if (compDBFeaturePackage == null)
										{
											flag = true;
											stringBuilder.AppendLine("Package '" + pkg.ID + "' found in Packages but not in any features.");
										}
										else
										{
											compDBFeature.Packages.Add(compDBFeaturePackage);
										}
									}
								}
							}
							compDBFeature.Type = DesktopCompDBGen.GetFeatureTypeFromGrouping(featureManifest.Features.MSFeatureGroups, compDBFeature.FeatureID);
							CS$<>8__locals1.newCompDB.Features.Add(compDBFeature);
						}
					}
					if (featureManifest.Features != null && featureManifest.Features.MSConditionalFeatures != null && featureManifest.Features.MSConditionalFeatures.Count<FMConditionalFeature>() != 0)
					{
						if (CS$<>8__locals1.newCompDB.MSConditionalFeatures == null)
						{
							CS$<>8__locals1.newCompDB.MSConditionalFeatures = new List<FMConditionalFeature>();
						}
						CS$<>8__locals1.newCompDB.MSConditionalFeatures.AddRange(featureManifest.Features.MSConditionalFeatures);
					}
				}
			}
			if (flag)
			{
				throw new ImageCommonException("ImageCommon::DesktopCompDBGen!GenerateCompDB: Errors processing FM File(s):\n" + stringBuilder.ToString());
			}
			CS$<>8__locals1.newCompDB.Packages = CS$<>8__locals1.newCompDB.Packages.Distinct(CompDBPackageInfoComparer.Standard).ToList<CompDBPackageInfo>();
			CS$<>8__locals1.newCompDB.Packages = (from pkg in CS$<>8__locals1.newCompDB.Packages
			select pkg.SetParentDB(CS$<>8__locals1.newCompDB)).ToList<CompDBPackageInfo>();
			if (CS$<>8__locals1.newCompDB.BuildID == Guid.Empty)
			{
				CS$<>8__locals1.newCompDB.BuildID = Guid.NewGuid();
				CS$<>8__locals1.newCompDB.BuildInfo = buildInfo;
			}
			return CS$<>8__locals1.newCompDB;
		}

		// Token: 0x06000096 RID: 150 RVA: 0x0000647C File Offset: 0x0000467C
		private static CompDBFeature.CompDBFeatureTypes GetFeatureTypeFromGrouping(List<FMFeatureGrouping> groupings, string feature)
		{
			CompDBFeature.CompDBFeatureTypes result = CompDBFeature.CompDBFeatureTypes.None;
			if (groupings != null && groupings.Any<FMFeatureGrouping>())
			{
				FMFeatureGrouping fmfeatureGrouping = groupings.FirstOrDefault((FMFeatureGrouping grp) => grp.FeatureIDs.Contains(feature, StringComparer.OrdinalIgnoreCase) && !string.IsNullOrEmpty(grp.GroupingType));
				if (fmfeatureGrouping != null && !Enum.TryParse<CompDBFeature.CompDBFeatureTypes>(fmfeatureGrouping.GroupingType, out result))
				{
					result = CompDBFeature.CompDBFeatureTypes.None;
				}
			}
			return result;
		}

		// Token: 0x06000097 RID: 151 RVA: 0x000064CC File Offset: 0x000046CC
		public static void GenerateCompDBFMs(List<string> langs, Guid buildID, string buildInfo, string fmDirectory, string conditionalFeaturesFMFile)
		{
			DesktopCompDBGen.<>c__DisplayClass59_0 CS$<>8__locals1 = new DesktopCompDBGen.<>c__DisplayClass59_0();
			if (!DesktopCompDBGen._bInitialized)
			{
				throw new Exception("ImageCommon::DesktopCompDBGen!GenerateCompDBFMs: Function called before being initialized.");
			}
			if (!LongPathDirectory.Exists(fmDirectory))
			{
				LongPathDirectory.CreateDirectory(fmDirectory);
			}
			DesktopCompDBGen._generatingFMs = true;
			FMCollectionManifest fmcollectionManifest = new FMCollectionManifest();
			fmcollectionManifest.Product = DesktopCompDBGen.c_WindowsDesktopProduct;
			fmcollectionManifest.SupportedLanguages = langs;
			CpuId cpuId;
			if (!Enum.TryParse<CpuId>(DesktopCompDBGen._buildArch, out cpuId))
			{
				cpuId = CpuId.Invalid;
			}
			if (LongPathFile.Exists(Path.Combine(fmDirectory, DesktopCompDBGen.c_MicorosftDesktopChunksMappingFileName)))
			{
				fmcollectionManifest.ChunkMappingsFile = FMCollection.c_FMDirectoryVariable + "\\" + DesktopCompDBGen.c_MicorosftDesktopChunksMappingFileName;
			}
			string text = null;
			FeatureManifest featureManifest = null;
			FMCollectionItem fmcollectionItem;
			if (!string.IsNullOrEmpty(conditionalFeaturesFMFile) && LongPathFile.Exists(conditionalFeaturesFMFile))
			{
				string fileName = Path.GetFileName(conditionalFeaturesFMFile);
				text = LongPath.Combine(fmDirectory, fileName);
				featureManifest = new FeatureManifest();
				FeatureManifest.ValidateAndLoad(ref featureManifest, conditionalFeaturesFMFile, DesktopCompDBGen._logger);
				fmcollectionItem = new FMCollectionItem();
				fmcollectionItem.CPUType = cpuId;
				fmcollectionItem.ID = (featureManifest.ID = DesktopCompDBGen.c_MicrosoftDesktopConditionsFM_ID);
				fmcollectionItem.Owner = (featureManifest.Owner = OwnerType.Microsoft.ToString());
				fmcollectionItem.ownerType = (featureManifest.OwnerType = OwnerType.Microsoft);
				fmcollectionItem.releaseType = (featureManifest.ReleaseType = ReleaseType.Production);
				fmcollectionItem.Path = FMCollection.c_FMDirectoryVariable + "\\" + fileName;
				featureManifest.BuildID = buildID.ToString();
				featureManifest.BuildInfo = buildInfo;
				fmcollectionManifest.FMs.Add(fmcollectionItem);
			}
			fmcollectionItem = new FMCollectionItem();
			FeatureManifest featureManifest2 = new FeatureManifest();
			DesktopCompDBGen.InitializeFMItem(ref fmcollectionItem, ref featureManifest2, DesktopCompDBGen.c_MicrosoftDesktopNeutralFM_ID, DesktopCompDBGen.c_MicrosoftDesktopNeutralFM, cpuId, fmDirectory, buildID.ToString(), buildInfo);
			fmcollectionManifest.FMs.Add(fmcollectionItem);
			fmcollectionItem = new FMCollectionItem();
			FeatureManifest featureManifest3 = new FeatureManifest();
			DesktopCompDBGen.InitializeFMItem(ref fmcollectionItem, ref featureManifest3, DesktopCompDBGen.c_MicrosoftDesktopLangsFM_ID, DesktopCompDBGen.c_MicrosoftDesktopLangsFM, cpuId, fmDirectory, buildID.ToString(), buildInfo);
			fmcollectionManifest.FMs.Add(fmcollectionItem);
			fmcollectionItem = new FMCollectionItem();
			FeatureManifest featureManifest4 = new FeatureManifest();
			DesktopCompDBGen.InitializeFMItem(ref fmcollectionItem, ref featureManifest4, DesktopCompDBGen.c_MicrosoftDesktopEditionsFM_ID, DesktopCompDBGen.c_MicrosoftDesktopEditionsFM, cpuId, fmDirectory, buildID.ToString(), buildInfo);
			fmcollectionManifest.FMs.Add(fmcollectionItem);
			fmcollectionItem = new FMCollectionItem();
			FeatureManifest featureManifest5 = new FeatureManifest();
			DesktopCompDBGen.InitializeFMItem(ref fmcollectionItem, ref featureManifest5, DesktopCompDBGen.c_MicrosoftDesktopToolsFM_ID, DesktopCompDBGen.c_MicrosoftDesktopToolsFM, cpuId, fmDirectory, buildID.ToString(), buildInfo);
			fmcollectionManifest.FMs.Add(fmcollectionItem);
			BuildCompDB buildCompDB = null;
			foreach (string lang in langs)
			{
				BuildCompDB buildCompDB2 = DesktopCompDBGen.GenerateCompDB(lang, buildID, buildInfo);
				if (buildCompDB == null)
				{
					buildCompDB = buildCompDB2;
				}
				else
				{
					buildCompDB.Merge(buildCompDB2);
				}
				DesktopCompDBGen._pkgMap = new Dictionary<string, CompDBPackageInfo>(StringComparer.OrdinalIgnoreCase);
				DesktopCompDBGen._featurePkgMap = new Dictionary<CompDBFeature, List<CompDBFeaturePackage>>();
				DesktopCompDBGen._clientEditionsFeatures = new Dictionary<DesktopCompDBGen.ClientTypes, List<CompDBFeature>>();
			}
			BuildCompDB srcDB = DesktopCompDBGen.GenerateCompDBForOptionalPkgs(DesktopCompDBGen.c_NeutralLanguage, buildID, buildInfo);
			buildCompDB.Merge(srcDB);
			featureManifest2.OSVersion = DesktopCompDBGen._editionVersionLookup[DesktopCompDBGen.ClientTypes.Client];
			featureManifest3.OSVersion = DesktopCompDBGen._editionVersionLookup[DesktopCompDBGen.ClientTypes.Client];
			featureManifest4.OSVersion = DesktopCompDBGen._editionVersionLookup[DesktopCompDBGen.ClientTypes.Client];
			featureManifest5.OSVersion = DesktopCompDBGen._editionVersionLookup[DesktopCompDBGen.ClientTypes.Client];
			if (featureManifest != null && !string.IsNullOrEmpty(text))
			{
				featureManifest.OSVersion = DesktopCompDBGen._editionVersionLookup[DesktopCompDBGen.ClientTypes.Client];
				featureManifest.WriteToFile(text);
			}
			List<CompDBFeature> list = (from feat in buildCompDB.Features
			where feat.Type == CompDBFeature.CompDBFeatureTypes.LanguagePack
			select feat).ToList<CompDBFeature>();
			CS$<>8__locals1.langPkgIDs = (from pkg in list.SelectMany((CompDBFeature feat) => feat.Packages)
			select pkg.ID).Distinct(StringComparer.OrdinalIgnoreCase).ToList<string>();
			(from pkg in buildCompDB.Packages
			where CS$<>8__locals1.langPkgIDs.Contains(pkg.ID, StringComparer.OrdinalIgnoreCase)
			select pkg).ToList<CompDBPackageInfo>();
			FMFeatureGrouping fmfeatureGrouping = new FMFeatureGrouping();
			fmfeatureGrouping.GroupingType = CompDBFeature.CompDBFeatureTypes.LanguagePack.ToString();
			fmfeatureGrouping.FeatureIDs = (from feat in list
			select feat.FeatureID).ToList<string>();
			featureManifest3.Features.MSFeatureGroups = new List<FMFeatureGrouping>
			{
				fmfeatureGrouping
			};
			List<CompDBPackageInfo> source = (from pkg in buildCompDB.Packages
			where pkg.SatelliteType == CompDBPackageInfo.SatelliteTypes.Language && !CS$<>8__locals1.langPkgIDs.Contains(pkg.ID, StringComparer.OrdinalIgnoreCase)
			select pkg).ToList<CompDBPackageInfo>();
			if (source.Any<CompDBPackageInfo>())
			{
				CompDBFeature compDBFeature = new CompDBFeature();
				compDBFeature.Type = CompDBFeature.CompDBFeatureTypes.None;
				compDBFeature.Group = OwnerType.Microsoft.ToString();
				compDBFeature.FeatureID = DesktopCompDBGen.c_BaseLanguageFeatureID;
				compDBFeature.Packages = (from pkg in source
				select new CompDBFeaturePackage(pkg.ID, false)).ToList<CompDBFeaturePackage>();
				list.Add(compDBFeature);
				CS$<>8__locals1.langPkgIDs.AddRange(from pkg in compDBFeature.Packages
				select pkg.ID);
			}
			IEnumerable<CompDBPackageInfo> packages = buildCompDB.Packages;
			Func<CompDBPackageInfo, bool> predicate;
			if ((predicate = CS$<>8__locals1.<>9__8) == null)
			{
				DesktopCompDBGen.<>c__DisplayClass59_0 CS$<>8__locals2 = CS$<>8__locals1;
				predicate = (CS$<>8__locals2.<>9__8 = ((CompDBPackageInfo pkg) => CS$<>8__locals2.langPkgIDs.Contains(pkg.ID, StringComparer.OrdinalIgnoreCase)));
			}
			using (IEnumerator<CompDBPackageInfo> enumerator2 = packages.Where(predicate).GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					CompDBPackageInfo pkg = enumerator2.Current;
					Func<CompDBFeaturePackage, bool> <>9__10;
					List<string> featureIDs = (from feat in list.Where(delegate(CompDBFeature feat)
					{
						IEnumerable<CompDBFeaturePackage> packages4 = feat.Packages;
						Func<CompDBFeaturePackage, bool> predicate4;
						if ((predicate4 = <>9__10) == null)
						{
							predicate4 = (<>9__10 = ((CompDBFeaturePackage featPkg) => featPkg.ID.Equals(pkg.ID, StringComparison.OrdinalIgnoreCase)));
						}
						return packages4.Any(predicate4);
					})
					select feat.FeatureID).ToList<string>();
					MSOptionalPkgFile msoptionalPkgFile = new MSOptionalPkgFile();
					msoptionalPkgFile.FeatureIdentifierPackage = false;
					msoptionalPkgFile.FeatureIDs = featureIDs;
					msoptionalPkgFile.Language = "(" + pkg.SatelliteValue + ")";
					msoptionalPkgFile.NoBasePackage = true;
					msoptionalPkgFile.ID = pkg.ID;
					string path = pkg.FirstPayloadItem.Path;
					msoptionalPkgFile.Directory = "$(mspackageroot)\\" + LongPath.GetDirectoryName(path);
					msoptionalPkgFile.Name = LongPath.GetFileName(path);
					msoptionalPkgFile.Version = pkg.VersionStr;
					featureManifest3.Features.Microsoft.Add(msoptionalPkgFile);
				}
			}
			string fileName2 = Path.Combine(fmDirectory, DesktopCompDBGen.c_MicrosoftDesktopLangsFM);
			featureManifest3.WriteToFile(fileName2);
			List<CompDBFeature> list2 = (from feat in buildCompDB.Features
			where feat.Type == CompDBFeature.CompDBFeatureTypes.OnDemandFeature
			select feat).ToList<CompDBFeature>();
			CS$<>8__locals1.neutralPkgIDs = (from pkg in list2.SelectMany((CompDBFeature feat) => feat.Packages)
			select pkg.ID).Distinct(StringComparer.OrdinalIgnoreCase).ToList<string>();
			(from pkg in buildCompDB.Packages
			where CS$<>8__locals1.neutralPkgIDs.Contains(pkg.ID, StringComparer.OrdinalIgnoreCase)
			select pkg).ToList<CompDBPackageInfo>();
			FMFeatureGrouping fmfeatureGrouping2 = new FMFeatureGrouping();
			fmfeatureGrouping2.GroupingType = CompDBFeature.CompDBFeatureTypes.OnDemandFeature.ToString();
			fmfeatureGrouping2.FeatureIDs = (from feat in list2
			select feat.FeatureID).ToList<string>();
			featureManifest2.Features.MSFeatureGroups = new List<FMFeatureGrouping>
			{
				fmfeatureGrouping2
			};
			List<CompDBPackageInfo> source2 = (from pkg in buildCompDB.Packages
			where pkg.SatelliteType != CompDBPackageInfo.SatelliteTypes.Language && !CS$<>8__locals1.neutralPkgIDs.Contains(pkg.ID, StringComparer.OrdinalIgnoreCase)
			select pkg).Distinct<CompDBPackageInfo>().ToList<CompDBPackageInfo>();
			if (source2.Any<CompDBPackageInfo>())
			{
				CompDBFeature compDBFeature2 = new CompDBFeature();
				compDBFeature2.Type = CompDBFeature.CompDBFeatureTypes.None;
				compDBFeature2.Group = OwnerType.Microsoft.ToString();
				compDBFeature2.FeatureID = DesktopCompDBGen.c_BaseNeutralFeatureID;
				compDBFeature2.Packages = (from pkg in source2
				select new CompDBFeaturePackage(pkg.ID, false)).ToList<CompDBFeaturePackage>();
				list2.Add(compDBFeature2);
				CS$<>8__locals1.neutralPkgIDs.AddRange(from pkg in compDBFeature2.Packages
				select pkg.ID);
			}
			IEnumerable<CompDBPackageInfo> packages2 = buildCompDB.Packages;
			Func<CompDBPackageInfo, bool> predicate2;
			if ((predicate2 = CS$<>8__locals1.<>9__20) == null)
			{
				DesktopCompDBGen.<>c__DisplayClass59_0 CS$<>8__locals4 = CS$<>8__locals1;
				predicate2 = (CS$<>8__locals4.<>9__20 = ((CompDBPackageInfo pkg) => CS$<>8__locals4.neutralPkgIDs.Contains(pkg.ID, StringComparer.OrdinalIgnoreCase)));
			}
			using (IEnumerator<CompDBPackageInfo> enumerator2 = packages2.Where(predicate2).GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					CompDBPackageInfo pkg = enumerator2.Current;
					Func<CompDBFeaturePackage, bool> <>9__22;
					List<string> featureIDs2 = (from feat in list2.Where(delegate(CompDBFeature feat)
					{
						IEnumerable<CompDBFeaturePackage> packages4 = feat.Packages;
						Func<CompDBFeaturePackage, bool> predicate4;
						if ((predicate4 = <>9__22) == null)
						{
							predicate4 = (<>9__22 = ((CompDBFeaturePackage featPkg) => featPkg.ID.Equals(pkg.ID, StringComparison.OrdinalIgnoreCase)));
						}
						return packages4.Any(predicate4);
					})
					select feat.FeatureID).ToList<string>();
					MSOptionalPkgFile msoptionalPkgFile2 = new MSOptionalPkgFile();
					msoptionalPkgFile2.FeatureIdentifierPackage = false;
					msoptionalPkgFile2.FeatureIDs = featureIDs2;
					msoptionalPkgFile2.ID = pkg.ID;
					string path2 = pkg.FirstPayloadItem.Path;
					msoptionalPkgFile2.Directory = "$(mspackageroot)\\" + LongPath.GetDirectoryName(path2);
					msoptionalPkgFile2.Name = LongPath.GetFileName(path2);
					msoptionalPkgFile2.Version = pkg.VersionStr;
					featureManifest2.Features.Microsoft.Add(msoptionalPkgFile2);
				}
			}
			string fileName3 = Path.Combine(fmDirectory, DesktopCompDBGen.c_MicrosoftDesktopNeutralFM);
			featureManifest2.WriteToFile(fileName3);
			List<CompDBFeature> list3 = (from feat in buildCompDB.Features
			where feat.Type == CompDBFeature.CompDBFeatureTypes.DesktopMedia
			select feat).ToList<CompDBFeature>();
			CS$<>8__locals1.editionPkgIDs = (from pkg in list3.SelectMany((CompDBFeature feat) => feat.Packages)
			select pkg.ID).Distinct(StringComparer.OrdinalIgnoreCase).ToList<string>();
			FMFeatureGrouping fmfeatureGrouping3 = new FMFeatureGrouping();
			fmfeatureGrouping3.GroupingType = CompDBFeature.CompDBFeatureTypes.DesktopMedia.ToString();
			fmfeatureGrouping3.FeatureIDs = (from feat in list3
			select feat.FeatureID).ToList<string>();
			featureManifest4.Features.MSFeatureGroups = new List<FMFeatureGrouping>
			{
				fmfeatureGrouping3
			};
			IEnumerable<CompDBPackageInfo> packages3 = buildCompDB.Packages;
			Func<CompDBPackageInfo, bool> predicate3;
			if ((predicate3 = CS$<>8__locals1.<>9__28) == null)
			{
				DesktopCompDBGen.<>c__DisplayClass59_0 CS$<>8__locals6 = CS$<>8__locals1;
				predicate3 = (CS$<>8__locals6.<>9__28 = ((CompDBPackageInfo pkg) => CS$<>8__locals6.editionPkgIDs.Contains(pkg.ID, StringComparer.OrdinalIgnoreCase)));
			}
			using (IEnumerator<CompDBPackageInfo> enumerator2 = packages3.Where(predicate3).GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					CompDBPackageInfo pkg = enumerator2.Current;
					Func<CompDBFeaturePackage, bool> <>9__30;
					List<string> featureIDs3 = (from feat in list3.Where(delegate(CompDBFeature feat)
					{
						IEnumerable<CompDBFeaturePackage> packages4 = feat.Packages;
						Func<CompDBFeaturePackage, bool> predicate4;
						if ((predicate4 = <>9__30) == null)
						{
							predicate4 = (<>9__30 = ((CompDBFeaturePackage featPkg) => featPkg.ID.Equals(pkg.ID, StringComparison.OrdinalIgnoreCase)));
						}
						return packages4.Any(predicate4);
					})
					select feat.FeatureID).ToList<string>();
					MSOptionalPkgFile msoptionalPkgFile3 = new MSOptionalPkgFile();
					msoptionalPkgFile3.FeatureIdentifierPackage = false;
					msoptionalPkgFile3.FeatureIDs = featureIDs3;
					msoptionalPkgFile3.ID = pkg.ID;
					string path3 = pkg.FirstPayloadItem.Path;
					msoptionalPkgFile3.Directory = "$(mspackageroot)\\" + LongPath.GetDirectoryName(path3);
					msoptionalPkgFile3.Name = LongPath.GetFileName(path3);
					msoptionalPkgFile3.Version = pkg.VersionStr;
					featureManifest4.Features.Microsoft.Add(msoptionalPkgFile3);
				}
			}
			List<CompDBChunkMapItem> list4 = new List<CompDBChunkMapItem>();
			foreach (CompDBFeature compDBFeature3 in list3)
			{
				MSOptionalPkgFile msoptionalPkgFile4 = new MSOptionalPkgFile();
				msoptionalPkgFile4.FeatureIdentifierPackage = true;
				msoptionalPkgFile4.FeatureIDs = new List<string>();
				msoptionalPkgFile4.FeatureIDs.Add(compDBFeature3.FeatureID);
				msoptionalPkgFile4.ID = compDBFeature3.FeatureID + DesktopCompDBGen.c_EsdFeaturePost;
				string text2 = Path.Combine(DesktopCompDBGen.c_ESDRoot, compDBFeature3.FeatureID);
				msoptionalPkgFile4.Directory = "$(mspackageroot)\\" + text2;
				msoptionalPkgFile4.Name = compDBFeature3.FeatureID + DesktopCompDBGen.c_EsdExtenstion;
				msoptionalPkgFile4.Version = DesktopCompDBGen._editionVersionLookup[DesktopCompDBGen.GetEditionFeatureClientType(compDBFeature3, buildCompDB.Packages)];
				featureManifest4.Features.Microsoft.Add(msoptionalPkgFile4);
				list4.Add(new CompDBChunkMapItem
				{
					Type = DesktopCompDBGen.PackageTypes.ESD,
					ChunkName = string.Format("MetaESD_{0}", compDBFeature3.FeatureID),
					Path = text2
				});
			}
			string fileName4 = Path.Combine(fmDirectory, DesktopCompDBGen.c_MicrosoftDesktopEditionsFM);
			featureManifest4.WriteToFile(fileName4);
			MSOptionalPkgFile msoptionalPkgFile5 = new MSOptionalPkgFile();
			msoptionalPkgFile5.FeatureIdentifierPackage = true;
			msoptionalPkgFile5.FeatureIDs = new List<string>();
			msoptionalPkgFile5.FeatureIDs.Add(DesktopCompDBGen.c_UpdateBoxName);
			msoptionalPkgFile5.ID = DesktopCompDBGen.c_UpdateBoxName;
			msoptionalPkgFile5.Directory = "$(mspackageroot)\\" + DesktopCompDBGen.c_UpdateBoxRoot;
			msoptionalPkgFile5.Name = DesktopCompDBGen.c_UpdateBoxFilename;
			featureManifest5.Features.Microsoft.Add(msoptionalPkgFile5);
			FMFeatureGrouping fmfeatureGrouping4 = new FMFeatureGrouping();
			fmfeatureGrouping4.GroupingType = CompDBFeature.CompDBFeatureTypes.Tool.ToString();
			fmfeatureGrouping4.FeatureIDs = msoptionalPkgFile5.FeatureIDs;
			featureManifest5.Features.MSFeatureGroups = new List<FMFeatureGrouping>();
			featureManifest5.Features.MSFeatureGroups.Add(fmfeatureGrouping4);
			string fileName5 = Path.Combine(fmDirectory, DesktopCompDBGen.c_MicrosoftDesktopToolsFM);
			featureManifest5.WriteToFile(fileName5);
			string xmlFile = Path.Combine(fmDirectory, "FMFileList.xml");
			fmcollectionManifest.WriteToFile(xmlFile);
			if (!string.IsNullOrEmpty(fmcollectionManifest.ChunkMappingsFile))
			{
				string chunkMappingFile = fmcollectionManifest.GetChunkMappingFile(fmDirectory);
				CompDBChunkMapping compDBChunkMapping = CompDBChunkMapping.ValidateAndLoad(chunkMappingFile, fmcollectionManifest.SupportedLanguages, DesktopCompDBGen._logger);
				compDBChunkMapping.ChunkMappings.RemoveAll((CompDBChunkMapItem map) => map.Type == DesktopCompDBGen.PackageTypes.ESD);
				compDBChunkMapping.ChunkMappings.AddRange(list4);
				compDBChunkMapping.WriteToFile(chunkMappingFile);
			}
		}

		// Token: 0x06000098 RID: 152 RVA: 0x000073EC File Offset: 0x000055EC
		private static DesktopCompDBGen.ClientTypes GetEditionFeatureClientType(CompDBFeature editionFeature, List<CompDBPackageInfo> packages)
		{
			List<string> featurePkgIDs = (from pkg in editionFeature.Packages
			select pkg.ID).ToList<string>();
			if (packages.Any((CompDBPackageInfo pkg) => featurePkgIDs.Contains(pkg.ID, StringComparer.OrdinalIgnoreCase) && pkg.FirstPayloadItem.Path.ToUpper(CultureInfo.InvariantCulture).Contains(DesktopCompDBGen.c_ClientNEditions.ToUpper(CultureInfo.InvariantCulture))))
			{
				return DesktopCompDBGen.ClientTypes.ClientN;
			}
			return DesktopCompDBGen.ClientTypes.Client;
		}

		// Token: 0x06000099 RID: 153 RVA: 0x0000744C File Offset: 0x0000564C
		private static void InitializeFMItem(ref FMCollectionItem fmItem, ref FeatureManifest fm, string FMID, string fmFile, CpuId cpuType, string fmDirectory, string buildID, string buildInfo)
		{
			fmItem.CPUType = cpuType;
			FMCollectionItem fmcollectionItem = fmItem;
			fm.ID = FMID;
			fmcollectionItem.ID = FMID;
			fmItem.Owner = (fm.Owner = OwnerType.Microsoft.ToString());
			fmItem.ownerType = (fm.OwnerType = OwnerType.Microsoft);
			fmItem.releaseType = (fm.ReleaseType = ReleaseType.Production);
			fmItem.Path = FMCollection.c_FMDirectoryVariable + "\\" + fmFile;
			fmItem.Critical = true;
			fm.BuildID = buildID;
			fm.BuildInfo = buildInfo;
			fm.Features = new FMFeatures();
			fm.Features.Microsoft = new List<MSOptionalPkgFile>();
		}

		// Token: 0x0600009A RID: 154 RVA: 0x00007508 File Offset: 0x00005708
		public static BuildCompDB GenerateCompDBForOptionalPkgs(string lang, Guid buildID, string buildInfo)
		{
			if (!DesktopCompDBGen._bInitialized)
			{
				throw new Exception("ImageCommon::DesktopCompDBGen!GenerateCompDB: Function called before being initialized.");
			}
			BuildCompDB buildCompDB = new BuildCompDB(DesktopCompDBGen._logger);
			buildCompDB.Product = DesktopCompDBGen.c_WindowsDesktopProduct;
			buildCompDB.ReleaseType = ReleaseType.Production;
			buildCompDB.BuildID = buildID;
			buildCompDB.BuildInfo = buildInfo;
			buildCompDB.BuildArch = DesktopCompDBGen._buildArch;
			try
			{
				LongPath.Combine(DesktopCompDBGen._pkgRoot, DesktopCompDBGen.c_DesktopUpdatesDirectory);
				DesktopCompDBGen._logger.LogInfo("ImageCommon::DesktopCompDBGen: Processing FOD ({0} and {1})...", new object[]
				{
					DesktopCompDBGen.c_NeutralDirectory,
					lang
				});
				DesktopCompDBGen.GenerateCompDBForRemainingOptionalPkgs(Path.GetFullPath(Path.Combine(DesktopCompDBGen._pkgRoot, DesktopCompDBGen.c_FODDirectory, DesktopCompDBGen.c_NeutralDirectory, DesktopCompDBGen.c_FODCabs)), DesktopCompDBGen.PackageTypes.FOD, lang, DesktopCompDBGen._pkgRoot);
				buildCompDB.Features = DesktopCompDBGen.CompDBFeatures();
				buildCompDB.Packages = DesktopCompDBGen.CompDBPackages();
			}
			catch (Exception ex)
			{
				DesktopCompDBGen._logger.LogException(ex);
				Environment.Exit(Marshal.GetHRForException(ex));
			}
			return buildCompDB;
		}

		// Token: 0x0600009B RID: 155 RVA: 0x000075FC File Offset: 0x000057FC
		public static BuildCompDB GenerateCompDB(string lang, Guid buildID, string buildInfo)
		{
			if (!DesktopCompDBGen._bInitialized)
			{
				throw new Exception("ImageCommon::DesktopCompDBGen!GenerateCompDB: Function called before being initialized.");
			}
			BuildCompDB buildCompDB = new BuildCompDB(DesktopCompDBGen._logger);
			buildCompDB.Product = DesktopCompDBGen.c_WindowsDesktopProduct;
			buildCompDB.ReleaseType = ReleaseType.Production;
			buildCompDB.BuildID = buildID;
			buildCompDB.BuildInfo = buildInfo;
			buildCompDB.BuildArch = DesktopCompDBGen._buildArch;
			try
			{
				string path = LongPath.Combine(DesktopCompDBGen._pkgRoot, DesktopCompDBGen.c_DesktopUpdatesDirectory);
				string text = LongPath.Combine(path, DesktopCompDBGen.c_EditionPackagesDirectory);
				DesktopCompDBGen._logger.LogInfo("ImageCommon::DesktopCompDBGen: Processing All Editions ({0} and {1})...", new object[]
				{
					DesktopCompDBGen.c_NeutralDirectory,
					lang
				});
				DesktopCompDBGen.CompDBForAllEditions(LongPath.Combine(text, DesktopCompDBGen.c_NeutralDirectory), lang, DesktopCompDBGen._pkgRoot);
				DesktopCompDBGen.CompDBForOtherPackages(Path.Combine(text, lang, DesktopCompDBGen.c_ClientDirectory), DesktopCompDBGen.PackageTypes.LANGPACK, lang, DesktopCompDBGen._pkgRoot);
				DesktopCompDBGen._logger.LogInfo("ImageCommon::DesktopCompDBGen: Processing FOD ({0} and {1})...", new object[]
				{
					DesktopCompDBGen.c_NeutralDirectory,
					lang
				});
				string path2 = LongPath.Combine(DesktopCompDBGen._pkgRoot, DesktopCompDBGen.c_FODDirectory);
				string editionsMap = LongPath.Combine(DesktopCompDBGen._sdxRoot, "data\\DistribData\\mc\\FeaturesOnDemand\\" + DesktopCompDBGen.c_FODEditionsMapPattern.Replace(DesktopCompDBGen.c_BuildArchVar, DesktopCompDBGen._buildArch, StringComparison.OrdinalIgnoreCase));
				DesktopCompDBGen.GenerateCompDB(Path.GetFullPath(path2), DesktopCompDBGen.PackageTypes.FOD, lang, DesktopCompDBGen._pkgRoot, editionsMap);
				string text2 = LongPath.Combine(path, DesktopCompDBGen.c_AppsDirectory);
				if (Directory.Exists(text2))
				{
					DesktopCompDBGen._logger.LogInfo("ImageCommon::DesktopCompDBGen: Processing APPS...", new object[0]);
					editionsMap = LongPath.Combine(text2, DesktopCompDBGen.c_AppsEditionsMap);
					DesktopCompDBGen.GenerateCompDB(text2, DesktopCompDBGen.PackageTypes.APP, lang, DesktopCompDBGen._pkgRoot, editionsMap);
				}
				buildCompDB.Features = DesktopCompDBGen.CompDBFeatures();
				buildCompDB.Packages = DesktopCompDBGen.CompDBPackages();
			}
			catch (Exception ex)
			{
				DesktopCompDBGen._logger.LogException(ex);
				Environment.Exit(Marshal.GetHRForException(ex));
			}
			return buildCompDB;
		}

		// Token: 0x0600009C RID: 156 RVA: 0x000077C0 File Offset: 0x000059C0
		public static string PackageIDFromAssemblyIdentity(AssemblyIdentity ai)
		{
			return string.Format("{0}_{1}{2}", ai.processorArchitecture, ai.name, ai.language.Equals("Neutral", StringComparison.OrdinalIgnoreCase) ? "" : ("_" + ai.language));
		}

		// Token: 0x0600009D RID: 157 RVA: 0x00007810 File Offset: 0x00005A10
		private static List<CompDBFeature> CompDBFeatures()
		{
			List<CompDBFeature> list = DesktopCompDBGen._featurePkgMap.Keys.ToList<CompDBFeature>();
			DesktopCompDBGen._logger.LogInfo("ImageCommon::DesktopCompDBGen: Generated {0} DesktopCompDB Features", new object[]
			{
				list.Count
			});
			return list;
		}

		// Token: 0x0600009E RID: 158 RVA: 0x00007851 File Offset: 0x00005A51
		private static List<CompDBPackageInfo> CompDBPackages()
		{
			DesktopCompDBGen._logger.LogInfo("ImageCommon::DesktopCompDBGen: Generated {0} DesktopCompDB Packages", new object[]
			{
				DesktopCompDBGen._pkgMap.Values.Count<CompDBPackageInfo>()
			});
			return DesktopCompDBGen._pkgMap.Values.ToList<CompDBPackageInfo>();
		}

		// Token: 0x0600009F RID: 159 RVA: 0x00007890 File Offset: 0x00005A90
		private static void CompDBForAllEditions(string pkgPath, string lang, string pkgRoot)
		{
			if (!Directory.Exists(pkgPath))
			{
				throw new DirectoryNotFoundException("ImageCommon::DesktopCompDBGen!CompDBForAllEditions: Invalid package path:" + pkgPath);
			}
			string editionsPkgMap = Path.Combine(pkgPath, DesktopCompDBGen.c_EditionPackageMap);
			Dictionary<string, List<string>> dictionary = DesktopCompDBGen.GenerateEditionsMap(lang, editionsPkgMap);
			XDocument xdocument = XDocument.Load(LongPath.Combine(DesktopCompDBGen._sdxRoot, DesktopCompDBGen.c_WindowsEditionsMap));
			XNamespace RNS = "urn:schemas-microsoft-com:windows:editions:v1";
			using (Dictionary<string, List<string>>.KeyCollection.Enumerator enumerator = dictionary.Keys.GetEnumerator())
			{
				Func<XElement, string> <>9__1;
				while (enumerator.MoveNext())
				{
					string edition = enumerator.Current;
					IEnumerable<XElement> source = from e1 in xdocument.Root.Elements(RNS + "Edition")
					where e1.Element(RNS + "Name").Value.ToString().Equals(edition, StringComparison.InvariantCultureIgnoreCase) && e1.Element(RNS + "Media") != null && (e1.Attribute(RNS + "legacy") == null || !e1.Attribute(RNS + "legacy").Value.Equals("true"))
					select e1;
					Func<XElement, string> selector;
					if ((selector = <>9__1) == null)
					{
						selector = (<>9__1 = ((XElement e1) => e1.Element(RNS + "Media").Value));
					}
					string text = source.Select(selector).First<string>();
					DesktopCompDBGen.ClientTypes key;
					if (Enum.TryParse<DesktopCompDBGen.ClientTypes>(text, out key))
					{
						if (!DesktopCompDBGen._clientEditionsFeatures.ContainsKey(key))
						{
							DesktopCompDBGen._clientEditionsFeatures.Add(key, new List<CompDBFeature>());
						}
						string featureId = edition + "_" + lang;
						using (List<string>.Enumerator enumerator2 = dictionary[edition].GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								string path = enumerator2.Current;
								string text2 = Path.Combine(pkgRoot, path);
								if (LongPathFile.Exists(text2))
								{
									AssemblyIdentity assemblyIdMum = DesktopCompDBGen.GetAssemblyIdMum(Path.ChangeExtension(text2, PkgConstants.c_strMumExtension));
									DesktopCompDBGen._editionVersionLookup[key] = assemblyIdMum.version;
									CompDBFeature item = DesktopCompDBGen.GenerateFeaturePackageMapping(featureId, assemblyIdMum, text2, lang, pkgRoot, DesktopCompDBGen.PackageTypes.FOD, null, false);
									if (!DesktopCompDBGen._clientEditionsFeatures[key].Contains(item))
									{
										DesktopCompDBGen._clientEditionsFeatures[key].Add(item);
									}
								}
							}
							continue;
						}
					}
					throw new ImageCommonException("ImageCommon::DesktopCompDBGen!CompDBForAllEditions: Unknown Client Type: " + text);
				}
			}
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x00007AE8 File Offset: 0x00005CE8
		private static void CompDBForOtherPackages(string pkgPathRoot, DesktopCompDBGen.PackageTypes pkgType, string lang, string pkgRoot)
		{
			List<string> list = new List<string>();
			list.Add(DesktopCompDBGen.c_WimExtenstion);
			list.Add(DesktopCompDBGen.c_EsdExtenstion);
			list.Add(DesktopCompDBGen.c_CabExtension);
			string path = pkgPathRoot;
			if (DesktopCompDBGen._generatingFMs && pkgType == DesktopCompDBGen.PackageTypes.LANGPACK && !lang.Equals("en-us", StringComparison.OrdinalIgnoreCase))
			{
				path = pkgPathRoot.Replace(lang, "en-us", StringComparison.OrdinalIgnoreCase);
			}
			foreach (string str in list)
			{
				foreach (string text in Directory.GetFiles(path, "*" + str))
				{
					AssemblyIdentity assemblyIdentity = null;
					string text2 = null;
					string text3 = text;
					if (pkgType == DesktopCompDBGen.PackageTypes.LANGPACK)
					{
						DesktopCompDBGen.GetAssemblyIdAndFeatureIdFromCab(text3, out assemblyIdentity, out text2);
						if (string.IsNullOrEmpty(text2))
						{
							text2 = DesktopCompDBGen.c_LanguagePackPrefix + "~" + lang;
						}
						if (!lang.Equals("en-us", StringComparison.OrdinalIgnoreCase))
						{
							text3 = text3.Replace("en-us", lang, StringComparison.OrdinalIgnoreCase);
							text2 = text2.Replace("en-us", lang, StringComparison.OrdinalIgnoreCase);
							assemblyIdentity.language = lang;
						}
					}
					else
					{
						assemblyIdentity = DesktopCompDBGen.GetAssemblyIdFromPkg(text);
					}
					DesktopCompDBGen.GenerateFeaturePackageMapping(DesktopCompDBGen.ClientTypes.Client + "_" + lang, assemblyIdentity, text3, lang, pkgRoot, pkgType, text2, false);
					DesktopCompDBGen.GenerateFeaturePackageMapping(DesktopCompDBGen.ClientTypes.ClientN + "_" + lang, assemblyIdentity, text3, lang, pkgRoot, DesktopCompDBGen.PackageTypes.FOD, null, false);
				}
			}
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x00007C78 File Offset: 0x00005E78
		private static void GenerateCompDB(string pkgPath, DesktopCompDBGen.PackageTypes pkgType, string lang, string pkgRoot, string editionsMap)
		{
			if (string.IsNullOrEmpty(editionsMap) || !LongPathFile.Exists(editionsMap))
			{
				throw new ImageCommonException("ImageCommon::DesktopCompDBGen!GenerateCompDB: Invalid editions map specified: " + editionsMap);
			}
			Dictionary<string, List<string>> dictionary = DesktopCompDBGen.GenerateEditionsMap(lang, editionsMap);
			foreach (string text in dictionary.Keys)
			{
				foreach (string file in dictionary[text])
				{
					string text2 = LongPath.Combine(pkgPath, file);
					if (!LongPathFile.Exists(text2))
					{
						text2 = Path.ChangeExtension(text2, DesktopCompDBGen.c_EsdExtenstion);
					}
					if (LongPathFile.Exists(text2))
					{
						AssemblyIdentity ai = null;
						string optionalFeatureId = null;
						if (pkgType == DesktopCompDBGen.PackageTypes.FOD)
						{
							DesktopCompDBGen.GetAssemblyIdAndFeatureIdFromCab(text2, out ai, out optionalFeatureId);
						}
						else
						{
							ai = DesktopCompDBGen.GetAssemblyIdFromPkg(text2);
						}
						DesktopCompDBGen.GenerateFeaturePackageMapping(text + "_" + lang, ai, text2, lang, pkgRoot, pkgType, optionalFeatureId, false);
						DesktopCompDBGen._processedOptionalPkgs.Add(text2);
					}
				}
			}
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x00007DAC File Offset: 0x00005FAC
		private static void GenerateCompDBForRemainingOptionalPkgs(string pkgPath, DesktopCompDBGen.PackageTypes pkgType, string lang, string pkgRoot)
		{
			foreach (string pkgPath2 in Directory.GetFiles(pkgPath, "*" + DesktopCompDBGen.c_CabExtension))
			{
				if (!DesktopCompDBGen._processedOptionalPkgs.Contains(pkgPath))
				{
					AssemblyIdentity ai = null;
					string text = null;
					if (pkgType == DesktopCompDBGen.PackageTypes.FOD)
					{
						DesktopCompDBGen.GetAssemblyIdAndFeatureIdFromCab(pkgPath2, out ai, out text);
						if (!string.IsNullOrEmpty(text))
						{
							DesktopCompDBGen.GenerateFeaturePackageMapping(string.Empty, ai, pkgPath2, lang, pkgRoot, pkgType, text, true);
						}
					}
				}
			}
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x00007E20 File Offset: 0x00006020
		private static Dictionary<string, List<string>> GenerateEditionsMap(string lang, string editionsPkgMap)
		{
			DesktopCompDBGen.<>c__DisplayClass71_0 CS$<>8__locals1 = new DesktopCompDBGen.<>c__DisplayClass71_0();
			CS$<>8__locals1.lang = lang;
			XDocument xdocument = XDocument.Load(editionsPkgMap);
			Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
			Dictionary<string, HashSet<string>> dictionary2 = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
			if (DesktopCompDBGen._clientEditionsFeatures.Count<KeyValuePair<DesktopCompDBGen.ClientTypes, List<CompDBFeature>>>() != 0)
			{
				foreach (KeyValuePair<DesktopCompDBGen.ClientTypes, List<CompDBFeature>> keyValuePair in DesktopCompDBGen._clientEditionsFeatures)
				{
					string key = keyValuePair.Key.ToString();
					dictionary2.Add(key, new HashSet<string>(StringComparer.OrdinalIgnoreCase));
					foreach (CompDBFeature compDBFeature in keyValuePair.Value)
					{
						string text = compDBFeature.FeatureID.Replace("_" + CS$<>8__locals1.lang, "", StringComparison.OrdinalIgnoreCase);
						if (!dictionary.ContainsKey(text))
						{
							dictionary.Add(text, new List<string>());
							dictionary2[key].Add(text);
						}
					}
				}
			}
			XDocument xdocument2 = XDocument.Load(LongPath.Combine(DesktopCompDBGen._sdxRoot, DesktopCompDBGen.c_WindowsEditionsMap));
			CS$<>8__locals1.RNS = "urn:schemas-microsoft-com:windows:editions:v1";
			CS$<>8__locals1.media = (from e1 in xdocument2.Root.Elements(CS$<>8__locals1.RNS + "Edition")
			where e1.Element(CS$<>8__locals1.RNS + "Media") != null && (e1.Attribute(CS$<>8__locals1.RNS + "legacy") == null || !e1.Attribute(CS$<>8__locals1.RNS + "legacy").Value.Equals("true"))
			select e1.Element(CS$<>8__locals1.RNS + "Media").Value).Distinct<string>().ToList<string>();
			foreach (XElement xelement in from p in xdocument.Root.Elements("Product")
			where CS$<>8__locals1.media.Contains(p.Attribute("name").Value.ToString())
			select p)
			{
				string value = xelement.Attribute("name").Value;
				List<string> list = new List<string>();
				foreach (XElement xelement2 in xelement.Elements("Edition"))
				{
					List<string> list2 = new List<string>();
					foreach (XElement xelement3 in xelement2.Elements("Package"))
					{
						IEnumerable<XElement> source = xelement3.Elements("lang");
						Func<XElement, bool> predicate;
						if ((predicate = CS$<>8__locals1.<>9__3) == null)
						{
							DesktopCompDBGen.<>c__DisplayClass71_0 CS$<>8__locals2 = CS$<>8__locals1;
							predicate = (CS$<>8__locals2.<>9__3 = ((XElement x) => x.Value.ToString().Equals(CS$<>8__locals2.lang) || x.Value.ToString().Equals("*")));
						}
						if (source.FirstOrDefault(predicate) != null)
						{
							list2.Add(xelement3.Attribute("name").Value);
						}
					}
					if (list2.Count<string>() > 0)
					{
						string value2 = xelement2.Attribute("name").Value;
						bool flag = false;
						if (xelement2.Attribute("ignoreAllSection") != null && xelement2.Attribute("ignoreAllSection").Value.Equals("true", StringComparison.OrdinalIgnoreCase))
						{
							flag = true;
						}
						if (value2.Equals("All", StringComparison.OrdinalIgnoreCase))
						{
							list = list2;
						}
						else
						{
							if (dictionary.ContainsKey(value2))
							{
								dictionary[value2].AddRange(list2);
								DesktopCompDBGen._logger.LogInfo("ImageCommon::DesktopCompDBGen: Adding to editionsMap. Edition: {0}, Num of packages: {1}", new object[]
								{
									value2,
									list2.Count
								});
							}
							else
							{
								dictionary.Add(value2, list2);
								DesktopCompDBGen._logger.LogInfo("ImageCommon::DesktopCompDBGen: Adding to editionsMap. Edition: {0}, Num of packages: {1}", new object[]
								{
									value2,
									list2.Count
								});
							}
							if (flag)
							{
								dictionary2[value].Remove(value2);
							}
						}
					}
				}
				if (dictionary2.Any<KeyValuePair<string, HashSet<string>>>())
				{
					foreach (string text2 in dictionary2[value])
					{
						dictionary[text2].AddRange(list);
						DesktopCompDBGen._logger.LogInfo("ImageCommon::DesktopCompDBGen: Adding to editionsMap. Edition: {0}, Num of packages: {1}", new object[]
						{
							text2,
							list.Count
						});
					}
				}
			}
			return dictionary;
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x00008308 File Offset: 0x00006508
		public static AssemblyIdentity GetAssemblyIdFromPkg(string pkgPath)
		{
			if (!DesktopCompDBGen._bInitialized)
			{
				throw new Exception("ImageCommon::DesktopCompDBGen!GetAssemblyIdFromPkg: Function called before being initialized.");
			}
			if (Path.GetExtension(pkgPath).Equals(DesktopCompDBGen.c_WimExtenstion, StringComparison.OrdinalIgnoreCase) || Path.GetExtension(pkgPath).Equals(DesktopCompDBGen.c_EsdExtenstion, StringComparison.OrdinalIgnoreCase))
			{
				return DesktopCompDBGen.GetAssemblyIdFromWim(pkgPath);
			}
			if (Path.GetExtension(pkgPath).Equals(DesktopCompDBGen.c_CabExtension, StringComparison.OrdinalIgnoreCase))
			{
				return DesktopCompDBGen.GetAssemblyIdFromCab(pkgPath);
			}
			throw new ImageCommonException("ImageCommon::DesktopCompDBGen!GetAssemblyIdFromPkg: Invalid package type:" + Path.GetExtension(pkgPath));
		}

		// Token: 0x060000A5 RID: 165 RVA: 0x00008384 File Offset: 0x00006584
		private static AssemblyIdentity GetAssemblyIdFromWim(string pkgPath)
		{
			string text = LongPath.Combine(Path.GetTempPath(), PkgConstants.c_strMumFile);
			Process process = new Process();
			process.StartInfo.FileName = LongPath.Combine(DesktopCompDBGen._sdxRoot, "tools\\PostBuildScripts\\DistribData\\esd\\tools\\x64\\esdtool.exe");
			process.StartInfo.Arguments = " /wimextract " + pkgPath + " 1";
			ProcessStartInfo startInfo = process.StartInfo;
			startInfo.Arguments = string.Concat(new string[]
			{
				startInfo.Arguments,
				" ",
				PkgConstants.c_strMumFile,
				" ",
				text
			});
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.CreateNoWindow = true;
			process.StartInfo.RedirectStandardOutput = true;
			DesktopCompDBGen._logger.LogInfo("ImageCommon::DesktopCompDBGen: Calling ESDTool with arguments: " + process.StartInfo.Arguments, new object[0]);
			process.Start();
			process.StandardOutput.ReadToEnd();
			process.WaitForExit();
			XElement xelement;
			if (process.ExitCode == 0)
			{
				XDocument xdocument = XDocument.Load(text);
				XNamespace @namespace = xdocument.Root.Name.Namespace;
				xelement = xdocument.Root.Element(@namespace + "assemblyIdentity");
			}
			else
			{
				DesktopCompDBGen._logger.LogInfo("ImageCommon::DesktopCompDBGen: EsdTool_exit_code: {0}", new object[]
				{
					process.ExitCode.ToString()
				});
				xelement = new XElement("urn:schemas-microsoft-com:asm.v3" + "assemblyIdentity");
				xelement.Add(new XAttribute("buildType", "release"));
				xelement.Add(new XAttribute("language", "neutral"));
				xelement.Add(new XAttribute("name", Path.GetFileNameWithoutExtension(pkgPath)));
				xelement.Add(new XAttribute("processorArchitecture", DesktopCompDBGen._buildArch));
				xelement.Add(new XAttribute("publicKeyToken", "0"));
				xelement.Add(new XAttribute("version", "00.0.00000.0000"));
			}
			return DesktopCompDBGen.GetAssemblyIdentity(xelement);
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x000085A0 File Offset: 0x000067A0
		private static void GetAssemblyIdAndFeatureIdFromCab(string pkgPath, out AssemblyIdentity ai, out string featureId)
		{
			string mumFileFromCab = DesktopCompDBGen.GetMumFileFromCab(pkgPath);
			ai = DesktopCompDBGen.GetAssemblyIdMum(mumFileFromCab);
			featureId = DesktopCompDBGen.GetFeatureIdFromMum(mumFileFromCab);
		}

		// Token: 0x060000A7 RID: 167 RVA: 0x000085C4 File Offset: 0x000067C4
		private static string GetMumFileFromCab(string pkgPath)
		{
			string tempFile = FileUtils.GetTempFile();
			Directory.CreateDirectory(tempFile);
			string text = LongPath.Combine(tempFile, PkgConstants.c_strMumFile);
			Process process = new Process();
			process.StartInfo.FileName = Environment.ExpandEnvironmentVariables("%systemroot%\\system32\\expand.exe");
			process.StartInfo.Arguments = " " + pkgPath;
			ProcessStartInfo startInfo = process.StartInfo;
			startInfo.Arguments = startInfo.Arguments + " -F:" + PkgConstants.c_strMumFile;
			startInfo = process.StartInfo;
			startInfo.Arguments = startInfo.Arguments + " " + tempFile;
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.CreateNoWindow = true;
			process.StartInfo.RedirectStandardOutput = true;
			DesktopCompDBGen._logger.LogInfo("ImageCommon::DesktopCompDBGen: Calling Expand.exe with arguments: " + process.StartInfo.Arguments, new object[0]);
			process.Start();
			DesktopCompDBGen._logger.LogInfo(process.StandardOutput.ReadToEnd(), new object[0]);
			process.WaitForExit();
			DesktopCompDBGen._logger.LogInfo("ImageCommon::DesktopCompDBGen: Expand.exe ExitCode: {0}", new object[]
			{
				process.ExitCode.ToString()
			});
			if (process.ExitCode != 0 || !LongPathFile.Exists(text))
			{
				throw new ImageCommonException("ImageCommon::DesktopCompDBGen!GetMumFileFromCab: Unable to extract update.mum from :" + pkgPath);
			}
			return text;
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x00008715 File Offset: 0x00006915
		private static AssemblyIdentity GetAssemblyIdFromCab(string pkgPath)
		{
			return DesktopCompDBGen.GetAssemblyIdMum(DesktopCompDBGen.GetMumFileFromCab(pkgPath));
		}

		// Token: 0x060000A9 RID: 169 RVA: 0x00008724 File Offset: 0x00006924
		private static AssemblyIdentity GetAssemblyIdMum(string mumPath)
		{
			XDocument xdocument = XDocument.Load(mumPath);
			XNamespace @namespace = xdocument.Root.Name.Namespace;
			return DesktopCompDBGen.GetAssemblyIdentity(xdocument.Root.Element(@namespace + "assemblyIdentity"));
		}

		// Token: 0x060000AA RID: 170 RVA: 0x00008764 File Offset: 0x00006964
		private static string GetFeatureIdFromMum(string mumPath)
		{
			XDocument xdocument = XDocument.Load(mumPath);
			XNamespace @namespace = xdocument.Root.Name.Namespace;
			XElement xelement = xdocument.Root.Element(@namespace + "package");
			bool flag = xelement.Element(@namespace + "declareCapability") != null;
			bool flag2 = xelement.Element(@namespace + "parent") != null && xelement.Element(@namespace + "parent").Element(@namespace + "assemblyIdentity").Attribute("name").Value.ToString().Contains("NanoServer-Edition");
			if (flag && !flag2)
			{
				XElement xelement2 = xdocument.Root.Element(@namespace + "package").Element(@namespace + "declareCapability").Element(@namespace + "capability").Element(@namespace + "capabilityIdentity");
				string value = xelement2.Attribute("name").Value;
				string text = null;
				string text2 = null;
				if (xelement2.Attribute("language") != null)
				{
					text = xelement2.Attribute("language").Value;
				}
				if (xelement2.Attribute("version") != null)
				{
					text2 = xelement2.Attribute("version").Value;
				}
				return value + "~" + (string.IsNullOrEmpty(text) ? "" : text) + (string.IsNullOrEmpty(text2) ? "" : ("~" + text2));
			}
			return string.Empty;
		}

		// Token: 0x060000AB RID: 171 RVA: 0x00008912 File Offset: 0x00006B12
		public static AssemblyIdentity GetAssemblyIdentity(XElement assemblyIdentity)
		{
			return (AssemblyIdentity)new XmlSerializer(typeof(AssemblyIdentity)).Deserialize(assemblyIdentity.CreateReader());
		}

		// Token: 0x060000AC RID: 172 RVA: 0x00008934 File Offset: 0x00006B34
		public static CompDBPackageInfo GenerateFeaturePackage(CompDBFeature feature, AssemblyIdentity ai, string pkgPath, string lang, string pkgRoot)
		{
			string id = DesktopCompDBGen.PackageIDFromAssemblyIdentity(ai);
			return DesktopCompDBGen.GenerateFeaturePackage(feature, id, ai, pkgPath, lang, pkgRoot);
		}

		// Token: 0x060000AD RID: 173 RVA: 0x00008954 File Offset: 0x00006B54
		public static CompDBPackageInfo GenerateFeaturePackage(CompDBFeature feature, string id, AssemblyIdentity ai, string pkgPath, string lang, string pkgRoot)
		{
			CompDBFeaturePackage featurePackage = new CompDBFeaturePackage();
			CompDBPackageInfo compDBPackageInfo = new CompDBPackageInfo();
			compDBPackageInfo.OwnerType = OwnerType.Microsoft;
			compDBPackageInfo.ReleaseType = ReleaseType.Production;
			CompDBFeaturePackage featurePackage2 = featurePackage;
			compDBPackageInfo.ID = id;
			featurePackage2.ID = id;
			if (feature.Packages.Any((CompDBFeaturePackage pkg) => pkg.ID.Equals(featurePackage.ID, StringComparison.OrdinalIgnoreCase)))
			{
				return null;
			}
			if (!ai.processorArchitecture.Equals(DesktopCompDBGen._buildArch, StringComparison.OrdinalIgnoreCase))
			{
				compDBPackageInfo.BuildArchOverride = ai.processorArchitecture;
			}
			if (!string.IsNullOrEmpty(ai.language) && !ai.language.Equals("neutral", StringComparison.OrdinalIgnoreCase))
			{
				compDBPackageInfo.SatelliteType = CompDBPackageInfo.SatelliteTypes.Language;
				compDBPackageInfo.SatelliteValue = ai.language;
			}
			compDBPackageInfo.PublicKeyToken = ai.publicKeyToken;
			VersionInfo version = default(VersionInfo);
			if (VersionInfo.TryParse(ai.version, out version))
			{
				compDBPackageInfo.Version = version;
			}
			CompDBPayloadInfo compDBPayloadInfo = new CompDBPayloadInfo(pkgPath, pkgRoot, compDBPackageInfo, false);
			compDBPackageInfo.Payload.Add(compDBPayloadInfo);
			if (!DesktopCompDBGen._generatingFMs && DesktopCompDBGen._generateHashes)
			{
				string key = compDBPayloadInfo.Path.ToUpper(CultureInfo.InvariantCulture);
				if (DesktopCompDBGen._pkgHashs.ContainsKey(key))
				{
					compDBPayloadInfo.PayloadHash = DesktopCompDBGen._pkgHashs[key];
					compDBPayloadInfo.PayloadSize = DesktopCompDBGen._pkgSizes[key];
				}
				else
				{
					compDBPayloadInfo.PayloadHash = CompDBPackageInfo.GetPackageHash(pkgPath);
					DesktopCompDBGen._pkgHashs.Add(key, compDBPayloadInfo.PayloadHash);
					compDBPayloadInfo.PayloadSize = CompDBPackageInfo.GetPackageSize(pkgPath);
					DesktopCompDBGen._pkgSizes.Add(key, compDBPayloadInfo.PayloadSize);
				}
			}
			if (pkgPath.ToUpper(CultureInfo.InvariantCulture).Contains(DesktopCompDBGen.c_ESDRoot.ToUpper(CultureInfo.InvariantCulture)))
			{
				featurePackage.PackageType = CompDBFeaturePackage.PackageTypes.MetadataESD;
			}
			feature.Packages.Add(featurePackage);
			return compDBPackageInfo;
		}

		// Token: 0x060000AE RID: 174 RVA: 0x00008B20 File Offset: 0x00006D20
		public static void PopulateHashesLookup(BuildCompDB srcCompDB)
		{
			foreach (CompDBPackageInfo compDBPackageInfo in srcCompDB.Packages)
			{
				foreach (CompDBPayloadInfo compDBPayloadInfo in compDBPackageInfo.Payload)
				{
					string key = compDBPayloadInfo.Path.ToUpper(CultureInfo.InvariantCulture);
					if (!string.IsNullOrEmpty(compDBPayloadInfo.PayloadHash) && !DesktopCompDBGen._pkgHashs.ContainsKey(key))
					{
						DesktopCompDBGen._pkgHashs.Add(key, compDBPayloadInfo.PayloadHash);
					}
					if (compDBPayloadInfo.PayloadSize > 0L && !DesktopCompDBGen._pkgSizes.ContainsKey(key))
					{
						DesktopCompDBGen._pkgSizes.Add(key, compDBPayloadInfo.PayloadSize);
					}
				}
			}
		}

		// Token: 0x060000AF RID: 175 RVA: 0x00008C10 File Offset: 0x00006E10
		private static CompDBFeature GenerateFeaturePackageMapping(string featureId, AssemblyIdentity ai, string pkgPath, string lang, string pkgRoot, DesktopCompDBGen.PackageTypes pkgType = DesktopCompDBGen.PackageTypes.FOD, string optionalFeatureId = null, bool optionalFeatureOnly = false)
		{
			DesktopCompDBGen.<>c__DisplayClass83_0 CS$<>8__locals1 = new DesktopCompDBGen.<>c__DisplayClass83_0();
			CS$<>8__locals1.featureId = featureId;
			CS$<>8__locals1.optionalFeatureId = optionalFeatureId;
			CompDBFeature compDBFeature = new CompDBFeature();
			CompDBPackageInfo compDBPackageInfo = DesktopCompDBGen.GenerateFeaturePackage(compDBFeature, ai, pkgPath, lang, pkgRoot);
			CS$<>8__locals1.featurePackage = compDBFeature.FindPackage(compDBPackageInfo.ID);
			if (!optionalFeatureOnly)
			{
				DesktopCompDBGen.ClientTypes key;
				if (Enum.TryParse<DesktopCompDBGen.ClientTypes>(CS$<>8__locals1.featureId.Replace("_" + lang, "", StringComparison.OrdinalIgnoreCase), out key))
				{
					if (DesktopCompDBGen._clientEditionsFeatures[key] == null)
					{
						throw new ImageCommonException("ImageCommon::DesktopCompDBGen!GenerateFeaturePackageMapping: The client '" + key.ToString() + "' was not found.  Ensure all Editions are processed before doing feature mapping for FOD\\Apps.");
					}
					using (List<CompDBFeature>.Enumerator enumerator = DesktopCompDBGen._clientEditionsFeatures[key].GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							CompDBFeature key2 = enumerator.Current;
							IEnumerable<CompDBFeaturePackage> source = DesktopCompDBGen._featurePkgMap[key2];
							Func<CompDBFeaturePackage, bool> predicate;
							if ((predicate = CS$<>8__locals1.<>9__0) == null)
							{
								DesktopCompDBGen.<>c__DisplayClass83_0 CS$<>8__locals2 = CS$<>8__locals1;
								predicate = (CS$<>8__locals2.<>9__0 = ((CompDBFeaturePackage pkg) => pkg.ID.Equals(CS$<>8__locals2.featurePackage.ID, StringComparison.OrdinalIgnoreCase)));
							}
							if (!source.Any(predicate))
							{
								DesktopCompDBGen._featurePkgMap[key2].Add(CS$<>8__locals1.featurePackage);
							}
						}
						goto IL_1A9;
					}
				}
				CompDBFeature compDBFeature2 = DesktopCompDBGen._featurePkgMap.Keys.FirstOrDefault((CompDBFeature feat) => feat.FeatureID.Equals(CS$<>8__locals1.featureId, StringComparison.OrdinalIgnoreCase));
				if (compDBFeature2 == null)
				{
					CompDBFeature compDBFeature3 = new CompDBFeature(CS$<>8__locals1.featureId, null, CompDBFeature.CompDBFeatureTypes.DesktopMedia, OwnerType.Microsoft.ToString());
					Dictionary<CompDBFeature, List<CompDBFeaturePackage>> featurePkgMap = DesktopCompDBGen._featurePkgMap;
					CompDBFeature compDBFeature4 = compDBFeature3;
					featurePkgMap.Add(compDBFeature4, compDBFeature4.Packages);
					compDBFeature = compDBFeature3;
				}
				else
				{
					compDBFeature = compDBFeature2;
				}
				if (!DesktopCompDBGen._featurePkgMap[compDBFeature].Any((CompDBFeaturePackage pkg) => pkg.ID.Equals(CS$<>8__locals1.featurePackage.ID, StringComparison.OrdinalIgnoreCase)))
				{
					DesktopCompDBGen._featurePkgMap[compDBFeature].Add(CS$<>8__locals1.featurePackage);
				}
			}
			IL_1A9:
			if (!string.IsNullOrEmpty(CS$<>8__locals1.optionalFeatureId) && DesktopCompDBGen._featurePkgMap.Keys.FirstOrDefault((CompDBFeature feat) => feat.FeatureID.Equals(CS$<>8__locals1.optionalFeatureId, StringComparison.OrdinalIgnoreCase)) == null)
			{
				CompDBFeature.CompDBFeatureTypes type = (pkgType == DesktopCompDBGen.PackageTypes.LANGPACK) ? CompDBFeature.CompDBFeatureTypes.LanguagePack : CompDBFeature.CompDBFeatureTypes.OnDemandFeature;
				CompDBFeature compDBFeature5 = new CompDBFeature(CS$<>8__locals1.optionalFeatureId, null, type, OwnerType.Microsoft.ToString());
				compDBFeature5.Packages.Add(CS$<>8__locals1.featurePackage);
				Dictionary<CompDBFeature, List<CompDBFeaturePackage>> featurePkgMap2 = DesktopCompDBGen._featurePkgMap;
				CompDBFeature compDBFeature6 = compDBFeature5;
				featurePkgMap2.Add(compDBFeature6, compDBFeature6.Packages);
				DesktopCompDBGen._logger.LogInfo("ImageCommon::DesktopCompDBGen: Generating feature package mapping. Feature: {0}, Package: {1}", new object[]
				{
					CS$<>8__locals1.optionalFeatureId,
					CS$<>8__locals1.featurePackage.ID
				});
			}
			if (!DesktopCompDBGen._pkgMap.ContainsKey(compDBPackageInfo.ID))
			{
				DesktopCompDBGen._pkgMap[compDBPackageInfo.ID] = compDBPackageInfo;
			}
			DesktopCompDBGen._logger.LogInfo("ImageCommon::DesktopCompDBGen: Generating feature package mapping. Feature: {0}, Package: {1}", new object[]
			{
				CS$<>8__locals1.featureId,
				CS$<>8__locals1.featurePackage.ID
			});
			return compDBFeature;
		}

		// Token: 0x060000B0 RID: 176 RVA: 0x00008ED0 File Offset: 0x000070D0
		public static void FilterCompDB(BuildCompDB compDB, List<DesktopCompDBGen.DesktopFilters> filters, List<string> features)
		{
			if (filters.Contains(DesktopCompDBGen.DesktopFilters.IncludeNone))
			{
				compDB.Features = new List<CompDBFeature>();
				compDB.Packages = new List<CompDBPackageInfo>();
				return;
			}
			List<CompDBFeature> list = new List<CompDBFeature>();
			foreach (CompDBFeature compDBFeature in compDB.Features)
			{
				switch (compDBFeature.Type)
				{
				case CompDBFeature.CompDBFeatureTypes.DesktopMedia:
					if (filters.Contains(DesktopCompDBGen.DesktopFilters.IncludeEditions))
					{
						list.Add(compDBFeature);
						continue;
					}
					break;
				case CompDBFeature.CompDBFeatureTypes.OnDemandFeature:
					if (filters.Contains(DesktopCompDBGen.DesktopFilters.IncludeOnDemandFeatures))
					{
						list.Add(compDBFeature);
						continue;
					}
					break;
				case CompDBFeature.CompDBFeatureTypes.LanguagePack:
					if (filters.Contains(DesktopCompDBGen.DesktopFilters.IncludeLanguagePacks))
					{
						list.Add(compDBFeature);
						continue;
					}
					break;
				}
				if (filters.Contains(DesktopCompDBGen.DesktopFilters.IncludeByFeatureIDs) && features.Contains(compDBFeature.FeatureIDWithFMID, StringComparer.OrdinalIgnoreCase))
				{
					list.Add(compDBFeature);
				}
			}
			if (filters.Contains(DesktopCompDBGen.DesktopFilters.CreateNeutralFeature))
			{
				CompDBFeature compDBFeature2 = new CompDBFeature(DesktopCompDBGen.c_BaseNeutralFeatureID, null, CompDBFeature.CompDBFeatureTypes.None, OwnerType.Microsoft.ToString());
				compDBFeature2.Packages = (from pkg in compDB.Packages
				where pkg.SatelliteType == CompDBPackageInfo.SatelliteTypes.Base
				select new CompDBFeaturePackage(pkg.ID, false)).ToList<CompDBFeaturePackage>();
				List<string> alreadyInOtherFeatures = list.SelectMany((CompDBFeature feat) => from pkg in feat.Packages
				select pkg.ID).Distinct(StringComparer.OrdinalIgnoreCase).ToList<string>();
				compDBFeature2.Packages.RemoveAll((CompDBFeaturePackage pkg) => alreadyInOtherFeatures.Contains(pkg.ID, StringComparer.OrdinalIgnoreCase));
				if (compDBFeature2.Packages.Any<CompDBFeaturePackage>())
				{
					list.Add(compDBFeature2);
				}
			}
			if (filters.Contains(DesktopCompDBGen.DesktopFilters.CreateLanguageFeature))
			{
				string satelliteValue = compDB.Packages.FirstOrDefault((CompDBPackageInfo pkg) => pkg.SatelliteType == CompDBPackageInfo.SatelliteTypes.Language).SatelliteValue;
				CompDBFeature compDBFeature3 = new CompDBFeature(DesktopCompDBGen.c_BaseLanguageFeatureID + satelliteValue, null, CompDBFeature.CompDBFeatureTypes.None, OwnerType.Microsoft.ToString());
				compDBFeature3.Packages = (from pkg in compDB.Packages
				where pkg.SatelliteType == CompDBPackageInfo.SatelliteTypes.Language
				select new CompDBFeaturePackage(pkg.ID, false)).ToList<CompDBFeaturePackage>();
				List<string> alreadyInOtherFeatures = list.SelectMany((CompDBFeature feat) => from pkg in feat.Packages
				select pkg.ID).Distinct(StringComparer.OrdinalIgnoreCase).ToList<string>();
				compDBFeature3.Packages.RemoveAll((CompDBFeaturePackage pkg) => alreadyInOtherFeatures.Contains(pkg.ID, StringComparer.OrdinalIgnoreCase));
				if (compDBFeature3.Packages.Any<CompDBFeaturePackage>())
				{
					list.Add(compDBFeature3);
				}
			}
			compDB.Features = list;
			List<string> filteredPackageIDs = list.SelectMany((CompDBFeature feat) => from pkg in feat.Packages
			select pkg.ID).Distinct(StringComparer.OrdinalIgnoreCase).ToList<string>();
			compDB.Packages = (from pkg in compDB.Packages
			where filteredPackageIDs.Contains(pkg.ID, StringComparer.OrdinalIgnoreCase)
			select pkg).ToList<CompDBPackageInfo>();
		}

		// Token: 0x04000060 RID: 96
		public static string c_EditionPackagesDirectory = "EditionPackages";

		// Token: 0x04000061 RID: 97
		public static string c_DesktopUpdatesDirectory = "UUP\\Desktop";

		// Token: 0x04000062 RID: 98
		public static string c_FODDirectory = "FeaturesOnDemand";

		// Token: 0x04000063 RID: 99
		public static string c_FODCabs = "Cabs";

		// Token: 0x04000064 RID: 100
		public static string c_AppsDirectory = "Apps";

		// Token: 0x04000065 RID: 101
		public static string c_NeutralDirectory = "Neutral";

		// Token: 0x04000066 RID: 102
		public static string c_ClientDirectory = "Client";

		// Token: 0x04000067 RID: 103
		public static string c_ESDRoot = "UUP\\DESKTOP\\MetadataESDs";

		// Token: 0x04000068 RID: 104
		public static string c_BuildArchVar = "$(buildArch)";

		// Token: 0x04000069 RID: 105
		public static string c_WindowsEditionsMap = "data\\Windowseditions.xml";

		// Token: 0x0400006A RID: 106
		public static string c_EditionPackageMap = "EditionPackageMap.xml";

		// Token: 0x0400006B RID: 107
		public static string c_FODEditionsMapPattern = "FOD_InstallList_" + DesktopCompDBGen.c_BuildArchVar + ".xml";

		// Token: 0x0400006C RID: 108
		public static string c_AppsEditionsMap = "EditionPackageMap.xml";

		// Token: 0x0400006D RID: 109
		public static string c_WindowsDesktopProduct = "Desktop";

		// Token: 0x0400006E RID: 110
		private static string c_CabExtension = PkgConstants.c_strCBSPackageExtension;

		// Token: 0x0400006F RID: 111
		private static string c_WimExtenstion = ".wim";

		// Token: 0x04000070 RID: 112
		private static string c_EsdExtenstion = ".esd";

		// Token: 0x04000071 RID: 113
		private static string c_EsdFeaturePost = "_esd";

		// Token: 0x04000072 RID: 114
		private static string c_ClientEditions = DesktopCompDBGen.ClientTypes.Client + "Editions";

		// Token: 0x04000073 RID: 115
		private static string c_ClientNEditions = DesktopCompDBGen.ClientTypes.ClientN + "Editions";

		// Token: 0x04000074 RID: 116
		private static string c_LanguagePackPrefix = "Language.UI";

		// Token: 0x04000075 RID: 117
		private static string c_BaseNeutralFeatureID = "BaseNeutral";

		// Token: 0x04000076 RID: 118
		private static string c_BaseLanguageFeatureID = "BaseLanguage_";

		// Token: 0x04000077 RID: 119
		public static string c_MicrosoftDesktopConditionsFM_ID = "MSDC";

		// Token: 0x04000078 RID: 120
		public static string c_MicrosoftDesktopNeutralFM = "MicrosoftDesktopNeutralFM.xml";

		// Token: 0x04000079 RID: 121
		public static string c_MicrosoftDesktopNeutralFM_ID = "MSDN";

		// Token: 0x0400007A RID: 122
		public static string c_MicrosoftDesktopLangsFM = "MicrosoftDesktopLangsFM.xml";

		// Token: 0x0400007B RID: 123
		public static string c_MicrosoftDesktopLangsFM_ID = "MSDL";

		// Token: 0x0400007C RID: 124
		public static string c_MicrosoftDesktopEditionsFM = "MicrosoftDesktopEditionsFM.xml";

		// Token: 0x0400007D RID: 125
		public static string c_MicrosoftDesktopEditionsFM_ID = "MSDE";

		// Token: 0x0400007E RID: 126
		public static string c_MicrosoftDesktopToolsFM = "MicrosoftDesktopToolsFM.xml";

		// Token: 0x0400007F RID: 127
		public static string c_MicrosoftDesktopToolsFM_ID = "MSDT";

		// Token: 0x04000080 RID: 128
		public static string c_MicorosftDesktopConditionalFeaturesFMName = "MicrosoftDesktopConditionsFM.xml";

		// Token: 0x04000081 RID: 129
		public static string c_MicorosftDesktopChunksMappingFileName = "MicrosoftDesktopChunksMapping.xml";

		// Token: 0x04000082 RID: 130
		public static string c_UpdateBoxName = "WindowsUpdateBox";

		// Token: 0x04000083 RID: 131
		public static string c_UpdateBoxFilename = DesktopCompDBGen.c_UpdateBoxName + ".exe";

		// Token: 0x04000084 RID: 132
		public static string c_UpdateBoxRoot = "WindowsUpdateBox";

		// Token: 0x04000085 RID: 133
		public static string c_NeutralLanguage = "Neutral";

		// Token: 0x04000086 RID: 134
		private static bool _bInitialized = false;

		// Token: 0x04000087 RID: 135
		private static IULogger _logger = new IULogger();

		// Token: 0x04000088 RID: 136
		private static string _sdxRoot;

		// Token: 0x04000089 RID: 137
		private static string _buildArch;

		// Token: 0x0400008A RID: 138
		private static string _pkgRoot;

		// Token: 0x0400008B RID: 139
		private static bool _generatingFMs = false;

		// Token: 0x0400008C RID: 140
		private static bool _generateHashes = false;

		// Token: 0x0400008D RID: 141
		private static Dictionary<DesktopCompDBGen.ClientTypes, List<CompDBFeature>> _clientEditionsFeatures = new Dictionary<DesktopCompDBGen.ClientTypes, List<CompDBFeature>>();

		// Token: 0x0400008E RID: 142
		private static Dictionary<CompDBFeature, List<CompDBFeaturePackage>> _featurePkgMap = new Dictionary<CompDBFeature, List<CompDBFeaturePackage>>();

		// Token: 0x0400008F RID: 143
		private static Dictionary<string, CompDBPackageInfo> _pkgMap = new Dictionary<string, CompDBPackageInfo>(StringComparer.OrdinalIgnoreCase);

		// Token: 0x04000090 RID: 144
		private static Dictionary<string, string> _pkgHashs = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

		// Token: 0x04000091 RID: 145
		private static Dictionary<string, long> _pkgSizes = new Dictionary<string, long>(StringComparer.OrdinalIgnoreCase);

		// Token: 0x04000092 RID: 146
		private static Dictionary<DesktopCompDBGen.ClientTypes, string> _editionVersionLookup = new Dictionary<DesktopCompDBGen.ClientTypes, string>();

		// Token: 0x04000093 RID: 147
		private static HashSet<string> _processedOptionalPkgs = new HashSet<string>();

		// Token: 0x02000050 RID: 80
		private enum ClientTypes
		{
			// Token: 0x040001E3 RID: 483
			Client,
			// Token: 0x040001E4 RID: 484
			ClientN
		}

		// Token: 0x02000051 RID: 81
		public enum DesktopFilters
		{
			// Token: 0x040001E6 RID: 486
			IncludeNone,
			// Token: 0x040001E7 RID: 487
			IncludeAll,
			// Token: 0x040001E8 RID: 488
			IncludeOnDemandFeatures,
			// Token: 0x040001E9 RID: 489
			IncludeEditions,
			// Token: 0x040001EA RID: 490
			IncludeLanguagePacks,
			// Token: 0x040001EB RID: 491
			IncludeByFeatureIDs,
			// Token: 0x040001EC RID: 492
			CreateNeutralFeature,
			// Token: 0x040001ED RID: 493
			CreateLanguageFeature
		}

		// Token: 0x02000052 RID: 82
		public enum PackageTypes
		{
			// Token: 0x040001EF RID: 495
			EDITION,
			// Token: 0x040001F0 RID: 496
			LANGPACK,
			// Token: 0x040001F1 RID: 497
			FOD,
			// Token: 0x040001F2 RID: 498
			APP,
			// Token: 0x040001F3 RID: 499
			NGEN,
			// Token: 0x040001F4 RID: 500
			ESD,
			// Token: 0x040001F5 RID: 501
			UPDATEBOX
		}
	}
}
