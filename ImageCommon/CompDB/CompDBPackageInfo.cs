using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Microsoft.WindowsPhone.FeatureAPI;
using Microsoft.WindowsPhone.ImageUpdate.PkgCommon;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;
using Microsoft.WindowsPhone.Imaging;

namespace Microsoft.WindowsPhone.CompDB
{
	// Token: 0x02000009 RID: 9
	public class CompDBPackageInfo
	{
		// Token: 0x0600003C RID: 60 RVA: 0x0000452F File Offset: 0x0000272F
		public CompDBPackageInfo()
		{
		}

		// Token: 0x0600003D RID: 61 RVA: 0x0000455C File Offset: 0x0000275C
		public CompDBPackageInfo(CompDBPackageInfo pkg)
		{
			this.UserInstallable = pkg.UserInstallable;
			this.ID = pkg.ID;
			this.OwnerType = pkg.OwnerType;
			this.Partition = pkg.Partition;
			this.Version = pkg.Version;
			this.ReleaseType = pkg.ReleaseType;
			this.SatelliteType = pkg.SatelliteType;
			this.SatelliteValue = pkg.SatelliteValue;
			this.PublicKeyToken = pkg.PublicKeyToken;
			this.BinaryPartition = pkg.BinaryPartition;
			this.BuildArchOverride = pkg.BuildArchOverride;
			this.Encrypted = pkg.Encrypted;
			this.SkipForPRSSigning = pkg.SkipForPRSSigning;
			this.SkipForPublishing = pkg.SkipForPublishing;
			this.UserInstallable = pkg.UserInstallable;
			foreach (CompDBPayloadInfo payload in pkg.Payload)
			{
				this.Payload.Add(new CompDBPayloadInfo(payload));
			}
		}

		// Token: 0x0600003E RID: 62 RVA: 0x00004698 File Offset: 0x00002898
		public CompDBPackageInfo(IPkgInfo pkgInfo, string packagePath, string msPackageRoot, string sourceFMFile, BuildCompDB parentDB, bool generateHash, bool isUserInstallable)
		{
			this.SetValues(pkgInfo, packagePath, msPackageRoot, sourceFMFile, parentDB, generateHash, isUserInstallable);
		}

		// Token: 0x0600003F RID: 63 RVA: 0x000046D8 File Offset: 0x000028D8
		public CompDBPackageInfo(FeatureManifest.FMPkgInfo pkgInfo, FeatureManifest fm, string fmFilename, string msPackageRoot, BuildCompDB parentDB, bool generateHash, bool isUserInstallable)
		{
			this.SetValues(pkgInfo, fm.OwnerType, fm.ReleaseType, fmFilename, msPackageRoot, parentDB, generateHash, isUserInstallable);
		}

		// Token: 0x06000040 RID: 64 RVA: 0x0000472C File Offset: 0x0000292C
		public CompDBPackageInfo(FeatureManifest.FMPkgInfo pkgInfo, FMCollectionItem fmCollectionItem, string msPackageRoot, BuildCompDB parentDB, bool generateHash, bool isUserInstallable)
		{
			this.SetValues(pkgInfo, fmCollectionItem.ownerType, fmCollectionItem.releaseType, Path.GetFileName(fmCollectionItem.Path), msPackageRoot, parentDB, generateHash, isUserInstallable);
		}

		// Token: 0x06000041 RID: 65 RVA: 0x00004788 File Offset: 0x00002988
		public CompDBPackageInfo SetParentDB(BuildCompDB parentDB)
		{
			this._parentDB = parentDB;
			this.Payload = (from pay in this.Payload
			select pay.SetParentPkg(this)).ToList<CompDBPayloadInfo>();
			return this;
		}

		// Token: 0x06000042 RID: 66 RVA: 0x000047B4 File Offset: 0x000029B4
		public void SetPackageHash(string packageFile)
		{
			if (this.Payload.Count != 1)
			{
				throw new ImageCommonException(string.Format("ImageCommon::CompDBPackageInfo!SetPackageHash: The Package payload must have one entry to call this function. PackageFile '{0}'.", packageFile));
			}
			this.FirstPayloadItem.SetPayloadHash(packageFile);
		}

		// Token: 0x06000043 RID: 67 RVA: 0x000047E4 File Offset: 0x000029E4
		public List<CompDBPublishingPackageInfo> GetPublishingPackages()
		{
			List<CompDBPublishingPackageInfo> list = new List<CompDBPublishingPackageInfo>();
			foreach (CompDBPayloadInfo srcPayload in this.Payload)
			{
				list.Add(new CompDBPublishingPackageInfo(srcPayload));
			}
			return list;
		}

		// Token: 0x06000044 RID: 68 RVA: 0x00004844 File Offset: 0x00002A44
		public static string GetPackageHash(string packageFile)
		{
			if (!LongPathFile.Exists(packageFile))
			{
				throw new ImageCommonException(string.Format("ImageCommon::CompDBPackageInfo!GetPackageHash: Package {0} does not exist", packageFile));
			}
			return Convert.ToBase64String(PackageTools.CalculateFileHash(packageFile));
		}

		// Token: 0x06000045 RID: 69 RVA: 0x0000486C File Offset: 0x00002A6C
		public static long GetPackageSize(string packageFile)
		{
			long result = 0L;
			try
			{
				result = new FileInfo(packageFile).Length;
			}
			catch
			{
				result = (long)LongPathFile.ReadAllBytes(packageFile).Length;
			}
			return result;
		}

		// Token: 0x06000046 RID: 70 RVA: 0x000048A8 File Offset: 0x00002AA8
		public CompDBPayloadInfo FindPayload(string path)
		{
			return this.Payload.FirstOrDefault((CompDBPayloadInfo pay) => pay.Path.Equals(path, StringComparison.OrdinalIgnoreCase));
		}

		// Token: 0x06000047 RID: 71 RVA: 0x000048DC File Offset: 0x00002ADC
		public static FeatureManifest.PackageGroups GetFMGroupFromFeatureID(string featureID)
		{
			FeatureManifest.PackageGroups result = FeatureManifest.PackageGroups.BASE;
			if (!string.IsNullOrEmpty(featureID) && featureID.Contains("_"))
			{
				string text = featureID.Substring(0, featureID.IndexOf('_'));
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

		// Token: 0x06000048 RID: 72 RVA: 0x00004940 File Offset: 0x00002B40
		public static string GetFMGroupValueFromFeatureID(string featureID)
		{
			FeatureManifest.PackageGroups packageGroups = FeatureManifest.PackageGroups.BASE;
			string result = "";
			if (!string.IsNullOrEmpty(featureID) && featureID.Contains("_"))
			{
				string text = featureID.Substring(0, featureID.IndexOf('_'));
				if (!Enum.TryParse<FeatureManifest.PackageGroups>(text, out packageGroups))
				{
					if (text.Equals("MS", StringComparison.OrdinalIgnoreCase))
					{
						packageGroups = FeatureManifest.PackageGroups.MSFEATURE;
						result = featureID.Substring("MS_".Length);
					}
					else if (text.Equals("OEM", StringComparison.OrdinalIgnoreCase))
					{
						packageGroups = FeatureManifest.PackageGroups.OEMFEATURE;
						result = featureID.Substring("OEM_".Length);
					}
				}
				else
				{
					result = featureID.Substring(text.Length + 1);
				}
			}
			return result;
		}

		// Token: 0x06000049 RID: 73 RVA: 0x000049DC File Offset: 0x00002BDC
		public bool Equals(CompDBPackageInfo pkg, CompDBPackageInfo.CompDBPackageInfoComparison compareType)
		{
			if (!this.ID.Equals(pkg.ID, StringComparison.OrdinalIgnoreCase) || !this.Partition.Equals(pkg.Partition, StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
			if (compareType == CompDBPackageInfo.CompDBPackageInfoComparison.OnlyUniqueID)
			{
				return true;
			}
			if (this.UserInstallable != pkg.UserInstallable || this.OwnerType != pkg.OwnerType || this.ReleaseType != pkg.ReleaseType || this.SatelliteType != pkg.SatelliteType || !string.Equals(this.SatelliteValue, pkg.SatelliteValue, StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
			if (compareType == CompDBPackageInfo.CompDBPackageInfoComparison.IgnorePayloadPaths)
			{
				return true;
			}
			if (this.Payload.Count != pkg.Payload.Count)
			{
				return false;
			}
			foreach (CompDBPayloadInfo compDBPayloadInfo in this.Payload)
			{
				bool flag = false;
				foreach (CompDBPayloadInfo compDBPayloadInfo2 in pkg.Payload)
				{
					if (string.Equals(compDBPayloadInfo.Path, compDBPayloadInfo2.Path, StringComparison.OrdinalIgnoreCase))
					{
						flag = true;
						if (compareType != CompDBPackageInfo.CompDBPackageInfoComparison.IgnorePayloadHashes && !string.Equals(compDBPayloadInfo.PayloadHash, compDBPayloadInfo2.PayloadHash))
						{
							return false;
						}
						break;
					}
				}
				if (!flag)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0600004A RID: 74 RVA: 0x00004B44 File Offset: 0x00002D44
		public int GetHashCode(CompDBPackageInfo.CompDBPackageInfoComparison compareType)
		{
			int num = this.ID.ToUpper(CultureInfo.InvariantCulture).GetHashCode();
			num ^= this.Partition.ToUpper(CultureInfo.InvariantCulture).GetHashCode();
			if (compareType != CompDBPackageInfo.CompDBPackageInfoComparison.OnlyUniqueIDAndFeatureID && compareType != CompDBPackageInfo.CompDBPackageInfoComparison.OnlyUniqueID)
			{
				num ^= this.UserInstallable.GetHashCode();
				num ^= this.OwnerType.GetHashCode();
				num ^= this.ReleaseType.GetHashCode();
				num ^= this.SatelliteType.GetHashCode();
				if (!string.IsNullOrEmpty(this.SatelliteValue))
				{
					num ^= this.SatelliteValue.ToUpper(CultureInfo.InvariantCulture).GetHashCode();
				}
				if (compareType != CompDBPackageInfo.CompDBPackageInfoComparison.IgnorePayloadPaths)
				{
					foreach (CompDBPayloadInfo compDBPayloadInfo in this.Payload)
					{
						num ^= compDBPayloadInfo.Path.ToUpper(CultureInfo.InvariantCulture).GetHashCode();
					}
				}
				if (compareType != CompDBPackageInfo.CompDBPackageInfoComparison.IgnorePayloadHashes)
				{
					foreach (CompDBPayloadInfo compDBPayloadInfo2 in this.Payload)
					{
						if (!string.IsNullOrEmpty(compDBPayloadInfo2.PayloadHash))
						{
							num ^= compDBPayloadInfo2.PayloadHash.ToUpper(CultureInfo.InvariantCulture).GetHashCode();
						}
					}
				}
			}
			return num;
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600004B RID: 75 RVA: 0x00004CBC File Offset: 0x00002EBC
		[XmlIgnore]
		public BuildCompDB ParentDB
		{
			get
			{
				return this._parentDB;
			}
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x0600004C RID: 76 RVA: 0x00004CC4 File Offset: 0x00002EC4
		[XmlIgnore]
		public string CBSAssemblyIdentity
		{
			get
			{
				return this.BuildArchOverride + "~" + this.ID + "~";
			}
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x0600004D RID: 77 RVA: 0x00004CE8 File Offset: 0x00002EE8
		[XmlIgnore]
		public string KeyForm
		{
			get
			{
				string text = (this.SatelliteType == CompDBPackageInfo.SatelliteTypes.Language || this.SatelliteType == CompDBPackageInfo.SatelliteTypes.LangModel) ? this.SatelliteValue : "";
				return string.Format("{0}~{1}~{2}~{3}~{4}", new object[]
				{
					this.ID,
					this.PublicKeyToken,
					this.BuildArch,
					text,
					this.VersionStr
				});
			}
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x0600004E RID: 78 RVA: 0x00004D4D File Offset: 0x00002F4D
		[XmlIgnore]
		public string BuildArch
		{
			get
			{
				if (!string.IsNullOrEmpty(this.BuildArchOverride))
				{
					return this.BuildArchOverride;
				}
				if (this._parentDB != null)
				{
					return this._parentDB.BuildArch;
				}
				return "";
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x0600004F RID: 79 RVA: 0x00004D7C File Offset: 0x00002F7C
		// (set) Token: 0x06000050 RID: 80 RVA: 0x00004DA0 File Offset: 0x00002FA0
		[XmlAttribute("Version")]
		public string VersionStr
		{
			get
			{
				return this.Version.ToString();
			}
			set
			{
				VersionInfo version;
				if (!VersionInfo.TryParse(value, out version))
				{
					throw new ImageCommonException(string.Format("ImageCommon::CompDBPackageInfo!VersionStr: Package {0}'s version '{1}' cannot be parsed.", this.ID, value));
				}
				this.Version = version;
			}
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000051 RID: 81 RVA: 0x00004DD5 File Offset: 0x00002FD5
		// (set) Token: 0x06000052 RID: 82 RVA: 0x00004DDD File Offset: 0x00002FDD
		[XmlIgnore]
		public VersionInfo Version { get; set; }

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000053 RID: 83 RVA: 0x00004DE6 File Offset: 0x00002FE6
		[XmlIgnore]
		public CompDBPayloadInfo FirstPayloadItem
		{
			get
			{
				if (this.Payload.Count > 0)
				{
					return this.Payload[0];
				}
				return null;
			}
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000054 RID: 84 RVA: 0x00004E04 File Offset: 0x00003004
		[XmlIgnore]
		public string BuildInfo
		{
			get
			{
				if (this._parentDB == null)
				{
					return "";
				}
				return this._parentDB.BuildInfo;
			}
		}

		// Token: 0x06000055 RID: 85 RVA: 0x00004E20 File Offset: 0x00003020
		public CompDBPackageInfo ClearPackageHashes()
		{
			foreach (CompDBPayloadInfo compDBPayloadInfo in this.Payload)
			{
				compDBPayloadInfo.ClearPayloadHash();
			}
			return this;
		}

		// Token: 0x06000056 RID: 86 RVA: 0x00004E74 File Offset: 0x00003074
		public CompDBPackageInfo SetPath(string path)
		{
			if (this.Payload.Count != 1)
			{
				throw new ImageCommonException(string.Format("ImageCommon::CompDBPackageInfo!SetPath: The Package payload must have one entry to call this function. Path '{0}'.", path));
			}
			this.FirstPayloadItem.SetPath(path);
			return this;
		}

		// Token: 0x06000057 RID: 87 RVA: 0x00004EA3 File Offset: 0x000030A3
		public CompDBPackageInfo SetPreviousPath(string path)
		{
			if (this.Payload.Count != 1)
			{
				throw new ImageCommonException(string.Format("ImageCommon::CompDBPackageInfo!SetPreviousPath: The Package payload must have one entry to call this function. Path '{0}'.", path));
			}
			this.FirstPayloadItem.SetPreviousPath(path);
			return this;
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00004ED2 File Offset: 0x000030D2
		public override string ToString()
		{
			return this.ID;
		}

		// Token: 0x06000059 RID: 89 RVA: 0x00004EDC File Offset: 0x000030DC
		private void SetValues(IPkgInfo pkgInfo, string packagePath, string msPackageRoot, string sourceFMFile, BuildCompDB parentDB, bool generateHash = false, bool isUserInstallable = false)
		{
			CompDBPackageInfo.SatelliteTypes pkgSatelliteType = CompDBPackageInfo.SatelliteTypes.Base;
			string pkgSatelliteValue = null;
			if (!string.IsNullOrEmpty(pkgInfo.Culture))
			{
				pkgSatelliteType = CompDBPackageInfo.SatelliteTypes.Language;
				pkgSatelliteValue = pkgInfo.Culture;
			}
			else if (!string.IsNullOrEmpty(pkgInfo.Resolution))
			{
				pkgSatelliteType = CompDBPackageInfo.SatelliteTypes.Resolution;
				pkgSatelliteValue = pkgInfo.Resolution;
			}
			string buildArch = CompDBPackageInfo.CpuString(pkgInfo.ComplexCpuType);
			this.SetValues(packagePath, pkgInfo.Name, pkgInfo.Partition, buildArch, new VersionInfo?(pkgInfo.Version), pkgInfo.PublicKey, pkgInfo.IsBinaryPartition, pkgSatelliteType, pkgSatelliteValue, pkgInfo.OwnerType, pkgInfo.ReleaseType, sourceFMFile, msPackageRoot, parentDB, generateHash, isUserInstallable);
		}

		// Token: 0x0600005A RID: 90 RVA: 0x00004F6C File Offset: 0x0000316C
		public static string CpuString(CpuId cpuId)
		{
			switch (cpuId)
			{
			case CpuId.AMD64_X86:
				return "wow64";
			case CpuId.ARM64_ARM:
				return "arm64.arm";
			case CpuId.ARM64_X86:
				return "arm64.x86";
			default:
				return cpuId.ToString().ToLower(CultureInfo.InvariantCulture);
			}
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00004FB8 File Offset: 0x000031B8
		public static CompDBPackageInfo.SatelliteTypes GetSatelliteTypeFromFMPkgInfo(FeatureManifest.FMPkgInfo fmPkgInfo, IPkgInfo pkgInfo)
		{
			CompDBPackageInfo.SatelliteTypes result = CompDBPackageInfo.SatelliteTypes.Base;
			if (!string.IsNullOrEmpty(fmPkgInfo.Language))
			{
				result = CompDBPackageInfo.SatelliteTypes.Language;
			}
			else if (!string.IsNullOrEmpty(fmPkgInfo.Resolution))
			{
				result = CompDBPackageInfo.SatelliteTypes.Resolution;
			}
			else if (fmPkgInfo.FMGroup == FeatureManifest.PackageGroups.KEYBOARD || fmPkgInfo.FMGroup == FeatureManifest.PackageGroups.SPEECH)
			{
				result = CompDBPackageInfo.SatelliteTypes.LangModel;
			}
			else if (!string.IsNullOrEmpty(pkgInfo.Culture) && pkgInfo.OwnerType == OwnerType.Microsoft && (fmPkgInfo.FeatureID.StartsWith("MS_SPEECHSYSTEM_", StringComparison.OrdinalIgnoreCase) || fmPkgInfo.FeatureID.StartsWith("MS_SPEECHDATA_", StringComparison.OrdinalIgnoreCase)))
			{
				result = CompDBPackageInfo.SatelliteTypes.LangModel;
			}
			return result;
		}

		// Token: 0x0600005C RID: 92 RVA: 0x00005040 File Offset: 0x00003240
		private void SetValues(FeatureManifest.FMPkgInfo pkgInfo, OwnerType fmOwnerType, ReleaseType fmReleaseType, string fmFilename, string msPackageRoot, BuildCompDB parentDB, bool generateHash = false, bool isUserInstallable = false)
		{
			string text = Path.ChangeExtension(pkgInfo.PackagePath, PkgConstants.c_strCBSPackageExtension);
			IPkgInfo pkgInfo2;
			if (LongPathFile.Exists(text))
			{
				pkgInfo2 = Package.LoadFromCab(text);
			}
			else
			{
				if (!LongPathFile.Exists(pkgInfo.PackagePath))
				{
					throw new ImageCommonException(string.Format("ImageCommon::CompDBPackageInfo!SetValues: Package file '{0}' could not be found.", pkgInfo.PackagePath));
				}
				pkgInfo2 = Package.LoadFromCab(pkgInfo.PackagePath);
			}
			CompDBPackageInfo.SatelliteTypes satelliteTypeFromFMPkgInfo = CompDBPackageInfo.GetSatelliteTypeFromFMPkgInfo(pkgInfo, pkgInfo2);
			string pkgSatelliteValue = null;
			switch (satelliteTypeFromFMPkgInfo)
			{
			case CompDBPackageInfo.SatelliteTypes.Language:
				pkgSatelliteValue = pkgInfo.Language;
				break;
			case CompDBPackageInfo.SatelliteTypes.Resolution:
				pkgSatelliteValue = pkgInfo.Resolution;
				break;
			case CompDBPackageInfo.SatelliteTypes.LangModel:
				pkgSatelliteValue = pkgInfo2.Culture;
				break;
			}
			string buildArch = CompDBPackageInfo.CpuString(pkgInfo2.ComplexCpuType);
			this.SetValues(pkgInfo.PackagePath, pkgInfo2.Name, pkgInfo2.Partition, buildArch, new VersionInfo?(pkgInfo2.Version), pkgInfo2.PublicKey, pkgInfo2.IsBinaryPartition, satelliteTypeFromFMPkgInfo, pkgSatelliteValue, pkgInfo2.OwnerType, pkgInfo2.ReleaseType, fmFilename, msPackageRoot, parentDB, generateHash, isUserInstallable);
		}

		// Token: 0x0600005D RID: 93 RVA: 0x00005130 File Offset: 0x00003330
		private void SetValues(string pkgPath, string pkgName, string pkgPartition, string buildArch, VersionInfo? pkgVersion, string pkgPublicKey, bool binaryPartition, CompDBPackageInfo.SatelliteTypes pkgSatelliteType, string pkgSatelliteValue, OwnerType fmOwnerType, ReleaseType fmReleaseType, string fmFilename, string msPackageRoot, BuildCompDB parentDB, bool generateHash = false, bool isUserInstallable = false)
		{
			(new char[1])[0] = '\\';
			this.ID = pkgName;
			this.Partition = pkgPartition;
			if (pkgVersion != null)
			{
				this.Version = pkgVersion.Value;
			}
			this.PublicKeyToken = pkgPublicKey;
			this.BinaryPartition = binaryPartition;
			CompDBPayloadInfo item = new CompDBPayloadInfo(pkgPath, msPackageRoot, this, generateHash);
			this.Payload.Add(item);
			this.OwnerType = fmOwnerType;
			this.ReleaseType = fmReleaseType;
			this.UserInstallable = isUserInstallable;
			this.SatelliteType = pkgSatelliteType;
			this.SatelliteValue = pkgSatelliteValue;
			if (!parentDB.BuildArch.Equals(buildArch, StringComparison.OrdinalIgnoreCase))
			{
				this.BuildArchOverride = buildArch;
			}
			this._parentDB = parentDB;
		}

		// Token: 0x0600005E RID: 94 RVA: 0x000051DE File Offset: 0x000033DE
		public CompDBPackageInfo ClearSkipForPublishing()
		{
			this.SkipForPublishing = false;
			return this;
		}

		// Token: 0x0600005F RID: 95 RVA: 0x000051E8 File Offset: 0x000033E8
		public CompDBPackageInfo SetSkipForPublishing()
		{
			this.SkipForPublishing = true;
			return this;
		}

		// Token: 0x06000060 RID: 96 RVA: 0x000051F2 File Offset: 0x000033F2
		public CompDBPackageInfo ClearSkipForPRSSigning()
		{
			this.SkipForPRSSigning = false;
			return this;
		}

		// Token: 0x06000061 RID: 97 RVA: 0x000051FC File Offset: 0x000033FC
		public CompDBPackageInfo SetSkipForPRSSigning()
		{
			if (this.ReleaseType == ReleaseType.Production)
			{
				this.SkipForPRSSigning = true;
			}
			return this;
		}

		// Token: 0x06000062 RID: 98 RVA: 0x00005210 File Offset: 0x00003410
		public CompDBPackageInfo SetPayloadType(CompDBPayloadInfo.PayloadTypes payloadType)
		{
			foreach (CompDBPayloadInfo compDBPayloadInfo in this.Payload)
			{
				compDBPayloadInfo.PayloadType = payloadType;
			}
			return this;
		}

		// Token: 0x04000030 RID: 48
		private BuildCompDB _parentDB;

		// Token: 0x04000031 RID: 49
		[XmlAttribute]
		public string ID;

		// Token: 0x04000032 RID: 50
		[XmlAttribute]
		[DefaultValue("MainOS")]
		public string Partition = "MainOS";

		// Token: 0x04000034 RID: 52
		[XmlAttribute]
		[DefaultValue(ReleaseType.Production)]
		public ReleaseType ReleaseType = ReleaseType.Production;

		// Token: 0x04000035 RID: 53
		[XmlAttribute]
		[DefaultValue(OwnerType.Microsoft)]
		public OwnerType OwnerType = OwnerType.Microsoft;

		// Token: 0x04000036 RID: 54
		[XmlAttribute]
		[DefaultValue(CompDBPackageInfo.SatelliteTypes.Base)]
		public CompDBPackageInfo.SatelliteTypes SatelliteType;

		// Token: 0x04000037 RID: 55
		[XmlAttribute]
		[DefaultValue(null)]
		public string SatelliteValue;

		// Token: 0x04000038 RID: 56
		[XmlAttribute]
		[DefaultValue(false)]
		public bool Encrypted;

		// Token: 0x04000039 RID: 57
		[XmlAttribute]
		public string PublicKeyToken;

		// Token: 0x0400003A RID: 58
		[XmlAttribute]
		[DefaultValue(false)]
		public bool BinaryPartition;

		// Token: 0x0400003B RID: 59
		[XmlAttribute]
		[DefaultValue(false)]
		public bool SkipForPublishing;

		// Token: 0x0400003C RID: 60
		[XmlAttribute]
		[DefaultValue(false)]
		public bool SkipForPRSSigning;

		// Token: 0x0400003D RID: 61
		[XmlAttribute]
		[DefaultValue(false)]
		public bool UserInstallable;

		// Token: 0x0400003E RID: 62
		[XmlAttribute]
		[DefaultValue(null)]
		public string BuildArchOverride;

		// Token: 0x0400003F RID: 63
		[XmlArrayItem(ElementName = "PayloadItem", Type = typeof(CompDBPayloadInfo), IsNullable = false)]
		[XmlArray]
		public List<CompDBPayloadInfo> Payload = new List<CompDBPayloadInfo>();

		// Token: 0x02000049 RID: 73
		public enum CompDBPackageInfoComparison
		{
			// Token: 0x040001C8 RID: 456
			Standard,
			// Token: 0x040001C9 RID: 457
			IgnorePayloadHashes,
			// Token: 0x040001CA RID: 458
			IgnorePayloadPaths,
			// Token: 0x040001CB RID: 459
			OnlyUniqueID,
			// Token: 0x040001CC RID: 460
			OnlyUniqueIDAndFeatureID
		}

		// Token: 0x0200004A RID: 74
		public enum SatelliteTypes
		{
			// Token: 0x040001CE RID: 462
			Base,
			// Token: 0x040001CF RID: 463
			Language,
			// Token: 0x040001D0 RID: 464
			Resolution,
			// Token: 0x040001D1 RID: 465
			LangModel
		}
	}
}
