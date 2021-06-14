using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.WindowsPhone.ImageUpdate.PkgCommon;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents.Internal
{
	// Token: 0x0200006C RID: 108
	public class PackageSetBuilderBase
	{
		// Token: 0x060001F2 RID: 498 RVA: 0x000078EC File Offset: 0x00005AEC
		protected PackageSetBuilderBase(CpuId cpuType, BuildType bldType, VersionInfo version)
		{
			this.CpuType = cpuType;
			this.BuildType = bldType;
			this.Version = version;
			this.Resolutions = new List<SatelliteId>();
		}

		// Token: 0x060001F3 RID: 499 RVA: 0x00007920 File Offset: 0x00005B20
		protected IPkgBuilder CreatePackage(SatelliteId satelliteId)
		{
			IPkgBuilder pkgBuilder = Package.Create();
			pkgBuilder.BuildType = this.BuildType;
			pkgBuilder.CpuType = this.CpuType;
			pkgBuilder.Version = this.Version;
			pkgBuilder.Owner = this.Owner;
			pkgBuilder.OwnerType = this.OwnerType;
			pkgBuilder.Component = this.Component;
			pkgBuilder.SubComponent = this.SubComponent;
			pkgBuilder.ReleaseType = this.ReleaseType;
			pkgBuilder.Partition = this.Partition;
			pkgBuilder.Platform = this.Platform;
			pkgBuilder.GroupingKey = this.GroupingKey;
			pkgBuilder.BuildString = this.Description;
			pkgBuilder.Resolution = satelliteId.Resolution;
			pkgBuilder.Culture = satelliteId.Culture;
			return pkgBuilder;
		}

		// Token: 0x17000077 RID: 119
		// (get) Token: 0x060001F4 RID: 500 RVA: 0x000079DA File Offset: 0x00005BDA
		// (set) Token: 0x060001F5 RID: 501 RVA: 0x000079E2 File Offset: 0x00005BE2
		public string Owner { get; set; }

		// Token: 0x17000078 RID: 120
		// (get) Token: 0x060001F6 RID: 502 RVA: 0x000079EB File Offset: 0x00005BEB
		// (set) Token: 0x060001F7 RID: 503 RVA: 0x000079F3 File Offset: 0x00005BF3
		public string Component { get; set; }

		// Token: 0x17000079 RID: 121
		// (get) Token: 0x060001F8 RID: 504 RVA: 0x000079FC File Offset: 0x00005BFC
		// (set) Token: 0x060001F9 RID: 505 RVA: 0x00007A04 File Offset: 0x00005C04
		public string SubComponent { get; set; }

		// Token: 0x1700007A RID: 122
		// (get) Token: 0x060001FA RID: 506 RVA: 0x00007A0D File Offset: 0x00005C0D
		public string Name
		{
			get
			{
				return PackageTools.BuildPackageName(this.Owner, this.Component, this.SubComponent);
			}
		}

		// Token: 0x1700007B RID: 123
		// (get) Token: 0x060001FB RID: 507 RVA: 0x00007A26 File Offset: 0x00005C26
		// (set) Token: 0x060001FC RID: 508 RVA: 0x00007A2E File Offset: 0x00005C2E
		public OwnerType OwnerType { get; set; }

		// Token: 0x1700007C RID: 124
		// (get) Token: 0x060001FD RID: 509 RVA: 0x00007A37 File Offset: 0x00005C37
		// (set) Token: 0x060001FE RID: 510 RVA: 0x00007A3F File Offset: 0x00005C3F
		public ReleaseType ReleaseType { get; set; }

		// Token: 0x1700007D RID: 125
		// (get) Token: 0x060001FF RID: 511 RVA: 0x00007A48 File Offset: 0x00005C48
		// (set) Token: 0x06000200 RID: 512 RVA: 0x00007A50 File Offset: 0x00005C50
		public string Partition
		{
			get
			{
				return this._partition;
			}
			set
			{
				this._partition = value;
				this._driveLetter = PackageTools.GetDefaultDriveLetter(value);
			}
		}

		// Token: 0x1700007E RID: 126
		// (get) Token: 0x06000201 RID: 513 RVA: 0x00007A65 File Offset: 0x00005C65
		// (set) Token: 0x06000202 RID: 514 RVA: 0x00007A6D File Offset: 0x00005C6D
		public string Platform { get; set; }

		// Token: 0x1700007F RID: 127
		// (get) Token: 0x06000203 RID: 515 RVA: 0x00007A76 File Offset: 0x00005C76
		// (set) Token: 0x06000204 RID: 516 RVA: 0x00007A7E File Offset: 0x00005C7E
		public string GroupingKey { get; set; }

		// Token: 0x17000080 RID: 128
		// (get) Token: 0x06000205 RID: 517 RVA: 0x00007A87 File Offset: 0x00005C87
		// (set) Token: 0x06000206 RID: 518 RVA: 0x00007A8F File Offset: 0x00005C8F
		public string Description { get; set; }

		// Token: 0x17000081 RID: 129
		// (get) Token: 0x06000207 RID: 519 RVA: 0x00007A98 File Offset: 0x00005C98
		public List<SatelliteId> Resolutions { get; }

		// Token: 0x17000082 RID: 130
		// (get) Token: 0x06000208 RID: 520 RVA: 0x00007AA0 File Offset: 0x00005CA0
		// (set) Token: 0x06000209 RID: 521 RVA: 0x00007AA8 File Offset: 0x00005CA8
		public CpuId CpuType { get; private set; }

		// Token: 0x17000083 RID: 131
		// (get) Token: 0x0600020A RID: 522 RVA: 0x00007AB1 File Offset: 0x00005CB1
		// (set) Token: 0x0600020B RID: 523 RVA: 0x00007AB9 File Offset: 0x00005CB9
		public BuildType BuildType { get; private set; }

		// Token: 0x17000084 RID: 132
		// (get) Token: 0x0600020C RID: 524 RVA: 0x00007AC2 File Offset: 0x00005CC2
		// (set) Token: 0x0600020D RID: 525 RVA: 0x00007ACA File Offset: 0x00005CCA
		public VersionInfo Version { get; private set; }

		// Token: 0x0600020E RID: 526 RVA: 0x00007AD4 File Offset: 0x00005CD4
		public void AddFile(SatelliteId satelliteId, FileInfo fileInfo)
		{
			if (fileInfo.DevicePath != null)
			{
				if (satelliteId.SatType == SatelliteType.Language && fileInfo.DevicePath.IndexOf(satelliteId.Id, StringComparison.InvariantCultureIgnoreCase) == -1)
				{
					throw new PkgGenException("A file is added as a langauge specific file, but the destination path '{0}' is not langauge specific", new object[]
					{
						fileInfo.DevicePath
					});
				}
				if (satelliteId.SatType == SatelliteType.Resolution)
				{
					bool flag = false;
					foreach (SatelliteId satelliteId2 in this.Resolutions)
					{
						if (satelliteId.Id == satelliteId2.Id)
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						throw new PkgGenException("A file is added as a resolution specific file, but the destination path '{0}' is not resolution specific", new object[]
						{
							fileInfo.DevicePath
						});
					}
				}
				if (!Path.IsPathRooted(fileInfo.DevicePath))
				{
					throw new PkgGenException("Invalid device path '{0}', absolute path is required", new object[]
					{
						fileInfo.DevicePath
					});
				}
				string pathRoot = Path.GetPathRoot(fileInfo.DevicePath.Substring(0, Math.Min(fileInfo.DevicePath.Length, 200)));
				if (!pathRoot.EndsWith("\\", StringComparison.InvariantCultureIgnoreCase))
				{
					throw new PkgGenException("Invalid device path '{0}', absolute path is required", new object[]
					{
						fileInfo.DevicePath
					});
				}
				if (!pathRoot.StartsWith("\\", StringComparison.InvariantCultureIgnoreCase))
				{
					if (!pathRoot.StartsWith(this._driveLetter, StringComparison.OrdinalIgnoreCase))
					{
						throw new PkgGenException("Invalid device path '{0}', only drive '{1}' can be used", new object[]
						{
							fileInfo.DevicePath,
							this._driveLetter
						});
					}
					fileInfo.DevicePath = fileInfo.DevicePath.Substring(this._driveLetter.Length);
				}
			}
			this._allFiles.Add(new KeyValuePair<SatelliteId, FileInfo>(satelliteId, fileInfo));
		}

		// Token: 0x04000170 RID: 368
		private string _partition;

		// Token: 0x04000171 RID: 369
		private string _driveLetter;

		// Token: 0x04000172 RID: 370
		protected List<KeyValuePair<SatelliteId, FileInfo>> _allFiles = new List<KeyValuePair<SatelliteId, FileInfo>>();
	}
}
