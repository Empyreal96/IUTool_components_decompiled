using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using Microsoft.WindowsPhone.FeatureAPI;
using Microsoft.WindowsPhone.ImageUpdate.PkgCommon;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;
using Microsoft.WindowsPhone.Imaging;

namespace Microsoft.WindowsPhone.CompDB
{
	// Token: 0x02000004 RID: 4
	[XmlType(Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
	[XmlRoot(ElementName = "CompDB", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate", IsNullable = false)]
	public class BuildCompDB
	{
		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600000E RID: 14 RVA: 0x000027EC File Offset: 0x000009EC
		// (set) Token: 0x0600000F RID: 15 RVA: 0x00002842 File Offset: 0x00000A42
		[DefaultValue(BuildCompDB.CompDBType.Invalid)]
		[XmlAttribute]
		public BuildCompDB.CompDBType Type
		{
			get
			{
				if (this._type == BuildCompDB.CompDBType.Invalid)
				{
					if (this is UpdateCompDB)
					{
						this._type = BuildCompDB.CompDBType.Update;
					}
					else if (this is BSPCompDB)
					{
						this._type = BuildCompDB.CompDBType.BSP;
					}
					else if (this is DeviceCompDB)
					{
						this._type = BuildCompDB.CompDBType.Device;
					}
					else
					{
						this._type = BuildCompDB.CompDBType.Build;
					}
				}
				return this._type;
			}
			set
			{
				this._type = value;
			}
		}

		// Token: 0x06000010 RID: 16 RVA: 0x0000284B File Offset: 0x00000A4B
		public bool ShouldSerializeMSConditionalFeatures()
		{
			return this.MSConditionalFeatures != null && this.MSConditionalFeatures.Count<FMConditionalFeature>() > 0;
		}

		// Token: 0x06000011 RID: 17 RVA: 0x00002868 File Offset: 0x00000A68
		public BuildCompDB()
		{
			if (this.Features == null)
			{
				this.Features = new List<CompDBFeature>();
			}
			if (this.Packages == null)
			{
				this.Packages = new List<CompDBPackageInfo>();
			}
		}

		// Token: 0x06000012 RID: 18 RVA: 0x000028DC File Offset: 0x00000ADC
		public BuildCompDB(IULogger logger)
		{
			this._iuLogger = logger;
			if (this.Features == null)
			{
				this.Features = new List<CompDBFeature>();
			}
			if (this.Packages == null)
			{
				this.Packages = new List<CompDBPackageInfo>();
			}
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00002958 File Offset: 0x00000B58
		public BuildCompDB(BuildCompDB srcDB)
		{
			this.BuildArch = srcDB.BuildArch;
			this.ReleaseType = srcDB.ReleaseType;
			this.Product = srcDB.Product;
			this.BuildID = srcDB.BuildID;
			this.BuildInfo = srcDB.BuildInfo;
			this.OSVersion = srcDB.OSVersion;
			if (srcDB.Features != null)
			{
				this.Features = (from feat in srcDB.Features
				select new CompDBFeature(feat)).ToList<CompDBFeature>();
			}
			if (srcDB.MSConditionalFeatures != null)
			{
				this.MSConditionalFeatures = (from feat in srcDB.MSConditionalFeatures
				select new FMConditionalFeature(feat)).ToList<FMConditionalFeature>();
			}
			if (this is BSPCompDB && srcDB is BSPCompDB)
			{
				BSPCompDB bspcompDB = srcDB as BSPCompDB;
				if (bspcompDB.OEMConditionalFeatures != null && bspcompDB.OEMConditionalFeatures.Any<FMConditionalFeature>())
				{
					BSPCompDB bspcompDB2 = this as BSPCompDB;
					if (bspcompDB2.OEMConditionalFeatures == null)
					{
						bspcompDB2.OEMConditionalFeatures = new List<FMConditionalFeature>(bspcompDB.OEMConditionalFeatures);
					}
					else
					{
						bspcompDB2.OEMConditionalFeatures.AddRange(bspcompDB.OEMConditionalFeatures);
					}
				}
			}
			if (srcDB.Packages != null)
			{
				this.Packages = (from pkg in srcDB.Packages
				select new CompDBPackageInfo(pkg)).ToList<CompDBPackageInfo>();
			}
			this.Revision = srcDB.Revision;
			this.SchemaVersion = srcDB.SchemaVersion;
			this.Type = srcDB.Type;
		}

		// Token: 0x06000014 RID: 20 RVA: 0x00002B28 File Offset: 0x00000D28
		public static void InitializeChunkMapping(string mappingFile, List<string> languages, IULogger logger)
		{
			BuildCompDB.ChunkMapping = CompDBChunkMapping.ValidateAndLoad(mappingFile, languages, logger);
		}

		// Token: 0x06000015 RID: 21 RVA: 0x00002B38 File Offset: 0x00000D38
		public static string GetProductNamePrefix(string product)
		{
			Edition edition = ImagingEditions.GetProductEdition(product);
			if (edition == null)
			{
				edition = ImagingEditions.Editions.FirstOrDefault((Edition ed) => ed.InternalProductDir.Equals(product, StringComparison.OrdinalIgnoreCase));
				if (edition == null)
				{
					return null;
				}
			}
			string result = product + ".BSP.";
			if (edition.IsProduct("Windows Phone"))
			{
				result = "Mobile.BSP.";
			}
			else if (edition.IsProduct("Windows Holographic"))
			{
				result = "HoloLens.BSP.";
			}
			else if (edition.IsProduct("Windows 10 IoT Core"))
			{
				result = "IOTCore.BSP.";
			}
			else if (edition.IsProduct("OneCore OS"))
			{
				result = "Onecore.BSP.";
			}
			return result;
		}

		// Token: 0x06000016 RID: 22 RVA: 0x00002BE4 File Offset: 0x00000DE4
		public void GenerateCompDB(FMCollection fmCollection, string fmDirectory, string msPackageRoot, string buildType, CpuId buildArch, string buildInfo)
		{
			this.GenerateCompDB(fmCollection, fmDirectory, msPackageRoot, buildType, buildArch, buildInfo, false, false, false, OwnerType.Invalid, ReleaseType.Invalid);
		}

		// Token: 0x06000017 RID: 23 RVA: 0x00002C08 File Offset: 0x00000E08
		public void GenerateCompDB(FMCollection fmCollection, string fmDirectory, string msPackageRoot, string buildType, CpuId buildArch, string buildInfo, bool generateHashes, bool ignoreSkipForPublishing, bool ignoreSkipForPRSSigning, OwnerType filterOnOwnerType, ReleaseType filterOnReleaseType)
		{
			if (fmCollection.Manifest == null)
			{
				throw new ImageCommonException("ImageCommon::BuildCompDB!GenerateCompDB: Unable to generate Build CompDB without a FM Collection.");
			}
			if (filterOnReleaseType == ReleaseType.Production)
			{
				this.ReleaseType = ReleaseType.Production;
			}
			else
			{
				this.ReleaseType = ReleaseType.Test;
			}
			this.Product = fmCollection.Manifest.Product;
			this.BuildArch = buildArch.ToString();
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = false;
			foreach (FMCollectionItem fmcollectionItem in fmCollection.Manifest.FMs)
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
				if ((buildArch == fmcollectionItem.CPUType || fmcollectionItem.CPUType == CpuId.Invalid) && (!ignoreSkipForPublishing || !fmcollectionItem.SkipForPublishing) && (filterOnReleaseType == ReleaseType.Invalid || fmcollectionItem.releaseType == filterOnReleaseType) && (filterOnOwnerType == OwnerType.Invalid || fmcollectionItem.ownerType == filterOnOwnerType))
				{
					FeatureManifest featureManifest = new FeatureManifest();
					string text = Environment.ExpandEnvironmentVariables(fmcollectionItem.Path);
					text = fmcollectionItem.ResolveFMPath(fmDirectory);
					FeatureManifest.ValidateAndLoad(ref featureManifest, text, this._iuLogger);
					if (featureManifest.OwnerType == OwnerType.Microsoft && this.BuildID == Guid.Empty && !string.IsNullOrEmpty(featureManifest.BuildID))
					{
						this.BuildID = new Guid(featureManifest.BuildID);
						this.BuildInfo = featureManifest.BuildInfo;
						this.OSVersion = featureManifest.OSVersion;
					}
					List<FeatureManifest.FMPkgInfo> allPackageByGroups = featureManifest.GetAllPackageByGroups(fmCollection.Manifest.SupportedLanguages, fmCollection.Manifest.SupportedLocales, fmCollection.Manifest.SupportedResolutions, buildType, buildArch.ToString(), msPackageRoot);
					foreach (FeatureManifest.FMPkgInfo fmpkgInfo in from pkg in allPackageByGroups
					where pkg.FMGroup == FeatureManifest.PackageGroups.OEMDEVICEPLATFORM
					select pkg)
					{
						fmpkgInfo.FMGroup = FeatureManifest.PackageGroups.DEVICE;
					}
					foreach (FeatureManifest.FMPkgInfo fmpkgInfo2 in from pkg in allPackageByGroups
					where pkg.FMGroup == FeatureManifest.PackageGroups.DEVICELAYOUT
					select pkg)
					{
						fmpkgInfo2.FMGroup = FeatureManifest.PackageGroups.SOC;
					}
					using (List<string>.Enumerator enumerator3 = (from pkg in allPackageByGroups
					select pkg.FeatureID).Distinct(this.IgnoreCase).ToList<string>().GetEnumerator())
					{
						while (enumerator3.MoveNext())
						{
							string feature = enumerator3.Current;
							List<FeatureManifest.FMPkgInfo> list = (from pkg in allPackageByGroups
							where pkg.FeatureID.Equals(feature, StringComparison.OrdinalIgnoreCase)
							select pkg).ToList<FeatureManifest.FMPkgInfo>();
							CompDBFeature compDBFeature = new CompDBFeature(feature, fmcollectionItem.ID, CompDBFeature.CompDBFeatureTypes.MobileFeature, (fmcollectionItem.ownerType == OwnerType.Microsoft) ? fmcollectionItem.ownerType.ToString() : OwnerType.OEM.ToString());
							foreach (FeatureManifest.FMPkgInfo fmpkgInfo3 in list)
							{
								CompDBFeaturePackage item = new CompDBFeaturePackage(fmpkgInfo3.ID, fmpkgInfo3.FeatureIdentifierPackage);
								compDBFeature.Packages.Add(item);
							}
							if (list.Any((FeatureManifest.FMPkgInfo pkg) => pkg.FMGroup == FeatureManifest.PackageGroups.MSFEATURE || pkg.FMGroup == FeatureManifest.PackageGroups.OEMFEATURE))
							{
								compDBFeature.Type = CompDBFeature.CompDBFeatureTypes.OptionalFeature;
							}
							this.Features.Add(compDBFeature);
						}
					}
					List<CompDBPackageInfo> list2 = new List<CompDBPackageInfo>();
					StringBuilder stringBuilder2 = new StringBuilder();
					bool flag2 = false;
					foreach (FeatureManifest.FMPkgInfo fmpkgInfo4 in allPackageByGroups)
					{
						CompDBPackageInfo compDBPackageInfo;
						try
						{
							compDBPackageInfo = new CompDBPackageInfo(fmpkgInfo4, fmcollectionItem, msPackageRoot, this, generateHashes, fmcollectionItem.UserInstallable);
							if (!fmpkgInfo4.ID.Equals(compDBPackageInfo.ID, StringComparison.OrdinalIgnoreCase))
							{
								compDBPackageInfo.ID = fmpkgInfo4.ID;
							}
						}
						catch (FileNotFoundException)
						{
							flag2 = true;
							stringBuilder2.AppendFormat("Error:\t{0}\n", fmpkgInfo4.ID);
							continue;
						}
						list2.Add(compDBPackageInfo);
					}
					if (flag2)
					{
						flag = true;
						stringBuilder.AppendFormat("\nThe FM File '{0}' following package file(s) could not be found: \n {1}", text, stringBuilder2.ToString());
					}
					if (fmcollectionItem.SkipForPublishing)
					{
						list2 = (from pkg in list2
						select pkg.SetSkipForPublishing()).ToList<CompDBPackageInfo>();
					}
					if (!ignoreSkipForPRSSigning && fmcollectionItem.SkipForPRSSigning)
					{
						list2 = (from pkg in list2
						select pkg.SetSkipForPRSSigning()).ToList<CompDBPackageInfo>();
					}
					this.Packages.AddRange(list2);
					foreach (CompDBFeature compDBFeature2 in this.Features)
					{
						if ((from pkg in compDBFeature2.Packages
						select this.FindPackage(pkg)).Any((CompDBPackageInfo pkg) => pkg.UserInstallable))
						{
							compDBFeature2.Type = CompDBFeature.CompDBFeatureTypes.OnDemandFeature;
						}
					}
					if (featureManifest.Features != null && featureManifest.Features.MSConditionalFeatures != null)
					{
						if (this.MSConditionalFeatures == null)
						{
							this.MSConditionalFeatures = new List<FMConditionalFeature>();
						}
						this.MSConditionalFeatures.AddRange(featureManifest.Features.MSConditionalFeatures);
					}
				}
			}
			if (flag)
			{
				throw new ImageCommonException("ImageCommon::BuildCompDB!GenerateCompDB: Errors processing FM File(s):\n" + stringBuilder.ToString());
			}
			this.Packages = this.Packages.Distinct(CompDBPackageInfoComparer.Standard).ToList<CompDBPackageInfo>();
			if (this.BuildID == Guid.Empty)
			{
				this.BuildID = Guid.NewGuid();
				this.BuildInfo = buildInfo;
			}
		}

		// Token: 0x06000018 RID: 24 RVA: 0x00003310 File Offset: 0x00001510
		public void GenerateHashes(string packageRoot)
		{
			bool flag = false;
			StringBuilder stringBuilder = new StringBuilder();
			foreach (CompDBPackageInfo compDBPackageInfo in this.Packages)
			{
				foreach (CompDBPayloadInfo compDBPayloadInfo in compDBPackageInfo.Payload)
				{
					string text = Path.Combine(packageRoot, compDBPayloadInfo.Path);
					if (!LongPathFile.Exists(text))
					{
						flag = true;
						stringBuilder.AppendLine("Error:\t" + text);
					}
					else
					{
						compDBPackageInfo.SetPackageHash(text);
					}
				}
			}
			if (flag)
			{
				throw new ImageCommonException("ImageCommon::BuildCompDB!GenerateHashes: Missing file(s):\n" + stringBuilder.ToString());
			}
		}

		// Token: 0x06000019 RID: 25 RVA: 0x000033F4 File Offset: 0x000015F4
		public CompDBPackageInfo FindPackage(CompDBFeaturePackage pkg)
		{
			return this.Packages.FirstOrDefault((CompDBPackageInfo searchPkg) => searchPkg.ID.Equals(pkg.ID, StringComparison.OrdinalIgnoreCase));
		}

		// Token: 0x0600001A RID: 26 RVA: 0x00003428 File Offset: 0x00001628
		public static BuildCompDB ValidateAndLoad(string xmlFile, IULogger logger)
		{
			BuildCompDB buildCompDB = new BuildCompDB();
			string text = string.Empty;
			string buildCompDBSchema = BuildPaths.BuildCompDBSchema;
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			foreach (string text2 in executingAssembly.GetManifestResourceNames())
			{
				if (text2.Contains(buildCompDBSchema))
				{
					text = text2;
					break;
				}
			}
			if (string.IsNullOrEmpty(text))
			{
				throw new ImageCommonException("ImageCommon::BuildCompDB!ValidateAndLoad: XSD resource was not found: " + buildCompDBSchema);
			}
			TextReader textReader = new StreamReader(LongPathFile.OpenRead(xmlFile));
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(BuildCompDB));
			try
			{
				buildCompDB = (BuildCompDB)xmlSerializer.Deserialize(textReader);
			}
			catch (Exception innerException)
			{
				throw new ImageCommonException("ImageCommon::BuildCompDB!ValidateAndLoad: Unable to parse Build CompDB XML file '" + xmlFile + "'", innerException);
			}
			finally
			{
				textReader.Close();
			}
			bool flag = "1.2".Equals(buildCompDB.SchemaVersion);
			using (Stream manifestResourceStream = executingAssembly.GetManifestResourceStream(text))
			{
				XsdValidator xsdValidator = new XsdValidator();
				try
				{
					xsdValidator.ValidateXsd(manifestResourceStream, xmlFile, logger);
				}
				catch (XsdValidatorException innerException2)
				{
					if (flag)
					{
						throw new ImageCommonException("ImageCommon::BuildCompDB!ValidateAndLoad: Unable to validate Build XSD for file '" + xmlFile + "'.", innerException2);
					}
					logger.LogWarning("Warning: ImageCommon::BuildCompDB!ValidateAndLoad: Unable to validate Build CompDB XSD for file '" + xmlFile + "'.", new object[0]);
					if (string.IsNullOrEmpty(buildCompDB.SchemaVersion))
					{
						logger.LogWarning("Warning: ImageCommon::BuildCompDB!ValidateAndLoad: Schema Version was not given in Build CompDB. Most up to date Schema Version is {1}.", new object[]
						{
							"1.2"
						});
					}
					else
					{
						logger.LogWarning("Warning: ImageCommon::BuildCompDB!ValidateAndLoad: Schema Version given in Build CompDB ({0}) does not match most up to date Schema Version ({1}).", new object[]
						{
							buildCompDB.SchemaVersion,
							"1.2"
						});
					}
				}
			}
			logger.LogInfo("BuildCompDB: Successfully validated the Build CompDB XML: {0}", new object[]
			{
				xmlFile
			});
			BuildCompDB parentDB = buildCompDB;
			buildCompDB.Packages = (from pkg in buildCompDB.Packages
			select pkg.SetParentDB(parentDB)).ToList<CompDBPackageInfo>();
			if (buildCompDB.ReleaseType == ReleaseType.Invalid)
			{
				if (buildCompDB.Packages.Any((CompDBPackageInfo pkg) => pkg.ReleaseType == ReleaseType.Test))
				{
					buildCompDB.ReleaseType = ReleaseType.Test;
				}
				else
				{
					buildCompDB.ReleaseType = ReleaseType.Production;
				}
			}
			return buildCompDB;
		}

		// Token: 0x0600001B RID: 27 RVA: 0x00003670 File Offset: 0x00001870
		public static BuildCompDB LoadCompDB(string xmlFile, IULogger logger)
		{
			BuildCompDB buildCompDB = new BuildCompDB();
			TextReader textReader = new StreamReader(LongPathFile.OpenRead(xmlFile));
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(BuildCompDB));
			try
			{
				buildCompDB = (BuildCompDB)xmlSerializer.Deserialize(textReader);
			}
			catch (Exception innerException)
			{
				throw new ImageCommonException("ImageCommon::BuildCompDB!LoadCompDB: Unable to parse CompDB XML file '" + xmlFile + "'", innerException);
			}
			finally
			{
				textReader.Close();
			}
			BuildCompDB result = null;
			switch (buildCompDB.Type)
			{
			case BuildCompDB.CompDBType.Build:
			case BuildCompDB.CompDBType.Baseless:
				result = BuildCompDB.ValidateAndLoad(xmlFile, logger);
				break;
			case BuildCompDB.CompDBType.Update:
				result = UpdateCompDB.ValidateAndLoad(xmlFile, logger);
				break;
			case BuildCompDB.CompDBType.Device:
				result = DeviceCompDB.ValidateAndLoad(xmlFile, logger);
				break;
			case BuildCompDB.CompDBType.BSP:
				result = BSPCompDB.ValidateAndLoad(xmlFile, logger);
				break;
			}
			return result;
		}

		// Token: 0x0600001C RID: 28 RVA: 0x0000373C File Offset: 0x0000193C
		public virtual void WriteToFile(string xmlFile)
		{
			string directoryName = Path.GetDirectoryName(xmlFile);
			if (!Directory.Exists(directoryName))
			{
				Directory.CreateDirectory(directoryName);
			}
			this.SchemaVersion = "1.2";
			this.Revision = "1";
			TextWriter textWriter = new StreamWriter(LongPathFile.OpenWrite(xmlFile));
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(BuildCompDB));
			try
			{
				xmlSerializer.Serialize(textWriter, this);
			}
			catch (Exception innerException)
			{
				throw new ImageCommonException("ImageCommon::BuildCompDB!WriteToFile: Unable to write Build CompDB XML file '" + xmlFile + "'", innerException);
			}
			finally
			{
				textWriter.Close();
			}
		}

		// Token: 0x0600001D RID: 29 RVA: 0x000037D8 File Offset: 0x000019D8
		public virtual void WriteToFile(string xmlFile, bool writePublishingFile)
		{
			this.WriteToFile(xmlFile);
			if (writePublishingFile)
			{
				CompDBPublishingInfo compDBPublishingInfo = new CompDBPublishingInfo(this, this._iuLogger);
				string path = Path.GetFileNameWithoutExtension(xmlFile) + CompDBPublishingInfo.c_CompDBPublishingInfoFileIdentifier + ".xml";
				string xmlFile2 = Path.Combine(Path.GetDirectoryName(xmlFile), path);
				compDBPublishingInfo.WriteToFile(xmlFile2);
			}
		}

		// Token: 0x0600001E RID: 30 RVA: 0x00003824 File Offset: 0x00001A24
		public static string GetBuildInfo()
		{
			return Environment.ExpandEnvironmentVariables("%_RELEASELABEL%.%_PARENTBRANCHBUILDNUMBER%.%_QFELEVEL%.%_BUILDTIME%");
		}

		// Token: 0x0600001F RID: 31 RVA: 0x00003830 File Offset: 0x00001A30
		public void CopyHashes(BuildCompDB srcCompDB)
		{
			using (List<CompDBPackageInfo>.Enumerator enumerator = this.Packages.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					CompDBPackageInfo pkg = enumerator.Current;
					CompDBPackageInfo compDBPackageInfo = srcCompDB.Packages.FirstOrDefault((CompDBPackageInfo src) => src.Equals(pkg, CompDBPackageInfo.CompDBPackageInfoComparison.IgnorePayloadHashes));
					if (compDBPackageInfo != null)
					{
						foreach (CompDBPayloadInfo compDBPayloadInfo in pkg.Payload)
						{
							CompDBPayloadInfo compDBPayloadInfo2 = compDBPackageInfo.FindPayload(compDBPayloadInfo.Path);
							if (compDBPayloadInfo2 != null && string.IsNullOrEmpty(compDBPayloadInfo2.PayloadHash))
							{
								compDBPayloadInfo.PayloadHash = compDBPayloadInfo2.PayloadHash;
							}
						}
					}
				}
			}
		}

		// Token: 0x06000020 RID: 32 RVA: 0x00003918 File Offset: 0x00001B18
		public void GenerateMissingHashes(string packageRoot)
		{
			foreach (CompDBPackageInfo compDBPackageInfo in this.Packages)
			{
				foreach (CompDBPayloadInfo compDBPayloadInfo in compDBPackageInfo.Payload)
				{
					if (string.IsNullOrEmpty(compDBPayloadInfo.PayloadHash))
					{
						string payloadHash = Path.Combine(packageRoot, compDBPayloadInfo.Path);
						compDBPayloadInfo.SetPayloadHash(payloadHash);
					}
				}
			}
		}

		// Token: 0x06000021 RID: 33 RVA: 0x000039C0 File Offset: 0x00001BC0
		public void FilterDB(OwnerType filterOnOwnerType, ReleaseType filterOnReleaseType)
		{
			this.FilterDB(filterOnOwnerType, filterOnReleaseType, false);
		}

		// Token: 0x06000022 RID: 34 RVA: 0x000039CC File Offset: 0x00001BCC
		public void FilterDB(OwnerType filterOnOwnerType, ReleaseType filterOnReleaseType, bool filterOutSkipForPublishing)
		{
			if (filterOutSkipForPublishing)
			{
				this.Packages = (from pkg in this.Packages
				where !pkg.SkipForPublishing
				select pkg).ToList<CompDBPackageInfo>();
			}
			if (filterOnOwnerType != OwnerType.Invalid)
			{
				this.Packages = (from pkg in this.Packages
				where pkg.OwnerType == filterOnOwnerType
				select pkg).ToList<CompDBPackageInfo>();
				if (this.Features != null)
				{
					this.Features = (from feat in this.Features
					where feat.Group.Equals(filterOnOwnerType.ToString(), StringComparison.OrdinalIgnoreCase)
					select feat).ToList<CompDBFeature>();
				}
			}
			if (filterOnReleaseType != ReleaseType.Invalid)
			{
				this.Packages = (from pkg in this.Packages
				where pkg.ReleaseType == filterOnReleaseType
				select pkg).ToList<CompDBPackageInfo>();
				if (filterOnReleaseType == ReleaseType.Production)
				{
					this.ReleaseType = ReleaseType.Production;
				}
				else
				{
					this.ReleaseType = ReleaseType.Test;
				}
			}
			if (this.Features != null)
			{
				BuildCompDB.<>c__DisplayClass46_1 CS$<>8__locals2 = new BuildCompDB.<>c__DisplayClass46_1();
				CS$<>8__locals2.filteredPackageIDs = (from pkg in this.Packages
				select pkg.ID).ToList<string>();
				List<CompDBFeature> list = new List<CompDBFeature>();
				if (this.Type == BuildCompDB.CompDBType.Update)
				{
					using (List<CompDBFeature>.Enumerator enumerator = this.Features.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							CompDBFeature compDBFeature = enumerator.Current;
							IEnumerable<CompDBFeaturePackage> packages = compDBFeature.Packages;
							Func<CompDBFeaturePackage, bool> predicate;
							if ((predicate = CS$<>8__locals2.<>9__5) == null)
							{
								BuildCompDB.<>c__DisplayClass46_1 CS$<>8__locals3 = CS$<>8__locals2;
								predicate = (CS$<>8__locals3.<>9__5 = ((CompDBFeaturePackage pkg) => CS$<>8__locals3.filteredPackageIDs.Contains(pkg.ID, StringComparer.OrdinalIgnoreCase)));
							}
							if (packages.Any(predicate))
							{
								list.Add(compDBFeature);
							}
							IEnumerable<CompDBFeaturePackage> packages2 = compDBFeature.Packages;
							Func<CompDBFeaturePackage, bool> predicate2;
							if ((predicate2 = CS$<>8__locals2.<>9__6) == null)
							{
								BuildCompDB.<>c__DisplayClass46_1 CS$<>8__locals4 = CS$<>8__locals2;
								predicate2 = (CS$<>8__locals4.<>9__6 = ((CompDBFeaturePackage pkg) => !CS$<>8__locals4.filteredPackageIDs.Contains(pkg.ID, StringComparer.OrdinalIgnoreCase) && pkg.UpdateType != CompDBFeaturePackage.UpdateTypes.Canonical));
							}
							foreach (CompDBFeaturePackage compDBFeaturePackage in packages2.Where(predicate2).ToList<CompDBFeaturePackage>())
							{
								compDBFeaturePackage.UpdateType = CompDBFeaturePackage.UpdateTypes.Canonical;
							}
						}
						goto IL_270;
					}
				}
				foreach (CompDBFeature compDBFeature2 in this.Features)
				{
					IEnumerable<CompDBFeaturePackage> packages3 = compDBFeature2.Packages;
					Func<CompDBFeaturePackage, bool> predicate3;
					if ((predicate3 = CS$<>8__locals2.<>9__7) == null)
					{
						BuildCompDB.<>c__DisplayClass46_1 CS$<>8__locals5 = CS$<>8__locals2;
						predicate3 = (CS$<>8__locals5.<>9__7 = ((CompDBFeaturePackage pkg) => CS$<>8__locals5.filteredPackageIDs.Contains(pkg.ID, StringComparer.OrdinalIgnoreCase)));
					}
					if (packages3.All(predicate3))
					{
						list.Add(compDBFeature2);
					}
				}
				IL_270:
				this.Features = list;
				CS$<>8__locals2.featurePkgIDs = (from pkg in this.Features.SelectMany((CompDBFeature feat) => feat.Packages)
				select pkg.ID).ToList<string>();
				this.Packages = (from pkg in this.Packages
				where CS$<>8__locals2.featurePkgIDs.Contains(pkg.ID, StringComparer.OrdinalIgnoreCase)
				select pkg).ToList<CompDBPackageInfo>();
			}
		}

		// Token: 0x06000023 RID: 35 RVA: 0x00003CF4 File Offset: 0x00001EF4
		public void Merge(BuildCompDB srcDB)
		{
			if (this.Type != srcDB.Type)
			{
				throw new ImageCommonException("ImageCommon::BuildCompDB!Merge: Unable to generate the CompDBs because they are different Types " + this.Type.ToString() + "\\" + srcDB.Type.ToString());
			}
			using (List<CompDBFeature>.Enumerator enumerator = srcDB.Features.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					CompDBFeature srcFeature = enumerator.Current;
					CompDBFeature compDBFeature = this.Features.FirstOrDefault((CompDBFeature feat) => feat.FeatureIDWithFMID.Equals(srcFeature.FeatureIDWithFMID, StringComparison.OrdinalIgnoreCase));
					if (compDBFeature == null)
					{
						this.Features.Add(srcFeature);
					}
					else
					{
						using (List<CompDBFeaturePackage>.Enumerator enumerator2 = srcFeature.Packages.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								CompDBFeaturePackage srcFeaturePkg = enumerator2.Current;
								if (compDBFeature.Packages.Any((CompDBFeaturePackage pkg) => !pkg.ID.Equals(srcFeaturePkg.ID, StringComparison.OrdinalIgnoreCase)))
								{
									compDBFeature.Packages.Add(srcFeaturePkg);
								}
							}
						}
					}
				}
			}
			List<string> pkgIDs = (from pkg in this.Packages
			select pkg.ID).ToList<string>();
			List<CompDBPackageInfo> collection = (from pkg in srcDB.Packages
			where !pkgIDs.Contains(pkg.ID, StringComparer.OrdinalIgnoreCase)
			select pkg).ToList<CompDBPackageInfo>();
			this.Packages.AddRange(collection);
		}

		// Token: 0x06000024 RID: 36 RVA: 0x00003EB4 File Offset: 0x000020B4
		public override string ToString()
		{
			return "Build DB: " + this.BuildInfo;
		}

		// Token: 0x04000008 RID: 8
		public const string c_BuildCompDBRevision = "1";

		// Token: 0x04000009 RID: 9
		public const string c_BuildCompDBSchemaVersion = "1.2";

		// Token: 0x0400000A RID: 10
		public const string c_BuildInfoEnvStr = "%_RELEASELABEL%.%_PARENTBRANCHBUILDNUMBER%.%_QFELEVEL%.%_BUILDTIME%";

		// Token: 0x0400000B RID: 11
		public const string c_PhoneProductNamePrefix = "Mobile.BSP.";

		// Token: 0x0400000C RID: 12
		public const string c_AnalogProductNamePrefix = "HoloLens.BSP.";

		// Token: 0x0400000D RID: 13
		public const string c_IotProductNamePrefix = "IOTCore.BSP.";

		// Token: 0x0400000E RID: 14
		public const string c_OnecoreProductNamePrefix = "Onecore.BSP.";

		// Token: 0x0400000F RID: 15
		public const string c_OtherProductNamePrefix = ".BSP.";

		// Token: 0x04000010 RID: 16
		internal IULogger _iuLogger;

		// Token: 0x04000011 RID: 17
		[XmlAttribute]
		public DateTime CreatedDate = DateTime.Now.ToUniversalTime();

		// Token: 0x04000012 RID: 18
		[XmlAttribute]
		public string Revision = "1";

		// Token: 0x04000013 RID: 19
		[XmlAttribute]
		public string SchemaVersion = "1.2";

		// Token: 0x04000014 RID: 20
		private BuildCompDB.CompDBType _type = BuildCompDB.CompDBType.Invalid;

		// Token: 0x04000015 RID: 21
		[XmlAttribute]
		public string Product;

		// Token: 0x04000016 RID: 22
		[XmlAttribute]
		public Guid BuildID;

		// Token: 0x04000017 RID: 23
		[XmlAttribute]
		public string BuildInfo;

		// Token: 0x04000018 RID: 24
		[XmlAttribute]
		public string OSVersion;

		// Token: 0x04000019 RID: 25
		[XmlAttribute]
		public string BuildArch;

		// Token: 0x0400001A RID: 26
		[XmlAttribute]
		[DefaultValue(ReleaseType.Invalid)]
		public ReleaseType ReleaseType;

		// Token: 0x0400001B RID: 27
		[XmlArrayItem(ElementName = "Feature", Type = typeof(CompDBFeature), IsNullable = false)]
		[XmlArray]
		public List<CompDBFeature> Features;

		// Token: 0x0400001C RID: 28
		[XmlArrayItem(ElementName = "ConditionalFeature", Type = typeof(FMConditionalFeature), IsNullable = false)]
		[XmlArray]
		public List<FMConditionalFeature> MSConditionalFeatures;

		// Token: 0x0400001D RID: 29
		[XmlArrayItem(ElementName = "Package", Type = typeof(CompDBPackageInfo), IsNullable = false)]
		[XmlArray]
		public List<CompDBPackageInfo> Packages;

		// Token: 0x0400001E RID: 30
		[XmlIgnore]
		internal StringComparer IgnoreCase = StringComparer.OrdinalIgnoreCase;

		// Token: 0x0400001F RID: 31
		public static CompDBChunkMapping ChunkMapping;

		// Token: 0x02000039 RID: 57
		public enum CompDBType
		{
			// Token: 0x0400018D RID: 397
			Invalid = -1,
			// Token: 0x0400018E RID: 398
			Build,
			// Token: 0x0400018F RID: 399
			Update,
			// Token: 0x04000190 RID: 400
			Device,
			// Token: 0x04000191 RID: 401
			BSP,
			// Token: 0x04000192 RID: 402
			Baseless
		}
	}
}
