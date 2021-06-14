using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using Microsoft.WindowsPhone.FeatureAPI;
using Microsoft.WindowsPhone.ImageUpdate.PkgCommon;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000030 RID: 48
	[XmlType(Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
	[XmlRoot(ElementName = "PublishingPackageInfo", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate", IsNullable = false)]
	public class PublishingPackageList
	{
		// Token: 0x060001EF RID: 495 RVA: 0x00011277 File Offset: 0x0000F477
		public PublishingPackageList()
		{
			if (this.Packages == null)
			{
				this.Packages = new List<PublishingPackageInfo>();
			}
		}

		// Token: 0x060001F0 RID: 496 RVA: 0x000112A0 File Offset: 0x0000F4A0
		public PublishingPackageList(string sourceListPath, string destListPath, IULogger logger)
		{
			PublishingPackageList publishingPackageList = PublishingPackageList.ValidateAndLoad(sourceListPath, logger);
			PublishingPackageList publishingPackageList2 = PublishingPackageList.ValidateAndLoad(destListPath, logger);
			List<PublishingPackageInfo> list = publishingPackageList.Packages;
			List<PublishingPackageInfo> list2 = publishingPackageList2.Packages;
			this.IsUpdateList = true;
			this.FeatureIdentifierPackages = publishingPackageList.FeatureIdentifierPackages;
			this.IsTargetFeatureEnabled = publishingPackageList.IsTargetFeatureEnabled;
			this.Packages = new List<PublishingPackageInfo>();
			IEnumerable<PublishingPackageInfo> enumerable = from srcPkg in list
			join destPkg in list2 on srcPkg.ID equals destPkg.ID
			where !destPkg.Path.Equals(srcPkg.Path, StringComparison.OrdinalIgnoreCase) && destPkg.Equals(srcPkg, PublishingPackageInfo.PublishingPackageInfoComparison.IgnorePaths)
			select destPkg.SetPreviousPath(srcPkg.Path);
			enumerable = (from pkg in enumerable
			select pkg.SetUpdateType(PublishingPackageInfo.UpdateTypes.Diff)).ToList<PublishingPackageInfo>();
			this.Packages.AddRange(enumerable);
			list = list.Except(enumerable, PublishingPackageInfoComparer.IgnorePaths).ToList<PublishingPackageInfo>();
			list2 = list2.Except(enumerable, PublishingPackageInfoComparer.IgnorePaths).ToList<PublishingPackageInfo>();
			this.Packages.AddRange(from pkg in list2.Intersect(list, PublishingPackageInfoComparer.IgnorePaths)
			select pkg.SetUpdateType(PublishingPackageInfo.UpdateTypes.Diff));
			this.Packages.AddRange(from pkg in list2.Except(list, PublishingPackageInfoComparer.IgnorePaths)
			select pkg.SetUpdateType(PublishingPackageInfo.UpdateTypes.Canonical));
			this.Packages.AddRange(from pkg in list.Except(list2, PublishingPackageInfoComparer.IgnorePaths)
			select pkg.SetUpdateType(PublishingPackageInfo.UpdateTypes.PKR));
			if (publishingPackageList.IsTargetFeatureEnabled || publishingPackageList.MSFeatureGroups.Count<FMFeatureGrouping>() > 0 || publishingPackageList.OEMFeatureGroups.Count<FMFeatureGrouping>() > 0)
			{
				this.MSFeatureGroups = publishingPackageList.MSFeatureGroups;
				this.OEMFeatureGroups = publishingPackageList.OEMFeatureGroups;
				return;
			}
			this.MSFeatureGroups = publishingPackageList2.MSFeatureGroups;
			this.OEMFeatureGroups = publishingPackageList2.OEMFeatureGroups;
		}

		// Token: 0x060001F1 RID: 497 RVA: 0x0001151C File Offset: 0x0000F71C
		public Dictionary<string, string> GetFeatureIDWithFMIDPackages(OwnerType forOwner)
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = false;
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			foreach (string text in this.GetFeatureIDWithFMIDs(forOwner))
			{
				List<PublishingPackageInfo> list = (from pkg in this.GetAllPackagesForFeatureIDWithFMID(text, forOwner)
				where pkg.IsFeatureIdentifierPackage
				select pkg).ToList<PublishingPackageInfo>();
				if (list.Count<PublishingPackageInfo>() > 1)
				{
					flag = true;
					stringBuilder.Append(Environment.NewLine + text + " : ");
					using (List<PublishingPackageInfo>.Enumerator enumerator2 = list.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							PublishingPackageInfo publishingPackageInfo = enumerator2.Current;
							stringBuilder.Append(Environment.NewLine + "\t" + publishingPackageInfo.ID);
						}
						continue;
					}
				}
				string value = "";
				if (list.Count<PublishingPackageInfo>() == 1)
				{
					value = list.ElementAt(0).ID;
				}
				dictionary.Add(text, value);
			}
			if (flag)
			{
				throw new AmbiguousArgumentException("Some features have more than one FeatureIdentifierPackage defined: " + stringBuilder.ToString());
			}
			return dictionary;
		}

		// Token: 0x060001F2 RID: 498 RVA: 0x00011670 File Offset: 0x0000F870
		public List<string> GetFeatureIDWithFMIDs(OwnerType forOwner)
		{
			IEnumerable<PublishingPackageInfo> source = from pkg in this.Packages
			where pkg.OwnerType == OwnerType.Microsoft || pkg.OwnerType == OwnerType.OEM
			select pkg;
			if (forOwner != OwnerType.Invalid)
			{
				source = from pkg in this.Packages
				where pkg.OwnerType == forOwner
				select pkg;
			}
			return (from pkg in source
			select pkg.FeatureIDWithFMID).Distinct(this.IgnoreCase).ToList<string>();
		}

		// Token: 0x060001F3 RID: 499 RVA: 0x0001170C File Offset: 0x0000F90C
		public List<PublishingPackageInfo> GetAllPackagesForFeature(string FeatureID, OwnerType forOwner)
		{
			return this.GetAllPackagesForFeatures(new List<string>
			{
				FeatureID
			}, forOwner);
		}

		// Token: 0x060001F4 RID: 500 RVA: 0x00011730 File Offset: 0x0000F930
		public List<PublishingPackageInfo> GetAllPackagesForFeatureIDWithFMID(string FeatureIDWithFMID, OwnerType forOwner)
		{
			List<PublishingPackageInfo> list = (from pkg in this.Packages
			where FeatureIDWithFMID.Equals(pkg.FeatureIDWithFMID, StringComparison.OrdinalIgnoreCase)
			select pkg).ToList<PublishingPackageInfo>();
			if (forOwner != OwnerType.Invalid)
			{
				list = (from pkg in list
				where pkg.OwnerType == forOwner
				select pkg).ToList<PublishingPackageInfo>();
			}
			return list;
		}

		// Token: 0x060001F5 RID: 501 RVA: 0x00011790 File Offset: 0x0000F990
		private List<PublishingPackageInfo> GetAllPackagesForFeaturesAndFMs(List<string> FeatureIDs, List<string> fmFilter, OwnerType forOwner = OwnerType.Invalid)
		{
			IEnumerable<PublishingPackageInfo> allPackagesForFeatures = this.GetAllPackagesForFeatures(FeatureIDs, forOwner);
			List<string> newFMFilter = (from fm in fmFilter
			select fm.ToUpper(CultureInfo.InvariantCulture).Replace("SKU", "FM", StringComparison.OrdinalIgnoreCase)).ToList<string>();
			List<string> newSKUFilter = (from fm in fmFilter
			select fm.ToUpper(CultureInfo.InvariantCulture).Replace("FM", "SKU", StringComparison.OrdinalIgnoreCase)).ToList<string>();
			return (from pkg in allPackagesForFeatures
			where newFMFilter.Contains(pkg.SourceFMFile, this.IgnoreCase) || newSKUFilter.Contains(pkg.SourceFMFile, this.IgnoreCase)
			select pkg).ToList<PublishingPackageInfo>();
		}

		// Token: 0x060001F6 RID: 502 RVA: 0x00011828 File Offset: 0x0000FA28
		public List<PublishingPackageInfo> GetAllPackagesForFeatures(List<string> FeatureIDs, OwnerType forOwner)
		{
			IEnumerable<PublishingPackageInfo> source = from pkg in this.Packages
			where pkg.OwnerType == forOwner && FeatureIDs.Contains(pkg.FeatureID, this.IgnoreCase)
			select pkg;
			if (forOwner == OwnerType.Invalid)
			{
				source = from pkg in this.Packages
				where FeatureIDs.Contains(pkg.FeatureID, this.IgnoreCase)
				select pkg;
			}
			return source.ToList<PublishingPackageInfo>();
		}

		// Token: 0x060001F7 RID: 503 RVA: 0x0001188E File Offset: 0x0000FA8E
		public List<PublishingPackageInfo> GetUpdatePackageList(OEMInput orgOemInput, OEMInput newOemInput, OEMInput.OEMFeatureTypes featureFilter)
		{
			return this.GetUpdatePackageList(orgOemInput, newOemInput, featureFilter, OwnerType.Invalid);
		}

		// Token: 0x060001F8 RID: 504 RVA: 0x0001189C File Offset: 0x0000FA9C
		public List<PublishingPackageInfo> GetUpdatePackageList(OEMInput orgOemInput, OEMInput newOemInput, OEMInput.OEMFeatureTypes featureFilter, OwnerType forOwner)
		{
			List<string> featureList = orgOemInput.GetFeatureList(featureFilter);
			List<string> featureList2 = newOemInput.GetFeatureList(featureFilter);
			List<string> fms = orgOemInput.GetFMs();
			List<string> fms2 = newOemInput.GetFMs();
			return this.GetUpdatePackageList(featureList, featureList2, orgOemInput.SupportedLanguages.UserInterface, newOemInput.SupportedLanguages.UserInterface, newOemInput.Resolutions, fms, fms2, forOwner, false);
		}

		// Token: 0x060001F9 RID: 505 RVA: 0x000118F0 File Offset: 0x0000FAF0
		private List<PublishingPackageInfo> GetUpdatePackageList(List<string> orgFeatures, List<string> newFeatures, List<string> orgLangs, List<string> newLangs, List<string> resolutions, List<string> orgFMs, List<string> newFMs, OwnerType forOwner = OwnerType.Invalid, bool DoOnlyChanges = false)
		{
			List<PublishingPackageInfo> list = new List<PublishingPackageInfo>();
			if (forOwner != OwnerType.Microsoft && forOwner != OwnerType.OEM && forOwner != OwnerType.Invalid)
			{
				return list;
			}
			List<string> addFeatures = newFeatures.Except(orgFeatures, this.IgnoreCase).ToList<string>();
			List<string> removeFeatures = orgFeatures.Except(newFeatures, this.IgnoreCase).ToList<string>();
			List<string> commonFeatures = newFeatures.Intersect(orgFeatures, this.IgnoreCase).ToList<string>();
			List<string> addLangs = newLangs.Except(orgLangs, this.IgnoreCase).ToList<string>();
			List<string> removeLangs = orgLangs.Except(newLangs, this.IgnoreCase).ToList<string>();
			List<string> commonLangs = newLangs.Intersect(orgLangs, this.IgnoreCase).ToList<string>();
			List<string> orgFMsNormalized = (from fm in orgFMs
			select this.NormalizeFM(fm)).ToList<string>();
			List<string> addFMs = newFMs.Except(orgFMsNormalized, this.IgnoreCase).ToList<string>();
			List<string> removeFMs = orgFMsNormalized.Except(newFMs, this.IgnoreCase).ToList<string>();
			List<string> commonFMs = newFMs.Intersect(orgFMsNormalized, this.IgnoreCase).ToList<string>();
			List<PublishingPackageInfo> list2 = (from pkg in this.Packages
			where addFeatures.Contains(pkg.FeatureID, this.IgnoreCase) && pkg.UpdateType != PublishingPackageInfo.UpdateTypes.PKR && newFMs.Contains(this.NormalizeFM(pkg.SourceFMFile), this.IgnoreCase) && (pkg.SatelliteType == PublishingPackageInfo.SatelliteTypes.Base || (pkg.SatelliteType == PublishingPackageInfo.SatelliteTypes.Resolution && resolutions.Contains(pkg.SatelliteValue, this.IgnoreCase)) || (pkg.SatelliteType == PublishingPackageInfo.SatelliteTypes.Language && newLangs.Contains(pkg.SatelliteValue, this.IgnoreCase)))
			select pkg).ToList<PublishingPackageInfo>();
			List<PublishingPackageInfo> list3 = (from pkg in this.Packages
			where removeFeatures.Contains(pkg.FeatureID, this.IgnoreCase) && orgFMsNormalized.Contains(this.NormalizeFM(pkg.SourceFMFile), this.IgnoreCase) && (pkg.SatelliteType == PublishingPackageInfo.SatelliteTypes.Base || (pkg.SatelliteType == PublishingPackageInfo.SatelliteTypes.Resolution && resolutions.Contains(pkg.SatelliteValue, this.IgnoreCase)) || (pkg.SatelliteType == PublishingPackageInfo.SatelliteTypes.Language && orgLangs.Contains(pkg.SatelliteValue, this.IgnoreCase)))
			select pkg).ToList<PublishingPackageInfo>();
			list2.AddRange((from pkg in this.Packages
			where commonFeatures.Contains(pkg.FeatureID, this.IgnoreCase) && addFMs.Contains(this.NormalizeFM(pkg.SourceFMFile), this.IgnoreCase) && pkg.UpdateType != PublishingPackageInfo.UpdateTypes.PKR && (pkg.SatelliteType == PublishingPackageInfo.SatelliteTypes.Base || (pkg.SatelliteType == PublishingPackageInfo.SatelliteTypes.Resolution && resolutions.Contains(pkg.SatelliteValue, this.IgnoreCase)) || (pkg.SatelliteType == PublishingPackageInfo.SatelliteTypes.Language && newLangs.Contains(pkg.SatelliteValue, this.IgnoreCase)))
			select pkg).ToList<PublishingPackageInfo>());
			list3.AddRange((from pkg in this.Packages
			where commonFeatures.Contains(pkg.FeatureID, this.IgnoreCase) && removeFMs.Contains(this.NormalizeFM(pkg.SourceFMFile), this.IgnoreCase) && (!this.IsUpdateList || pkg.UpdateType != PublishingPackageInfo.UpdateTypes.Canonical) && (pkg.SatelliteType == PublishingPackageInfo.SatelliteTypes.Base || (pkg.SatelliteType == PublishingPackageInfo.SatelliteTypes.Resolution && resolutions.Contains(pkg.SatelliteValue, this.IgnoreCase)) || (pkg.SatelliteType == PublishingPackageInfo.SatelliteTypes.Language && orgLangs.Contains(pkg.SatelliteValue, this.IgnoreCase)))
			select pkg).ToList<PublishingPackageInfo>());
			list2.AddRange((from pkg in this.Packages
			where commonFeatures.Contains(pkg.FeatureID, this.IgnoreCase) && newFMs.Contains(this.NormalizeFM(pkg.SourceFMFile), this.IgnoreCase) && pkg.UpdateType != PublishingPackageInfo.UpdateTypes.PKR && pkg.SatelliteType == PublishingPackageInfo.SatelliteTypes.Language && addLangs.Contains(pkg.SatelliteValue, this.IgnoreCase)
			select pkg).ToList<PublishingPackageInfo>());
			list3.AddRange((from pkg in this.Packages
			where commonFeatures.Contains(pkg.FeatureID, this.IgnoreCase) && orgFMsNormalized.Contains(this.NormalizeFM(pkg.SourceFMFile), this.IgnoreCase) && pkg.SatelliteType == PublishingPackageInfo.SatelliteTypes.Language && removeLangs.Contains(pkg.SatelliteValue, this.IgnoreCase)
			select pkg).ToList<PublishingPackageInfo>());
			if (this.IsUpdateList && !DoOnlyChanges)
			{
				list.AddRange((from pkg in this.Packages
				where commonFeatures.Contains(pkg.FeatureID, this.IgnoreCase) && commonFMs.Contains(this.NormalizeFM(pkg.SourceFMFile), this.IgnoreCase) && (pkg.SatelliteType == PublishingPackageInfo.SatelliteTypes.Base || (pkg.SatelliteType == PublishingPackageInfo.SatelliteTypes.Resolution && resolutions.Contains(pkg.SatelliteValue, this.IgnoreCase)) || (pkg.SatelliteType == PublishingPackageInfo.SatelliteTypes.Language && commonLangs.Contains(pkg.SatelliteValue, this.IgnoreCase)))
				select pkg).ToList<PublishingPackageInfo>());
				list3 = list3.Except(from pkg in list3
				where pkg.UpdateType == PublishingPackageInfo.UpdateTypes.Canonical && pkg.OwnerType != OwnerType.Microsoft
				select pkg).ToList<PublishingPackageInfo>();
			}
			list2 = this.ChangeToCanonicals(list2);
			list3 = this.ChangeToPKRs(list3);
			list.AddRange(list2);
			list.AddRange(list3);
			if (forOwner != OwnerType.Invalid)
			{
				list = (from pkg in list
				where pkg.OwnerType == forOwner
				select pkg).ToList<PublishingPackageInfo>();
			}
			return this.RemoveDuplicatesPkgs(list);
		}

		// Token: 0x060001FA RID: 506 RVA: 0x00011BED File Offset: 0x0000FDED
		private string NormalizeFM(string fm)
		{
			return fm.ToUpper(CultureInfo.InvariantCulture).Replace("SKU", "FM", StringComparison.OrdinalIgnoreCase);
		}

		// Token: 0x060001FB RID: 507 RVA: 0x00011C0C File Offset: 0x0000FE0C
		public UpdateOSInput GetUpdateInput(string build1OEMInput, string build2OEMInput, string msPackageRoot, CpuId cpuType, IULogger logger)
		{
			UpdateOSInput updateOSInput = new UpdateOSInput();
			List<string> list = new List<string>();
			char[] trimChars = new char[]
			{
				'\\'
			};
			msPackageRoot = msPackageRoot.Trim(trimChars);
			OEMInput orgOemInput = new OEMInput();
			OEMInput oeminput = new OEMInput();
			OEMInput.ValidateInput(ref orgOemInput, build1OEMInput, logger, msPackageRoot, cpuType.ToString());
			OEMInput.ValidateInput(ref oeminput, build2OEMInput, logger, msPackageRoot, cpuType.ToString());
			list = (from pkg in this.GetUpdatePackageList(orgOemInput, oeminput, (OEMInput.OEMFeatureTypes)268435455)
			select pkg.Path).ToList<string>();
			list = list.Distinct(this.IgnoreCase).ToList<string>();
			for (int i = 0; i < list.Count; i++)
			{
				list[i] = Path.Combine(msPackageRoot, list[i]);
			}
			updateOSInput.PackageFiles = list;
			updateOSInput.Description = "(Updating to)" + oeminput.Description;
			return updateOSInput;
		}

		// Token: 0x060001FC RID: 508 RVA: 0x00011D0B File Offset: 0x0000FF0B
		public List<PublishingPackageInfo> GetPackageListForPOP(OEMInput oemInput1, OEMInput oemInput2)
		{
			List<PublishingPackageInfo> list = new List<PublishingPackageInfo>();
			list.AddRange(this.GetMSPackageListForPOP(oemInput1, oemInput2));
			list.AddRange(this.GetOEMPackageListForPOP(oemInput1, oemInput2));
			return list;
		}

		// Token: 0x060001FD RID: 509 RVA: 0x00011D30 File Offset: 0x0000FF30
		private List<string> GetDepricatedFeatures(List<string> featureIDs)
		{
			List<string> list = new List<string>();
			if (!this.IsUpdateList)
			{
				return list;
			}
			foreach (string text in featureIDs)
			{
				if ((from pkg in this.GetAllPackagesForFeature(text, OwnerType.Invalid)
				where pkg.UpdateType > PublishingPackageInfo.UpdateTypes.PKR
				select pkg).Count<PublishingPackageInfo>() == 0)
				{
					list.Add(text);
				}
			}
			return list;
		}

		// Token: 0x060001FE RID: 510 RVA: 0x00011DC4 File Offset: 0x0000FFC4
		private List<string> GetDepricatedLangs(List<string> Langs)
		{
			List<string> list = new List<string>();
			if (!this.IsUpdateList)
			{
				return list;
			}
			using (List<string>.Enumerator enumerator = Langs.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					string lang = enumerator.Current;
					if ((from pkg in this.Packages
					where pkg.SatelliteType == PublishingPackageInfo.SatelliteTypes.Language && pkg.SatelliteValue.Equals(lang, StringComparison.OrdinalIgnoreCase) && pkg.UpdateType > PublishingPackageInfo.UpdateTypes.PKR
					select pkg).Count<PublishingPackageInfo>() == 0)
					{
						list.Add(lang);
					}
				}
			}
			return list;
		}

		// Token: 0x060001FF RID: 511 RVA: 0x00011E54 File Offset: 0x00010054
		public List<PublishingPackageInfo> GetMSPackageListForPOP(OEMInput orgOemInput, OEMInput newOemInput)
		{
			List<string> list = orgOemInput.GetFeatureList((OEMInput.OEMFeatureTypes)268433407);
			List<string> featureList = newOemInput.GetFeatureList((OEMInput.OEMFeatureTypes)268433407);
			List<string> fms = orgOemInput.GetFMs();
			List<string> fms2 = newOemInput.GetFMs();
			List<string> list2 = orgOemInput.SupportedLanguages.UserInterface;
			List<string> userInterface = newOemInput.SupportedLanguages.UserInterface;
			list = list.Except(this.GetDepricatedFeatures(list)).ToList<string>();
			list2 = list2.Except(this.GetDepricatedLangs(list2)).ToList<string>();
			bool doOnlyChanges = true;
			List<PublishingPackageInfo> list3 = this.GetUpdatePackageList(list, featureList, list2, userInterface, newOemInput.Resolutions, fms, fms2, OwnerType.Microsoft, doOnlyChanges);
			if (this.IsUpdateList)
			{
				list3 = list3.Except(this.GetSourceOnlyPkgs((from pkg in list3
				where pkg.UpdateType == PublishingPackageInfo.UpdateTypes.PKR
				select pkg).ToList<PublishingPackageInfo>())).ToList<PublishingPackageInfo>();
			}
			return list3;
		}

		// Token: 0x06000200 RID: 512 RVA: 0x00011F30 File Offset: 0x00010130
		private List<PublishingPackageInfo> GetSourceOnlyPkgs(List<PublishingPackageInfo> list)
		{
			List<string> second = (from pkg in this.Packages
			where pkg.UpdateType == PublishingPackageInfo.UpdateTypes.Diff || pkg.UpdateType == PublishingPackageInfo.UpdateTypes.Canonical
			select pkg.ID).Distinct<string>().ToList<string>();
			List<string> first = (from pkg in this.Packages
			select pkg.ID).Distinct(StringComparer.OrdinalIgnoreCase).ToList<string>();
			List<string> inSourceOnlyPackageIDs = first.Except(second, this.IgnoreCase).Distinct(this.IgnoreCase).ToList<string>();
			return (from pkg in list
			where inSourceOnlyPackageIDs.Contains(pkg.ID, StringComparer.OrdinalIgnoreCase)
			select pkg).ToList<PublishingPackageInfo>();
		}

		// Token: 0x06000201 RID: 513 RVA: 0x00012010 File Offset: 0x00010210
		private List<PublishingPackageInfo> RemoveDuplicatesPkgs(List<PublishingPackageInfo> pkgList)
		{
			List<PublishingPackageInfo> list = new List<PublishingPackageInfo>();
			List<string> excludePackages = new List<string>();
			List<PublishingPackageInfo> source = (from pkg in pkgList
			where pkg.UpdateType == PublishingPackageInfo.UpdateTypes.PKR
			select pkg).ToList<PublishingPackageInfo>();
			List<PublishingPackageInfo> source2 = (from pkg in pkgList
			where pkg.UpdateType == PublishingPackageInfo.UpdateTypes.Canonical
			select pkg).ToList<PublishingPackageInfo>();
			List<PublishingPackageInfo> collection = (from pkg in pkgList
			where pkg.UpdateType == PublishingPackageInfo.UpdateTypes.Diff
			select pkg).ToList<PublishingPackageInfo>();
			excludePackages = (from pkgID in source
			select pkgID.ID).Intersect(from dupPkg in source2
			select dupPkg.ID, this.IgnoreCase).ToList<string>();
			list.AddRange((from pkg in source
			where !excludePackages.Contains(pkg.ID, this.IgnoreCase)
			select pkg).ToList<PublishingPackageInfo>());
			list.AddRange((from pkg in source2
			where !excludePackages.Contains(pkg.ID, this.IgnoreCase)
			select pkg).ToList<PublishingPackageInfo>());
			list.AddRange(collection);
			return list.Distinct<PublishingPackageInfo>().ToList<PublishingPackageInfo>();
		}

		// Token: 0x06000202 RID: 514 RVA: 0x00012168 File Offset: 0x00010368
		private List<PublishingPackageInfo> ChangeToCanonicals(List<PublishingPackageInfo> pkgs)
		{
			List<PublishingPackageInfo> list = new List<PublishingPackageInfo>();
			foreach (PublishingPackageInfo pkg in pkgs)
			{
				PublishingPackageInfo item = this.ChangeToCanonical(pkg);
				list.Add(item);
			}
			return list.Distinct<PublishingPackageInfo>().ToList<PublishingPackageInfo>();
		}

		// Token: 0x06000203 RID: 515 RVA: 0x000121D0 File Offset: 0x000103D0
		private PublishingPackageInfo ChangeToCanonical(PublishingPackageInfo pkg)
		{
			PublishingPackageInfo publishingPackageInfo = new PublishingPackageInfo(pkg);
			if (publishingPackageInfo.UpdateType != PublishingPackageInfo.UpdateTypes.Canonical)
			{
				publishingPackageInfo.UpdateType = PublishingPackageInfo.UpdateTypes.Canonical;
			}
			return publishingPackageInfo;
		}

		// Token: 0x06000204 RID: 516 RVA: 0x000121F8 File Offset: 0x000103F8
		private List<PublishingPackageInfo> ChangeToPKRs(List<PublishingPackageInfo> pkgs)
		{
			List<PublishingPackageInfo> list = new List<PublishingPackageInfo>();
			foreach (PublishingPackageInfo pkg in pkgs)
			{
				PublishingPackageInfo item = this.ChangeToPKR(pkg);
				list.Add(item);
			}
			return list.Distinct<PublishingPackageInfo>().ToList<PublishingPackageInfo>();
		}

		// Token: 0x06000205 RID: 517 RVA: 0x00012260 File Offset: 0x00010460
		private PublishingPackageInfo ChangeToPKR(PublishingPackageInfo pkg)
		{
			PublishingPackageInfo publishingPackageInfo = new PublishingPackageInfo(pkg);
			if (publishingPackageInfo.UpdateType != PublishingPackageInfo.UpdateTypes.PKR)
			{
				string text = publishingPackageInfo.Path.ToLower(CultureInfo.InvariantCulture);
				text = text.Replace(PkgConstants.c_strPackageExtension, PkgConstants.c_strRemovalPkgExtension, StringComparison.OrdinalIgnoreCase);
				text = text.Replace(PkgConstants.c_strDiffPackageExtension, PkgConstants.c_strRemovalPkgExtension, StringComparison.OrdinalIgnoreCase);
				text = text.Replace(".spkg.cab", ".spkr.cab", StringComparison.OrdinalIgnoreCase);
				text = text.ToLower(CultureInfo.InvariantCulture).Replace(".spku.cab", ".spkr.cab", StringComparison.OrdinalIgnoreCase);
				publishingPackageInfo.Path = text;
				publishingPackageInfo.UpdateType = PublishingPackageInfo.UpdateTypes.PKR;
			}
			return publishingPackageInfo;
		}

		// Token: 0x06000206 RID: 518 RVA: 0x000122F0 File Offset: 0x000104F0
		public List<PublishingPackageInfo> GetOEMPackageListForPOP(OEMInput oemInput1, OEMInput oemInput2)
		{
			List<PublishingPackageInfo> list = this.GetUpdatePackageList(oemInput1, oemInput2, (OEMInput.OEMFeatureTypes)268434431, OwnerType.OEM);
			if (this.IsUpdateList)
			{
				list = list.Except(this.GetTargetOnlyPkgs((from pkg in list
				where pkg.UpdateType == PublishingPackageInfo.UpdateTypes.PKR
				select pkg).ToList<PublishingPackageInfo>())).ToList<PublishingPackageInfo>();
			}
			return list;
		}

		// Token: 0x06000207 RID: 519 RVA: 0x00012354 File Offset: 0x00010554
		private List<PublishingPackageInfo> GetTargetOnlyPkgs(List<PublishingPackageInfo> list)
		{
			List<string> sourcePackageIDs = (from pkg in this.Packages
			where pkg.UpdateType == PublishingPackageInfo.UpdateTypes.Diff || pkg.UpdateType == PublishingPackageInfo.UpdateTypes.PKR
			select pkg.ID).Distinct<string>().ToList<string>();
			return (from pkg in list
			where !sourcePackageIDs.Contains(pkg.ID, StringComparer.OrdinalIgnoreCase)
			select pkg).ToList<PublishingPackageInfo>();
		}

		// Token: 0x06000208 RID: 520 RVA: 0x000123DC File Offset: 0x000105DC
		public List<PublishingPackageInfo> GetAllPackagesForImage(OEMInput oemInput)
		{
			return this.GetAllPackagesForImage(oemInput, OwnerType.Invalid);
		}

		// Token: 0x06000209 RID: 521 RVA: 0x000123E8 File Offset: 0x000105E8
		public List<PublishingPackageInfo> GetAllPackagesForImage(OEMInput oemInput, OwnerType forOwnerType)
		{
			List<PublishingPackageInfo> list = new List<PublishingPackageInfo>();
			OEMInput.OEMFeatureTypes forFeatures = (OEMInput.OEMFeatureTypes)268435455;
			if (forOwnerType == OwnerType.Microsoft)
			{
				forFeatures = (OEMInput.OEMFeatureTypes)268433407;
			}
			else if (forOwnerType == OwnerType.OEM)
			{
				forFeatures = (OEMInput.OEMFeatureTypes)268434431;
			}
			else if (forOwnerType != OwnerType.Invalid)
			{
				return list;
			}
			List<string> featureList = oemInput.GetFeatureList(forFeatures);
			List<string> fms = oemInput.GetFMs();
			List<string> langs = oemInput.SupportedLanguages.UserInterface;
			List<string> res = oemInput.Resolutions;
			List<PublishingPackageInfo> allPackagesForFeaturesAndFMs = this.GetAllPackagesForFeaturesAndFMs(featureList, fms, forOwnerType);
			list.AddRange((from pkg in allPackagesForFeaturesAndFMs
			where pkg.SatelliteType == PublishingPackageInfo.SatelliteTypes.Base
			select pkg).ToList<PublishingPackageInfo>());
			list.AddRange((from pkg in allPackagesForFeaturesAndFMs
			where pkg.SatelliteType == PublishingPackageInfo.SatelliteTypes.Language && langs.Contains(pkg.SatelliteValue, this.IgnoreCase)
			select pkg).ToList<PublishingPackageInfo>());
			list.AddRange((from pkg in allPackagesForFeaturesAndFMs
			where pkg.SatelliteType == PublishingPackageInfo.SatelliteTypes.Resolution && res.Contains(pkg.SatelliteValue, this.IgnoreCase)
			select pkg).ToList<PublishingPackageInfo>());
			return list.Distinct<PublishingPackageInfo>().ToList<PublishingPackageInfo>();
		}

		// Token: 0x0600020A RID: 522 RVA: 0x000124DC File Offset: 0x000106DC
		public void ValidateConstraints()
		{
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			foreach (IGrouping<string, PublishingPackageInfo> source in from pkg in this.Packages
			where pkg.OwnerType == OwnerType.Microsoft
			group pkg by pkg.ID)
			{
				if (source.Count<PublishingPackageInfo>() > 1)
				{
					IEnumerable<string> enumerable = source.Select((PublishingPackageInfo pkg) => pkg.FeatureID).Distinct(this.IgnoreCase);
					if (source.Count<PublishingPackageInfo>() != enumerable.Count<string>())
					{
						foreach (IGrouping<string, PublishingPackageInfo> source2 in source.GroupBy((PublishingPackageInfo pkg) => pkg.FeatureID).Where((IGrouping<string, PublishingPackageInfo> gp) => gp.Count<PublishingPackageInfo>() > 1))
						{
							stringBuilder.AppendLine(string.Concat(new object[]
							{
								"\t",
								source2.First<PublishingPackageInfo>().FeatureID,
								": (",
								source2.First<PublishingPackageInfo>().ID,
								" Count=",
								source2.Count<PublishingPackageInfo>(),
								")\n"
							}));
						}
					}
					if (this.IsTargetFeatureEnabled)
					{
						List<List<string>> list = this.OEMFeatureGroups.Where((FMFeatureGrouping fGroup) => fGroup.Constraint == FMFeatureGrouping.FeatureConstraints.OneAndOnlyOne || fGroup.Constraint == FMFeatureGrouping.FeatureConstraints.ZeroOrOne).Select((FMFeatureGrouping fGroup) => fGroup.AllFeatureIDs).ToList<List<string>>();
						list.AddRange(this.MSFeatureGroups.Where((FMFeatureGrouping fGroup) => fGroup.Constraint == FMFeatureGrouping.FeatureConstraints.OneAndOnlyOne || fGroup.Constraint == FMFeatureGrouping.FeatureConstraints.ZeroOrOne).Select((FMFeatureGrouping fGroup) => fGroup.AllFeatureIDs));
						list.AddRange(this.GetImplicitConstraints());
						bool flag = false;
						foreach (List<string> second in list)
						{
							if (enumerable.Except(second).Count<string>() == 0)
							{
								flag = true;
								break;
							}
						}
						if (!flag)
						{
							stringBuilder2.AppendLine(string.Concat(new string[]
							{
								"\t",
								source.First<PublishingPackageInfo>().ID,
								": (",
								string.Join(", ", enumerable.ToArray<string>()),
								")\n"
							}));
						}
					}
				}
			}
			StringBuilder stringBuilder3 = new StringBuilder();
			if (stringBuilder.Length != 0)
			{
				stringBuilder3.AppendLine("The following Features have packages listed more than once:\n" + stringBuilder.ToString());
			}
			if (stringBuilder2.Length != 0)
			{
				stringBuilder3.AppendLine("The following package is included in multiple features without constraints preventing them from being included in the same image:\n" + stringBuilder2.ToString());
			}
			if (stringBuilder3.Length != 0)
			{
				throw new ImageCommonException(stringBuilder3.ToString());
			}
		}

		// Token: 0x0600020B RID: 523 RVA: 0x00012888 File Offset: 0x00010A88
		public List<List<string>> GetImplicitConstraints()
		{
			List<List<string>> list = new List<List<string>>();
			using (IEnumerator<FeatureManifest.PackageGroups> enumerator = (from pkg in this.Packages
			where pkg.FMGroup != FeatureManifest.PackageGroups.MSFEATURE && pkg.FMGroup != FeatureManifest.PackageGroups.OEMFEATURE && pkg.FMGroup > FeatureManifest.PackageGroups.BASE
			select pkg.FMGroup).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					FeatureManifest.PackageGroups fmGroup = enumerator.Current;
					list.Add((from pkg in this.Packages
					where pkg.FMGroup == fmGroup
					select pkg.FeatureID.ToString()).Distinct(this.IgnoreCase).ToList<string>());
				}
			}
			return list;
		}

		// Token: 0x0600020C RID: 524 RVA: 0x0001297C File Offset: 0x00010B7C
		public static void FixUpDuplicateFIPFeatures(PublishingPackageList sourcePpil)
		{
			foreach (IEnumerable<FeatureIdentifierPackage> source in from fip in sourcePpil.FeatureIdentifierPackages
			group fip by fip.FeatureIDWithFMID into grp
			where grp.Count<FeatureIdentifierPackage>() > 1
			select grp)
			{
				int num = 0;
				foreach (FeatureIdentifierPackage featureIdentifierPackage in source.ToList<FeatureIdentifierPackage>())
				{
					num++;
					if (num > 1)
					{
						PublishingPackageList.<>c__DisplayClass39_0 CS$<>8__locals1 = new PublishingPackageList.<>c__DisplayClass39_0();
						string text = featureIdentifierPackage.FeatureID + num;
						CS$<>8__locals1.orgFeatureIDWithFMID = featureIdentifierPackage.FeatureIDWithFMID;
						List<FMFeatureGrouping> list = new List<FMFeatureGrouping>();
						if (sourcePpil.MSFeatureGroups != null)
						{
							list.AddRange(sourcePpil.MSFeatureGroups);
						}
						if (sourcePpil.OEMFeatureGroups != null)
						{
							list.AddRange(sourcePpil.OEMFeatureGroups);
						}
						IEnumerable<FMFeatureGrouping> source2 = list;
						Func<FMFeatureGrouping, bool> predicate;
						if ((predicate = CS$<>8__locals1.<>9__2) == null)
						{
							PublishingPackageList.<>c__DisplayClass39_0 CS$<>8__locals2 = CS$<>8__locals1;
							predicate = (CS$<>8__locals2.<>9__2 = ((FMFeatureGrouping grp) => grp.AllFeatureIDWithFMIDs.Contains(CS$<>8__locals2.orgFeatureIDWithFMID, StringComparer.OrdinalIgnoreCase)));
						}
						foreach (FMFeatureGrouping fmfeatureGrouping in source2.Where(predicate))
						{
							fmfeatureGrouping.FeatureIDs.Add(text);
						}
						sourcePpil.Packages.AddRange(PublishingPackageList.GetRenamedDuplicateFeaturePackages(CS$<>8__locals1.orgFeatureIDWithFMID, text, sourcePpil));
						featureIdentifierPackage.FeatureID = text;
					}
				}
			}
		}

		// Token: 0x0600020D RID: 525 RVA: 0x00012B64 File Offset: 0x00010D64
		private static List<PublishingPackageInfo> GetRenamedDuplicateFeaturePackages(string orgFeatureIDWithFMID, string newFeatureID, PublishingPackageList sourcePPIL)
		{
			PublishingPackageList.<>c__DisplayClass40_0 CS$<>8__locals1 = new PublishingPackageList.<>c__DisplayClass40_0();
			CS$<>8__locals1.orgFeatureIDWithFMID = orgFeatureIDWithFMID;
			List<PublishingPackageInfo> list = new List<PublishingPackageInfo>();
			IEnumerable<PublishingPackageInfo> packages = sourcePPIL.Packages;
			Func<PublishingPackageInfo, bool> predicate;
			if ((predicate = CS$<>8__locals1.<>9__0) == null)
			{
				PublishingPackageList.<>c__DisplayClass40_0 CS$<>8__locals2 = CS$<>8__locals1;
				predicate = (CS$<>8__locals2.<>9__0 = ((PublishingPackageInfo pkg) => pkg.FeatureIDWithFMID.Equals(CS$<>8__locals2.orgFeatureIDWithFMID, StringComparison.OrdinalIgnoreCase)));
			}
			foreach (PublishingPackageInfo pkg2 in packages.Where(predicate))
			{
				list.Add(new PublishingPackageInfo(pkg2)
				{
					FeatureID = newFeatureID
				});
			}
			return list;
		}

		// Token: 0x0600020E RID: 526 RVA: 0x00012BF8 File Offset: 0x00010DF8
		public static PublishingPackageList ValidateAndLoad(string xmlFile, IULogger logger)
		{
			PublishingPackageList result = new PublishingPackageList();
			string text = string.Empty;
			string publishingPackageInfoSchema = BuildPaths.PublishingPackageInfoSchema;
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			foreach (string text2 in executingAssembly.GetManifestResourceNames())
			{
				if (text2.Contains(publishingPackageInfoSchema))
				{
					text = text2;
					break;
				}
			}
			if (string.IsNullOrEmpty(text))
			{
				throw new ImageCommonException("ImageCommon!ValidateAndLoad: XSD resource was not found: " + publishingPackageInfoSchema);
			}
			using (Stream manifestResourceStream = executingAssembly.GetManifestResourceStream(text))
			{
				XsdValidator xsdValidator = new XsdValidator();
				try
				{
					xsdValidator.ValidateXsd(manifestResourceStream, xmlFile, logger);
				}
				catch (XsdValidatorException innerException)
				{
					throw new ImageCommonException("ImageCommon!ValidateAndLoad: Unable to validate Publishing Package Info XSD for file '" + xmlFile + "'.", innerException);
				}
			}
			logger.LogInfo("ImageCommon: Successfully validated the Publishing Package Info XML: {0}", new object[]
			{
				xmlFile
			});
			TextReader textReader = new StreamReader(LongPathFile.OpenRead(xmlFile));
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(PublishingPackageList));
			try
			{
				result = (PublishingPackageList)xmlSerializer.Deserialize(textReader);
			}
			catch (Exception innerException2)
			{
				throw new ImageCommonException("ImageCommon!ValidateAndLoad: Unable to parse Publishing Package Info XML file '" + xmlFile + "'", innerException2);
			}
			finally
			{
				textReader.Close();
			}
			return result;
		}

		// Token: 0x0600020F RID: 527 RVA: 0x00012D48 File Offset: 0x00010F48
		public void WriteToFile(string xmlFile)
		{
			TextWriter textWriter = new StreamWriter(LongPathFile.OpenWrite(xmlFile));
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(PublishingPackageList));
			try
			{
				xmlSerializer.Serialize(textWriter, this);
			}
			catch (Exception innerException)
			{
				throw new ImageCommonException("ImageCommon!WriteToFile: Unable to write Publishing Package List XML file '" + xmlFile + "'", innerException);
			}
			finally
			{
				textWriter.Close();
			}
		}

		// Token: 0x04000151 RID: 337
		private const string DiffPackageExtension = ".spku.cab";

		// Token: 0x04000152 RID: 338
		private const string CanonicalPackageExtension = ".spkg.cab";

		// Token: 0x04000153 RID: 339
		private const string RemovePackageExtension = ".spkr.cab";

		// Token: 0x04000154 RID: 340
		[DefaultValue(false)]
		public bool IsUpdateList;

		// Token: 0x04000155 RID: 341
		[DefaultValue(false)]
		public bool IsTargetFeatureEnabled;

		// Token: 0x04000156 RID: 342
		[XmlArrayItem(ElementName = "FeatureGroup", Type = typeof(FMFeatureGrouping), IsNullable = false)]
		[XmlArray]
		public List<FMFeatureGrouping> MSFeatureGroups;

		// Token: 0x04000157 RID: 343
		[XmlArrayItem(ElementName = "FeatureGroup", Type = typeof(FMFeatureGrouping), IsNullable = false)]
		[XmlArray]
		public List<FMFeatureGrouping> OEMFeatureGroups;

		// Token: 0x04000158 RID: 344
		[XmlArrayItem(ElementName = "PackageInfo", Type = typeof(PublishingPackageInfo), IsNullable = false)]
		[XmlArray]
		public List<PublishingPackageInfo> Packages;

		// Token: 0x04000159 RID: 345
		[XmlArrayItem(ElementName = "FeatureIdentifierPackage", Type = typeof(FeatureIdentifierPackage), IsNullable = false)]
		[XmlArray]
		public List<FeatureIdentifierPackage> FeatureIdentifierPackages;

		// Token: 0x0400015A RID: 346
		[XmlIgnore]
		private StringComparer IgnoreCase = StringComparer.OrdinalIgnoreCase;
	}
}
