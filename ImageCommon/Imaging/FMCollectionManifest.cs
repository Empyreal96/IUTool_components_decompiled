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

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x0200002E RID: 46
	[XmlType(Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
	[XmlRoot(ElementName = "FMCollectionManifest", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate", IsNullable = false)]
	public class FMCollectionManifest
	{
		// Token: 0x060001E0 RID: 480 RVA: 0x00010B9C File Offset: 0x0000ED9C
		public bool ShouldSerializeSupportedLocales()
		{
			return this.SupportedLocales != null && this.SupportedLocales.Count<string>() > 0;
		}

		// Token: 0x060001E1 RID: 481 RVA: 0x00010BB6 File Offset: 0x0000EDB6
		public bool ShouldSerializeSupportedResolutions()
		{
			return this.SupportedResolutions != null && this.SupportedResolutions.Count<string>() > 0;
		}

		// Token: 0x060001E2 RID: 482 RVA: 0x00010BD0 File Offset: 0x0000EDD0
		public bool ShouldSerializeFeatureIdentifierPackages()
		{
			return this.FeatureIdentifierPackages != null && this.FeatureIdentifierPackages.Count<FeatureIdentifierPackage>() > 0;
		}

		// Token: 0x060001E3 RID: 483 RVA: 0x00010BEA File Offset: 0x0000EDEA
		public string GetChunkMappingFile(string fmDirectory)
		{
			if (string.IsNullOrEmpty(this.ChunkMappingsFile))
			{
				return this.ChunkMappingsFile;
			}
			return this.ChunkMappingsFile.Replace(FMCollection.c_FMDirectoryVariable, fmDirectory, StringComparison.OrdinalIgnoreCase);
		}

		// Token: 0x060001E4 RID: 484 RVA: 0x00010C14 File Offset: 0x0000EE14
		public List<CpuId> GetWowGuestCpuTypes(CpuId cpuType)
		{
			return ImagingEditions.GetWowGuestCpuTypes((from fm in this.FMs
			where fm.ownerType == OwnerType.Microsoft
			select Path.GetFileName(fm.Path)).ToList<string>(), cpuType);
		}

		// Token: 0x060001E5 RID: 485 RVA: 0x00010C7C File Offset: 0x0000EE7C
		public static FMCollectionManifest ValidateAndLoad(string xmlFile, IULogger logger)
		{
			FMCollectionManifest fmcollectionManifest = new FMCollectionManifest();
			string text = string.Empty;
			string fmcollectionSchema = BuildPaths.FMCollectionSchema;
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			foreach (string text2 in executingAssembly.GetManifestResourceNames())
			{
				if (text2.Contains(fmcollectionSchema))
				{
					text = text2;
					break;
				}
			}
			if (string.IsNullOrEmpty(text))
			{
				throw new ImageCommonException("ImageCommon!ValidateAndLoad: XSD resource was not found: " + fmcollectionSchema);
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
					throw new ImageCommonException("ImageCommon!ValidateAndLoad: Unable to validate FM Collection Manifest XSD for file '" + xmlFile + "'.", innerException);
				}
			}
			logger.LogInfo("ImageCommon: Successfully validated the Feature Manifest XML: {0}", new object[]
			{
				xmlFile
			});
			TextReader textReader = new StreamReader(LongPathFile.OpenRead(xmlFile));
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(FMCollectionManifest));
			try
			{
				fmcollectionManifest = (FMCollectionManifest)xmlSerializer.Deserialize(textReader);
			}
			catch (Exception innerException2)
			{
				throw new ImageCommonException("ImageCommon!ValidateAndLoad: Unable to parse FM Collection XML file '" + xmlFile + "'.", innerException2);
			}
			finally
			{
				textReader.Close();
			}
			List<IGrouping<string, FMCollectionItem>> list = (from g in fmcollectionManifest.FMs.GroupBy((FMCollectionItem fm) => fm.ID, StringComparer.OrdinalIgnoreCase)
			where g.Count<FMCollectionItem>() > 1
			select g).ToList<IGrouping<string, FMCollectionItem>>();
			if (list.Count<IGrouping<string, FMCollectionItem>>() > 0)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (IGrouping<string, FMCollectionItem> grouping in list)
				{
					if ((from gp in grouping
					select gp.Path).Distinct(StringComparer.OrdinalIgnoreCase).Count<string>() == 1)
					{
						if (!grouping.Any((FMCollectionItem gp) => gp.CPUType == CpuId.Invalid))
						{
							if ((from gp in grouping
							select gp.CPUType).Distinct<CpuId>().Count<CpuId>() == grouping.Count<FMCollectionItem>())
							{
								continue;
							}
						}
					}
					stringBuilder.AppendLine(Environment.NewLine + "\t" + grouping.Key + ": ");
					foreach (FMCollectionItem fmcollectionItem in grouping)
					{
						stringBuilder.AppendLine(string.Concat(new string[]
						{
							"\t\t",
							fmcollectionItem.Path,
							" (CpuType=",
							fmcollectionItem.CPUType.ToString(),
							")"
						}));
					}
				}
				if (stringBuilder.Length != 0)
				{
					throw new ImageCommonException("ImageCommon!ValidateAndLoad: Duplicate FMIDs found in FM Collection XML file '" + xmlFile + "': " + stringBuilder.ToString());
				}
			}
			return fmcollectionManifest;
		}

		// Token: 0x060001E6 RID: 486 RVA: 0x00011024 File Offset: 0x0000F224
		public void WriteToFile(string xmlFile)
		{
			TextWriter textWriter = new StreamWriter(LongPathFile.OpenWrite(xmlFile));
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(FMCollectionManifest));
			try
			{
				xmlSerializer.Serialize(textWriter, this);
			}
			catch (Exception innerException)
			{
				throw new FeatureAPIException("FMCollection!WriteToFile: Unable to write FM Collection XML file '" + xmlFile + "'", innerException);
			}
			finally
			{
				textWriter.Close();
			}
		}

		// Token: 0x060001E7 RID: 487 RVA: 0x00011094 File Offset: 0x0000F294
		public void ValidateFeatureIdentiferPackages(List<PublishingPackageInfo> packages)
		{
			if (this.FeatureIdentifierPackages == null || !this.FeatureIdentifierPackages.Any<FeatureIdentifierPackage>())
			{
				return;
			}
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = false;
			using (List<FeatureIdentifierPackage>.Enumerator enumerator = this.FeatureIdentifierPackages.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					FeatureIdentifierPackage fip = enumerator.Current;
					if (fip.FixUpAction == FeatureIdentifierPackage.FixUpActions.None || (fip.FixUpAction == FeatureIdentifierPackage.FixUpActions.AndFeature && !string.IsNullOrEmpty(fip.ID)))
					{
						List<PublishingPackageInfo> list = null;
						if (packages.Any<PublishingPackageInfo>())
						{
							list = (from pkg in packages
							where string.Equals(pkg.FeatureID, fip.FeatureID, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(fip.ID) && string.Equals(pkg.ID, fip.ID, StringComparison.OrdinalIgnoreCase) && string.Equals(pkg.Partition, fip.Partition, StringComparison.OrdinalIgnoreCase)
							select pkg).ToList<PublishingPackageInfo>();
						}
						if (list == null || !list.Any<PublishingPackageInfo>())
						{
							flag = true;
							stringBuilder.AppendFormat("\t{0} (FeatureID={1})\n", fip.ID, fip.FeatureID);
						}
					}
				}
			}
			if (flag)
			{
				throw new ImageCommonException("ImageCommon!ValidateAndLoad: The following Feature Identifier Packages specified in the FMCollectionManifest could not be found:" + stringBuilder);
			}
		}

		// Token: 0x0400013D RID: 317
		[XmlAttribute]
		public string Product;

		// Token: 0x0400013E RID: 318
		[DefaultValue(false)]
		public bool IsBuildFeatureEnabled;

		// Token: 0x0400013F RID: 319
		[XmlArrayItem(ElementName = "Language", Type = typeof(string), IsNullable = false)]
		[XmlArray]
		public List<string> SupportedLanguages = new List<string>();

		// Token: 0x04000140 RID: 320
		[XmlArrayItem(ElementName = "Locale", Type = typeof(string), IsNullable = false)]
		[XmlArray]
		public List<string> SupportedLocales = new List<string>();

		// Token: 0x04000141 RID: 321
		[XmlArrayItem(ElementName = "Resolution", Type = typeof(string), IsNullable = false)]
		[XmlArray]
		public List<string> SupportedResolutions = new List<string>();

		// Token: 0x04000142 RID: 322
		[XmlArrayItem(ElementName = "FM", Type = typeof(FMCollectionItem), IsNullable = false)]
		[XmlArray]
		public List<FMCollectionItem> FMs = new List<FMCollectionItem>();

		// Token: 0x04000143 RID: 323
		[XmlArrayItem(ElementName = "FeatureIdentifierPackage", Type = typeof(FeatureIdentifierPackage), IsNullable = false)]
		[XmlArray]
		public List<FeatureIdentifierPackage> FeatureIdentifierPackages = new List<FeatureIdentifierPackage>();

		// Token: 0x04000144 RID: 324
		public string ChunkMappingsFile;
	}
}
