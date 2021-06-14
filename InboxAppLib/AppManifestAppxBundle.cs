using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.ImageUpdate.InboxAppLib
{
	// Token: 0x02000004 RID: 4
	public class AppManifestAppxBundle : AppManifestAppxBase
	{
		// Token: 0x06000017 RID: 23 RVA: 0x00002551 File Offset: 0x00000751
		public AppManifestAppxBundle(string packageBasePath, string manifestBasePath) : base(packageBasePath, manifestBasePath)
		{
			this._manifestType = AppManifestAppxBase.AppxManifestType.Bundle;
			this._isBundle = true;
		}

		// Token: 0x06000018 RID: 24 RVA: 0x00002574 File Offset: 0x00000774
		public override void ReadManifest()
		{
			StringBuilder stringBuilder = new StringBuilder();
			LogUtil.Message("Parsing BundleManifest: {0}", new object[]
			{
				this._packageBasePath
			});
			try
			{
				IStream inputStream = StreamFactory.CreateFileStream(this._packageBasePath);
				IAppxBundleReader appxBundleReader = ((IAppxBundleFactory)new AppxBundleFactory()).CreateBundleReader(inputStream);
				IAppxBundleManifestReader manifest = appxBundleReader.GetManifest();
				IAppxManifestPackageId packageId = manifest.GetPackageId();
				this._title = packageId.GetName();
				this._publisher = packageId.GetPublisher();
				this._version = packageId.GetVersion().ToString();
				this._packageFullName = packageId.GetPackageFullName();
				appxBundleReader.GetPayloadPackages();
				IAppxBundleManifestPackageInfoEnumerator packageInfoItems = manifest.GetPackageInfoItems();
				while (packageInfoItems.GetHasCurrent())
				{
					IAppxBundleManifestPackageInfo current = packageInfoItems.GetCurrent();
					IAppxManifestPackageId packageId2 = current.GetPackageId();
					string value = string.Empty;
					AppManifestAppxBundle.BundlePackage bundlePackage = new AppManifestAppxBundle.BundlePackage(current.GetPackageType());
					bundlePackage.FileName = current.GetFileName();
					bundlePackage.ProcessorArchitecture = packageId2.GetArchitecture().ToString();
					bundlePackage.Version = packageId2.GetVersion().ToString();
					bundlePackage.ResourceID = packageId2.GetResourceId();
					IAppxManifestQualifiedResourcesEnumerator resources = current.GetResources();
					while (resources.GetHasCurrent())
					{
						IAppxManifestQualifiedResource current2 = resources.GetCurrent();
						DX_FEATURE_LEVEL dxfeatureLevel = current2.GetDXFeatureLevel();
						uint scale = current2.GetScale();
						value = current2.GetLanguage();
						if (scale != 0U)
						{
							bundlePackage.Resources.Add(new AppManifestAppxBase.Resource("Scale", scale.ToString()));
						}
						else if (!string.IsNullOrEmpty(current2.GetLanguage()))
						{
							bundlePackage.Resources.Add(new AppManifestAppxBase.Resource("Language", value));
							value = string.Empty;
						}
						else if (dxfeatureLevel != DX_FEATURE_LEVEL.DX_FEATURE_LEVEL_UNSPECIFIED)
						{
							bundlePackage.Resources.Add(new AppManifestAppxBase.Resource("DXFeatureLevel", dxfeatureLevel.ToString()));
						}
						resources.MoveNext();
					}
					this._bundlePackages.Add(bundlePackage);
					packageInfoItems.MoveNext();
				}
			}
			catch (COMException ex)
			{
				if (ex.ErrorCode == -2147221164)
				{
					throw new ArgumentException("The appxbundle provided has an invalid manifest.");
				}
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

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000019 RID: 25 RVA: 0x00002824 File Offset: 0x00000A24
		public List<AppManifestAppxBundle.BundlePackage> BundlePackages
		{
			get
			{
				return this._bundlePackages;
			}
		}

		// Token: 0x04000013 RID: 19
		private readonly List<AppManifestAppxBundle.BundlePackage> _bundlePackages = new List<AppManifestAppxBundle.BundlePackage>();

		// Token: 0x0200004F RID: 79
		public sealed class BundlePackage : IComparable, IComparer
		{
			// Token: 0x06000121 RID: 289 RVA: 0x00007128 File Offset: 0x00005328
			public BundlePackage(APPX_BUNDLE_PAYLOAD_PACKAGE_TYPE packageType)
			{
				this.PackageType = packageType;
			}

			// Token: 0x06000122 RID: 290 RVA: 0x00007142 File Offset: 0x00005342
			public int CompareTo(object obj)
			{
				return this.Compare(this, obj);
			}

			// Token: 0x06000123 RID: 291 RVA: 0x0000714C File Offset: 0x0000534C
			public int Compare(object obj1, object obj2)
			{
				if (obj1 is AppManifestAppxBundle.BundlePackage && obj2 is AppManifestAppxBundle.BundlePackage)
				{
					AppManifestAppxBundle.BundlePackage bundlePackage = obj1 as AppManifestAppxBundle.BundlePackage;
					AppManifestAppxBundle.BundlePackage bundlePackage2 = obj2 as AppManifestAppxBundle.BundlePackage;
					return string.Compare(bundlePackage.FileName, bundlePackage2.FileName, StringComparison.OrdinalIgnoreCase);
				}
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "cannot compare objects of type {0} against type {1}", new object[]
				{
					obj1.GetType(),
					obj2.GetType()
				}));
			}

			// Token: 0x06000124 RID: 292 RVA: 0x000071B4 File Offset: 0x000053B4
			public override bool Equals(object obj)
			{
				return this.Compare(this, obj) == 0;
			}

			// Token: 0x06000125 RID: 293 RVA: 0x00007081 File Offset: 0x00005281
			public override int GetHashCode()
			{
				return base.GetHashCode();
			}

			// Token: 0x06000126 RID: 294 RVA: 0x000071C1 File Offset: 0x000053C1
			public override string ToString()
			{
				return string.Format(CultureInfo.InvariantCulture, "(PackageType)=\"{0}\", (FileName)=\"{1}\", (Architecture)=\"{2}\"", new object[]
				{
					this.PackageType,
					this.FileName,
					this.ProcessorArchitecture
				});
			}

			// Token: 0x17000035 RID: 53
			// (get) Token: 0x06000127 RID: 295 RVA: 0x000071F8 File Offset: 0x000053F8
			// (set) Token: 0x06000128 RID: 296 RVA: 0x00007200 File Offset: 0x00005400
			public APPX_BUNDLE_PAYLOAD_PACKAGE_TYPE PackageType { get; set; }

			// Token: 0x17000036 RID: 54
			// (get) Token: 0x06000129 RID: 297 RVA: 0x00007209 File Offset: 0x00005409
			// (set) Token: 0x0600012A RID: 298 RVA: 0x00007211 File Offset: 0x00005411
			public string Version { get; set; }

			// Token: 0x17000037 RID: 55
			// (get) Token: 0x0600012B RID: 299 RVA: 0x0000721A File Offset: 0x0000541A
			// (set) Token: 0x0600012C RID: 300 RVA: 0x00007222 File Offset: 0x00005422
			public string ProcessorArchitecture { get; set; }

			// Token: 0x17000038 RID: 56
			// (get) Token: 0x0600012D RID: 301 RVA: 0x0000722B File Offset: 0x0000542B
			// (set) Token: 0x0600012E RID: 302 RVA: 0x00007233 File Offset: 0x00005433
			public string FileName { get; set; }

			// Token: 0x17000039 RID: 57
			// (get) Token: 0x0600012F RID: 303 RVA: 0x0000723C File Offset: 0x0000543C
			// (set) Token: 0x06000130 RID: 304 RVA: 0x00007244 File Offset: 0x00005444
			public string ResourceID { get; set; }

			// Token: 0x1700003A RID: 58
			// (get) Token: 0x06000131 RID: 305 RVA: 0x0000724D File Offset: 0x0000544D
			public List<AppManifestAppxBase.Resource> Resources
			{
				get
				{
					return this._resources;
				}
			}

			// Token: 0x040000E3 RID: 227
			private readonly List<AppManifestAppxBase.Resource> _resources = new List<AppManifestAppxBase.Resource>();
		}
	}
}
