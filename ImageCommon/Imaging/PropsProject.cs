using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using Microsoft.WindowsPhone.FeatureAPI;
using Microsoft.WindowsPhone.ImageUpdate.PkgCommon;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x0200002B RID: 43
	[XmlType(Namespace = "http://schemas.microsoft.com/developer/msbuild/2003")]
	[XmlRoot(ElementName = "Project", Namespace = "http://schemas.microsoft.com/developer/msbuild/2003", IsNullable = false)]
	public class PropsProject
	{
		// Token: 0x060001C9 RID: 457 RVA: 0x00004257 File Offset: 0x00002457
		public PropsProject()
		{
		}

		// Token: 0x060001CA RID: 458 RVA: 0x0000F93D File Offset: 0x0000DB3D
		public PropsProject(List<string> supportedUILanguages, List<string> supportedLocales, List<string> supportedResolutions, List<CpuId> supportedWowGuestCpuTypes, string buildType, string cpuType, string MSPackageRoot)
		{
			this._supportedUILangs = supportedUILanguages;
			this._supportedLocales = supportedLocales;
			this._supportedResolutions = supportedResolutions;
			this._supportedWowCpuTypes = supportedWowGuestCpuTypes;
			this._buildType = buildType;
			this._cpuType = cpuType;
			this._MSPackageRoot = MSPackageRoot;
		}

		// Token: 0x17000071 RID: 113
		// (get) Token: 0x060001CB RID: 459 RVA: 0x0000F97A File Offset: 0x0000DB7A
		// (set) Token: 0x060001CC RID: 460 RVA: 0x0000F982 File Offset: 0x0000DB82
		[XmlIgnore]
		public List<PropsFile> Files
		{
			get
			{
				return this.ItemGroup;
			}
			set
			{
				this.ItemGroup = value;
			}
		}

		// Token: 0x060001CD RID: 461 RVA: 0x0000F98C File Offset: 0x0000DB8C
		public void AddPackages(FeatureManifest fm)
		{
			List<FeatureManifest.FMPkgInfo> allPackagesByGroups = fm.GetAllPackagesByGroups(this._supportedUILangs, this._supportedLocales, this._supportedResolutions, this._supportedWowCpuTypes, this._buildType, this._cpuType, this._MSPackageRoot);
			if (this.Files == null)
			{
				this.Files = new List<PropsFile>();
			}
			foreach (FeatureManifest.FMPkgInfo fmpkgInfo in allPackagesByGroups)
			{
				PropsFile propFile = new PropsFile();
				string rawBasePath = fmpkgInfo.RawBasePath;
				string fileName = Path.GetFileName(fmpkgInfo.PackagePath);
				propFile.Include = this.ConvertToInclude(rawBasePath, fileName);
				if ((from prop in this.Files
				where prop.Include.Equals(propFile.Include, StringComparison.OrdinalIgnoreCase)
				select prop).Count<PropsFile>() == 0)
				{
					propFile.Feature = PropsProject.FeatureTypes.Generated_Product_Packages.ToString();
					propFile.InstallPath = this.ConvertToInstallPath(rawBasePath);
					this.SetGUID(ref propFile);
					propFile.Owner = "FeatureManifest";
					propFile.BusinessReason = "Device Imaging";
					this.Files.Add(propFile);
				}
			}
		}

		// Token: 0x060001CE RID: 462 RVA: 0x0000FAD8 File Offset: 0x0000DCD8
		private string ConvertToInstallPath(string installPath)
		{
			string originalString = installPath.Replace(Path.GetFileName(installPath), "").TrimEnd(new char[]
			{
				'\\'
			});
			originalString = originalString.Replace("$(cputype)", "$(_BuildArch)", StringComparison.OrdinalIgnoreCase);
			originalString = originalString.Replace("$(buildtype)", "$(_BuildType)", StringComparison.OrdinalIgnoreCase);
			return originalString.Replace("$(mspackageroot)", "$(WP_PACKAGES_INSTALL_PATH)", StringComparison.OrdinalIgnoreCase);
		}

		// Token: 0x060001CF RID: 463 RVA: 0x0000FB40 File Offset: 0x0000DD40
		private string ConvertToInclude(string include, string fileName)
		{
			string originalString = include.Replace(Path.GetFileName(include), fileName, StringComparison.OrdinalIgnoreCase);
			originalString = originalString.Replace("$(cputype)", "$(_BuildArch)", StringComparison.OrdinalIgnoreCase);
			originalString = originalString.Replace("$(buildtype)", "$(_BuildType)", StringComparison.OrdinalIgnoreCase);
			return originalString.Replace("$(mspackageroot)", "$(BINARY_ROOT)\\prebuilt", StringComparison.OrdinalIgnoreCase);
		}

		// Token: 0x060001D0 RID: 464 RVA: 0x0000FB98 File Offset: 0x0000DD98
		private void SetGUID(ref PropsFile propsFile)
		{
			string text = Guid.NewGuid().ToString("B");
			if (string.Equals(this._cpuType, FeatureManifest.CPUType_ARM, StringComparison.OrdinalIgnoreCase))
			{
				if (string.Equals(this._buildType, "fre", StringComparison.OrdinalIgnoreCase))
				{
					propsFile.MC_ARM_FRE = text;
					return;
				}
				propsFile.MC_ARM_CHK = text;
				return;
			}
			else if (string.Equals(this._cpuType, FeatureManifest.CPUType_X86, StringComparison.OrdinalIgnoreCase))
			{
				if (string.Equals(this._buildType, "fre", StringComparison.OrdinalIgnoreCase))
				{
					propsFile.MC_X86_FRE = text;
					return;
				}
				propsFile.MC_X86_CHK = text;
				return;
			}
			else
			{
				if (!string.Equals(this._cpuType, FeatureManifest.CPUType_ARM64, StringComparison.OrdinalIgnoreCase))
				{
					if (string.Equals(this._cpuType, FeatureManifest.CPUType_AMD64, StringComparison.OrdinalIgnoreCase))
					{
						if (string.Equals(this._buildType, "fre", StringComparison.OrdinalIgnoreCase))
						{
							propsFile.MC_AMD64_FRE = text;
							return;
						}
						propsFile.MC_AMD64_CHK = text;
					}
					return;
				}
				if (string.Equals(this._buildType, "fre", StringComparison.OrdinalIgnoreCase))
				{
					propsFile.MC_ARM64_FRE = text;
					return;
				}
				propsFile.MC_ARM64_CHK = text;
				return;
			}
		}

		// Token: 0x060001D1 RID: 465 RVA: 0x0000FC98 File Offset: 0x0000DE98
		public static PropsProject ValidateAndLoad(string xmlFile, IULogger logger)
		{
			PropsProject result = new PropsProject();
			string text = string.Empty;
			string propsProjectSchema = BuildPaths.PropsProjectSchema;
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			foreach (string text2 in executingAssembly.GetManifestResourceNames())
			{
				if (text2.Contains(propsProjectSchema))
				{
					text = text2;
					break;
				}
			}
			if (string.IsNullOrEmpty(text))
			{
				throw new ImageCommonException("ImageCommon!ValidateInput: XSD resource was not found: " + propsProjectSchema);
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
					throw new ImageCommonException("ImageCommon!ValidateInput: Unable to validate Props Project XSD.", innerException);
				}
			}
			logger.LogInfo("ImageCommon: Successfully validated the Props Project XML: {0}", new object[]
			{
				xmlFile
			});
			TextReader textReader = new StreamReader(xmlFile);
			try
			{
				result = (PropsProject)new XmlSerializer(typeof(PropsProject)).Deserialize(textReader);
			}
			catch (Exception innerException2)
			{
				throw new ImageCommonException("ImageCommon!ValidateInput: Unable to parse Props Project XML file.", innerException2);
			}
			finally
			{
				textReader.Close();
			}
			return result;
		}

		// Token: 0x060001D2 RID: 466 RVA: 0x0000FDC8 File Offset: 0x0000DFC8
		public void Merge(PropsProject sourceProps)
		{
			if (this.Files == null)
			{
				if (sourceProps != null && sourceProps.Files != null)
				{
					this.Files = sourceProps.Files;
					return;
				}
			}
			else if (sourceProps != null && sourceProps.Files != null)
			{
				this.Files.AddRange(sourceProps.Files);
			}
		}

		// Token: 0x060001D3 RID: 467 RVA: 0x0000FE08 File Offset: 0x0000E008
		public void WriteToFile(string fileName)
		{
			TextWriter textWriter = new StreamWriter(fileName);
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(PropsProject));
			try
			{
				xmlSerializer.Serialize(textWriter, this);
			}
			catch (Exception innerException)
			{
				throw new ImageCommonException("ImageCommon!WriteToFile: Unable to write Props Project XML file '" + fileName + "'", innerException);
			}
			finally
			{
				textWriter.Close();
			}
		}

		// Token: 0x04000124 RID: 292
		private List<string> _supportedUILangs;

		// Token: 0x04000125 RID: 293
		private List<string> _supportedLocales;

		// Token: 0x04000126 RID: 294
		private List<string> _supportedResolutions;

		// Token: 0x04000127 RID: 295
		private List<CpuId> _supportedWowCpuTypes;

		// Token: 0x04000128 RID: 296
		private string _buildType;

		// Token: 0x04000129 RID: 297
		private string _cpuType;

		// Token: 0x0400012A RID: 298
		private string _MSPackageRoot;

		// Token: 0x0400012B RID: 299
		[XmlArrayItem(ElementName = "File", Type = typeof(PropsFile), IsNullable = false)]
		[XmlArray]
		public List<PropsFile> ItemGroup;

		// Token: 0x0200007F RID: 127
		public enum FeatureTypes
		{
			// Token: 0x040002D0 RID: 720
			Generated_Product_Packages
		}
	}
}
