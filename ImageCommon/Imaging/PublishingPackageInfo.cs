using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Microsoft.WindowsPhone.FeatureAPI;
using Microsoft.WindowsPhone.ImageUpdate.PkgCommon;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000031 RID: 49
	public class PublishingPackageInfo
	{
		// Token: 0x06000211 RID: 529 RVA: 0x00012DC1 File Offset: 0x00010FC1
		public PublishingPackageInfo()
		{
		}

		// Token: 0x06000212 RID: 530 RVA: 0x00012DF4 File Offset: 0x00010FF4
		public PublishingPackageInfo(PublishingPackageInfo pkg)
		{
			this.UserInstallable = pkg.UserInstallable;
			this.FeatureID = pkg.FeatureID;
			this.FMID = pkg.FMID;
			this.ID = pkg.ID;
			this.IsFeatureIdentifierPackage = pkg.IsFeatureIdentifierPackage;
			this.OwnerType = pkg.OwnerType;
			this.Partition = pkg.Partition;
			this.Version = pkg.Version;
			this.Path = pkg.Path;
			this.PreviousPath = pkg.PreviousPath;
			this.ReleaseType = pkg.ReleaseType;
			this.SatelliteType = pkg.SatelliteType;
			this.SatelliteValue = pkg.SatelliteValue;
			this.SourceFMFile = pkg.SourceFMFile;
			this.UpdateType = pkg.UpdateType;
		}

		// Token: 0x06000213 RID: 531 RVA: 0x00012EE8 File Offset: 0x000110E8
		public PublishingPackageInfo(FeatureManifest.FMPkgInfo pkgInfo, FMCollectionItem fmCollectionItem, string msPackageRoot, bool isUserInstallable)
		{
			char[] trimChars = new char[]
			{
				'\\'
			};
			string text = pkgInfo.PackagePath;
			string text2 = text;
			int startIndex = 0;
			int length = text.Length;
			string text3 = text;
			text = text2.Substring(startIndex, length - text3.Substring(text3.LastIndexOf(".", StringComparison.OrdinalIgnoreCase)).Length);
			text += ".cab";
			if (!LongPathFile.Exists(pkgInfo.PackagePath) && !LongPathFile.Exists(text))
			{
				throw new FileNotFoundException("ImageCommon!PublishingPackageInfo: The package file '" + pkgInfo.PackagePath + "' could not be found.");
			}
			if (!string.IsNullOrEmpty(pkgInfo.ID) && !string.IsNullOrEmpty(pkgInfo.Partition) && pkgInfo.Version != null)
			{
				VersionInfo? version = pkgInfo.Version;
				VersionInfo empty = VersionInfo.Empty;
				if ((version == null || (version != null && !(version.GetValueOrDefault() == empty))) && (!LongPathFile.Exists(text) || string.Compare(text, pkgInfo.PackagePath, StringComparison.OrdinalIgnoreCase) == 0))
				{
					this.ID = pkgInfo.ID;
					this.Partition = pkgInfo.Partition;
					this.Version = pkgInfo.Version.Value;
					goto IL_182;
				}
			}
			IPkgInfo pkgInfo2 = Package.LoadFromCab(LongPathFile.Exists(text) ? text : pkgInfo.PackagePath);
			this.ID = pkgInfo2.Name;
			this.Partition = pkgInfo2.Partition;
			this.Version = pkgInfo2.Version;
			IL_182:
			this.Path = pkgInfo.PackagePath.Replace(msPackageRoot, "", StringComparison.OrdinalIgnoreCase).Trim(trimChars);
			this.SourceFMFile = System.IO.Path.GetFileName(fmCollectionItem.Path.ToUpper(CultureInfo.InvariantCulture));
			FeatureManifest.PackageGroups fmgroup = pkgInfo.FMGroup;
			if (fmgroup != FeatureManifest.PackageGroups.DEVICELAYOUT)
			{
				if (fmgroup != FeatureManifest.PackageGroups.OEMDEVICEPLATFORM)
				{
					this.FeatureID = pkgInfo.FeatureID;
				}
				else
				{
					this.FeatureID = pkgInfo.FeatureID.Replace(FeatureManifest.PackageGroups.OEMDEVICEPLATFORM.ToString(), FeatureManifest.PackageGroups.DEVICE.ToString(), StringComparison.OrdinalIgnoreCase);
				}
			}
			else
			{
				this.FeatureID = pkgInfo.FeatureID.Replace(FeatureManifest.PackageGroups.DEVICELAYOUT.ToString(), FeatureManifest.PackageGroups.SOC.ToString());
			}
			this.FMID = fmCollectionItem.ID;
			if (isUserInstallable)
			{
				this.FeatureID = PublishingPackageInfo.UserInstallableFeatureIDPrefix + this.FeatureID;
			}
			this.OwnerType = fmCollectionItem.ownerType;
			this.ReleaseType = fmCollectionItem.releaseType;
			this.UserInstallable = fmCollectionItem.UserInstallable;
			if (!string.IsNullOrEmpty(pkgInfo.Language))
			{
				this.SatelliteType = PublishingPackageInfo.SatelliteTypes.Language;
				this.SatelliteValue = pkgInfo.Language;
			}
			else if (!string.IsNullOrEmpty(pkgInfo.Resolution))
			{
				this.SatelliteType = PublishingPackageInfo.SatelliteTypes.Resolution;
				this.SatelliteValue = pkgInfo.Resolution;
			}
			else
			{
				this.SatelliteType = PublishingPackageInfo.SatelliteTypes.Base;
				this.SatelliteValue = null;
			}
			this.IsFeatureIdentifierPackage = pkgInfo.FeatureIdentifierPackage;
			this.UpdateType = PublishingPackageInfo.UpdateTypes.Canonical;
		}

		// Token: 0x06000214 RID: 532 RVA: 0x000131E8 File Offset: 0x000113E8
		public bool Equals(PublishingPackageInfo pkg, PublishingPackageInfo.PublishingPackageInfoComparison compareType)
		{
			return this.ID.Equals(pkg.ID, StringComparison.OrdinalIgnoreCase) && this.Partition.Equals(pkg.Partition, StringComparison.OrdinalIgnoreCase) && (compareType == PublishingPackageInfo.PublishingPackageInfoComparison.OnlyUniqueID || (this.FeatureID.Equals(pkg.FeatureID, StringComparison.OrdinalIgnoreCase) && string.Equals(this.FMID, pkg.FMID, StringComparison.OrdinalIgnoreCase) && (compareType == PublishingPackageInfo.PublishingPackageInfoComparison.OnlyUniqueIDAndFeatureID || (this.UserInstallable == pkg.UserInstallable && this.OwnerType == pkg.OwnerType && this.ReleaseType == pkg.ReleaseType && this.SatelliteType == pkg.SatelliteType && string.Equals(this.SatelliteValue, pkg.SatelliteValue, StringComparison.OrdinalIgnoreCase) && (compareType == PublishingPackageInfo.PublishingPackageInfoComparison.IgnorePaths || (this.Path.Equals(pkg.Path, StringComparison.OrdinalIgnoreCase) && string.Equals(this.PreviousPath, pkg.PreviousPath, StringComparison.OrdinalIgnoreCase)))))));
		}

		// Token: 0x06000215 RID: 533 RVA: 0x000132D4 File Offset: 0x000114D4
		public int GetHashCode(PublishingPackageInfo.PublishingPackageInfoComparison compareType)
		{
			int num = this.ID.ToUpper(CultureInfo.InvariantCulture).GetHashCode();
			num ^= this.Partition.ToUpper(CultureInfo.InvariantCulture).GetHashCode();
			if (compareType != PublishingPackageInfo.PublishingPackageInfoComparison.OnlyUniqueID)
			{
				num ^= this.FeatureID.ToUpper(CultureInfo.InvariantCulture).GetHashCode();
				if (!string.IsNullOrEmpty(this.FMID))
				{
					num ^= this.FMID.GetHashCode();
				}
			}
			if (compareType != PublishingPackageInfo.PublishingPackageInfoComparison.OnlyUniqueIDAndFeatureID && compareType != PublishingPackageInfo.PublishingPackageInfoComparison.OnlyUniqueID)
			{
				num ^= this.UserInstallable.GetHashCode();
				num ^= this.OwnerType.GetHashCode();
				num ^= this.ReleaseType.GetHashCode();
				num ^= this.SatelliteType.GetHashCode();
				if (!string.IsNullOrEmpty(this.SatelliteValue))
				{
					num ^= this.SatelliteValue.ToUpper(CultureInfo.InvariantCulture).GetHashCode();
				}
				if (compareType != PublishingPackageInfo.PublishingPackageInfoComparison.IgnorePaths)
				{
					num ^= this.Path.ToUpper(CultureInfo.InvariantCulture).GetHashCode();
					if (!string.IsNullOrEmpty(this.PreviousPath))
					{
						num ^= this.PreviousPath.ToUpper(CultureInfo.InvariantCulture).GetHashCode();
					}
				}
			}
			return num;
		}

		// Token: 0x06000216 RID: 534 RVA: 0x00013404 File Offset: 0x00011604
		public PublishingPackageInfo SetPreviousPath(string path)
		{
			this.PreviousPath = path;
			return this;
		}

		// Token: 0x06000217 RID: 535 RVA: 0x0001340E File Offset: 0x0001160E
		public PublishingPackageInfo SetUpdateType(PublishingPackageInfo.UpdateTypes type)
		{
			this.UpdateType = type;
			return this;
		}

		// Token: 0x17000073 RID: 115
		// (get) Token: 0x06000218 RID: 536 RVA: 0x00013418 File Offset: 0x00011618
		[XmlIgnore]
		public string FeatureIDWithFMID
		{
			get
			{
				if (!string.IsNullOrEmpty(this.FMID))
				{
					return FeatureManifest.GetFeatureIDWithFMID(this.FeatureID, this.FMID);
				}
				return this.FeatureID;
			}
		}

		// Token: 0x17000074 RID: 116
		// (get) Token: 0x06000219 RID: 537 RVA: 0x00013440 File Offset: 0x00011640
		[XmlIgnore]
		public FeatureManifest.PackageGroups FMGroup
		{
			get
			{
				FeatureManifest.PackageGroups result = FeatureManifest.PackageGroups.BASE;
				if (!string.IsNullOrEmpty(this.FeatureID) && this.FeatureID.Contains('_'))
				{
					string text = this.FeatureID.Substring(0, this.FeatureID.IndexOf('_'));
					if (!Enum.TryParse<FeatureManifest.PackageGroups>(text, out result))
					{
						if (text.Equals("MS", StringComparison.OrdinalIgnoreCase))
						{
							result = FeatureManifest.PackageGroups.MSFEATURE;
						}
						else if (text.Equals("OEM", StringComparison.OrdinalIgnoreCase))
						{
							result = FeatureManifest.PackageGroups.OEMFEATURE;
						}
					}
				}
				return result;
			}
		}

		// Token: 0x0600021A RID: 538 RVA: 0x000134B4 File Offset: 0x000116B4
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				this.ID,
				" (",
				this.FeatureIDWithFMID,
				") : ",
				this.UpdateType
			});
		}

		// Token: 0x0400015B RID: 347
		public static string UserInstallableFeatureIDPrefix = "USERINSTALLABLE_";

		// Token: 0x0400015C RID: 348
		public string ID;

		// Token: 0x0400015D RID: 349
		[DefaultValue(false)]
		public bool IsFeatureIdentifierPackage;

		// Token: 0x0400015E RID: 350
		public string Path;

		// Token: 0x0400015F RID: 351
		[DefaultValue(null)]
		public string PreviousPath;

		// Token: 0x04000160 RID: 352
		[DefaultValue("MainOS")]
		public string Partition = "MainOS";

		// Token: 0x04000161 RID: 353
		public string FeatureID;

		// Token: 0x04000162 RID: 354
		public string FMID;

		// Token: 0x04000163 RID: 355
		public VersionInfo Version = VersionInfo.Empty;

		// Token: 0x04000164 RID: 356
		[DefaultValue(ReleaseType.Production)]
		public ReleaseType ReleaseType = ReleaseType.Production;

		// Token: 0x04000165 RID: 357
		[DefaultValue(OwnerType.Microsoft)]
		public OwnerType OwnerType = OwnerType.Microsoft;

		// Token: 0x04000166 RID: 358
		[DefaultValue(false)]
		public bool UserInstallable;

		// Token: 0x04000167 RID: 359
		[DefaultValue(PublishingPackageInfo.SatelliteTypes.Base)]
		public PublishingPackageInfo.SatelliteTypes SatelliteType;

		// Token: 0x04000168 RID: 360
		[DefaultValue(null)]
		public string SatelliteValue;

		// Token: 0x04000169 RID: 361
		[DefaultValue(PublishingPackageInfo.UpdateTypes.Canonical)]
		public PublishingPackageInfo.UpdateTypes UpdateType = PublishingPackageInfo.UpdateTypes.Canonical;

		// Token: 0x0400016A RID: 362
		public string SourceFMFile;

		// Token: 0x02000099 RID: 153
		public enum PublishingPackageInfoComparison
		{
			// Token: 0x04000342 RID: 834
			IgnorePaths,
			// Token: 0x04000343 RID: 835
			OnlyUniqueID,
			// Token: 0x04000344 RID: 836
			OnlyUniqueIDAndFeatureID
		}

		// Token: 0x0200009A RID: 154
		public enum UpdateTypes
		{
			// Token: 0x04000346 RID: 838
			PKR,
			// Token: 0x04000347 RID: 839
			Diff,
			// Token: 0x04000348 RID: 840
			Canonical
		}

		// Token: 0x0200009B RID: 155
		public enum SatelliteTypes
		{
			// Token: 0x0400034A RID: 842
			Base,
			// Token: 0x0400034B RID: 843
			Language,
			// Token: 0x0400034C RID: 844
			Resolution
		}
	}
}
