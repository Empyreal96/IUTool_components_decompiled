using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.WindowsPhone.ImageUpdate.Tools;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgCommon
{
	// Token: 0x02000011 RID: 17
	internal class PkgBuilder : WPPackageBase, IPkgBuilder, IDisposable
	{
		// Token: 0x06000084 RID: 132 RVA: 0x00004230 File Offset: 0x00002430
		private static bool IsGeneratedFileType(FileType type)
		{
			return type == FileType.Manifest || type == FileType.Catalog;
		}

		// Token: 0x06000085 RID: 133 RVA: 0x0000423D File Offset: 0x0000243D
		private static bool IsGeneratedFileType(FileType type, PackageStyle style, string name)
		{
			if (style == PackageStyle.SPKG)
			{
				return PkgBuilder.IsGeneratedFileType(type);
			}
			return name.Equals(PkgConstants.c_strCBSCatalogFile, StringComparison.OrdinalIgnoreCase) || name.Equals(PkgConstants.c_strMumFile, StringComparison.OrdinalIgnoreCase);
		}

		// Token: 0x06000086 RID: 134 RVA: 0x00004266 File Offset: 0x00002466
		private string GetTempFile()
		{
			if (this._tmpDir == null)
			{
				this._tmpDir = FileUtils.GetTempDirectory();
			}
			return FileUtils.GetTempFile(this._tmpDir);
		}

		// Token: 0x06000087 RID: 135 RVA: 0x00004288 File Offset: 0x00002488
		private void ValidateRegistry()
		{
			string tempDirectory = FileUtils.GetTempDirectory();
			try
			{
				List<string> list = new List<string>();
				foreach (FileEntry fileEntry in from x in this._allFiles
				where x.FileType == FileType.Registry
				select x)
				{
					string text = Path.Combine(tempDirectory, Path.GetFileName(fileEntry.DevicePath));
					LongPathFile.Copy(fileEntry.SourcePath, text);
					list.Add(text);
				}
				List<string> list2 = new List<string>();
				foreach (FileEntry fileEntry2 in from x in this._allFiles
				where x.FileType == FileType.RegistryMultiStringAppend
				select x)
				{
					string text2 = Path.Combine(tempDirectory, Path.GetFileName(fileEntry2.DevicePath));
					LongPathFile.Copy(fileEntry2.SourcePath, text2);
					list2.Add(text2);
				}
				RegValidator.Validate(list, list2);
			}
			catch (IUException innerException)
			{
				throw new PackageException(innerException, "Registry validation failed for package '{0}'", new object[]
				{
					base.Name
				});
			}
			finally
			{
				FileUtils.DeleteTree(tempDirectory);
			}
		}

		// Token: 0x06000088 RID: 136 RVA: 0x00004428 File Offset: 0x00002628
		private void BuildCabPaths()
		{
			for (int i = 0; i < this._allFiles.Count; i++)
			{
				if (this._allFiles[i].CabPath == null)
				{
					this._allFiles[i].CabPath = PackageTools.MakeShortPath(this._allFiles[i].DevicePath, i.ToString());
				}
			}
		}

		// Token: 0x06000089 RID: 137 RVA: 0x0000448C File Offset: 0x0000268C
		private void SaveManifest()
		{
			this.m_pkgManifest.Files = this._allFiles.ToArray();
			if (base.ReleaseType == ReleaseType.Production)
			{
				PackageTools.CheckCrossPartitionFiles(base.Name, base.Partition, from x in this._allFiles
				select x.DevicePath);
			}
			this.m_pkgManifest.Save(this._manifestEntry.SourcePath);
		}

		// Token: 0x0600008A RID: 138 RVA: 0x0000450C File Offset: 0x0000270C
		private void Cleanup()
		{
			if (this._tmpDir != null)
			{
				try
				{
					FileUtils.DeleteTree(this._tmpDir);
				}
				catch (IOException)
				{
				}
				catch (UnauthorizedAccessException)
				{
				}
				this._tmpDir = null;
			}
		}

		// Token: 0x0600008B RID: 139 RVA: 0x00004558 File Offset: 0x00002758
		public void AddFile(IFileEntry file, string source, string embedSignCategory = "None")
		{
			this.AddFile(file.FileType, source, file.DevicePath, file.Attributes, file.SourcePackage, (file.CabPath == null) ? "" : file.CabPath, embedSignCategory);
		}

		// Token: 0x0600008C RID: 140 RVA: 0x00004590 File Offset: 0x00002790
		public void AddFile(FileType type, string source, string destination, FileAttributes attributes, string sourcePackage, string embedSignCategory = "None")
		{
			if (!LongPathFile.Exists(source))
			{
				throw new PackageException("File {0} doesn't exist", new object[]
				{
					source
				});
			}
			if (string.IsNullOrEmpty(destination))
			{
				throw new PackageException("DevicePath can't be empty");
			}
			if (destination.Contains("\\.\\") || destination.Contains("\\..\\") || destination.Contains("\\\\"))
			{
				throw new PackageException("DevicePath can't contain  \\.\\ or \\..\\ or \\\\");
			}
			if (destination.Length > PkgConstants.c_iMaxDevicePath)
			{
				throw new PackageException("DevicePath can't exceed {0} characters", new object[]
				{
					PkgConstants.c_iMaxDevicePath
				});
			}
			if (!Path.IsPathRooted(destination))
			{
				throw new PackageException("Only absolute path is allowed in DevicePath: '{0}'", new object[]
				{
					destination
				});
			}
			if (!string.IsNullOrEmpty(sourcePackage) && sourcePackage.Length > PkgConstants.c_iMaxPackageName)
			{
				throw new PackageException("SourcePackage can't be longer than {0} characters", new object[]
				{
					PkgConstants.c_iMaxPackageName
				});
			}
			if (base.IsBinaryPartition)
			{
				throw new PackageException("File with device path '{0}' can not be added since the package contains a BinaryPartition file ", new object[]
				{
					destination
				});
			}
			if (type == FileType.Regular)
			{
				string text = Array.Find<string>(PkgConstants.c_strSpecialFolders, (string x) => destination.StartsWith(x, StringComparison.InvariantCultureIgnoreCase));
				if (text != null)
				{
					throw new PackageException("Failed to add file (Type: '{0}', SourcePath: '{1}', DevicePath: '{2}'): regular files are not allowed in folder '{3}' on device", new object[]
					{
						type,
						source,
						destination,
						text
					});
				}
			}
			else if ((type == FileType.Registry || type == FileType.RegistryMultiStringAppend) && !PkgConstants.c_strHivePartitions.Contains(base.Partition, StringComparer.InvariantCultureIgnoreCase))
			{
				throw new PackageException("Failed to add registry file '{0}' to partition '{1}': only the listed partitions can have registry hives: {2}", new object[]
				{
					destination,
					base.Partition,
					string.Join(",", PkgConstants.c_strHivePartitions)
				});
			}
			FileAttributes fileAttributes = attributes & ~PkgConstants.c_validAttributes;
			if (fileAttributes != (FileAttributes)0)
			{
				throw new PackageException("Failed to add file (Type: '{0}', SourcePath: '{1}', DevicePath: '{2}'): invalid attributes '{3}' specified", new object[]
				{
					type,
					source,
					destination,
					fileAttributes
				});
			}
			FileEntry fileEntry = this._allFiles.Find((FileEntry x) => string.Compare(x.DevicePath, destination, StringComparison.OrdinalIgnoreCase) == 0);
			if (fileEntry != null)
			{
				if (!source.Equals(fileEntry.SourcePath, StringComparison.InvariantCultureIgnoreCase) && !File.ReadAllBytes(source).SequenceEqual(File.ReadAllBytes(fileEntry.SourcePath)))
				{
					throw new PackageException("Failed to add file (Type: '{0}', SourcePath: '{1}', DevicePath: '{2}'): there is already a '{3}' file with same DevicePath ('{4}') in the package. However, their source paths point to different files.", new object[]
					{
						type,
						source,
						destination,
						fileEntry.SourcePath,
						fileEntry.FileType
					});
				}
				if (!embedSignCategory.Equals(fileEntry.EmbeddedSigningCategory, StringComparison.OrdinalIgnoreCase))
				{
					throw new PackageException("Failed to add file (Type: '{0}', SourcePath: '{1}', DevicePath: '{2}'): there is already a '{3}' file with same DevicePath ('{4}') in the package. However, their signing options are different.", new object[]
					{
						type,
						source,
						destination,
						fileEntry.SourcePath,
						fileEntry.FileType
					});
				}
				if (attributes != fileEntry.Attributes)
				{
					throw new PackageException("Failed to add file (Type: '{0}', SourcePath: '{1}', DevicePath: '{2}'): there is already a '{3}' file with same DevicePath ('{4}') in the package. However, their attributes are different.", new object[]
					{
						type,
						source,
						destination,
						fileEntry.SourcePath,
						fileEntry.FileType
					});
				}
				LogUtil.Warning("Ignoring addition of duplicate file '{0}'. Please make sure that this is expected.", new object[]
				{
					destination
				});
			}
			if (fileEntry == null)
			{
				FileEntry item = new FileEntry(type, destination, attributes, source, sourcePackage, embedSignCategory);
				this._allFiles.Add(item);
			}
		}

		// Token: 0x0600008D RID: 141 RVA: 0x00004900 File Offset: 0x00002B00
		public void AddFile(FileType type, string source, string destination, FileAttributes attributes, string sourcePackage, string cabPath, string embedSignCategory)
		{
			if (!LongPathFile.Exists(source))
			{
				throw new PackageException("File {0} doesn't exist", new object[]
				{
					source
				});
			}
			if (string.IsNullOrEmpty(destination))
			{
				throw new PackageException("DevicePath can't be empty");
			}
			if (destination.Contains("\\.\\") || destination.Contains("\\..\\") || destination.Contains("\\\\"))
			{
				throw new PackageException("DevicePath can't contain  \\.\\ or \\..\\ or \\\\");
			}
			if (destination.Length > PkgConstants.c_iMaxDevicePath)
			{
				throw new PackageException("DevicePath can't exceed {0} characters", new object[]
				{
					PkgConstants.c_iMaxDevicePath
				});
			}
			if (!Path.IsPathRooted(destination))
			{
				throw new PackageException("Only absolute path is allowed in DevicePath: '{0}'", new object[]
				{
					destination
				});
			}
			if (!string.IsNullOrEmpty(sourcePackage) && sourcePackage.Length > PkgConstants.c_iMaxPackageName)
			{
				throw new PackageException("SourcePackage can't be longer than {0} characters", new object[]
				{
					PkgConstants.c_iMaxPackageName
				});
			}
			if (base.IsBinaryPartition)
			{
				throw new PackageException("File with device path '{0}' can not be added since the package contains a BinaryPartition file ", new object[]
				{
					destination
				});
			}
			if (type == FileType.Regular)
			{
				string text = Array.Find<string>(PkgConstants.c_strSpecialFolders, (string x) => destination.StartsWith(x, StringComparison.InvariantCultureIgnoreCase));
				if (text != null)
				{
					throw new PackageException("Failed to add file (Type: '{0}', SourcePath: '{1}', DevicePath: '{2}'): regular files are not allowed in folder '{3}' on device", new object[]
					{
						type,
						source,
						destination,
						text
					});
				}
			}
			else if ((type == FileType.Registry || type == FileType.RegistryMultiStringAppend) && !PkgConstants.c_strHivePartitions.Contains(base.Partition, StringComparer.InvariantCultureIgnoreCase))
			{
				throw new PackageException("Failed to add registry file '{0}' to partition '{1}': only the listed partitions can have registry hives: {2}", new object[]
				{
					destination,
					base.Partition,
					string.Join(",", PkgConstants.c_strHivePartitions)
				});
			}
			FileAttributes fileAttributes = attributes & ~PkgConstants.c_validAttributes;
			if (fileAttributes != (FileAttributes)0)
			{
				throw new PackageException("Failed to add file (Type: '{0}', SourcePath: '{1}', DevicePath: '{2}'): invalid attributes '{3}' specified", new object[]
				{
					type,
					source,
					destination,
					fileAttributes
				});
			}
			FileEntry fileEntry = this._allFiles.Find((FileEntry x) => string.Compare(x.DevicePath, destination, StringComparison.OrdinalIgnoreCase) == 0);
			if (fileEntry != null)
			{
				if (!source.Equals(fileEntry.SourcePath, StringComparison.InvariantCultureIgnoreCase) && !File.ReadAllBytes(source).SequenceEqual(File.ReadAllBytes(fileEntry.SourcePath)))
				{
					throw new PackageException("Failed to add file (Type: '{0}', SourcePath: '{1}', DevicePath: '{2}'): there is already a '{3}' file with same DevicePath ('{4}') in the package. However, their source paths point to different files.", new object[]
					{
						type,
						source,
						destination,
						fileEntry.SourcePath,
						fileEntry.FileType
					});
				}
				if (!embedSignCategory.Equals(fileEntry.EmbeddedSigningCategory))
				{
					throw new PackageException("Failed to add file (Type: '{0}', SourcePath: '{1}', DevicePath: '{2}'): there is already a '{3}' file with same DevicePath ('{4}') in the package. However, their signing options are different.", new object[]
					{
						type,
						source,
						destination,
						fileEntry.SourcePath,
						fileEntry.FileType
					});
				}
				if (attributes != fileEntry.Attributes)
				{
					throw new PackageException("Failed to add file (Type: '{0}', SourcePath: '{1}', DevicePath: '{2}'): there is already a '{3}' file with same DevicePath ('{4}') in the package. However, their attributes are different.", new object[]
					{
						type,
						source,
						destination,
						fileEntry.SourcePath,
						fileEntry.FileType
					});
				}
				LogUtil.Warning("Ignoring addition of duplicate file '{0}'. Please make sure that this is expected.", new object[]
				{
					destination
				});
			}
			if (fileEntry == null)
			{
				FileEntry fileEntry2 = new FileEntry(type, destination, attributes, source, sourcePackage, embedSignCategory);
				fileEntry2.CabPath = cabPath;
				this._allFiles.Add(fileEntry2);
			}
		}

		// Token: 0x0600008E RID: 142 RVA: 0x00004C78 File Offset: 0x00002E78
		public void RemoveFile(string destination)
		{
			FileEntry fileEntry = this._allFiles.Find((FileEntry x) => string.Compare(x.DevicePath, destination, StringComparison.OrdinalIgnoreCase) == 0);
			if (fileEntry == null)
			{
				throw new PackageException("Failed to remove file with device path '{0}': file is not in the package yet", new object[]
				{
					destination
				});
			}
			if (fileEntry.FileType == FileType.Manifest || fileEntry.FileType == FileType.Catalog)
			{
				throw new PackageException("Failed to remove file with device path '{0}': file wtih type '{0}' cannot be removed", new object[]
				{
					destination,
					fileEntry.FileType
				});
			}
			this._allFiles.Remove(fileEntry);
		}

		// Token: 0x0600008F RID: 143 RVA: 0x00004D0F File Offset: 0x00002F0F
		public void RemoveAllFiles()
		{
			this._allFiles.RemoveAll((FileEntry x) => !PkgBuilder.IsGeneratedFileType(x.FileType));
		}

		// Token: 0x06000090 RID: 144 RVA: 0x00004D3C File Offset: 0x00002F3C
		public new IFileEntry FindFile(string devicePath)
		{
			return this._allFiles.Find((FileEntry x) => string.Compare(x.DevicePath, devicePath, StringComparison.OrdinalIgnoreCase) == 0);
		}

		// Token: 0x06000091 RID: 145 RVA: 0x00004D6D File Offset: 0x00002F6D
		public void SetIsRemoval(bool isRemoval)
		{
			this.m_pkgManifest.IsRemoval = isRemoval;
		}

		// Token: 0x06000092 RID: 146 RVA: 0x00004D7B File Offset: 0x00002F7B
		public void SetPkgFileSigner(IPkgFileSigner signer)
		{
			this._signer = signer;
		}

		// Token: 0x06000093 RID: 147 RVA: 0x00004D84 File Offset: 0x00002F84
		public void SaveCab(string cabPath)
		{
			this.SaveCab(cabPath, true, PackageStyle.SPKG);
		}

		// Token: 0x06000094 RID: 148 RVA: 0x00004D8F File Offset: 0x00002F8F
		public void SaveCab(string cabPath, PackageStyle outputStyle)
		{
			this.SaveCab(cabPath, true, outputStyle);
		}

		// Token: 0x06000095 RID: 149 RVA: 0x00004D9A File Offset: 0x00002F9A
		public void SaveCab(string cabPath, bool compress)
		{
			this.SaveCab(cabPath, compress ? Package.DefaultCompressionType : CompressionType.None, PackageStyle.SPKG);
		}

		// Token: 0x06000096 RID: 150 RVA: 0x00004DAF File Offset: 0x00002FAF
		public void SaveCab(string cabPath, bool compress, PackageStyle outputStyle)
		{
			this.SaveCab(cabPath, compress ? Package.DefaultCompressionType : CompressionType.None, outputStyle);
		}

		// Token: 0x06000097 RID: 151 RVA: 0x00004DC4 File Offset: 0x00002FC4
		public void SaveCab(string cabPath, CompressionType compressionType)
		{
			this.SaveCab(cabPath, compressionType, PackageStyle.SPKG);
		}

		// Token: 0x06000098 RID: 152 RVA: 0x00004DCF File Offset: 0x00002FCF
		public void SaveCab(string cabPath, CompressionType compressionType, PackageStyle outputStyle)
		{
			this.SaveCab(cabPath, compressionType, outputStyle, PackageTools.SIGNING_HINT.SIGNING_HINT_TEST);
		}

		// Token: 0x06000099 RID: 153 RVA: 0x00004DDC File Offset: 0x00002FDC
		public void SaveCab(string cabPath, CompressionType compressionType, PackageStyle outputStyle, PackageTools.SIGNING_HINT signingHint)
		{
			if (outputStyle == PackageStyle.CBS)
			{
				this.SaveCBS(cabPath, compressionType);
				this._manifestEntry.DevicePath = Path.Combine("\\", PkgConstants.c_strMumFile);
				this._manifestEntry.CabPath = PkgConstants.c_strMumFile;
				this._manifestEntry.Attributes = (PkgConstants.c_defaultAttributes & ~FileAttributes.Compressed);
				this._catalogEntry.DevicePath = "\\Windows\\System32\\catroot\\{F750E6C3-38EE-11D1-85E5-00C04FC295EE}\\update.cat";
				this._catalogEntry.CabPath = PkgConstants.c_strCBSCatalogFile;
				this._catalogEntry.SourcePath = this.GetTempFile() + "_" + PkgConstants.c_strCatalogFile;
				this._allFiles.ForEach(delegate(FileEntry x)
				{
					x.CabPath = x.CabPath.TrimStart(new char[]
					{
						'\\'
					});
				});
			}
			else
			{
				this.ValidateRegistry();
				this._allFiles.ForEach(delegate(FileEntry x)
				{
					x.CabPath = null;
				});
				this._manifestEntry.DevicePath = Path.Combine(PkgConstants.c_strDsmDeviceFolder, base.Name + PkgConstants.c_strDsmExtension);
				this._manifestEntry.CabPath = PkgConstants.c_strDsmFile;
				this._manifestEntry.Attributes = (PkgConstants.c_defaultAttributes & ~FileAttributes.Compressed);
				if (this._manifestEntry.SourcePath == null)
				{
					this._manifestEntry.SourcePath = this.GetTempFile() + "_" + PkgConstants.c_strDsmFile;
				}
				this._catalogEntry.DevicePath = Path.Combine(PkgConstants.c_strCatalogDeviceFolder, base.Name + PkgConstants.c_strCatalogFileExtension);
				this._catalogEntry.CabPath = PkgConstants.c_strCatalogFile;
				if (this._catalogEntry.SourcePath == null)
				{
					this._catalogEntry.SourcePath = this.GetTempFile() + "_" + PkgConstants.c_strCatalogFile;
				}
				this.BuildCabPaths();
				this.SaveManifest();
			}
			IEnumerable<FileEntry> source = from x in this._allFiles
			where x.FileType != FileType.Catalog
			select x;
			string[] sourcePaths = (from x in source
			select x.SourcePath).ToArray<string>();
			string[] devicePaths = (from x in source
			select x.DevicePath).ToArray<string>();
			if (outputStyle == PackageStyle.SPKG)
			{
				this._catalogEntry.CalculateFileSizes();
				this._manifestEntry.CalculateFileSizes();
				this.SaveManifest();
			}
			List<string> list = new List<string>();
			foreach (FileEntry fileEntry in from x in this._allFiles
			where !x.EmbeddedSigningCategory.Equals("None", StringComparison.OrdinalIgnoreCase)
			select x)
			{
				string tempFile = this.GetTempFile();
				File.Copy(fileEntry.SourcePath, tempFile);
				list.Add(tempFile);
				try
				{
					this._signer.SignFileWithOptions(tempFile, fileEntry.EmbeddedSigningCategory);
				}
				catch (PackageException innerException)
				{
					throw new PackageException(innerException, "Unable to sign file {0} with options {1}", new object[]
					{
						fileEntry.SourcePath,
						fileEntry.EmbeddedSigningCategory
					});
				}
				fileEntry.SourcePath = tempFile;
			}
			try
			{
				PackageTools.CreateCatalog(sourcePaths, devicePaths, PackageTools.GetCatalogPackageName(base.ReleaseType, base.Name), this._signer, this._catalogEntry.SourcePath, signingHint);
			}
			catch
			{
				LogUtil.Error("Failed to create catalog file for package {0}", new object[]
				{
					base.Name
				});
				throw;
			}
			CabArchiver cab = new CabArchiver();
			this._allFiles.ForEach(delegate(FileEntry x)
			{
				cab.AddFile(x.CabPath, x.SourcePath);
			});
			cab.Save(cabPath, compressionType);
			try
			{
				if (signingHint == PackageTools.SIGNING_HINT.SIGNING_HINT_TEST)
				{
					LogUtil.Message("PkgCommonManaged: SaveCab: Signing cab path '{0}' with -testonly.", new object[]
					{
						cabPath
					});
					this._signer.SignFileWithOptions(cabPath, "-testonly");
				}
				else if (signingHint == PackageTools.SIGNING_HINT.SIGNING_HINT_BRANCH_POLICY)
				{
					LogUtil.Message("PkgCommonManaged: SaveCab: Signing cab path '{0}' according to branch policy.", new object[]
					{
						cabPath
					});
					this._signer.SignFile(cabPath);
				}
			}
			catch (Exception innerException2)
			{
				throw new PackageException(innerException2, "Failed to sign generated package: {0}", new object[]
				{
					cabPath
				});
			}
			finally
			{
				list.ForEach(delegate(string x)
				{
					File.Delete(x);
				});
			}
			GC.KeepAlive(this);
		}

		// Token: 0x0600009A RID: 154 RVA: 0x0000526C File Offset: 0x0000346C
		public void SaveCBS(string cabPath, CompressionType compressionType)
		{
			string value = string.IsNullOrEmpty(this.m_pkgManifest.Culture) ? "neutral" : this.m_pkgManifest.Culture;
			string value2 = string.IsNullOrEmpty(this.m_pkgManifest.PublicKey) ? PkgConstants.c_strCBSPublicKey : this.m_pkgManifest.PublicKey;
			XNamespace ns = "urn:schemas-microsoft-com:asm.v3";
			XElement xelement = new XElement(ns + "assembly", new object[]
			{
				new XAttribute("manifestVersion", "1.0"),
				new XAttribute("description", this.m_pkgManifest.Name),
				new XAttribute("displayName", this.m_pkgManifest.Name),
				new XAttribute("company", "Microsoft Corporation"),
				new XAttribute("copyright", "Microsoft Corporation")
			});
			XElement content = new XElement(ns + "assemblyIdentity", new object[]
			{
				new XAttribute("name", this.m_pkgManifest.Name + "-Package"),
				new XAttribute("version", this.m_pkgManifest.Version),
				new XAttribute("processorArchitecture", this.m_pkgManifest.CpuString()),
				new XAttribute("language", value),
				new XAttribute("buildType", this.m_pkgManifest.BuildType),
				new XAttribute("publicKeyToken", value2),
				new XAttribute("versionScope", "nonSxS")
			});
			XElement xelement2 = new XElement(ns + "package", new object[]
			{
				new XAttribute("identifier", this.m_pkgManifest.Name),
				new XAttribute("releaseType", "Feature Pack")
			});
			xelement.Add(content);
			XElement xelement3 = new XElement(ns + "customInformation");
			XElement content2 = new XElement(ns + "phoneInformation", new object[]
			{
				new XAttribute("phoneRelease", this.m_pkgManifest.ReleaseType),
				new XAttribute("phoneOwner", this.m_pkgManifest.Owner),
				new XAttribute("phoneOwnerType", this.m_pkgManifest.OwnerType),
				new XAttribute("phoneComponent", this.m_pkgManifest.Component),
				new XAttribute("phoneSubComponent", this.m_pkgManifest.SubComponent),
				new XAttribute("phoneGroupingKey", this.m_pkgManifest.GroupingKey)
			});
			xelement3.Add(content2);
			XElement content3 = new XElement(ns + "file", new object[]
			{
				new XAttribute("name", "\\" + PkgConstants.c_strMumFile),
				new XAttribute("size", 0),
				new XAttribute("staged", 0),
				new XAttribute("compressed", 0),
				new XAttribute("sourcePackage", this.m_pkgManifest.Name),
				new XAttribute("embeddedSign", ""),
				new XAttribute("keyform", ""),
				new XAttribute("cabpath", PkgConstants.c_strMumFile)
			});
			xelement3.Add(content3);
			content3 = new XElement(ns + "file", new object[]
			{
				new XAttribute("name", "\\" + PkgConstants.c_strCBSCatalogFile),
				new XAttribute("size", 0),
				new XAttribute("staged", 0),
				new XAttribute("compressed", 0),
				new XAttribute("sourcePackage", this.m_pkgManifest.Name),
				new XAttribute("embeddedSign", ""),
				new XAttribute("keyform", ""),
				new XAttribute("cabpath", PkgConstants.c_strCBSCatalogFile)
			});
			xelement3.Add(content3);
			foreach (FileEntry fileEntry in this._allFiles)
			{
				if (fileEntry.DevicePath != null)
				{
					if (fileEntry.FileType == FileType.Manifest)
					{
						if (Path.GetExtension(fileEntry.DevicePath) == PkgConstants.c_strMumExtension)
						{
							continue;
						}
						XDocument xdocument = XDocument.Load(fileEntry.SourcePath);
						if (xdocument.Root.Descendants(xdocument.Root.Name.Namespace + "deployment").Count<XElement>() != 0)
						{
							XElement xelement4 = xdocument.Root.Element(xdocument.Root.Name.Namespace + "assemblyIdentity");
							string text = xelement4.Attribute("name").Value;
							if (text.LastIndexOf("-Deployment", StringComparison.OrdinalIgnoreCase) != -1)
							{
								string text2 = text;
								text = text2.Remove(text2.LastIndexOf("-Deployment", StringComparison.OrdinalIgnoreCase));
							}
							else if (text.LastIndexOf(".Deployment", StringComparison.OrdinalIgnoreCase) != -1)
							{
								string text3 = text;
								text = text3.Remove(text3.LastIndexOf(".Deployment", StringComparison.OrdinalIgnoreCase));
							}
							XElement xelement5 = new XElement(ns + "update", new XAttribute("name", text));
							XElement xelement6 = new XElement(ns + "package", new object[]
							{
								new XAttribute("contained", "false"),
								new XAttribute("integrate", "hidden")
							});
							XElement content4 = new XElement(ns + "assemblyIdentity", new object[]
							{
								new XAttribute("buildType", xelement4.Attribute("buildType").Value),
								new XAttribute("name", text + "-Package"),
								new XAttribute("version", xelement4.Attribute("version").Value),
								new XAttribute("language", xelement4.Attribute("language").Value),
								new XAttribute("processorArchitecture", xelement4.Attribute("processorArchitecture").Value),
								new XAttribute("publicKeyToken", xelement4.Attribute("publicKeyToken").Value)
							});
							xelement6.Add(content4);
							xelement5.Add(xelement6);
							xelement2.Add(xelement5);
						}
					}
					else if (fileEntry.FileType == FileType.Catalog && Path.GetFileName(fileEntry.DevicePath).Equals("update.cat", StringComparison.OrdinalIgnoreCase))
					{
						continue;
					}
					content3 = new XElement(ns + "file", new object[]
					{
						new XAttribute("name", fileEntry.DevicePath),
						new XAttribute("size", fileEntry.Size),
						new XAttribute("staged", fileEntry.StagedSize),
						new XAttribute("compressed", fileEntry.CompressedSize),
						new XAttribute("sourcePackage", fileEntry.SourcePackage),
						new XAttribute("embeddedSign", fileEntry.EmbeddedSigningCategory),
						new XAttribute("keyform", Path.GetDirectoryName(fileEntry.CabPath).TrimStart(new char[]
						{
							'\\'
						})),
						new XAttribute("cabpath", fileEntry.CabPath.TrimStart(new char[]
						{
							'\\'
						}))
					});
					xelement3.Add(content3);
				}
			}
			xelement2.AddFirst(xelement3);
			xelement.Add(xelement2);
			XDocument xdocument2 = new XDocument(new object[]
			{
				xelement
			});
			string text4 = this.GetTempFile() + "_" + PkgConstants.c_strDsmFile;
			xdocument2.Save(text4);
			this._manifestEntry.SourcePath = text4;
		}

		// Token: 0x0600009B RID: 155 RVA: 0x00005BC0 File Offset: 0x00003DC0
		public void SaveCBSR(string cabPath, CompressionType compressionType)
		{
			if (this.m_pkgManifest.IsBinaryPartition)
			{
				throw new PackageException("Cannot remove binary partitions. Tried to remove: " + this.m_pkgManifest.Partition);
			}
			string value = string.IsNullOrEmpty(this.m_pkgManifest.PublicKey) ? PkgConstants.c_strCBSPublicKey : this.m_pkgManifest.PublicKey;
			this._manifestEntry.DevicePath = Path.Combine(PkgConstants.c_strDsmDeviceFolder, base.Name + PkgConstants.c_strMumExtension);
			this._manifestEntry.CabPath = PkgConstants.c_strMumFile;
			this._manifestEntry.Attributes = (PkgConstants.c_defaultAttributes & ~FileAttributes.Compressed);
			this._allFiles.Clear();
			this._allFiles.Add(this._manifestEntry);
			if (this._manifestEntry.SourcePath == null)
			{
				this._manifestEntry.SourcePath = this.GetTempFile() + "_" + PkgConstants.c_strDsmFile;
			}
			this._catalogEntry.DevicePath = Path.Combine(PkgConstants.c_strCatalogDeviceFolder, base.Name + PkgConstants.c_strCatalogFileExtension);
			this._catalogEntry.CabPath = PkgConstants.c_strCBSCatalogFile;
			if (this._catalogEntry.SourcePath == null)
			{
				this._catalogEntry.SourcePath = this.GetTempFile() + "_" + PkgConstants.c_strCBSCatalogFile;
			}
			string value2 = this.m_pkgManifest.Culture;
			if (string.IsNullOrEmpty(value2))
			{
				value2 = "neutral";
			}
			VersionInfo version = this.m_pkgManifest.Version;
			XNamespace ns = "urn:schemas-microsoft-com:cbs";
			XElement xelement = new XElement(ns + "assembly", new object[]
			{
				new XAttribute("manifestVersion", "1.0"),
				new XAttribute("description", this.m_pkgManifest.Name + ".Recall"),
				new XAttribute("displayName", this.m_pkgManifest.Name + " Recall"),
				new XAttribute("company", "Microsoft Corporation"),
				new XAttribute("copyright", "Microsoft Corporation")
			});
			XElement content = new XElement(ns + "assemblyIdentity", new object[]
			{
				new XAttribute("name", this.m_pkgManifest.Name + ".Recall"),
				new XAttribute("version", version),
				new XAttribute("language", value2),
				new XAttribute("processorArchitecture", this.m_pkgManifest.CpuString()),
				new XAttribute("buildType", "release"),
				new XAttribute("publicKeyToken", value)
			});
			XElement xelement2 = new XElement(ns + "package", new object[]
			{
				new XAttribute("identifier", this.m_pkgManifest.Name),
				new XAttribute("releaseType", "Hotfix"),
				new XAttribute("restart", "possible"),
				new XAttribute("targetPartition", this.m_pkgManifest.Partition),
				new XAttribute("binaryPartition", "false")
			});
			XElement xelement3 = new XElement(ns + "recall");
			XElement content2 = new XElement(ns + "assemblyIdentity", new object[]
			{
				new XAttribute("name", this.m_pkgManifest.Name),
				new XAttribute("version", version),
				new XAttribute("language", value2),
				new XAttribute("processorArchitecture", this.m_pkgManifest.CpuString()),
				new XAttribute("publicKeyToken", value)
			});
			XElement xelement4 = new XElement(ns + "customInformation");
			XElement content3 = new XElement(ns + "phoneInformation", new object[]
			{
				new XAttribute("phoneRelease", this.m_pkgManifest.ReleaseType),
				new XAttribute("phoneOwner", this.m_pkgManifest.Owner),
				new XAttribute("phoneOwnerType", this.m_pkgManifest.OwnerType),
				new XAttribute("phoneComponent", this.m_pkgManifest.Component),
				new XAttribute("phoneSubComponent", this.m_pkgManifest.SubComponent),
				new XAttribute("phoneGroupingKey", this.m_pkgManifest.GroupingKey)
			});
			xelement4.AddFirst(new XElement(ns + "file", new object[]
			{
				new XAttribute("name", "\\" + PkgConstants.c_strMumFile),
				new XAttribute("size", 0),
				new XAttribute("staged", 0),
				new XAttribute("compressed", 0),
				new XAttribute("sourcePackage", this.m_pkgManifest.Name),
				new XAttribute("embeddedSign", "none"),
				new XAttribute("cabPath", PkgConstants.c_strMumFile)
			}));
			xelement4.AddFirst(new XElement(ns + "file", new object[]
			{
				new XAttribute("name", "\\Windows\\System32\\catroot\\{F750E6C3-38EE-11D1-85E5-00C04FC295EE}\\update.cat"),
				new XAttribute("size", 0),
				new XAttribute("staged", 0),
				new XAttribute("compressed", 0),
				new XAttribute("sourcePackage", this.m_pkgManifest.Name),
				new XAttribute("embeddedSign", "none"),
				new XAttribute("cabPath", PkgConstants.c_strCBSCatalogFile)
			}));
			xelement4.AddFirst(content3);
			xelement3.Add(content2);
			xelement2.Add(xelement4);
			xelement2.Add(xelement3);
			xelement.Add(content);
			xelement.Add(xelement2);
			XDocument xdocument = new XDocument(new object[]
			{
				xelement
			});
			this.SaveManifest();
			xdocument.Save(this._manifestEntry.SourcePath, SaveOptions.None);
			FileEntry[] source = new FileEntry[]
			{
				this._manifestEntry
			};
			string[] sourcePaths = (from x in source
			select x.SourcePath).ToArray<string>();
			string[] devicePaths = (from x in source
			select x.DevicePath).ToArray<string>();
			try
			{
				PackageTools.CreateCatalog(sourcePaths, devicePaths, PackageTools.GetCatalogPackageName(base.ReleaseType, base.Name), this._catalogEntry.SourcePath);
			}
			catch
			{
				LogUtil.Error("Failed to create catalog file for package {0}", new object[]
				{
					base.Name
				});
				throw;
			}
			this._catalogEntry.CalculateFileSizes();
			this._manifestEntry.CalculateFileSizes();
			this.SaveManifest();
			xdocument.Save(this._manifestEntry.SourcePath, SaveOptions.None);
			try
			{
				PackageTools.CreateCatalog(sourcePaths, devicePaths, PackageTools.GetCatalogPackageName(base.ReleaseType, base.Name), this._catalogEntry.SourcePath);
			}
			catch
			{
				LogUtil.Error("Failed to create catalog file for package {0}", new object[]
				{
					base.Name
				});
				throw;
			}
			CabArchiver cabArchiver = new CabArchiver();
			cabArchiver.AddFile(this._manifestEntry.CabPath, this._manifestEntry.SourcePath);
			cabArchiver.AddFile(this._catalogEntry.CabPath, this._catalogEntry.SourcePath);
			cabArchiver.Save(cabPath, compressionType);
			try
			{
				this._signer.SignFile(cabPath);
			}
			catch (Exception innerException)
			{
				throw new PackageException(innerException, "Failed to sign generated package: {0}", new object[]
				{
					cabPath
				});
			}
		}

		// Token: 0x0600009C RID: 156 RVA: 0x0000645C File Offset: 0x0000465C
		~PkgBuilder()
		{
			this.Cleanup();
		}

		// Token: 0x0600009D RID: 157 RVA: 0x00006488 File Offset: 0x00004688
		public void Dispose()
		{
			this.Cleanup();
			GC.SuppressFinalize(this);
		}

		// Token: 0x0600009E RID: 158 RVA: 0x00006496 File Offset: 0x00004696
		internal PkgBuilder() : this(new PkgManifest())
		{
		}

		// Token: 0x0600009F RID: 159 RVA: 0x000064A3 File Offset: 0x000046A3
		internal PkgBuilder(PkgManifest pkgManifest) : this(pkgManifest, null)
		{
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x000064B0 File Offset: 0x000046B0
		internal PkgBuilder(PkgManifest pkgManifest, string tmpDir) : base(pkgManifest)
		{
			this._tmpDir = tmpDir;
			this._allFiles.Insert(0, this._manifestEntry);
			this._allFiles.Insert(0, this._catalogEntry);
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x0000652C File Offset: 0x0000472C
		internal static PkgBuilder Load(string cabPath)
		{
			string tempDirectory = FileUtils.GetTempDirectory();
			WPCanonicalPackage wpcanonicalPackage = WPCanonicalPackage.ExtractAndLoad(cabPath, tempDirectory);
			PkgBuilder pkgBuilder = new PkgBuilder(wpcanonicalPackage.Manifest, tempDirectory);
			pkgBuilder._allFiles.AddRange((from x in wpcanonicalPackage.Files
			where !PkgBuilder.IsGeneratedFileType(x.FileType, pkgBuilder.PackageStyle, Path.GetFileName(x.CabPath))
			select x).Cast<FileEntry>());
			return pkgBuilder;
		}

		// Token: 0x04000015 RID: 21
		private string _tmpDir;

		// Token: 0x04000016 RID: 22
		private FileEntry _manifestEntry = new FileEntry
		{
			FileType = FileType.Manifest
		};

		// Token: 0x04000017 RID: 23
		private FileEntry _catalogEntry = new FileEntry
		{
			FileType = FileType.Catalog
		};

		// Token: 0x04000018 RID: 24
		private List<FileEntry> _allFiles = new List<FileEntry>();

		// Token: 0x04000019 RID: 25
		private IPkgFileSigner _signer = new PkgFileSignerDefault();
	}
}
