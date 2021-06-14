using System;
using System.IO;
using System.Text;
using Microsoft.WindowsPhone.ImageUpdate.PkgCommon;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.ImageUpdate.Tools.Images
{
	// Token: 0x02000004 RID: 4
	public class PackageInfo
	{
		// Token: 0x06000023 RID: 35 RVA: 0x000044E4 File Offset: 0x000026E4
		public PackageInfo(string tempFileDirectory)
		{
			this.SetTempDirectoryPath(tempFileDirectory);
		}

		// Token: 0x06000024 RID: 36 RVA: 0x00004509 File Offset: 0x00002709
		public PackageInfo(IPkgInfo package, string tempFileDirectory)
		{
			this._package = package;
			this.SetTempDirectoryPath(tempFileDirectory);
		}

		// Token: 0x06000025 RID: 37 RVA: 0x00004538 File Offset: 0x00002738
		public PackageInfo(string fileName, string tempFileDirectory)
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
			this._PackageFile = fileName;
			try
			{
				this._package = Package.LoadFromCab(this._PackageFile);
			}
			catch (Exception ex)
			{
				throw new ImagesException("Tools.ImgCommon!PackageInfo: Failed to load Package file '" + fileName + "' : " + ex.Message, ex);
			}
		}

		// Token: 0x06000026 RID: 38 RVA: 0x000045EC File Offset: 0x000027EC
		public void Dispose()
		{
			if (this._cleanUpTempDir)
			{
				FileUtils.DeleteTree(this._tempDirectoryPath);
				this._tempDirectoryPath = string.Empty;
			}
		}

		// Token: 0x06000027 RID: 39 RVA: 0x0000460C File Offset: 0x0000280C
		private void SetTempDirectoryPath(string tempFileDirectory)
		{
			this._tempDirectoryPath = tempFileDirectory;
			if (!Directory.Exists(this._tempDirectoryPath))
			{
				Directory.CreateDirectory(this._tempDirectoryPath);
			}
			this._tempDirectoryPath = FileUtils.GetShortPathName(this._tempDirectoryPath);
		}

		// Token: 0x06000028 RID: 40 RVA: 0x0000463F File Offset: 0x0000283F
		public string DisplayFileName()
		{
			return this.DisplayFileName(this._PackageFile);
		}

		// Token: 0x06000029 RID: 41 RVA: 0x00004650 File Offset: 0x00002850
		public string DisplayFileName(string packageFile)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("****************************************************************************");
			stringBuilder.AppendLine("****************************************************************************");
			stringBuilder.AppendLine("File: " + packageFile);
			stringBuilder.AppendLine("****************************************************************************");
			stringBuilder.AppendLine("****************************************************************************");
			stringBuilder.AppendLine();
			return stringBuilder.ToString();
		}

		// Token: 0x0600002A RID: 42 RVA: 0x000046B0 File Offset: 0x000028B0
		public string DisplayPackage(bool fSummary, bool fDisplayHeader)
		{
			return PackageInfo.DisplayPackage(this._package, fSummary, fDisplayHeader);
		}

		// Token: 0x0600002B RID: 43 RVA: 0x000046C0 File Offset: 0x000028C0
		public static string DisplayPackage(IPkgInfo package, bool fSummary, bool fDisplayHeader)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (!fSummary || (fSummary && fDisplayHeader))
			{
				stringBuilder.AppendLine();
				stringBuilder.AppendFormat("{0,-50:G}\t", "Package Name");
				stringBuilder.AppendFormat("{0,-15:G}\t", "Version");
				stringBuilder.AppendFormat("{0,-15:G}\t", "Canonical\\Diff");
				stringBuilder.AppendFormat("{0,-20:G}\t", "Build");
				stringBuilder.AppendFormat("{0,-10:G}\t", "File Count");
				stringBuilder.AppendLine();
				stringBuilder.AppendLine("__________________________________________________\t_______________\t_______________\t____________________\t__________");
			}
			stringBuilder.AppendFormat("{0,-50:G}\t", package.Name);
			stringBuilder.AppendFormat("{0,-15:G}\t", package.Version);
			stringBuilder.AppendFormat("{0,-15:G}\t", package.Type);
			stringBuilder.AppendFormat("{0,-20:G}\t", package.BuildString);
			stringBuilder.AppendFormat("{0,-10:G}\t", package.FileCount);
			stringBuilder.AppendLine();
			if (!fSummary)
			{
				stringBuilder.AppendLine("[Package Details]");
				stringBuilder.AppendLine();
				stringBuilder.AppendLine("\t[Package]");
				stringBuilder.AppendFormat("\t\tName: {0}", package.Name);
				stringBuilder.AppendLine();
				stringBuilder.AppendFormat("\t\tOwner: {0}", package.Owner);
				stringBuilder.AppendLine();
				stringBuilder.AppendFormat("\t\tComponent: {0}", package.Component);
				stringBuilder.AppendLine();
				if (!string.IsNullOrEmpty(package.SubComponent))
				{
					stringBuilder.AppendFormat("\t\tSubComponent: {0}", package.SubComponent);
					stringBuilder.AppendLine();
				}
				stringBuilder.AppendFormat("\t\tPartition: {0}", package.Partition);
				stringBuilder.AppendLine();
				stringBuilder.AppendFormat("\t\tVersion: ", new object[0]);
				stringBuilder.AppendLine();
				stringBuilder.AppendFormat("\t\t\tMajor: {0}", package.Version.Major);
				stringBuilder.AppendLine();
				stringBuilder.AppendFormat("\t\t\tMinor: {0}", package.Version.Minor);
				stringBuilder.AppendLine();
				stringBuilder.AppendFormat("\t\t\tQFE: {0}", package.Version.QFE);
				stringBuilder.AppendLine();
				stringBuilder.AppendFormat("\t\t\tBuild: {0}", package.Version.Build);
				stringBuilder.AppendLine();
				stringBuilder.AppendFormat("\t\tOwner Type: {0}", package.OwnerType);
				stringBuilder.AppendLine();
				stringBuilder.AppendFormat("\t\tRelease Type: {0}", package.ReleaseType);
				stringBuilder.AppendLine();
				stringBuilder.AppendFormat("\t\tBuild Type: {0}", package.BuildType);
				stringBuilder.AppendLine();
				stringBuilder.AppendFormat("\t\tCPU: {0}", package.CpuType);
				stringBuilder.AppendLine();
				if (!string.IsNullOrEmpty(package.Culture))
				{
					stringBuilder.AppendFormat("\t\tCulture: {0}", package.Culture);
					stringBuilder.AppendLine();
				}
				if (!string.IsNullOrEmpty(package.Resolution))
				{
					stringBuilder.AppendFormat("\t\tResolution: {0}", package.Resolution);
					stringBuilder.AppendLine();
				}
				stringBuilder.AppendFormat("\t\tPackage Type: {0}", package.Type);
				stringBuilder.AppendLine();
				stringBuilder.AppendFormat("\t\tBuild: {0}", package.BuildString);
				stringBuilder.AppendLine();
				stringBuilder.AppendFormat("\t\tGroup Key: {0}", package.GroupingKey);
				stringBuilder.AppendLine();
				stringBuilder.AppendFormat("\t\tFile Count: {0}", package.FileCount);
				stringBuilder.AppendLine();
				stringBuilder.AppendLine();
				stringBuilder.AppendLine();
				stringBuilder.AppendFormat("\t\t[File Summary for package {0}]", package.Name);
				stringBuilder.AppendLine();
				stringBuilder.AppendLine();
				stringBuilder.AppendFormat("\t\t{0,-50:G}\t", "Filename");
				stringBuilder.AppendFormat("{0,-15:G}\t", "Size (bytes)");
				stringBuilder.AppendFormat("{0,-25:G}\t", "Attributes");
				stringBuilder.AppendFormat("{0,-15:G}\t", "Type");
				stringBuilder.AppendFormat("{0,-35:G}\t", "Device Path");
				stringBuilder.AppendLine();
				stringBuilder.AppendLine("\t\t__________________________________________________\t_______________\t_________________________\t_______________\t___________________________________");
				foreach (IFileEntry fileEntry in package.Files)
				{
					string fileName = Path.GetFileName(fileEntry.DevicePath);
					string arg = fileEntry.DevicePath.Replace(fileName, "", StringComparison.OrdinalIgnoreCase);
					stringBuilder.AppendFormat("\t\t{0,-50:G}\t", fileName);
					stringBuilder.AppendFormat("{0,-15:G}\t", fileEntry.Size);
					stringBuilder.AppendFormat("{0,-25:G}\t", fileEntry.Attributes);
					stringBuilder.AppendFormat("{0,-15:G}\t", fileEntry.FileType);
					stringBuilder.AppendFormat("{0,-35:G}\t", arg);
					stringBuilder.AppendLine();
				}
				stringBuilder.AppendLine();
				stringBuilder.AppendLine("\t\t[Files]");
				foreach (IFileEntry fileEntry2 in package.Files)
				{
					stringBuilder.AppendLine("\t\t\t[File]");
					string fileName = Path.GetFileName(fileEntry2.DevicePath);
					string arg = fileEntry2.DevicePath.Replace(fileName, "", StringComparison.OrdinalIgnoreCase);
					stringBuilder.AppendFormat("\t\t\t\tName: {0}", fileName);
					stringBuilder.AppendLine();
					stringBuilder.AppendFormat("\t\t\t\tSize: " + PackageInfo._dwordFormatString + " bytes", fileEntry2.Size, fileEntry2.Size);
					stringBuilder.AppendLine();
					stringBuilder.AppendFormat("\t\t\t\tAttributes: {0}", fileEntry2.Attributes);
					stringBuilder.AppendLine();
					stringBuilder.AppendFormat("\t\t\t\tPath: {0}", arg);
					stringBuilder.AppendLine();
					stringBuilder.AppendFormat("\t\t\t\tPackage: {0}", package.Name);
					stringBuilder.AppendLine();
					stringBuilder.AppendFormat("\t\t\t\tFileType: {0}", fileEntry2.FileType);
					stringBuilder.AppendLine();
					stringBuilder.AppendFormat("\t\t\t\tSource Package: {0}", fileEntry2.SourcePackage);
					stringBuilder.AppendLine();
					stringBuilder.AppendLine();
					stringBuilder.AppendLine();
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x0600002C RID: 44 RVA: 0x00004334 File Offset: 0x00002534
		public string DisplayPackages(string packagesInfo)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("[Packages]");
			stringBuilder.AppendLine();
			stringBuilder.Append(packagesInfo);
			return stringBuilder.ToString();
		}

		// Token: 0x04000011 RID: 17
		private IPkgInfo _package;

		// Token: 0x04000012 RID: 18
		private IULogger _logger;

		// Token: 0x04000013 RID: 19
		private string _PackageFile;

		// Token: 0x04000014 RID: 20
		private static string _dwordFormatString = "{0} (0x{0:X})";

		// Token: 0x04000015 RID: 21
		private ASCIIEncoding _enc = new ASCIIEncoding();

		// Token: 0x04000016 RID: 22
		private string _tempDirectoryPath = string.Empty;

		// Token: 0x04000017 RID: 23
		private bool _cleanUpTempDir;
	}
}
