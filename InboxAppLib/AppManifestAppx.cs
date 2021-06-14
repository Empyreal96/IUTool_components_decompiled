using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using Microsoft.WindowsPhone.ImageUpdate.InboxAppLib.Interfaces;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.ImageUpdate.InboxAppLib
{
	// Token: 0x02000002 RID: 2
	public class AppManifestAppx : AppManifestAppxBase
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public AppManifestAppx(string packageBasePath, string manifestBasePath) : base(packageBasePath, manifestBasePath)
		{
			this._manifestType = AppManifestAppxBase.AppxManifestType.MainPackage;
		}

		// Token: 0x06000002 RID: 2 RVA: 0x00002064 File Offset: 0x00000264
		public override void ReadManifest()
		{
			StringBuilder stringBuilder = new StringBuilder();
			LogUtil.Message("Parsing AppxManifest: {0}", new object[]
			{
				this._packageBasePath
			});
			IAppxFactory appxFactory = (IAppxFactory)new AppxFactory();
			IAppxManifestReader appxManifestReader = null;
			IAppxManifestProperties appxManifestProperties = null;
			try
			{
				if (!string.IsNullOrEmpty(this._packageBasePath))
				{
					IStream inputStream = StreamFactory.CreateFileStream(this._packageBasePath);
					appxManifestReader = appxFactory.CreatePackageReader(inputStream).GetManifest();
				}
				else
				{
					IStream inputStream = StreamFactory.CreateFileStream(this._manifestBasePath);
					appxManifestReader = appxFactory.CreateManifestReader(inputStream);
				}
				appxManifestProperties = appxManifestReader.GetProperties();
				IAppxManifestPackageId packageId = appxManifestReader.GetPackageId();
				base.PopulateDefaultPackageProperties(packageId);
			}
			catch
			{
				LogUtil.Message("An exception occured while trying to read the manifest.");
				throw;
			}
			try
			{
				IAppxManifestQualifiedResourcesEnumerator qualifiedResources = ((IAppxManifestReader2)appxManifestReader).GetQualifiedResources();
				string value = string.Empty;
				while (qualifiedResources.GetHasCurrent())
				{
					IAppxManifestQualifiedResource current = qualifiedResources.GetCurrent();
					DX_FEATURE_LEVEL dxfeatureLevel = current.GetDXFeatureLevel();
					uint scale = current.GetScale();
					value = current.GetLanguage();
					if (scale != 0U)
					{
						this._resources.Add(new AppManifestAppxBase.Resource("Scale", scale.ToString()));
					}
					else if (!string.IsNullOrEmpty(current.GetLanguage()))
					{
						this._resources.Add(new AppManifestAppxBase.Resource("Language", value));
						value = string.Empty;
					}
					else if (dxfeatureLevel != DX_FEATURE_LEVEL.DX_FEATURE_LEVEL_UNSPECIFIED)
					{
						this._resources.Add(new AppManifestAppxBase.Resource("DXFeatureLevel", dxfeatureLevel.ToString()));
					}
					qualifiedResources.MoveNext();
				}
				this._isFramework = appxManifestProperties.GetBoolValue("Framework");
				this._isResource = appxManifestProperties.GetBoolValue("ResourcePackage");
				IAppxManifestPackageDependenciesEnumerator packageDependencies = appxManifestReader.GetPackageDependencies();
				PackageDependency packageDependency = new PackageDependency();
				while (packageDependencies.GetHasCurrent())
				{
					IAppxManifestPackageDependency current2 = packageDependencies.GetCurrent();
					packageDependency.Name = current2.GetName();
					packageDependency.MinVersion = current2.GetMinVersion().ToString();
					this._packageDependencies.Add(packageDependency);
					packageDependencies.MoveNext();
				}
			}
			catch (COMException ex)
			{
				if (ex.ErrorCode == -2147221164)
				{
					throw new ArgumentException("The system failed to find the appropriate AppxPackaging class registration when trying to parse appx or appxbundle.");
				}
				LogUtil.Error("An exception occured while trying to extract properties from the AppxPackage file.");
				throw;
			}
			if (string.IsNullOrEmpty(this._title))
			{
				stringBuilder.AppendLine("Title is not defined in the manifest");
			}
			if (string.IsNullOrEmpty(this._version))
			{
				stringBuilder.AppendLine("Version is not defined in the manifest");
			}
			if (string.IsNullOrEmpty(this._publisher))
			{
				stringBuilder.AppendLine("Publisher is not defined in the manifest");
			}
			if (!string.IsNullOrEmpty(stringBuilder.ToString()))
			{
				LogUtil.Error(stringBuilder.ToString());
				throw new InvalidDataException(stringBuilder.ToString());
			}
		}
	}
}
