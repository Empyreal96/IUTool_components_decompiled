using System;
using System.Collections.Generic;
using System.Globalization;
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
	// Token: 0x02000003 RID: 3
	[XmlType(Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
	[XmlRoot(ElementName = "CompDB", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate", IsNullable = false)]
	public class BSPCompDB : BuildCompDB
	{
		// Token: 0x06000007 RID: 7 RVA: 0x00002188 File Offset: 0x00000388
		public BSPCompDB()
		{
		}

		// Token: 0x06000008 RID: 8 RVA: 0x00002190 File Offset: 0x00000390
		public BSPCompDB(IULogger logger)
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

		// Token: 0x06000009 RID: 9 RVA: 0x000021C5 File Offset: 0x000003C5
		public bool ShouldSerializeOEMConditionalFeatures()
		{
			return this.OEMConditionalFeatures != null && this.OEMConditionalFeatures.Count<FMConditionalFeature>() > 0;
		}

		// Token: 0x0600000A RID: 10 RVA: 0x000021E0 File Offset: 0x000003E0
		public void GenerateBSPCompDB(string oemInputXMLFile, string fmDirectory, string msPackageRoot, string buildType, CpuId buildArch, string buildInfo)
		{
			this.Revision = "1";
			this.SchemaVersion = "1.3";
			this.BuildInfo = buildInfo;
			this.BuildArch = buildArch.ToString();
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = false;
			OEMInput oeminput = new OEMInput();
			OEMInput.ValidateInput(ref oeminput, oemInputXMLFile, this._iuLogger, msPackageRoot, buildArch.ToString());
			CompDBFeature compDBFeature = new CompDBFeature(FeatureManifest.PackageGroups.BASE.ToString(), "", CompDBFeature.CompDBFeatureTypes.MobileFeature, OwnerType.OEM.ToString());
			compDBFeature.Packages = new List<CompDBFeaturePackage>();
			foreach (string text in oeminput.AdditionalFMs)
			{
				string text2;
				if (text.ToUpper(CultureInfo.InvariantCulture).Contains("$(FMDIRECTORY)"))
				{
					text2 = Environment.ExpandEnvironmentVariables(text).ToUpper(CultureInfo.InvariantCulture).Replace("$(FMDIRECTORY)", fmDirectory, StringComparison.OrdinalIgnoreCase);
				}
				else
				{
					text2 = Path.Combine(fmDirectory, Path.GetFileName(text));
				}
				FeatureManifest featureManifest = new FeatureManifest();
				FeatureManifest.ValidateAndLoad(ref featureManifest, text2, this._iuLogger);
				featureManifest.OemInput = oeminput;
				List<string> packageFileList = featureManifest.GetPackageFileList();
				StringBuilder stringBuilder2 = new StringBuilder();
				bool flag2 = false;
				foreach (string text3 in packageFileList)
				{
					IPkgInfo pkgInfo = null;
					try
					{
						pkgInfo = Package.LoadFromCab(text3);
						CompDBFeaturePackage item = new CompDBFeaturePackage(pkgInfo.Name, false);
						compDBFeature.Packages.Add(item);
						CompDBPackageInfo item2 = new CompDBPackageInfo(pkgInfo, text3, msPackageRoot, text2, this, true, false);
						this.Packages.Add(item2);
					}
					catch (FileNotFoundException)
					{
						flag2 = true;
						stringBuilder2.AppendFormat("\t{0}\n", (pkgInfo == null) ? text3 : pkgInfo.Name);
					}
				}
				if (flag2)
				{
					flag = true;
					stringBuilder.AppendFormat("\nThe FM File '{0}' following package file(s) could not be found: \n {1}", text2, stringBuilder2.ToString());
				}
				if (featureManifest.Features != null && featureManifest.Features.OEMConditionalFeatures != null)
				{
					if (this.OEMConditionalFeatures == null)
					{
						this.OEMConditionalFeatures = new List<FMConditionalFeature>(featureManifest.Features.OEMConditionalFeatures);
					}
					else
					{
						this.OEMConditionalFeatures.AddRange(featureManifest.Features.OEMConditionalFeatures);
					}
				}
			}
			if (flag)
			{
				throw new ImageCommonException("ImageCommon::BSPCompDB!GetBSPCompDB: Errors processing FM File(s):\n" + stringBuilder.ToString());
			}
			this.Features.Add(compDBFeature);
			this.Packages = this.Packages.Distinct<CompDBPackageInfo>().ToList<CompDBPackageInfo>();
		}

		// Token: 0x0600000B RID: 11 RVA: 0x000024B4 File Offset: 0x000006B4
		public new static BSPCompDB ValidateAndLoad(string xmlFile, IULogger logger)
		{
			BSPCompDB bspcompDB = new BSPCompDB();
			string text = string.Empty;
			string bspcompDBSchema = BuildPaths.BSPCompDBSchema;
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			foreach (string text2 in executingAssembly.GetManifestResourceNames())
			{
				if (text2.Contains(bspcompDBSchema))
				{
					text = text2;
					break;
				}
			}
			if (string.IsNullOrEmpty(text))
			{
				throw new ImageCommonException("ImageCommon::BSPCompDB!ValidateAndLoad: XSD resource was not found: " + bspcompDBSchema);
			}
			TextReader textReader = new StreamReader(LongPathFile.OpenRead(xmlFile));
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(BSPCompDB));
			try
			{
				bspcompDB = (BSPCompDB)xmlSerializer.Deserialize(textReader);
			}
			catch (Exception innerException)
			{
				throw new ImageCommonException("ImageCommon::BSPCompDB!ValidateAndLoad: Unable to parse BSP CompDB XML file '" + xmlFile + "'", innerException);
			}
			finally
			{
				textReader.Close();
			}
			bool flag = "1.2".Equals(bspcompDB.SchemaVersion);
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
						throw new ImageCommonException("ImageCommon::BSPCompDB!ValidateAndLoad: Unable to validate BSP CompDB XSD for file '" + xmlFile + "'.", innerException2);
					}
					logger.LogWarning("Warning: ImageCommon::BSPCompDB!ValidateAndLoad: Unable to validate BSP CompDB XSD for file '" + xmlFile + "'.", new object[0]);
					if (string.IsNullOrEmpty(bspcompDB.SchemaVersion))
					{
						logger.LogWarning("Warning: ImageCommon::BSPCompDB!ValidateAndLoad: Schema Version was not given in BSP CompDB. Most up to date Schema Version is {1}.", new object[]
						{
							"1.3"
						});
					}
					else
					{
						logger.LogWarning("Warning: ImageCommon::BSPCompDB!ValidateAndLoad: Schema Version given in BSP CompDB ({0}) does not match most up to date Schema Version ({1}).", new object[]
						{
							bspcompDB.SchemaVersion,
							"1.3"
						});
					}
				}
			}
			logger.LogInfo("BSPCompDB: Successfully validated the BSP CompDB XML: {0}", new object[]
			{
				xmlFile
			});
			BuildCompDB parentDB = bspcompDB;
			bspcompDB.Packages = (from pkg in bspcompDB.Packages
			select pkg.SetParentDB(parentDB)).ToList<CompDBPackageInfo>();
			if (bspcompDB.ReleaseType == ReleaseType.Invalid)
			{
				if (bspcompDB.Packages.Any((CompDBPackageInfo pkg) => pkg.ReleaseType == ReleaseType.Test))
				{
					bspcompDB.ReleaseType = ReleaseType.Test;
				}
				else
				{
					bspcompDB.ReleaseType = ReleaseType.Production;
				}
			}
			return bspcompDB;
		}

		// Token: 0x0600000C RID: 12 RVA: 0x000026FC File Offset: 0x000008FC
		public override void WriteToFile(string xmlFile)
		{
			string directoryName = Path.GetDirectoryName(xmlFile);
			if (!Directory.Exists(directoryName))
			{
				Directory.CreateDirectory(directoryName);
			}
			this.SchemaVersion = "1.3";
			this.Revision = "1";
			TextWriter textWriter = new StreamWriter(LongPathFile.OpenWrite(xmlFile));
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(BSPCompDB));
			try
			{
				xmlSerializer.Serialize(textWriter, this);
			}
			catch (Exception innerException)
			{
				throw new ImageCommonException("ImageCommon::BSPCompDB!WriteToFile: Unable to write BSP CompDB XML file '" + xmlFile + "'", innerException);
			}
			finally
			{
				textWriter.Close();
			}
		}

		// Token: 0x0600000D RID: 13 RVA: 0x00002798 File Offset: 0x00000998
		public override string ToString()
		{
			return string.Concat(new string[]
			{
				"BSP DB: ",
				this.BSPProductName,
				" ",
				this.BSPVersion,
				" (",
				this.BuildInfo,
				")"
			});
		}

		// Token: 0x04000003 RID: 3
		public const string c_BSPCompDBRevision = "1";

		// Token: 0x04000004 RID: 4
		public const string c_BSPCompDBSchemaVersion = "1.3";

		// Token: 0x04000005 RID: 5
		[XmlAttribute]
		public string BSPVersion;

		// Token: 0x04000006 RID: 6
		[XmlAttribute]
		public string BSPProductName;

		// Token: 0x04000007 RID: 7
		[XmlArrayItem(ElementName = "ConditionalFeature", Type = typeof(FMConditionalFeature), IsNullable = false)]
		[XmlArray]
		public List<FMConditionalFeature> OEMConditionalFeatures;
	}
}
