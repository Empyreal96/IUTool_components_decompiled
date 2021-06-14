using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using Microsoft.Phone.Test.TestMetadata.Helper;
using Microsoft.WindowsPhone.ImageUpdate.PkgCommon;
using Microsoft.WindowsPhone.ImageUpdate.Tools;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents.Internal
{
	// Token: 0x0200006E RID: 110
	public class PackageSetBuilder : PackageSetBuilderBase, IPackageSetBuilder
	{
		// Token: 0x06000214 RID: 532 RVA: 0x00007E0F File Offset: 0x0000600F
		private bool WhetherRazzleEnv()
		{
			return this._isRazzleEnv;
		}

		// Token: 0x06000215 RID: 533 RVA: 0x00007E18 File Offset: 0x00006018
		private void SaveManifest(SatelliteId satelliteId, IEnumerable<FileInfo> files, string outputDir)
		{
			using (IPkgBuilder pkgBuilder = base.CreatePackage(satelliteId))
			{
				string filename = Path.Combine(outputDir, pkgBuilder.Name + PkgConstants.c_strPackageExtension);
				string text = pkgBuilder.Name + ".man.dsm.xml";
				CabApiWrapper.ExtractOne(filename, outputDir, "man.dsm.xml");
				text = Path.Combine(outputDir, text);
				if (LongPathFile.Exists(text))
				{
					LongPathFile.Delete(text);
				}
				LongPathFile.Move(Path.Combine(outputDir, "man.dsm.xml"), text);
				this.InsertDependencyList(text);
			}
		}

		// Token: 0x06000216 RID: 534 RVA: 0x00007EAC File Offset: 0x000060AC
		private void InsertBinaryDependency(XmlDocument manifestXml, XmlNode depNode)
		{
			string path = "pkgdep_supress.txt";
			string text = Path.Combine(LongPath.GetDirectoryName(Assembly.GetExecutingAssembly().Location), path);
			DependencySuppression dependencySuppression = null;
			if (LongPathFile.Exists(text))
			{
				dependencySuppression = new DependencySuppression(text);
			}
			else
			{
				LogUtil.Message("pkgdep_suppress.txt is missing. Ignore it and move on.");
			}
			IEnumerable<string> source = from x in this._allFiles
			select Path.GetFileName(x.Value.SourcePath);
			foreach (string text2 in this.binaryDependencySet)
			{
				if ((dependencySuppression == null || !dependencySuppression.IsFileSupressed(text2)) && !source.Contains(text2))
				{
					XmlElement xmlElement = manifestXml.CreateElement("Binary", manifestXml.DocumentElement.NamespaceURI);
					xmlElement.SetAttribute("Name", text2);
					depNode.AppendChild(xmlElement);
				}
			}
		}

		// Token: 0x06000217 RID: 535 RVA: 0x00007FA4 File Offset: 0x000061A4
		private void InsertEnvrionmentDependency(XmlDocument manifestXml, XmlNode depNode)
		{
			foreach (string value in this.environmentPathDependencySet)
			{
				XmlElement xmlElement = manifestXml.CreateElement("EnvrionmentPath", manifestXml.DocumentElement.NamespaceURI);
				xmlElement.SetAttribute("Name", value);
				depNode.AppendChild(xmlElement);
			}
		}

		// Token: 0x06000218 RID: 536 RVA: 0x0000801C File Offset: 0x0000621C
		private void InsertExplicitDependency(XmlDocument manifestXml, XmlNode depNode)
		{
			if (!string.IsNullOrEmpty(this.spkgMetaFile))
			{
				if (!LongPathFile.Exists(this.spkgMetaFile))
				{
					throw new FileNotFoundException(this.spkgMetaFile);
				}
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.Load(this.spkgMetaFile);
				XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(xmlDocument.NameTable);
				xmlNamespaceManager.AddNamespace("tm", xmlDocument.DocumentElement.NamespaceURI);
				XmlNode xmlNode = xmlDocument.SelectSingleNode("/tm:Metadata/tm:Dependencies", xmlNamespaceManager);
				if (xmlNode != null)
				{
					string pattern = "xmlns=\"(.*?)\"";
					string str = Regex.Replace(xmlNode.InnerXml, pattern, "", RegexOptions.IgnoreCase);
					depNode.InnerXml += str;
				}
			}
		}

		// Token: 0x06000219 RID: 537 RVA: 0x000080C8 File Offset: 0x000062C8
		private XmlNode CreateDependenciesNode(XmlDocument manifestXml)
		{
			XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(manifestXml.NameTable);
			xmlNamespaceManager.AddNamespace("iu", manifestXml.DocumentElement.NamespaceURI);
			string qualifiedName = "Dependencies";
			XmlNode xmlNode = manifestXml.SelectSingleNode("//iu:Package/iu:Dependencies", xmlNamespaceManager);
			if (xmlNode == null)
			{
				XmlElement newChild = manifestXml.CreateElement(qualifiedName, manifestXml.DocumentElement.NamespaceURI);
				xmlNode = manifestXml.DocumentElement.AppendChild(newChild);
			}
			else
			{
				xmlNode.RemoveAll();
			}
			return xmlNode;
		}

		// Token: 0x0600021A RID: 538 RVA: 0x00008138 File Offset: 0x00006338
		private void InsertDependencyList(string manifestFileName)
		{
			if (string.IsNullOrEmpty(manifestFileName))
			{
				throw new ArgumentNullException("manifestFileName");
			}
			if (!LongPathFile.Exists(manifestFileName))
			{
				throw new InvalidDataException(string.Format("File {0} does not exist", manifestFileName));
			}
			LogUtil.Message("Add dependency info to the manifest file...");
			try
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.Load(manifestFileName);
				XmlNode depNode = this.CreateDependenciesNode(xmlDocument);
				this.InsertBinaryDependency(xmlDocument, depNode);
				this.InsertEnvrionmentDependency(xmlDocument, depNode);
				this.InsertExplicitDependency(xmlDocument, depNode);
				xmlDocument.Save(manifestFileName);
			}
			catch (Exception ex)
			{
				LogUtil.Message("Error occurred in inserting dependencies to the manifest file.");
				LogUtil.Message(ex.Message);
				throw ex;
			}
		}

		// Token: 0x0600021B RID: 539 RVA: 0x000081D8 File Offset: 0x000063D8
		private void BuildPackage(SatelliteId satelliteId, IEnumerable<FileInfo> files, string outputDir)
		{
			using (IPkgBuilder pkgBuilder = base.CreatePackage(satelliteId))
			{
				bool flag = this.WhetherRazzleEnv();
				string text = Path.Combine(outputDir, pkgBuilder.Name + PkgConstants.c_strPackageExtension);
				string str = PkgConstants.c_strPackageExtension + PkgConstants.c_strCustomMetadataExtension;
				LogUtil.Message("Building package '{0}'", new object[]
				{
					text
				});
				foreach (FileInfo fileInfo in files)
				{
					if (fileInfo.Type != FileType.Invalid)
					{
						string text2 = fileInfo.DevicePath;
						if (text2 == null)
						{
							switch (fileInfo.Type)
							{
							case FileType.Registry:
								text2 = Path.Combine(PkgConstants.c_strRguDeviceFolder, pkgBuilder.Name + PkgConstants.c_strRguExtension);
								goto IL_163;
							case FileType.SecurityPolicy:
								text2 = Path.Combine(PkgConstants.c_strPolicyDeviceFolder, pkgBuilder.Name + PkgConstants.c_strPolicyExtension);
								goto IL_163;
							case FileType.BinaryPartition:
								text2 = "\\" + pkgBuilder.Partition + ".bin";
								goto IL_163;
							case FileType.RegistryMultiStringAppend:
								text2 = Path.Combine(PkgConstants.c_strRgaDeviceFolder, pkgBuilder.Name + PkgConstants.c_strRegAppendExtension);
								goto IL_163;
							case FileType.Certificates:
								text2 = Path.Combine(PkgConstants.c_strCertStoreDeviceFolder, pkgBuilder.Name + PkgConstants.c_strCertStoreExtension);
								goto IL_163;
							}
							throw new PkgGenException("DevicePath must be specified for file type file type {0}", new object[]
							{
								fileInfo.Type
							});
						}
						IL_163:
						LogUtil.Message("Adding file '{0}' to package '{1}' as '{2}'", new object[]
						{
							fileInfo.SourcePath,
							text,
							text2
						});
						pkgBuilder.AddFile(fileInfo.Type, fileInfo.SourcePath, text2, fileInfo.Attributes, null, fileInfo.EmbeddedSigningCategory);
						if (flag && fileInfo.Type == FileType.Regular)
						{
							string directoryName = LongPath.GetDirectoryName(text2);
							string fileName = Path.GetFileName(fileInfo.SourcePath);
							string strA = Path.GetExtension(fileName).ToLowerInvariant();
							string fileName2 = Path.GetFileName(text2);
							string strB = Path.GetFileNameWithoutExtension(text) + str;
							if (string.Compare(strA, ".dll", StringComparison.InvariantCultureIgnoreCase) == 0 || string.Compare(strA, ".exe", StringComparison.InvariantCultureIgnoreCase) == 0 || string.Compare(strA, ".sys", StringComparison.InvariantCultureIgnoreCase) == 0)
							{
								foreach (PortableExecutableDependency portableExecutableDependency in BinaryFile.GetDependency(fileInfo.SourcePath))
								{
									this.binaryDependencySet.Add(Path.GetFileName(portableExecutableDependency.Name));
								}
								if (!directoryName.StartsWith("\\Windows\\", StringComparison.InvariantCultureIgnoreCase))
								{
									this.environmentPathDependencySet.Add(directoryName);
								}
							}
							else if (string.Compare(fileName2, strB, true, CultureInfo.InvariantCulture) == 0)
							{
								LogUtil.Message("Found spkg meta file, source: '{0}', Destination: '{1}'", new object[]
								{
									fileName,
									fileName2
								});
								this.spkgMetaFile = fileInfo.SourcePath;
							}
						}
					}
				}
				pkgBuilder.SaveCab(text, this._doCompression);
				LogUtil.Message("Done package \"{0}\"", new object[]
				{
					text
				});
				if (this._spkgOutputFile != null)
				{
					LongPathFile.AppendAllText(this._spkgOutputFile, string.Format("{0}{1}", text, Environment.NewLine));
				}
			}
		}

		// Token: 0x0600021C RID: 540 RVA: 0x00008568 File Offset: 0x00006768
		public void AddRegValue(SatelliteId satelliteId, RegValueInfo valueInfo)
		{
			this._allValues.Add(new KeyValuePair<SatelliteId, RegValueInfo>(satelliteId, valueInfo));
		}

		// Token: 0x0600021D RID: 541 RVA: 0x0000857C File Offset: 0x0000677C
		public void AddMultiSzSegment(string keyName, string valueName, params string[] valueSegments)
		{
			this._rgaBuilder.AddRgaValue(keyName, valueName, valueSegments);
		}

		// Token: 0x0600021E RID: 542 RVA: 0x0000858C File Offset: 0x0000678C
		public void Save(string outputDir)
		{
			string tempDirectory = FileUtils.GetTempDirectory();
			bool flag = this.WhetherRazzleEnv();
			try
			{
				foreach (IGrouping<SatelliteId, KeyValuePair<SatelliteId, RegValueInfo>> grouping in from x in this._allValues
				group x by x.Key)
				{
					string text = Path.Combine(tempDirectory, "reg" + grouping.Key.FileSuffix + PkgConstants.c_strRguExtension);
					RegBuilder.Build(grouping.Select((KeyValuePair<SatelliteId, RegValueInfo> x) => x.Value), text);
					base.AddFile(grouping.Key, new FileInfo
					{
						Type = FileType.Registry,
						SourcePath = text,
						DevicePath = null,
						Attributes = (PkgConstants.c_defaultAttributes & ~FileAttributes.Compressed)
					});
				}
				if (this._rgaBuilder.HasContent)
				{
					string text2 = Path.Combine(tempDirectory, "regMultiSz" + PkgConstants.c_strRegAppendExtension);
					this._rgaBuilder.Save(text2);
					base.AddFile(SatelliteId.Neutral, new FileInfo
					{
						Type = FileType.RegistryMultiStringAppend,
						SourcePath = text2,
						DevicePath = null,
						Attributes = (PkgConstants.c_defaultAttributes & ~FileAttributes.Compressed)
					});
				}
				foreach (IGrouping<SatelliteId, KeyValuePair<SatelliteId, FileInfo>> grouping2 in from x in this._allFiles
				group x by x.Key)
				{
					this.BuildPackage(grouping2.Key, grouping2.Select((KeyValuePair<SatelliteId, FileInfo> x) => x.Value), outputDir);
					if (flag)
					{
						this.SaveManifest(grouping2.Key, grouping2.Select((KeyValuePair<SatelliteId, FileInfo> x) => x.Value), outputDir);
					}
				}
			}
			finally
			{
				FileUtils.DeleteTree(tempDirectory);
			}
		}

		// Token: 0x0600021F RID: 543 RVA: 0x000087F8 File Offset: 0x000069F8
		public PackageSetBuilder(CpuId cpuType, BuildType bldType, VersionInfo version, bool doCompression, bool isRazzleEnv, string spkgOutputFile) : base(cpuType, bldType, version)
		{
			this._doCompression = doCompression;
			this._isRazzleEnv = isRazzleEnv;
			this._spkgOutputFile = spkgOutputFile;
		}

		// Token: 0x06000220 RID: 544 RVA: 0x0000885D File Offset: 0x00006A5D
		public PackageSetBuilder(CpuId cpuType, BuildType bldType, VersionInfo version, bool doCompression, bool isRazzleEnv) : this(cpuType, bldType, version, doCompression, isRazzleEnv, null)
		{
		}

		// Token: 0x04000180 RID: 384
		private bool _doCompression;

		// Token: 0x04000181 RID: 385
		private List<KeyValuePair<SatelliteId, RegValueInfo>> _allValues = new List<KeyValuePair<SatelliteId, RegValueInfo>>();

		// Token: 0x04000182 RID: 386
		private RgaBuilder _rgaBuilder = new RgaBuilder();

		// Token: 0x04000183 RID: 387
		private HashSet<string> binaryDependencySet = new HashSet<string>();

		// Token: 0x04000184 RID: 388
		private HashSet<string> environmentPathDependencySet = new HashSet<string>();

		// Token: 0x04000185 RID: 389
		private string spkgMetaFile = string.Empty;

		// Token: 0x04000186 RID: 390
		private string _spkgOutputFile;

		// Token: 0x04000187 RID: 391
		private bool _isRazzleEnv;
	}
}
