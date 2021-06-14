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
	// Token: 0x02000007 RID: 7
	public class AppPackageAppxBundle : AppPackageAppx
	{
		// Token: 0x06000036 RID: 54 RVA: 0x00003BB4 File Offset: 0x00001DB4
		public AppPackageAppxBundle(InboxAppParameters parameters) : base(parameters, true, "")
		{
			this._isTopLevelPackage = true;
			if (!InboxAppUtils.ExtensionMatches(this._parameters.PackageBasePath, ".appxbundle"))
			{
				throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "Packages without a \"{0}\" extension are not supported.", new object[]
				{
					".appxbundle"
				}));
			}
		}

		// Token: 0x06000037 RID: 55 RVA: 0x00003C1C File Offset: 0x00001E1C
		public override void OpenPackage()
		{
			Path.Combine(Path.Combine(this.DecompressBundle(), "AppxMetadata"), "AppxBundleManifest.xml");
			base.ReadManifest(this._parameters.PackageBasePath, true);
			base.ReadProvXML();
			this._provXML.DependencyHash = InboxAppUtils.CalcHash(this._parameters.PackageBasePath) + InboxAppUtils.CalcHash(this._parameters.LicenseBasePath);
		}

		// Token: 0x06000038 RID: 56 RVA: 0x00003C8C File Offset: 0x00001E8C
		public override List<PkgObject> Map(IInboxAppPackage appPackage, IPkgProject packageGenerator, OSComponentBuilder osComponent)
		{
			if (osComponent == null)
			{
				throw new ArgumentNullException("osComponent", "osComponent must not be null!");
			}
			string installDestinationPath = InboxAppUtils.ResolveDestinationPath(base.GetInstallDestinationPath(this._isTopLevelPackage), this._parameters.InfuseIntoDataPartition, packageGenerator);
			string licenseFileDestinationPath = InboxAppUtils.ResolveDestinationPath(this._provXML.LicenseDestinationPath, this._parameters.InfuseIntoDataPartition, packageGenerator);
			FileGroupBuilder fileGroupBuilder = osComponent.AddFileGroup();
			base.ProcessDirectory(this._packageFilesDirectory, this._packageFilesDirectory, fileGroupBuilder, "AppxMetadata");
			this._provXML.Update(installDestinationPath, licenseFileDestinationPath);
			string text = Path.Combine(this._parameters.WorkingBaseDir, Path.GetFileName(this._parameters.ProvXMLBasePath));
			this._provXML.Save(text);
			fileGroupBuilder.AddFile(text, Path.GetDirectoryName(this._provXML.ProvXMLDestinationPath)).SetName(Path.GetFileName(this._provXML.ProvXMLDestinationPath).RemoveSrcExtension());
			LogUtil.Message(string.Format(CultureInfo.InvariantCulture, "AppxBundle ProvXML file added to package: Source Path \"{0}\", Destination Dir \"{1}\"", new object[]
			{
				text,
				Path.GetDirectoryName(this._provXML.ProvXMLDestinationPath)
			}));
			if (this._parameters.UpdateValue != UpdateType.UpdateNotNeeded)
			{
				fileGroupBuilder.AddFile(text, Path.GetDirectoryName(this._provXML.UpdateProvXMLDestinationPath)).SetName(Path.GetFileName(this._provXML.UpdateProvXMLDestinationPath));
				LogUtil.Message(string.Format(CultureInfo.InvariantCulture, "Appxbundle ProvXML file added to package: Source Path \"{0}\", Destination Dir \"{1}\", File \"{2}\"", new object[]
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
			LogUtil.Message(string.Format(CultureInfo.InvariantCulture, "AppxBundle license file added to package: Source Path \"{0}\", Destination Dir \"{1}\" File Name \"{2}\"", new object[]
			{
				text2,
				directoryName,
				fileName
			}));
			AppManifestAppxBundle appManifestAppxBundle = this._manifest as AppManifestAppxBundle;
			foreach (KeyValuePair<string, AppPackageAppx> keyValuePair in this._appxs)
			{
				AppManifestAppxBundle.BundlePackage bundlePackage = new AppManifestAppxBundle.BundlePackage(APPX_BUNDLE_PAYLOAD_PACKAGE_TYPE.APPX_BUNDLE_PAYLOAD_PACKAGE_TYPE_RESOURCE);
				bundlePackage.FileName = keyValuePair.Key;
				if (appManifestAppxBundle.BundlePackages.IndexOf(bundlePackage) >= 0)
				{
					IInboxAppPackage value = keyValuePair.Value;
					((IInboxAppToPkgObjectsMappingStrategy)value).Map(value, packageGenerator, osComponent);
				}
				else
				{
					LogUtil.Warning(string.Format(CultureInfo.InvariantCulture, "Could not find an AppX payload file with the filename {0}. Please make sure the appx bundle manifest contains correct filenames for each Package element.", new object[]
					{
						bundlePackage.FileName
					}));
				}
			}
			return new List<PkgObject>
			{
				osComponent.ToPkgObject()
			};
		}

		// Token: 0x06000039 RID: 57 RVA: 0x00003F7C File Offset: 0x0000217C
		private string DecompressBundle()
		{
			string empty = string.Empty;
			int num = NativeMethods.Unbundle(this._parameters.PackageBasePath, this._parameters.WorkingBaseDir, false, IntPtr.Zero, ref empty);
			if (num != 0)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Unpack returned error {3}. One of the following fields may be empty or have an invalid value:\n(inputBundlePath)=\"{0}\" (outputDirectoryPath)=\"{1}\" (destinationDirectory)=\"{2}\"", new object[]
				{
					this._parameters.PackageBasePath,
					this._parameters.WorkingBaseDir,
					empty,
					num
				}));
			}
			this._packageFilesDirectory = empty;
			string[] files = Directory.GetFiles(empty, "*.appx", SearchOption.TopDirectoryOnly);
			if (files.Length == 0)
			{
				throw new InvalidDataException(string.Format(CultureInfo.InvariantCulture, "The Appx Bundle \"{0}\" is empty!", new object[]
				{
					this._parameters.PackageBasePath
				}));
			}
			foreach (string text in files)
			{
				AppPackageAppx appPackageAppx = new AppPackageAppx(new InboxAppParameters(text, string.Empty, string.Empty, this._parameters.InfuseIntoDataPartition, this._parameters.UpdateValue, this._parameters.Category, this._parameters.WorkingBaseDir), false, Path.GetFileNameWithoutExtension(text));
				appPackageAppx.OpenPackage();
				this._appxs.Add(Path.GetFileName(text), appPackageAppx);
			}
			return empty;
		}

		// Token: 0x0600003A RID: 58 RVA: 0x000040BD File Offset: 0x000022BD
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "AppX Bundle package: (Title)=\"{0}\" (BasePath)=\"{1}\"", new object[]
			{
				this._manifest.Title,
				this._parameters.PackageBasePath
			});
		}

		// Token: 0x04000023 RID: 35
		private Dictionary<string, AppPackageAppx> _appxs = new Dictionary<string, AppPackageAppx>();
	}
}
