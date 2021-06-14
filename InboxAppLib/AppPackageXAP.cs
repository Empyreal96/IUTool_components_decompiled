using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.WindowsPhone.ImageUpdate.InboxAppLib.Interfaces;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.Interfaces;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.ImageUpdate.InboxAppLib
{
	// Token: 0x02000008 RID: 8
	public class AppPackageXAP : IInboxAppPackage, IInboxAppToPkgObjectsMappingStrategy
	{
		// Token: 0x0600003B RID: 59 RVA: 0x000040F0 File Offset: 0x000022F0
		public AppPackageXAP(InboxAppParameters parameters)
		{
			if (parameters == null)
			{
				throw new ArgumentNullException("parameters", "parameters must not be null!");
			}
			this._parameters = parameters;
			if (!InboxAppUtils.ExtensionMatches(this._parameters.PackageBasePath, ".xap"))
			{
				throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "Packages without a \"{0}\" extension are not supported.", new object[]
				{
					".xap"
				}));
			}
		}

		// Token: 0x0600003C RID: 60 RVA: 0x0000416D File Offset: 0x0000236D
		public void OpenPackage()
		{
			this.DecompressPackage();
			this.ReadManifest();
			this.ReadProvXML();
		}

		// Token: 0x0600003D RID: 61 RVA: 0x00004181 File Offset: 0x00002381
		public List<string> GetCapabilities()
		{
			return this._manifest.Capabilities;
		}

		// Token: 0x0600003E RID: 62 RVA: 0x0000418E File Offset: 0x0000238E
		public IInboxAppManifest GetManifest()
		{
			return this._manifest;
		}

		// Token: 0x0600003F RID: 63 RVA: 0x00002FC3 File Offset: 0x000011C3
		public IInboxAppToPkgObjectsMappingStrategy GetPkgObjectsMappingStrategy()
		{
			return this;
		}

		// Token: 0x06000040 RID: 64 RVA: 0x00004198 File Offset: 0x00002398
		public string GetInstallDestinationPath(bool isTopLevelPackage = true)
		{
			string text = string.Empty;
			string result = string.Empty;
			if (this.IsPackageSLL())
			{
				text = (this._parameters.InfuseIntoDataPartition ? "$(runtime.data)\\Programs\\WindowsApps" : "$(runtime.windows)\\InfusedApps");
				result = Path.Combine(text, this._lightupManifest.PackageFullName);
			}
			else if (this._parameters.InfuseIntoDataPartition)
			{
				text = "$(runtime.data)\\Programs\\{0}\\install";
				result = string.Format(CultureInfo.InvariantCulture, text, new object[]
				{
					this._manifest.ProductID
				});
			}
			else
			{
				text = "$(runtime.commonfiles)\\InboxApps";
				result = Path.Combine(text, this._manifest.ProductID);
			}
			return result;
		}

		// Token: 0x06000041 RID: 65 RVA: 0x00004238 File Offset: 0x00002438
		public List<PkgObject> Map(IInboxAppPackage appPackage, IPkgProject packageGenerator, OSComponentBuilder osComponent)
		{
			if (osComponent == null)
			{
				throw new ArgumentNullException("osComponent", "osComponent must not be null!");
			}
			string installDestinationPath = InboxAppUtils.ResolveDestinationPath(this.GetInstallDestinationPath(true), this._parameters.InfuseIntoDataPartition, packageGenerator);
			string licenseFileDestinationPath = InboxAppUtils.ResolveDestinationPath(this._provXML.LicenseDestinationPath, this._parameters.InfuseIntoDataPartition, packageGenerator);
			FileGroupBuilder fileGroupBuilder = osComponent.AddFileGroup();
			this.ProcessDirectory(this._packageFilesDirectory, this._packageFilesDirectory, fileGroupBuilder);
			this._provXML.Update(installDestinationPath, licenseFileDestinationPath);
			string text = Path.Combine(this._parameters.WorkingBaseDir, Path.GetFileName(this._parameters.ProvXMLBasePath));
			this._provXML.Save(text);
			fileGroupBuilder.AddFile(text, Path.GetDirectoryName(this._provXML.ProvXMLDestinationPath));
			LogUtil.Message(string.Format(CultureInfo.InvariantCulture, "XAP ProvXML file added to package: Source Path \"{0}\", Destination Dir \"{1}\"", new object[]
			{
				text,
				Path.GetDirectoryName(this._provXML.ProvXMLDestinationPath)
			}));
			if (this._parameters.UpdateValue != UpdateType.UpdateNotNeeded)
			{
				fileGroupBuilder.AddFile(text, Path.GetDirectoryName(this._provXML.UpdateProvXMLDestinationPath)).SetName(Path.GetFileName(this._provXML.UpdateProvXMLDestinationPath));
				LogUtil.Message(string.Format(CultureInfo.InvariantCulture, "Appx ProvXML file added to package: Source Path \"{0}\", Destination Dir \"{1}\", File \"{2}\"", new object[]
				{
					text,
					Path.GetDirectoryName(this._provXML.UpdateProvXMLDestinationPath),
					Path.GetFileName(this._provXML.UpdateProvXMLDestinationPath)
				}));
			}
			string text2 = Path.Combine(this._parameters.WorkingBaseDir, Path.GetFileName(this._parameters.LicenseBasePath));
			File.Copy(this._parameters.LicenseBasePath, text2);
			string directoryName = Path.GetDirectoryName(this._provXML.LicenseDestinationPath);
			string fileName = Path.GetFileName(this._provXML.LicenseDestinationPath);
			fileGroupBuilder.AddFile(text2, directoryName).SetName(fileName);
			LogUtil.Message(string.Format(CultureInfo.InvariantCulture, "XAP license file added to package: Source Path \"{0}\", Destination Dir \"{1}\" File Name \"{2}\"", new object[]
			{
				text2,
				directoryName,
				fileName
			}));
			return new List<PkgObject>
			{
				osComponent.ToPkgObject()
			};
		}

		// Token: 0x06000042 RID: 66 RVA: 0x00004450 File Offset: 0x00002650
		protected void DecompressPackage()
		{
			this._packageFilesDirectory = Path.Combine(this._parameters.WorkingBaseDir, "Content");
			if (Directory.Exists(this._packageFilesDirectory))
			{
				Directory.Delete(this._packageFilesDirectory, true);
			}
			InboxAppUtils.Unzip(this._parameters.PackageBasePath, this._packageFilesDirectory);
		}

		// Token: 0x06000043 RID: 67 RVA: 0x000044A8 File Offset: 0x000026A8
		protected void ReadManifest()
		{
			if (this._manifest == null)
			{
				string manifestBasePath = Path.Combine(this._packageFilesDirectory, "WMAppManifest.xml");
				this._manifest = new AppManifestXAP(manifestBasePath);
				this._manifest.ReadManifest();
				string text = Path.Combine(this._packageFilesDirectory, "appxmanifest.xml");
				if (File.Exists(text))
				{
					LogUtil.Message(string.Format(CultureInfo.InvariantCulture, "XAP file is a Silverlight Lightup app with an extra {0}", new object[]
					{
						"appxmanifest.xml"
					}));
					this._lightupManifest = AppManifestAppxBase.CreateAppxManifest(string.Empty, text, false);
					this._lightupManifest.ReadManifest();
				}
			}
		}

		// Token: 0x06000044 RID: 68 RVA: 0x00004540 File Offset: 0x00002740
		protected void ReadProvXML()
		{
			this._provXML = new ProvXMLXAP(this._parameters, this._manifest);
			this._provXML.ReadProvXML();
		}

		// Token: 0x06000045 RID: 69 RVA: 0x00004564 File Offset: 0x00002764
		protected void ProcessDirectory(string targetBaseDir, string baseBasePath, FileGroupBuilder groupBuilder)
		{
			foreach (string fileBasePath in Directory.GetFiles(targetBaseDir))
			{
				this.ProcessFile(fileBasePath, baseBasePath, groupBuilder);
			}
			foreach (string text in Directory.EnumerateDirectories(targetBaseDir))
			{
				LogUtil.Message(string.Format(CultureInfo.InvariantCulture, "Processing sub directory: {0}", new object[]
				{
					text
				}));
				this.ProcessDirectory(text, baseBasePath, groupBuilder);
			}
		}

		// Token: 0x06000046 RID: 70 RVA: 0x000045F8 File Offset: 0x000027F8
		protected void ProcessFile(string fileBasePath, string baseBasePath, FileGroupBuilder groupBuilder)
		{
			string path = string.Empty;
			string directoryName = Path.GetDirectoryName(fileBasePath);
			if (directoryName.StartsWith(baseBasePath, StringComparison.OrdinalIgnoreCase))
			{
				path = directoryName.Remove(0, baseBasePath.Length).TrimStart(new char[]
				{
					'\\'
				});
			}
			string text = Path.Combine(this.GetInstallDestinationPath(true), path);
			groupBuilder.AddFile(fileBasePath, text);
			LogUtil.Message(string.Format(CultureInfo.InvariantCulture, "XAP content file added to package: Source Path \"{0}\", Destination Dir \"{1}\"", new object[]
			{
				fileBasePath,
				text
			}));
		}

		// Token: 0x06000047 RID: 71 RVA: 0x00004673 File Offset: 0x00002873
		protected bool IsPackageSLL()
		{
			return this._lightupManifest != null;
		}

		// Token: 0x06000048 RID: 72 RVA: 0x0000467E File Offset: 0x0000287E
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "XAP package: (Title)=\"{0}\" (BasePath)=\"{1}\"", new object[]
			{
				this._manifest.Title,
				this._parameters.PackageBasePath
			});
		}

		// Token: 0x04000024 RID: 36
		private InboxAppParameters _parameters;

		// Token: 0x04000025 RID: 37
		private AppManifestXAP _manifest;

		// Token: 0x04000026 RID: 38
		private AppManifestAppxBase _lightupManifest;

		// Token: 0x04000027 RID: 39
		private IInboxProvXML _provXML;

		// Token: 0x04000028 RID: 40
		private readonly List<string> _capabilities = new List<string>();

		// Token: 0x04000029 RID: 41
		private string _packageFilesDirectory = string.Empty;
	}
}
