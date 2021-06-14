using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.WindowsPhone.ImageUpdate.InboxAppLib.Interfaces;

namespace Microsoft.WindowsPhone.ImageUpdate.InboxAppLib
{
	// Token: 0x02000003 RID: 3
	public abstract class AppManifestAppxBase : IInboxAppManifest
	{
		// Token: 0x06000003 RID: 3 RVA: 0x00002324 File Offset: 0x00000524
		public static AppManifestAppxBase CreateAppxManifest(string packageBasePath, string manifestBasePath, bool isBundle)
		{
			AppManifestAppxBase result;
			if (!isBundle)
			{
				result = new AppManifestAppx(packageBasePath, manifestBasePath);
			}
			else
			{
				result = new AppManifestAppxBundle(packageBasePath, manifestBasePath);
			}
			return result;
		}

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000004 RID: 4 RVA: 0x00002349 File Offset: 0x00000549
		public string Filename
		{
			get
			{
				return Path.GetFileName(this._manifestBasePath);
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000005 RID: 5 RVA: 0x00002356 File Offset: 0x00000556
		public string Title
		{
			get
			{
				return this._title;
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000006 RID: 6 RVA: 0x0000235E File Offset: 0x0000055E
		public string Description
		{
			get
			{
				return this._description;
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000007 RID: 7 RVA: 0x00002366 File Offset: 0x00000566
		public string Publisher
		{
			get
			{
				return this._publisher;
			}
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000008 RID: 8 RVA: 0x0000236E File Offset: 0x0000056E
		public List<string> Capabilities
		{
			get
			{
				return this._capabilities;
			}
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000009 RID: 9 RVA: 0x00002376 File Offset: 0x00000576
		public string ProductID
		{
			get
			{
				return this._productID;
			}
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x0600000A RID: 10 RVA: 0x0000237E File Offset: 0x0000057E
		public string Version
		{
			get
			{
				return this._version;
			}
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x0600000B RID: 11 RVA: 0x00002386 File Offset: 0x00000586
		public APPX_PACKAGE_ARCHITECTURE ProcessorArchitecture
		{
			get
			{
				return this._processorArchitecture;
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x0600000C RID: 12 RVA: 0x0000238E File Offset: 0x0000058E
		public string ResourceID
		{
			get
			{
				return this._resourceID;
			}
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x0600000D RID: 13 RVA: 0x00002396 File Offset: 0x00000596
		public string PackageFullName
		{
			get
			{
				return this._packageFullName;
			}
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x0600000E RID: 14 RVA: 0x0000239E File Offset: 0x0000059E
		public bool IsFramework
		{
			get
			{
				return this._isFramework;
			}
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x0600000F RID: 15 RVA: 0x000023A6 File Offset: 0x000005A6
		public bool IsBundle
		{
			get
			{
				return this._isBundle;
			}
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000010 RID: 16 RVA: 0x000023AE File Offset: 0x000005AE
		public bool IsResource
		{
			get
			{
				return this._isResource;
			}
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000011 RID: 17 RVA: 0x000023B6 File Offset: 0x000005B6
		public List<PackageDependency> PackageDependencies
		{
			get
			{
				return this._packageDependencies;
			}
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000012 RID: 18 RVA: 0x000023BE File Offset: 0x000005BE
		public List<AppManifestAppxBase.Resource> Resources
		{
			get
			{
				return this._resources;
			}
		}

		// Token: 0x06000013 RID: 19 RVA: 0x000023C8 File Offset: 0x000005C8
		protected AppManifestAppxBase(string packageBasePath, string manifestBasePath)
		{
			if (!string.IsNullOrEmpty(packageBasePath))
			{
				this._packageBasePath = InboxAppUtils.ValidateFileOrDir(packageBasePath, false);
				this._manifestBasePath = "AppxManifest.xml";
				return;
			}
			if (!string.IsNullOrEmpty(manifestBasePath))
			{
				this._manifestBasePath = InboxAppUtils.ValidateFileOrDir(manifestBasePath, false);
				return;
			}
			throw new FileNotFoundException("The package or manifest name needs to be populated.");
		}

		// Token: 0x06000014 RID: 20 RVA: 0x000024B4 File Offset: 0x000006B4
		protected void PopulateDefaultPackageProperties(IAppxManifestPackageId packageId)
		{
			this._title = packageId.GetName();
			this._publisher = packageId.GetPublisher();
			this._version = packageId.GetVersion().ToString();
			this._processorArchitecture = packageId.GetArchitecture();
			this._resourceID = packageId.GetResourceId();
			this._packageFullName = packageId.GetPackageFullName();
		}

		// Token: 0x06000015 RID: 21
		public abstract void ReadManifest();

		// Token: 0x06000016 RID: 22 RVA: 0x00002511 File Offset: 0x00000711
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "Appx manifest: (Type): {0}, (Filename): \"{1}\", (Title): \"{2}\", (PackageFullName): \"{3}\" ", new object[]
			{
				this._manifestType,
				this.Filename,
				this.Title,
				this.PackageFullName
			});
		}

		// Token: 0x04000001 RID: 1
		protected string _title = string.Empty;

		// Token: 0x04000002 RID: 2
		protected string _description = string.Empty;

		// Token: 0x04000003 RID: 3
		protected string _publisher = string.Empty;

		// Token: 0x04000004 RID: 4
		protected List<string> _capabilities = new List<string>();

		// Token: 0x04000005 RID: 5
		protected string _version = string.Empty;

		// Token: 0x04000006 RID: 6
		protected string _manifestBasePath = string.Empty;

		// Token: 0x04000007 RID: 7
		protected string _manifestDestinationPath = string.Empty;

		// Token: 0x04000008 RID: 8
		protected string _packageBasePath = string.Empty;

		// Token: 0x04000009 RID: 9
		protected string _productID = string.Empty;

		// Token: 0x0400000A RID: 10
		protected string _packageFullName = string.Empty;

		// Token: 0x0400000B RID: 11
		protected APPX_PACKAGE_ARCHITECTURE _processorArchitecture = APPX_PACKAGE_ARCHITECTURE.APPX_PACKAGE_ARCHITECTURE_NEUTRAL;

		// Token: 0x0400000C RID: 12
		protected string _resourceID = string.Empty;

		// Token: 0x0400000D RID: 13
		protected bool _isFramework;

		// Token: 0x0400000E RID: 14
		protected bool _isBundle;

		// Token: 0x0400000F RID: 15
		protected bool _isResource;

		// Token: 0x04000010 RID: 16
		protected readonly List<PackageDependency> _packageDependencies = new List<PackageDependency>();

		// Token: 0x04000011 RID: 17
		protected readonly List<AppManifestAppxBase.Resource> _resources = new List<AppManifestAppxBase.Resource>();

		// Token: 0x04000012 RID: 18
		protected AppManifestAppxBase.AppxManifestType _manifestType;

		// Token: 0x0200004D RID: 77
		public enum AppxManifestType
		{
			// Token: 0x040000D9 RID: 217
			Undefined,
			// Token: 0x040000DA RID: 218
			MainPackage,
			// Token: 0x040000DB RID: 219
			Bundle
		}

		// Token: 0x0200004E RID: 78
		public sealed class Resource
		{
			// Token: 0x0600011E RID: 286 RVA: 0x000070EC File Offset: 0x000052EC
			public Resource(string key, string value)
			{
				this._key = key;
				this._value = value;
			}

			// Token: 0x17000033 RID: 51
			// (get) Token: 0x0600011F RID: 287 RVA: 0x00007118 File Offset: 0x00005318
			public string Key
			{
				get
				{
					return this._key;
				}
			}

			// Token: 0x17000034 RID: 52
			// (get) Token: 0x06000120 RID: 288 RVA: 0x00007120 File Offset: 0x00005320
			public string Value
			{
				get
				{
					return this._value;
				}
			}

			// Token: 0x040000DC RID: 220
			private string _key = string.Empty;

			// Token: 0x040000DD RID: 221
			private string _value = string.Empty;
		}
	}
}
