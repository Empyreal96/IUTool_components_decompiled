using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents.Internal;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.Interfaces;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents
{
	// Token: 0x02000043 RID: 67
	internal class PackageWrapper : IPkgProject
	{
		// Token: 0x0600010A RID: 266 RVA: 0x0000597C File Offset: 0x00003B7C
		internal PackageWrapper(IPackageGenerator packageGenerator, PackageProject package)
		{
			this.packageLogger = new PackageLogger();
			this.packageGenerator = packageGenerator;
			this.package = package;
		}

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x0600010B RID: 267 RVA: 0x0000599D File Offset: 0x00003B9D
		public string TempDirectory
		{
			get
			{
				return this.packageGenerator.TempDirectory;
			}
		}

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x0600010C RID: 268 RVA: 0x000059AA File Offset: 0x00003BAA
		public IPkgLogger Log
		{
			get
			{
				return this.packageLogger;
			}
		}

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x0600010D RID: 269 RVA: 0x000059B2 File Offset: 0x00003BB2
		public IMacroResolver MacroResolver
		{
			get
			{
				return this.packageGenerator.MacroResolver;
			}
		}

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x0600010E RID: 270 RVA: 0x000059C0 File Offset: 0x00003BC0
		public IDictionary<string, string> Attributes
		{
			get
			{
				return new Dictionary<string, string>
				{
					{
						"Name",
						this.package.Name
					},
					{
						"Owner",
						this.package.Owner
					},
					{
						"Partition",
						this.package.Partition
					},
					{
						"Platform",
						this.package.Platform
					}
				};
			}
		}

		// Token: 0x0600010F RID: 271 RVA: 0x00005A2A File Offset: 0x00003C2A
		public IEnumerable<SatelliteId> GetSatelliteValues(SatelliteType type)
		{
			return this.packageGenerator.GetSatelliteValues(type);
		}

		// Token: 0x06000110 RID: 272 RVA: 0x00005A38 File Offset: 0x00003C38
		public void AddToCapabilities(XElement element)
		{
			this.package.AddToCapabilities(element);
		}

		// Token: 0x06000111 RID: 273 RVA: 0x00005A46 File Offset: 0x00003C46
		public void AddToAuthorization(XElement element)
		{
			this.package.AddToAuthorization(element);
		}

		// Token: 0x040000EF RID: 239
		private PackageProject package;

		// Token: 0x040000F0 RID: 240
		private IPackageGenerator packageGenerator;

		// Token: 0x040000F1 RID: 241
		private PackageLogger packageLogger;
	}
}
