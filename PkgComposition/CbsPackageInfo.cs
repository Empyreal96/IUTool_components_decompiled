using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Composition.Packaging.Interfaces;
using Microsoft.Composition.ToolBox;
using Microsoft.Composition.ToolBox.Cab;
using Microsoft.Composition.ToolBox.IO;
using Microsoft.Composition.ToolBox.Logging;

namespace Microsoft.Composition.Packaging
{
	// Token: 0x02000004 RID: 4
	public class CbsPackageInfo : IPackageInfo
	{
		// Token: 0x0600005D RID: 93 RVA: 0x00003693 File Offset: 0x00001893
		public CbsPackageInfo(string sourcePath)
		{
			this.Initialize(sourcePath, new Logger());
		}

		// Token: 0x0600005E RID: 94 RVA: 0x000036A7 File Offset: 0x000018A7
		public CbsPackageInfo(string sourcePath, Logger logger)
		{
			this.Initialize(sourcePath, logger);
		}

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x0600005F RID: 95 RVA: 0x000036B7 File Offset: 0x000018B7
		public string PackageName
		{
			get
			{
				return this.root.Keyform.Name;
			}
		}

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x06000060 RID: 96 RVA: 0x000036C9 File Offset: 0x000018C9
		public string Owner
		{
			get
			{
				return this.root.PhoneInformation.PhoneOwner;
			}
		}

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x06000061 RID: 97 RVA: 0x000036DB File Offset: 0x000018DB
		public PhoneReleaseType PhoneReleaseType
		{
			get
			{
				return this.root.PhoneInformation.PhoneReleaseType;
			}
		}

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x06000062 RID: 98 RVA: 0x000036ED File Offset: 0x000018ED
		public string Component
		{
			get
			{
				return this.root.PhoneInformation.PhoneComponent;
			}
		}

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x06000063 RID: 99 RVA: 0x000036FF File Offset: 0x000018FF
		public string SubComponent
		{
			get
			{
				return this.root.PhoneInformation.PhoneSubComponent;
			}
		}

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x06000064 RID: 100 RVA: 0x00003711 File Offset: 0x00001911
		public Version Version
		{
			get
			{
				return this.root.Keyform.Version;
			}
		}

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x06000065 RID: 101 RVA: 0x00003723 File Offset: 0x00001923
		public PhoneOwnerType OwnerType
		{
			get
			{
				return this.root.PhoneInformation.PhoneOwnerType;
			}
		}

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000066 RID: 102 RVA: 0x00003735 File Offset: 0x00001935
		public string ReleaseType
		{
			get
			{
				return this.root.Keyform.ReleaseType;
			}
		}

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x06000067 RID: 103 RVA: 0x00003747 File Offset: 0x00001947
		public BuildType BuildType
		{
			get
			{
				return this.root.Keyform.BuildType;
			}
		}

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x06000068 RID: 104 RVA: 0x00003759 File Offset: 0x00001959
		public CpuArch HostArch
		{
			get
			{
				return this.root.Keyform.HostArch;
			}
		}

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x06000069 RID: 105 RVA: 0x0000376B File Offset: 0x0000196B
		public CpuArch GuestArch
		{
			get
			{
				return this.root.Keyform.GuestArch;
			}
		}

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x0600006A RID: 106 RVA: 0x0000377D File Offset: 0x0000197D
		public string CpuString
		{
			get
			{
				return ManifestToolBox.GetCpuString(this.root.Keyform);
			}
		}

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x0600006B RID: 107 RVA: 0x0000378F File Offset: 0x0000198F
		public bool IsWow
		{
			get
			{
				return this.GuestArch > CpuArch.Invalid;
			}
		}

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x0600006C RID: 108 RVA: 0x0000379A File Offset: 0x0000199A
		public string Culture
		{
			get
			{
				return this.root.Keyform.Language;
			}
		}

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x0600006D RID: 109 RVA: 0x000037AC File Offset: 0x000019AC
		public string PublicKey
		{
			get
			{
				return this.root.Keyform.PublicKeyToken;
			}
		}

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x0600006E RID: 110 RVA: 0x000037BE File Offset: 0x000019BE
		public string GroupingKey
		{
			get
			{
				return this.root.PhoneInformation.PhoneGroupingKey;
			}
		}

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x0600006F RID: 111 RVA: 0x000037D0 File Offset: 0x000019D0
		public string Partition
		{
			get
			{
				return this.root.Partition;
			}
		}

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x06000070 RID: 112 RVA: 0x000037DD File Offset: 0x000019DD
		public bool? BinaryPartition
		{
			get
			{
				return this.root.BinaryPartition;
			}
		}

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x06000071 RID: 113 RVA: 0x000037EA File Offset: 0x000019EA
		public PackageType PackageType
		{
			get
			{
				return this.Metrics.PackageType;
			}
		}

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x06000072 RID: 114 RVA: 0x000037F7 File Offset: 0x000019F7
		// (set) Token: 0x06000073 RID: 115 RVA: 0x000037FF File Offset: 0x000019FF
		public PackageMetrics Metrics { get; private set; }

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x06000074 RID: 116 RVA: 0x00003808 File Offset: 0x00001A08
		public bool IsFIPPackage
		{
			get
			{
				return this.root.GetCBSFeatureInfo() != null;
			}
		}

		// Token: 0x06000075 RID: 117 RVA: 0x00003818 File Offset: 0x00001A18
		private void Initialize(string sourcePath, Logger logger)
		{
			if (!FileToolBox.Extension(sourcePath).Equals(PkgConstants.MumExtension, StringComparison.InvariantCultureIgnoreCase))
			{
				string temporaryPath = PathToolBox.GetTemporaryPath();
				List<string> list = new List<string>();
				list.Add(PkgConstants.UpdateMum);
				DirectoryToolBox.Create(temporaryPath);
				CabToolBox.ExtractSelected(sourcePath, temporaryPath, list);
				sourcePath = PathToolBox.Combine(temporaryPath, PkgConstants.UpdateMum);
			}
			this.logger = logger;
			this.Metrics = new PackageMetrics(this.logger, LoadType.PackageInfo);
			this.Metrics.PackageType = this.GetPackageType(sourcePath);
			this.Load(sourcePath);
		}

		// Token: 0x06000076 RID: 118 RVA: 0x0000389C File Offset: 0x00001A9C
		private PackageType GetPackageType(string sourcePath)
		{
			PackageType result = PackageType.Regular;
			XElement xelement = ManifestToolBox.Load(sourcePath).Root;
			XContainer xcontainer = xelement.Elements(xelement.Name.Namespace + "package").First<XElement>();
			XElement xelement2 = xelement.Element(xelement.Name.Namespace + "assemblyIdentity");
			XElement xelement3 = xcontainer.Element(xelement.Name.Namespace + "customInformation");
			if (xelement3 != null)
			{
				bool flag = xelement3.Element(xelement.Name.Namespace + "phoneInformation") != null;
				XElement xelement4 = xelement3.Element(xelement.Name.Namespace + "declareCapability");
				if (flag)
				{
					result = PackageType.OneCore;
				}
				else if (xelement4 != null && xelement4.Descendants(xelement.Name.Namespace + "featurePackage").Count<XElement>() == 0)
				{
					if (xelement2.Attribute(xelement.Name.Namespace + "language").Value.ToString().Equals(PkgConstants.NeutralCulture, StringComparison.InvariantCultureIgnoreCase))
					{
						result = PackageType.FOD_Neutral;
					}
					else
					{
						result = PackageType.FOD_Lang;
					}
				}
			}
			return result;
		}

		// Token: 0x06000077 RID: 119 RVA: 0x000039B4 File Offset: 0x00001BB4
		private void Load(string sourcePath)
		{
			try
			{
				this.logger.LogInfo("Loading Package:" + sourcePath, new object[0]);
				this.Metrics.StartOperation();
				this.root = PkgManifest.LoadFromDisk(sourcePath, this.Metrics);
				this.logger.LogInfo("\nPackage Info...", new object[0]);
				this.logger.LogInfo("Top Level Package : {0}", new object[]
				{
					this.PackageName
				});
				this.logger.LogInfo("Architecture      : {0}", new object[]
				{
					this.HostArch
				});
				this.logger.LogInfo("Version           : {0}", new object[]
				{
					this.Version
				});
				this.logger.LogInfo("Culture           : {0}", new object[]
				{
					this.Culture
				});
				this.logger.LogInfo("Release Type      : {0}", new object[]
				{
					this.ReleaseType
				});
				this.logger.LogInfo("Public Key        : {0}", new object[]
				{
					this.PublicKey
				});
				this.logger.LogInfo("Target Partition  : {0}", new object[]
				{
					this.Partition
				});
				this.logger.LogInfo("Binary Partition  : {0}", new object[]
				{
					this.BinaryPartition
				});
				this.Metrics.StopOperation();
				this.Metrics.LogMetrics();
			}
			catch (Exception)
			{
				throw;
			}
		}

		// Token: 0x04000009 RID: 9
		private Logger logger;

		// Token: 0x0400000A RID: 10
		private IManifest root;
	}
}
