using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.Composition.Packaging.Interfaces;
using Microsoft.Composition.ToolBox;
using Microsoft.Composition.ToolBox.IO;
using Microsoft.Composition.ToolBox.Reflection;
using Microsoft.Composition.ToolBox.Security;

namespace Microsoft.Composition.Packaging
{
	// Token: 0x02000008 RID: 8
	public class PkgManifest : StatefulObject, IManifest, IFile
	{
		// Token: 0x060000B7 RID: 183 RVA: 0x00004770 File Offset: 0x00002970
		public PkgManifest(string sourcePath, string sourcePackage, PackageMetrics pkgMetrics)
		{
			this.pkgFile = new PkgFile(FileType.Manifest, sourcePath, PkgConstants.RuntimeBootdrive + PkgConstants.FileRedirectPath, PathToolBox.GetFileNameFromPath(sourcePath), sourcePackage);
			this.PhoneInformation = new PhoneInformation();
			this.manifestRoot = ManifestToolBox.Load(this.SourcePath).Root;
			if (this.metrics == null)
			{
				this.metrics = pkgMetrics;
			}
			this.Load();
		}

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x060000B8 RID: 184 RVA: 0x00004815 File Offset: 0x00002A15
		public FileType FileType
		{
			get
			{
				if (this.pkgFile != null)
				{
					return this.pkgFile.FileType;
				}
				return FileType.Invalid;
			}
		}

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x060000B9 RID: 185 RVA: 0x0000482C File Offset: 0x00002A2C
		public string DevicePath
		{
			get
			{
				if (this.pkgFile != null)
				{
					return this.pkgFile.DevicePath;
				}
				return string.Empty;
			}
		}

		// Token: 0x17000045 RID: 69
		// (get) Token: 0x060000BA RID: 186 RVA: 0x00004847 File Offset: 0x00002A47
		// (set) Token: 0x060000BB RID: 187 RVA: 0x00004862 File Offset: 0x00002A62
		public string CabPath
		{
			get
			{
				if (this.pkgFile != null)
				{
					return this.pkgFile.CabPath;
				}
				return string.Empty;
			}
			set
			{
				if (this.pkgFile != null)
				{
					this.pkgFile.CabPath = value;
				}
			}
		}

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x060000BC RID: 188 RVA: 0x00004878 File Offset: 0x00002A78
		// (set) Token: 0x060000BD RID: 189 RVA: 0x00004893 File Offset: 0x00002A93
		public string SourcePath
		{
			get
			{
				if (this.pkgFile != null)
				{
					return this.pkgFile.SourcePath;
				}
				return string.Empty;
			}
			set
			{
				this.pkgFile.SourcePath = value;
			}
		}

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x060000BE RID: 190 RVA: 0x000048A1 File Offset: 0x00002AA1
		public long Size
		{
			get
			{
				if (this.pkgFile != null)
				{
					return this.pkgFile.Size;
				}
				return 0L;
			}
		}

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x060000BF RID: 191 RVA: 0x000048B9 File Offset: 0x00002AB9
		public long CompressedSize
		{
			get
			{
				if (this.pkgFile != null)
				{
					return this.pkgFile.CompressedSize;
				}
				return 0L;
			}
		}

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x060000C0 RID: 192 RVA: 0x000048D1 File Offset: 0x00002AD1
		public long StagedSize
		{
			get
			{
				if (this.pkgFile != null)
				{
					return this.pkgFile.StagedSize;
				}
				return 0L;
			}
		}

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x060000C1 RID: 193 RVA: 0x000048E9 File Offset: 0x00002AE9
		public string SourcePackage
		{
			get
			{
				if (this.pkgFile != null)
				{
					return this.pkgFile.SourcePackage;
				}
				return string.Empty;
			}
		}

		// Token: 0x1700004B RID: 75
		// (get) Token: 0x060000C2 RID: 194 RVA: 0x00004904 File Offset: 0x00002B04
		public string FileHash
		{
			get
			{
				if (this.pkgFile != null)
				{
					return this.pkgFile.FileHash;
				}
				return string.Empty;
			}
		}

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x060000C3 RID: 195 RVA: 0x0000491F File Offset: 0x00002B1F
		public bool SignInfoRequired
		{
			get
			{
				return this.pkgFile != null && this.pkgFile.SignInfoRequired;
			}
		}

		// Token: 0x1700004D RID: 77
		// (get) Token: 0x060000C4 RID: 196 RVA: 0x00004936 File Offset: 0x00002B36
		public string EmbeddedSigningCategory
		{
			get
			{
				if (this.pkgFile != null)
				{
					return this.pkgFile.EmbeddedSigningCategory;
				}
				return PkgConstants.EmbeddedSigningCategory_None;
			}
		}

		// Token: 0x1700004E RID: 78
		// (get) Token: 0x060000C5 RID: 197 RVA: 0x00004951 File Offset: 0x00002B51
		// (set) Token: 0x060000C6 RID: 198 RVA: 0x00004959 File Offset: 0x00002B59
		public ManifestType ManifestType { get; private set; }

		// Token: 0x1700004F RID: 79
		// (get) Token: 0x060000C7 RID: 199 RVA: 0x00004964 File Offset: 0x00002B64
		// (set) Token: 0x060000C8 RID: 200 RVA: 0x00004B89 File Offset: 0x00002D89
		public Keyform Keyform
		{
			get
			{
				if (string.IsNullOrEmpty(this.keyform.ReleaseType))
				{
					this.keyform.ReleaseType = PkgConstants.DefaultReleaseType;
				}
				if (string.IsNullOrEmpty(this.keyform.Language))
				{
					this.keyform.Language = PkgConstants.NeutralCulture;
				}
				if (string.IsNullOrEmpty(this.keyform.Name) && this.PhoneInformation != null && !string.IsNullOrEmpty(this.PhoneInformation.PhoneComponent))
				{
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.AppendFormat("{0}.{1}", this.PhoneInformation.PhoneOwner, this.PhoneInformation.PhoneComponent);
					if (!string.IsNullOrEmpty(this.PhoneInformation.PhoneSubComponent))
					{
						stringBuilder.AppendFormat(".{0}", this.PhoneInformation.PhoneSubComponent);
					}
					this.keyform.Name = stringBuilder.ToString();
				}
				if (this.metrics != null && this.ManifestType == ManifestType.Package)
				{
					if (this.metrics.PackageType == PackageType.OneCore && !string.IsNullOrEmpty(this.keyform.Language) && !this.keyform.Language.Equals(PkgConstants.NeutralCulture, StringComparison.InvariantCultureIgnoreCase) && !this.keyform.Name.ToLower(CultureInfo.InvariantCulture).Contains("_lang_" + this.keyform.Language.ToLower(CultureInfo.InvariantCulture)))
					{
						this.keyform.Name = this.keyform.Name + "_Lang_" + this.keyform.Language;
					}
					if (this.metrics.PackageType == PackageType.OneCore && this.keyform.GuestArch != CpuArch.Invalid && !this.keyform.Name.ToLower(CultureInfo.InvariantCulture).Contains("_wow_") && !this.keyform.Name.ToLower(CultureInfo.InvariantCulture).Contains(".guest"))
					{
						this.keyform.Name = this.keyform.Name + "_Wow_" + ManifestToolBox.GetCpuString(this.keyform);
					}
				}
				return this.keyform;
			}
			set
			{
				this.keyform = value;
			}
		}

		// Token: 0x17000050 RID: 80
		// (get) Token: 0x060000C9 RID: 201 RVA: 0x00004B92 File Offset: 0x00002D92
		// (set) Token: 0x060000CA RID: 202 RVA: 0x00004B9A File Offset: 0x00002D9A
		public PhoneInformation PhoneInformation { get; set; }

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x060000CB RID: 203 RVA: 0x00004BA3 File Offset: 0x00002DA3
		// (set) Token: 0x060000CC RID: 204 RVA: 0x00004BAB File Offset: 0x00002DAB
		public string UpdateName { get; set; }

		// Token: 0x17000052 RID: 82
		// (get) Token: 0x060000CD RID: 205 RVA: 0x00004BB4 File Offset: 0x00002DB4
		// (set) Token: 0x060000CE RID: 206 RVA: 0x00004BBC File Offset: 0x00002DBC
		public bool NoMerge { get; set; }

		// Token: 0x17000053 RID: 83
		// (get) Token: 0x060000CF RID: 207 RVA: 0x00004BC5 File Offset: 0x00002DC5
		// (set) Token: 0x060000D0 RID: 208 RVA: 0x00004BCD File Offset: 0x00002DCD
		public bool SelfUpdate { get; set; }

		// Token: 0x17000054 RID: 84
		// (get) Token: 0x060000D1 RID: 209 RVA: 0x00004BD6 File Offset: 0x00002DD6
		// (set) Token: 0x060000D2 RID: 210 RVA: 0x00004BDE File Offset: 0x00002DDE
		public bool? BinaryPartition { get; set; }

		// Token: 0x17000055 RID: 85
		// (get) Token: 0x060000D3 RID: 211 RVA: 0x00004BE7 File Offset: 0x00002DE7
		// (set) Token: 0x060000D4 RID: 212 RVA: 0x00004BF0 File Offset: 0x00002DF0
		public string Partition
		{
			get
			{
				return this.targetPartition;
			}
			set
			{
				this.targetPartition = value;
				if (this.BinaryPartition == null)
				{
					this.BinaryPartition = new bool?(false);
				}
			}
		}

		// Token: 0x17000056 RID: 86
		// (get) Token: 0x060000D5 RID: 213 RVA: 0x00004C20 File Offset: 0x00002E20
		// (set) Token: 0x060000D6 RID: 214 RVA: 0x00004C28 File Offset: 0x00002E28
		public string InfName { get; set; }

		// Token: 0x17000057 RID: 87
		// (get) Token: 0x060000D7 RID: 215 RVA: 0x00004C31 File Offset: 0x00002E31
		// (set) Token: 0x060000D8 RID: 216 RVA: 0x00004C39 File Offset: 0x00002E39
		public string InfRanking { get; set; }

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x060000D9 RID: 217 RVA: 0x00004C44 File Offset: 0x00002E44
		public IEnumerable<IFile> AllFiles
		{
			get
			{
				List<IFile> list = new List<IFile>();
				list.AddRange(this.payload);
				list.Add(this);
				if (!this.IsLeaf())
				{
					foreach (IManifest manifest in this.manifest)
					{
						list.AddRange(manifest.AllFiles);
					}
				}
				return list;
			}
		}

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x060000DA RID: 218 RVA: 0x00004CC0 File Offset: 0x00002EC0
		public IEnumerable<IFile> AllPayloadFiles
		{
			get
			{
				List<IFile> list = new List<IFile>();
				list.AddRange(this.payload);
				if (!this.IsLeaf())
				{
					foreach (IManifest manifest in this.manifest)
					{
						list.AddRange(manifest.AllPayloadFiles);
					}
				}
				return list;
			}
		}

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x060000DB RID: 219 RVA: 0x00004D34 File Offset: 0x00002F34
		public IEnumerable<IManifest> AllManifestFiles
		{
			get
			{
				List<IManifest> list = new List<IManifest>();
				list.Add(this);
				if (!this.IsLeaf())
				{
					foreach (IManifest manifest in this.manifest)
					{
						list.AddRange(manifest.AllManifestFiles);
					}
				}
				return list;
			}
		}

		// Token: 0x1700005B RID: 91
		// (get) Token: 0x060000DC RID: 220 RVA: 0x00004DA4 File Offset: 0x00002FA4
		public IEnumerable<IFile> CurrentPayloadFiles
		{
			get
			{
				return this.payload;
			}
		}

		// Token: 0x1700005C RID: 92
		// (get) Token: 0x060000DD RID: 221 RVA: 0x00004DAC File Offset: 0x00002FAC
		public IEnumerable<IManifest> CurrentManifestFiles
		{
			get
			{
				return this.manifest;
			}
		}

		// Token: 0x060000DE RID: 222 RVA: 0x00004DB4 File Offset: 0x00002FB4
		public static PkgManifest LoadFromDisk(string sourcePath, PackageMetrics metrics)
		{
			return PkgManifest.LoadFromDisk(sourcePath, metrics, string.Empty);
		}

		// Token: 0x060000DF RID: 223 RVA: 0x00004DC4 File Offset: 0x00002FC4
		public static PkgManifest LoadFromDisk(string sourcePath, PackageMetrics metrics, string sourcePackage)
		{
			PkgManifest processedManifest = metrics.GetProcessedManifest(sourcePath);
			if (processedManifest != null)
			{
				metrics.Logger.LogInfo("Returning processed manifest for {0}", new object[]
				{
					sourcePath
				});
				return processedManifest;
			}
			metrics.Logger.LogInfo("Loading manifest file {0}", new object[]
			{
				sourcePath
			});
			return new PkgManifest(sourcePath, sourcePackage, metrics);
		}

		// Token: 0x060000E0 RID: 224 RVA: 0x00004E1C File Offset: 0x0000301C
		public static string SerializeEmptyMum(Keyform keyform, string outputDirectory)
		{
			string text = Path.Combine(outputDirectory, Keyform.GenerateCBSKeyform(keyform) + PkgConstants.MumExtension);
			XDocument xdocument = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), new object[0]);
			XNamespace ns = PkgConstants.CMIV3NS;
			xdocument.Add(new XElement(ns + "assembly", new XAttribute("manifestVersion", "1.0")));
			XElement content = PkgManifest.SerializeAssemblyIdentity(keyform);
			xdocument.Root.Add(content);
			XElement content2 = new XElement(ns + "package", new object[]
			{
				new XAttribute("identifier", keyform.Name),
				new XAttribute("releaseType", keyform.ReleaseType)
			});
			xdocument.Root.Add(content2);
			ManifestToolBox.Save(xdocument, text);
			return text;
		}

		// Token: 0x060000E1 RID: 225 RVA: 0x00004F08 File Offset: 0x00003108
		public static XElement GenerateFeaturePackageInfo(IPackageInfo pkgInfo, XNamespace rootNS, string featureId, string pkgGroup)
		{
			string satelliteType = ManifestToolBox.GetSatelliteType(pkgInfo.Culture, pkgInfo.OwnerType.ToString(), pkgInfo.PackageName, featureId, pkgGroup);
			XElement xelement = new XElement(rootNS + "package", new XAttribute("satelliteType", satelliteType));
			if (!string.IsNullOrEmpty(pkgInfo.Partition))
			{
				xelement.Add(new XAttribute("targetPartition", pkgInfo.Partition));
			}
			if (pkgInfo.BinaryPartition != null && pkgInfo.BinaryPartition == true)
			{
				xelement.Add(new XAttribute("binaryPartition", pkgInfo.BinaryPartition));
			}
			string cpuString = ManifestToolBox.GetCpuString(pkgInfo.HostArch, pkgInfo.GuestArch);
			if (pkgInfo.Culture.Equals(PkgConstants.NeutralCulture, StringComparison.InvariantCultureIgnoreCase))
			{
				xelement.Add(new XElement(rootNS + "assemblyIdentity", new object[]
				{
					new XAttribute("name", pkgInfo.PackageName),
					new XAttribute("processorArchitecture", cpuString),
					new XAttribute("publicKeyToken", pkgInfo.PublicKey),
					new XAttribute("version", pkgInfo.Version.ToString())
				}));
			}
			else
			{
				xelement.Add(new XElement(rootNS + "assemblyIdentity", new object[]
				{
					new XAttribute("language", pkgInfo.Culture),
					new XAttribute("name", pkgInfo.PackageName),
					new XAttribute("processorArchitecture", cpuString),
					new XAttribute("publicKeyToken", pkgInfo.PublicKey),
					new XAttribute("version", pkgInfo.Version.ToString())
				}));
			}
			return xelement;
		}

		// Token: 0x060000E2 RID: 226 RVA: 0x00005118 File Offset: 0x00003318
		public void Validate()
		{
			if (this.Keyform.HostArch == CpuArch.Invalid)
			{
				throw new PkgException("PkgManifest::Validate: Host architecture has not been set. Manifest: {0}", new object[]
				{
					this.Keyform.Name
				});
			}
			if (this.Keyform.HostArch == this.Keyform.GuestArch)
			{
				throw new PkgException("PkgManifest::Validate: Host architecture and Guest architecture are the same. Value: {0}", new object[]
				{
					this.Keyform.HostArch
				});
			}
			if (string.IsNullOrWhiteSpace(this.Keyform.Name))
			{
				throw new PkgException("PkgManifest::Validate: The name of a package can not be null or empty. Manifest: {0}", new object[]
				{
					this.Keyform.Name
				});
			}
			if (string.IsNullOrWhiteSpace(this.Keyform.Language))
			{
				throw new PkgException("PkgManifest::Validate: The lanugage of a package can not be null or empty. Manifest: {0}", new object[]
				{
					this.Keyform.Name
				});
			}
			if (string.IsNullOrWhiteSpace(this.Keyform.PublicKeyToken))
			{
				throw new PkgException("PkgManifest::Validate: The public key token of a package can not be null or empty. Manifest: {0}", new object[]
				{
					this.Keyform.Name
				});
			}
			if (this.Keyform.Version.Equals(PkgConstants.InvalidVersion))
			{
				throw new PkgException("PkgManifest::Validate: The version of a package can not be {0}.", new object[]
				{
					this.Keyform.Version.ToString()
				});
			}
			if (this.ManifestType == ManifestType.Package && string.IsNullOrWhiteSpace(this.Keyform.ReleaseType))
			{
				throw new PkgException("PkgManifest::Validate: The release type of a package can not be null or empty. Manifest: {0}", new object[]
				{
					this.Keyform.Name
				});
			}
			if (this.ManifestType == ManifestType.Driver && string.IsNullOrWhiteSpace(this.InfName))
			{
				throw new PkgException("PkgManifest::Validate: The name of the target inf of a driver can not be null or empty. Manifest: {0}", new object[]
				{
					this.Keyform.Name
				});
			}
			if (this.ManifestType == ManifestType.Driver && string.IsNullOrWhiteSpace(this.InfRanking))
			{
				throw new PkgException("PkgManifest::Validate: The name of the priority rank of a driver can not be null or empty. Manifest: {0}", new object[]
				{
					this.Keyform.Name
				});
			}
			foreach (PkgManifest pkgManifest in this.manifest)
			{
				((IManifest)pkgManifest).Validate();
			}
		}

		// Token: 0x060000E3 RID: 227 RVA: 0x00005344 File Offset: 0x00003544
		public void SaveManifest(string outputFolder)
		{
			if (this.AllManifestFiles.Count<IManifest>() == 1)
			{
				string name = this.Keyform.Name + "-" + ManifestType.Deployment.ToString();
				this.AddFile(PkgManifest.LoadFromDisk(PkgManifest.SerializeEmptyDeployment(new Keyform(this.Keyform)
				{
					Name = name
				}, outputFolder), this.metrics));
			}
			this.SaveManifest(this.Keyform.Name, outputFolder);
		}

		// Token: 0x060000E4 RID: 228 RVA: 0x000053C4 File Offset: 0x000035C4
		public void SaveManifest(string mergedPackageName, string outputFolder)
		{
			foreach (PkgManifest pkgManifest in this.manifest)
			{
				pkgManifest.SaveManifest(mergedPackageName, outputFolder);
			}
			if (base.IsDirty())
			{
				this.metrics.Logger.LogInfo("Writing {0} to disk", new object[]
				{
					this.SourcePath
				});
				switch (this.ManifestType)
				{
				case ManifestType.Package:
					this.SerializeMum(outputFolder);
					break;
				case ManifestType.Deployment:
					this.SerializeDeployment(mergedPackageName, outputFolder);
					break;
				case ManifestType.Component:
					this.SerializeComponent(outputFolder);
					break;
				}
				this.Clean();
			}
		}

		// Token: 0x060000E5 RID: 229 RVA: 0x00005480 File Offset: 0x00003680
		public void AddParent(XElement assemblyId)
		{
			Keyform keyform = new Keyform();
			keyform.Name = assemblyId.Attribute("name").Value;
			keyform.Language = ((assemblyId.Attribute("language") == null) ? string.Empty : assemblyId.Attribute("language").Value);
			keyform.HostArch = ManifestToolBox.GetHostType(assemblyId.Attribute("processorArchitecture").Value);
			keyform.GuestArch = ManifestToolBox.GetGuestType(assemblyId.Attribute("processorArchitecture").Value);
			keyform.PublicKeyToken = assemblyId.Attribute("publicKeyToken").Value;
			keyform.Version = new Version(assemblyId.Attribute("version").Value);
			keyform.BuildType = BuildType.Invalid;
			if (assemblyId.Attribute("buildType") != null)
			{
				keyform.BuildType = (BuildType)Enum.Parse(typeof(BuildType), assemblyId.Attribute("buildType").Value, true);
			}
			this.parents.Add(keyform);
			base.TouchObject();
		}

		// Token: 0x060000E6 RID: 230 RVA: 0x000055B8 File Offset: 0x000037B8
		public void AddFile(IFile file)
		{
			this.AddFile(file, PkgConstants.EmbeddedSigningCategory_None);
		}

		// Token: 0x060000E7 RID: 231 RVA: 0x000055C8 File Offset: 0x000037C8
		public void AddFile(IFile file, string embeddedSigningCategory)
		{
			PkgManifest validManifest = this.GetValidManifest(file.FileType, ManifestType.Invalid);
			if (validManifest.CurrentPayloadFiles.FirstOrDefault((IFile x) => string.Compare(x.SourcePath, file.SourcePath, StringComparison.OrdinalIgnoreCase) == 0) == null)
			{
				validManifest.payload.Add(file as PkgFile);
				this.metrics.ProcessPayload(file.SourcePath, file as PkgFile);
				base.TouchObject();
			}
		}

		// Token: 0x060000E8 RID: 232 RVA: 0x0000564B File Offset: 0x0000384B
		public void AddFile(IManifest file)
		{
			this.AddFile(file, PkgConstants.EmbeddedSigningCategory_None);
		}

		// Token: 0x060000E9 RID: 233 RVA: 0x0000565C File Offset: 0x0000385C
		public void AddFile(IManifest file, string embeddedSigningCategory)
		{
			PkgManifest validManifest = this.GetValidManifest(file.FileType, file.ManifestType);
			if (validManifest.CurrentManifestFiles.FirstOrDefault((IManifest x) => string.Compare(x.SourcePath, file.SourcePath, StringComparison.OrdinalIgnoreCase) == 0) == null)
			{
				validManifest.manifest.Add(file as PkgManifest);
				this.metrics.ProcessManifest(file.SourcePath, file as PkgManifest);
				base.TouchObject();
			}
		}

		// Token: 0x060000EA RID: 234 RVA: 0x000056E9 File Offset: 0x000038E9
		public void AddFile(FileType fileType, string sourcePath, string destinationPath, string sourcePackage)
		{
			this.AddFile(fileType, sourcePath, destinationPath, sourcePackage, PkgConstants.EmbeddedSigningCategory_None);
		}

		// Token: 0x060000EB RID: 235 RVA: 0x000056FC File Offset: 0x000038FC
		public void AddFile(FileType fileType, string sourcePath, string destinationPath, string sourcePackage, string embeddedSigningCategory)
		{
			string text = PathToolBox.GetFileNameFromPath(sourcePath);
			if (fileType == FileType.Regular || fileType == FileType.Catalog)
			{
				if (!new Regex("\\$\\(\\w+.\\w+\\)", RegexOptions.IgnoreCase).Match(destinationPath).Success)
				{
					IntPtr intPtr;
					if (NativeMethods.ReplaceFilePathWithMacroPath(destinationPath, out intPtr) != 0)
					{
						throw new PkgException("PkgManifest::AddFile: Failed to convert file path {0} to macro path", new object[]
						{
							destinationPath
						});
					}
					destinationPath = Marshal.PtrToStringUni(intPtr);
					NativeMethods.ManagedMemoryFree(intPtr);
					destinationPath = DirectoryToolBox.GetDirectoryFromFilePath(destinationPath) + "\\";
				}
				PkgFile processedPayload = this.metrics.GetProcessedPayload(sourcePath);
				if (processedPayload == null || !destinationPath.Equals(processedPayload.DevicePath, StringComparison.InvariantCultureIgnoreCase))
				{
					PkgManifest validManifest = this.GetValidManifest(fileType, ManifestType.Invalid);
					if (fileType == FileType.Regular)
					{
						text = PathToolBox.Combine(Keyform.GenerateKeyform(validManifest.Keyform), text);
					}
					PkgFile pkgFile = new PkgFile(fileType, sourcePath, destinationPath, text, sourcePackage, embeddedSigningCategory);
					if (processedPayload != null && pkgFile.CabPath.Equals(processedPayload.CabPath, StringComparison.InvariantCultureIgnoreCase))
					{
						if (!pkgFile.FileHash.Equals(processedPayload.FileHash, StringComparison.InvariantCultureIgnoreCase))
						{
							throw new PkgException("PkgManifest::AddFile: Cannot add file {0}. File({1}) with same cab path but with different device path already exists", new object[]
							{
								pkgFile.SourcePath,
								processedPayload.SourcePath
							});
						}
						pkgFile.SourcePath = processedPayload.SourcePath;
					}
					validManifest.payload.Add(pkgFile);
					this.metrics.ProcessPayload(pkgFile.SourcePath, pkgFile);
				}
			}
			else if (fileType == FileType.Manifest)
			{
				if (this.metrics.GetProcessedManifest(sourcePath) == null)
				{
					PkgManifest pkgManifest = new PkgManifest(sourcePath, sourcePackage, this.metrics);
					PkgManifest validManifest = this.GetValidManifest(fileType, pkgManifest.ManifestType);
					validManifest.manifest.Add(pkgManifest);
					this.metrics.ProcessManifest(sourcePath, pkgManifest);
				}
			}
			else
			{
				if (fileType != FileType.Registry)
				{
					throw new PkgException("PkgManifest::Addfile: Cannot add file:{0} with invalid file type:{1}", new object[]
					{
						sourcePath,
						fileType
					});
				}
				PkgManifest validManifest = this.GetValidManifest(fileType, ManifestType.Invalid);
				XDocument xdocument = ManifestToolBox.Load(sourcePath);
				XNamespace @namespace = xdocument.Root.Name.Namespace;
				validManifest.registry.AddRange(xdocument.Root.Elements(@namespace + "registryKey").ToList<XElement>());
			}
			base.TouchObject();
		}

		// Token: 0x060000EC RID: 236 RVA: 0x00005910 File Offset: 0x00003B10
		public void RemoveFile(IFile file)
		{
			this.payload.RemoveAll((PkgFile x) => x.SourcePath.Equals(file.SourcePath, StringComparison.InvariantCultureIgnoreCase) && x.SourcePath.Equals(file.SourcePath, StringComparison.InvariantCultureIgnoreCase));
			base.TouchObject();
		}

		// Token: 0x060000ED RID: 237 RVA: 0x00005948 File Offset: 0x00003B48
		public void RemoveFile(IManifest file)
		{
			this.manifest.RemoveAll((PkgManifest x) => x.SourcePath.Equals(file.SourcePath, StringComparison.InvariantCultureIgnoreCase) && x.SourcePath.Equals(file.SourcePath, StringComparison.InvariantCultureIgnoreCase));
			base.TouchObject();
		}

		// Token: 0x060000EE RID: 238 RVA: 0x00005980 File Offset: 0x00003B80
		public void RemoveFile(FileType fileType, string destinationPath)
		{
			if (fileType == FileType.Regular)
			{
				this.payload.RemoveAll((PkgFile x) => x.CabPath.Equals(destinationPath, StringComparison.InvariantCultureIgnoreCase));
			}
			else if (fileType == FileType.Manifest)
			{
				this.manifest.RemoveAll((PkgManifest x) => x.CabPath.Equals(destinationPath, StringComparison.InvariantCultureIgnoreCase));
			}
			base.TouchObject();
		}

		// Token: 0x060000EF RID: 239 RVA: 0x000059DA File Offset: 0x00003BDA
		public void RemoveAllFiles()
		{
			this.payload.Clear();
			this.manifest.Clear();
			base.TouchObject();
		}

		// Token: 0x060000F0 RID: 240 RVA: 0x000059F8 File Offset: 0x00003BF8
		public List<Keyform> GetParents()
		{
			return this.parents;
		}

		// Token: 0x060000F1 RID: 241 RVA: 0x00005A00 File Offset: 0x00003C00
		public IEnumerable<string> GetAllSourcePaths()
		{
			return from x in this.AllFiles
			select x.SourcePath;
		}

		// Token: 0x060000F2 RID: 242 RVA: 0x00005A2C File Offset: 0x00003C2C
		public IEnumerable<string> GetAllCabPaths()
		{
			return from x in this.AllFiles
			select x.CabPath;
		}

		// Token: 0x060000F3 RID: 243 RVA: 0x00005A58 File Offset: 0x00003C58
		public XElement GetCBSFeatureInfo()
		{
			return this.fipInfo;
		}

		// Token: 0x060000F4 RID: 244 RVA: 0x00005A60 File Offset: 0x00003C60
		public void SetCBSFeatureInfo(string fmId, string featureId, string pkgGroup, List<IPackageInfo> packages)
		{
			if ((from pkg in packages
			where pkg.PackageName.Equals(this.keyform.Name, StringComparison.InvariantCultureIgnoreCase)
			select pkg).LongCount<IPackageInfo>() == 0L)
			{
				this.SerializeMum(PathToolBox.GetTemporaryPath());
				packages.Add(new CbsPackageInfo(this.SourcePath));
			}
			XNamespace @namespace = this.manifestRoot.Name.Namespace;
			XElement xelement = new XElement(@namespace + "declareCapability");
			XElement xelement2;
			if (string.IsNullOrEmpty(fmId))
			{
				xelement2 = new XElement(@namespace + "capability", new object[]
				{
					new XAttribute("group", this.PhoneInformation.PhoneOwnerType.ToString()),
					new XElement(@namespace + "capabilityIdentity", new XAttribute("name", featureId))
				});
			}
			else
			{
				xelement2 = new XElement(@namespace + "capability", new object[]
				{
					new XAttribute("group", this.PhoneInformation.PhoneOwnerType.ToString()),
					new XAttribute("FMID", fmId),
					new XElement(@namespace + "capabilityIdentity", new XAttribute("name", featureId))
				});
			}
			XElement xelement3 = new XElement(@namespace + "featurePackage");
			foreach (IPackageInfo pkgInfo in packages)
			{
				xelement3.Add(PkgManifest.GenerateFeaturePackageInfo(pkgInfo, @namespace, featureId, pkgGroup));
			}
			xelement2.Add(xelement3);
			xelement.Add(xelement2);
			this.fipInfo = xelement;
			this.Touch();
		}

		// Token: 0x060000F5 RID: 245 RVA: 0x00005C28 File Offset: 0x00003E28
		public void Clean()
		{
			base.WashObject();
		}

		// Token: 0x060000F6 RID: 246 RVA: 0x00005C30 File Offset: 0x00003E30
		public void Touch()
		{
			base.TouchObject();
		}

		// Token: 0x060000F7 RID: 247 RVA: 0x00005C38 File Offset: 0x00003E38
		private static XElement SerializeAssemblyIdentity(string name, Keyform keyform)
		{
			XElement xelement = new XElement(PkgConstants.CMIV3NS + "assemblyIdentity", new object[]
			{
				new XAttribute("name", name),
				new XAttribute("language", keyform.Language),
				new XAttribute("processorArchitecture", ManifestToolBox.GetCpuString(keyform)),
				new XAttribute("publicKeyToken", keyform.PublicKeyToken),
				new XAttribute("version", keyform.Version.ToString())
			});
			if (keyform.VersionScope.Equals("nonSxS", StringComparison.InvariantCultureIgnoreCase))
			{
				xelement.Add(new XAttribute("versionScope", keyform.VersionScope));
			}
			if (keyform.BuildType != BuildType.Invalid)
			{
				xelement.Add(new XAttribute("buildType", keyform.BuildType.ToString().ToLower(CultureInfo.InvariantCulture)));
			}
			if (!string.IsNullOrEmpty(keyform.InstallType))
			{
				xelement.Add(new XAttribute("type", keyform.InstallType));
			}
			return xelement;
		}

		// Token: 0x060000F8 RID: 248 RVA: 0x00005D6F File Offset: 0x00003F6F
		private static XElement SerializeAssemblyIdentity(Keyform keyform)
		{
			return PkgManifest.SerializeAssemblyIdentity(keyform.Name, keyform);
		}

		// Token: 0x060000F9 RID: 249 RVA: 0x00005D80 File Offset: 0x00003F80
		private static string SerializeEmptyDeployment(Keyform keyform, string outputDirectory)
		{
			string text = PathToolBox.Combine(outputDirectory, Keyform.GenerateKeyform(keyform) + PkgConstants.ManifestExtension);
			XDocument xdocument = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), new object[0]);
			XNamespace ns = PkgConstants.CMIV3NS;
			xdocument.Add(new XElement(ns + "assembly", new XAttribute("manifestVersion", "1.0")));
			XElement content = PkgManifest.SerializeAssemblyIdentity(keyform);
			xdocument.Root.Add(content);
			xdocument.Root.Add(new XElement(ns + "deployment"));
			ManifestToolBox.Save(xdocument, text);
			return text;
		}

		// Token: 0x060000FA RID: 250 RVA: 0x00005E30 File Offset: 0x00004030
		private static string SerializeEmptyComponent(Keyform keyform, string outputDirectory)
		{
			string text = PathToolBox.Combine(outputDirectory, Keyform.GenerateKeyform(keyform) + PkgConstants.ManifestExtension);
			XDocument xdocument = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), new object[0]);
			XNamespace ns = PkgConstants.CMIV3NS;
			xdocument.Add(new XElement(ns + "assembly", new XAttribute("manifestVersion", "1.0")));
			XElement content = PkgManifest.SerializeAssemblyIdentity(keyform);
			xdocument.Root.Add(content);
			ManifestToolBox.Save(xdocument, text);
			return text;
		}

		// Token: 0x060000FB RID: 251 RVA: 0x00005EC4 File Offset: 0x000040C4
		private void SerializeMum(string outputFolder)
		{
			XDocument xdocument = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), new object[0]);
			XNamespace ns = PkgConstants.CMIV3NS;
			xdocument.Add(new XElement(ns + "assembly", new XAttribute("manifestVersion", "1.0")));
			string name = this.Keyform.Name;
			XElement content = PkgManifest.SerializeAssemblyIdentity(name, this.Keyform);
			xdocument.Root.Add(content);
			if (this.metrics.PackageType == PackageType.OneCore && this.Keyform.GuestArch != CpuArch.Invalid && this.Keyform.ReleaseType.Equals("Product", StringComparison.InvariantCultureIgnoreCase))
			{
				this.Keyform.ReleaseType = "Feature Pack";
			}
			XElement xelement = new XElement(ns + "package", new object[]
			{
				new XAttribute("identifier", name),
				new XAttribute("releaseType", this.Keyform.ReleaseType)
			});
			if (this.SelfUpdate)
			{
				xelement.Add(new XAttribute("selfUpdate", this.SelfUpdate));
			}
			if (this.NoMerge)
			{
				xelement.Add(new XElement(ns + "customInformation", new XElement(ns + "noAutoMerge")));
			}
			if (this.BinaryPartition != null)
			{
				xelement.Add(new XAttribute("binaryPartition", this.BinaryPartition));
			}
			if (!string.IsNullOrEmpty(this.Partition))
			{
				xelement.Add(new XAttribute("targetPartition", this.Partition));
			}
			foreach (Keyform keyform in this.GetParents())
			{
				XElement xelement2 = new XElement(ns + "parent", new XAttribute("integrate", "delegate"));
				XElement xelement3 = new XElement(ns + "assemblyIdentity", new object[]
				{
					new XAttribute("name", keyform.Name),
					new XAttribute("language", keyform.Language),
					new XAttribute("processorArchitecture", ManifestToolBox.GetCpuString(keyform)),
					new XAttribute("publicKeyToken", keyform.PublicKeyToken),
					new XAttribute("version", keyform.Version.ToString())
				});
				if (keyform.BuildType != BuildType.Invalid)
				{
					xelement3.Add(new XAttribute("buildType", keyform.BuildType.ToString().ToLower(CultureInfo.InvariantCulture)));
				}
				xelement2.Add(xelement3);
				xelement.Add(xelement2);
			}
			foreach (IManifest manifest in this.manifest)
			{
				XElement xelement4 = new XElement(ns + "update", new XAttribute("name", (!string.IsNullOrEmpty(manifest.UpdateName)) ? manifest.UpdateName : manifest.FileHash));
				XElement content2 = PkgManifest.SerializeAssemblyIdentity(manifest.Keyform);
				if (manifest.ManifestType == ManifestType.Driver)
				{
					xelement4.Add(new XAttribute("restart", "required"));
					XElement xelement5 = new XElement(ns + "driver", new object[]
					{
						new XAttribute("inf", manifest.InfName),
						new XAttribute("ranking", manifest.InfRanking)
					});
					xelement5.Add(content2);
					xelement4.AddFirst(xelement5);
				}
				else if (manifest.ManifestType == ManifestType.Package)
				{
					XElement xelement6 = new XElement(ns + "package");
					xelement6.Add(content2);
					xelement4.Add(xelement6);
				}
				else
				{
					XElement xelement7 = new XElement(ns + "component");
					xelement7.Add(content2);
					xelement4.Add(xelement7);
				}
				xelement.Add(xelement4);
			}
			xdocument.Root.Add(xelement);
			if (this.metrics.PackageType == PackageType.OneCore)
			{
				this.CabPath = PkgConstants.UpdateMum;
			}
			this.SourcePath = PathToolBox.Combine(outputFolder, this.CabPath);
			ManifestToolBox.Save(xdocument, this.SourcePath);
			if (this.metrics.PackageType == PackageType.OneCore)
			{
				this.GenerateAndSignCatalog(false);
				this.GenerateCustomInformation(ManifestToolBox.Load(this.SourcePath));
			}
			this.GenerateAndSignCatalog(true);
		}

		// Token: 0x060000FC RID: 252 RVA: 0x000063DC File Offset: 0x000045DC
		private void GenerateCustomInformation(XDocument workingMan)
		{
			foreach (IManifest manifest in (from x in this.AllManifestFiles
			where x.ManifestType == ManifestType.Package
			select x).ToList<IManifest>())
			{
				XNamespace ns = PkgConstants.CMIV3NS;
				XElement xelement = new XElement(ns + "customInformation");
				XNamespace @namespace = workingMan.Root.Name.Namespace;
				XElement xelement2 = workingMan.Root.Elements(@namespace + "package").First<XElement>();
				if (xelement2.Element(@namespace + "customInformation") != null)
				{
					xelement2.Element(@namespace + "customInformation").Remove();
				}
				if (this.fipInfo != null)
				{
					xelement.Add(this.fipInfo);
				}
				if (this.phoneSecurityInfo != null)
				{
					xelement.Add(this.phoneSecurityInfo);
				}
				if (!string.IsNullOrEmpty(manifest.PhoneInformation.PhoneComponent))
				{
					string text = manifest.PhoneInformation.PhoneSubComponent;
					if (this.Keyform.GuestArch != CpuArch.Invalid)
					{
						if (string.IsNullOrEmpty(text))
						{
							text = "Guest";
						}
						else
						{
							text += ".Guest";
						}
					}
					XElement content = new XElement(ns + "phoneInformation", new object[]
					{
						new XAttribute("phoneRelease", manifest.PhoneInformation.PhoneReleaseType.ToString()),
						new XAttribute("phoneOwner", manifest.PhoneInformation.PhoneOwner),
						new XAttribute("phoneOwnerType", manifest.PhoneInformation.PhoneOwnerType.ToString()),
						new XAttribute("phoneComponent", manifest.PhoneInformation.PhoneComponent),
						new XAttribute("phoneSubComponent", text),
						new XAttribute("phoneGroupingKey", manifest.PhoneInformation.PhoneGroupingKey)
					});
					xelement.Add(content);
				}
				foreach (IManifest manifest2 in new HashSet<IManifest>
				{
					manifest
				}.Union(manifest.AllManifestFiles))
				{
					ulong num;
					ulong num2;
					ulong num3;
					FileToolBox.ImagingSizes(manifest2.SourcePath, out num, out num2, out num3);
					XElement xelement3 = new XElement(ns + "file", new object[]
					{
						new XAttribute("name", Path.GetFileName(manifest2.SourcePath)),
						new XAttribute("size", num),
						new XAttribute("staged", num2),
						new XAttribute("compressed", num3),
						new XAttribute("sourcePackage", manifest2.SourcePackage),
						new XAttribute("cabpath", Path.GetFileName(manifest2.SourcePath))
					});
					xelement.Add(xelement3);
					foreach (IFile file in manifest2.CurrentPayloadFiles)
					{
						xelement3 = new XElement(ns + "file", new object[]
						{
							new XAttribute("name", Path.Combine(file.DevicePath, PathToolBox.GetFileNameFromPath(file.CabPath))),
							new XAttribute("size", file.Size),
							new XAttribute("staged", file.StagedSize),
							new XAttribute("compressed", file.CompressedSize),
							new XAttribute("sourcePackage", manifest2.SourcePackage),
							new XAttribute("cabpath", file.CabPath)
						});
						if (file.SignInfoRequired)
						{
							xelement3.Add(new XAttribute("hash", file.FileHash));
							xelement3.Add(new XAttribute("signInfo", "true"));
						}
						xelement.Add(xelement3);
					}
				}
				xelement2.AddFirst(xelement);
			}
			ManifestToolBox.Save(workingMan, this.SourcePath);
		}

		// Token: 0x060000FD RID: 253 RVA: 0x000068E0 File Offset: 0x00004AE0
		private void SerializeDeployment(string mergedPackageName, string outputFolder)
		{
			this.Keyform.Name = mergedPackageName.ToLower(CultureInfo.InvariantCulture).Replace("package", "merged");
			this.Keyform.Name = this.Keyform.Name.Replace('.', '-');
			Keyform keyform = this.Keyform;
			keyform.Name = keyform.Name + "-" + this.ManifestType.ToString();
			this.UpdateName = this.Keyform.Name;
			XDocument xdocument = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), new object[0]);
			XNamespace ns = PkgConstants.CMIV3NS;
			xdocument.Add(new XElement(ns + "assembly", new XAttribute("manifestVersion", "1.0")));
			XElement content = PkgManifest.SerializeAssemblyIdentity(this.Keyform);
			xdocument.Root.Add(content);
			xdocument.Root.Add(new XElement(ns + "deployment"));
			foreach (IManifest manifest in this.manifest)
			{
				XElement xelement = new XElement(ns + "dependency");
				XElement xelement2 = new XElement(ns + "dependentAssembly", new XAttribute("dependencyType", "install"));
				XElement content2 = PkgManifest.SerializeAssemblyIdentity(manifest.Keyform);
				xelement2.Add(content2);
				xelement.Add(xelement2);
				xdocument.Root.Add(xelement);
			}
			this.CabPath = Keyform.GenerateKeyform(this.Keyform) + PkgConstants.ManifestExtension;
			this.SourcePath = PathToolBox.Combine(outputFolder, this.CabPath);
			ManifestToolBox.Save(xdocument, this.SourcePath);
		}

		// Token: 0x060000FE RID: 254 RVA: 0x00006AD8 File Offset: 0x00004CD8
		private void SerializeComponent(string outputFolder)
		{
			XDocument xdocument = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), new object[0]);
			XNamespace ns = PkgConstants.CMIV3NS;
			xdocument.Add(new XElement(ns + "assembly", new XAttribute("manifestVersion", "1.0")));
			XElement content = PkgManifest.SerializeAssemblyIdentity(this.Keyform);
			xdocument.Root.Add(content);
			foreach (IFile file in this.payload)
			{
				XElement xelement = new XElement(ns + "file");
				xelement.Add(new XAttribute("name", PathToolBox.GetFileNameFromPath(file.CabPath)));
				xelement.Add(new XAttribute("destinationPath", file.DevicePath));
				XNamespace ns2 = "urn:schemas-microsoft-com:asm.v2";
				XNamespace ns3 = "http://www.w3.org/2000/09/xmldsig#";
				string text = Convert.ToBase64String(SHA256.Create().ComputeHash(new FileStream(file.SourcePath, FileMode.Open, FileAccess.Read)));
				XElement xelement2 = new XElement(ns2 + "hash", new XAttribute(XNamespace.Xmlns + "asmv2", "urn:schemas-microsoft-com:asm.v2"));
				XElement xelement3 = new XElement(ns3 + "Transforms", new XAttribute(XNamespace.Xmlns + "dsig", "http://www.w3.org/2000/09/xmldsig#"));
				xelement3.Add(new XElement(ns3 + "Transform", new XAttribute("Algorithm", "urn:schemas-microsoft-com:HashTransforms.Identity")));
				XElement content2 = new XElement(ns3 + "DigestMethod", new object[]
				{
					new XAttribute(XNamespace.Xmlns + "dsig", "http://www.w3.org/2000/09/xmldsig#"),
					new XAttribute("Algorithm", "http://www.w3.org/2000/09/xmldsig#sha256")
				});
				XElement content3 = new XElement(ns3 + "DigestValue", new object[]
				{
					new XAttribute(XNamespace.Xmlns + "dsig", "http://www.w3.org/2000/09/xmldsig#"),
					text
				});
				xelement2.Add(xelement3);
				xelement2.Add(content2);
				xelement2.Add(content3);
				xelement.Add(xelement2);
				xdocument.Root.Add(xelement);
			}
			if (this.registry.Any<XElement>())
			{
				XElement xelement4 = new XElement(ns + "registryKeys");
				foreach (XElement content4 in this.registry)
				{
					xelement4.Add(content4);
				}
				xdocument.Root.Add(xelement4);
			}
			this.SourcePath = PathToolBox.Combine(outputFolder, this.CabPath);
			ManifestToolBox.Save(xdocument, this.SourcePath);
		}

		// Token: 0x060000FF RID: 255 RVA: 0x00006E00 File Offset: 0x00005000
		private void Load()
		{
			if (!File.Exists(this.SourcePath))
			{
				throw new FileNotFoundException(string.Format("PkgManifest::Load: File '{0}' was not found.", this.SourcePath));
			}
			this.ManifestType = ManifestToolBox.GetManifestType(this.SourcePath, this.manifestRoot);
			this.Keyform.BuildType = BuildType.Invalid;
			this.ParseCustomInformation(this.manifestRoot);
			this.ParseAssemblyIdentity(this.manifestRoot.Element(this.manifestRoot.Name.Namespace + "assemblyIdentity"));
			if (this.metrics.PackageLoadType == LoadType.Package)
			{
				XNamespace @namespace = this.manifestRoot.Name.Namespace;
				if (this.ManifestType == ManifestType.Package)
				{
					using (IEnumerator<XElement> enumerator = this.manifestRoot.Elements(@namespace + "package").GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							XElement xelement = enumerator.Current;
							foreach (XElement xelement2 in xelement.Elements(@namespace + "update"))
							{
								foreach (XElement element in xelement2.Descendants(this.manifestRoot.Name.Namespace + "assemblyIdentity"))
								{
									this.LoadManifest(element);
								}
							}
							this.ParseParents(@namespace, xelement);
						}
						goto IL_238;
					}
				}
				foreach (XElement xelement3 in this.manifestRoot.Elements(@namespace + "dependency"))
				{
					foreach (XElement xelement4 in xelement3.Elements(@namespace + "dependentAssembly"))
					{
						foreach (XElement element2 in xelement4.Descendants(this.manifestRoot.Name.Namespace + "assemblyIdentity"))
						{
							this.LoadManifest(element2);
						}
					}
				}
				IL_238:
				this.LoadCatalog();
				this.ProcessManifestFiles();
			}
		}

		// Token: 0x06000100 RID: 256 RVA: 0x000070A0 File Offset: 0x000052A0
		private void LoadManifest(XElement element)
		{
			bool flag = this.IsManifestDiscoverable(element);
			string value = element.Attribute("name").Value;
			string text = (element.Attribute("language") == null) ? "neutral" : element.Attribute("language").Value;
			string value2 = element.Attribute("processorArchitecture").Value;
			string value3 = element.Attribute("publicKeyToken").Value;
			string value4;
			if (element.Attribute("version") == null)
			{
				value4 = element.Parent.Element(element.Name.Namespace + "bindingRedirect").Attribute("newVersion").Value;
			}
			else
			{
				value4 = element.Attribute("version").Value;
			}
			string text2 = (element.Attribute("type") == null) ? string.Empty : element.Attribute("type").Value;
			string text3 = (element.Attribute("versionScope") == null) ? "SxS" : element.Attribute("versionScope").Value;
			string text4 = Keyform.GenerateCBSKeyform(value, value3, value2, value4, text);
			text4 = PathToolBox.Combine(DirectoryToolBox.GetDirectoryFromFilePath(this.SourcePath), text4) + PkgConstants.MumExtension;
			if (FileToolBox.Exists(text4))
			{
				PkgManifest pkgManifest = PkgManifest.LoadFromDisk(text4, this.metrics, this.Keyform.Name);
				if (!string.IsNullOrEmpty(element.Parent.Parent.Attribute("name").Value))
				{
					pkgManifest.UpdateName = element.Parent.Parent.Attribute("name").Value;
				}
				pkgManifest.UpdateName = element.Parent.Parent.Attribute("name").Value.ToString();
				if (element.Parent.Parent.Descendants(this.manifestRoot.Name.Namespace + "selectable").Count<XElement>() > 0 || (element.Parent.Parent.Descendants(this.manifestRoot.Name.Namespace + "noAutoMerge").Count<XElement>() > 0 && this.metrics.PackageType != PackageType.OneCore))
				{
					this.metrics.Logger.LogInfo("Load: nomerge/selectable manifest", new object[0]);
					pkgManifest.NoMerge = true;
				}
				this.AddFile(pkgManifest);
				return;
			}
			string language = (text.Equals("*") || string.IsNullOrEmpty(text)) ? "neutral" : text;
			text4 = Keyform.GenerateKeyform(value2, value, value4, value3, language, text2, text3);
			text4 = PathToolBox.Combine(DirectoryToolBox.GetDirectoryFromFilePath(this.SourcePath), text4) + PkgConstants.ManifestExtension;
			bool flag2 = FileToolBox.Exists(text4);
			if (!flag2 && text3.Equals("nonSxS", StringComparison.InvariantCultureIgnoreCase))
			{
				text3 = string.Empty;
				text4 = Keyform.GenerateKeyform(value2, value, value4, value3, language, text2, text3);
				text4 = PathToolBox.Combine(DirectoryToolBox.GetDirectoryFromFilePath(this.SourcePath), text4) + PkgConstants.ManifestExtension;
				flag2 = FileToolBox.Exists(text4);
			}
			if (flag2)
			{
				PkgManifest pkgManifest2 = PkgManifest.LoadFromDisk(text4, this.metrics, (this.ManifestType == ManifestType.Package) ? this.Keyform.Name : this.SourcePackage);
				if (pkgManifest2.ManifestType == ManifestType.Component && pkgManifest2.Keyform.Language != null && pkgManifest2.Keyform.Language != text)
				{
					pkgManifest2 = new PkgManifest(text4, this.SourcePackage, this.metrics);
					pkgManifest2.ManifestType = ManifestToolBox.GetManifestType(text4, ManifestToolBox.Load(text4).Root);
					pkgManifest2.Keyform.BuildType = BuildType.Invalid;
				}
				if (pkgManifest2.ManifestType != ManifestType.Package && pkgManifest2.ManifestType != ManifestType.Deployment)
				{
					pkgManifest2.Keyform.Name = value;
					if (element.Attribute("buildType") != null && !string.IsNullOrEmpty(element.Attribute("buildType").Value))
					{
						pkgManifest2.Keyform.BuildType = (BuildType)Enum.Parse(typeof(BuildType), element.Attribute("buildType").Value, true);
					}
					pkgManifest2.Keyform.Language = text;
					pkgManifest2.Keyform.HostArch = ManifestToolBox.GetHostType(value2);
					pkgManifest2.Keyform.GuestArch = ManifestToolBox.GetGuestType(value2);
					pkgManifest2.Keyform.PublicKeyToken = value3;
					pkgManifest2.Keyform.Version = new Version(value4);
					pkgManifest2.Keyform.VersionScope = text3;
					pkgManifest2.Keyform.InstallType = text2;
				}
				if (element.Parent.Name.LocalName.Equals("driver"))
				{
					pkgManifest2.InfName = element.Parent.Attribute("inf").Value;
					pkgManifest2.InfRanking = element.Parent.Attribute("ranking").Value;
				}
				if (text2.Equals("win32", StringComparison.InvariantCultureIgnoreCase) || text2.Equals("win32-policy", StringComparison.InvariantCultureIgnoreCase))
				{
					this.metrics.Logger.LogInfo("Win32/win32-policy manifest {0}", new object[]
					{
						pkgManifest2.SourcePath
					});
					pkgManifest2.NoMerge = true;
				}
				if (this.ManifestType == ManifestType.Package)
				{
					pkgManifest2.UpdateName = element.Parent.Parent.Attribute("name").Value.ToString();
					if (element.Parent.Parent.Descendants(this.manifestRoot.Name.Namespace + "selectable").Count<XElement>() > 0 || (element.Parent.Parent.Descendants(this.manifestRoot.Name.Namespace + "noAutoMerge").Count<XElement>() > 0 && this.metrics.PackageType != PackageType.OneCore))
					{
						this.metrics.Logger.LogInfo("Nomerge/selectable manifest {0}", new object[]
						{
							pkgManifest2.SourcePath
						});
						pkgManifest2.NoMerge = true;
					}
				}
				this.AddFile(pkgManifest2);
				return;
			}
			if (flag)
			{
				throw new FileNotFoundException(string.Format("PkgManifest::Load: Could not find manifest with the name '{0}'.", value));
			}
		}

		// Token: 0x06000101 RID: 257 RVA: 0x00007730 File Offset: 0x00005930
		private void LoadCatalog()
		{
			if (this.ManifestType == ManifestType.Package)
			{
				string text = Path.ChangeExtension(this.SourcePath, PkgConstants.CatExtension);
				if (FileToolBox.Exists(text))
				{
					this.AddFile(FileType.Catalog, text, PkgConstants.RuntimeBootdrive, this.SourcePackage);
				}
			}
		}

		// Token: 0x06000102 RID: 258 RVA: 0x00007774 File Offset: 0x00005974
		private void ParseCustomInformation(XElement manifestRoot)
		{
			if (this.ManifestType == ManifestType.Package)
			{
				XElement xelement = manifestRoot.Elements(manifestRoot.Name.Namespace + "package").First<XElement>();
				this.Keyform.ReleaseType = xelement.Attribute("releaseType").Value;
				if (xelement.Attribute("binaryPartition") != null)
				{
					this.BinaryPartition = new bool?(xelement.Attribute("binaryPartition").Value.Equals("true", StringComparison.InvariantCultureIgnoreCase));
				}
				if (xelement.Attribute("targetPartition") != null)
				{
					this.Partition = xelement.Attribute("targetPartition").Value;
				}
				if (xelement.Attribute("selfUpdate") != null)
				{
					this.SelfUpdate = xelement.Attribute("selfUpdate").Value.Equals("true", StringComparison.InvariantCultureIgnoreCase);
				}
				XElement xelement2 = xelement.Element(manifestRoot.Name.Namespace + "customInformation");
				if (xelement2 != null)
				{
					this.fipInfo = xelement2.Element(manifestRoot.Name.Namespace + "declareCapability");
					this.phoneSecurityInfo = xelement2.Element(manifestRoot.Name.Namespace + "phoneSecurityPolicyMarker");
					XElement xelement3 = xelement2.Element(manifestRoot.Name.Namespace + "phoneInformation");
					if (xelement3 != null)
					{
						this.PhoneInformation.PhoneReleaseType = (PhoneReleaseType)Enum.Parse(typeof(PhoneReleaseType), xelement3.Attribute("phoneRelease").Value, true);
						this.PhoneInformation.PhoneOwner = xelement3.Attribute("phoneOwner").Value.ToString();
						this.PhoneInformation.PhoneOwnerType = (PhoneOwnerType)Enum.Parse(typeof(PhoneOwnerType), xelement3.Attribute("phoneOwnerType").Value, true);
						this.PhoneInformation.PhoneComponent = xelement3.Attribute("phoneComponent").Value;
						this.PhoneInformation.PhoneSubComponent = xelement3.Attribute("phoneSubComponent").Value;
						this.PhoneInformation.PhoneGroupingKey = xelement3.Attribute("phoneGroupingKey").Value;
					}
				}
			}
		}

		// Token: 0x06000103 RID: 259 RVA: 0x000079E8 File Offset: 0x00005BE8
		private void ParseAssemblyIdentity(XElement assemblyIdentity)
		{
			if (assemblyIdentity.Attribute("buildType") != null)
			{
				this.Keyform.BuildType = (BuildType)Enum.Parse(typeof(BuildType), assemblyIdentity.Attribute("buildType").Value, true);
			}
			this.Keyform.Name = assemblyIdentity.Attribute("name").Value;
			this.Keyform.Language = ((assemblyIdentity.Attribute("language") == null) ? string.Empty : assemblyIdentity.Attribute("language").Value);
			this.Keyform.HostArch = ManifestToolBox.GetHostType(assemblyIdentity.Attribute("processorArchitecture").Value);
			this.Keyform.GuestArch = ManifestToolBox.GetGuestType(assemblyIdentity.Attribute("processorArchitecture").Value);
			this.Keyform.PublicKeyToken = assemblyIdentity.Attribute("publicKeyToken").Value;
			this.Keyform.VersionScope = ((assemblyIdentity.Attribute("versionScope") == null) ? "SxS" : assemblyIdentity.Attribute("versionScope").Value);
			string value;
			if (assemblyIdentity.Attribute("version") == null)
			{
				value = assemblyIdentity.Parent.Element(assemblyIdentity.Name.Namespace + "bindingRedirect").Attribute("newVersion").Value;
			}
			else
			{
				value = assemblyIdentity.Attribute("version").Value;
			}
			this.Keyform.Version = new Version(value);
			this.Keyform.InstallType = ((assemblyIdentity.Attribute("type") == null) ? string.Empty : assemblyIdentity.Attribute("type").Value);
		}

		// Token: 0x06000104 RID: 260 RVA: 0x00007BE4 File Offset: 0x00005DE4
		private void ParseParents(XNamespace rootNS, XElement package)
		{
			bool flag = false;
			if (this.metrics.PackageType == PackageType.Regular || this.metrics.PackageType == PackageType.FOD_Neutral)
			{
				flag = true;
			}
			else if (this.metrics.PackageType == PackageType.FOD_Lang && string.IsNullOrEmpty(this.SourcePackage))
			{
				flag = true;
			}
			if (flag)
			{
				foreach (XElement xelement in package.Elements(rootNS + "parent"))
				{
					foreach (XElement xelement2 in xelement.Descendants(this.manifestRoot.Name.Namespace + "assemblyIdentity"))
					{
						Keyform keyform = new Keyform();
						keyform.Name = xelement2.Attribute("name").Value;
						keyform.Language = ((xelement2.Attribute("language") == null) ? string.Empty : xelement2.Attribute("language").Value);
						keyform.HostArch = ManifestToolBox.GetHostType(xelement2.Attribute("processorArchitecture").Value);
						keyform.GuestArch = ManifestToolBox.GetGuestType(xelement2.Attribute("processorArchitecture").Value);
						keyform.PublicKeyToken = xelement2.Attribute("publicKeyToken").Value;
						keyform.Version = new Version(xelement2.Attribute("version").Value);
						keyform.BuildType = BuildType.Invalid;
						if (xelement2.Attribute("buildType") != null)
						{
							keyform.BuildType = (BuildType)Enum.Parse(typeof(BuildType), xelement2.Attribute("buildType").Value, true);
						}
						this.parents.Add(keyform);
					}
					base.TouchObject();
				}
			}
		}

		// Token: 0x06000105 RID: 261 RVA: 0x00007E20 File Offset: 0x00006020
		private bool IsManifestDiscoverable(XElement assemblyIdentity)
		{
			bool result = true;
			XNamespace ns = PkgConstants.CMIV3NS;
			if (assemblyIdentity.Parent.Parent.Attribute("discoverable") != null)
			{
				if (assemblyIdentity.Parent.Parent.Attribute("discoverable").Value.Equals("no", StringComparison.InvariantCultureIgnoreCase))
				{
					result = false;
				}
			}
			else if (assemblyIdentity.Parent.Parent.Attribute(ns + "discoverable") != null && assemblyIdentity.Parent.Parent.Attribute(ns + "discoverable").Value.Equals("no", StringComparison.InvariantCultureIgnoreCase))
			{
				result = false;
			}
			if (assemblyIdentity.Parent.Parent.Attribute("resourceType") != null && assemblyIdentity.Parent.Parent.Attribute("resourceType").Value.Equals("Resources", StringComparison.InvariantCultureIgnoreCase))
			{
				result = false;
			}
			if (assemblyIdentity.Parent.Attribute("dependencyType") != null && assemblyIdentity.Parent.Attribute("dependencyType").Value.Equals("prerequisite", StringComparison.InvariantCultureIgnoreCase))
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06000106 RID: 262 RVA: 0x00007F5D File Offset: 0x0000615D
		private bool IsLeaf()
		{
			return this.manifest.Count == 0;
		}

		// Token: 0x06000107 RID: 263 RVA: 0x00007F70 File Offset: 0x00006170
		private void GenerateAndSignCatalog(bool signCatalog = true)
		{
			IFile file = this.CurrentPayloadFiles.FirstOrDefault((IFile x) => x.FileType == FileType.Catalog);
			if (file != null)
			{
				this.RemoveFile(file);
			}
			List<string> list = new List<string>();
			list.AddRange(from x in this.AllFiles
			select x.SourcePath);
			string packageName = this.Keyform.Name;
			if (this.PhoneInformation.PhoneReleaseType.Equals(PhoneReleaseType.Test))
			{
				packageName = "DebugPackage";
			}
			string text = PathToolBox.Combine(PathToolBox.GetTemporaryPath(), Path.GetFileNameWithoutExtension(this.SourcePath) + ".cat");
			SecurityToolBox.CreateCatalog(text, list, packageName);
			if (signCatalog)
			{
				SecurityToolBox.SignFile(text);
			}
			this.AddFile(FileType.Catalog, text, "$(runtime.system32)\\catroot\\{F750E6C3-38EE-11D1-85E5-00C04FC295EE}", this.SourcePackage);
		}

		// Token: 0x06000108 RID: 264 RVA: 0x00008060 File Offset: 0x00006260
		private PkgManifest CreateEmptyManifest(ManifestType manifestType)
		{
			Keyform keyform = new Keyform(this.Keyform.Name + "-" + manifestType.ToString(), this.Keyform.HostArch, this.Keyform.GuestArch, this.Keyform.BuildType, this.Keyform.ReleaseType, this.Keyform.Language, PkgConstants.NonSxSVersionScope, this.Keyform.Version, this.Keyform.PublicKeyToken);
			string temporaryPath = PathToolBox.GetTemporaryPath();
			string sourcePath = string.Empty;
			if (manifestType == ManifestType.Component)
			{
				sourcePath = PkgManifest.SerializeEmptyComponent(keyform, temporaryPath);
			}
			else
			{
				if (manifestType != ManifestType.Deployment)
				{
					throw new PkgException("PkgManifest::CreateEmptyManifest: Cannot create empty manifest of type {0}", new object[]
					{
						manifestType.ToString()
					});
				}
				sourcePath = PkgManifest.SerializeEmptyDeployment(keyform, temporaryPath);
			}
			return PkgManifest.LoadFromDisk(sourcePath, this.metrics);
		}

		// Token: 0x06000109 RID: 265 RVA: 0x0000813C File Offset: 0x0000633C
		private PkgManifest GetValidManifest(FileType newFileType, ManifestType newManifestType = ManifestType.Invalid)
		{
			if (newFileType == FileType.Regular || newFileType == FileType.Registry)
			{
				if (this.ManifestType != ManifestType.Component && this.ManifestType != ManifestType.Driver)
				{
					PkgManifest pkgManifest = this.manifest.FirstOrDefault((PkgManifest x) => x.ManifestType == ManifestType.Deployment);
					if (this.ManifestType != ManifestType.Deployment)
					{
						if (pkgManifest == null)
						{
							pkgManifest = this.CreateEmptyManifest(ManifestType.Deployment);
							this.AddFile(pkgManifest);
						}
					}
					else
					{
						pkgManifest = this;
					}
					PkgManifest pkgManifest2 = pkgManifest.manifest.FirstOrDefault((PkgManifest x) => x.ManifestType == ManifestType.Component);
					if (pkgManifest2 == null)
					{
						pkgManifest2 = this.CreateEmptyManifest(ManifestType.Component);
						pkgManifest.AddFile(pkgManifest2);
					}
					return pkgManifest2;
				}
			}
			else if (newFileType == FileType.Manifest)
			{
				if (newManifestType == ManifestType.Component && this.ManifestType == ManifestType.Package)
				{
					PkgManifest pkgManifest3 = this.manifest.FirstOrDefault((PkgManifest x) => x.ManifestType == ManifestType.Deployment);
					if (pkgManifest3 == null)
					{
						pkgManifest3 = this.CreateEmptyManifest(ManifestType.Deployment);
					}
					this.AddFile(pkgManifest3);
					return pkgManifest3;
				}
				if ((newManifestType == ManifestType.Deployment && this.ManifestType == ManifestType.Deployment) || (newManifestType == ManifestType.Component && this.ManifestType == ManifestType.Component))
				{
					this.metrics.Logger.LogWarning("{0} {1} is referencing a {2}", new object[]
					{
						this.ManifestType,
						this.Keyform.Name,
						newManifestType
					});
				}
				else if ((newManifestType == ManifestType.Package && this.ManifestType == ManifestType.Deployment) || (newManifestType == ManifestType.Package && this.ManifestType == ManifestType.Component) || (newManifestType == ManifestType.Deployment && this.ManifestType == ManifestType.Component) || (newManifestType == ManifestType.Deployment && this.ManifestType == ManifestType.Deployment))
				{
					throw new PkgException("PkgManifest::GetValidManifest: Cannot add {0} to {1}", new object[]
					{
						newManifestType.ToString(),
						this.ManifestType.ToString()
					});
				}
			}
			return this;
		}

		// Token: 0x0600010A RID: 266 RVA: 0x00008314 File Offset: 0x00006514
		private void ProcessManifestFiles()
		{
			string text = Path.Combine(Path.GetDirectoryName(this.SourcePath), Path.GetFileNameWithoutExtension(this.CabPath));
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(this.CabPath);
			string sourcePath = this.SourcePath;
			if (Directory.Exists(text))
			{
				if (!NativeMethods.ParseManaged(this.metrics.ParsemanifestSession, sourcePath))
				{
					throw new PkgException("PkgManifest::ProcessManifestFiles: Failed to parse manifest file: {0}. The manifest format is incorrect.", new object[]
					{
						sourcePath
					});
				}
				int fileCountManaged = NativeMethods.GetFileCountManaged(this.metrics.ParsemanifestSession);
				for (int i = 0; i < fileCountManaged; i++)
				{
					IntPtr intPtr;
					if (NativeMethods.GetSourceNameManaged(this.metrics.ParsemanifestSession, i, out intPtr) != 0)
					{
						throw new PkgException("PkgManifest::ProcessManifestFiles: Failed to get SourceName for file {1}: {0}", new object[]
						{
							i + 1,
							sourcePath
						});
					}
					string text2 = Marshal.PtrToStringUni(intPtr);
					NativeMethods.ManagedMemoryFree(intPtr);
					if (NativeMethods.GetSourcePathManaged(this.metrics.ParsemanifestSession, i, out intPtr) != 0)
					{
						throw new PkgException("PkgManifest::ProcessManifestFiles: Failed to get SourcePath for file {1}: {0}", new object[]
						{
							i + 1,
							sourcePath
						});
					}
					string str = Marshal.PtrToStringUni(intPtr);
					NativeMethods.ManagedMemoryFree(intPtr);
					if (NativeMethods.GetDestinationNameManaged(this.metrics.ParsemanifestSession, i, out intPtr) != 0)
					{
						throw new PkgException("PkgManifest::ProcessManifestFiles: Failed to get DestinationName for file {1}: {0}", new object[]
						{
							i + 1,
							sourcePath
						});
					}
					Marshal.PtrToStringUni(intPtr);
					NativeMethods.ManagedMemoryFree(intPtr);
					if (NativeMethods.GetDestinationPathManaged(this.metrics.ParsemanifestSession, i, out intPtr) != 0)
					{
						throw new PkgException("PkgManifest::ProcessManifestFiles: Failed to get DestinationPath for file {1}: {0}", new object[]
						{
							i + 1,
							sourcePath
						});
					}
					string text3 = Marshal.PtrToStringUni(intPtr);
					NativeMethods.ManagedMemoryFree(intPtr);
					string text4 = text2;
					string text5 = "\\" + str;
					int num = text4.LastIndexOf("\\", StringComparison.InvariantCultureIgnoreCase);
					if (text4.LastIndexOf("\\", StringComparison.InvariantCultureIgnoreCase) >= 0)
					{
						text5 += text4.Substring(0, num + 1);
						text4 = text4.Substring(num + 1);
					}
					if (!text5.EndsWith("\\", StringComparison.InvariantCultureIgnoreCase))
					{
						text5 += "\\";
					}
					text5 = text5.Replace("\\.\\", "\\");
					string text6;
					if (string.IsNullOrEmpty(text3))
					{
						text6 = PathToolBox.Combine(PkgConstants.FileRedirectPath, fileNameWithoutExtension);
						text4 = text2;
					}
					else
					{
						NativeMethods.ReplaceMacrosManaged(this.metrics.ParsemanifestSession, text3, out intPtr);
						text6 = Marshal.PtrToStringUni(intPtr);
						NativeMethods.ManagedMemoryFree(intPtr);
						if (string.IsNullOrEmpty(text6))
						{
							throw new NullReferenceException(string.Format("Failed to replace macros in path {0}. Macros in path may not be supported.", text3));
						}
					}
					this.AddFile(new PkgFile(FileType.Regular, text + text5 + text4, PkgConstants.RuntimeBootdrive + text6, fileNameWithoutExtension + text5 + text4, this.SourcePackage)
					{
						SignInfoRequired = this.metrics.SignInfoFiles.Contains(PathToolBox.GetFileNameFromPath(text4))
					});
				}
			}
		}

		// Token: 0x0400002B RID: 43
		private PackageMetrics metrics;

		// Token: 0x0400002C RID: 44
		private List<Keyform> parents = new List<Keyform>();

		// Token: 0x0400002D RID: 45
		private List<PkgManifest> manifest = new List<PkgManifest>();

		// Token: 0x0400002E RID: 46
		private List<PkgFile> payload = new List<PkgFile>();

		// Token: 0x0400002F RID: 47
		private List<XElement> registry = new List<XElement>();

		// Token: 0x04000030 RID: 48
		private PkgFile pkgFile;

		// Token: 0x04000031 RID: 49
		private XElement manifestRoot;

		// Token: 0x04000032 RID: 50
		private Keyform keyform = new Keyform();

		// Token: 0x04000033 RID: 51
		private string targetPartition;

		// Token: 0x04000034 RID: 52
		private XElement fipInfo;

		// Token: 0x04000035 RID: 53
		private XElement phoneSecurityInfo;
	}
}
