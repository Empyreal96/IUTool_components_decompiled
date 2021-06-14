using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using Microsoft.WindowsPhone.FeatureAPI;
using Microsoft.WindowsPhone.ImageUpdate.PkgCommon;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;
using Microsoft.WindowsPhone.Imaging;

namespace Microsoft.WindowsPhone.CompDB
{
	// Token: 0x02000011 RID: 17
	[XmlType(Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
	[XmlRoot(ElementName = "CompDB", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate", IsNullable = false)]
	public class DeviceCompDB : BuildCompDB
	{
		// Token: 0x060000B4 RID: 180 RVA: 0x0000947D File Offset: 0x0000767D
		public bool ShouldSerializeConditionAnswers()
		{
			return this.ConditionAnswers != null && this.ConditionAnswers.GetAllConditions().Count<Condition>() > 0;
		}

		// Token: 0x060000B5 RID: 181 RVA: 0x0000949C File Offset: 0x0000769C
		public DeviceCompDB()
		{
		}

		// Token: 0x060000B6 RID: 182 RVA: 0x000094B0 File Offset: 0x000076B0
		public DeviceCompDB(DeviceCompDB srcDB) : base(srcDB)
		{
			this.BSPBuildID = srcDB.BSPBuildID;
			this.BSPBuildInfo = srcDB.BSPBuildInfo;
			if (srcDB.ConditionAnswers != null)
			{
				this.ConditionAnswers = new DeviceConditionAnswers(srcDB.ConditionAnswers);
			}
			if (srcDB.Languages != null)
			{
				this.Languages = (from lang in srcDB.Languages
				select new CompDBLanguage(lang)).ToList<CompDBLanguage>();
			}
			if (srcDB.MSConditionalFeatures != null)
			{
				this.MSConditionalFeatures = (from cf in srcDB.MSConditionalFeatures
				select new FMConditionalFeature(cf)).ToList<FMConditionalFeature>();
			}
			if (srcDB.Resolutions != null)
			{
				this.Resolutions = (from res in srcDB.Resolutions
				select new CompDBResolution(res)).ToList<CompDBResolution>();
			}
		}

		// Token: 0x060000B7 RID: 183 RVA: 0x000095B8 File Offset: 0x000077B8
		public new static DeviceCompDB ValidateAndLoad(string xmlFile, IULogger logger)
		{
			DeviceCompDB deviceCompDB = new DeviceCompDB();
			string text = string.Empty;
			string deviceCompDBSchema = BuildPaths.DeviceCompDBSchema;
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			foreach (string text2 in executingAssembly.GetManifestResourceNames())
			{
				if (text2.Contains(deviceCompDBSchema))
				{
					text = text2;
					break;
				}
			}
			if (string.IsNullOrEmpty(text))
			{
				throw new ImageCommonException("ImageCommon::DeviceCompDB!ValidateAndLoad: XSD resource was not found: " + deviceCompDBSchema);
			}
			TextReader textReader = new StreamReader(LongPathFile.OpenRead(xmlFile));
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(DeviceCompDB));
			try
			{
				deviceCompDB = (DeviceCompDB)xmlSerializer.Deserialize(textReader);
			}
			catch (Exception innerException)
			{
				throw new ImageCommonException("ImageCommon::DeviceCompDB!ValidateAndLoad: Unable to parse Device CompDB XML file '" + xmlFile + "'", innerException);
			}
			finally
			{
				textReader.Close();
			}
			bool flag = "1.2".Equals(deviceCompDB.SchemaVersion);
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
						throw new ImageCommonException("ImageCommon::DeviceCompDB!ValidateAndLoad: Unable to validate Device CompDB XSD for file '" + xmlFile + "'.", innerException2);
					}
					logger.LogWarning("ImageCommon::DeviceCompDB!ValidateAndLoad: Unable to validate Device CompDB XSD for file '" + xmlFile + "'.", new object[0]);
					if (string.IsNullOrEmpty(deviceCompDB.SchemaVersion))
					{
						logger.LogWarning("Warning: ImageCommon::DeviceCompDB!ValidateAndLoad: Schema Version was not given in Device CompDB. Most up to date Schema Version is {1}.", new object[]
						{
							"1.2"
						});
					}
					else
					{
						logger.LogWarning("Warning: ImageCommon::DeviceCompDB!ValidateAndLoad: Schema Version given in Device CompDB ({0}) does not match most up to date Schema Version ({1}).", new object[]
						{
							deviceCompDB.SchemaVersion,
							"1.2"
						});
					}
				}
			}
			logger.LogInfo("DeviceCompDB: Successfully validated the Device CompDB XML: {0}", new object[]
			{
				xmlFile
			});
			BuildCompDB parentDB = deviceCompDB;
			deviceCompDB.Packages = (from pkg in deviceCompDB.Packages
			select pkg.SetParentDB(parentDB)).ToList<CompDBPackageInfo>();
			if (deviceCompDB.ReleaseType == ReleaseType.Invalid)
			{
				if (deviceCompDB.Packages.Any((CompDBPackageInfo pkg) => pkg.ReleaseType == ReleaseType.Test))
				{
					deviceCompDB.ReleaseType = ReleaseType.Test;
				}
				else
				{
					deviceCompDB.ReleaseType = ReleaseType.Production;
				}
			}
			return deviceCompDB;
		}

		// Token: 0x060000B8 RID: 184 RVA: 0x00009800 File Offset: 0x00007A00
		public override void WriteToFile(string xmlFile)
		{
			string directoryName = Path.GetDirectoryName(xmlFile);
			if (!Directory.Exists(directoryName))
			{
				Directory.CreateDirectory(directoryName);
			}
			foreach (CompDBPackageInfo compDBPackageInfo in this.Packages)
			{
				compDBPackageInfo.Payload.RemoveAll((CompDBPayloadInfo pay) => true);
			}
			this.SchemaVersion = "1.2";
			this.Revision = "1";
			TextWriter textWriter = new StreamWriter(LongPathFile.OpenWrite(xmlFile));
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(DeviceCompDB));
			try
			{
				xmlSerializer.Serialize(textWriter, this);
			}
			catch (Exception innerException)
			{
				throw new ImageCommonException("ImageCommon::DeviceCompDB!WriteToFile: Unable to write Device CompDB XML file '" + xmlFile + "'", innerException);
			}
			finally
			{
				textWriter.Close();
			}
		}

		// Token: 0x060000B9 RID: 185 RVA: 0x00009900 File Offset: 0x00007B00
		public static DeviceCompDB CreateDBFromPackages(List<IPkgInfo> packages, List<Hashtable> registryTable, string targetDBFile, IULogger logger)
		{
			DeviceCompDB.<>c__DisplayClass12_1 CS$<>8__locals1 = new DeviceCompDB.<>c__DisplayClass12_1();
			Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
			DeviceCompDB deviceCompDB = new DeviceCompDB();
			deviceCompDB.BuildArch = (from pkg in packages
			group pkg by pkg.CpuType into g
			orderby g.Count<IPkgInfo>() descending
			select g.Key).First<CpuId>().ToString();
			deviceCompDB.Features = new List<CompDBFeature>();
			List<CompDBPackageInfo> list = new List<CompDBPackageInfo>();
			deviceCompDB.Resolutions = (from res in (from pkg in packages
			where !string.IsNullOrEmpty(pkg.Resolution)
			select pkg.Resolution).Distinct<string>()
			select new CompDBResolution(res)).ToList<CompDBResolution>();
			List<string> source = (from res in deviceCompDB.Resolutions
			select res.Id).ToList<string>();
			foreach (IPkgInfo pkgInfo in packages)
			{
				string text = string.Concat(new string[]
				{
					FileUtils.GetTempFile(),
					".",
					pkgInfo.Name,
					".",
					PkgConstants.c_strMumFile
				});
				string mumDevicePath = "\\" + PkgConstants.c_strMumDeviceFolder + "\\" + PkgConstants.c_strMumFile;
				bool flag = false;
				try
				{
					pkgInfo.ExtractFile(mumDevicePath, text, true);
					flag = true;
				}
				catch
				{
				}
				if (!flag)
				{
					IFileEntry fileEntry = pkgInfo.Files.FirstOrDefault((IFileEntry file) => file.DevicePath.Equals(mumDevicePath, StringComparison.OrdinalIgnoreCase));
					if (fileEntry != null)
					{
						string text2 = (fileEntry as FileEntry).SourcePath;
						text2 = Path.GetDirectoryName(text2);
						string text3 = string.IsNullOrEmpty(pkgInfo.Culture) ? "" : pkgInfo.Culture;
						string str = string.Format("{0}~{1}~{2}~{3}~{4}", new object[]
						{
							pkgInfo.Name,
							pkgInfo.PublicKey,
							pkgInfo.CpuType,
							text3,
							pkgInfo.Version.ToString()
						});
						text2 = Path.Combine(text2, str + ".mum");
						LongPathFile.Copy(text2, text, true);
					}
				}
				string featureInfoXML = PrepCBSFeature.GetFeatureInfoXML(text);
				File.Delete(text);
				if (!string.IsNullOrEmpty(featureInfoXML))
				{
					string fmID;
					string featureID;
					string group;
					string text4;
					List<FeatureManifest.FMPkgInfo> list2;
					PrepCBSFeature.ParseFeatureInfoXML(featureInfoXML, out fmID, out featureID, out group, out text4, out list2);
					CompDBFeature compDBFeature = new CompDBFeature(featureID, fmID, CompDBFeature.CompDBFeatureTypes.MobileFeature, group);
					compDBFeature.Packages = new List<CompDBFeaturePackage>();
					foreach (FeatureManifest.FMPkgInfo fmpkgInfo in list2)
					{
						CompDBFeaturePackage item = new CompDBFeaturePackage(fmpkgInfo.ID, pkgInfo.Name.Equals(fmpkgInfo.ID, StringComparison.OrdinalIgnoreCase));
						if (!string.IsNullOrEmpty(fmpkgInfo.Language))
						{
							if (!dictionary.ContainsKey(fmpkgInfo.Language))
							{
								dictionary[fmpkgInfo.Language] = new List<string>();
							}
							dictionary[fmpkgInfo.Language].Add(fmpkgInfo.ID);
						}
						else if (!string.IsNullOrEmpty(fmpkgInfo.Resolution) && !source.Contains(fmpkgInfo.Resolution, StringComparer.OrdinalIgnoreCase))
						{
							continue;
						}
						compDBFeature.Packages.Add(item);
					}
					deviceCompDB.Features.Add(compDBFeature);
				}
				CompDBPackageInfo item2 = new CompDBPackageInfo(pkgInfo, null, null, null, deviceCompDB, false, false);
				list.Add(item2);
			}
			CS$<>8__locals1.langModelPackageIDs = (from pkg in (from feat in deviceCompDB.Features
			where feat.FeatureID.StartsWith(FeatureManifest.PackageGroups.KEYBOARD.ToString() + "_", StringComparison.OrdinalIgnoreCase) || feat.FeatureID.StartsWith(FeatureManifest.PackageGroups.SPEECH.ToString() + "_", StringComparison.OrdinalIgnoreCase)
			select feat).SelectMany((CompDBFeature feat) => feat.Packages)
			select pkg.ID).ToList<string>();
			IEnumerable<CompDBPackageInfo> source2 = list;
			Func<CompDBPackageInfo, bool> predicate;
			if ((predicate = CS$<>8__locals1.<>9__11) == null)
			{
				DeviceCompDB.<>c__DisplayClass12_1 CS$<>8__locals3 = CS$<>8__locals1;
				predicate = (CS$<>8__locals3.<>9__11 = ((CompDBPackageInfo pkg) => CS$<>8__locals3.langModelPackageIDs.Contains(pkg.ID, StringComparer.OrdinalIgnoreCase)));
			}
			foreach (CompDBPackageInfo compDBPackageInfo in source2.Where(predicate))
			{
				compDBPackageInfo.SatelliteType = CompDBPackageInfo.SatelliteTypes.LangModel;
			}
			CS$<>8__locals1.featurePackageIDs = (from pkg in deviceCompDB.Features.SelectMany((CompDBFeature feat) => feat.Packages)
			select pkg.ID).ToList<string>();
			deviceCompDB.Packages = (from pkg in list
			where CS$<>8__locals1.featurePackageIDs.Contains(pkg.ID, StringComparer.OrdinalIgnoreCase)
			select pkg).ToList<CompDBPackageInfo>();
			deviceCompDB.Languages = (from lang in (from pkg in deviceCompDB.Packages
			where pkg.SatelliteType == CompDBPackageInfo.SatelliteTypes.Language
			select pkg.SatelliteValue).Distinct<string>()
			select new CompDBLanguage(lang)).ToList<CompDBLanguage>();
			CS$<>8__locals1.languages = (from lang in deviceCompDB.Languages
			select lang.Id).ToList<string>();
			CS$<>8__locals1.removeLanguagePackageIDs = (from pair in dictionary
			where !CS$<>8__locals1.languages.Contains(pair.Key, StringComparer.OrdinalIgnoreCase)
			select pair).SelectMany((KeyValuePair<string, List<string>> pair) => from id in pair.Value
			select id).ToList<string>();
			foreach (CompDBFeature compDBFeature2 in deviceCompDB.Features)
			{
				List<CompDBFeaturePackage> packages2 = compDBFeature2.Packages;
				Predicate<CompDBFeaturePackage> match;
				if ((match = CS$<>8__locals1.<>9__22) == null)
				{
					DeviceCompDB.<>c__DisplayClass12_1 CS$<>8__locals4 = CS$<>8__locals1;
					match = (CS$<>8__locals4.<>9__22 = ((CompDBFeaturePackage pkg) => CS$<>8__locals4.removeLanguagePackageIDs.Contains(pkg.ID, StringComparer.OrdinalIgnoreCase)));
				}
				packages2.RemoveAll(match);
			}
			if (!string.IsNullOrEmpty(targetDBFile) && LongPathFile.Exists(targetDBFile))
			{
				BuildCompDB buildCompDB = BuildCompDB.LoadCompDB(targetDBFile, logger);
				if (buildCompDB.MSConditionalFeatures.SelectMany((FMConditionalFeature feat) => feat.GetAllConditions()).ToList<Condition>().Any((Condition cond) => cond.Type != Condition.ConditionType.Feature))
				{
					deviceCompDB.ConditionAnswers = new DeviceConditionAnswers(logger);
					deviceCompDB.ConditionAnswers.PopulateConditionAnswers(buildCompDB.MSConditionalFeatures, registryTable);
				}
			}
			CS$<>8__locals1.fmDevicePath = "\\" + DevicePaths.MSFMPath;
			IPkgInfo pkgInfo2 = packages.Where(delegate(IPkgInfo pkg)
			{
				IEnumerable<IFileEntry> files = pkg.Files;
				Func<IFileEntry, bool> predicate2;
				if ((predicate2 = CS$<>8__locals1.<>9__26) == null)
				{
					predicate2 = (CS$<>8__locals1.<>9__26 = ((IFileEntry fi) => fi.DevicePath.StartsWith(CS$<>8__locals1.fmDevicePath, StringComparison.OrdinalIgnoreCase)));
				}
				return files.Any(predicate2);
			}).FirstOrDefault<IPkgInfo>();
			if (pkgInfo2 != null)
			{
				IFileEntry fileEntry2 = pkgInfo2.Files.FirstOrDefault((IFileEntry fi) => fi.DevicePath.StartsWith(CS$<>8__locals1.fmDevicePath, StringComparison.OrdinalIgnoreCase));
				if (fileEntry2 != null)
				{
					string text5 = FileUtils.GetTempFile() + "." + Path.GetFileName(fileEntry2.DevicePath);
					pkgInfo2.ExtractFile(fileEntry2.DevicePath, text5, true);
					FeatureManifest featureManifest = new FeatureManifest();
					FeatureManifest.ValidateAndLoad(ref featureManifest, text5, logger);
					File.Delete(text5);
					if (!string.IsNullOrEmpty(featureManifest.BuildID))
					{
						DeviceCompDB deviceCompDB2 = deviceCompDB;
						deviceCompDB2.BuildID = (deviceCompDB2.BSPBuildID = new Guid(featureManifest.BuildID));
					}
					if (!string.IsNullOrEmpty(featureManifest.BuildInfo))
					{
						DeviceCompDB deviceCompDB3 = deviceCompDB;
						deviceCompDB3.BuildInfo = (deviceCompDB3.BSPBuildInfo = featureManifest.BuildInfo);
					}
				}
			}
			return deviceCompDB;
		}

		// Token: 0x060000BA RID: 186 RVA: 0x0000A1B8 File Offset: 0x000083B8
		public override string ToString()
		{
			return "Device DB: " + this.BSPBuildInfo;
		}

		// Token: 0x0400009A RID: 154
		public const string c_DeviceCompDBRevision = "1";

		// Token: 0x0400009B RID: 155
		public const string c_DeviceCompDBSchemaVersion = "1.2";

		// Token: 0x0400009C RID: 156
		[XmlAttribute]
		public Guid BSPBuildID;

		// Token: 0x0400009D RID: 157
		[XmlAttribute]
		public string BSPBuildInfo;

		// Token: 0x0400009E RID: 158
		[XmlArrayItem(ElementName = "Language", Type = typeof(CompDBLanguage), IsNullable = false)]
		[XmlArray]
		[DefaultValue(null)]
		public List<CompDBLanguage> Languages;

		// Token: 0x0400009F RID: 159
		[XmlArrayItem(ElementName = "Resolution", Type = typeof(CompDBResolution), IsNullable = false)]
		[XmlArray]
		[DefaultValue(null)]
		public List<CompDBResolution> Resolutions;

		// Token: 0x040000A0 RID: 160
		[XmlElement("ConditionAnswers")]
		public DeviceConditionAnswers ConditionAnswers = new DeviceConditionAnswers();
	}
}
