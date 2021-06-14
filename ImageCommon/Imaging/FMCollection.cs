using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.WindowsPhone.FeatureAPI;
using Microsoft.WindowsPhone.ImageUpdate.PkgCommon;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x0200002D RID: 45
	public class FMCollection
	{
		// Token: 0x060001D6 RID: 470 RVA: 0x0000FE7C File Offset: 0x0000E07C
		public void LoadFromManifest(string xmlFile, IULogger logger)
		{
			this.Logger = logger;
			this.Manifest = FMCollectionManifest.ValidateAndLoad(xmlFile, this.Logger);
		}

		// Token: 0x060001D7 RID: 471 RVA: 0x0000FE97 File Offset: 0x0000E097
		public PublishingPackageList GetPublishingPackageList2(string fmDirectory, string msPackageRoot, string buildType, CpuId cpuType, bool cbsBased)
		{
			return this.GetPublishingPackageList2(fmDirectory, msPackageRoot, buildType, cpuType, cbsBased, false);
		}

		// Token: 0x060001D8 RID: 472 RVA: 0x0000FEA8 File Offset: 0x0000E0A8
		public PublishingPackageList GetPublishingPackageList2(string fmDirectory, string msPackageRoot, string buildType, CpuId cpuType, bool cbsBased, bool skipMissingPackages)
		{
			PublishingPackageList publishingPackageList = this.GetPublishingPackageList(fmDirectory, msPackageRoot, buildType, cpuType, skipMissingPackages);
			foreach (PublishingPackageInfo publishingPackageInfo in publishingPackageList.Packages)
			{
				string path = Path.Combine(msPackageRoot, publishingPackageInfo.Path);
				if (FileUtils.IsTargetUpToDate(publishingPackageInfo.Path, Path.ChangeExtension(path, PkgConstants.c_strCBSPackageExtension)))
				{
					publishingPackageInfo.Path = Path.ChangeExtension(publishingPackageInfo.Path, PkgConstants.c_strCBSPackageExtension);
				}
			}
			return publishingPackageList;
		}

		// Token: 0x060001D9 RID: 473 RVA: 0x0000FF40 File Offset: 0x0000E140
		public PublishingPackageList GetPublishingPackageList(string fmDirectory, string msPackageRoot, string buildType, CpuId cpuType)
		{
			return this.GetPublishingPackageList(fmDirectory, msPackageRoot, buildType, cpuType, false);
		}

		// Token: 0x060001DA RID: 474 RVA: 0x0000FF50 File Offset: 0x0000E150
		public PublishingPackageList GetPublishingPackageList(string fmDirectory, string msPackageRoot, string buildType, CpuId cpuType, bool skipMissingPackages)
		{
			PublishingPackageList publishingPackageList = new PublishingPackageList();
			publishingPackageList.MSFeatureGroups = new List<FMFeatureGrouping>();
			publishingPackageList.OEMFeatureGroups = new List<FMFeatureGrouping>();
			if (this.Manifest == null)
			{
				throw new ImageCommonException("ImageCommon!GetPublishingPackageList: Unable to generate Publishing Package List without a FM Collection.");
			}
			publishingPackageList.IsTargetFeatureEnabled = this.Manifest.IsBuildFeatureEnabled;
			if (!this.Manifest.IsBuildFeatureEnabled)
			{
				if (this.Manifest.FeatureIdentifierPackages != null)
				{
					publishingPackageList.FeatureIdentifierPackages = this.Manifest.FeatureIdentifierPackages;
				}
				else
				{
					publishingPackageList.FeatureIdentifierPackages = new List<FeatureIdentifierPackage>();
				}
			}
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = false;
			foreach (FMCollectionItem fmcollectionItem in this.Manifest.FMs)
			{
				if (!string.IsNullOrEmpty(fmcollectionItem.ID))
				{
					if (fmcollectionItem.ID.Equals("MSASOEMFM") && fmcollectionItem.CPUType != CpuId.Invalid)
					{
						fmcollectionItem.CPUType = CpuId.Invalid;
					}
					if ((fmcollectionItem.ID.Equals("IOTVMGen1X86", StringComparison.OrdinalIgnoreCase) || fmcollectionItem.ID.Equals("IOTVMGen1AMD64", StringComparison.OrdinalIgnoreCase)) && fmcollectionItem.SkipForPublishing)
					{
						fmcollectionItem.SkipForPublishing = false;
					}
				}
				if ((cpuType == fmcollectionItem.CPUType || fmcollectionItem.CPUType == CpuId.Invalid) && !fmcollectionItem.SkipForPublishing)
				{
					FeatureManifest featureManifest = new FeatureManifest();
					string text = Environment.ExpandEnvironmentVariables(fmcollectionItem.Path);
					text = text.ToUpper(CultureInfo.InvariantCulture).Replace(FMCollection.c_FMDirectoryVariable, fmDirectory, StringComparison.OrdinalIgnoreCase);
					FeatureManifest.ValidateAndLoad(ref featureManifest, text, this.Logger);
					List<FeatureManifest.FMPkgInfo> list = featureManifest.GetAllPackagesByGroups(this.Manifest.SupportedLanguages, this.Manifest.SupportedLocales, this.Manifest.SupportedResolutions, this.Manifest.GetWowGuestCpuTypes(cpuType), buildType, cpuType.ToString(), msPackageRoot);
					List<PublishingPackageInfo> list2 = new List<PublishingPackageInfo>();
					if (skipMissingPackages)
					{
						List<string> missingPackageFeatures = (from pkg in list
						where !File.Exists(pkg.PackagePath)
						select pkg.FeatureID).ToList<string>();
						list = (from pkg in list
						where !missingPackageFeatures.Contains(pkg.FeatureID, this.IgnoreCase)
						select pkg).ToList<FeatureManifest.FMPkgInfo>();
					}
					StringBuilder stringBuilder2 = new StringBuilder();
					bool flag2 = false;
					using (List<FeatureManifest.FMPkgInfo>.Enumerator enumerator2 = list.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							FeatureManifest.FMPkgInfo pkgInfo = enumerator2.Current;
							if ((from fip in this.Manifest.FeatureIdentifierPackages
							where fip.FeatureID.Equals(pkgInfo.FeatureID, StringComparison.OrdinalIgnoreCase) && fip.ID.Equals(pkgInfo.ID, StringComparison.OrdinalIgnoreCase) && fip.FixUpAction == FeatureIdentifierPackage.FixUpActions.Ignore
							select fip).Count<FeatureIdentifierPackage>() == 0)
							{
								PublishingPackageInfo publishingPackageInfo;
								try
								{
									publishingPackageInfo = new PublishingPackageInfo(pkgInfo, fmcollectionItem, msPackageRoot, fmcollectionItem.UserInstallable);
								}
								catch (FileNotFoundException)
								{
									flag2 = true;
									stringBuilder2.AppendFormat("\t{0}\n", pkgInfo.ID);
									continue;
								}
								if (publishingPackageInfo.IsFeatureIdentifierPackage)
								{
									FeatureIdentifierPackage idPkg = new FeatureIdentifierPackage(publishingPackageInfo);
									if (publishingPackageList.FeatureIdentifierPackages == null)
									{
										publishingPackageList.FeatureIdentifierPackages = new List<FeatureIdentifierPackage>();
									}
									else
									{
										List<FeatureIdentifierPackage> list3 = (from fipPkg in publishingPackageList.FeatureIdentifierPackages
										where string.Equals(fipPkg.ID, idPkg.ID, StringComparison.OrdinalIgnoreCase) && string.Equals(fipPkg.Partition, idPkg.Partition, StringComparison.OrdinalIgnoreCase)
										select fipPkg).ToList<FeatureIdentifierPackage>();
										if (list3.Count<FeatureIdentifierPackage>() > 0)
										{
											StringBuilder stringBuilder3 = new StringBuilder();
											stringBuilder3.Append(Environment.NewLine + idPkg.FeatureID + " : ");
											foreach (FeatureIdentifierPackage featureIdentifierPackage in list3)
											{
												stringBuilder3.Append(Environment.NewLine + "\t" + featureIdentifierPackage.ID);
											}
											throw new AmbiguousArgumentException("Some features have more than one FeatureIdentifierPackage defined: " + stringBuilder3.ToString());
										}
									}
									publishingPackageList.FeatureIdentifierPackages.Add(idPkg);
								}
								list2.Add(publishingPackageInfo);
							}
						}
					}
					if (flag2)
					{
						flag = true;
						stringBuilder.AppendFormat("\nThe FM File '{0}' following package file(s) could not be found: \n {1}", text, stringBuilder2.ToString());
					}
					publishingPackageList.Packages.AddRange(list2);
					if (featureManifest.Features != null)
					{
						if (featureManifest.Features.MSFeatureGroups != null)
						{
							foreach (FMFeatureGrouping fmfeatureGrouping in featureManifest.Features.MSFeatureGroups)
							{
								fmfeatureGrouping.FMID = fmcollectionItem.ID;
								publishingPackageList.MSFeatureGroups.Add(fmfeatureGrouping);
							}
						}
						if (featureManifest.Features.OEMFeatureGroups != null)
						{
							foreach (FMFeatureGrouping fmfeatureGrouping2 in featureManifest.Features.OEMFeatureGroups)
							{
								fmfeatureGrouping2.FMID = fmcollectionItem.ID;
								publishingPackageList.OEMFeatureGroups.Add(fmfeatureGrouping2);
							}
						}
					}
				}
			}
			if (flag)
			{
				throw new ImageCommonException("ImageCommon!GetPublishingPackageList: Errors processing FM File(s):\n" + stringBuilder.ToString());
			}
			this.DoFeatureIDFixUps(ref publishingPackageList);
			if (!this.Manifest.IsBuildFeatureEnabled)
			{
				List<FeatureIdentifierPackage> list4 = new List<FeatureIdentifierPackage>();
				foreach (FeatureIdentifierPackage featureIdentifierPackage2 in publishingPackageList.FeatureIdentifierPackages)
				{
					FeatureIdentifierPackage newFip = featureIdentifierPackage2;
					if (string.IsNullOrEmpty(newFip.FMID))
					{
						PublishingPackageInfo publishingPackageInfo2 = publishingPackageList.Packages.Find((PublishingPackageInfo pkg) => pkg.ID.Equals(newFip.ID, StringComparison.OrdinalIgnoreCase) && pkg.FeatureID.Equals(newFip.FeatureID, StringComparison.OrdinalIgnoreCase));
						if (publishingPackageInfo2 == null)
						{
							throw new ImageCommonException("ImageCommon!GetPublishingPackageList: Unable to find FeatureIdentifierPackage in Package List: " + featureIdentifierPackage2.ID);
						}
						newFip.FMID = publishingPackageInfo2.FMID;
					}
					list4.Add(newFip);
				}
				publishingPackageList.FeatureIdentifierPackages = list4;
			}
			this.ValidateFeatureIdentifers(publishingPackageList);
			publishingPackageList.ValidateConstraints();
			return publishingPackageList;
		}

		// Token: 0x060001DB RID: 475 RVA: 0x00010614 File Offset: 0x0000E814
		private void DoFeatureIDFixUps(ref PublishingPackageList fullList)
		{
			if (fullList.FeatureIdentifierPackages == null || fullList.FeatureIdentifierPackages.Count<FeatureIdentifierPackage>() == 0)
			{
				return;
			}
			List<FeatureIdentifierPackage> list = (from pkg in fullList.FeatureIdentifierPackages
			where pkg.FixUpAction == FeatureIdentifierPackage.FixUpActions.Ignore || pkg.FixUpAction == FeatureIdentifierPackage.FixUpActions.MoveToAnotherFeature
			select pkg).ToList<FeatureIdentifierPackage>();
			using (List<FeatureIdentifierPackage>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					FeatureIdentifierPackage fip = enumerator.Current;
					List<PublishingPackageInfo> list2;
					if (string.IsNullOrEmpty(fip.ID))
					{
						list2 = (from pkg in fullList.Packages
						where string.Equals(pkg.FeatureID, fip.FeatureID, StringComparison.OrdinalIgnoreCase)
						select pkg).ToList<PublishingPackageInfo>();
					}
					else
					{
						list2 = (from pkg in fullList.Packages
						where (string.Equals(pkg.ID, fip.ID, StringComparison.OrdinalIgnoreCase) || (pkg.SatelliteType == PublishingPackageInfo.SatelliteTypes.Language && pkg.ID.StartsWith(fip.ID + PkgFile.DefaultLanguagePattern, StringComparison.OrdinalIgnoreCase)) || (pkg.SatelliteType == PublishingPackageInfo.SatelliteTypes.Resolution && pkg.ID.StartsWith(fip.ID + PkgFile.DefaultResolutionPattern, StringComparison.OrdinalIgnoreCase))) && string.Equals(pkg.Partition, fip.Partition, StringComparison.OrdinalIgnoreCase) && string.Equals(pkg.FeatureID, fip.FeatureID, StringComparison.OrdinalIgnoreCase)
						select pkg).ToList<PublishingPackageInfo>();
					}
					fullList.Packages = fullList.Packages.Except(list2).ToList<PublishingPackageInfo>();
					if (fip.FixUpAction == FeatureIdentifierPackage.FixUpActions.MoveToAnotherFeature)
					{
						foreach (PublishingPackageInfo publishingPackageInfo in list2)
						{
							fullList.Packages.Remove(publishingPackageInfo);
							foreach (string featureID in fip.FixUpActionValue.Split(new char[]
							{
								':'
							}, StringSplitOptions.RemoveEmptyEntries).ToList<string>())
							{
								PublishingPackageInfo publishingPackageInfo2 = new PublishingPackageInfo(publishingPackageInfo);
								publishingPackageInfo2.FeatureID = featureID;
								fullList.Packages.Add(publishingPackageInfo2);
							}
						}
					}
				}
			}
			fullList.FeatureIdentifierPackages = fullList.FeatureIdentifierPackages.Except(list).ToList<FeatureIdentifierPackage>();
			if (!fullList.IsTargetFeatureEnabled)
			{
				fullList.Packages = fullList.Packages.Distinct(PublishingPackageInfoComparer.IgnorePaths).ToList<PublishingPackageInfo>();
			}
		}

		// Token: 0x060001DC RID: 476 RVA: 0x00010858 File Offset: 0x0000EA58
		private void ValidateFeatureIdentifers(PublishingPackageList list)
		{
			if (list.FeatureIdentifierPackages == null || !list.FeatureIdentifierPackages.Any<FeatureIdentifierPackage>())
			{
				return;
			}
			this.Manifest.ValidateFeatureIdentiferPackages(list.Packages);
			IEnumerable<string> first = (from pkg in list.Packages
			where pkg.OwnerType == OwnerType.Microsoft
			select pkg.FeatureIDWithFMID).Distinct<string>().ToList<string>();
			List<string> second = (from pkg in list.FeatureIdentifierPackages
			where pkg.ownerType == OwnerType.Microsoft
			select pkg.FeatureIDWithFMID).Distinct<string>().ToList<string>();
			List<string> list2 = first.Except(second).ToList<string>();
			if (list2.Count<string>() != 0)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (string str in list2)
				{
					stringBuilder.Append(Environment.NewLine + "\t" + str);
				}
				throw new AmbiguousArgumentException("FeatureAPI!ValidateFeatureIdentifiers: The following features don't have the required FeatureIdentifierPackage defined: " + stringBuilder.ToString());
			}
			list.GetFeatureIDWithFMIDPackages(OwnerType.Invalid);
			List<string> fipPackageIDs = new List<string>(from pkg in list.Packages
			where pkg.IsFeatureIdentifierPackage
			select pkg.ID + "." + pkg.Partition);
			List<PublishingPackageInfo> source = (from pkg in list.Packages
			where pkg.OwnerType == OwnerType.Microsoft && fipPackageIDs.Contains(pkg.ID + "." + pkg.Partition, this.IgnoreCase)
			select pkg).ToList<PublishingPackageInfo>();
			StringBuilder stringBuilder2 = new StringBuilder();
			bool flag = false;
			using (List<string>.Enumerator enumerator = fipPackageIDs.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					string packageID = enumerator.Current;
					List<PublishingPackageInfo> list3 = new List<PublishingPackageInfo>(from listPkg in source
					where string.Equals(listPkg.ID + "." + listPkg.Partition, packageID, StringComparison.OrdinalIgnoreCase)
					select listPkg);
					if (list3.Count > 1)
					{
						flag = true;
						foreach (PublishingPackageInfo publishingPackageInfo in list3)
						{
							stringBuilder2.AppendLine();
							stringBuilder2.AppendFormat("\t{0} ({1}) {2}", publishingPackageInfo.ID, publishingPackageInfo.FeatureID, publishingPackageInfo.IsFeatureIdentifierPackage ? "(IsFeatureIdentifierPackage)" : "");
						}
						stringBuilder2.AppendLine();
					}
				}
			}
			if (flag)
			{
				throw new AmbiguousArgumentException("FeatureAPI!ValidateFeatureIdentifiers: Feature Identifier Packages found in multiple Features: " + stringBuilder2.ToString());
			}
		}

		// Token: 0x060001DD RID: 477 RVA: 0x00010B64 File Offset: 0x0000ED64
		public static string ResolveFMPath(string fmPath, string fmDirectory)
		{
			if (string.IsNullOrEmpty(fmDirectory))
			{
				return fmPath;
			}
			return fmPath.Replace(FMCollection.c_FMDirectoryVariable, fmDirectory, StringComparison.OrdinalIgnoreCase);
		}

		// Token: 0x04000139 RID: 313
		public static string c_FMDirectoryVariable = "$(FMDIRECTORY)";

		// Token: 0x0400013A RID: 314
		public FMCollectionManifest Manifest;

		// Token: 0x0400013B RID: 315
		public IULogger Logger;

		// Token: 0x0400013C RID: 316
		private StringComparer IgnoreCase = StringComparer.OrdinalIgnoreCase;
	}
}
