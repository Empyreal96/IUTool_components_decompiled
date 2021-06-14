using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using Microsoft.WindowsPhone.ImageUpdate.PkgCommon;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.Interfaces;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;
using Microsoft.WindowsPhone.Imaging;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents.Internal
{
	// Token: 0x02000072 RID: 114
	public class PackageGenerator : IPackageGenerator
	{
		// Token: 0x0600023D RID: 573 RVA: 0x00008880 File Offset: 0x00006A80
		private IPackageSetBuilder CreatePackageSetBuilder(CpuId cpuId, BuildType bldType, VersionInfo version, bool doCompression, bool isRazzleEnv, string spkgOutputFile = null)
		{
			if (this._buildPass == BuildPass.BuildTOC)
			{
				return new PackageTocBuilder(cpuId, bldType, version);
			}
			return new PackageSetBuilder(cpuId, bldType, version, doCompression, isRazzleEnv, spkgOutputFile);
		}

		// Token: 0x0600023E RID: 574 RVA: 0x000088A4 File Offset: 0x00006AA4
		private void BuildSpecialFiles(PackageProject proj, string projPath, string tmpDir)
		{
			if (!proj.IsBinaryPartition)
			{
				string text = null;
				if (projPath.ToLower(CultureInfo.InvariantCulture).EndsWith(".pkg.xml", true, CultureInfo.InvariantCulture))
				{
					text = projPath.Remove(projPath.Length - 8);
					text += ".policy.xml";
				}
				if (text != null && LongPathFile.Exists(text))
				{
					this._pkgSetBuilder.AddFile(SatelliteId.Neutral, new FileInfo
					{
						Type = FileType.SecurityPolicy,
						SourcePath = text,
						Attributes = (PkgConstants.c_defaultAttributes & ~FileAttributes.Compressed)
					});
				}
				else
				{
					this._macroResolver.BeginLocal();
					this._macroResolver.Register("runtime.root", string.Empty, true);
					string text2 = Path.Combine(tmpDir, "policy.xml");
					XmlDocument projectXml = proj.ToXmlDocument();
					if (this.PolicyCompiler.Compile(this._pkgSetBuilder.Name, proj.ProjectFilePath, projectXml, this._macroResolver, text2))
					{
						this._pkgSetBuilder.AddFile(SatelliteId.Neutral, new FileInfo
						{
							Type = FileType.SecurityPolicy,
							SourcePath = text2,
							Attributes = (PkgConstants.c_defaultAttributes & ~FileAttributes.Compressed)
						});
					}
					this._macroResolver.EndLocal();
				}
			}
			if (this._certFiles.Count > 0)
			{
				string text3 = Path.Combine(tmpDir, "certs.dat");
				if (CertStoreBuilder.Build(this._certFiles, text3))
				{
					this._pkgSetBuilder.AddFile(SatelliteId.Neutral, new FileInfo
					{
						Type = FileType.Certificates,
						SourcePath = text3,
						Attributes = PkgConstants.c_defaultAttributes
					});
				}
			}
			if (this._bcdStores.Count > 0)
			{
				int num = 0;
				foreach (string inputFile in this._bcdStores)
				{
					string path = string.Format("{0}_BCDStore_{1}{2}", PackageTools.BuildPackageName(proj.Owner, proj.Component, proj.SubComponent), num, PkgConstants.c_strRguExtension);
					string text4 = Path.Combine(tmpDir, path);
					string devicePath = Path.Combine(PkgConstants.c_strRguDeviceFolder, path);
					BcdConverter.ConvertBCD(inputFile, text4);
					this._pkgSetBuilder.AddFile(SatelliteId.Neutral, new FileInfo
					{
						Type = FileType.Registry,
						SourcePath = text4,
						DevicePath = devicePath,
						Attributes = PkgConstants.c_defaultAttributes
					});
					num++;
				}
			}
			this._pkgSetBuilder.AddFile(SatelliteId.Neutral, new FileInfo
			{
				Type = FileType.Invalid,
				SourcePath = null,
				DevicePath = null
			});
		}

		// Token: 0x17000093 RID: 147
		// (get) Token: 0x0600023F RID: 575 RVA: 0x00008B38 File Offset: 0x00006D38
		// (set) Token: 0x06000240 RID: 576 RVA: 0x00008B40 File Offset: 0x00006D40
		public ISecurityPolicyCompiler PolicyCompiler { get; private set; }

		// Token: 0x17000094 RID: 148
		// (get) Token: 0x06000241 RID: 577 RVA: 0x00008B49 File Offset: 0x00006D49
		public XmlValidator XmlValidator
		{
			get
			{
				return this._schemaValidator;
			}
		}

		// Token: 0x17000095 RID: 149
		// (get) Token: 0x06000242 RID: 578 RVA: 0x00008B51 File Offset: 0x00006D51
		public BuildPass BuildPass
		{
			get
			{
				return this._buildPass;
			}
		}

		// Token: 0x17000096 RID: 150
		// (get) Token: 0x06000243 RID: 579 RVA: 0x00008B59 File Offset: 0x00006D59
		public CpuId CPU
		{
			get
			{
				return this._pkgSetBuilder.CpuType;
			}
		}

		// Token: 0x17000097 RID: 151
		// (get) Token: 0x06000244 RID: 580 RVA: 0x00008B66 File Offset: 0x00006D66
		public IMacroResolver MacroResolver
		{
			get
			{
				return this._macroResolver;
			}
		}

		// Token: 0x17000098 RID: 152
		// (get) Token: 0x06000245 RID: 581 RVA: 0x00008B6E File Offset: 0x00006D6E
		public string TempDirectory
		{
			get
			{
				return this._tempDir;
			}
		}

		// Token: 0x17000099 RID: 153
		// (get) Token: 0x06000246 RID: 582 RVA: 0x00008B76 File Offset: 0x00006D76
		public string ToolPaths
		{
			get
			{
				return this._toolPaths;
			}
		}

		// Token: 0x06000247 RID: 583 RVA: 0x00008B7E File Offset: 0x00006D7E
		public IEnumerable<SatelliteId> GetSatelliteValues(SatelliteType type)
		{
			if (type == SatelliteType.Language)
			{
				return this._allLangauges;
			}
			if (type != SatelliteType.Resolution)
			{
				throw new PkgGenException("Unsupported ExpansionKey '{0}'", new object[]
				{
					type
				});
			}
			return this._allResolutions;
		}

		// Token: 0x06000248 RID: 584 RVA: 0x00008BB1 File Offset: 0x00006DB1
		public void AddRegMultiSzSegment(string keyName, string valueName, params string[] valueSegments)
		{
			this._pkgSetBuilder.AddMultiSzSegment(this._macroResolver.Resolve(keyName), this._macroResolver.Resolve(valueName), (from x in valueSegments
			select this._macroResolver.Resolve(x)).ToArray<string>());
		}

		// Token: 0x06000249 RID: 585 RVA: 0x00008BF0 File Offset: 0x00006DF0
		public void AddRegValue(string keyName, string valueName, RegValueType valueType, string value, SatelliteId satelliteId)
		{
			this._pkgSetBuilder.AddRegValue(satelliteId, new RegValueInfo
			{
				KeyName = this._macroResolver.Resolve(keyName),
				ValueName = this._macroResolver.Resolve(valueName),
				Type = valueType,
				Value = this._macroResolver.Resolve(value)
			});
		}

		// Token: 0x0600024A RID: 586 RVA: 0x00008C4D File Offset: 0x00006E4D
		public void AddRegValue(string keyName, string valueName, RegValueType valueType, string value)
		{
			this.AddRegValue(keyName, valueName, valueType, value, SatelliteId.Neutral);
		}

		// Token: 0x0600024B RID: 587 RVA: 0x00008C60 File Offset: 0x00006E60
		public void AddRegExpandValue(string keyName, string valueName, string value)
		{
			this._macroResolver.BeginLocal();
			this._macroResolver.Register("runtime.root", "%SystemDrive%", true);
			this._macroResolver.Register("runtime.windows", "%SystemRoot%", true);
			this.AddRegValue(keyName, valueName, RegValueType.ExpandString, value, SatelliteId.Neutral);
			this._macroResolver.EndLocal();
		}

		// Token: 0x0600024C RID: 588 RVA: 0x00008CBE File Offset: 0x00006EBE
		public void AddRegKey(string keyName, SatelliteId satelliteId)
		{
			this._pkgSetBuilder.AddRegValue(satelliteId, new RegValueInfo
			{
				KeyName = this._macroResolver.Resolve(keyName),
				ValueName = null
			});
		}

		// Token: 0x0600024D RID: 589 RVA: 0x00008CEA File Offset: 0x00006EEA
		public void AddRegKey(string keyName)
		{
			this.AddRegKey(keyName, SatelliteId.Neutral);
		}

		// Token: 0x0600024E RID: 590 RVA: 0x00008CF8 File Offset: 0x00006EF8
		public void AddFile(string sourcePath, string devicePath, FileAttributes attributes, SatelliteId satelliteId, string embedSignCategory)
		{
			this._pkgSetBuilder.AddFile(satelliteId, new FileInfo
			{
				Type = FileType.Regular,
				SourcePath = this._macroResolver.Resolve(sourcePath),
				DevicePath = this._macroResolver.Resolve(devicePath),
				Attributes = attributes,
				EmbeddedSigningCategory = embedSignCategory
			});
		}

		// Token: 0x0600024F RID: 591 RVA: 0x00008D51 File Offset: 0x00006F51
		public void AddFile(string sourcePath, string devicePath, FileAttributes attributes, SatelliteId satelliteId)
		{
			this.AddFile(sourcePath, devicePath, attributes, satelliteId, "None");
		}

		// Token: 0x06000250 RID: 592 RVA: 0x00008D63 File Offset: 0x00006F63
		public void AddFile(string sourcePath, string devicePath, FileAttributes attributes)
		{
			this.AddFile(sourcePath, devicePath, attributes, SatelliteId.Neutral);
		}

		// Token: 0x06000251 RID: 593 RVA: 0x00008D74 File Offset: 0x00006F74
		public void AddCertificate(string sourcePath)
		{
			string item = this._macroResolver.Resolve(sourcePath);
			this._certFiles.Add(item);
		}

		// Token: 0x06000252 RID: 594 RVA: 0x00008D9C File Offset: 0x00006F9C
		public void AddBinaryPartition(string sourcePath)
		{
			this._pkgSetBuilder.AddFile(SatelliteId.Neutral, new FileInfo
			{
				Type = FileType.BinaryPartition,
				SourcePath = this._macroResolver.Resolve(sourcePath),
				DevicePath = null,
				Attributes = PkgConstants.c_defaultAttributes
			});
		}

		// Token: 0x06000253 RID: 595 RVA: 0x00008DEC File Offset: 0x00006FEC
		public void AddBCDStore(string sourcePath)
		{
			string item = this._macroResolver.Resolve(sourcePath);
			this._bcdStores.Add(item);
		}

		// Token: 0x06000254 RID: 596 RVA: 0x00008E14 File Offset: 0x00007014
		public RegGroup ImportRegistry(string sourcePath)
		{
			try
			{
				this._schemaValidator.Validate(sourcePath);
			}
			catch (XmlSchemaValidationException ex)
			{
				throw new PkgGenProjectException(ex, sourcePath, ex.LineNumber, ex.LinePosition, ex.Message, new object[0]);
			}
			RegGroup result = null;
			try
			{
				using (XmlReader xmlReader = XmlReader.Create(sourcePath))
				{
					result = RegGroup.Load(xmlReader);
				}
			}
			catch (XmlException innerException)
			{
				throw new PkgGenProjectException(innerException, sourcePath, "Failed to import registry settings from file.", new object[0]);
			}
			return result;
		}

		// Token: 0x06000255 RID: 597 RVA: 0x00008EAC File Offset: 0x000070AC
		public void Build(string projPath, string outputDir, bool compress)
		{
			try
			{
				this._macroResolver.BeginLocal();
				PackageProject packageProject = PackageProject.Load(projPath, this._plugins, this);
				this._pkgSetBuilder.OwnerType = packageProject.OwnerType;
				this._pkgSetBuilder.ReleaseType = packageProject.ReleaseType;
				this._pkgSetBuilder.Owner = this._macroResolver.Resolve(packageProject.Owner);
				this._pkgSetBuilder.Component = this._macroResolver.Resolve(packageProject.Component);
				this._pkgSetBuilder.SubComponent = this._macroResolver.Resolve(packageProject.SubComponent);
				this._pkgSetBuilder.Platform = this._macroResolver.Resolve(packageProject.Platform);
				this._pkgSetBuilder.Partition = this._macroResolver.Resolve(packageProject.Partition);
				this._pkgSetBuilder.GroupingKey = this._macroResolver.Resolve(packageProject.GroupingKey);
				this._pkgSetBuilder.Description = this._macroResolver.Resolve(packageProject.Description);
				this._pkgSetBuilder.Resolutions.Clear();
				this._pkgSetBuilder.Resolutions.AddRange(this._allResolutions);
				this._macroResolver.Register("runtime.root", PackageTools.GetDefaultDriveLetter(packageProject.Partition), true);
				packageProject.Build(this);
				this._macroResolver.EndLocal();
				if (this._buildPass != BuildPass.BuildTOC)
				{
					this.BuildSpecialFiles(packageProject, projPath, this._tempDir);
				}
				if (!LongPathDirectory.Exists(outputDir))
				{
					LongPathDirectory.CreateDirectory(outputDir);
				}
				this._pkgSetBuilder.Save(outputDir);
			}
			catch (PkgGenProjectException)
			{
				throw;
			}
			catch (PackageException ex)
			{
				throw new PkgGenProjectException(ex, projPath, ex.Message, new object[0]);
			}
			catch (PkgGenException ex2)
			{
				throw new PkgGenProjectException(ex2, projPath, ex2.Message, new object[0]);
			}
			catch (XmlSchemaValidationException ex3)
			{
				throw new PkgGenProjectException(ex3, projPath, ex3.LineNumber, ex3.LinePosition, ex3.Message, new object[0]);
			}
			finally
			{
				FileUtils.DeleteTree(this._tempDir);
			}
		}

		// Token: 0x06000256 RID: 598 RVA: 0x00009114 File Offset: 0x00007314
		public PackageGenerator(Dictionary<string, IPkgPlugin> plugins, BuildPass buildPass, CpuId cpuId, BuildType bldType, VersionInfo version, ISecurityPolicyCompiler policyCompiler, MacroResolver macroResolver, XmlValidator schemaValidator, string languages, string resolutions, string toolPaths, bool isRazzleEnv) : this(plugins, buildPass, cpuId, bldType, version, policyCompiler, macroResolver, schemaValidator, languages, resolutions, toolPaths, isRazzleEnv, false)
		{
		}

		// Token: 0x06000257 RID: 599 RVA: 0x00009140 File Offset: 0x00007340
		public PackageGenerator(Dictionary<string, IPkgPlugin> plugins, BuildPass buildPass, CpuId cpuId, BuildType bldType, VersionInfo version, ISecurityPolicyCompiler policyCompiler, MacroResolver macroResolver, XmlValidator schemaValidator, string languages, string resolutions, string toolPaths, bool isRazzleEnv, bool doCompression) : this(plugins, buildPass, cpuId, bldType, version, policyCompiler, macroResolver, schemaValidator, languages, resolutions, toolPaths, isRazzleEnv, doCompression, null)
		{
		}

		// Token: 0x06000258 RID: 600 RVA: 0x0000916C File Offset: 0x0000736C
		public PackageGenerator(Dictionary<string, IPkgPlugin> plugins, BuildPass buildPass, CpuId cpuId, BuildType bldType, VersionInfo version, ISecurityPolicyCompiler policyCompiler, MacroResolver macroResolver, XmlValidator schemaValidator, string languages, string resolutions, string toolPaths, bool isRazzleEnv, bool doCompression, string spkgOutputFile)
		{
			this._tempDir = FileUtils.GetTempDirectory();
			this._plugins = (plugins ?? new Dictionary<string, IPkgPlugin>());
			this._buildPass = buildPass;
			this._macroResolver = macroResolver;
			this._schemaValidator = schemaValidator;
			this._pkgSetBuilder = this.CreatePackageSetBuilder(cpuId, bldType, version, doCompression, isRazzleEnv, spkgOutputFile);
			this._toolPaths = toolPaths;
			this.PolicyCompiler = policyCompiler;
			if (string.IsNullOrEmpty(languages))
			{
				this._allLangauges = new List<SatelliteId>();
			}
			else
			{
				this._allLangauges = (from x in languages.Split(new char[]
				{
					';'
				})
				select SatelliteId.Create(SatelliteType.Language, x)).ToList<SatelliteId>();
			}
			if (string.IsNullOrEmpty(resolutions))
			{
				this._allResolutions = new List<SatelliteId>();
				return;
			}
			this._allResolutions = (from x in resolutions.Split(new char[]
			{
				';'
			})
			select SatelliteId.Create(SatelliteType.Resolution, x)).ToList<SatelliteId>();
		}

		// Token: 0x04000190 RID: 400
		private BuildPass _buildPass;

		// Token: 0x04000191 RID: 401
		private List<SatelliteId> _allLangauges = new List<SatelliteId>();

		// Token: 0x04000192 RID: 402
		private List<SatelliteId> _allResolutions = new List<SatelliteId>();

		// Token: 0x04000193 RID: 403
		private MacroResolver _macroResolver;

		// Token: 0x04000194 RID: 404
		private IPackageSetBuilder _pkgSetBuilder;

		// Token: 0x04000195 RID: 405
		private XmlValidator _schemaValidator;

		// Token: 0x04000196 RID: 406
		private List<string> _certFiles = new List<string>();

		// Token: 0x04000197 RID: 407
		private List<string> _bcdStores = new List<string>();

		// Token: 0x04000198 RID: 408
		private string _tempDir;

		// Token: 0x04000199 RID: 409
		private string _toolPaths;

		// Token: 0x0400019A RID: 410
		private Dictionary<string, IPkgPlugin> _plugins;
	}
}
