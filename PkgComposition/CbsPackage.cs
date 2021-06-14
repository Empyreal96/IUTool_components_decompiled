using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Composition.Packaging.Interfaces;
using Microsoft.Composition.ToolBox;
using Microsoft.Composition.ToolBox.Cab;
using Microsoft.Composition.ToolBox.IO;
using Microsoft.Composition.ToolBox.Logging;
using Microsoft.Composition.ToolBox.Security;

namespace Microsoft.Composition.Packaging
{
	// Token: 0x02000002 RID: 2
	public class CbsPackage : IPackage
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public CbsPackage(string sourcePath)
		{
			this.Initialize(sourcePath, new Logger(), true);
		}

		// Token: 0x06000002 RID: 2 RVA: 0x00002065 File Offset: 0x00000265
		public CbsPackage(string sourcePath, Logger logger)
		{
			this.Initialize(sourcePath, logger, true);
		}

		// Token: 0x06000003 RID: 3 RVA: 0x00002078 File Offset: 0x00000278
		public CbsPackage(Keyform keyform)
		{
			string outputDirectory = PathToolBox.Combine(PathToolBox.GetTemporaryPath(), PkgConstants.UpdateMum);
			string sourcePath = PkgManifest.SerializeEmptyMum(keyform, outputDirectory);
			this.Initialize(sourcePath, new Logger(), true);
		}

		// Token: 0x06000004 RID: 4 RVA: 0x000020B0 File Offset: 0x000002B0
		public CbsPackage(Keyform keyform, Logger logger)
		{
			string outputDirectory = PathToolBox.Combine(PathToolBox.GetTemporaryPath(), PkgConstants.UpdateMum);
			string sourcePath = PkgManifest.SerializeEmptyMum(keyform, outputDirectory);
			this.Initialize(sourcePath, logger, true);
		}

		// Token: 0x06000005 RID: 5 RVA: 0x000020E4 File Offset: 0x000002E4
		public CbsPackage()
		{
			Keyform keyform = new Keyform(string.Empty, CpuArch.Invalid, CpuArch.Invalid, BuildType.Invalid, string.Empty, string.Empty, PkgConstants.SxSVersionScope, PkgConstants.InvalidVersion, PkgConstants.DefaultPublicKeyToken);
			string outputDirectory = PathToolBox.Combine(PathToolBox.GetTemporaryPath(), PkgConstants.UpdateMum);
			string sourcePath = PkgManifest.SerializeEmptyMum(keyform, outputDirectory);
			this.Initialize(sourcePath, new Logger(), false);
		}

		// Token: 0x06000006 RID: 6 RVA: 0x00002144 File Offset: 0x00000344
		public CbsPackage(Logger logger)
		{
			Keyform keyform = new Keyform(string.Empty, CpuArch.Invalid, CpuArch.Invalid, BuildType.Invalid, string.Empty, string.Empty, PkgConstants.SxSVersionScope, PkgConstants.InvalidVersion, PkgConstants.DefaultPublicKeyToken);
			string outputDirectory = PathToolBox.Combine(PathToolBox.GetTemporaryPath(), PkgConstants.UpdateMum);
			string sourcePath = PkgManifest.SerializeEmptyMum(keyform, outputDirectory);
			this.Initialize(sourcePath, logger, false);
		}

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000007 RID: 7 RVA: 0x0000219D File Offset: 0x0000039D
		// (set) Token: 0x06000008 RID: 8 RVA: 0x000021AF File Offset: 0x000003AF
		public string PackageName
		{
			get
			{
				return this.root.Keyform.Name;
			}
			set
			{
				this.root.Keyform.Name = value;
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000009 RID: 9 RVA: 0x000021C2 File Offset: 0x000003C2
		// (set) Token: 0x0600000A RID: 10 RVA: 0x000021D4 File Offset: 0x000003D4
		public string Owner
		{
			get
			{
				return this.root.PhoneInformation.PhoneOwner;
			}
			set
			{
				this.root.PhoneInformation.PhoneOwner = value;
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600000B RID: 11 RVA: 0x000021E7 File Offset: 0x000003E7
		// (set) Token: 0x0600000C RID: 12 RVA: 0x000021F9 File Offset: 0x000003F9
		public PhoneReleaseType PhoneReleaseType
		{
			get
			{
				return this.root.PhoneInformation.PhoneReleaseType;
			}
			set
			{
				this.root.PhoneInformation.PhoneReleaseType = value;
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x0600000D RID: 13 RVA: 0x0000220C File Offset: 0x0000040C
		// (set) Token: 0x0600000E RID: 14 RVA: 0x0000221E File Offset: 0x0000041E
		public string Component
		{
			get
			{
				return this.root.PhoneInformation.PhoneComponent;
			}
			set
			{
				if (value != null)
				{
					this.Metrics.PackageType = PackageType.OneCore;
				}
				this.root.PhoneInformation.PhoneComponent = value;
			}
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600000F RID: 15 RVA: 0x00002240 File Offset: 0x00000440
		// (set) Token: 0x06000010 RID: 16 RVA: 0x00002252 File Offset: 0x00000452
		public string SubComponent
		{
			get
			{
				return this.root.PhoneInformation.PhoneSubComponent;
			}
			set
			{
				this.root.PhoneInformation.PhoneSubComponent = value;
			}
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000011 RID: 17 RVA: 0x00002265 File Offset: 0x00000465
		// (set) Token: 0x06000012 RID: 18 RVA: 0x00002277 File Offset: 0x00000477
		public Version Version
		{
			get
			{
				return this.root.Keyform.Version;
			}
			set
			{
				this.root.Keyform.Version = value;
			}
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000013 RID: 19 RVA: 0x0000228A File Offset: 0x0000048A
		// (set) Token: 0x06000014 RID: 20 RVA: 0x0000229C File Offset: 0x0000049C
		public PhoneOwnerType OwnerType
		{
			get
			{
				return this.root.PhoneInformation.PhoneOwnerType;
			}
			set
			{
				this.root.PhoneInformation.PhoneOwnerType = value;
			}
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000015 RID: 21 RVA: 0x000022AF File Offset: 0x000004AF
		// (set) Token: 0x06000016 RID: 22 RVA: 0x000022C1 File Offset: 0x000004C1
		public string ReleaseType
		{
			get
			{
				return this.root.Keyform.ReleaseType;
			}
			set
			{
				this.root.Keyform.ReleaseType = value;
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000017 RID: 23 RVA: 0x000022D4 File Offset: 0x000004D4
		// (set) Token: 0x06000018 RID: 24 RVA: 0x000022E6 File Offset: 0x000004E6
		public BuildType BuildType
		{
			get
			{
				return this.root.Keyform.BuildType;
			}
			set
			{
				this.root.Keyform.BuildType = value;
			}
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000019 RID: 25 RVA: 0x000022F9 File Offset: 0x000004F9
		// (set) Token: 0x0600001A RID: 26 RVA: 0x0000230B File Offset: 0x0000050B
		public CpuArch HostArch
		{
			get
			{
				return this.root.Keyform.HostArch;
			}
			set
			{
				this.root.Keyform.HostArch = value;
			}
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x0600001B RID: 27 RVA: 0x0000231E File Offset: 0x0000051E
		// (set) Token: 0x0600001C RID: 28 RVA: 0x00002330 File Offset: 0x00000530
		public CpuArch GuestArch
		{
			get
			{
				return this.root.Keyform.GuestArch;
			}
			set
			{
				this.root.Keyform.GuestArch = value;
			}
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x0600001D RID: 29 RVA: 0x00002343 File Offset: 0x00000543
		public string CpuString
		{
			get
			{
				return ManifestToolBox.GetCpuString(this.root.Keyform);
			}
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x0600001E RID: 30 RVA: 0x00002355 File Offset: 0x00000555
		public bool IsWow
		{
			get
			{
				return this.GuestArch > CpuArch.Invalid;
			}
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x0600001F RID: 31 RVA: 0x00002360 File Offset: 0x00000560
		// (set) Token: 0x06000020 RID: 32 RVA: 0x00002372 File Offset: 0x00000572
		public string Culture
		{
			get
			{
				return this.root.Keyform.Language;
			}
			set
			{
				this.root.Keyform.Language = value;
			}
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000021 RID: 33 RVA: 0x00002385 File Offset: 0x00000585
		// (set) Token: 0x06000022 RID: 34 RVA: 0x00002397 File Offset: 0x00000597
		public string PublicKey
		{
			get
			{
				return this.root.Keyform.PublicKeyToken;
			}
			set
			{
				this.root.Keyform.PublicKeyToken = value;
			}
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000023 RID: 35 RVA: 0x000023AA File Offset: 0x000005AA
		// (set) Token: 0x06000024 RID: 36 RVA: 0x000023BC File Offset: 0x000005BC
		public string GroupingKey
		{
			get
			{
				return this.root.PhoneInformation.PhoneGroupingKey;
			}
			set
			{
				this.root.PhoneInformation.PhoneGroupingKey = value;
			}
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000025 RID: 37 RVA: 0x000023CF File Offset: 0x000005CF
		// (set) Token: 0x06000026 RID: 38 RVA: 0x000023DC File Offset: 0x000005DC
		public string Partition
		{
			get
			{
				return this.root.Partition;
			}
			set
			{
				this.root.Partition = value;
			}
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000027 RID: 39 RVA: 0x000023EA File Offset: 0x000005EA
		// (set) Token: 0x06000028 RID: 40 RVA: 0x000023F7 File Offset: 0x000005F7
		public bool? BinaryPartition
		{
			get
			{
				return this.root.BinaryPartition;
			}
			set
			{
				this.root.BinaryPartition = value;
			}
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000029 RID: 41 RVA: 0x00002405 File Offset: 0x00000605
		public PackageType PackageType
		{
			get
			{
				return this.Metrics.PackageType;
			}
		}

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x0600002A RID: 42 RVA: 0x00002412 File Offset: 0x00000612
		// (set) Token: 0x0600002B RID: 43 RVA: 0x0000241A File Offset: 0x0000061A
		public PackageMetrics Metrics { get; private set; }

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x0600002C RID: 44 RVA: 0x00002423 File Offset: 0x00000623
		public IEnumerable<IFile> AllFiles
		{
			get
			{
				return this.root.AllFiles;
			}
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x0600002D RID: 45 RVA: 0x00002430 File Offset: 0x00000630
		public IEnumerable<IFile> AllPayloadFiles
		{
			get
			{
				return this.root.AllPayloadFiles;
			}
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x0600002E RID: 46 RVA: 0x0000243D File Offset: 0x0000063D
		public IEnumerable<IManifest> AllManifestFiles
		{
			get
			{
				return this.root.AllManifestFiles;
			}
		}

		// Token: 0x0600002F RID: 47 RVA: 0x0000244C File Offset: 0x0000064C
		public static bool CheckCBSFeatureInfo(string fmId, string featureId, string pkgGroup, List<string> pkgPaths, bool ignoreMissingSatellites)
		{
			Dictionary<IPackageInfo, string> dictionary = new Dictionary<IPackageInfo, string>();
			Logger logger = new Logger();
			foreach (string text in pkgPaths)
			{
				if (dictionary.Values.Contains(text))
				{
					logger.LogWarning("CbsPackage::CheckCBSFeatureInfo: Ignoring duplicate package {0} with featureId:{1} and fmId:{2}", new object[]
					{
						text,
						featureId,
						fmId
					});
				}
				else
				{
					dictionary.Add(new CbsPackageInfo(text), text);
				}
			}
			return CbsPackage.CheckCBSFeatureInfo(fmId, featureId, pkgGroup, dictionary, ignoreMissingSatellites, logger);
		}

		// Token: 0x06000030 RID: 48 RVA: 0x000024E8 File Offset: 0x000006E8
		public void Validate()
		{
			this.logger.LogInfo("\nValidating Package '{0}'", new object[]
			{
				this.PackageName
			});
			if (this.HostArch == CpuArch.Invalid)
			{
				throw new PkgException("CbsPackage::Validate: Host architecture has not been set.");
			}
			if (this.HostArch == this.GuestArch)
			{
				throw new PkgException("CbsPackage::Validate: Host architecture and Guest architecture are the same. Value: {0}", new object[]
				{
					this.HostArch
				});
			}
			if (string.IsNullOrWhiteSpace(this.PackageName))
			{
				throw new PkgException("CbsPackage::Validate: The name of a package can not be null or empty.");
			}
			if (string.IsNullOrWhiteSpace(this.Culture))
			{
				throw new PkgException("CbsPackage::Validate: The lanugage of a package can not be null or empty.");
			}
			if (string.IsNullOrWhiteSpace(this.PublicKey))
			{
				throw new PkgException("CbsPackage::Validate: The public key token of a package can not be null or empty.");
			}
			if (string.IsNullOrWhiteSpace(this.ReleaseType))
			{
				throw new PkgException("CbsPackage::Validate: The release type of a package can not be null or empty.");
			}
			if (this.Version.Equals(PkgConstants.InvalidVersion))
			{
				throw new PkgException("CbsPackage::Validate: The version of a package can not be {0}.", new object[]
				{
					this.Version.ToString()
				});
			}
			if (this.root == null)
			{
				throw new PkgException("CbsPackage::Validate: The package is empty. A package must contain at least one manifest.");
			}
			this.root.Validate();
		}

		// Token: 0x06000031 RID: 49 RVA: 0x00002604 File Offset: 0x00000804
		public void SavePackage(string outputFolder)
		{
			this.logger.LogInfo("\nSaving package {0} to {1}", new object[]
			{
				this.PackageName,
				outputFolder
			});
			this.Validate();
			if (!DirectoryToolBox.Exists(outputFolder))
			{
				DirectoryToolBox.Create(outputFolder);
			}
			if (!string.IsNullOrEmpty(this.Component))
			{
				this.Metrics.PackageType = PackageType.OneCore;
			}
			this.root.SaveManifest(outputFolder);
			if (this.PackageType != PackageType.Regular)
			{
				this.Purge(outputFolder);
			}
			this.Aggregate(outputFolder);
		}

		// Token: 0x06000032 RID: 50 RVA: 0x00002684 File Offset: 0x00000884
		public void SaveCab(string cabPath)
		{
			this.logger.LogInfo("\nSaving CAB {0} for package {1}", new object[]
			{
				cabPath,
				this.PackageName
			});
			string directoryFromFilePath = DirectoryToolBox.GetDirectoryFromFilePath(this.root.SourcePath);
			this.SavePackage(directoryFromFilePath);
			this.Archive(cabPath, CabToolBox.CompressionType.None);
		}

		// Token: 0x06000033 RID: 51 RVA: 0x000026D4 File Offset: 0x000008D4
		public void SaveCab(string cabPath, CabToolBox.CompressionType compressionType)
		{
			this.logger.LogInfo("\nSaving CAB {0} for package {1}", new object[]
			{
				cabPath,
				this.PackageName
			});
			string directoryFromFilePath = DirectoryToolBox.GetDirectoryFromFilePath(this.root.SourcePath);
			this.SavePackage(directoryFromFilePath);
			this.Archive(cabPath, compressionType);
		}

		// Token: 0x06000034 RID: 52 RVA: 0x00002724 File Offset: 0x00000924
		public IManifest GetRoot()
		{
			return this.root;
		}

		// Token: 0x06000035 RID: 53 RVA: 0x0000272C File Offset: 0x0000092C
		public IFile FindFile(string destinationPath)
		{
			return this.AllFiles.First((IFile x) => x.DevicePath.Equals(destinationPath, StringComparison.InvariantCultureIgnoreCase));
		}

		// Token: 0x06000036 RID: 54 RVA: 0x0000275D File Offset: 0x0000095D
		public void AddFile(IFile file)
		{
			this.AddFile(file, PkgConstants.EmbeddedSigningCategory_None);
		}

		// Token: 0x06000037 RID: 55 RVA: 0x0000276B File Offset: 0x0000096B
		public void AddFile(IFile file, string embeddedSigningCategory)
		{
			this.root.AddFile(file, embeddedSigningCategory);
		}

		// Token: 0x06000038 RID: 56 RVA: 0x0000277A File Offset: 0x0000097A
		public void AddFile(IManifest file)
		{
			this.AddFile(file, PkgConstants.EmbeddedSigningCategory_None);
		}

		// Token: 0x06000039 RID: 57 RVA: 0x00002788 File Offset: 0x00000988
		public void AddFile(IManifest file, string embeddedSigningCategory)
		{
			this.root.AddFile(file, embeddedSigningCategory);
		}

		// Token: 0x0600003A RID: 58 RVA: 0x00002797 File Offset: 0x00000997
		public void AddFile(FileType fileType, string sourcePath, string destinationPath, string sourcePackage)
		{
			this.AddFile(fileType, sourcePath, destinationPath, sourcePackage, PkgConstants.EmbeddedSigningCategory_None);
		}

		// Token: 0x0600003B RID: 59 RVA: 0x000027A9 File Offset: 0x000009A9
		public void AddFile(FileType fileType, string sourcePath, string destinationPath, string sourcePackage, string embeddedSigningCategory)
		{
			this.root.AddFile(fileType, sourcePath, destinationPath, sourcePackage, embeddedSigningCategory);
		}

		// Token: 0x0600003C RID: 60 RVA: 0x000027BD File Offset: 0x000009BD
		public void RemoveFile(IFile file)
		{
			this.root.RemoveFile(file);
		}

		// Token: 0x0600003D RID: 61 RVA: 0x000027CB File Offset: 0x000009CB
		public void RemoveFile(IManifest file)
		{
			this.root.RemoveFile(file);
		}

		// Token: 0x0600003E RID: 62 RVA: 0x000027D9 File Offset: 0x000009D9
		public void RemoveFile(FileType fileType, string destinationPath)
		{
			this.root.RemoveFile(fileType, destinationPath);
		}

		// Token: 0x0600003F RID: 63 RVA: 0x000027E8 File Offset: 0x000009E8
		public void RemoveAllFiles()
		{
			this.root.RemoveAllFiles();
		}

		// Token: 0x06000040 RID: 64 RVA: 0x000027F5 File Offset: 0x000009F5
		public IEnumerable<string> GetAllSourcePaths()
		{
			if (this.root == null)
			{
				return new List<string>();
			}
			return this.root.GetAllSourcePaths();
		}

		// Token: 0x06000041 RID: 65 RVA: 0x00002810 File Offset: 0x00000A10
		public IEnumerable<string> GetAllCabPaths()
		{
			if (this.root == null)
			{
				return new List<string>();
			}
			return this.root.GetAllCabPaths();
		}

		// Token: 0x06000042 RID: 66 RVA: 0x0000282B File Offset: 0x00000A2B
		public void SetCBSFeatureInfo(string fmId, string featureId, string pkgGroup, List<IPackageInfo> packages)
		{
			this.root.SetCBSFeatureInfo(fmId, featureId, pkgGroup, packages);
		}

		// Token: 0x06000043 RID: 67 RVA: 0x00002840 File Offset: 0x00000A40
		public void SetCBSFeatureInfo(string fmId, string featureId, string pkgGroup, List<string> pkgPaths)
		{
			List<IPackageInfo> list = new List<IPackageInfo>();
			foreach (string sourcePath in pkgPaths)
			{
				list.Add(new CbsPackageInfo(sourcePath));
			}
			this.SetCBSFeatureInfo(fmId, featureId, pkgGroup, list);
		}

		// Token: 0x06000044 RID: 68 RVA: 0x000028A4 File Offset: 0x00000AA4
		private static void EnableParsing(string root)
		{
			object obj = CbsPackage.wcpLock;
			lock (obj)
			{
				if (!CbsPackage.wcpEnabled)
				{
					CbsPackage.wcpEnabled = true;
					if (!NativeMethods.InitWcpManaged())
					{
						throw new UnauthorizedAccessException("Failed to initialize WCP. This is usually the result of not running in an elevated environment.");
					}
				}
			}
		}

		// Token: 0x06000045 RID: 69 RVA: 0x000028FC File Offset: 0x00000AFC
		private static bool CheckCBSFeatureInfo(string fmId, string featureId, string pkgGroup, Dictionary<IPackageInfo, string> packages, bool ignoreMissingSatellites, Logger logger)
		{
			List<IPackageInfo> list = (from x in packages.Keys
			where x.IsFIPPackage
			select x).ToList<IPackageInfo>();
			bool result = true;
			if (list.Count > 1)
			{
				logger.LogError("CbsPackage::CheckCBSFeatureInfo: Feature(featureId:{0}) inside FM(fmId:{1}) contains multiple FIP packages:", new object[]
				{
					featureId,
					fmId
				});
				foreach (IPackageInfo packageInfo in list)
				{
					logger.LogError(packageInfo.PackageName, new object[0]);
				}
				result = false;
			}
			else if (!list.Any<IPackageInfo>())
			{
				logger.LogError("CbsPackage::CheckCBSFeatureInfo: Feature(featureId:{0}) inside FM(fmId:{1}) doesn't contain a FIP package", new object[]
				{
					featureId,
					fmId
				});
				result = false;
			}
			else
			{
				CbsPackage cbsPackage = new CbsPackage(packages[list.First<IPackageInfo>()]);
				XNamespace @namespace = cbsPackage.GetCBSFeatureInfo().Name.Namespace;
				XElement xelement = cbsPackage.GetCBSFeatureInfo().Element(@namespace + "capability");
				CbsPackage.ValidateFIPAttributes(xelement.Attribute("group"), cbsPackage.OwnerType.ToString(), cbsPackage.PackageName, featureId, fmId, logger);
				XAttribute xattribute = xelement.Attribute("FMID");
				if (xattribute != null && fmId != null)
				{
					CbsPackage.ValidateFIPAttributes(xattribute, fmId, cbsPackage.PackageName, featureId, fmId, logger);
				}
				CbsPackage.ValidateFIPAttributes(xelement.Element(@namespace + "capabilityIdentity").Attribute("name"), featureId, cbsPackage.PackageName, featureId, fmId, logger);
				Dictionary<string, XElement> dictionary = new Dictionary<string, XElement>(StringComparer.InvariantCultureIgnoreCase);
				foreach (XElement xelement2 in xelement.Element(@namespace + "featurePackage").Elements(@namespace + "package"))
				{
					string value = xelement2.Element(@namespace + "assemblyIdentity").Attribute("name").Value;
					dictionary.Add(value, xelement2);
				}
				foreach (IPackageInfo packageInfo2 in packages.Keys)
				{
					if (dictionary.ContainsKey(packageInfo2.PackageName))
					{
						XElement xelement3 = dictionary[packageInfo2.PackageName];
						string satelliteType = ManifestToolBox.GetSatelliteType(packageInfo2.Culture, packageInfo2.OwnerType.ToString(), packageInfo2.PackageName, featureId, pkgGroup);
						CbsPackage.ValidateFIPAttributes(xelement3.Attribute("satelliteType"), satelliteType, packageInfo2.PackageName, featureId, fmId, logger);
						XAttribute xattribute2 = xelement3.Attribute("binaryPartition");
						if (xattribute2 == null)
						{
							xattribute2 = new XAttribute("binaryPartition", "false");
						}
						CbsPackage.ValidateFIPAttributes(xattribute2, (packageInfo2.BinaryPartition != null) ? packageInfo2.BinaryPartition.ToString() : "false", packageInfo2.PackageName, featureId, fmId, logger);
						XAttribute xattribute3 = xelement3.Attribute("targetPartition");
						if (xattribute3 == null)
						{
							xattribute3 = new XAttribute("targetPartition", PkgConstants.MainOsPartition);
						}
						CbsPackage.ValidateFIPAttributes(xattribute3, packageInfo2.Partition, packageInfo2.PackageName, featureId, fmId, logger);
						XAttribute xattribute4 = xelement3.Element(@namespace + "assemblyIdentity").Attribute("language");
						if (xattribute4 == null)
						{
							xattribute4 = new XAttribute("language", PkgConstants.NeutralCulture);
						}
						CbsPackage.ValidateFIPAttributes(xattribute4, packageInfo2.Culture, packageInfo2.PackageName, featureId, fmId, logger);
						CbsPackage.ValidateFIPAttributes(xelement3.Element(@namespace + "assemblyIdentity").Attribute("processorArchitecture"), ManifestToolBox.GetCpuString(packageInfo2.HostArch, packageInfo2.GuestArch), packageInfo2.PackageName, featureId, fmId, logger);
						CbsPackage.ValidateFIPAttributes(xelement3.Element(@namespace + "assemblyIdentity").Attribute("publicKeyToken"), packageInfo2.PublicKey, packageInfo2.PackageName, featureId, fmId, logger);
						CbsPackage.ValidateFIPAttributes(xelement3.Element(@namespace + "assemblyIdentity").Attribute("version"), packageInfo2.Version.ToString(), packageInfo2.PackageName, featureId, fmId, logger);
					}
					else
					{
						logger.LogError("CbsPackage::CheckCBSFeatureInfo: Invalid FIP info for Feature(featureId:{0}) inside FM(fmId:{1}). Package {2} is missing from FIP info inside package:{3}", new object[]
						{
							featureId,
							fmId,
							packageInfo2.PackageName,
							cbsPackage.PackageName
						});
						result = false;
					}
				}
				HashSet<string> passedPackageNames = new HashSet<string>(from x in packages.Keys
				select x.PackageName);
				HashSet<string> hashSet = new HashSet<string>(from x in dictionary
				where !passedPackageNames.Contains(x.Key) && (!ignoreMissingSatellites || (x.Key.IndexOf(PkgConstants.DefaultLanguagePattern, StringComparison.OrdinalIgnoreCase) == -1 && x.Key.IndexOf(PkgConstants.DefaultResolutionPattern, StringComparison.OrdinalIgnoreCase) == -1))
				select x into y
				select y.Key);
				if (hashSet.Any<string>())
				{
					logger.LogError("CbsPackage::CheckCBSFeatureInfo: Invalid FIP info. Following packages are not in Feature(featureId:{0}) inside FM(fmId:{1}). But are present in FIP info inside {2}", new object[]
					{
						featureId,
						fmId,
						cbsPackage.PackageName
					});
					foreach (string format in hashSet)
					{
						logger.LogError(format, new object[0]);
					}
					result = false;
				}
			}
			return result;
		}

		// Token: 0x06000046 RID: 70 RVA: 0x00002F30 File Offset: 0x00001130
		private static bool ValidateFIPAttributes(XAttribute attribute, string expectedValue, string packageName, string featureId, string fmId, Logger logger)
		{
			if (!attribute.Value.Equals(expectedValue, StringComparison.InvariantCultureIgnoreCase))
			{
				logger.LogError("CbsPackage::CheckCBSFeatureInfo: Invalid FIP info inside package {0} with featureId:{1} and fmId:{2}. Attribute:{3}, Expected Value:{4}, Actual Value:{5}", new object[]
				{
					packageName,
					featureId,
					fmId,
					attribute.Name,
					featureId,
					attribute.Value
				});
				return false;
			}
			return true;
		}

		// Token: 0x06000047 RID: 71 RVA: 0x00002F84 File Offset: 0x00001184
		private XElement GetCBSFeatureInfo()
		{
			return this.root.GetCBSFeatureInfo();
		}

		// Token: 0x06000048 RID: 72 RVA: 0x00002F94 File Offset: 0x00001194
		private void Purge(string targetDir)
		{
			this.logger.LogInfo("Purging package {0} in directory {1}", new object[]
			{
				this.PackageName,
				targetDir
			});
			HashSet<string> hashSet = new HashSet<string>(from entry in this.AllManifestFiles.ToList<IManifest>()
			select entry.SourcePath, StringComparer.InvariantCultureIgnoreCase);
			foreach (string text in DirectoryToolBox.Files(targetDir, false, false))
			{
				if (!hashSet.Contains(text))
				{
					ManifestToolBox.SafeRetryCleanup(text);
				}
			}
			this.logger.LogInfo("Done purging package {0}", new object[]
			{
				this.PackageName
			});
		}

		// Token: 0x06000049 RID: 73 RVA: 0x00003070 File Offset: 0x00001270
		private void Aggregate(string targetDir)
		{
			this.logger.LogInfo("Aggregating package {0} to directory {1}", new object[]
			{
				this.PackageName,
				targetDir
			});
			List<string> list = new List<string>(this.GetAllSourcePaths());
			List<string> list2 = new List<string>(this.GetAllCabPaths());
			if (list.Count != list2.Count)
			{
				throw new FormatException("Failure during 'Aggregation'. The number of files and the number of cab paths are not equal.");
			}
			for (int i = 0; i < list.Count; i++)
			{
				string text = PathToolBox.Combine(targetDir, list2[i]);
				if (!text.Replace('/', '\\').Equals(list[i].Replace('/', '\\'), StringComparison.InvariantCultureIgnoreCase))
				{
					ManifestToolBox.RetryCopy(list[i], text);
				}
			}
			this.logger.LogInfo("Done aggregating package {0}", new object[]
			{
				this.PackageName
			});
		}

		// Token: 0x0600004A RID: 74 RVA: 0x00003140 File Offset: 0x00001340
		private void Archive(string cabPath, CabToolBox.CompressionType compressionType)
		{
			this.logger.LogInfo("Creating and signing cab file {0}", new object[]
			{
				cabPath
			});
			HashSet<string> source = new HashSet<string>(this.GetAllSourcePaths());
			HashSet<string> source2 = new HashSet<string>(this.GetAllCabPaths());
			CabToolBox.CreateCab(cabPath, source.ToList<string>(), source2.ToList<string>());
			SecurityToolBox.SignFile(cabPath);
			this.logger.LogInfo("Done Signing cab file {0}", new object[]
			{
				cabPath
			});
		}

		// Token: 0x0600004B RID: 75 RVA: 0x000031B4 File Offset: 0x000013B4
		private void Initialize(string sourcePath, Logger logger, bool enableWcp)
		{
			IntPtr zero = IntPtr.Zero;
			if (!FileToolBox.Extension(sourcePath).Equals(PkgConstants.MumExtension, StringComparison.InvariantCultureIgnoreCase))
			{
				string temporaryPath = PathToolBox.GetTemporaryPath();
				DirectoryToolBox.Create(temporaryPath);
				CabToolBox.ExtractCab(sourcePath, temporaryPath);
				sourcePath = PathToolBox.Combine(temporaryPath, PkgConstants.UpdateMum);
			}
			if (enableWcp)
			{
				CbsPackage.EnableParsing(sourcePath);
				if (!NativeMethods.ParsePackageManaged(sourcePath, out zero))
				{
					throw new PkgException("CbsPackage::Initialize: Couldn't parse package information from {0}. This usually happens when a required privilege is not held by the client.", new object[]
					{
						sourcePath
					});
				}
			}
			this.logger = logger;
			this.Metrics = new PackageMetrics(this.logger, zero, LoadType.Package);
			this.Metrics.PackageType = this.GetPackageType(sourcePath);
			this.Metrics.SignInfoFiles.Clear();
			this.Metrics.SignInfoFiles.Union(this.GetSignInfoFiles(sourcePath));
			this.Load(sourcePath);
		}

		// Token: 0x0600004C RID: 76 RVA: 0x00003280 File Offset: 0x00001480
		private HashSet<string> GetSignInfoFiles(string sourcePath)
		{
			XElement xelement = ManifestToolBox.Load(sourcePath).Root;
			XElement xelement2 = xelement.Elements(xelement.Name.Namespace + "package").First<XElement>().Element(xelement.Name.Namespace + "customInformation");
			HashSet<string> hashSet = new HashSet<string>();
			if (xelement2 != null)
			{
				foreach (XElement xelement3 in xelement2.Elements(xelement.Name.Namespace + "file"))
				{
					if (xelement3.Attribute("signInfo") != null && xelement3.Attribute("signInfo").Value.ToString().Equals("true", StringComparison.InvariantCultureIgnoreCase))
					{
						hashSet.Add(PathToolBox.GetFileNameFromPath(xelement3.Attribute("name").Value));
					}
				}
			}
			return hashSet;
		}

		// Token: 0x0600004D RID: 77 RVA: 0x00003390 File Offset: 0x00001590
		private PackageType GetPackageType(string sourcePath)
		{
			PackageType result = PackageType.Regular;
			XElement xelement = ManifestToolBox.Load(sourcePath).Root;
			XElement xelement2 = xelement.Elements(xelement.Name.Namespace + "package").First<XElement>();
			XElement xelement3 = xelement.Element(xelement.Name.Namespace + "assemblyIdentity");
			XElement xelement4 = xelement2.Element(xelement.Name.Namespace + "customInformation");
			XElement xelement5 = xelement2.Element(xelement.Name.Namespace + "declareCapability");
			if (xelement4 != null)
			{
				if (xelement4.Element(xelement.Name.Namespace + "phoneInformation") != null)
				{
					result = PackageType.OneCore;
				}
			}
			else if (xelement5 != null && xelement5.Descendants(xelement.Name.Namespace + "featurePackage").Count<XElement>() == 0)
			{
				if (xelement3.Attribute("language").Value.ToString().Equals(PkgConstants.NeutralCulture, StringComparison.InvariantCultureIgnoreCase))
				{
					result = PackageType.FOD_Neutral;
				}
				else
				{
					result = PackageType.FOD_Lang;
				}
			}
			return result;
		}

		// Token: 0x0600004E RID: 78 RVA: 0x00003498 File Offset: 0x00001698
		private void Load(string sourcePath)
		{
			try
			{
				this.logger.LogInfo("\nLoading Package:" + sourcePath, new object[0]);
				this.Metrics.StartOperation();
				this.root = PkgManifest.LoadFromDisk(sourcePath, this.Metrics);
				foreach (IManifest manifest in this.AllManifestFiles)
				{
					manifest.Clean();
				}
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

		// Token: 0x04000001 RID: 1
		private static bool wcpEnabled = false;

		// Token: 0x04000002 RID: 2
		private static object wcpLock = CbsPackage.wcpEnabled;

		// Token: 0x04000003 RID: 3
		private Logger logger;

		// Token: 0x04000004 RID: 4
		private IManifest root;
	}
}
