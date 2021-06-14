using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.WindowsPhone.ImageUpdate.Customization.XML;
using Microsoft.WindowsPhone.ImageUpdate.PkgCommon;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon;
using Microsoft.WindowsPhone.ImageUpdate.Tools;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;
using Microsoft.WindowsPhone.MCSF.Offline;
using Microsoft.WindowsPhone.Multivariant.Offline;

namespace Microsoft.WindowsPhone.ImageUpdate.Customization
{
	// Token: 0x02000005 RID: 5
	public static class CustomContentGenerator
	{
		// Token: 0x06000023 RID: 35 RVA: 0x0000226C File Offset: 0x0000046C
		public static CustomContent GenerateCustomContent(Customizations customizationInput)
		{
			bool flag = !string.IsNullOrWhiteSpace(customizationInput.CustomizationXMLFilePath);
			bool flag2 = !string.IsNullOrWhiteSpace(customizationInput.CustomizationPPKGFilePath);
			if (!flag && !flag2)
			{
				throw new ArgumentException("CustomizationXMLFilePath or CustomizationPPKGFilePath must be set");
			}
			if (flag)
			{
				if (!File.Exists(customizationInput.CustomizationXMLFilePath))
				{
					throw new ArgumentException("CustomizationXMLFilePath points to a file that does not exist");
				}
				if (!string.Equals(Path.GetExtension(customizationInput.CustomizationXMLFilePath), ".xml", StringComparison.OrdinalIgnoreCase))
				{
					throw new ArgumentException("CustomizationXMLFilePath points to a file that does not have the correct extension (.xml)");
				}
			}
			if (flag2)
			{
				if (!File.Exists(customizationInput.CustomizationPPKGFilePath))
				{
					throw new ArgumentException("CustomizationPPKGFilePath points to a file that does not exist");
				}
				if (!string.Equals(Path.GetExtension(customizationInput.CustomizationPPKGFilePath), ".ppkg", StringComparison.OrdinalIgnoreCase))
				{
					throw new ArgumentException("CustomizationPPKGFilePath points to a file that does not have the correct extension (.ppkg)");
				}
			}
			if (string.IsNullOrWhiteSpace(customizationInput.OutputDirectory))
			{
				throw new ArgumentException("OutputDirectory must be set");
			}
			if (!Directory.Exists(customizationInput.OutputDirectory))
			{
				throw new ArgumentException("OutputDirectory points to a location that does not exist");
			}
			CustomContent customContent = new CustomContent();
			List<CustomizationError> list = new List<CustomizationError>();
			PolicyStore policyStore = new PolicyStore();
			try
			{
				policyStore.LoadPolicyFromPackages(customizationInput.ImagePackages);
			}
			catch (MCSFOfflineException ex)
			{
				list.Add(new CustomizationError(CustomizationErrorSeverity.Error, null, ex.Message));
				customContent.CustomizationErrors = list;
				return customContent;
			}
			customContent.CustomizationErrors = new List<CustomizationError>();
			List<string> list2 = new List<string>();
			if (flag2)
			{
				list2.AddRange(CustomContentGenerator.ProcessCustomizationPPKG(customizationInput, policyStore, ref customContent));
				if (customContent.CustomizationErrors.Any((CustomizationError x) => x.Severity == CustomizationErrorSeverity.Error))
				{
					return customContent;
				}
			}
			if (flag)
			{
				List<CustomizationError> second;
				ImageCustomizations customizations = CustomContentGenerator.LoadCustomizations(customizationInput.CustomizationXMLFilePath, policyStore, out second);
				customContent.CustomizationErrors = customContent.CustomizationErrors.Concat(second);
				if (customContent.CustomizationErrors.Any((CustomizationError x) => x.Severity == CustomizationErrorSeverity.Error))
				{
					return customContent;
				}
				try
				{
					list2.AddRange(CustomContentGenerator.GeneratePackages(customizationInput, customizations, policyStore));
					customContent.DataContent = CustomContentGenerator.GenerateDataPartitionContent(customizations);
				}
				catch (CustomizationException ex2)
				{
					CustomizationError source = new CustomizationError(CustomizationErrorSeverity.Error, null, ex2.ToString());
					customContent.CustomizationErrors = customContent.CustomizationErrors.Concat(source.ToEnumerable<CustomizationError>());
				}
			}
			customContent.PackagePaths = list2;
			return customContent;
		}

		// Token: 0x06000024 RID: 36 RVA: 0x000024BC File Offset: 0x000006BC
		private static ImageCustomizations LoadCustomizations(string customizationFilePath, PolicyStore policyStore, out List<CustomizationError> loadErrors)
		{
			loadErrors = new List<CustomizationError>();
			ImageCustomizations imageCustomizations = ImageCustomizations.LoadFromPath(customizationFilePath);
			IEnumerable<CustomizationError> enumerable;
			imageCustomizations = imageCustomizations.GetMergedCustomizations(out enumerable);
			loadErrors.AddRange(enumerable);
			if ((from x in enumerable
			where x.Severity == CustomizationErrorSeverity.Error
			select x).Count<CustomizationError>() > 0)
			{
				return null;
			}
			enumerable = CustomContentGenerator.VerifyCustomizations(imageCustomizations, policyStore);
			loadErrors.AddRange(enumerable);
			if ((from x in enumerable
			where x.Severity == CustomizationErrorSeverity.Error
			select x).Count<CustomizationError>() > 0)
			{
				return null;
			}
			return imageCustomizations;
		}

		// Token: 0x06000025 RID: 37 RVA: 0x00002558 File Offset: 0x00000758
		private static IEnumerable<string> ProcessCustomizationPPKG(Customizations customizationInput, PolicyStore policyStore, ref CustomContent response)
		{
			IEnumerable<string> result = null;
			try
			{
				string customizationPPKGFilePath = customizationInput.CustomizationPPKGFilePath;
				if ((from x in response.CustomizationErrors
				where x.Severity == CustomizationErrorSeverity.Error
				select x).Count<CustomizationError>() > 0)
				{
					return result;
				}
				response.DataContent = new List<KeyValuePair<string, string>>();
				if ((from x in response.CustomizationErrors
				where x.Severity == CustomizationErrorSeverity.Error
				select x).Count<CustomizationError>() > 0)
				{
					return result;
				}
				string owner = "OEM";
				OwnerType ownerType = OwnerType.OEM;
				result = CustomContentGenerator.GeneratePPKGPackage(customizationPPKGFilePath, customizationInput, owner, ownerType, ref response);
				if ((from x in response.CustomizationErrors
				where x.Severity == CustomizationErrorSeverity.Error
				select x).Count<CustomizationError>() > 0)
				{
					return result;
				}
			}
			catch (CustomizationException ex)
			{
				CustomizationError source = new CustomizationError(CustomizationErrorSeverity.Error, null, ex.ToString());
				response.CustomizationErrors = response.CustomizationErrors.Concat(source.ToEnumerable<CustomizationError>());
			}
			return result;
		}

		// Token: 0x06000026 RID: 38 RVA: 0x0000267C File Offset: 0x0000087C
		private static IEnumerable<string> GeneratePPKGPackage(string outputPPKGFile, Customizations config, string owner, OwnerType ownerType, ref CustomContent response)
		{
			List<string> list = new List<string>();
			string destination = Path.Combine(CustomContentGenerator.ProvPKGDevicePath, Path.GetFileName(config.CustomizationPPKGFilePath));
			list.Add(new CustomizationPackage(PkgConstants.c_strMainOsPartition)
			{
				Component = config.ImageDeviceName,
				SubComponent = "PPKG",
				Owner = owner,
				OwnerType = ownerType,
				CpuType = config.ImageCpuType,
				BuildType = config.ImageBuildType,
				Version = config.ImageVersion,
				Files = 
				{
					new CustomizationFile(outputPPKGFile, destination)
				}
			}.SavePackage(config.OutputDirectory));
			return list;
		}

		// Token: 0x06000027 RID: 39 RVA: 0x0000271C File Offset: 0x0000091C
		public static IEnumerable<CustomizationError> VerifyCustomizations(ImageCustomizations customizations, IEnumerable<IPkgInfo> packages)
		{
			PolicyStore policyStore = new PolicyStore();
			try
			{
				policyStore.LoadPolicyFromPackages(packages);
			}
			catch (MCSFOfflineException ex)
			{
				return new List<CustomizationError>
				{
					new CustomizationError(CustomizationErrorSeverity.Error, null, ex.Message)
				};
			}
			return CustomContentGenerator.VerifyCustomizations(customizations, policyStore);
		}

		// Token: 0x06000028 RID: 40 RVA: 0x00002770 File Offset: 0x00000970
		public static IEnumerable<CustomizationError> VerifyCustomizations(ImageCustomizations customizations, PolicyStore policyStore)
		{
			List<CustomizationError> list = new List<CustomizationError>();
			IEnumerable<CustomizationError> collection = CustomContentGenerator.VerifyTargets(customizations.Targets, customizations);
			list.AddRange(collection);
			if (customizations.StaticVariant != null)
			{
				collection = CustomContentGenerator.VerifyApplicationsGroup(customizations.StaticVariant.ApplicationGroups, customizations.StaticVariant);
				list.AddRange(collection);
				collection = CustomContentGenerator.VerifySettingGroups(customizations.StaticVariant.SettingGroups, customizations.StaticVariant, customizations, policyStore);
				list.AddRange(collection);
				collection = CustomContentGenerator.VerifyDataAssetGroups(customizations.StaticVariant.DataAssetGroups, customizations.StaticVariant);
				list.AddRange(collection);
			}
			foreach (Variant variant in customizations.Variants)
			{
				collection = CustomContentGenerator.VerifyTargetRefs(variant.TargetRefs, customizations.Targets, variant);
				list.AddRange(collection);
				collection = CustomContentGenerator.VerifyApplicationsGroup(variant.ApplicationGroups, variant);
				list.AddRange(collection);
				collection = CustomContentGenerator.VerifySettingGroups(variant.SettingGroups, variant, customizations, policyStore);
				list.AddRange(collection);
			}
			return list;
		}

		// Token: 0x06000029 RID: 41 RVA: 0x00002880 File Offset: 0x00000A80
		public static IEnumerable<CustomizationError> VerifyTargets(List<Target> targets, ImageCustomizations parent)
		{
			return CustomContentGenerator.VerifyTargets(targets, parent, true);
		}

		// Token: 0x0600002A RID: 42 RVA: 0x0000288C File Offset: 0x00000A8C
		public static IEnumerable<CustomizationError> VerifyTargets(List<Target> targets, ImageCustomizations parent, bool verifyChildren)
		{
			List<CustomizationError> list = new List<CustomizationError>();
			if (verifyChildren)
			{
				foreach (Target target in targets)
				{
					list.AddRange(CustomContentGenerator.VerifyTarget(target, parent));
				}
			}
			return list;
		}

		// Token: 0x0600002B RID: 43 RVA: 0x000028EC File Offset: 0x00000AEC
		public static IEnumerable<CustomizationError> VerifyTarget(Target target, ImageCustomizations customizations)
		{
			return CustomContentGenerator.VerifyTarget(target, customizations, true);
		}

		// Token: 0x0600002C RID: 44 RVA: 0x000028F8 File Offset: 0x00000AF8
		public static IEnumerable<CustomizationError> VerifyTarget(Target target, ImageCustomizations customizations, bool verifyChildren)
		{
			List<CustomizationError> list = new List<CustomizationError>();
			list.AddRange(CustomContentGenerator.VerifyTargetId(target));
			list.AddRange(CustomContentGenerator.VerifyTargetList(target));
			if (verifyChildren)
			{
				foreach (TargetState conditions in target.TargetStates)
				{
					list.AddRange(CustomContentGenerator.VerifyConditions(conditions, target, customizations));
				}
			}
			return list;
		}

		// Token: 0x0600002D RID: 45 RVA: 0x00002974 File Offset: 0x00000B74
		public static IEnumerable<CustomizationError> VerifyImportSource(Import import)
		{
			List<CustomizationError> list = new List<CustomizationError>();
			list.AddRange(CustomContentGenerator.VerifyExpandedPath(import.Source, import.ToEnumerable<Import>(), "Import source"));
			return list;
		}

		// Token: 0x0600002E RID: 46 RVA: 0x00002998 File Offset: 0x00000B98
		private static IEnumerable<string> GeneratePackages(Customizations config, ImageCustomizations customizations, PolicyStore policyStore)
		{
			List<string> list = new List<string>();
			string tempDirectory = FileUtils.GetTempDirectory();
			try
			{
				MVDatastore mvdatastore = new MVDatastore();
				MVDatastore mvdatastore2 = new MVDatastore();
				if (customizations.StaticVariant != null)
				{
					MVVariant item = CustomContentGenerator.GenerateGroupForVariant(customizations.StaticVariant, customizations, policyStore);
					mvdatastore.Variants.Add(item);
				}
				foreach (Variant variant in ((IEnumerable<Variant>)customizations.Variants))
				{
					MVVariant item2 = CustomContentGenerator.GenerateGroupForVariant(variant, customizations, policyStore);
					mvdatastore2.Variants.Add(item2);
				}
				string text = Path.Combine(tempDirectory, "StaticDatastore");
				mvdatastore.SaveStaticDatastore(text);
				string text2 = Path.Combine(tempDirectory, "MVDatastore");
				string text3 = Path.Combine(tempDirectory, "Provisioning");
				string text4 = Path.Combine(tempDirectory, "CriticalSettings");
				mvdatastore2.SaveDatastore(text2, text3, text4);
				string shadowRegRoot = Path.Combine(tempDirectory, "MVShadowing");
				bool provisionCab = Directory.EnumerateFiles(text3).Any<string>();
				bool criticalCab = Directory.EnumerateFiles(text4).Any<string>();
				IEnumerable<RegValueInfo> defaultDatastoreRegistration = MVDatastore.GetDefaultDatastoreRegistration(provisionCab, criticalCab);
				HashSet<RegFilePartitionInfo> hashSet = new HashSet<RegFilePartitionInfo>(mvdatastore.SaveShadowRegistry(shadowRegRoot, defaultDatastoreRegistration));
				HashSet<RegFilePartitionInfo> hashSet2 = new HashSet<RegFilePartitionInfo>(from r in hashSet
				where r.partition.Equals(PkgConstants.c_strMainOsPartition, StringComparison.OrdinalIgnoreCase)
				select r);
				string text5 = CustomContentGenerator.GenerateMainOSPackage(config, customizations, policyStore, hashSet2, text, text2, text3, text4);
				if (text5 != null)
				{
					list.Add(text5);
				}
				text5 = CustomContentGenerator.GenerateVariantAppsPackage(config, customizations, policyStore, PkgConstants.c_strDataPartition);
				if (text5 != null)
				{
					list.Add(text5);
				}
				text5 = CustomContentGenerator.GenerateVariantAppsPackage(config, customizations, policyStore, PkgConstants.c_strMainOsPartition);
				if (text5 != null)
				{
					list.Add(text5);
				}
				foreach (RegFilePartitionInfo regFilePartitionInfo in hashSet.Except(hashSet2))
				{
					CustomizationPackage customizationPackage = new CustomizationPackage(regFilePartitionInfo.partition);
					customizationPackage.Component = config.ImageDeviceName;
					customizationPackage.Owner = customizations.Owner;
					customizationPackage.OwnerType = customizations.OwnerType;
					customizationPackage.CpuType = config.ImageCpuType;
					customizationPackage.BuildType = config.ImageBuildType;
					customizationPackage.Version = config.ImageVersion;
					customizationPackage.AddFile(FileType.Registry, regFilePartitionInfo.regFilename, CustomizationPackage.ShadowRegFilePath);
					string item3 = customizationPackage.SavePackage(config.OutputDirectory);
					list.Add(item3);
				}
				text5 = CustomContentGenerator.GenerateStaticAppsPackage(config, customizations, policyStore, PkgConstants.c_strDataPartition);
				if (text5 != null)
				{
					list.Add(text5);
				}
				text5 = CustomContentGenerator.GenerateStaticAppsPackage(config, customizations, policyStore, PkgConstants.c_strMainOsPartition);
				if (text5 != null)
				{
					list.Add(text5);
				}
			}
			finally
			{
				FileUtils.DeleteTree(tempDirectory);
			}
			return list;
		}

		// Token: 0x0600002F RID: 47 RVA: 0x00002C6C File Offset: 0x00000E6C
		private static string GenerateMainOSPackage(Customizations config, ImageCustomizations customizations, PolicyStore policyStore, IEnumerable<RegFilePartitionInfo> registryFiles, string staticDatastoreOutputRoot, string datastoreOutputRoot, string provisioningOutputRoot, string criticalSettingsOutputRoot)
		{
			CustomizationPackage customizationPackage = new CustomizationPackage(PkgConstants.c_strMainOsPartition);
			customizationPackage.Component = config.ImageDeviceName;
			customizationPackage.Owner = customizations.Owner;
			customizationPackage.OwnerType = customizations.OwnerType;
			customizationPackage.CpuType = config.ImageCpuType;
			customizationPackage.BuildType = config.ImageBuildType;
			customizationPackage.Version = config.ImageVersion;
			customizationPackage.Files.AddRange(CustomContentGenerator.GenerateAssetFileList(customizations, policyStore, ""));
			IEnumerable<string> enumerable = Directory.EnumerateFiles(staticDatastoreOutputRoot);
			foreach (string text in enumerable)
			{
				string text2 = FileUtils.RerootPath(text, staticDatastoreOutputRoot, "\\Programs\\PhoneProvisioner_OEM\\OEM\\");
				customizationPackage.AddFile(text, text2);
				string text3 = Path.GetFileName(text).Substring("static_settings_group".Length);
				text3 = "mxipupdate" + text3;
				text2 = FileUtils.RerootPath(text, staticDatastoreOutputRoot, "\\Windows\\System32\\Migrators\\DuMigrationProvisionerOEM\\provxml");
				text2 = Path.Combine(Path.GetDirectoryName(text2), text3);
				customizationPackage.AddFile(text, text2);
			}
			foreach (string text4 in from x in Directory.EnumerateFiles(datastoreOutputRoot).Concat(Directory.EnumerateFiles(provisioningOutputRoot)).Concat(Directory.EnumerateFiles(criticalSettingsOutputRoot))
			where Path.GetExtension(x).Equals(".xml")
			select x)
			{
				string text5 = FileUtils.RerootPath(text4, datastoreOutputRoot, "\\Programs\\CommonFiles\\ADC\\Microsoft\\");
				text5 = FileUtils.RerootPath(text5, provisioningOutputRoot, "\\Programs\\CommonFiles\\ADC\\Microsoft\\");
				text5 = FileUtils.RerootPath(text5, criticalSettingsOutputRoot, "\\Programs\\CommonFiles\\ADC\\Microsoft\\");
				customizationPackage.AddFile(text4, text5);
			}
			enumerable = Directory.EnumerateFiles(datastoreOutputRoot);
			enumerable = enumerable.Concat(Directory.EnumerateFiles(provisioningOutputRoot));
			enumerable = from x in enumerable
			where Path.GetExtension(x).Equals(".provxml")
			select x;
			if (enumerable.Count<string>() > 0)
			{
				string text6 = Path.Combine(datastoreOutputRoot, "ProvisionData.cab");
				CabApiWrapper.CreateCabSelected(text6, enumerable.ToArray<string>(), provisioningOutputRoot, provisioningOutputRoot, CompressionType.LZX);
				string text7 = FileUtils.RerootPath(text6, datastoreOutputRoot, "\\Programs\\CommonFiles\\ADC\\Microsoft\\");
				text7 = FileUtils.RerootPath(text7, provisioningOutputRoot, "\\Programs\\CommonFiles\\ADC\\Microsoft\\");
				customizationPackage.AddFile(text6, text7);
			}
			enumerable = Directory.EnumerateFiles(criticalSettingsOutputRoot);
			enumerable = from x in enumerable
			where Path.GetExtension(x).Equals(".provxml")
			select x;
			if (enumerable.Count<string>() > 0)
			{
				string text8 = Path.Combine(criticalSettingsOutputRoot, "ProvisionDataCriticalSettings.cab");
				CabApiWrapper.CreateCabSelected(text8, enumerable.ToArray<string>(), criticalSettingsOutputRoot, criticalSettingsOutputRoot, CompressionType.LZX);
				string destinationPath = FileUtils.RerootPath(text8, criticalSettingsOutputRoot, "\\Programs\\CommonFiles\\ADC\\Microsoft\\");
				customizationPackage.AddFile(text8, destinationPath);
			}
			customizationPackage.Files.AddRange(CustomContentGenerator.GenerateStaticAppLicenseList(customizations));
			customizationPackage.Files.AddRange(CustomContentGenerator.GenerateStaticAppProvXMLList(customizations));
			foreach (RegFilePartitionInfo regFilePartitionInfo in registryFiles)
			{
				customizationPackage.AddFile(FileType.Registry, regFilePartitionInfo.regFilename, CustomizationPackage.ShadowRegFilePath);
			}
			return customizationPackage.SavePackage(config.OutputDirectory);
		}

		// Token: 0x06000030 RID: 48 RVA: 0x00002FA8 File Offset: 0x000011A8
		private static string GenerateVariantAppsPackage(Customizations config, ImageCustomizations customizations, PolicyStore policyStore, string partition)
		{
			CustomizationPackage customizationPackage = new CustomizationPackage(partition);
			customizationPackage.Component = config.ImageDeviceName;
			customizationPackage.SubComponent += ".VariantApps";
			customizationPackage.Owner = customizations.Owner;
			customizationPackage.OwnerType = customizations.OwnerType;
			customizationPackage.CpuType = config.ImageCpuType;
			customizationPackage.BuildType = config.ImageBuildType;
			customizationPackage.Version = config.ImageVersion;
			if (partition.Equals(PkgConstants.c_strMainOsPartition))
			{
				customizationPackage.AddFiles(CustomContentGenerator.GenerateAssetFileList(customizations, policyStore, "VariantApps"));
			}
			foreach (Application application in from x in customizations.Variants.SelectMany((Variant x) => x.ApplicationGroups).SelectMany((Applications x) => x.Items)
			where x.TargetPartition.Equals(partition, StringComparison.OrdinalIgnoreCase)
			select x)
			{
				if (!string.IsNullOrEmpty(application.Source))
				{
					customizationPackage.AddFile(FileType.Regular, application.ExpandedSourcePath, application.DeviceDestination);
				}
				if (!string.IsNullOrEmpty(application.License))
				{
					customizationPackage.AddFile(FileType.Regular, application.ExpandedLicensePath, application.DeviceLicense);
				}
			}
			if (customizationPackage.Files.Any<CustomizationFile>())
			{
				return customizationPackage.SavePackage(config.OutputDirectory);
			}
			return null;
		}

		// Token: 0x06000031 RID: 49 RVA: 0x00003140 File Offset: 0x00001340
		private static string GenerateStaticAppsPackage(Customizations config, ImageCustomizations customizations, PolicyStore policyStore, string partition)
		{
			CustomizationPackage customizationPackage = new CustomizationPackage(partition);
			customizationPackage.Component = config.ImageDeviceName;
			customizationPackage.SubComponent += ".StaticApps";
			customizationPackage.Owner = customizations.Owner;
			customizationPackage.OwnerType = customizations.OwnerType;
			customizationPackage.CpuType = config.ImageCpuType;
			customizationPackage.BuildType = config.ImageBuildType;
			customizationPackage.Version = config.ImageVersion;
			if (partition.Equals(PkgConstants.c_strMainOsPartition))
			{
				customizationPackage.AddFiles(CustomContentGenerator.GenerateAssetFileList(customizations, policyStore, "StaticApps"));
			}
			if (customizations.StaticVariant != null)
			{
				IEnumerable<Application> enumerable = customizations.StaticVariant.ApplicationGroups.SelectMany((Applications x) => x.Items);
				enumerable = from x in enumerable
				where x.TargetPartition.Equals(partition, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(x.DeviceDestination)
				select x;
				if (enumerable.Any<Application>())
				{
					string tempDirectory = FileUtils.GetTempDirectory();
					foreach (Application application in enumerable)
					{
						if (!string.IsNullOrEmpty(application.Source))
						{
							customizationPackage.AddFile(FileType.Regular, application.ExpandedSourcePath, application.DeviceDestination);
						}
						if (!partition.Equals(PkgConstants.c_strMainOsPartition, StringComparison.OrdinalIgnoreCase))
						{
							customizationPackage.AddFile(FileType.Regular, application.ExpandedLicensePath, application.DeviceLicense);
							XElement rootNode = XElement.Load(application.ExpandedProvXMLPath);
							KeyValuePair<Guid, XElement> keyValuePair = application.UpdateProvXml(rootNode);
							string fileName = Path.GetFileName(application.ProvXML);
							string text = Path.Combine(tempDirectory, fileName);
							keyValuePair.Value.Save(text);
							customizationPackage.AddFile(FileType.Regular, text, application.DeviceProvXML);
						}
					}
				}
			}
			if (customizationPackage.Files.Any<CustomizationFile>())
			{
				return customizationPackage.SavePackage(config.OutputDirectory);
			}
			return null;
		}

		// Token: 0x06000032 RID: 50 RVA: 0x0000333C File Offset: 0x0000153C
		private static IEnumerable<CustomizationFile> GenerateAssetFileList(ImageCustomizations customizations, PolicyStore policyStore, string package = "")
		{
			List<CustomizationFile> list = new List<CustomizationFile>();
			IEnumerable<Variant> enumerable = customizations.Variants;
			if (customizations.StaticVariant != null)
			{
				enumerable = enumerable.Concat(customizations.StaticVariant.ToEnumerable<StaticVariant>());
			}
			foreach (Settings settings in enumerable.SelectMany((Variant x) => x.SettingGroups))
			{
				if (policyStore.SettingGroupByPath(settings.Path) != null)
				{
					foreach (Asset asset in settings.Assets)
					{
						PolicyAssetInfo policyAssetInfo = policyStore.AssetByPathAndName(settings.Path, asset.Name);
						if (policyAssetInfo != null && package.Equals(policyAssetInfo.TargetPackage, StringComparison.OrdinalIgnoreCase))
						{
							string devicePath = asset.GetDevicePathWithMacros(policyAssetInfo);
							if (!list.Any((CustomizationFile x) => x.Destination.Equals(devicePath)))
							{
								list.Add(new CustomizationFile(asset.ExpandedSourcePath, devicePath));
							}
						}
					}
				}
			}
			return list;
		}

		// Token: 0x06000033 RID: 51 RVA: 0x0000348C File Offset: 0x0000168C
		private static IEnumerable<CustomizationFile> GenerateStaticAppLicenseList(ImageCustomizations customizations)
		{
			List<CustomizationFile> list = new List<CustomizationFile>();
			if (customizations.StaticVariant == null)
			{
				return list;
			}
			foreach (Application application in from x in customizations.StaticVariant.ApplicationGroups.SelectMany((Applications x) => x.Items)
			where x.TargetPartition.Equals(PkgConstants.c_strMainOsPartition, StringComparison.OrdinalIgnoreCase)
			select x)
			{
				list.Add(new CustomizationFile(application.ExpandedLicensePath, application.DeviceLicense));
			}
			return list;
		}

		// Token: 0x06000034 RID: 52 RVA: 0x00003548 File Offset: 0x00001748
		private static IEnumerable<CustomizationFile> GenerateStaticAppProvXMLList(ImageCustomizations customizations)
		{
			List<CustomizationFile> list = new List<CustomizationFile>();
			if (customizations.StaticVariant == null)
			{
				return list;
			}
			string tempDirectory = FileUtils.GetTempDirectory();
			foreach (Application application in from x in customizations.StaticVariant.ApplicationGroups.SelectMany((Applications x) => x.Items)
			where x.TargetPartition.Equals(PkgConstants.c_strMainOsPartition, StringComparison.OrdinalIgnoreCase)
			select x)
			{
				XElement rootNode = XElement.Load(application.ExpandedProvXMLPath);
				KeyValuePair<Guid, XElement> keyValuePair = application.UpdateProvXml(rootNode);
				string text = Path.GetFileName(application.ProvXML);
				string text2 = Path.Combine(tempDirectory, text);
				keyValuePair.Value.Save(text2);
				list.Add(new CustomizationFile(text2, application.DeviceProvXML));
				if (text.StartsWith("MPAP_", StringComparison.OrdinalIgnoreCase))
				{
					text = text.Substring(5);
				}
				text = text.Insert(0, "mxipupdate_");
				list.Add(new CustomizationFile(text2, Path.Combine("Windows\\System32\\Migrators\\DuMigrationProvisionerMicrosoft\\provxml", text)));
			}
			return list;
		}

		// Token: 0x06000035 RID: 53 RVA: 0x00003688 File Offset: 0x00001888
		private static List<KeyValuePair<string, string>> GenerateDataPartitionContent(ImageCustomizations customizations)
		{
			List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
			if (customizations.StaticVariant != null)
			{
				foreach (var <>f__AnonymousType in from x in customizations.StaticVariant.DataAssetGroups
				from item in x.Items
				select new
				{
					Key = x.Type,
					Value = item
				})
				{
					string expandedSourcePath = <>f__AnonymousType.Value.ExpandedSourcePath;
					if (File.Exists(expandedSourcePath))
					{
						string fileName = Path.GetFileName(<>f__AnonymousType.Value.Source);
						string text = <>f__AnonymousType.Key.DestinationPath();
						text = Path.Combine(text, fileName);
						list.Add(new KeyValuePair<string, string>(expandedSourcePath, text));
					}
					else if (Directory.Exists(expandedSourcePath))
					{
						foreach (string text2 in Directory.EnumerateFiles(expandedSourcePath, "*", SearchOption.AllDirectories))
						{
							string text3 = <>f__AnonymousType.Key.DestinationPath();
							text3 = FileUtils.RerootPath(text2, expandedSourcePath, text3);
							list.Add(new KeyValuePair<string, string>(text2, text3));
						}
					}
				}
			}
			return list;
		}

		// Token: 0x06000036 RID: 54 RVA: 0x000037F0 File Offset: 0x000019F0
		private static MVVariant GenerateGroupForVariant(Variant variant, ImageCustomizations customizations, PolicyStore policyStore)
		{
			MVVariant mvvariant = new MVVariant(variant.Name);
			foreach (TargetRef targetRef in variant.TargetRefs)
			{
				Target targetWithId = customizations.GetTargetWithId(targetRef.Id);
				if (targetWithId != null)
				{
					foreach (TargetState targetState in targetWithId.TargetStates)
					{
						MVCondition provisioningCondition = new MVCondition();
						targetState.Items.ForEach(delegate(Condition x)
						{
							provisioningCondition.KeyValues.Add(x.Name, new WPConstraintValue(x.Value, x.IsWildCard));
						});
						mvvariant.Conditions.Add(provisioningCondition);
					}
				}
			}
			if (variant.TargetRefs.Count > 0)
			{
				CustomContentGenerator.GenerateAppProvisioningForVariant(mvvariant, variant);
			}
			CustomContentGenerator.GenerateSettingsForVariant(mvvariant, variant, policyStore);
			return mvvariant;
		}

		// Token: 0x06000037 RID: 55 RVA: 0x000038F4 File Offset: 0x00001AF4
		private static void GenerateAppProvisioningForVariant(MVVariant provisioningVariant, Variant variant)
		{
			foreach (Application application in variant.ApplicationGroups.SelectMany((Applications x) => x.Items))
			{
				XElement rootNode = XElement.Load(application.ExpandedProvXMLPath);
				KeyValuePair<Guid, XElement> item = application.UpdateProvXml(rootNode);
				provisioningVariant.Applications.Add(item);
			}
		}

		// Token: 0x06000038 RID: 56 RVA: 0x0000397C File Offset: 0x00001B7C
		private static void GenerateSettingsForVariant(MVVariant provisioningVariant, Variant variant, PolicyStore policyStore)
		{
			foreach (Settings settings in variant.SettingGroups)
			{
				PolicyGroup policyGroup = policyStore.SettingGroupByPath(settings.Path);
				if (policyGroup != null)
				{
					MVSettingProvisioning groupProvisioning = MVSettingProvisioning.General;
					if (policyGroup.ImageTimeOnly || variant is StaticVariant)
					{
						groupProvisioning = MVSettingProvisioning.Static;
					}
					else if (policyGroup.FirstVariationOnly)
					{
						groupProvisioning = MVSettingProvisioning.RunOnce;
					}
					else if (policyGroup.CriticalSettings)
					{
						groupProvisioning = MVSettingProvisioning.Connectivity;
					}
					MVSettingGroup mvsettingGroup = new MVSettingGroup(settings.Path, policyGroup.Path);
					PolicyMacroTable groupMacros = policyGroup.GetMacroTable(settings.Path);
					foreach (Setting setting in settings.Items)
					{
						PolicySetting policySetting = policyStore.SettingByPathAndName(settings.Path, setting.Name);
						if (policySetting != null)
						{
							PolicyMacroTable macroTable = policySetting.GetMacroTable(groupMacros, setting.Name);
							IEnumerable<string> resolvedProvisioningPath = policySetting.Destination.GetResolvedProvisioningPath(macroTable);
							if (policySetting.Destination.Destination != PolicySettingDestinationType.None)
							{
								MVSetting mvsetting = new MVSetting(resolvedProvisioningPath, policySetting.Destination.GetResolvedRegistryKey(macroTable), policySetting.Destination.GetResolvedRegistryValueName(macroTable), policySetting.Destination.Type, policySetting.Partition);
								mvsetting.ProvisioningTime = groupProvisioning;
								mvsetting.Value = policySetting.TransformValue(setting.Value, setting.Type);
								if (setting.Type != null)
								{
									mvsetting.DataType = setting.Type;
								}
								mvsettingGroup.Settings.Add(mvsetting);
							}
						}
					}
					foreach (IGrouping<string, Asset> grouping in from x in settings.Assets
					group x by x.Name)
					{
						PolicyAssetInfo assetInfo = policyGroup.AssetByName(grouping.Key);
						if (assetInfo != null && assetInfo.GenerateAssetProvXML)
						{
							if (assetInfo.OemRegValue != null)
							{
								PolicySettingDestination policySettingDestination = new PolicySettingDestination(assetInfo.Name + ".OEMAssets", policyGroup);
								MVSetting item = assetInfo.ToVariantSetting(grouping, policySettingDestination.GetResolvedProvisioningPath(groupMacros), groupProvisioning, groupMacros);
								mvsettingGroup.Settings.Add(item);
							}
							else
							{
								foreach (IGrouping<CustomizationAssetOwner, Asset> grouping2 in grouping.GroupBy((Asset x) => x.Type))
								{
									PolicySettingDestination destination = new PolicySettingDestination(assetInfo.Name + ((grouping2.Key == CustomizationAssetOwner.OEM) ? ".OEMAssets" : ".MOAssets"), policyGroup);
									IEnumerable<MVSetting> collection = grouping2.Select((Asset x) => x.ToVariantSetting(assetInfo, destination.GetResolvedProvisioningPath(groupMacros), groupProvisioning, groupMacros));
									mvsettingGroup.Settings.AddRange(collection);
								}
							}
						}
					}
					provisioningVariant.SettingsGroups.Add(mvsettingGroup);
				}
			}
		}

		// Token: 0x06000039 RID: 57 RVA: 0x00003D8C File Offset: 0x00001F8C
		public static IEnumerable<CustomizationError> VerifyTargetList(Target target)
		{
			return CustomContentGenerator.VerifyTargetList(target, true);
		}

		// Token: 0x0600003A RID: 58 RVA: 0x00003D98 File Offset: 0x00001F98
		public static IEnumerable<CustomizationError> VerifyTargetList(Target target, bool verifyChildren)
		{
			List<CustomizationError> list = new List<CustomizationError>();
			if (target.TargetStates == null || target.TargetStates.Count<TargetState>() == 0)
			{
				CustomizationError item = new CustomizationError(CustomizationErrorSeverity.Error, target.ToEnumerable<Target>(), Strings.EmptyTargetStates);
				list.Add(item);
			}
			return list;
		}

		// Token: 0x0600003B RID: 59 RVA: 0x00003DDA File Offset: 0x00001FDA
		public static IEnumerable<CustomizationError> VerifyTargetId(Target target)
		{
			return new List<CustomizationError>();
		}

		// Token: 0x0600003C RID: 60 RVA: 0x00003DE1 File Offset: 0x00001FE1
		public static IEnumerable<CustomizationError> VerifyConditions(TargetState conditions, Target target, ImageCustomizations customizations)
		{
			return CustomContentGenerator.VerifyConditions(conditions, target, customizations, true);
		}

		// Token: 0x0600003D RID: 61 RVA: 0x00003DEC File Offset: 0x00001FEC
		public static IEnumerable<CustomizationError> VerifyConditions(TargetState conditions, Target target, ImageCustomizations customizations, bool verifyChildren)
		{
			List<CustomizationError> list = new List<CustomizationError>();
			if (conditions.Items == null || conditions.Items.Count<Condition>() == 0)
			{
				CustomizationError item = new CustomizationError(CustomizationErrorSeverity.Warning, target.ToEnumerable<Target>(), Strings.EmptyTargetState);
				list.Add(item);
			}
			if (verifyChildren)
			{
				foreach (Condition condition in conditions.Items)
				{
					list.AddRange(CustomContentGenerator.VerifyCondition(condition, target, customizations));
				}
			}
			return list;
		}

		// Token: 0x0600003E RID: 62 RVA: 0x00003E80 File Offset: 0x00002080
		public static IEnumerable<CustomizationError> VerifyCondition(Condition condition, Target target, ImageCustomizations customizations)
		{
			List<CustomizationError> list = new List<CustomizationError>();
			list.AddRange(CustomContentGenerator.VerifyConditionName(condition, target));
			list.AddRange(CustomContentGenerator.VerifyConditionValue(condition, target, customizations));
			return list;
		}

		// Token: 0x0600003F RID: 63 RVA: 0x00003EA4 File Offset: 0x000020A4
		public static IEnumerable<CustomizationError> VerifyConditionName(Condition condition, Target target)
		{
			List<CustomizationError> list = new List<CustomizationError>();
			if (!MVCondition.IsValidKey(condition.Name))
			{
				CustomizationError item = new CustomizationError(CustomizationErrorSeverity.Error, target.ToEnumerable<Target>(), Strings.InvalidConditionName, new object[]
				{
					target.Id,
					condition.Name
				});
				list.Add(item);
			}
			if (!MVCondition.ValidKeys.Contains(condition.Name, StringComparer.OrdinalIgnoreCase))
			{
				CustomizationError item2 = new CustomizationError(CustomizationErrorSeverity.Warning, target.ToEnumerable<Target>(), Strings.UnknownConditionName, new object[]
				{
					target.Id,
					condition.Name
				});
				list.Add(item2);
			}
			return list;
		}

		// Token: 0x06000040 RID: 64 RVA: 0x00003F40 File Offset: 0x00002140
		public static IEnumerable<CustomizationError> VerifyConditionValue(Condition condition, Target target, ImageCustomizations customizations)
		{
			List<CustomizationError> list = new List<CustomizationError>();
			string text;
			if (!MVCondition.IsValidValue(condition.Name, new WPConstraintValue(condition.Value, condition.IsWildCard), out text))
			{
				CustomizationError item = new CustomizationError(CustomizationErrorSeverity.Error, target.ToEnumerable<Target>(), Strings.InvalidConditionValue, new object[]
				{
					target.Id,
					condition.Name,
					condition.Value,
					text
				});
				list.Add(item);
			}
			return list;
		}

		// Token: 0x06000041 RID: 65 RVA: 0x00003FB2 File Offset: 0x000021B2
		public static IEnumerable<CustomizationError> VerifyApplicationsGroup(IEnumerable<Applications> groups, Variant variant)
		{
			return CustomContentGenerator.VerifyApplicationsGroup(groups, variant, true);
		}

		// Token: 0x06000042 RID: 66 RVA: 0x00003FBC File Offset: 0x000021BC
		public static IEnumerable<CustomizationError> VerifyApplicationsGroup(IEnumerable<Applications> groups, Variant variant, bool verifyChildren)
		{
			List<CustomizationError> list = new List<CustomizationError>();
			if (verifyChildren)
			{
				foreach (Applications applications in groups)
				{
					list.AddRange(CustomContentGenerator.VerifyApplications(applications, variant));
				}
			}
			return list;
		}

		// Token: 0x06000043 RID: 67 RVA: 0x00004014 File Offset: 0x00002214
		public static IEnumerable<CustomizationError> VerifyApplications(Applications applications, Variant variant)
		{
			return CustomContentGenerator.VerifyApplications(applications, variant, true);
		}

		// Token: 0x06000044 RID: 68 RVA: 0x00004020 File Offset: 0x00002220
		public static IEnumerable<CustomizationError> VerifyApplications(Applications applications, Variant variant, bool verifyChildren)
		{
			List<CustomizationError> list = new List<CustomizationError>();
			if (applications.Items == null || applications.Items.Count<Application>() == 0)
			{
				CustomizationError item = new CustomizationError(CustomizationErrorSeverity.Warning, variant.ToEnumerable<Variant>(), Strings.EmptyApplications);
				list.Add(item);
			}
			if (verifyChildren)
			{
				foreach (Application application in applications.Items)
				{
					list.AddRange(CustomContentGenerator.VerifyApplication(application, variant));
				}
			}
			return list;
		}

		// Token: 0x06000045 RID: 69 RVA: 0x000040B4 File Offset: 0x000022B4
		public static IEnumerable<CustomizationError> VerifyApplication(Application application, Variant variant)
		{
			List<CustomizationError> list = new List<CustomizationError>();
			list.AddRange(CustomContentGenerator.VerifyApplicationSource(application, variant));
			list.AddRange(CustomContentGenerator.VerifyApplicationLicense(application));
			list.AddRange(CustomContentGenerator.VerifyApplicationProvXML(application, variant));
			list.AddRange(CustomContentGenerator.VerifyApplicationTargetPartition(application));
			return list;
		}

		// Token: 0x06000046 RID: 70 RVA: 0x000040F0 File Offset: 0x000022F0
		public static IEnumerable<CustomizationError> VerifyApplicationSource(Application application, Variant variant)
		{
			List<CustomizationError> list = new List<CustomizationError>();
			if (variant is StaticVariant || !string.IsNullOrWhiteSpace(application.Source))
			{
				list.AddRange(CustomContentGenerator.VerifyExpandedPath(application.Source, application.ToEnumerable<Application>(), Application.SourceFieldName));
			}
			return list;
		}

		// Token: 0x06000047 RID: 71 RVA: 0x00004135 File Offset: 0x00002335
		public static IEnumerable<CustomizationError> VerifyApplicationLicense(Application application)
		{
			List<CustomizationError> list = new List<CustomizationError>();
			list.AddRange(CustomContentGenerator.VerifyExpandedPath(application.License, application.ToEnumerable<Application>(), Application.LicenseFieldName));
			return list;
		}

		// Token: 0x06000048 RID: 72 RVA: 0x00004158 File Offset: 0x00002358
		public static IEnumerable<CustomizationError> VerifyApplicationProvXML(Application application, Variant variant)
		{
			List<CustomizationError> list = new List<CustomizationError>();
			list.AddRange(CustomContentGenerator.VerifyExpandedPath(application.ProvXML, application.ToEnumerable<Application>(), Application.ProvXMLFieldName));
			if (list.Count((CustomizationError x) => x.Severity.Equals(CustomizationErrorSeverity.Error)) == 0 && !(variant is StaticVariant))
			{
				list.AddRange(application.VerifyProvXML());
			}
			return list;
		}

		// Token: 0x06000049 RID: 73 RVA: 0x000041C4 File Offset: 0x000023C4
		public static IEnumerable<CustomizationError> VerifyApplicationTargetPartition(Application application)
		{
			List<CustomizationError> list = new List<CustomizationError>();
			string targetPartition = application.TargetPartition;
			if (!Application.ValidPartitions.Contains(application.TargetPartition, StringComparer.OrdinalIgnoreCase))
			{
				string text = string.IsNullOrWhiteSpace(application.Source) ? application.ProvXML : application.Source;
				list.Add(new CustomizationError(CustomizationErrorSeverity.Error, application.ToEnumerable<Application>(), Strings.ApplicationPartitionInvalid, new object[]
				{
					text,
					targetPartition,
					string.Join(", ", Application.ValidPartitions)
				}));
			}
			return list;
		}

		// Token: 0x0600004A RID: 74 RVA: 0x00004249 File Offset: 0x00002449
		public static IEnumerable<CustomizationError> VerifyTargetRefs(IEnumerable<TargetRef> TargetRefs, IEnumerable<Target> targets, Variant parent)
		{
			return CustomContentGenerator.VerifyTargetRefs(TargetRefs, targets, parent, true);
		}

		// Token: 0x0600004B RID: 75 RVA: 0x00004254 File Offset: 0x00002454
		public static IEnumerable<CustomizationError> VerifyTargetRefs(IEnumerable<TargetRef> TargetRefs, IEnumerable<Target> targets, Variant parent, bool verifyChildren)
		{
			List<CustomizationError> list = new List<CustomizationError>();
			if (TargetRefs == null || TargetRefs.Count<TargetRef>() == 0)
			{
				CustomizationError item = new CustomizationError(CustomizationErrorSeverity.Error, parent.ToEnumerable<Variant>(), Strings.EmptyTargetRefs, new object[]
				{
					parent.Name
				});
				list.Add(item);
			}
			if (verifyChildren)
			{
				foreach (TargetRef targetRef in TargetRefs)
				{
					list.AddRange(CustomContentGenerator.VerifyTargetRef(targetRef, targets));
				}
			}
			return list;
		}

		// Token: 0x0600004C RID: 76 RVA: 0x000042E0 File Offset: 0x000024E0
		public static IEnumerable<CustomizationError> VerifyTargetRef(TargetRef targetRef, IEnumerable<Target> targets)
		{
			List<CustomizationError> list = new List<CustomizationError>();
			if ((from x in targets
			where x.Id.Equals(targetRef.Id)
			select x).Count<Target>() == 0)
			{
				CustomizationError item = new CustomizationError(CustomizationErrorSeverity.Error, targetRef.ToEnumerable<TargetRef>(), Strings.UnknownTarget, new object[]
				{
					targetRef.Id
				});
				list.Add(item);
			}
			return list;
		}

		// Token: 0x0600004D RID: 77 RVA: 0x0000434C File Offset: 0x0000254C
		public static IEnumerable<CustomizationError> VerifySettingGroups(IEnumerable<Settings> groups, Variant parent, ImageCustomizations customizations, PolicyStore policyStore)
		{
			return CustomContentGenerator.VerifySettingGroups(groups, parent, customizations, policyStore, true);
		}

		// Token: 0x0600004E RID: 78 RVA: 0x00004358 File Offset: 0x00002558
		public static IEnumerable<CustomizationError> VerifySettingGroups(IEnumerable<Settings> groups, Variant parent, ImageCustomizations customizations, PolicyStore policyStore, bool verifyChildren)
		{
			List<CustomizationError> list = new List<CustomizationError>();
			if (verifyChildren)
			{
				foreach (Settings group in groups)
				{
					list.AddRange(CustomContentGenerator.VerifySettingsGroup(group, parent, customizations, policyStore));
				}
			}
			return list;
		}

		// Token: 0x0600004F RID: 79 RVA: 0x000043B4 File Offset: 0x000025B4
		public static IEnumerable<CustomizationError> VerifySettingsGroup(Settings group, Variant variant, ImageCustomizations customizations, PolicyStore policyStore)
		{
			return CustomContentGenerator.VerifySettingsGroup(group, variant, customizations, policyStore, true);
		}

		// Token: 0x06000050 RID: 80 RVA: 0x000043C0 File Offset: 0x000025C0
		public static IEnumerable<CustomizationError> VerifySettingsGroup(Settings group, Variant variant, ImageCustomizations customizations, PolicyStore policyStore, bool verifyChildren)
		{
			List<CustomizationError> list = new List<CustomizationError>();
			list.AddRange(CustomContentGenerator.VerifySettingsGroupList(group, policyStore));
			list.AddRange(CustomContentGenerator.VerifySettingsGroupPath(group, variant, policyStore));
			if (verifyChildren)
			{
				foreach (Setting setting in group.Items)
				{
					list.AddRange(CustomContentGenerator.VerifySetting(setting, group, policyStore));
				}
				foreach (Asset asset in group.Assets)
				{
					list.AddRange(CustomContentGenerator.VerifyAsset(asset, group, customizations, policyStore));
				}
			}
			return list;
		}

		// Token: 0x06000051 RID: 81 RVA: 0x0000448C File Offset: 0x0000268C
		public static IEnumerable<CustomizationError> VerifySettingsGroupList(Settings group, PolicyStore policyStore)
		{
			List<CustomizationError> list = new List<CustomizationError>();
			if (group.Items == null || group.Items.Count<Setting>() == 0)
			{
				CustomizationError item = new CustomizationError(CustomizationErrorSeverity.Warning, group.ToEnumerable<Settings>(), Strings.EmptySettingsGroup, new object[]
				{
					group.Path
				});
				list.Add(item);
				return list;
			}
			PolicyGroup policyGroup = policyStore.SettingGroupByPath(group.Path);
			if (policyGroup == null)
			{
				CustomizationError item2 = new CustomizationError(CustomizationErrorSeverity.Warning, group.ToEnumerable<Settings>(), Strings.UnknownSettingsPath, new object[]
				{
					group.Path
				});
				list.Add(item2);
				return list;
			}
			if (policyGroup.Atomic)
			{
				using (IEnumerator<PolicySetting> enumerator = policyGroup.Settings.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						PolicySetting policy = enumerator.Current;
						if (!group.Items.Any((Setting x) => policy.Equals(policyGroup.SettingByName(x.Name))))
						{
							CustomizationError item3 = new CustomizationError(CustomizationErrorSeverity.Error, group.ToEnumerable<Settings>(), Strings.AtomicSettingMissing, new object[]
							{
								group.Path,
								policy.Name
							});
							list.Add(item3);
						}
					}
				}
			}
			return list;
		}

		// Token: 0x06000052 RID: 82 RVA: 0x000045E4 File Offset: 0x000027E4
		public static IEnumerable<CustomizationError> VerifySettingsGroupPath(Settings group, Variant variant, PolicyStore policyStore)
		{
			List<CustomizationError> list = new List<CustomizationError>();
			PolicyGroup policyGroup = policyStore.SettingGroupByPath(group.Path);
			if (policyGroup == null)
			{
				CustomizationError item = new CustomizationError(CustomizationErrorSeverity.Warning, group.ToEnumerable<Settings>(), Strings.UnknownSettingsPath, new object[]
				{
					group.Path
				});
				list.Add(item);
				return list;
			}
			if (policyGroup.ImageTimeOnly && !(variant is StaticVariant))
			{
				CustomizationError item2 = new CustomizationError(CustomizationErrorSeverity.Error, group.ToEnumerable<Settings>(), Strings.VariantedImageTimeOnlySettingsGroup, new object[]
				{
					group.Path
				});
				list.Add(item2);
			}
			return list;
		}

		// Token: 0x06000053 RID: 83 RVA: 0x0000466A File Offset: 0x0000286A
		public static IEnumerable<CustomizationError> VerifySetting(Setting setting, Settings group, PolicyStore policyStore)
		{
			return CustomContentGenerator.VerifySetting(setting, group, policyStore, null, true);
		}

		// Token: 0x06000054 RID: 84 RVA: 0x00004676 File Offset: 0x00002876
		public static IEnumerable<CustomizationError> VerifySetting(Setting setting, Settings group, PolicyStore policyStore, PolicyGroup policyGroup)
		{
			return CustomContentGenerator.VerifySetting(setting, group, policyStore, policyGroup, true);
		}

		// Token: 0x06000055 RID: 85 RVA: 0x00004684 File Offset: 0x00002884
		public static IEnumerable<CustomizationError> VerifySetting(Setting setting, Settings group, PolicyStore policyStore, PolicyGroup policyGroup, bool verifyChildren)
		{
			List<CustomizationError> list = new List<CustomizationError>();
			if (policyGroup == null)
			{
				policyGroup = policyStore.SettingGroupByPath(group.Path);
				if (policyGroup == null)
				{
					CustomizationError item = new CustomizationError(CustomizationErrorSeverity.Warning, group.ToEnumerable<Settings>(), Strings.UnknownSettingsPath, new object[]
					{
						group.Path
					});
					list.Add(item);
				}
			}
			list.AddRange(CustomContentGenerator.VerifySettingName(setting, group, policyStore));
			list.AddRange(CustomContentGenerator.VerifySettingValue(setting, group, policyStore));
			return list;
		}

		// Token: 0x06000056 RID: 86 RVA: 0x000046F0 File Offset: 0x000028F0
		public static IEnumerable<CustomizationError> VerifySettingName(Setting setting, Settings group, PolicyStore policyStore)
		{
			return CustomContentGenerator.VerifySettingName(setting, group, policyStore, null);
		}

		// Token: 0x06000057 RID: 87 RVA: 0x000046FC File Offset: 0x000028FC
		public static IEnumerable<CustomizationError> VerifySettingName(Setting setting, Settings group, PolicyStore policyStore, PolicyGroup policyGroup)
		{
			List<CustomizationError> list = new List<CustomizationError>();
			if ((from x in @group.Items
			where setting.Name.Equals(x.Name, StringComparison.OrdinalIgnoreCase)
			select x).Count<Setting>() > 1)
			{
				CustomizationError item = new CustomizationError(CustomizationErrorSeverity.Error, setting.ToEnumerable<Setting>(), Strings.DuplicateSettings, new object[]
				{
					setting.Name,
					group.Path
				});
				list.Add(item);
			}
			if (policyGroup == null)
			{
				policyGroup = policyStore.SettingGroupByPath(group.Path);
				if (policyGroup == null)
				{
					CustomizationError item2 = new CustomizationError(CustomContentGenerator.enforcingStrictSettingPolicies(), group.ToEnumerable<Settings>(), Strings.UnableToValidateSettingNameUnknownSettingsPath, new object[]
					{
						group.Path
					});
					list.Add(item2);
					return list;
				}
			}
			PolicySetting policySetting = policyGroup.SettingByName(setting.Name);
			if (policySetting == null)
			{
				CustomizationError item3 = new CustomizationError(CustomContentGenerator.enforcingStrictSettingPolicies(), setting.ToEnumerable<Setting>(), Strings.UnknownSettingName, new object[]
				{
					group.Path,
					setting.Name
				});
				list.Add(item3);
				return list;
			}
			if (!PolicySetting.ValidPartitions.Contains(policySetting.Partition, StringComparer.OrdinalIgnoreCase))
			{
				CustomizationError item4 = new CustomizationError(CustomizationErrorSeverity.Error, setting.ToEnumerable<Setting>(), Strings.SettingPartitionInvalid, new object[]
				{
					group.Path,
					setting.Name,
					policySetting.Partition,
					string.Join(", ", PolicySetting.ValidPartitions)
				});
				list.Add(item4);
				return list;
			}
			return list;
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00004884 File Offset: 0x00002A84
		public static CustomizationErrorSeverity enforcingStrictSettingPolicies()
		{
			if (Customizations.StrictSettingPolicies)
			{
				return CustomizationErrorSeverity.Error;
			}
			return CustomizationErrorSeverity.Warning;
		}

		// Token: 0x06000059 RID: 89 RVA: 0x00004890 File Offset: 0x00002A90
		public static IEnumerable<CustomizationError> VerifySettingValue(Setting setting, Settings group, PolicyStore policyStore)
		{
			return CustomContentGenerator.VerifySettingValue(setting, group, policyStore, null);
		}

		// Token: 0x0600005A RID: 90 RVA: 0x0000489C File Offset: 0x00002A9C
		public static IEnumerable<CustomizationError> VerifySettingValue(Setting setting, Settings group, PolicyStore policyStore, PolicySetting policySetting)
		{
			List<CustomizationError> list = new List<CustomizationError>();
			if (policySetting == null)
			{
				PolicyGroup policyGroup = policyStore.SettingGroupByPath(group.Path);
				if (policyGroup == null)
				{
					CustomizationError item = new CustomizationError(CustomizationErrorSeverity.Warning, group.ToEnumerable<Settings>(), Strings.UnableToValidateSettingValueUnknownSettingsPath, new object[]
					{
						group.Path
					});
					list.Add(item);
					return list;
				}
				if (policyGroup != null)
				{
					policySetting = policyGroup.SettingByName(setting.Name);
					if (policySetting == null)
					{
						CustomizationError item2 = new CustomizationError(CustomizationErrorSeverity.Warning, setting.ToEnumerable<Setting>(), Strings.UnableToValidateSettingValueUnknownSettingName, new object[]
						{
							group.Path,
							setting.Name
						});
						list.Add(item2);
						return list;
					}
				}
			}
			if (setting.Value == null)
			{
				CustomizationError item3 = new CustomizationError(CustomizationErrorSeverity.Error, setting.ToEnumerable<Setting>(), Strings.NullSettingValue, new object[]
				{
					group.Path,
					setting.Name
				});
				list.Add(item3);
				return list;
			}
			if (!policySetting.IsValidValue(setting.Value, setting.Type))
			{
				CustomizationError item4 = new CustomizationError(CustomizationErrorSeverity.Error, setting.ToEnumerable<Setting>(), Strings.InvalidSettingValue, new object[]
				{
					group.Path,
					setting.Name,
					setting.Value
				});
				list.Add(item4);
				return list;
			}
			if (policySetting.AssetInfo == null)
			{
				return list;
			}
			bool flag = group.Assets.Find((Asset x) => setting.Value.Equals(x.Id, StringComparison.OrdinalIgnoreCase)) != null;
			if (!flag)
			{
				flag = (policySetting.AssetInfo.Presets.Find((PolicyEnum preset) => preset.Value.Equals(setting.Value, StringComparison.OrdinalIgnoreCase)) != null);
			}
			if (!flag)
			{
				CustomizationError item5 = new CustomizationError(CustomizationErrorSeverity.Error, setting.ToEnumerable<Setting>(), Strings.AssetNotFound, new object[]
				{
					setting.Value,
					group.Path
				});
				list.Add(item5);
			}
			return list;
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00004A98 File Offset: 0x00002C98
		public static IEnumerable<CustomizationError> VerifyAssets(Settings settings, PolicyGroup group)
		{
			List<CustomizationError> list = new List<CustomizationError>();
			if (settings.Assets != null && settings.Assets.Count<Asset>() != 0)
			{
				if (group == null)
				{
					CustomizationError item = new CustomizationError(CustomizationErrorSeverity.Warning, settings.ToEnumerable<Settings>(), Strings.UnableToValidateSettingAssetsUnknownSettingsPath, new object[]
					{
						settings.Path
					});
					list.Add(item);
				}
				else if (group.Assets == null || group.Assets.Count<PolicyAssetInfo>() == 0)
				{
					CustomizationError item2 = new CustomizationError(CustomizationErrorSeverity.Error, settings.ToEnumerable<Settings>(), Strings.AssetsNotSupported, new object[]
					{
						settings.Path
					});
					list.Add(item2);
				}
			}
			return list;
		}

		// Token: 0x0600005C RID: 92 RVA: 0x00004B2C File Offset: 0x00002D2C
		private static IEnumerable<CustomizationError> GetAssetInfo(Asset asset, string groupPath, string fieldName, PolicyStore policyStore, out PolicyAssetInfo assetInfo)
		{
			List<CustomizationError> list = new List<CustomizationError>();
			assetInfo = null;
			if (policyStore.SettingGroupByPath(groupPath) == null)
			{
				CustomizationError item = new CustomizationError(CustomizationErrorSeverity.Warning, asset.ToEnumerable<Asset>(), Strings.UnableToValidateAssetFieldUnknownSettingsPath, new object[]
				{
					fieldName,
					asset.Id,
					groupPath
				});
				list.Add(item);
				return list;
			}
			assetInfo = policyStore.AssetByPathAndName(groupPath, asset.Name);
			if (assetInfo == null)
			{
				CustomizationError item2 = new CustomizationError(CustomizationErrorSeverity.Warning, asset.ToEnumerable<Asset>(), Strings.UnableToValidateAssetFieldUnknownAssetName, new object[]
				{
					fieldName,
					asset.Id,
					groupPath,
					asset.Name
				});
				list.Add(item2);
				return list;
			}
			return list;
		}

		// Token: 0x0600005D RID: 93 RVA: 0x00004BCF File Offset: 0x00002DCF
		public static IEnumerable<CustomizationError> VerifyAsset(Asset asset, Settings group, ImageCustomizations customizations, PolicyStore policyStore)
		{
			return CustomContentGenerator.VerifyAsset(asset, group, customizations, policyStore, null);
		}

		// Token: 0x0600005E RID: 94 RVA: 0x00004BDC File Offset: 0x00002DDC
		public static IEnumerable<CustomizationError> VerifyAsset(Asset asset, Settings group, ImageCustomizations customizations, PolicyStore policyStore, PolicyAssetInfo assetInfo)
		{
			List<CustomizationError> list = new List<CustomizationError>();
			if (assetInfo == null)
			{
				list.AddRange(CustomContentGenerator.GetAssetInfo(asset, group.Path, "Asset", policyStore, out assetInfo));
			}
			if (assetInfo == null)
			{
				CustomizationError item = new CustomizationError(CustomizationErrorSeverity.Warning, asset.ToEnumerable<Asset>(), Strings.AssetNotSupported, new object[]
				{
					asset.Name,
					group.Path
				});
				list.Add(item);
			}
			list.AddRange(CustomContentGenerator.VerifyAssetType(asset, group, policyStore, assetInfo));
			list.AddRange(CustomContentGenerator.VerifyAssetSource(asset, group, customizations, policyStore, assetInfo));
			list.AddRange(CustomContentGenerator.VerifyAssetTargetFileName(asset, group, customizations, policyStore, assetInfo));
			list.AddRange(CustomContentGenerator.VerifyAssetDisplayName(asset, group));
			list.AddRange(CustomContentGenerator.VerifyAssetTargetPackage(asset, group, customizations, policyStore));
			return list;
		}

		// Token: 0x0600005F RID: 95 RVA: 0x00004C91 File Offset: 0x00002E91
		public static IEnumerable<CustomizationError> VerifyAssetType(Asset asset, Settings group, PolicyStore policyStore)
		{
			return CustomContentGenerator.VerifyAssetType(asset, group, policyStore, null);
		}

		// Token: 0x06000060 RID: 96 RVA: 0x00004C9C File Offset: 0x00002E9C
		public static IEnumerable<CustomizationError> VerifyAssetType(Asset asset, Settings group, PolicyStore policyStore, PolicyAssetInfo assetInfo)
		{
			List<CustomizationError> list = new List<CustomizationError>();
			if (assetInfo == null)
			{
				list.AddRange(CustomContentGenerator.GetAssetInfo(asset, group.Path, "Type", policyStore, out assetInfo));
			}
			if (assetInfo == null)
			{
				CustomizationError item = new CustomizationError(CustomizationErrorSeverity.Warning, asset.ToEnumerable<Asset>(), Strings.UnableToValidateAssetTypeUnknownSettingsPath, new object[]
				{
					group.Path
				});
				list.Add(item);
				return list;
			}
			if (asset.Type == CustomizationAssetOwner.MobileOperator && assetInfo.MORegKey == null)
			{
				CustomizationError item2 = new CustomizationError(CustomizationErrorSeverity.Error, asset.ToEnumerable<Asset>(), Strings.SettingDoesNotSupportOperatorAssets, new object[]
				{
					group.Path,
					asset.Name
				});
				list.Add(item2);
			}
			return list;
		}

		// Token: 0x06000061 RID: 97 RVA: 0x00004D3C File Offset: 0x00002F3C
		public static IEnumerable<CustomizationError> VerifyAssetSource(Asset asset, Settings group, ImageCustomizations customizations, PolicyStore policyStore)
		{
			return CustomContentGenerator.VerifyAssetSource(asset, group, customizations, policyStore, null);
		}

		// Token: 0x06000062 RID: 98 RVA: 0x00004D48 File Offset: 0x00002F48
		public static IEnumerable<CustomizationError> VerifyAssetSource(Asset asset, Settings group, ImageCustomizations customizations, PolicyStore policyStore, PolicyAssetInfo assetInfo)
		{
			List<CustomizationError> list = new List<CustomizationError>();
			list.AddRange(CustomContentGenerator.VerifyExpandedPath(asset.Source, asset.ToEnumerable<Asset>(), Asset.SourceFieldName));
			if (assetInfo == null)
			{
				list.AddRange(CustomContentGenerator.GetAssetInfo(asset, group.Path, Asset.SourceFieldName, policyStore, out assetInfo));
			}
			if (assetInfo == null)
			{
				CustomizationError item = new CustomizationError(CustomizationErrorSeverity.Warning, asset.ToEnumerable<Asset>(), Strings.UnableToValidateAssetTypeUnknownSettingsPath, new object[]
				{
					group.Path
				});
				list.Add(item);
				return list;
			}
			if (string.IsNullOrWhiteSpace(asset.TargetFileName))
			{
				list.AddRange(CustomContentGenerator.verifyAssetId(asset, group, customizations, policyStore, assetInfo));
			}
			return list;
		}

		// Token: 0x06000063 RID: 99 RVA: 0x00004DE4 File Offset: 0x00002FE4
		public static IEnumerable<CustomizationError> VerifyAssetDisplayName(Asset asset, Settings group)
		{
			List<CustomizationError> list = new List<CustomizationError>();
			if (!string.IsNullOrEmpty(asset.DisplayName) && (from x in @group.Assets
			where asset.DisplayName.Equals(x.DisplayName, StringComparison.OrdinalIgnoreCase) && asset.Name.Equals(x.Name, StringComparison.OrdinalIgnoreCase)
			select x).Count<Asset>() > 1)
			{
				CustomizationError item = new CustomizationError(CustomizationErrorSeverity.Warning, group.ToEnumerable<Settings>(), Strings.AssetWithDuplicateDisplayName, new object[]
				{
					group.Path,
					asset.DisplayName
				});
				list.Add(item);
			}
			return list;
		}

		// Token: 0x06000064 RID: 100 RVA: 0x00004E6C File Offset: 0x0000306C
		public static IEnumerable<CustomizationError> VerifyAssetTargetFileName(Asset asset, Settings group, ImageCustomizations customizations, PolicyStore policyStore)
		{
			return CustomContentGenerator.VerifyAssetTargetFileName(asset, group, customizations, policyStore, null);
		}

		// Token: 0x06000065 RID: 101 RVA: 0x00004E78 File Offset: 0x00003078
		public static IEnumerable<CustomizationError> VerifyAssetTargetFileName(Asset asset, Settings group, ImageCustomizations customizations, PolicyStore policyStore, PolicyAssetInfo assetInfo)
		{
			List<CustomizationError> list = new List<CustomizationError>();
			if (string.IsNullOrWhiteSpace(asset.TargetFileName))
			{
				return list;
			}
			if (asset.TargetFileName.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
			{
				CustomizationError item = new CustomizationError(CustomizationErrorSeverity.Error, asset.ToEnumerable<Asset>(), Strings.AssetInvalidTargetFileName, new object[]
				{
					asset.Name,
					group.Path,
					asset.TargetFileName
				});
				list.Add(item);
				return list;
			}
			if (assetInfo == null)
			{
				list.AddRange(CustomContentGenerator.GetAssetInfo(asset, group.Path, "Target Filename", policyStore, out assetInfo));
			}
			if (assetInfo == null)
			{
				CustomizationError item2 = new CustomizationError(CustomizationErrorSeverity.Warning, asset.ToEnumerable<Asset>(), Strings.UnableToValidateAssetTypeUnknownSettingsPath, new object[]
				{
					group.Path
				});
				list.Add(item2);
			}
			list.AddRange(CustomContentGenerator.verifyAssetId(asset, group, customizations, policyStore, assetInfo));
			return list;
		}

		// Token: 0x06000066 RID: 102 RVA: 0x00004F48 File Offset: 0x00003148
		private static IEnumerable<CustomizationError> verifyAssetId(Asset asset, Settings group, ImageCustomizations customizations, PolicyStore policyStore, PolicyAssetInfo assetInfo = null)
		{
			List<CustomizationError> list = new List<CustomizationError>();
			if (assetInfo == null)
			{
				assetInfo = policyStore.AssetByPathAndName(group.Path, asset.Name);
			}
			if (assetInfo == null)
			{
				return list;
			}
			if (assetInfo != null && !assetInfo.IsValidFileType(Path.GetFileName(asset.Id)))
			{
				CustomizationError item = new CustomizationError(CustomizationErrorSeverity.Error, asset.ToEnumerable<Asset>(), Strings.UnsupportedFileType, new object[]
				{
					group.Path,
					asset.Name,
					Path.GetExtension(asset.Id),
					string.Join(", ", assetInfo.FileTypes)
				});
				list.Add(item);
			}
			IEnumerable<Settings> enumerable = customizations.Variants.SelectMany((Variant x) => x.SettingGroups);
			if (customizations.StaticVariant != null)
			{
				enumerable = enumerable.Concat(customizations.StaticVariant.SettingGroups);
			}
			foreach (Settings settings in enumerable)
			{
				PolicyGroup policyGroup = policyStore.SettingGroupByPath(settings.Path);
				if (policyGroup != null)
				{
					List<Asset> list2 = new List<Asset>();
					foreach (Asset asset2 in settings.Assets)
					{
						PolicyAssetInfo policyAssetInfo = policyGroup.AssetByName(asset2.Name);
						if (policyAssetInfo != null && asset.GetDevicePathWithMacros(assetInfo).Equals(asset2.GetDevicePathWithMacros(policyAssetInfo)))
						{
							list2.Add(asset2);
						}
					}
					foreach (Asset asset3 in list2)
					{
						if (policyGroup.AssetByName(asset3.Name) != null && !asset.ExpandedSourcePath.Equals(asset3.ExpandedSourcePath, StringComparison.OrdinalIgnoreCase))
						{
							list.Add(new CustomizationError(CustomizationErrorSeverity.Error, asset.ToEnumerable<Asset>(), Strings.AssetTargetConflict, new object[]
							{
								asset.Id,
								asset.Source,
								asset3.Source
							}));
						}
					}
				}
			}
			return list;
		}

		// Token: 0x06000067 RID: 103 RVA: 0x000051B0 File Offset: 0x000033B0
		public static IEnumerable<CustomizationError> VerifyAssetTargetPackage(Asset asset, Settings group, ImageCustomizations customizations, PolicyStore policyStore)
		{
			List<CustomizationError> list = new List<CustomizationError>();
			PolicyAssetInfo policyAssetInfo = policyStore.AssetByPathAndName(group.Path, asset.Name);
			if (policyAssetInfo == null)
			{
				return list;
			}
			IEnumerable<Settings> enumerable = customizations.Variants.SelectMany((Variant x) => x.SettingGroups);
			if (customizations.StaticVariant != null)
			{
				enumerable = enumerable.Concat(customizations.StaticVariant.SettingGroups);
			}
			foreach (Settings settings in enumerable)
			{
				PolicyGroup policyGroup = policyStore.SettingGroupByPath(settings.Path);
				if (policyGroup != null)
				{
					List<Asset> list2 = new List<Asset>();
					foreach (Asset asset2 in settings.Assets)
					{
						PolicyAssetInfo policyAssetInfo2 = policyGroup.AssetByName(asset2.Name);
						if (policyAssetInfo2 != null && asset.GetDevicePathWithMacros(policyAssetInfo).Equals(asset2.GetDevicePathWithMacros(policyAssetInfo2)))
						{
							list2.Add(asset2);
						}
					}
					foreach (Asset asset3 in list2)
					{
						PolicyAssetInfo policyAssetInfo3 = policyGroup.AssetByName(asset3.Name);
						if (policyAssetInfo3 != null && !policyAssetInfo.TargetPackage.Equals(policyAssetInfo3.TargetPackage, StringComparison.OrdinalIgnoreCase))
						{
							IEnumerable<IDefinedIn> source = new List<IDefinedIn>
							{
								asset,
								asset3
							};
							string message = string.Format(Strings.AssetPackageConflict, new object[]
							{
								asset.Name,
								policyAssetInfo.TargetPackage,
								asset3.Name,
								policyAssetInfo3.TargetPackage,
								asset.GetDevicePath(policyAssetInfo.TargetDir)
							});
							list.Add(new CustomizationError(CustomizationErrorSeverity.Error, source.DistinctBy((IDefinedIn x) => x.DefinedInFile), message));
						}
					}
				}
			}
			return list;
		}

		// Token: 0x06000068 RID: 104 RVA: 0x00005410 File Offset: 0x00003610
		public static IEnumerable<CustomizationError> VerifyDataAssetGroups(IEnumerable<DataAssets> groups, Variant parent)
		{
			return CustomContentGenerator.VerifyDataAssetGroups(groups, parent, true);
		}

		// Token: 0x06000069 RID: 105 RVA: 0x0000541C File Offset: 0x0000361C
		public static IEnumerable<CustomizationError> VerifyDataAssetGroups(IEnumerable<DataAssets> groups, Variant parent, bool verifyChildren)
		{
			List<CustomizationError> list = new List<CustomizationError>();
			if (verifyChildren)
			{
				foreach (DataAssets group in groups)
				{
					list.AddRange(CustomContentGenerator.VerifyDataAssetGroup(group, parent));
				}
			}
			return list;
		}

		// Token: 0x0600006A RID: 106 RVA: 0x00005474 File Offset: 0x00003674
		public static IEnumerable<CustomizationError> VerifyDataAssetGroup(DataAssets group, Variant variant)
		{
			return CustomContentGenerator.VerifyDataAssetGroup(group, variant, true);
		}

		// Token: 0x0600006B RID: 107 RVA: 0x00005480 File Offset: 0x00003680
		public static IEnumerable<CustomizationError> VerifyDataAssetGroup(DataAssets group, Variant variant, bool verifyChildren)
		{
			List<CustomizationError> list = new List<CustomizationError>();
			if (group.Items == null || group.Items.Count<DataAsset>() == 0)
			{
				CustomizationError item = new CustomizationError(CustomizationErrorSeverity.Warning, variant.ToEnumerable<Variant>(), Strings.EmptyDataAssetsGroup);
				list.Add(item);
			}
			if (verifyChildren)
			{
				foreach (DataAsset dataAsset in group.Items)
				{
					list.AddRange(CustomContentGenerator.VerifyDataAsset(dataAsset));
				}
			}
			return list;
		}

		// Token: 0x0600006C RID: 108 RVA: 0x00005510 File Offset: 0x00003710
		public static IEnumerable<CustomizationError> VerifyDataAsset(DataAsset dataAsset)
		{
			List<CustomizationError> list = new List<CustomizationError>();
			list.AddRange(CustomContentGenerator.VerifyExpandedPath(dataAsset.Source, dataAsset.ToEnumerable<DataAsset>(), DataAsset.SourceFieldName));
			return list;
		}

		// Token: 0x0600006D RID: 109 RVA: 0x00005534 File Offset: 0x00003734
		public static IEnumerable<CustomizationError> VerifyExpandedPath(string path, IEnumerable<IDefinedIn> defined, string fieldName)
		{
			List<CustomizationError> list = new List<CustomizationError>();
			if (string.IsNullOrEmpty(path))
			{
				list.Add(new CustomizationError(CustomizationErrorSeverity.Error, defined, Strings.PathEmptySource, new object[]
				{
					fieldName
				}));
				return list;
			}
			if (path.Contains('%'))
			{
				list.Add(new CustomizationError(CustomizationErrorSeverity.Error, defined, Strings.PathInvalidPercent));
				return list;
			}
			string path2;
			try
			{
				path2 = ImageCustomizations.ExpandPath(path, true);
			}
			catch (PkgGenException ex)
			{
				list.Add(new CustomizationError(CustomizationErrorSeverity.Error, defined, Strings.PathUnresolvedVariable, new object[]
				{
					fieldName,
					path,
					ex.Message
				}));
				return list;
			}
			if (!CustomContentGenerator.VerifyFullPath(path2))
			{
				list.Add(new CustomizationError(CustomizationErrorSeverity.Error, defined, Strings.PathNotAbsolute, new object[]
				{
					fieldName,
					path
				}));
				return list;
			}
			if (!fieldName.Equals("Import source"))
			{
				string fileName = Path.GetFileName(path);
				string fileName2 = Path.GetFileName(path2);
				if (!string.Equals(fileName, fileName2, StringComparison.OrdinalIgnoreCase))
				{
					list.Add(new CustomizationError(CustomizationErrorSeverity.Error, defined, Strings.PathFileNameContainsVars, new object[]
					{
						fieldName,
						path
					}));
					return list;
				}
			}
			if (!File.Exists(path2))
			{
				if (fieldName.Equals(DataAsset.SourceFieldName) && !Directory.Exists(path2))
				{
					list.Add(new CustomizationError(CustomizationErrorSeverity.Error, defined, Strings.PathDoesNotExist, new object[]
					{
						fieldName,
						path
					}));
					return list;
				}
				if (!fieldName.Equals(DataAsset.SourceFieldName))
				{
					list.Add(new CustomizationError(CustomizationErrorSeverity.Error, defined, Strings.PathFileDoesNotExist, new object[]
					{
						fieldName,
						path
					}));
					return list;
				}
			}
			return list;
		}

		// Token: 0x0600006E RID: 110 RVA: 0x000056BC File Offset: 0x000038BC
		private static bool VerifyFullPath(string path)
		{
			return Path.IsPathRooted(path) && Path.GetPathRoot(path).Contains("\\");
		}

		// Token: 0x04000012 RID: 18
		private static string ProvPKGDevicePath = "\\ProgramData\\Microsoft\\Provisioning";
	}
}
