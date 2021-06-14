using System;
using System.IO;
using System.Xml.Serialization;
using Microsoft.WindowsPhone.ImageUpdate.PkgCommon;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.FeatureAPI
{
	// Token: 0x02000024 RID: 36
	public class EditionPackage
	{
		// Token: 0x17000026 RID: 38
		// (get) Token: 0x060000C9 RID: 201 RVA: 0x00007D64 File Offset: 0x00005F64
		[XmlIgnore]
		public string FMDevicePath
		{
			get
			{
				return Path.Combine(this.FMDeviceDir, this.FMDeviceName);
			}
		}

		// Token: 0x060000CA RID: 202 RVA: 0x00007D77 File Offset: 0x00005F77
		public override string ToString()
		{
			return this.PackageName;
		}

		// Token: 0x060000CB RID: 203 RVA: 0x00007D80 File Offset: 0x00005F80
		public string GetPackagePath(string msPackageRoot, string cpuType, string buildType)
		{
			string text = this.GetRawPackagePath(msPackageRoot);
			text = this.ProcessEnvs(text, cpuType, buildType);
			if (!File.Exists(text))
			{
				string text2;
				if (Path.GetExtension(text).Equals(PkgConstants.c_strCBSPackageExtension, StringComparison.OrdinalIgnoreCase))
				{
					text2 = Path.ChangeExtension(text, PkgConstants.c_strPackageExtension);
				}
				else
				{
					text2 = Path.ChangeExtension(text, PkgConstants.c_strCBSPackageExtension);
				}
				if (File.Exists(text2))
				{
					text = text2;
				}
			}
			return text;
		}

		// Token: 0x060000CC RID: 204 RVA: 0x00007DDF File Offset: 0x00005FDF
		public string GetRawPackagePath(string msPackageRoot)
		{
			return Path.Combine(msPackageRoot, this.RelativePath, this.PackageName);
		}

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x060000CD RID: 205 RVA: 0x00007DF3 File Offset: 0x00005FF3
		[XmlIgnore]
		public FeatureManifest FM
		{
			get
			{
				return this._fm;
			}
		}

		// Token: 0x060000CE RID: 206 RVA: 0x00007DFC File Offset: 0x00005FFC
		public FeatureManifest LoadFM(string tempDirectory, string msPackageRoot, string cpuType, string buildType)
		{
			string tempDirectory2 = FileUtils.GetTempDirectory();
			try
			{
				string packagePath = this.GetPackagePath(msPackageRoot, cpuType, buildType);
				string text = Path.Combine(tempDirectory2, Path.GetFileName(packagePath));
				LongPathFile.Copy(packagePath, text);
				IPkgInfo pkgInfo = Package.LoadFromCab(text);
				bool overwriteExistingFiles = true;
				string text2 = Path.Combine(tempDirectory2, Path.GetFileName(this.FMDevicePath));
				pkgInfo.ExtractFile(this.FMDevicePath, text2, overwriteExistingFiles);
				FeatureManifest.ValidateAndLoad(ref this._fm, text2, new IULogger());
			}
			finally
			{
				FileUtils.DeleteTree(tempDirectory2);
			}
			return this._fm;
		}

		// Token: 0x060000CF RID: 207 RVA: 0x00007E88 File Offset: 0x00006088
		private string ProcessEnvs(string source, string cpuType, string buildType)
		{
			string text = source;
			if (!string.IsNullOrWhiteSpace(cpuType))
			{
				text = text.Replace("$(cputype)", cpuType, StringComparison.OrdinalIgnoreCase);
			}
			if (!string.IsNullOrWhiteSpace(buildType))
			{
				text = text.Replace("$(buildtype)", buildType, StringComparison.OrdinalIgnoreCase);
			}
			return Environment.ExpandEnvironmentVariables(text);
		}

		// Token: 0x060000D0 RID: 208 RVA: 0x00007ECB File Offset: 0x000060CB
		public bool ExistsUnder(string msPackageRoot, string cpuType, string buildType)
		{
			return LongPathFile.Exists(this.GetPackagePath(msPackageRoot, cpuType, buildType));
		}

		// Token: 0x040000AB RID: 171
		[XmlAttribute]
		public string RelativePath;

		// Token: 0x040000AC RID: 172
		[XmlAttribute]
		public string PackageName;

		// Token: 0x040000AD RID: 173
		[XmlAttribute]
		public string FMDeviceDir;

		// Token: 0x040000AE RID: 174
		[XmlAttribute]
		public string FMDeviceName;

		// Token: 0x040000AF RID: 175
		[XmlAttribute]
		public string AKName;

		// Token: 0x040000B0 RID: 176
		private FeatureManifest _fm;
	}
}
