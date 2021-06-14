using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.WindowsPhone.ImageUpdate.InboxAppLib.Interfaces;
using Microsoft.WindowsPhone.ImageUpdate.PkgCommon;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.Interfaces;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.ImageUpdate.InboxAppLib
{
	// Token: 0x02000006 RID: 6
	public class AppPackageAppx : IInboxAppPackage, IInboxAppToPkgObjectsMappingStrategy
	{
		// Token: 0x06000024 RID: 36 RVA: 0x00002E78 File Offset: 0x00001078
		public AppPackageAppx(InboxAppParameters parameters, bool isTopLevelPackage = true, string packageSubBaseDir = "")
		{
			if (parameters == null)
			{
				throw new ArgumentNullException("parameters", "parameters must not be null!");
			}
			this._parameters = parameters;
			this._isTopLevelPackage = isTopLevelPackage;
			this._packageSubBaseDir = packageSubBaseDir;
			if (!this._isTopLevelPackage && !InboxAppUtils.ExtensionMatches(this._parameters.PackageBasePath, ".appx"))
			{
				throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "Packages without a \"{0}\" extension are not supported.", new object[]
				{
					".appx"
				}));
			}
		}

		// Token: 0x06000025 RID: 37 RVA: 0x00002F18 File Offset: 0x00001118
		public virtual void OpenPackage()
		{
			Path.Combine(this.DecompressPackage(this._parameters.PackageBasePath, this._parameters.WorkingBaseDir), "AppxManifest.xml");
			this.ReadManifest(this._parameters.PackageBasePath, false);
			if (this._isTopLevelPackage && !string.IsNullOrWhiteSpace(this._parameters.ProvXMLBasePath))
			{
				this.ReadProvXML();
				this._provXML.DependencyHash = InboxAppUtils.CalcHash(this._parameters.PackageBasePath) + InboxAppUtils.CalcHash(this._parameters.LicenseBasePath);
			}
		}

		// Token: 0x06000026 RID: 38 RVA: 0x00002FAE File Offset: 0x000011AE
		public List<string> GetCapabilities()
		{
			return this._manifest.Capabilities;
		}

		// Token: 0x06000027 RID: 39 RVA: 0x00002FBB File Offset: 0x000011BB
		public IInboxAppManifest GetManifest()
		{
			return this._manifest;
		}

		// Token: 0x06000028 RID: 40 RVA: 0x00002FC3 File Offset: 0x000011C3
		public IInboxAppToPkgObjectsMappingStrategy GetPkgObjectsMappingStrategy()
		{
			return this;
		}

		// Token: 0x06000029 RID: 41 RVA: 0x00002FC8 File Offset: 0x000011C8
		public string GetInstallDestinationPath(bool isTopLevelPackage)
		{
			string path = string.Empty;
			string path2 = this._parameters.InfuseIntoDataPartition ? "$(runtime.data)\\Programs\\WindowsApps" : "$(runtime.windows)\\InfusedApps";
			if (!this._parameters.InfuseIntoDataPartition)
			{
				if (this._manifest.IsFramework)
				{
					path = "Frameworks";
				}
				else if (this._manifest.IsBundle || isTopLevelPackage)
				{
					path = "Applications";
				}
				else
				{
					path = "Packages";
				}
			}
			return Path.Combine(path2, path, this._manifest.PackageFullName);
		}

		// Token: 0x0600002A RID: 42 RVA: 0x00003044 File Offset: 0x00001244
		public virtual List<PkgObject> Map(IInboxAppPackage appPackage, IPkgProject packageGenerator, OSComponentBuilder osComponent)
		{
			if (osComponent == null)
			{
				throw new ArgumentNullException("osComponent", "INTERNAL ERROR:osComponent must not be null!");
			}
			FileGroupBuilder fileGroupBuilder = osComponent.AddFileGroup();
			if ((AppManifestAppxBase)appPackage.GetManifest() == null)
			{
				throw new ArgumentException("INTERNAL ERROR: appPackage is not AppManifestAppxBase or subclass!", "appPackage");
			}
			if (!this._isTopLevelPackage)
			{
				this.SetFileGroupFilter(packageGenerator, fileGroupBuilder);
			}
			this.ProcessDirectory(this._packageFilesDirectory, this._packageFilesDirectory, fileGroupBuilder, "");
			if (this._isTopLevelPackage)
			{
				string installDestinationPath = InboxAppUtils.ResolveDestinationPath(this.GetInstallDestinationPath(this._isTopLevelPackage), this._parameters.InfuseIntoDataPartition, packageGenerator);
				string licenseFileDestinationPath = InboxAppUtils.ResolveDestinationPath(this._provXML.LicenseDestinationPath, this._parameters.InfuseIntoDataPartition, packageGenerator);
				this.ValidateDependencies(packageGenerator);
				if (string.IsNullOrWhiteSpace(this._parameters.ProvXMLBasePath) || this._provXML == null)
				{
					throw new InvalidOperationException("INTERNAL ERROR: Attempting to process a provXML for an .appx that is contained within a bundle is not allowed!");
				}
				this._provXML.Update(installDestinationPath, licenseFileDestinationPath);
				string text = Path.Combine(this._parameters.WorkingBaseDir, Path.GetFileName(this._parameters.ProvXMLBasePath));
				this._provXML.Save(text);
				fileGroupBuilder.AddFile(text, Path.GetDirectoryName(this._provXML.ProvXMLDestinationPath)).SetName(Path.GetFileName(this._provXML.ProvXMLDestinationPath).RemoveSrcExtension());
				LogUtil.Message(string.Format(CultureInfo.InvariantCulture, "Appx ProvXML file added to package: Source Path \"{0}\", Destination Dir \"{1}\"", new object[]
				{
					text,
					Path.GetDirectoryName(this._provXML.ProvXMLDestinationPath)
				}));
				if (this._parameters.UpdateValue != UpdateType.UpdateNotNeeded)
				{
					fileGroupBuilder.AddFile(text, Path.GetDirectoryName(this._provXML.UpdateProvXMLDestinationPath)).SetName(Path.GetFileName(this._provXML.UpdateProvXMLDestinationPath).RemoveSrcExtension());
					LogUtil.Message(string.Format(CultureInfo.InvariantCulture, "Appx ProvXML file added to package: Source Path \"{0}\", Destination Dir \"{1}\", File \"{2}\"", new object[]
					{
						text,
						Path.GetDirectoryName(this._provXML.UpdateProvXMLDestinationPath),
						Path.GetFileName(this._provXML.UpdateProvXMLDestinationPath)
					}));
				}
				if (!this._manifest.IsFramework)
				{
					if (string.IsNullOrWhiteSpace(this._parameters.LicenseBasePath))
					{
						throw new InvalidOperationException("INTERNAL ERROR: Attempting to process a license for an .appx that is contained within a bundle is not allowed!");
					}
					string text2 = Path.Combine(this._parameters.WorkingBaseDir, Path.GetFileName(this._parameters.LicenseBasePath));
					File.Copy(this._parameters.LicenseBasePath, text2);
					string directoryName = Path.GetDirectoryName(this._provXML.LicenseDestinationPath);
					string fileName = Path.GetFileName(this._provXML.LicenseDestinationPath);
					fileGroupBuilder.AddFile(text2, directoryName).SetName(fileName);
					LogUtil.Message(string.Format(CultureInfo.InvariantCulture, "Appx license file added to package: Source Path \"{0}\", Destination Dir \"{1}\" File Name \"{2}\"", new object[]
					{
						text2,
						directoryName,
						fileName
					}));
				}
			}
			return new List<PkgObject>
			{
				osComponent.ToPkgObject()
			};
		}

		// Token: 0x0600002B RID: 43 RVA: 0x00003313 File Offset: 0x00001513
		protected string DecompressPackage(string packageBasePath, string workingBaseDir)
		{
			return this.DecompressPackageUsingAppxPackaging(packageBasePath, workingBaseDir);
		}

		// Token: 0x0600002C RID: 44 RVA: 0x00003320 File Offset: 0x00001520
		protected string DecompressPackageUsingAppxPackaging(string packageBasePath, string workingBaseDir)
		{
			this._packageFilesDirectory = Path.Combine(workingBaseDir, "Content");
			if (!this._isTopLevelPackage && !string.IsNullOrWhiteSpace(this._packageSubBaseDir))
			{
				this._packageFilesDirectory = Path.Combine(this._packageFilesDirectory, this._packageSubBaseDir);
			}
			if (Directory.Exists(this._packageFilesDirectory))
			{
				Directory.Delete(this._packageFilesDirectory, true);
			}
			string empty = string.Empty;
			int num = NativeMethods.Unpack(packageBasePath, this._packageFilesDirectory, false, IntPtr.Zero, ref empty);
			if (num != 0)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Unpack returned error {3}. One of the following fields may be empty or have an invalid value:\n(inputPackagePath)=\"{0}\" (outputDirectoryPath)=\"{1}\" (destinationDirectory)=\"{2}\"", new object[]
				{
					packageBasePath,
					this._packageFilesDirectory,
					empty,
					num
				}));
			}
			return this._packageFilesDirectory;
		}

		// Token: 0x0600002D RID: 45 RVA: 0x000033DC File Offset: 0x000015DC
		protected void ReadManifest(string packageFileBasePath, bool isBundle)
		{
			if (this._manifest == null)
			{
				this._manifest = AppManifestAppxBase.CreateAppxManifest(packageFileBasePath, string.Empty, isBundle);
				this._manifest.ReadManifest();
			}
		}

		// Token: 0x0600002E RID: 46 RVA: 0x00003403 File Offset: 0x00001603
		protected void ReadProvXML()
		{
			this._provXML = ProvXMLAppx.CreateAppxProvXML(this._parameters, this._manifest);
			this._provXML.ReadProvXML();
		}

		// Token: 0x0600002F RID: 47 RVA: 0x00003428 File Offset: 0x00001628
		private void ValidateDependencies(IPkgProject packageGenerator)
		{
			if (this._manifest.PackageDependencies.Count > 0)
			{
				foreach (PackageDependency packageDependency in this._manifest.PackageDependencies)
				{
					if (!packageDependency.IsValid())
					{
						LogUtil.Message("One or more of the PackageDependency elements in the AppX manifest is invalid (Hint: \"{0}\"). Skipping.", new object[]
						{
							packageDependency
						});
					}
					else
					{
						StringBuilder stringBuilder = new StringBuilder("$(");
						stringBuilder.Append("appxframework.");
						stringBuilder.Append(packageDependency.Name);
						stringBuilder.Append(")");
						string text = stringBuilder.ToString();
						string text2 = packageGenerator.MacroResolver.Resolve(text, MacroResolveOptions.SkipOnUnknownMacro);
						if (string.IsNullOrEmpty(text2) || text2 == text)
						{
							throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "The package dependency \"{0}\" is not registered with PkgGen. Please check pkggen.cfg.xml for a list of valid macros with ID names that start with {1}.", new object[]
							{
								packageDependency.Name,
								"appxframework."
							}));
						}
						if (!packageDependency.MeetsVersionRequirements(text2))
						{
							PackageDependency packageDependency2 = new PackageDependency(packageDependency.Name, text2);
							throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "The MinVersion specified in the application's package dependency {0} is not supported by this build of Windows Phone (which is built with {1}). Please lower the MinVersion or make sure you are using an updated Windows Phone Adaptation Kit.", new object[]
							{
								packageDependency,
								packageDependency2
							}));
						}
					}
				}
			}
		}

		// Token: 0x06000030 RID: 48 RVA: 0x00003574 File Offset: 0x00001774
		private void SetFileGroupFilter(IPkgProject packageGenerator, FileGroupBuilder fileGroupBuilder)
		{
			if (fileGroupBuilder == null)
			{
				throw new ArgumentNullException("fileGroupBuilder", "INTERNAL ERROR: fileGroupBuilder must not be null!");
			}
			if (packageGenerator == null)
			{
				throw new ArgumentNullException("packageGenerator", "INTERNAL ERROR: packageGenerator must not be null!");
			}
			if (!this._manifest.IsResource)
			{
				CpuId cpuId = CpuId.Invalid;
				if (this._manifest.ProcessorArchitecture.Equals(APPX_PACKAGE_ARCHITECTURE.APPX_PACKAGE_ARCHITECTURE_X86))
				{
					cpuId = CpuId.X86;
				}
				else if (this._manifest.ProcessorArchitecture.Equals(APPX_PACKAGE_ARCHITECTURE.APPX_PACKAGE_ARCHITECTURE_ARM))
				{
					cpuId = CpuId.ARM;
				}
				else if (this._manifest.ProcessorArchitecture.Equals(APPX_PACKAGE_ARCHITECTURE.APPX_PACKAGE_ARCHITECTURE_X64))
				{
					cpuId = CpuId.AMD64;
				}
				else if (this._manifest.ProcessorArchitecture.Equals(APPX_PACKAGE_ARCHITECTURE.APPX_PACKAGE_ARCHITECTURE_ARM64))
				{
					cpuId = CpuId.ARM64;
				}
				if (cpuId != CpuId.Invalid)
				{
					fileGroupBuilder.SetCpuId(cpuId);
					LogUtil.Message(string.Format(CultureInfo.InvariantCulture, "Setting file group filter: Cpu Id = {0}", new object[]
					{
						cpuId.ToString()
					}));
				}
				return;
			}
			if (this._manifest.Resources.Count <= 0)
			{
				LogUtil.Message(string.Format(CultureInfo.InvariantCulture, "The appx manifest {0} for bundle subpackage {1} contains no resources - skipping setting file group filters.", new object[]
				{
					this._manifest.Filename,
					this._parameters.PackageBasePath
				}));
				return;
			}
			AppManifestAppxBase.Resource resource = this._manifest.Resources[0];
			if (resource.Key.Equals("Language", StringComparison.OrdinalIgnoreCase))
			{
				string text = this.MapBCP47TagToSupportedNLSLocaleName(resource.Value, packageGenerator);
				if (!string.IsNullOrWhiteSpace(text))
				{
					LogUtil.Warning(string.Format(CultureInfo.InvariantCulture, "(appxbundle lang pack splitting feature disabled temporarily) Setting file group filter: Language = {0} ManifestLanuage = {1} for appx \"{2}\"", new object[]
					{
						text,
						resource.Value,
						this._parameters.PackageBasePath
					}));
					return;
				}
				LogUtil.Message(string.Format(CultureInfo.InvariantCulture, "NOTE: The language value {0} listed in the Appx manifest as a resource language is not a supported value - skipping mapping to PkgGen's language values.", new object[]
				{
					resource.Value
				}));
				return;
			}
			else
			{
				if (!resource.Key.Equals("Scale", StringComparison.OrdinalIgnoreCase))
				{
					resource.Key.Equals("DXFeatureLevel", StringComparison.OrdinalIgnoreCase);
					return;
				}
				string text2 = this.MapScaleToSupportedResolution(resource.Value);
				if (!string.IsNullOrWhiteSpace(text2))
				{
					LogUtil.Message(string.Format(CultureInfo.InvariantCulture, "Setting file group filter: Resolution = {0}", new object[]
					{
						text2
					}));
					return;
				}
				LogUtil.Message(string.Format(CultureInfo.InvariantCulture, "NOTE: The scale value {0} listed in the Appx manifest as a resource scale is not a supported value - skipping mapping to PkgGen's resolution values.", new object[]
				{
					resource.Value
				}));
				return;
			}
		}

		// Token: 0x06000031 RID: 49 RVA: 0x000037E8 File Offset: 0x000019E8
		protected void ProcessDirectory(string targetBaseDir, string baseBasePath, FileGroupBuilder groupBuilder, string specificSubdirectoryPattern = "")
		{
			LogUtil.Message(string.Format(CultureInfo.InvariantCulture, "Target directory to process: {0}", new object[]
			{
				targetBaseDir
			}));
			foreach (string fileBasePath in Directory.GetFiles(targetBaseDir))
			{
				this.ProcessFile(fileBasePath, baseBasePath, groupBuilder);
			}
			if (string.IsNullOrWhiteSpace(specificSubdirectoryPattern))
			{
				using (IEnumerator<string> enumerator = Directory.EnumerateDirectories(targetBaseDir).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						string text = enumerator.Current;
						LogUtil.Message(string.Format(CultureInfo.InvariantCulture, "Processing sub directory: {0}", new object[]
						{
							text
						}));
						this.ProcessDirectory(text, baseBasePath, groupBuilder, "");
					}
					return;
				}
			}
			foreach (string text2 in Directory.EnumerateDirectories(targetBaseDir, specificSubdirectoryPattern))
			{
				LogUtil.Message(string.Format(CultureInfo.InvariantCulture, "Processing specific sub directory: {0}", new object[]
				{
					text2
				}));
				this.ProcessDirectory(text2, baseBasePath, groupBuilder, "");
			}
		}

		// Token: 0x06000032 RID: 50 RVA: 0x0000390C File Offset: 0x00001B0C
		protected void ProcessFile(string fileBasePath, string baseBasePath, FileGroupBuilder groupBuilder)
		{
			if (!InboxAppUtils.ExtensionMatches(fileBasePath, ".appx"))
			{
				string path = string.Empty;
				string empty = string.Empty;
				string directoryName = Path.GetDirectoryName(fileBasePath);
				if (directoryName.StartsWith(baseBasePath, StringComparison.OrdinalIgnoreCase))
				{
					path = directoryName.Remove(0, baseBasePath.Length).TrimStart(new char[]
					{
						'\\'
					});
				}
				string text = Path.Combine(this.GetInstallDestinationPath(this._isTopLevelPackage), path);
				groupBuilder.AddFile(fileBasePath, text);
				LogUtil.Message(string.Format(CultureInfo.InvariantCulture, "AppX content file added to package: Source Path \"{0}\", Destination Dir \"{1}\"", new object[]
				{
					fileBasePath,
					text
				}));
			}
		}

		// Token: 0x06000033 RID: 51 RVA: 0x000039A0 File Offset: 0x00001BA0
		private string MapBCP47TagToSupportedNLSLocaleName(string bcp47Tag, IPkgProject packageGenerator)
		{
			string result = string.Empty;
			string text = string.Empty;
			if (packageGenerator == null)
			{
				throw new ArgumentNullException("packageGenerator", "INTERNAL ERROR: packageGenerator must not be null!");
			}
			if (string.IsNullOrWhiteSpace(bcp47Tag))
			{
				throw new ArgumentNullException("bcp47Tag", "INTERNAL ERROR: bcp47Tag must not be null or empty!");
			}
			text = InboxAppUtils.MapNeutralToSpecificCulture(bcp47Tag);
			if (string.IsNullOrWhiteSpace(text))
			{
				text = bcp47Tag.ToLowerInvariant();
			}
			foreach (SatelliteId satelliteId in packageGenerator.GetSatelliteValues(SatelliteType.Language))
			{
				if (satelliteId.Id.ToLowerInvariant() == text)
				{
					result = string.Format(CultureInfo.InvariantCulture, "({0})", new object[]
					{
						satelliteId.Id
					});
					break;
				}
			}
			return result;
		}

		// Token: 0x06000034 RID: 52 RVA: 0x00003A6C File Offset: 0x00001C6C
		private string MapScaleToSupportedResolution(string scale)
		{
			if (string.IsNullOrWhiteSpace(scale))
			{
				throw new ArgumentNullException("scale", "INTERNAL ERROR: scale must not be null or empty!");
			}
			string result = string.Empty;
			uint num = <PrivateImplementationDetails>.ComputeStringHash(scale);
			if (num <= 1864979416U)
			{
				if (num != 720139952U)
				{
					if (num != 1731450012U)
					{
						if (num != 1864979416U)
						{
							return result;
						}
						if (!(scale == "140"))
						{
							return result;
						}
						goto IL_100;
					}
					else
					{
						if (!(scale == "100"))
						{
							return result;
						}
						goto IL_100;
					}
				}
				else if (!(scale == "225"))
				{
					return result;
				}
			}
			else if (num <= 4113327457U)
			{
				if (num != 1933075630U)
				{
					if (num != 4113327457U)
					{
						return result;
					}
					if (!(scale == "150"))
					{
						return result;
					}
					return "(720x1280)";
				}
				else
				{
					if (!(scale == "120"))
					{
						return result;
					}
					goto IL_100;
				}
			}
			else if (num != 4146044052U)
			{
				if (num != 4214140266U)
				{
					return result;
				}
				if (!(scale == "160"))
				{
					return result;
				}
				return "(768x1280)";
			}
			else if (!(scale == "180"))
			{
				return result;
			}
			return "(1080x1920)";
			IL_100:
			result = "(480x800)";
			return result;
		}

		// Token: 0x06000035 RID: 53 RVA: 0x00003B80 File Offset: 0x00001D80
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "AppX package: (Title)=\"{0}\" (BasePath)=\"{1}\"", new object[]
			{
				this._manifest.Title,
				this._parameters.PackageBasePath
			});
		}

		// Token: 0x0400001C RID: 28
		protected InboxAppParameters _parameters;

		// Token: 0x0400001D RID: 29
		protected bool _isTopLevelPackage;

		// Token: 0x0400001E RID: 30
		protected string _packageSubBaseDir = string.Empty;

		// Token: 0x0400001F RID: 31
		protected AppManifestAppxBase _manifest;

		// Token: 0x04000020 RID: 32
		protected ProvXMLAppx _provXML;

		// Token: 0x04000021 RID: 33
		private readonly List<string> _capabilities = new List<string>();

		// Token: 0x04000022 RID: 34
		protected string _packageFilesDirectory = string.Empty;
	}
}
