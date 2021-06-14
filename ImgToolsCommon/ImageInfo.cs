using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.WindowsPhone.ImageUpdate.PkgCommon;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;
using Microsoft.WindowsPhone.Imaging;

namespace Microsoft.WindowsPhone.ImageUpdate.Tools.Images
{
	// Token: 0x02000003 RID: 3
	public class ImageInfo
	{
		// Token: 0x0600000B RID: 11 RVA: 0x000027FC File Offset: 0x000009FC
		public ImageInfo(string tempFileDirectory)
		{
			this.SetTempDirectoryPath(tempFileDirectory);
		}

		// Token: 0x0600000C RID: 12 RVA: 0x0000282C File Offset: 0x00000A2C
		public ImageInfo(WPImage wpImage, string tempFileDirectory)
		{
			this._wpImage = wpImage;
			this.SetTempDirectoryPath(tempFileDirectory);
		}

		// Token: 0x0600000D RID: 13 RVA: 0x00002864 File Offset: 0x00000A64
		public ImageInfo(string fileName, string tempFileDirectory)
		{
			if (tempFileDirectory != null)
			{
				this.SetTempDirectoryPath(tempFileDirectory);
			}
			else
			{
				this.SetTempDirectoryPath(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()));
				this._cleanUpTempDir = true;
			}
			this._logger = new IULogger();
			this._imageFile = fileName;
			try
			{
				this._wpImage = new WPImage(this._logger);
				this._cleanUpWPImage = true;
				this._wpImage.LoadImage(fileName);
			}
			catch (ImagesException)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw new ImagesException("Tools.ImgCommon!ImageInfo: Failed to load image file '" + fileName + "' : " + ex.Message, ex);
			}
		}

		// Token: 0x0600000E RID: 14 RVA: 0x00002944 File Offset: 0x00000B44
		public void Dispose()
		{
			if (this._wpImage != null)
			{
				if (this._cleanUpWPImage)
				{
					this._wpImage.Dispose();
				}
				this._wpImage = null;
			}
			if (this._cleanUpTempDir)
			{
				FileUtils.DeleteTree(this._tempDirectoryPath);
				this._tempDirectoryPath = string.Empty;
			}
		}

		// Token: 0x0600000F RID: 15 RVA: 0x00002991 File Offset: 0x00000B91
		private void SetTempDirectoryPath(string tempFileDirectory)
		{
			this._tempDirectoryPath = tempFileDirectory;
			if (!Directory.Exists(this._tempDirectoryPath))
			{
				Directory.CreateDirectory(this._tempDirectoryPath);
			}
			this._tempDirectoryPath = FileUtils.GetShortPathName(this._tempDirectoryPath);
		}

		// Token: 0x06000010 RID: 16 RVA: 0x000029C4 File Offset: 0x00000BC4
		public string DisplayFileName()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("****************************************************************************");
			stringBuilder.AppendLine("****************************************************************************");
			stringBuilder.AppendLine("File: " + this._imageFile);
			stringBuilder.AppendLine("****************************************************************************");
			stringBuilder.AppendLine("****************************************************************************");
			stringBuilder.AppendLine();
			return stringBuilder.ToString();
		}

		// Token: 0x06000011 RID: 17 RVA: 0x00002A2C File Offset: 0x00000C2C
		public string DisplayMetadata()
		{
			StringBuilder stringBuilder = new StringBuilder();
			new char[1];
			if (this._wpImage == null)
			{
				return "";
			}
			stringBuilder.AppendLine("[Metadata]");
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("Device Platform IDs:");
			foreach (string arg in this._wpImage.DevicePlatformIDs)
			{
				stringBuilder.AppendFormat("\tID: {0}", arg);
				stringBuilder.AppendLine();
			}
			if (!this._wpImage.IsFFU)
			{
				return stringBuilder.ToString();
			}
			if (this._wpImage.Metadata == null)
			{
				return stringBuilder.ToString();
			}
			stringBuilder.AppendLine("[Secure Header]");
			stringBuilder.AppendFormat("\tcbSize: " + this._dwordFormatString + "  bytes", this._wpImage.Metadata.GetSecureHeader.ByteCount, this._wpImage.Metadata.GetSecureHeader.ByteCount);
			stringBuilder.AppendLine();
			stringBuilder.AppendFormat("\tSignature: {0}", this._enc.GetString(FullFlashUpdateHeaders.GetSecuritySignature()));
			stringBuilder.AppendLine();
			stringBuilder.AppendFormat("\tChunk Size: " + this._dwordFormatString + " KB", this._wpImage.Metadata.GetSecureHeader.ChunkSize, this._wpImage.Metadata.GetSecureHeader.ChunkSize);
			stringBuilder.AppendLine();
			stringBuilder.AppendFormat("\tHash Algorithm ID: " + this._dwordFormatString, this._wpImage.Metadata.GetSecureHeader.HashAlgorithmID, this._wpImage.Metadata.GetSecureHeader.HashAlgorithmID);
			stringBuilder.AppendLine();
			stringBuilder.AppendFormat("\tCatalog Size: " + this._dwordFormatString + " bytes", this._wpImage.Metadata.GetSecureHeader.CatalogSize, this._wpImage.Metadata.GetSecureHeader.CatalogSize);
			stringBuilder.AppendLine();
			stringBuilder.AppendFormat("\tHash Table Size: " + this._dwordFormatString + " bytes", this._wpImage.Metadata.GetSecureHeader.HashTableSize, this._wpImage.Metadata.GetSecureHeader.HashTableSize);
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("[Catalog File]");
			string text = this._tempDirectoryPath + "\\WMImage.cat";
			File.WriteAllBytes(text, this._wpImage.Metadata.CatalogData);
			X509Certificate signingCert = this.GetSigningCert(text);
			if (signingCert == null)
			{
				stringBuilder.AppendLine("\tNot Signed");
			}
			else
			{
				stringBuilder.AppendLine("\tSigned with: ");
				stringBuilder.AppendFormat("\t\t\tSubject: {0}", signingCert.Subject);
				stringBuilder.AppendLine();
				stringBuilder.AppendFormat("\t\t\t\tIssuer: {0}", signingCert.Issuer);
				stringBuilder.AppendLine();
			}
			stringBuilder.AppendLine("[Image Header]");
			stringBuilder.AppendFormat("\tcbSize: " + this._dwordFormatString + "  bytes", this._wpImage.Metadata.GetImageHeader.ByteCount, this._wpImage.Metadata.GetImageHeader.ByteCount);
			stringBuilder.AppendLine();
			stringBuilder.AppendFormat("\tSignature: {0}", this._enc.GetString(FullFlashUpdateHeaders.GetImageSignature()));
			stringBuilder.AppendLine();
			stringBuilder.AppendFormat("\tManifest Length: " + this._dwordFormatString + " bytes", this._wpImage.Metadata.GetImageHeader.ManifestLength, this._wpImage.Metadata.GetImageHeader.ManifestLength);
			stringBuilder.AppendLine();
			stringBuilder.AppendFormat("\tChunk Size: " + this._dwordFormatString + " KB", this._wpImage.Metadata.GetImageHeader.ChunkSize, this._wpImage.Metadata.GetImageHeader.ChunkSize);
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("[Manifest]");
			stringBuilder.AppendLine();
			MemoryStream memoryStream = new MemoryStream();
			this._wpImage.Metadata.GetManifest.WriteToStream(memoryStream);
			string text2 = this._enc.GetString(memoryStream.GetBuffer());
			text2 = text2.Substring(0, (int)memoryStream.Length);
			stringBuilder.AppendLine(text2);
			return stringBuilder.ToString();
		}

		// Token: 0x06000012 RID: 18 RVA: 0x00002F3C File Offset: 0x0000113C
		public string DisplayStore()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (this._wpImage == null)
			{
				return "";
			}
			stringBuilder.AppendLine();
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("****************************************************************************");
			stringBuilder.AppendLine("****************************************************************************");
			stringBuilder.AppendLine("\t\t\tStore");
			stringBuilder.AppendLine("****************************************************************************");
			stringBuilder.AppendLine("****************************************************************************");
			stringBuilder.AppendLine("[Store]");
			stringBuilder.AppendLine();
			using (WPStore store = this._wpImage.Store)
			{
				int num = 0;
				using (List<WPPartition>.Enumerator enumerator = store.Partitions.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (!enumerator.Current.IsWim)
						{
							num++;
						}
					}
				}
				stringBuilder.AppendFormat("Sector Size: " + this._dwordFormatString + "  bytes", store.SectorSize, store.SectorSize);
				stringBuilder.AppendLine();
				stringBuilder.AppendFormat("Min Sector Count: " + this._dwordFormatString + "  sectors", store.MinSectorCount, store.MinSectorCount);
				stringBuilder.AppendLine();
				stringBuilder.AppendFormat("PartitionCount: {0}", num);
				stringBuilder.AppendLine();
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06000013 RID: 19 RVA: 0x000030BC File Offset: 0x000012BC
		public string DisplayPartitions()
		{
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			if (this._wpImage == null)
			{
				return "";
			}
			stringBuilder.AppendLine();
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("****************************************************************************");
			stringBuilder.AppendLine("****************************************************************************");
			stringBuilder.AppendLine("\t\t\tPartitions");
			stringBuilder.AppendLine("****************************************************************************");
			stringBuilder.AppendLine("****************************************************************************");
			stringBuilder.AppendLine("[Partitions]");
			stringBuilder.AppendLine();
			stringBuilder2.AppendLine();
			stringBuilder2.AppendLine();
			stringBuilder2.AppendLine("****************************************************************************");
			stringBuilder2.AppendLine("****************************************************************************");
			stringBuilder2.AppendLine("\t\t\tWIMs");
			stringBuilder2.AppendLine("****************************************************************************");
			stringBuilder2.AppendLine("****************************************************************************");
			stringBuilder2.AppendLine("[WIMs]");
			stringBuilder2.AppendLine();
			foreach (WPPartition wppartition in this._wpImage.Partitions)
			{
				if (wppartition.IsWim)
				{
					stringBuilder2.Append(this.DisplayWIM(wppartition));
				}
				else
				{
					stringBuilder.Append(this.DisplayPartition(wppartition));
				}
			}
			return stringBuilder.ToString() + stringBuilder2.ToString();
		}

		// Token: 0x06000014 RID: 20 RVA: 0x00003218 File Offset: 0x00001418
		public string DisplayPartition(WPPartition partition)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("\t[Partition]");
			stringBuilder.AppendFormat("\t\tName: {0}", partition.Name);
			stringBuilder.AppendLine();
			if (!string.IsNullOrEmpty(partition.FileSystem))
			{
				stringBuilder.AppendFormat("\t\tFile System: {0}", partition.FileSystem);
				stringBuilder.AppendLine();
			}
			stringBuilder.AppendFormat("\t\tType: {0}", partition.PartitionType);
			stringBuilder.AppendLine();
			stringBuilder.AppendFormat("\t\tTotal Sectors: " + this._dwordFormatString + " sectors", partition.TotalSectors, partition.TotalSectors);
			stringBuilder.AppendLine();
			stringBuilder.AppendFormat("\t\tUse All Space: {0}", partition.UseAllSpace.ToString());
			stringBuilder.AppendLine();
			stringBuilder.AppendFormat("\t\tSectors In Use: " + this._dwordFormatString + " sectors", partition.SectorsInUse, partition.SectorsInUse);
			stringBuilder.AppendLine();
			stringBuilder.AppendFormat("\t\tRequiredToFlash: {0}", partition.RequiredToFlash.ToString());
			stringBuilder.AppendLine();
			stringBuilder.AppendFormat("\t\tBootable: {0}", partition.Bootable.ToString());
			stringBuilder.AppendLine();
			stringBuilder.AppendFormat("\t\tReadOnly: {0}", partition.ReadOnly.ToString());
			stringBuilder.AppendLine();
			stringBuilder.AppendFormat("\t\tHidden: {0}", partition.Hidden.ToString());
			stringBuilder.AppendLine();
			stringBuilder.AppendFormat("\t\tPrimary Partition: {0}", partition.PrimaryPartition);
			stringBuilder.AppendLine();
			stringBuilder.AppendFormat("\t\tAttach Drive Letter: {0}", partition.AttachDriveLetter.ToString());
			stringBuilder.AppendLine();
			if (partition.ByteAlignment != 0U)
			{
				stringBuilder.AppendFormat("\t\tByte Alignment: " + this._dwordFormatString + " bytes", partition.ByteAlignment, partition.ByteAlignment);
				stringBuilder.AppendLine();
			}
			if (partition.SectorAlignment != 0U)
			{
				stringBuilder.AppendFormat("\t\tSector Alignment: " + this._dwordFormatString + " sectors", partition.SectorAlignment, partition.SectorAlignment);
				stringBuilder.AppendLine();
			}
			if (partition.InvalidPartition)
			{
				stringBuilder.AppendLine("\t\tPartition is inaccessible.");
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06000015 RID: 21 RVA: 0x0000347C File Offset: 0x0000167C
		public string DisplayWIM(WPPartition partition)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("\t[WIM]");
			stringBuilder.AppendFormat("\t\tFile Name: {0}", partition.WimFile);
			stringBuilder.AppendLine();
			if (!string.IsNullOrEmpty(partition.FileSystem))
			{
				stringBuilder.AppendFormat("\t\tFile System: {0}", partition.FileSystem);
				stringBuilder.AppendLine();
			}
			stringBuilder.AppendFormat("\t\tWim File Size: " + this._dwordFormatString + " bytes", partition.WimFileSize, partition.WimFileSize);
			stringBuilder.AppendLine();
			stringBuilder.AppendFormat("\t\tWim File Content Size: " + this._dwordFormatString + " bytes", partition.WimFileContentSize, partition.WimFileContentSize);
			stringBuilder.AppendLine();
			return stringBuilder.ToString();
		}

		// Token: 0x06000016 RID: 22 RVA: 0x00003554 File Offset: 0x00001754
		public string DisplayPackageList(WPPartition partition)
		{
			StringBuilder stringBuilder = new StringBuilder();
			SortedList<string, string> sortedList = new SortedList<string, string>();
			stringBuilder.AppendFormat("Packages for Partition {0}\n", partition.Name);
			foreach (IPkgInfo pkgInfo in partition.Packages)
			{
				string text = pkgInfo.Name.ToUpper(CultureInfo.InvariantCulture);
				SortedList<string, string> sortedList2 = sortedList;
				string text2 = text;
				sortedList2.Add(text2, text2);
			}
			foreach (string str in sortedList.Keys)
			{
				stringBuilder.AppendLine("\t" + str);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06000017 RID: 23 RVA: 0x0000362C File Offset: 0x0000182C
		public string DisplayRegistry()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (this._wpImage == null)
			{
				return "";
			}
			foreach (WPPartition partition in this._wpImage.Partitions)
			{
				stringBuilder.Append(this.DisplayRegistry(partition));
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06000018 RID: 24 RVA: 0x000036A8 File Offset: 0x000018A8
		public string DisplayRegistry(WPPartition partition)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (partition.IsBinaryPartition)
			{
				return "";
			}
			stringBuilder.AppendLine();
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("****************************************************************************");
			stringBuilder.AppendLine("****************************************************************************");
			stringBuilder.AppendLine(string.Concat(new string[]
			{
				"\t\t\tRegistry for ",
				partition.PartitionTypeLabel,
				" '",
				partition.Name,
				"'"
			}));
			stringBuilder.AppendLine("****************************************************************************");
			stringBuilder.AppendLine("****************************************************************************");
			if (partition.HasRegistryHive(SystemRegistryHiveFiles.DEFAULT))
			{
				this.DisplayPartitionRegistryHive(partition, SystemRegistryHiveFiles.DEFAULT, ref stringBuilder);
			}
			if (partition.HasRegistryHive(SystemRegistryHiveFiles.DRIVERS))
			{
				this.DisplayPartitionRegistryHive(partition, SystemRegistryHiveFiles.DRIVERS, ref stringBuilder);
			}
			if (partition.HasRegistryHive(SystemRegistryHiveFiles.SAM))
			{
				this.DisplayPartitionRegistryHive(partition, SystemRegistryHiveFiles.SAM, ref stringBuilder);
			}
			if (partition.HasRegistryHive(SystemRegistryHiveFiles.SECURITY))
			{
				this.DisplayPartitionRegistryHive(partition, SystemRegistryHiveFiles.SECURITY, ref stringBuilder);
			}
			if (partition.HasRegistryHive(SystemRegistryHiveFiles.SOFTWARE))
			{
				this.DisplayPartitionRegistryHive(partition, SystemRegistryHiveFiles.SOFTWARE, ref stringBuilder);
			}
			if (partition.HasRegistryHive(SystemRegistryHiveFiles.SYSTEM))
			{
				this.DisplayPartitionRegistryHive(partition, SystemRegistryHiveFiles.SYSTEM, ref stringBuilder);
			}
			if (partition.HasRegistryHive(SystemRegistryHiveFiles.BCD))
			{
				this.DisplayPartitionRegistryHive(partition, SystemRegistryHiveFiles.BCD, ref stringBuilder);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06000019 RID: 25 RVA: 0x000037D0 File Offset: 0x000019D0
		private void DisplayPartitionRegistryHive(WPPartition partition, SystemRegistryHiveFiles hiveType, ref StringBuilder outputStr)
		{
			string keyPrefix = RegistryUtils.MapHiveToMountPoint(hiveType);
			HiveToRegConverter hiveToRegConverter = new HiveToRegConverter(partition.GetRegistryHivePath(hiveType), keyPrefix);
			outputStr.AppendLine();
			outputStr.AppendFormat("[{2} {0}: {1} ]", partition.Name, partition.GetRegistryHiveDevicePath(hiveType), partition.PartitionTypeLabel);
			outputStr.AppendLine();
			hiveToRegConverter.ConvertToReg(ref outputStr);
		}

		// Token: 0x0600001A RID: 26 RVA: 0x00003828 File Offset: 0x00001A28
		public string DisplayPackages(bool fSummary)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (this._wpImage == null)
			{
				return "";
			}
			stringBuilder.AppendLine("[Packages]");
			stringBuilder.AppendLine();
			foreach (WPPartition partition in this._wpImage.Partitions)
			{
				stringBuilder.Append(this.DisplayPackages(partition, fSummary));
			}
			return stringBuilder.ToString();
		}

		// Token: 0x0600001B RID: 27 RVA: 0x000038B8 File Offset: 0x00001AB8
		public string DisplayDefaultCerts()
		{
			StringBuilder stringBuilder = new StringBuilder(null);
			foreach (WPPartition partition in this._wpImage.Partitions)
			{
				stringBuilder.AppendLine(this.DisplayDefaultCert(partition));
			}
			return stringBuilder.ToString();
		}

		// Token: 0x0600001C RID: 28 RVA: 0x00003924 File Offset: 0x00001B24
		public string DisplayDefaultCert(WPPartition partition)
		{
			StringBuilder stringBuilder = new StringBuilder(null);
			foreach (string text in Directory.EnumerateFiles(Path.Combine(partition.PartitionPath, PkgConstants.c_strDsmDeviceFolder), "*.dat"))
			{
				stringBuilder.AppendLine("Certs file " + text + " info for partition " + partition.Name);
				byte[] array = File.ReadAllBytes(text);
				if (array != null)
				{
					stringBuilder.AppendLine(this.DisplayDefaultCerts(array));
				}
				else
				{
					stringBuilder.AppendLine("\tUnable to extract '" + text + "' file.");
				}
				stringBuilder.AppendLine();
			}
			return stringBuilder.ToString();
		}

		// Token: 0x0600001D RID: 29 RVA: 0x000039E4 File Offset: 0x00001BE4
		public string DisplayPackages(WPPartition partition, bool fSummary)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine();
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("****************************************************************************");
			stringBuilder.AppendLine("****************************************************************************");
			stringBuilder.AppendLine(string.Concat(new string[]
			{
				"\t\t\tPackages for ",
				partition.PartitionTypeLabel,
				" '",
				partition.Name,
				"'"
			}));
			stringBuilder.AppendLine("****************************************************************************");
			stringBuilder.AppendLine("****************************************************************************");
			stringBuilder.AppendLine();
			stringBuilder.AppendFormat("[Packages Summary for " + partition.PartitionTypeLabel + " {0}]", partition.Name);
			stringBuilder.AppendLine();
			if (partition.Packages.Count == 0)
			{
				stringBuilder.AppendLine("None");
			}
			else
			{
				stringBuilder.AppendLine();
				stringBuilder.AppendFormat("{0,-25:G}\t", "Package Name");
				stringBuilder.AppendFormat("{0,-10:G}\t", "Version");
				stringBuilder.AppendFormat("{0,-15:G}\t", "Canonical\\Diff");
				stringBuilder.AppendFormat("{0,-20:G}\t", "Build");
				stringBuilder.AppendFormat("{0,-10:G}\t", "File Count");
				stringBuilder.AppendLine();
				stringBuilder.AppendLine("_________________________\t__________\t_______________\t____________________\t__________");
				foreach (IPkgInfo pkgInfo in partition.Packages)
				{
					if (!pkgInfo.IsBinaryPartition)
					{
						stringBuilder.AppendFormat("{0,-25:G}\t", pkgInfo.Name);
						stringBuilder.AppendFormat("{0,-10:G}\t", pkgInfo.Version);
						stringBuilder.AppendFormat("{0,-15:G}\t", pkgInfo.Type);
						stringBuilder.AppendFormat("{0,-20:G}\t", pkgInfo.BuildString);
						stringBuilder.AppendFormat("{0,-10:G}\t", pkgInfo.FileCount);
						stringBuilder.AppendLine();
					}
				}
				stringBuilder.AppendLine();
				stringBuilder.AppendLine("[Package Details]");
				stringBuilder.AppendLine();
				foreach (IPkgInfo pkgInfo2 in partition.Packages)
				{
					if (!pkgInfo2.IsBinaryPartition)
					{
						stringBuilder.AppendLine("\t[Package]");
						stringBuilder.AppendFormat("\t\tName: {0}", pkgInfo2.Name);
						stringBuilder.AppendLine();
						stringBuilder.AppendFormat("\t\tOwner: {0}", pkgInfo2.Owner);
						stringBuilder.AppendLine();
						stringBuilder.AppendFormat("\t\tComponent: {0}", pkgInfo2.Component);
						stringBuilder.AppendLine();
						if (!string.IsNullOrEmpty(pkgInfo2.SubComponent))
						{
							stringBuilder.AppendFormat("\t\tSubComponent: {0}", pkgInfo2.SubComponent);
							stringBuilder.AppendLine();
						}
						stringBuilder.AppendFormat("\t\t" + partition.PartitionTypeLabel + ": {0}", pkgInfo2.Partition);
						stringBuilder.AppendLine();
						stringBuilder.AppendFormat("\t\tVersion: ", new object[0]);
						stringBuilder.AppendLine();
						stringBuilder.AppendFormat("\t\t\tMajor: {0}", pkgInfo2.Version.Major);
						stringBuilder.AppendLine();
						stringBuilder.AppendFormat("\t\t\tMinor: {0}", pkgInfo2.Version.Minor);
						stringBuilder.AppendLine();
						stringBuilder.AppendFormat("\t\t\tQFE: {0}", pkgInfo2.Version.QFE);
						stringBuilder.AppendLine();
						stringBuilder.AppendFormat("\t\t\tBuild: {0}", pkgInfo2.Version.Build);
						stringBuilder.AppendLine();
						stringBuilder.AppendFormat("\t\tOwner Type: {0}", pkgInfo2.OwnerType);
						stringBuilder.AppendLine();
						stringBuilder.AppendFormat("\t\tRelease Type: {0}", pkgInfo2.ReleaseType);
						stringBuilder.AppendLine();
						stringBuilder.AppendFormat("\t\tBuild Type: {0}", pkgInfo2.BuildType);
						stringBuilder.AppendLine();
						stringBuilder.AppendFormat("\t\tCPU: {0}", pkgInfo2.CpuType);
						stringBuilder.AppendLine();
						if (!string.IsNullOrEmpty(pkgInfo2.Culture))
						{
							stringBuilder.AppendFormat("\t\tCulture: {0}", pkgInfo2.Culture);
							stringBuilder.AppendLine();
						}
						if (!string.IsNullOrEmpty(pkgInfo2.Resolution))
						{
							stringBuilder.AppendFormat("\t\tResolution: {0}", pkgInfo2.Resolution);
							stringBuilder.AppendLine();
						}
						stringBuilder.AppendFormat("\t\tPackage Type: {0}", pkgInfo2.Type);
						stringBuilder.AppendLine();
						stringBuilder.AppendFormat("\t\tBuild: {0}", pkgInfo2.BuildString);
						stringBuilder.AppendLine();
						stringBuilder.AppendFormat("\t\tGroup Key: {0}", pkgInfo2.GroupingKey);
						stringBuilder.AppendLine();
						stringBuilder.AppendFormat("\t\tFile Count: {0}", pkgInfo2.FileCount);
						stringBuilder.AppendLine();
						stringBuilder.AppendLine();
						stringBuilder.AppendLine();
						stringBuilder.AppendFormat("\t\t[File Summary for package {0}]", pkgInfo2.Name);
						stringBuilder.AppendLine();
						stringBuilder.AppendLine();
						stringBuilder.AppendFormat("\t\t{0,-35:G}\t", "Filename");
						stringBuilder.AppendFormat("{0,-15:G}\t", "Size (bytes)");
						stringBuilder.AppendFormat("{0,-25:G}\t", "Attributes");
						stringBuilder.AppendFormat("{0,-10:G}\t", "Type");
						stringBuilder.AppendFormat("{0,-35:G}\t", "Device Path");
						stringBuilder.AppendLine();
						stringBuilder.AppendLine("\t\t___________________________________\t_______________\t_________________________\t__________\t___________________________________");
						foreach (IFileEntry fileEntry in pkgInfo2.Files)
						{
							string fileName = Path.GetFileName(fileEntry.DevicePath);
							string arg = fileEntry.DevicePath.Replace(fileName, "", StringComparison.OrdinalIgnoreCase);
							string path = partition.PartitionPath + fileEntry.DevicePath;
							stringBuilder.AppendFormat("\t\t{0,-35:G}\t", fileName);
							if (!File.Exists(path))
							{
								stringBuilder.AppendFormat("{0,-15:G}\t", "File not found");
							}
							else
							{
								FileInfo fileInfo = new FileInfo(partition.PartitionPath + fileEntry.DevicePath);
								stringBuilder.AppendFormat("{0,-15:G}\t", fileInfo.Length);
							}
							stringBuilder.AppendFormat("{0,-25:G}\t", fileEntry.Attributes);
							stringBuilder.AppendFormat("{0,-10:G}\t", fileEntry.FileType);
							stringBuilder.AppendFormat("{0,-35:G}\t", arg);
							stringBuilder.AppendLine();
						}
						stringBuilder.AppendLine();
						if (!fSummary)
						{
							stringBuilder.AppendLine("\t\t[Files]");
							foreach (IFileEntry fileEntry2 in pkgInfo2.Files)
							{
								stringBuilder.AppendLine("\t\t\t[File]");
								string fileName = Path.GetFileName(fileEntry2.DevicePath);
								string arg = fileEntry2.DevicePath.Replace(fileName, "", StringComparison.OrdinalIgnoreCase);
								string text = partition.PartitionPath + fileEntry2.DevicePath;
								string empty = string.Empty;
								long num = 0L;
								string text2 = string.Empty;
								bool flag = false;
								if (File.Exists(text))
								{
									flag = true;
									FileInfo fileInfo2 = new FileInfo(text);
									FileVersionInfo.GetVersionInfo(text);
									num = fileInfo2.Length;
									text2 = fileInfo2.CreationTime.ToString();
								}
								stringBuilder.AppendFormat("\t\t\t\tName: {0} {1}", fileName, flag ? "" : "(File not Found)");
								stringBuilder.AppendLine();
								stringBuilder.AppendFormat("\t\t\t\tCreation Date\\Time: " + (flag ? text2 : "Unknown"), new object[0]);
								stringBuilder.AppendLine();
								stringBuilder.AppendFormat("\t\t\t\tVersion Number: " + (flag ? empty : "Unknown"), new object[0]);
								stringBuilder.AppendLine();
								if (flag)
								{
									stringBuilder.AppendFormat("\t\t\t\tSize: " + this._dwordFormatString + " bytes", num, num);
								}
								else
								{
									stringBuilder.AppendFormat("\t\t\t\tSize: Unknown", new object[0]);
								}
								stringBuilder.AppendLine();
								stringBuilder.AppendFormat("\t\t\t\tAttributes: {0}", fileEntry2.Attributes);
								stringBuilder.AppendLine();
								stringBuilder.AppendFormat("\t\t\t\tPath: {0}", arg);
								stringBuilder.AppendLine();
								stringBuilder.AppendFormat("\t\t\t\tPackage: {0}", pkgInfo2.Name);
								stringBuilder.AppendLine();
								stringBuilder.AppendFormat("\t\t\t\tFileType: {0}", fileEntry2.FileType);
								stringBuilder.AppendLine();
								stringBuilder.AppendFormat("\t\t\t\tSource Package: {0}", fileEntry2.SourcePackage);
								stringBuilder.AppendLine();
							}
						}
					}
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x0600001E RID: 30 RVA: 0x00004304 File Offset: 0x00002504
		private X509Certificate GetSigningCert(string fileName)
		{
			X509Certificate result = null;
			try
			{
				result = new X509Certificate2(fileName);
			}
			catch
			{
				return null;
			}
			return result;
		}

		// Token: 0x0600001F RID: 31 RVA: 0x00004334 File Offset: 0x00002534
		public string DisplayPackages(string packagesInfo)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("[Packages]");
			stringBuilder.AppendLine();
			stringBuilder.Append(packagesInfo);
			return stringBuilder.ToString();
		}

		// Token: 0x06000020 RID: 32 RVA: 0x0000435B File Offset: 0x0000255B
		public string DisplayPartitions(string partitionsInfo)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("[Partitions]");
			stringBuilder.AppendLine();
			stringBuilder.Append(partitionsInfo);
			return stringBuilder.ToString();
		}

		// Token: 0x06000021 RID: 33 RVA: 0x00004382 File Offset: 0x00002582
		public string DisplayWIMs(string wimsInfo)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("[WIMs]");
			stringBuilder.AppendLine();
			stringBuilder.Append(wimsInfo);
			return stringBuilder.ToString();
		}

		// Token: 0x06000022 RID: 34 RVA: 0x000043AC File Offset: 0x000025AC
		public string DisplayDefaultCerts(byte[] defaultCerts)
		{
			StringBuilder stringBuilder = new StringBuilder();
			try
			{
				if (defaultCerts != null && defaultCerts.Length != 0)
				{
					X509Certificate2 x509Certificate;
					for (int i = 0; i < defaultCerts.Length - 1; i += x509Certificate.RawData.Length)
					{
						byte[] array = new byte[defaultCerts.Length];
						for (int j = 0; j < defaultCerts.Length - i; j++)
						{
							array[j] = defaultCerts[i + j];
						}
						x509Certificate = new X509Certificate2(array);
						stringBuilder.AppendLine("\t[Cert]");
						stringBuilder.AppendLine("\t\tSubject: " + x509Certificate.Subject);
						stringBuilder.AppendLine("\t\tIssuer: " + x509Certificate.Issuer);
						stringBuilder.AppendLine("\t\tSerial Number: " + x509Certificate.SerialNumber);
						stringBuilder.AppendLine("\t\tSignature Algorithm: " + x509Certificate.SignatureAlgorithm.FriendlyName);
						stringBuilder.AppendLine("\t\tVersion: " + x509Certificate.Version);
					}
				}
				else
				{
					stringBuilder.AppendLine("Empty file");
				}
			}
			catch (Exception ex)
			{
				stringBuilder.AppendLine("Error parsing file: " + ex.Message);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x04000009 RID: 9
		private WPImage _wpImage;

		// Token: 0x0400000A RID: 10
		private IULogger _logger;

		// Token: 0x0400000B RID: 11
		private string _imageFile;

		// Token: 0x0400000C RID: 12
		private string _dwordFormatString = "{0} (0x{0:X})";

		// Token: 0x0400000D RID: 13
		private ASCIIEncoding _enc = new ASCIIEncoding();

		// Token: 0x0400000E RID: 14
		private string _tempDirectoryPath = string.Empty;

		// Token: 0x0400000F RID: 15
		private bool _cleanUpTempDir;

		// Token: 0x04000010 RID: 16
		private bool _cleanUpWPImage;
	}
}
