using System;
using System.Collections.Generic;
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
	// Token: 0x02000016 RID: 22
	[XmlType(Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
	[XmlRoot(ElementName = "CompDB", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate", IsNullable = false)]
	public class UpdateCompDB : BuildCompDB
	{
		// Token: 0x060000D2 RID: 210 RVA: 0x00002188 File Offset: 0x00000388
		public UpdateCompDB()
		{
		}

		// Token: 0x060000D3 RID: 211 RVA: 0x0000B187 File Offset: 0x00009387
		public UpdateCompDB(UpdateCompDB srcDB) : base(srcDB)
		{
			this.TargetBuildID = srcDB.TargetBuildID;
			this.TargetBuildInfo = srcDB.TargetBuildInfo;
			this.TargetOSVersion = srcDB.TargetOSVersion;
		}

		// Token: 0x060000D4 RID: 212 RVA: 0x0000B1B4 File Offset: 0x000093B4
		public UpdateCompDB(BuildCompDB srcBuild, BuildCompDB tgtBuild, IULogger logger)
		{
			UpdateCompDB.<>c__DisplayClass7_0 CS$<>8__locals1 = new UpdateCompDB.<>c__DisplayClass7_0();
			CS$<>8__locals1.<>4__this = this;
			this._iuLogger = logger;
			if (string.IsNullOrEmpty(srcBuild.Product) != string.IsNullOrEmpty(tgtBuild.Product))
			{
				throw new ImageCommonException("ImageCommon::UpdateCompDB!UpdateCompDB: The source and target DBs must be for the same Product: src=" + (string.IsNullOrEmpty(srcBuild.Product) ? "null" : srcBuild.Product) + " tgt=" + (string.IsNullOrEmpty(tgtBuild.Product) ? "null" : tgtBuild.Product));
			}
			if (!srcBuild.Product.Equals(tgtBuild.Product))
			{
				throw new ImageCommonException("ImageCommon::UpdateCompDB!UpdateCompDB: The source and target DBs must be for the same Product: src=" + srcBuild.Product + " tgt=" + tgtBuild.Product);
			}
			if (srcBuild.ReleaseType != tgtBuild.ReleaseType)
			{
				throw new ImageCommonException("ImageCommon::UpdateCompDB!UpdateCompDB: The source and target DBs must have the same Release Type: src=" + srcBuild.ReleaseType.ToString() + " tgt=" + tgtBuild.ReleaseType.ToString());
			}
			this.TargetBuildID = tgtBuild.BuildID;
			this.BuildID = srcBuild.BuildID;
			this.BuildArch = srcBuild.BuildArch;
			this.TargetBuildInfo = tgtBuild.BuildInfo;
			this.BuildInfo = srcBuild.BuildInfo;
			this.TargetOSVersion = tgtBuild.OSVersion;
			this.OSVersion = srcBuild.OSVersion;
			this.Revision = "1";
			this.SchemaVersion = "1.2";
			this.Packages.AddRange(srcBuild.Packages.Intersect(tgtBuild.Packages, CompDBPackageInfoComparer.IgnorePackageHash));
			List<CompDBPackageInfo> collection = (from pkg in srcBuild.Packages
			where pkg.Payload.Count<CompDBPayloadInfo>() == 1
			select pkg into srcPkg
			join destPkg in 
				from pkg in tgtBuild.Packages
				where pkg.Payload.Count<CompDBPayloadInfo>() == 1
				select pkg on srcPkg.ID equals destPkg.ID
			where !destPkg.FirstPayloadItem.Path.Equals(srcPkg.FirstPayloadItem.Path, StringComparison.OrdinalIgnoreCase) && destPkg.Equals(srcPkg, CompDBPackageInfo.CompDBPackageInfoComparison.IgnorePayloadPaths)
			select destPkg.SetPreviousPath(srcPkg.FirstPayloadItem.Path)).ToList<CompDBPackageInfo>();
			this.Packages.AddRange(collection);
			this.Packages = (from pkg in this.Packages
			select new CompDBPackageInfo(pkg).ClearPackageHashes()).ToList<CompDBPackageInfo>();
			this.Packages = (from pkg in this.Packages
			select pkg.ClearSkipForPublishing()).ToList<CompDBPackageInfo>();
			this.Packages = (from pkg in this.Packages
			select pkg.ClearSkipForPRSSigning()).ToList<CompDBPackageInfo>();
			this.Packages = (from pkg in this.Packages
			select pkg.SetPayloadType(CompDBPayloadInfo.PayloadTypes.Diff)).ToList<CompDBPackageInfo>();
			List<FMConditionalFeature> source = (from feat in tgtBuild.MSConditionalFeatures
			where feat.UpdateAction == FeatureCondition.Action.NoUpdate
			select feat).ToList<FMConditionalFeature>();
			CS$<>8__locals1.noUpdateFeatureIDs = (from feat in source
			select feat.FeatureIDWithFMID).ToList<string>();
			List<CompDBFeature> list = (from feat in tgtBuild.Features
			select new CompDBFeature(feat)).ToList<CompDBFeature>();
			list.RemoveAll((CompDBFeature feat) => CS$<>8__locals1.noUpdateFeatureIDs.Contains(feat.FeatureIDWithFMID, StringComparer.OrdinalIgnoreCase));
			using (List<CompDBFeature>.Enumerator enumerator = srcBuild.Features.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					UpdateCompDB.<>c__DisplayClass7_3 CS$<>8__locals2 = new UpdateCompDB.<>c__DisplayClass7_3();
					CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
					CS$<>8__locals2.srcFeature = enumerator.Current;
					UpdateCompDB.<>c__DisplayClass7_1 CS$<>8__locals3 = new UpdateCompDB.<>c__DisplayClass7_1();
					CS$<>8__locals3.CS$<>8__locals2 = CS$<>8__locals2;
					UpdateCompDB.<>c__DisplayClass7_1 CS$<>8__locals4 = CS$<>8__locals3;
					CS$<>8__locals4.srcFeaturePackages = new List<CompDBFeaturePackage>(CS$<>8__locals4.CS$<>8__locals2.srcFeature.Packages);
					UpdateCompDB.<>c__DisplayClass7_1 CS$<>8__locals5 = CS$<>8__locals3;
					CS$<>8__locals5.newFeature = new CompDBFeature(CS$<>8__locals5.CS$<>8__locals2.srcFeature.FeatureID, CS$<>8__locals3.CS$<>8__locals2.srcFeature.FMID, CS$<>8__locals3.CS$<>8__locals2.srcFeature.Type, CS$<>8__locals3.CS$<>8__locals2.srcFeature.Group);
					if (source.FirstOrDefault((FMConditionalFeature noUpdate) => noUpdate.FeatureIDWithFMID.Equals(CS$<>8__locals3.newFeature.FeatureIDWithFMID, StringComparison.OrdinalIgnoreCase)) != null)
					{
						CS$<>8__locals3.newFeature.Packages = (from pkg in CS$<>8__locals3.srcFeaturePackages
						select pkg.SetUpdateType(CompDBFeaturePackage.UpdateTypes.NoUpdate)).ToList<CompDBFeaturePackage>();
						using (List<CompDBFeaturePackage>.Enumerator enumerator2 = CS$<>8__locals3.newFeature.Packages.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								CompDBFeaturePackage featurePkg = enumerator2.Current;
								this.Packages.RemoveAll((CompDBPackageInfo pkg) => pkg.ID.Equals(featurePkg.ID, StringComparison.OrdinalIgnoreCase));
							}
						}
						this.Features.Add(CS$<>8__locals3.newFeature);
					}
					else
					{
						CompDBFeature compDBFeature = list.FirstOrDefault((CompDBFeature ftr) => ftr.FeatureIDWithFMID.Equals(CS$<>8__locals3.CS$<>8__locals2.srcFeature.FeatureIDWithFMID, StringComparison.OrdinalIgnoreCase));
						if (compDBFeature == null)
						{
							CS$<>8__locals3.newFeature.Packages = (from pkg in CS$<>8__locals3.srcFeaturePackages
							select pkg.SetUpdateType(CompDBFeaturePackage.UpdateTypes.Removal)).ToList<CompDBFeaturePackage>();
						}
						else
						{
							CS$<>8__locals3.newFeature.Packages = new List<CompDBFeaturePackage>();
							List<CompDBFeaturePackage> tgtFeaturePackages = new List<CompDBFeaturePackage>(compDBFeature.Packages);
							List<CompDBFeaturePackage> source2 = (from pkg in CS$<>8__locals3.srcFeaturePackages
							where (from pkg2 in tgtFeaturePackages
							select pkg2.ID).Contains(pkg.ID, CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.IgnoreCase)
							select pkg).ToList<CompDBFeaturePackage>();
							CS$<>8__locals3.newFeature.Packages.AddRange(from pkg in source2
							select pkg.SetUpdateType(CompDBFeaturePackage.UpdateTypes.Diff));
							List<CompDBFeaturePackage> source3 = (from pkg in CS$<>8__locals3.srcFeaturePackages
							where !(from pkg2 in tgtFeaturePackages
							select pkg2.ID).Contains(pkg.ID, CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.IgnoreCase)
							select pkg).ToList<CompDBFeaturePackage>();
							CS$<>8__locals3.newFeature.Packages.AddRange(from pkg in source3
							select pkg.SetUpdateType(CompDBFeaturePackage.UpdateTypes.Removal));
							List<CompDBFeaturePackage> collection2 = (from pkg in tgtFeaturePackages
							where !(from pkg2 in CS$<>8__locals3.srcFeaturePackages
							select pkg2.ID).Contains(pkg.ID, CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.IgnoreCase)
							select pkg).ToList<CompDBFeaturePackage>();
							CS$<>8__locals3.newFeature.Packages.AddRange(collection2);
						}
						this.Features.Add(CS$<>8__locals3.newFeature);
					}
				}
			}
		}

		// Token: 0x060000D5 RID: 213 RVA: 0x0000B934 File Offset: 0x00009B34
		public new static UpdateCompDB ValidateAndLoad(string xmlFile, IULogger logger)
		{
			UpdateCompDB updateCompDB = new UpdateCompDB();
			string text = string.Empty;
			string updateCompDBSchema = BuildPaths.UpdateCompDBSchema;
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			foreach (string text2 in executingAssembly.GetManifestResourceNames())
			{
				if (text2.Contains(updateCompDBSchema))
				{
					text = text2;
					break;
				}
			}
			if (string.IsNullOrEmpty(text))
			{
				throw new ImageCommonException("ImageCommon::UpdateCompDB!ValidateAndLoad: XSD resource was not found: " + updateCompDBSchema);
			}
			TextReader textReader = new StreamReader(LongPathFile.OpenRead(xmlFile));
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(UpdateCompDB));
			try
			{
				updateCompDB = (UpdateCompDB)xmlSerializer.Deserialize(textReader);
			}
			catch (Exception innerException)
			{
				throw new ImageCommonException("ImageCommon::UpdateCompDB!ValidateAndLoad: Unable to parse Update CompDB XML file '" + xmlFile + "'", innerException);
			}
			finally
			{
				textReader.Close();
			}
			bool flag = "1.2".Equals(updateCompDB.SchemaVersion);
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
						throw new ImageCommonException("ImageCommon::UpdateCompDB!ValidateAndLoad: Unable to validate Update CompDB XSD for file '" + xmlFile + "'.", innerException2);
					}
					logger.LogWarning("Warning: ImageCommon::UpdateCompDB!ValidateAndLoad: Unable to validate Update CompDB XSD for file '" + xmlFile + "'.", new object[0]);
					if (string.IsNullOrEmpty(updateCompDB.SchemaVersion))
					{
						logger.LogWarning("Warning: ImageCommon::UpdateCompDB!ValidateAndLoad: Schema Version was not given in Update CompDB. Most up to date Schema Version is {1}.", new object[]
						{
							"1.2"
						});
					}
					else
					{
						logger.LogWarning("Warning: ImageCommon::UpdateCompDB!ValidateAndLoad: Schema Version given in Update CompDB ({0}) does not match most up to date Schema Version ({1}).", new object[]
						{
							updateCompDB.SchemaVersion,
							"1.2"
						});
					}
				}
			}
			logger.LogInfo("UpdateCompDB: Successfully validated the Update CompDB XML: {0}", new object[]
			{
				xmlFile
			});
			BuildCompDB parentDB = updateCompDB;
			updateCompDB.Packages = (from pkg in updateCompDB.Packages
			select pkg.SetParentDB(parentDB)).ToList<CompDBPackageInfo>();
			if (updateCompDB.ReleaseType == ReleaseType.Invalid)
			{
				if (updateCompDB.Packages.Any((CompDBPackageInfo pkg) => pkg.ReleaseType == ReleaseType.Test))
				{
					updateCompDB.ReleaseType = ReleaseType.Test;
				}
				else
				{
					updateCompDB.ReleaseType = ReleaseType.Production;
				}
			}
			return updateCompDB;
		}

		// Token: 0x060000D6 RID: 214 RVA: 0x0000BB7C File Offset: 0x00009D7C
		public override void WriteToFile(string xmlFile)
		{
			string directoryName = Path.GetDirectoryName(xmlFile);
			if (!Directory.Exists(directoryName))
			{
				Directory.CreateDirectory(directoryName);
			}
			this.SchemaVersion = "1.2";
			this.Revision = "1";
			TextWriter textWriter = new StreamWriter(LongPathFile.OpenWrite(xmlFile));
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(UpdateCompDB));
			try
			{
				xmlSerializer.Serialize(textWriter, this);
			}
			catch (Exception innerException)
			{
				throw new ImageCommonException("ImageCommon::UpdateCompDB!WriteToFile: Unable to write Update CompDB XML file '" + xmlFile + "'", innerException);
			}
			finally
			{
				textWriter.Close();
			}
		}

		// Token: 0x060000D7 RID: 215 RVA: 0x0000BC18 File Offset: 0x00009E18
		public override string ToString()
		{
			return "Update DB: " + this.BuildInfo + " to " + this.TargetBuildInfo;
		}

		// Token: 0x040000B5 RID: 181
		public const string c_UpdateCompDBRevision = "1";

		// Token: 0x040000B6 RID: 182
		public const string c_UpdateCompDBSchemaVersion = "1.2";

		// Token: 0x040000B7 RID: 183
		[XmlAttribute]
		public Guid TargetBuildID;

		// Token: 0x040000B8 RID: 184
		[XmlAttribute]
		public string TargetBuildInfo;

		// Token: 0x040000B9 RID: 185
		[XmlAttribute]
		public string TargetOSVersion;
	}
}
