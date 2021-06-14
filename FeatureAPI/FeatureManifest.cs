using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using Microsoft.Composition.Packaging;
using Microsoft.WindowsPhone.ImageUpdate.PkgCommon;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.FeatureAPI
{
	// Token: 0x0200000E RID: 14
	[XmlType(Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
	[XmlRoot(ElementName = "FeatureManifest", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate", IsNullable = false)]
	public class FeatureManifest
	{
		// Token: 0x06000031 RID: 49 RVA: 0x0000318A File Offset: 0x0000138A
		public FeatureManifest()
		{
		}

		// Token: 0x06000032 RID: 50 RVA: 0x000038D4 File Offset: 0x00001AD4
		public FeatureManifest(FeatureManifest srcFM)
		{
			if (srcFM == null)
			{
				return;
			}
			if (srcFM.Revision != null)
			{
				this.Revision = srcFM.Revision;
			}
			if (srcFM.SchemaVersion != null)
			{
				this.SchemaVersion = srcFM.SchemaVersion;
			}
			if (srcFM.BasePackages != null)
			{
				this.BasePackages = new List<PkgFile>(srcFM.BasePackages);
			}
			if (srcFM.BootLocalePackageFile != null)
			{
				this.BootLocalePackageFile = srcFM.BootLocalePackageFile;
			}
			if (srcFM.BootUILanguagePackageFile != null)
			{
				this.BootUILanguagePackageFile = srcFM.BootUILanguagePackageFile;
			}
			if (srcFM.CPUPackages != null)
			{
				this.CPUPackages = new List<PkgFile>(srcFM.CPUPackages);
			}
			if (srcFM.DeviceLayoutPackages != null)
			{
				this.DeviceLayoutPackages = new List<DeviceLayoutPkgFile>(srcFM.DeviceLayoutPackages);
			}
			if (srcFM.DeviceSpecificPackages != null)
			{
				this.DeviceSpecificPackages = new List<DevicePkgFile>(srcFM.DeviceSpecificPackages);
			}
			if (srcFM.Features != null)
			{
				this.Features = new FMFeatures();
				if (srcFM.Features.MSFeatureGroups != null)
				{
					this.Features.MSFeatureGroups = new List<FMFeatureGrouping>(srcFM.Features.MSFeatureGroups);
				}
				if (srcFM.Features.Microsoft != null)
				{
					this.Features.Microsoft = new List<MSOptionalPkgFile>(srcFM.Features.Microsoft);
				}
				if (srcFM.Features.OEM != null)
				{
					this.Features.OEM = new List<OEMOptionalPkgFile>(srcFM.Features.OEM);
				}
				if (srcFM.Features.OEMFeatureGroups != null)
				{
					this.Features.OEMFeatureGroups = new List<FMFeatureGrouping>(srcFM.Features.OEMFeatureGroups);
				}
			}
			if (srcFM.KeyboardPackages != null)
			{
				this.KeyboardPackages = new List<KeyboardPkgFile>(srcFM.KeyboardPackages);
			}
			if (srcFM.OEMDevicePlatformPackages != null)
			{
				this.OEMDevicePlatformPackages = new List<OEMDevicePkgFile>(srcFM.OEMDevicePlatformPackages);
			}
			if (srcFM.PrereleasePackages != null)
			{
				this.PrereleasePackages = new List<PrereleasePkgFile>(srcFM.PrereleasePackages);
			}
			if (srcFM.ReleasePackages != null)
			{
				this.ReleasePackages = new List<ReleasePkgFile>(srcFM.ReleasePackages);
			}
			if (srcFM.SOCPackages != null)
			{
				this.SOCPackages = new List<SOCPkgFile>(srcFM.SOCPackages);
			}
			if (srcFM.SpeechPackages != null)
			{
				this.SpeechPackages = new List<SpeechPkgFile>(srcFM.SpeechPackages);
			}
			if (srcFM.SVPackages != null)
			{
				this.SVPackages = new List<SVPkgFile>(srcFM.SVPackages);
			}
			this.SourceFile = srcFM.SourceFile;
			this.OemInput = srcFM.OemInput;
			this.OSVersion = srcFM.OSVersion;
			this.ID = srcFM.ID;
			this.BuildID = srcFM.BuildID;
			this.BuildInfo = srcFM.BuildInfo;
			this.ReleaseType = srcFM.ReleaseType;
			this.OwnerType = srcFM.OwnerType;
			this.Owner = srcFM.Owner;
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000033 RID: 51 RVA: 0x00003B70 File Offset: 0x00001D70
		// (set) Token: 0x06000034 RID: 52 RVA: 0x00003B9C File Offset: 0x00001D9C
		[XmlAttribute("OwnerName")]
		[DefaultValue(null)]
		public string Owner
		{
			get
			{
				if (this.OwnerType == OwnerType.Microsoft)
				{
					return OwnerType.Microsoft.ToString();
				}
				return this._owner;
			}
			set
			{
				if (this.OwnerType == OwnerType.Microsoft)
				{
					this._owner = null;
					return;
				}
				this._owner = value;
			}
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000035 RID: 53 RVA: 0x00003BB6 File Offset: 0x00001DB6
		// (set) Token: 0x06000036 RID: 54 RVA: 0x00003BF0 File Offset: 0x00001DF0
		[XmlAttribute]
		[DefaultValue(null)]
		public string OSVersion
		{
			get
			{
				if (this._osVersion == null || string.IsNullOrEmpty(this._osVersion.ToString()))
				{
					return null;
				}
				return this._osVersion.ToString();
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					this._osVersion = null;
					return;
				}
				string[] array = value.Split(new char[]
				{
					'.'
				});
				ushort[] array2 = new ushort[4];
				for (int i = 0; i < Math.Min(array.Count<string>(), 4); i++)
				{
					if (string.IsNullOrEmpty(array[i]))
					{
						array2[i] = 0;
					}
					else
					{
						array2[i] = ushort.Parse(array[i]);
					}
				}
				if (array.Count<string>() != 4)
				{
					this._osVersion = null;
					return;
				}
				this._osVersion = new VersionInfo?(new VersionInfo(array2[0], array2[1], array2[2], array2[3]));
			}
		}

		// Token: 0x06000037 RID: 55 RVA: 0x00003C8F File Offset: 0x00001E8F
		public bool ShouldSerializeBasePackages()
		{
			return this.BasePackages != null && this.BasePackages.Count != 0;
		}

		// Token: 0x06000038 RID: 56 RVA: 0x00003CA9 File Offset: 0x00001EA9
		public bool ShouldSerializeCPUPackages()
		{
			return false;
		}

		// Token: 0x06000039 RID: 57 RVA: 0x00003CAC File Offset: 0x00001EAC
		public bool ShouldSerializeFeatures()
		{
			return this.Features != null;
		}

		// Token: 0x0600003A RID: 58 RVA: 0x00003CB7 File Offset: 0x00001EB7
		public bool ShouldSerializeReleasePackages()
		{
			return this.ReleasePackages != null && this.ReleasePackages.Count != 0;
		}

		// Token: 0x0600003B RID: 59 RVA: 0x00003CD1 File Offset: 0x00001ED1
		public bool ShouldSerializePrereleasePackages()
		{
			return this.PrereleasePackages != null && this.PrereleasePackages.Count != 0;
		}

		// Token: 0x0600003C RID: 60 RVA: 0x00003CEB File Offset: 0x00001EEB
		public bool ShouldSerializeOEMDevicePlatformPackages()
		{
			return this.OEMDevicePlatformPackages != null && this.OEMDevicePlatformPackages.Count != 0;
		}

		// Token: 0x0600003D RID: 61 RVA: 0x00003D05 File Offset: 0x00001F05
		public bool ShouldSerializeDeviceLayoutPackages()
		{
			return this.DeviceLayoutPackages != null && this.DeviceLayoutPackages.Count != 0;
		}

		// Token: 0x0600003E RID: 62 RVA: 0x00003D1F File Offset: 0x00001F1F
		public bool ShouldSerializeSOCPackages()
		{
			return this.SOCPackages != null && this.SOCPackages.Count != 0;
		}

		// Token: 0x0600003F RID: 63 RVA: 0x00003D39 File Offset: 0x00001F39
		public bool ShouldSerializeSVPackages()
		{
			return this.SVPackages != null && this.SVPackages.Count != 0;
		}

		// Token: 0x06000040 RID: 64 RVA: 0x00003D53 File Offset: 0x00001F53
		public bool ShouldSerializeDeviceSpecificPackages()
		{
			return this.DeviceSpecificPackages != null && this.DeviceSpecificPackages.Count != 0;
		}

		// Token: 0x06000041 RID: 65 RVA: 0x00003D6D File Offset: 0x00001F6D
		public bool ShouldSerializeSpeechPackages()
		{
			return this.SpeechPackages != null && this.SpeechPackages.Count != 0;
		}

		// Token: 0x06000042 RID: 66 RVA: 0x00003D87 File Offset: 0x00001F87
		public bool ShouldSerializeKeyboardPackages()
		{
			return this.KeyboardPackages != null && this.KeyboardPackages.Count != 0;
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000043 RID: 67 RVA: 0x00003DA1 File Offset: 0x00001FA1
		public IEnumerable<PkgFile> AllPackages
		{
			get
			{
				return this._allPackages;
			}
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000044 RID: 68 RVA: 0x00003DAC File Offset: 0x00001FAC
		private IEnumerable<PkgFile> _allPackages
		{
			get
			{
				List<PkgFile> list = new List<PkgFile>();
				if (this.BootUILanguagePackageFile != null)
				{
					list.Add(this.BootUILanguagePackageFile);
				}
				if (this.BootLocalePackageFile != null)
				{
					list.Add(this.BootLocalePackageFile);
				}
				if (this.BasePackages != null)
				{
					list.AddRange(this.BasePackages);
				}
				if (this.Features != null)
				{
					if (this.Features.Microsoft != null)
					{
						list.AddRange(this.Features.Microsoft);
					}
					if (this.Features.OEM != null)
					{
						list.AddRange(this.Features.OEM);
					}
				}
				if (this.SVPackages != null)
				{
					list.AddRange(this.SVPackages);
				}
				if (this.SOCPackages != null)
				{
					list.AddRange(this.SOCPackages);
				}
				if (this.OEMDevicePlatformPackages != null)
				{
					list.AddRange(this.OEMDevicePlatformPackages);
				}
				if (this.DeviceLayoutPackages != null)
				{
					list.AddRange(this.DeviceLayoutPackages);
				}
				if (this.DeviceSpecificPackages != null)
				{
					list.AddRange(this.DeviceSpecificPackages);
				}
				if (this.ReleasePackages != null)
				{
					list.AddRange(this.ReleasePackages);
				}
				if (this.PrereleasePackages != null)
				{
					list.AddRange(this.PrereleasePackages);
				}
				if (this.KeyboardPackages != null)
				{
					list.AddRange(this.KeyboardPackages);
				}
				if (this.SpeechPackages != null)
				{
					list.AddRange(this.SpeechPackages);
				}
				return list;
			}
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000045 RID: 69 RVA: 0x00003EF4 File Offset: 0x000020F4
		// (set) Token: 0x06000046 RID: 70 RVA: 0x00003EFC File Offset: 0x000020FC
		[XmlIgnore]
		public OEMInput OemInput
		{
			get
			{
				return this._oemInput;
			}
			set
			{
				this._oemInput = value;
				foreach (PkgFile pkgFile in this._allPackages)
				{
					pkgFile.OemInput = this._oemInput;
				}
			}
		}

		// Token: 0x06000047 RID: 71 RVA: 0x00003F54 File Offset: 0x00002154
		public List<string> GetFeatureIDs(bool fMSFeatures, bool fOEMFeatures)
		{
			List<string> list = new List<string>();
			if (this.Features != null)
			{
				if (this.Features.Microsoft != null && fMSFeatures)
				{
					foreach (OptionalPkgFile optionalPkgFile in this.Features.Microsoft)
					{
						list.AddRange(optionalPkgFile.FeatureIDs);
					}
				}
				if (this.Features.OEM != null && fOEMFeatures)
				{
					foreach (OptionalPkgFile optionalPkgFile2 in this.Features.OEM)
					{
						list.AddRange(optionalPkgFile2.FeatureIDs);
					}
				}
			}
			return list.Distinct(FeatureManifest.IgnoreCase).ToList<string>();
		}

		// Token: 0x06000048 RID: 72 RVA: 0x00004044 File Offset: 0x00002244
		private List<FeatureManifest.FMPkgInfo> GetPackagesFromList(PkgFile pkg, List<string> listValues)
		{
			List<FeatureManifest.FMPkgInfo> list = new List<FeatureManifest.FMPkgInfo>();
			foreach (string text in listValues)
			{
				FeatureManifest.FMPkgInfo fmpkgInfo = new FeatureManifest.FMPkgInfo(pkg, pkg.GroupValue);
				FeatureManifest.PackageGroups fmgroup = pkg.FMGroup;
				if (fmgroup <= FeatureManifest.PackageGroups.BOOTLOCALE)
				{
					if (fmgroup != FeatureManifest.PackageGroups.BOOTUI)
					{
						if (fmgroup != FeatureManifest.PackageGroups.BOOTLOCALE)
						{
							goto IL_11B;
						}
						fmpkgInfo.PackagePath = this.BootLocalePackageFile.PackagePath.Replace("$(bootlocale)", text, StringComparison.OrdinalIgnoreCase);
						fmpkgInfo.ID = this.BootLocalePackageFile.ID.Replace("$(bootlocale)", text, StringComparison.OrdinalIgnoreCase);
					}
					else
					{
						fmpkgInfo.PackagePath = pkg.PackagePath.Replace("$(bootuilanguage)", text, StringComparison.OrdinalIgnoreCase);
						fmpkgInfo.ID = this.BootUILanguagePackageFile.ID.Replace("$(bootuilanguage)", text, StringComparison.OrdinalIgnoreCase);
					}
				}
				else
				{
					if (fmgroup != FeatureManifest.PackageGroups.KEYBOARD && fmgroup != FeatureManifest.PackageGroups.SPEECH)
					{
						goto IL_11B;
					}
					fmpkgInfo.ID = pkg.ID + PkgFile.DefaultLanguagePattern + text;
					string text2 = pkg.PackagePath;
					string extension = Path.GetExtension(text2);
					text2 = text2.Replace(extension, PkgFile.DefaultLanguagePattern + text + extension, StringComparison.OrdinalIgnoreCase);
					fmpkgInfo.PackagePath = text2;
				}
				fmpkgInfo.GroupValue = text;
				list.Add(fmpkgInfo);
				continue;
				IL_11B:
				throw new FeatureAPIException(string.Concat(new object[]
				{
					"FeatureAPI!GetPackagesFromList: Called with non supported FMGroup '",
					pkg.FMGroup,
					"' for package '",
					pkg.PackagePath,
					"'"
				}));
			}
			return list;
		}

		// Token: 0x06000049 RID: 73 RVA: 0x000041F4 File Offset: 0x000023F4
		private List<FeatureManifest.FMPkgInfo> GetSatellites(PkgFile pkg, List<string> supportedUILanguages, List<string> supportedResolutions, List<CpuId> supportedWowCputypes, string cpuType, string groupValue = null)
		{
			List<FeatureManifest.FMPkgInfo> list = new List<FeatureManifest.FMPkgInfo>();
			string groupValue2 = groupValue ?? pkg.GroupValue;
			if (groupValue != null && string.IsNullOrWhiteSpace(groupValue) && pkg.GroupValues.Count<string>() > 1)
			{
				throw new FeatureAPIException("FeatureAPI!GetSatellites: Called with multiple group values '" + pkg.GroupValues + "' for package and no override");
			}
			if (!string.IsNullOrEmpty(pkg.Language) && supportedUILanguages.Count != 0)
			{
				foreach (string text in PkgFile.GetSupportedList(pkg.Language, supportedUILanguages))
				{
					list.Add(new FeatureManifest.FMPkgInfo(pkg, groupValue2)
					{
						Language = text,
						PackagePath = pkg.GetLanguagePackagePath(text),
						RawBasePath = pkg.RawPackagePath,
						ID = pkg.ID + PkgFile.DefaultLanguagePattern + text,
						FeatureIdentifierPackage = false,
						PublicKey = pkg.PublicKey
					});
				}
			}
			if (!string.IsNullOrEmpty(pkg.Resolution) && supportedResolutions.Count != 0)
			{
				foreach (string text2 in PkgFile.GetSupportedList(pkg.Resolution, supportedResolutions))
				{
					list.Add(new FeatureManifest.FMPkgInfo(pkg, groupValue2)
					{
						Resolution = text2,
						PackagePath = pkg.GetResolutionPackagePath(text2),
						RawBasePath = pkg.RawPackagePath,
						ID = pkg.ID + PkgFile.DefaultResolutionPattern + text2,
						FeatureIdentifierPackage = false
					});
				}
			}
			bool flag = !string.IsNullOrEmpty(pkg.Wow);
			bool flag2 = flag && !string.IsNullOrEmpty(pkg.LangWow) && !string.IsNullOrEmpty(pkg.Language);
			bool flag3 = flag && !string.IsNullOrEmpty(pkg.ResWow) && !string.IsNullOrEmpty(pkg.Resolution);
			if (flag && supportedWowCputypes.Any<CpuId>())
			{
				List<string> supportedList = PkgFile.GetSupportedList(pkg.Wow, (from cpuid in supportedWowCputypes
				select cpuid.ToString()).ToList<string>());
				List<FeatureManifest.FMPkgInfo> list2 = new List<FeatureManifest.FMPkgInfo>();
				List<string> list3 = pkg.Wow.Equals("*") ? supportedList : supportedList.Intersect(pkg.Wow.Split(new char[]
				{
					';'
				}, StringSplitOptions.RemoveEmptyEntries), StringComparer.OrdinalIgnoreCase).ToList<string>();
				List<string> list4 = flag2 ? (pkg.LangWow.Equals("*") ? supportedList : supportedList.Intersect(pkg.LangWow.Split(new char[]
				{
					';'
				}, StringSplitOptions.RemoveEmptyEntries), StringComparer.OrdinalIgnoreCase).ToList<string>()) : new List<string>();
				List<string> list5 = flag3 ? (pkg.ResWow.Equals("*") ? supportedList : supportedList.Intersect(pkg.ResWow.Split(new char[]
				{
					';'
				}, StringSplitOptions.RemoveEmptyEntries), StringComparer.OrdinalIgnoreCase).ToList<string>()) : new List<string>();
				foreach (FeatureManifest.FMPkgInfo fmpkgInfo in list)
				{
					List<string> list6 = new List<string>();
					if (flag2 && !string.IsNullOrEmpty(fmpkgInfo.Language))
					{
						list6 = list4;
					}
					else if (flag3 && !string.IsNullOrEmpty(fmpkgInfo.Resolution))
					{
						list6 = list5;
					}
					foreach (string text3 in list6)
					{
						string text4 = (cpuType + "." + text3.ToString()).ToLower(CultureInfo.InvariantCulture);
						FeatureManifest.FMPkgInfo fmpkgInfo2 = new FeatureManifest.FMPkgInfo(fmpkgInfo);
						fmpkgInfo2.Wow = text4;
						string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fmpkgInfo2.PackagePath);
						fmpkgInfo2.PackagePath = fmpkgInfo2.PackagePath.Replace("$(mspackageroot)", "$(mspackageroot)\\Wow\\" + text4, StringComparison.OrdinalIgnoreCase);
						fmpkgInfo2.PackagePath = fmpkgInfo2.PackagePath.Replace(fileNameWithoutExtension, fileNameWithoutExtension + PkgFile.DefaultWowPattern + text4, StringComparison.OrdinalIgnoreCase);
						fmpkgInfo2.ID = fmpkgInfo2.ID + PkgFile.DefaultWowPattern + text4;
						list2.Add(fmpkgInfo2);
					}
				}
				list.AddRange(list2);
				if (flag)
				{
					foreach (string text5 in list3)
					{
						string text6 = cpuType + "." + text5.ToString();
						FeatureManifest.FMPkgInfo fmpkgInfo3 = new FeatureManifest.FMPkgInfo(pkg, groupValue2);
						fmpkgInfo3.Wow = text6;
						fmpkgInfo3.PackagePath = pkg.GetWowPackagePath(text5);
						fmpkgInfo3.PackagePath = fmpkgInfo3.PackagePath.Replace("$(mspackageroot)", "$(mspackageroot)\\Wow\\" + text6, StringComparison.OrdinalIgnoreCase);
						fmpkgInfo3.ID = pkg.ID + PkgFile.DefaultWowPattern + text6.ToString();
						fmpkgInfo3.FeatureIdentifierPackage = false;
						list.Add(fmpkgInfo3);
					}
				}
			}
			return list;
		}

		// Token: 0x0600004A RID: 74 RVA: 0x000047B0 File Offset: 0x000029B0
		public List<string> GetPRSPackages(List<string> supportedUILanguages, List<string> supportedLocales, List<string> supportedResolutions, List<CpuId> supportedWowCpuTypes, string buildType, string cpuType, string msPackageRoot)
		{
			return (from pkg in this.GetAllPackagesByGroups(supportedUILanguages, supportedLocales, supportedResolutions, supportedWowCpuTypes, buildType, cpuType, msPackageRoot)
			where (!pkg.FMGroup.Equals(FeatureManifest.PackageGroups.RELEASE) || !string.Equals(pkg.GroupValue, "Test", StringComparison.OrdinalIgnoreCase)) && !pkg.FMGroup.Equals(FeatureManifest.PackageGroups.OEMFEATURE)
			select pkg.PackagePath).Distinct(FeatureManifest.IgnoreCase).ToList<string>();
		}

		// Token: 0x0600004B RID: 75 RVA: 0x00004825 File Offset: 0x00002A25
		public List<FeatureManifest.FMPkgInfo> GetAllPackageByGroups(List<string> supportedUILanguages, List<string> supportedLocales, List<string> supportedResolutions, string buildType, string cpuType, string msPackageRoot)
		{
			return this.GetAllPackagesByGroups(supportedUILanguages, supportedLocales, supportedResolutions, new List<CpuId>(), buildType, cpuType, msPackageRoot);
		}

		// Token: 0x0600004C RID: 76 RVA: 0x0000483C File Offset: 0x00002A3C
		public List<FeatureManifest.FMPkgInfo> GetAllPackagesByGroups(List<string> supportedUILanguages, List<string> supportedLocales, List<string> supportedResolutions, List<CpuId> supportedWowCpuTypes, string buildType, string cpuType, string msPackageRoot)
		{
			List<FeatureManifest.FMPkgInfo> list = new List<FeatureManifest.FMPkgInfo>();
			if (string.IsNullOrEmpty(buildType))
			{
				buildType = Environment.GetEnvironmentVariable("_BUILDTYPE");
			}
			if (string.IsNullOrEmpty(cpuType))
			{
				cpuType = Environment.GetEnvironmentVariable("_BUILDARCH");
			}
			foreach (PkgFile pkgFile in this._allPackages)
			{
				if (string.IsNullOrEmpty(cpuType) || pkgFile.IncludesCPUType(cpuType))
				{
					FeatureManifest.PackageGroups fmgroup = pkgFile.FMGroup;
					if (fmgroup != FeatureManifest.PackageGroups.BOOTUI)
					{
						if (fmgroup != FeatureManifest.PackageGroups.BOOTLOCALE)
						{
							FeatureManifest.FMPkgInfo fmpkgInfo;
							switch (fmgroup)
							{
							case FeatureManifest.PackageGroups.MSFEATURE:
							case FeatureManifest.PackageGroups.OEMFEATURE:
								using (List<string>.Enumerator enumerator2 = (pkgFile as OptionalPkgFile).FeatureIDs.GetEnumerator())
								{
									while (enumerator2.MoveNext())
									{
										string groupValue = enumerator2.Current;
										if (!pkgFile.NoBasePackage)
										{
											PkgFile pkgFile2 = pkgFile;
											fmpkgInfo = new FeatureManifest.FMPkgInfo(pkgFile2, pkgFile2.GroupValue);
											fmpkgInfo.GroupValue = groupValue;
											list.Add(fmpkgInfo);
										}
										list.AddRange(this.GetSatellites(pkgFile, supportedUILanguages, supportedResolutions, supportedWowCpuTypes, cpuType, groupValue));
									}
									continue;
								}
								break;
							case FeatureManifest.PackageGroups.KEYBOARD:
							case FeatureManifest.PackageGroups.SPEECH:
							{
								List<string> supportedList = PkgFile.GetSupportedList(pkgFile.Language);
								list.AddRange(this.GetPackagesFromList(pkgFile, supportedList));
								continue;
							}
							}
							if (pkgFile.GroupValues != null)
							{
								using (List<string>.Enumerator enumerator2 = pkgFile.GroupValues.GetEnumerator())
								{
									while (enumerator2.MoveNext())
									{
										string groupValue2 = enumerator2.Current;
										fmpkgInfo = new FeatureManifest.FMPkgInfo(pkgFile, groupValue2);
										list.Add(fmpkgInfo);
										list.AddRange(this.GetSatellites(pkgFile, supportedUILanguages, supportedResolutions, supportedWowCpuTypes, cpuType, groupValue2));
									}
									continue;
								}
							}
							fmpkgInfo = new FeatureManifest.FMPkgInfo(pkgFile, null);
							list.Add(fmpkgInfo);
							list.AddRange(this.GetSatellites(pkgFile, supportedUILanguages, supportedResolutions, supportedWowCpuTypes, cpuType, null));
						}
						else
						{
							list.AddRange(this.GetPackagesFromList(pkgFile, supportedLocales));
						}
					}
					else
					{
						list.AddRange(this.GetPackagesFromList(pkgFile, supportedUILanguages));
					}
				}
			}
			this.ProcessVariablesForList(ref list, buildType, cpuType, msPackageRoot);
			return list;
		}

		// Token: 0x0600004D RID: 77 RVA: 0x00004A84 File Offset: 0x00002C84
		public List<string> GetUILangFeatures(List<string> packages)
		{
			List<string> result = new List<string>();
			PkgFile pkgFile = this._allPackages.First((PkgFile pkg) => pkg.Language != null && pkg.Language.Equals("*"));
			if (pkgFile != null)
			{
				result = this.GetValuesForPackagesMatchingPattern(pkgFile.ID, packages, PkgFile.DefaultLanguagePattern).ToList<string>();
			}
			return result;
		}

		// Token: 0x0600004E RID: 78 RVA: 0x00004AE0 File Offset: 0x00002CE0
		public List<string> GetResolutionFeatures(List<string> packages)
		{
			List<string> result = new List<string>();
			PkgFile pkgFile = this._allPackages.First((PkgFile pkg) => pkg.Resolution != null && pkg.Resolution.Equals("*"));
			if (pkgFile != null)
			{
				result = this.GetValuesForPackagesMatchingPattern(pkgFile.ID, packages, PkgFile.DefaultResolutionPattern).ToList<string>();
			}
			return result;
		}

		// Token: 0x0600004F RID: 79 RVA: 0x00004B3C File Offset: 0x00002D3C
		private List<string> GetValuesForPackagesMatchingPattern(string pattern, List<string> packages, string satelliteStr)
		{
			List<string> list = new List<string>();
			string text = pattern.ToUpper(CultureInfo.InvariantCulture);
			text = text.Replace(PkgConstants.c_strPackageExtension.ToUpper(CultureInfo.InvariantCulture), "", StringComparison.OrdinalIgnoreCase);
			if (!string.IsNullOrEmpty(satelliteStr) && !text.EndsWith(satelliteStr, StringComparison.OrdinalIgnoreCase))
			{
				text += satelliteStr;
			}
			foreach (string text2 in packages)
			{
				if (text2.StartsWith(text, StringComparison.OrdinalIgnoreCase))
				{
					list.Add(text2.Substring(text.Length));
				}
			}
			return list;
		}

		// Token: 0x06000050 RID: 80 RVA: 0x00004BE8 File Offset: 0x00002DE8
		public List<FeatureManifest.FMPkgInfo> GetFeatureIdentifierPackages()
		{
			List<string> supportedUILanguages = new List<string>();
			List<string> supportedLocales = new List<string>();
			List<string> supportedResolutions = new List<string>();
			string cpuType = "";
			string buildType = "";
			string msPackageRoot = "";
			return (from pkg in this.GetAllPackageByGroups(supportedUILanguages, supportedLocales, supportedResolutions, buildType, cpuType, msPackageRoot)
			where pkg.FeatureIdentifierPackage
			select pkg).ToList<FeatureManifest.FMPkgInfo>();
		}

		// Token: 0x06000051 RID: 81 RVA: 0x00004C54 File Offset: 0x00002E54
		private void ProcessVariablesForList(ref List<FeatureManifest.FMPkgInfo> packageList, string buildType, string cpuType, string msPackageRoot)
		{
			for (int i = 0; i < packageList.Count; i++)
			{
				FeatureManifest.FMPkgInfo fmpkgInfo = packageList[i];
				fmpkgInfo.PackagePath = FeatureManifest.ProcessVariablesForPkgPath(fmpkgInfo.PackagePath, buildType, cpuType, msPackageRoot);
				packageList[i] = fmpkgInfo;
			}
		}

		// Token: 0x06000052 RID: 82 RVA: 0x00004C9C File Offset: 0x00002E9C
		private static string ProcessVariablesForPkgPath(string packageFile, string buildType, string cpuType, string msPackageRoot)
		{
			string text = packageFile;
			if (!string.IsNullOrEmpty(buildType))
			{
				text = text.Replace("$(buildtype)", buildType, StringComparison.OrdinalIgnoreCase);
			}
			if (!string.IsNullOrEmpty(cpuType))
			{
				text = text.Replace("$(cputype)", cpuType, StringComparison.OrdinalIgnoreCase);
			}
			if (!string.IsNullOrEmpty(msPackageRoot))
			{
				text = text.Replace("$(mspackageroot)", msPackageRoot, StringComparison.OrdinalIgnoreCase);
			}
			return Environment.ExpandEnvironmentVariables(text);
		}

		// Token: 0x06000053 RID: 83 RVA: 0x00004CF5 File Offset: 0x00002EF5
		public List<string> GetAllPackageFileList(List<string> supportedUILanguages, List<string> supportedResolutions, List<string> supportedLocales, string buildType, string cpuType, string msPackageRoot)
		{
			return this.GetAllPackageFilesList(supportedUILanguages, supportedResolutions, supportedLocales, new List<CpuId>(), buildType, cpuType, msPackageRoot);
		}

		// Token: 0x06000054 RID: 84 RVA: 0x00004D0C File Offset: 0x00002F0C
		public List<string> GetAllPackageFilesList(List<string> supportedUILanguages, List<string> supportedResolutions, List<string> supportedLocales, List<CpuId> supportedWowTypes, string buildType, string cpuType, string msPackageRoot)
		{
			List<string> list = new List<string>();
			if (string.IsNullOrEmpty(buildType))
			{
				buildType = Environment.GetEnvironmentVariable("_BUILDTYPE");
				if (string.IsNullOrEmpty(buildType))
				{
					buildType = "fre";
				}
			}
			if (string.IsNullOrEmpty(cpuType))
			{
				cpuType = Environment.GetEnvironmentVariable("_BUILDARCH");
				if (string.IsNullOrEmpty(cpuType))
				{
					cpuType = FeatureManifest.CPUType_ARM;
				}
			}
			List<FeatureManifest.FMPkgInfo> allPackagesByGroups = this.GetAllPackagesByGroups(supportedUILanguages, supportedLocales, supportedResolutions, supportedWowTypes, buildType, cpuType, msPackageRoot);
			list.AddRange(from pkg in allPackagesByGroups
			select pkg.PackagePath);
			return list.Distinct(FeatureManifest.IgnoreCase).ToList<string>();
		}

		// Token: 0x06000055 RID: 85 RVA: 0x00004DB4 File Offset: 0x00002FB4
		public List<string> GetPackageFileList()
		{
			List<string> list = new List<string>();
			List<FeatureManifest.FMPkgInfo> source = new List<FeatureManifest.FMPkgInfo>();
			source = this.GetFilteredPackagesByGroups();
			list.AddRange(from pkg in source
			select pkg.PackagePath);
			return list.Distinct(FeatureManifest.IgnoreCase).ToList<string>();
		}

		// Token: 0x06000056 RID: 86 RVA: 0x00004E10 File Offset: 0x00003010
		public List<FeatureManifest.FMPkgInfo> GetFilteredPackagesByGroups()
		{
			List<string> list = new List<string>();
			char[] separators = new char[]
			{
				';'
			};
			if (this._oemInput == null)
			{
				throw new FeatureAPIException("FeatureAPI!GetFilteredPackagesByGroups: The OEMInput reference cannot be null.  Set the OEMInput before calling this function.");
			}
			if (this.Features != null)
			{
				this.Features.ValidateConstraints(this._oemInput.MSFeatureIDs, this._oemInput.OEMFeatureIDs);
			}
			list.Add(this._oemInput.BootLocale);
			return (from pkg in this.GetAllPackagesByGroups(this._oemInput.SupportedLanguages.UserInterface, list, this._oemInput.Resolutions, this._oemInput.Edition.GetSupportedWowCpuTypes(CpuIdParser.Parse(this._oemInput.CPUType)), this._oemInput.BuildType, this._oemInput.CPUType, this._oemInput.MSPackageRoot)
			where pkg.FMGroup.Equals(FeatureManifest.PackageGroups.BASE) || (pkg.FMGroup.Equals(FeatureManifest.PackageGroups.RELEASE) && string.Equals(pkg.GroupValue, this._oemInput.ReleaseType, StringComparison.OrdinalIgnoreCase)) || (pkg.FMGroup.Equals(FeatureManifest.PackageGroups.PRERELEASE) && this._oemInput.ExcludePrereleaseFeatures && string.Equals(pkg.GroupValue, "replacement", StringComparison.OrdinalIgnoreCase)) || (!this._oemInput.ExcludePrereleaseFeatures && string.Equals(pkg.GroupValue, "protected", StringComparison.OrdinalIgnoreCase)) || (pkg.FMGroup.Equals(FeatureManifest.PackageGroups.SV) && string.Equals(pkg.GroupValue, this._oemInput.SV, StringComparison.OrdinalIgnoreCase)) || (pkg.FMGroup.Equals(FeatureManifest.PackageGroups.SOC) && string.Equals(pkg.GroupValue, this._oemInput.SOC, StringComparison.OrdinalIgnoreCase)) || (pkg.FMGroup.Equals(FeatureManifest.PackageGroups.DEVICE) && string.Equals(pkg.GroupValue, this._oemInput.Device, StringComparison.OrdinalIgnoreCase)) || (pkg.FMGroup.Equals(FeatureManifest.PackageGroups.DEVICELAYOUT) && string.Equals(pkg.GroupValue, this._oemInput.SOC, StringComparison.OrdinalIgnoreCase)) || (pkg.FMGroup.Equals(FeatureManifest.PackageGroups.OEMDEVICEPLATFORM) && string.Equals(pkg.GroupValue, this._oemInput.Device, StringComparison.OrdinalIgnoreCase)) || pkg.FMGroup.Equals(FeatureManifest.PackageGroups.BOOTLOCALE) || (pkg.FMGroup.Equals(FeatureManifest.PackageGroups.BOOTUI) && string.Equals(pkg.GroupValue, this._oemInput.BootUILanguage, StringComparison.OrdinalIgnoreCase)) || (pkg.FMGroup.Equals(FeatureManifest.PackageGroups.KEYBOARD) && this._oemInput.SupportedLanguages.Keyboard.Contains(pkg.GroupValue, FeatureManifest.IgnoreCase)) || (pkg.FMGroup.Equals(FeatureManifest.PackageGroups.SPEECH) && this._oemInput.SupportedLanguages.Speech.Contains(pkg.GroupValue, FeatureManifest.IgnoreCase)) || (pkg.FMGroup.Equals(FeatureManifest.PackageGroups.MSFEATURE) && this._oemInput.Features.Microsoft != null && this._oemInput.Features.Microsoft.Intersect(pkg.GroupValue.Split(separators, StringSplitOptions.RemoveEmptyEntries), FeatureManifest.IgnoreCase).Count<string>() > 0) || (pkg.FMGroup.Equals(FeatureManifest.PackageGroups.OEMFEATURE) && this._oemInput.Features.OEM != null && this._oemInput.Features.OEM.Intersect(pkg.GroupValue.Split(separators, StringSplitOptions.RemoveEmptyEntries), FeatureManifest.IgnoreCase).Count<string>() > 0)
			select pkg).ToList<FeatureManifest.FMPkgInfo>();
		}

		// Token: 0x06000057 RID: 87 RVA: 0x00004F08 File Offset: 0x00003108
		public void Merge(FeatureManifest sourceFM)
		{
			if (sourceFM.SchemaVersion != null)
			{
				if (string.Compare(sourceFM.SchemaVersion, "1.2", StringComparison.OrdinalIgnoreCase) > 0)
				{
					throw new FeatureAPIException("FeatureAPI!Merge: The source FM has a higher SchemaVersion than supported. Cannot merge.");
				}
				if (this.SchemaVersion == null)
				{
					this.SchemaVersion = sourceFM.SchemaVersion;
				}
				else
				{
					this.SchemaVersion = ((string.Compare(this.SchemaVersion, sourceFM.SchemaVersion, StringComparison.OrdinalIgnoreCase) > 0) ? this.SchemaVersion : sourceFM.SchemaVersion);
				}
			}
			if (this.SchemaVersion != null && string.Compare(this.SchemaVersion, "1.2", StringComparison.OrdinalIgnoreCase) > 0)
			{
				throw new FeatureAPIException("FeatureAPI!Merge: The current FM has a higher SchemaVersion than supported. Cannot merge.");
			}
			if (this.OwnerType != sourceFM.OwnerType && this.OwnerType == OwnerType.Invalid)
			{
				this.OwnerType = sourceFM.OwnerType;
			}
			if (this.ReleaseType != sourceFM.ReleaseType && this.ReleaseType == ReleaseType.Invalid)
			{
				this.ReleaseType = sourceFM.ReleaseType;
			}
			if (string.IsNullOrEmpty(this.Owner) != string.IsNullOrEmpty(sourceFM.Owner))
			{
				if (string.IsNullOrEmpty(this.Owner))
				{
					this.Owner = sourceFM.Owner;
				}
			}
			else if (!string.IsNullOrEmpty(this.Owner) && !this.Owner.Equals(sourceFM.Owner, StringComparison.OrdinalIgnoreCase))
			{
				throw new FeatureAPIException(string.Concat(new string[]
				{
					"FeatureAPI!Merge: The source FM and the destination FM have different Owners '",
					this.Owner,
					"' and '",
					sourceFM.Owner,
					"'. Cannot merge."
				}));
			}
			if (string.IsNullOrEmpty(this.ID) != string.IsNullOrEmpty(sourceFM.ID))
			{
				if (string.IsNullOrEmpty(this.ID))
				{
					this.Owner = sourceFM.ID;
				}
			}
			else if (!string.IsNullOrEmpty(this.ID) && !this.ID.Equals(sourceFM.ID, StringComparison.OrdinalIgnoreCase))
			{
				throw new FeatureAPIException(string.Concat(new string[]
				{
					"FeatureAPI!Merge: The source FM and the destination FM have different IDs '",
					this.ID,
					"' and '",
					sourceFM.ID,
					"'. Cannot merge."
				}));
			}
			if (sourceFM.BootUILanguagePackageFile != null)
			{
				if (this.BootUILanguagePackageFile != null)
				{
					throw new FeatureAPIException("FeatureAPI!Merge: The source FM and the destination FM cannot both contain BootUILanguagePackageFile. Cannot merge.");
				}
				this.BootUILanguagePackageFile = sourceFM.BootUILanguagePackageFile;
			}
			if (sourceFM.BootLocalePackageFile != null)
			{
				if (this.BootLocalePackageFile != null)
				{
					throw new FeatureAPIException("FeatureAPI!Merge: The source FM and the destination FM cannot both contain BootUILanguagePackageFile. Cannot merge.");
				}
				this.BootLocalePackageFile = sourceFM.BootLocalePackageFile;
			}
			if (sourceFM.BasePackages != null)
			{
				if (this.BasePackages == null)
				{
					this.BasePackages = sourceFM.BasePackages;
				}
				else
				{
					this.BasePackages.AddRange(sourceFM.BasePackages);
				}
			}
			if (sourceFM.ReleasePackages != null)
			{
				if (this.ReleasePackages == null)
				{
					this.ReleasePackages = sourceFM.ReleasePackages;
				}
				else
				{
					this.ReleasePackages.AddRange(sourceFM.ReleasePackages);
				}
			}
			if (sourceFM != null)
			{
				if (this.PrereleasePackages == null)
				{
					this.PrereleasePackages = sourceFM.PrereleasePackages;
				}
				else
				{
					this.PrereleasePackages.AddRange(sourceFM.PrereleasePackages);
				}
			}
			if (sourceFM.SVPackages != null)
			{
				if (this.SVPackages == null)
				{
					this.SVPackages = sourceFM.SVPackages;
				}
				else
				{
					this.SVPackages.AddRange(sourceFM.SVPackages);
				}
			}
			if (sourceFM.SOCPackages != null)
			{
				if (this.SOCPackages == null)
				{
					this.SOCPackages = sourceFM.SOCPackages;
				}
				else
				{
					this.SOCPackages.AddRange(sourceFM.SOCPackages);
				}
			}
			if (sourceFM.DeviceLayoutPackages != null)
			{
				if (this.DeviceLayoutPackages == null)
				{
					this.DeviceLayoutPackages = sourceFM.DeviceLayoutPackages;
				}
				else
				{
					this.DeviceLayoutPackages.AddRange(sourceFM.DeviceLayoutPackages);
				}
			}
			if (sourceFM.DeviceSpecificPackages != null)
			{
				if (this.DeviceSpecificPackages == null)
				{
					this.DeviceSpecificPackages = sourceFM.DeviceSpecificPackages;
				}
				else
				{
					this.DeviceSpecificPackages.AddRange(sourceFM.DeviceSpecificPackages);
				}
			}
			if (sourceFM.OEMDevicePlatformPackages != null)
			{
				if (this.OEMDevicePlatformPackages == null)
				{
					this.OEMDevicePlatformPackages = sourceFM.OEMDevicePlatformPackages;
				}
				else
				{
					this.OEMDevicePlatformPackages.AddRange(sourceFM.OEMDevicePlatformPackages);
				}
			}
			if (sourceFM.Features != null)
			{
				if (this.Features == null)
				{
					this.Features = sourceFM.Features;
				}
				else
				{
					this.Features.Merge(sourceFM.Features);
				}
			}
			if (sourceFM.SpeechPackages != null)
			{
				if (this.SpeechPackages == null)
				{
					this.SpeechPackages = sourceFM.SpeechPackages;
				}
				else
				{
					this.SpeechPackages.AddRange(sourceFM.SpeechPackages);
				}
			}
			if (sourceFM.KeyboardPackages != null)
			{
				if (this.KeyboardPackages == null)
				{
					this.KeyboardPackages = sourceFM.KeyboardPackages;
					return;
				}
				this.KeyboardPackages.AddRange(sourceFM.KeyboardPackages);
			}
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00005354 File Offset: 0x00003554
		public void WriteToFile(string fileName)
		{
			this.SchemaVersion = "1.2";
			this.Revision = "1";
			string directoryName = Path.GetDirectoryName(fileName);
			if (!LongPathDirectory.Exists(directoryName))
			{
				LongPathDirectory.CreateDirectory(directoryName);
			}
			TextWriter textWriter = new StreamWriter(LongPathFile.OpenWrite(fileName));
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(FeatureManifest));
			try
			{
				xmlSerializer.Serialize(textWriter, this);
			}
			catch (Exception innerException)
			{
				throw new FeatureAPIException("FeatureAPI!WriteToFile: Unable to write Feature Manifest XML file '" + fileName + "'", innerException);
			}
			finally
			{
				textWriter.Close();
			}
		}

		// Token: 0x06000059 RID: 89 RVA: 0x000053F0 File Offset: 0x000035F0
		public string GetOEMDevicePlatformPackage(string device)
		{
			string text = null;
			if (this.OEMDevicePlatformPackages != null)
			{
				foreach (DevicePkgFile devicePkgFile in this.OEMDevicePlatformPackages)
				{
					if (devicePkgFile.IsValidGroupValue(device))
					{
						text = devicePkgFile.PackagePath;
						break;
					}
				}
			}
			if (string.IsNullOrEmpty(text) && this.DeviceSpecificPackages != null)
			{
				DevicePkgFile devicePkgFile2 = this.DeviceSpecificPackages.Find((DevicePkgFile pkg) => pkg.FeatureIdentifierPackage && pkg.IsValidGroupValue(device));
				if (devicePkgFile2 != null)
				{
					text = devicePkgFile2.PackagePath;
				}
			}
			return text;
		}

		// Token: 0x0600005A RID: 90 RVA: 0x000054A0 File Offset: 0x000036A0
		public string GetDeviceLayoutPackage(string SOC)
		{
			string text = null;
			if (this.DeviceLayoutPackages != null)
			{
				foreach (SOCPkgFile socpkgFile in this.DeviceLayoutPackages)
				{
					if (socpkgFile.IsValidGroupValue(SOC))
					{
						text = socpkgFile.PackagePath;
						break;
					}
				}
			}
			if (string.IsNullOrEmpty(text) && this.SOCPackages != null)
			{
				SOCPkgFile socpkgFile2 = this.SOCPackages.Find((SOCPkgFile pkg) => pkg.FeatureIdentifierPackage && pkg.IsValidGroupValue(SOC));
				if (socpkgFile2 != null)
				{
					text = socpkgFile2.PackagePath;
				}
			}
			return text;
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00005550 File Offset: 0x00003750
		public void ProcessVariables()
		{
			foreach (PkgFile pkgFile in this._allPackages)
			{
				pkgFile.ProcessVariables();
			}
		}

		// Token: 0x0600005C RID: 92 RVA: 0x0000559C File Offset: 0x0000379C
		public static void ValidateAndLoad(ref FeatureManifest fm, string xmlFile, IULogger logger)
		{
			IULogger iulogger = new IULogger();
			iulogger.ErrorLogger = null;
			iulogger.InformationLogger = null;
			if (!LongPathFile.Exists(xmlFile))
			{
				throw new FeatureAPIException("FeatureAPI!ValidateAndLoad: FeatureManifest file was not found: " + xmlFile);
			}
			string text = string.Empty;
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			string[] manifestResourceNames = executingAssembly.GetManifestResourceNames();
			string featureManifestSchema = DevicePaths.FeatureManifestSchema;
			foreach (string text2 in manifestResourceNames)
			{
				if (text2.Contains(featureManifestSchema))
				{
					text = text2;
					break;
				}
			}
			if (string.IsNullOrEmpty(text))
			{
				throw new FeatureAPIException("FeatureAPI!ValidateAndLoad: XSD resource was not found: " + featureManifestSchema);
			}
			TextReader textReader = new StreamReader(LongPathFile.OpenRead(xmlFile));
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(FeatureManifest));
			try
			{
				fm = (FeatureManifest)xmlSerializer.Deserialize(textReader);
			}
			catch (Exception innerException)
			{
				throw new FeatureAPIException("FeatureAPI!ValidateInput: Unable to parse Feature Manifest XML file '" + xmlFile + "'.", innerException);
			}
			finally
			{
				textReader.Close();
			}
			bool flag = "1.2".Equals(fm.SchemaVersion);
			using (Stream manifestResourceStream = executingAssembly.GetManifestResourceStream(text))
			{
				XsdValidator xsdValidator = new XsdValidator();
				try
				{
					xsdValidator.ValidateXsd(manifestResourceStream, xmlFile, iulogger);
				}
				catch (XsdValidatorException innerException2)
				{
					if (flag)
					{
						throw new FeatureAPIException("FeatureAPI!ValidateInput: Unable to validate Feature Manifest XSD for file '" + xmlFile + "'.", innerException2);
					}
					logger.LogWarning("Warning: FeatureAPI!ValidateInput: Unable to validate Feature Manifest XSD for file '" + xmlFile + "'.", new object[0]);
					if (string.IsNullOrEmpty(fm.SchemaVersion))
					{
						logger.LogWarning("Warning: Schema Version was not given in FM. Most up to date Schema Version is {1}.", new object[]
						{
							"1.2"
						});
					}
					else
					{
						logger.LogWarning("Warning: Schema Version given in FM ({0}) does not match most up to date Schema Version ({1}).", new object[]
						{
							fm.SchemaVersion,
							"1.2"
						});
					}
				}
			}
			logger.LogInfo("FeatureAPI: Successfully validated the Feature Manifest XML: {0}", new object[]
			{
				xmlFile
			});
			if (fm.CPUPackages != null)
			{
				if (fm.BasePackages != null)
				{
					fm.BasePackages.AddRange(fm.CPUPackages);
				}
				else
				{
					FeatureManifest featureManifest = fm;
					featureManifest.BasePackages = featureManifest.CPUPackages;
				}
				fm.CPUPackages = new List<PkgFile>();
			}
			if (fm.Features != null && fm.Features.OEMConditionalFeatures != null)
			{
				if (fm.Features.OEMConditionalFeatures.Any((FMConditionalFeature cond) => cond.UpdateAction != FeatureCondition.Action.NoUpdate))
				{
					throw new FeatureAPIException("FeatureAPI!ValidateInput: Feature Manifest XML file '" + xmlFile + "' contains a OEMConditionalFeature with an UpdateAction other than 'NoUpdate'.  Only 'NoUpdate' is supported for OEMCondtionalFeatures");
				}
			}
			fm.SourceFile = Path.GetFileName(xmlFile).ToUpper(CultureInfo.InvariantCulture);
		}

		// Token: 0x0600005D RID: 93 RVA: 0x0000584C File Offset: 0x00003A4C
		public void AddPkgFile(PkgFile pkgEntry)
		{
			switch (pkgEntry.FMGroup)
			{
			case FeatureManifest.PackageGroups.BASE:
				if (pkgEntry != null)
				{
					PkgFile item = new PkgFile(pkgEntry);
					if (this.BasePackages == null)
					{
						this.BasePackages = new List<PkgFile>();
					}
					this.BasePackages.Add(item);
					return;
				}
				throw new FeatureAPIException(string.Concat(new object[]
				{
					"FeatureAPI!AddPkgFile: Expected 'PkgFile' package type in FMGroup '",
					pkgEntry.FMGroup,
					"' for package '",
					pkgEntry.ID,
					"'"
				}));
			case FeatureManifest.PackageGroups.RELEASE:
				if (pkgEntry is ReleasePkgFile)
				{
					ReleasePkgFile releasePkgFile = new ReleasePkgFile();
					releasePkgFile.CopyPkgFile(pkgEntry);
					if (this.ReleasePackages == null)
					{
						this.ReleasePackages = new List<ReleasePkgFile>();
					}
					this.ReleasePackages.Add(releasePkgFile);
					return;
				}
				throw new FeatureAPIException(string.Concat(new object[]
				{
					"FeatureAPI!AddPkgFile: Expected 'ReleasePkgFile' package type in FMGroup '",
					pkgEntry.FMGroup,
					"' for package '",
					pkgEntry.ID,
					"'"
				}));
			case FeatureManifest.PackageGroups.DEVICELAYOUT:
				if (pkgEntry is DeviceLayoutPkgFile)
				{
					DeviceLayoutPkgFile deviceLayoutPkgFile = new DeviceLayoutPkgFile();
					deviceLayoutPkgFile.CopyPkgFile(pkgEntry);
					if (this.DeviceLayoutPackages == null)
					{
						this.DeviceLayoutPackages = new List<DeviceLayoutPkgFile>();
					}
					this.DeviceLayoutPackages.Add(deviceLayoutPkgFile);
					return;
				}
				throw new FeatureAPIException(string.Concat(new object[]
				{
					"FeatureAPI!AddPkgFile: Expected 'DeviceLayoutPkgFile' package type in FMGroup '",
					pkgEntry.FMGroup,
					"' for package '",
					pkgEntry.ID,
					"'"
				}));
			case FeatureManifest.PackageGroups.OEMDEVICEPLATFORM:
				if (pkgEntry is OEMDevicePkgFile)
				{
					OEMDevicePkgFile oemdevicePkgFile = new OEMDevicePkgFile();
					oemdevicePkgFile.CopyPkgFile(pkgEntry);
					if (this.OEMDevicePlatformPackages == null)
					{
						this.OEMDevicePlatformPackages = new List<OEMDevicePkgFile>();
					}
					this.OEMDevicePlatformPackages.Add(oemdevicePkgFile);
					return;
				}
				throw new FeatureAPIException(string.Concat(new object[]
				{
					"FeatureAPI!AddPkgFile: Expected 'OEMDevicePkgFile' package type in FMGroup '",
					pkgEntry.FMGroup,
					"' for package '",
					pkgEntry.ID,
					"'"
				}));
			case FeatureManifest.PackageGroups.SV:
				if (pkgEntry is SVPkgFile)
				{
					SVPkgFile svpkgFile = new SVPkgFile();
					svpkgFile.CopyPkgFile(pkgEntry);
					if (this.SVPackages == null)
					{
						this.SVPackages = new List<SVPkgFile>();
					}
					this.SVPackages.Add(svpkgFile);
					return;
				}
				throw new FeatureAPIException(string.Concat(new object[]
				{
					"FeatureAPI!AddPkgFile: Expected 'SVPkgFile' package type in FMGroup '",
					pkgEntry.FMGroup,
					"' for package '",
					pkgEntry.ID,
					"'"
				}));
			case FeatureManifest.PackageGroups.SOC:
				if (pkgEntry is SOCPkgFile)
				{
					SOCPkgFile socpkgFile = new SOCPkgFile();
					socpkgFile.CPUType = null;
					socpkgFile.CopyPkgFile(pkgEntry);
					if (this.SOCPackages == null)
					{
						this.SOCPackages = new List<SOCPkgFile>();
					}
					this.SOCPackages.Add(socpkgFile);
					return;
				}
				throw new FeatureAPIException(string.Concat(new object[]
				{
					"FeatureAPI!AddPkgFile: Expected 'SOCPkgFile' package type in FMGroup '",
					pkgEntry.FMGroup,
					"' for package '",
					pkgEntry.ID,
					"'"
				}));
			case FeatureManifest.PackageGroups.DEVICE:
				if (pkgEntry is DevicePkgFile)
				{
					DevicePkgFile devicePkgFile = new DevicePkgFile();
					devicePkgFile.CopyPkgFile(pkgEntry);
					if (this.DeviceSpecificPackages == null)
					{
						this.DeviceSpecificPackages = new List<DevicePkgFile>();
					}
					this.DeviceSpecificPackages.Add(devicePkgFile);
					return;
				}
				throw new FeatureAPIException(string.Concat(new object[]
				{
					"FeatureAPI!AddPkgFile: Expected 'DevicePkgFile' package type in FMGroup '",
					pkgEntry.FMGroup,
					"' for package '",
					pkgEntry.ID,
					"'"
				}));
			case FeatureManifest.PackageGroups.MSFEATURE:
			case FeatureManifest.PackageGroups.OEMFEATURE:
				if (this.Features == null)
				{
					this.Features = new FMFeatures();
				}
				if (pkgEntry.FMGroup == FeatureManifest.PackageGroups.MSFEATURE)
				{
					if (this.Features.Microsoft == null)
					{
						this.Features.Microsoft = new List<MSOptionalPkgFile>();
					}
					if (pkgEntry is MSOptionalPkgFile)
					{
						MSOptionalPkgFile msoptionalPkgFile = new MSOptionalPkgFile();
						msoptionalPkgFile.CopyPkgFile(pkgEntry);
						this.Features.Microsoft.Add(msoptionalPkgFile);
						return;
					}
					throw new FeatureAPIException(string.Concat(new object[]
					{
						"FeatureAPI!AddPkgFile: Expected 'MSOptionalPkgFile' package type in FMGroup '",
						pkgEntry.FMGroup,
						"' for package '",
						pkgEntry.ID,
						"'"
					}));
				}
				else
				{
					if (this.Features.OEM == null)
					{
						this.Features.OEM = new List<OEMOptionalPkgFile>();
					}
					if (pkgEntry is OEMOptionalPkgFile)
					{
						OEMOptionalPkgFile oemoptionalPkgFile = new OEMOptionalPkgFile();
						oemoptionalPkgFile.CopyPkgFile(pkgEntry);
						this.Features.OEM.Add(oemoptionalPkgFile);
						return;
					}
					throw new FeatureAPIException(string.Concat(new object[]
					{
						"FeatureAPI!AddPkgFile: Expected 'OEMOptionalPkgFile' package type in FMGroup '",
						pkgEntry.FMGroup,
						"' for package '",
						pkgEntry.ID,
						"'"
					}));
				}
				break;
			case FeatureManifest.PackageGroups.KEYBOARD:
				if (pkgEntry is KeyboardPkgFile)
				{
					KeyboardPkgFile keyboardPkgFile = new KeyboardPkgFile();
					keyboardPkgFile.CopyPkgFile(pkgEntry);
					if (this.KeyboardPackages == null)
					{
						this.KeyboardPackages = new List<KeyboardPkgFile>();
					}
					this.KeyboardPackages.Add(keyboardPkgFile);
					return;
				}
				throw new FeatureAPIException(string.Concat(new object[]
				{
					"FeatureAPI!AddPkgFile: Expected 'KeyboardPkgFile' package type in FMGroup '",
					pkgEntry.FMGroup,
					"' for package '",
					pkgEntry.ID,
					"'"
				}));
			case FeatureManifest.PackageGroups.SPEECH:
				if (pkgEntry is SpeechPkgFile)
				{
					SpeechPkgFile speechPkgFile = new SpeechPkgFile();
					speechPkgFile.CopyPkgFile(pkgEntry);
					if (this.SpeechPackages == null)
					{
						this.SpeechPackages = new List<SpeechPkgFile>();
					}
					this.SpeechPackages.Add(speechPkgFile);
					return;
				}
				throw new FeatureAPIException(string.Concat(new object[]
				{
					"FeatureAPI!AddPkgFile: Expected 'SpeechPkgFile' package type in FMGroup '",
					pkgEntry.FMGroup,
					"' for package '",
					pkgEntry.ID,
					"'"
				}));
			case FeatureManifest.PackageGroups.PRERELEASE:
				if (pkgEntry is PrereleasePkgFile)
				{
					PrereleasePkgFile prereleasePkgFile = new PrereleasePkgFile();
					prereleasePkgFile.CopyPkgFile(pkgEntry);
					if (this.PrereleasePackages == null)
					{
						this.PrereleasePackages = new List<PrereleasePkgFile>();
					}
					this.PrereleasePackages.Add(prereleasePkgFile);
					return;
				}
				throw new FeatureAPIException(string.Concat(new object[]
				{
					"FeatureAPI!AddPkgFile: Expected 'PrereleasePkgFile' package type in FMGroup '",
					pkgEntry.FMGroup,
					"' for package '",
					pkgEntry.ID,
					"'"
				}));
			}
			throw new FeatureAPIException(string.Concat(new object[]
			{
				"FeatureAPI!AddPkgFile: Unsupported FMGroup '",
				pkgEntry.FMGroup,
				"' for package '",
				pkgEntry.ID,
				"'.  Update the code to handle this new type."
			}));
		}

		// Token: 0x0600005E RID: 94 RVA: 0x00005EB4 File Offset: 0x000040B4
		public void AddPackagesFromMergeResult(List<FeatureManifest.FMPkgInfo> packageList, FeatureManifest.PackageGroups fmGroup, string fmGroupValue, List<MergeResult> results, List<string> supportedLanguages, List<string> supportedResolutions, string packageOutputDir, string packageOutputDirReplacement)
		{
			foreach (MergeResult mergeResult in results)
			{
				if ((fmGroup == FeatureManifest.PackageGroups.MSFEATURE || fmGroup == FeatureManifest.PackageGroups.OEMFEATURE) && this.Features == null)
				{
					this.Features = new FMFeatures();
				}
				PkgFile pkgFile = new PkgFile();
				bool flag = false;
				switch (fmGroup)
				{
				case FeatureManifest.PackageGroups.BASE:
					if (this.BasePackages == null)
					{
						this.BasePackages = new List<PkgFile>();
					}
					this.BasePackages.Add(pkgFile);
					break;
				case FeatureManifest.PackageGroups.RELEASE:
				{
					ReleasePkgFile releasePkgFile = new ReleasePkgFile();
					if (this.ReleasePackages == null)
					{
						this.ReleasePackages = new List<ReleasePkgFile>();
					}
					this.ReleasePackages.Add(releasePkgFile);
					pkgFile = releasePkgFile;
					break;
				}
				case FeatureManifest.PackageGroups.DEVICELAYOUT:
				{
					DeviceLayoutPkgFile deviceLayoutPkgFile = new DeviceLayoutPkgFile();
					if (this.DeviceLayoutPackages == null)
					{
						this.DeviceLayoutPackages = new List<DeviceLayoutPkgFile>();
					}
					deviceLayoutPkgFile.CPUType = mergeResult.PkgInfo.CpuType.ToString();
					this.DeviceLayoutPackages.Add(deviceLayoutPkgFile);
					pkgFile = deviceLayoutPkgFile;
					flag = true;
					break;
				}
				case FeatureManifest.PackageGroups.OEMDEVICEPLATFORM:
				{
					OEMDevicePkgFile oemdevicePkgFile = new OEMDevicePkgFile();
					if (this.OEMDevicePlatformPackages == null)
					{
						this.OEMDevicePlatformPackages = new List<OEMDevicePkgFile>();
					}
					this.OEMDevicePlatformPackages.Add(oemdevicePkgFile);
					pkgFile = oemdevicePkgFile;
					break;
				}
				case FeatureManifest.PackageGroups.SV:
				{
					SVPkgFile svpkgFile = new SVPkgFile();
					if (this.SVPackages == null)
					{
						this.SVPackages = new List<SVPkgFile>();
					}
					this.SVPackages.Add(svpkgFile);
					pkgFile = svpkgFile;
					break;
				}
				case FeatureManifest.PackageGroups.SOC:
				{
					SOCPkgFile socpkgFile = new SOCPkgFile();
					if (this.SOCPackages == null)
					{
						this.SOCPackages = new List<SOCPkgFile>();
					}
					socpkgFile.CPUType = mergeResult.PkgInfo.CpuType.ToString();
					this.SOCPackages.Add(socpkgFile);
					pkgFile = socpkgFile;
					break;
				}
				case FeatureManifest.PackageGroups.DEVICE:
				{
					DevicePkgFile devicePkgFile = new DevicePkgFile();
					if (this.DeviceSpecificPackages == null)
					{
						this.DeviceSpecificPackages = new List<DevicePkgFile>();
					}
					this.DeviceSpecificPackages.Add(devicePkgFile);
					pkgFile = devicePkgFile;
					break;
				}
				case FeatureManifest.PackageGroups.MSFEATURE:
				{
					MSOptionalPkgFile msoptionalPkgFile = new MSOptionalPkgFile();
					if (this.Features.Microsoft == null)
					{
						this.Features.Microsoft = new List<MSOptionalPkgFile>();
					}
					this.Features.Microsoft.Add(msoptionalPkgFile);
					pkgFile = msoptionalPkgFile;
					break;
				}
				case FeatureManifest.PackageGroups.OEMFEATURE:
				{
					OEMOptionalPkgFile oemoptionalPkgFile = new OEMOptionalPkgFile();
					if (this.Features.OEM == null)
					{
						this.Features.OEM = new List<OEMOptionalPkgFile>();
					}
					this.Features.OEM.Add(oemoptionalPkgFile);
					pkgFile = oemoptionalPkgFile;
					break;
				}
				case FeatureManifest.PackageGroups.KEYBOARD:
					if (this.KeyboardPackages == null)
					{
						this.KeyboardPackages = new List<KeyboardPkgFile>();
					}
					this.KeyboardPackages.Add(pkgFile as KeyboardPkgFile);
					break;
				case FeatureManifest.PackageGroups.SPEECH:
					if (this.SpeechPackages == null)
					{
						this.SpeechPackages = new List<SpeechPkgFile>();
					}
					this.SpeechPackages.Add(pkgFile as SpeechPkgFile);
					break;
				case FeatureManifest.PackageGroups.PRERELEASE:
				{
					PrereleasePkgFile prereleasePkgFile = new PrereleasePkgFile();
					if (this.PrereleasePackages == null)
					{
						this.PrereleasePackages = new List<PrereleasePkgFile>();
					}
					this.PrereleasePackages.Add(prereleasePkgFile);
					pkgFile = prereleasePkgFile;
					break;
				}
				}
				pkgFile.InitializeWithMergeResult(mergeResult, fmGroup, fmGroupValue, supportedLanguages, supportedResolutions);
				if (flag)
				{
					pkgFile.FeatureIdentifierPackage = false;
				}
				if (!string.IsNullOrEmpty(packageOutputDirReplacement))
				{
					pkgFile.Directory = pkgFile.Directory.Replace(packageOutputDir, packageOutputDirReplacement, StringComparison.OrdinalIgnoreCase);
				}
				if (!pkgFile.Directory.Contains(packageOutputDirReplacement))
				{
					FeatureManifest.FMPkgInfo fmpkgInfo = packageList.Single((FeatureManifest.FMPkgInfo pkg) => pkg.PackagePath.StartsWith(Path.ChangeExtension(pkgFile.PackagePath, ""), StringComparison.OrdinalIgnoreCase));
					if (fmpkgInfo != null)
					{
						pkgFile.Directory = LongPath.GetDirectoryName(fmpkgInfo.RawBasePath);
					}
				}
			}
		}

		// Token: 0x0600005F RID: 95 RVA: 0x000062C0 File Offset: 0x000044C0
		public static string GetFeatureIDWithFMID(string featureID, string fmID)
		{
			if (string.IsNullOrEmpty(fmID))
			{
				return featureID;
			}
			return featureID + "." + fmID;
		}

		// Token: 0x06000060 RID: 96 RVA: 0x000062D8 File Offset: 0x000044D8
		public List<string> CheckCBSFeaturesInfo(List<string> supportedUILanguages, List<string> supportedLocales, List<string> supportedResolutions, string product, string buildType, string cpuType, string msPackageRoot)
		{
			return this.CheckCBSFeaturesInfo(supportedUILanguages, supportedLocales, supportedResolutions, product, buildType, cpuType, msPackageRoot, true);
		}

		// Token: 0x06000061 RID: 97 RVA: 0x000062F8 File Offset: 0x000044F8
		public List<string> CheckCBSFeaturesInfo(List<string> supportedUILanguages, List<string> supportedLocales, List<string> supportedResolutions, string product, string buildType, string cpuType, string msPackageRoot, bool ignoreMissingSatellites)
		{
			List<string> list = new List<string>();
			CpuId hostCpuType;
			if (!Enum.TryParse<CpuId>(cpuType, out hostCpuType))
			{
				throw new FeatureAPIException("FeatureAPI!CheckCBSFeaturesInfo: The CpuType passed '" + cpuType + "' was not a valid CpuType");
			}
			Edition productEdition = ImagingEditions.GetProductEdition(product);
			List<CpuId> supportedWowCpuTypes = new List<CpuId>();
			if (productEdition != null)
			{
				supportedWowCpuTypes = productEdition.GetSupportedWowCpuTypes(hostCpuType);
			}
			List<FeatureManifest.FMPkgInfo> list2 = this.GetAllPackagesByGroups(supportedUILanguages, supportedLocales, supportedResolutions, supportedWowCpuTypes, buildType, cpuType, msPackageRoot);
			foreach (FeatureManifest.FMPkgInfo fmpkgInfo in from pkg in list2
			where pkg.FMGroup == FeatureManifest.PackageGroups.OEMDEVICEPLATFORM
			select pkg)
			{
				fmpkgInfo.FMGroup = FeatureManifest.PackageGroups.DEVICE;
			}
			foreach (FeatureManifest.FMPkgInfo fmpkgInfo2 in from pkg in list2
			where pkg.FMGroup == FeatureManifest.PackageGroups.DEVICELAYOUT
			select pkg)
			{
				fmpkgInfo2.FMGroup = FeatureManifest.PackageGroups.SOC;
			}
			List<FeatureManifest.FMPkgInfo> second = (from pkg in list2
			where Path.GetExtension(pkg.PackagePath).Equals(PkgConstants.c_strPackageExtension, StringComparison.OrdinalIgnoreCase)
			select pkg).ToList<FeatureManifest.FMPkgInfo>();
			list2 = list2.Except(second).ToList<FeatureManifest.FMPkgInfo>();
			using (List<string>.Enumerator enumerator2 = (from pkg in list2
			select pkg.FeatureID).Distinct(FeatureManifest.IgnoreCase).ToList<string>().GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					string feature = enumerator2.Current;
					List<FeatureManifest.FMPkgInfo> source = (from pkg in list2
					where feature.Equals(pkg.FeatureID, StringComparison.OrdinalIgnoreCase)
					select pkg).ToList<FeatureManifest.FMPkgInfo>();
					string pkgGroup = source.FirstOrDefault<FeatureManifest.FMPkgInfo>().FMGroup.ToString();
					List<string> pkgPaths = (from pkg in source
					select pkg.PackagePath).ToList<string>();
					if (!CbsPackage.CheckCBSFeatureInfo(this.ID, feature, pkgGroup, pkgPaths, ignoreMissingSatellites))
					{
						list.Add(feature);
					}
				}
			}
			return list;
		}

		// Token: 0x06000062 RID: 98 RVA: 0x0000655C File Offset: 0x0000475C
		public override string ToString()
		{
			return this.SourceFile;
		}

		// Token: 0x0400004D RID: 77
		public static readonly string CPUType_ARM = CpuId.ARM.ToString();

		// Token: 0x0400004E RID: 78
		public static readonly string CPUType_X86 = CpuId.X86.ToString();

		// Token: 0x0400004F RID: 79
		public static readonly string CPUType_ARM64 = CpuId.ARM64.ToString();

		// Token: 0x04000050 RID: 80
		public static readonly string CPUType_AMD64 = CpuId.AMD64.ToString();

		// Token: 0x04000051 RID: 81
		public const string FMRevisionNumber = "1";

		// Token: 0x04000052 RID: 82
		public const string FMSchemaVersion = "1.2";

		// Token: 0x04000053 RID: 83
		public const string BuildType_FRE = "fre";

		// Token: 0x04000054 RID: 84
		public const string BuildType_CHK = "chk";

		// Token: 0x04000055 RID: 85
		public const string Prerelease_Protected = "protected";

		// Token: 0x04000056 RID: 86
		public const string Prerelease_Protected_Replacement = "replacement";

		// Token: 0x04000057 RID: 87
		public const string MSPackageRootVariable = "$(mspackageroot)";

		// Token: 0x04000058 RID: 88
		public const string CpuTypeVariable = "$(cputype)";

		// Token: 0x04000059 RID: 89
		public const string BuildTypeVariable = "$(buildtype)";

		// Token: 0x0400005A RID: 90
		private static StringComparer IgnoreCase = StringComparer.OrdinalIgnoreCase;

		// Token: 0x0400005B RID: 91
		[XmlAttribute]
		public string Revision;

		// Token: 0x0400005C RID: 92
		[XmlAttribute]
		public string SchemaVersion;

		// Token: 0x0400005D RID: 93
		[XmlAttribute]
		[DefaultValue(ReleaseType.Invalid)]
		public ReleaseType ReleaseType;

		// Token: 0x0400005E RID: 94
		[XmlAttribute]
		[DefaultValue(OwnerType.Invalid)]
		public OwnerType OwnerType;

		// Token: 0x0400005F RID: 95
		private string _owner;

		// Token: 0x04000060 RID: 96
		[XmlAttribute]
		public string ID;

		// Token: 0x04000061 RID: 97
		[XmlAttribute]
		public string BuildInfo;

		// Token: 0x04000062 RID: 98
		[XmlAttribute]
		public string BuildID;

		// Token: 0x04000063 RID: 99
		private VersionInfo? _osVersion;

		// Token: 0x04000064 RID: 100
		[XmlArrayItem(ElementName = "PackageFile", Type = typeof(PkgFile), IsNullable = false)]
		[XmlArray]
		public List<PkgFile> BasePackages;

		// Token: 0x04000065 RID: 101
		[XmlArrayItem(ElementName = "PackageFile", Type = typeof(PkgFile), IsNullable = false)]
		[XmlArray]
		public List<PkgFile> CPUPackages;

		// Token: 0x04000066 RID: 102
		public BootUIPkgFile BootUILanguagePackageFile;

		// Token: 0x04000067 RID: 103
		public BootLocalePkgFile BootLocalePackageFile;

		// Token: 0x04000068 RID: 104
		public FMFeatures Features;

		// Token: 0x04000069 RID: 105
		[XmlArrayItem(ElementName = "PackageFile", Type = typeof(ReleasePkgFile), IsNullable = false)]
		[XmlArray]
		public List<ReleasePkgFile> ReleasePackages;

		// Token: 0x0400006A RID: 106
		[XmlArrayItem(ElementName = "PackageFile", Type = typeof(PrereleasePkgFile), IsNullable = false)]
		[XmlArray]
		public List<PrereleasePkgFile> PrereleasePackages;

		// Token: 0x0400006B RID: 107
		[XmlArrayItem(ElementName = "PackageFile", Type = typeof(OEMDevicePkgFile), IsNullable = false)]
		[XmlArray]
		public List<OEMDevicePkgFile> OEMDevicePlatformPackages;

		// Token: 0x0400006C RID: 108
		[XmlArrayItem(ElementName = "PackageFile", Type = typeof(DeviceLayoutPkgFile), IsNullable = false)]
		[XmlArray]
		public List<DeviceLayoutPkgFile> DeviceLayoutPackages;

		// Token: 0x0400006D RID: 109
		[XmlArrayItem(ElementName = "PackageFile", Type = typeof(SOCPkgFile), IsNullable = false)]
		[XmlArray]
		public List<SOCPkgFile> SOCPackages;

		// Token: 0x0400006E RID: 110
		[XmlArrayItem(ElementName = "PackageFile", Type = typeof(SVPkgFile), IsNullable = false)]
		[XmlArray]
		public List<SVPkgFile> SVPackages;

		// Token: 0x0400006F RID: 111
		[XmlArrayItem(ElementName = "PackageFile", Type = typeof(DevicePkgFile), IsNullable = false)]
		[XmlArray]
		public List<DevicePkgFile> DeviceSpecificPackages;

		// Token: 0x04000070 RID: 112
		[XmlArrayItem(ElementName = "PackageFile", Type = typeof(SpeechPkgFile), IsNullable = false)]
		[XmlArray]
		public List<SpeechPkgFile> SpeechPackages;

		// Token: 0x04000071 RID: 113
		[XmlArrayItem(ElementName = "PackageFile", Type = typeof(KeyboardPkgFile), IsNullable = false)]
		[XmlArray]
		public List<KeyboardPkgFile> KeyboardPackages;

		// Token: 0x04000072 RID: 114
		private OEMInput _oemInput;

		// Token: 0x04000073 RID: 115
		[XmlIgnore]
		public string SourceFile;

		// Token: 0x02000031 RID: 49
		public enum PackageGroups
		{
			// Token: 0x040000F4 RID: 244
			BASE,
			// Token: 0x040000F5 RID: 245
			BOOTUI,
			// Token: 0x040000F6 RID: 246
			BOOTLOCALE,
			// Token: 0x040000F7 RID: 247
			RELEASE,
			// Token: 0x040000F8 RID: 248
			DEVICELAYOUT,
			// Token: 0x040000F9 RID: 249
			OEMDEVICEPLATFORM,
			// Token: 0x040000FA RID: 250
			SV,
			// Token: 0x040000FB RID: 251
			SOC,
			// Token: 0x040000FC RID: 252
			DEVICE,
			// Token: 0x040000FD RID: 253
			MSFEATURE,
			// Token: 0x040000FE RID: 254
			OEMFEATURE,
			// Token: 0x040000FF RID: 255
			KEYBOARD,
			// Token: 0x04000100 RID: 256
			SPEECH,
			// Token: 0x04000101 RID: 257
			PRERELEASE
		}

		// Token: 0x02000032 RID: 50
		public class FMPkgInfo
		{
			// Token: 0x06000112 RID: 274 RVA: 0x00008BBC File Offset: 0x00006DBC
			public FMPkgInfo()
			{
			}

			// Token: 0x06000113 RID: 275 RVA: 0x00008BD0 File Offset: 0x00006DD0
			public FMPkgInfo(FeatureManifest.FMPkgInfo srcPkg)
			{
				this.FeatureIdentifierPackage = srcPkg.FeatureIdentifierPackage;
				this.FMGroup = srcPkg.FMGroup;
				this.GroupValue = srcPkg.GroupValue;
				this.ID = srcPkg.ID;
				this.Language = srcPkg.Language;
				this.Partition = srcPkg.Partition;
				this.PackagePath = srcPkg.PackagePath;
				this.RawBasePath = srcPkg.RawBasePath;
				this.Resolution = srcPkg.Resolution;
				this.Version = srcPkg.Version;
			}

			// Token: 0x06000114 RID: 276 RVA: 0x00008C68 File Offset: 0x00006E68
			public FMPkgInfo(PkgFile pkg, string groupValue)
			{
				this.PackagePath = pkg.PackagePath;
				this.RawBasePath = pkg.RawPackagePath;
				this.ID = Environment.ExpandEnvironmentVariables(pkg.ID);
				this.FeatureIdentifierPackage = pkg.FeatureIdentifierPackage;
				this.SetVersion(pkg.Version);
				this.Partition = pkg.Partition;
				this.FMGroup = pkg.FMGroup;
				this.GroupValue = groupValue;
				this.PublicKey = pkg.PublicKey;
				this.BinaryPartition = pkg.BinaryPartition;
			}

			// Token: 0x06000115 RID: 277 RVA: 0x00008D00 File Offset: 0x00006F00
			public FMPkgInfo(string packagePath, string id, FeatureManifest.PackageGroups fmGroup, string groupValue, string partition, string language, string resolution, bool featureIdentifierPackage, VersionInfo? version)
			{
				this.PackagePath = packagePath;
				this.ID = id;
				this.FMGroup = fmGroup;
				this.GroupValue = groupValue;
				this.Language = language;
				this.Partition = partition;
				this.Resolution = resolution;
				this.FeatureIdentifierPackage = featureIdentifierPackage;
				this.Version = version;
			}

			// Token: 0x06000116 RID: 278 RVA: 0x00008D64 File Offset: 0x00006F64
			public string[] GetGroupValueList()
			{
				string[] result = new string[0];
				if (this.FMGroup == FeatureManifest.PackageGroups.MSFEATURE || this.FMGroup == FeatureManifest.PackageGroups.OEMFEATURE)
				{
					result = this.GroupValue.Split(FeatureManifest.FMPkgInfo.separators, StringSplitOptions.RemoveEmptyEntries);
				}
				return result;
			}

			// Token: 0x06000117 RID: 279 RVA: 0x00008D9F File Offset: 0x00006F9F
			public bool IsOptionalCore()
			{
				return this.IsOptionalProductionCore() || this.IsOptionalTestCore();
			}

			// Token: 0x06000118 RID: 280 RVA: 0x00008DB1 File Offset: 0x00006FB1
			private bool IsOptionalProductionCore()
			{
				return FeatureManifest.FMPkgInfo.ProductionCoreOptionalFeatures.Intersect(this.GetGroupValueList(), FeatureManifest.IgnoreCase).Count<string>() > 0;
			}

			// Token: 0x06000119 RID: 281 RVA: 0x00008DD3 File Offset: 0x00006FD3
			private bool IsOptionalTestCore()
			{
				return FeatureManifest.FMPkgInfo.TestCoreOptionalFeatures.Intersect(this.GetGroupValueList(), FeatureManifest.IgnoreCase).Count<string>() > 0;
			}

			// Token: 0x0600011A RID: 282 RVA: 0x00008DF8 File Offset: 0x00006FF8
			public void SetVersion(string versionStr)
			{
				if (string.IsNullOrEmpty(versionStr))
				{
					this.Version = null;
					return;
				}
				VersionInfo value = default(VersionInfo);
				if (VersionInfo.TryParse(versionStr, out value))
				{
					this.Version = new VersionInfo?(value);
					return;
				}
				this.Version = null;
			}

			// Token: 0x17000034 RID: 52
			// (get) Token: 0x0600011B RID: 283 RVA: 0x00008E48 File Offset: 0x00007048
			public FeatureManifest.FMPkgInfo.CorePackageTypeEnum CorePackageType
			{
				get
				{
					if (this.FMGroup.Equals(FeatureManifest.PackageGroups.BASE) || this.FMGroup.Equals(FeatureManifest.PackageGroups.KEYBOARD) || this.FMGroup.Equals(FeatureManifest.PackageGroups.SPEECH) || this.FMGroup.Equals(FeatureManifest.PackageGroups.BOOTUI) || this.FMGroup.Equals(FeatureManifest.PackageGroups.BOOTLOCALE))
					{
						return FeatureManifest.FMPkgInfo.CorePackageTypeEnum.RetailCore | FeatureManifest.FMPkgInfo.CorePackageTypeEnum.NonRetailCore | FeatureManifest.FMPkgInfo.CorePackageTypeEnum.TestCore;
					}
					if (this.FMGroup == FeatureManifest.PackageGroups.RELEASE)
					{
						if (string.Equals(this.GroupValue, "Production", StringComparison.OrdinalIgnoreCase))
						{
							return FeatureManifest.FMPkgInfo.CorePackageTypeEnum.RetailCore;
						}
						return FeatureManifest.FMPkgInfo.CorePackageTypeEnum.NonRetailCore | FeatureManifest.FMPkgInfo.CorePackageTypeEnum.TestCore;
					}
					else
					{
						if (this.IsOptionalProductionCore())
						{
							return FeatureManifest.FMPkgInfo.CorePackageTypeEnum.NonRetailCore;
						}
						if (this.IsOptionalTestCore())
						{
							return FeatureManifest.FMPkgInfo.CorePackageTypeEnum.TestCore;
						}
						return FeatureManifest.FMPkgInfo.CorePackageTypeEnum.NonCore;
					}
				}
			}

			// Token: 0x17000035 RID: 53
			// (get) Token: 0x0600011C RID: 284 RVA: 0x00008F0C File Offset: 0x0000710C
			public string FeatureID
			{
				get
				{
					FeatureManifest.PackageGroups fmgroup = this.FMGroup;
					string result;
					if (fmgroup != FeatureManifest.PackageGroups.BASE)
					{
						if (fmgroup != FeatureManifest.PackageGroups.MSFEATURE)
						{
							if (fmgroup != FeatureManifest.PackageGroups.OEMFEATURE)
							{
								result = this.FMGroup.ToString() + "_" + this.GroupValue.ToUpper(CultureInfo.InvariantCulture);
							}
							else
							{
								result = "OEM_" + this.GroupValue;
							}
						}
						else
						{
							result = "MS_" + this.GroupValue;
						}
					}
					else
					{
						result = FeatureManifest.PackageGroups.BASE.ToString();
					}
					return result;
				}
			}

			// Token: 0x0600011D RID: 285 RVA: 0x00008F94 File Offset: 0x00007194
			public override string ToString()
			{
				return string.Format("{0} ({1})", this.ID, this.FeatureID);
			}

			// Token: 0x04000102 RID: 258
			public const string ReleaseType_Production = "Production";

			// Token: 0x04000103 RID: 259
			public const string ReleaseType_Test = "Test";

			// Token: 0x04000104 RID: 260
			public static char[] separators = new char[]
			{
				';'
			};

			// Token: 0x04000105 RID: 261
			public static string[] ProductionCoreOptionalFeatures = new string[]
			{
				"PRODUCTION_CORE"
			};

			// Token: 0x04000106 RID: 262
			public static string[] TestCoreOptionalFeatures = new string[]
			{
				"MOBLECORE_TEST",
				"BOOTSEQUENCE_TEST"
			};

			// Token: 0x04000107 RID: 263
			public FeatureManifest.PackageGroups FMGroup;

			// Token: 0x04000108 RID: 264
			public string GroupValue;

			// Token: 0x04000109 RID: 265
			public string Language;

			// Token: 0x0400010A RID: 266
			public string Resolution;

			// Token: 0x0400010B RID: 267
			public string Wow;

			// Token: 0x0400010C RID: 268
			public string PackagePath;

			// Token: 0x0400010D RID: 269
			public string RawBasePath;

			// Token: 0x0400010E RID: 270
			public string ID;

			// Token: 0x0400010F RID: 271
			public bool FeatureIdentifierPackage;

			// Token: 0x04000110 RID: 272
			public string Partition = string.Empty;

			// Token: 0x04000111 RID: 273
			public VersionInfo? Version;

			// Token: 0x04000112 RID: 274
			public string PublicKey;

			// Token: 0x04000113 RID: 275
			public bool BinaryPartition;

			// Token: 0x02000046 RID: 70
			[Flags]
			public enum CorePackageTypeEnum
			{
				// Token: 0x04000157 RID: 343
				NonCore = 0,
				// Token: 0x04000158 RID: 344
				RetailCore = 1,
				// Token: 0x04000159 RID: 345
				NonRetailCore = 2,
				// Token: 0x0400015A RID: 346
				TestCore = 4
			}
		}
	}
}
