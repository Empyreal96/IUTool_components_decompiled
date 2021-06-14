using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.WindowsPhone.ImageUpdate.PkgCommon;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;
using Microsoft.WindowsPhone.Imaging;

namespace Microsoft.WindowsPhone.ImageUpdate.Tools.Images
{
	// Token: 0x02000008 RID: 8
	public class WPPartition : FullFlashUpdateImage.FullFlashUpdatePartition
	{
		// Token: 0x06000053 RID: 83 RVA: 0x00005984 File Offset: 0x00003B84
		public WPPartition(WPImage image, string wimPath, ImageStorageManager storageManager, IULogger logger)
		{
			this._storageManager = storageManager;
			this._logger = logger;
			this._image = image;
			base.Name = Path.GetFileNameWithoutExtension(wimPath);
			base.AttachDriveLetter = false;
			base.Bootable = true;
			base.FileSystem = "NTFS";
			base.Hidden = false;
			base.PartitionType = "None";
			base.PrimaryPartition = "None";
			base.ReadOnly = true;
			base.RequiredToFlash = false;
			base.UseAllSpace = false;
			base.ByteAlignment = 0U;
			base.SectorsInUse = 0U;
			base.TotalSectors = 0U;
			base.SectorAlignment = 0U;
			this._isWIM = true;
			this._sourceWIM = wimPath;
			this.InitializeWIM();
		}

		// Token: 0x06000054 RID: 84 RVA: 0x00005A6C File Offset: 0x00003C6C
		~WPPartition()
		{
		}

		// Token: 0x06000055 RID: 85 RVA: 0x00005A94 File Offset: 0x00003C94
		private void InitializeWIM()
		{
			this._wimMountPoint = Path.Combine(this._image.TempDirectoryPath, Path.GetFileNameWithoutExtension(this._sourceWIM) + ".mnt");
			this._wimPath = Path.Combine(this._image.TempDirectoryPath, Path.GetFileName(this._sourceWIM));
			File.Copy(this._sourceWIM, this._wimPath);
			Directory.CreateDirectory(this._wimMountPoint);
			this.MountWim(this._wimPath, this._wimMountPoint);
			this._path = this._wimMountPoint;
			FileInfo fileInfo = new FileInfo(this._sourceWIM);
			this._wimFileSize = fileInfo.Length;
			this._wimFileContentSize = WPPartition.GetDirectoryFileContentSize(this._path);
			this.LoadPackages();
		}

		// Token: 0x06000056 RID: 86 RVA: 0x00005B58 File Offset: 0x00003D58
		private static long GetDirectoryFileContentSize(string rootDir)
		{
			long num = 0L;
			DirectoryInfo directoryInfo = new DirectoryInfo(rootDir);
			try
			{
				foreach (FileInfo fileInfo in directoryInfo.EnumerateFiles("*.*", SearchOption.TopDirectoryOnly))
				{
					num += fileInfo.Length;
				}
			}
			catch (UnauthorizedAccessException)
			{
				return num;
			}
			foreach (DirectoryInfo directoryInfo2 in directoryInfo.GetDirectories())
			{
				if (string.Compare("SYSTEM VOLUME INFORMATION", Path.GetFileName(directoryInfo2.Name), true, CultureInfo.InvariantCulture) != 0)
				{
					num += WPPartition.GetDirectoryFileContentSize(directoryInfo2.FullName);
				}
			}
			return num;
		}

		// Token: 0x06000057 RID: 87 RVA: 0x00005C20 File Offset: 0x00003E20
		public WPPartition(WPImage image, InputPartition partition, ImageStorageManager storageManager, IULogger logger)
		{
			this._storageManager = storageManager;
			this._logger = logger;
			this._image = image;
			base.Name = partition.Name;
			base.AttachDriveLetter = partition.AttachDriveLetter;
			base.Bootable = partition.Bootable;
			base.FileSystem = partition.FileSystem;
			base.Hidden = partition.Hidden;
			base.PartitionType = partition.Type;
			base.PrimaryPartition = partition.PrimaryPartition;
			base.ReadOnly = partition.ReadOnly;
			base.RequiredToFlash = partition.RequiredToFlash;
			base.TotalSectors = partition.TotalSectors;
			base.UseAllSpace = partition.UseAllSpace;
			base.ByteAlignment = partition.ByteAlignment;
			this.Win32Accessible = image.Win32Accessible;
			try
			{
				ulong freeBytesOnVolume = this._storageManager.GetFreeBytesOnVolume(partition.Name);
				base.SectorsInUse = partition.TotalSectors - (uint)freeBytesOnVolume / image.Store.SectorSize;
				if (freeBytesOnVolume % (ulong)image.Store.SectorSize > 0UL)
				{
					uint sectorsInUse = base.SectorsInUse;
					base.SectorsInUse = sectorsInUse - 1U;
				}
			}
			catch
			{
				base.SectorsInUse = 0U;
			}
			base.SectorAlignment = 0U;
			this.Initialize();
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00005D94 File Offset: 0x00003F94
		public WPPartition(WPImage image, FullFlashUpdateImage.FullFlashUpdatePartition partition, ImageStorageManager storageManager, IULogger logger)
		{
			this._storageManager = storageManager;
			this._logger = logger;
			this._image = image;
			base.Name = partition.Name;
			base.AttachDriveLetter = partition.AttachDriveLetter;
			base.Bootable = partition.Bootable;
			base.FileSystem = partition.FileSystem;
			base.Hidden = partition.Hidden;
			base.PartitionType = partition.PartitionType;
			base.PrimaryPartition = partition.PrimaryPartition;
			base.ReadOnly = partition.ReadOnly;
			base.RequiredToFlash = partition.RequiredToFlash;
			base.SectorsInUse = partition.SectorsInUse;
			base.TotalSectors = partition.TotalSectors;
			base.UseAllSpace = partition.UseAllSpace;
			base.ByteAlignment = partition.ByteAlignment;
			base.SectorAlignment = partition.SectorAlignment;
			this.Win32Accessible = image.Win32Accessible;
			this.Initialize();
		}

		// Token: 0x06000059 RID: 89 RVA: 0x00005EB0 File Offset: 0x000040B0
		public WPPartition(WPImage image, ImagePartition partition, IULogger logger)
		{
			this._storageManager = null;
			this._logger = logger;
			this._image = image;
			this._path = partition.Root;
			this._wimFileContentSize = WPPartition.GetDirectoryFileContentSize(this._path);
			if (partition.MountedDriveInfo != null)
			{
				base.Name = partition.MountedDriveInfo.VolumeLabel;
				base.FileSystem = partition.MountedDriveInfo.DriveFormat;
				base.PartitionType = partition.MountedDriveInfo.DriveType.ToString();
				base.SectorsInUse = (uint)(partition.MountedDriveInfo.TotalSize - partition.MountedDriveInfo.TotalFreeSpace);
				base.TotalSectors = (uint)partition.MountedDriveInfo.TotalSize;
			}
			else
			{
				base.Name = partition.Name;
				base.SectorsInUse = (uint)this._wimFileContentSize;
				base.FileSystem = "NTFS";
				base.PartitionType = "WIM";
			}
			base.AttachDriveLetter = false;
			base.Bootable = true;
			base.Hidden = false;
			base.PrimaryPartition = "None";
			base.ReadOnly = true;
			base.RequiredToFlash = false;
			base.UseAllSpace = false;
			base.ByteAlignment = 0U;
			base.SectorAlignment = 0U;
			this._isWIM = true;
			this._sourceWIM = this._image.MCImage.ImagePath;
		}

		// Token: 0x0600005A RID: 90 RVA: 0x00006038 File Offset: 0x00004238
		private void Initialize()
		{
			if (this._storageManager.MainOSStorage.StoreId.StoreType == ImageConstants.PartitionTypeMbr)
			{
				this._mbrPartitionType = this._storageManager.MainOSStorage.GetPartitionTypeMbr(base.Name);
			}
			else
			{
				this._gptPartitionType = this._storageManager.MainOSStorage.GetPartitionTypeGpt(base.Name);
			}
			try
			{
				this._path = this._storageManager.GetPartitionPath(base.Name);
				if (this.Win32Accessible)
				{
					this.MakeWin32Accessible();
				}
			}
			catch (Exception ex)
			{
				throw new ImagesException("Failed call to GetPartitionPath for partition '" + base.Name + "' with error: " + ex.Message, ex.InnerException);
			}
			this.LoadPackages();
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00006104 File Offset: 0x00004304
		public void Dispose()
		{
			if (this.IsWim)
			{
				this.DismountWim(this._wimPath, this._wimMountPoint);
			}
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x0600005C RID: 92 RVA: 0x00006124 File Offset: 0x00004324
		public string MountPoint
		{
			get
			{
				if (string.IsNullOrEmpty(this._mountPoint))
				{
					string text = Path.Combine(this._image.TempDirectoryPath, base.Name);
					if (Directory.Exists(text))
					{
						Directory.Delete(text);
					}
					Directory.CreateDirectory(text);
					this._mountPoint = text + "\\";
				}
				return this._mountPoint;
			}
		}

		// Token: 0x0600005D RID: 93 RVA: 0x00006181 File Offset: 0x00004381
		private void CheckWin32Accessible()
		{
			this._win32Accessible = !this.PartitionPath.StartsWith("\\\\.\\", StringComparison.OrdinalIgnoreCase);
		}

		// Token: 0x0600005E RID: 94 RVA: 0x000061A0 File Offset: 0x000043A0
		private void MakeWin32Accessible()
		{
			if (this._win32Accessible || this._attemptedToMakeWin32Accessible)
			{
				return;
			}
			this.CheckWin32Accessible();
			if (!this._win32Accessible)
			{
				if (base.Name.Equals(ImageConstants.MAINOS_PARTITION_NAME, StringComparison.OrdinalIgnoreCase))
				{
					this._path = this._image.MainOSPath;
				}
				else
				{
					this._path = Path.Combine(this._image.MainOSPath, base.Name);
				}
			}
			this._attemptedToMakeWin32Accessible = true;
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x0600005F RID: 95 RVA: 0x00006215 File Offset: 0x00004415
		public bool IsWim
		{
			get
			{
				return this._isWIM;
			}
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000060 RID: 96 RVA: 0x0000621D File Offset: 0x0000441D
		public string WimFile
		{
			get
			{
				return this._sourceWIM;
			}
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000061 RID: 97 RVA: 0x00006225 File Offset: 0x00004425
		public string PartitionTypeLabel
		{
			get
			{
				if (!this.IsWim)
				{
					return "Partition";
				}
				return "WIM";
			}
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000062 RID: 98 RVA: 0x0000623A File Offset: 0x0000443A
		public long WimFileSize
		{
			get
			{
				return this._wimFileSize;
			}
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000063 RID: 99 RVA: 0x00006242 File Offset: 0x00004442
		public long WimFileContentSize
		{
			get
			{
				return this._wimFileContentSize;
			}
		}

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000064 RID: 100 RVA: 0x0000624A File Offset: 0x0000444A
		// (set) Token: 0x06000065 RID: 101 RVA: 0x00006252 File Offset: 0x00004452
		public bool IsBinaryPartition
		{
			get
			{
				return this._isBinaryPartition;
			}
			set
			{
				this._isBinaryPartition = value;
			}
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000066 RID: 102 RVA: 0x0000625B File Offset: 0x0000445B
		public bool InvalidPartition
		{
			get
			{
				return this._bInvalidPartition;
			}
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000067 RID: 103 RVA: 0x00006263 File Offset: 0x00004463
		public string PartitionPath
		{
			get
			{
				return this._path;
			}
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000068 RID: 104 RVA: 0x0000626B File Offset: 0x0000446B
		public int PackageCount
		{
			get
			{
				return this._packages.Count;
			}
		}

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000069 RID: 105 RVA: 0x00006278 File Offset: 0x00004478
		[CLSCompliant(false)]
		public List<IPkgInfo> Packages
		{
			get
			{
				return this._packages;
			}
		}

		// Token: 0x0600006A RID: 106 RVA: 0x00006280 File Offset: 0x00004480
		public bool HasRegistryHive(SystemRegistryHiveFiles hiveType)
		{
			return File.Exists(this.GetRegistryHivePath(hiveType));
		}

		// Token: 0x0600006B RID: 107 RVA: 0x00006290 File Offset: 0x00004490
		public string GetRegistryHiveDevicePath(SystemRegistryHiveFiles hiveType)
		{
			bool isUefiBoot = false;
			string registryHiveFilePath;
			if (base.Name.ToUpper(CultureInfo.InvariantCulture) == "EFIESP")
			{
				if (this._storageManager.MainOSStorage.StoreId.StoreType == ImageConstants.PartitionTypeGpt)
				{
					isUefiBoot = true;
				}
				registryHiveFilePath = DevicePaths.GetRegistryHiveFilePath(hiveType, isUefiBoot);
			}
			else
			{
				registryHiveFilePath = DevicePaths.GetRegistryHiveFilePath(hiveType);
			}
			return registryHiveFilePath;
		}

		// Token: 0x0600006C RID: 108 RVA: 0x000062EE File Offset: 0x000044EE
		public string GetRegistryHivePath(SystemRegistryHiveFiles hiveType)
		{
			return Path.Combine(this.PartitionPath, this.GetRegistryHiveDevicePath(hiveType));
		}

		// Token: 0x0600006D RID: 109 RVA: 0x00006304 File Offset: 0x00004504
		private void LoadPackages()
		{
			char[] trimChars = new char[]
			{
				'\\'
			};
			string path = Path.Combine(this.PartitionPath, PkgConstants.c_strDsmDeviceFolder.TrimStart(trimChars));
			List<string> list = new List<string>();
			if (Directory.Exists(path))
			{
				list.AddRange(Directory.EnumerateFiles(path, PkgConstants.c_strDsmSearchPattern));
			}
			string path2 = Path.Combine(this.PartitionPath, PkgConstants.c_strMumDeviceFolder.TrimStart(trimChars));
			if (Directory.Exists(path2))
			{
				list.AddRange(Directory.EnumerateFiles(path2, PkgConstants.c_strMumSearchPattern));
			}
			foreach (string text in list)
			{
				try
				{
					IPkgInfo item = Package.LoadInstalledPackage(text, this._path);
					this._packages.Add(item);
				}
				catch (Exception ex)
				{
					this._logger.LogError("Tools.ImgCommon!LoadPackages: Failed to load package dsm\\mum file '{0}' in Partition '{1}'  with error: {2} ", new object[]
					{
						text,
						base.Name,
						ex.Message
					});
				}
			}
		}

		// Token: 0x0600006E RID: 110 RVA: 0x0000641C File Offset: 0x0000461C
		public void CopyAsBinary(string destinationFile)
		{
			FileStream fileStream = new FileStream(destinationFile, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
			this._storageManager.LockAndDismountVolume(base.Name);
			SafeVolumeHandle safeVolumeHandle = new SafeVolumeHandle(this._storageManager.MainOSStorage, base.Name);
			VirtualMemoryPtr virtualMemoryPtr = new VirtualMemoryPtr(1048576U);
			ulong num = this._storageManager.GetPartitionSize(base.Name) * (ulong)this._image.Store.SectorSize;
			ulong num2 = 0UL;
			try
			{
				while (num2 < num)
				{
					uint num3 = 0U;
					uint num4 = 0U;
					uint bytesToRead = (uint)Math.Min(num - num2, 1048576UL);
					Win32Exports.ReadFile(safeVolumeHandle.VolumeHandle, virtualMemoryPtr, bytesToRead, out num3);
					Win32Exports.WriteFile(fileStream.SafeFileHandle, virtualMemoryPtr, num3, out num4);
					num2 += (ulong)num3;
				}
			}
			catch (Exception ex)
			{
				throw new ImagesException("Tools.ImgCommon!CopyAsBinary: Failed while writing binary partition " + base.Name + ": " + ex.Message, ex.InnerException);
			}
			finally
			{
				virtualMemoryPtr.Close();
				safeVolumeHandle.Close();
				fileStream.Close();
				safeVolumeHandle = null;
				fileStream = null;
			}
		}

		// Token: 0x0600006F RID: 111 RVA: 0x00006540 File Offset: 0x00004740
		private static bool FAILED(int hr)
		{
			return hr < 0;
		}

		// Token: 0x06000070 RID: 112 RVA: 0x00006546 File Offset: 0x00004746
		private bool MountWim(string wimPath, string mountPoint)
		{
			return !WPPartition.FAILED(WPPartition.MountWim(wimPath, mountPoint, Path.GetDirectoryName(wimPath)));
		}

		// Token: 0x06000071 RID: 113
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, EntryPoint = "IU_MountWim")]
		private static extern int MountWim([MarshalAs(UnmanagedType.LPWStr)] string WimPath, [MarshalAs(UnmanagedType.LPWStr)] string MountPath, [MarshalAs(UnmanagedType.LPWStr)] string TemporaryPath);

		// Token: 0x06000072 RID: 114 RVA: 0x0000655F File Offset: 0x0000475F
		private bool DismountWim(string wimPath, string mountPoint)
		{
			return !WPPartition.FAILED(WPPartition.DismountWim(wimPath, mountPoint, 0));
		}

		// Token: 0x06000073 RID: 115
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, EntryPoint = "IU_DismountWim")]
		private static extern int DismountWim([MarshalAs(UnmanagedType.LPWStr)] string WimPath, [MarshalAs(UnmanagedType.LPWStr)] string MountPath, int CommitMode);

		// Token: 0x04000029 RID: 41
		private ImageStorageManager _storageManager;

		// Token: 0x0400002A RID: 42
		private IULogger _logger;

		// Token: 0x0400002B RID: 43
		private string _path = string.Empty;

		// Token: 0x0400002C RID: 44
		private List<IPkgInfo> _packages = new List<IPkgInfo>();

		// Token: 0x0400002D RID: 45
		private byte _mbrPartitionType;

		// Token: 0x0400002E RID: 46
		private Guid _gptPartitionType;

		// Token: 0x0400002F RID: 47
		private bool _isBinaryPartition;

		// Token: 0x04000030 RID: 48
		private WPImage _image;

		// Token: 0x04000031 RID: 49
		private bool _bInvalidPartition;

		// Token: 0x04000032 RID: 50
		private bool _isWIM;

		// Token: 0x04000033 RID: 51
		private string _sourceWIM = string.Empty;

		// Token: 0x04000034 RID: 52
		private long _wimFileSize;

		// Token: 0x04000035 RID: 53
		private long _wimFileContentSize;

		// Token: 0x04000036 RID: 54
		private string _wimMountPoint = string.Empty;

		// Token: 0x04000037 RID: 55
		private string _wimPath = string.Empty;

		// Token: 0x04000038 RID: 56
		private bool _win32Accessible;

		// Token: 0x04000039 RID: 57
		private bool _attemptedToMakeWin32Accessible;

		// Token: 0x0400003A RID: 58
		private string _mountPoint;

		// Token: 0x0400003B RID: 59
		public bool Win32Accessible;

		// Token: 0x0400003C RID: 60
		private const int S_OK = 0;

		// Token: 0x0400003D RID: 61
		private const int WimNoCommit = 0;
	}
}
