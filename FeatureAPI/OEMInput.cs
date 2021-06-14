using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.FeatureAPI
{
	// Token: 0x02000003 RID: 3
	[XmlType(Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
	[XmlRoot(ElementName = "OEMInput", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate", IsNullable = false)]
	public class OEMInput
	{
		// Token: 0x06000006 RID: 6 RVA: 0x000020A7 File Offset: 0x000002A7
		public OEMInput()
		{
		}

		// Token: 0x06000007 RID: 7 RVA: 0x000020BC File Offset: 0x000002BC
		public OEMInput(OEMInput srcOEMInput)
		{
			if (srcOEMInput == null)
			{
				return;
			}
			this._msPackageRoot = srcOEMInput._msPackageRoot;
			this._product = srcOEMInput._product;
			if (srcOEMInput.AdditionalFMs != null)
			{
				this.AdditionalFMs = new List<string>();
				this.AdditionalFMs.AddRange(srcOEMInput.AdditionalFMs);
			}
			this.BootLocale = srcOEMInput.BootLocale;
			this.BootUILanguage = srcOEMInput.BootUILanguage;
			this.BuildType = srcOEMInput.BuildType;
			this.CPUType = srcOEMInput.CPUType;
			this.Description = srcOEMInput.Description;
			this.Device = srcOEMInput.Device;
			this.ExcludePrereleaseFeatures = srcOEMInput.ExcludePrereleaseFeatures;
			if (srcOEMInput.Features != null)
			{
				this.Features = new OEMInputFeatures();
				if (srcOEMInput.Features.Microsoft != null)
				{
					this.Features.Microsoft = new List<string>();
					this.Features.Microsoft.AddRange(srcOEMInput.Features.Microsoft);
				}
				if (srcOEMInput.Features.OEM != null)
				{
					this.Features.OEM = new List<string>();
					this.Features.OEM.AddRange(srcOEMInput.Features.OEM);
				}
			}
			this.FormatDPP = srcOEMInput.FormatDPP;
			if (srcOEMInput.PackageFiles != null)
			{
				this.PackageFiles = new List<string>();
				this.PackageFiles.AddRange(srcOEMInput.PackageFiles);
			}
			this.ReleaseType = srcOEMInput.ReleaseType;
			if (srcOEMInput.Resolutions != null)
			{
				this.Resolutions = new List<string>();
				this.Resolutions.AddRange(srcOEMInput.Resolutions);
			}
			this.SOC = srcOEMInput.SOC;
			if (srcOEMInput.SupportedLanguages != null)
			{
				this.SupportedLanguages = new SupportedLangs();
				if (srcOEMInput.SupportedLanguages.UserInterface != null)
				{
					this.SupportedLanguages.UserInterface = new List<string>();
					this.SupportedLanguages.UserInterface.AddRange(srcOEMInput.SupportedLanguages.UserInterface);
				}
				if (srcOEMInput.SupportedLanguages.Keyboard != null)
				{
					this.SupportedLanguages.Keyboard = new List<string>();
					this.SupportedLanguages.Keyboard.AddRange(srcOEMInput.SupportedLanguages.Keyboard);
				}
				if (srcOEMInput.SupportedLanguages.Speech != null)
				{
					this.SupportedLanguages.Speech = new List<string>();
					this.SupportedLanguages.Speech.AddRange(srcOEMInput.SupportedLanguages.Speech);
				}
			}
			this.SV = srcOEMInput.SV;
			this.UserStoreMapData = srcOEMInput.UserStoreMapData;
		}

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000008 RID: 8 RVA: 0x00002333 File Offset: 0x00000533
		[XmlIgnore]
		public Edition Edition
		{
			get
			{
				if (this._edition == null)
				{
					this._edition = ImagingEditions.GetProductEdition(this.Product);
				}
				return this._edition;
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000009 RID: 9 RVA: 0x00002354 File Offset: 0x00000554
		// (set) Token: 0x0600000A RID: 10 RVA: 0x0000236F File Offset: 0x0000056F
		public string Product
		{
			get
			{
				if (this._product == null)
				{
					this._product = OEMInput.DefaultProduct;
				}
				return this._product;
			}
			set
			{
				this._product = value;
				this._edition = null;
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600000B RID: 11 RVA: 0x0000237F File Offset: 0x0000057F
		public bool IsMMOS
		{
			get
			{
				return this.Edition.IsProduct("Phone Manufacturing OS");
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x0600000C RID: 12 RVA: 0x00002391 File Offset: 0x00000591
		// (set) Token: 0x0600000D RID: 13 RVA: 0x000023B8 File Offset: 0x000005B8
		[XmlIgnore]
		public string MSPackageRoot
		{
			get
			{
				if (string.IsNullOrWhiteSpace(this._msPackageRoot))
				{
					this._msPackageRoot = this.Edition.MSPackageRoot;
				}
				return this._msPackageRoot;
			}
			set
			{
				if (value == null)
				{
					this._msPackageRoot = null;
					return;
				}
				char[] trimChars = new char[]
				{
					'\\'
				};
				this._msPackageRoot = value.TrimEnd(trimChars);
			}
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600000E RID: 14 RVA: 0x000023E9 File Offset: 0x000005E9
		[XmlIgnore]
		public List<string> FeatureIDs
		{
			get
			{
				return this.GetFeatureList();
			}
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x0600000F RID: 15 RVA: 0x000023F1 File Offset: 0x000005F1
		[XmlIgnore]
		public List<string> MSFeatureIDs
		{
			get
			{
				return this.GetFeatureList((OEMInput.OEMFeatureTypes)268433407);
			}
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000010 RID: 16 RVA: 0x000023FE File Offset: 0x000005FE
		[XmlIgnore]
		public List<string> OEMFeatureIDs
		{
			get
			{
				return this.GetFeatureList((OEMInput.OEMFeatureTypes)268434431);
			}
		}

		// Token: 0x06000011 RID: 17 RVA: 0x0000240C File Offset: 0x0000060C
		public string ProcessOEMInputVariables(string value)
		{
			return value.Replace("$(device)", this.Device, StringComparison.OrdinalIgnoreCase).Replace("$(releasetype)", this.ReleaseType, StringComparison.OrdinalIgnoreCase).Replace("$(buildtype)", this.BuildType, StringComparison.OrdinalIgnoreCase).Replace("$(cputype)", this.CPUType, StringComparison.OrdinalIgnoreCase).Replace("$(bootuilanguage)", this.BootUILanguage, StringComparison.OrdinalIgnoreCase).Replace("$(bootlocale)", this.BootLocale, StringComparison.OrdinalIgnoreCase).Replace("$(mspackageroot)", this.MSPackageRoot, StringComparison.OrdinalIgnoreCase);
		}

		// Token: 0x06000012 RID: 18 RVA: 0x00002494 File Offset: 0x00000694
		public static void ValidateInput(ref OEMInput xmlInput, string xmlFile, IULogger logger, string msPackageDir, string cpuType)
		{
			string text = string.Empty;
			string oeminputSchema = DevicePaths.OEMInputSchema;
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			foreach (string text2 in executingAssembly.GetManifestResourceNames())
			{
				if (text2.Contains(oeminputSchema))
				{
					text = text2;
					break;
				}
			}
			if (string.IsNullOrEmpty(text))
			{
				throw new FeatureAPIException("FeatureAPI!OEMInput::ValidateInput: XSD resource was not found: " + oeminputSchema);
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
					throw new FeatureAPIException(string.Format("FeatureAPI!ValidateInput: Unable to validate OEM Input XSD for file '{0}'", xmlFile), innerException);
				}
			}
			logger.LogInfo("FeatureAPI: Successfully validated the OEM Input XML: {0}", new object[]
			{
				xmlFile
			});
			TextReader textReader = new StreamReader(LongPathFile.OpenRead(xmlFile));
			try
			{
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(OEMInput));
				xmlInput = (OEMInput)xmlSerializer.Deserialize(textReader);
				if (!string.IsNullOrWhiteSpace(msPackageDir))
				{
					xmlInput.MSPackageRoot = msPackageDir;
				}
				xmlInput.CPUType = cpuType;
				xmlInput.BuildType = Environment.ExpandEnvironmentVariables(xmlInput.BuildType);
				OEMInput oeminput = xmlInput;
				oeminput.Description = oeminput.ProcessOEMInputVariables(oeminput.Description);
				xmlInput.Description = Environment.ExpandEnvironmentVariables(xmlInput.Description);
				if (xmlInput.PackageFiles != null)
				{
					for (int j = 0; j < xmlInput.PackageFiles.Count; j++)
					{
						List<string> packageFiles = xmlInput.PackageFiles;
						int index = j;
						OEMInput oeminput2 = xmlInput;
						packageFiles[index] = oeminput2.ProcessOEMInputVariables(oeminput2.PackageFiles[j]);
						xmlInput.PackageFiles[j] = Environment.ExpandEnvironmentVariables(xmlInput.PackageFiles[j]);
					}
				}
				if (xmlInput.Edition.RequiresKeyboard && (xmlInput.SupportedLanguages.Keyboard == null || xmlInput.SupportedLanguages.Keyboard.Count == 0))
				{
					throw new FeatureAPIException("FeatureAPI!ValidateInput: At least one Keyboard language must be specified.");
				}
				if (xmlInput.AdditionalFMs != null)
				{
					for (int k = 0; k < xmlInput.AdditionalFMs.Count; k++)
					{
						xmlInput.AdditionalFMs[k] = Environment.ExpandEnvironmentVariables(xmlInput.AdditionalFMs[k]);
					}
				}
				if (xmlInput.Features == null)
				{
					xmlInput.Features = new OEMInputFeatures();
				}
			}
			catch (Exception innerException2)
			{
				throw new FeatureAPIException("FeatureAPI!ValidateInput: Unable to parse OEM Input XML file.", innerException2);
			}
			finally
			{
				textReader.Close();
			}
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00002748 File Offset: 0x00000948
		public void WriteToFile(string fileName)
		{
			using (XmlWriter xmlWriter = XmlWriter.Create(fileName, new XmlWriterSettings
			{
				Indent = true
			}))
			{
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(OEMInput));
				try
				{
					xmlSerializer.Serialize(xmlWriter, this);
				}
				catch (Exception innerException)
				{
					throw new FeatureAPIException("FeatureAPI!WriteToFile: Unable to write OEM Input XML file '" + fileName + "'", innerException);
				}
			}
		}

		// Token: 0x06000014 RID: 20 RVA: 0x000027C4 File Offset: 0x000009C4
		public static List<string> GetPackagesFromDSMs(List<string> dsmPaths)
		{
			string text = ".dsm.xml";
			List<string> list = new List<string>();
			foreach (string path in dsmPaths)
			{
				string[] files = LongPathDirectory.GetFiles(path, "*" + text);
				for (int i = 0; i < files.Length; i++)
				{
					string text2 = Path.GetFileName(files[i]);
					text2 = text2.Substring(0, text2.Length - text.Length);
					list.Add(text2);
				}
			}
			return list.Distinct(OEMInput.IgnoreCase).ToList<string>();
		}

		// Token: 0x06000015 RID: 21 RVA: 0x00002874 File Offset: 0x00000A74
		public void InferOEMInputFromPackageList(string msFMPattern, List<string> packages)
		{
			IULogger logger = new IULogger();
			string[] files;
			if (LongPathDirectory.Exists(msFMPattern))
			{
				files = LongPathDirectory.GetFiles(msFMPattern);
			}
			else
			{
				files = LongPathDirectory.GetFiles(LongPath.GetDirectoryName(msFMPattern), Path.GetFileName(msFMPattern));
			}
			string[] array = files;
			int i = 0;
			while (i < array.Length)
			{
				string xmlFile = array[i];
				FeatureManifest fm = new FeatureManifest();
				try
				{
					FeatureManifest.ValidateAndLoad(ref fm, xmlFile, logger);
				}
				catch
				{
					goto IL_53;
				}
				goto IL_4A;
				IL_53:
				i++;
				continue;
				IL_4A:
				this.InferOEMInput(fm, packages);
				goto IL_53;
			}
		}

		// Token: 0x06000016 RID: 22 RVA: 0x000028F0 File Offset: 0x00000AF0
		public void InferOEMInput(FeatureManifest fm, List<string> packages)
		{
			if (this.SupportedLanguages == null)
			{
				this.SupportedLanguages = new SupportedLangs();
				this.SupportedLanguages.UserInterface = new List<string>();
				this.SupportedLanguages.Speech = new List<string>();
				this.SupportedLanguages.Keyboard = new List<string>();
			}
			if (this.SupportedLanguages.UserInterface.Count<string>() == 0)
			{
				this.SupportedLanguages.UserInterface = fm.GetUILangFeatures(packages);
			}
			if (this.Resolutions == null)
			{
				this.Resolutions = new List<string>();
			}
			if (this.Resolutions.Count<string>() == 0)
			{
				this.Resolutions = fm.GetResolutionFeatures(packages);
			}
			if (this.Features == null)
			{
				this.Features = new OEMInputFeatures();
				this.Features.Microsoft = new List<string>();
				this.Features.OEM = new List<string>();
			}
			foreach (FeatureManifest.FMPkgInfo fmpkgInfo in fm.GetFeatureIdentifierPackages())
			{
				string text = fmpkgInfo.ID;
				if (fmpkgInfo.FMGroup == FeatureManifest.PackageGroups.KEYBOARD || fmpkgInfo.FMGroup == FeatureManifest.PackageGroups.SPEECH)
				{
					text = text + PkgFile.DefaultLanguagePattern + fmpkgInfo.GroupValue;
				}
				if (packages.Contains(text, OEMInput.IgnoreCase))
				{
					this.SetOEMInputValue(fmpkgInfo);
				}
			}
			if (fm.BootLocalePackageFile != null)
			{
				string bootLocaleBaseName = fm.BootLocalePackageFile.ID.Replace("$(bootlocale)", "", StringComparison.OrdinalIgnoreCase);
				List<string> list = (from pkg in packages
				where pkg.StartsWith(bootLocaleBaseName, StringComparison.OrdinalIgnoreCase)
				select pkg).ToList<string>();
				if (list.Count > 0)
				{
					this.BootLocale = list[0].Substring(bootLocaleBaseName.Length);
				}
			}
			if (fm.BootUILanguagePackageFile != null)
			{
				string bootLangBaseName = fm.BootUILanguagePackageFile.ID.Replace("$(bootuilanguage)", "", StringComparison.OrdinalIgnoreCase);
				List<string> list2 = (from pkg in packages
				where pkg.StartsWith(bootLangBaseName, StringComparison.OrdinalIgnoreCase)
				select pkg).ToList<string>();
				if (list2.Count > 0)
				{
					this.BootUILanguage = list2[0].Substring(bootLangBaseName.Length);
				}
			}
		}

		// Token: 0x06000017 RID: 23 RVA: 0x00002B2C File Offset: 0x00000D2C
		private void SetOEMInputValue(FeatureManifest.FMPkgInfo FeatureIDPkg)
		{
			switch (FeatureIDPkg.FMGroup)
			{
			case FeatureManifest.PackageGroups.RELEASE:
				this.ReleaseType = FeatureIDPkg.GroupValue;
				return;
			case FeatureManifest.PackageGroups.DEVICELAYOUT:
				this.SOC = FeatureIDPkg.GroupValue;
				return;
			case FeatureManifest.PackageGroups.OEMDEVICEPLATFORM:
				this.Device = FeatureIDPkg.GroupValue;
				return;
			case FeatureManifest.PackageGroups.SV:
				this.SV = FeatureIDPkg.GroupValue;
				return;
			case FeatureManifest.PackageGroups.SOC:
				this.SOC = FeatureIDPkg.GroupValue;
				return;
			case FeatureManifest.PackageGroups.DEVICE:
				this.Device = FeatureIDPkg.GroupValue;
				return;
			case FeatureManifest.PackageGroups.MSFEATURE:
				this.Features.Microsoft.Add(FeatureIDPkg.GroupValue);
				return;
			case FeatureManifest.PackageGroups.OEMFEATURE:
				this.Features.OEM.Add(FeatureIDPkg.GroupValue);
				return;
			case FeatureManifest.PackageGroups.KEYBOARD:
				this.SupportedLanguages.Keyboard.Add(FeatureIDPkg.GroupValue);
				return;
			case FeatureManifest.PackageGroups.SPEECH:
				this.SupportedLanguages.Speech.Add(FeatureIDPkg.GroupValue);
				return;
			case FeatureManifest.PackageGroups.PRERELEASE:
				this.ExcludePrereleaseFeatures = FeatureIDPkg.GroupValue.Equals("replacement", StringComparison.OrdinalIgnoreCase);
				return;
			default:
				return;
			}
		}

		// Token: 0x06000018 RID: 24 RVA: 0x00002C36 File Offset: 0x00000E36
		public List<string> GetFeatureList()
		{
			return this.GetFeatureList((OEMInput.OEMFeatureTypes)268435455);
		}

		// Token: 0x06000019 RID: 25 RVA: 0x00002C44 File Offset: 0x00000E44
		public List<string> GetFeatureList(OEMInput.OEMFeatureTypes forFeatures)
		{
			List<string> list = new List<string>();
			if (forFeatures.HasFlag(OEMInput.OEMFeatureTypes.BASE))
			{
				list.Add(FeatureManifest.PackageGroups.BASE.ToString());
			}
			if (forFeatures.HasFlag(OEMInput.OEMFeatureTypes.SV))
			{
				list.Add(this.FMGroupAndValueToFeatureID(FeatureManifest.PackageGroups.SV, this.SV));
			}
			if (forFeatures.HasFlag(OEMInput.OEMFeatureTypes.SOC))
			{
				list.Add(this.FMGroupAndValueToFeatureID(FeatureManifest.PackageGroups.SOC, this.SOC));
			}
			if (forFeatures.HasFlag(OEMInput.OEMFeatureTypes.DEVICE))
			{
				list.Add(this.FMGroupAndValueToFeatureID(FeatureManifest.PackageGroups.DEVICE, this.Device));
			}
			if (forFeatures.HasFlag(OEMInput.OEMFeatureTypes.RELEASE))
			{
				list.Add(this.FMGroupAndValueToFeatureID(FeatureManifest.PackageGroups.RELEASE, this.ReleaseType));
			}
			if (forFeatures.HasFlag(OEMInput.OEMFeatureTypes.UILANGS) && this.SupportedLanguages != null && this.SupportedLanguages.UserInterface != null)
			{
				foreach (string value in this.SupportedLanguages.UserInterface)
				{
					list.Add(this.KeyAndValueToFeatureID(OEMInput.OEMFeatureTypes.UILANGS.ToString(), value));
				}
			}
			if (forFeatures.HasFlag(OEMInput.OEMFeatureTypes.KEYBOARD) && this.SupportedLanguages != null)
			{
				foreach (string value2 in this.SupportedLanguages.Keyboard)
				{
					list.Add(this.FMGroupAndValueToFeatureID(FeatureManifest.PackageGroups.KEYBOARD, value2));
				}
			}
			if (forFeatures.HasFlag(OEMInput.OEMFeatureTypes.SPEECH) && this.SupportedLanguages != null)
			{
				foreach (string value3 in this.SupportedLanguages.Speech)
				{
					list.Add(this.FMGroupAndValueToFeatureID(FeatureManifest.PackageGroups.SPEECH, value3));
				}
			}
			if (forFeatures.HasFlag(OEMInput.OEMFeatureTypes.BOOTUI))
			{
				list.Add(this.FMGroupAndValueToFeatureID(FeatureManifest.PackageGroups.BOOTUI, this.BootUILanguage));
			}
			if (forFeatures.HasFlag(OEMInput.OEMFeatureTypes.BOOTLOCALE))
			{
				list.Add(this.FMGroupAndValueToFeatureID(FeatureManifest.PackageGroups.BOOTLOCALE, this.BootLocale));
			}
			if (forFeatures.HasFlag(OEMInput.OEMFeatureTypes.RESOULTIONS) && this.Resolutions != null)
			{
				foreach (string value4 in this.Resolutions)
				{
					list.Add(this.KeyAndValueToFeatureID(OEMInput.OEMFeatureTypes.RESOULTIONS.ToString(), value4));
				}
			}
			if (forFeatures.HasFlag(OEMInput.OEMFeatureTypes.PRERELEASE))
			{
				string value5 = this.ExcludePrereleaseFeatures ? "REPLACEMENT" : "PROTECTED";
				list.Add(this.FMGroupAndValueToFeatureID(FeatureManifest.PackageGroups.PRERELEASE, value5));
			}
			if (this.Features != null)
			{
				if (forFeatures.HasFlag(OEMInput.OEMFeatureTypes.MSFEATURES) && this.Features.Microsoft != null)
				{
					list.AddRange(from feature in this.Features.Microsoft
					select "MS_" + feature);
				}
				if (forFeatures.HasFlag(OEMInput.OEMFeatureTypes.OEMFEATURES) && this.Features.OEM != null)
				{
					list.AddRange(from feature in this.Features.OEM
					select "OEM_" + feature);
				}
			}
			return list;
		}

		// Token: 0x0600001A RID: 26 RVA: 0x00003054 File Offset: 0x00001254
		private string FMGroupAndValueToFeatureID(FeatureManifest.PackageGroups group, string value)
		{
			return this.KeyAndValueToFeatureID(group.ToString(), value);
		}

		// Token: 0x0600001B RID: 27 RVA: 0x0000306C File Offset: 0x0000126C
		private string KeyAndValueToFeatureID(string key, string value)
		{
			string text = key.ToUpper(CultureInfo.InvariantCulture) + "_";
			if (string.IsNullOrEmpty(value))
			{
				text += "INVALID";
			}
			else
			{
				text += value.ToUpper(CultureInfo.InvariantCulture);
			}
			return text;
		}

		// Token: 0x0600001C RID: 28 RVA: 0x000030B8 File Offset: 0x000012B8
		public List<string> GetFMs()
		{
			List<string> list = new List<string>();
			if (this.Edition != null)
			{
				list.AddRange(from pkg in this.Edition.CoreFeatureManifestPackages
				select pkg.FMDeviceName);
			}
			foreach (string path in this.AdditionalFMs)
			{
				string item = Path.GetFileName(path).ToUpper(CultureInfo.InvariantCulture);
				list.Add(item);
			}
			return list;
		}

		// Token: 0x04000001 RID: 1
		private static readonly string DefaultProduct = "Windows Phone";

		// Token: 0x04000002 RID: 2
		public static readonly string BuildType_FRE = "fre";

		// Token: 0x04000003 RID: 3
		public static readonly string BuildType_CHK = "chk";

		// Token: 0x04000004 RID: 4
		private const string ExcludePrereleaseTrueValue = "REPLACEMENT";

		// Token: 0x04000005 RID: 5
		private const string ExcludePrereleaseFalseValue = "PROTECTED";

		// Token: 0x04000006 RID: 6
		private string _product = OEMInput.DefaultProduct;

		// Token: 0x04000007 RID: 7
		private static StringComparer IgnoreCase = StringComparer.OrdinalIgnoreCase;

		// Token: 0x04000008 RID: 8
		private Edition _edition;

		// Token: 0x04000009 RID: 9
		public string Description;

		// Token: 0x0400000A RID: 10
		public string SOC;

		// Token: 0x0400000B RID: 11
		public string SV;

		// Token: 0x0400000C RID: 12
		public string Device;

		// Token: 0x0400000D RID: 13
		public string ReleaseType;

		// Token: 0x0400000E RID: 14
		public string BuildType;

		// Token: 0x0400000F RID: 15
		public SupportedLangs SupportedLanguages;

		// Token: 0x04000010 RID: 16
		public string BootUILanguage;

		// Token: 0x04000011 RID: 17
		public string BootLocale;

		// Token: 0x04000012 RID: 18
		[XmlArrayItem(ElementName = "Resolution", Type = typeof(string), IsNullable = false)]
		[XmlArray]
		[DefaultValue(null)]
		public List<string> Resolutions;

		// Token: 0x04000013 RID: 19
		[XmlArrayItem(ElementName = "AdditionalFM", Type = typeof(string), IsNullable = false)]
		[XmlArray]
		[DefaultValue(null)]
		public List<string> AdditionalFMs;

		// Token: 0x04000014 RID: 20
		public OEMInputFeatures Features;

		// Token: 0x04000015 RID: 21
		[XmlArrayItem(ElementName = "AdditionalSKU", Type = typeof(string), IsNullable = false)]
		[XmlArray]
		[DefaultValue(null)]
		public List<string> AdditionalSKUs;

		// Token: 0x04000016 RID: 22
		[XmlArrayItem(ElementName = "OptionalFeature", Type = typeof(string), IsNullable = false)]
		[XmlArray]
		[DefaultValue(null)]
		public List<string> InternalOptionalFeatures;

		// Token: 0x04000017 RID: 23
		[XmlArrayItem(ElementName = "OptionalFeatures", Type = typeof(string), IsNullable = false)]
		[XmlArray]
		[DefaultValue(null)]
		public List<string> ProductionOptionalFeatures;

		// Token: 0x04000018 RID: 24
		[XmlArrayItem(ElementName = "OptionalFeature", Type = typeof(string), IsNullable = false)]
		[XmlArray]
		[DefaultValue(null)]
		public List<string> MSOptionalFeatures;

		// Token: 0x04000019 RID: 25
		public UserStoreMapData UserStoreMapData;

		// Token: 0x0400001A RID: 26
		public string FormatDPP;

		// Token: 0x0400001B RID: 27
		[DefaultValue(false)]
		public bool ExcludePrereleaseFeatures;

		// Token: 0x0400001C RID: 28
		[XmlArrayItem(ElementName = "PackageFile", Type = typeof(string), IsNullable = false)]
		[XmlArray]
		[DefaultValue(null)]
		public List<string> PackageFiles;

		// Token: 0x0400001D RID: 29
		private string _msPackageRoot;

		// Token: 0x0400001E RID: 30
		[XmlIgnore]
		public string CPUType;

		// Token: 0x0400001F RID: 31
		public const OEMInput.OEMFeatureTypes ALLFEATURES = (OEMInput.OEMFeatureTypes)268435455;

		// Token: 0x04000020 RID: 32
		public const OEMInput.OEMFeatureTypes MSONLYFEATURES = (OEMInput.OEMFeatureTypes)268433407;

		// Token: 0x04000021 RID: 33
		public const OEMInput.OEMFeatureTypes OEMONLYFEATURES = (OEMInput.OEMFeatureTypes)268434431;

		// Token: 0x0200002C RID: 44
		[Flags]
		public enum OEMFeatureTypes
		{
			// Token: 0x040000DB RID: 219
			NONE = 0,
			// Token: 0x040000DC RID: 220
			BASE = 1,
			// Token: 0x040000DD RID: 221
			BOOTUI = 2,
			// Token: 0x040000DE RID: 222
			BOOTLOCALE = 4,
			// Token: 0x040000DF RID: 223
			RELEASE = 8,
			// Token: 0x040000E0 RID: 224
			SV = 32,
			// Token: 0x040000E1 RID: 225
			SOC = 64,
			// Token: 0x040000E2 RID: 226
			DEVICE = 128,
			// Token: 0x040000E3 RID: 227
			KEYBOARD = 256,
			// Token: 0x040000E4 RID: 228
			SPEECH = 512,
			// Token: 0x040000E5 RID: 229
			MSFEATURES = 1024,
			// Token: 0x040000E6 RID: 230
			OEMFEATURES = 2048,
			// Token: 0x040000E7 RID: 231
			PRERELEASE = 4096,
			// Token: 0x040000E8 RID: 232
			UILANGS = 8192,
			// Token: 0x040000E9 RID: 233
			RESOULTIONS = 16384
		}
	}
}
